# AR-0037: Репозитории реализуют IUnitOfWorkRepository; коммит — из Application

Источник: ADR-0032

## Правило

Каждый репозиторий реализует `IUnitOfWorkRepository` с методом `CommitAsync(CancellationToken)`. Реализация делегирует вызов `DbContext.SaveChangesAsync`. Репозитории не вызывают `SaveChangesAsync` самостоятельно ни в каком другом методе. Application-сервис явно вызывает `CommitAsync` в конце каждого метода, изменяющего состояние.

## Запрещено

- Вызывать `SaveChangesAsync` внутри методов репозитория (кроме `CommitAsync`).
- Управлять транзакцией из контроллера или инфраструктурного слоя.
- Использовать `TransactionScope` с async/await.
