# CS134 Final Project — Phantom Coliseum

A third-person basketball mini-game made in Unity: shoot hoops to reach the win score while a ghost chases you. Hit the ghost with the ball to stun it and create space.

## Gameplay

- **Goal**: Reach **10 points** to win.
- **Lives**: You start with **3 lives**. Touching the ghost costs 1 life. At 0 lives: **Game Over**.
- **Enemy**:
  - Chases the player.
  - On contact with the player: removes a life and stuns itself for **3 seconds**.
  - If the ball hits the enemy: enemy is stunned for **3 seconds**.

## Controls

- **WASD**: Move
- **Mouse**: Look / rotate player
- **E**: Pick up / drop ball
- **Left click (hold)**: Charge shot
- **Left click (release)**: Shoot
- **R**: Restart scene

## Scoring

Per shot (deduped so you can’t farm points from repeated bounces):

- **Backboard hit**: +1
- **Rim hit**: +3
- **Ball through hoop**: +10

Hoop makes are validated so “sideways through the net” shots don’t count (the ball must be moving **mostly downward** through the hoop trigger).

## UI Flow

- **Start Panel**: Shows rules and a Start button (game paused).
- **In-game HUD**: Score + lives (hearts).
- **Win Panel**: Shown when score reaches 10 (game paused).
- **Game Over Panel**: Shown when lives reach 0 (game paused).
- **Play Again**: Reloads the scene from the beginning.

UI uses **TextMeshPro** (`TMP_Text`) in scripts.

## Audio

The project uses an `AudioManager` with:

- **Menu music** (start panel)
- **Gameplay music** (during play)
- **SFX**: ball spawn/reset, backboard hit, rim hit, win sting, lose sting

After win/lose stings play, menu music resumes automatically (uses realtime waiting, so it works while the game is paused).

## Visual Effects

The basketball has a non-looping particle system driven by `BallEffects`:

- **Reset/spawn**: green
- **Backboard hit**: blue
- **Rim hit**: red

## Key Scripts (Assets/Game/Scripts)

- `PlayerController.cs`: Third-person mouse look + WASD Rigidbody movement
- `BallPickup.cs`: Pickup/drop and charged shooting (left click)
- `BallScoreTracker.cs`: Detects rim/backboard/hoop interactions and awards points
- `BallReset.cs`: Resets the ball if it goes too far and triggers spawn VFX/SFX
- `BallEffects.cs`: Particle color + replay control for the ball
- `EnemyAI.cs`: Ghost chasing, contact damage + stun logic, and animation parameters
- `PlayerHealth.cs`: 3-life system, triggers game over
- `GameManager.cs`: Score/lives UI, start/win/lose menus, pause/restart logic
- `AudioManager.cs`: Music + SFX routing
- `RimColliderBuilder.cs`: Builds a ring of capsule colliders for the rim and tags segments
- `HeartLivesUI.cs`: Hearts UI (Image-based or TMP text fallback)

## Unity Setup Notes

### Tags used

Create these under **Edit → Project Settings → Tags and Layers**:

- `Basketball`
- `Backboard`
- `Rim`
- `Hoop`
- `Player`

### Hoop trigger

Create a trigger collider slightly **below** the rim, tag it `Hoop`. This is what counts makes.

### Rim collider

The rim is represented by multiple capsule colliders in a ring (built by `RimColliderBuilder`) instead of a MeshCollider for more reliable physics.

### Scene restart

For `RestartScene()` to work:

- Add the active scene in **File → Build Settings → Add Open Scenes**.

## How to Run

1. Open the main scene (e.g. `DemoScene2`) in Unity.
2. Press Play.
3. Click **Start** on the start panel.

or go to https://golublue.itch.io/phantom-coliseum

