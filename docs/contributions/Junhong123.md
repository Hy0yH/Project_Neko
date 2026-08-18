# 담당 구현 — Junhong123

[프로젝트 README로 돌아가기](../../README.md)

> 이 문서는 Git 커밋과 현재 코드를 기준으로 작성한 기여 초안입니다. 문서 소유자가 내용을 검토하고 공동 구현의 역할 분담을 확정해야 합니다.

## 역할 요약

플레이어 FSM과 이동, 형태 전환, 전투·체력 및 HUD를 중심으로 구현했습니다. 옥상 스테이지와 체크포인트, 영속 오브젝트를 구성해 씬 이동과 부활 이후에도 플레이 흐름이 이어지도록 작업했습니다.

## 주요 구현

### 플레이어 FSM과 이동

**과제**

이동, 점프, 벽 슬라이드, 벽 점프와 사망처럼 조건이 다른 행동을 안정적으로 전환해야 했습니다.

**구현**

- 공통 `PlayerState`를 기준으로 행동 상태를 분리했습니다.
- 접지와 벽 감지 결과에 따라 이동 상태를 전환했습니다.
- 캐릭터 방향 전환, 이동 속도와 점프 동작을 플레이어 컨트롤러에 연결했습니다.

**결과**

플레이어 행동의 조건과 실행 로직을 상태별로 관리할 수 있는 이동 구조를 구성했습니다.

**관련 코드**

- [Player States](../../Assets/Scripts/Runtime/Player/States)
- [Player Controller](../../Assets/Scripts/Runtime/Player/Core/PlayerController.cs)

### 형태 전환과 전투

**과제**

고양이와 이족보행 형태가 서로 다른 이동·충돌·공격 특성을 사용하면서 자연스럽게 전환되어야 했습니다.

**구현**

- 형태별 이동 속도, Animator, Collider와 공격 데이터를 `PlayerFormSO`로 관리했습니다.
- 변신 애니메이션 종료 시점에 다음 형태의 데이터를 적용했습니다.
- 기본 공격과 차지 공격의 Startup, Active, Recovery 타이밍과 Hitbox를 데이터로 분리했습니다.

**결과**

형태와 공격 수치를 코드 수정 없이 데이터 에셋에서 조정할 수 있는 플레이어 전투 구조를 구성했습니다.

**관련 코드**

- [Player Form](../../Assets/Scripts/Runtime/Player/Core/PlayerForm.cs)
- [Player Combat](../../Assets/Scripts/Runtime/Player/Combat)
- [Player Data](../../Assets/Scripts/Runtime/Player/Data)

### 체력·회복과 HUD

**과제**

피격과 사망 상태를 화면에 표시하고, 수집한 츄르를 사용해 회복할 수 있어야 했습니다.

**구현**

- `IDamageable`을 사용하는 플레이어 체력 및 피격 처리를 구성했습니다.
- 인벤토리의 회복 아이템 수량과 회복 입력을 연결했습니다.
- 체력과 츄르 보유량 변화가 HUD에 반영되도록 구성했습니다.

**결과**

플레이어 상태와 자원 변화를 즉시 확인하고 회복을 사용할 수 있는 게임 피드백을 제공했습니다.

**관련 코드**

- [Player Health and Inventory](../../Assets/Scripts/Runtime/Player/Core)
- [HUD](../../Assets/Scripts/Runtime/UI/HUD)

### 옥상 스테이지와 체크포인트

**과제**

게임 시작 스테이지를 구성하고, 사망 후 마지막으로 활성화한 위치에서 플레이를 재개해야 했습니다.

**구현**

- 옥상 맵과 플레이 동선을 구성했습니다.
- 체크포인트 활성화 위치를 저장하고 부활 흐름에 연결했습니다.
- 플레이어와 공통 관리 오브젝트가 씬 전환 중 유지되도록 Bootstrap·Persistent 구조를 구성했습니다.

**결과**

스테이지 이동과 사망 이후에도 플레이어 및 진행 관리 오브젝트가 일관되게 유지됩니다.

**관련 코드**

- [Rooftop Stage](../../Assets/Scenes/RoofTop_Street.unity)
- [Checkpoints](../../Assets/Scripts/Runtime/World/Objects)
- [Persistent Objects](../../Assets/Scripts/Runtime/Common/Persistent)

## 협업 구현

### 보스전과 엔딩 흐름

- **본인 담당:** 플레이어 이동·전투·피격 시스템과 HUD
- **협업자 담당:** 보스 조우 Trigger, 보스 이동·체력, 빌라 Blur 제어와 엔딩 컷신 연동
- **공동 결과:** 보스 구역 진입부터 전투, 처치 후 엔딩까지 이어지는 진행 흐름을 완성했습니다.

**관련 코드**

- [Player Combat](../../Assets/Scripts/Runtime/Player/Combat)
- [Bosses](../../Assets/Scripts/Runtime/Enemies/Bosses)

### 전체 스테이지 진행

- **본인 담당:** 옥상 스테이지, 플레이어 영속화와 체크포인트
- **협업자 담당:** 지하·빌라 스테이지와 씬 전환 흐름
- **공동 결과:** 옥상에서 지하를 거쳐 빌라와 엔딩으로 이어지는 플레이 동선을 구성했습니다.

**관련 코드**

- [Rooftop Stage](../../Assets/Scenes/RoofTop_Street.unity)
- [Scene Flow](../../Assets/Scripts/Runtime/World/Scenes)

## 문서 관리

- 문서 소유자가 역할 요약과 상세 구현 내용을 검토한 후 확정합니다.
- 공동 작업의 분담 표현은 `Hy0yH`의 기여 문서와 일치하도록 함께 검토합니다.
- 새로운 구현을 추가할 때 `과제 → 구현 → 결과 → 관련 코드` 형식을 유지합니다.
