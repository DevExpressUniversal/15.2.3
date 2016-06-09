#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.SharePoint.Internal;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
namespace DevExpress.SharePoint {
	public class SPxGridView : ASPxGridView {
		const string AttachImageName = "attach.gif";
		const string AttachImageToolTip = "Attachments";
		const string EditButtonID = "Edit";
		const string DeleteButtonID = "Delete";
		const string NewButtonID = "New";
		const string SourcePageQueryParamName = "Source";
		const string RootFolderQueryParamName = "RootFolder";
		const string CustomButtonClickHandler = "function(s,e){ spxOnCustomButtonClick(s, e); }";
		const string JSPropertyKey = "cpListItemPermissions";
		const SPFieldLookupValue LookUpFieldType = null;
		SPList list;
		SPView previousView;
		Dictionary<GridViewDataColumn, SPField> spColumnFields;
		SPWeb web;
		Dictionary<int, int> visibleIndexDictionary = null;
		public SPxGridView() {
			this.spColumnFields = new Dictionary<GridViewDataColumn, SPField>();
			EnableRowsCache = false;
			SettingsBehavior.AllowGroup = true;
			SettingsBehavior.AllowSort = true;
			SettingsBehavior.AllowDragDrop = true;
			SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
			SettingsBehavior.ConfirmDelete = true;
			Settings.ShowGroupPanel = true;
			CustomButtonCallback += new ASPxGridViewCustomButtonCallbackEventHandler(OnCustomButtonCallback);
			CustomUnboundColumnData += SPxGridView_CustomUnboundColumnData;
			CustomColumnDisplayText += SPxGridView_CustomColumnDisplayText;
			BeforeHeaderFilterFillItems += SPxGridView_BeforeHeaderFilterFillItems;
			Styles.AlternatingRow.Enabled = DefaultBoolean.True;
			BorderTop.BorderStyle = BorderStyle.None;
			BorderLeft.BorderStyle = BorderStyle.None;
			BorderRight.BorderStyle = BorderStyle.None;
			ClientSideEvents.Init = "function(s, e) { InitGridCommandItems(s); }";
			ClientSideEvents.EndCallback = "function(s, e) { InitGridCommandItems(s); }";
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ASPxPopupMenu popUpMenu = new ASPxPopupMenu();
			popUpMenu.ID = "SPPopUpControl";
			popUpMenu.ClientSideEvents.Init = "function(s, e) {}";
			popUpMenu.ClientSideEvents.ItemClick = "function(s, e)" +
													"{" +
													"if (e.item.name=='btnDeleteItem')" +
													"   {" +
													"       s.dxSPGRid.DeleteGridRow(s.dxSPGridId);" +
													"       s.Hide();" +
													"   }" +
													"}";
			popUpMenu.PopupHorizontalAlign = PopupHorizontalAlign.OutsideRight;
			popUpMenu.PopupVerticalAlign = PopupVerticalAlign.TopSides;
			popUpMenu.PopupAction = PopupAction.MouseOver;
			popUpMenu.CloseAction = PopupMenuCloseAction.MouseOut;
			string actionTitle = SPUtility.GetLocalizedString("$Resources:cui_ButViewItem", "core", (uint)Web.UICulture.LCID);
			DevExpress.Web.MenuItem subItem = new DevExpress.Web.MenuItem(actionTitle, "btnViewItem", "/_layouts/15/images/DOC_SP16.GIF",
				SPxListViewUtils.GetItemDisplayFormUrl(List));
			popUpMenu.Items.Add(subItem);
			actionTitle = SPUtility.GetLocalizedString("$Resources:cui_ButEditItem", "core", (uint)Web.UICulture.LCID);
			subItem = new DevExpress.Web.MenuItem(actionTitle, "btnEditItem", "/_layouts/15/images/edititem.gif",
				SPxListViewUtils.GetItemEditFormUrl(List));
			popUpMenu.Items.Add(subItem);
			actionTitle = SPUtility.GetLocalizedString("$Resources:cui_WebAppWorkflowTitle", "core", (uint)Web.UICulture.LCID);
			subItem = new DevExpress.Web.MenuItem(actionTitle, "btnWorkflow", "/_layouts/15/images/ManageWorkflow16.png",
				SPxListViewUtils.GetItemWorkflowFormUrl(List, this.Page.Request.Url.AbsoluteUri));
			subItem.BeginGroup = true;
			popUpMenu.Items.Add(subItem);
			actionTitle = SPUtility.GetLocalizedString("$Resources:cui_ButAlertMe", "core", (uint)Web.UICulture.LCID);
			subItem = new DevExpress.Web.MenuItem(actionTitle, "btnAlertMe", "/_layouts/15/images/alertme.png",
				SPxListViewUtils.GetItemAlertMeFormUrl(List, this.Page.Request.Url.AbsoluteUri));
			popUpMenu.Items.Add(subItem);
			actionTitle = SPUtility.GetLocalizedString("$Resources:cui_ButVersionHistory", "core", (uint)Web.UICulture.LCID);
			subItem = new DevExpress.Web.MenuItem(actionTitle, "btnVersionHistory", "/_layouts/15/images/VERSIONS.GIF",
				SPxListViewUtils.GetItemViewVersionFormUrl(List, this.Page.Request.Url.AbsoluteUri));
			subItem.BeginGroup = true;
			popUpMenu.Items.Add(subItem);
			actionTitle = SPUtility.GetLocalizedString("$Resources:cui_ButDeleteItem", "core", (uint)Web.UICulture.LCID);
			subItem = new DevExpress.Web.MenuItem(actionTitle, "btnDeleteItem", "/_layouts/15/images/delitem.gif", "");
			subItem.BeginGroup = true;
			popUpMenu.Items.Add(subItem);
			Controls.Add(popUpMenu);
			popUpMenu.DataBind();
		}
		void SPxGridView_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			string column = e.Column.FieldName;
			if(e.Column.UnboundType != UnboundColumnType.Bound)
				column = column.Substring(0, column.LastIndexOf("_Unbound"));
			SPField currentField = GetSPFieldByColumn(e.Column);
			SPListItem currentItem = null;
			try {
				var dataObject = DataProxy.GetRow(e.VisibleRowIndex);
				if(dataObject is Microsoft.SharePoint.WebControls.SPDataSourceViewResultItem)
					dataObject = ((Microsoft.SharePoint.WebControls.SPDataSourceViewResultItem)dataObject).ResultItem;
				if(dataObject != null)
					if(dataObject is SPListItem)
						currentItem = (SPListItem)dataObject;
			} catch { }
			if(currentItem != null) {
				object fieldValue = null;
				fieldValue = currentItem.Fields.ContainsField(column) ? currentItem[column] : null;
				if(fieldValue != null) {
					if(currentField is SPFieldDateTime) {
						DateTime localTime = (DateTime)fieldValue;
						DateTime universalTime = Web.RegionalSettings.TimeZone.LocalTimeToUTC(localTime);
						e.DisplayText = currentField.GetFieldValueAsHtml(universalTime);
					} else
						if(currentField is SPFieldAttachments) {
							bool isAttachment = (bool)fieldValue;
							if(isAttachment)
								e.DisplayText = string.Format("<img src='{0}' alt='{1}'/>", GetAttachImageUrl(), AttachImageToolTip);
							else
								e.DisplayText = string.Empty;
						} else if(IsLinkComputedField(currentField)) {
							if(currentItem.Folder != null) {
								SPView currentView = SPxListViewUtils.GetSPListView(List, Context);
								e.DisplayText = string.Format("<a class='dxspg_ci' id='myGrid_ci_{1}'  href='{6}{0}?RootFolder={3}&FolderCTID={4}&View={5}'>{2}</a>", currentView.ServerRelativeUrl,
													e.VisibleRowIndex.ToString(),
													currentField.GetFieldValueAsText(fieldValue),
													HttpUtility.UrlEncode(currentItem.Folder.ServerRelativeUrl),
													HttpUtility.UrlEncode(currentItem.ContentType.Id.ToString()),
													HttpUtility.UrlEncode(currentView.ID.ToString()), Web.Url);
							} else
								e.DisplayText = string.Format("<a class='dxspg_ci' id='myGrid_ci_{1}'  href='{0}{3}'>{2}</a>", GetLinkComputedFieldUrlPrefix(), e.VisibleRowIndex.ToString(), currentField.GetFieldValueAsText(fieldValue), currentItem.ID.ToString());
							SPxGridView currentGrid = ((SPxGridView)sender);
							Hashtable h = null;
							if(currentGrid.JSProperties.ContainsKey(JSPropertyKey)) {
								h = currentGrid.JSProperties[JSPropertyKey] as Hashtable;
								if(h.ContainsKey(e.VisibleRowIndex))
									h[e.VisibleRowIndex] = new {
										Edit = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
										Delete = SPxListViewUtils.DoesUserHavePermissionsToDeleteListItem(currentItem),
										Workflow = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
										ViewVersions = SPxListViewUtils.DoesUserHavePermissionsToVioewVersionsListItem(currentItem)
									};
								else
									h.Add(e.VisibleRowIndex, new {
										Edit = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
										Delete = SPxListViewUtils.DoesUserHavePermissionsToDeleteListItem(currentItem),
										Workflow = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
										ViewVersions = SPxListViewUtils.DoesUserHavePermissionsToVioewVersionsListItem(currentItem)
									});
							} else {
								h = new Hashtable();
								h.Add(e.VisibleRowIndex, new {
									Edit = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
									Delete = SPxListViewUtils.DoesUserHavePermissionsToDeleteListItem(currentItem),
									Workflow = SPxListViewUtils.DoesUserHavePermissionsToEditListItem(currentItem),
									ViewVersions = SPxListViewUtils.DoesUserHavePermissionsToVioewVersionsListItem(currentItem)
								});
								currentGrid.JSProperties.Add(JSPropertyKey, h);
							}
						} else
							e.DisplayText = currentField.GetFieldValueAsHtml(fieldValue);
				} else
					e.DisplayText = currentField.GetFieldValueAsHtml(fieldValue);
			}
		}
		void SPxGridView_BeforeHeaderFilterFillItems(object sender, ASPxGridViewBeforeHeaderFilterFillItemsEventArgs e) {
			string column = e.Column.FieldName;
			SPField currentField = GetSPFieldByColumn(e.Column);
			if(currentField is SPFieldLookup) {
				if(((SPFieldLookup)currentField).AllowMultipleValues) {
					List<KeyValuePair<int, string>> values = SPxListViewUtils.GetDistinctValuesForSPFieldMultiLookUpColumn(List, currentField);
					if(values != null) {
						foreach(KeyValuePair<int, string> keyValue in values) {
							FilterValue filterValue = new FilterValue(keyValue.Value, string.Empty);
							FunctionOperator co = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column), new OperandValue(keyValue.Value));
							filterValue.Query = co.ToString();
							e.Values.Add(filterValue);
						}
						e.Handled = true;
					}
				} else {
					DataTable values = SPxListViewUtils.GetDistinctValuesForSPFielLookUpColumn(List, currentField);
					if(values != null) {
						e.Values.Clear();
						e.AddShowAll();
						foreach(DataRow currentRow in values.Rows) {
							if(currentRow[currentField.InternalName] != null) {
								SPFieldLookupValue keyValue = new SPFieldLookupValue(currentRow[currentField.InternalName].ToString());
								FilterValue filterValue = new FilterValue(keyValue.LookupValue, string.Empty);
								FunctionOperator co = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column), new OperandValue(keyValue.LookupValue));
								filterValue.Query = co.ToString();
								e.Values.Add(filterValue);
							}
						}
						e.Handled = true;
					}
				}
			}
			if(currentField is SPFieldMultiChoice) {
				List<string> values = SPxListViewUtils.GetDistinctValuesForSPFieldMultiChoiceColumn(List, currentField);
				if(values != null) {
					e.Values.Clear();
					e.AddShowAll();
					foreach(string keyValie in values) {
						FilterValue filterValue = new FilterValue(keyValie, string.Empty);
						FunctionOperator co = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(column), new OperandValue(keyValie));
						filterValue.Query = co.ToString();
						e.Values.Add(filterValue);
					}
					e.Handled = true;
				}
			}
			if(currentField is SPFieldAttachments) {
				e.Values.Clear();
				e.AddShowAll();
				FilterValue filterValue = new FilterValue("No", "False");
				e.Values.Add(filterValue);
				filterValue = new FilterValue("Yes", "True");
				e.Values.Add(filterValue);
				e.Handled = true;
			}
		}
		void SPxGridView_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e) {
			if(!e.IsGetData)
				return;
			int key = (int)e.GetListSourceFieldValue(((SPxGridView)sender).KeyFieldName);
			int visibleIndex = ((SPxGridView)sender).FindVisibleIndexByKeyValue(key);
			SPListItem currentItem = null;
			if(visibleIndex < 0)
				try {
					currentItem = List.GetItemById(key);
				} catch { } else
				try {
					var dataObject = DataProxy.GetRow(visibleIndex);
					if(dataObject is Microsoft.SharePoint.WebControls.SPDataSourceViewResultItem)
						dataObject = ((Microsoft.SharePoint.WebControls.SPDataSourceViewResultItem)dataObject).ResultItem;
					if(dataObject != null)
						if(dataObject is SPListItem)
							currentItem = (SPListItem)dataObject;
				} catch { }
			string column = e.Column.FieldName.Substring(0, e.Column.FieldName.LastIndexOf("_Unbound"));
			if(currentItem != null) {
				object fieldValue = null;
				fieldValue = currentItem.Fields.ContainsField(column) ? currentItem[column] : null;
				e.Value = fieldValue != null ? fieldValue.ToString() : string.Empty;
			}
		}
		[Browsable(false)]
		public SPList List {
			get { return list; }
			set {
				if(List == value)
					return;
				this.list = value;
				OnListChanged();
			}
		}
		[Browsable(false)]
		public SPWeb Web {
			get { return web; }
			set { web = value; }
		}
		protected Dictionary<GridViewDataColumn, SPField> SPColumnFields { get { return spColumnFields; } }
		protected SPView PreviousView { get { return previousView; } }
		protected Dictionary<int, int> VisibleIndexDictionary { get { return visibleIndexDictionary; } }
		public SPField GetSPFieldByColumn(GridViewDataColumn column) {
			if(SPColumnFields.ContainsKey(column))
				return SPColumnFields[column];
			return null;
		}
		protected override ASPxGridTextSettings CreateSettingsText() {
			return new SPxGridViewTextSettings(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new List<IStateManager>().ToArray();
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			if(IsAllowDeleting())
				RegisterScriptBlock("CustomButtonClickHandlerFunc",
					RenderUtils.GetScriptHtml(SPxListViewUtils.GetCustomButtonClickHandlerScript(this)));
		}
		protected void OnListChanged() {
			SPView currentView = SPxListViewUtils.GetSPListView(List, Context);
			if(PreviousView == currentView)
				return;
			this.previousView = currentView;
			RebuildColumns(currentView);
		}
		protected void RebuildColumns(SPView view) {
			Columns.Clear();
			SPColumnFields.Clear();
			RebuildColumnBySPView(view);
		}
		protected void RebuildColumnBySPView(SPView view) {
			if(view == null)
				return;
			this.visibleIndexDictionary = new Dictionary<int, int>();
			SPViewInfoInfo viewInfo = new SPViewInfoInfo(view);
			SettingsPager.PageSize = viewInfo.RowCount;
			SettingsPager.Visible = view.Paged;
			if(viewInfo.IsGroupExpanded)
				SettingsBehavior.AutoExpandAllGroups = true;
			if(IsCommandColumnShow())
				Columns.Add(CreateCommandColumn());
			foreach(string fieldName in view.ViewFields) {
				if(!List.Fields.ContainsField(fieldName))
					continue;
				SPField field = List.Fields.GetFieldByInternalName(fieldName);
				if(!IsFieldAllowable(field))
					continue;
				GridViewDataColumn column = CreateGridViewColumnByField(field, viewInfo);
				SPColumnFields.Add(column, field);
				Columns.Add(column);
			}
		}
		protected GridViewColumn CreateCommandColumn() {
			GridViewCommandColumn column = new GridViewCommandColumn();
			SPxGridViewTextSettings settingsText = (SPxGridViewTextSettings)SettingsText;
			if(IsAllowEditing()) {
				ClientSideEvents.CustomButtonClick = CustomButtonClickHandler;
				GridViewCommandColumnCustomButton editButton = new GridViewCommandColumnCustomButton();
				editButton.Text = settingsText.GetSPxGridViewCommandButtonText(GridCommandButtonType.Edit);
				editButton.ID = EditButtonID;
				column.CustomButtons.Add(editButton);
			}
			if(IsAllowAddNewItem()) {
				GridViewCommandColumnCustomButton newButton = new GridViewCommandColumnCustomButton();
				newButton.Text = settingsText.GetSPxGridViewCommandButtonText(GridCommandButtonType.New);
				newButton.ID = NewButtonID;
				column.CustomButtons.Add(newButton);
			}
			if(IsAllowDeleting()) {
				GridViewCommandColumnCustomButton deleteButton = new GridViewCommandColumnCustomButton();
				deleteButton.Text = settingsText.GetSPxGridViewCommandButtonText(GridCommandButtonType.Delete);
				deleteButton.ID = DeleteButtonID;
				column.CustomButtons.Add(deleteButton);
			}
			return column;
		}
		protected GridViewDataColumn CreateGridViewColumnByField(SPField field, SPViewInfoInfo viewInfo) {
			GridViewDataColumn column = SPxListViewUtils.CreateSPxGridViewColumnInstance(field);
			column.FieldName = column.UnboundType == UnboundColumnType.Bound ? field.InternalName : field.InternalName + "_Unbound";
			if(field is SPFieldAttachments) {
				column.Width = Unit.Pixel(40);
				column.Caption = string.Format("<img src='{0}' alt='{1}'/>", GetAttachImageUrl(), AttachImageToolTip);
			} else
				column.Caption = field.Title;
			column.Settings.AllowSort = field.Sortable ? DefaultBoolean.True : DefaultBoolean.False;
			column.Settings.AllowHeaderFilter = field.Filterable ? DefaultBoolean.True : DefaultBoolean.False;
			column.Settings.AllowAutoFilter = field.Filterable ? DefaultBoolean.True : DefaultBoolean.False;
			column.Settings.AllowGroup = SPxListViewUtils.IsFieldAllowMultipleValues(field) ? DefaultBoolean.False : DefaultBoolean.True;
			if(viewInfo.IsSortedField(field)) {
				column.SortIndex = viewInfo.GetSortIndex(field);
				column.SortOrder = viewInfo.GetSortOrder(field);
			}
			if(viewInfo.IsGroupedField(field)) {
				column.GroupIndex = viewInfo.GetGroupIndex(field);
				column.SortOrder = viewInfo.GetGroupOrder(field);
				GroupSummary.Add(CreateDefaultGroupSummaryItem(column));
			}
			if(viewInfo.IsSummaryExist(field)) {
				if(!Settings.ShowFooter)
					Settings.ShowFooter = true;
				TotalSummary.Add(CreateSummaryItem(column, viewInfo.GetSummaryType(field)));
			}
			return column;
		}
		protected ASPxSummaryItem CreateSummaryItem(GridViewDataColumn column, SummaryItemType type) {
			ASPxSummaryItem summaryItem = new ASPxSummaryItem();
			summaryItem.FieldName = column.FieldName;
			summaryItem.ShowInColumn = column.FieldName;
			summaryItem.SummaryType = type;
			return summaryItem;
		}
		protected ASPxSummaryItem CreateDefaultGroupSummaryItem(GridViewDataColumn column) {
			ASPxSummaryItem summaryItem = new ASPxSummaryItem();
			summaryItem.FieldName = column.FieldName;
			summaryItem.SummaryType = SummaryItemType.Count;
			return summaryItem;
		}
		protected void OnCustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e) {
			UriBuilder uriBuilder = new UriBuilder();
			string query = "";
			if(e.ButtonID == EditButtonID) {
				uriBuilder.Path = GetEditFormUrl();
				query += "id=" + GetSPListItemByVisibleIndex(e.VisibleIndex).ID;
			}
			if(e.ButtonID == NewButtonID) {
				uriBuilder.Path = GetNewFormUrl();
				string param = RootFolderQueryParamName + "=" + List.RootFolder;
				query += query != "" ? "&" + param : param;
			}
			if(!string.IsNullOrEmpty(uriBuilder.Path)) {
				string param = SourcePageQueryParamName + "=" + GetSourcePageUrl();
				query += query != "" ? "&" + param : param;
			}
			uriBuilder.Query = query;
			ASPxWebControl.RedirectOnCallback(uriBuilder.Uri.PathAndQuery);
		}
		protected internal bool IsFieldAllowable(SPField field) {
			if(field == null || field.Hidden)
				return false;
			if(field is SPFieldComputed)
				return IsLinkComputedField(field);
			return true;
		}
		protected bool IsCommandColumnShow() {
			return List.DoesUserHavePermissions(SPBasePermissions.AddListItems) ||
				List.DoesUserHavePermissions(SPBasePermissions.EditListItems) ||
				List.DoesUserHavePermissions(SPBasePermissions.DeleteListItems);
		}
		protected bool IsAllowAddNewItem() {
			return List.DoesUserHavePermissions(SPBasePermissions.AddListItems);
		}
		protected bool IsAllowEditing() {
			return List.DoesUserHavePermissions(SPBasePermissions.EditListItems);
		}
		protected bool IsAllowDeleting() {
			return List.DoesUserHavePermissions(SPBasePermissions.DeleteListItems);
		}
		public SPListItem GetSPListItemByVisibleIndex(int visibleIndex) {
			return (GetRow(visibleIndex) as SPDataSourceViewResultItem).ResultItem as SPListItem;
		}
		protected internal bool IsLinkComputedField(SPField field) {
			bool ret = false;
			if(field is SPFieldComputed && field.FieldReferences != null) {
				for(int i = 0; i < field.FieldReferences.Length; i++) {
					string fieldRef = field.FieldReferences[i];
					if(fieldRef == "LinkTitleNoMenu" ||
						fieldRef == "_EditMenuTableStart" ||
						fieldRef == "_EditMenuTableEnd") {
						ret = true;
						break;
					}
				}
			}
			return ret;
		}
		protected internal string GetAttachImageUrl() {
			return SPWebPartManager.GetClassResourcePath(Web, typeof(SPxGridView)) + "/Images/" + AttachImageName;
		}
		protected string GetNewFormUrl() {
			return List.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl;
		}
		protected string GetEditFormUrl() {
			return List.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl;
		}
		protected string GetSourcePageUrl() {
			return Context.Request.Url.AbsoluteUri;
		}
		protected internal string GetLinkComputedFieldUrlPrefix() {
			return List.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl + "?ID=";
		}
	}
	public class SPxGridViewTextSettings : ASPxGridViewTextSettings {
		public SPxGridViewTextSettings(ASPxGridView grid) : base(grid) { }
		protected internal string GetSPxGridViewCommandButtonText(GridCommandButtonType type) {
			return GetCommandButtonText(type);
		}
	}
}
