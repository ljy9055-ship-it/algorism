# StoryForge — 글쓰기 전용 AI 1차 실행형 프로토타입

## 무엇이 가능한가
- 한국어 TXT 원고를 회차별로 등록하고 로컬 SQLite에 저장
- 장면별 상세 구성을 입력해, 등록 원고의 관련 부분을 참고한 새 초안 생성
- 생성 원고 수정·로컬 저장·TXT 다운로드
- GPT 호출 없이 업로드/검색 기능만 우선 점검 가능

**주의:** 이 버전은 문체를 실제 파인튜닝하지 않았고, 정밀 문체 분석기·세계관 관리·자동 교정 학습도 아직 없습니다. 문자 유사도 검색 기반 기초 실험입니다.

## 설치 및 실행 (Windows / PowerShell)
1. Python 3.11 이상 설치. 설치할 때 `Add Python to PATH` 활성화.
2. 이 폴더에서 PowerShell 실행.
3. 아래 명령을 순서대로 실행:

```powershell
py -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install -r requirements.txt
Copy-Item .env.example .env
notepad .env
```

4. `.env` 파일의 `your_api_key_here`를 본인의 **OpenAI API 키**로 교체. `OPENAI_MODEL`은 접근 가능한 모델 ID로 설정.
5. 실행:

```powershell
python -m streamlit run app.py --server.address 127.0.0.1
```

6. 브라우저에 표시된 로컬 주소(보통 `http://127.0.0.1:8501`) 접속.

PowerShell에서 활성화가 차단되면 가상환경을 활성화하는 대신 `\.venv\Scripts\python.exe -m pip install -r requirements.txt`와 `\.venv\Scripts\python.exe -m streamlit run app.py --server.address 127.0.0.1` 명령을 직접 사용할 수 있습니다.

## 원고 업로드
- UTF-8 또는 CP949 `.txt` 파일 지원
- 현재 파일당 2MB 제한
- 선택한 파일 묶음의 장면 유형은 공통으로 적용됨. 혼합 유형이라면 나누어 업로드할 것.
- 원고 삭제/수정 관리 UI는 후속 버전에서 추가 예정.
- 원고/초안 파일은 `storyforge.sqlite3`에 저장됨. 백업 시 해당 파일을 복사.

## 비용 및 개인정보
- ChatGPT Plus와 OpenAI API 결제는 별도.
- 원고와 DB는 PC에 저장되지만, `원고 생성`을 누르면 장면 구성과 선택된 원고 발췌문(최대 3개)이 OpenAI API로 전송됨.
- `.env`는 공개 저장소에 커밋하지 말 것. 외부 접속 가능하게 서버를 실행하지 말 것.
- 이 프로토타입에는 로그인/암호화가 없으며 개인 PC에서만 사용할 것.

## 개발 단계
1. 완료 목표: 원고 업로드, 검색, API 원고 생성, 저장.
2. 다음: 문체 분석(문장/문단 길이, 대사 비율, 어휘, 반복 습관), 장면 태깅, 문체 프로필 파일.
3. 다음: 정답 원고 홀드아웃 평가, 생성문 비교 및 피드백 기록.
4. 다음: 작품별 세계관/말투 관리와 편집기 강화.
