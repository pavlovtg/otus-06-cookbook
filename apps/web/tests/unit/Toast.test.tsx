import { render, screen, act } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Toast, ToastStack, useToasts } from "@/components/ui/Toast";
import { renderHook } from "@testing-library/react";

describe("Toast", () => {
  it("рендерит сообщение", () => {
    render(<Toast item={{ id: "1", message: "Сохранено" }} />);
    expect(screen.getByText("Сохранено")).toBeInTheDocument();
  });

  it("применяет класс toast-success для kind=success", () => {
    const { container } = render(
      <Toast item={{ id: "1", message: "OK", kind: "success" }} />
    );
    expect(container.querySelector(".toast-success")).toBeInTheDocument();
  });

  it("применяет класс toast-error для kind=error", () => {
    const { container } = render(
      <Toast item={{ id: "1", message: "Ошибка", kind: "error" }} />
    );
    expect(container.querySelector(".toast-error")).toBeInTheDocument();
  });

  it("применяет класс toast-default для kind=default", () => {
    const { container } = render(
      <Toast item={{ id: "1", message: "Инфо", kind: "default" }} />
    );
    expect(container.querySelector(".toast-default")).toBeInTheDocument();
  });

  it("не применяет kind-класс если kind не передан", () => {
    const { container } = render(
      <Toast item={{ id: "1", message: "Инфо" }} />
    );
    // только классы toast и is-show
    const el = container.querySelector(".toast")!;
    expect(el.className).toBe("toast is-show");
  });

  it("всегда имеет класс is-show", () => {
    const { container } = render(
      <Toast item={{ id: "1", message: "Инфо" }} />
    );
    expect(container.querySelector(".is-show")).toBeInTheDocument();
  });
});

describe("ToastStack", () => {
  it("рендерит несколько тостов", () => {
    render(
      <ToastStack
        items={[
          { id: "1", message: "Первый" },
          { id: "2", message: "Второй" },
        ]}
      />
    );
    expect(screen.getByText("Первый")).toBeInTheDocument();
    expect(screen.getByText("Второй")).toBeInTheDocument();
  });

  it("рендерит пустой стек", () => {
    const { container } = render(<ToastStack items={[]} />);
    expect(container.querySelector(".toast-stack")).toBeInTheDocument();
    expect(container.querySelector(".toast")).not.toBeInTheDocument();
  });
});

describe("useToasts", () => {
  it("начинает с пустым массивом", () => {
    const { result } = renderHook(() => useToasts());
    expect(result.current.toasts).toHaveLength(0);
  });

  it("добавляет тост при вызове push", () => {
    const { result } = renderHook(() => useToasts());
    act(() => {
      result.current.push("Привет");
    });
    expect(result.current.toasts).toHaveLength(1);
    expect(result.current.toasts[0]?.message).toBe("Привет");
  });

  it("добавляет тост с kind=success", () => {
    const { result } = renderHook(() => useToasts());
    act(() => {
      result.current.push("Сохранено", "success");
    });
    expect(result.current.toasts[0]?.kind).toBe("success");
  });

  it("добавляет тост с kind=default по умолчанию", () => {
    const { result } = renderHook(() => useToasts());
    act(() => {
      result.current.push("Инфо");
    });
    expect(result.current.toasts[0]?.kind).toBe("default");
  });

  it("удаляет тост через 3 секунды", async () => {
    vi.useFakeTimers();
    const { result } = renderHook(() => useToasts());
    act(() => {
      result.current.push("Временный");
    });
    expect(result.current.toasts).toHaveLength(1);

    await act(async () => {
      vi.advanceTimersByTime(3000);
    });

    expect(result.current.toasts).toHaveLength(0);
    vi.useRealTimers();
  });
});
