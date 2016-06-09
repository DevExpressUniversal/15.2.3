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
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Text;
using DevExpress.XtraPivotGrid.Data;
using System.Web.UI;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.ASPxPivotGrid.Html;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxPivotGrid {
	public class ASPxPivotGridRenderHelper {
		static readonly int 
			FilterPopupDefaultWidth = 220,
			FilterPopupDefaultHeight = 200,
			FilterPopupDefaultMinWidth = 180;
		ASPxPivotGrid pivotGrid;
		readonly Dictionary<string, object[]> fieldValueContextMenuParams;
		readonly Dictionary<string, object[]> headerContextMenuParams;
		public ASPxPivotGridRenderHelper(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			this.fieldValueContextMenuParams = new Dictionary<string, object[]>();
			this.headerContextMenuParams = new Dictionary<string, object[]>();
		}
		protected ASPxPivotGrid PivotGrid {
			get { return pivotGrid; }
		}
		protected PivotFieldItemCollection Fields {
			get { return Data.FieldItems; }
		}
		protected PivotGroupItemCollection Groups {
			get { return Data.FieldItems.GroupItems; }
		}
		protected PivotGridWebData Data {
			get { return PivotGrid.Data; }
		}
		protected ScriptHelper ScriptHelper {
			get { return PivotGrid.ScriptHelper; }
		}
		protected string ClientID {
			get { return PivotGrid.ClientID; }
		}
		protected Page Page {
			get { return PivotGrid.Page; }
		}
		ImageProperties RenderImage(string imageName) {
			return PivotGrid.Images.GetImageProperties(PivotGrid.Page, imageName, true);
		}
		public ImageProperties GetHeaderFilterImage() {
			return RenderImage(PivotGridImages.HeaderFilterName);
		}
		public ImageProperties GetHeaderActiveFilterImage() {
			return RenderImage(PivotGridImages.HeaderActiveFilterName);
		}
		public ImageProperties GetHeaderSortImage(PivotSortOrder sortOrder) {
			return sortOrder == PivotSortOrder.Ascending ?
				RenderImage(PivotGridImages.HeaderSortUpName) :
				RenderImage(PivotGridImages.HeaderSortDownName);
		}
		public bool GetHeadersVisible(PivotArea area, bool useFieldCollections) {
			return Data.GetFieldCountByArea(area, useFieldCollections) > 0 ||
					Data.GetFieldCountByArea(PivotArea.DataArea, useFieldCollections) > 1 &&
					   (area == PivotArea.ColumnArea && Data.OptionsDataField.Area == PivotDataArea.ColumnArea ||
						area == PivotArea.RowArea && Data.OptionsDataField.Area == PivotDataArea.RowArea);
		}
		public ImageProperties GetFieldValueCollapsedImage(bool isCollapsed) {
			return GetExpandedCollapsedImage(!isCollapsed);
		}
		public ImageProperties GetGroupSeparatorImage() {
			return RenderImage(PivotGridImages.GroupSeparatorName);
		}
		public ImageProperties GetGroupButtonImage(bool isExpanded) {
			return GetExpandedCollapsedImage(isExpanded);
		}
		public ImageProperties GetExpandedCollapsedImage(bool isExpanded) {
			return isExpanded ?
				RenderImage(PivotGridImages.FieldValueExpandedName) :
				RenderImage(PivotGridImages.FieldValueCollapsedName);
		}
		public ImageProperties GetFilterWindowSizeGripImage() {
			return RenderImage(PivotGridImages.FilterWindowSizeGripName);
		}
		public ImageProperties GetCustomizationFieldsCloseImage() {
			return RenderImage(PivotGridImages.CustomizationFieldsCloseName);
		}
		public ImageProperties GetCustomizationFieldsBackgroundImage() {
			return RenderImage(PivotGridImages.CustomizationFieldsBackgroundName);
		}
		public ImageProperties GetDragArrowUpImage() {
			return RenderImage(PivotGridImages.DragArrowUpName);
		}
		public ImageProperties GetDragArrowDownImage() {
			return RenderImage(PivotGridImages.DragArrowDownName);
		}
		public ImageProperties GetDragArrowRightImage() {
			return RenderImage(PivotGridImages.DragArrowRightName);
		}
		public ImageProperties GetDragArrowLeftImage() {
			return RenderImage(PivotGridImages.DragArrowLeftName);
		}
		public ImageProperties GetDragHideFieldImage() {
			return RenderImage(PivotGridImages.DragHideFieldName);
		}
		public ImageProperties GetLoadingPanelImage() {
			return RenderImage(PivotGridImages.LoadingPanelImageName);
		}
		public ImageProperties GetDataHeadersImage() {
			return RenderImage(PivotGridImages.DataHeadersPopupName);
		}
		public ImageProperties GetSortByColumnImage() {
			return RenderImage(PivotGridImages.SortByColumnName);
		}
		public ImageProperties GetPrefilterImage() {
			return RenderImage(PivotGridImages.PrefilterButtonName);
		}
		public ImageProperties GetTreeViewNodeLoadingPanelImage() {
			return RenderImage(PivotGridImages.TreeViewNodeLoadingPanelImageName);
		}
		public string GetAllowedAreaIdsScript(string objectName) {
			Dictionary<string, List<string>> allowed = GetAllowedAreaIds();
			if(allowed.Count == 0)
				return string.Empty;
			StringBuilder stb = new StringBuilder();
			stb.Append(objectName).Append(".pivotGrid_AllowedAreaIds[\"").Append(ClientID).AppendLine("\"] = {");
			foreach(string key in allowed.Keys) {
				List<string> ids = allowed[key];
				stb.Append("\"").Append(ClientID).Append("_");
				stb.Append(key).Append("\" : [");
				for(int i = 0; i < ids.Count; i++) {
					string value = ids[i];
					stb.Append("\"").Append(ClientID).Append("_").Append(value).Append("\", ");
				}
				stb.Length -= 2;
				stb.AppendLine("],");
			}
			stb.Length -= 3;
			stb.Append("};");
			return stb.ToString();
		}
		protected internal Dictionary<string, List<string>> GetAllowedAreaIds() {
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach(PivotFieldItem field in Fields) {
				if(!Data.IsOLAP && field.AllowedAreas == PivotGridAllowedAreas.All || field.InnerGroupIndex > 0)
					continue;
				dictionary.Add(GetHeaderIdPrefix(field) + ScriptHelper.GetID(field), GetAllowedAreaIds(field));
			}
			if(Data.DataField.Visible)
				dictionary.Add(GetHeaderIdPrefix(Data.FieldItems.DataFieldItem) + ScriptHelper.GetID(Data.FieldItems.DataFieldItem), GetAllowedAreaIds(Data.FieldItems.DataFieldItem));
			return dictionary;
		}
		string GetHeaderIdPrefix(PivotFieldItemBase field) {
			string prefix = "";
			if(!field.Visible) {
				return PivotGridHtmlCustomizationFields.ElementName_ID + "_";
			}
			return prefix;
		}
		protected internal List<string> GetAllowedAreaIds(PivotFieldItemBase field) {
			List<string> ids = new List<string>();
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(!field.IsAreaAllowed(area))
					continue;
				List<PivotFieldItemBase> fields = Data.GetFieldItemsByArea(area);
				int beforeCount = ids.Count;
				if(GetHeadersVisible(area, false))
					AddAreaIds(ids, fields);
				if(ids.Count == beforeCount)
					ids.Add(ScriptHelper.GetAreaID(area));
				else if(PivotGrid.OptionsCustomization.CustomizationFormStyle == DevExpress.XtraPivotGrid.Customization.CustomizationFormStyle.Excel2007 || PivotGrid.MasterControlImplementation.ContainsRelatedCustomizationForm)
					ids.Add(ScriptHelper.GetCustomizationAreaID(area));
				ids.Add(ScriptHelper.GetHeaderTableID(area));
			}
			return ids;
		}
		void AddAreaIds(List<string> ids, List<PivotFieldItemBase> fields) {
			foreach(PivotFieldItemBase field in fields) {
				if(field.InnerGroupIndex > 0 && (field.Area != PivotArea.RowArea || !field.Visible))
					continue;
				ids.Add(GetHeaderIdPrefix(field) + ScriptHelper.GetID(field));
			}
		}
		public string GetAfterCallBackInitializeScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("ASPx.pivotGrid_SetRenderOptions(\"{0}\", {1});", ClientID, HtmlConvertor.ToJSON(GetDynamicRenderOptions()));
			stb.AppendFormat("ASPx.pivotGrid_AfterCallBackInitialize(\"{0}\");", ClientID);
			return stb.ToString();
		}
		bool IsScrollingEnabled(Unit size, ScrollBarMode scrollBarMode, bool isColumn) {
			PivotWebVisualItems visualItems = pivotGrid.Data.VisualItems;
			int count = isColumn ? visualItems.ColumnCount : visualItems.RowCount;
			return !size.IsEmpty && scrollBarMode != ScrollBarMode.Hidden && count > 0;
		}
		ScrollBarMode ActualHorzScrollBarMode { get { return pivotGrid.OptionsView.ActualHorizontalScrollBarMode; } }
		ScrollBarMode ActualVertScrollBarMode { get { return pivotGrid.OptionsView.VerticalScrollBarMode; } }
		bool IsHorzScrollingEnabled { get { return IsScrollingEnabled(pivotGrid.Width, ActualHorzScrollBarMode, true); } }
		bool IsVertScrollingEnabled { get { return IsScrollingEnabled(pivotGrid.Height, ActualVertScrollBarMode, false); } }
		public bool UseDynamicRender {
			get {
				bool forceDynamicRender = pivotGrid.HasCallbackAnimation;
				return forceDynamicRender || IsHorzScrollingEnabled || IsVertScrollingEnabled;
			}
		}
		public ClientDynamicRenderOptions GetDynamicRenderOptions() {
			if(!UseDynamicRender)
				return null;
			return new ClientDynamicRenderOptions {
				Horz = CreateScrollingOptions(true, IsHorzScrollingEnabled, ActualHorzScrollBarMode),
				Vert = CreateScrollingOptions(false, IsVertScrollingEnabled, ActualVertScrollBarMode)
			};
		}
		ClientScrollingOptions CreateScrollingOptions(bool isColumn, bool enabled, ScrollBarMode scrollBarMode) {
			ClientPagingOptions pagingOpts = null;
			PivotWebVisualItems items = pivotGrid.Data.VisualItems;
			if(enabled) {
				bool hasPaging = items.HasPaging(isColumn);
				int pageIndex = hasPaging ? items.GetPageIndex(isColumn) : 0;
				int totalRowsCount = items.GetLastLevelUnpagedItemCount(isColumn);
				int pageSize = hasPaging ? items.GetPageSize(isColumn) : totalRowsCount;
				int pageCount = hasPaging ? items.GetPageCount(isColumn) : 1;
				int lastLevelItemsCount = items.GetLastLevelItemCount(isColumn);
				int rowsCount = lastLevelItemsCount;
				var startItem = hasPaging ? items.GetPageStartItem(isColumn) : items.GetLastLevelItem(isColumn, 0);
				var endItem = hasPaging ? items.GetPageEndItem(isColumn) : items.GetLastLevelItem(isColumn, lastLevelItemsCount - 1);
				int pageRowsCount = endItem.MinLastLevelIndex - startItem.MinLastLevelIndex + 1;
				pagingOpts = new ClientPagingOptions {
					PageRowsCount = pageRowsCount,
					RowsCount = rowsCount,
					PageIndex = pageIndex,
					PageSize = pageSize,
					TotalRowsCount = totalRowsCount,
					PageCount = pageCount,
					StartPageCellId = PivotGridHtmlFieldValueCellBase.GetFieldValueCellID(startItem),
					EndPageCellId = PivotGridHtmlFieldValueCellBase.GetFieldValueCellID(endItem)
				};
			}
			ClientScrollingOptions opts = new ClientScrollingOptions {
				Enabled = enabled,
				VirtualPagingEnabled = items.IsVirtualScrollingMode(isColumn),
				ScrollBarMode = scrollBarMode,
				PagingOptions = pagingOpts
			};
			return opts;
		}
		public class ClientDynamicRenderOptions {
			public ClientScrollingOptions Horz { get; set; }
			public ClientScrollingOptions Vert { get; set; }
		}
		public class ClientScrollingOptions {
			public bool Enabled { get; set; }
			public ScrollBarMode ScrollBarMode { get; set; }
			public bool VirtualPagingEnabled { get; set; }
			public ClientPagingOptions PagingOptions { get; set; }
		}
		public class ClientPagingOptions {
			public int RowsCount { get; set; }
			public int PageRowsCount { get; set; }
			public int PageIndex { get; set; }
			public int PageSize { get; set; }
			public int TotalRowsCount { get; set; }
			public int PageCount { get; set; }
			public string StartPageCellId { get; set; }
			public string EndPageCellId { get; set; }
		}
		public string GetRowTreeIE8LayoutFixScript() {
			if(!(RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion == 8) ||
					Data.OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree || Data.RowFieldCount < 2) {
				return string.Empty;
			}
			int maxLevel = -1;
			PivotFieldValueItem maxLevelItem = null;
			for(int i = 0; i < Data.VisualItems.RowCount; i++) {
				PivotFieldValueItem item = Data.VisualItems.GetItem(false, i);
				int level = item.StartLevel;
				if(level > maxLevel) {
					maxLevel = level;
					maxLevelItem = item;
				}
			}
			if(maxLevelItem == null) return string.Empty;
			string clientID = ClientID + "_" + PivotGridHtmlFieldValueCellBase.GetFieldValueCellID(maxLevelItem);
			return "ASPx.pivotGrid_FixIE8RowTreeLayout('" + clientID + "' , '" + ClientID + "');";
		}
		public int GetHeaderFilterPopupHeight() {
			if(!Data.OptionsCustomization.FilterPopupWindowHeight.IsEmpty)
				return (int)Data.OptionsCustomization.FilterPopupWindowHeight.Value;
			return FilterPopupDefaultHeight;
		}
		public int GetHeaderFilterPopupWidth() {
			if(!Data.OptionsCustomization.FilterPopupWindowWidth.IsEmpty) {
				int width = (int)Data.OptionsCustomization.FilterPopupWindowWidth.Value;
				return width < FilterPopupDefaultMinWidth ? FilterPopupDefaultMinWidth : width;
			}
			return FilterPopupDefaultWidth;
		}
		public string GetFilterPopupScript() {
			return String.Format(
				"if(ASPx.pivotGrid_FilterPopupSize[{0}] == null) ASPx.pivotGrid_FilterPopupSize[{0}] = [ ];" +
				"ASPx.pivotGrid_FilterPopupSize[{0}]['default'] = {1};",
				HtmlConvertor.ToScript(PivotGrid.ClientID), HtmlConvertor.ToJSON(new int[] { GetHeaderFilterPopupWidth(), GetHeaderFilterPopupHeight() }));
		}
		public void AddFieldValueContextMenu(string id, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields) {
			object[] contextMenuParams = new object[] { 
					ScriptHelper.GetCollapsedFieldValueChangeState(item, false), 
					item.IsCollapsed,
					item.Field != null ? ((PivotFieldItem)item.Field).ClientID : string.Empty,
					item.VisibleIndex,
					item.CanShowSortBySummary,
					item.Area.ToString(),
					GetSortedFieldsString(sortedFields),
					(item.IsDataLocatedInThisArea || Data.DataFieldCount == 1) && item.DataField != null ? item.DataField.Index : -1,
					item.Index
				};
			if(!fieldValueContextMenuParams.ContainsKey(id))
				fieldValueContextMenuParams.Add(id, contextMenuParams);
		}
		public void AddHeaderContextMenu(string id, PivotFieldItem field) {
			if(!headerContextMenuParams.ContainsKey(id)) {
				string fieldID = field != null ? field.ClientID : "";
				headerContextMenuParams.Add(id, new string[] { fieldID });
			}
		}
		public void ResetMenus() {
			fieldValueContextMenuParams.Clear();
			headerContextMenuParams.Clear();
		}
		public string GetContextMenuScript(string objectName) {
			StringBuilder stb = new StringBuilder();
			GetContextMenuScriptCore(stb, objectName + ".pivotGrid_FieldValueCMParams", fieldValueContextMenuParams);
			GetContextMenuScriptCore(stb, objectName + ".pivotGrid_HeaderCMParams", headerContextMenuParams);
			return stb.ToString();
		}
		protected void GetContextMenuScriptCore(StringBuilder stb, string varName, Dictionary<string, object[]> menuParams) {
			stb.Append(varName + "[\"").Append(ClientID).Append("\"] = [\r\n");
			bool first = true;
			foreach(string elemID in menuParams.Keys) {
				if(!first)
					stb.Append(",\r\n");
				stb.Append("[\"").Append(elemID).Append("\", ");
				object[] pars = menuParams[elemID];
				for(int i = 0; i < pars.Length; i++) {
					AddParamStr(stb, pars[i]);
					if(i != pars.Length - 1)
						stb.Append(", ");
				}
				stb.Append("]");
				first = false;
			}
			stb.Append("];\r\n");
		}
		protected void AddParamStr(StringBuilder stb, object param) {
			if(param == null)
				throw new ArgumentNullException("param");
			if(param is string) {
				stb.Append("\"").Append(param.ToString()).Append("\"");
				return;
			}
			if(param is int) {
				stb.Append((int)param);
				return;
			}
			if(param is bool) {
				stb.Append((bool)param ? "true" : "false");
				return;
			}
			if(param is string[]) {
				AddStringArrayParam(stb, (string[])param);
				return;
			}
			throw new ArgumentException("param");
		}
		protected void AddStringArrayParam(StringBuilder stb, object[] p) {
			stb.Append("[");
			for(int i = 0; i < p.Length; i++) {
				AddParamStr(stb, p[i]);
				if(i != p.Length - 1)
					stb.Append(", ");
			}
			stb.Append("]");
		}
		protected string[] GetSortedFieldsString(List<PivotGridFieldPair> sortedFields) {
			if(sortedFields == null || sortedFields.Count == 0) return new string[0];
			string[] res = new string[sortedFields.Count];
			for(int i = 0; i < sortedFields.Count; i++) {
				PivotGridFieldPair pair = sortedFields[i];
				res[i] = pair.FieldItem.Index.ToString() + "_" + pair.DataFieldItem.Index.ToString();
			}
			return res;
		}
		public string GetHoverScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			foreach(PivotFieldItem field in Fields) {
				if(!field.CanDrag) continue;
				helper.AddStyle(Data.GetHeaderHoverStyle(field), ScriptHelper.GetHeaderID(field), 
					ScriptHelper.FieldHeaderIdPostfixes, PivotGrid.IsEnabled());
			}
			PivotGrid.CustomizationFieldsInternal.AddHoverScript(helper);
			helper.GetCreateHoverScript(stb);
			return stb.ToString();
		}
		public string GetGroupsScript(string objectName) {
			StringBuilder stb = new StringBuilder();
			stb.Append(objectName).Append(".pivotGrid_Groups[\"").Append(ClientID).AppendLine("\"] = [");
			for(int i = 0; i < Groups.Count; i++) {
				PivotGroupItem group = Groups[i];
				stb.Append("\tnew ASPxClientPivotGridGroup(\"").Append(ClientID).Append("\", ");
				GetGroupFields(stb, group);
				stb.Append(")");
				if(i != Groups.Count - 1)
					stb.AppendLine(", ");
				else
					stb.AppendLine();
			}
			stb.Append("];");
			return stb.ToString();
		}
		protected void GetGroupFields(StringBuilder stb, PivotGroupItem group) {
			stb.Append("[");
			List<PivotFieldItemBase> fields = group.GetVisibleFields();
			for(int i = 0; i < fields.Count; i++) {
				PivotFieldItemBase field = fields[i];
				stb.Append("\"").Append(ScriptHelper.GetHeaderID(field)).Append("\"");
				if(i != fields.Count - 1)
					stb.Append(", ");
			}
			stb.Append("]");
		}
	}
}
