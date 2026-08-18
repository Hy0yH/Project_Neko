# 담당 구현 — Hy0yH

[프로젝트 README로 돌아가기](../../README.md)

> 이 문서는 Git 커밋과 현재 코드를 기준으로 작성한 기여 초안입니다. 공동 구현의 역할 분담은 협업자 검토 후 확정합니다.

## 역할 요약

적 AI와 보스전, 지하·빌라 스테이지, 메뉴·설정·컷신 UI를 중심으로 구현했습니다. 여러 스테이지를 하나의 진행 흐름으로 연결하고, 빌라 구역별 Blur와 사운드 등 플레이 연출을 구성했습니다.

## 주요 구현

### FSM 기반 적 AI

**과제**

적이 플레이어와의 거리 및 공격 가능 여부에 따라 순찰, 추적, 공격 행동을 전환할 수 있어야 했습니다.

**구현**

- `IEnemyState`를 기준으로 `Idle`, `Patrol`, `Chase`, `Attack` 상태를 분리했습니다.
- 상태 진입과 실행 흐름을 `Enemy`에서 관리하도록 구성했습니다.
- 이동과 공격 동작을 ScriptableObject로 분리해 수치와 패턴을 데이터로 설정했습니다.

**결과**

상태별 책임이 분리되고, 이동·공격 데이터를 교체해 적 행동을 확장할 수 있는 구조를 구성했습니다.

**관련 코드**

- [Enemy Core](../../Assets/Scripts/Runtime/Enemies/Core)
- [Enemy States](../../Assets/Scripts/Runtime/Enemies/States)
- [Enemy Data](../../Assets/Scripts/Runtime/Enemies/Data)
- [Enemy AI 트러블슈팅 기록](../../EnemyAI_Troubleshooting.md)

### 지하·빌라 스테이지와 씬 흐름

**과제**

옥상, 지하 통로, 빌라를 이동할 때 진입 방향에 맞는 위치에서 플레이어가 등장하고, 부활과 일반 이동을 구분해야 했습니다.

**구현**

- 이동할 씬과 Spawn ID를 함께 전달하는 씬 전환 흐름을 구성했습니다.
- 씬 로드가 완료되면 일치하는 `SceneSpawnPoint`를 찾아 플레이어를 배치했습니다.
- 지하 통로와 빌라 씬을 구성하고 스테이지 간 연결 지점을 설정했습니다.

**결과**

여러 씬을 오갈 때 이동 방향과 목적지에 맞는 위치에서 플레이를 이어갈 수 있습니다.

**관련 코드**

- [Scene Flow](../../Assets/Scripts/Runtime/World/Scenes)
- [Underground Stage](../../Assets/Scenes/UndergroundStage.unity)
- [Villa Stage](../../Assets/Scenes/Villa.unity)

### 메뉴·설정·컷신 UI

**과제**

게임 시작, 설정, 일시 정지, 컷신과 엔딩으로 이어지는 사용자 흐름이 필요했습니다.

**구현**

- 메인 메뉴와 버튼 Hover·Click 상호작용을 구성했습니다.
- 밝기와 Master/BGM/SFX 볼륨을 조절하고 PlayerPrefs에 저장했습니다.
- 이미지 기반 오프닝·엔딩 컷신과 다음 씬 이동을 연결했습니다.

**결과**

게임 실행부터 플레이, 일시 정지와 엔딩까지 필요한 UI 흐름을 제공했습니다.

**관련 코드**

- [Menu and Cutscene UI](../../Assets/Scripts/Runtime/UI/Menus)
- [Display Settings](../../Assets/Scripts/Runtime/UI/Settings)

### 빌라 구역별 Blur 연출

**과제**

여러 층이 한 화면에 표시되는 빌라에서 현재 플레이 구역을 강조하고 공간의 깊이감을 표현해야 했습니다.

**구현**

- URP 2D Renderer Feature와 전용 Blur Shader·Material을 구성했습니다.
- Trigger Zone으로 플레이어가 위치한 층을 판별했습니다.
- 현재 층과 보스전·엔딩 진행 상태에 맞춰 Blur 적용 구역을 변경했습니다.

**결과**

플레이 중인 층은 선명하게 유지하고 다른 구역은 흐리게 표현해 시선을 현재 진행 구역에 집중시켰습니다.

**관련 코드**

- [Villa Environment](../../Assets/Scripts/Runtime/World/Environment)
- [Villa Blur Shader](../../Assets/Shader/VillaFloorBlur.shader)

## 협업 구현

### 보스전과 엔딩 흐름

- **본인 담당:** 보스 조우 Trigger, 보스 이동·체력, 빌라 Blur 제어와 엔딩 컷신 연동
- **협업자 담당:** 플레이어 이동·전투·피격 시스템과 HUD
- **공동 결과:** 보스 구역 진입부터 전투, 처치 후 엔딩까지 이어지는 진행 흐름을 완성했습니다.

**관련 코드**

- [Bosses](../../Assets/Scripts/Runtime/Enemies/Bosses)
- [Player Combat](../../Assets/Scripts/Runtime/Player/Combat)
- [Ending Cutscene](../../Assets/Scripts/Runtime/UI/Menus/EndingCutsceneTrigger.cs)

### 전체 스테이지 진행

- **본인 담당:** 지하·빌라 스테이지와 씬 전환 흐름
- **협업자 담당:** 옥상 스테이지, 플레이어 영속화와 체크포인트
- **공동 결과:** 옥상에서 지하를 거쳐 빌라와 엔딩으로 이어지는 플레이 동선을 구성했습니다.

**관련 코드**

- [Scene Flow](../../Assets/Scripts/Runtime/World/Scenes)
- [Persistent Objects](../../Assets/Scripts/Runtime/Common/Persistent)
- [Checkpoints](../../Assets/Scripts/Runtime/World/Objects)

## 문서 관리

- 이 문서의 역할 설명을 수정할 때 관련 코드, 커밋 또는 PR 링크를 함께 확인합니다.
- 공동 작업의 분담 표현은 `Junhong123`의 기여 문서와 일치하도록 함께 검토합니다.
- 새로운 구현을 추가할 때 `과제 → 구현 → 결과 → 관련 코드` 형식을 유지합니다.
