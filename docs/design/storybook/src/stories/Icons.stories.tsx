import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import * as Icons from '../icons';

const meta: Meta = { title: 'Foundation/Icons' };
export default meta;

export const All: StoryObj = {
  render: () => {
    const entries = Object.entries(Icons).filter(([n]) => n.endsWith('Icon'));
    return (
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(6, 1fr)', gap: 12 }}>
        {entries.map(([name, Comp]: any) => (
          <div
            key={name}
            style={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              gap: 8,
              padding: 14,
              background: '#17181c',
              borderRadius: 8,
              color: '#fff',
              boxShadow: 'inset 0 0 17px rgba(255,255,255,0.06)',
            }}
          >
            <Comp size={20} />
            <span className="t-micro" style={{ textAlign: 'center' }}>{name.replace('Icon', '')}</span>
          </div>
        ))}
      </div>
    );
  },
};
