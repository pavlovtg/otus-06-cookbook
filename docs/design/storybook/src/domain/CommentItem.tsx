import * as React from 'react';
import { TrashIcon } from '../icons';
import { Button } from '../components/Button';
import { getUser, type Comment } from '../mocks';

export function CommentItem({ comment, canDelete, onDelete }: { comment: Comment; canDelete?: boolean; onDelete?: () => void }) {
  const author = getUser(comment.author_id);
  const initial = (author?.display_name || '?').slice(0, 1);
  return (
    <div className="comment">
      <span className="avatar">{initial}</span>
      <div>
        <div className="comment-head">
          <span className="author">{author?.display_name ?? '—'}</span>
          <span className="time">{new Date(comment.created_at).toLocaleString('ru', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })}</span>
        </div>
        <div className="text">{comment.text}</div>
        {canDelete && (
          <div style={{ marginTop: 8 }}>
            <Button variant="ghost" size="sm" icon={<TrashIcon size={14} />} onClick={onDelete}>
              Удалить
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}
