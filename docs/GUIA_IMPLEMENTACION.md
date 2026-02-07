# Guía de implementación (Unity 5.6 / C# 4.0)

Esta guía explica **cómo llevar el prototipo a una escena jugable** usando los scripts del repositorio.

## 1) Preparación del proyecto

1. Crea un proyecto en **Unity 5.6** (3D).
2. Copia la carpeta `Assets/Scripts` dentro de tu proyecto.
3. Crea estas capas (opcional pero recomendado):
   - `Player`
   - `Enemy`
   - `Hitbox`
   - `Hurtbox`
4. En `Edit > Project Settings > Time`, deja `Fixed Timestep` en valor por defecto (0.02) al inicio.

## 2) Input Manager (sistema clásico)

En `Edit > Project Settings > Input`, verifica estos ejes/botones:

- `Horizontal`
- `Vertical`
- `Jump`
- `Fire1` (ataque ligero)
- `Fire2` (ataque pesado)
- `Fire3` (dash)
- `Mouse X`

Sugerencia de mapeo teclado:
- Movimiento: WASD
- Jump: Space
- Fire1: J / Mouse0
- Fire2: K / Mouse1
- Fire3: Left Ctrl / Left Shift
- Lock-on: Tab (hardcoded en `PlayerCombat`)

## 3) Escena base de arena

1. Crea un plano para suelo y algunos muros para cerrar la arena.
2. Añade iluminación básica + postproceso simple (si tienes paquete compatible).
3. Crea un `Empty` llamado `GameSystems` para agrupar managers.

## 4) Configuración del Player

### 4.1 GameObject principal

1. Crea un objeto `Player` con:
   - `CharacterController`
   - `PlayerController`
   - `PlayerCombat`
   - `Hurtbox`
2. Pon tag `Player`.
3. Ajusta `CharacterController` (ejemplo inicial):
   - Height: `1.8`
   - Radius: `0.35`
   - Center Y: `0.9`

### 4.2 Arma / Hitbox

1. Crea un hijo `WeaponHitbox` (por ejemplo en mano/espada).
2. Añade `BoxCollider` con `Is Trigger` activado.
3. Añade script `Hitbox`.
4. Arrastra `WeaponHitbox` a `PlayerCombat.weaponHitbox`.

### 4.3 DamageData de ataques

En `PlayerCombat`, configura:
- `lightAttack`
- `heavyAttack`
- `launcherAttack`
- `slamAttack`

Valores recomendados para arrancar:
- Light: `Amount 8`, `HitStun 0.15`, `Knockback 1.5`, `StyleValue 20`, `Reaction Light`
- Heavy: `Amount 16`, `HitStun 0.3`, `Knockback 3`, `StyleValue 35`, `Reaction Heavy`
- Launcher: `Amount 12`, `HitStun 0.45`, `Knockback 2`, `StyleValue 45`, `Reaction Launch`
- Slam (aéreo): `Amount 18`, `HitStun 0.35`, `Knockback 4`, `StyleValue 40`, `Reaction Slam`, `IsAirAttack true`

## 5) Animator del Player (control clásico)

Aunque el código funciona sin Animator complejo, para sensación DMC configura:

1. `Animator` en `Player`.
2. Estados base:
   - `Locomotion`
   - `Jump`
   - `Fall`
   - `Light1/Light2/Light3`
   - `Heavy`
   - `Launcher`
   - `AirSlash`
   - `Slam`
   - `Dodge`
3. Parameters sugeridos:
   - `Speed` (float)
   - `Grounded` (bool)
   - `VerticalSpeed` (float)
   - `AttackIndex` (int)
   - `DoHeavy` (trigger)
   - `DoLauncher` (trigger)
   - `DoSlam` (trigger)

### Eventos de animación (clave)

En cada animación de ataque:
- Evento al iniciar ventana de golpe: activar hitbox (`Hitbox.Activate` vía wrapper o Animation Event method).
- Evento al finalizar ventana: llamar `PlayerCombat.EndAttackWindow()`.

> Recomendación: crea un `PlayerAnimationEvents.cs` como puente para eventos de animación y evita acoplar lógica en clips.

## 6) Configuración de enemigos

### 6.1 Prefab enemigo base

Crea `Enemy_Base` con:
- Collider + (opcional) Rigidbody kinemático
- `Hurtbox`
- `EnemyAI`
- `EnemyCombat`
- hijo `EnemyHitbox` con `Hitbox` + Trigger Collider

En `EnemyCombat`:
- Asigna `hitbox` = `EnemyHitbox`
- Configura `meleeDamage` (ejemplo `Amount 10`, `HitStun 0.2`, `Reaction Light`)

### 6.2 Tipos de enemigo

Duplica el prefab y ajusta parámetros:
1. **Melee rápido**: `moveSpeed` alto, `attackCooldown` bajo, daño medio.
2. **Tanque lento**: `moveSpeed` bajo, `attackCooldown` medio, daño/vida altos.
3. **Volador**: más alto en Y, animaciones aéreas, menor `attackDistance` vertical.

## 7) Spawner por oleadas

1. Crea objeto `WaveSpawner`.
2. Añade `EnemyWaveSpawner`.
3. Define `spawnPoints` (empties repartidos por arena).
4. En `waves`, añade prefabs y cantidad por oleada.
5. Ajusta `waveDelay` (ej. 2-4 s).

## 8) Cámara tercera persona

1. En cámara principal añade `ThirdPersonCamera`.
2. Asigna:
   - `target` = Player
   - `playerCombat` = componente del player
3. Ajusta:
   - `offset`: `(0, 3, -6)` base
   - `followSpeed`: 8-12
   - `rotationSpeed`: 100-160
   - `lockOnZoom`: `-4.5` aprox

## 9) Sistema de estilo + HUD

### 9.1 StyleManager

1. Crea objeto `StyleManager` en `GameSystems`.
2. Añade script `StyleManager`.
3. Ajusta thresholds y penalizaciones según ritmo deseado.

### 9.2 HUD

1. Crea `Canvas` (Screen Space Overlay).
2. Añade 3 `Text`:
   - HP
   - Style
   - Combo
3. Crea objeto `CombatHUD` y añade script.
4. Asigna referencias: `playerHurtbox`, `styleManager`, textos.

## 10) Música dinámica por rango

No viene implementada directamente, pero puedes hacer:
1. Script `DynamicMusicController` con `AudioSource`.
2. Leer `StyleManager.CurrentRank` cada frame o por cambios.
3. Crossfade de capas (base, intensa, climax) según `D-SSS`.

## 11) Ajuste fino para “feeling DMC”

Prioriza estos puntos:

- **Input buffer corto** (80-150 ms).
- **Cancel windows**:
  - jump cancel en impactos aéreos
  - dodge cancel en frames tardíos
- **Hitstop real** (no setear y resetear en la misma función).
- **Knockback direccional** consistente según lock-on.
- **Telegraph enemigo** claro pero agresivo.

## 12) Orden recomendado de implementación

1. Movimiento + cámara.
2. Hitbox/Hurtbox con daño básico.
3. Combos light/heavy funcionales.
4. Launcher + follow-up aéreo + slam.
5. IA enemiga y oleadas.
6. Style rank + HUD.
7. Animator + VFX/SFX + pulido final.

## 13) Checklist de validación

- [ ] El jugador se mueve, salta, doble salta y dasha sin trabas.
- [ ] Ataques conectan solo una vez por ventana activa.
- [ ] Launcher eleva y permite persecución aérea.
- [ ] Lock-on fija objetivo y la cámara lo encuadra.
- [ ] Enemigos alternan Idle/Chase/Attack/Stunned correctamente.
- [ ] El estilo sube por variedad y cae por pasividad/daño.
- [ ] HUD refleja HP, rango y combo en tiempo real.

---

Si quieres, en un siguiente paso te puedo dejar una **plantilla de escena** (jerarquía exacta de GameObjects + valores por componente) para copiar/pegar en Unity rápidamente.
