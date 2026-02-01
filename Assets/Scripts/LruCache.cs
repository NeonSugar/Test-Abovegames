using System.Collections.Generic;

public class LruCache<TKey, TValue>
{
	private readonly int _capacity;
	private readonly Dictionary<TKey, LinkedListNode<(TKey key, TValue val)>> _map = new();
	private readonly LinkedList<(TKey key, TValue val)> _list = new();

	public LruCache(int capacity)
	{
		_capacity = capacity;
	}

	public bool TryGet(TKey key, out TValue value)
	{
		if (_map.TryGetValue(key, out var node))
		{
			_list.Remove(node);
			_list.AddFirst(node);
			value = node.Value.val;
			return true;
		}
		value = default;
		return false;
	}

	public void Put(TKey key, TValue value)
	{
		if (_map.TryGetValue(key, out var existing))
		{
			_list.Remove(existing);
			_map.Remove(key);
		}

		var node = new LinkedListNode<(TKey, TValue)>((key, value));
		_list.AddFirst(node);
		_map[key] = node;

		if (_map.Count > _capacity)
		{
			var last = _list.Last;
			_list.RemoveLast();
			_map.Remove(last.Value.key);
		}
	}
}