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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Web.FilterControl;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public abstract class PivotGridHeaderHtmlTableBase : InternalTable {
		PivotGridHeaderTemplateItem templateItem;
		PivotGridHtmlHeaderFilter filterButton;
		PivotGridHtmlHeaderText headerText;
		InternalTableCell sortButton;
		public PivotGridHeaderHtmlTableBase(PivotGridWebData data, PivotFieldItemBase field, DefaultBoolean canDrag, ISupportsFieldsCustomization control)
			: this(new PivotGridHeaderTemplateItem(data, field, canDrag, control)) {
		}
		public PivotGridHeaderHtmlTableBase(PivotGridHeaderTemplateItem templateItem) {
			this.templateItem = templateItem;
		}
		protected PivotGridHeaderTemplateItem Item { get { return templateItem; } }
		protected PivotGridWebData Data { get { return Item.Data; } }
		protected PivotGridField Field { get { return Item.Field; } }
		protected PivotFieldItem FieldItem { get { return Item.FieldItem; } }
		protected PivotGridHtmlHeaderFilter FilterButton { get { return filterButton; } }
		protected PivotGridHtmlHeaderText HeaderText { get { return headerText; } }
		protected void CreateHierarchyCore() {
			SetID();
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			if(Item.IsGroupButtonVisible)
				row.Controls.Add(new PivotGridHtmlGroupButton(Item));
			headerText = new PivotGridHtmlHeaderText(Item);
			row.Controls.Add(headerText);
			if(Item.IsSortButtonVisible) {
				sortButton = new PivotGridHtmlHeaderSort(Item);
				row.Controls.Add(sortButton);
			}
			if(Item.IsFilterButtonVisible) {
				this.filterButton = new PivotGridHtmlHeaderFilter(Item);
				row.Controls.Add(this.filterButton);
			}
			if(Item.Field.Visible)
				Item.AddContextMenu();
		}
		protected virtual void SetID() {
			ID = Item.ID;
		}
		protected void PrepareHierarchyCore(bool applyAllStyles) {
			CellPadding = 0;
			CellSpacing = 0;
			if(applyAllStyles) {
				Data.GetHeaderTableStyle(FieldItem).AssignToControl(this);
				if(RenderUtils.Browser.IsOpera)
					RenderUtils.SetStyleStringAttribute(this, "border-collapse", "separate");   
				if(this.filterButton != null && !Field.Visible)
					this.filterButton.HorizontalAlign = HorizontalAlign.Right;
			} else {
				Data.GetEmptyHeaderStyle(FieldItem).AssignToControl(this);
				}
			if(headerText != null && !Item.IsAccessibilityCompliant && (Item.CanSort || Item.Field.CanSortOLAP))
				headerText.Attributes.Add("onclick", Item.HeaderClickScript);
			if(sortButton != null && !Item.IsAccessibilityCompliant)
				sortButton.Attributes.Add("onclick", Item.HeaderClickScript);
			if(Item.CanDrag)
				Attributes.Add(TouchUtils.TouchMouseDownEventName, Item.HeaderMouseDownScript);
		}
	}
	public class PivotGridHtmlHeaderContent : PivotGridHeaderHtmlTableBase {
		public PivotGridHtmlHeaderContent(PivotGridWebData data, PivotFieldItemBase field, DefaultBoolean canDrag, ISupportsFieldsCustomization control)
			: base(data, field, canDrag, control) {
		}
		protected ITemplate HeaderTemplate {
			get { return Field.HeaderTemplate != null ? Field.HeaderTemplate : Data.HeaderTemplate; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(HeaderTemplate != null)
				InstantiateTemplate();
			else
				CreateHierarchyCore();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareHierarchyCore(HeaderTemplate == null);
		}
		protected void InstantiateTemplate() {
			ID = Item.ID;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			InternalTableCell cell = new InternalTableCell();
			row.Cells.Add(cell);
			PivotGridHeaderTemplateContainer templateContainer = new PivotGridHeaderTemplateContainer(Item);
			cell.Controls.Add(templateContainer);
			Data.SetupTemplateContainer(templateContainer, HeaderTemplate);
			if(Item != null)
				this.Style.Add(HtmlTextWriterStyle.Width, "100%");
		}
	}
	public class PivotGridHtmlHeaderCell : InternalTableCell {
		PivotFieldItem field;
		PivotGridWebData data;
		public PivotFieldItem Field { get { return field; } }
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		public PivotGridHtmlHeaderCell(PivotGridWebData data, PivotFieldItem field) {
			this.field = field;
			this.data = data;
		}
	}
	public class PivotGridHtmlGroupButton : PivotGridHtmlHeaderCell {
		Image GroupButtonImage;
		HyperLink link;
		public PivotGridHtmlGroupButton(PivotGridHeaderTemplateItem item)
			: base(item.Data, item.FieldItem) {
			ID = ScriptHelper.GetGroupButtonID(Field);
		}
		protected override void CreateControlHierarchy() {
			GroupButtonImage = RenderUtils.CreateImage();
			if(Data.PivotGrid.IsAccessibilityCompliantRender() && Data.PivotGrid.IsEnabled()) {
				this.link = RenderUtils.CreateHyperLink();
				Controls.Add(this.link);
				this.link.Controls.Add(GroupButtonImage);
			} else {
				Controls.Add(GroupButtonImage);
			}
		}
		protected override void PrepareControlHierarchy() {
			Data.GetGroupButtonStyle(Field).AssignToControl(this);
			Data.PivotGrid.RenderHelper.GetGroupButtonImage(Field.ExpandedInFieldsGroup).AssignToControl(GroupButtonImage, Data.IsDesignMode);
			if(Data.PivotGrid.IsEnabled()) {
				string js = ScriptHelper.GetGroupButtonOnClick(Field.Index.ToString());
				if(this.link != null) {
					this.link.NavigateUrl = string.Format("javascript:{0}", js);
				} else {
					GroupButtonImage.Attributes.Add("onclick", js);
				}
			}
		}
	}
	public class PivotGridHtmlHeaderText : PivotGridHtmlHeaderCell {
		object content;
		PivotGridHeaderTemplateItem item;
		public PivotGridHtmlHeaderText(PivotGridHeaderTemplateItem item)
			: base(item.Data, item.FieldItem) {
			ID = ScriptHelper.GetHeaderTextCellID(Field);
			content = GetDefaultContent();
			this.item = item;
		}
		public object Content {
			get { return content; }
			set {
				content = value;
				ResetControlHierarchy();
			}
		}
		PivotGridHeaderTemplateItem Item {
			get { return item; }
		}
		WebControl transparentDiv;
		WebControl TransParentDiv {
			get {
				if(transparentDiv == null)
					transparentDiv = CreateTransparentDiv();
				return transparentDiv;
			}
		}
		WebControl CreateTransparentDiv() {
			WebControl transparentDiv = new WebControl(HtmlTextWriterTag.Div);
			transparentDiv.Style[HtmlTextWriterStyle.Height] = "100%";
			transparentDiv.Style[HtmlTextWriterStyle.Width] = "100%";
			return transparentDiv;
		}
		protected override void CreateControlHierarchy() {
			ControlCollection controls = GetControlCollection();
			Control control = Content as Control;
			if(control != null) {
				controls.Add(control);
				return;
			}
			string text = Content != null ? Content.ToString() : null;
			if(string.IsNullOrEmpty(text))
				text = "&nbsp;";
			SetText(text);
		}
		ControlCollection GetControlCollection() {
			if(!Item.IsFieldListItem)
				return Controls;
			Controls.Add(TransParentDiv);
			return TransParentDiv.Controls;
		}
		void SetText(string text) {
			if(!Item.IsFieldListItem)
				Text = text;
			else
				transparentDiv.Controls.Add(new LiteralControl(text));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderTextStyle(Field). AssignToControl(this);
			if(Item.IsFieldListItem) {
				Data.GetHeaderStyle(Field).AssignToControl(transparentDiv);
				transparentDiv.Style["background"] = "none transparent scroll repeat 0% 0%";
			}
		}
		object GetDefaultContent() {
			string text = Field.ToString();
			if(Field.Visible && Field.ShowSortButton && PivotGrid.IsAccessibilityCompliantRender()
					&& PivotGrid.IsEnabled() && !string.IsNullOrEmpty(text)) {
				HyperLink link = RenderUtils.CreateHyperLink();
				link.Text = text;
				link.NavigateUrl = ScriptHelper.GetAccessibleSortUrl(Field);
				RenderUtils.SetStringAttribute(link, "onmousedown", PivotGridWebData.CancelBubbleJs);
				return link;
			}
			return text;
		}
	}
	public class PivotGridHtmlHeaderSort : PivotGridHtmlHeaderCell {
		Image sortImage;
		public PivotGridHtmlHeaderSort(PivotGridHeaderTemplateItem item)
			: base(item.Data, item.FieldItem) {
			ID = ScriptHelper.GetHeaderSortCellID(Field);
		}
		protected Image SortImage { get { return sortImage; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.sortImage = RenderUtils.CreateImage();
			Controls.Add(sortImage);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderSortStyle(Field).AssignToControl(this);
			Field.SortImage.AssignToControl(sortImage, Data.IsDesignMode);
			if(Field.Area == PivotArea.RowArea && Field.Visible)
				Width = 1;
		}
	}
	public class PivotGridHtmlHeaderFilter : PivotGridHtmlHeaderCell {
		Image filterImage;
		HyperLink link;
		PivotGridHeaderTemplateItem item;
		public PivotGridHtmlHeaderFilter(PivotGridHeaderTemplateItem item)
			: base(item.Data, item.FieldItem) {
			ID = ScriptHelper.GetHeaderFilterCellID(Field);
			this.item = item;
		}
		protected Image FilterImage { get { return filterImage; } }
		protected HyperLink Link { get { return link; } }
		PivotGridHeaderTemplateItem Item { get { return item; } }
		protected override void CreateControlHierarchy() {
			this.filterImage = RenderUtils.CreateImage();
			if(Data.PivotGrid.IsAccessibilityCompliantRender() && Data.PivotGrid.IsEnabled() && Item.AllowFilter) {
				this.link = RenderUtils.CreateHyperLink();
				Controls.Add(Link);
				Link.Controls.Add(FilterImage);
			} else
				Controls.Add(FilterImage);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderFilterStyle(Field).AssignToControl(this);
			if(Item.FieldList != null && Item.FieldList.DeferredUpdates) {
				if(Field.DeferFilterButtonImage != null)
					Field.DeferFilterButtonImage.AssignToControl(FilterImage, Data.IsDesignMode);
			} else {
				if(Field.FilterButtonImage != null)
					Field.FilterButtonImage.AssignToControl(FilterImage, Data.IsDesignMode);
			}
			if(!Field.Visible) Width = Unit.Percentage(100);
			if(Data.PivotGrid.IsEnabled() && Item.AllowFilter) {
				if(Link != null) {
					Link.NavigateUrl = string.Format("javascript:{0}", ScriptHelper.GetFilterButtonOnClick(Item, Item.IsFieldListItem));
					FilterImage.CssClass = RenderUtils.CombineCssClasses(FilterImage.CssClass, "dxpg__hfb");
				} else {
					if(Field.Visible)
						FilterImage.CssClass = RenderUtils.CombineCssClasses(FilterImage.CssClass, "dxpg__hfb");
					RenderUtils.SetCursor(FilterImage, RenderUtils.GetDefaultCursor());
				}
			}
			if(Field.Area == PivotArea.RowArea && Field.Visible)
				Width = 1;
		}
	}
	public class PivotGridHtmlRowHeaderContainer : EmptyPivotGridHtmlRowHeaderContainer {
		PivotFieldItemBase field;
		PivotGridHtmlHeaderContent header;
		Image leftSeparator, rightSeparator;
		InternalTable containerTable;
		public PivotGridHtmlRowHeaderContainer(PivotGridWebData data, PivotFieldItemBase field, int rowSpan)
			: base(data, rowSpan) {
			this.field = field;
		}
		public PivotFieldItemBase Field {
			get { return field; }
		}
		public PivotGridHtmlHeaderContent Header {
			get { return header; }
		}
		public InternalTable ContainerTable {
			get {
				return containerTable;
			}
		}
		protected ASPxPivotGridRenderHelper RenderHelper {
			get { return Data.PivotGrid.RenderHelper; }
		}
		public Image LeftSeparator {
			get { return leftSeparator; }
		}
		public Image RightSeparator {
			get { return rightSeparator; }
		}
		public bool IsFirst {
			get {
				return Data.GetFieldByLevel(false, 0) == Data.GetField(Field);
			}
		}
		public bool IsLast {
			get {
				return Data.GetFieldByLevel(false, Data.GetFieldCountByArea(PivotArea.RowArea) - 1) == Data.GetField(Field);
			}
		}
		public bool RequiresRightGroupSeparator {
			get {
				PivotGroupItem group = Field.Group;
				return group != null &&
					Field.InnerGroupIndex != group.Fields.Count - 1 &&
					group.Fields[Field.InnerGroupIndex + 1].IsFieldVisibleInGroup;
			}
		}
		public bool RequiresLeftGroupSeparator {
			get { return Field.Group != null && Field.InnerGroupIndex != 0; }
		}
		protected override void CreateControlHierarchy() {
			CreateContainerTable();
			CreateLeftGroupSeparator();
			CreateHeader();
			CreateRightGroupSeparator();
		}
		protected void CreateContainerTable() {
			this.containerTable = new InternalTable();
			Controls.Add(ContainerTable);
			ContainerTable.Rows.Add(new InternalTableRow());
		}
		protected void CreateHeader() {
			this.header = new PivotGridHtmlHeaderContent(Data, Field, DefaultBoolean.True, null);
			AddControl(Header);
		}
		protected void CreateLeftGroupSeparator() {
			if(RequiresLeftGroupSeparator) {
				this.leftSeparator = new Image();
				AddControl(LeftSeparator);
			}
		}
		protected void CreateRightGroupSeparator() {
			if(RequiresRightGroupSeparator) {
				this.rightSeparator = new Image();
				AddControl(RightSeparator);
			}
		}
		protected void AddControl(WebControl control) {
			InternalTableCell cell = new InternalTableCell();
			cell.Controls.Add(control);
			ContainerTable.Rows[0].Cells.Add(cell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareContainerControl();
			if(LeftSeparator != null)
				PrepareSeparator(LeftSeparator);
			if(RightSeparator != null)
				PrepareSeparator(RightSeparator);
			SetPaddings();
		}
		private void PrepareContainerControl() {
			ContainerTable.CellPadding = 0;
			ContainerTable.CellSpacing = 0;
			ContainerTable.BorderStyle = BorderStyle.None;
			ContainerTable.Width = new Unit(100, UnitType.Percentage);
			if(RenderUtils.Browser.IsOpera) {
				RenderUtils.SetStyleStringAttribute(ContainerTable, "border-collapse", "separate"); 
				for(int i = 0; i < ContainerTable.Rows[0].Cells.Count; i++)
					ContainerTable.Rows[0].Cells[i].BorderStyle = BorderStyle.None;
			}
		}
		protected void PrepareSeparator(Image separator) {
			RenderHelper.GetGroupSeparatorImage().AssignToControl(separator, Data.IsDesignMode);
			Data.Styles.ApplyGroupSeparatorStyle(separator);
		}
		protected void SetPaddings() {
			Paddings paddings = Data.GetAreaPaddings(PivotArea.RowArea, IsFirst, IsLast);
			if(RequiresLeftGroupSeparator) {
				LeftSeparator.Width = paddings.PaddingLeft;
				((InternalTableCell)LeftSeparator.Parent).Width = LeftSeparator.Width;  
				LeftSeparator.Height = new Unit(1, UnitType.Pixel); 
				paddings.PaddingLeft = 0;
			}
			if(RequiresRightGroupSeparator) {
				RightSeparator.Width = paddings.PaddingRight;
				((InternalTableCell)RightSeparator.Parent).Width = RightSeparator.Width;  
				RightSeparator.Height = new Unit(1, UnitType.Pixel); 
				paddings.PaddingRight = 0;
			}
			RenderUtils.SetPaddings(this, paddings);
		}
	}
	public class EmptyPivotGridHtmlRowHeaderContainer : InternalTableCell {
		PivotGridWebData data;
		public EmptyPivotGridHtmlRowHeaderContainer(PivotGridWebData data, int rowSpan) {
			this.data = data;
			RowSpan = rowSpan;
		}
		public PivotGridWebData Data {
			get { return data; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetAreaStyle(PivotArea.RowArea).AssignToControl(this);
		}
	}
}
