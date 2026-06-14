"use client";

import { useRef, useTransition } from "react";
import { useRouter } from "next/navigation";
import { uploadRecipePhoto, deleteRecipePhoto } from "@/lib/bff/recipes";

interface RecipePhotoActionsProps {
  recipeId: string;
  photoId: string | null;
}

export function RecipePhotoActions({
  recipeId,
  photoId,
}: RecipePhotoActionsProps) {
  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [isPending, startTransition] = useTransition();

  function handleUploadClick() {
    fileInputRef.current?.click();
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;

    startTransition(async () => {
      try {
        await uploadRecipePhoto(recipeId, file);
        router.refresh();
      } catch (err) {
        console.error("Ошибка загрузки фото:", err);
      }
    });

    e.target.value = "";
  }

  function handleDelete() {
    if (!photoId) return;
    if (!confirm("Удалить фото рецепта?")) return;

    startTransition(async () => {
      try {
        await deleteRecipePhoto(recipeId, photoId);
        router.refresh();
      } catch (err) {
        console.error("Ошибка удаления фото:", err);
      }
    });
  }

  return (
    <div className="photo-actions" style={{ display: "flex", gap: 8, marginTop: 8 }}>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png"
        style={{ display: "none" }}
        onChange={handleFileChange}
      />
      {photoId == null ? (
        <button
          className="btn btn-ghost btn-sm"
          onClick={handleUploadClick}
          disabled={isPending}
        >
          {isPending ? "Загрузка…" : "Загрузить фото"}
        </button>
      ) : (
        <>
          <button
            className="btn btn-ghost btn-sm"
            onClick={handleUploadClick}
            disabled={isPending}
          >
            {isPending ? "Загрузка…" : "Заменить фото"}
          </button>
          <button
            className="btn btn-ghost btn-sm"
            onClick={handleDelete}
            disabled={isPending}
          >
            {isPending ? "Удаление…" : "Удалить фото"}
          </button>
        </>
      )}
    </div>
  );
}
