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

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Rendering {
	public class GridViewTextBuilder : GridTextBuilder {
		public GridViewTextBuilder(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return (ASPxGridView)base.Grid; } }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		public virtual string GetFooterCaption(GridViewColumn column, string lineDivider) {
			if(column == null) return string.Empty;
			var list = Grid.GetVisibleTotalSummaryItems(column);
			if(list.Count == 0) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < list.Count; n++) {
				if(n > 0) sb.Append(lineDivider);
				ASPxSummaryItem item = list[n];
				object value = Grid.GetTotalSummaryValue(item);
				string text = item.GetTotalFooterDisplayText(Grid.ColumnHelper.FindColumnByString(item.FieldName), value);
				sb.Append(Grid.RaiseSummaryDisplayText(GetSummaryDisplayTextEventArgs(item, value, text)));
			}
			return sb.ToString();
		}
		public virtual string GetGroupRowDisplayText(GridViewDataColumn column, int visibleIndex) {
			string fieldName = column.FieldName;
			if(column.GroupIndex > -1) {
				fieldName = String.Empty;
			}
			object value = DataProxy.GetRowValue(visibleIndex, fieldName);
			GridColumnInfo info = Grid.SortData.GetInfo(column);
			string res = string.Empty;
			DevExpress.Utils.FormatInfo format = null;
			if(info != null) {
				value = info.UpdateGroupDisplayValue(value);
				format = info.GetColumnGroupFormat();
			}
			if(format != null) {
				format.Format = System.Globalization.CultureInfo.CurrentCulture;
				res = format.GetDisplayText(value);
			} else {
				res = GetGroupRowDisplayText(column, visibleIndex, value, true);
			}
			if(info != null) res = info.GetGroupDisplayText(value, res);
			ASPxGridViewColumnDisplayTextEventArgs e = new ASPxGridViewColumnDisplayTextEventArgs(column, visibleIndex, value);
			e.DisplayText = res;
			Grid.RaiseCustomGroupDisplayText(e);
			var text = e.DisplayText;
			if(e.EncodeHtml && text != res)
				text = System.Web.HttpUtility.HtmlEncode(text);
			return text;
		}
		public virtual string GetGroupRowText(GridViewDataColumn column, int visibleIndex) {
			string value = GetGroupRowDisplayText(column, visibleIndex);
			string summary = Grid.GetGroupRowSummaryText(visibleIndex);
			return string.Format(Grid.Settings.GroupFormat, column.ToString(), value, summary);
		}
		public virtual string GetGroupRowFooterText(GridViewColumn column, int visibleIndex) {
			return GetGroupRowFooterText(column, visibleIndex, "<br/>");
		}
		public virtual string GetGroupRowFooterText(GridViewColumn column, int visibleIndex, string lineDivider) {
			if(column == null) return string.Empty;
			List<ASPxSummaryItem> list = Grid.GetGroupFooterSummaryItems(column);
			if(list.Count == 0) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < list.Count; n++) {
				ASPxSummaryItem item = list[n];
				if(n > 0) sb.Append(lineDivider);
				object value = Grid.GetGroupSummaryValue(visibleIndex, item);
				if(value == null) continue;
				string text = item.GetGroupFooterDisplayText(Grid.ColumnHelper.FindColumnByString(item.FieldName), value);
				sb.Append(Grid.RaiseSummaryDisplayText(new ASPxGridViewSummaryDisplayTextEventArgs(item, value, text, visibleIndex, true)));
			}
			return sb.ToString();
		}
		public virtual string GetPreviewText(int visibleIndex) {
			return Grid.GetPreviewText(visibleIndex);
		}
		protected virtual string GetGroupRowDisplayText(IWebGridDataColumn column, int visibleIndex, object value, bool highlightSearchText) {
			return GetDisplayTextCore(column, visibleIndex, value, (editor, col, vIndex, val, encodeValue) => {
				var args = GetDisplayControlArgsCore(col, vIndex, val);
				if(highlightSearchText && RenderHelper.RequireHighlightSearchText(col) && !Grid.IsExported)
					args.HighlightTextProcessor = RenderHelper.GetHighlightTextProcessor(col);
				return editor.GetDisplayText(args);
			});
		}
		protected override ASPxGridSummaryDisplayTextEventArgs GetSummaryDisplayTextEventArgs(ASPxSummaryItemBase item, object value, string text) {
			return new ASPxGridViewSummaryDisplayTextEventArgs((ASPxSummaryItem)item, value, text, -1, false);
		}
	}
	public class GridViewRenderHelper : GridRenderHelper {
		public const string
			HeaderTableID = "DXHeaderTable",
			FixedColumnsDivID = "DXFixedColumnsDiv",
			FixedColumnsContentDivID = "DXFixedColumnsContentDiv",
			FooterRowID = "DXFooterRow",
			DataRowID = "DXDataRow",
			PreviewRowID = "DXPRow",
			DetailRowID = "DXDRow",
			AdaptiveDetailRowID = "DXADRow",
			AdaptiveHeaderPanelID = "DXAHeaderPanel",
			AdaptiveFooterPanelID = "DXAFooterPanel",
			AdaptiveHeaderID = "DXADHeader",
			GroupRowID = "DXGroupRow",
			ParentRowsID = "DXparentrow",
			ParentRowsWindowID = "DXparentrowswindow",
			HeaderRowID = "DXHeadersRow",
			FilterRowID = "DXFilterRow",
			EditingRowID = "DXEditingRow",
			EmptyDataRowID = "DXEmptyRow",
			FilterRowMenuID = "DXFilterRowMenu",
			GroupPanelID = "grouppanel",
			EmptyHeader = "emptyheader",
			AccessibilityHeaderRowFormat = "DXAc{0}Row";
		Dictionary<string, string> autoFilterTextValues;
		public GridViewRenderHelper(ASPxGridView grid)
			: base(grid) {
			this.autoFilterTextValues = new Dictionary<string, string>();
			HeaderTemplates = new TemplateContainerCollection(Grid);
			FilterTemplates = new TemplateContainerCollection(Grid);
			FilterRowTemplates = new TemplateContainerCollection(Grid);
			RowCellTemplates = new TemplateContainerCollection(Grid);
			EditRowCellTemplates = new TemplateContainerCollection(Grid);
			GroupRowTemplates = new TemplateContainerCollection(Grid);
			DetailRowTemplates = new TemplateContainerCollection(Grid);
			PreviewRowTemplates = new TemplateContainerCollection(Grid);
			DataRowTemplates = new TemplateContainerCollection(Grid);
			EmptyDataRowTemplates = new TemplateContainerCollection(Grid);
			FooterRowTemplates = new TemplateContainerCollection(Grid);
			FooterCellTemplates = new TemplateContainerCollection(Grid);
			GroupFooterRowTemplates = new TemplateContainerCollection(Grid);
			GroupFooterCellTemplates = new TemplateContainerCollection(Grid);
			TitleTemplates = new TemplateContainerCollection(Grid);
			StatusBarTemplates = new TemplateContainerCollection(Grid);
			PagerBarTemplates = new TemplateContainerCollection(Grid);
			EditFormTemplates = new TemplateContainerCollection(Grid);
			FormLayoutItemTemplates = new TemplateContainerCollection(Grid);
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new GridViewColumnHelper ColumnHelper { get { return base.ColumnHelper as GridViewColumnHelper; } }
		public GridViewContextMenuStyles StylesContextMenu { get { return Grid.StylesContextMenu; } }
		public new GridViewStyles Styles { get { return base.Styles as GridViewStyles; } }
		public new GridViewImages Images { get { return base.Images as GridViewImages; } }
		public new ASPxGridViewScripts Scripts { get { return base.Scripts as ASPxGridViewScripts; } }
		public new GridViewTextBuilder TextBuilder { get { return base.TextBuilder as GridViewTextBuilder; } }
		public TemplateContainerCollection HeaderTemplates { get; private set; }
		public TemplateContainerCollection FilterTemplates { get; private set; }
		public TemplateContainerCollection FilterRowTemplates { get; private set; }
		public TemplateContainerCollection RowCellTemplates { get; private set; }
		public TemplateContainerCollection EditRowCellTemplates { get; private set; }
		public TemplateContainerCollection GroupRowTemplates { get; private set; }
		public TemplateContainerCollection DetailRowTemplates { get; private set; }
		public TemplateContainerCollection PreviewRowTemplates { get; private set; }
		public TemplateContainerCollection DataRowTemplates { get; private set; }
		public TemplateContainerCollection EmptyDataRowTemplates { get; private set; }
		public TemplateContainerCollection FooterRowTemplates { get; private set; }
		public TemplateContainerCollection GroupFooterRowTemplates { get; private set; }
		public TemplateContainerCollection FooterCellTemplates { get; private set; }
		public TemplateContainerCollection GroupFooterCellTemplates { get; private set; }
		public TemplateContainerCollection TitleTemplates { get; private set; }
		public TemplateContainerCollection StatusBarTemplates { get; private set; }
		public TemplateContainerCollection PagerBarTemplates { get; private set; }
		public TemplateContainerCollection EditFormTemplates { get; private set; }
		public TemplateContainerCollection FormLayoutItemTemplates { get; private set; }
		public string GetColumnFilterEditId(IWebGridColumn column) {
			return FilterRowEditorID + GetColumnGlobalIndex(column).ToString();
		}
		public string GetDetailRowId(int visibleIndex) {
			return DetailRowID + GetVisibleIndexString(visibleIndex);
		}
		public string GetSampleAdaptiveDetailRowId() {
			return AdaptiveDetailRowID;
		}
		public string GetRowId(int visibleIndex) {
			return DataProxy.GetRowType(visibleIndex) == WebRowType.Data ? GetDataRowId(visibleIndex) : GetGroupRowId(visibleIndex);
		}
		public string GetDataRowId(int visibleIndex) {
			return DataRowID + GetVisibleIndexString(visibleIndex);
		}
		public string GetPreviewRowId(int visibleIndex) {
			return PreviewRowID + GetVisibleIndexString(visibleIndex);
		}
		public string GetGroupRowId(int visibleIndex) {
			string prefix = DataProxy.IsRowExpanded(visibleIndex) ? GroupRowID + GroupRowStringExpandedSuffix : GroupRowID;
			return prefix + GetVisibleIndexString(visibleIndex);
		}
		public bool HasFixedColumns {
			get { return ShowHorizontalScrolling && Grid.FixedColumnCount > 0 && GroupCount == 0 && !Grid.Settings.ShowPreview && !HasDetailRows && Grid.Templates.DataRow == null; }
		}
		public override bool AllowRemoveCellRightBorder {
			get {
				if(IsRightToLeft)
					return !ShowVerticalScrolling;
				return !HasScrolling && !HasFixedColumns;
			}
		}
		public Dictionary<string, string> AutoFilterTextValues {
			get {
				if(autoFilterTextValues.Count == 0)
					PopulateAutoFilterTextValues(autoFilterTextValues);
				return autoFilterTextValues;
			}
		}
		public bool AllowMultiColumnAutoFilter { get { return Grid.SettingsBehavior.FilterRowMode == GridViewFilterRowMode.OnClick; } }
		public override void Invalidate() {
			base.Invalidate();
			this.autoFilterTextValues.Clear();
			this.autorFilterTextValuesPopuplated = false;
		}
		protected override void InvalidateTemplates() {
			base.InvalidateTemplates();
			DetailRowTemplates.Clear();
			PreviewRowTemplates.Clear();
			HeaderTemplates.Clear();
			EditRowCellTemplates.Clear();
			RowCellTemplates.Clear();
			GroupRowTemplates.Clear();
			DataRowTemplates.Clear();
			EmptyDataRowTemplates.Clear();
			FooterRowTemplates.Clear();
			FooterCellTemplates.Clear();
			TitleTemplates.Clear();
			StatusBarTemplates.Clear();
			PagerBarTemplates.Clear();
			EditFormTemplates.Clear();
			FormLayoutItemTemplates.Clear();
		}
		protected override GridCookiesBase CreateGridSEO() {
			return new GridViewSEOProcessing(Grid);
		}
		protected override ASPxGridScripts CreateGridScripts() {
			return new ASPxGridViewScripts(Grid);
		}
		protected override GridTextBuilder CreateTextBuilder() {
			return new GridViewTextBuilder(Grid);
		}
		public ASPxEditBase CreateAutoFilterEditor(TableCell cell, GridViewDataColumn column, object value, EditorInplaceMode mode) {
			EditPropertiesBase properties = GetColumnAutoFilterEdit(column);
			AutoFilterCondition condition = Grid.FilterHelper.GetColumnAutoFilterCondition(column);
			CreateEditControlArgs args = new CreateEditControlArgs(value, column.GetDataType(),
				Grid.ImagesEditors, StylesEditors, Grid, mode, InplaceAllowEditorSizeRecalc);
			if(condition == AutoFilterCondition.Equals && value != null && !string.IsNullOrEmpty(value.ToString())) {
				args.EditValue = DataProxy.ConvertValue(column.FieldName, value); 
			}
			ASPxGridViewEditorCreateEventArgs ei = new ASPxGridViewEditorCreateEventArgs(column, -1, properties, null, args.EditValue);
			Grid.RaiseAutoFilterEditorCreate(ei);
			args.EditValue = ei.Value;
			ASPxEditBase editor = ei.EditorProperties.CreateEdit(args);
			EditorsIntegrationHelper.DisableValidation(editor);
			SetupFilterEditor(editor);
			editor.ID = GetColumnFilterEditId(column);
			editor.EnableClientSideAPI = true;
			editor.EnableViewState = false;
			ClientIDHelper.EnableClientIDGeneration(editor);
			if(Grid.DesignMode) {
				editor.DataSource = null;
				editor.DataSourceID = "";
			}
			cell.Controls.Add(editor);
			if(Grid.AccessibilityCompliant) {
				string caption = String.Format(ASPxperienceLocalizer.GetString(ASPxperienceStringId.GridViewFilterRow_InputTitle), 
					Grid.RenderHelper.TextBuilder.GetHeaderCaption(column));
				editor.AccessibilityInputTitle = caption;
			}
			editor.DataBind();  
			ASPxGridViewEditorEventArgs e = new ASPxGridViewEditorEventArgs(column, -1, editor, null, editor.Value);
			Grid.RaiseAutoFilterEditorInitialize(e);
			editor.Properties.ResetRenderStyles();
			EditorList.Add(e.Editor);
			return e.Editor;
		}
		bool autorFilterTextValuesPopuplated = false;
		protected virtual void PopulateAutoFilterTextValues(Dictionary<string, string> result) {
			if(autorFilterTextValuesPopuplated)
				return;
			var visibleColumnsTexts = new Dictionary<string, string>();
			foreach(var pair in Grid.ColumnFilterInfo) {
				var prop = pair.Key;
				if(object.ReferenceEquals(prop, null))
					continue;
				var dataColumn = ColumnHelper.FindColumnByKey(prop.PropertyName) as GridViewDataColumn;
				if(dataColumn == null || !ColumnHelper.AllVisibleColumns.Contains(dataColumn))
					continue;
				visibleColumnsTexts[dataColumn.FieldName] = Grid.FilterHelper.GetColumnAutoFilterText(dataColumn, pair.Value);
			}
			if(visibleColumnsTexts.Count > 0) {
				var args = new ASPxGridViewOnClickRowFilterEventArgs(GridViewAutoFilterEventKind.ExtractDisplayText);
				args.Values = visibleColumnsTexts;
				Grid.RaiseProcessOnClickRowFilter(args);
				foreach(var pair in args.Values)
					result.Add(pair.Key, pair.Value);
			}
			this.autorFilterTextValuesPopuplated = true;
		}
		public virtual string GetColumnAutoFilterText(GridViewDataColumn column) {
			if(AllowMultiColumnAutoFilter) {
				if(AutoFilterTextValues.ContainsKey(column.FieldName))
					return AutoFilterTextValues[column.FieldName];
				return string.Empty;
			}
			var op = Grid.GetColumnFilter(column);
			if(object.ReferenceEquals(op, null))
				return string.Empty;
			string res = Grid.FilterHelper.GetColumnAutoFilterText(column, op);
			ASPxGridViewAutoFilterEventArgs e = new ASPxGridViewAutoFilterEventArgs(column, op, GridViewAutoFilterEventKind.ExtractDisplayText, res);
			Grid.RaiseProcessColumnAutoFilter(e);
			return e.Value;
		}
		public int IndentColumnCount { get { return GroupCount + (HasDetailButton ? 1 : 0) + (HasAdaptiveDetailButtonOnTheLeft ? 1 : 0); } }
		public int TotalSpanCount { get { return ColumnSpanCount + IndentColumnCount + (HasAdaptiveDetailButtonOnTheRight ? 1 : 0); } }
		public int ColumnSpanCount { get { return Math.Max(1, ColumnHelper.Leafs.Count); } }
		public int GroupCount { get { return Grid.GroupCount; } }
		public GridViewCommandColumn FirstCommandColumnWithNewButton { get { return Grid.AllColumns.OfType<GridViewCommandColumn>().FirstOrDefault(c => c.ShowNewButton); } }
		public bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		public override bool RequireHeaderTopBorder { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Bottom && !Grid.Settings.ShowGroupPanel; } }
		public override bool RequireFixedTableLayout { get { return base.RequireFixedTableLayout || Grid.Settings.UseFixedTableLayout || AllowColumnResizing; } }
		public override bool AllowColumnResizing { get { return Grid.SettingsBehavior.ColumnResizeMode != ColumnResizeMode.Disabled; } }
		public override bool RequireTablesHelperScripts { get { return base.RequireTablesHelperScripts || AllowColumnResizing; } }
		public bool RequireRenderFilterRowMenu {
			get {
				if(!Grid.Settings.ShowFilterRow) return false;
				foreach(GridViewDataColumn column in Grid.DataColumns) {
					if(IsFilterRowMenuIconVisible(column))
						return true;
				}
				return false;
			}
		}
		public override bool RequireRenderCustomizationWindow {
			get {
				if(Grid.SettingsBehavior.EnableCustWindowPropertyChanged)
					return Grid.SettingsBehavior.EnableCustomizationWindow;
				return Grid.SettingsCustomizationWindowInternal.EnabledInternal;
			}
		}
		public bool RequireRenderContextMenu { get { return RequireRenderGroupPanelContextMenu || RequireRenderColumnsContextMenu || RequireRenderRowsContextMenu || RequireRenderFooterContextMenu; } }
		public bool RequireRenderGroupPanelContextMenu {
			get {
				if(Grid.SettingsContextMenu.EnableGroupPanelMenu == DefaultBoolean.Default)
					return Grid.SettingsContextMenu.Enabled;
				if(!Grid.Settings.ShowGroupPanel)
					return false;
				return Grid.SettingsContextMenu.EnableGroupPanelMenu == DefaultBoolean.True;
			}
		}
		public bool RequireRenderColumnsContextMenu {
			get {
				if(Grid.SettingsContextMenu.EnableColumnMenu == DefaultBoolean.Default)
					return Grid.SettingsContextMenu.Enabled;
				return Grid.SettingsContextMenu.EnableColumnMenu == DefaultBoolean.True;
			}
		}
		public bool RequireRenderRowsContextMenu {
			get {
				if(Grid.SettingsContextMenu.EnableRowMenu == DefaultBoolean.Default)
					return Grid.SettingsContextMenu.Enabled;
				return Grid.SettingsContextMenu.EnableRowMenu == DefaultBoolean.True;
			}
		}
		public bool RequireRenderFooterContextMenu {
			get {
				if(Grid.SettingsContextMenu.EnableFooterMenu == DefaultBoolean.Default)
					return Grid.SettingsContextMenu.Enabled;
				if(!Grid.Settings.ShowFooter)
					return false;
				return Grid.SettingsContextMenu.EnableFooterMenu == DefaultBoolean.True;
			}
		}
		public bool RequireLastVisibleRowBottomBorder { get { return AllowBatchEditing && (HasScrolling || RequireRenderBottomPagerControl || ShowFooter); } }
		public bool UseFixedGroups { get { return Grid.SettingsBehavior.AllowFixedGroups && ShowVerticalScrolling; } }
		public bool IsRemoveBorderFromMainTableLastRow { get { return IsRemoveBorderFromMainTableLastNewItemRow && !RequireRenderNewItemRowAtBottom; } }
		public bool IsRemoveBorderFromMainTableLastNewItemRow { get { return !RequireRenderBottomPagerControl && !ShowVerticalScrolling && !ShowFooter; } }
		protected bool RequireRenderNewItemRowAtBottom { get { return DataProxy.IsNewRowEditing && Grid.SettingsEditing.NewItemRowPosition == GridViewNewItemRowPosition.Bottom; } }
		public bool HasAdaptivity { get { return Grid.IsAdaptivityEnabled(); } }
		public bool HasDetailRows { get { return Grid.SettingsDetail.ShowDetailRow; } }
		public bool HasDetailButton { get { return HasDetailRows && Grid.SettingsDetail.ShowDetailButtons; } }
		public bool HasAdaptiveDetailButtonOnTheLeft {
			get { return HasAdaptivity && Grid.SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCells && 
				Grid.SettingsAdaptivity.AdaptiveColumnPosition == GridViewAdaptiveColumnPosition.Left; }
		}
		public bool HasAdaptiveDetailButtonOnTheRight { 
			get { return HasAdaptivity && Grid.SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCells && 
				Grid.SettingsAdaptivity.AdaptiveColumnPosition == GridViewAdaptiveColumnPosition.Right; } 
		}
		public bool HasDetailRow(int visibleIndex) { return HasDetailRows && DataProxy.DetailRows.IsVisible(visibleIndex); }
		public bool HasGroupRowFooter(int visibleIndex) { return GetGroupFooterVisibleIndexes(visibleIndex) != null; }
		public List<int> GetGroupFooterVisibleIndexes(int visibleIndex) {
			if(Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.Hidden) return null;
			return DataProxy.GetGroupFooterVisibleIndexes(visibleIndex, Grid.Settings.ShowGroupFooter == GridViewGroupFooterMode.VisibleIfExpanded);
		}
		public override bool HasAnySelectAllCheckbox {
			get {
				if(AllowSelectSingleRowOnly || DataProxy.VisibleRowCount == 0)
					return false;
				return ColumnHelper.AllVisibleColumns.OfType<GridViewCommandColumn>().Any(c => c.SelectAllCheckboxMode != GridViewSelectAllCheckBoxMode.None);
			}
		}
		protected override bool HasAnySelectCheckBoxInternal { get { return ColumnHelper.AllVisibleColumns.OfType<GridViewCommandColumn>().Any(c => c.ShowSelectCheckbox); } }
		public override Unit GetPopupEditFormWidth() {
			if(!SettingsPopup.EditForm.Width.IsEmpty)
				return SettingsPopup.EditForm.Width;
			return Grid.SettingsEditing.Width;
		}
		public override Unit GetPopupEditFormHeight() {
			if(!SettingsPopup.EditForm.Height.IsEmpty)
				return SettingsPopup.EditForm.Height;
			return Grid.SettingsEditing.Height;
		}
		public override PopupHorizontalAlign GetPopupEditFormHorizontalAlign() {
			if(SettingsPopup.EditForm.IsPropertyChanged("HorizontalAlign"))
				return SettingsPopup.EditForm.HorizontalAlign;
			return Grid.SettingsEditing.HorizontalAlign;
		}
		public override PopupVerticalAlign GetPopupEditFormVerticalAlign() {
			if(SettingsPopup.EditForm.IsPropertyChanged("VerticalAlign"))
				return SettingsPopup.EditForm.VerticalAlign;
			return Grid.SettingsEditing.VerticalAlign;
		}
		public override int GetPopupEditFormHorizontalOffset() {
			if(SettingsPopup.EditForm.IsPropertyChanged("HorizontalOffset"))
				return SettingsPopup.EditForm.HorizontalOffset;
			return Grid.SettingsEditing.HorizontalOffset;
		}
		public override int GetPopupEditFormVerticalOffset() {
			if(SettingsPopup.EditForm.IsPropertyChanged("VerticalOffset"))
				return SettingsPopup.EditForm.VerticalOffset;
			return Grid.SettingsEditing.VerticalOffset;
		}
		public override bool GetPopupEditFormShowHeader() {
			if(SettingsPopup.EditForm.IsPropertyChanged("ShowHeader"))
				return SettingsPopup.EditForm.ShowHeader;
			return Grid.SettingsEditing.ShowHeader;
		}
		public override bool GetPopupEditFormAllowResize() {
			if(SettingsPopup.EditForm.IsPropertyChanged("AllowResize"))
				return SettingsPopup.EditForm.AllowResize;
			return Grid.SettingsEditing.AllowResize;
		}
		public override bool GetPopupEditFormModal() {
			if(SettingsPopup.EditForm.IsPropertyChanged("Modal"))
				return SettingsPopup.EditForm.Modal;
			return Grid.SettingsEditing.Modal;
		}
		public override Unit GetCustomizationWindowWidth() {
			if(!SettingsPopup.CustomizationWindow.Width.IsEmpty)
				return SettingsPopup.CustomizationWindow.Width;
			if(!Grid.SettingsCustomizationWindowInternal.WidthInternal.IsEmpty)
				return Grid.SettingsCustomizationWindowInternal.WidthInternal;
			return GridViewCustomizationWindowPopupSettings.DefaultWidth;
		}
		public override Unit GetCustomizationWindowHeight() {
			if(!SettingsPopup.CustomizationWindow.Height.IsEmpty) {
				if(SettingsPopup.CustomizationWindow.Height.Type == UnitType.Percentage)
					return Unit.Empty;
				return SettingsPopup.CustomizationWindow.Height;
			}
			if(!Grid.SettingsCustomizationWindowInternal.HeightInternal.IsEmpty)
				return Grid.SettingsCustomizationWindowInternal.HeightInternal;
			return GridViewCustomizationWindowPopupSettings.DefaultHeight;
		}
		public override PopupHorizontalAlign GetCustomizationWindowHorizontalAlign() {
			if(SettingsPopup.CustomizationWindow.IsPropertyChanged("HorizontalAlign"))
				return SettingsPopup.CustomizationWindow.HorizontalAlign;
			return Grid.SettingsCustomizationWindowInternal.HorizontalAlign;
		}
		public override PopupVerticalAlign GetCustomizationWindowVerticalAlign() {
			if(SettingsPopup.CustomizationWindow.IsPropertyChanged("VerticalAlign"))
				return SettingsPopup.CustomizationWindow.VerticalAlign;
			return Grid.SettingsCustomizationWindowInternal.VerticalAlign;
		}
		public override int GetCustomizationWindowHorizontalOffset() {
			if(SettingsPopup.CustomizationWindow.IsPropertyChanged("HorizontalOffset"))
				return SettingsPopup.CustomizationWindow.HorizontalOffset;
			return Grid.SettingsCustomizationWindowInternal.HorizontalOffset;
		}
		public override int GetCustomizationWindowVerticalOffset() {
			if(SettingsPopup.CustomizationWindow.IsPropertyChanged("VerticalOffset"))
				return SettingsPopup.CustomizationWindow.VerticalOffset;
			return Grid.SettingsCustomizationWindowInternal.VerticalOffset;
		}
		public override GridCommandColumnButtonControl CreateCommandButtonControl(WebColumnBase column, GridCommandButtonType buttonType, int visibleIndex, bool postponeClick) {
			if(!CanCreateCommandButton(buttonType))
				return null;
			var commandColumn = column as GridViewCommandColumn;
			var buttonSettings = GetCommandButtonSettings(buttonType);
			var renderType = GetButtonType(buttonSettings, commandColumn != null ? commandColumn.ButtonType : GridCommandButtonRenderMode.Default);
			var style = new ButtonControlStyles(null);
			style.CopyFrom(buttonSettings.Styles);
			var image = GetCommandButtonImage(buttonType, buttonSettings);
			var text = buttonSettings.Text;
			if(string.IsNullOrEmpty(text))
				text = Grid.SettingsText.GetCommandButtonText(buttonType, AllowBatchEditing); 
			var isNewRow = DataProxy.IsNewRowEditing && visibleIndex == BaseListSourceDataController.NewItemRow;
			var isEditingRow = isNewRow || visibleIndex >= 0 && DataProxy.IsRowEditing(visibleIndex);
			var args = new ASPxGridViewCommandButtonEventArgs(commandColumn, ConvertButtonType(buttonType), text, image, style, visibleIndex, isEditingRow, renderType); 
			Grid.RaiseCommandButtonInitialize(args);
			if(!args.Visible)
				return null;
			return new GridCommandColumnButtonControl(args, Grid, GetCommandButtonClickHandlerArgs(buttonType), postponeClick);
		}
		protected virtual ImageProperties GetCommandButtonImage(GridCommandButtonType buttonType, GridCommandButtonSettings buttonSettings) {
			ImageProperties image = new ImageProperties();
			if(buttonSettings.ButtonType == GridCommandButtonRenderMode.Image) {
				if(buttonType == GridCommandButtonType.HideAdaptiveDetail)
					image.CopyFrom(Images.GetImageProperties(Grid.Page, GridViewImages.HideAdaptiveDetailButtonName));
				if(buttonType == GridCommandButtonType.ShowAdaptiveDetail)
					image.CopyFrom(Images.GetImageProperties(Grid.Page, GridViewImages.ShowAdaptiveDetailButtonName));
			}
			image.CopyFrom(buttonSettings.Image);
			return image;
		}
		protected override GridCommandButtonSettings GetCommandButtonSettings(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.ShowAdaptiveDetail:
					return Grid.SettingsCommandButton.ShowAdaptiveDetailButton;
				case GridCommandButtonType.HideAdaptiveDetail:
					return Grid.SettingsCommandButton.HideAdaptiveDetailButton;
				case GridCommandButtonType.ApplyFilter:
					return Grid.SettingsCommandButton.ApplyFilterButton;
				case GridCommandButtonType.ClearFilter:
					return Grid.SettingsCommandButton.ClearFilterButton;
			}
			return base.GetCommandButtonSettings(buttonType);
		}
		protected override GetCommandColumnButtonClickHandlerArgs GetCommandButtonClickHandlerArgs(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.ApplyFilter:
					return Scripts.GetApplyOnClickRowFilterFuncArgs;
				case GridCommandButtonType.ClearFilter:
					return Scripts.GetClearFilterFuncArgs;
				case GridCommandButtonType.ShowAdaptiveDetail:
					return Scripts.GetShowAdaptiveDetailFuncArgs;
				case GridCommandButtonType.HideAdaptiveDetail:
					return Scripts.GetHideAdaptiveDetailFuncArgs;
			}
			return base.GetCommandButtonClickHandlerArgs(buttonType);
		}
		public static ColumnCommandButtonType ConvertButtonType(GridCommandButtonType source) {
			switch(source) {
				case GridCommandButtonType.Edit:
					return ColumnCommandButtonType.Edit;
				case GridCommandButtonType.New:
					return ColumnCommandButtonType.New;
				case GridCommandButtonType.Delete:
					return ColumnCommandButtonType.Delete;
				case GridCommandButtonType.Select:
					return ColumnCommandButtonType.Select;
				case GridCommandButtonType.Update:
					return ColumnCommandButtonType.Update;
				case GridCommandButtonType.Cancel:
					return ColumnCommandButtonType.Cancel;
				case GridCommandButtonType.ClearFilter:
					return ColumnCommandButtonType.ClearFilter;
				case GridCommandButtonType.SelectCheckbox:
					return ColumnCommandButtonType.SelectCheckbox;
				case GridCommandButtonType.ApplyFilter:
					return ColumnCommandButtonType.ApplyFilter;
				case GridCommandButtonType.ApplySearchPanelFilter:
					return ColumnCommandButtonType.ApplySearchPanelFilter;
				case GridCommandButtonType.ClearSearchPanelFilter:
					return ColumnCommandButtonType.ClearSearchPanelFilter;
				case GridCommandButtonType.ShowAdaptiveDetail:
					return ColumnCommandButtonType.ShowAdaptiveDetail;
				case GridCommandButtonType.HideAdaptiveDetail:
					return ColumnCommandButtonType.HideAdaptiveDetail;
			}
			throw new ArgumentException();
		}
		public virtual bool ShowVerticalGridLine { get { return Grid.Settings.GridLines == GridLines.Both || Grid.Settings.GridLines == GridLines.Vertical; } }
		public virtual bool ShowHorizontalGridLine { get { return Grid.Settings.GridLines == GridLines.Both || Grid.Settings.GridLines == GridLines.Horizontal; } }
		public override ImageProperties GetHeaderFilterPopupSizeGripImage() {
			return GetImage(GridViewImages.WindowResizerImageName);
		}
		public override ImageProperties GetHeaderFilterPopupSizeGripRtlImage() {
			return GetImage(GridViewImages.WindowResizerRtlImageName);
		}
		public bool HasPreviewRow(int visibleIndex) {
			if(!Grid.Settings.ShowPreview) return false;
			string text = TextBuilder.GetPreviewText(visibleIndex);
			if(text != null && text.Trim().Length > 0) return true;
			return Grid.Templates.PreviewRow != null;
		}
		public void SetCellWidthIfRequired(GridViewColumn column, TableCell cell, int visibleIndex) {
			if(RequireSetCellWidth(visibleIndex))
				cell.Width = column.Width;
		}
		bool RequireSetCellWidth(int visibleIndex) {
			if(RequireFixedTableLayout || Grid.Settings.ShowColumnHeaders)
				return false;
			if(!Grid.IsEditing || Grid.SettingsEditing.DisplayEditingRow)
				return visibleIndex == DataProxy.VisibleStartIndex;
			if(Grid.IsNewRowEditing) {
				if(Grid.VisibleRowCount < 1 && Grid.SettingsEditing.IsInline)
					return visibleIndex == DevExpress.Data.BaseListSourceDataController.NewItemRow;
				return visibleIndex == DataProxy.VisibleStartIndex;
			}
			if(Grid.VisibleRowCount < 2 || Grid.EditingRowVisibleIndex > Grid.VisibleStartIndex)
				return visibleIndex == DataProxy.VisibleStartIndex;
			return visibleIndex == 1 + Grid.EditingRowVisibleIndex;
		}
		public GridViewContextMenu GetContextMenu(GridViewContextMenuType type) {
			switch(type) {
				case GridViewContextMenuType.GroupPanel:
					return Grid.ContainerControl.GroupPanelContextMenu;
				case GridViewContextMenuType.Columns:
					return Grid.ContainerControl.ColumnsContextMenu;
				case GridViewContextMenuType.Rows:
					return Grid.ContainerControl.RowsContextMenu;
				case GridViewContextMenuType.Footer:
					return Grid.ContainerControl.FooterContextMenu;
			}
			return null;
		}
		public bool IsHideDataCellsWindowLimitMode() {
			return Grid.SettingsAdaptivity.AdaptivityMode == GridViewAdaptivityMode.HideDataCellsWindowLimit;
		}
		public bool IsFilterRowMenuIconVisible(GridViewColumn column) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(dataColumn == null) return false;
			if(Grid.FilterHelper.GetFilterRowTypeKind(dataColumn) == FilterRowTypeKind.SingleOption)
				return false;
			if(dataColumn.Settings.ShowFilterRowMenu == DefaultBoolean.True) return true;
			if(dataColumn.Settings.ShowFilterRowMenu == DefaultBoolean.False) return false;
			return Grid.Settings.ShowFilterRowMenu;
		}
		public bool IsFilterRowMenuLikeItemVisible(GridViewDataColumn column) {
			if(column.Settings.ShowFilterRowMenuLikeItem == DefaultBoolean.True) return true;
			if(column.Settings.ShowFilterRowMenuLikeItem == DefaultBoolean.False) return false;
			return Grid.Settings.ShowFilterRowMenuLikeItem;
		}
		protected virtual void SetupFilterEditor(ASPxEditBase editor) {
			ASPxDateEdit date = editor as ASPxDateEdit;
			if(date != null) date.CalendarProperties.ShowClearButton = true;
		}
		public EditPropertiesBase GetColumnAutoFilterEdit(GridViewDataColumn column) {
			EditPropertiesBase res = GetColumnEdit(column);
			if(res is CheckBoxProperties) {
				CheckBoxProperties check = res as CheckBoxProperties;
				ComboBoxProperties combo = new ComboBoxProperties();
				combo.ValueType = check.ValueType;
				combo.Items.Add(check.DisplayTextChecked, check.ValueChecked);
				combo.Items.Add(check.DisplayTextUnchecked, check.ValueUnchecked);
				return combo;
			}
			if(column.GetFilterMode() != ColumnFilterMode.DisplayText) {
				if(res is DateEditProperties) return res;
				if(res is ComboBoxProperties) return res;
				if(res is SpinEditProperties) return res;
				if(res is ButtonEditProperties) return res;
			} else {
				return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
			}
			return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
		}
		public bool RequireExtraCell { get { return ShowHorizontalScrolling; } }
		public void AddHorzScrollExtraCell(TableRow row) {
			if(RequireExtraCell)
				row.Cells.Add(new GridViewTableHorzScrollExtraCell(this));
		}
		public virtual TableCell CreateContentCell(GridViewTableDataRow row, GridViewColumn column, int index, int visibleRowIndex) {
			if(column is GridViewDataColumn) return new GridViewTableDataCell(this, row, column as GridViewDataColumn, visibleRowIndex, ShouldRemoveLeftBorder(index), ShouldRemoveRightBorder(index));
			if(column is GridViewCommandColumn) return new GridViewTableCommandCell(this, row, column as GridViewCommandColumn, visibleRowIndex, ShouldRemoveLeftBorder(index), ShouldRemoveRightBorder(index));
			if(column is GridViewBandColumn) return new GridViewTableEmptyBandCell(this, column, visibleRowIndex, ShouldRemoveLeftBorder(index), ShouldRemoveRightBorder(index));
			return RenderUtils.CreateTableCell();
		}
		public virtual TableCell CreateInlineEditorCell(GridViewColumn column, int index, int visibleRowIndex) {
			if(column is GridViewCommandColumn) return new GridViewTableCommandCell(this, column as GridViewCommandColumn, visibleRowIndex, ShouldRemoveLeftBorder(index), ShouldRemoveRightBorder(index));
			if(column is GridViewDataColumn) {
				return new GridViewTableInlineEditorCell(this, column as GridViewDataColumn, visibleRowIndex, ShouldRemoveLeftBorder(index), ShouldRemoveRightBorder(index));
			}
			return RenderUtils.CreateTableCell();
		}
		public virtual AppearanceStyle GetGroupPanelStyle() {
			return MergeStyle<AppearanceStyle>(GridViewStyles.GroupPanelStyleName, Styles.GroupPanel);
		}
		protected override GridHeaderStyle GetHeaderStyleCore(IWebGridColumn column) {
			GridViewHeaderStyle result = MergeStyle<GridViewHeaderStyle>(GridStyles.HeaderStyleName, Styles.Header);
			var gridColumn = column as GridViewColumn;
			if(gridColumn != null)
				result.CopyFrom(gridColumn.HeaderStyle);
			if(IsRightToLeft && result.HorizontalAlign == HorizontalAlign.NotSet)
				result.HorizontalAlign = HorizontalAlign.Right;
			return result;
		}
		public GridViewHeaderPanelStyle GetHeaderPanelStyle() {
			return MergeStyle<GridViewHeaderPanelStyle>(GridViewStyles.HeaderPanelStyleName, Styles.HeaderPanel);
		}
		public GridViewStyleBase GetAdaptiveHeaderPanelStyle() {
			return MergeStyle<GridViewStyleBase>(GridViewStyles.AdaptiveHeaderPanelStyleName, Styles.AdaptiveHeaderPanel);
		}
		public GridViewStyleBase GetAdaptiveFooterPanelStyle() {
			return MergeStyle<GridViewStyleBase>(GridViewStyles.AdaptiveFooterPanelStyleName, Styles.AdaptiveFooterPanel);
		}
		public GridViewRowStyle GetFilterRowStyle() {
			return MergeStyle<GridViewRowStyle>(GridViewStyles.FilterRowStyleName, Styles.FilterRow);
		}
		public GridViewFilterCellStyle GetFilterCellStyle(GridViewDataColumn column) {
			GridViewFilterCellStyle result = new GridViewFilterCellStyle();
			result.CopyFrom(Styles.FilterCell);
			result.CopyFrom(column.FilterCellStyle);
			ApplyDisplayControlTextAlign(result, column);
			return result;
		}
		public GridViewGroupRowStyle GetFocusedGroupRowStyle() {
			return MergeStyle<GridViewGroupRowStyle>(GridViewStyles.FocusedGroupRowStyleName, Styles.GroupRow, Styles.FocusedGroupRow);
		}
		public override AppearanceStyleBase GetRowHotTrackStyle() {
			return MergeStyle<AppearanceStyleBase>(GridViewStyles.DataRowHoverStyleName, Styles.RowHotTrack);
		}
		public GridViewDataRowStyle GetEmptyDataRowStyle() {
			return MergeStyle<GridViewDataRowStyle>(GridViewStyles.EmptyDataRowStyleName, Styles.EmptyDataRow);
		}
		public GridViewDataRowStyle GetSelectedRowStyle() {
			return MergeStyle<GridViewDataRowStyle>(GridViewStyles.SelectedRowStyleName, Styles.SelectedRow);
		}
		public GridViewDataRowStyle GetFocusedRowStyle() {
			return MergeStyle<GridViewDataRowStyle>(GridViewStyles.FocusedRowStyleName, Styles.FocusedRow);
		}
		public GridViewInlineEditRowStyle GetInlineEditRowStyle() {
			return MergeStyle<GridViewInlineEditRowStyle>(GridViewStyles.InlineEditRowStyleName, Styles.InlineEditRow);
		}
		public GridViewEditCellStyle GetInlineEditCellStyle(GridViewDataColumn column) {
			GridViewEditCellStyle result = MergeStyle<GridViewEditCellStyle>(GridViewStyles.InlineEditCellStyleName, Styles.InlineEditCell, column.EditCellStyle);
			ApplyDisplayControlTextAlign(result, column);
			return result;
		}
		public GridViewDataRowStyle GetEditFormDisplayRowStyle() {
			return MergeStyle<GridViewDataRowStyle>(GridViewStyles.EditFormDisplayRowStyleName, Styles.Row, Styles.EditFormDisplayRow);
		}
		public AppearanceStyleBase GetEditFormRowStyle() {
			AppearanceStyle result = MergeStyle<AppearanceStyle>(GridViewStyles.EditFormStyleName);
			result.CopyFrom(Styles.Row, true);
			result.CopyFrom(Styles.EditForm, true);
			return result;
		}
		public AppearanceStyleBase GetEditFormRowCellStyle() {
			AppearanceStyle result = new AppearanceStyle();
			CopyCellAttributes(Styles.EditForm, result);
			return result;
		}
		public GridViewEditFormTableStyle GetEditFormTableStyle() {
			return MergeStyle<GridViewEditFormTableStyle>(GridViewStyles.EditFormTableStyleName, Styles.EditFormTable);
		}
		public GridViewEditFormCaptionStyle GetEditFormEditorCaptionStyle(GridViewDataColumn column) {
			return MergeStyle<GridViewEditFormCaptionStyle>(GridViewStyles.EditFormCaptionStyleName, Styles.EditFormColumnCaption, column.EditFormCaptionStyle);
		}
		public GridViewEditCellStyle GetEditFormEditorCellStyle(GridViewDataColumn column) {
			return MergeStyle<GridViewEditCellStyle>(GridViewStyles.EditFormCellStyleName, Styles.EditFormCell, column.EditCellStyle);
		}
		public override AppearanceStyle GetCommandItemsCellStyle() {
			return MergeStyle<AppearanceStyle>(GridViewStyles.CommandColumnStyleName, Styles.CommandColumn);
		}
		public GridViewRowStyle GetEditingErrorRowStyle() {
			return MergeStyle<GridViewRowStyle>(GridViewStyles.EditingErrorRowStyleName, Styles.EditingErrorRow);
		}
		public GridViewFooterStyle GetFooterStyle() {
			return MergeStyle<GridViewFooterStyle>(GridViewStyles.FooterStyleName, Styles.Footer);
		}
		public GridViewFooterStyle GetFooterCellStyle(GridViewColumn column) {
			GridViewFooterStyle result = new GridViewFooterStyle();
			CopyCellAttributes(Styles.Footer, result);
			result.CopyFrom(column.FooterCellStyle);
			ApplyDisplayControlTextAlign(result, column);
			return result;
		}
		public override AppearanceStyle GetBatchEditCellStyle() {
			return MergeStyle<GridViewCellStyle>(GridStyles.BatchEditCellStyleName, Styles.Cell, Styles.BatchEditCell);
		}
		public override AppearanceStyle GetBatchEditModifiedCellStyle() {
			return MergeStyle<GridViewCellStyle>(GridStyles.BatchEditModifiedCellStyleName, Styles.Cell, Styles.BatchEditModifiedCell);
		}
		protected virtual AppearanceStyleBase GetContextMenuStyle() {
			return MergeStyle<AppearanceStyleBase>(GridViewStyles.ContextMenuStyleName);
		}
		public GridViewContextMenuStyle GetGroupPanelContextMenuStyle() {
			var result = new GridViewContextMenuStyle(Grid);
			result.Style.CopyFrom(GetContextMenuStyle());
			result.CopyFrom(StylesContextMenu.Common);
			result.CopyFrom(StylesContextMenu.GroupPanel);
			return result;
		}
		public GridViewContextMenuStyle GetColumnContextMenuStyle() {
			var result = new GridViewContextMenuStyle(Grid);
			result.Style.CopyFrom(GetContextMenuStyle());
			result.CopyFrom(StylesContextMenu.Common);
			result.CopyFrom(StylesContextMenu.Column);
			return result;
		}
		public GridViewContextMenuStyle GetRowContextMenuStyle() {
			var result = new GridViewContextMenuStyle(Grid);
			result.Style.CopyFrom(GetContextMenuStyle());
			result.CopyFrom(StylesContextMenu.Common);
			result.CopyFrom(StylesContextMenu.Row);
			return result;
		}
		public GridViewContextMenuStyle GetFooterContextMenuStyle() {
			var result = new GridViewContextMenuStyle(Grid);
			result.Style.CopyFrom(GetContextMenuStyle());
			result.CopyFrom(StylesContextMenu.Common);
			result.CopyFrom(StylesContextMenu.Footer);
			return result;
		}
		class StyleCacheKeys {
			public static readonly object
				GroupRow = new object(),
				DataRow = new object(),
				DataRowAlt = new object(),
				DetailRow = new object(),
				DetailCell = new object(),
				DetailButton = new object(),
				PreviewRow = new object(),
				DataCell = new object(),
				GroupFooterRow = new object(),
				GroupFooterCell = new object(),
				CommandCell = new object(),
				CommandCellItem = new object(),
				FixedColumnCell = new object();
		}
		public GridViewGroupRowStyle GetGroupRowStyle() {
			return Grid.GetCachedStyle<GridViewGroupRowStyle>(
				delegate() { return MergeStyle<GridViewGroupRowStyle>(GridViewStyles.GroupRowStyleName, Styles.GroupRow); },
				StyleCacheKeys.GroupRow
			);
		}
		public GridViewDataRowStyle GetDataRowStyle(int dataRowIndex) {
			bool alt = UseAlternatingRowStyle(dataRowIndex);
			return Grid.GetCachedStyle<GridViewDataRowStyle>(
				delegate() {
					GridViewDataRowStyle result = MergeStyle<GridViewDataRowStyle>(GridViewStyles.DataRowStyleName, Styles.Row);
					if(alt) {
						result.CopyFrom(Styles.CreateStyleCopyByName<GridViewDataRowStyle>(GridViewStyles.DataRowAltStyleName));
						result.CopyFrom(Styles.AlternatingRow);
					}
					return result;
				},
				alt ? StyleCacheKeys.DataRowAlt : StyleCacheKeys.DataRow
			);
		}
		bool UseAlternatingRowStyle(int dataRowIndex) {
			if(dataRowIndex % 2 == 0)
				return false;
			if(Styles.AlternatingRow.Enabled == DefaultBoolean.False)
				return false;
			if(Styles.AlternatingRow.Enabled == DefaultBoolean.True)
				return true;
			return !Styles.AlternatingRow.IsEmpty;
		}
		public override AppearanceStyleBase GetDataCellStyle(GridViewColumn column) {
			return Grid.GetCachedStyle<GridViewCellStyle>(
				delegate() {
					GridViewCellStyle result = new GridViewCellStyle();
					result.CopyFrom(Styles.Cell);
					result.CopyFrom(column.CellStyle);
					if(ColumnHelper.FixedColumns.Contains(column))
						result.CopyFrom(GetFixedColumnStyle());
					ApplyDisplayControlTextAlign(result, column);
					return result;
				},
				StyleCacheKeys.DataCell, column
			);
		}
		public GridViewStyleBase GetFixedColumnStyle() {
			return Grid.GetCachedStyle<GridViewStyleBase>(
				() => MergeStyle<GridViewStyleBase>(GridViewStyles.FixedColumnStyleName, Styles.FixedColumn),
				StyleCacheKeys.FixedColumnCell
			);
		}
		public GridViewDataRowStyle GetDetailRowStyle() {
			return Grid.GetCachedStyle<GridViewDataRowStyle>(
				delegate() { return MergeStyle<GridViewDataRowStyle>(GridViewStyles.DetailRowStyleName, Styles.DetailRow); },
				StyleCacheKeys.DetailRow
			);
		}
		public GridViewCellStyle GetDetailCellStyle() {
			return Grid.GetCachedStyle<GridViewCellStyle>(
				delegate() { return MergeStyle<GridViewCellStyle>(GridViewStyles.DetailCellStyleName, Styles.DetailCell); },
				StyleCacheKeys.DetailCell
			);
		}
		public GridViewCellStyle GetDetailButtonStyle() {
			return Grid.GetCachedStyle<GridViewCellStyle>(
				delegate() { return MergeStyle<GridViewCellStyle>(GridViewStyles.DetailButtonStyleName, Styles.DetailButton); },
				StyleCacheKeys.DetailButton
			);
		}
		public GridViewDataRowStyle GetPreviewRowStyle() {
			return Grid.GetCachedStyle<GridViewDataRowStyle>(
				delegate() { return MergeStyle<GridViewDataRowStyle>(GridViewStyles.PreviewRowStyleName, Styles.PreviewRow); },
				StyleCacheKeys.PreviewRow
			);
		}
		public GridViewGroupFooterStyle GetGroupFooterStyle() {
			return Grid.GetCachedStyle<GridViewGroupFooterStyle>(
				delegate() { return MergeStyle<GridViewGroupFooterStyle>(GridViewStyles.GroupFooterStyleName, Styles.GroupFooter); },
				StyleCacheKeys.GroupFooterRow
			);
		}
		public GridViewGroupFooterStyle GetGroupFooterCellStyle(GridViewColumn column) {
			return Grid.GetCachedStyle<GridViewGroupFooterStyle>(
				delegate() {
					GridViewGroupFooterStyle result = new GridViewGroupFooterStyle();
					CopyCellAttributes(Styles.GroupFooter, result);
					result.CopyFrom(column.GroupFooterCellStyle);
					ApplyDisplayControlTextAlign(result, column);
					return result;
				},
				StyleCacheKeys.GroupFooterCell, column
			);
		}
		public GridViewCommandColumnStyle GetCommandColumnStyle(GridViewColumn column) {
			var extraStyles = new List<Style> { Styles.CommandColumn, column.CellStyle };
			if(ColumnHelper.FixedColumns.Contains(column))
				extraStyles.Add(GetFixedColumnStyle());
			return Grid.GetCachedStyle<GridViewCommandColumnStyle>(
				() => MergeStyle<GridViewCommandColumnStyle>(GridViewStyles.CommandColumnStyleName, extraStyles.ToArray()),
				StyleCacheKeys.CommandCell, column
			);
		}
		public GridViewCommandColumnStyle GetCommandColumnItemStyle(GridViewColumn column) {
			return Grid.GetCachedStyle<GridViewCommandColumnStyle>(
				delegate() { return MergeStyle<GridViewCommandColumnStyle>(GridViewStyles.CommandColumnItemStyleName, Styles.CommandColumnItem); },
				StyleCacheKeys.CommandCellItem, column
			);
		}
		public bool ShouldRemoveLeftBorder(int leafIndex) {
			if(IsRightToLeft)
				return leafIndex == ColumnHelper.Leafs.Count - 1;
			return leafIndex == 0 && Grid.GroupCount > 0;
		}
		public bool ShouldRemoveRightBorder(int leafIndex) {
			if(IsRightToLeft)
				return leafIndex == 0;
			return leafIndex == ColumnHelper.Leafs.Count - 1;
		}
		public override bool AddHeaderTemplateControl(IWebGridColumn column, Control templateContainer, GridHeaderLocation headerLocation) {
			var gridColumn = column as GridViewColumn;
			if(gridColumn == null) return false;
			ITemplate template = GetTemplate(Grid.Templates.Header, gridColumn.HeaderTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewHeaderTemplateContainer(gridColumn, headerLocation), HeaderTemplates);
			return true;
		}
		public bool AddHeaderCaptionTemplateControl(GridViewColumn column, Control templateContainer, GridHeaderLocation headerLocation) {
			ITemplate template = GetTemplate(Grid.Templates.HeaderCaption, column.HeaderCaptionTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewHeaderTemplateContainer(column, headerLocation), HeaderTemplates);
			return true;
		}
		public bool AddFilterCellTemplateControl(GridViewColumn column, Control templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.FilterCell, column.FilterTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewFilterCellTemplateContainer(column), FilterTemplates);
			return true;
		}
		public bool AddFilterRowTemplateControl(TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.FilterRow;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewFilterRowTemplateContainer(Grid), FilterRowTemplates);
			return true;
		}
		public bool AddDataItemTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.DataItem, column.DataItemTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, CreateDataItemTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex, column), RowCellTemplates);
			return true;
		}
		protected virtual GridViewDataItemTemplateContainer CreateDataItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column) {
			return new GridViewDataItemTemplateContainer(Grid, row, visibleIndex, column);
		}
		public bool AddEditItemTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = column.EditItemTemplate;
			if(template == null || IsGridExported) return false;
			GridViewEditItemTemplateContainer editItemTemplateContainer = null;
			if(AllowBatchEditing)
				editItemTemplateContainer = new GridViewBatchEditItemTemplateContainer(Grid, column);
			else
				editItemTemplateContainer = new GridViewEditItemTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex, column);
			AddTemplateToControl(templateContainer, template, editItemTemplateContainer, EditRowCellTemplates);
			return true;
		}
		public bool AddGroupRowContentTemplateControl(int visibleIndex, GridViewDataColumn column, Control templateContainer) {
			ITemplate template = Grid.Templates.GroupRowContent;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewGroupRowTemplateContainer(Grid, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupRowTemplates);
			return true;
		}
		public bool AddGroupRowTemplateControl(int visibleIndex, GridViewDataColumn column, TableRow row, int spanCount) {
			ITemplate template = GetTemplate(Grid.Templates.GroupRow, column.GroupRowTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewGroupRowTemplateContainer(Grid, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupRowTemplates);
			return true;
		}
		public bool AddPreviewRowTemplateControl(int visibleIndex, GridViewTablePreviewRow row, int spanCount) {
			ITemplate template = Grid.Templates.PreviewRow;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewPreviewRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), PreviewRowTemplates);
			return true;
		}
		public bool AddDetailRowTemplateControl(int visibleIndex, GridViewTableDetailRow row, int spanCount) {
			ITemplate template = Grid.Templates.DetailRow;
			if(template == null) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewDetailRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), DetailRowTemplates);
			return true;
		}
		public Control AddAdaptiveDetailRowControl(GridViewTableAdaptiveDetailRow row, int spanCount) {
			return CreateTemplateCell(row, spanCount);
		}
		protected TableCell CreateTemplateCell(TableRow row, int spanCount) {
			TableCell cell = new InternalTableCell();
			cell.ColumnSpan = spanCount;
			row.Cells.Add(cell);
			AppendGridCssClassName(cell);
			return cell;
		}
		public bool AddDataRowTemplateControl(int visibleIndex, TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.DataRow;
			if(template == null || IsGridExported) return false;
			var templateContainer = new GridTableCell(this, false, true);
			templateContainer.ColumnSpan = spanCount;
			row.Cells.Add(templateContainer);
			GridViewDataRowTemplateContainer cont = new GridViewDataRowTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex);
			AddTemplateToControl(templateContainer, template, cont, DataRowTemplates);
			return true;
		}
		public bool AddFooterRowTemplateControl(TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.FooterRow;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewFooterRowTemplateContainer(Grid), FooterRowTemplates);
			return true;
		}
		public bool AddGroupFooterRowTemplateControl(TableRow row, GridViewDataColumn column, int spanCount, int visibleIndex) {
			ITemplate template = Grid.Templates.GroupFooterRow;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewGroupFooterRowTemplateContainer(Grid, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupFooterRowTemplates);
			return true;
		}
		public bool AddFooterCellTemplateControl(GridViewColumn column, Control templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.FooterCell, column.FooterTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewFooterCellTemplateContainer(column), FooterCellTemplates);
			return true;
		}
		public bool AddGroupFooterCellTemplateControl(Control templateContainer, GridViewDataColumn groupedColumn, GridViewColumn column, int visibleIndex) {
			ITemplate template = GetTemplate(Grid.Templates.GroupFooterCell, column.GroupFooterTemplate);
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewGroupFooterCellTemplateContainer(Grid, groupedColumn, column, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), GroupFooterCellTemplates);
			return true;
		}
		public bool AddEmptyDataRowTemplateControl(TableRow row, int spanCount) {
			ITemplate template = Grid.Templates.EmptyDataRow;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(CreateTemplateCell(row, spanCount), template, new GridViewEmptyDataRowTemplateContainer(Grid), EmptyDataRowTemplates);
			return true;
		}
		public override bool AddTitleTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.TitlePanel;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewTitleTemplateContainer(Grid), TitleTemplates);
			return true;
		}
		public override bool AddStatusBarTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.StatusBar;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewStatusBarTemplateContainer(Grid), StatusBarTemplates);
			return true;
		}
		public override bool AddPagerBarTemplateControl(WebControl templateContainer, GridViewPagerBarPosition position, string pagerId) {
			ITemplate template = Grid.Templates.PagerBar;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewPagerBarTemplateContainer(Grid, position, pagerId), PagerBarTemplates);
			return true;
		}
		public override bool AddEditFormTemplateControl(WebControl templateContainer, int visibleIndex) {
			ITemplate template = Grid.Templates.EditForm;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new GridViewEditFormTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex), EditFormTemplates);
			return true;
		}
		public bool AddEditFormLayoutItemTemplateControl(ColumnLayoutItem layoutItem, int visibleIndex, GridViewDataColumn column) {
			ITemplate template = layoutItem.Template;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(layoutItem.NestedControlContainer, template,
				new GridViewEditFormLayoutItemTemplateContainer(Grid, DataProxy.GetRowForTemplate(visibleIndex), visibleIndex, column, layoutItem),
				FormLayoutItemTemplates);
			EnsureNestedControlContainerRecursive(layoutItem.NestedControlContainer);
			return true;
		}
		public bool HasTemplate(params ITemplate[] templates) {
			foreach(ITemplate template in templates)
				if(template != null) return true;
			return false;
		}
		protected internal void EnsureNestedControlContainerRecursive(LayoutItemNestedControlContainer controlContainer) { 
			if(MvcUtils.RenderMode == MvcRenderMode.None)
				return;
			RenderUtils.EnsureChildControlsRecursive(controlContainer, true);
		}
	}
	public class GridViewClientStylesInfo : GridClientStylesInfo {
		public GridViewClientStylesInfo(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected new GridViewRenderHelper RenderHelper { get { return (GridViewRenderHelper)base.RenderHelper; } }
		protected override AppearanceStyle GetSelectedItemStyle() {
			return RenderHelper.GetSelectedRowStyle();
		}
		protected override AppearanceStyle GetFocusedItemStyle() {
			return RenderHelper.GetFocusedRowStyle();
		}
		protected override void PopulateStyleInfo(Dictionary<string, object> styleInfo) {
			base.PopulateStyleInfo(styleInfo);
			styleInfo["fgi"] = GetStyleInfo(RenderHelper.GetFocusedGroupRowStyle());
		}
		protected override WebControl CreateErrorItemControl() { return new GridViewTableEditingErrorRow(RenderHelper, true); }
	}
}
