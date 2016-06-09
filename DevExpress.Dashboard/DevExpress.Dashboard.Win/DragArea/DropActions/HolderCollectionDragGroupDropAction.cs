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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class HolderCollectionDragGroupDropAction<THolder> : IDropAction where THolder: class, IDataItemHolder {
		readonly HolderCollectionDragGroup<THolder> destinationGroup;
		readonly DragItem destinationDragItem;
		readonly IDragObject dragObject;
		public HolderCollectionDragGroupDropAction(IDragObject dragObject, HolderCollectionDragGroup<THolder> destinationGroup, DragItem destinationDragItem) {
			this.dragObject = dragObject;
			this.destinationGroup = destinationGroup;
			this.destinationDragItem = destinationDragItem;
		}
		void IDropAction.ShowIndicator(bool visible) {
			if(visible)
				destinationDragItem.SetDropDestinationState();
			else
				destinationDragItem.ResetState();
			destinationGroup.Section.Area.Invalidate();
		}
		int GetGroupIndex() {
			DragGroup dragSourceGroup = dragObject.GetGroup();
			int newGroupIndex = destinationGroup.Section.IndexOf(destinationGroup);
			if(dragObject.IsDataField || newGroupIndex <= dragSourceGroup.Section.IndexOf(dragSourceGroup))
				return newGroupIndex;
			if(OldGroupWillBeRemoved())
				return newGroupIndex - 1;
			return newGroupIndex;
		}
		bool OldGroupWillBeRemoved() {
			if(dragObject.IsGroup)
				return true;
			DragGroup sourceGroup = dragObject.GetGroup();
			HolderCollectionDragGroup<THolder> holderSourceGroup = sourceGroup as HolderCollectionDragGroup<THolder>;
			if(holderSourceGroup != null && holderSourceGroup.Holder.Count == 1)
				return true;
			return false;
		}
		IHistoryItem IDropAction.PerformDrop() {
			DragSection section = destinationGroup.Section;
			DragArea area = section.Area;
			DataDashboardItem dashboardItem = area.DashboardItem;
			DragAreaHistoryItem historyItem = null;
			if(dragObject.DragSource != null)
				historyItem = dragObject.DragSource.PerformDrag(dragObject, dragObject.SameDragGroup(destinationGroup)) as DragAreaHistoryItem;
			if(historyItem == null) 
				historyItem = new DragAreaHistoryItem(dashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			int sectionIndex = destinationGroup.SectionIndex;
			int groupIndex = GetGroupIndex();
			int itemIndex = destinationGroup.IndexOf(destinationDragItem);
			IList<THolder> holders = destinationGroup.CreateHolders(dragObject);
			IList<DataItem> dataItems = destinationGroup.CreateDataItems(holders[0], dragObject, itemIndex);
			IList<DataItem> currentDataItems = destinationDragItem.DataItems;
			IList<THolder> currentHolders = destinationGroup.Holders;
			bool removeCurrentGroup = !destinationGroup.IsNewGroup && !object.ReferenceEquals(holders, currentHolders);
			if(removeCurrentGroup) {
				for(int i = currentDataItems.Count - 1; i >= 0; i--) {
					int actualGroupIndex = i == 0 ? groupIndex : groupIndex + 1;
					DragAreaHistoryItemRecord setItemToNullRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, null, sectionIndex, groupIndex, itemIndex);
					historyItem.RedoRecords.Add(setItemToNullRedoRecord);
					DragAreaHistoryItemRecord setItemToNullUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, currentDataItems[i], sectionIndex, actualGroupIndex, itemIndex);
					historyItem.UndoRecords.Insert(0, setItemToNullUndoRecord);
					DragAreaHistoryItemRecord removeContainerRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.RemoveGroup, currentHolders[i], sectionIndex, groupIndex, itemIndex);
					historyItem.RedoRecords.Add(removeContainerRedoRecord);
					DragAreaHistoryItemRecord removeContainerUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.InsertGroup, currentHolders[i], sectionIndex, actualGroupIndex, itemIndex);
					historyItem.UndoRecords.Insert(0, removeContainerUndoRecord);
				}
			}
			bool addNewGroup = destinationGroup.IsNewGroup || removeCurrentGroup;
			for(int i = 0; i < dataItems.Count; i++) {
				int actualGroupIndex = i == 0 ? groupIndex : groupIndex + 1;
				if(addNewGroup) {
					DragAreaHistoryItemRecord insertContainerRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.InsertGroup, holders[i], sectionIndex, actualGroupIndex, itemIndex);
					historyItem.RedoRecords.Add(insertContainerRedoRecord);
					DragAreaHistoryItemRecord insertContainerUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.RemoveGroup, holders[i], sectionIndex, groupIndex, itemIndex);
					historyItem.UndoRecords.Insert(0, insertContainerUndoRecord);
				}
				DragAreaHistoryItemRecord setRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, dataItems[i], sectionIndex, actualGroupIndex, itemIndex);
				historyItem.RedoRecords.Add(setRedoRecord);
				DragAreaHistoryItemRecord setUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, addNewGroup ? null : destinationDragItem.DataItems[i], sectionIndex, groupIndex, itemIndex);
				historyItem.UndoRecords.Insert(0, setUndoRecord);
			}
			return historyItem;
		}
	}
}
