# HacknSlash Prototype (Unity 5.6 / C# 4.0)

Prototipo base de un juego **hack & slash** en tercera persona inspirado en Devil May Cry.

## Objetivo del prototipo

- Combate rápido con ataques light/heavy, launchers, slams y seguimiento aéreo.
- Sistema de estilo con rangos `D -> C -> B -> A -> S -> SS -> SSS`.
- Lock-on y cámara dinámica enfocada al combate.
- IA enemiga agresiva por estados con soporte de oleadas.

## Arquitectura implementada

- `PlayerController`: movimiento, dash, salto y doble salto.
- `PlayerCombat`: combos, cancel windows, lock-on y aplicación de daño.
- `PlayerStateMachine`: estados de locomoción y combate del jugador.
- `EnemyAI`: lógica de decisión ofensiva.
- `EnemyCombat`: ataque y daño al jugador.
- `EnemyStateMachine`: estados del enemigo (idle/chase/attack/stunned/airborne/dead).
- `Hitbox` / `Hurtbox` / `DamageData`: núcleo de colisiones de combate.
- `StyleManager`: medidor de estilo y degradación por pasividad/repetición.
- `ThirdPersonCamera`: cámara semi-libre con lock-on y zoom dinámico.
- `EnemyWaveSpawner`: spawn por oleadas tipo arena.
- `CombatHUD`: HUD minimalista (vida, estilo, combo).

## Notas de integración en Unity 5.6

1. Crear capas para `Enemy`, `Player` y colisiones de hit/hurt.
2. Asignar `Animator` clásico en jugador y enemigos.
3. Configurar eventos de animación para abrir/cerrar hitboxes.
4. Enlazar `StyleManager` y `CombatHUD` en escena principal.
5. Configurar música dinámica leyendo `StyleManager.CurrentRank`.

## Alcance

Este repositorio contiene una base funcional orientada a prototipo y extensión.
No incluye assets artísticos, animaciones ni sonidos finales.

## Guía paso a paso

Consulta la guía práctica de implementación en `docs/GUIA_IMPLEMENTACION.md`.
