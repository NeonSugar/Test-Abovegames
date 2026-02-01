using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BannerCarousel : MonoBehaviour
{
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private float _autoScrollSeconds = 5f;
	[SerializeField] private float _snapSpeed = 10f;
	[SerializeField] private float _swipeThreshold = 0.15f;

	[Header("Dots")]
	[SerializeField] private RectTransform _dotsRoot;
	[SerializeField] private Image _dotPrefab;
	[SerializeField] private Color _dotActiveColor = Color.white;
	[SerializeField] private Color _dotInactiveColor = new Color(1, 1, 1, 0.35f);
	[SerializeField] private float _dotAnimationDuration;

	private readonly List<Image> _dots = new();

	private int _page = 0;
	private int _pagesCount = 3;

	private float _timer;
	private bool _dragging;

	private float _dragStartPos;

	private void Awake()
	{
		UIDragForwarder.DragBegan += WhenDragBegan;
		UIDragForwarder.DragBeganInternal += WhenDragBeganInternal;
		UIDragForwarder.DragEnded += WhenDragEnded;

	}

	private void Start()
	{
		Rebuild();
	}

	private void OnDestroy()
	{
		UIDragForwarder.DragBegan -= WhenDragBegan;
		UIDragForwarder.DragBeganInternal -= WhenDragBeganInternal;
		UIDragForwarder.DragEnded -= WhenDragEnded;
	}

	private void Rebuild()
	{
		BuildDots();
		ApplyDots();
		JumpToPage(_page);
	}

	private void Update()
	{
		if (_pagesCount <= 1) return;

		if (!_dragging)
		{
			_timer += Time.deltaTime;
			if (_autoScrollSeconds > 0f && _timer >= _autoScrollSeconds)
			{
				_timer = 0f;
				Next();
			}

			var target = PageToNormalized(_page);
			_scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_scrollRect.horizontalNormalizedPosition, target, Time.deltaTime * _snapSpeed);

			if (Mathf.Abs(_scrollRect.horizontalNormalizedPosition - target) < 0.0005f)
				_scrollRect.horizontalNormalizedPosition = target;
		}
	}

	private void JumpToPage(int index)
	{
		if (_pagesCount == 0) return;
		_page = Mathf.Clamp(index, 0, _pagesCount - 1);
		_scrollRect.horizontalNormalizedPosition = PageToNormalized(_page);
		_timer = 0f;
		ApplyDots();
	}


	private void Next()
	{
		_page = (_page + 1) % _pagesCount;
		ApplyDots();
	}

	private void Prev()
	{
		_page = (_page - 1 + _pagesCount) % _pagesCount;
		ApplyDots();
	}

	private void BuildDots()
	{
		_dotsRoot.DestroyChildren();
		_dots.Clear();

		if (_dotsRoot == null || _dotPrefab == null) return;

		for (var i = 0; i < _pagesCount; i++)
		{
			var dot = Instantiate(_dotPrefab, _dotsRoot);
			dot.color = _dotInactiveColor;
			_dots.Add(dot);
		}
	}

	private void ApplyDots()
	{
		if (_dots.Count != _pagesCount) return;

		for (var i = 0; i < _dots.Count; i++)
		{
			_dots[i].DOColor((i == _page) ? _dotActiveColor : _dotInactiveColor, _dotAnimationDuration);
		}
	}

	private void UpdatePageIndexFromScroll()
	{
		var pos = _scrollRect.horizontalNormalizedPosition;
		var nearest = NormalizedToNearestPage(pos);
		if (nearest != _page)
			_page = nearest;
	}

	private float PageToNormalized(int page)
	{
		if (_pagesCount <= 1) return 0f;
		return page / (float)(_pagesCount - 1);
	}

	private int NormalizedToNearestPage(float normalized)
	{
		if (_pagesCount <= 1) return 0;
		return Mathf.Clamp(Mathf.RoundToInt(normalized * (_pagesCount - 1)), 0, _pagesCount - 1);
	}

	private void WhenDragBegan(PointerEventData eventData)
	{
		_dragging = true;
		_timer = 0f;
		_dragStartPos = _scrollRect.horizontalNormalizedPosition;
	}

	private void WhenDragBeganInternal(PointerEventData eventData)
	{
		// empty
	}

	private void WhenDragEnded(PointerEventData eventData)
	{
		_dragging = false;
		_timer = 0f;

		var endPos = _scrollRect.horizontalNormalizedPosition;
		var delta = endPos - _dragStartPos;

		if (Mathf.Abs(delta) >= _swipeThreshold)
		{
			if (delta > 0f) Next();
			else Prev();
		}
		else
		{
			_page = NormalizedToNearestPage(endPos);
		}

		ApplyDots();
	}
}