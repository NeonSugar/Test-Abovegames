// Assets/Scripts/GridGalleryController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private GalleryItemRepresentation itemPrefab;
    [SerializeField] private ImageLoader imageLoader;
    [SerializeField] private Popup popup;

    [Header("Lazy Load")]
    [SerializeField] private int prefetchRows = 2;

    private readonly List<GalleryItemData> _data = new();
    private readonly List<GalleryItemRepresentation> _views = new();

    private float _rowHeight;
    private float _viewportHeight;

    private int _lastStart = -1;
    private int _lastEnd = -1;

    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(_ => UpdateVisible());
        TabBar.TabChanged += SetTab;
    }

    private void Start()
    {
        CacheLayoutMetrics();
    }

    private void SetTab(TabFilter filter)
    {
        BuildData(filter);

        foreach(var item in _views)
            Destroy(item.gameObject);

        _views.Clear();

        foreach(var item in _data)
        {
            var v = Instantiate(itemPrefab, content);
            v.BindVisual(item, OnItemClicked);
            _views.Add(v);
        }

        scrollRect.verticalNormalizedPosition = 1f;
        _lastStart = _lastEnd = -1;
        CacheLayoutMetrics();
        UpdateVisible(force: true);
    }

    private void CacheLayoutMetrics()
    {
        _rowHeight = grid.cellSize.y + grid.spacing.y;
        _viewportHeight = scrollRect.viewport.rect.height;
    }

    private void BuildData(TabFilter filter)
    {
        _data.Clear();

        var (firstIndex, lastIndex) = Config.Instance.GetIndexRange();

        for (var i = firstIndex; i <= lastIndex; i++)
        {
            var ok = filter switch
            {
                TabFilter.All => true,
                TabFilter.Odd => (i % 2 == 1),
                TabFilter.Even => (i % 2 == 0),
                _ => true
            };

            if (ok)
                _data.Add(new (i));
        }
    }

    private void UpdateVisible(bool force = false)
    {
        CacheLayoutMetrics();

        var scrollY = content.anchoredPosition.y;

        var startRow = Mathf.FloorToInt((scrollY - grid.padding.top) / _rowHeight);
        startRow = Mathf.Max(0, startRow - prefetchRows);

        var endRow = Mathf.CeilToInt((scrollY + _viewportHeight) / _rowHeight);
        endRow = Mathf.Max(0, endRow + prefetchRows);

        var columns = grid.constraintCount;
        var startIndex = startRow * columns;
        var endIndex = Mathf.Min(_views.Count - 1, ((endRow + 1) * columns) - 1);

        if (!force && startIndex == _lastStart && endIndex == _lastEnd)
            return;

        _lastStart = startIndex;
        _lastEnd = endIndex;

        for (var i = startIndex; i <= endIndex; i++)
        {
            if (i < 0 || i >= _views.Count) continue;
            _views[i].EnsureLoaded(imageLoader);
        }
    }

    private void OnItemClicked(GalleryItemData data)
    {
        if (data.IsPremium)
            popup.ShowPremium();
        else
            popup.ShowImage(data.Url);
    }
}