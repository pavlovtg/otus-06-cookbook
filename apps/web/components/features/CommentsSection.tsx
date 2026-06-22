"use client";

import * as React from "react";
import { CommentItem } from "./CommentItem";
import { Pagination } from "@/components/ui/Pagination";
import {
  getComments,
  addComment,
  deleteComment,
  type CommentDto,
  type PagedResultCommentDto,
} from "@/lib/bff/comments";

const PAGE_SIZE = 10;

interface CommentsSectionProps {
  recipeId: string;
  initialData: PagedResultCommentDto;
  currentUserId?: string;
  recipeAuthorId?: string | null;
  isAdmin?: boolean;
}

export function CommentsSection({
  recipeId,
  initialData,
  currentUserId,
  recipeAuthorId,
  isAdmin,
}: CommentsSectionProps) {
  const [data, setData] = React.useState<PagedResultCommentDto>(initialData);
  const [page, setPage] = React.useState(initialData.page);
  const [loading, setLoading] = React.useState(false);
  const [text, setText] = React.useState("");
  const [submitting, setSubmitting] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  const totalPages = Math.ceil(data.total / PAGE_SIZE);

  const isAuthenticated = !!currentUserId;
  const hasMyComment = data.items.some((c) => c.authorId === currentUserId);

  async function loadPage(p: number) {
    setLoading(true);
    try {
      const result = await getComments(recipeId, p, PAGE_SIZE);
      setData(result);
      setPage(p);
    } catch {
      // ignore
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!text.trim() || submitting) return;
    setSubmitting(true);
    setError(null);
    try {
      await addComment(recipeId, text.trim());
      setText("");
      await loadPage(1);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Ошибка при отправке");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(commentId: string) {
    try {
      await deleteComment(recipeId, commentId);
      await loadPage(page);
    } catch {
      // ignore
    }
  }

  function canDelete(comment: CommentDto): boolean {
    if (!currentUserId) return false;
    return (
      comment.authorId === currentUserId ||
      recipeAuthorId === currentUserId ||
      !!isAdmin
    );
  }

  const showForm = isAuthenticated && !hasMyComment;

  return (
    <div className="card card-pad-lg" style={{ marginTop: 32 }}>
      <h3 className="t-subheading" style={{ marginBottom: 16 }}>
        Комментарии{data.total > 0 ? ` (${data.total})` : ""}
      </h3>

      {showForm && (
        <form onSubmit={handleSubmit} style={{ marginBottom: 24 }}>
          <div className="field">
            <textarea
              className="textarea"
              placeholder="Напишите комментарий…"
              value={text}
              onChange={(e) => setText(e.target.value)}
              maxLength={2000}
              rows={3}
              disabled={submitting}
            />
            {error && <span className="error-text">{error}</span>}
          </div>
          <div className="form-actions" style={{ marginTop: 8 }}>
            <button
              type="submit"
              className="btn btn-primary btn-sm"
              disabled={submitting || !text.trim()}
            >
              {submitting ? "Отправка…" : "Отправить"}
            </button>
          </div>
        </form>
      )}

      {data.items.length === 0 ? (
        <p className="t-small" style={{ textAlign: "center", padding: "24px 0" }}>
          Комментариев пока нет
        </p>
      ) : (
        <div
          className="comments-list"
          style={{ opacity: loading ? 0.6 : 1, transition: "opacity 200ms" }}
        >
          {data.items.map((comment) => (
            <CommentItem
              key={comment.id}
              comment={comment}
              canDelete={canDelete(comment)}
              onDelete={() => handleDelete(comment.id)}
            />
          ))}
        </div>
      )}

      {totalPages > 1 && (
        <div style={{ marginTop: 16 }}>
          <Pagination
            page={page}
            total={totalPages}
            onChange={loadPage}
          />
        </div>
      )}
    </div>
  );
}
