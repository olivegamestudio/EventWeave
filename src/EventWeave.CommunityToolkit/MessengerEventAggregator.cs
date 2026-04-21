using CommunityToolkit.Mvvm.Messaging;
using System.Runtime.CompilerServices;

namespace EventWeave;

/// <summary>
/// Infrastructure implementation of the <see cref="IEventAggregator"/>, wrapping the <see cref="IMessenger"/> from the Community Toolkit.
/// </summary>
/// <param name="messenger">The underlying messenger used for message dispatch.</param>
public sealed class MessengerEventAggregator(IMessenger messenger) : IEventAggregator
{
	const int _token = 0;

	/// <inheritdoc />
	public void Subscribe<TMessage>(object service, Action<TMessage> action) where TMessage : class
	{
		messenger.Register<object, TMessage, int>(service, _token, (_, message) => action(message));
	}

	/// <inheritdoc />
	public void Unsubscribe<TMessage>(object subscriber) where TMessage : class
	{
		messenger.Unregister<TMessage, int>(subscriber, _token);
	}

	/// <inheritdoc />
	public void Publish<TMessage>(TMessage message, [CallerMemberName] string? caller = null, [CallerFilePath] string? file = null) where TMessage : class
	{
		messenger.Send(message, _token);
	}
}
