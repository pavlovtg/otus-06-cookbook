import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { RecipePhoto } from '../photo';

const meta: Meta = {
  title: 'Domain/RecipePhotoActions',
  parameters: { layout: 'centered' },
};
export default meta;
type S = StoryObj;

function PhotoBlock({
  photoUrl,
  recipeId = 'r1',
  title = 'Карбонара',
}: {
  photoUrl?: string;
  recipeId?: string;
  title?: string;
}) {
  const [currentUrl, setCurrentUrl] = React.useState<string | undefined>(photoUrl);
  const [isPending, setIsPending] = React.useState(false);
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  function handleUploadClick() {
    fileInputRef.current?.click();
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setIsPending(true);
    setTimeout(() => {
      setCurrentUrl(URL.createObjectURL(file));
      setIsPending(false);
    }, 800);
    e.target.value = '';
  }

  function handleDelete() {
    if (!confirm('Удалить фото рецепта?')) return;
    setIsPending(true);
    setTimeout(() => {
      setCurrentUrl(undefined);
      setIsPending(false);
    }, 500);
  }

  return (
    <div style={{ width: 360 }}>
      <div className="main-photo" style={{ borderRadius: 12, overflow: 'hidden', marginBottom: 8 }}>
        {currentUrl ? (
          <img src={currentUrl} alt={title} style={{ width: '100%', height: 220, objectFit: 'cover', display: 'block' }} />
        ) : (
          <RecipePhoto seed={recipeId} title={title} />
        )}
      </div>
      <div style={{ display: 'flex', gap: 8 }}>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png"
          style={{ display: 'none' }}
          onChange={handleFileChange}
        />
        {currentUrl == null ? (
          <button className="btn btn-ghost btn-sm" onClick={handleUploadClick} disabled={isPending}>
            {isPending ? 'Загрузка…' : 'Загрузить фото'}
          </button>
        ) : (
          <>
            <button className="btn btn-ghost btn-sm" onClick={handleUploadClick} disabled={isPending}>
              {isPending ? 'Загрузка…' : 'Заменить фото'}
            </button>
            <button className="btn btn-ghost btn-sm" onClick={handleDelete} disabled={isPending}>
              {isPending ? 'Удаление…' : 'Удалить фото'}
            </button>
          </>
        )}
      </div>
    </div>
  );
}

export const WithPhoto: S = {
  name: 'С фото — кнопки «Заменить» и «Удалить»',
  render: () => (
    <PhotoBlock
      photoUrl="https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=300&fit=crop"
      title="Карбонара"
    />
  ),
};

export const WithoutPhoto: S = {
  name: 'Без фото — кнопка «Загрузить»',
  render: () => <PhotoBlock title="Борщ" recipeId="r5" />,
};

export const Playground: S = {
  name: 'Playground — интерактивный',
  render: () => (
    <div style={{ display: 'flex', gap: 32, flexWrap: 'wrap' }}>
      <div>
        <p style={{ marginBottom: 8, fontSize: 13, opacity: 0.6 }}>С фото</p>
        <PhotoBlock
          photoUrl="https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=300&fit=crop"
          title="Маргарита"
          recipeId="r3"
        />
      </div>
      <div>
        <p style={{ marginBottom: 8, fontSize: 13, opacity: 0.6 }}>Без фото</p>
        <PhotoBlock title="Шакшука" recipeId="r2" />
      </div>
    </div>
  ),
};
