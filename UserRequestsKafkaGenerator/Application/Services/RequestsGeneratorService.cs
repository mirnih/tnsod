using System.Collections.Concurrent;
using UserRequestsKafkaGenerator.Application.Mappers;
using UserRequestsKafkaGenerator.Domain.Entities;
using UserRequestsKafkaGenerator.Domain.Interfaces;

namespace UserRequestsKafkaGenerator.Application.Services;

public sealed class RequestsGeneratorService : IRequestsGeneratorService, IAsyncDisposable
{
    private readonly IKafkaProducerRepository _repo;
    private readonly IEventMapper _mapper;

    private readonly ConcurrentDictionary<int, UserRequestTask> _tasks = new();
    private readonly ConcurrentDictionary<int, Task> _loops = new();
    private int _nextId;

    public RequestsGeneratorService(IKafkaProducerRepository repo, IEventMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public int AddTask(int userId, string endpoint, int rpm)
    {
        var id = Interlocked.Increment(ref _nextId);
        var t = new UserRequestTask(id, userId, endpoint, rpm);
        if (!_tasks.TryAdd(id, t)) return -1;
        var loop = Task.Run(() => LoopAsync(t));
        _loops[id] = loop;
        return id;
    }

    public bool UpdateTaskRpm(int id, int rpm)
    {
        if (rpm <= 0) return false;
        if (!_tasks.TryGetValue(id, out var t)) return false;
        t.Update(rpm: rpm);
        return true;
    }

    public bool UpdateTaskEndpoint(int id, string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint)) return false;
        if (!_tasks.TryGetValue(id, out var t)) return false;
        t.Update(endpoint: endpoint);
        return true;
    }

    public bool RemoveTask(int id)
    {
        if (!_tasks.TryRemove(id, out var t)) return false;
        t.Cts.Cancel();
        if (_loops.TryRemove(id, out var loop))
        {
            try { loop.Wait(); } catch { }
        }
        return true;
    }

    public IReadOnlyCollection<UserRequestTask> List() => _tasks.Values.OrderBy(x => x.Id).ToArray();

    private async Task LoopAsync(UserRequestTask t)
    {
        while (!t.Cts.IsCancellationRequested)
        {
            if (t.UserId <= 0 || string.IsNullOrWhiteSpace(t.Endpoint))
            {
                t.Cts.Cancel();
                break;
            }

            var rpm = t.Rpm > 0 ? t.Rpm : 1;
            var delayMs = Math.Max(1, (int)Math.Round(60000.0 / rpm));

            try
            {
                var evt = _mapper.ToKafkaEvent(t);
                await _repo.ProduceAsync(evt, t.Cts.Token);
            }
            catch { }

            try
            {
                await Task.Delay(delayMs, t.Cts.Token);
            }
            catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var t in _tasks.Values) t.Cts.Cancel();
        try { await Task.WhenAll(_loops.Values); } catch { }
        await _repo.DisposeAsync();
    }
}
