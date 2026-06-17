"use client";

import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { useState } from "react";
import { login } from "@/lib/bff/auth";

export default function LoginPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const fd = new FormData(e.currentTarget);
    const email = fd.get("email") as string;
    const password = fd.get("password") as string;

    try {
      await login({ email, password });
      const from = searchParams.get("from") ?? "/";
      router.push(from);
      router.refresh();
    } catch {
      setError("Неверный email или пароль");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <div>
          <h1>С возвращением.</h1>
          <p className="sub">Войдите, чтобы продолжить готовить.</p>
        </div>

        <form className="modal-body" onSubmit={handleSubmit} autoComplete="off">
          <div className="field">
            <label htmlFor="email">Email</label>
            <input
              id="email"
              className="input"
              type="email"
              name="email"
              required
              autoComplete="email"
            />
          </div>
          <div className="field">
            <label htmlFor="password">Пароль</label>
            <input
              id="password"
              className="input"
              type="password"
              name="password"
              required
              minLength={8}
              autoComplete="current-password"
            />
          </div>
          {error && <p className="error-text">{error}</p>}
          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading ? "Входим…" : "Войти"}
          </button>
        </form>

        <div className="switch">
          Нет аккаунта?{" "}
          <Link href="/register">Зарегистрироваться</Link>
        </div>
      </div>
    </div>
  );
}
