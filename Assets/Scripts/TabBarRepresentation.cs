using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TabBarRepresentation : MonoBehaviour
{
	[SerializeField] private Color _activeColor;
	[SerializeField] private Color _inactiveColor;

	[SerializeField] private TMP_Text _allText;
	[SerializeField] private TMP_Text _OddText;
	[SerializeField] private TMP_Text _EvenText;

	[SerializeField] private RectTransform _slider;

	[SerializeField] private float _colorAnimationDuration = 0.2f;
	[SerializeField] private float _sliderAnimationDuration = 0.2f;

	private void Awake()
	{
		TabBar.TabChanged += WhenTabChanged;
	}

	private void OnDestroy()
	{
		TabBar.TabChanged -= WhenTabChanged;
	}

	private void WhenTabChanged(TabFilter tab)
	{
		var (active, inactives) = tab switch {
			TabFilter.All => (_allText, new List<TMP_Text> {_OddText, _EvenText}),
			TabFilter.Odd => (_OddText, new () {_allText, _EvenText}),
			TabFilter.Even => (_EvenText, new () {_allText, _OddText}),
			_ => throw new ArgumentOutOfRangeException(nameof(tab), tab, null)
		};

		Draw(active, inactives);
	}

	private void Draw(TMP_Text active, List<TMP_Text> inactive)
	{
		foreach(var item in inactive)
		{
			item.DOColor(_inactiveColor, _colorAnimationDuration);
		}

		active.DOColor(_activeColor, _colorAnimationDuration);
		_slider.DOMoveX(active.rectTransform.position.x, _sliderAnimationDuration);
	}
}