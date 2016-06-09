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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using System;
namespace DevExpress.DashboardWin.Native {
	public class HolderCollectionDragGroup<THolder> : HolderDragGroupBase<THolder> where THolder : class, IDataItemHolder {
		public bool IsNewGroup { get { return UnlimitedSection.IsNewGroup(this); } }
		public int GroupIndex { get { return UnlimitedSection.GetGroupIndex(this); } }
		HolderCollectionDragSection<THolder> UnlimitedSection { get { return (HolderCollectionDragSection<THolder>)Section; } }
		public HolderCollectionDragGroup(string optionsButtonImageName, THolder holder)
			: base(optionsButtonImageName, holder) {
		}
		public void AddContainer(THolder holder) {
			Holders.Add(holder);
			UpdateNames();
			UpdateDataItems();
		}
		public override IDropAction GetDropAction(Point point, IDragObject dragObject) {
			if(AcceptableDragObject(dragObject)) {
				foreach(DragItem dragItem in Items)
					if(dragItem.Bounds.Contains(point))
						return !dragObject.SameDataItem(dragItem.DataItem) ? (IDropAction)new HolderCollectionDragGroupDropAction<THolder>(dragObject, this, dragItem) : new SelfDropAction(dragObject);
			}
			return null;
		}
		public override void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record) {
			int elementIndex = record.ElementIndex;
			if(record.Operation == DragAreaHistoryItemOperation.Set && elementIndex < Names.Count)
				SetDataItem(elementIndex, record.HolderIndex, (DataItem)record.Data);
		}
		public override void Cleanup() {
			UnlimitedSection.CleanupGroups();
		}
		public virtual IList<THolder> CreateHolders(IDragObject dragObject) {
			return Holders;
		}
		public virtual IList<DataItem> CreateDataItems(THolder targetHolder, IDragObject dragObject, int itemIndex) {
			return new List<DataItem> { dragObject.GetMeasure(Section.Area.DashboardItem) };
		}
		protected virtual bool AcceptableDragObject(IDragObject dragObject) {
			return dragObject.GetMeasure(Section.Area.DashboardItem) != null;
		}
		protected override void ExecuteDragAction(DragAreaHistoryItem historyItem, IDragObject dragObject, bool isSameDragGroup) {
			DragItem dragItem = dragObject.DragItem;
			DragArea area = Section.Area;
			int sectionIndex = SectionIndex;
			int groupIndex = GroupIndex;
			if(dragObject.IsGroup) {
				for(int i = 0; i < Holders.Count; i++) {
					DragAreaHistoryItemRecord removeContainerRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.RemoveGroup, Holders[i], sectionIndex, groupIndex, 0);
					historyItem.RedoRecords.Add(removeContainerRedoRecord);
					DragAreaHistoryItemRecord removeContainerUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.InsertGroup, Holders[i], sectionIndex, groupIndex, 0);
					historyItem.UndoRecords.Insert(0, removeContainerUndoRecord);
				}
			}
			else if(dragItem.DataItems.Count > 1) {
				for(int i = 0; i < dragItem.DataItems.Count; i++) {
					DragAreaHistoryItemRecord setRedoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Set, null, sectionIndex, groupIndex, 0, 0);
					historyItem.RedoRecords.Add(setRedoRecord);
					DragAreaHistoryItemRecord setUndoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Set, dragItem.DataItems[i], sectionIndex, groupIndex, 0, -1);
					historyItem.UndoRecords.Insert(0, setUndoRecord);
					DragAreaHistoryItemRecord removeContainerRedoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.RemoveGroup, Holders[i], SectionIndex, GroupIndex, 0);
					historyItem.RedoRecords.Add(removeContainerRedoRecord);
					DragAreaHistoryItemRecord removeContainerUndoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.InsertGroup, Holders[i], SectionIndex, GroupIndex, 0);
					historyItem.UndoRecords.Insert(0, removeContainerUndoRecord);
				}
			}
			else {
				int itemIndex = IndexOf(dragItem);
				DataItem dataItem = GetDataItem(itemIndex);
				DragAreaHistoryItemRecord setRedoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Set, null, sectionIndex, groupIndex, itemIndex);
				historyItem.RedoRecords.Add(setRedoRecord);
				DragAreaHistoryItemRecord setUndoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.Set, dataItem, sectionIndex, groupIndex, itemIndex);
				historyItem.UndoRecords.Insert(0, setUndoRecord);
				if(Holder.Count == 1 && !isSameDragGroup) {
					DragAreaHistoryItemRecord removeGroupRedoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.RemoveGroup, Holder, sectionIndex, groupIndex, 0);
					historyItem.RedoRecords.Add(removeGroupRedoRecord);
					DragAreaHistoryItemRecord removeGroupUndoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.InsertGroup, Holder, sectionIndex, groupIndex, 0);
					historyItem.UndoRecords.Insert(0, removeGroupUndoRecord);
				}
			}
		}
		public override void ClearContent(DragAreaHistoryItem historyItem) {
			int sectionIndex = SectionIndex;
			int groupIndex = 0;
			for(int i = 0; i < Holders.Count; i++) {
				DragAreaHistoryItemRecord removeContainerRedoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.RemoveGroup, Holders[i], sectionIndex, groupIndex, 0);
				historyItem.RedoRecords.Add(removeContainerRedoRecord);
				DragAreaHistoryItemRecord removeContainerUndoRecord = new DragAreaHistoryItemRecord(Section.Area, DragAreaHistoryItemOperation.InsertGroup, Holders[i], sectionIndex, groupIndex, 0);
				historyItem.UndoRecords.Insert(0, removeContainerUndoRecord);
			}
		}
	}
}
