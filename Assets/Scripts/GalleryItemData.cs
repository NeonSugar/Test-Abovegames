using System;

[Serializable]
public readonly struct GalleryItemData
{
	public readonly int Index;
	public readonly string Url;
	public readonly bool IsPremium;

	public GalleryItemData(int index)
	{
		Index = index;
		Url = Config.Instance.GetPictureUrl(index);
		IsPremium = Config.Instance.IsPremiumIndex(index);
	}
}