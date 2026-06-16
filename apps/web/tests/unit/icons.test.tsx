import { render } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import {
  SearchIcon,
  ClockIcon,
  FlameIcon,
  UserIcon,
  HeartIcon,
  HeartFillIcon,
  StarIcon,
  StarOIcon,
  PlusIcon,
  MinusIcon,
  XIcon,
  TrashIcon,
  EditIcon,
  ArrowLeftIcon,
  CopyIcon,
  PrintIcon,
  BookIcon,
  CalendarIcon,
  CartIcon,
  LeafIcon,
  LayersIcon,
  ChartIcon,
  LockIcon,
} from "@/components/icons";

const icons = [
  ["SearchIcon", SearchIcon],
  ["ClockIcon", ClockIcon],
  ["FlameIcon", FlameIcon],
  ["UserIcon", UserIcon],
  ["HeartIcon", HeartIcon],
  ["HeartFillIcon", HeartFillIcon],
  ["StarIcon", StarIcon],
  ["StarOIcon", StarOIcon],
  ["PlusIcon", PlusIcon],
  ["MinusIcon", MinusIcon],
  ["XIcon", XIcon],
  ["TrashIcon", TrashIcon],
  ["EditIcon", EditIcon],
  ["ArrowLeftIcon", ArrowLeftIcon],
  ["CopyIcon", CopyIcon],
  ["PrintIcon", PrintIcon],
  ["BookIcon", BookIcon],
  ["CalendarIcon", CalendarIcon],
  ["CartIcon", CartIcon],
  ["LeafIcon", LeafIcon],
  ["LayersIcon", LayersIcon],
  ["ChartIcon", ChartIcon],
  ["LockIcon", LockIcon],
] as const;

describe("icons", () => {
  it.each(icons)("%s рендерит SVG", (_name, Icon) => {
    const { container } = render(<Icon />);
    expect(container.querySelector("svg")).toBeInTheDocument();
  });

  it.each(icons)("%s принимает size prop", (_name, Icon) => {
    const { container } = render(<Icon size={24} />);
    const svg = container.querySelector("svg")!;
    expect(svg.getAttribute("width")).toBe("24");
    expect(svg.getAttribute("height")).toBe("24");
  });
});
