using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PremiumPlanRepresentation : MonoBehaviour
{
	[SerializeField] private Image _dotImage;
	[SerializeField] private TMP_Text _mainText;
	[SerializeField] private TMP_Text _additionText;

	[SerializeField] private Button _button;

	[Header("Decoration")]
	[SerializeField] private Sprite _dotActive;
	[SerializeField] private Sprite _dotInactive;
	[SerializeField] private Color _mainTextActiveColor;
	[SerializeField] private Color _mainTextInactiveColor;
	[SerializeField] private Color _additionTextActiveColor;
	[SerializeField] private Color _additionTextInactiveColor;

	[SerializeField] private float _animationDuration;

	public static event Action<PremiumPlanRepresentation> ButtonPushed;

	private void Awake()
	{
		_button.onClick.AddListener(() => ButtonPushed?.Invoke(this));
		ButtonPushed += WhenButtonPushed;
	}

	private void OnDestroy()
	{
		ButtonPushed -= WhenButtonPushed;
	}

	private void WhenButtonPushed(PremiumPlanRepresentation representation)
	{
		if (representation == this)
			Activate();
		else
			Deactivate();
	}

	private void Activate()
	{
		_dotImage.sprite = _dotActive;
		_mainText.DOColor(_mainTextActiveColor, _animationDuration);
		_additionText?.DOColor(_additionTextActiveColor, _animationDuration);
	}

	private void Deactivate()
	{
		_dotImage.sprite = _dotInactive;
		_mainText.DOColor(_mainTextInactiveColor, _animationDuration);
		_additionText?.DOColor(_additionTextInactiveColor, _animationDuration);
	}
}