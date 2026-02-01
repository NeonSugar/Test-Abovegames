using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
	[Header("Common")]
	[SerializeField] private Image _dimmer;

	[Header("Image Popup")]
	[SerializeField] private GameObject _imagePopup;
	[SerializeField] private Image _imagePopupPreview;
	[SerializeField] private Button _imagePopupClose;

	[Header("Premium Popup")]
	[SerializeField] private GameObject _premiumPopup;
	[SerializeField] private RectTransform _premiumPanel;
	[SerializeField] private Button _premiumClose;

	[Header("Deps")]
	[SerializeField] private ImageLoader _loader;

	[Header("Animation")]
	[SerializeField] private float _animationDuration = 0.3f;

	private CancellationTokenSource _cts;

	private void Awake()
	{
		HideAll(true);

		_imagePopupClose.onClick.AddListener(HideAll);
		_premiumClose.onClick.AddListener(HideAll);

		_dimmer.GetComponent<Button>()?.onClick.AddListener(HideAll);
	}

	public void ShowImage(string url)
	{
		HideAll(true);
		_dimmer.gameObject.SetActive(true);
		_imagePopup.SetActive(true);
		_dimmer.DOFade(0.7f, _animationDuration);

		_cts?.Cancel();
		_cts?.Dispose();
		_cts = new ();

		_ = LoadToPopup(url, _cts.Token);
	}

	public void ShowPremium()
	{
		HideAll(true);
		_dimmer.gameObject.SetActive(true);
		_dimmer.DOFade(0.7f, _animationDuration).OnComplete(() =>
		{
			_premiumPopup.SetActive(true);
			_premiumPanel.DOScale(Vector3.one, _animationDuration);
		});
	}

	private void HideAll(bool instantly)
	{
		_cts?.Cancel();
		_imagePopup.SetActive(false);
		_premiumPopup.SetActive(false);
		_premiumPanel.DOScale(Vector3.zero, _animationDuration);
		if (!instantly) _dimmer.DOFade(0, _animationDuration).OnComplete(() => _dimmer.gameObject.SetActive(false));
	}

	private void HideAll() => HideAll(false);

	private async System.Threading.Tasks.Task LoadToPopup(string url, CancellationToken ct)
	{
		try
		{
			var sprite = await _loader.LoadSpriteAsync(url, ct);
			if (ct.IsCancellationRequested) return;
			if (sprite != null) _imagePopupPreview.sprite = sprite;
		}
		catch (System.OperationCanceledException) { }
	}
}