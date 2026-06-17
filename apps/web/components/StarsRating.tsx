"use client";

import * as React from "react";
import { StarIcon, StarOIcon } from "@/components/icons";

export interface StarsRatingProps {
  value?: number;
  defaultValue?: number;
  onChange?: (v: number) => void;
  readOnly?: boolean;
  size?: number;
}

export function StarsRating({
  value,
  defaultValue = 0,
  onChange,
  readOnly,
  size = 20,
}: StarsRatingProps) {
  const [inner, setInner] = React.useState(defaultValue);
  const v = value !== undefined ? value : inner;

  const set = (n: number) => {
    if (readOnly) return;
    if (value === undefined) setInner(n);
    onChange?.(n);
  };

  return (
    <div className={["stars", readOnly ? "is-readonly" : ""].filter(Boolean).join(" ")}>
      {[1, 2, 3, 4, 5].map((i) => (
        <button
          key={i}
          type="button"
          className={i <= v ? "is-on" : ""}
          onClick={() => set(i)}
          aria-label={`${i} из 5`}
        >
          {i <= v ? <StarIcon size={size} /> : <StarOIcon size={size} />}
        </button>
      ))}
    </div>
  );
}
