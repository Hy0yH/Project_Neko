# Project Neko

> 고양이와 이족보행 형태를 오가며 옥상, 지하 통로, 빌라를 탐험하는 2D 액션 플랫포머

[![Unity](https://img.shields.io/badge/Unity-6000.3.9f1-000000?logo=unity&logoColor=white)](https://unity.com/)
![Language](https://img.shields.io/badge/Language-C%23-512BD4?logo=csharp&logoColor=white)
![Render Pipeline](https://img.shields.io/badge/URP-2D-222C37)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D4?logo=windows&logoColor=white)

[게임 다운로드](https://github.com/Hy0yH/Project_Neko/releases) · [팀 구성 및 기여](#-팀-구성-및-기여) · [개발 환경에서 실행](#개발-환경에서-실행)

<!--
대표 플레이 GIF가 준비되면 아래 주석을 해제합니다.
![Project Neko 플레이 화면](docs/images/hero-gameplay.gif)
촬영 및 파일명 규칙: docs/images/README.md
-->

## 프로젝트 소개

Project Neko는 두 가지 형태의 특성을 활용해 스테이지를 이동하고 적과 전투하는 팀 프로젝트입니다. 플레이어는 옥상에서 출발해 지하 통로와 빌라로 이동하며 일반 적과 보스를 상대하고, 체크포인트와 컷신을 통해 하나의 진행 흐름을 경험합니다.

| 항목 | 내용 |
|---|---|
| 장르 | 2D 액션 플랫포머 |
| 개발 형태 | 2인 팀 프로젝트 |
| 개발 기간 | 2026.04–2026.07 |
| 플랫폼 | Windows |
| 엔진 | Unity 6000.3.9f1 |

## 핵심 플레이와 기능

### 형태 전환

- 고양이와 이족보행 형태를 실시간으로 전환합니다.
- 형태별 이동 속도, 공격, 애니메이션과 충돌 범위를 별도 데이터로 관리합니다.
- 변신 애니메이션이 끝나는 시점에 새로운 형태의 능력과 외형을 적용합니다.

### 이동과 전투

- 이동, 점프, 벽 슬라이드와 벽 점프를 상태 기반으로 처리합니다.
- 기본 공격과 차지 공격, 피격, 체력, 사망 및 회복 기능을 제공합니다.
- 일반 적과 보스 모두 동일한 데미지 인터페이스를 통해 공격 판정을 처리합니다.

### 적 AI와 보스전

- 적은 `Idle`, `Patrol`, `Chase`, `Attack` 상태를 전환하며 행동합니다.
- 이동과 공격 패턴을 ScriptableObject 데이터로 분리했습니다.
- 보스 구역 진입부터 전투, 처치 후 엔딩 컷신까지 하나의 흐름으로 연결됩니다.

### 월드 진행

- 옥상, 지하 통로, 빌라로 이어지는 세 개의 플레이 스테이지를 제공합니다.
- 씬 이동 방향에 맞는 Spawn ID를 사용해 플레이어의 등장 위치를 결정합니다.
- 체크포인트 부활, 패럴랙스 배경, 구역별 Blur 등 스테이지 연출을 포함합니다.

## 스테이지

| 스테이지 | 주요 내용 |
|---|---|
| 옥상 거리 | 게임 시작 구간과 기본 이동·전투 진행 |
| 지하 통로 | 적 전투와 연결 통로를 이용한 스테이지 탐색 |
| 빌라 | 층별 Blur 연출, 보스전과 엔딩 진행 |

## 조작법

| 입력 | 동작 |
|---|---|
| 방향키 | 이동 |
| `C` | 점프 |
| `X` | 공격 |
| `Z` | 회복 |
| `Shift` | 형태 전환 |
| `Esc` | 일시 정지 / 뒤로 가기 |

## 기술 스택

- Unity 6000.3.9f1, C#
- Universal Render Pipeline 2D
- Unity Input System
- Cinemachine
- Timeline
- ScriptableObject
- TextMesh Pro

## 주요 코드 구성

```text
Assets/Scripts/Runtime/
├─ Player/      # 플레이어 상태, 이동, 전투와 데이터
├─ Enemies/     # 일반 적 FSM, 공격 데이터와 보스
├─ World/       # 씬 흐름, 체크포인트와 환경 연출
├─ UI/          # HUD, 메뉴, 설정과 컷신
└─ Common/      # 공통 전투 인터페이스와 영속 오브젝트
```

## 👥 팀 구성 및 기여

| 팀원 | 주요 역할 | 핵심 기여 | 상세 |
|---|---|---|---|
| [@Hy0yH](https://github.com/Hy0yH) | 적·스테이지·UI 시스템 | 적 FSM, 보스전, 지하·빌라 스테이지, 씬 전환, 메뉴·설정·컷신, Blur 연출 | [상세 보기](docs/contributions/Hy0yH.md) |
| [@Junhong123](https://github.com/Junhong123) | 플레이어·전투 시스템 | 플레이어 FSM, 이동·벽 점프, 형태 전환, 전투·체력, HUD, 옥상 스테이지, 체크포인트 | [상세 보기](docs/contributions/Junhong123.md) |

위 표는 Git 커밋 기록을 기준으로 정리한 역할 초안입니다. 공동 작업의 구체적인 역할 분담은 두 팀원이 검토한 뒤 확정하며, 상세 내용은 각 기여 문서에서 관리합니다.

### 문서 협업 원칙

- 루트 README는 프로젝트 전체의 공통 정보만 다룹니다.
- 공통 README 골격을 먼저 `main`에 병합한 뒤 개인 상세 문서를 수정합니다.
- Hy0yH는 `docs/readme-hy0yh`, Junhong123은 `docs/readme-junhong123` 브랜치에서 자신의 문서를 관리합니다.
- 개인 문서 변경과 루트 README 변경은 PR을 분리해 같은 파일을 동시에 수정하지 않습니다.
- 여러 사람이 참여한 기능은 `본인 담당`, `협업자 담당`, `공동 결과`로 구분합니다.
- 공통 내용이나 역할 표를 변경할 때는 다른 팀원의 PR 리뷰를 받습니다.
- 동일 기능에 대한 설명이 겹치면 역할 분담을 두 기여 문서에 같은 표현으로 반영한 뒤 병합합니다.

## 실행 방법

### 배포 버전

[GitHub Releases](https://github.com/Hy0yH/Project_Neko/releases)에서 Windows 빌드를 내려받아 실행합니다. 아직 Release가 없다면 아래 개발 환경 실행 방법을 이용해 주세요.

### 개발 환경에서 실행

1. 저장소를 clone합니다.

   ```bash
   git clone https://github.com/Hy0yH/Project_Neko.git
   ```

2. Unity Hub에서 프로젝트를 Unity `6000.3.9f1`로 엽니다.
3. `Assets/Scenes/MainMenu.unity` 씬을 엽니다.
4. Unity Editor의 Play 버튼을 누릅니다.

다른 Unity 버전으로 열면 패키지 및 에셋 재임포트 결과가 달라질 수 있습니다.

## 에셋 및 라이선스

- TextMesh Pro 샘플 리소스에는 [Liberation Sans OFL](Assets/TextMesh%20Pro/Fonts/LiberationSans%20-%20OFL.txt)과 [EmojiOne Attribution](Assets/TextMesh%20Pro/Sprites/EmojiOne%20Attribution.txt)이 포함되어 있습니다.
- Pretendard 폰트, 음원, 맵 및 스프라이트의 제작·제공 출처와 재배포 조건은 팀 검토 후 명시해야 합니다.
- 현재 저장소 루트에는 별도의 `LICENSE`가 없습니다. 저장소 공개가 코드와 에셋의 재사용 또는 재배포 허가를 의미하지 않습니다.

## 문서 자료

- [README 이미지 준비 가이드](docs/images/README.md)
- [Hy0yH 기여 상세](docs/contributions/Hy0yH.md)
- [Junhong123 기여 상세](docs/contributions/Junhong123.md)
