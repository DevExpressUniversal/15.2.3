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
using DevExpress.Data.Filtering;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using System.ComponentModel.Design;
namespace DevExpress.DashboardWin.Native {
	public enum DragAreaHistoryItemOperation { Insert, Set, Remove, InsertGroup, RemoveGroup, CustomOperation };
	public class DragAreaHistoryItemRecord {
		readonly DragArea dragArea;
		readonly DragAreaHistoryItemOperation operation;
		readonly object data;
		readonly int sectionIndex;
		readonly int groupIndex;
		readonly int elementIndex;
		readonly int holderIndex;
		public int SectionIndex { get { return sectionIndex; } }
		public int GroupIndex { get { return groupIndex; ; } }
		public DragAreaHistoryItemOperation Operation { get { return operation; } }
		public object Data { get { return data; } }
		public int ElementIndex { get { return elementIndex; } }
		public int HolderIndex { get { return holderIndex; } }
		public DragAreaHistoryItemRecord(DragArea dragArea, DragAreaHistoryItemOperation operation, object data, int sectionIndex, int groupIndex, int elementIndex)
			: this(dragArea, operation, data, sectionIndex, groupIndex, elementIndex, -1) {
		}
		public DragAreaHistoryItemRecord(DragArea dragArea, DragAreaHistoryItemOperation operation, object data, int sectionIndex, int groupIndex, int elementIndex, int holderIndex) {
			this.dragArea = dragArea;
			this.data = data;
			this.sectionIndex = sectionIndex;
			this.groupIndex = groupIndex;
			this.elementIndex = elementIndex;
			this.holderIndex = holderIndex;
			this.operation = operation;
		}
		public void Execute() {
			dragArea.Sections[sectionIndex].ApplyHistoryItemRecord(this);
		}
	}
	public class DragAreaHistoryItemContext {
		public IDashboardDataSource UndoDataSource { get; private set; }
		public IDashboardDataSource RedoDataSource { get; private set; }
		public string UndoDataMember { get; private set; }
		public string RedoDataMember { get; private set; }
		public void InitializeUndo(IDashboardDataSource dataSource, string dataMember) {
			UndoDataSource = dataSource;
			UndoDataMember = dataMember;
		}
		public void InitializeRedo(IDashboardDataSource dataSource, string dataMember) {
			RedoDataSource = dataSource;
			RedoDataMember = dataMember;
		}
	}	
	public abstract class DragAreaHistoryItemBase : IHistoryItem {
		readonly DataDashboardItem dashboardItem;
		readonly DashboardWinStringId captionId;
		readonly List<DragAreaHistoryItemRecord> redoRecords = new List<DragAreaHistoryItemRecord>();
		readonly List<DragAreaHistoryItemRecord> undoRecords = new List<DragAreaHistoryItemRecord>();
		string previousFilterString;
		DimensionsHistoryContext dimensionsContext;
		FormatRulesHistoryContext formatRulesHistoryContext;
		DragAreaHistoryItemContext context;
		public string Caption { get { return string.Format(DashboardWinLocalizer.GetString(captionId), dashboardItem.Name); } }
		public List<DragAreaHistoryItemRecord> RedoRecords { get { return redoRecords; } }
		public List<DragAreaHistoryItemRecord> UndoRecords { get { return undoRecords; } }
		protected DataDashboardItem DashboardItem { get { return dashboardItem; } }
		protected DragAreaHistoryItemBase(DataDashboardItem dashboardItem, DashboardWinStringId captionId) {
			this.dashboardItem = dashboardItem;
			this.captionId = captionId;
		}
		void ExecuteRecords(DashboardDesigner designer, List<DragAreaHistoryItemRecord> records) {
			designer.SelectedDashboardItem = dashboardItem;
			foreach (DragAreaHistoryItemRecord record in records)
				record.Execute();
		}
		public virtual void Undo(DashboardDesigner designer) {
			BeginExecute(designer);
			try {
				ExecuteRecords(designer, undoRecords);
				if (dimensionsContext != null)
					dimensionsContext.Undo();
				if(formatRulesHistoryContext != null)
					formatRulesHistoryContext.Undo();
			}
			finally {
				EndExecute(designer);
			}
			if (dashboardItem != null) {
				if (context != null)
					dashboardItem.SetDataSource(context.UndoDataSource, context.UndoDataMember);
				dashboardItem.FilterString = previousFilterString;
			}
			UpdateDesignerDataSource(designer);
		}
		public virtual void Redo(DashboardDesigner designer) {
			previousFilterString = dashboardItem.FilterString;
			RedoInternal(designer);
			UpdateDesignerDataSource(designer);
		}
		protected void RedoInternal(DashboardDesigner designer) {
			BeginExecute(designer);
			try {
				ExecuteRedoRecords(designer);
				dimensionsContext = new DimensionsHistoryContext(dashboardItem);
				formatRulesHistoryContext = new FormatRulesHistoryContext(dashboardItem);
			}
			finally {
				EndExecute(designer);
			}
			if (context != null)
				dashboardItem.SetDataSource(context.RedoDataSource, context.RedoDataMember);
			else {
				context = new DragAreaHistoryItemContext();
				context.InitializeUndo(dashboardItem.DataSource, dashboardItem.DataMember);
				EnsureDashboardItemDataSource(designer);
				context.InitializeRedo(dashboardItem.DataSource, dashboardItem.DataMember);
			}
		}
		protected void ExecuteRedoRecords(DashboardDesigner designer) {
			ExecuteRecords(designer, redoRecords);
		}
		protected void EnsureDashboardItemDataSource(DashboardDesigner designer) {
			dashboardItem.EnsureDataSource(designer.SelectedDataSourceInfo);
		}
		protected void UpdateDesignerDataSource(DashboardDesigner designer) {
			designer.DragAreaScrollableControl.DragArea.UpdateDataSource();
		}
		void BeginExecute(DashboardDesigner designer) {
			dashboardItem.Dashboard.BeginUpdate();
			dashboardItem.LockRenamingMap();			
			DragAreaControl dragAreaControl = designer.DragAreaScrollableControl.DragArea;
			dragAreaControl.BeginUpdate();
		}
		void EndExecute(DashboardDesigner designer) {			
			DragAreaControl dragAreaControl = designer.DragAreaScrollableControl.DragArea;
			dragAreaControl.EndUpdate();			
			dashboardItem.UnlockRenamingMap();
			dashboardItem.Dashboard.EndUpdate();
		}
	}
	public class DragAreaHistoryItem : DragAreaHistoryItemBase {
		public DragAreaHistoryItem(DataDashboardItem dashboardItem, DashboardWinStringId captionId)
			: base(dashboardItem, captionId) {
		}
		public override void Redo(DashboardDesigner designer) {
			if(designer.IsDashboardVSDesignMode) {
				Dashboard dashboard = designer.Dashboard;
				IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
				DesignerTransaction transaction = designerHost.CreateTransaction(Caption);
				try {
					IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
					changeService.OnComponentChanging(DashboardItem, null);
					RedoInternal(designer);
					changeService.OnComponentChanged(DashboardItem, null, null, null);					
				}
				catch (InvalidOperationException) { 
					transaction.Cancel();
					throw;
				}
				transaction.Commit();
				UpdateDesignerDataSource(designer);
			}
			else
				base.Redo(designer);
		}
	}
	public class SelectorContextHistoryItem : DragAreaHistoryItemBase {
		public SelectorContextHistoryItem(DataDashboardItem dashboardItem, DashboardWinStringId captionId)
			: base(dashboardItem, captionId) {
		}
		public void Cancel(DashboardDesigner designer) {
			if(designer.IsDashboardVSDesignMode)
				UpdateDesignerDataSource(designer);
			else
				Undo(designer);
		}
	}
}
