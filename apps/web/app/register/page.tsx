"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { register } from "@/lib/bff/auth";

export default function RegisterPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const fd = new FormData(e.currentTarget);
    const displayName = fd.get("displayName") as string;
    const email = fd.get("email") as string;
    const password = fd.get("password") as string;

    try {
      await register({ displayName, email, password });
      router.push("/");
      router.refresh();
    } catch {
      setError("Не удалось создать аккаунт. Возможно, email уже занят.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <div>
          <h1>
            Создайте <span className="t-gradient">аккаунт</span>.
          </h1>
          <p className="sub">Присоединяйтесь к кулинарной книге.</p>
        </div>

        <form className="modal-body" onSubmit={handleSubmit} autoComplete="off">
          <div className="field">
            <label htmlFor="displayName">Имя</label>
            <input
              id="displayName"
              className="input"
              type="text"
              name="displayName"
              required
              minLength={3}
              maxLength={200}
              autoComplete="name"
            />
          </div>
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
              autoComplete="new-password"
            />
          </div>
          {error && <p className="error-text">{error}</p>}
          <button className="btn btn-primary" type="submit" disabled={loading}>
            {loading ? "Создаём…" : "Создать аккаунт"}
          </button>
        </form>

        <div className="switch">
          Уже есть аккаунт?{" "}
          <Link href="/login">Войти</Link>
        </div>
      </div>
    </div>
  );
}
