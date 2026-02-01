using System;
using UnityEngine;
using UnityEngine.UI;

public class TabBar : MonoBehaviour
{
    [SerializeField] private Button btnAll;
    [SerializeField] private Button btnOdd;
    [SerializeField] private Button btnEven;

    public static event Action<TabFilter> TabChanged;

    private void Awake()
    {
        btnAll.onClick.AddListener(() => TabChanged?.Invoke(TabFilter.All));
        btnOdd.onClick.AddListener(() => TabChanged?.Invoke(TabFilter.Odd));
        btnEven.onClick.AddListener(() => TabChanged?.Invoke(TabFilter.Even));
    }

    private void Start()
    {
        TabChanged?.Invoke(TabFilter.All);
    }
}