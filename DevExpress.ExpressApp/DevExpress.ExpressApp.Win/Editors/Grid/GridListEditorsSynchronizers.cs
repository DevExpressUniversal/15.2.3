#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data;
using DevExpress.Data.Summary;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraGrid.Views.BandedGrid;
namespace DevExpress.ExpressApp.Win.Editors {
	public static class GridControlModelSynchronizer {
		private static IModelSynchronizable CreateGridViewColumnsModelSynchronizer(IModelListView model, GridListEditor gridListEditor, bool canShowBands) {
			if(canShowBands) {
				return new BandedGridViewColumnsModelSynchronizer(gridListEditor, model);
			}
			else {
				return new GridViewColumnsModelSynchronizer(gridListEditor, model);
			}
		}
		public static void ApplyModel(IModelListView model, GridListEditor gridListEditor) {
			bool canShowBands = WinColumnsListEditor.GetCanShowBands(model);
			new FooterVisibleModelSynchronizer(gridListEditor, model).ApplyModel();
			new FilterModelSynchronizer(gridListEditor, model).ApplyModel();
			new GridViewModelSynchronizer(gridListEditor, gridListEditor.GridView, model).ApplyModel();
			CreateGridViewColumnsModelSynchronizer(model, gridListEditor, canShowBands).ApplyModel(); 
			new GridSummaryModelSynchronizer(gridListEditor.GridView, model).ApplyModel();
			if(canShowBands) {
				new XafBandedGridViewModelSynchronizer((BandedGridView)gridListEditor.GridView, (IModelBandsLayoutWin)model.BandsLayout).ApplyModel();
			}
		}
		public static void SaveModel(IModelListView model, GridListEditor gridListEditor) {
			bool canShowBands = WinColumnsListEditor.GetCanShowBands(model);
			new FooterVisibleModelSynchronizer(gridListEditor, model).SynchronizeModel();
			new FilterModelSynchronizer(gridListEditor, model).SynchronizeModel();
			new GridViewModelSynchronizer(gridListEditor, gridListEditor.GridView, model).SynchronizeModel();
			CreateGridViewColumnsModelSynchronizer(model, gridListEditor, canShowBands).SynchronizeModel();
			new GridSummaryModelSynchronizer(gridListEditor.GridView, model).SynchronizeModel();
			if(canShowBands) {
				new XafBandedGridViewModelSynchronizer((BandedGridView)gridListEditor.GridView, (IModelBandsLayoutWin)model.BandsLayout).SynchronizeModel();
			}
		}
	}
	public class GridViewModelSynchronizer : ModelSynchronizer<GridView, IModelListView> {
		private WinColumnsListEditor gridListEditor;
		public GridViewModelSynchronizer(WinColumnsListEditor gridListEditor, GridView gridView, IModelListView model)
			: base(gridView, model) {
			this.gridListEditor = gridListEditor;
			gridListEditor.ControlsCreated += new EventHandler(gridListEditor_ControlsCreated);
		}
		private void gridListEditor_ControlsCreated(object sender, EventArgs e) {
			Control.OptionsView.ShowFooter = Model.IsFooterVisible;
			Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
			Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
		}
		protected override void ApplyModelCore() {
			Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
			Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
			if(Model is IModelListViewShowAutoFilterRow) {
				Control.OptionsView.ShowAutoFilterRow = ((IModelListViewShowAutoFilterRow)Model).ShowAutoFilterRow;
			}
			if(Model is IModelListViewShowFindPanel) {
				if(((IModelListViewShowFindPanel)Model).ShowFindPanel) {
					Control.ShowFindPanel();
				}
				else {
					Control.HideFindPanel();
				}
			}
		}
		public override void SynchronizeModel() {
			Model.AutoExpandAllGroups = Control.OptionsBehavior.AutoExpandAllGroups;
			Model.IsGroupPanelVisible = Control.OptionsView.ShowGroupPanel;
			if(Model is IModelListViewShowAutoFilterRow) {
				((IModelListViewShowAutoFilterRow)Model).ShowAutoFilterRow = Control.OptionsView.ShowAutoFilterRow;
			}
			if(Model is IModelListViewShowFindPanel) {
				((IModelListViewShowFindPanel)Model).ShowFindPanel = Control.IsFindPanelVisible;
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(gridListEditor != null) {
				gridListEditor.ControlsCreated -= new EventHandler(gridListEditor_ControlsCreated);
				gridListEditor = null;
			}
		}
	}
	public class GridSummaryModelSynchronizer : SummaryModelSynchronizer<GridView, IModelListView> {
		bool needForceApplyModel = false;
		public GridSummaryModelSynchronizer(GridView gridView, IModelListView model)
			: base(gridView, model) {
		}
		protected override void ApplyModelCore() {
			if(CanApplyModel) {
				needForceApplyModel = false;
				Control.DataSourceChanged -= Control_DataSourceChanged;
				base.ApplyModelCore();
				ApplySortGroupSummary();
			}
			else {
				needForceApplyModel = true;
				Control.DataSourceChanged -= Control_DataSourceChanged;
				Control.DataSourceChanged += Control_DataSourceChanged;
			}
		}
		public override void Dispose() {
			Control.DataSourceChanged -= Control_DataSourceChanged;
			base.Dispose();
		}
		private void Control_DataSourceChanged(object sender, EventArgs e) {
			if(needForceApplyModel) {
				ApplyModelCore();
			}
		}
#if DebugTest
		public bool CanApplyModel_ForTests = false;
#endif
		private bool CanApplyModel {
			get {
#if DebugTest
				if(CanApplyModel_ForTests) {
					return true;
				}
#endif
				return Control.DataSource != null;
			}
		}
		public override void SynchronizeModel() {
			base.SynchronizeModel();
			SynchronizeGroupSummary();
		}
		protected override void ApplyColumnDisplayFormatToGroupSummary() {
			foreach(ISummaryItem summaryItem in Control.GroupSummary) {
				IModelColumn modelColumn = Model.Columns[summaryItem.FieldName];
				if(modelColumn != null && !string.IsNullOrEmpty(modelColumn.DisplayFormat)) {
					summaryItem.DisplayFormat = string.Format(summaryItem.DisplayFormat, modelColumn.DisplayFormat);
				}
			}
		}
		protected override void RemoveGroupSummaryForProtectedColumns() {
			base.RemoveGroupSummaryForProtectedColumns();
			for(int i = Control.GroupSummary.Count - 1; i >= 0; i--) {
				ISummaryItem item = Control.GroupSummary[i];
				foreach(GridColumn column in Control.Columns) {
					if(column.FieldName == item.FieldName && (column.OptionsColumn.AllowGroup == DevExpress.Utils.DefaultBoolean.False)) {
						Control.GroupSummary.RemoveAt(i);
					}
				}
			}
		}
		private void SynchronizeGroupSummary() {
			foreach(IModelNode modelColumn in Model.Columns) {
				modelColumn.ClearValue("GroupSummaryFieldName");
				modelColumn.ClearValue("GroupSummarySortOrder");
				modelColumn.ClearValue("GroupSummaryType");
			}
			if(Control is IModelSynchronizersHolder) {
				IModelSynchronizersHolder modelSynchronizersHolder = (IModelSynchronizersHolder)Control;
				foreach(GroupSummarySortInfo sortInfo in Control.GroupSummarySortInfo) {
					IGridColumnModelSynchronizer columnInfo = modelSynchronizersHolder.GetSynchronizer(sortInfo.GroupColumn) as IGridColumnModelSynchronizer;
					if(columnInfo != null) {
						string id = columnInfo.Model.Id;
						IModelColumn modelColumn = Model.Columns[id];
						if(modelColumn is IModelColumnWin) {
							IModelColumnWin frameColumn = (IModelColumnWin)modelColumn;
							frameColumn.GroupSummaryType = (SummaryType)Enum.Parse(typeof(SummaryType), sortInfo.SummaryItem.SummaryType.ToString());
							frameColumn.GroupSummarySortOrder = (ColumnSortOrder)Enum.Parse(typeof(ColumnSortOrder), sortInfo.SortOrder.ToString());
							frameColumn.GroupSummaryFieldName = sortInfo.SummaryItem.FieldName;
						}
					}
				}
			}
		}
		private void ApplySortGroupSummary() {
			List<GroupSummarySortInfo> list = new List<GroupSummarySortInfo>();
			foreach(IModelColumn frameColumn in Model.Columns) {
				if(frameColumn is IModelColumnWin) {
					string fieldName = ((IModelColumnWin)frameColumn).GroupSummaryFieldName;
					ColumnSortOrder sortOrder = ((IModelColumnWin)frameColumn).GroupSummarySortOrder;
					SummaryItemType summaryItemType = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), ((IModelColumnWin)frameColumn).GroupSummaryType.ToString());
					GridSummaryItem groupSummaryItem = null;
					if(summaryItemType != SummaryItemType.None) {
						if(!string.IsNullOrEmpty(fieldName)) {
							foreach(GridSummaryItem item in Control.GroupSummary) {
								if(item.FieldName == fieldName && item.SummaryType == summaryItemType) {
									groupSummaryItem = item;
									break;
								}
							}
						}
						else if(summaryItemType == SummaryItemType.Count) {
							foreach(GridSummaryItem item in Control.GroupSummary) {
								if(item.SummaryType == SummaryItemType.Count) {
									groupSummaryItem = item;
									break;
								}
							}
						}
					}
					GridColumn gridColumn = Control.Columns[frameColumn.PropertyName];
					if(gridColumn == null) {
						gridColumn = Control.Columns[frameColumn.PropertyName + "!"];
					}
					if(groupSummaryItem != null && gridColumn != null) {
						list.Add(new GroupSummarySortInfo(groupSummaryItem, gridColumn, sortOrder));
					}
				}
			}
			if(list.Count != 0) {
				Control.GroupSummarySortInfo.ClearAndAddRange(list.ToArray());
			}
		}
	}
}
