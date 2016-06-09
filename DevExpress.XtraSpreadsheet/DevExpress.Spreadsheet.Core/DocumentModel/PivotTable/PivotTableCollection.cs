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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTableCachedRTree
	public class PivotTableCachedRTree : CellRangeWithUnionCachedRTreeBase<PivotTable> {
		protected override CellRangeBase GetItemRange(PivotTable item) {
			return item.WholeRange;
		}
	}
	#endregion
	#region SetUniqueReferencedItemsBuilder
	public static class SetUniqueReferencedItemsBuilder  {
		public static void Build<TItem>(IList<TItem> items) where TItem : class {
			int count = items.Count;
			if (count <= 1)
				return;
			int primaryPosition = 0;
			int secondaryPosition = 1;
			while (primaryPosition < count - 1) {
				TItem primaryItem = items[primaryPosition];
				TItem secondaryItem = items[secondaryPosition];
				if (Object.ReferenceEquals(primaryItem, secondaryItem)) {
					items.RemoveAt(secondaryPosition);
					count--;
					if (secondaryPosition == count) {
						primaryPosition++;
						secondaryPosition = primaryPosition + 1;
					}
				} else {
					if (secondaryPosition < count - 1)
						secondaryPosition++;
					else {
						primaryPosition++;
						secondaryPosition = primaryPosition + 1;
					}
				}
			}
		}
	}
	#endregion
	#region PivotTableCollection
	public class PivotTableCollection : UndoableNamedItemCollection<PivotTable> {
		#region Fields
		PivotTableCachedRTree tree;
		#endregion
		public PivotTableCollection(IDocumentModelPart documentModelPart)
			: base(documentModelPart, StringExtensions.ComparerInvariantCultureIgnoreCase) {
		}
		#region Propertes
		protected internal PivotTableCachedRTree Tree {
			get {
				if (tree == null)
					tree = CreateTree();
				return tree;
			}
		}
		protected internal PivotTableCachedRTree InnerTree { get { return tree; } }
		#endregion
		#region CreateTree
		PivotTableCachedRTree CreateTree() {
			PivotTableCachedRTree result = new PivotTableCachedRTree();
			int count = InnerList.Count;
			for (int i = 0; i < count; i++)
				result.Insert(InnerList[i]);
			return result;
		}
		#endregion
		#region Insert\remove from tree
		void InsertToTree(PivotTable item) {
			if (InnerTree != null)
				InnerTree.Insert(item);
		}
		void RemoveFromTree(PivotTable item) {
			if (InnerTree != null)
				InnerTree.Remove(item);
		}
		void RemoveFromTree(PivotTable item, CellRangeBase range) {
			if (InnerTree != null)
				InnerTree.Remove(item, range);
		}
		#endregion
		#region Register
		protected override void RegisterItem(PivotTable item) {
			item.WholeRangeChanged += OnItemRangeChanged;
			InsertToTree(item);
			base.RegisterItem(item);
		}
		protected override void UnRegisterItem(PivotTable item) {
			item.WholeRangeChanged -= OnItemRangeChanged;
			RemoveFromTree(item);
			base.UnRegisterItem(item);
		}
		#endregion
		#region ClearCore
		public override void ClearCore() {
			base.ClearCore();
			if (InnerTree != null)
				InnerTree.Clear();
		}
		#endregion
		#region CheckIntegrity
		protected internal void CheckIntegrity(CheckIntegrityFlags flags) {
			if (InnerTree != null && InnerTree.Count != InnerList.Count)
				DevExpress.XtraSpreadsheet.Utils.IntegrityChecks.Fail("RTree items count must be equal to collection count");
		}
		#endregion
		protected internal IList<string> GetExistingNames() {
			int count = Count;
			List<string> result = new List<string>(count);
			for (int i = 0; i < count; i++)
				result.Add(this[i].Name);
			return result;
		}
		public bool ContainsItemsInRange(CellRangeBase range, bool orIntersects) {
			return range.Exists((innerRange) => Tree.SearchItemOrNull(innerRange, orIntersects) != null);
		}
		public void RemoveItems(List<PivotTable> pivotTables) {
			SetUniqueReferencedItemsBuilder.Build(pivotTables);
			foreach (PivotTable pivotTable in pivotTables)
				this.Remove(pivotTable);
		}
		#region GetItems
		public List<PivotTable> GetItems(CellRangeBase range, bool orIntersects) {
			List<PivotTable> result = Tree.Search(range, orIntersects);
			SetUniqueReferencedItemsBuilder.Build(result);
			return result;
		}
		public List<PivotTable> GetRangesItems(IList<CellRange> ranges, bool orIntersects) {
			List<PivotTable> result = new List<PivotTable>();
			foreach (CellRange cellRange in ranges)
				result.AddRange(GetItems(cellRange, orIntersects));
			SetUniqueReferencedItemsBuilder.Build(result);
			return result;
		}
		public PivotTable TryGetItem(CellPosition position) {
			return Tree.Search(position.Column, position.Row);
		}
		#endregion
		#region Notifications
		public IModelErrorInfo CanRangesRemove(IList<CellRange> ranges, bool orIntersects, RemoveCellMode mode) {
			foreach (CellRange range in ranges) 
				foreach (PivotTable pivotTable in this.GetItems(range, orIntersects)) {
					IModelErrorInfo error = pivotTable.Location.CanRangeRemove(range, mode);
					if (error != null) 
						return error;
				}
			return null;
		}
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			for (int i = 0; i < InnerList.Count; i++) {
				IModelErrorInfo error = InnerList[i].CanRangeInsert(range, mode);
				if (error != null)
					return error;
			}
			return null;
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			for (int i = 0; i < InnerList.Count; i++) {
				IModelErrorInfo error = InnerList[i].CanRangeRemove(range, mode);
				if (error != null)
					return error;
			}
			return null;
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = 0; i < Count; ++i)
				this[i].OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--) {
				PivotTable pivotTable = this[i];
				if (!pivotTable.OnRangeRemoving(context)) {
					PivotTableRemoveCommand command = new PivotTableRemoveCommand(pivotTable);
					command.Execute();
				}
			}
		}
		#endregion
		public override UndoableClonableCollection<PivotTable> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new PivotTableCollection(documentModelPart);
		}
		public override PivotTable GetCloneItem(PivotTable item, IDocumentModelPart documentModelPart) {
			throw new ArgumentException("Not implemeted");
		}
		internal void MarkupPivotTablesForInvalidateFormatCache(CellIntervalRange affectedRange) {
			MarkupPivotTablesForInvalidateFormatCacheHistoryItem item = new MarkupPivotTablesForInvalidateFormatCacheHistoryItem(this, affectedRange);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal void InvalidateFormatCache(CellRange range) {
			List<PivotTable> items = GetItems(range, true);
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].CalculationInfo.InvalidateStyleFormatCache();
		}
		protected internal void OnItemRangeChanged(object sender, CellRangeChangedEventArgsBase<CellRangeBase> e) {
			PivotTable item = sender as PivotTable;
			if (item == null)
				return;
			CellRangeBase oldRange = e.OldRange;
			CellRangeBase itemRange = item.WholeRange;
			if (itemRange.Equals(oldRange))
				return;
			RemoveFromTree(item, oldRange);
			InsertToTree(item);
		}
		#region GetUniqueIntersectedItems
		public ICollection<PivotTable> GetUniqueIntersectedItems(IList<CellRange> ranges) {
			ICollection<PivotTable> result = new HashSet<PivotTable>();
			int rangesCount = ranges.Count;
			for (int i = 0; i < rangesCount; i++) {
				List<PivotTable> intersectedItems = GetItems(ranges[i], true);
				int intersectedItemsCount = intersectedItems.Count;
				for (int j = 0; j < intersectedItemsCount; j++) {
					PivotTable intersectedItem = intersectedItems[j];
					if (!result.Contains(intersectedItem))
						result.Add(intersectedItem);
				}
			}
			return result;
		}
		#endregion
	}
	#endregion
}
