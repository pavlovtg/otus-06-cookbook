import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Modal } from "@/components/ui/Modal";

describe("Modal", () => {
  it("рендерит children когда open=true", () => {
    render(
      <Modal open onClose={() => {}}>
        <p>Содержимое</p>
      </Modal>
    );
    expect(screen.getByText("Содержимое")).toBeInTheDocument();
  });

  it("рендерит title", () => {
    render(
      <Modal open onClose={() => {}} title="Заголовок">
        <p>Содержимое</p>
      </Modal>
    );
    expect(screen.getByText("Заголовок")).toBeInTheDocument();
  });

  it("рендерит footer", () => {
    render(
      <Modal open onClose={() => {}} footer={<button>Сохранить</button>}>
        <p>Содержимое</p>
      </Modal>
    );
    expect(screen.getByText("Сохранить")).toBeInTheDocument();
  });

  it("не рендерит footer если не передан", () => {
    const { container } = render(
      <Modal open onClose={() => {}}>
        <p>Содержимое</p>
      </Modal>
    );
    expect(container.querySelector(".form-actions")).not.toBeInTheDocument();
  });

  it("вызывает onClose при клике на кнопку закрытия", () => {
    const onClose = vi.fn();
    render(
      <Modal open onClose={onClose}>
        <p>Содержимое</p>
      </Modal>
    );
    fireEvent.click(screen.getByRole("button", { name: "Закрыть" }));
    expect(onClose).toHaveBeenCalledOnce();
  });

  it("вызывает onClose при нажатии Escape", () => {
    const onClose = vi.fn();
    render(
      <Modal open onClose={onClose}>
        <p>Содержимое</p>
      </Modal>
    );
    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).toHaveBeenCalledOnce();
  });

  it("не вызывает onClose при нажатии других клавиш", () => {
    const onClose = vi.fn();
    render(
      <Modal open onClose={onClose}>
        <p>Содержимое</p>
      </Modal>
    );
    fireEvent.keyDown(document, { key: "Enter" });
    expect(onClose).not.toHaveBeenCalled();
  });

  it("не добавляет обработчик Escape когда open=false", () => {
    const onClose = vi.fn();
    render(
      <Modal open={false} onClose={onClose}>
        <p>Содержимое</p>
      </Modal>
    );
    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).not.toHaveBeenCalled();
  });

  it("применяет modal-lg для size=lg", () => {
    const { container } = render(
      <Modal open onClose={() => {}} size="lg">
        <p>Содержимое</p>
      </Modal>
    );
    expect(container.querySelector(".modal-lg")).toBeInTheDocument();
  });

  it("не применяет modal-lg для size=md", () => {
    const { container } = render(
      <Modal open onClose={() => {}} size="md">
        <p>Содержимое</p>
      </Modal>
    );
    expect(container.querySelector(".modal-lg")).not.toBeInTheDocument();
  });

  it("добавляет is-open на backdrop когда open=true", () => {
    const { container } = render(
      <Modal open onClose={() => {}}>
        <p>Содержимое</p>
      </Modal>
    );
    expect(container.querySelector(".modal-backdrop")).toHaveClass("is-open");
  });

  it("не добавляет is-open на backdrop когда open=false", () => {
    const { container } = render(
      <Modal open={false} onClose={() => {}}>
        <p>Содержимое</p>
      </Modal>
    );
    expect(container.querySelector(".modal-backdrop")).not.toHaveClass("is-open");
  });

  it("вызывает onClose при клике на backdrop", () => {
    const onClose = vi.fn();
    const { container } = render(
      <Modal open onClose={onClose}>
        <p>Содержимое</p>
      </Modal>
    );
    const backdrop = container.querySelector(".modal-backdrop")!;
    fireEvent.click(backdrop);
    expect(onClose).toHaveBeenCalledOnce();
  });
});
