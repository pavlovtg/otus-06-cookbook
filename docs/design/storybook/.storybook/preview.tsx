import type { Preview } from '@storybook/react';
import React from 'react';
import '../src/styles.css';
import '../src/motion.css';

const preview: Preview = {
  parameters: {
    backgrounds: {
      default: 'canvas',
      values: [
        { name: 'canvas', value: '#0b0b15' },
        { name: 'deep', value: '#06060f' },
        { name: 'surface', value: '#17181c' },
      ],
    },
    layout: 'padded',
    controls: { matchers: { color: /(background|color)$/i, date: /Date$/i } },
  },
  decorators: [
    (Story) => (
      <div style={{ minWidth: 320, padding: 24, color: '#d4d4d4', fontFamily: 'Inter, sans-serif' }}>
        <Story />
      </div>
    ),
  ],
};

export default preview;
