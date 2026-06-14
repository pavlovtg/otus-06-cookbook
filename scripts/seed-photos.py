#!/usr/bin/env python3
"""
Скрипт для скачивания фотографий рецептов с povar.ru.
Ищет каждый рецепт по названию, берёт первый результат, скачивает og:image.

Зависимости:
    pip install requests beautifulsoup4

Использование:
    python3 scripts/seed-photos.py
"""

import uuid
import time
import json
import shutil
from pathlib import Path

import requests
from bs4 import BeautifulSoup

RECIPES = [
    ("11111111-0000-0000-0000-000000000001", "Борщ"),
    ("11111111-0000-0000-0000-000000000002", "Оливье"),
    ("11111111-0000-0000-0000-000000000003", "Пельмени"),
    ("11111111-0000-0000-0000-000000000004", "Блины"),
    ("11111111-0000-0000-0000-000000000005", "Котлеты по-киевски"),
    ("11111111-0000-0000-0000-000000000006", "Солянка"),
    ("11111111-0000-0000-0000-000000000007", "Шашлык"),
    ("11111111-0000-0000-0000-000000000008", "Окрошка"),
    ("11111111-0000-0000-0000-000000000009", "Плов"),
    ("11111111-0000-0000-0000-000000000010", "Медовик"),
    ("11111111-0000-0000-0000-000000000011", "Гречневая каша с грибами"),
    ("11111111-0000-0000-0000-000000000012", "Рассольник"),
    ("11111111-0000-0000-0000-000000000013", "Щи из свежей капусты"),
    ("11111111-0000-0000-0000-000000000014", "Курица в сметанном соусе"),
    ("11111111-0000-0000-0000-000000000015", "Вареники с картофелем"),
    ("11111111-0000-0000-0000-000000000016", "Запечённый лосось"),
    ("11111111-0000-0000-0000-000000000017", "Греческий салат"),
    ("11111111-0000-0000-0000-000000000018", "Тефтели в томатном соусе"),
    ("11111111-0000-0000-0000-000000000019", "Пицца Маргарита"),
    ("11111111-0000-0000-0000-000000000020", "Паста Карбонара"),
    ("11111111-0000-0000-0000-000000000021", "Манты"),
    ("11111111-0000-0000-0000-000000000022", "Рыбные котлеты"),
    ("11111111-0000-0000-0000-000000000023", "Запечённые овощи"),
    ("11111111-0000-0000-0000-000000000024", "Творожная запеканка"),
    ("11111111-0000-0000-0000-000000000025", "Харчо"),
    ("11111111-0000-0000-0000-000000000026", "Сырники"),
    ("11111111-0000-0000-0000-000000000027", "Куриный суп с лапшой"),
    ("11111111-0000-0000-0000-000000000028", "Жаркое из свинины с картофелем"),
    ("11111111-0000-0000-0000-000000000029", "Фаршированный перец"),
    ("11111111-0000-0000-0000-000000000030", "Уха"),
    ("11111111-0000-0000-0000-000000000031", "Чебуреки"),
    ("11111111-0000-0000-0000-000000000032", "Бефстроганов"),
    ("11111111-0000-0000-0000-000000000033", "Картофельное пюре"),
    ("11111111-0000-0000-0000-000000000034", "Голубцы"),
    ("11111111-0000-0000-0000-000000000035", "Пирог с яблоками"),
    ("11111111-0000-0000-0000-000000000036", "Тыквенный суп-пюре"),
    ("11111111-0000-0000-0000-000000000037", "Котлеты из индейки"),
    ("11111111-0000-0000-0000-000000000038", "Ризотто с грибами"),
    ("11111111-0000-0000-0000-000000000039", "Чечевичный суп"),
    ("11111111-0000-0000-0000-000000000040", "Омлет с овощами"),
    ("11111111-0000-0000-0000-000000000041", "Баклажаны по-грузински"),
    ("11111111-0000-0000-0000-000000000042", "Пирожки с капустой"),
    ("11111111-0000-0000-0000-000000000043", "Стейк из говядины"),
    ("11111111-0000-0000-0000-000000000044", "Морковный торт"),
    ("11111111-0000-0000-0000-000000000045", "Суп из чечевицы с копчёностями"),
    ("11111111-0000-0000-0000-000000000046", "Курица терияки"),
    ("11111111-0000-0000-0000-000000000047", "Пшённая каша с тыквой"),
    ("11111111-0000-0000-0000-000000000048", "Сельдь под шубой"),
    ("11111111-0000-0000-0000-000000000049", "Брускетта с томатами"),
    ("11111111-0000-0000-0000-000000000050", "Запечённая утка с яблоками"),
    ("11111111-0000-0000-0000-000000000051", "Паэлья с морепродуктами"),
    ("11111111-0000-0000-0000-000000000052", "Кролик в вине"),
    ("11111111-0000-0000-0000-000000000053", "Овощное рагу"),
    ("11111111-0000-0000-0000-000000000054", "Тирамису"),
    ("11111111-0000-0000-0000-000000000055", "Минестроне"),
    ("11111111-0000-0000-0000-000000000056", "Шаурма"),
    ("11111111-0000-0000-0000-000000000057", "Форель на гриле"),
    ("11111111-0000-0000-0000-000000000058", "Пирог с вишней"),
    ("11111111-0000-0000-0000-000000000059", "Суши роллы"),
    ("11111111-0000-0000-0000-000000000060", "Торт Наполеон"),
]

PHOTOS_DIR = Path(__file__).parent.parent / "apps" / "seed" / "photos"
RESULTS_FILE = Path(__file__).parent / "seed-photos-result.json"

MIN_FILE_SIZE = 30 * 1024
MAX_FILE_SIZE = 15 * 1024 * 1024

HEADERS = {
    "User-Agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 "
                  "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
    "Accept-Language": "ru-RU,ru;q=0.9,en;q=0.8",
}

SESSION = requests.Session()
SESSION.headers.update(HEADERS)


def search_recipe_url(title):
    """Ищет рецепт на povar.ru, возвращает URL первого результата или None."""
    search_url = "https://povar.ru/xmlsearch?query=" + requests.utils.quote(title)
    try:
        resp = SESSION.get(search_url, timeout=15)
        resp.raise_for_status()
    except Exception as e:
        print("  [ERROR] Поиск {}: {}".format(title, e))
        return None

    soup = BeautifulSoup(resp.text, "html.parser")

    # Результаты поиска: ссылки вида /recipes/...
    for a in soup.select("a[href]"):
        href = a["href"]
        if "/recipes/" in href and href.endswith(".html") and "#" not in href:
            if href.startswith("http"):
                return href
            return "https://povar.ru" + href

    return None


def get_og_image(recipe_url):
    """Возвращает URL og:image со страницы рецепта или None."""
    try:
        resp = SESSION.get(recipe_url, timeout=15)
        resp.raise_for_status()
    except Exception as e:
        print("  [ERROR] Страница {}: {}".format(recipe_url[:80], e))
        return None

    soup = BeautifulSoup(resp.text, "html.parser")

    tag = soup.find("meta", property="og:image")
    if tag and tag.get("content"):
        return tag["content"]

    # Запасной вариант: первое большое изображение рецепта
    for img in soup.select("img[src]"):
        src = img.get("src", "")
        if "povar.ru" in src and ("/img/" in src or "/upload/" in src):
            return src

    return None


def download_photo(url, dest_path):
    """Скачивает фото по URL, сохраняет в dest_path. Возвращает True при успехе."""
    try:
        resp = SESSION.get(url, timeout=30, stream=True)
        resp.raise_for_status()
        body = resp.content
    except Exception as e:
        print("  [ERROR] Download: {}".format(e))
        return False

    if len(body) < MIN_FILE_SIZE:
        print("  [SKIP] Слишком маленький: {} байт".format(len(body)))
        return False
    if len(body) > MAX_FILE_SIZE:
        print("  [SKIP] Слишком большой: {} байт".format(len(body)))
        return False

    dest_path.write_bytes(body)
    return True


def detect_ext(url):
    """Определяет расширение файла по URL."""
    url_lower = url.lower().split("?")[0]
    if url_lower.endswith(".png"):
        return ".png"
    if url_lower.endswith(".webp"):
        return ".webp"
    return ".jpeg"


def clear_photos_dir():
    """Удаляет все файлы из папки с фото."""
    if PHOTOS_DIR.exists():
        shutil.rmtree(PHOTOS_DIR)
    PHOTOS_DIR.mkdir(parents=True, exist_ok=True)
    print("Папка {} очищена".format(PHOTOS_DIR))


def main():
    # Очищаем старые фото и результаты
    clear_photos_dir()
    RESULTS_FILE.write_text(json.dumps({}, ensure_ascii=False, indent=2))
    print("Результаты сброшены\n")

    results = {}

    for recipe_id, title in RECIPES:
        print("[{}] Ищу на povar.ru...".format(title))

        recipe_url = search_recipe_url(title)
        if not recipe_url:
            print("  [FAIL] Рецепт не найден в поиске")
            time.sleep(0.5)
            continue

        print("  URL: {}".format(recipe_url))

        photo_url = get_og_image(recipe_url)
        if not photo_url:
            print("  [FAIL] og:image не найден")
            time.sleep(0.5)
            continue

        ext = detect_ext(photo_url)
        photo_id = str(uuid.uuid4())
        dest = PHOTOS_DIR / "{}{}".format(photo_id, ext)

        print("  Фото: {}...".format(photo_url[:80]))
        if download_photo(photo_url, dest):
            results[recipe_id] = photo_id
            RESULTS_FILE.write_text(json.dumps(results, ensure_ascii=False, indent=2))
            size_kb = dest.stat().st_size // 1024
            print("  [OK] {} ({}KB)".format(dest.name, size_kb))
        else:
            print("  [FAIL] Не удалось скачать")

        time.sleep(0.5)

    print("\n" + "=" * 60)
    print("// Вставьте в SeedData.cs:")
    print("public static readonly (Guid RecipeId, Guid PhotoId)[] RecipePhotoSeeds =")
    print("[")
    for rid, pid in results.items():
        print('    (new Guid("{}"), new Guid("{}")),'.format(rid, pid))
    print("];")
    print("=" * 60)
    print("\nВсего скачано: {}/{} фото".format(len(results), len(RECIPES)))


if __name__ == "__main__":
    main()
