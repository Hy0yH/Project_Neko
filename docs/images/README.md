# README 이미지 준비 가이드

[프로젝트 README로 돌아가기](../../README.md)

이 디렉터리는 루트 README와 개인 기여 문서에서 사용하는 게임 플레이 이미지 전용입니다. 두 팀원이 동일한 파일명과 촬영 기준을 사용하면 이미지 교체 시 문서 충돌을 줄일 수 있습니다.

## 권장 파일

| 파일명 | 내용 | 사용 위치 |
|---|---|---|
| `hero-gameplay.gif` | 형태 전환 후 적과 전투하는 대표 장면 | README 상단 |
| `feature-form-change.gif` | 고양이와 이족보행 형태 전환 | 핵심 플레이 |
| `feature-enemy-ai.gif` | 순찰 → 추적 → 공격 상태 전환 | 적 AI 소개 |
| `feature-wall-movement.gif` | 벽 슬라이드와 벽 점프 | 이동 소개 |
| `stage-rooftop.png` | 옥상 거리 대표 화면 | 스테이지 소개 |
| `stage-underground.png` | 지하 통로 대표 화면 | 스테이지 소개 |
| `stage-villa.png` | 빌라와 Blur 연출 대표 화면 | 스테이지 소개 |
| `boss-encounter.png` | 보스전 대표 화면 | 보스전 소개 |

## 촬영 기준

- 화면 비율과 해상도를 통일합니다.
- GIF는 핵심 동작이 바로 보이는 5–8초 구간으로 편집합니다.
- 에디터 Gizmo, Console, 개인 정보와 디버그 UI는 화면에서 제외합니다.
- 시작과 끝 프레임을 자연스럽게 연결해 반복 재생 시 끊김을 줄입니다.
- 같은 기능의 새 이미지는 기존 파일명을 유지해 README 수정 없이 교체합니다.

## 대표 GIF 연결

`hero-gameplay.gif`를 추가한 다음 루트 `README.md` 상단의 다음 줄에서 HTML 주석을 제거합니다.

```markdown
![Project Neko 플레이 화면](docs/images/hero-gameplay.gif)
```
