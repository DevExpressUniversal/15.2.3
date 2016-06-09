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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class InsertDragGroupDropAction<THolder> : IDropAction where THolder : class, IDataItemHolder {
		readonly HolderCollectionDragGroup<THolder> previousGroup;
		readonly HolderCollectionDragGroup<THolder> nextGroup;
		readonly IDragObject dragObject;
		readonly DragArea area;
		readonly int sectionIndex;
		public InsertDragGroupDropAction(IDragObject dragObject, HolderCollectionDragGroup<THolder> previousGroup, HolderCollectionDragGroup<THolder> nextGroup) {
			this.dragObject = dragObject;
			this.previousGroup = previousGroup;
			this.nextGroup = nextGroup;
			this.area = previousGroup != null ? previousGroup.Section.Area : nextGroup.Section.Area;
			this.sectionIndex = previousGroup != null ? previousGroup.SectionIndex : nextGroup.SectionIndex;
		}
		int GetGroupIndex() {
			DragGroup dragSourceGroup = dragObject.GetGroup();
			int nextGroupIndex = nextGroup.Section.IndexOf(nextGroup);
			if(dragObject.IsDataField || nextGroupIndex <= dragSourceGroup.Section.IndexOf(dragSourceGroup))
				return nextGroupIndex;
			if(OldGroupWillBeRemoved())
				return nextGroupIndex - 1;
			return nextGroupIndex;
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
		public void ShowIndicator(bool visible) {
			if(previousGroup != null) {
				previousGroup.SizeState = visible ? DragItemSizeState.ShrunkFromNext : DragItemSizeState.Normal;
				if(previousGroup.ItemList.Count != 0)
					previousGroup.ItemList[previousGroup.ItemList.Count - 1].SizeState = visible ? DragItemSizeState.ShrunkFromNext : DragItemSizeState.Normal;
			}
			if(nextGroup != null) {
				nextGroup.SizeState = visible ? DragItemSizeState.ShrunkFromPrevious : DragItemSizeState.Normal;
				if(nextGroup.ItemList.Count != 0)
					nextGroup.ItemList[0].SizeState = visible ? DragItemSizeState.ShrunkFromPrevious : DragItemSizeState.Normal;
			}
			area.Invalidate();
		}
		public IHistoryItem PerformDrop() {
			HolderCollectionDragGroup<THolder> group = null;
			if(dragObject.IsGroup)
				group = dragObject.GetGroup() as HolderCollectionDragGroup<THolder>;
			if(group == null) {
				HolderCollectionDragSection<THolder> section = area.Sections[sectionIndex] as HolderCollectionDragSection<THolder>;
				group = section.CreateGroup(null);
			}
			int newGroupIndex = GetGroupIndex();
			IList<THolder> holders = dragObject.IsGroup ? group.Holders : group.CreateHolders(dragObject);
			DragAreaHistoryItem historyItem = null;
			if(dragObject.DragSource != null)
				historyItem = dragObject.DragSource.PerformDrag(dragObject, false) as DragAreaHistoryItem;
			if(historyItem == null)
				historyItem = new DragAreaHistoryItem(area.DashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			for(int i = 0; i < holders.Count; i++) {
				int actualGroupIndex = i == 0 ? newGroupIndex : newGroupIndex + 1;
				DragAreaHistoryItemRecord insertContainerRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.InsertGroup, holders[i], sectionIndex, actualGroupIndex, 0);
				historyItem.RedoRecords.Add(insertContainerRedoRecord);
				DragAreaHistoryItemRecord insertContainerUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.RemoveGroup, holders[i], sectionIndex, newGroupIndex, 0);
				historyItem.UndoRecords.Insert(0, insertContainerUndoRecord);
				if(!dragObject.IsGroup) {
					List<DataItem> dataItems = (List<DataItem>)group.CreateDataItems(holders[0], dragObject, 0);
					DragAreaHistoryItemRecord setRedoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, dataItems[i], sectionIndex, actualGroupIndex, 0);
					historyItem.RedoRecords.Add(setRedoRecord);
					DragAreaHistoryItemRecord setUndoRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Set, null, sectionIndex, newGroupIndex, 0);
					historyItem.UndoRecords.Insert(0, setUndoRecord);
				}
			}
			return historyItem;
		}
	}
}
