import "@testing-library/jest-dom";
import { vi } from "vitest";

// `server-only` бросает в браузерном/тестовом окружении; в Vitest подменяем no-op.
vi.mock("server-only", () => ({}));
