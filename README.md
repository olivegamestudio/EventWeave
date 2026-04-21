# EventWeave

Lightweight event aggregation for .NET. Zero dependencies. Decoupled pub/sub messaging between services.

## Why

When services need to communicate without knowing about each other, you need a message bus. EventWeave provides a minimal `IEventAggregator` that handles subscription, publication, and cleanup — with weak references to prevent memory leaks.

No frameworks. No reflection. No magic. Just pub/sub.

## Install

```
dotnet add package EventWeave
dotnet add package EventWeave.CommunityToolkit
```

## Install into Unity

In `Package Manager` add the following git url:

```
https://github.com/olivegamestudio/EventWeave.git?path=/upm
```

## Quick start

Define a message as a record:

```csharp
public sealed record OrderPlacedMessage(int OrderId, decimal Total);
```

Subscribe:

```csharp
public sealed class NotificationService
{
    public NotificationService(IEventAggregator events)
    {
        events.Subscribe<OrderPlacedMessage>(this, OnOrderPlaced);
    }

    void OnOrderPlaced(OrderPlacedMessage message)
    {
        // send notification
    }
}
```

Publish:

```csharp
public sealed class OrderService
{
    readonly IEventAggregator _events;

    public OrderService(IEventAggregator events)
    {
        _events = events;
    }

    public void PlaceOrder(int orderId, decimal total)
    {
        // process order
        _events.Publish(new OrderPlacedMessage(orderId, total));
    }
}
```

Unsubscribe when done:

```csharp
events.Unsubscribe(this);
```

## Setup

### Standalone (no DI)

```csharp
IEventAggregator events = new EventAggregator();
```

### Microsoft DI

```csharp
services.AddSingleton<IEventAggregator, EventAggregator>();
```

### CommunityToolkit.Mvvm adapter

If you already use `IMessenger` in your presentation layer and want EventWeave in your application layer:

```
dotnet add package EventWeave.CommunityToolkit
```

```csharp
services.AddSingleton<IMessenger, WeakReferenceMessenger>();
services.AddSingleton<IEventAggregator, MessengerEventAggregator>();
```

## Diagnostics

`Publish` automatically captures caller information for debugging:

```csharp
public interface IEventAggregator
{
    void Publish<TMessage>(
        TMessage message,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? file = null)
        where TMessage : class;
}
```

## Packages

| Package | Purpose |
|---|---|
| `EventWeave` | Core library. `IEventAggregator`, `EventAggregator`. Zero dependencies, netstandard2.0. |
| `EventWeave.CommunityToolkit` | Adapter `MessengerEventAggregator` wrapping `IMessenger` from CommunityToolkit.Mvvm. |

## Design principles

- **Zero dependencies** in the core package. Runs on .NET Framework 4.6.2, .NET 10, Unity, MonoGame — anywhere netstandard2.0 works.
- **Thread safe.** Subscriptions and publications are safe to call from any thread.
- **No reflection.** All wiring is explicit or source-generated.
- **Messages are records.** Immutable, equatable, pattern-matchable.

## License

MIT
