"""Local character n-gram retrieval. No excerpts leave the PC until generation."""
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity


def split_chunks(text, size=1100, overlap=150):
    if not text:
        return []
    chunks = []
    stride = size - overlap
    for start in range(0, len(text), stride):
        chunk = text[start:start + size]
        if chunk.strip():
            chunks.append(chunk)
        if start + size >= len(text):
            break
    return chunks


def retrieve(documents, query, category="전체", limit=3):
    pool = []
    for doc in documents:
        for chunk in split_chunks(doc["body"]):
            pool.append({"title": doc["title"], "category": doc["category"], "text": chunk})
    if not pool:
        return []
    texts = [item["text"] for item in pool]
    vectorizer = TfidfVectorizer(analyzer="char", ngram_range=(2, 4), max_features=35000)
    matrix = vectorizer.fit_transform(texts + [query or "소설 장면"])
    scores = cosine_similarity(matrix[-1], matrix[:-1]).ravel()
    # Prefer matching scene types, while still allowing discovery across categories.
    scored = sorted(enumerate(scores), key=lambda kv: float(kv[1]) + (0.12 if category != "전체" and pool[kv[0]]["category"] == category else 0), reverse=True)
    # Avoid selecting overlapping fragments from the same chapter repeatedly.
    out, seen_titles = [], set()
    for idx, score in scored:
        item = pool[idx]
        if item["title"] in seen_titles:
            continue
        seen_titles.add(item["title"])
        out.append({**item, "score": round(float(score), 3)})
        if len(out) >= limit:
            break
    return out
