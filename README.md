# Wheel Spin Case Study

A zone-by-zone wheel game built on risk versus reward. The player spins the wheel in each zone and accumulates rewards; if a bomb comes up, they either revive or lose everything. **Cash Out** banks the rewards at any time.

- **Unity:** 2021.3.45f2
- **Scene:** `Assets/Scenes/WheelCaseStudy.unity`
- **Dependencies:** DOTween, TextMeshPro, UGUI, [UIParticle](https://github.com/mob-sakai/ParticleEffectForUGUI)

---

## Game Flow

```
StartRun ─► Zone 1 ─► Spin ─┬─ Reward ─► add to Ledger ─► next Zone ─► ... ─► last Zone ─► RunCompleted
                            │
                            └─ Bomb ───► RunFailed ─┬─ Revive ──► same Zone, wheel without bomb
                                                    └─ Give Up ─► rewards cleared, new run
            Cash Out (active run, non-empty ledger) ─► RunCashedOut
```

| Tier   | Rule (default)       | Bomb |
|--------|----------------------|------|
| Gold   | `zone % 30 == 0`     | No   |
| Silver | `zone % 5 == 0`      | No   |
| Bronze | All other zones      | Yes  |

---

## Architecture: C# First, Thin Unity Shell

Game rules live in a pure C# layer with no Unity dependency. This is not a convention — it is **enforced by the compiler**:

```
Assets/Scripts/
  Core/        CaseStudy.WheelSpin.Core.asmdef   noEngineReferences: true  → NO UnityEngine
  (root)       CaseStudy.WheelSpin.asmdef        Core + UnityEngine + DOTween + TMP + UIParticle
  Editor/      CaseStudy.WheelSpin.Editor.asmdef Core + Runtime (inspector tooling)
```

Adding `using UnityEngine;` to Core fails with **`CS0246`**. Dependencies point one way only: Runtime → Core.

### Layers

| Layer | Responsibility | Example types |
|-------|----------------|---------------|
| **Core** (pure C#) | Rules, probability, state machine, reward ledger | `WheelSession`, `WheelSpinner`, `RandomWeightedResultCalculator`, `PenaltyOdds`, `WheelTierRuleProvider`, `RewardLedger`, `AnimationGate`, `AmountSplit` |
| **Config / Zone** (ScriptableObject) | Converts designer data into Core models | `WheelConfigAsset`, `ZoneAsset.ToZone`, `ScriptableObjectZoneProvider : IZoneProvider` |
| **Presentation** (plain classes, use Unity APIs) | Turns outcomes into animation and sequences them | `WheelPresenter`, `SpinTimeline`, `WheelPresenterSet`, `*Presenter`, `WheelSpinAnimator` |
| **View** (MonoBehaviour) | Serialized references and Unity calls only | `WheelView`, `RewardHolderView`, `RevivePopup`, views backed by `ViewPool<T>` |
| **Composition root** | Builds the object graph in one place | `WheelController` (MonoBehaviour) → `WheelGame.TryCreate` |

### Key Decisions

- **One composition root, no singletons.** `WheelGame.TryCreate` wires the whole graph by hand; `WheelController` only holds config + seed and routes player actions.
- **Logic first, presentation second.** `WheelSession.TrySpin` resolves the outcome **immediately** and raises events. `SpinOutcomeBuffer` collapses those events into a single `SpinOutcome` struct, which the presenter then plays back. The result never depends on animation.
- **Busy lock lives in presentation.** `WheelPresenter.IsBusy` blocks actions while animating; Core knows nothing about it.
- **Fresh-instance contract.** `IZoneProvider.GetZone` returns a new `Zone` on every call (no cache), so mutable `WheelSlice` fields can never leak into a shared object. Revive reuses the same method with `penaltyDisabled: true`.
- **Validation at startup.** `ScriptableObjectZoneProvider.TryValidate` catches an empty zone set, null entries and zones whose total weight is 0 before the game starts.
- **Configuration in assets.** Tier → color/sprite mapping lives in `WheelTierViewDatabase`; animation tuning lives in the `WheelSpinSettings` / `RewardFlightSettings` / `PenaltyEffectSettings` ScriptableObjects. A new tier visual is an asset change, not a code change.
- **Deterministic seed.** Enabling `_useFixedSeed` on `WheelController` feeds `System.Random(seed)`, reproducing the same spin sequence.

### What C# First Buys Us

- **Testable without opening Unity:** every Core type can be constructed with `new`. Swapping `IWheelSpinResultCalculator` for a fake makes scenarios like "the 3rd spin is a bomb" deterministic.
- **Fast iteration:** rules can be verified in EditMode without scenes, prefabs or Play Mode.
- **Durable boundaries:** layer violations are not left to code review; the build breaks.
- **Replaceable presentation:** DOTween, UGUI or effects can change without touching game rules. The same Core could run in a server-side validator or a simulation.
- **Easier debugging:** because outcome and animation are separate, "was the wrong reward granted, or just shown wrong?" is answered at a glance.
- **Safe odds tuning:** the probability math (`PenaltyOdds`) is a pure function, so the editor and runtime share the exact same code.

---

## Algorithms

### 1. Weighted Random Selection — `RandomWeightedResultCalculator`
- Sum the weights, draw `roll ∈ [0, total)`, pick the first slice whose cumulative sum exceeds `roll`.
- Each slice's probability is `weight / total`.
- **Complexity:** O(n) time, O(1) memory, no allocation. With n = 8, an alias method or binary search is unnecessary.
- `total <= 0` → `-1`; `WheelSpinner` falls back to index `0` (already prevented by startup validation).

### 2. Bomb Chance → Weight — `PenaltyOdds`
Designers enter a **percentage**, not a weight (global, or per-zone override). With `S` as the sum of the other slice weights and `p` as the target chance:

```
w = round(S · p / (1 − p))        ⇒   p = w / (S + w)
```

- `p` is clamped to `[0, 0.9]` (as `p → 1`, `w → ∞`).
- If `p > 0`, then `w ≥ 1`; rounding never silently removes the bomb.
- `ChanceFor` is the inverse; the editor displays the actual (rounded) probability.

### 3. Tier Rule — `WheelTierRuleProvider`
- Modular arithmetic, O(1). Gold is checked before Silver, so overlaps (30 is a multiple of 5) resolve to Gold.
- Bombs only appear in Bronze zones: when `HasPenalty(index)` is false, `ZoneAsset.ToZone` puts a reward slice in the bomb slot.

### 4. Reward Ledger — `RewardLedger`
- `List<Entry>` + `Dictionary<string,int>` (itemId → index).
- **Add:** O(1) average; duplicate items merge while **first-earned order is preserved** (stable board ordering).
- `Entry` is a `readonly struct` → no heap allocation per entry.

### 5. Splitting an Amount Across Icons — `AmountSplit`
- `share = amount / count`; the remainder (`amount % count`) goes to the last icon.
- Parts always sum to `amount` (no loss from integer division).
- Writes into a preallocated `int[]` buffer; O(count), no allocation.

### 6. Parallel Animation Sync — `AnimationGate` (Core)
The reward flight, zone strip scroll and wheel change run in parallel and finish at different times.
- `Begin(onComplete)` → each `Track()` increments a counter and returns a single **preallocated** callback → `Seal()` closes registration.
- Completion fires exactly once, only when `sealed && pending == 0`. Parts that finish synchronously before sealing cannot trigger early completion.
- `Cancel()` drops the pending callback, so half-finished animations during revive/reset cannot break the flow.
- Being Unity-agnostic, sequencing logic stays in Core while tweens stay in Presentation.

### 7. Stroboscopic Aliasing Guard — `SpinAliasing`
If the wheel rotates more than **half a slice** per frame, the eye perceives it spinning backwards (wagon-wheel effect, Nyquist limit).

```
maxSafe°/s   = (360 / sliceCount) · 0.5 · fps · 0.9
peak°/s      = easePeakFactor · totalDegrees / duration
safeDuration = max(duration, easePeakFactor · totalDegrees / maxSafe°/s)
```

- `easePeakFactor`: ratio of the easing curve's peak velocity to its average velocity (Linear 1, Quad 2, Cubic 3, Quart 4, Expo ≈ 6.93).
- Duration is only ever extended, never shortened.

### 8. Three-Phase Wheel Spin — `WheelSpinAnimator`
- **Target angle:** `repeat(indicatorAngle ± step · sliceIndex, 360)`.
- **Distance:** direction-aware delta from the current angle to the target + a random number of full turns in `MinTurns..MaxTurns`.
- **Phases:** Windup (pull back against the spin direction) → Spin (to target + overshoot) → Settle (ease back onto the target).
- Windup and overshoot are capped at `180° / sliceCount` (half a slice), so the wheel never appears to land on a neighbor.
- A single `travelled` scalar is tweened; getter/setter delegates are created once in the constructor.

### 9. Reward Flight — `RewardFlightSpawner` / `RewardFlightIconView`
- **Placement:** icons are spread across `360° / count` angular slots plus jitter. Radius `R · √U(inner, 1)` gives an **area-uniform distribution** within the ring (no clustering at the center).
- **Path:** quadratic Bézier `B(t) = (1−t)²·P₀ + 2(1−t)t·C + t²·P₁`; control point `C` is offset from the midpoint perpendicular to the path by `arcHeight · jitter`, so each icon traces a slightly different arc.
- **Timing:** spawn at `i · spawnInterval`, fly at `flyStart + i · flightInterval`. On arrival, the counter increases by that icon's `AmountSplit` share (stepped count-up); on spawn, the wheel's number decreases by the same share.

### 10. Board Focus Scroll — `RewardScrollFocus`
- The target slot's viewport-local top/bottom edges are compared with a **visible band** defined by two anchors.
- Inside the band → delta = 0, the board does not move. Outside → scroll by the overflow, clamped to `[0, contentHeight − viewportHeight]`.
- `GetWorldCorners` writes into a preallocated `Vector3[4]` buffer.

### 11. View Pool — `ViewPool<T>`
- Compacted list: `[0, activeCount)` is active, the rest is idle. `Acquire` is O(1) amortized and only calls `Instantiate` when the pool is exhausted.
- `Prewarm` creates instances at startup; `DestroyCreated` destroys only instances the pool itself created (pre-existing scene objects are untouched).
- The reward board, zone strip, flight icons and cash-out popup all share this pool.

---

## Allocation Discipline

Spin and animation paths follow production-mode rules:

- No LINQ, closure capture or `params`.
- Tween callbacks (`TweenCallback`, `DOGetter/DOSetter`, `Action`) are bound to delegates once in constructors; no new delegates per spin.
- `SpinResult`, `SpinOutcome`, `RewardLedger.Entry` → structs.
- Buffers (`_shareArray`, `_iconArray`, `_cornerBuffer`) grow only when capacity is insufficient.
- UI elements are pooled via `ViewPool<T>`.
- Known exception: `ZoneAsset.ToZone` allocates a new `WheelSlice[]` per zone (required by the fresh-instance contract; once per zone transition, not per frame).

---

## Folder Layout

```
Assets/Scripts/
  Core/            Pure C# domain (Animation, Reward, Session, Wheel, Zone)
  App/             WheelGame (graph construction), DisplaySetup, screenshot tool
  Config/          WheelConfigAsset, ItemAsset, ItemDatabaseAsset
  Zone/            ZoneAsset, ZoneSetAsset, ScriptableObjectZoneProvider, zone views
  Presentation/    Presenters, SpinTimeline, WheelSpinAnimator, SpinAliasing, settings SOs
  Wheel/ Reward/ Popup/ Button/ Audio/   MonoBehaviour views
  Utils/           ViewPool<T>, ViewFormat
  Editor/          Zone / ZoneSet custom inspectors
  WheelController.cs   Composition root
```

---

## Running

1. Open the project with Unity **2021.3.45f2**.
2. Open `Assets/Scenes/WheelCaseStudy.unity` and press Play.
3. Content settings: `Assets/ScriptableObjects/WheelConfig.asset` (tier intervals, global bomb chance, zone set, item database).
4. For a reproducible spin sequence, enable **Use Fixed Seed** on the `WheelController` in the scene.

## Known Gaps

- There is no EditMode test assembly yet. Since Core is Unity-agnostic, the first targets are ready: `RandomWeightedResultCalculator` distribution, `PenaltyOdds` round-trip, `WheelSession` revive/cash-out flow, `AnimationGate` sequencing.
- `WheelSpinAnimator.TravelDegrees` and `RewardFlightSpawner` use `UnityEngine.Random` for visual variety; the seed makes only the game outcome (Core) deterministic, not the animation variation.
