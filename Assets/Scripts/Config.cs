using System;
using UnityEngine;

public class Config : MonoBehaviour
{
	[SerializeField] private string _basePicsUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";
	[SerializeField] private string _fileExtension = "jpg";

	[SerializeField] [Range(1,300)] private int _firstIndex = 1;
	[SerializeField] [Range(2,300)] private int _lastIndex = 66;

	[SerializeField] [Range(1, 300)] private int _premiumEveryN = 4;

	public static Config Instance { get; private set; }

	private void Awake()
	{
		if(Instance is not null)
		{
			Debug.LogError($"{nameof(Config)} instance already exists!");
			return;
		}

		Instance = this;
	}

	public string GetPictureUrl(int index) => $"{_basePicsUrl}{index}.{_fileExtension}";
	public (int first, int last) GetIndexRange() => (_firstIndex, _lastIndex);
	public bool IsPremiumIndex(int index) => index % _premiumEveryN == 0;
}