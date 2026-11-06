"""StoryForge — personal Korean web-novel style writing prototype."""
import os
from pathlib import Path

import streamlit as st
from dotenv import load_dotenv
from openai import OpenAI
from storage import init_db, add_document, list_documents, get_documents, save_draft, list_drafts
from retrieval import retrieve

load_dotenv(Path(__file__).resolve().parent / ".env")
init_db()

st.set_page_config(page_title="StoryForge", page_icon="📖", layout="wide")
st.title("📖 StoryForge")
st.caption("개인용 소설 문체 참고·집필 프로토타입 | 원고는 로컬 SQLite에 저장됩니다")

CATEGORIES = ["일상", "대화", "전투", "감정", "설명", "기타"]
tab_upload, tab_write, tab_drafts = st.tabs(["① 원고 자료실", "② AI 집필실", "③ 저장한 초안"])

with tab_upload:
    st.subheader("직접 쓴 원고 등록")
    st.info("문체 자료로 쓸 원고만 올리세요. 첫 실험에서는 10~20화부터 시작하는 것을 권장합니다.")
    category = st.selectbox("이 원고들의 주요 장면 유형", CATEGORIES)
    uploads = st.file_uploader("UTF-8 TXT 파일을 선택하세요 (복수 선택 가능)", type=["txt"], accept_multiple_files=True)
    if st.button("선택한 원고 등록", type="primary"):
        added, skipped, errors = 0, 0, []
        for file in uploads or []:
            if file.size > 2_000_000:
                errors.append(f"{file.name}: 2MB를 초과합니다")
                continue
            try:
                raw = file.getvalue()
                try:
                    body = raw.decode("utf-8-sig")
                except UnicodeDecodeError:
                    body = raw.decode("cp949")
                if add_document(file.name, category, body):
                    added += 1
                else:
                    skipped += 1
            except Exception as exc:
                errors.append(f"{file.name}: {exc}")
        st.success(f"등록: {added}개 / 중복: {skipped}개")
        for err in errors:
            st.error(err)
    docs = list_documents()
    st.metric("등록한 회차", len(docs))
    if docs:
        st.dataframe(docs, use_container_width=True, hide_index=True)

with tab_write:
    st.subheader("장면 구성 → 문체 참고 → 초안")
    st.caption("이 단계의 검색은 문자 유사도 기반이며, 문체 분석 모델을 학습시킨 상태는 아닙니다.")
    draft_title = st.text_input("장면 제목", "1장 — 첫 만남")
    target_category = st.selectbox("장면 유형", ["전체"] + CATEGORIES)
    scene_plan = st.text_area("장면별 상세 구성", height=190, placeholder="사건 순서, 행동, 감정 변화, 장면 목적을 입력하세요.")
    constraints = st.text_area("금지 사항·설정·인물 말투 (선택)", height=105, placeholder="예: 등장인물의 의도를 임의로 추가하지 말 것.")
    model = st.text_input("사용할 OpenAI 모델 ID", value=os.getenv("OPENAI_MODEL", "gpt-6.1-sol"))
    docs = get_documents()
    references = retrieve(docs, scene_plan, target_category, 3) if docs and scene_plan.strip() else []
    with st.expander("선택된 참고 원고 보기", expanded=False):
        for ref in references:
            st.write(f"**{ref['title']}** · {ref['category']} · 검색점수 {ref['score']}")
            st.text(ref["text"][:1100])
    if st.button("원고 생성", type="primary", disabled=not bool(scene_plan.strip())):
        key = os.getenv("OPENAI_API_KEY", "")
        if not key or key == "your_api_key_here":
            st.error(".env 파일의 OPENAI_API_KEY를 설정하세요.")
        else:
            reference_text = "\n\n".join(f"[참고 {i+1} | {r['title']} | {r['category']}]\n{r['text']}" for i,r in enumerate(references))
            instructions = (
                "당신은 한국어 웹소설 집필 보조자입니다. 사용자의 장면 구성을 충실히 원고로 옮깁니다. "
                "참고 원고의 문장 호흡, 서술과 대사의 배합, 어휘와 리듬을 분석하여 새로운 문장에 반영하세요. "
                "참고 원고의 사건, 인명, 문장을 복제하거나 현재 이야기의 사실로 취급하지 마세요. "
                "사용자가 주지 않은 중요한 세계관 설정이나 인물 동기를 임의로 확정하지 마세요. "
                "완성 원고만 출력하세요."
            )
            prompt = (
                f"[장면 제목]\n{draft_title}\n\n[장면 유형]\n{target_category}\n\n"
                f"[사용자가 확정한 장면 구성]\n{scene_plan}\n\n[설정 및 금지 사항]\n{constraints or '없음'}\n\n"
                f"[문체 참고용 발췌문: 다른 작품의 사건을 그대로 옮기지 말 것]\n{reference_text or '없음'}"
            )
            try:
                with st.spinner("초안 작성 중"):
                    response = OpenAI(api_key=key).responses.create(model=model, instructions=instructions, input=prompt)
                output = response.output_text
                if not output.strip():
                    st.warning("모델이 텍스트를 반환하지 않았습니다.")
                else:
                    st.session_state["current_draft"] = output
                    st.session_state["current_meta"] = (draft_title, scene_plan, target_category, model)
            except Exception as exc:
                st.error(f"생성 실패: {exc}")
    if st.session_state.get("current_draft"):
        st.divider()
        edited = st.text_area("초안 확인 및 수정", value=st.session_state["current_draft"], height=420)
        st.download_button("TXT 다운로드", data=edited, file_name="storyforge_draft.txt", mime="text/plain")
        if st.button("수정된 초안 로컬 저장"):
            title, plan, cat, mod = st.session_state["current_meta"]
            save_draft(title, plan, cat, mod, edited)
            st.success("SQLite에 저장했습니다.")

with tab_drafts:
    st.subheader("저장한 초안")
    drafts = list_drafts()
    if not drafts:
        st.info("아직 저장한 초안이 없습니다.")
    for draft in drafts:
        with st.expander(f"{draft['title']} · {draft['created_at']}"):
            st.text_area("본문", value=draft["body"], height=260, key=f"saved-{draft['id']}")
            st.download_button("TXT 다운로드", data=draft["body"], file_name=f"draft_{draft['id']}.txt", key=f"download-{draft['id']}")
