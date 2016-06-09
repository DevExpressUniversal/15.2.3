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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	public interface ISpreadsheetNamedObject {
		string Name { get; }
		event NameChangedEventHandler NameChanged;
	}
	#region UndoableNamedItemCollection<T> :
	public abstract class UndoableNamedItemCollection<T> : UndoableClonableCollection<T> where T : ISpreadsheetNamedObject {
		#region Fields
		readonly Dictionary<string, T> namesHash;
		#endregion
		protected UndoableNamedItemCollection(IDocumentModelPart documentModelPart)
			: this(documentModelPart, StringExtensions.ComparerInvariantCulture) {
		}
		protected UndoableNamedItemCollection(IDocumentModelPart documentModelPart, IEqualityComparer<string> comparer)
			: base(documentModelPart) {
			this.namesHash = new Dictionary<string, T>(comparer);
		}
		#region Properties
		public Dictionary<string, T> NamesHash { get { return namesHash; } }
		public T this[string name] { get { return namesHash[name]; } }
		#endregion
		protected virtual void RegisterItem(T item) {
			item.NameChanged += item_NameChanged; 
		}
		protected virtual void UnRegisterItem(T item) {
			item.NameChanged -= item_NameChanged;
		}
		void item_NameChanged(object sender, NameChangedEventArgs e) {
			T item = NamesHash[e.OldName];
			Debug.Assert(item.Name == e.Name);
			NamesHash.Remove(e.OldName);
			NamesHash.Add(e.Name, item);
		}
		public override int AddInternal(T item) {
			NamesHash.Add(item.Name, item);
			return base.AddInternal(item);
		}
		protected  override void InsertInternal(int index, T item) {
			NamesHash.Add(item.Name, item);
			base.InsertInternal(index, item);
		}
		public override int AddWithoutHistoryAndNotifications(T item) {
			NamesHash.Add(item.Name, item);
			RegisterItem(item);
			return base.AddWithoutHistoryAndNotifications(item);
		}
		public override void ClearCore() {
			foreach (T item in this)
				UnRegisterItem(item);
			this.NamesHash.Clear();
			base.ClearCore();
		}
		public bool Contains(string name) {
			return NamesHash.ContainsKey(name);
		}
		public bool TryGetItemByName(string name, out T result) {
			return NamesHash.TryGetValue(name, out result);
		}
		public override void AddRangeCore(IEnumerable<T> collection) {
			Guard.ArgumentNotNull(collection, "collection");
			foreach (T item in collection) {
				NamesHash.Add(item.Name, item);
				RegisterItem(item);
			}
			base.AddRangeCore(collection);
		}
		public override void RemoveAtCore(int index) {
			string name = this[index].Name;
			NamesHash.Remove(name);
			base.RemoveAtCore(index);
		}
		public virtual bool Remove(string name) {
			T item = default(T);
			if (!NamesHash.TryGetValue(name, out item))
				return false;
			int index = IndexOf(item);
			if (index >= 0)
				RemoveAt(index); 
			return true;
		}
		protected override void OnItemInserted(int index, T item) {
			RegisterItem(item);
			base.OnItemInserted(index, item);
		}
		protected override void OnItemRemoved(int index, T item) {
			UnRegisterItem(item);
			base.OnItemRemoved(index, item);
		}
	}
	#endregion
	#region ISpreadsheetRangeObject
	public interface ISpreadsheetRangeObject {
		CellRange Range { get; }
		event CellRangeChangedEventHandler RangeChanged;
	}
	#endregion
	#region UndoableNamedItemTreeCollection
	public abstract class UndoableNamedItemTreeCollection<T> : UndoableNamedItemCollection<T> where T : class, ISpreadsheetRangeObject, ISpreadsheetNamedObject {
		#region Fields
		RangeObjectCachedRTree<T> tree;
		#endregion
		protected UndoableNamedItemTreeCollection(IDocumentModelPart documentModel, IEqualityComparer<string> comparer)
			: base(documentModel, comparer) {
		}
		#region Propertes
		protected internal RangeObjectCachedRTree<T> Tree {
			get {
				if (tree == null)
					tree = CreateTree();
				return tree;
			}
		}
		protected internal RangeObjectCachedRTree<T> InnerTree { get { return tree; } }
		#endregion
		RangeObjectCachedRTree<T> CreateTree() {
			RangeObjectCachedRTree<T> result = new RangeObjectCachedRTree<T>();
			int count = InnerList.Count;
			for (int i = 0; i < count; i++)
				InsertToTree(result, InnerList[i]);
			return result;
		}
		protected override void RegisterItem(T item) {
			item.RangeChanged += OnItemRangeChanged;
			InsertToTree(item);
			base.RegisterItem(item);
		}
		protected override void UnRegisterItem(T item) {
			item.RangeChanged -= OnItemRangeChanged;
			RemoveFromTree(item);
			base.UnRegisterItem(item);
		}
		protected internal virtual void OnItemRangeChanged(object sender, CellRangeChangedEventArgs e) {
			T item = sender as T;
			if (item == null)
				return;
			CellRange oldRange = e.OldRange;
			if (item.Range.Equals(oldRange))
				return;
			RemoveFromTree(item, oldRange);
			InsertToTree(item);
		}
		#region Insert\remove from tree
		void InsertToTree(T item) {
			if (InnerTree != null)
				InsertToTree(InnerTree, item);
		}
		void InsertToTree(RangeObjectCachedRTree<T> tree, T item) {
			tree.Insert(item);
		}
		void RemoveFromTree(T item) {
			if (InnerTree == null)
				return;
			InnerTree.Remove(item);
		}
		void RemoveFromTree(T item, CellRange range) {
			if (InnerTree == null)
				return;
			InnerTree.Remove(item, range);
		}
		#endregion
		public bool ContainsItemsInRange(CellRangeBase range, bool orIntersects) {
			return range.Exists((innerRange) => Tree.SearchItemOrNull(innerRange, orIntersects) != null);
		}
		public List<T> GetItems(CellRange range, bool orIntersects) {
			return Tree.Search(range, orIntersects);
		}
		public T TryGetItem(CellPosition position) {
			return Tree.Search(position.Column, position.Row);
		}
		public List<T> GetItemsIncludesOrIntersects(CellRangeBase range, IList<T> items) {
			List<T> result = new List<T>();
			if (items.Count <= 0)
				return result;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				T item = items[i];
				CellRange itemRange = item.Range;
				if (range.Intersects(itemRange))
					result.Add(item);
			}
			return result;
		}
		public override void ClearCore() {
			base.ClearCore();
			if (InnerTree != null)
				InnerTree.Clear();
		}
		protected internal virtual void CheckIntegrity(CheckIntegrityFlags flags) {
			if (InnerTree != null && InnerTree.Count != InnerList.Count)
				DevExpress.XtraSpreadsheet.Utils.IntegrityChecks.Fail("RTree items count must be equal to collection count");
		}
	}
	#endregion
}
