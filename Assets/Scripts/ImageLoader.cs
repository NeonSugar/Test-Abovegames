using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    [Header("Cache")]
    [SerializeField] private int spriteCacheCapacity = 64;
    [SerializeField] private int maxParallelRequests = 6;

    private LruCache<string, Sprite> _spriteCache;
    private SemaphoreSlim _semaphore;
    private readonly Dictionary<string, Task<Sprite>> _inflight = new();

    private void Awake()
    {
        _spriteCache = new (spriteCacheCapacity);
        _semaphore = new (maxParallelRequests, maxParallelRequests);
    }

    public async Task<Sprite> LoadSpriteAsync(string url, CancellationToken ct)
    {
        Debug.Log($"Trying load sprite {url}");
        if (_spriteCache.TryGet(url, out var cached))
            return cached;

        if (_inflight.TryGetValue(url, out var inflightTask))
            return await inflightTask;

        var task = InternalLoad(url, ct);
        _inflight[url] = task;

        try
        {
            var sprite = await task;
            if (sprite != null)
                _spriteCache.Put(url, sprite);
            return sprite;
        }
        finally
        {
            _inflight.Remove(url);
        }
    }

    private async Task<Sprite> InternalLoad(string url, CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            using var req = UnityWebRequestTexture.GetTexture(url);
            var op = req.SendWebRequest();

            while (!op.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to load: {url} | {req.error}");
                return null;
            }

            var tex = DownloadHandlerTexture.GetContent(req);
            if (tex == null) return null;

            var rect = new Rect(0, 0, tex.width, tex.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, rect, pivot, 100f);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}