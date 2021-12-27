using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Interim;

public interface IBackgroundTaskQueue
{
	ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

	ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
		CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
	private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

	public BackgroundTaskQueue(int capacity)
	{
		// Capacity should be set based on the expected application load and
		// number of concurrent threads accessing the queue.            
		// BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
		// which completes only when space became available. This leads to backpressure,
		// in case too many publishers/calls start accumulating.
		var options = new BoundedChannelOptions(capacity) { FullMode = BoundedChannelFullMode.Wait };
		_queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
	}

	public async ValueTask QueueBackgroundWorkItemAsync(
		Func<CancellationToken, ValueTask> workItem)
	{
		if (workItem == null)
			throw new ArgumentNullException(nameof(workItem));

		await _queue.Writer.WriteAsync(workItem);
	}

	public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken) => await _queue.Reader.ReadAsync(cancellationToken);
}

public class QueuedHostedService : BackgroundService
{
	public QueuedHostedService(IBackgroundTaskQueue taskQueue) => TaskQueue = taskQueue;

	public IBackgroundTaskQueue TaskQueue { get; }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var workItem =
				await TaskQueue.DequeueAsync(stoppingToken);

			try
			{
				await workItem(stoppingToken);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}