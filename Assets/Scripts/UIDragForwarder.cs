using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragForwarder : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	public static event Action<PointerEventData> DragBegan;
	public static event Action<PointerEventData> DragBeganInternal;
	public static event Action<PointerEventData> DragEnded;

	[Header("Forward to these")]
	[SerializeField] private ScrollRect _scrollRect;

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		_scrollRect?.OnInitializePotentialDrag(eventData);
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		_scrollRect?.OnBeginDrag(eventData);
		DragBegan?.Invoke(eventData);
	}
	public void OnDrag(PointerEventData eventData)
	{
		_scrollRect?.OnDrag(eventData);
		DragBeganInternal?.Invoke(eventData);
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		_scrollRect?.OnEndDrag(eventData);
		DragEnded?.Invoke(eventData);
	}
}