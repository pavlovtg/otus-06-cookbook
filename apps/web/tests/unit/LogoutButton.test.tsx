import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";

vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
    refresh: vi.fn(),
  }),
}));

vi.mock("@/lib/bff/auth", () => ({
  logout: vi.fn().mockResolvedValue(undefined),
}));

import { LogoutButton } from "@/components/features/LogoutButton";
import { logout } from "@/lib/bff/auth";

describe("LogoutButton", () => {
  it("рендерит кнопку «Выйти»", () => {
    render(<LogoutButton />);
    expect(screen.getByText("Выйти")).toBeInTheDocument();
  });

  it("вызывает logout при клике", async () => {
    render(<LogoutButton />);
    fireEvent.click(screen.getByText("Выйти"));
    await waitFor(() => {
      expect(logout).toHaveBeenCalledOnce();
    });
  });
});
