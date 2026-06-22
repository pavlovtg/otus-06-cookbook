"use client";

import * as React from "react";
import type { CommentDto } from "@/lib/bff/comments";

interface CommentItemProps {
  comment: CommentDto;
  canDelete?: boolean;
  onDelete?: () => void;
}

export function CommentItem({ comment, canDelete, onDelete }: CommentItemProps) {
  const initial = (comment.authorName || "?").slice(0, 1).toUpperCase();
  const date = new Date(comment.createdAt).toLocaleString("ru", {
    day: "numeric",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });

  return (
    <div className="comment">
      <span className="avatar">{initial}</span>
      <div>
        <div className="comment-head">
          <span className="author">{comment.authorName}</span>
          <span className="time">{date}</span>
        </div>
        <div className="text">{comment.text}</div>
        {canDelete && (
          <div style={{ marginTop: 8 }}>
            <button
              type="button"
              className="btn btn-ghost btn-sm"
              onClick={onDelete}
            >
              Удалить
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
