# C4 Context — Cookbook

Источник: ADR-0008

## Описание

Системный контекст «Книги рецептов». Единственный внешний актор — конечный пользователь в браузере; внешних интегрируемых систем у MVP нет.

## Диаграмма

```plantuml
@startuml
title C4 L1 — System Context: Cookbook

skinparam rectangle {
  BackgroundColor<<system>> #1168BD
  FontColor<<system>> white
  BackgroundColor<<person>> #08427B
  FontColor<<person>> white
}

rectangle "Домашний повар" as user <<person>>
rectangle "Cookbook" as cookbook <<system>>

user --> cookbook : [HTTPS]
@enduml
```
