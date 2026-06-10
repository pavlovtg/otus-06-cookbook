import { useState } from 'react';

export type RatingStarsProps = {
  value?: number;
  readOnly?: boolean;
  onChange?: (v: number) => void;
};

export function RatingStars({ value = 0, readOnly, onChange }: RatingStarsProps) {
  const [v, setV] = useState(value);
  return (
    <div className="stars" role="radiogroup" aria-label="Рейтинг">
      {[1, 2, 3, 4, 5].map((i) => (
        <span
          key={i}
          className={'s' + (i <= v ? ' on' : '')}
          role="radio"
          aria-checked={i === v}
          onClick={() => {
            if (readOnly) return;
            setV(i);
            onChange?.(i);
          }}
        >
          ★
        </span>
      ))}
    </div>
  );
}
