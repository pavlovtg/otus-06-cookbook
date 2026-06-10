import { useState } from 'react';
import { fakeApi } from '../mock/fakeApi';

export function LoginScreen({ onLogged }: { onLogged?: (name: string) => void }) {
  const [tab, setTab] = useState<'login' | 'register'>('login');
  const [email, setEmail] = useState('anna@test.ru');
  const [password, setPassword] = useState('pass1234');
  const [name, setName] = useState('');
  const [err, setErr] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(e: any) {
    e.preventDefault();
    setErr(null);
    setInfo(null);
    if (!/.+@.+\..+/.test(email)) return setErr('Некорректный email');
    if (password.length < 6) return setErr('Пароль ≥ 6 символов');
    if (tab === 'register' && name.length < 2) return setErr('Имя слишком короткое');
    setBusy(true);
    try {
      if (tab === 'login') {
        const r = await fakeApi.login({ email, password });
        onLogged?.(r.user.name);
      } else {
        setInfo('Регистрация прототипа: пользователь не сохраняется. Используйте тестовых.');
      }
    } catch (ex: any) {
      setErr(ex.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: '0 auto' }}>
      <h1>Вход</h1>
      {/* style: tabs */}
      <div className="tabs">
        <button className={tab === 'login' ? 'active' : ''} onClick={() => setTab('login')}>Войти</button>
        <button className={tab === 'register' ? 'active' : ''} onClick={() => setTab('register')}>Регистрация</button>
      </div>
      {/* style: form */}
      <form className="form card" onSubmit={submit}>
        {tab === 'register' && (
          <label>Имя
            <input value={name} onChange={(e) => setName(e.target.value)} minLength={2} required />
          </label>
        )}
        <label>Email
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </label>
        <label>Пароль
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} minLength={6} required />
        </label>
        {err && <div className="field-err">{err}</div>}
        {info && <div className="notice">{info}</div>}
        <div className="row">
          <button className="btn primary" type="submit" disabled={busy}>{tab === 'login' ? 'Войти' : 'Создать аккаунт'}</button>
          <span className="muted">Тестовые: anna@test.ru / boris@test.ru — pass1234</span>
        </div>
      </form>
    </div>
  );
}
