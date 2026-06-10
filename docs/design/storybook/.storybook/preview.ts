import type { Preview } from '@storybook/react';
import '../src/styles/tokens.css';
import '../src/styles/components.css';

const preview: Preview = {
  parameters: {
    layout: 'padded',
    controls: { matchers: { color: /(background|color)$/i, date: /Date$/i } },
    backgrounds: {
      default: 'app',
      values: [
        { name: 'app', value: '#fafaf7' },
        { name: 'surface', value: '#ffffff' },
        { name: 'dark', value: '#1f1f1f' },
      ],
    },
  },
};
export default preview;
