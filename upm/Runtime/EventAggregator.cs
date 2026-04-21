using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit {}
}

namespace EventWeave
{
	public sealed class EventAggregator : IEventAggregator
	{
		readonly object _lock = new();
		
		readonly Dictionary<Type, List<Subscription>> _subscriptions = new();

		/// <inheritdoc />
		public void Subscribe<TMessage>(object subscriber, Action<TMessage> action) where TMessage : class
		{
			Subscription subscription = new(new WeakReference(subscriber), message => action((TMessage)message));

			lock (_lock)
			{
				if (!_subscriptions.TryGetValue(typeof(TMessage), out List<Subscription>? subs))
				{
					subs = new List<Subscription>();
					_subscriptions[typeof(TMessage)] = subs;
				}

				subs.Add(subscription);
			}
		}

		/// <inheritdoc />
		public void Unsubscribe<TMessage>(object subscriber) where TMessage : class
		{
			lock (_lock)
			{
				foreach (List<Subscription> subs in _subscriptions.Values)
				{
					subs.RemoveAll(s => !s.Subscriber.IsAlive || s.Subscriber.Target == subscriber);
				}
			}
		}

		/// <inheritdoc />
		public void Publish<TMessage>(TMessage message, [CallerMemberName] string? caller = null,
			[CallerFilePath] string? file = null)
			where TMessage : class
		{
			List<Action<object>> handlers;

			lock (_lock)
			{
				if (!_subscriptions.TryGetValue(typeof(TMessage), out List<Subscription>? subs))
					return;

				// Clean dead refs and snapshot
				subs.RemoveAll(s => !s.Subscriber.IsAlive);
				handlers = subs.Select(s => s.Handler).ToList();
			}

			// Invoke outside lock
			foreach (Action<object> handler in handlers)
			{
				handler(message);
			}
		}

		sealed record Subscription(WeakReference Subscriber, Action<object> Handler);
	}
}