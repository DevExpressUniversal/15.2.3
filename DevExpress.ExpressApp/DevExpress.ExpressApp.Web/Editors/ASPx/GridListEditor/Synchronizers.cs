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
using System.Web.UI.WebControls;
using DevExpress.Data.Summary;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridViewModelSynchronizer : ModelSynchronizer<ASPxGridView, IModelListView> {
		private static void ApplyPagingCore(IModelListViewWeb modelListView, ASPxGridView gridView) {
			gridView.PageIndex = modelListView.PageIndex;
			gridView.SettingsPager.PageSize = modelListView.PageSize > 0 ? modelListView.PageSize : PagerModelSynchronizer.DefaultPageSize;
		}
		public static void ApplyPagingModel(IModelListViewWeb modelListView, ASPxGridView gridView) {
			ApplyPagingCore(modelListView, gridView);
			gridView.DataBound += (s, e) => {
				ApplyPagingCore(modelListView, gridView);
			};
		}
		private static void SavePagingModel(IModelListViewWeb modelListView, ASPxGridView gridView) {
			modelListView.PageIndex = gridView.PageIndex;
			modelListView.PageSize = gridView.SettingsPager.PageSize;
		}
		private static void ApplyFilterCore(IModelListView model, ASPxGridView gridView) {
			gridView.FilterExpression = model.Filter;
			gridView.FilterEnabled = model.FilterEnabled;
		}
		public static void ApplyFilterModel(IModelListView model, ASPxGridView gridView) {
			ApplyFilterCore(model, gridView);
			gridView.DataBound += (s, e) => {
				ApplyFilterCore(model, gridView);
			};
		}
		private static void SaveFilterModel(IModelListView model, ASPxGridView gridView) {
			model.Filter = gridView.FilterExpression;
			model.FilterEnabled = gridView.FilterEnabled;
		}
		public ASPxGridViewModelSynchronizer(ASPxGridView gridView, IModelListView model)
			: base(gridView, model) {
		}
		protected override void ApplyModelCore() {
			ApplyGridViewModel(Model, Control);
		}
		public override void SynchronizeModel() {
			SaveGridViewModel(Model, Control);
		}
		public static void ApplyGridViewModel(IModelListView model, ASPxGridView gridView) {
			ApplyGridViewModel(model, gridView, true);
		}
	public static void ApplyGridViewModel(IModelListView model, ASPxGridView gridView, bool canManageGridId) {
		gridView.Settings.ShowFooter = model.IsFooterVisible;
			if(canManageGridId) {
				gridView.ID = WebIdHelper.GetListEditorControlId(model.Id); 
			}
			gridView.Settings.ShowGroupPanel = model.IsGroupPanelVisible;
			gridView.SettingsBehavior.AutoExpandAllGroups = model.AutoExpandAllGroups;
			if(model is IModelListViewShowAutoFilterRow) {
				gridView.Settings.ShowFilterRow = ((IModelListViewShowAutoFilterRow)model).ShowAutoFilterRow;
			}
			IModelOptionsWebProvider modelOptionsWebProvider = model as IModelOptionsWebProvider;
			if(modelOptionsWebProvider != null) {
				IModelOptionsWeb optionsNode = modelOptionsWebProvider.GetOptionsWeb();
				if(optionsNode != null) {
					gridView.SettingsBehavior.AllowSort = optionsNode.ListViewAllowSort;
					gridView.SettingsPager.PageSizeItemSettings.Visible = optionsNode.ListViewEnablePageSizeChooser;
					gridView.SettingsBehavior.EnableCustomizationWindow = optionsNode.ListViewEnableColumnChooser;
				}
			}
			IModelListViewWeb modelListViewWeb = (IModelListViewWeb)model;
			InlineEditMode inlineEditMode = modelListViewWeb.InlineEditMode;
			gridView.SettingsEditing.Mode = (GridViewEditingMode)Enum.Parse(typeof(GridViewEditingMode), inlineEditMode.ToString());
			if(inlineEditMode == InlineEditMode.PopupEditForm) {
				gridView.SettingsPopup.EditForm.Width = Unit.Pixel(400);
			}
			if(modelListViewWeb.EnableEndlessPaging) {
				gridView.SettingsPager.Mode = GridViewPagerMode.EndlessPaging;
				gridView.Settings.VerticalScrollableHeight = modelListViewWeb.VerticalScrollableHeight;
			}
			if(modelListViewWeb.DetailRowMode != DetailRowMode.None && modelListViewWeb.DetailRowView != null && !model.AllowEdit) {
				gridView.SettingsDetail.ShowDetailRow = true;
				if(modelListViewWeb.DetailRowMode == DetailRowMode.DetailViewWithActions) {
					gridView.SettingsDetail.AllowOnlyOneMasterRowExpanded = true;
				}
			}
		}
		public static void SaveGridViewModel(IModelListView model, ASPxGridView gridView) {
			model.IsFooterVisible = gridView.Settings.ShowFooter;
			model.IsGroupPanelVisible = gridView.Settings.ShowGroupPanel;
			if(model is IModelListViewShowAutoFilterRow) {
				((IModelListViewShowAutoFilterRow)model).ShowAutoFilterRow = gridView.Settings.ShowFilterRow;
			}
			SavePagingModel((IModelListViewWeb)model, gridView);
			SaveFilterModel(model, gridView);
		}
		public static void ApplyModel(IModelListView model, ASPxGridListEditor listEditor) {
			ApplyModel(model, listEditor, true);
		}
		public static void ApplyModel(IModelListView model, ASPxGridListEditor listEditor, bool canManageGridId) {
			ApplyGridViewModel(model, listEditor.Grid, canManageGridId);
			ASPxGridViewColumnsSynchronizer.ApplyModel(model, listEditor);
			ApplyPagingModel((IModelListViewWeb)model, listEditor.Grid);
			ApplyFilterModel(model, listEditor.Grid);
			new ASPxGridSummaryModelSynchronizer(listEditor.Grid, model, listEditor).ApplyModel();
		}
		public static void SaveModel(IModelListView model, ASPxGridListEditor listEditor) {
			SaveGridViewModel(model, listEditor.Grid);
			ASPxGridViewColumnsSynchronizer.SaveModel(model, listEditor);
			new ASPxGridSummaryModelSynchronizer(listEditor.Grid, model, listEditor).SynchronizeModel();
		}
	}
	public class ASPxGridViewColumnsSynchronizer {
		private static IModelSynchronizable CreateGridViewColumnsModelSynchronizer(IModelListView model, ASPxGridListEditor listEditor) {
			if(ASPxGridListEditor.UseASPxGridViewDataSpecificColumns) {
				if(model.BandsLayout.Enable) {
					return new ASPxGridViewBandColumnsModelSynchronizer(listEditor, model);
				}
				else {
					return new ASPxGridViewColumnsModelSynchronizer(listEditor, model);
				}
			}
			else {
				return new ColumnsListEditorModelSynchronizer(listEditor, model);
			}
		}
		public static void ApplyModel(IModelListView model, ASPxGridListEditor listEditor) {
			CreateGridViewColumnsModelSynchronizer(model, listEditor).ApplyModel();
		}
		public static void SaveModel(IModelListView model, ASPxGridListEditor listEditor) {
			CreateGridViewColumnsModelSynchronizer(model, listEditor).SynchronizeModel();
		}
	}
	public class ASPxGridSummaryModelSynchronizer : SummaryModelSynchronizer<ASPxGridView, IModelListView> {
		IDataItemTemplateInfoProvider dataItemTemplateInfoProvider;
		public ASPxGridSummaryModelSynchronizer(ASPxGridView gridView, IModelListView model, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider)
			: base(gridView, model) {
			Guard.ArgumentNotNull(dataItemTemplateInfoProvider, "dataItemTemplateInfoProvider");
			this.dataItemTemplateInfoProvider = dataItemTemplateInfoProvider;
		}
		private List<ASPxSummaryItem> GetAllSummaryItems() {
			List<ASPxSummaryItem> result = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem item in Control.TotalSummary) {
				result.Add(item);
			}
			foreach(ASPxSummaryItem item in Control.GroupSummary) {
				result.Add(item);
			}
			return result;
		}
		protected override void ApplyColumnDisplayFormatToGroupSummary() {
			base.ApplyColumnDisplayFormatToGroupSummary();
			foreach(ASPxSummaryItem summaryItem in GetAllSummaryItems()) {
				IModelColumn modelColumn = Model.Columns[summaryItem.FieldName];
				if(modelColumn != null && !string.IsNullOrEmpty(modelColumn.DisplayFormat)) {
					if(!string.IsNullOrEmpty(summaryItem.DisplayFormat)) {
						summaryItem.DisplayFormat = string.Format(summaryItem.DisplayFormat, modelColumn.DisplayFormat);
					}
					else {
						summaryItem.ValueDisplayFormat = modelColumn.DisplayFormat;
					}
				}
			}
		}
		protected override void RemoveGroupSummaryForProtectedColumns() {
			base.RemoveGroupSummaryForProtectedColumns();
			if(!ASPxGridListEditor.UseASPxGridViewDataSpecificColumns) {
				for(int i = Control.GroupSummary.Count - 1; i >= 0; i--) {
					ISummaryItem item = Control.GroupSummary[i];
					foreach(GridViewColumn column in Control.Columns) {
						GridViewDataColumn gridColumn = column as GridViewDataColumn;
						IGridViewDataColumnInfo info = dataItemTemplateInfoProvider.GetColumnInfo(column) as IGridViewDataColumnInfo;
						if((gridColumn != null) && info != null && gridColumn.FieldName == item.FieldName && !new ASPxGridViewColumnWrapper(Control, gridColumn, info).AllowGroupingChange) {
							Control.GroupSummary.RemoveAt(i);
						}
					}
				}
			}
		}
		protected override SummaryItemsSerializer CreateSummaryItemsSerializer(ISummaryItemsOwner itemsOwner) {
			if(!ASPxGridListEditor.UseASPxGridViewDataSpecificColumns) {
				return base.CreateSummaryItemsSerializer(itemsOwner);
			}
			else {
				return new ASPxGridXafSummaryItemsSerializer(itemsOwner);
			}
		}
	}
}
