# Tradeo — Fintech SaaS Website (источники)

## Шот

- Dribbble: <https://dribbble.com/shots/26253584-Tradeo-Fintech-Saas-Website>
- Автор: Emon🌟
- Живая реализация (webflow): <https://tradeo-saas.webflow.io/home-v1>

## Артефакты

- `01_*.png` — обложка шота 1600×1200: три превью лендинга на **фиолетовом** фоне (Слой A — подача на Dribbble).
- `03_*.png` — длинное превью «We're Pioneers in the Crypto Space» (1440×8153): варианты hero, IDE-сниппет «Trust and Transparency», блок команды (B&W фото), блок инвесторов, testimonials.
- `04_*.png` — длинное превью «Powerful Features to Simplify Finance» (1440×6607): AI-блок, AI-Powered Fraud Detection, candle-чарт.
- `05_*.png` — длинное превью с акцентом на 3D-карту в перспективе (1440×6165).
- `live-home-full.png` — full-page скриншот живого сайта (1440px viewport @2x, ≈19840px высоты).
- `live-02_hero.png`, `live-03_journey-tabs.png`, `live-04_features-grid.png`, `live-05_integrations.png`, `live-06_pricing-cards.png`, `live-07_testimonials.png` — посекционные PNG-кропы живого сайта по селекторам `section.home-01 / .works-02 / .features-04 / .integration-02 / .pricing-02 / .testimonials-03`.

## Контент

H1 шота (варианты на разных кадрах):

- «Revolutionizing Crypto Banking on AI-Intelligence»
- «We're Pioneers in the Crypto Space»
- «Powerful Features to Simplify Finance»
- «Life changing Crypto Banking, Powered by AI» (live)

Eyebrow-pills: Top AI Platform · How it Works · Features · Integrations · Pricing · Testimonials · Trusted · Investor · Teams.

## Метод

1. Профиль Dribbble за AWS WAF — прямой `curl` отдаёт `x-amzn-waf-action: challenge`.
2. Шот открыт через Playwright headful Chromium, проскроллен до конца. Кадры выше блока `<h2>More by Emon🌟</h2>` (Y≈16708px) собраны и сдедуплицированы по `userupload/<id>`.
3. Дубликат hero отсеян по md5.
4. Параллельно открыт `tradeo-saas.webflow.io/home-v1` — снят full-page screenshot + 12 посекционных кропов + извлечены `getComputedStyle` для топ-цветов, радиусов, теней, шрифтов и градиентов.
