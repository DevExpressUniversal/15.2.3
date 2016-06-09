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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office {
	#region UniqueItemsCache<T> (abstract class)
	public abstract class UniqueItemsCache<T> : ISupportsSizeOf where T : ICloneable<T>, ISupportsSizeOf {
		readonly List<T> items;
		Dictionary<T, int> itemDictionary;
		protected IDocumentModel workbook;
		DXCollectionUniquenessProviderType uniquenessProviderType = DXCollectionUniquenessProviderType.MinimizeMemoryUsage;
		protected UniqueItemsCache(IDocumentModelUnitConverter unitConverter, IDocumentModel workbook) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			Guard.ArgumentNotNull(workbook, "workbook");
			this.items = new List<T>();
			this.workbook = workbook;
			InitItems(unitConverter);
		}
		protected UniqueItemsCache(IDocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.items = new List<T>();
			InitItems(unitConverter);
		}
		#region Properties
		public T this[int index] { get { return items[index]; } }
		public T DefaultItem { get { return items[0]; } }
		public int Count { get { return items.Count; } }
		protected List<T> Items { get { return items; } }
		protected Dictionary<T, int> ItemDictionary { get { return itemDictionary; } }
		protected DXCollectionUniquenessProviderType UniquenessProviderType {
			get { return uniquenessProviderType; }
			set {
				if (value == DXCollectionUniquenessProviderType.None)
					value = DXCollectionUniquenessProviderType.MinimizeMemoryUsage;
				if (uniquenessProviderType == value)
					return;
				if (value == DXCollectionUniquenessProviderType.MaximizePerformance)
					SwitchToDictionaryMethod();
				else
					SwitchToIndexOfMethod();
			}
		}
		void SwitchToIndexOfMethod() {
			this.itemDictionary = null;
			this.uniquenessProviderType = DXCollectionUniquenessProviderType.MinimizeMemoryUsage;
		}
		void SwitchToDictionaryMethod() {
			this.itemDictionary = new Dictionary<T, int>();
			this.uniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			int count = Count;
			for (int i = 0; i < count; i++)
				itemDictionary.Add(items[i], i);
		}
		#endregion
		public bool IsIndexValid(int index) {
			return index >= 0 && index < items.Count;
		}
		protected virtual void InitItems(IDocumentModelUnitConverter unitConverter) {
			T defaultItem = CreateDefaultItem(unitConverter);
			if (defaultItem != null)
				AppendItem(defaultItem);
		}
		protected abstract T CreateDefaultItem(IDocumentModelUnitConverter unitConverter);
		public int GetItemIndex(T item) {
			int index = LookupItem(item);
			return (index >= 0) ? index : AddItemCore(item);
		}
		protected virtual int AddItemCore(T item) {
			if (item == null)
				return -1;
			return AppendItem(item.Clone());
		}
		public int AddItem(T item) {
			int index = LookupItem(item);
			return index >= 0 ? index : AddItemCore(item);
		}
		protected int LookupItem(T item) {
			if (uniquenessProviderType == DXCollectionUniquenessProviderType.MaximizePerformance) {
				int result;
				if (itemDictionary.TryGetValue(item, out result))
					return result;
				else
					return -1;
			}
			else
				return items.IndexOf(item);
		}
		protected int AppendItem(T item) {
			int result = Count;
			items.Add(item);
			if (uniquenessProviderType == DXCollectionUniquenessProviderType.MaximizePerformance) {
				itemDictionary.Add(item, result);
				System.Diagnostics.Debug.Assert(items.Count == itemDictionary.Count);
			}
			return result;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			int result = ObjectSizeHelper.CalculateApproxObjectSize32(this);
			int count = this.Count;
			for (int i = 0; i < count; i++)
				result += this[i].SizeOf();
			return result;
		}
		#endregion
		public virtual void CopyFrom(UniqueItemsCache<T> source) {
			this.items.Clear();
			if (itemDictionary != null)
				this.itemDictionary.Clear();
			int currentIndex = 0;
			foreach (T item in source.Items) {
				T clone = item.Clone();
				items.Add(clone);
				if (uniquenessProviderType == DXCollectionUniquenessProviderType.MaximizePerformance)
					itemDictionary.Add(clone, currentIndex);
				currentIndex++;
			}
		}
	}
	#endregion
}
