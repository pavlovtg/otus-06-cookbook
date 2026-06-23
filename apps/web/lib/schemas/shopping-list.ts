import { z } from "zod";

export const ShoppingListItemSchema = z.object({
  ingredientId: z.string().uuid(),
  title: z.string().min(1),
  amount: z.number(),
  unit: z.string(),
});

export type ShoppingListItem = z.infer<typeof ShoppingListItemSchema>;

export const ShoppingListGroupSchema = z.object({
  category: z.string().min(1),
  items: z.array(ShoppingListItemSchema),
});

export type ShoppingListGroup = z.infer<typeof ShoppingListGroupSchema>;
