# syntax=docker/dockerfile:1.7
FROM node:20-alpine

RUN corepack enable && corepack prepare pnpm@9.15.0 --activate

WORKDIR /app

COPY package.json pnpm-lock.yaml* ./
RUN pnpm install --frozen-lockfile || pnpm install

COPY . .

EXPOSE 6006

CMD ["pnpm", "storybook", "--host", "0.0.0.0", "--no-open"]
