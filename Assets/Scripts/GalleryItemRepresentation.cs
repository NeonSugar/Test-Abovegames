using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GalleryItemRepresentation : MonoBehaviour
{
	[SerializeField] private Image preview;
	[SerializeField] private GameObject premiumBadge;
	[SerializeField] private Sprite placeholder;

	private GalleryItemData _data;
	private CancellationTokenSource _cts;
	private bool _isLoaded;

	public void BindVisual(GalleryItemData data, System.Action<GalleryItemData> onClick)
	{
		_data = data;
		_isLoaded = false;

		premiumBadge.SetActive(data.IsPremium);
		preview.sprite = placeholder;

		var btn = GetComponent<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => onClick?.Invoke(_data));
	}

	public void EnsureLoaded(ImageLoader loader)
	{
		if (_isLoaded) return;

		_isLoaded = true;

		_cts?.Cancel();
		_cts?.Dispose();
		_cts = new ();

		_ = LoadAsync(loader, _cts.Token);
	}

	private async Task LoadAsync(
		ImageLoader loader,
		CancellationToken ct)
	{
		try
		{
			var sprite = await loader.LoadSpriteAsync(_data.Url, ct);
			if (ct.IsCancellationRequested) return;
			if (sprite != null)
				preview.sprite = sprite;
		}
		catch (System.OperationCanceledException) { }
	}

	private void OnDestroy()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}
}