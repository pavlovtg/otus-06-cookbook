# STYLE_GUIDE — Tradeo Fintech SaaS

Декомпозиция визуального стиля шота [Tradeo — Fintech Saas Website by Emon🌟](https://dribbble.com/shots/26253584-Tradeo-Fintech-Saas-Website). Источник токенов 1:1 — `tradeo-saas.webflow.io/home-v1`.

## Главный инсайт: два слоя

Студия делает 70% впечатления через **подачу обложки** и 30% через **сам UI**. Раскладываем стиль на:

- **Слой A — подача (Dribbble cover)**: hero-крупно на **светло-фиолетовом** ярком фоне, 2–3 «слайда» лендинга показаны как floating-плитки с мягкой outer-тенью. Эта подача нужна только обложке.
- **Слой B — сам продукт (живой сайт)**: глубокий blue-black фон, точечная hairline-grid, фиолетовый акцент в типографике и градиентных декорациях.

Не смешивать слои. На реальной странице фиолетового заливочного фона нет. На обложке/презентации — фиолетовый превалирует.

---

## Слой A — Подача (для обложки/hero-промо)

### Композиция

- Светло-фиолетовая заливка `#A38BFF`-диапазон (близко к `rgb(163, 139, 255)`) занимает весь фон.
- На фон помещены 2–3 длинных превью лендинга в виде вертикальных «панелей», слегка сдвинутых по вертикали, без сильного tilt — почти плоские, ровные.
- Между панелями зазор ≈ 1 ширины колонки контента.
- Сами панели — это уже Слой B (dark UI), они «вырезаны» на фиолет, что даёт максимальный контраст value (light bg vs dark UI).
- Тени панелей мягкие, рассеянные, цвет тени — **не чёрный**, а более тёмный фиолетовый (color-burn от фона).

### Свет / грейд

- Источник света — диффузный сверху-слева.
- Грейд тёплый: на фиолетовом фоне присутствует едва уловимый розово-оранжевый свет на одном краю композиции (от радиального glow или элемента UI).
- Без эффектов «3D-render», без bloom, без glass-reflections. Графика 2D, плоская, чистая.

### Материал

- Превью лендингов — растровые, без bevel/shadow внутри. Они выглядят как screenshot, помещённый на цветной фон, а не как mockup на устройстве.

### Когда использовать Слой A

- Только для обложки Dribbble, hero-OG-картинки, slide-preview в кейс-стади. **Не на самом сайте.**

---

## Слой B — Сам продукт (рабочие экраны)

Это и есть live-сайт. Всё ниже — про него.

### Палитра

Тёмная база, фиолетовый акцент, никаких ярких насыщенных цветов вне акцента.

| Роль | HEX | RGB | Назначение |
|------|-----|-----|------------|
| `bg/canvas` | `#0B0B15` | rgb(11, 11, 21) | Основной фон страницы (почти blue-black) |
| `bg/canvas-deep` | `#06060F`–`#0A0A14` | rgb(6, 6, 16) | Под героем, в самом верху страницы |
| `bg/surface-1` | `#0C0C16` | rgb(12, 12, 22) | Поверхность секций |
| `bg/surface-2` | `#17171F`–`#17181C` | rgb(23, 23, 32) | Карточки, тайлы |
| `bg/surface-3` | `#252537` | rgb(37, 37, 55) | Приподнятые элементы, выделенный таб |
| `bg/overlay-hairline` | `rgba(255, 255, 255, 0.06)` | — | Inset-«стекло» на surface (см. «Край определяется светом») |
| `bg/overlay-hairline-strong` | `rgba(255, 255, 255, 0.22)` | — | Активный/hover state |
| `fg/primary` | `#FFFFFF` | rgb(255, 255, 255) | Заголовки, главный текст |
| `fg/secondary` | `#D4D4D4` | rgb(212, 212, 212) | Параграфы, описания |
| `fg/muted` | `#A9A9AC` | rgb(169, 169, 172) | Навигация, лейблы, second tier |
| `accent/purple-500` | `#9968FF` | rgb(153, 104, 255) | Главный акцент (gradient start, link, brand) |
| `accent/pink-400` | `#FF6CB2` | rgb(255, 108, 178) | Gradient middle (purple→pink→orange ribbon) |
| `accent/orange-300` | `#FFAF56` | rgb(255, 175, 86) | Gradient end |
| `accent/link` | `#7D79E7` → `#A279E6` | linear 90° | Линки и подсвеченное слово в заголовке |
| `data/up` | green-pastel band | — | Бары роста на bar chart |
| `data/down` | red-pastel band | — | Бары падения |
| `data/sparkline` | `#9968FF` | — | Тонкий fill/stroke sparkline и candle highlight |

**Правила цвета:**

- На странице ровно **один hue владеет всем** — фиолетовый. Pink/orange появляются ТОЛЬКО внутри 3-стоп-градиента и занимают <5% площади.
- Зелёный/красный — только в data-viz, никогда в UI-chrome.
- Между surface-уровнями разница 4–6 пунктов по value (#0B → #17 → #25). Не больше.
- Бордеры цвета `rgba(255,255,255,0.06)` — это не настоящий border, это inset-shadow (см. ниже).

### Типографика

Один шрифт. Минимум весов. Очень крупный hero. Negative tracking на крупных размерах.

- **Шрифт:** `Inter` (fallback `Arial, sans-serif`).
- **Веса в работе:** `400 Regular` (body), `500 Medium` (все headings, числа, CTA-лейблы). **Bold/600+ не используется.** Это узнаваемый признак стиля — H1 на 80px весит `Medium`, не `Bold`.
- **Negative letter-spacing на крупных размерах:** -0.07em для H1/H2, -0.02em для H3, normal для ≤24px.

| Токен | Размер / line-height / tracking | Вес | Использование |
|-------|---------------------------------|-----|---------------|
| `text/hero` | 80 / 88 / −5.6px (−0.07em) | 500 | H1 hero |
| `text/display` | 56 / 67.2 / −3.92px (−0.07em) | 500 | H2 секций |
| `text/heading` | 34 / 40.8 / −0.68px (−0.02em) | 500 | H3, KPI-числа («$0», «17,435», «$53,647.00») |
| `text/subheading` | 24 / нормальный / 0 | 500 | Названия карточек («Effortless Usability»), пункты табов |
| `text/body` | 16 / 24 / 0 | 400 | Параграфы, nav-link, кнопки |
| `text/body-medium` | 16 / 24 / 0 | 500 | CTA-кнопки, активный таб |
| `text/small` | 14 / 20 / 0 | 400 | Лейблы карточек, deep secondary |
| `text/micro` | 12 / 16 / 0 | 400 | Метки на чартах, легенда |

**Mixed-weight трюк (это критично):**

Heading состоит из двух частей разного цвета **и одного веса** (всё 500):

- Серый-приглушённый («Experience», «Your Journey to», «Your Success,», «It's not enough You need to») — `#FFFFFF` с пониженной непрозрачностью или сразу `#D4D4D4`.
- Один-два слова в фиолетовом градиенте (`Crypto`, `Innovate`, `Success`, `prove it`, `Crypto Space`) — `linear-gradient(90deg, #7D79E7, #A279E6)` с `background-clip: text`.

Это даёт «акцент без тяжести» — взгляд цепляется за фиолетовое слово, а вес остаётся лёгким.

### Радиусы

Большие, но не экстремальные. Используются **3 размера**:

- `radius/pill` = **100px** — eyebrow-pills, primary CTA, бейджи. Доминирует в системе (53 случая в DOM).
- `radius/card` = **8px** — карточки, тайлы, кнопки, инпуты (51 случай).
- `radius/card-lg` = **12px** — большие контейнеры, секции-обёртки.
- (хвост: `8.2px`, `6px` — мелкие иконные подложки).

Никаких 16/20/24px. Никаких squircle. Только эти три.

### Как определяется край (это критично — иначе откатимся в дом-стайл)

**Не бордером.** Везде, где на скриншоте видна тонкая светлая граница карточки — это НЕ `border: 1px solid white/10`. Это **двойная inset+outer тень**:

```text
box-shadow:
  inset  0  0 17px 0 rgba(255, 255, 255, 0.06),  /* верхний rim light */
         0  0 19px 0 rgba(0,   0,   0,   0.06);   /* мягкий ground shadow */
```

Эта пара даёт ощущение «стеклянной плёнки» поверх dark surface. Использовать на 95% карточек. Никакого `1px solid`.

Для приподнятых элементов с purple-glow:

```text
box-shadow:
  inset 0 1.3px 0 rgba(255, 255, 255, 0.55),         /* top edge highlight */
        0 2.6px 6.5px rgba(0, 40, 54, 0.05),
        0 3.9px 6.1px rgba(147, 50, 255, 0.15),
        0 17.7px 27.5px rgba(147, 50, 255, 0.15);
```

Это паттерн для primary-CTA и активного тогглера (purple glow under pill).

Для радиальных декораций фона:

- `radial-gradient(circle at 100% 0, rgba(118, 146, 255, 0) 77%, rgba(122, 150, 255, 0.02))` — еле заметный голубоватый glow в углу секции.
- `radial-gradient(circle, rgba(226, 232, 255, 0) 31%, rgba(226, 232, 255, 0.08))` — мягкий «звёздный» light spot.

### Метрики компоновки

- **Контейнер:** контент-ширина ≈ 1280px, поля по 80px на 1440px.
- **Сетка:** 12 колонок, gap 24px. Feature-карточки — 3 в ряд (или 2 + 1 акцентная).
- **Вертикальные отступы между секциями:** ~120–140px (на webflow padding section ≈ 100–120px top/bottom).
- **Внутри карточки:** padding 24–32px, между заголовком карточки и body 12–16px, между body и controls 24px.
- **Hero высота:** ≈ 1608px (огромный) — герой это полноценный экран + floating dashboard preview снизу с overflow-cut на следующую секцию.

### Иерархия экрана: «один герой»

На каждой секции **ровно один** доминирующий элемент. Иерархия:

1. Eyebrow-pill (12–16px) — навигационный якорь секции в pill-капсуле с иконкой и хайрлайн-линиями слева/справа: `——— [icon] Pricing ———`.
2. H2 (56px Medium) — обычно две строки, одна — серая, одна — purple-gradient.
3. Подзаголовок (16px Regular, `#A9A9AC`) — 1–2 строки, по центру.
4. Один контент-блок: либо таб-группа, либо grid карточек, либо product UI.

### Каталог компонентов

#### Eyebrow Pill (навигация секции)

- Pill с радиусом 100px, фон `bg/surface-1` + inset-rim, padding 8px 16px.
- Левая 16×16 иконка (lucide / phosphor-style, stroke 1.5), gap 8px до текста.
- Текст: 16 / Medium / `#FFFFFF`.
- По бокам — два hairline-сегмента ~120px каждый, цвет `rgba(255,255,255,0.12)`.

#### Primary CTA Button

- `radius/pill` 100px, padding 12px 24px.
- Фон: `#FFFFFF`, текст `#0B0B15`, weight 500.
- Tap-state: легкий scale 0.98 + purple glow shadow.

#### Secondary CTA (ghost)

- Те же размеры, прозрачный фон, текст `#FFFFFF`, hairline-обводка через inset shadow `0 0 0 1px rgba(255,255,255,0.12)` (не настоящий border).

#### Tab Group (Dashboard Overview / Invoice Management / ...)

- Pill-контейнер на surface-1.
- Каждый таб = pill, padding 12px 20px, icon + label.
- Активный таб: inset-highlight + лёгкий purple wash.

#### Toggle (Monthly / Yearly)

- Pill-контейнер. Активная половина — purple-pink gradient pill внутри, текст белый, остальная сторона прозрачная с серым текстом.

#### Card (feature / KPI)

- Surface-2, radius 8px, padding 24–28px.
- Inset+outer shadow (см. выше).
- Если в карточке есть иконка-leaf — она в 56×56 box, radius 8.2px, surface-3, inset highlight.
- Иногда decorative blur — крупный фиолетовый радиальный пик glow по краю карточки.

#### Data-tile (KPI с числом и delta)

- Лейбл (`text/small`, `fg/muted`) сверху.
- Число (`text/heading` 34px Medium) ниже.
- Delta (`+5.74` / `+37.8%`) рядом или ниже, цвет `data/up` или `data/down`, в маленькой капсуле с заливкой того же цвета 12% прозрачности.

#### Coin / Token Row

- Круглая иконка-токен 40px (BTC, ETH, BNB, SOL — цветные filled icons).
- Тикер крупно (16 Medium) + полный hairline-subtext под ним.
- Справа sparkline (фиолетовый stroke, ~80×24px) или дельта.

#### Chart — Candlestick

- Тёмный фон, без рамки.
- Свечи: up — пастельно-зелёные, down — пастельно-розово-красные. Без ярких лайм/RED.
- Линия `MA 7 close` / `MA 25 close` — две тонкие цветные линии (оранжевая + фиолетовая).
- Шкала времени `15m · 1H · 4H · 1D · 1W` — pill-таб-группа.

#### Chart — Bar (months)

- Bars вертикальные, тонкие (4–6px), gap ~3px.
- Цвет в одном ряду — единая палитра: всё зелёное / всё красное / pastel-gradient (purple→pink→orange) для multi-series.
- Без подписей значений на барах; ось X — месяцы (Apr May Jun ... Sep), мини-лейблы 12px muted.

#### Chart — Sparkline / Line

- Stroke `accent/purple-500`, 1.5–2px, без area-fill. Иногда лёгкий fade gradient под линией (от 20% к 0%).

#### Chart — Gauge / Half-donut

- Половина круга, заливка purple-pink gradient ribbon.
- В центре — крупное число (`$1,232`, `+3%`), под ним мелкий лейбл («Binance»).
- По дуге — мелкие тики и числа `$30 $50 $130`.

#### Code Snippet (Trust & Transparency block)

- IDE-like dark surface, monospace (вероятно JetBrains Mono или системный SF Mono).
- Подсветка: ключевые слова — pink/purple, строки — peach/orange, типы — turquoise, комментарии — серый.
- Окно с тремя dot-controls? — **нет**, без traffic-lights, просто блок с заголовком слева сверху.

#### Avatar / Photo

- B&W контрастные фотографии для team/investors (важно: они **дезатурированы** в монохром, чтобы не воевать с фиолетовым акцентом).
- Цветные square-photo (бирюзовый/синий фон) — только в testimonials, чтобы добавить «человечности» в монохромный лендинг.
- Радиус: 8px squircle-like. Не круглые. Это особенность.

#### «Atom» Diagram (Integrations / Instant Account)

- Концентрические dotted-rings вокруг центрального элемента.
- На разных орбитах сидят 6–8 кругов с лого/иконками, каждый — radial-glow подсветка от центра.
- Центральный круг — крупнее, выделен purple-pink gradient.

#### Logo Bar (Trusted by)

- Логотипы партнёров (`Logoipsum`, `logo`, `LOMO`, `BOGO`, ∞) — серым stroke, без заливки, в один ряд, центрировано, gap ~64px.

#### Hairline Decorators

- Тонкие линии (`rgba(255,255,255,0.06)`) — горизонтальные разделители, вертикальная «light bar» (см. между «Your Journey» и продуктовым превью — вертикальная purple-pink gradient линия как ось симметрии).

### Иконография

- Stroke 1.5px, square-ish (lucide / phosphor-style).
- Размер 16/20/24px.
- Цвет `fg/muted`, активный — `fg/primary`. Не фиолетовый.
- Иконка-капсула: 16px icon в 32–56px squircle (radius 8.2px), surface-2, inset highlight.

### Анимация и микро-интеракции (по визуальному следу)

- Eyebrow-pills и кнопки имеют едва заметный pulse purple-glow.
- Скролл-эффекты: floating dashboard preview под hero «выезжает» снизу и слегка наклонен в перспективе — это статика-обманка, имитация 3D, без реального tilt.
- Toggle Monthly↔Yearly — pill-морф (gradient pill сдвигается между двумя положениями).

---

## Структура серии (как устроена обложка → лендинг)

Шот = 3–4 кадра, показывающих один и тот же бренд в разных композициях:

1. **Hero вариант A** — обложка с превью на purple фоне (Слой A).
2. **Длинный кадр 1** («Revolutionizing...») — полный home v1.
3. **Длинный кадр 2** («Powerful Features...») — продуктовая страница features с большими чартами.
4. **Длинный кадр 3** («Pioneers in the Crypto Space») — about/teams с фотографиями и IDE-блоком.

Каждый длинный кадр следует одной схеме сверху вниз:

```text
NAV
HERO (eyebrow → H1 → sub → CTA → floating product UI snippet с overflow cut)
LOGO BAR ("Over the 4M+ connected with us")
WORKS / JOURNEY (eyebrow → H2 → подзаг → tab group + Experience-tile)
FEATURES grid 3×N (тайлы с иконкой → заголовок → текст; есть «mega-tile» с большим slab-текстом «Made for Business Growth»)
INTEGRATIONS (atom-диаграмма с лого)
PRICING (eyebrow → H2 → Monthly/Yearly toggle → 3 плана; средний = Pro, акцентный)
TESTIMONIALS (заголовок с двумя цветами + grid карточек-отзывов с цветными B&W фото)
CTA-FOOTER («Get Started With Invoicing Now» — крупный H2 на тёмной плашке + CTA)
FOOTER (logo + 4 колонки ссылок)
```

---

## Почему это работает

- **Один шрифт + Medium-only** на крупных размерах создаёт ощущение «лёгкости и точности», которое в fintech читается как «современность и не-агрессивность» — противоположность Bold-heavy лендингам банков.
- **Тёмный канвас + один акцент (purple)** даёт product-focus: продуктовые скрины «светятся» в композиции, никакая декорация не отвлекает.
- **Negative tracking −0.07em** на 56/80px собирает заголовок в плотный «брикет» и подсвечивает purple-слово как пик ритма.
- **Pill-радиусы 100px** + 8px-карточки = два узнаваемых формата, между которыми система переключается без шума.
- **Свет вместо бордера** (двойной inset+outer shadow) даёт «глубину», не утяжеляя UI, и работает одинаково на любом surface-уровне.
- **B&W фото людей** на цветных квадратных фонах в testimonials — единственная цветная неоновая нота в нижней половине страницы, что фиксирует взгляд на «социальном доказательстве».

---

## Анти-паттерны

Чего НЕ делать (иначе откатимся в дом-стайл генератора):

- ❌ **Bold/700+ в заголовках.** Только Medium. Тяжёлый bold ломает «лёгкость» бренда.
- ❌ **Нарисованные бордеры** `1px solid #2a2a2a` на карточках. Только inset+outer shadow.
- ❌ **Радиусы 16/20/24px** (дефолт shadcn/Tailwind). Только 8 / 12 / 100.
- ❌ **Несколько hue одновременно** в UI-chrome (cyan + purple + pink). Один фиолетовый.
- ❌ **Bright neon green/red** в data-viz. Только pastel-band.
- ❌ **Drop-shadow с чёрным размытым шлейфом** в духе material. Тени мягкие и почти прозрачные.
- ❌ **Стеклянный glassmorphism** с backdrop-filter blur. Здесь — flat surfaces + inset rim.
- ❌ **Tilt/3D-perspective** на product screenshots. Они почти плоские; перспектива создаётся через лёгкий overflow-cut и floating, не через transform.
- ❌ **Цветные эмодзи/3D-render иллюстрации.** Только 2D stroke-иконки и B&W фотографии.
- ❌ **Lavender/lilac пастельные фоны на самой странице.** Фиолетовый — только акцент и Dribbble-обложка.
- ❌ **Полностью круглые аватары.** Здесь squircle 8px.

---

## Чек-лист «попали в стиль»

Прежде чем считать макет соответствующим Tradeo, проверить:

- [ ] Фон страницы blue-black `#0B0B15` или темнее
- [ ] Один шрифт Inter, веса только 400 и 500
- [ ] H1 80px / lh 88 / tracking −5.6px
- [ ] H2 56px с negative tracking −0.07em
- [ ] Один-два слова в каждом H2 выделены через `linear-gradient(90deg, #7D79E7, #A279E6)` + `background-clip: text`
- [ ] Eyebrow-pill над каждым H2 с иконкой и hairline-сегментами по бокам
- [ ] Радиусы только 100 / 12 / 8 px
- [ ] Карточки без border, только inset rim white 6% + outer shadow black 6%
- [ ] Primary CTA — белая pill, текст почти чёрный
- [ ] Один акцент: фиолетовый. Pink/orange — только внутри 3-стоп gradient в декоре
- [ ] Чарты pastel, без неона; sparkline 1.5–2px purple
- [ ] B&W фото людей в squircle 8px
- [ ] Atom-диаграмма для Integrations
- [ ] Между секциями ~120px vertical padding
- [ ] Нет обводок 1px; нет glass blur; нет 3D-render
