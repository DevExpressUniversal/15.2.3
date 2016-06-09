#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	internal class Sorter : IComparer 
	{
		protected int InnerCompare(object x, object y)
		{
			LanguageElement first = (LanguageElement)x;
			LanguageElement second = (LanguageElement)y;
			SourceRange lSourceRangeX = first.Range;
			SourceRange lSourceRangeY = second.Range;
			if (first is QueuedEdit && second is QueuedEdit 
				&& first.StartLine == second.StartLine)
			{
				if (first == second)
					return 0;
				if (first is QueuedDelete)
					return 1;
				else
					return -1;
			}
			if (lSourceRangeX.Top > lSourceRangeY.Top)
				return 1;
			else if (lSourceRangeX.Top < lSourceRangeY.Top)
				return -1;
			else
			{
				if (lSourceRangeX.Bottom > lSourceRangeY.Bottom)
					return 1;
				else if (lSourceRangeX.Bottom < lSourceRangeY.Bottom)
					return -1;
				else return 0;
			}
		}
		#region Compare
		int IComparer.Compare(object x, object y)
		{
			return InnerCompare(x, y);
		}
		#endregion
	}
	internal class ReverseSorter : Sorter, IComparer
	{
		#region Compare
		int IComparer.Compare(object x, object y)
		{
			return -InnerCompare(x, y);
		}
		#endregion
	}
  public class LanguageElementCollection : LanguageElementCollectionBase
  {
		public new static readonly LanguageElementCollection Empty = new LanguageElementCollection();
	#region Add
	public new int Add(LanguageElement value)
	{
	  return base.Add(value);
	}
	#endregion
		#region Add
		public void Add(LanguageElementCollection collection)
		{
			AddRange(collection);
		}
		#endregion
		#region AddRange
		public void AddRange(LanguageElementCollection collection)
		{
			if (collection == null)
				return;
			for (int i = 0; i < collection.Count; i++)
			{
				if (collection[i] == null)
					continue;
				Add(collection[i]);
			}
		}
		#endregion
	#region Contains
	public new bool Contains(LanguageElement value)
	{
	  return base.Contains(value);
	}
	#endregion
	#region CopyTo
	public void CopyTo(LanguageElement[] array, int index)
	{
	  base.CopyTo(array, index);
	}
	#endregion
	#region IndexOf
	public new int IndexOf(LanguageElement value)
	{
	  return base.IndexOf(value);
	}
	#endregion
	#region Insert
	public new void Insert(int index, LanguageElement value)
	{
	  base.Insert(index, value);
	}
	#endregion
	#region Remove
	public new void Remove(LanguageElement value)
	{
	  base.Remove(value);
	}
	#endregion
		#region Sort()
	public void Sort()
	{
			Sort(new Sorter());
	}
	#endregion
		#region SortReverse
	public void SortReverse()
	{
	  Sort(new ReverseSorter());
	}
	#endregion
		public static LanguageElementCollection FromNodeList(NodeList nodes)
		{
			LanguageElementCollection lResult = new LanguageElementCollection();
			lResult.AddRange(nodes);
			return lResult;
		}
		public static LanguageElementCollection FromArray(LanguageElement[] elements)
		{
			LanguageElementCollection lResult = new LanguageElementCollection();
			lResult.AddRange(elements);
			return lResult;
		}
	#region RemoveElementsInRange
	public void RemoveElementsInRange(SourceRange range)
	{
	  for(int i = this.Count - 1; i >= 0; i--)		
	  {
		if (range.Contains(this[i].Range))
		  RemoveAt(i);		
	  }
	}
	#endregion
	#region RemoveRange
	public void RemoveRange(int startIndex, int count)
	{
	  for(int i = 0; i < count; i++)
		RemoveAt(startIndex);
	}
	#endregion
	protected override NodeList CreateInstance()
		{
			return new LanguageElementCollection();
		}
		public new LanguageElementCollection DeepClone(ElementCloneOptions options)
		{
			LanguageElementCollection lClone = CreateInstance() as LanguageElementCollection;
			lClone.AddRange(base.DeepClone(options));
			return lClone;
		}
	#region this[int index]
	public new LanguageElement this[int index] 
	{
	  get
	  {
		return (LanguageElement) base[index];
	  }
	  set
	  {
		base[index] = value;
	  }
	}
	#endregion
  }
}
