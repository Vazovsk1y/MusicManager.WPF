using System;
using System.Collections.Generic;

namespace MusicManager.WPF.Infrastructure;

public static class CollectionExtensions
{
	public static void AddRange<T>(this IList<T> values, IEnumerable<T> collectionToAdd)
	{
		ArgumentNullException.ThrowIfNull(collectionToAdd);

		foreach (var item in collectionToAdd)
		{
			values.Add(item);
		}
	}
}
