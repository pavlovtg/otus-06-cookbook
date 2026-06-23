export const dynamic = "force-dynamic";

import { redirect } from "next/navigation";
import Link from "next/link";
import { getSession } from "@/lib/session";
import { getShoppingList } from "@/lib/bff/shopping-list.server";
import { ShoppingListActions } from "./ShoppingListActions";
import logger from "@/lib/logger";
import type { ShoppingListGroup } from "@/lib/schemas/shopping-list";

export default async function ShoppingListPage() {
  const session = await getSession();
  if (!session.user) {
    redirect("/login");
  }

  let groups: ShoppingListGroup[] = [];
  try {
    groups = await getShoppingList();
  } catch (err) {
    logger.error({ err }, "Failed to load shopping list");
  }

  const isEmpty = groups.length === 0;

  return (
    <div className="main">
      <div className="page-head">
        <div className="left">
          <h1 className="t-heading">
            Список <span className="t-gradient">покупок</span>
          </h1>
        </div>
        {!isEmpty && (
          <div style={{ display: "flex", gap: "8px" }}>
            <ShoppingListActions groups={groups} />
          </div>
        )}
      </div>

      {isEmpty ? (
        <div className="state">
          <p>Добавьте блюда в планировщик</p>
          <Link href="/planner" className="btn btn-ghost">
            Перейти в планировщик
          </Link>
        </div>
      ) : (
        <div className="shopping-table">
          {groups.map((group) => (
            <div key={group.category}>
              <div className="shopping-group-head">{group.category}</div>
              {group.items.map((item) => (
                <div key={item.ingredientId} className="shopping-row">
                  <span>{item.title}</span>
                  <span className="qty">{item.amount}</span>
                  <span className="unit">{item.unit}</span>
                </div>
              ))}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
