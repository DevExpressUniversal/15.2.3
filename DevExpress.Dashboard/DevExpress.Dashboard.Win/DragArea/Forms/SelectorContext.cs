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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using System;
using System.ComponentModel.Design;
namespace DevExpress.DashboardWin.Native {
	public class ChartSelectorContext {
		readonly DashboardDesigner designer;
		readonly IChartSeriesSection section;
		readonly ChartSeries series;
		readonly int groupIndex;
		readonly DashboardWinStringId historyItemCaptionId;
		readonly bool interactivityOnArgumentsEnabled;
		DesignerTransaction transaction;
		ChartSeriesOptions initialOptions;
		ChartSeriesOptions options;
		SelectorContextHistoryItem historyItem;
		public ChartSeries Series { get { return series; } }
		public ChartSeriesOptions Options { get { return options; } }
		public bool InteractivityOnArgumentsEnabled { get { return interactivityOnArgumentsEnabled; } }
		public ChartSelectorContext(DashboardDesigner designer, IChartSeriesSection section, ChartSeries series, int groupIndex, DashboardWinStringId historyItemCaptionId) {
			initialOptions = new ChartSeriesOptions(this, series);
			options = new ChartSeriesOptions(this, series);
			this.designer = designer;
			this.section = section;
			this.series = series;
			this.groupIndex = groupIndex;
			this.historyItemCaptionId = historyItemCaptionId;
			ChartDashboardItem chartDashboardItem = section.Area.DashboardItem as ChartDashboardItem;
			if(chartDashboardItem != null) {
				this.interactivityOnArgumentsEnabled = (chartDashboardItem.InteractivityOptions.IsDrillDownEnabled || chartDashboardItem.InteractivityOptions.MasterFilterMode != DashboardItemMasterFilterMode.None)
					&& chartDashboardItem.InteractivityOptions.TargetDimensions != TargetDimensions.Series;
			}
		}
		internal void AddRedoRecord(SelectorContextHistoryItem historyItem, int groupIndex, object data) {
			historyItem.RedoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.CustomOperation, data, section.SectionIndex, groupIndex, 0));
		}
		internal void AddUndoRecord(SelectorContextHistoryItem historyItem, int groupIndex, object data) {
			historyItem.UndoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.CustomOperation, data, section.SectionIndex, groupIndex, 0));
		}
		internal void AddToHistoryItemChangeOption(SelectorContextHistoryItem historyItem, ICustomOperation<ChartSeries> redo, ICustomOperation<ChartSeries> undo) {
			AddRedoRecord(historyItem, groupIndex, redo);
			AddUndoRecord(historyItem, groupIndex, undo);
		}
		public void ApplyHistoryItem() {
			if(historyItem != null) {
				if(transaction != null) {
					Dashboard dashboard = designer.Dashboard;
					DataDashboardItem dashboardItem = section.Area.DashboardItem;
					IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
					changeService.OnComponentChanged(dashboardItem, null, null, null);
					transaction.Commit();
				}
				designer.History.Add(historyItem);
			}
		}
		public void Execute(ChartSeriesConverter converter) {
			designer.DragAreaScrollableControl.DragArea.BeginUpdate();
			DataDashboardItem dashboardItem = section.Area.DashboardItem;
			SelectorContextHistoryItem newHistoryItem = new SelectorContextHistoryItem(dashboardItem, historyItemCaptionId);
			bool isNewGroup = groupIndex < 0;
			dashboardItem.Dashboard.BeginUpdate();
			if(!isNewGroup)
				dashboardItem.LockChanging();
			try {
				if(historyItem != null)
					historyItem.Undo(designer);
				if(isNewGroup) {
					ChartSeries newSeries = converter.CreateSeries();
					newSeries.AssignOptions(options);
					newHistoryItem.RedoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.RemoveGroup, section.DefaultSeries, section.SectionIndex, groupIndex, 0));
					newHistoryItem.RedoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.InsertGroup, newSeries, section.SectionIndex, groupIndex, 0));
					newHistoryItem.UndoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.RemoveGroup, newSeries, section.SectionIndex, groupIndex, 0));
					newHistoryItem.UndoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.InsertGroup, section.DefaultSeries, section.SectionIndex, groupIndex, 0));
				} else {
					IList<ChartSeries> newSeries = converter.PrepareSeries(series);
					newHistoryItem.RedoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.RemoveGroup, series, section.SectionIndex, groupIndex, 0));
					newHistoryItem.UndoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.InsertGroup, series, section.SectionIndex, groupIndex, 0));
					initialOptions.AddUndoRecords(newHistoryItem, groupIndex);
					int index = groupIndex;
					for(int i = 0; i < newSeries.Count; i++, index++) {
						newHistoryItem.RedoRecords.Add(new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.InsertGroup, newSeries[i], section.SectionIndex, index, 0));
						options.AddRedoRecords(newHistoryItem, index);
						newHistoryItem.UndoRecords.Insert(0, new DragAreaHistoryItemRecord(section.Area, DragAreaHistoryItemOperation.RemoveGroup, newSeries[i], section.SectionIndex, index, 0));
					}
				}
				if(designer.IsDashboardVSDesignMode) {
					if(transaction == null) {
						Dashboard dashboard = designer.Dashboard;
						IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
						transaction = designerHost.CreateTransaction(newHistoryItem.Caption);
						IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
						changeService.OnComponentChanging(dashboardItem, null);
					}
				}
				newHistoryItem.Redo(designer);
				foreach(DragAreaHistoryItemRecord record in newHistoryItem.RedoRecords)
					if(record.Operation == DragAreaHistoryItemOperation.InsertGroup)
						section.SelectGroup(record.GroupIndex);
				section.SelectGroup(groupIndex);
			} finally {
				dashboardItem.Dashboard.EndUpdate();
				if(!isNewGroup)
					dashboardItem.UnlockChanging();
			}
			historyItem = newHistoryItem;
			designer.DragAreaScrollableControl.DragArea.EndUpdate();
		}
		public void Cancel() {
			if(transaction != null)
				transaction.Cancel();
			if(historyItem != null)
				historyItem.Cancel(designer);
		}
	}
}
