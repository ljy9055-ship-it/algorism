"""SQLite persistence for the local StoryForge prototype."""
import hashlib
import sqlite3
from pathlib import Path

ROOT = Path(__file__).resolve().parent
DB_PATH = ROOT / "storyforge.sqlite3"


def connect():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def init_db():
    with connect() as conn:
        conn.executescript("""
        CREATE TABLE IF NOT EXISTS documents (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            category TEXT NOT NULL DEFAULT '기타',
            body TEXT NOT NULL,
            digest TEXT NOT NULL UNIQUE,
            created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS drafts (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            scene_plan TEXT NOT NULL,
            category TEXT NOT NULL,
            model TEXT NOT NULL,
            body TEXT NOT NULL,
            created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        """)


def add_document(title, category, body):
    if not body.strip():
        raise ValueError("빈 원고는 등록할 수 없습니다.")
    digest = hashlib.sha256(body.encode("utf-8")).hexdigest()
    with connect() as conn:
        cursor = conn.execute(
            "INSERT OR IGNORE INTO documents(title,category,body,digest) VALUES(?,?,?,?)",
            (title, category, body, digest),
        )
        return cursor.rowcount > 0


def list_documents():
    with connect() as conn:
        return [dict(row) for row in conn.execute(
            "SELECT id,title,category,length(body) AS length,created_at FROM documents ORDER BY id DESC"
        )]


def get_documents():
    with connect() as conn:
        return [dict(row) for row in conn.execute(
            "SELECT id,title,category,body FROM documents ORDER BY id"
        )]


def save_draft(title, scene_plan, category, model, body):
    with connect() as conn:
        conn.execute(
            "INSERT INTO drafts(title,scene_plan,category,model,body) VALUES(?,?,?,?,?)",
            (title, scene_plan, category, model, body),
        )


def list_drafts():
    with connect() as conn:
        return [dict(row) for row in conn.execute(
            "SELECT id,title,category,model,body,created_at FROM drafts ORDER BY id DESC LIMIT 50"
        )]
