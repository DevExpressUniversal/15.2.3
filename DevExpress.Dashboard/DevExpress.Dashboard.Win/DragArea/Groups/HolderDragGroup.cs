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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using System.Collections.Generic;
namespace DevExpress.DashboardWin.Native {
	public abstract class HolderDragGroup : HolderDragGroupBase<IDataItemHolder>, IDragObjectAcceptor, IDataItemsCreator {
		protected HolderDragGroup(string optionsButtonImageName, IDataItemHolder holder)
			: base(optionsButtonImageName, holder) {
			Holder.Changed += OnDataItemsChanged;
		}
		public override IDropAction GetDropAction(Point point, IDragObject dragObject) {
			foreach(DragItem dragItem in Items) {
				if(dragItem.Bounds.Contains(point)) {
					DataItem dataItem = CreateDataItems(dragObject)[0];
					if (!dragObject.SameDataItem(dragItem.DataItem))
						return Holder.IsCompatible(dataItem, dragObject.DataSourceSchema) ? new SetDragItemDropAction(dragObject, this, dragItem) : null;
					return new SelfDropAction(dragObject);
				}
			}
			return null;
		}
		public override void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record) {
			int elementIndex = record.ElementIndex;
			if(elementIndex < Names.Count) {
				switch(record.Operation) {
					case DragAreaHistoryItemOperation.Set:
					case DragAreaHistoryItemOperation.Insert:
						SetDataItem(elementIndex, record.HolderIndex, (DataItem)record.Data);
						break;
					case DragAreaHistoryItemOperation.Remove:
						SetDataItem(elementIndex, record.HolderIndex, null);
						break;
				}
			}
		}
		public abstract IList<DataItem> CreateDataItems(IDragObject dragObject);
		public abstract bool AcceptableDragObject(IDragObject dragObject);
		protected override void ExecuteDragAction(DragAreaHistoryItem historyItem, IDragObject dragObject, bool isSameDragGroup) {
			ExecuteDragAction(historyItem, dragObject.DragItem);
		}
		public override void ClearContent(DragAreaHistoryItem historyItem) {
			foreach(DragItem dragItem in Items)
				ExecuteDragAction(historyItem, dragItem);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				Holder.Changed -= OnDataItemsChanged;
			base.Dispose(disposing);
		}
		void ExecuteDragAction(DragAreaHistoryItem historyItem, DragItem dragItem) {
			int sectionIndex = SectionIndex;
			int elementIndex = IndexOf(dragItem);
			DataItem dataItem = dragItem.DataItem;
			DragAreaHistoryItemRecord redoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Remove, dataItem, sectionIndex, 0, elementIndex);
			historyItem.RedoRecords.Add(redoRecord);
			DragAreaHistoryItemRecord undoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Insert, dataItem, sectionIndex, 0, elementIndex);
			historyItem.UndoRecords.Insert(0, undoRecord);
		}
		void OnDataItemsChanged(object sender, EventArgs e) {
			UpdateDataItems();
		}
	}
}
