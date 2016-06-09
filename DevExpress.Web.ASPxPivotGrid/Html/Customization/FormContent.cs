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
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public interface ISupportsFieldsCustomization {
		PivotGridWebData Data { get; }
		PivotGridStyles Styles { get; }
		string UniqueID { get; }
		string ClientID { get; }
		CustomizationFormStyle FormStyle { get; }
		CustomizationFormLayout FormLayout { get; set; }
		bool DeferredUpdates { get; set; }
		bool AllowSortInForm { get; }
		bool AllowFilterInForm { get; }
		ASPxPivotGridPopupMenu LayoutMenu { get; }
		PivotCustomizationFormImages Images { get; }
		CustomizationFormAllowedLayouts AllowedFormLayouts { get; }
	}
	public class PivotGridHtmlCustomizationFields : ASPxWebControl {
		public const string ElementName_LayoutPrefix = "_FL";
		public const string ElementName = "CustFields";
		public const string ElementName_DeferredUpdate = "FLDefere";
		public const string ElementName_LayoutButton = "FLButton";
		public const string ElementName_FilterRowLists = "FLFRDiv";
		public const string ElementName_ColumnDataLists = "FLCDDiv";
		public const string ElementName_ListHeader = "FLTextDiv";
		public const string ElementName_ListHeaderImgDiv = "FLTextImgDiv";
		public const string ElementName_ListBody = "FLListDiv";
		ISupportsFieldsCustomization control;
		CustomizationTreeView treeView;
		public PivotGridHtmlCustomizationFields(ISupportsFieldsCustomization control) {
			this.control = control;
			if (IsMvcRender())
				ClientIDHelper.DisableClientIDGeneration(this);
		}
		public static string ElementName_ID {
			get { return CreateIdByName(ElementName); }
		}
		public override string ClientID {
			get {
				return IsMvcRender() || string.IsNullOrEmpty(Control.ClientID) ? base.ClientID : string.Format("{0}_{1}", Control.ClientID, ElementName_ID);
			}
		}
		protected ISupportsFieldsCustomization Control { get { return control; } }
		protected CustomizationTreeView TreeView { get { return treeView; } }
		protected PivotGridWebData Data { get { return Control.Data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		ImageProperties RenderImage(string imageName) {
			return Control.Images.GetImageProperties(Page, imageName, true);
		}
		public override string UniqueID { get { return GetUniqueID(); } }
		string GetUniqueID() {
			return Control.UniqueID + "_" + ElementName_ID;
		}
		CustomizationFormFields FieldListFields { get { return Data.FieldListFields; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
		}
		protected AppearanceStyleBase CreateStyleByName(string name) {
			return Control.Styles.CreateStyleByName(name);
		}
		protected static string CreateIdByName(string name) {
			return "dxpg" + name;
		}
		protected override void CreateControlHierarchy() {
			Controls.Clear();
			this.ID = ElementName_ID;
			if(Control.FormStyle == CustomizationFormStyle.Excel2007)
				CreateExcel2007Headers();
			else
				CreateSimpleHeaders();
			base.CreateControlHierarchy();
		}
		public void ReadPostData() {
			if(Request == null) return;
			string deferredUpdateCB = Request.Params[UniqueID + "$" + CreateIdByName(ElementName_DeferredUpdate)];
			string customizationFormLayout = Request.Params[UniqueID + "$" + CreateIdByName(ElementName_LayoutButton)];
			if(deferredUpdateCB != null)
				Control.DeferredUpdates = deferredUpdateCB == "C";
			if(customizationFormLayout != null)
				Control.FormLayout = (CustomizationFormLayout)Enum.Parse(typeof(CustomizationFormLayout), customizationFormLayout);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RenderUtils.IsOverflowStyleSeparated || RenderUtils.Browser.IsFirefox && RenderUtils.Browser.Version >= 3)
				Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
			Style.Add(HtmlTextWriterStyle.OverflowY, "hidden");
			if(Control.FormStyle == CustomizationFormStyle.Excel2007) {
				PrepareHeadersAreaImages();
				PrepareLayoutButtonImage();
			}
		}
		void PrepareHeadersAreaImages() {
			foreach(HeaderPresenterType area in Enum.GetValues(typeof(HeaderPresenterType))) {
				if(HeadersAreaImagesCash.ContainsKey(area))
					PrepareHeadersAreaImage(area);
			}
		}
		void PrepareHeadersAreaImage(HeaderPresenterType area) {
			Image image = HeadersAreaImagesCash[area];
			string imageName = GetHeadersAreaImageName(area);
			RenderImage(imageName).AssignToControl(image, DesignMode);
		}
		void PrepareLayoutButtonImage() {
			LayoutButton.Image.Assign(RenderImage(PivotCustomizationFormImages.LayoutButtonName));
		}
		public void PrepareLayoutMenu() {
			if(Control.FormStyle != CustomizationFormStyle.Excel2007)
				return;
			if(Control.LayoutMenu == null)
				return;
			PrepareLayoutMenu(Control.LayoutMenu, Control.FormLayout, Control.AllowedFormLayouts);
			Control.LayoutMenu.PivotGrid.RaisePopupMenuCreated(Control.LayoutMenu);
		}
		protected internal void PrepareLayoutMenu(ASPxPivotGridPopupMenu layoutMenu, CustomizationFormLayout formLayout, CustomizationFormAllowedLayouts allowedLayouts) {
			foreach(CustomizationFormLayout layout in Enum.GetValues(typeof(CustomizationFormLayout))) {
				if(!CustomizationFormEnumExtensions.IsLayoutAllowed(allowedLayouts, layout))
					continue;
				DevExpress.Web.MenuItem item = layoutMenu.Items.Add(PivotGridLocalizer.GetString(CustomizationFormEnumExtensions.GetStringId(layout)), layout.ToString());
				item.Image.Assign(RenderImage(GetLayoutImageName(layout)));
				if(layout == formLayout)
					item.ClientEnabled = false;
			}
		}
		string GetHeadersAreaImageName(HeaderPresenterType area) {
			switch(area) {
				case HeaderPresenterType.FilterAreaHeaders:
					return PivotCustomizationFormImages.FilterAreaHeadersName;
				case HeaderPresenterType.ColumnAreaHeaders:
					return PivotCustomizationFormImages.ColumnAreaHeadersName;
				case HeaderPresenterType.RowAreaHeaders:
					return PivotCustomizationFormImages.RowAreaHeadersName;
				case HeaderPresenterType.DataAreaHeaders:
					return PivotCustomizationFormImages.DataAreaHeadersName;
				case HeaderPresenterType.FieldList:
					return PivotCustomizationFormImages.FieldListHeadersName;
				default:
					return string.Empty;
			}			
		}
		string GetLayoutImageName(CustomizationFormLayout layout) {
			switch(layout) {
				case CustomizationFormLayout.StackedDefault:
					return PivotCustomizationFormImages.StackedDefaultLayoutName;
				case CustomizationFormLayout.StackedSideBySide:
					return PivotCustomizationFormImages.StackedSideBySideLayoutName;
				case CustomizationFormLayout.TopPanelOnly:
					return PivotCustomizationFormImages.TopPanelOnlyLayoutName;
				case CustomizationFormLayout.BottomPanelOnly2by2:
					return PivotCustomizationFormImages.BottomPanelOnly2by2LayoutName;
				case CustomizationFormLayout.BottomPanelOnly1by4:
					return PivotCustomizationFormImages.BottomPanelOnly1by4LayoutName;
				default:
					return string.Empty;
			}
		}
		void SetTableStyle(Table maintable) {
			if(maintable != null) {
				maintable.Width = Unit.Percentage(100);
				maintable.GridLines = GridLines.None;
				maintable.Style.Add(HtmlTextWriterStyle.BorderCollapse, "collapse");
				maintable.CellPadding = 0;
				maintable.CellSpacing = 0;
			}
		}
		void CreateSimpleHeaders() {
			CssClass = string.Empty;
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			div.Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
			div.Style.Add(HtmlTextWriterStyle.OverflowY, "auto");
			div.Height = Unit.Percentage(100);
			Table fieldsTable = RenderUtils.CreateTable();
			div.Controls.Add(fieldsTable);
			SetHeadersIdCssClass(HeaderPresenterType.FieldList, div);
			Controls.Add(div);
			ReadOnlyCollection<PivotFieldItemBase> fields = FieldListFields[HeaderPresenterType.FieldList];
			foreach(PivotFieldItemBase field in fields)
				CreateHeader(field, fieldsTable);
			TableRow row = RenderUtils.CreateTableRow();
			fieldsTable.Rows.Add(row);
			row.Cells.Add(RenderUtils.CreateTableCell());
			SetTableStyle(fieldsTable);
		}
		protected Paddings GetHeaderPaddings() {
			return Control.FormStyle == CustomizationFormStyle.Excel2007 ? new Paddings(Unit.Pixel(2), Unit.Pixel(2), Unit.Pixel(2), Unit.Pixel(1)) : new Paddings(Unit.Pixel(3), Unit.Pixel(3), Unit.Pixel(3), Unit.Pixel(0));
		}
		void CreateHeader(PivotFieldItemBase field, Table table) {
			Paddings headerPaddings = GetHeaderPaddings();
			WebControl header = HeaderHelper.CreateHeader(field, Data, Control);
			if(header == null)
				return;
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			headerPaddings.AssignToControl(cell);
			cell.Controls.Add(header);
			row.Cells.Add(cell);
		}
		void CreateExcel2007Headers() {
			CssClass = Control.FormLayout.ToString();
			if(IsDesignMode)
				CreateExcel2007HeaderDesignTime();
			else
				CreateExcel2007HeaderRunTime();
		}
		protected virtual void CreateExcel2007HeaderRunTime() {
			Controls.Add(CreateLayoutButtonDiv());
			Controls.Add(CreateFieldLists());
			Controls.Add(CreateDeferUpdatesDiv());
		}
		WebControl CreateFieldLists() {
			WebControl listsDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName + "Div").AssignToControl(listsDiv);
			CreateAreaFieldList(HeaderPresenterType.FieldList, listsDiv);
			WebControl FRDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName_FilterRowLists).AssignToControl(FRDiv);
			CreateAreaFieldList(HeaderPresenterType.FilterAreaHeaders, FRDiv);
			CreateAreaFieldList(HeaderPresenterType.RowAreaHeaders, FRDiv);
			listsDiv.Controls.Add(FRDiv);
			WebControl CDDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName_ColumnDataLists).AssignToControl(CDDiv);
			CreateAreaFieldList(HeaderPresenterType.ColumnAreaHeaders, CDDiv);
			CreateAreaFieldList(HeaderPresenterType.DataAreaHeaders, CDDiv);
			listsDiv.Controls.Add(CDDiv);
			return listsDiv;
		}
		WebControl CreateDeferUpdatesDiv() {
			WebControl deferDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			HtmlTable table = new HtmlTable();
			table.Style[HtmlTextWriterStyle.Width] = "100%";
			HtmlTableRow row = new HtmlTableRow();
			table.Rows.Add(row);
			CreateStyleByName(ElementName_DeferredUpdate + "Div").AssignToControl(deferDiv);
			deferDiv.ID = CreateIdByName(ElementName_DeferredUpdate + "Div");
			HtmlTableCell checkBoxCell = new HtmlTableCell();
			checkBoxCell.Controls.Add(CreateDeferDivCheckBox());
			HtmlTableCell updateButtonCell = new HtmlTableCell();
			updateButtonCell.Controls.Add(CreateDeferDivButton());
			row.Cells.Add(checkBoxCell);
			row.Cells.Add(updateButtonCell);
			deferDiv.Controls.Add(table);
			return deferDiv;
		}
		ASPxButton layoutButton;
		ASPxButton LayoutButton { get { return layoutButton; } }
		WebControl CreateLayoutButtonDiv() {
			WebControl buttonDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName_LayoutButton + "Div").AssignToControl(buttonDiv);
			layoutButton = new ASPxButton();
			layoutButton.AutoPostBack = false;
			layoutButton.ParentSkinOwner = PivotGrid;
			CreateStyleByName(ElementName_LayoutButton).AssignToControl(layoutButton);
			layoutButton.Attributes.Add("onclick", string.Format(@"ASPx.pivotGrid_OnFieldListLayoutButtonClick('{0}', event)", Control.ClientID));
			buttonDiv.Controls.Add(layoutButton);
			HtmlInputHidden layoutInput = new HtmlInputHidden();
			layoutInput.Value = Control.FormLayout.ToString();
			layoutInput.ID = CreateIdByName(ElementName_LayoutButton);
			buttonDiv.Controls.Add(layoutInput);
			if(IsOneLayoutAllowed(Control.AllowedFormLayouts))
				buttonDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
			return buttonDiv;
		}
		bool IsOneLayoutAllowed(CustomizationFormAllowedLayouts allowedLayouts) {
		  int count = 0;
		  foreach(CustomizationFormAllowedLayouts lay in Enum.GetValues(typeof(CustomizationFormAllowedLayouts))) {
			  if(lay != CustomizationFormAllowedLayouts.All && (allowedLayouts & lay) != 0)
				  count++;
		  }
		  return count <= 1;
		}
		ASPxCheckBox CreateDeferDivCheckBox() {
			ASPxCheckBox cb = new ASPxCheckBox();
			cb.AutoPostBack = false;
			cb.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormDeferLayoutUpdate);
			cb.ID = CreateIdByName(ElementName_DeferredUpdate);
			cb.Checked = Control.DeferredUpdates;
			cb.Style.Add("border-collapse", "separate");
			cb.ClientSideEvents.CheckedChanged = GetDeferredUpdatesCheckBoxOnCheckedChangedHandler();
			cb.ParentSkinOwner = PivotGrid ?? Control as ISkinOwner;
			return cb;
		}
		string GetDeferredUpdatesCheckBoxOnCheckedChangedHandler() {
			return string.Format(
				@"function (s,e) {{
                    if (!s.previousValue) {{
                        {1}.SetEnabled(false)
                        ASPx.pivotGrid_CustomizationFormResumeUpdates('{0}');
                    }} else {{
                        {1}.SetEnabled(true);
                        ASPx.pivotGrid_CustomizationFormDeferUpdates('{0}');
                    }}
                }}",
				Control.ClientID, GetDeferButtonId()
			);
		}
		string GetDeferButtonId() {
			return GetUniqueID() + "_" + CreateIdByName(ElementName_DeferredUpdate + "DB");
		}
		ASPxButton CreateDeferDivButton() {
			ASPxButton deferDivButton = new ASPxButton();
			deferDivButton.Text = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormUpdate);
			CreateStyleByName(ElementName_DeferredUpdate + "DB").AssignToControl(deferDivButton);
			deferDivButton.ClientInstanceName = GetDeferButtonId();
			deferDivButton.ClientSideEvents.Click = GetDeferredUpdatesButtonOnClickHandler();
			deferDivButton.AutoPostBack = false;
			deferDivButton.ClientEnabled = Control.DeferredUpdates;
			deferDivButton.ParentSkinOwner = PivotGrid;
			return deferDivButton;
		}
		string GetDeferredUpdatesButtonOnClickHandler() {
			return string.Format("function(s, e) {{ ASPx.pivotGrid_CustomizationFormUpdate('{0}'); }}", Control.ClientID);
		}
		void CreateAreaFieldList(HeaderPresenterType area, WebControl parentWebControl) {
			ReadOnlyCollection<PivotFieldItemBase> fields = FieldListFields[area];
			WebControl mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			SetHeadersIdCssClass(area, mainDiv);
			mainDiv.Controls.Add(CreateAreaFieldListTextDiv(area));
			WebControl listDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName_ListBody).AssignToControl(listDiv);
			mainDiv.Controls.Add(listDiv);
			WebControl paddingDiv2 = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			listDiv.Controls.Add(paddingDiv2);
			WebControl paddingDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			paddingDiv2.Controls.Add(paddingDiv);
			Table listTable = new InternalTable();
			paddingDiv.Controls.Add(listTable);
			listTable.Width = Unit.Percentage(100);
			foreach(PivotFieldItemBase field in fields)
				CreateHeader(field, listTable);
			AddEmptyTableRow(listTable);			 
			SetTableStyle(listTable);
			if(area == HeaderPresenterType.FieldList && Data.ShowCustomizationTree) {
				listTable.Style.Add(HtmlTextWriterStyle.Display, "none");
				listTable.ID = "listCF";
				if(PivotGrid != null && !PivotGrid.IsDataBound && !PivotGrid.IsDataBinding && !string.IsNullOrEmpty(PivotGrid.OLAPConnectionString))
					PivotGrid.DataBind();
				this.treeView = new CustomizationTreeView(PivotGrid, Control);
				paddingDiv.Controls.Add(TreeView);
				PivotCustomizationFieldsTree tree = new PivotCustomizationFieldsTree(new CustomizationFormFields(Data), Data);
				tree.Update(true);
				TreeView.AddNodes(tree);
			}
			parentWebControl.Controls.Add(mainDiv);
		}
		Dictionary<HeaderPresenterType, Image> headersAreaImagesCash;
		Dictionary<HeaderPresenterType, Image> HeadersAreaImagesCash {
			get {
				if(headersAreaImagesCash == null)
					headersAreaImagesCash = new Dictionary<HeaderPresenterType, Image>();
				return headersAreaImagesCash;
			}
		}
		WebControl CreateAreaFieldListTextDiv(HeaderPresenterType area) {	
			WebControl textDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			CreateStyleByName(ElementName_ListHeader).AssignToControl(textDiv);
			WebControl headersListImageDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Image image = RenderUtils.CreateImage();
			HeadersAreaImagesCash[area] = image;
			headersListImageDiv.Controls.Add(image);
			CreateStyleByName(ElementName_ListHeaderImgDiv).AssignToControl(headersListImageDiv);
			textDiv.Controls.Add(headersListImageDiv);
			WebControl textDiv2 = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			textDiv.Controls.Add(textDiv2);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl();
			literalControl.Text = PivotGridLocalizer.GetString(FieldListFields.GetStringId(area));
			textDiv2.Controls.Add(literalControl);
			return textDiv;
		}
		void AddEmptyTableRow(Table listTable) {
			TableRow row = RenderUtils.CreateTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			listTable.Rows.Add(row);
			row.Height = Unit.Pixel(0);
		}
		void SetHeadersIdCssClass(HeaderPresenterType area, WebControl div) {
			if(area == HeaderPresenterType.FieldList)
				div.ID = ElementName_ID + area.ToString();
			else
				div.ID = ScriptHelper.GetAreaID(PivotEnumExtensionsBase.ToPivotArea(area));
			div.CssClass = CreateStyleByName(ElementName + area.ToString()).CssClass + " " + CreateStyleByName(ElementName).CssClass;
		}
		public void AddHoverScript(StateScriptRenderHelper helper) {
			if(Control.FormStyle == CustomizationFormStyle.Excel2007) {
				foreach(HeaderPresenterType area in Enum.GetValues(typeof(HeaderPresenterType)))
					AddHoverScriptCore(helper, FieldListFields[area]);
			}
			else {
				AddHoverScriptCore(helper, FieldListFields.HiddenFields);
			}
		}
		void AddHoverScriptCore(StateScriptRenderHelper helper, ReadOnlyCollection<PivotFieldItemBase> fields) {
			foreach(PivotFieldItemBase fieldItem in fields) {
				string headerId = ElementName_ID + "_" + ((fieldItem.Group == null) ?
				   ScriptHelper.GetHeaderID(fieldItem) : ScriptHelper.GetGroupHeaderID(fieldItem.Group));
				helper.AddStyle(Control.Data.GetHeaderHoverStyle((PivotFieldItem)fieldItem), headerId,
							ScriptHelper.FieldHeaderIdPostfixes, this.IsEnabled());
			}
		}
		#region Design-Time Layout
		bool IsDesignMode {
			get { return DesignMode; }
		}
		int designTableSpan;
		int DesignTableSpan {
			get { return designTableSpan; }
			set { designTableSpan = value; }
		}
		string DesignLayoutButtonCellHeight { get { return Unit.Pixel(36).ToString(); } }
		string DesignDeferredUpdatesCellHeight { get { return Unit.Pixel(35).ToString(); } }
		string DesignDeferredUpdatesCellWidht { get { return Unit.Percentage(50).ToString(); } }
		string DesignTextCellHeight { get { return Unit.Pixel(28).ToString(); } }
		string DesignImageCellWidth { get { return Unit.Pixel(16).ToString(); } }
		string DesignImageCellHeight { get { return Unit.Pixel(16).ToString(); } }
		void CreateExcel2007HeaderDesignTime() {
			Table mainTable = new InternalTable();
			mainTable.Style[HtmlTextWriterStyle.Width] = Unit.Percentage(100).ToString();
			mainTable.Style[HtmlTextWriterStyle.Height] = Unit.Percentage(100).ToString();
			DesignCreateLayoutButtonTableRow(mainTable);
			DesignCreateFieldList(mainTable);
			DesignCreateDeferredUpdatesTableRow(mainTable);
			Controls.Add(mainTable);
		}
		TableRow DesignGetLayoutButtonTableRow() {
			TableRow row = new TableRow();
			TableCell leftCell = DesignGetSpacerTableCell();
			TableCell rightCell = new TableCell();
			rightCell.Style[HtmlTextWriterStyle.Height] = DesignLayoutButtonCellHeight;
			leftCell.Style[HtmlTextWriterStyle.Height] = DesignLayoutButtonCellHeight;
			rightCell.HorizontalAlign = HorizontalAlign.Right;
			layoutButton = new ASPxButton();
			layoutButton.Height = Unit.Pixel(27);
			layoutButton.Width = Unit.Pixel(36);
			rightCell.Controls.Add(layoutButton);
			row.Cells.Add(DesignGetEmptySpanCell(36));
			row.Cells.Add(leftCell);
			row.Cells.Add(DesignGetEmptySpanCell(36));
			row.Cells.Add(rightCell);
			row.Cells.Add(DesignGetEmptySpanCell(36));
			return row;
		}
		TableRow DesignGetDeferredUpdatesTableRow() {
			TableRow row = new TableRow();
			TableCell leftCell = new TableCell();
			TableCell rightCell = new TableCell();
			ASPxCheckBox checkBox = CreateDeferDivCheckBox();
			checkBox.CssClass = string.Empty;
			leftCell.HorizontalAlign = HorizontalAlign.Left;
			leftCell.Controls.Add(checkBox);
			ASPxButton button = CreateDeferDivButton();
			button.Height = Unit.Pixel(25);
			button.Width = Unit.Pixel(60);
			button.CssClass = string.Empty;
			rightCell.HorizontalAlign = HorizontalAlign.Right;
			rightCell.Controls.Add(button);
			leftCell.Style[HtmlTextWriterStyle.Width] = DesignDeferredUpdatesCellWidht;
			leftCell.Style[HtmlTextWriterStyle.Height] = DesignDeferredUpdatesCellHeight;
			rightCell.Style[HtmlTextWriterStyle.Width] = DesignDeferredUpdatesCellWidht;
			rightCell.Style[HtmlTextWriterStyle.Height] = DesignDeferredUpdatesCellHeight;
			row.Cells.Add(DesignGetEmptySpanCell(35));
			row.Cells.Add(leftCell);
			row.Cells.Add(DesignGetEmptySpanCell(35));
			row.Cells.Add(rightCell);
			row.Cells.Add(DesignGetEmptySpanCell(35));
			return row;
		}
		void DesignCreateLayoutButtonTableRow(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutButtonTableRow());
		}
		void DesignCreateDeferredUpdatesTableRow(Table mainTable) {
			if(Control.FormLayout != CustomizationFormLayout.TopPanelOnly)
				mainTable.Rows.Add(DesignGetDeferredUpdatesTableRow());
		}
		void DesignCreateFieldList(Table mainTable) {
			switch(Control.FormLayout) {
				case CustomizationFormLayout.StackedSideBySide:
					DesignTableSpan = 2;
					DesignCreateStackedSideBySideLayout(mainTable);
					break;
				case CustomizationFormLayout.BottomPanelOnly2by2:
					DesignTableSpan = 3;
					DesignCreateBottomPanelOnly2by2Layout(mainTable);
					break;
				case CustomizationFormLayout.StackedDefault:
					DesignTableSpan = 2;
					DesignCreateStackedDefaultLayout(mainTable);
					break;
				case CustomizationFormLayout.BottomPanelOnly1by4:
					DesignTableSpan = 2;
					DesignCreateBottomPanelOnly1by4Layout(mainTable);
					break;
				case CustomizationFormLayout.TopPanelOnly:
					DesignTableSpan = 3;
					DesignCreateTopPanelOnlyLayout(mainTable);
					break;
			}
		}
		TableCell DesignGetAreaFieldListTextTableCell(HeaderPresenterType area) {
			TableCell cell = new TableCell();
			WebControl textDiv = DesignCreateAreaFieldListTextDiv(area);
			cell.Controls.Add(textDiv);
			cell.Style[HtmlTextWriterStyle.VerticalAlign] = "bottom";
			cell.Style[HtmlTextWriterStyle.Height] = DesignTextCellHeight;
			return cell;
		}
		WebControl DesignCreateAreaFieldListTextDiv(HeaderPresenterType area) {
			Table table = new InternalTable();
			table.Style[HtmlTextWriterStyle.Width] = "100%";
			TableRow row = new TableRow();
			table.Rows.Add(row);
			TableCell imageCell = new TableCell();
			imageCell.Style[HtmlTextWriterStyle.Width] = DesignImageCellWidth;
			imageCell.Style[HtmlTextWriterStyle.Height] = DesignImageCellHeight;
			row.Cells.Add(imageCell);
			Image image = RenderUtils.CreateImage();
			image.Style[HtmlTextWriterStyle.Width] = DesignImageCellWidth;
			image.Style[HtmlTextWriterStyle.Height] = DesignImageCellHeight;
			imageCell.Controls.Add(image);
			HeadersAreaImagesCash[area] = image;
			CreateStyleByName(ElementName_ListHeaderImgDiv).AssignToControl(imageCell);
			imageCell.Style[HtmlTextWriterStyle.Margin] = "0px 0px 0px 0px";
			TableCell textCell = new TableCell();
			textCell.Style[HtmlTextWriterStyle.MarginLeft] = "4px";
			row.Cells.Add(textCell);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl();
			textCell.Controls.Add(literalControl);
			literalControl.Text = PivotGridLocalizer.GetString(FieldListFields.GetStringId(area));
			return table;
		}
		TableCell DesignGetSpacerTableCell() {
			TableCell cell = new TableCell();
			cell.Text = "&nbsp;";
			return cell;
		}
		TableCell DesignGetAreaFieldListTableCell(int counter) {
			TableCell cell = DesignGetSpacerTableCell();
			cell.RowSpan = DesignTableSpan * counter;
			cell.Style[HtmlTextWriterStyle.BorderColor] = "#9f9f9f";
			cell.Style[HtmlTextWriterStyle.BorderWidth] = "1px";
			cell.Style[HtmlTextWriterStyle.BorderStyle] = "solid";
			return cell;
		}
		TableCell DesignGetAreaFieldListTableCell() {
			return DesignGetAreaFieldListTableCell(1);
		}
		TableRow DesignGetLayoutTableRow(TableCell cell1, TableCell cell2) {
			TableRow row = new TableRow();
			row.Cells.Add(DesignGetEmptySpanCell(28));
			if(cell1 != null)
				row.Cells.Add(cell1);
			row.Cells.Add(DesignGetEmptySpanCell(28));
			if(cell2 != null)
				row.Cells.Add(cell2);
			if(cell1 != null && cell2 != null) {
				cell1.Style[HtmlTextWriterStyle.Width] = Unit.Percentage(50).ToString();
				cell2.Style[HtmlTextWriterStyle.Width] = Unit.Percentage(50).ToString();
			}
			row.Cells.Add(DesignGetEmptySpanCell(28));
			return row;
		}
		TableRow DesignGetLayoutTableRow(TableCell cell) {
			TableRow row = new TableRow();
			row.Cells.Add(DesignGetEmptySpanCell());
			cell.ColumnSpan = 3;
			row.Cells.Add(cell);
			row.Cells.Add(DesignGetEmptySpanCell());
			return row;
		}
		void DesignAddEmptySpanRows(Table mainTable, int cellsCount) {
			for(int i = 0; i < DesignTableSpan - 1; i++) {
				TableRow row = new TableRow();
				for(int c = 0; c < cellsCount; c++)
					row.Cells.Add(DesignGetEmptySpanCell());
				mainTable.Rows.Add(row);
			}
		}
		TableCell DesignGetEmptySpanCell() {
			return DesignGetEmptySpanCell(-1);
		}
		TableCell DesignGetEmptySpanCell(int height) {
			TableCell cell = DesignGetSpacerTableCell();
			cell.Style[HtmlTextWriterStyle.Width] = Unit.Pixel(4).ToString();
			if(height > 0)
				cell.Style[HtmlTextWriterStyle.Height] = Unit.Pixel(height).ToString();
			return cell;
		}
		void DesignCreateStackedSideBySideLayout(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FieldList),
								DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FilterAreaHeaders)));
			TableCell hiddenFieldsCell = DesignGetAreaFieldListTableCell();
			hiddenFieldsCell.RowSpan = DesignTableSpan * 4 + 3;
			mainTable.Rows.Add(DesignGetLayoutTableRow(hiddenFieldsCell, DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTextTableCell(HeaderPresenterType.RowAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTextTableCell(HeaderPresenterType.ColumnAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTextTableCell(HeaderPresenterType.DataAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(null, DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
		}
		void DesignCreateBottomPanelOnly2by2Layout(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FilterAreaHeaders),
					DesignGetAreaFieldListTextTableCell(HeaderPresenterType.ColumnAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell(), DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.RowAreaHeaders),
					DesignGetAreaFieldListTextTableCell(HeaderPresenterType.DataAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell(), DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
		}
		void DesignCreateStackedDefaultLayout(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FieldList)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 2);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FilterAreaHeaders),
					DesignGetAreaFieldListTextTableCell(HeaderPresenterType.ColumnAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell(), DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.RowAreaHeaders),
					DesignGetAreaFieldListTextTableCell(HeaderPresenterType.DataAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell(), DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 3);
		}
		void DesignCreateBottomPanelOnly1by4Layout(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.FilterAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 2);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.RowAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 2);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.ColumnAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 2);
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTextTableCell(HeaderPresenterType.DataAreaHeaders)));
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell()));
			DesignAddEmptySpanRows(mainTable, 2);
		}
		void DesignCreateTopPanelOnlyLayout(Table mainTable) {
			mainTable.Rows.Add(DesignGetLayoutTableRow(DesignGetAreaFieldListTableCell(4)));
			for(int i = 0; i < 4; i++)
				DesignAddEmptySpanRows(mainTable, 2);
		}
		#endregion
	}
}
