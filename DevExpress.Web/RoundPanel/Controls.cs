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
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Linq.Expressions;
using System.Linq;
using Color = System.Drawing.Color;
namespace DevExpress.Web.Internal {
	public enum PanelPartType {
		TopLeftCorner,
		TopEdge,
		NoHeaderTopEdge,
		TopRightCorner,
		HeaderLeftEdge,
		HeaderContent,
		HeaderRightEdge,
		LeftEdge,
		Content,
		RightEdge,
		BottomLeftCorner,
		BottomEdge,
		BottomRightCorner,
		HeaderRow,
		ContentRow,
		ContentBottomRow,
		TopRow
	}
	public enum PanelPartRenderingMode { None, Html, Image }
	public class RPControlBase : ASPxInternalWebControl {
		public RPControlBase(ASPxRoundPanel panel) {
			RoundPanel = panel;
		}
		protected ASPxRoundPanel RoundPanel { get; private set; }
		protected bool IsRightToLeft {
			get {
				return (RoundPanel as ISkinOwner).IsRightToLeft();
			}
		}
	}
	public class RPGroupBoxCaptionControl : RPControlBase {
		protected WebControl MainElement { get; private set; }
		protected WebControl Span { get; private set; }
		protected HyperLink HyperLink { get; private set; }
		private readonly bool CanUseBorderRadius;
		public RPGroupBoxCaptionControl(ASPxRoundPanel panel, bool canUseBorderRadius)
			: this(panel) {
			CanUseBorderRadius = canUseBorderRadius;
		}
		public RPGroupBoxCaptionControl(ASPxRoundPanel panel)
			: base(panel) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainElement = null;
			Span = null;
			HyperLink = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			MainElement.ID = RoundPanel.GetGroupBoxCaptionID();
			Controls.Add(MainElement);
			ITemplate template = RoundPanel.HeaderTemplate;
			if(template != null) {
				RoundPanelHeaderTemplateContainer container = new RoundPanelHeaderTemplateContainer(RoundPanel);
				template.InstantiateIn(container);
				container.AddToHierarchy(MainElement, RoundPanel.GetHeaderTemplateContainerID());
			} else {
				Span = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				MainElement.Controls.Add(Span);
				HyperLink = RenderUtils.CreateHyperLink();
				Span.Controls.Add(HyperLink);
				HyperLink.ID = RoundPanel.GetHeaderTextContainerID();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareMainElement();
			PrepareSpan();
			PrepareHyperlink();
		}
		private AppearanceStyleBase GetSpanStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.AssignWithoutBorders(RoundPanel.GetHeaderStyle());
			style.BorderTop.Assign(RoundPanel.GetHeaderBorder(BorderType.Top, View.GroupBox));
			style.BorderBottom.Assign(RoundPanel.GetHeaderBorder(BorderType.Bottom, View.GroupBox));
			style.BorderLeft.Assign(RoundPanel.GetHeaderBorder(BorderType.Left, View.GroupBox));
			style.BorderRight.Assign(RoundPanel.GetHeaderBorder(BorderType.Right, View.GroupBox));
			return style;
		}
		private void PrepareHyperlink() {
			if(HyperLink != null) {
				RenderUtils.PrepareHyperLink(HyperLink, RoundPanel.HtmlEncode(RoundPanel.HeaderText),
					RoundPanel.HeaderNavigateUrl, RoundPanel.Target, RoundPanel.ToolTip, RoundPanel.IsEnabled());
				RoundPanel.GetHeaderLinkStyle().AssignToHyperLink(HyperLink);
			}
		}
		private void PrepareSpan() {
			if(Span != null) {
				AppearanceStyleBase style = GetSpanStyle();
				style.VerticalAlign = VerticalAlign.NotSet;
				style.AssignToControl(Span);
				RoundPanel.GetHeaderPaddings().AssignToControl(Span);
			}
		}
		private void PrepareMainElement() {
			AppearanceStyleBase headerStyle = RoundPanel.GetHeaderStyle();
			MainElement.Style.Add(HtmlTextWriterStyle.Position, "relative");
			MainElement.Style.Add(HtmlTextWriterStyle.TextAlign, IsRightToLeft ? "right" : "left");
			RenderUtils.SetStyleUnitAttribute(MainElement, "left", RoundPanel.GroupBoxCaptionOffsetX);
			RenderUtils.SetStyleUnitAttribute(MainElement, "top", RoundPanel.GroupBoxCaptionOffsetY);
			RenderUtils.SetVerticalMargins(MainElement, Unit.Empty, GetMarginBottom());
			headerStyle.AssignToControl(MainElement, AttributesRange.Font);
		}
		protected internal Unit GetExternalMarginTop() {
			return GetExternalMargin(RoundPanel.GroupBoxCaptionOffsetY, BorderType.Top);
		}
		protected internal Unit GetExternalMarginLeft() {
			return GetExternalMargin(RoundPanel.GroupBoxCaptionOffsetX, BorderType.Left);
		}
		private Unit GetExternalMargin(Unit offset, BorderType border) {
			if(offset.IsEmpty || offset.Type != UnitType.Pixel)
				return Unit.Empty;
			double borderWidthValue = 0;
			if(border == BorderType.Top || border == BorderType.Bottom)
				borderWidthValue = RoundPanel.GetHeaderBorder(border, View.GroupBox).BorderWidth.Value;
			return Unit.Pixel((int)Math.Abs(Math.Min(offset.Value - borderWidthValue, 0)));
		}
		private Unit GetMarginBottom() {
			if(RoundPanel.GroupBoxCaptionOffsetY.IsEmpty || RoundPanel.GroupBoxCaptionOffsetY.Type != UnitType.Pixel)
				return Unit.Empty;
			return Unit.Pixel((int)(RoundPanel.GroupBoxCaptionOffsetY.Value +
				RoundPanel.GetHeaderBorder(BorderType.Left, View.GroupBox).BorderWidth.Value));
		}
	}
	public class RPHeaderControl : RPControlBase {
		protected HyperLink HeaderHyperLink { get; private set; }
		protected WebControl TextSpan { get; private set; }
		protected Image Image { get; private set; }
		protected WebControl ContentWrapperDiv { get; private set; }
		protected WebControl TextSpanWrapper { get; private set; }
		internal DivButtonControl CollapseButton { get; private set; }
		protected WebControl Container {
			get {
				if(HeaderHyperLink != null)
					return HeaderHyperLink;
				if(ContentWrapperDiv != null)
					return ContentWrapperDiv;
				return this;
			}
		}
		public RPHeaderControl(ASPxRoundPanel panel)
			: base(panel) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentWrapperDiv = null;
			CollapseButton = null;
			TextSpan = null;
			Image = null;
			HeaderHyperLink = null;
			TextSpanWrapper = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(AddHeaderTemplate())
				return;
			if(!AddHeaderContentTemplate()) {
				CreateAndAppendContentWrapperDiv(RoundPanel.CanRenderCollapseButton());
				if(!string.IsNullOrEmpty(RoundPanel.HeaderNavigateUrl)) {
					HeaderHyperLink = RenderUtils.CreateHyperLink(true, true);
					(ContentWrapperDiv ?? this).Controls.Add(HeaderHyperLink);
				}
				if(!RoundPanel.HeaderImage.IsEmpty)
					CreateImage();
				CreateText();
			}
			CreateCollapseButton();
		}
		private void CreateAndAppendContentWrapperDiv(bool createCondition = true) {
			if(!createCondition)
				return;
			ContentWrapperDiv = RenderUtils.CreateDiv();
			Controls.Add(ContentWrapperDiv);
		}
		protected virtual bool AddHeaderTemplate() {
			ITemplate template = RoundPanel.HeaderTemplate;
			if(template != null) {
				RoundPanelHeaderTemplateContainer container = new RoundPanelHeaderTemplateContainer(RoundPanel);
				template.InstantiateIn(container);
				container.AddToHierarchy(this, RoundPanel.GetHeaderTemplateContainerID());
				return true;
			}
			return false;
		}
		protected virtual bool AddHeaderContentTemplate() {
			ITemplate template = RoundPanel.HeaderContentTemplate;
			if(template != null) {
				CreateAndAppendContentWrapperDiv();
				RoundPanelHeaderContentTemplateContainer container = new RoundPanelHeaderContentTemplateContainer(RoundPanel);
				template.InstantiateIn(container);
				container.AddToHierarchy(ContentWrapperDiv, RoundPanel.GetHeaderContentTemplateContainerID());
				return true;
			}
			return false;
		}
		protected virtual void CreateCollapseButton() {
			if(RoundPanel.CanRenderCollapseButton()) {
				CollapseButton = new DivButtonControl();
				Controls.AddAt(0, CollapseButton);
			}
		}
		protected virtual void CreateImage() {
			Image = RenderUtils.CreateImage();
			Container.Controls.Add(Image);
		}
		protected virtual void CreateText() {
			if(!RoundPanel.HeaderImage.IsEmpty && RoundPanel.HeaderStyle.Wrap == Utils.DefaultBoolean.True) {
				TextSpanWrapper = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				Container.Controls.Add(TextSpanWrapper);
			}
			TextSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			TextSpan.ID = RoundPanel.GetHeaderTextContainerID();
			(TextSpanWrapper ?? Container).Controls.Add(TextSpan);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(ContentWrapperDiv != null)
				RenderUtils.AppendDefaultDXClassName(ContentWrapperDiv, RoundPanel.GetHeaderContentWrapperCssClassName());
			if(HeaderHyperLink != null)
				PrepareHeaderHyperLink();
			if(TextSpan != null)
				PrepareText();
			if(Image != null)
				PrepareImage();
			if(CollapseButton != null)
				PrepareCollapseButton();
		}
		protected virtual void PrepareHeaderHyperLink() {
			RenderUtils.PrepareHyperLink(HeaderHyperLink, "", RoundPanel.HeaderNavigateUrl,
				RoundPanel.Target, "", RoundPanel.IsEnabled());
			RoundPanel.GetHeaderLinkStyle().AssignToHyperLink(HeaderHyperLink, true);
		}
		protected virtual void PrepareImage() {
			Image.CssClass = RoundPanel.GetHeaderImageCssClassName();
			RoundPanel.HeaderImage.AssignToControl(Image, DesignMode);
			if(IsRightToLeft)
				RenderUtils.SetHorizontalMargins(Image, RoundPanel.GetImageSpacing(), Unit.Empty);
			else
				RenderUtils.SetHorizontalMargins(Image, Unit.Empty, RoundPanel.GetImageSpacing());
			Image.AlternateText = RoundPanel.ToolTip;
			RenderUtils.AppendDefaultDXClassName(Image, RoundPanel.GetCssClassNamePrefix());
			RenderUtils.SetVerticalAlignClass(Image, RoundPanel.HeaderStyle.VerticalAlign);
		}
		protected virtual void PrepareText() {
			TextSpan.CssClass = RoundPanel.GetHeaderTextCssClassName();
			AppearanceStyleBase style = RoundPanel.GetHeaderStyle();
			style.AssignToControl(TextSpan, AttributesRange.Font);
			RenderUtils.SetWrap(TextSpan, style.Wrap, DesignMode);
			TextSpan.Controls.Add(RenderUtils.CreateLiteralControl(RoundPanel.HtmlEncode(RoundPanel.HeaderText)));
			RenderUtils.SetVerticalAlignClass(TextSpan, RoundPanel.HeaderStyle.VerticalAlign);
			if(TextSpanWrapper != null) {
				RenderUtils.SetVerticalAlignClass(TextSpanWrapper, RoundPanel.HeaderStyle.VerticalAlign);
				RenderUtils.SetStyleUnitAttribute(TextSpanWrapper, "width", Unit.Pixel(0));
			}
		}
		protected virtual void PrepareCollapseButton() {
			CollapseButton.ButtonID = RoundPanel.GetCollapseButtonId();
			CollapseButton.ButtonImageID = RoundPanel.GetCollapseButtonImageId();
			RenderUtils.AppendDefaultDXClassName(CollapseButton, RoundPanel.GetCollapseButtonCssClassName());
			CollapseButton.ButtonStyle = RoundPanel.GetCollapseButtonStyles();
			CollapseButton.ButtonPaddings = RoundPanel.GetCollapseButtonPaddings();
			CollapseButton.ButtonImage = RoundPanel.GetButtonImageProperties();
			if(!IsEnabled()) {
				CollapseButton.ButtonStyle.CopyFrom(RoundPanel.CollapseButtonStyle.GetDisabledStyle());
			}
		}
	}
	public class RPRenderingModeDictionary : Dictionary<PanelPartType, PanelPartRenderingMode> {
		protected ASPxRoundPanel RoundPanel { get; private set; }
		public RPRenderingModeDictionary(ASPxRoundPanel panel) {
			RoundPanel = panel;
		}
		public PanelPartRenderingMode GetPartRenderingMode(PanelPartType type) {
			if(!ContainsKey(type))
				Add(type, GetPartRenderingModeInternal(type));
			return this[type];
		}
		private PanelPartRenderingMode GetPartRenderingModeInternal(PanelPartType type) {
			switch(type) {
				case PanelPartType.TopLeftCorner:
					if(RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty) {
						if(RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty ||
							RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty)
							return PanelPartRenderingMode.None;
					} else
						return PanelPartRenderingMode.Image;
					break;
				case PanelPartType.TopRightCorner:
					if(RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty) {
						if(RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty ||
							RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty)
							return PanelPartRenderingMode.None;
					} else
						return PanelPartRenderingMode.Image;
					break;
				case PanelPartType.BottomLeftCorner:
					if(RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty) {
						if(RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty ||
							RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty)
							return PanelPartRenderingMode.None;
					} else
						return PanelPartRenderingMode.Image;
					break;
				case PanelPartType.BottomRightCorner:
					if(RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty) {
						if(RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty ||
							RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty)
							return PanelPartRenderingMode.None;
					} else
						return PanelPartRenderingMode.Image;
					break;
				case PanelPartType.LeftEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.RightEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.TopEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.BottomEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.HeaderLeftEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.TopLeftCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.BottomLeftCorner).IsEmpty ||
						GetPartRenderingMode(PanelPartType.HeaderContent) == PanelPartRenderingMode.None)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.HeaderContent:
					if(!RoundPanel.ShowHeader || RoundPanel.IsGroupBox)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.HeaderRightEdge:
					if(RoundPanel.GetCornerPart(PanelPartType.TopRightCorner).IsEmpty &&
						RoundPanel.GetCornerPart(PanelPartType.BottomRightCorner).IsEmpty ||
						GetPartRenderingMode(PanelPartType.HeaderContent) == PanelPartRenderingMode.None)
						return PanelPartRenderingMode.None;
					break;
				case PanelPartType.ContentBottomRow:
				case PanelPartType.Content:
				case PanelPartType.ContentRow:
					if((RoundPanel.DesignMode || !RoundPanel.Enabled) && RoundPanel.CanBeCollapsed() && RoundPanel.Collapsed)
						return PanelPartRenderingMode.None;
					break;
			}
			return PanelPartRenderingMode.Html;
		}
	}
	public class RPRoundPanelRenderingMode {
		protected RPRenderingModeDictionary Dictionary { get; private set; }
		public RPRoundPanelRenderingMode(ASPxRoundPanel panel) {
			Dictionary = new RPRenderingModeDictionary(panel);
		}
		public PanelPartRenderingMode this[PanelPartType type] {
			get {
				return Dictionary.GetPartRenderingMode(type);
			}
		}
		public void Clear() {
			Dictionary.Clear();
		}
	}
	public class RPRoundPanelControl : RPControlBase {
		protected Table TableContainer { get; private set; }
		protected TableCell CellTopLeft { get; private set; }
		protected TableCell CellMiddleTop { get; private set; }
		protected TableCell CellTopRight { get; private set; }
		protected TableCell CellHeaderLeft { get; private set; }
		protected TableCell CellHeaderContent { get; private set; }
		protected TableCell CellHeaderRight { get; private set; }
		protected TableCell CellMiddleLeft { get; private set; }
		protected TableCell CellContent { get; private set; }
		protected TableCell CellMiddleRight { get; private set; }
		protected TableCell CellBottomLeft { get; private set; }
		protected TableCell CellMiddleBottom { get; private set; }
		protected TableCell CellBottomRight { get; private set; }
		protected TableCell CellHeaderSeparator { get; private set; }
		protected TableRow HeaderSeparatorRow { get; private set; }
		protected TableRow TopRow { get; private set; }
		protected TableRow HeaderRow { get; private set; }
		protected TableRow ContentRow { get; private set; }
		protected TableRow ContentBottomRow { get; private set; }
		protected RPGroupBoxCaptionControl GroupBoxCaptionControl { get; private set; }
		protected RPHeaderControl HeaderControl { get; private set; }
		protected RPRoundPanelRenderingMode RenderingMode { get; private set; }
		protected WebControl AnimationWrapper { get; private set; }
		protected WebControl ContentWrapper { get; private set; }
		protected WebControl ContentContainer {
			get {
				return ContentWrapper ?? CellContent;
			}
		}
		protected internal DivButtonControl GetCollapseButtonControl() {
			if(HeaderControl != null)
				return HeaderControl.CollapseButton;
			return null;
		}
		public RPRoundPanelControl(ASPxRoundPanel panel)
			: base(panel) {
			RenderingMode = new RPRoundPanelRenderingMode(RoundPanel);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			HeaderSeparatorRow = null;
			CellHeaderSeparator = null;
			TableContainer = null;
			AnimationWrapper = null;
			ContentWrapper = null;
			CellTopLeft = null;
			CellMiddleTop = null;
			CellTopRight = null;
			CellHeaderLeft = null;
			CellHeaderContent = null;
			CellHeaderRight = null;
			CellMiddleLeft = null;
			CellContent = null;
			CellMiddleRight = null;
			CellBottomLeft = null;
			CellMiddleBottom = null;
			CellBottomRight = null;
			TopRow = null;
			HeaderRow = null;
			ContentRow = null;
			ContentBottomRow = null;
			GroupBoxCaptionControl = null;
			HeaderControl = null;
		}
		protected virtual void AddTableRow(TableRow row, PanelPartType type) {
			if(RenderingMode[type] != PanelPartRenderingMode.None)
				AddTableRow(row);
		}
		protected virtual void AddTableRow(TableRow row) {
			if(row != null && row.Cells.Count > 0)
				TableContainer.Rows.Add(row);
		}
		protected virtual void AddTableCell(TableCell cell, TableRow row, PanelPartType type) {
			if(cell != null && row != null && RenderingMode[type] != PanelPartRenderingMode.None)
				row.Cells.Add(cell);
		}
		protected void CreateContentContainerHierarchy() {
			CellContent = RenderUtils.CreateTableCell();
			if(IsAnimationWrapperNeeded()) {
				AnimationWrapper = RenderUtils.CreateDiv();
				CellContent.Controls.Add(AnimationWrapper);
			}
			if(IsContentWrapperNeeded()) {
				ContentWrapper = RenderUtils.CreateDiv();
				WebControl container = (WebControl)(AnimationWrapper ?? CellContent);
				container.Controls.Add(ContentWrapper);
			}
		}
		protected void CreateHeaderSeparatorHierarchy() {
			if(IsRenderModeNone(PanelPartType.HeaderContent) ||
			   !IsRenderWithoutCornerImages() || RoundPanel.GetHeaderBorder(BorderType.Bottom, View.Standard).IsEmpty)
				return;
			CellHeaderSeparator = RenderUtils.CreateTableCell();
			HeaderSeparatorRow = RenderUtils.CreateTableRow();
			HeaderSeparatorRow.Controls.Add(CellHeaderSeparator);
		}
		protected bool IsContentWrapperNeeded() {
			return !DesignMode && RoundPanel.IsCollapsingAvailable();
		}
		protected bool IsAnimationWrapperNeeded() {
			return IsContentWrapperNeeded() &&
				(RoundPanel.EnableAnimation || (RenderUtils.Browser.IsFirefox && !RoundPanel.IsClientSideAPIEnabled()));
		}
		protected bool CanUseBorderRadius() {
			return !(Browser.IsIE && Browser.Version < 9) && !RoundPanel.CornerRadius.IsEmpty && IsRenderWithoutCornerImages();
		}
		private bool IsRenderWithoutCornerImages() {
			return IsRenderModeNone(PanelPartType.BottomLeftCorner, PanelPartType.BottomRightCorner,
				PanelPartType.TopLeftCorner, PanelPartType.TopRightCorner);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			RoundPanel.UpdatePanelContentVisibility();
			RenderingMode.Clear();
			TableContainer = RenderUtils.CreateTable(true);
			Controls.Add(TableContainer);
			CreateContentContainerHierarchy();
			CreateHeaderSeparatorHierarchy();
			CellTopLeft = RenderUtils.CreateTableCell();
			CellMiddleTop = RenderUtils.CreateTableCell();
			CellTopRight = RenderUtils.CreateTableCell();
			CellMiddleRight = RenderUtils.CreateTableCell();
			CellBottomRight = RenderUtils.CreateTableCell();
			CellMiddleBottom = RenderUtils.CreateTableCell();
			CellBottomLeft = RenderUtils.CreateTableCell();
			CellMiddleLeft = RenderUtils.CreateTableCell();
			CellHeaderLeft = RenderUtils.CreateTableCell();
			CellHeaderContent = RenderUtils.CreateTableCell();
			CellHeaderRight = RenderUtils.CreateTableCell();
			TopRow = RenderUtils.CreateTableRow();
			HeaderRow = RenderUtils.CreateTableRow();
			ContentRow = RenderUtils.CreateTableRow();
			ContentBottomRow = RenderUtils.CreateTableRow();
			AddTableCell(CellTopLeft, TopRow, PanelPartType.TopLeftCorner);
			AddTableCell(CellMiddleTop, TopRow, PanelPartType.TopEdge);
			AddTableCell(CellTopRight, TopRow, PanelPartType.TopRightCorner);
			AddTableCell(CellHeaderLeft, HeaderRow, PanelPartType.HeaderLeftEdge);
			AddTableCell(CellHeaderContent, HeaderRow, PanelPartType.HeaderContent);
			AddTableCell(CellHeaderRight, HeaderRow, PanelPartType.HeaderRightEdge);
			AddTableCell(CellMiddleLeft, ContentRow, PanelPartType.LeftEdge);
			AddTableCell(CellContent, ContentRow, PanelPartType.Content);
			AddTableCell(CellMiddleRight, ContentRow, PanelPartType.RightEdge);
			AddTableCell(CellBottomLeft, ContentBottomRow, PanelPartType.BottomLeftCorner);
			AddTableCell(CellMiddleBottom, ContentBottomRow, PanelPartType.BottomEdge);
			AddTableCell(CellBottomRight, ContentBottomRow, PanelPartType.BottomRightCorner);
			AddTableRow(TopRow, PanelPartType.TopRow);
			AddTableRow(HeaderRow, PanelPartType.HeaderRow);
			AddTableRow(HeaderSeparatorRow);
			AddTableRow(ContentRow, PanelPartType.ContentRow);
			AddTableRow(ContentBottomRow, PanelPartType.ContentBottomRow);
			CreateCornerImages();
			if(RoundPanel.ShowHeader && RoundPanel.IsGroupBox) {
				GroupBoxCaptionControl = new RPGroupBoxCaptionControl(RoundPanel, CanUseBorderRadius());
				CellContent.Controls.AddAt(0, GroupBoxCaptionControl);
			}
			ContentContainer.ID = RoundPanel.GetCallbackResultControlId();
			ContentContainer.Controls.Add(RoundPanel.PanelContent);
			if(RoundPanel.ShowHeader && !RoundPanel.IsGroupBox) {
				HeaderControl = new RPHeaderControl(RoundPanel);
				CellHeaderContent.Controls.Add(HeaderControl);
			}
		}
		private void CreateCornerImage(TableCell cell, PanelCornerPart props, bool imageIsResource) {
			if(!props.IsEmpty) {
				Image image = RenderUtils.CreateImage();
				cell.Controls.Add(image);
				props.AssignToControl(image, DesignMode, imageIsResource && !DesignMode);
				if(string.IsNullOrEmpty(image.CssClass))
					image.CssClass = RoundPanel.GetCustomCornerImageCssClassName();
				image.AlternateText = RoundPanel.ToolTip;
			}
		}
		private void CreateCornerImages() {
			bool isCornerImageResource;
			CreateCornerImage(CellTopLeft,
				RoundPanel.GetCornerPart(IsRightToLeft ? PanelPartType.TopRightCorner : PanelPartType.TopLeftCorner, out isCornerImageResource),
					isCornerImageResource);
			CreateCornerImage(CellTopRight,
				RoundPanel.GetCornerPart(IsRightToLeft ? PanelPartType.TopLeftCorner : PanelPartType.TopRightCorner, out isCornerImageResource),
					isCornerImageResource);
			CreateCornerImage(CellBottomLeft,
				RoundPanel.GetCornerPart(IsRightToLeft ? PanelPartType.BottomRightCorner : PanelPartType.BottomLeftCorner, out isCornerImageResource),
					isCornerImageResource);
			CreateCornerImage(CellBottomRight,
				RoundPanel.GetCornerPart(IsRightToLeft ? PanelPartType.BottomLeftCorner : PanelPartType.BottomRightCorner, out isCornerImageResource),
					isCornerImageResource);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			short storedTabIndex = RoundPanel.TabIndex;
			RoundPanel.TabIndex = 0;
			RenderingMode.Clear();
			RenderUtils.AssignAttributes(RoundPanel, TableContainer);
			RenderUtils.SetVisibility(TableContainer, RoundPanel.IsClientVisible(), true);
			if(!RoundPanel.ContentHeight.IsEmpty)
				TableContainer.Height = Unit.Empty;
			if(RoundPanel.GetControlStyle().CssClass != "")
				RenderUtils.AppendDefaultDXClassName(TableContainer, RoundPanel.GetControlStyle().CssClass);
			if(!IsRenderWithoutCornerImages())
				RenderUtils.AppendDefaultDXClassName(TableContainer, RoundPanel.GetHasDefaultImagesCssClassName());
			if(!IsContentWrapperNeeded())
				RenderUtils.AppendDefaultDXClassName(TableContainer, RoundPanel.GetHasNotCollapsingCssClassName());
			if(GroupBoxCaptionControl != null) {
				Unit marginTop = GroupBoxCaptionControl.GetExternalMarginTop();
				Unit marginLeft = GroupBoxCaptionControl.GetExternalMarginLeft();
				if(!marginTop.IsEmpty)
					RenderUtils.SetStyleUnitAttribute(TableContainer, "margin-top", marginTop);
				if(!marginLeft.IsEmpty)
					RenderUtils.SetStyleUnitAttribute(TableContainer, "margin-left", marginLeft);
			}
			RoundPanel.BackgroundImage.AssignToControl(TableContainer);
			PrepareCornerPart(CellTopLeft, IsRightToLeft ? PanelPartType.TopRightCorner : PanelPartType.TopLeftCorner);
			PrepareCornerPart(CellTopRight, IsRightToLeft ? PanelPartType.TopLeftCorner : PanelPartType.TopRightCorner);
			PrepareCornerPart(CellBottomLeft, IsRightToLeft ? PanelPartType.BottomRightCorner : PanelPartType.BottomLeftCorner);
			PrepareCornerPart(CellBottomRight, IsRightToLeft ? PanelPartType.BottomLeftCorner : PanelPartType.BottomRightCorner);
			PrepareMiddlePart(CellMiddleLeft, IsRightToLeft ? PanelPartType.RightEdge : PanelPartType.LeftEdge);
			PrepareMiddlePart(CellMiddleRight, IsRightToLeft ? PanelPartType.LeftEdge : PanelPartType.RightEdge);
			PrepareMiddlePart(CellMiddleTop, PanelPartType.TopEdge);
			PrepareMiddlePart(CellMiddleBottom, PanelPartType.BottomEdge);
			PrepareHeaderLeftRightPart(CellHeaderLeft, IsRightToLeft ? PanelPartType.HeaderRightEdge : PanelPartType.HeaderLeftEdge);
			PrepareHeaderMiddlePart();
			PrepareHeaderLeftRightPart(CellHeaderRight, IsRightToLeft ? PanelPartType.HeaderLeftEdge : PanelPartType.HeaderRightEdge);
			PrepareHeaderSeparator();
			PrepareContentPart();
			PrepareAnimationWrapper();
			ApplyBorderRadius();
			if(RoundPanel.IsDefaultButtonAssigned() && RoundPanel.IsEnabled()) {
				CellContent.TabIndex = storedTabIndex;
				RoundPanel.AddDefaultButtonScript(CellContent);
			}
			if(RoundPanel.Collapsed && RoundPanel.CanBeCollapsed())
				RenderUtils.AppendDefaultDXClassName(TableContainer, RoundPanel.GetCollapsedCssClassName());
		}
		protected void ApplyBorderRadius() {
			if(!CanUseBorderRadius())
				return;
			Unit cornerRadius = RoundPanel.CornerRadius;
			SetBorderRadius(TableContainer, cornerRadius);
			if(IsRenderModeNone(PanelPartType.HeaderContent))
				SetBorderRadius(CellContent, cornerRadius);
			else if(IsRenderModeNone(PanelPartType.Content))
				SetBorderRadius(CellHeaderContent, cornerRadius);
			else {
				SetBorderRadius(CellHeaderContent, cornerRadius, Unit.Empty);
				SetBorderRadius(CellContent, Unit.Empty, cornerRadius);
			}
		}
		protected void PrepareAnimationWrapper() {
			bool useCustomDisplayValue = RenderUtils.Browser.IsFirefox && !RoundPanel.IsClientSideAPIEnabled(); 
			PrepareWrapper(AnimationWrapper, RoundPanel.GetAnimationWrapperCssClassName(), useCustomDisplayValue ? "table" : "");
			PrepareWrapper(ContentWrapper, RoundPanel.GetContentWrapperCssClassName(), useCustomDisplayValue ? "table-cell" : "");
		}
		void PrepareWrapper(WebControl wrapper, string cssClassName, string displayValue) {
			if(wrapper != null) {
				RenderUtils.AppendDefaultDXClassName(wrapper, cssClassName);
				if(!string.IsNullOrEmpty(displayValue))
					RenderUtils.SetStyleStringAttribute(wrapper, "display", displayValue);
			}
		}
		private void PrepareCornerPart(TableCell cell, PanelPartType type) {
			PanelPartRenderingMode mode = RenderingMode[type];
			if(mode == PanelPartRenderingMode.None)
				return;
			if(mode == PanelPartRenderingMode.Html) {
				AppearanceStyle cornerStyle = new AppearanceStyle();
				System.Drawing.Color headerBackColor = RoundPanel.GetHeaderStyle().BackColor;
				System.Drawing.Color controlBackColor = RoundPanel.GetControlStyle().BackColor;
				if(!controlBackColor.IsEmpty)
					cornerStyle.BackColor = controlBackColor;
				switch(type) {
					case PanelPartType.TopLeftCorner:
						cornerStyle.BorderLeft.CopyFrom(RoundPanel.GetBorder(BorderType.Left));
						cornerStyle.BorderTop.CopyFrom(RoundPanel.GetBorder(BorderType.Top));
						if((RenderingMode[PanelPartType.HeaderLeftEdge] != PanelPartRenderingMode.None) &&
							!headerBackColor.IsEmpty)
							cornerStyle.BackColor = headerBackColor;
						break;
					case PanelPartType.TopRightCorner:
						cornerStyle.BorderTop.CopyFrom(RoundPanel.GetBorder(BorderType.Top));
						cornerStyle.BorderRight.CopyFrom(RoundPanel.GetBorder(BorderType.Right));
						if((RenderingMode[PanelPartType.HeaderRightEdge] != PanelPartRenderingMode.None) &&
							!headerBackColor.IsEmpty)
							cornerStyle.BackColor = headerBackColor;
						break;
					case PanelPartType.BottomLeftCorner:
						cornerStyle.BorderLeft.CopyFrom(RoundPanel.GetBorder(BorderType.Left));
						cornerStyle.BorderBottom.CopyFrom(RoundPanel.GetBorder(BorderType.Bottom));
						break;
					case PanelPartType.BottomRightCorner:
						cornerStyle.BorderRight.CopyFrom(RoundPanel.GetBorder(BorderType.Right));
						cornerStyle.BorderBottom.CopyFrom(RoundPanel.GetBorder(BorderType.Bottom));
						break;
					default:
						throw new ArgumentException("RPRoundPanelControl: corner part type must be one of PanelPartType.TopLeft, PanelPartType.TopRight, PanelPartType.BottomLeft, PanelPartType.BottomRight");
				}
				cornerStyle.AssignToControl(cell);
			} else if(mode == PanelPartRenderingMode.Image) {
				ImageProperties cornerImage;
				switch(type) {
					case PanelPartType.TopLeftCorner:
					case PanelPartType.TopRightCorner:
					case PanelPartType.BottomLeftCorner:
					case PanelPartType.BottomRightCorner:
						cornerImage = RoundPanel.GetCornerPart(type);
						break;
					default:
						throw new ArgumentException("RPRoundPanelControl: corner part type must be one of PanelPartType.TopLeft, PanelPartType.TopRight, PanelPartType.BottomLeft, PanelPartType.BottomRight");
				}
				if(!cornerImage.Width.IsEmpty)
					cell.Style.Add(HtmlTextWriterStyle.Width, cornerImage.Width.ToString());
				if(!cornerImage.Height.IsEmpty)
					cell.Style.Add(HtmlTextWriterStyle.Height, cornerImage.Height.ToString());
			}
			if(RoundPanel.Cursor != "")
				cell.Style.Add(HtmlTextWriterStyle.Cursor, RoundPanel.Cursor.ToLower());
		}
		private void PrepareMiddlePart(TableCell cell, PanelPartType type) {
			PanelPartRenderingMode mode = RenderingMode[type];
			AppearanceStyleBase controlStyle = RoundPanel.GetControlStyle();
			if(mode == PanelPartRenderingMode.None)
				return;
			else if(mode == PanelPartRenderingMode.Html) {
				AppearanceStyle middleStyle = new AppearanceStyle();
				middleStyle.CssClass = RoundPanel.GetCssClassName(type);
				HeaderStyle headerStyle = RoundPanel.GetHeaderStyle();
				if(!controlStyle.BackColor.IsEmpty)
					middleStyle.BackColor = controlStyle.BackColor;
				switch(type) {
					case PanelPartType.LeftEdge:
						if(!RoundPanel.Parts.LeftEdgeInternal.BackColor.IsEmpty)
							middleStyle.BackColor = RoundPanel.Parts.LeftEdgeInternal.BackColor;
						if(!RoundPanel.Parts.LeftEdgeInternal.BackgroundImage.IsEmpty)
							middleStyle.BackgroundImage.CopyFrom(RoundPanel.Parts.LeftEdgeInternal.BackgroundImage);
						middleStyle.BorderLeft.CopyFrom(RoundPanel.GetBorder(BorderType.Left));
						if(IsRenderModeNone(PanelPartType.BottomEdge))
							middleStyle.BorderBottom.CopyFrom(RoundPanel.GetBorder(BorderType.Bottom));
						if(IsRenderModeNone(PanelPartType.TopEdge, PanelPartType.HeaderContent))
							SetBorder(cell, RoundPanel.GetHeaderBorder(BorderType.Top, View.Standard), BorderType.Top);
						break;
					case PanelPartType.RightEdge:
						if(!RoundPanel.Parts.RightEdgeInternal.BackColor.IsEmpty)
							middleStyle.BackColor = RoundPanel.Parts.RightEdgeInternal.BackColor;
						if(!RoundPanel.Parts.RightEdgeInternal.BackgroundImage.IsEmpty)
							middleStyle.BackgroundImage.CopyFrom(RoundPanel.Parts.RightEdgeInternal.BackgroundImage);
						middleStyle.BorderRight.CopyFrom(RoundPanel.GetBorder(BorderType.Right));
						if(IsRenderModeNone(PanelPartType.BottomEdge))
							middleStyle.BorderBottom.CopyFrom(RoundPanel.GetBorder(BorderType.Bottom));
						if(IsRenderModeNone(PanelPartType.TopEdge, PanelPartType.HeaderContent))
							SetBorder(cell, RoundPanel.GetHeaderBorder(BorderType.Top, View.Standard), BorderType.Top);
						break;
					case PanelPartType.TopEdge:
						if(!headerStyle.BackColor.IsEmpty && RoundPanel.ShowHeader && !RoundPanel.IsGroupBox)
							middleStyle.BackColor = headerStyle.BackColor;
						PanelPart part = RoundPanel.Parts.TopEdgeInternal;
						if(!RoundPanel.ShowHeader || (RoundPanel.ShowHeader && RoundPanel.IsGroupBox)) {
							middleStyle.CssClass = RoundPanel.GetCssClassName(PanelPartType.NoHeaderTopEdge);
							if(!RoundPanel.Parts.NoHeaderTopEdgeInternal.IsEmpty)
								part = RoundPanel.Parts.NoHeaderTopEdgeInternal;
						}
						if(!part.BackColor.IsEmpty)
							middleStyle.BackColor = part.BackColor;
						if(!part.BackgroundImage.IsEmpty)
							middleStyle.BackgroundImage.CopyFrom(part.BackgroundImage);
						if(RenderingMode[PanelPartType.HeaderContent] != PanelPartRenderingMode.None)
							middleStyle.BorderTop.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Top, View.Standard));
						else
							middleStyle.BorderTop.CopyFrom(RoundPanel.GetBorder(BorderType.Top));
						if(IsRenderModeNone(PanelPartType.LeftEdge))
							middleStyle.BorderLeft.CopyFrom(RoundPanel.GetBorder(BorderType.Left));
						if(IsRenderModeNone(PanelPartType.RightEdge))
							middleStyle.BorderRight.CopyFrom(RoundPanel.GetBorder(BorderType.Right));
						break;
					case PanelPartType.BottomEdge:
						if(!RoundPanel.Parts.BottomEdgeInternal.BackColor.IsEmpty)
							middleStyle.BackColor = RoundPanel.Parts.BottomEdgeInternal.BackColor;
						if(!RoundPanel.Parts.BottomEdgeInternal.BackgroundImage.IsEmpty)
							middleStyle.BackgroundImage.CopyFrom(RoundPanel.Parts.BottomEdgeInternal.BackgroundImage);
						middleStyle.BorderBottom.CopyFrom(RoundPanel.GetBorder(BorderType.Bottom));
						if(IsRenderModeNone(PanelPartType.LeftEdge))
							middleStyle.BorderLeft.CopyFrom(RoundPanel.GetBorder(BorderType.Left));
						if(IsRenderModeNone(PanelPartType.RightEdge))
							middleStyle.BorderRight.CopyFrom(RoundPanel.GetBorder(BorderType.Right));
						break;
					default:
						throw new ArgumentException("RPRoundPanelControl: middle part type must be one of PanelPartType.MiddleLeft, PanelPartType.MiddleRight, PanelPartType.MiddleTop, PanelPartType.MiddleBottom, PanelPartType.HeaderBottomEdge");
				}
				middleStyle.AssignToControl(cell);
			}
			if(controlStyle.Cursor != "")
				cell.Style.Add(HtmlTextWriterStyle.Cursor, controlStyle.Cursor.ToLower());
		}
		private void PrepareContentPart() {
			if(ContentRow != null)
				RenderUtils.AppendDefaultDXClassName(ContentRow, RoundPanel.GetContentRowCssClassName());
			if(ContentBottomRow != null)
				RenderUtils.AppendDefaultDXClassName(ContentBottomRow, RoundPanel.GetContentBottomRowCssClassName());
			CellContent.ID = RoundPanel.GetRoundPanelContentID();
			AppearanceStyleBase controlStyle = RoundPanel.GetControlStyle();
			controlStyle.BackgroundImage.Assign(new BackgroundImage());
			controlStyle.AssignToControl(CellContent, AttributesRange.Cell | AttributesRange.Font, false, true);
			if(!RoundPanel.Width.IsEmpty)
				CellContent.Width = Unit.Percentage(100);
			if(controlStyle.Cursor != "")
				CellContent.Style.Add(HtmlTextWriterStyle.Cursor, controlStyle.Cursor.ToLower());
			if(!RoundPanel.Content.BackgroundImage.IsEmpty)
				RoundPanel.Content.BackgroundImage.AssignToControl(CellContent);
			else
				controlStyle.BackgroundImage.AssignToControl(CellContent);
			if(!RoundPanel.Content.BackColor.IsEmpty)
				CellContent.BackColor = RoundPanel.Content.BackColor;
			else
				CellContent.BackColor = controlStyle.BackColor;
			RenderUtils.AppendDefaultDXClassName(CellContent, RoundPanel.GetContentCellCssClassName());
			RenderUtils.SetPaddings(ContentContainer, RoundPanel.GetContentPaddings());
			if(IsRenderModeNone(PanelPartType.TopEdge, PanelPartType.HeaderContent))
				SetBorder(CellContent, RoundPanel.GetBorder(BorderType.Top), BorderType.Top);
			if(IsRenderModeNone(PanelPartType.BottomEdge))
				SetBorder(CellContent, RoundPanel.GetBorder(BorderType.Bottom), BorderType.Bottom);
			if(IsRenderModeNone(PanelPartType.LeftEdge))
				SetBorder(CellContent, RoundPanel.GetBorder(BorderType.Left), BorderType.Left);
			if(IsRenderModeNone(PanelPartType.RightEdge))
				SetBorder(CellContent, RoundPanel.GetBorder(BorderType.Right), BorderType.Right);
			controlStyle.Border.Reset();
			PrepareContentContainerHeight(controlStyle);
		}
		private void PrepareContentContainerHeight(AppearanceStyleBase controlStyle) {
			Unit height = GetHeight(RoundPanel.ContentHeight, controlStyle, RoundPanel.GetContentPaddings());
			RenderUtils.SetBorderBox(CellContent);
			RenderUtils.SetStyleUnitAttribute(CellContent, "height", height);
			if(ContentWrapper != null)
				RenderUtils.SetBorderBox(ContentWrapper);
		}
		private Unit GetHeight(Unit height, AppearanceStyleBase style, Paddings paddings) {
			return height;
		}
		private void PrepareHeaderLeftRightPart(TableCell cell, PanelPartType type) {
			PanelPartRenderingMode mode = RenderingMode[type];
			if(mode == PanelPartRenderingMode.None)
				return;
			else if(mode == PanelPartRenderingMode.Html) {
				AppearanceStyle style = new AppearanceStyle();
				style.CssClass = RoundPanel.GetCssClassName(type);
				AppearanceStyle headerStyle = RoundPanel.GetHeaderStyle();
				if(!headerStyle.BackColor.IsEmpty)
					style.BackColor = headerStyle.BackColor;
				switch(type) {
					case PanelPartType.HeaderLeftEdge:
						if(!RoundPanel.Parts.HeaderLeftEdgeInternal.BackColor.IsEmpty)
							style.BackColor = RoundPanel.Parts.HeaderLeftEdgeInternal.BackColor;
						if(!RoundPanel.Parts.HeaderLeftEdgeInternal.BackgroundImage.IsEmpty)
							style.BackgroundImage.CopyFrom(RoundPanel.Parts.HeaderLeftEdgeInternal.BackgroundImage);
						style.BorderLeft.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Left, View.Standard));
						break;
					case PanelPartType.HeaderRightEdge:
						if(!RoundPanel.Parts.HeaderRightEdgeInternal.BackColor.IsEmpty)
							style.BackColor = RoundPanel.Parts.HeaderRightEdgeInternal.BackColor;
						if(!RoundPanel.Parts.HeaderRightEdgeInternal.BackgroundImage.IsEmpty)
							style.BackgroundImage.CopyFrom(RoundPanel.Parts.HeaderRightEdgeInternal.BackgroundImage);
						style.BorderRight.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Right, View.Standard));
						break;
					default:
						throw new ArgumentException("RPRoundPanelControl: header part type must be one of PanelPartType.HeaderLeft, PanelPartType.HeaderMiddle, PanelPartType.HeaderRight.");
				}
				style.BorderBottom.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Bottom, View.Standard));
				style.AssignToControl(cell);
				if(IsRenderModeNone(PanelPartType.TopEdge))
					SetBorder(cell, RoundPanel.GetHeaderBorder(BorderType.Top, View.Standard), BorderType.Top);
			}
		}
		private void PrepareHeaderMiddlePart() {
			if(RenderingMode[PanelPartType.HeaderContent] == PanelPartRenderingMode.Html) {
				HeaderStyle headerStyle = RoundPanel.GetHeaderStyle();
				Paddings headerPaddings = RoundPanel.GetHeaderPaddings();
				HeaderStyle unborderedHeaderStyle = new HeaderStyle();
				unborderedHeaderStyle.AssignWithoutBorders(headerStyle);
				if(IsRenderModeNone(PanelPartType.TopEdge))
					unborderedHeaderStyle.BorderTop.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Top, View.Standard));
				if(IsRenderModeNone(PanelPartType.LeftEdge))
					unborderedHeaderStyle.BorderLeft.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Left, View.Standard));
				if(IsRenderModeNone(PanelPartType.RightEdge))
					unborderedHeaderStyle.BorderRight.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Right, View.Standard));
				unborderedHeaderStyle.BorderBottom.CopyFrom(RoundPanel.GetHeaderBorder(BorderType.Bottom, View.Standard));
				if(!unborderedHeaderStyle.Height.IsEmpty)
					unborderedHeaderStyle.Height = GetHeight(unborderedHeaderStyle.Height, headerStyle, headerPaddings);
				unborderedHeaderStyle.AssignToControl(CellHeaderContent);
				CellHeaderContent.Height = unborderedHeaderStyle.Height;
				CellHeaderContent.ID = RoundPanel.GetHeaderElementContainerId();
				RenderUtils.SetPaddings(CellHeaderContent, headerPaddings);
				if(!RoundPanel.HeaderContent.BackgroundImage.IsEmpty)
					RoundPanel.HeaderContent.BackgroundImage.AssignToControl(CellHeaderContent);
				if(!RoundPanel.HeaderContent.BackColor.IsEmpty)
					CellHeaderContent.BackColor = RoundPanel.HeaderContent.BackColor;
				if(RoundPanel.IsCollapsingAvailable() && RoundPanel.AllowCollapsingByHeaderClick)
					RenderUtils.AppendDefaultDXClassName(CellHeaderContent, "dxrp-headerClickable");
				RenderUtils.SetBorderBox(CellHeaderContent);
			}
		}
		protected void PrepareHeaderSeparator() {
			if(CellHeaderSeparator == null)
				return;
			RenderUtils.AppendDefaultDXClassName(CellHeaderSeparator, RoundPanel.GetHeaderSeparatorCssClassName());
			BorderBottom bottomBorder = new BorderBottom();
			bottomBorder.Assign(RoundPanel.GetHeaderBorder(BorderType.Bottom, View.Standard));
			bottomBorder.AssignToControl(CellHeaderSeparator);
			(new BorderBottom(Color.Empty, BorderStyle.None, 0)).AssignToControl(CellHeaderContent);
			if(!RoundPanel.HeaderContent.BackColor.IsEmpty)
				CellHeaderSeparator.BackColor = RoundPanel.HeaderContent.BackColor;
		}
		private void SetBorder(WebControl control, Border border, BorderType type) {
			if(!border.IsEmpty) {
				string attributeName = string.Format("border-{0}", type.ToString().ToLower());
				control.Style.Remove(attributeName);
				control.Style.Add(attributeName, border.ToString().ToLower());
			}
		}
		private void SetBorderRadius(WebControl control, Unit topLeftRight, Unit bottomRightLeft) {
			string topLeftRightValue = topLeftRight.IsEmpty ? "0px" : topLeftRight.ToString();
			string bottomLeftRightValue = bottomRightLeft.IsEmpty ? "0px" : bottomRightLeft.ToString();
			SetBorderRadius(control, string.Format("{0} {0} {1} {1}", topLeftRightValue, bottomLeftRightValue));
		}
		private void SetBorderRadius(WebControl control, Unit radius) {
			SetBorderRadius(control, radius.ToString());
		}
		private void SetBorderRadius(WebControl control, string value) {
			if(control != null)
				RenderUtils.SetStyleStringAttribute(control, "border-radius", value);
		}
		private bool IsRenderModeNone(params PanelPartType[] parts) {
			if(parts != null)
				return parts.All(part => RenderingMode[part] == PanelPartRenderingMode.None);
			return true;
		}
	}
}
