import { notFound } from "next/navigation";
import { getRecipe } from "@/lib/bff/gateway";
import { EditRecipeForm } from "./EditRecipeForm";

interface Props {
  params: Promise<{ id: string }>;
}

export default async function EditRecipePage({ params }: Props) {
  const { id } = await params;

  let recipe;
  try {
    recipe = await getRecipe(id);
  } catch {
    notFound();
  }

  return (
    <>
      <div className="page-head">
        <div className="left">
          <h1 className="t-heading">Редактировать рецепт</h1>
          <p className="t-small">{recipe.title}</p>
        </div>
      </div>
      <EditRecipeForm recipe={recipe} />
    </>
  );
}
