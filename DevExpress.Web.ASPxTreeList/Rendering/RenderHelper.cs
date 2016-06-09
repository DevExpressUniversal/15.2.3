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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public class TreeListRenderHelper {
		#region Style-cache keys
		static readonly object
			StyleKeyDefault = new object(),
			StyleKeyHeader = new object(),
			StyleKeyIndent = new object(),
			StyleKeyIndentWButton = new object(),
			StyleKeyIndentFirst = new object(),
			StyleKeyIndentMiddle = new object(),
			StyleKeyIndentLast = new object(),
			StyleKeyIndentRoot = new object(),
			StyleKeySelectionCell = new object(),
			StyleKeyNode = new object(),
			StyleKeyNodeAlt = new object(),
			StyleKeyNodeSelected = new object(),
			StyleKeyNodeFocused = new object(),
			StyleKeyPreview = new object(),
			StyleKeyGroupFooter = new object(),
			StyleKeyFooter = new object(),
			StyleKeyPagerTopPanel = new object(),
			StyleKeyPagerBottomPanel = new object(),
			StyleKeyDataCell = new object(),
			StyleKeyGroupFooterCell = new object(),
			StyleKeyFooterCell = new object(),
			StyleKeyPreviewCell = new object(),
			StyleKeyError = new object(),
			StyleKeyInlineEditCell = new object(),
			StyleKeyEditFormCell = new object(),
			StyleKeyEditFormCaption = new object(),
			StyleKeyCommandCell = new object(),
			StyleKeyCommandButton = new object();
		#endregion		
		internal const bool UseStateCompression = true;
		internal const char SeparatorToken = ' ';		
		internal const string EditCaptionFormat = "{0}:";
		internal static string
			RootNodeKey = String.Empty,
			NewNodeKey = null;
		internal const int PopupEditFormRowIndex = Int32.MinValue + 1;
		internal const string CancelBubbleJs = "event.cancelBubble=true";
		static int indentDefaultWidth = 23;
		#region ID consts
		public const string PopupEditFormID = "PEF";
		internal const string
			FocusedKeyFieldName = "FKey",
			SelectionFieldName = "Sel",
			SelectAllMarkFieldName = "SAM",
			EditorValuesFieldName = "EV",
			ScrollStateInputName = "SSKey",
			ResizingStateInputName = "RSI",
			UpdatableCellID = "U",
			DataTableID = "D",
			HeaderTableID = "H",
			FooterTableID = "F",
			PagerPanelID = "PP",
			StyleTableID = "ST",
			SelectAllCheckID = "SelAll",
			DragAndDropArrowDownID = "DAD",
			DragAndDropArrowUpID = "DAU",
			DragAndDropHideID = "DH",
			DragAndDropNodeID = "DN",
			ScrollableControlID = "SC",
			HiddenEmptyRowID = "HER",
			RowIDSuffix = "R-",
			DragAndDropTargetMark = "DX-DnD-",
			HeaderIDSuffix = "H-",
			CustomizationWindowSuffix = "CW",
			HeaderSuffix = "HDR",
			EmptyHeaderSuffix = "EH",
			EditorIDSuffix = "DXEDITOR",
			ErrorCellSuffix = "Error",
			HeaderCaptionTemplatePrefix = "tHC",
			DataCellTemplatePrefix = "tDC",
			PreviewCellTemplatePrefix = "tP",
			GroupFooterCellTemplatePrefix = "tGF",
			FooterCellTemplatePrefix = "tF",
			EditCellTemplatePrefix = "tEC",
			EditFormTemplateID = "tEF",
			BorderClassNamePrefix = "dxtl__B",
			ExpandButtonClassName = "dxtl__Expand",
			CollapseButtonClassName = "dxtl__Collapse",
			SelectionCheckClassName = "dxtl__Sel",
			IndentCellSystemClassNameIE = "dxtl__IE",
			IndentCellSystemClassName = "dxtl__IM",
			CommandCellClassName = "dxtl__cc";
		#endregion
		ASPxTreeList treeList;
		TreeListRootRowInfo rootRowInfo;
		List<Control> editTemplateContainers;
		public TreeListRenderHelper(ASPxTreeList treeList) {
			this.treeList = treeList;
			this.rootRowInfo = new TreeListRootRowInfo(TreeDataHelper);
		}
		public static int IndentDefaultWidth { get { return indentDefaultWidth; } set { indentDefaultWidth = value; } }
		public ASPxTreeList TreeList { get { return treeList; } }
		public bool IsRightToLeft { get { return (TreeList as ISkinOwner).IsRightToLeft(); } }
		public ReadOnlyTreeListColumnCollection<TreeListDataColumn> AllDataColumns { get { return new ReadOnlyTreeListColumnCollection<TreeListDataColumn>(CreateAllDataColumnsList()); } }
		public TreeListTemplateHelper TemplateHelper { get { return TreeList.TemplateHelper; } }
		public TreeListDataHelper TreeDataHelper { get { return TreeList.TreeDataHelper; } }
		public List<TreeListRowInfo> Rows { get { return TreeDataHelper.Rows; } }
		public IList<TreeListColumn>  VisibleColumns { get { return TreeList.VisibleColumns; } }
		public TreeListRootRowInfo RootRowInfo { get { return rootRowInfo; } }
		public List<Control> EditTemplateContainers {
			get {
				if(editTemplateContainers == null)
					editTemplateContainers = new List<Control>();
				return editTemplateContainers;
			}
		}
		public int MaxVisibleIndentCount {
			get {
				int value = TreeDataHelper.MaxVisibleLevel;
				if(!IsRootIndentVisible) value--;
				return value;
			}
		}		
		public TreeListColumn LastVisibleColumn {
			get {
				if(VisibleColumns.Count < 1)
					return null;
				return VisibleColumns[VisibleColumns.Count - 1];
			}
		}
		public TreeListColumn FirstVisibleColumn {
			get {
				if(VisibleColumns.Count < 1)
					return null;
				return VisibleColumns[0];
			}
		}
		public bool HasSingleCommandColumn() {
			int count = 0;
			foreach(TreeListColumn column in TreeList.VisibleColumns) {
				if(column is TreeListCommandColumn) count++;
				if(count > 1) return false;
			}
			return count == 1;
		}
		protected TreeListTemplates Templates { get { return TreeList.Templates; } }
		protected TreeListStyles Styles { get { return TreeList.Styles; } }
		protected List<TreeListDataColumn> CreateAllDataColumnsList() {
			return TreeList.Columns.OfType<TreeListDataColumn>().ToList();
		}
		public Control GetDataCellDisplayControl(TreeListRowInfo row, TreeListDataColumn column) {
			CreateDisplayControlArgs args = GetEditorDisplayControlArgs(row, column);
			return column.PropertiesEdit.CreateDisplayControl(args);
		}
		public string GetDataCellText(TreeListRowInfo row, TreeListDataColumn column, bool allowEmpty) {
			CreateDisplayControlArgs args = GetEditorDisplayControlArgs(row, column);
			string value = column.PropertiesEdit.GetDisplayText(args);
			return allowEmpty ? value : FilterCellText(value);
		}		
		public string GetPreviewText(TreeListRowInfo row, bool allowEmpty) {
			object value = null;
			string fieldName = TreeList.PreviewFieldName;
			if(!String.IsNullOrEmpty(fieldName))
				value = row.GetValue(fieldName);
			string str = value == null ? String.Empty : value.ToString();
			return allowEmpty ? str : FilterCellText(str);
		}
		public string GetFooterText(TreeListRowInfo row, TreeListColumn column) {
			return GetFooterText(row, column, "<br/>", false);
		}
		public string GetFooterText(TreeListRowInfo row, TreeListColumn column, string separator, bool allowEmpty) {
			StringBuilder builder = new StringBuilder();
			foreach(TreeListSummaryItem item in TreeList.Summary) {
				string name = item.ShowInColumn;				
				if(!TreeListUtils.IsSummaryVisible(column, item.ShowInColumn))
					continue;
				if(builder.Length > 0)
					builder.Append(separator);
				builder.AppendFormat(item.GetFormatString(), TreeDataHelper.GetSummaryValue(row.NodeKey, item));
			}
			return allowEmpty ? builder.ToString() : FilterCellText(builder.ToString());
		}
		public void SetDefaultDisplayControlAlign(TableCell cell, TreeListDataColumn column) {
			if(cell.HorizontalAlign != HorizontalAlign.NotSet && cell.HorizontalAlign != HorizontalAlign.Justify)
				return;
			cell.HorizontalAlign = GetColumnDefaultDisplayControlAlign(column);
		}
		public HorizontalAlign GetColumnDefaultDisplayControlAlign(TreeListDataColumn column) {
			HorizontalAlign align = HorizontalAlign.NotSet;
			Type type = ReflectionUtils.StripNullableType(TreeDataHelper.GetFieldType(column.FieldName));
			if(DataUtils.IsNumericType(type))
				align = HorizontalAlign.Right;
			else
				align = column.PropertiesEdit.GetDisplayControlDefaultAlign();
			return align;
		}
		public CreateDisplayControlArgs GetEditorDisplayControlArgs(TreeListRowInfo row, TreeListDataColumn column) {
			return new CreateDisplayControlArgs(row.GetValue(column.FieldName), TreeDataHelper.GetFieldType(column.FieldName), 
				null, row, TreeList.ImagesEditors, TreeList.StylesEditors, TreeList, TreeList.EditorRegistrator, TreeList.DesignMode);
		}
		string FilterCellText(string text) {
			return String.IsNullOrEmpty(text) ? "&nbsp;" : text;
		}
		public string GetCommandButtonText(TreeListCommandColumn column, TreeListCommandColumnButtonType type) {
			string text = String.Empty;
			if(column != null) 
				text = GetCommandColumnButton(column, type).Text;
			if(!String.IsNullOrEmpty(text))
				return text;
			switch(type) {
				case TreeListCommandColumnButtonType.Edit:
					return TreeList.SettingsText.CommandEdit;
				case TreeListCommandColumnButtonType.New:
					return TreeList.SettingsText.CommandNew;
				case TreeListCommandColumnButtonType.Delete:
					return TreeList.SettingsText.CommandDelete;
				case TreeListCommandColumnButtonType.Update:
					return TreeList.SettingsText.CommandUpdate;
				case TreeListCommandColumnButtonType.Cancel:
					return TreeList.SettingsText.CommandCancel;
			}			
			throw new NotImplementedException();
		}
		public TreeListHeaderTemplateContainer CreateHeaderTemplateContainer(Control parent, TreeListColumn column) {
			ITemplate template = column.HeaderCaptionTemplate;
			if(template == null)
				template = Templates.HeaderCaption;
			if(template == null)
				return null;
			TreeListHeaderTemplateContainer container = new TreeListHeaderTemplateContainer(TreeList, column);			
			container.AddToHierarchy(parent, TemplateHelper.GetHeaderCaptionContainerId(column));
			TemplateHelper.HeaderCaptionContainers.Add(new TreeListTemplateRegistration(container, column));
			template.InstantiateIn(container);			
			return container;
		}
		public TreeListDataCellTemplateContainer CreateDataCellTemplateContainer(Control parent, TreeListRowInfo row, TreeListDataColumn column) {
			ITemplate template = column.DataCellTemplate;
			if(template == null)
				template = Templates.DataCell;
			if(template == null)
				return null;
			TreeListDataCellTemplateContainer container = new TreeListDataCellTemplateContainer(TreeList, row, column);			
			container.AddToHierarchy(parent, TemplateHelper.GetDataCellContainerId(column, row.NodeKey));
			TemplateHelper.DataCellContainers.Add(new TreeListTemplateRegistration(container, column, row.NodeKey));
			template.InstantiateIn(container);			
			return container;
		}
		public TreeListPreviewTemplateContainer CreatePreviewTemplateContainer(Control parent, TreeListRowInfo row) {
			ITemplate template = Templates.Preview;
			if(template == null)
				return null;
			TreeListPreviewTemplateContainer container = new TreeListPreviewTemplateContainer(TreeList, row);
			container.AddToHierarchy(parent, TemplateHelper.GetPreviewContainerId(row.NodeKey));
			TemplateHelper.PreviewContainers.Add(new TreeListTemplateRegistration(container, row.NodeKey));
			template.InstantiateIn(container);			
			return container;
		}
		public TreeListFooterCellTemplateContainer CreateFooterTemplateContainer(Control parent, TreeListRowInfo row, TreeListColumn column, bool total) {
			ITemplate template = null;
			if(column != null)
				template = total ? column.FooterCellTemplate : column.GroupFooterCellTemplate;
			if(template == null)
				template = total ? Templates.FooterCell : Templates.GroupFooterCell;
			if(template == null)
				return null;
			TreeListFooterCellTemplateContainer container = new TreeListFooterCellTemplateContainer(TreeList, row, column);
			if(total) {
				container.AddToHierarchy(parent, TemplateHelper.GetFooterContainerId(column));
				TemplateHelper.FooterContainers.Add(new TreeListTemplateRegistration(container, column));
			} else {
				container.AddToHierarchy(parent, TemplateHelper.GetGroupFooterContainerId(column, row.NodeKey));
				TemplateHelper.GroupFooterContainers.Add(new TreeListTemplateRegistration(container, column, row.NodeKey));
			}
			template.InstantiateIn(container);			
			return container;
		}
		public TreeListDataCellTemplateContainer CreateEditCellTemplateContainer(Control parent, TreeListRowInfo row, TreeListDataColumn column) {
			ITemplate template = column.EditCellTemplate;
			if(template == null)
				return null;
			TreeListEditCellTemplateContainer container = new TreeListEditCellTemplateContainer(TreeList, row, column);			
			container.AddToHierarchy(parent, TemplateHelper.GetEditCellContainerId(column, EditingNodeKey));
			TemplateHelper.EditCellContainers.Add(new TreeListTemplateRegistration(container, column));
			template.InstantiateIn(container);
			EditTemplateContainers.Add(container);
			return container;
		}
		public TreeListEditFormTemplateContainer CreateEditFormTemplateContainer(Control parent, int rowIndex) {
			ITemplate template = Templates.EditForm;
			if(template == null)
				return null;
			TreeListRowInfo row = GetRowByIndex(rowIndex);
			if(rowIndex == PopupEditFormRowIndex)
				row = TreeDataHelper.GetPopupEditFormRowInfo();
			TreeListEditFormTemplateContainer container = new TreeListEditFormTemplateContainer(TreeList, row, rowIndex);
			container.AddToHierarchy(parent, TemplateHelper.GetEditFormContainerId(EditingNodeKey));
			TemplateHelper.EditFormContainers.Add(new TreeListTemplateRegistration(container));
			template.InstantiateIn(container);
			EditTemplateContainers.Add(container);
			return container;
		}
		public void CreateHeaderCaption(TableCell cell,  TreeListColumn column) {
			Control control = CreateHeaderTemplateContainer(cell, column);
			if(control != null)
				control.DataBind();
			else {
				if(!TryAddAccessibleHeaderLink(cell, column))
					cell.Text = FilterCellText(column.GetCaption());
			}
		}
		bool TryAddAccessibleHeaderLink(TableCell cell, TreeListColumn column) {
			if(!TreeList.IsAccessibilityCompliantRender(true)) return false;
			TreeListDataColumn dataColumn = column as TreeListDataColumn;
			if(dataColumn == null || !IsColumnSortable(dataColumn)) return false;
			string text = column.GetCaption();
			if(string.IsNullOrEmpty(text)) return false;
			HyperLink link = RenderUtils.CreateHyperLink();
			link.Text = text;
			link.NavigateUrl = string.Format("javascript:{0}", GetAccessibleSortOnClick(column));
			RenderUtils.SetStringAttribute(link, "onmousedown", TreeListRenderHelper.CancelBubbleJs);
			if(RenderUtils.IsHtml5Mode(link) && TreeList.IsAccessibilityCompliantRender()) {
				RenderUtils.SetStringAttribute(link, "role", "button");
				RenderUtils.SetStringAttribute(link, "aria-label", string.Format(AccessibilityUtils.TreeListHeaderLinkFormatString, text,
				   AccessibilityUtils.GetStringSortOrder((column as TreeListDataColumn).SortOrder)));
			}
			cell.Controls.Add(link);
			return true;
		}
		public string GetHeaderCellId(int columnIndex) {
			StringBuilder builder = new StringBuilder();
			if(IsColumnDragDropEnabled)
				builder.Append(DragAndDropTargetMark);
			builder.AppendFormat("{0}{1}", HeaderIDSuffix, columnIndex);
			return builder.ToString();
		}
		public string GetDataRowId(int index) {
			string key = GetRowByIndex(index).NodeKey;
			if(key == null) return null;
			return RowIDSuffix + TreeListUtils.EscapeNodeKey(key);
		}
		public string GetEditorId(TreeListDataColumn column) {
			return TreeListRenderHelper.EditorIDSuffix + column.Index.ToString();
		}
		public string GetDataTableID(TreeListDataTableRenderPart renderPart) {
			switch(renderPart) { 
				case TreeListDataTableRenderPart.Header:
					return HeaderTableID;
				case TreeListDataTableRenderPart.Footer:
					return FooterTableID;
				case TreeListDataTableRenderPart.Content:
				case TreeListDataTableRenderPart.All:
					return DataTableID;
			}
			throw new ArgumentException();
		}
		public int HeaderFirstColumnSpan { get { return TreeDataHelper.MaxVisibleLevel; } }
		public int GetFirstDataColumnSpan(int indentCount) {
			return MaxVisibleIndentCount - indentCount + 1;
		}
		public int GetWideDataCellColumnSpan(int indentCount) {
			int value = MaxVisibleIndentCount - indentCount + VisibleColumns.Count;
			if(VisibleColumns.Count < 1)
				value++;
			if(IsSelectionEnabled)
				value++;
			return value;
		}
		public static int FilterTableSpanValue(int value) {
			return value > 1 ? value : 0;
		}
		public int GetTotalCellCount() {
			var value = GetWideDataCellColumnSpan(0);
			if(TreeDataHelper.MaxVisibleLevel == 0)
				value++;
			if(HasHorizontalScrollBar)
				value++;
			return value;
		}
		public ImageProperties GetNodeImage(TreeListRowInfo row) {
			if(HasNodeImage(row)) {
				if(TreeDataHelper.IsNodeExpanded(row.NodeKey))
					return GetImage(IsRightToLeft ? TreeListImages.ExpandedButtonRtlName : TreeListImages.ExpandedButtonName);
				return GetImage(IsRightToLeft ? TreeListImages.CollapsedButtonRtlName : TreeListImages.CollapsedButtonName);
			}
			return new ImageProperties();
		}
		public ImageProperties GetImage(string name) {
			return TreeList.Images.GetImageProperties(TreeList.Page, name);
		}
		public bool HasNodeImage(TreeListRowInfo row) {
			return row.HasButton;
		}
		public InternalCheckBoxImageProperties GetCheckBoxImage(CheckState checkState) {
			return TreeList.GetCheckBoxImage(checkState);
		}
		#region Styles
		public AppearanceStyleBase GetICBStyle() {
			return TreeList.GetICBStyle();
		}
		public bool TreeWidthSpecifiedInPixels { get { return !TreeList.Width.IsEmpty && TreeList.Width.Type != UnitType.Percentage; } }
		public bool IsAlternatingNodeStyleEnabled { get { return GetAlternatingNodeStyle().Enabled == DefaultBoolean.True; } }
		public AppearanceStyleBase GetTreeLineCellStyle(TreeListRowInfo row, TreeListRowIndentType indent, bool isLeaf) {
			bool hasImage = isLeaf && HasNodeImage(row);
			if(!TreeList.Settings.ShowTreeLines)
				indent = TreeListRowIndentType.None;
			return TreeList.InternalCreateStyle(
				delegate() {
					AppearanceStyleBase style = hasImage
						? Styles.MergeIndentWithButtonStyle()
						: Styles.MergeIndentStyle();
					switch(indent) {
						case TreeListRowIndentType.First:
							style.CopyFrom(Styles.MergeTreeLineFirstStyle());
							break;
						case TreeListRowIndentType.Middle:
							style.CopyFrom(Styles.MergeTreeLineMiddleStyle());
							break;
						case TreeListRowIndentType.Last:
							style.CopyFrom(Styles.MergeTreeLineLastStyle());
							break;
						case TreeListRowIndentType.Root:
							style.CopyFrom(Styles.MergeTreeLineRootStyle());
							break;
					}
					return style;
				},
				hasImage ? StyleKeyIndentWButton : StyleKeyIndent, GetIndentTypeStyleKey(indent));
		}
		public AppearanceStyleBase GetSelectionCellStyle() {			
			return TreeList.InternalCreateStyle(
				delegate() {
					AppearanceStyleBase style = Styles.MergeSelectionCellStyle();
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, StyleKeySelectionCell);
		}
		public AppearanceStyleBase GetNodeStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeNodeStyle, StyleKeyNode);			
		}
		public TreeListAlternatingNodeStyle GetAlternatingNodeStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeAlternatingNodeStyle, StyleKeyNodeAlt) as TreeListAlternatingNodeStyle;
		}
		public AppearanceStyleBase GetSelectedNodeStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeSelectedNodeStyle, StyleKeyNodeSelected);
		}
		public AppearanceStyleBase GetFocusedNodeStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeFocusedNodeStyle, StyleKeyNodeFocused);			
		}
		public AppearanceStyleBase GetInlineEditNodeStyle() {
			return Styles.MergeInlineEditNodeStyle();
		}
		public AppearanceStyleBase GetEditFormDisplayNodeStyle() {
			return Styles.MergeEditFormDisplayNodeStyle();
		}
		public AppearanceStyleBase GetPreviewStyle() {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.MergePreviewStyle());
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, StyleKeyPreview);
		}
		public TreeListHeaderStyle GetHeaderStyle(TreeListColumn column) {
			return (TreeListHeaderStyle)TreeList.InternalCreateStyle(
				delegate() {					
					TreeListHeaderStyle style = new TreeListHeaderStyle();
					style.CopyFrom(Styles.MergeHeaderStyle());
					if(column != null)
						style.CopyFrom(column.HeaderStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyHeader);
		}				
		public AppearanceStyleBase GetGroupFooterStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeGroupFooterStyle, StyleKeyGroupFooter);			
		}
		public AppearanceStyleBase GetFooterStyle() {
			return TreeList.InternalCreateStyle(Styles.MergeFooterStyle, StyleKeyFooter);			
		}
		public TreeListPagerPanelStyle GetPagerTopPanelStyle() {
			return TreeList.InternalCreateStyle(Styles.MergePagerTopPanelStyle, StyleKeyPagerTopPanel) as TreeListPagerPanelStyle;
		}
		public TreeListPagerPanelStyle GetPagerBottomPanelStyle() {
			return TreeList.InternalCreateStyle(Styles.MergePagerBottomPanelStyle, StyleKeyPagerBottomPanel) as TreeListPagerPanelStyle;
		}
		public TreeListPopupEditFormStyle GetPopupEditFormStyle() { 
			return Styles.MergePopupEditFormStyle(); 
		}
		public AppearanceStyle GetRootControlStyle() {
			return TreeList.GetControlStyle() as AppearanceStyle;
		}		
		public AppearanceStyleBase GetErrorStyle() {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.MergeErrorStyle());
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, StyleKeyError);			
		}
		public AppearanceStyleBase GetEditFormEditCellStyle(TreeListDataColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.MergeEditFormEditCellStyle());
					if(column != null)
						style.CopyFrom(column.EditCellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyEditFormCell);
		}
		public AppearanceStyleBase GetEditFormCaptionStyle(TreeListDataColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.MergeEditFormCaptionStyle());
					if(column != null)
						style.CopyFrom(column.EditFormCaptionStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyEditFormCaption);
		}		
		public AppearanceStyleBase GetEditFormStyle() {
			return Styles.MergeEditFormStyle();
		}
		public TreeListCommandCellStyle GetCommandCellStyle(TreeListCommandColumn column) {
			return (TreeListCommandCellStyle)TreeList.InternalCreateStyle(
				delegate() {
					TreeListCommandCellStyle style = new TreeListCommandCellStyle();
					style.CopyFrom(Styles.MergeCommandCellStyle());
					if(column != null)
						style.CopyFrom(column.CellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyCommandCell);
		}
		public AppearanceStyleBase GetCommandButtonStyle(ButtonControlStyle buttonStyle) {
			return TreeList.InternalCreateStyle(
				delegate() {
					AppearanceStyle style = new AppearanceStyle();
					style.CopyFrom(Styles.CommandButton);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					if(buttonStyle != null)
						style.CopyFrom(buttonStyle);
					return style;
				}, StyleKeyCommandButton);
		}
		public AppearanceStyleBase GetCellStyle(TreeListColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.Font.CopyFrom(GetRootControlStyle().Font);
					style.Font.CopyFrom(GetNodeStyle().Font);
					style.CopyFrom(Styles.Cell);
					if(column != null)
						style.CopyFrom(column.CellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyDataCell);
		}
		public AppearanceStyleBase GetGroupFooterCellStyle(TreeListColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.GroupFooterCell);
					if(column != null)
						style.CopyFrom(column.GroupFooterCellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyGroupFooterCell);
		}
		public AppearanceStyleBase GetFooterCellStyle(TreeListColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.FooterCell);
					if(column != null)
						style.CopyFrom(column.FooterCellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyFooterCell);
		}
		public AppearanceStyleBase GetInlineEditCellStyle(TreeListDataColumn column) {
			return TreeList.InternalCreateStyle(
				delegate() {
					TreeListCellStyle style = new TreeListCellStyle();
					style.CopyFrom(Styles.InlineEditCell);
					if(column != null)
						style.CopyFrom(column.EditCellStyle);
					if(!IsEnabled)
						style.CopyFrom(TreeList.DisabledStyle);
					return style;
				}, GetColumnStyleKey(column), StyleKeyInlineEditCell);
		}
		public Unit GetHeaderSortImageSpacing(TreeListColumn column) {
			Unit value = (GetHeaderStyle(column) as TreeListHeaderStyle).SortImageSpacing;
			return value.IsEmpty ? 8 : value;
		}
		public static void SetZeroCellSpacing(Table table) {
			if(!RenderUtils.IsHtml5Mode(table)) {
				table.CellSpacing = -1;
				RenderUtils.SetStringAttribute(table, "cellspacing", "0");
			}
			else {
				table.CellSpacing = 0;
				table.Style["border-collapse"] = "separate";
			}
		}
		object GetIndentTypeStyleKey(TreeListRowIndentType type) {
			switch(type) {
				case TreeListRowIndentType.None: return StyleKeyDefault;
				case TreeListRowIndentType.Root: return StyleKeyIndentRoot;
				case TreeListRowIndentType.Middle: return StyleKeyIndentMiddle;
				case TreeListRowIndentType.Last: return StyleKeyIndentLast;
				case TreeListRowIndentType.First: return StyleKeyIndentFirst;
				default:
					throw new NotSupportedException();
			}
		}
		object GetColumnStyleKey(TreeListColumn column) {
			return column == null ? StyleKeyDefault : column;
		}
		#endregion
		public string GetPagerOnClick(string arg) {
			if(!IsEnabled) 
				return String.Empty;
			return String.Format("ASPx.TLPager('{0}',{1})", TreeList.ClientID, HtmlConvertor.ToScript(arg));
		}
		public string GetPagerOnPageSizeChange() {
			if(!IsEnabled)
				return String.Empty;
			return String.Format("function(s, e) {{ ASPx.TLPager('{0}',e.value) }}", TreeList.ClientID);
		}
		public string GetDataTableOnMouseDown() {
			if(!IsEnabled)
				return String.Empty;
			return String.Format("ASPx.TLMouseDown('{0}',event)", TreeList.ClientID);
		}
		public string GetDataTableOnClick() {
			if(!IsEnabled) 
				return String.Empty;
			return String.Format("ASPx.TLClick('{0}',event)", TreeList.ClientID);
		}
		public string GetDataTableOnDblClick() {
			if(!IsEnabled) 
				return String.Empty;
			return String.Format("ASPx.TLDblClick('{0}',event)", TreeList.ClientID);
		}
		public string GetHeaderOnMouseDown() {
			if(!IsEnabled) 
				return String.Empty;
			return String.Format("ASPx.TLHeaderDown('{0}',this,event)", TreeList.ClientID);
		}
		public string GetHeaderOnClick() {
			if(!IsEnabled) 
				return String.Empty;
			return String.Format("aspxTLHeaderClick('{0}',this,event)", TreeList.ClientID);
		}
		public string GetCustomizationWindowOnCloseUp() {
			return String.Format("function() {{ ASPx.TLCWCloseUp('{0}'); }}", TreeList.ClientID);
		}
		public string GetCommandButtonOnClick(TreeListCommandColumnButtonType type, string key) {
			if(!IsEnabled)
				return String.Empty;
			key = HtmlConvertor.ToScript(key);
			switch(type) {
				case TreeListCommandColumnButtonType.Edit:
					return String.Format("ASPx.TLStartEdit('{0}',{1})", TreeList.ClientID, key);
				case TreeListCommandColumnButtonType.New:
					return String.Format("ASPx.TLStartEditNewNode('{0}',{1})", TreeList.ClientID, key);
				case TreeListCommandColumnButtonType.Delete:
					return String.Format("ASPx.TLDeleteNode('{0}',{1})", TreeList.ClientID, key);
				case TreeListCommandColumnButtonType.Update:
					return String.Format("ASPx.TLUpdateEdit('{0}')", TreeList.ClientID);
				case TreeListCommandColumnButtonType.Cancel:
					return String.Format("ASPx.TLCancelEdit('{0}')", TreeList.ClientID);
				default:
					throw new NotImplementedException();
			}
		}
		public object[] GetCommandButtonClickArguments(TreeListCommandColumnButtonType type, string key) {
			if (!IsEnabled) return null;
			switch (type) {
				case TreeListCommandColumnButtonType.Edit:
				case TreeListCommandColumnButtonType.New:
				case TreeListCommandColumnButtonType.Delete:
					return new object[] { key };
				case TreeListCommandColumnButtonType.Update:
				case TreeListCommandColumnButtonType.Cancel:
					return null;
				default:
					throw new NotImplementedException();
			}
		}
		public string GetCustomButtonOnClick(string key, int index, string id) {
			return String.Format("ASPx.TLCustomButton('{0}',{1},{2},{3})", TreeList.ClientID, HtmlConvertor.ToScript(key), index, HtmlConvertor.ToScript(id));
		}
		public object[] GetCustomButtonClickArguments(string key, int index, string id) {
			return new object[] { key, index, id };
		}
		public string GetOnContextMenu(string objectType, object objectKey) {
			if(!IsEnabled)
				return String.Empty;
			return String.Format("return ASPx.TLMenu('{0}','{1}',{2},event)", TreeList.ClientID, objectType, HtmlConvertor.ToJSON(objectKey));
		}
		public string GetAccessibleSortOnClick(TreeListColumn column) {
			if(!IsEnabled) return String.Empty;
			return String.Format("ASPx.TLSort('{0}',{1})", TreeList.ClientID, column.Index);
		}
		public string GetToggleExpansionUrl(TreeListRowInfo row) {
			return String.Format("javascript:ASPx.TL{0}('{1}',{2})", row.Expanded ? "Collapse" : "Expand", TreeList.ClientID, HtmlConvertor.ToScript(row.NodeKey));
		}
		public virtual string GetFullRenderResult() {
			Control container = TreeList.GetUpdateableContainer();
			if(container == null)
				return String.Empty;
			return GetCompactRenderResult(container);
		}
		public virtual string GetPartialRenderResult(string nodeKey) {
			List<TableRow> rows = GetRowsForPartialUpdateNodeRender(nodeKey);
			if(rows != null) {
				StringBuilder builder = new StringBuilder();
				foreach(TableRow row in rows) {
					builder.Append(GetCompactRenderResult(row));
				}
				return builder.ToString();
			}
			return String.Empty;
		}
		public List<TableRow> GetRowsForPartialUpdateNodeRender(string nodeKey) {
			if(TreeList.CommandToExecute.IsPartialUpdateNodeSingle)
				return GetRowsForSingleNodeRender(nodeKey);
			Table table = TreeList.GetDataTable();
			return table == null ? null : TreeListUtils.GetSubtreeHtmlRows(table, nodeKey);
		}
		List<TableRow> GetRowsForSingleNodeRender(string nodeKey) {
			List<TableRow> rows = null;
			Table table = TreeList.GetDataTable();
			if(table != null) {
				int count = 1 + TreeListBuilderHelper.GetAuxRowCount(this, nodeKey);
				int renderedCount = 0;
				bool inside = false;
				foreach(TableRow row in table.Rows) {
					TreeListDataRowBase dataRow = row as TreeListDataRowBase;
					if(dataRow == null)
						continue;
					if(!inside && dataRow.RowInfo.NodeKey == nodeKey) {
						inside = true;
						rows = new List<TableRow>();
					}
					if(inside) {
						rows.Add(dataRow);
						renderedCount++;
						if(renderedCount == count)
							break;
					}
				}
			}
			return rows;
		}
		string GetCompactRenderResult(Control control) {
			using(StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture)) {
				using(HtmlTextWriter htmlWriter = new HtmlTextWriter(writer, String.Empty)) {
					htmlWriter.NewLine = String.Empty;
					control.RenderControl(htmlWriter);
				}
				return writer.ToString();
			}
		}
		public bool IsTopPagerVisible { get { return ArePagersVisible() && TreeList.SettingsPager.Position != PagerPosition.Bottom; } }
		public bool IsBottomPagerVisible { get { return ArePagersVisible() && treeList.SettingsPager.Position != PagerPosition.Top; } }
		bool ArePagersVisible() {
			return TreeList.SettingsPager.Mode == TreeListPagerMode.ShowPager
				&& TreeList.SettingsPager.Visible
				&& (TreeList.DesignMode || TreeList.SettingsPager.AlwaysShowPager || TreeDataHelper.PageCount > 1
					|| TreeDataHelper.IsPageSizeItemVisible());
		}
		public bool IsSelectionEnabled { get { return TreeList.SettingsSelection.Enabled; } }
		public bool IsSelectAllCheckVisible { get { return IsSelectionEnabled && TreeList.SettingsSelection.AllowSelectAll; } }
		public bool IsNodeDisplayedInEditMode { 
			get { return EditMode == TreeListEditMode.EditFormAndDisplayNode || EditMode == TreeListEditMode.PopupEditForm; } 
		}
		public bool NeedProcessSelectionChangedOnServer {
			get {
				return TreeList.SettingsBehavior.ProcessSelectionChangedOnServer
					|| TreeList.SettingsSelection.Recursive
					|| TreeList.SettingsSelection.AllowSelectAll;
			}
		}
		public bool IsRowSelectable(TreeListRowInfo row) {
			return IsSelectionEnabled && row.AllowSelect && (EditingNodeKey != row.NodeKey || IsNodeDisplayedInEditMode);
		}
		public bool HasIndentCells { get { return IsRootIndentVisible || MaxVisibleIndentCount > 0; } }
		public TreeListRowIndentType FilterAuxRowIndent(TreeListRowIndentType indent) {
			if(indent == TreeListRowIndentType.Middle || indent == TreeListRowIndentType.First)
				return TreeListRowIndentType.Root;
			if(indent == TreeListRowIndentType.Last)
				return TreeListRowIndentType.None;
			return indent;
		}
		public int GetLevelFromIndentCount(int indentCount) {
			if(!IsRootIndentVisible)
				indentCount++;
			return indentCount;
		}
		public TreeListEditMode EditMode { get { return TreeList.SettingsEditing.Mode; } }
		public string EditingNodeKey { get { return TreeDataHelper.EditingKey; } }
		public string NewNodeParentKey { get { return TreeDataHelper.NewNodeParentKey; } }
		public bool IsNewNodeEditing { get { return TreeDataHelper.IsNewNodeEditing; } }
		public bool IsEditing { get { return TreeDataHelper.IsEditing; } }
		public bool RenderPopupEditForm { 
			get { return IsEditing && TreeList.SettingsEditing.IsPopupEditForm && TreeDataHelper.GetPopupEditFormRowInfo() != null; } 
		}
		public ASPxEditBase CreateEditor(TreeListDataColumn column, object value, EditorInplaceMode mode, bool isDummy) {
			CreateEditControlArgs args = new CreateEditControlArgs(value, TreeDataHelper.GetFieldType(column.FieldName), TreeList.ImagesEditors, TreeList.StylesEditors, TreeList, mode, TreeWidthSpecifiedInPixels);
			ASPxEditBase edit = column.PropertiesEdit.CreateEdit(args);
			if(!isDummy) {
				edit.ID = GetEditorId(column);
				edit.EnableClientSideAPI = true;
				edit.EnableViewState = false;				
			}
			return edit;
		}
		protected internal virtual void PrepareEditor(ASPxEditBase baseEdit, TreeListDataColumn column, int columnSpan) {
			if(baseEdit.Width.IsEmpty)
				baseEdit.Width = Unit.Percentage(100);
			if(columnSpan > 1 && baseEdit.Height.IsEmpty)
				baseEdit.Height = Unit.Percentage(100);
			baseEdit.ReadOnly = column.ReadOnly;
			ASPxEdit edit = baseEdit as ASPxEdit;
			if(edit == null)
				return;
			string error = TreeDataHelper.GetEditingError(column.FieldName);
			bool hasError = !String.IsNullOrEmpty(error);
			edit.IsValid = !hasError;
			edit.ValidationSettings.ErrorText = hasError ? error : string.Empty;
		}
		public void PerformEditorRegistration() {
			foreach(TreeListColumn column in TreeList.Columns) {
				TreeListDataColumn dataColumn = column as TreeListDataColumn;
				if(dataColumn == null) continue;
				ASPxEditBase editor = CreateEditor(dataColumn, null, EditorInplaceMode.Inplace, true);
				TreeList.EditorRegistrator.Controls.Add(editor);
			}			
		}
		public virtual bool IsNodeDragDropEnabled { get { return TreeList.SettingsEditing.AllowNodeDragDrop; } }		
		public bool IsEditingKey(string key) {
			return key == TreeListRenderHelper.NewNodeKey || key == EditingNodeKey;
		}
		public TreeListCommandColumnButton GetCommandColumnButton(TreeListCommandColumn column, TreeListCommandColumnButtonType type) {
			switch(type) {
				case TreeListCommandColumnButtonType.New:
					return column.NewButton;
				case TreeListCommandColumnButtonType.Cancel:
					return column.CancelButton;
				case TreeListCommandColumnButtonType.Delete:
					return column.DeleteButton;
				case TreeListCommandColumnButtonType.Edit:
					return column.EditButton;
				case TreeListCommandColumnButtonType.Update:
					return column.UpdateButton;
				default:
					throw new NotImplementedException();
			}
		}
		public TreeListSettingsDataSecurity SettingsDataSecurity { get { return TreeList.SettingsDataSecurity; } }
		public bool IsEnabled { get { return TreeList.IsEnabled(); } }
		public bool IsSortingEnabled { get { return TreeList.SettingsBehavior.AllowSort; } }
		public bool IsPreviewRowVisible { get { return TreeList.Settings.ShowPreview; } }
		public bool IsHeaderRowVisible { get { return TreeList.Settings.ShowColumnHeaders; } }		
		public bool IsColumnDragDropEnabled { get { return TreeList.Columns.Count > 1 && TreeList.SettingsBehavior.AllowDragDrop; } } 
		public bool NeedRenderCustomizationWindow { 
			get {
				return !TreeList.DesignMode
					&& IsHeaderRowVisible
					&& TreeList.SettingsBehavior.AllowDragDrop
					&& TreeList.SettingsCustomizationWindow.Enabled
					&& IsEnabled;
			} 
		}
		public bool NeedRenderStyleTable { 
			get {
				if(!IsEnabled)
					return false;
				return CanRenderStyleTable && (IsFocusedNodeEnabled || IsSelectionEnabled); 
			} 
		}
		public bool NeedExactStyleTable { 
			get { return IsAlternatingNodeStyleEnabled 
					|| TreeList.IsHtmlRowPreparedEventAssigned() 
					|| IsNodeDisplayedInEditMode; } 
		}
		public bool IsTotalFooterVisible { get { return TreeList.Settings.ShowFooter && Rows.Count > 0; } }
		public bool IsGroupFooterVisible { get { return TreeList.Settings.ShowGroupFooter; } }
		public GridLines GridLines { get { return TreeList.Settings.GridLines; } }
		public bool VerticalGridLinesVisible { get { return GridLines == GridLines.Both || GridLines == GridLines.Vertical; } }
		public bool HorzGridLinesVisible { get { return GridLines == GridLines.Both || GridLines == GridLines.Horizontal; } }
		public bool IsRootIndentVisible { get { return TreeList.Settings.ShowRoot; } }
		public bool IsFocusedNodeEnabled { 
			get { return IsEnabled && (TreeList.SettingsBehavior.AllowFocusedNode || TreeList.KeyboardSupport); } 
		}
		public bool NeedRenderLoadingPanel {
			get {
				if(TreeList.DesignMode || !TreeList.IsEnabled())
					return false;
				TreeListSettingsLoadingPanel settings = TreeList.SettingsLoadingPanel;
				return settings.Enabled && (settings.ShowOnPostBacks || TreeList.InternalIsCallbacksEnabled());
			}
		}
		public bool IsDataDataTableDblClickRequired { 
			get { return TreeList.SettingsBehavior.ExpandCollapseAction == TreeListExpandCollapseAction.NodeDblClick || !String.IsNullOrEmpty(TreeList.ClientSideEvents.NodeDblClick); } 
		}
		public bool SuppressOuterGridLines { get { return TreeList.Settings.SuppressOuterGridLines; } }
		public bool ContextMenuEventAssigned { get { return !String.IsNullOrEmpty(TreeList.ClientSideEvents.ContextMenu); } }
		protected virtual bool CanRenderStyleTable {
			get { return TreeList.Page != null && !TreeList.DesignMode; }
		}
		public ScrollBarMode HorizontalScrollBarMode { get { return TreeList.Settings.HorizontalScrollBarMode; } }
		public ScrollBarMode VerticalScrollBarMode { get { return TreeList.Settings.VerticalScrollBarMode; } }
		public bool HasScrolling { get { return HasVerticalScrollBar || HasHorizontalScrollBar; } }
		public bool HasVerticalScrollBar { get { return VerticalScrollBarMode != ScrollBarMode.Hidden; } }
		public bool HasHorizontalScrollBar { get { return HorizontalScrollBarMode != ScrollBarMode.Hidden; } }
		public bool HasEllipsis {
			get {
				return TreeList.SettingsBehavior.AllowEllipsisInText ||
					   TreeList.Columns.OfType<IWebColumn>().Any(x => x.GetAllowEllipsisInText());
			}
		}
		public bool RequireFixedTableLayout { get { return HasEllipsis || HasScrolling || AllowColumnResizing; } }
		public void AddHorzScrollExtraCell(TableRow row) {
			AddHorzScrollExtraCell(row, false);
		}
		public void AddHorzScrollExtraCell(TableRow row, bool isHeaderCell) {
			if(HasHorizontalScrollBar)
				row.Cells.Add(new TreeListHorzScrollExtraCell(this, isHeaderCell));
		}
		public ColumnResizeMode ColumnResizeMode { get { return TreeList.SettingsBehavior.ColumnResizeMode; } }
		public bool AllowColumnResizing { get { return ColumnResizeMode != ColumnResizeMode.Disabled; } }
		public int GetColumnMinWidth(TreeListColumn column) {
			return column.MinWidth > 0 ? column.MinWidth : TreeList.Settings.ColumnMinWidth;
		}
		public bool IsColumnSortable(TreeListDataColumn column) {
			return IsSortingEnabled && column.AllowSort != DefaultBoolean.False || column.AllowSort == DefaultBoolean.True;
		}
		public bool IsColumnClickable(TreeListColumn column) {
			if(!IsEnabled)
				return false;
			TreeListDataColumn dataCol = column as TreeListDataColumn;
			return AllowColumnResizing || IsColumnDragDropEnabled || dataCol != null && IsColumnSortable(dataCol);
		}
		public static Control CreateNbsp() {
			return RenderUtils.CreateLiteralControl("&nbsp;");
		}
		public TreeListRowInfo GetRowByIndex(int index) {
			if(index == TreeListRootRowInfo.RowIndex)
				return RootRowInfo;
			if(index < 0 || index > Rows.Count - 1)
				return null;
			return Rows[index];
		}
		public bool NeedDataRowPointerCursor(TreeListRowInfo row) {
			return row.HasButton && TreeList.SettingsBehavior.ExpandCollapseAction == TreeListExpandCollapseAction.NodeClick && IsEnabled;
		}
	}
}
