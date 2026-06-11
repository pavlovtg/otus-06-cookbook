import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';

const meta: Meta = { title: 'Foundation/Tokens' };
export default meta;

type S = StoryObj;

const Swatch = ({ name, value }: { name: string; value: string }) => (
  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
    <div style={{ width: 56, height: 56, borderRadius: 8, background: value, boxShadow: 'inset 0 0 17px rgba(255,255,255,0.06)' }} />
    <div style={{ display: 'flex', flexDirection: 'column' }}>
      <span style={{ color: '#fff', fontWeight: 500 }}>{name}</span>
      <span className="t-micro">{value}</span>
    </div>
  </div>
);

export const Palette: S = {
  render: () => (
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 14 }}>
      <Swatch name="bg/canvas" value="#0b0b15" />
      <Swatch name="bg/canvas-deep" value="#06060f" />
      <Swatch name="bg/surface-1" value="#0c0c16" />
      <Swatch name="bg/surface-2" value="#17181c" />
      <Swatch name="bg/surface-3" value="#252537" />
      <Swatch name="fg/primary" value="#ffffff" />
      <Swatch name="fg/secondary" value="#d4d4d4" />
      <Swatch name="fg/muted" value="#a9a9ac" />
      <Swatch name="accent/purple" value="#9968ff" />
      <Swatch name="accent/pink" value="#ff6cb2" />
      <Swatch name="accent/orange" value="#ffaf56" />
      <Swatch name="data/up" value="#76d9a3" />
      <Swatch name="danger" value="#f47272" />
    </div>
  ),
};

export const Typography: S = {
  render: () => (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div className="t-hero">Готовим <span className="t-gradient">сегодня</span></div>
      <div className="t-display">Display 40 / Medium</div>
      <div className="t-heading">Heading 28 / Medium</div>
      <div className="t-subheading">Subheading 20 / Medium</div>
      <div className="t-body">Body 16 / Regular — lorem ipsum dolor sit amet</div>
      <div className="t-body-medium">Body 16 / Medium — кнопки и активный таб</div>
      <div className="t-small">Small 14 — лейблы, deep secondary</div>
      <div className="t-micro">Micro 12 — метки на чартах</div>
    </div>
  ),
};

export const Radii: S = {
  render: () => (
    <div style={{ display: 'flex', gap: 16, alignItems: 'flex-end' }}>
      {[
        { name: 'pill (100px)', r: 100 },
        { name: 'card (8px)', r: 8 },
        { name: 'card-lg (12px)', r: 12 },
      ].map((x) => (
        <div key={x.name} style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
          <div style={{ width: 120, height: 80, background: '#17181c', borderRadius: x.r, boxShadow: 'inset 0 0 17px rgba(255,255,255,0.06), 0 0 19px rgba(0,0,0,0.06)' }} />
          <span className="t-small">{x.name}</span>
        </div>
      ))}
    </div>
  ),
};
