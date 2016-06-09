#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public abstract class DataItemCollectionDragGroup<TDataItem> : DragGroup, IDragObjectAcceptor, IDataItemsCreator where TDataItem : DataItem {
		readonly IDataItemCollection<TDataItem> dataItems;
		readonly List<DragItem> items = new List<DragItem>();
		Rectangle dropBounds;
		public IDataItemCollection<TDataItem> DataItems { get { return dataItems; } }
		public override IEnumerable<DragItem> Items { get { return items; } }
		public override int DataItemsCount { get { return dataItems.Count; } }
		int SpaceBetweenItems { get { return Section.Area.ParentControl.DrawingContext.Painters.GroupPainter.ItemIndent; } }
		protected DataItemCollectionDragGroup(IDataItemCollection<TDataItem> dataItems)
			: base(null) {
			this.dataItems = dataItems;
			dataItems.CollectionChanged += DataItems_Changed;
		}
		public override void Initialize(DragSection section) {
			base.Initialize(section);
			Update();
		}
		void DataItems_Changed(object sender, EventArgs e) {
			Update();
			Section.Area.Arrange();
		}
		void Update() {
			items.Clear();
			string itemName = Section.ItemName;
			string itemNamePlural = Section.ItemNamePlural;
			FillDragItems(itemName, itemNamePlural);
			items.Add(new DragItem(DataSource, this, itemName, itemNamePlural));
		}
		protected virtual void FillDragItem(string itemName, string itemNamePlural, TDataItem dataItem) {
			items.Add(new DragItem(DataSource, dataItem, this, itemName, itemNamePlural));
		}
		void FillDragItems(string itemName, string itemNamePlural) {
			foreach(TDataItem dataItem in dataItems)
				FillDragItem(itemName, itemNamePlural, dataItem);
		}
		DragItem FindDragItem(TDataItem dataItem) {
			foreach(DragItem item in Items) {
				if(item.DataItem == dataItem)
					return item;
			}
			return null;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				dataItems.CollectionChanged -= DataItems_Changed;
		}
		public override void Paint(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			base.Paint(drawingContext, cache);
			DragItem itemBefore = null;
			DragItem itemAfter = null;
			foreach(DragItem dragItem in items) {
				if(dragItem.SizeState == DragItemSizeState.ShrunkFromNext)
					itemBefore = dragItem;
				if(dragItem.SizeState == DragItemSizeState.ShrunkFromPrevious)
					itemAfter = dragItem;
			}
			if(itemBefore != null || itemAfter != null) {
				Rectangle bounds = dropBounds;
				int top = bounds.Top;
				int bottom = bounds.Bottom;
				Rectangle separator;
				if(itemBefore == null || itemAfter == null) {
					if(itemBefore != null) {
						bounds = itemBefore.DrawingBounds;
						top = bounds.Bottom + 1;
					}
					if(itemAfter != null) {
						bounds = itemAfter.DrawingBounds;
						bottom = bounds.Top - 1;
					}
					separator = Rectangle.FromLTRB(bounds.Left, top, bounds.Right, bottom);
				}
				else {
					Rectangle drawingBounds = itemBefore.DrawingBounds;
					top = drawingBounds.Bottom + 1;
					separator = Rectangle.FromLTRB(drawingBounds.Left, top, drawingBounds.Right, itemAfter.DrawingBounds.Top - 1);
				}
				ObjectPainter.DrawObject(cache, drawingContext.Painters.GroupPainter.ObjectPainter,
					new GroupInfoArgs(cache, separator, drawingContext.Appearances.GroupAppearance, DragGroupState.Hot));
			}
		}
		public override IDropAction GetDropAction(Point point, IDragObject dragObject) {
			if(!Bounds.Contains(point))
				return null;
			if(items.Count == 1 && items[0].Bounds.Contains(point))
				return new SetDragItemDropAction(dragObject, this, items[0]);
			for(int i = 0; i < items.Count; i++) {
				DragItem previousItem = i > 0 ? items[i - 1] : null;
				DragItem nextItem = i + 1 < items.Count ? items[i + 1] : null;
				DragItem currentItem = items[i];
				if(currentItem.Bounds.Contains(point)) {
					if(point.Y < currentItem.Bounds.Y + currentItem.Bounds.Height / 4 + 1)
						return GetItemDropActionForTopQuarter(dragObject, previousItem, currentItem);
					if(point.Y > (currentItem.Bounds.Y + currentItem.Bounds.Height) - currentItem.Bounds.Height / 4 && currentItem.DataItem != null)
						return GetItemDropActionForBottomQuarter(dragObject, nextItem, currentItem);
					if(dragObject.DragItem == currentItem)
						return new SelfDropAction(dragObject);
					return new SetDragItemDropAction(dragObject, this, currentItem);
				}
				Rectangle spaceAfter = new Rectangle(currentItem.Bounds.X, currentItem.Bounds.Y + currentItem.Bounds.Height, currentItem.Bounds.Width, SpaceBetweenItems);
				if(items.Count != 1 && currentItem.DataItem != null && spaceAfter.Contains(point))
					return new InsertDragItemDropAction(dragObject, this, nextItem, currentItem);
			}
			return null;
		}
		IDropAction GetItemDropActionForBottomQuarter(IDragObject dragObject, DragItem nextItem, DragItem currentItem) {
			if(dragObject.DragItem != null && (dragObject.DragItem == currentItem || dragObject.DragItem == nextItem))
				return new SelfDropAction(dragObject);
			return new InsertDragItemDropAction(dragObject, this, currentItem, nextItem);
		}
		IDropAction GetItemDropActionForTopQuarter(IDragObject dragObject, DragItem previousItem, DragItem nextItem) {
			if(dragObject.DragItem != null && (dragObject.DragItem == previousItem || dragObject.DragItem == nextItem))
				return new SelfDropAction(dragObject);
			return new InsertDragItemDropAction(dragObject, this, previousItem, nextItem);
		}
		public override void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record) {
			int elementIndex = record.ElementIndex;
			TDataItem dataItem = (TDataItem)record.Data;
			switch (record.Operation) {
				case DragAreaHistoryItemOperation.Remove:
					if(elementIndex < DataItemsCount)
						dataItems.RemoveAt(elementIndex);
					break;
				case DragAreaHistoryItemOperation.Set:
					if(elementIndex < DataItemsCount) {
						DragItem dragItem = FindDragItem((TDataItem)dataItems[elementIndex]);
						foreach(TDataItem item in dragItem.DataItems)
							dataItems.Remove(item);
						dataItems.Insert(elementIndex, dataItem);
					}
					else
						dataItems.Add(dataItem);
					break;
				default:
					if (elementIndex >= DataItemsCount)
						dataItems.Add(dataItem);
					else
						dataItems.Insert(elementIndex, dataItem);
					break;
			}
		}
		int IndexOf(DragItem dragItem) {
			return items.IndexOf(dragItem);
		}
		public override int ActualIndexOf(DragItem dragItem) {
			int result = 0;
			int dragItemIndex = IndexOf(dragItem);
			for(int i = 0; i < dragItemIndex; i++) {
				result += items[i].DataItems.Count;
			}
			return result;
		}
		protected override void AfterBoundsCalculation(Rectangle bounds) {
			dropBounds = bounds;
		}
		int IndexOf(TDataItem dataItem) {
			return dataItems.IndexOf(dataItem);
		}
		public override DataItem GetDataItemByIndex(int index) {
			return (DataItem)dataItems[index];
		}
		protected override void ExecuteDragAction(DragAreaHistoryItem historyItem, IDragObject dragObject, bool isSameDragGroup) {
			IList<TDataItem> dataItems = CreateDataItems(dragObject);
			ExecuteDragAction(historyItem, dataItems);
		}
		void ExecuteDragAction(DragAreaHistoryItem historyItem, IList<TDataItem> dataItems) {
			for(int i = 0; i < dataItems.Count; i++) {
				TDataItem dataItem = dataItems[i];
				int elementIndex = IndexOf(dataItem) - i;
				DragAreaHistoryItemRecord redoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Remove, dataItem, SectionIndex, 0, elementIndex);
				historyItem.RedoRecords.Add(redoRecord);
				DragAreaHistoryItemRecord undoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Insert, dataItem, SectionIndex, 0, elementIndex);
				historyItem.UndoRecords.Insert(0, undoRecord);
			}
		}
		public override void ClearContent(DragAreaHistoryItem historyItem) {
			ExecuteDragAction(historyItem, dataItems);
		}
		protected abstract IList<TDataItem> CreateDataItems(IDragObject dragObject);
		IList<DataItem> IDataItemsCreator.CreateDataItems(IDragObject dragObject) {
			return new List<DataItem>(CreateDataItems(dragObject));
		}
		public abstract bool AcceptableDragObject(IDragObject dragObject);
	}
}
