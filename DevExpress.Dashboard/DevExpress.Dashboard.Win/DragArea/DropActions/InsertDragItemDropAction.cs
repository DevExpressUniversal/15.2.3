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
namespace DevExpress.DashboardWin.Native {
	public class InsertDragItemDropAction : IDropAction {
		readonly DragGroup destinationGroup;
		readonly DragItem previousItem;
		readonly DragItem nextItem;
		readonly IDragObject dragObject;
		readonly int itemIndex;
		public InsertDragItemDropAction(IDragObject dragObject, DragGroup destinationGroup, DragItem previousItem, DragItem nextItem) {
			this.dragObject = dragObject;
			this.destinationGroup = destinationGroup;
			this.previousItem = previousItem;
			this.nextItem = nextItem;
			itemIndex = destinationGroup.ActualIndexOf(nextItem);
		}
		IHistoryItem IDropAction.PerformDrop() {
			DragArea area = destinationGroup.Section.Area;
			DragAreaHistoryItem historyItem = null;
			if(dragObject.DragSource != null)
				historyItem = dragObject.DragSource.PerformDrag(dragObject, dragObject.SameDragGroup(destinationGroup)) as DragAreaHistoryItem;
			if(historyItem == null) {
				DataDashboardItem dashboardItem = area.DashboardItem;
				historyItem = new DragAreaHistoryItem(dashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			}
			int actualItemIndex = itemIndex;
			int sectionIndex = destinationGroup.SectionIndex;
			int totalCorrection = 0;
			for(int i = 0; i < historyItem.RedoRecords.Count; i++) {
				DragAreaHistoryItemRecord record = historyItem.RedoRecords[i];
				if(record.SectionIndex == sectionIndex && record.GroupIndex == 0 && record.ElementIndex - totalCorrection < actualItemIndex) {
					totalCorrection++;
				}
			}
			actualItemIndex -= totalCorrection;
			IDataItemsCreator dataItemsCreator = destinationGroup as IDataItemsCreator;
			if(dataItemsCreator != null) {
				IList<DataItem> dataItems = dataItemsCreator.CreateDataItems(dragObject);
				for(int i = 0; i < dataItems.Count; i++) {
					DragAreaHistoryItemRecord redoItemRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Insert, dataItems[i], sectionIndex, 0, actualItemIndex + i);
					historyItem.RedoRecords.Add(redoItemRecord);
					DragAreaHistoryItemRecord undoItemRecord = new DragAreaHistoryItemRecord(area, DragAreaHistoryItemOperation.Remove, null, sectionIndex, 0, actualItemIndex + i);
					historyItem.UndoRecords.Insert(0, undoItemRecord);
				}
			}
			return historyItem;
		}
		void IDropAction.ShowIndicator(bool visible) {
			if(previousItem != null)
				previousItem.SizeState = visible ? DragItemSizeState.ShrunkFromNext : DragItemSizeState.Normal;
			nextItem.SizeState = visible ? DragItemSizeState.ShrunkFromPrevious : DragItemSizeState.Normal;
			destinationGroup.Section.Area.Invalidate();
		}
	}
}
