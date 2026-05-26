# Calificación — TP01 "Operación Dron Urbano"

> Evaluación realizada contra `Trabajo Práctico 2C N°01.pdf` y el análisis de código incrustado como comentarios en los scripts del proyecto.

---

## Bugs / Riesgos críticos detectados en el código

Los siguientes pueden provocar errores en ejecución (criterio de "Insuficiente automático" si ocurren en demo):

| #  | Severidad   | Archivo                                         | Riesgo                                                                                                                                          |
|----|-------------|-------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------|
| 1  | **Crítica** | `CitizensSpawner.GetBullet()`                   | `InvalidOperationException` si los enemigos disparan más rápido que el retorno al pool.                                                         |
| 2  | **Crítica** | `StateBase.GoToNextPoint()`                     | `StackOverflowException` si `waypoints.Length == 1` (recursión infinita sin caso base).                                                         |
| 3  | Alta        | `Bullet.cs`                                     | Mezcla `Destroy` y `ReturnBulletToPool` → `MissingReferenceException` latente.                                                                  |
| 4  | Alta        | `UIGameOver.cs` ↔ `UIPauseMenu.cs`              | Inconsistencia `"MainMenu"` vs `"MainMenuScene"` → el botón de Game Over puede fallar al salir al menú.                                         |
| 5  | Alta        | `PlayerBullet.Init`                             | Si `BulletSpeed = 0` en el SO, `duration = ∞` y la bala nunca vuelve al pool.                                                                   |
| 6  | Alta        | `Bullet.cs`, `PlayerBullet.cs`, `FsmManager.cs` | Múltiples `GameObject.Find` con magic strings ("Player", "Ciudadanos", "Waypoints", "Racing Drone Merged") → cualquier renombre rompe el juego. |
| 7  | Media       | `PlayerController.SecondaryShoot()`             | El cooldown `shootCdAux` se decrementa pero nunca se chequea → el M2 dispara cada frame.                                                        |
| 8  | Media       | `FsmManager.CheckPlayerPosition`                | Llamado en `Update`: en el umbral de aggro de 30u, el enemigo oscila entre Walking↔Shoot frame a frame.                                         |
| 9  | Media       | `PlayerController.Rotate`                       | `transform.Rotate` sobre un Rigidbody en `FixedUpdate` → jitter físico, posibles NaN en colisiones.                                             |
| 10 | **Crítica** | En Testeo                                       | Se rompió tras morir en el segundo nivel.                                                                                                       |

> Ninguno de los bugs 1 a 9 es de "muerte instantánea" en el flujo principal (entrar → jugar → matar enemigos → ganar nivel), pero #1 y #4 se pueden disparar fácilmente en una demo de evaluación. **Es muy recomendable corregirlos antes de la presentación oral**.

---

## Patrones de error transversales

- **Object Pooling implementado a medias.** El concepto está pero la implementación contradice el patrón (Enqueue+Dequeue inmediato, Destroy convive con Return).
- **`GameObject.Find` + magic strings** en lugar de inyección por SerializeField o Setup.
- **Eventos suscritos en Awake sin desuscribir** en objetos pool-reciclados → duplicación de handlers tras reciclar (FsmManager).
- **Toggles disfrazados de funciones** (`PauseGame`, `ChangePerspective`).
- **`float` para conteos** (`CurrentLevel`, `EnemiesLeft`, `EnemySpawnQuantityLevelN`, `BulletInstantiateQuantity`) → deben ser `int`.
- **DRY roto**: tres `case` casi idénticos en `CitizensSpawner.InitializePool` y `LevelManager.Start`.
- **God Class** `AudioManager` con 12+ handlers acoplados a 5 emisores distintos.

---

## Nota Final **6**

### Justificación

El proyecto cumple la totalidad de los puntos obligatorios de Partes 1 y 2 (movimiento físico, cámara dual, vida con UI, FSM funcional, dos tipos de disparo, láser predictivo, combate bidireccional, score) y **la mayoría de los puntos estructurales** de Parte 3 (MainMenu, créditos, Level Manager con ScriptableObjects, contenedores en jerarquía).

Sin embargo:

- **Object Pool propio existe pero tiene bugs de diseño graves** (Enqueue+Dequeue inmediato, `Destroy` conviviendo con `Return`, falta de chequeo de cola vacía) → impide alcanzar la franja 9–10.
- **No se aplicaron Clases Abstractas** para `Person`/`Ciudadano` ni **Interfaces** (`IDamageable`) — dos puntos explícitamente valorados en la rúbrica de Parte 3.
- **No hay `DontDestroyOnLoad`** sobre los pools entre escenas.
- Inconsistencias de nombres de escena (`"MainMenu"` vs `"MainMenuScene"`) que pueden romper un botón en demo.
- **Crasheo en runtime **