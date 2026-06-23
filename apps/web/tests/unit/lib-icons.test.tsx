import { render } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import {
  BookIcon,
  LeafIcon,
  LayersIcon,
  CalendarIcon,
  CopyIcon,
  PrintIcon,
  CartIcon,
} from "@/lib/icons";

const icons = [
  ["BookIcon", BookIcon],
  ["LeafIcon", LeafIcon],
  ["LayersIcon", LayersIcon],
  ["CalendarIcon", CalendarIcon],
  ["CopyIcon", CopyIcon],
  ["PrintIcon", PrintIcon],
  ["CartIcon", CartIcon],
] as const;

describe("lib/icons", () => {
  it.each(icons)("%s рендерит SVG", (_name, Icon) => {
    const { container } = render(<Icon />);
    expect(container.querySelector("svg")).toBeInTheDocument();
  });

  it.each(icons)("%s принимает size prop", (_name, Icon) => {
    const { container } = render(<Icon size={20} />);
    const svg = container.querySelector("svg")!;
    expect(svg.getAttribute("width")).toBe("20");
    expect(svg.getAttribute("height")).toBe("20");
  });
});
