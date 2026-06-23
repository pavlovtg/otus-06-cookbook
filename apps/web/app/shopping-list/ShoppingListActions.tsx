"use client";

import * as React from "react";
import { CopyIcon, PrintIcon } from "@/lib/icons";
import type { ShoppingListGroup } from "@/lib/schemas/shopping-list";
import { IngredientCategoryLabels, type IngredientCategory } from "@/lib/schemas/ingredient";

interface ShoppingListActionsProps {
  groups: ShoppingListGroup[];
}

export function ShoppingListActions({ groups }: ShoppingListActionsProps) {
  const [copied, setCopied] = React.useState(false);

  function buildText(): string {
    return groups
      .map((g) => {
        const rows = g.items
          .map((item) => `- ${item.title}, ${parseFloat(item.amount.toFixed(2))} ${item.unit}`)
          .join("\n");
        return `${IngredientCategoryLabels[g.category as IngredientCategory] ?? g.category}\n${rows}`;
      })
      .join("\n\n");
  }

  async function handleCopy() {
    try {
      await navigator.clipboard.writeText(buildText());
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      // ignore
    }
  }

  function handlePrint() {
    window.print();
  }

  return (
    <>
      <button className="btn btn-ghost" onClick={handleCopy}>
        <CopyIcon size={14} />
        {copied ? "Скопировано" : "Скопировать"}
      </button>
      <button className="btn btn-primary" onClick={handlePrint}>
        <PrintIcon size={14} />
        Распечатать
      </button>
    </>
  );
}
