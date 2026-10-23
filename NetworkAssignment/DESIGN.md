# 게임 네트워크 프로그래밍 — 설계 설명

> UDP의 기존 설계 기록을 유지하고, 실제 동작을 확인한 NGO 코인 게임 설계를 추가했습니다. 구현하지 않은 보안 강화 및 영속 저장은 제한 사항에 구분해 적었습니다.

## 1. 과제의 분리

- **과제 A:** Unity Netcode for GameObjects 기반 코인 게임. Host는 서버 판정과 클라이언트 기능을 함께 담당하고 Client는 자신의 플레이어 입력·요청을 전송합니다. **구현 및 기능 시험 확인.**
- **과제 B:** `System.Net.Sockets.UdpClient` 기반 별도 콘솔 피어 프로그램. A, B가 각각 자기 2D 위치만 변경하고 상대에게 전송합니다. **실행 및 여러 시험을 확인함.**

## 2. UDP 구성과 데이터 흐름

```text
[피어 A: 내 위치 x,y, 자체 sequence]  <--- UDP --->  [피어 B: 내 위치 x,y, 자체 sequence]
      |   W/A/S/D 또는 B 시험                            |   W/A/S/D 또는 B 시험
      v                                                   v
  위치 메시지 생성 → UTF-8 인코딩 → 송신          위치 메시지 생성 → UTF-8 인코딩 → 송신
      ^                                                   ^
      | 수신 작업(별도 비동기 루프)                         | 수신 작업(별도 비동기 루프)
      v                                                   v
  형식·ID·숫자 검사 → 순서 번호 확인 → 상대 상태 저장 → P에서 표시
```

콘솔의 입력 루프와 비동기 `ReceiveLoopAsync`를 분리하여 수신을 기다리는 동안 입력이 멈추지 않게 합니다. 공유 상대 위치·순서 번호와 통계는 `stateLock`으로 보호합니다.

## 3. UDP 메시지 규약

- 인코딩: UTF-8
- 형식: `피어ID,순서번호,X,Y` (쉼표로 구분한 네 필드)
- 예시: `A,2,1,1`

| 필드 | 자료형/검사 | 설명 |
|---|---|---|
| 피어ID | 문자열, 기대 상대 ID `A` 또는 `B` | 위치의 소유자 식별 |
| 순서번호 | `long`, 1 이상의 정수 | 새로운 위치 메시지를 만들 때 각 피어가 증가 |
| X | `int` | 송신자의 2D X 위치 |
| Y | `int` | 송신자의 2D Y 위치 |

A의 기본 수신 포트는 9001, B의 기본 수신 포트는 9002이며 IP는 `127.0.0.1`입니다. 시작 시 로컬/상대 IPv4와 포트를 입력으로 변경할 수 있습니다. 상대 피어 ID는 자신의 ID에 따라 결정합니다.

## 4. 최신 위치 적용 규칙

1. 수신 데이터그램을 UTF-8 문자열로 변환하여 쉼표 기준 네 필드로 나눕니다.
2. 필드 수가 4가 아니거나 송신 ID가 기대한 상대 ID와 다르면 거절합니다.
3. 순서 번호·좌표가 정수로 파싱되지 않거나 순서 번호가 1 미만이면 거절합니다.
4. `receivedSequence <= lastRemoteSequence`이면 위치를 적용하지 않고 오래된·중복 메시지 수를 증가시킵니다.
5. 더 큰 번호의 메시지만 상대의 `remoteX`, `remoteY`, `lastRemoteSequence`에 반영합니다.
6. 내 위치 `x,y`는 상대 메시지를 받아도 바뀌지 않습니다.

`T`, `R`은 이전/최신 메시지를 원래 번호·좌표 그대로 다시 보내므로 무시 처리 시험에 적합합니다. `E`는 잘못된 메시지를 만들어 전송합니다. `B` 전송량 시험은 새 메시지마다 X·순서 번호를 증가시키고 같은 네 필드 규약을 사용합니다.

## 5. 상태·통계 구조

| 상태 | 의미 |
|---|---|
| `x`, `y`, `sequence` | 본인 위치와 송신 순서 번호 |
| `remoteX`, `remoteY`, `lastRemoteSequence` | 상대의 마지막 **적용된** 위치·순서 번호 |
| `totalDatagrams` | 수신 API에서 반환된 전체 UDP 데이터그램 수 |
| `receivedCount` | 상대 상태에 정상 적용한 메시지 수 |
| `ignoredCount` | 오래되었거나 같은 순서 번호여서 무시한 메시지 수 |
| `invalidCount` | 형식·ID·숫자 검증 실패 메시지 수 |

일반적인 유효 메시지 처리에서는 `totalDatagrams = receivedCount + ignoredCount + invalidCount`로 분류를 검증할 수 있습니다(기타 처리 예외가 없다면). 누적 카운터는 프로세스를 종료하면 초기화됩니다.

**손실 해석:** 송신 API 성공은 전송 요청 성공이며 전달 확인은 아닙니다. `송신 성공 - 정상 반영 수`를 그대로 확정 손실로 단정할 수 없습니다. 지연 도착, 순서 역전, 중복 메시지 무시, 관찰 종료 시점의 영향을 따로 고려해야 합니다. 현재 구현은 전체 수신 수를 집계하지만 네트워크 수준의 확정 손실 검증 프로토콜을 제공하지 않습니다.

## 6. 실시간 상태와 영속 데이터 구분

- 실시간 상태: 각 피어의 X/Y 좌표, 최신 순서 번호, 수신/오류 카운터(실행 중 메모리 보관).
- 오래 보관할 데이터: README의 실행 설정, DESIGN의 규약·설계, TEST_REPORT의 시험 조건과 측정 결과.
- UDP 프로그램은 실행 간 좌표·순서 번호를 디스크에 복원하지 않습니다(요구하지 않는 기능).

## 7. 운영 상태와 오류 기록

- 시작: 피어 ID, 내/상대 IP·포트 출력
- 수신: 적용된 상대 좌표와 순서 번호 출력
- 거절: 형식 오류, 알 수 없는 ID, 숫자 오류 출력
- 순서 판정: 오래된·중복 메시지 무시 출력
- 송신량 시험: 송신 시도, 송신 성공, 송신 오류, 경과 시간 출력
- 상태 조회: 미수신 또는 최신 상대 상태, 전체/반영/무시/거절 카운터 표시
- 종료: `Q`로 수신 작업을 취소하고 UDP 소켓 종료

## 8. NGO 코인 게임 설계 및 데이터 흐름

```text
[Host 시작: NGO 서버 + 로컬 Client]  <--- Unity Transport / NGO ---> [원격 Client]
                  |                                           |
          본인 입력 처리                               본인 입력 처리
                  +--> 각자 소유 Player의 CharacterController 이동
                               |
                   소유자 권한 NetworkTransform
                               |
                      양쪽 위치 화면 반영

[PlayerMovement: E/F1~F5 입력] → [CoinGameManager.RequestCoin(coinId)]
                        → [서버 대상 RPC(RequestCoinRpc)]
                        → [서버: 실제 SenderClientId 확인]
                        → [고정 ID로 코인 조회 / 상태 / PlayerObject / 거리 검사]
                        ├── 거절: 사유 로그, 점수·코인 변경 없음
                        └── 승인: 코인 IsCollected=true, 공동 Score += 1
                                      |
                    NetworkVariable이 Host·Client·재접속 Client에 동기화
                                      |
                          UI 점수 갱신 / 코인 렌더러·콜라이더 비활성화
```

### 주요 객체와 책임

| 구성요소 | 역할 |
|---|---|
| `NetworkLauncher` | Host/Client 시작, IP·포트 설정, 연결 이벤트 기반 상태 UI, 실패 후 재시도 및 연결 종료 |
| `Player` 프리팹 (`NetworkObject`) | 참여자마다 생성, 소유자 부여 |
| `PlayerMovement` (`NetworkBehaviour`) | 소유자만 WASD 입력 처리, `CharacterController.Move`, E/F1~F5 요청 입력 |
| 소유자 권한 `NetworkTransform` | 움직인 소유자의 Transform을 상대 인스턴스에 동기화 |
| `CoinGameManager` (씬의 `NetworkObject`) | 코인 ID 조회, 송신자·거리·상태 확인, 서버만 `Score` 변경 |
| `CoinPickup` (씬의 `NetworkObject`) | 고정 `coinId`, `pickupDistance`, 서버 쓰기 `IsCollected`, 상태 변화 시 코인 렌더러·콜라이더 변경 |

### 점수·코인 구조와 서버 검사

- 공동 점수 `NetworkVariable<int> Score`: `Everyone` 읽기, `Server` 쓰기, 최초 0. 개인별 점수는 사용하지 않습니다.
- 코인 10개: Inspector에서 `coinId`를 `0`부터 `9`까지 고정 부여. `FindObjectsByType()` 결과의 배열 순서는 안정적인 식별자로 사용하지 않습니다.
- 각 코인의 `NetworkVariable<bool> IsCollected`: `Everyone` 읽기, `Server` 쓰기, 최초 false. true가 되면 양쪽에서 표시와 충돌체를 끕니다.
- 서버는 RPC의 `SenderClientId`로 연결된 플레이어 객체를 찾고, 요청 ID가 유효한 코인인지, 아직 미획득인지, 서버가 보고 있는 플레이어와 코인의 거리가 `pickupDistance` 이내인지 검사합니다.
- 승인 시 서버에서 `IsCollected.Value = true`로 변경하고 `Score.Value++`를 수행합니다. 같은 코인에 대해 나중에 처리되는 요청은 `IsCollected` 검사에서 거절되어 중복 보상이 방지됩니다.
- 검사 로그: `[거절] 잘못된 코인 ID`, `거리 초과`, `이미 획득한 코인`, `플레이어 없음`, `[획득 성공]` 등. 실패한 요청은 코인·점수 상태를 바꾸지 않습니다.

### 검증 시험을 위한 입력

- `F1`: 잘못된 ID 999 요청. `F2`: 멀리 있는 미획득 코인 요청. `F3`: 이미 획득한 코인 ID 요청.
- `F4`: 한 참여자가 같은 ID로 두 번 요청. `F5`: 두 참여자가 코인 ID 0을 연속 요청해 경쟁 상황 관찰.
- 이 테스트는 정상적인 플레이와 분리된 개발·평가용 조작입니다. F5는 정확히 동시 도착을 재현하는 자동화 부하 테스트가 아닙니다.

### 동기화와 재접속 정책

- Unity Editor Host와 Windows 빌드 Client를 기본 주소 `127.0.0.1:7777`로 접속하여 두 화면에서 이동·점수·코인 가시성이 일치하는지 확인했습니다.
- Client가 종료돼도 Host의 네트워크 게임은 계속됩니다. Client 재접속 시 서버가 현재 보관 중인 공동 점수와 코인 상태를 동기화합니다.
- 재접속 전에 사용한 플레이어 객체는 다시 생성됩니다. 공동 점수 정책이므로 개인별 점수 복구는 필요하지 않습니다.

## 9. 실시간 상태·영속 데이터 및 운영 기록

| 종류 | 상태와 관리 주체 | 보관 정책 |
|---|---|---|
| NGO 실시간 | 현재 연결 참여자, 각 플레이어 위치, 공동 점수, 코인 획득 여부 | Host/서버 실행 중 메모리, 재접속 Client에 현재 값 동기화 |
| UDP 실시간 | 피어별 좌표·순서 번호·수신 통계 | 각 콘솔 프로세스 메모리, 재시작 시 초기화 |
| 영속 보관 | 실행 환경, 규약, 조작법, 시험 조건·실측값 | `README.md`, `DESIGN.md`, `TEST_REPORT.md` |

운영 상태 관찰 항목은 시작 시 주소·포트, Host·Client 역할, Client 연결 시도·성공·해제·실패, RPC 승인·거절 원인, UDP 파싱 실패·무시 수·송신/수신 수·측정 시간입니다. NGO 접속 UI는 연결 이벤트와 10초 타임아웃을 사용하며 실패 후 재시도를 허용합니다. 완전한 영속 로그 파일이나 서버 저장 시스템은 구현하지 않았습니다.

## 10. 기술 선택 이유·출처·직접 수정한 부분

- **NGO + Unity Transport:** `NetworkObject` 기반 플레이어 스폰·소유권, RPC와 `NetworkVariable`을 통해 수업 규모의 네트워크 게임 구성에 적합.
- **CharacterController:** Rigidbody 힘 대신 명시적인 이동을 구현하기 쉽고 과제 요구에 맞음.
- **Owner-authoritative NetworkTransform:** 로컬 조작 입력 반응을 얻기 쉬움. **제한:** 서버가 위치 자체를 강제로 검증하지 않으므로 허위 위치를 보내는 조작된 Client에는 안전하지 않음. 거리 판정은 서버가 *보유한* 위치에 대한 판정일 뿐입니다.
- **고정 coinId:** `FindObjectsByType`의 불확정 객체 반환 순서를 네트워크 ID로 사용했을 때의 잘못된 매핑 위험을 제거.
- **서버 쓰기 NetworkVariable:** Client가 점수와 획득 상태를 확정할 수 없도록 제한. 늦게 접속한 Client도 현재 값을 받을 수 있음.
- **UDP `UdpClient`, 비동기 수신, 순서 번호:** UDP의 비보장 전달·순서 특성을 학습하고 최신 위치만 적용하기 위함.

**실제 작성·수정 과정:** ChatGPT로 제안된 코드의 구조를 참고하고 Unity/C# 프로젝트에서 직접 적용·수정·빌드·실행·시험했습니다. NGO에서는 프리팹 불일치 재빌드, `Active Input Handling = Both` 변경, 고정 ID 도입, F1~F5 시험 입력, 상태 이벤트 UI 보완을 진행했습니다. UDP에서는 변수 충돌(`CS0136`) 수정, `T`·`R`·`E`·`B` 입력과 누적 통계, 10ms/1ms/대기 없음 시험을 다뤘습니다.

**자료 출처:** 과제 요구사항, 수업에서 익힌 C#/Unity 사용법, 대화 중 제공된 코드 제안을 참조했습니다. 제출 전 실제 참고한 수업자료 및 Unity 공식 문서 URL과 방문일을 확인하여 기입하세요. 참고할 공식 문서: https://docs-multiplayer.unity3d.com/netcode/current/about/ , https://docs.unity3d.com/ , https://learn.microsoft.com/dotnet/api/system.net.sockets.udpclient . URL은 기술 참고 자료로 제시했으며 실제 개인 열람 이력은 확인하지 않았습니다.

**남은 확인 사항:** NGO/Unity Transport 설치 버전, .NET SDK 버전, 실제 프로젝트/씬 명칭, 코인 ID 중복 방지 자동 검사 여부(현재 없음), 서버 권한 이동 검증(현재 없음).
