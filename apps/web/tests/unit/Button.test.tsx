import { render, screen, fireEvent, act } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Button, IconButton, AsyncButton } from "@/components/ui/Button";

describe("Button", () => {
  it("рендерит children", () => {
    render(<Button>Сохранить</Button>);
    expect(screen.getByText("Сохранить")).toBeInTheDocument();
  });

  it("применяет класс btn-primary по умолчанию", () => {
    render(<Button>OK</Button>);
    expect(screen.getByRole("button")).toHaveClass("btn-primary");
  });

  it("применяет класс btn-ghost для variant=ghost", () => {
    render(<Button variant="ghost">OK</Button>);
    expect(screen.getByRole("button")).toHaveClass("btn-ghost");
  });

  it("применяет класс btn-danger для variant=danger", () => {
    render(<Button variant="danger">OK</Button>);
    expect(screen.getByRole("button")).toHaveClass("btn-danger");
  });

  it("применяет класс btn-sm для size=sm", () => {
    render(<Button size="sm">OK</Button>);
    expect(screen.getByRole("button")).toHaveClass("btn-sm");
  });

  it("не применяет btn-sm для size=md", () => {
    render(<Button size="md">OK</Button>);
    expect(screen.getByRole("button")).not.toHaveClass("btn-sm");
  });

  it("рендерит icon и trailingIcon", () => {
    render(
      <Button icon={<span data-testid="icon" />} trailingIcon={<span data-testid="trailing" />}>
        OK
      </Button>
    );
    expect(screen.getByTestId("icon")).toBeInTheDocument();
    expect(screen.getByTestId("trailing")).toBeInTheDocument();
  });

  it("не рендерит span если children == null", () => {
    const { container } = render(<Button icon={<span />} />);
    expect(container.querySelector("span")).toBeInTheDocument(); // icon span
    // нет вложенного span с текстом
    expect(container.querySelectorAll("span").length).toBe(1);
  });

  it("передаёт type=submit", () => {
    render(<Button type="submit">OK</Button>);
    expect(screen.getByRole("button")).toHaveAttribute("type", "submit");
  });

  it("передаёт дополнительные props (disabled)", () => {
    render(<Button disabled>OK</Button>);
    expect(screen.getByRole("button")).toBeDisabled();
  });
});

describe("IconButton", () => {
  it("рендерит aria-label", () => {
    render(<IconButton label="Удалить">X</IconButton>);
    expect(screen.getByRole("button", { name: "Удалить" })).toBeInTheDocument();
  });

  it("применяет btn-icon по умолчанию", () => {
    render(<IconButton label="X">X</IconButton>);
    expect(screen.getByRole("button")).toHaveClass("btn-icon");
  });

  it("применяет btn-icon-sm для size=sm", () => {
    render(<IconButton label="X" size="sm">X</IconButton>);
    expect(screen.getByRole("button")).toHaveClass("btn-icon-sm");
  });

  it("применяет is-on для active=true", () => {
    render(<IconButton label="X" active>X</IconButton>);
    expect(screen.getByRole("button")).toHaveClass("is-on");
  });

  it("не применяет is-on для active=false", () => {
    render(<IconButton label="X" active={false}>X</IconButton>);
    expect(screen.getByRole("button")).not.toHaveClass("is-on");
  });

  it("применяет is-danger для tone=danger", () => {
    render(<IconButton label="X" tone="danger">X</IconButton>);
    expect(screen.getByRole("button")).toHaveClass("is-danger");
  });

  it("не применяет is-danger для tone=default", () => {
    render(<IconButton label="X" tone="default">X</IconButton>);
    expect(screen.getByRole("button")).not.toHaveClass("is-danger");
  });
});

describe("AsyncButton", () => {
  it("рендерит children в idle состоянии", () => {
    render(<AsyncButton onAction={async () => {}}>Отправить</AsyncButton>);
    expect(screen.getByText("Отправить")).toBeInTheDocument();
  });

  it("показывает 'Загрузка' во время выполнения", async () => {
    let resolve!: () => void;
    const promise = new Promise<void>((r) => { resolve = r; });
    render(<AsyncButton onAction={() => promise}>Отправить</AsyncButton>);

    fireEvent.click(screen.getByRole("button"));
    expect(screen.getByText("Загрузка")).toBeInTheDocument();
    expect(screen.getByRole("button")).toBeDisabled();

    await act(async () => { resolve(); await promise; });
  });

  it("показывает '✓ Готово' после успешного выполнения", async () => {
    render(<AsyncButton onAction={async () => {}}>Отправить</AsyncButton>);

    await act(async () => {
      fireEvent.click(screen.getByRole("button"));
    });

    expect(screen.getByText("✓ Готово")).toBeInTheDocument();
  });

  it("возвращается в idle при ошибке", async () => {
    render(
      <AsyncButton onAction={async () => { throw new Error("fail"); }}>
        Отправить
      </AsyncButton>
    );

    await act(async () => {
      fireEvent.click(screen.getByRole("button"));
    });

    expect(screen.getByText("Отправить")).toBeInTheDocument();
  });
});
