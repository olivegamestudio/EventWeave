using System;
using System.Runtime.CompilerServices;

namespace EventWeave
{
	/// <summary>
	/// Defines a service for loosely coupled communication between services using a publish-subscribe pattern.
	/// </summary>
	public interface IEventAggregator
	{
		/// <summary>
		/// Subscribes a service to a specific message type.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
		/// <param name="service">The service instance that is subscribing.</param>
		/// <param name="action">The action to execute when the message is received.</param>
		void Subscribe<TMessage>(object service, Action<TMessage> action) where TMessage : class;

		/// <summary>
		/// Unsubscribes a service from all message types it is subscribed to.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
		/// <param name="subscriber">The service instance that is unsubscribing.</param>
		void Unsubscribe<TMessage>(object subscriber) where TMessage : class;

		/// <summary>
		/// Publishes a message to all subscribers.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message to publish.</typeparam>
		/// <param name="message">The message instance to publish.</param>
		/// <param name="caller">The name of the method that called this method.</param>
		/// <param name="file">The file path of the source code file that contains the method.</param>
		void Publish<TMessage>(TMessage message, [CallerMemberName] string? caller = null,
			[CallerFilePath] string? file = null) where TMessage : class;
	}
}
