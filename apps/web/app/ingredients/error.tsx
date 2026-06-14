"use client";

interface Props {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function IngredientsError({ error, reset }: Props) {
  return (
    <div className="state">
      <div
        className="state-eyebrow"
        style={{ color: "var(--danger)", boxShadow: "inset 0 0 0 1px rgba(244,114,114,0.3)" }}
      >
        Ошибка загрузки
      </div>
      <p className="t-small" style={{ maxWidth: 400 }}>
        {error.message || "Не удалось загрузить список ингредиентов."}
      </p>
      {error.digest && (
        <p className="t-micro" style={{ color: "var(--fg-muted)" }}>
          Код: {error.digest}
        </p>
      )}
      <button className="btn btn-ghost btn-sm" onClick={reset}>
        Попробовать снова
      </button>
    </div>
  );
}
