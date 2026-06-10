import { useEffect, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { Category, Ingredient, Tag, Unit } from '../mock/types';

type Row = { ingredientId: string; qty: number; unit: Unit };

export function RecipeForm() {
  const [title, setTitle] = useState('Новый рецепт');
  const [desc, setDesc] = useState('');
  const [time, setTime] = useState(30);
  const [difficulty, setDifficulty] = useState<'лёгкая' | 'средняя' | 'сложная'>('средняя');
  const [servings, setServings] = useState(2);
  const [isPublic, setIsPublic] = useState(true);
  const [steps, setSteps] = useState<string[]>(['Шаг 1']);
  const [rows, setRows] = useState<Row[]>([]);
  const [catIds, setCatIds] = useState<string[]>([]);
  const [tagIds, setTagIds] = useState<string[]>([]);
  const [cats, setCats] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  const [ings, setIngs] = useState<Ingredient[]>([]);
  const [submitted, setSubmitted] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    fakeApi.getCategories().then(setCats);
    fakeApi.getTags().then(setTags);
    fakeApi.getIngredients().then((arr) => { setIngs(arr); if (arr[0]) setRows([{ ingredientId: arr[0].id, qty: 100, unit: arr[0].unit }]); });
  }, []);

  function validate() {
    const e: Record<string, string> = {};
    if (title.trim().length < 3) e.title = 'Не короче 3 символов';
    if (desc.trim().length < 10) e.desc = 'Опишите подробнее';
    if (time < 1 || time > 600) e.time = '1–600 мин';
    if (servings < 1) e.servings = '≥ 1';
    if (steps.some((s) => !s.trim())) e.steps = 'Пустые шаги недопустимы';
    if (rows.length === 0) e.ings = 'Добавьте ≥ 1 ингредиента';
    setErrors(e);
    return Object.keys(e).length === 0;
  }

  function onSubmit(e: any) {
    e.preventDefault();
    if (validate()) setSubmitted(true);
  }

  return (
    <>
      <h1>Создание рецепта</h1>
      {submitted && <div className="notice">Прототип: рецепт «{title}» успешно «сохранён» (в реальной БД не сохраняется).</div>}
      <form className="form card" onSubmit={onSubmit}>
        <div className={'field' + (errors.title ? ' has-error' : '')}>
          <label>Название
            <input value={title} onChange={(e) => setTitle(e.target.value)} required />
          </label>
          {errors.title && <div className="field-err">{errors.title}</div>}
        </div>
        <div className={'field' + (errors.desc ? ' has-error' : '')}>
          <label>Описание
            <textarea value={desc} onChange={(e) => setDesc(e.target.value)} />
          </label>
          {errors.desc && <div className="field-err">{errors.desc}</div>}
        </div>
        <div className="row">
          <label>Время, мин
            <input type="number" min={1} max={600} value={time} onChange={(e) => setTime(Number(e.target.value))} />
          </label>
          <label>Сложность
            <select value={difficulty} onChange={(e) => setDifficulty(e.target.value as any)}>
              <option>лёгкая</option><option>средняя</option><option>сложная</option>
            </select>
          </label>
          <label>Порций
            <input type="number" min={1} value={servings} onChange={(e) => setServings(Number(e.target.value))} />
          </label>
          <label>Фото (мок)
            <input type="file" onChange={() => { /* mock */ }} />
          </label>
        </div>
        <label className="row" style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
          <input type="checkbox" checked={isPublic} onChange={(e) => setIsPublic(e.target.checked)} />
          Публичный рецепт
        </label>

        <h3>Шаги</h3>
        {steps.map((s, i) => (
          <div className="step-row" key={i}>
            <span>{i + 1}.</span>
            <input value={s} onChange={(e) => setSteps(steps.map((x, j) => j === i ? e.target.value : x))} />
            <button type="button" className="btn sm" onClick={() => setSteps(steps.filter((_, j) => j !== i))}>✕</button>
          </div>
        ))}
        <button type="button" className="btn" onClick={() => setSteps([...steps, 'Новый шаг'])}>+ шаг</button>
        {errors.steps && <div className="field-err">{errors.steps}</div>}

        <h3>Ингредиенты</h3>
        {rows.map((r, i) => (
          <div className="ing-row" key={i}>
            <select value={r.ingredientId} onChange={(e) => {
              const ing = ings.find((x) => x.id === e.target.value);
              setRows(rows.map((x, j) => j === i ? { ...x, ingredientId: e.target.value, unit: ing ? ing.unit : x.unit } : x));
            }}>
              {ings.map((ing) => <option key={ing.id} value={ing.id}>{ing.name}</option>)}
            </select>
            <input type="number" min={0} step="0.1" value={r.qty} onChange={(e) => setRows(rows.map((x, j) => j === i ? { ...x, qty: Number(e.target.value) } : x))} />
            <select value={r.unit} onChange={(e) => setRows(rows.map((x, j) => j === i ? { ...x, unit: e.target.value as Unit } : x))}>
              <option>г</option><option>шт</option><option>мл</option>
            </select>
            <button type="button" className="btn sm" onClick={() => setRows(rows.filter((_, j) => j !== i))}>✕</button>
          </div>
        ))}
        <button type="button" className="btn" onClick={() => ings[0] && setRows([...rows, { ingredientId: ings[0]!.id, qty: 100, unit: ings[0]!.unit }])}>+ ингредиент</button>
        {errors.ings && <div className="field-err">{errors.ings}</div>}

        <h3>Категории / теги</h3>
        <div className="row">
          {cats.map((c) => (
            <label key={c.id} className="row" style={{ flexDirection: 'row', gap: 4 }}>
              <input type="checkbox" checked={catIds.includes(c.id)} onChange={(e) => setCatIds(e.target.checked ? [...catIds, c.id] : catIds.filter((x) => x !== c.id))} />
              {c.name}
            </label>
          ))}
        </div>
        <div className="row">
          {tags.map((t) => (
            <label key={t.id} className="row" style={{ flexDirection: 'row', gap: 4 }}>
              <input type="checkbox" checked={tagIds.includes(t.id)} onChange={(e) => setTagIds(e.target.checked ? [...tagIds, t.id] : tagIds.filter((x) => x !== t.id))} />
              #{t.name}
            </label>
          ))}
        </div>

        <div className="row">
          <button type="submit" className="btn primary">Сохранить</button>
          <button type="button" className="btn">Отмена</button>
        </div>
      </form>
    </>
  );
}
