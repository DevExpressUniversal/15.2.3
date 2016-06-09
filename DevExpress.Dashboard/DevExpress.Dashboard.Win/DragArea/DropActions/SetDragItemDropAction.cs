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
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class SetDragItemDropAction : IDropAction {
		readonly DragGroup destinationGroup;
		readonly DragItem destinationDragItem;
		readonly IDragObject dragObject;
		readonly int itemIndex;
		public SetDragItemDropAction(IDragObject dragObject, DragGroup destinationGroup, DragItem destinationDragItem) {
			this.dragObject = dragObject;
			this.destinationGroup = destinationGroup;
			this.destinationDragItem = destinationDragItem;
			itemIndex = destinationGroup.ActualIndexOf(destinationDragItem);
		}
		IHistoryItem IDropAction.PerformDrop() {
			DragArea area = destinationGroup.Section.Area;
			DragAreaHistoryItem historyItem = null;
			if(dragObject.DragSource != null)
				historyItem = dragObject.DragSource.PerformDrag(dragObject, dragObject.SameDragGroup(destinationGroup)) as DragAreaHistoryItem;
			if(historyItem == null)
				historyItem = new DragAreaHistoryItem(area.DashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			int actualItemIndex = itemIndex;
			int actualDataItemsCount = destinationGroup.DataItemsCount;
			int totalCorrection = 0;
			for(int i = 0; i < historyItem.RedoRecords.Count; i++) {
				DragAreaHistoryItemRecord record = historyItem.RedoRecords[i];
				if(record.SectionIndex == destinationGroup.SectionIndex && record.GroupIndex == 0) {
					if(record.ElementIndex - totalCorrection < actualItemIndex)
						totalCorrection++;
					actualDataItemsCount--;
				}
			}
			actualItemIndex -= totalCorrection;
			IDataItemsCreator dataItemsCreator = destinationGroup as IDataItemsCreator;
			if(dataItemsCreator != null) {
				IList<DataItem> dataItems = dataItemsCreator.CreateDataItems(dragObject);
				for(int i = 0; i < dataItems.Count; i++) {
					DragAreaHistoryItemOperation operation = (actualItemIndex < actualDataItemsCount && i == 0) ?
						DragAreaHistoryItemOperation.Set :
						DragAreaHistoryItemOperation.Insert;
					DragAreaHistoryItemRecord record = new DragAreaHistoryItemRecord(area, operation, dataItems[i], destinationGroup.SectionIndex, 0, actualItemIndex + i);
					historyItem.RedoRecords.Add(record);
					DragAreaHistoryItemRecord undoItemRecord;
					if(operation == DragAreaHistoryItemOperation.Insert)
						undoItemRecord = new DragAreaHistoryItemRecord(destinationGroup.Section.Area, DragAreaHistoryItemOperation.Remove, null, destinationGroup.SectionIndex, 0, actualItemIndex + i);
					else
						undoItemRecord = new DragAreaHistoryItemRecord(destinationGroup.Section.Area, DragAreaHistoryItemOperation.Set, destinationGroup.GetDataItemByIndex(itemIndex + i), destinationGroup.SectionIndex, 0, actualItemIndex + i);
					historyItem.UndoRecords.Insert(0, undoItemRecord);
				}
			}
			return historyItem;
		}
		void IDropAction.ShowIndicator(bool visible) {
			if(visible)
				destinationDragItem.SetDropDestinationState();
			else
				destinationDragItem.ResetState();
			destinationGroup.Section.Area.Invalidate();
		}
	}
}
