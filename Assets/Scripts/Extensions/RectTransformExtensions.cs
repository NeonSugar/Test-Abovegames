using UnityEngine;

namespace Extensions
{
	public static class RectTransformExtensions
	{
		public static void DestroyChildren(this RectTransform origin)
		{
			foreach(var child in origin)
			{
				Object.Destroy((Object)child);
			}
		}
	}
}