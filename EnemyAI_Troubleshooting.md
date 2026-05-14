# Enemy AI Troubleshooting

Project_Neko enemy AI 구현 중 발생한 Patrol / Detection / Chase 관련 문제 기록.

## 1. `Enemy`에 `detectionRange`가 없다는 오류

### 오류

```text
error CS1061: 'Enemy' does not contain a definition for 'detectionRange'
```

### 원인

`EnemyIdleState` 또는 다른 상태 클래스에서 아래처럼 사용하고 있는데:

```csharp
enemy.detectionRange
```

`Enemy.cs` 안에 같은 이름의 변수가 없으면 발생한다.

C#은 대소문자를 구분하므로 `DetectionRange`, `detectRange`, `detectionrange`는 모두 다른 변수로 취급된다.

### 해결

`Enemy.cs`에 다음 변수를 추가한다.

```csharp
public float detectionRange;
```

아직 감지 기능을 구현하지 않는 단계라면, `enemy.detectionRange`를 사용하는 감지 코드를 잠시 주석 처리해도 된다.

## 2. `Transform`에 `LocalScale`이 없다는 오류

### 오류

```text
error CS1061: 'Transform' does not contain a definition for 'LocalScale'
```

### 원인

Unity의 Transform scale 속성 이름은 `LocalScale`이 아니라 `localScale`이다.

### 해결

잘못된 코드:

```csharp
enemy.transform.LocalScale
```

올바른 코드:

```csharp
enemy.transform.localScale
```

## 3. Enemy가 움직이지 않음

### 증상

Patrol 코드를 작성했지만 Enemy가 가만히 있다.

### 원인

`Enemy`가 실제로 `PatrolState`에 진입하지 않으면 `EnemyPatrolState.UpdateState()`와 `EnemyPatrolState.FixedUpdateState()`가 호출되지 않는다.

즉, Patrol 로직이 있어도 현재 상태가 `null`이거나 다른 상태면 움직이지 않는다.

### 확인할 것

`Enemy.cs`에서 Patrol 상태가 생성되어 있는지 확인한다.

```csharp
patrolState = new EnemyPatrolState(this);
```

그리고 시작 시점에 Patrol 상태로 진입해야 한다.

```csharp
ChangeState(patrolState);
```

### 추가 체크리스트

```text
patrolPoints가 2개 이상 연결되어 있는가?
movementSO에 NewBasicMovement가 연결되어 있는가?
Enemy 오브젝트에 Rigidbody2D가 있는가?
currentState가 null이 아닌가?
```

## 4. Enemy가 양옆으로 늘어나며 사라짐

### 오류

```text
transform.localScale assign attempt for 'Enemy' is not valid.
Input localScale is { Infinity, ... }
```

### 증상

Enemy가 좌우로 계속 늘어나더니 사라진다.

### 원인

`SetFacingDirection()`에 방향값이 아니라 거리값을 넣어서 발생했다.

문제가 되는 형태:

```csharp
float dirX = Mathf.Abs(enemy.transform.position.x - targetPoint.position.x);
SetFacingDirection(dirX);
```

`Mathf.Abs()`는 방향이 아니라 거리다. 예를 들어 거리값이 `5`면, 아래 코드에서 scale이 매 프레임 5배씩 커진다.

```csharp
scale.x = Mathf.Abs(scale.x) * dirX;
```

그러면 scale 값이 계속 커져서 결국 `Infinity`가 된다.

### 해결

`UpdateState()`에서는 도착 판정만 한다.

```csharp
float distanceX = Mathf.Abs(enemy.transform.position.x - targetPoint.position.x);

if (distanceX <= enemy.patrolReachDistance)
{
    MoveToNextPatrolPoint();
}
```

방향 설정은 `FixedUpdateState()`에서 `Mathf.Sign()`으로 계산한 방향값만 사용한다.

```csharp
float dirX = Mathf.Sign(targetPoint.position.x - enemy.transform.position.x);
SetFacingDirection(dirX);
```

### 핵심 구분

```csharp
Mathf.Abs(...)
```

거리 계산용.

```csharp
Mathf.Sign(...)
```

방향 계산용.

`SetFacingDirection()`에는 `-1` 또는 `1` 같은 방향값만 넣어야 한다.

## 5. 플레이어 감지가 되지 않음

### 증상

`CanDetectPlayer()`를 구현했지만 `Debug.Log("플레이어를 발견했습니다!")`가 출력되지 않는다.

### 원인 후보

`detectionRange` 또는 `detectionHeight`가 Inspector에서 `0`으로 되어 있으면 감지 조건이 거의 항상 실패한다.

예를 들어:

```csharp
bool isWithinXRange = Mathf.Abs(distanceX) <= enemy.detectionRange;
bool isWithinYRange = distanceY <= enemy.detectionHeight;
```

`detectionRange`가 `0`이면 X 좌표가 완전히 같아야만 감지된다.

`detectionHeight`가 `0`이면 Y 좌표도 거의 완전히 같아야만 감지된다.

### 해결

Inspector에서 임시로 다음과 같이 설정해 테스트한다.

```text
Detection Range: 5
Detection Height: 1.5
```

또한 현재 감지 방식이 "전방 감지"라면, 플레이어가 Enemy가 바라보는 방향에 있어야 한다.

```csharp
bool isPlayerInFront = Mathf.Sign(distanceX) == facingDirection;
```

Enemy가 오른쪽을 보고 있는데 플레이어가 왼쪽에 있으면 감지하지 않는다.

## 6. 게임 시작 시 Enemy가 가만히 있다가 나중에 움직임

### 증상

게임 시작 직후 Enemy가 가만히 있다. 플레이어와 접촉하거나 특정 상황 이후에야 움직이기 시작한다.

### 원인

Enemy의 시작 상태가 `IdleState`였고, `IdleState` 안에서 플레이어 감지 조건에 걸리면 `return`만 하고 있었다.

예시:

```csharp
if (distanceToPlayer <= enemy.detectionRange)
{
    // enemy.ChangeState(enemy.chaseState);
    return;
}
```

Chase 전환은 주석 처리되어 있는데 `return`은 실행되므로, Idle 상태에서 Patrol로 넘어가지 못하고 멈춘다.

### 해결

Patrol 테스트 단계에서는 시작 상태를 바로 Patrol로 둔다.

```csharp
private void Start()
{
    ChangeState(patrolState);
}
```

또는 Idle 상태의 감지 로직이 아직 미완성이라면 `return`으로 흐름을 막지 않도록 수정한다.

## 7. 감지는 되지만 ChaseState로 전환되지 않음

### 증상

`Debug.Log("플레이어를 발견했습니다!")`는 출력되지만 Enemy가 Chase 상태로 바뀌지 않는다.

### 원인

`EnemyPatrolState`에서 Chase 전환 코드가 주석 처리되어 있었다.

문제가 되는 형태:

```csharp
if (CanDetectPlayer())
{
    // enemy.ChangeState(enemy.chaseState);
    Debug.Log("플레이어를 발견했습니다!");
    return;
}
```

### 해결

감지 성공 시 Chase 상태로 전환한다.

```csharp
if (CanDetectPlayer())
{
    Debug.Log("플레이어를 발견했습니다!");
    enemy.ChangeState(enemy.chaseState);
    return;
}
```

### 추가 확인

`Enemy.cs`에서 `chaseState`가 생성되어 있어야 한다.

```csharp
chaseState = new EnemyChaseState(this);
```

`losePlayerRange`가 `0`이면 Chase로 들어가자마자 Patrol로 돌아갈 수 있으므로, Inspector 값도 확인한다.

```text
detectionRange = 5
losePlayerRange = 7
```

처럼 `losePlayerRange`를 `detectionRange`보다 크게 두는 것이 좋다.

## 8. ChaseState에서 PatrolState로 돌아가지 않음

### 증상

Enemy가 Chase 상태로 전환된 뒤, 플레이어가 멀어져도 Patrol 상태로 돌아가지 않는다.

### 기본 복귀 조건

```csharp
float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTarget.position);

if (distanceToPlayer > enemy.losePlayerRange)
{
    enemy.ChangeState(enemy.patrolState);
    return;
}
```

### 원인 후보 1: 실제 거리가 `losePlayerRange`를 넘지 않음

Chase 중에는 Enemy가 계속 플레이어를 따라가기 때문에, 생각보다 거리가 벌어지지 않을 수 있다.

아래 로그로 실제 값을 확인한다.

```csharp
Debug.Log($"Distance To Player: {distanceToPlayer}, Lose Range: {enemy.losePlayerRange}");
```

`distanceToPlayer`가 `losePlayerRange`보다 커지지 않으면 Patrol로 돌아가지 않는 것이 정상이다.

### 원인 후보 2: Play 중 Inspector 값을 바꿈

Unity는 Play 중에 바꾼 Inspector 값을 Stop 후 원래대로 되돌린다.

`detectionRange`, `detectionHeight`, `losePlayerRange`는 Play 전에 설정해야 한다.

### 원인 후보 3: Patrol로 돌아가자마자 다시 감지됨

Patrol로 복귀했지만 즉시 `CanDetectPlayer()`가 true가 되어 다시 Chase로 들어갈 수 있다.

확인용 로그:

```csharp
Debug.Log("플레이어를 놓쳐서 Patrol로 복귀");
enemy.ChangeState(enemy.patrolState);
```

그리고 `EnemyPatrolState.EnterState()`에:

```csharp
Debug.Log("PatrolState 진입");
```

을 넣으면 복귀 여부를 확인할 수 있다.

### 설계 개선 후보

Patrol 감지가 X/Y 기준이라면 Chase 해제도 X/Y 기준으로 맞추는 것이 자연스럽다.

```csharp
float distanceX = Mathf.Abs(enemy.playerTarget.position.x - enemy.transform.position.x);
float distanceY = Mathf.Abs(enemy.playerTarget.position.y - enemy.transform.position.y);

bool isTooFarX = distanceX > enemy.losePlayerRange;
bool isDifferentFloor = distanceY > enemy.detectionHeight;

if (isTooFarX || isDifferentFloor)
{
    enemy.ChangeState(enemy.patrolState);
    return;
}
```

## 9. 오른쪽에 있는 플레이어를 감지하지 못함

### 증상

Enemy가 왼쪽으로 이동 중일 때는 플레이어 감지가 된다.  
하지만 오른쪽으로 이동 중일 때는 플레이어가 오른쪽에 있어도 감지되지 않는다.

`PlayerDir` 로그가 예상과 다르게 나온다.

### 관련 로그

```csharp
Debug.Log(
    $"Target: {enemy.playerTarget.name}, EnemyX: {enemyPosition.x}, PlayerX: {playerPosition.x}, DistanceX: {distanceX}, PlayerDir: {Mathf.Sign(distanceX)}"
);
```

### 원인

`Enemy`의 `playerTarget`에 실제 움직이는 플레이어 오브젝트가 아니라 다른 Transform이 연결되어 있었다.

예를 들어 씬 구조가 다음과 같을 때:

```text
Player
  2_0
```

실제로 움직이는 오브젝트가 `2_0`인데 `playerTarget`에는 부모 `Player`나 다른 오브젝트가 연결되어 있으면, 거리 계산과 방향 판단이 잘못된다.

감지 로직은 아래 값을 기준으로 동작한다.

```csharp
enemy.playerTarget.position
```

따라서 `playerTarget`이 잘못 연결되어 있으면 `distanceX`, `PlayerDir`, `distanceY`가 모두 실제 화면 위치와 다르게 계산될 수 있다.

### 해결

Enemy Inspector에서 `playerTarget`에 실제로 이동하는 플레이어 오브젝트의 Transform을 연결한다.

### 확인 방법

```text
1. Play 실행
2. Hierarchy에서 Enemy 선택
3. Enemy의 playerTarget에 연결된 오브젝트 확인
4. 해당 오브젝트의 Transform Position이 실제 캐릭터 이동과 함께 변하는지 확인
```

### 체크리스트

```text
playerTarget이 실제 플레이어인가?
playerTarget.position이 플레이어의 현재 월드 위치와 일치하는가?
부모 Player와 자식 캐릭터 오브젝트 중 실제 이동하는 쪽을 넣었는가?
```

### 핵심 요약

```text
PlayerDir이 이상하면 감지 공식보다 playerTarget 참조를 먼저 확인한다.
```

## Enemy Inspector 체크리스트

Enemy AI가 이상하게 동작할 때 먼저 아래 항목을 확인한다.

```text
movementSO가 연결되어 있는가?
playerTarget이 실제 움직이는 플레이어 오브젝트인가?
patrolPoints가 2개 이상 연결되어 있는가?
detectionRange가 0보다 큰가?
detectionHeight가 0보다 큰가?
losePlayerRange가 detectionRange보다 큰가?
Rigidbody2D가 Enemy 오브젝트에 붙어 있는가?
게임 시작 시 ChangeState(patrolState)가 호출되는가?
```

## 상태별 역할 정리

```text
UpdateState
- 감지
- 상태 전환
- 웨이포인트 도착 판단
- 타이머 체크

FixedUpdateState
- Rigidbody 이동
- velocity 변경
- 물리 처리

ExitState
- 상태를 떠날 때 정리
- 속도 초기화
- 애니메이션 정리
```

## 현재 권장 흐름

```text
PatrolState
1. CanDetectPlayer() 확인
2. 감지되면 ChaseState로 전환
3. 감지되지 않으면 웨이포인트 도착 여부 확인
4. FixedUpdateState에서 현재 웨이포인트 방향으로 이동

ChaseState
1. playerTarget이 없으면 PatrolState로 복귀
2. 플레이어가 losePlayerRange 밖이면 PatrolState로 복귀
3. FixedUpdateState에서 플레이어 방향으로 이동
```

