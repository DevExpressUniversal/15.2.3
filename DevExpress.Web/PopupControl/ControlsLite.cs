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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class PCControlBase : ASPxInternalWebControl {
		private ASPxPopupControlBase fPopupControl = null;
		public ASPxPopupControlBase PopupControl {
			get { return fPopupControl; }
		}
		public PCControlBase(ASPxPopupControlBase popupControl) {
			fPopupControl = popupControl;
		}
		protected bool IsRightToLeft {
			get { return (PopupControl as ISkinOwner).IsRightToLeft(); }
		}
	}
	public class PCControl : PCControlBase {
		private Dictionary<PopupWindow, PCWindowControlBase> windowControls = null;
		private Dictionary<PopupWindow, WebControl> modalBackgroundControls = null;
		public PCControl(ASPxPopupControlBase popupControl)
			: base(popupControl) {
		}
		protected Dictionary<PopupWindow, PCWindowControlBase> WindowControls {
			get { return windowControls; }
			set { windowControls = value; }
		}
		public PCWindowControlBase GetWindowControl(PopupWindow window) {
			return WindowControls.ContainsKey(window) ? WindowControls[window] : null;
		}
		protected override void ClearControlFields() {
			this.windowControls = null;
			this.modalBackgroundControls = null;
		}
		protected override void CreateControlHierarchy() {
			WindowControls = new Dictionary<PopupWindow, PCWindowControlBase>();
			this.modalBackgroundControls = new Dictionary<PopupWindow, WebControl>();
			for(int i = 0; i < PopupControl.WindowsInternal.Count; i++) {
				PopupWindow window = PopupControl.WindowsInternal[i];
				if(window.Enabled || DesignMode)
					CreateWindowControl(window);
			}
			if(PopupControl.HasDefaultWindowInternal())
				CreateWindowControl(PopupControl.DefaultWindow);
		}
		protected override void PrepareControlHierarchy() {
			foreach(WebControl modalBackgroundControl in this.modalBackgroundControls.Values)
				PopupControl.GetModalBackgroundStyle().AssignToControl(modalBackgroundControl);
		}
		protected virtual void CreateWindowControl(PopupWindow window) {
		}
	}
	public class PCWindowControlBase : PCControlBase {
		private PopupWindow fWindow = null;
		public PopupWindow Window {
			get { return fWindow; }
		}
		public PCWindowControlBase(PopupWindow window)
			: base(window.PopupControl) {
			fWindow = window;
		}
		public bool AddTemplate(ITemplate template, WebControl destination, string id) {
			if(template == null)
				return false;
			PopupControlTemplateContainer templateContainer = PopupControl.CreateTemplateContainer(Window);
			template.InstantiateIn(templateContainer);
			templateContainer.AddToHierarchy(destination, id);
			return true;
		}
	}
	public class PopupControlLite : PCControl {
		public PopupControlLite(ASPxPopupControlBase popupControl)
			: base(popupControl) {
		}
		protected override void CreateControlHierarchy() {
			WindowControls = new Dictionary<PopupWindow, PCWindowControlBase>();
			if (PopupControl.HasDefaultWindowInternal())
				CreateWindowControl(PopupControl.DefaultWindow);
			foreach(PopupWindow window in PopupControl.WindowsInternal) {
				if (window.Enabled || DesignMode)
					CreateWindowControl(window);
			}
		}
		protected override void PrepareControlHierarchy() {
		}
		protected override void CreateWindowControl(PopupWindow window) {
			PopupWindowControlLiteBase windowControl;
			if(DesignMode)
				windowControl = new PopupWindowControlLiteDesignMode(window);
			else
				windowControl = new PopupWindowControlLite(window);
			WindowControls.Add(window, windowControl);
			Controls.Add(windowControl);
			if(PopupControl.NeedRenderIFrameBehindPopupElement()) {
				WebControl iFrame = RenderUtils.CreateFakeIFrame(PopupControl.GetPopupWindowIFrameElementID(window),
					RenderUtils.PopupControlZIndex - 1);
				Controls.Add(iFrame);
			}
			if (window.Modal && !DesignMode)
				Controls.Add(new PopupWindowModalControlLite(window));
		}
	}
	public class PopupWindowControlLiteBase : PCWindowControlBase {
		protected override bool HasRootTag() { return true; }
		protected virtual bool IsTableRootTag { get { return true; } }
		protected override HtmlTextWriterTag TagKey { get { return IsTableRootTag ? HtmlTextWriterTag.Table : HtmlTextWriterTag.Div; } }
		public PopupWindowControlLiteBase(PopupWindow popupWindow)
			: base(popupWindow) {
		}
		protected override void CreateControlHierarchy() {
			if(PopupControl.IsHeaderVisible(Window))
				AddChild(new PopupWindowHeaderControlLite(Window));
			AddChild(new PopupWindowContentControlLite(Window));
			if(PopupControl.IsFooterVisible(Window))
				AddChild(new PopupWindowFooterControlLite(Window));
		}
		protected override void PrepareControlHierarchy() {
			if(IsTableRootTag)
				RenderUtils.ApplyCellPaddingAndSpacing(this);
			RenderUtils.AssignAttributes(PopupControl, this, true);
			Width = PopupControl.GetWindowWidth(Window);
			Height = PopupControl.GetWindowHeight(Window);
		}
		protected virtual void AddChild(PCWindowControlBase child) {
		}
		protected WebControl CreateTableCell() {
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			Controls.Add(row);
			WebControl cell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			row.Controls.Add(cell);
			return cell;
		}
	}
	public class PopupWindowControlLiteDesignMode : PopupWindowControlLiteBase {
		public PopupWindowControlLiteDesignMode(PopupWindow popupWindow)
			: base(popupWindow) {
		}
		protected override void AddChild(PCWindowControlBase child) {
			WebControl cell = CreateTableCell();
			if(child is PopupWindowContentControlLite)
				cell.Style["height"] = "100%";
			cell.Controls.Add(child);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupControl.GetControlStyle().AssignToControl(this);
			PopupControl.GetMainDivStyle().AssignToControl(this);
			RenderUtils.AppendDefaultDXClassName(this, PopupControlStyles.MainDivCssClassName);
		}
	}
	public class PopupWindowControlLite : PopupWindowControlLiteBase {
		private WebControl tableCell = null;
		protected WebControl TableCell {
			get {
				if(tableCell == null)
					tableCell = CreateTableCell();
				return tableCell;
			}
		}
		protected override bool IsTableRootTag { get { return false; } }
		protected WebControl ContentContainer { get { return this; } }
		protected WebControl Container { get { return MainDiv ?? this; } }
		protected WebControl MainDiv { get; private set; }
		protected WebControl AnimationWrapperForOldIE { get; private set; }
		public PopupWindowControlLite(PopupWindow popupWindow)
			: base(popupWindow) {
		}
		protected override void ClearControlFields() {
			MainDiv = null;
		}
		protected override void CreateControlHierarchy() {
			ID = PopupControl.GetPopupWindowID(Window);
			if(PopupControl.PopupAnimationType != AnimationType.None || PopupControl.CloseAnimationType != AnimationType.None)
				CreateMainElement();
			base.CreateControlHierarchy();
		}
		protected void CreateMainElement() {
			MainDiv = RenderUtils.CreateDiv();
			ContentContainer.Controls.Add(MainDiv);
		}
		protected override void AddChild(PCWindowControlBase child) {
			(MainDiv ?? ContentContainer).Controls.Add(child);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupControl.GetControlStyle().AssignToControl(this);
			if(AnimationWrapperForOldIE != null)
				RenderUtils.AppendDefaultDXClassName(AnimationWrapperForOldIE, PopupControlStyles.AnimationWrapperOldIEClassName);
			RenderUtils.AppendDefaultDXClassName(Container, PopupControlStyles.MainDivCssClassName);
			PopupControl.GetMainDivStyle().AssignToControl(Container);
			if(PopupControl.ShowShadow)
				RenderUtils.AppendDefaultDXClassName(Container, PopupControlStyles.ShadowCssClassName);
			if (PopupControl.IsRightToLeft())
				RenderUtils.AppendDefaultDXClassName(this, PopupControlStyles.RtlMarkerCssClassName);
			ToolTip = PopupControl.GetToolTip(Window);
			if (PopupControl.IsWindowDragging())
				RenderUtils.SetPreventSelectionAttribute(this);
			if(PopupControl.AllowResize || PopupControl.IsWindowDragging())
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this);
			Style.Add("z-index", RenderUtils.PopupControlZIndex.ToString());
			Style[HtmlTextWriterStyle.Display] = "none";
			Style[HtmlTextWriterStyle.Visibility] = "hidden";
		}
	}
	public class PopupWindowContentWrapper : ASPxInternalWebControl {
		public PopupWindowContentWrapper() : base() {
		}
	}
	public class PopupWindowModalControlLite : ASPxInternalWebControl {
		PopupWindow window = null;
		public PopupWindowModalControlLite(PopupWindow popupWindow) {
			this.window = popupWindow;
		}
		protected PopupWindow Window { get { return window; } }
		protected ASPxPopupControlBase PopupControl { get { return Window.PopupControl; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			ID = PopupControl.GetPopupWindowModalElementID(Window);
		}
		protected override void PrepareControlHierarchy() {
			PopupControl.GetModalBackgroundStyle().AssignToControl(this);
			Style.Add("z-index", (RenderUtils.PopupControlZIndex - 1).ToString());
		}
	}
	public class PopupWindowHeaderControlLite : PCWindowControlBase {
		Image imageControl = null;
		LiteralControl textControl = null;
		WebControl textSpan = null;
		HyperLink hyperLinkControl = null;
		DivButtonControl closeButtonControl = null;
		DivButtonControl pinButtonControl = null;
		DivButtonControl refreshButtonControl = null;
		DivButtonControl collapseButtonControl = null;
		DivButtonControl maxButtonControl = null;
		WebControl contentDiv = null;
		public PopupWindowHeaderControlLite(PopupWindow popupWindow)
			: base(popupWindow) {
		}
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected LiteralControl TextControl { get { return textControl; } }
		protected WebControl TextSpan { get { return textSpan; } }
		protected HyperLink HyperLinkControl { get { return hyperLinkControl; } }
		protected Image ImageControl { get { return imageControl; } }
		protected ImageProperties Image { get { return PopupControl.GetHeaderImageProperties(Window); } }
		protected string NavigateUrl { get { return PopupControl.GetHeaderNavigateUrl(Window); } }
		protected Unit ImageSpacing { get { return PopupControl.GetHeaderImageSpacing(Window); } }
		protected bool IsCloseButtonVisible { get { return PopupControl.GetShowWindowCloseButton(Window); } }
		protected bool IsPinButtonVisible { get { return PopupControl.GetShowWindowPinButton(Window); } }
		protected bool IsRefreshButtonVisible { get { return PopupControl.GetShowWindowRefreshButton(Window); } }
		protected bool IsCollapseButtonVisible { get { return PopupControl.GetShowWindowCollapseButton(Window); } }
		protected bool IsMaximizeButtonVisible { get { return PopupControl.GetShowWindowMaximizeButton(Window); } }
		protected DivButtonControl CloseButton { 
			get { return this.closeButtonControl; }
			set { this.closeButtonControl = value; }
		}
		protected DivButtonControl PinButton {
			get { return this.pinButtonControl; }
			set { this.pinButtonControl = value; }
		}
		protected DivButtonControl RefreshButton {
			get { return this.refreshButtonControl; }
			set { this.refreshButtonControl = value; }
		}
		protected DivButtonControl CollapseButton {
			get { return this.collapseButtonControl; }
			set { this.collapseButtonControl = value; }
		}
		protected DivButtonControl MaximizeButton {
			get { return this.maxButtonControl; }
			set { this.maxButtonControl = value; }
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void ClearControlFields() {
			this.imageControl = null;
			this.textControl = null;
			this.textSpan = null;
			this.hyperLinkControl = null;
			this.pinButtonControl = null;
			this.refreshButtonControl = null;
			this.closeButtonControl = null;
		}
		protected override void CreateControlHierarchy() {
			if(AddHeaderTemplate())
				return;
			CreateCloseButton();
			CreateMaximizeButton();
			CreateCollapseButton();
			CreateRefreshButton();
			CreatePinButton();
			CreateContent();
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected void CreateContent() {
			this.contentDiv = RenderUtils.CreateDiv();
			Controls.Add(ContentDiv);
			if(AddHeaderContentTemplate())
				return;
			WebControl container = ContentDiv;
			if(!string.IsNullOrEmpty(NavigateUrl)) {
				this.hyperLinkControl = RenderUtils.CreateHyperLink(true, true);
				container.Controls.Add(HyperLinkControl);
				container = HyperLinkControl;
			}
			CreateImage(container);
			CreateTextSpan(container);
		}
		private void CreateTextSpan(WebControl container) {
			this.textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			container.Controls.Add(TextSpan);
			this.textControl = RenderUtils.CreateLiteralControl();
			TextSpan.Controls.Add(TextControl);
		}
		protected void CreateImage(WebControl container) {
			if (!Image.IsEmpty) {
				this.imageControl = RenderUtils.CreateImage();
				container.Controls.Add(ImageControl);
			}
		}
		protected void CreateCloseButton() {
			if(IsCloseButtonVisible) {
				CloseButton = new DivButtonControl();
				Controls.Add(CloseButton);
			}
		}
		protected void CreatePinButton() {
			if(IsPinButtonVisible) {
				PinButton = new DivButtonControl();
				Controls.Add(PinButton);
				PinButton.ButtonImageID = PopupControl.GetHeaderButtonImageID(ASPxPopupControlBase.PopupHeaderButton.PinButton, Window);
			}
		}
		protected void CreateRefreshButton() {
			if(IsRefreshButtonVisible) {
				RefreshButton = new DivButtonControl();
				Controls.Add(RefreshButton);
			}
		}
		protected void CreateCollapseButton() {
			if(IsCollapseButtonVisible) {
				CollapseButton = new DivButtonControl();
				Controls.Add(CollapseButton);
				CollapseButton.ButtonImageID = PopupControl.GetHeaderButtonImageID(ASPxPopupControlBase.PopupHeaderButton.CollapseButton, Window);
			}
		}
		protected void CreateMaximizeButton() {
			if(IsMaximizeButtonVisible) {
				MaximizeButton = new DivButtonControl();
				Controls.Add(MaximizeButton);
				MaximizeButton.ButtonImageID = PopupControl.GetHeaderButtonImageID(ASPxPopupControlBase.PopupHeaderButton.MaximizeButton, Window);
			}
		}
		protected override void PrepareControlHierarchy() {
			PopupControl.GetHeaderStyle(Window).AssignToControl(this, true);
			RenderUtils.ResetWrap(this);
			if(ContentDiv != null) 
				RenderUtils.AppendDefaultDXClassName(ContentDiv, PopupControlStyles.HeaderContentContainerCssClassName);
			if(IsCloseButtonVisible || IsPinButtonVisible || IsRefreshButtonVisible || IsCollapseButtonVisible || IsMaximizeButtonVisible)
				RenderUtils.AppendDefaultDXClassName(this, PopupControlStyles.HeaderWithCloseButtonCssMarker);
			if (HyperLinkControl != null) {
				RenderUtils.AppendDefaultDXClassName(HyperLinkControl, PopupControlStyles.InternalHyperLinkCssClassName);
				RenderUtils.PrepareHyperLink(HyperLinkControl, string.Empty, NavigateUrl,
					PopupControl.GetTarget(Window), string.Empty, true);
				PopupControl.GetHeaderLinkStyle(Window).AssignToHyperLink(HyperLinkControl);
			}
			if (ImageControl != null) {
				RenderUtils.AppendDefaultDXClassName(ImageControl, PopupControlStyles.HeaderImageCssClassName);
				RenderUtils.SetVerticalAlignClass(ImageControl, PopupControl.GetHeaderStyle(Window).VerticalAlign);
				Image.AssignToControl(ImageControl, DesignMode);
				RenderUtils.SetHorizontalMargins(ImageControl, Unit.Empty, ImageSpacing);
				RenderUtils.MergeImageWithItemToolTip(ImageControl, PopupControl.GetToolTip(Window));
			}
			if (TextControl != null) {
				RenderUtils.AppendDefaultDXClassName(TextSpan, PopupControlStyles.HeaderTextCssClassName);
				RenderUtils.SetVerticalAlignClass(TextSpan, PopupControl.GetHeaderStyle(Window).VerticalAlign);
				RenderUtils.SetWrap(TextSpan, PopupControl.GetHeaderStyle(Window).Wrap);
				TextControl.Text = PopupControl.HtmlEncode(PopupControl.GetHeaderText(Window));
			}
			if (PopupControl.IsHeaderDragging(Window)) {
				RenderUtils.SetPreventSelectionAttribute(this);
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this);
			}
			if(CloseButton != null) {
				CloseButton.ButtonImage = PopupControl.GetCloseButtonImageProperties(Window);
				if(PopupControl.IsAccessibilityCompliantRender(true))
					CloseButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
				CloseButton.ButtonStyle = PopupControl.GetCloseButtonStyle(Window);
				CloseButton.ButtonPaddings = PopupControl.GetCloseButtonPaddings(Window);
			}
			if(PinButton != null) {
				PinButton.ButtonImage = PopupControl.GetPinButtonImageProperties(Window);
				if(PopupControl.IsAccessibilityCompliantRender(true))
					PinButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
				PinButton.ButtonStyle = PopupControl.GetPinButtonStyle(Window);
				PinButton.ButtonPaddings = PopupControl.GetPinButtonPaddings(Window);
			}
			if(RefreshButton != null) {
				RefreshButton.ButtonImage = PopupControl.GetRefreshButtonImageProperties(Window);
				if(PopupControl.IsAccessibilityCompliantRender(true))
					RefreshButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
				RefreshButton.ButtonStyle = PopupControl.GetRefreshButtonStyle(Window);
				RefreshButton.ButtonPaddings = PopupControl.GetRefreshButtonPaddings(Window);
			}
			if(CollapseButton != null) {
				CollapseButton.ButtonImage = PopupControl.GetCollapseButtonImageProperties(Window);
				if(PopupControl.IsAccessibilityCompliantRender(true))
					CollapseButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
				CollapseButton.ButtonStyle = PopupControl.GetCollapseButtonStyle(Window);
				CollapseButton.ButtonPaddings = PopupControl.GetCollapseButtonPaddings(Window);
			}
			if(MaximizeButton != null) {
				MaximizeButton.ButtonImage = PopupControl.GetMaximizeButtonImageProperties(Window);
				if(PopupControl.IsAccessibilityCompliantRender(true))
					MaximizeButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
				MaximizeButton.ButtonStyle = PopupControl.GetMaximizeButtonStyle(Window);
				MaximizeButton.ButtonPaddings = PopupControl.GetMaximizeButtonPaddings(Window);
			}
		}
		protected bool AddHeaderTemplate() {
			return AddTemplate(PopupControl.GetHeaderTemplate(Window), this, PopupControl.GetHeaderTemplateContainerID(Window));
		}
		protected bool AddHeaderContentTemplate() {
			return AddTemplate(PopupControl.GetHeaderContentTemplate(Window), ContentDiv, PopupControl.GetHeaderContentTemplateContainerID(Window));
		}
	}
	public class PopupWindowContentControlLite : PCWindowControlBase {
		LiteralControl textControl = null;
		WebControl contentDiv = null;
		public PopupWindowContentControlLite(PopupWindow popupWindow)
			: base(popupWindow) {
			popupWindow.SetContentContainer(this);
		}
		protected override bool HasRootTag() {
			return !PopupControl.IsCallback;
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected PopupWindowContentWrapper ToggleVisibleContentWrapper { get; private set; }
		protected override void ClearControlFields() {
			this.textControl = null;
			this.contentDiv = null;
		}
		protected bool IsChildCallback(Control rootControl) {
			return rootControl.Controls.OfType<ASPxWebControl>().Any(x => x.IsCallback) 
				|| rootControl.Controls.OfType<Control>().Any(IsChildCallback);
		}
		protected override void CreateControlHierarchy() {
			bool renderForCallbackResult = PopupControl.IsCallback;
			bool divWrapperRequired = !renderForCallbackResult;
			this.contentDiv = divWrapperRequired ? RenderUtils.CreateDiv() : new ContentControl();
			Controls.Add(this.contentDiv);
			CreateContentHierarchy(this.contentDiv);
		}
		private void CreateContentHierarchy(WebControl parentControl) {
			ToggleVisibleContentWrapper = new PopupWindowContentWrapper();
			parentControl.Controls.Add(ToggleVisibleContentWrapper);
			bool templateAdded = AddContentTemplate(ToggleVisibleContentWrapper);
			if(!templateAdded) {
				if(string.IsNullOrEmpty(Window.ContentUrl)) {
					this.textControl = RenderUtils.CreateLiteralControl();
					ToggleVisibleContentWrapper.Controls.Add(this.textControl);
					ToggleVisibleContentWrapper.Controls.Add(Window.ContentControl);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, PopupControlStyles.ContentWrapperCssClassName);
			PopupControl.GetContentStyle(Window).AssignToControl(this.contentDiv ?? this);
			if(this.contentDiv != null) {
				RenderUtils.AppendDefaultDXClassName(this.contentDiv, PopupControlStyles.ContentCssClassName);
				RenderUtils.SetPaddings(this.contentDiv, PopupControl.GetContentPaddings(Window));
			}
			if (!string.IsNullOrEmpty(Window.ContentUrl)) {
				Style[HtmlTextWriterStyle.Overflow] = "auto";
			} else if(this.contentDiv != null) {
					PopupControl.SetContentOverflow(this.contentDiv);
			}
			if (this.textControl != null) {
				string text = PopupControl.HtmlEncode(PopupControl.GetContentText(Window));
				if (PopupControl.EncodeHtml)
					text = HtmlConvertor.ToMultilineHtml(text);
				if (!DesignMode && string.IsNullOrEmpty(text) && Window.ContentControl.Controls.Count == 0)
					text = "&nbsp;";
				this.textControl.Text = text;
			}
			if(!PopupControl.GetIsWindowContentVisible(Window) && !PopupControl.IsCallback && !IsChildCallback(ToggleVisibleContentWrapper))
				ToggleVisibleContentWrapper.Visible = false;
		}
		protected bool AddContentTemplate(WebControl container) {
			return AddTemplate(PopupControl.GetContentTemplate(Window), container, PopupControl.GetContentTemplateContainerID(Window));
		}
	}
	public class PopupWindowFooterControlLite : PCWindowControlBase {
		Image imageControl = null;
		LiteralControl textControl = null;
		WebControl textSpan = null;
		HyperLink hyperLinkControl = null;
		WebControl contentDiv = null;
		Image sizeGrip = null;
		public PopupWindowFooterControlLite(PopupWindow popupWindow)
			: base(popupWindow) {
		}
		protected string NavigateUrl { get { return PopupControl.GetFooterNavigateUrl(Window); } }
		protected ImageProperties Image { get { return PopupControl.GetFooterImageProperties(Window); } }
		protected Unit ImageSpacing { get { return PopupControl.GetFooterImageSpacing(Window); } }
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected Image ImageControl { get { return imageControl; } }
		protected LiteralControl TextControl { get { return textControl; } }
		protected HyperLink HyperLinkControl { get { return hyperLinkControl; } }
		protected WebControl TextSpan { get { return textSpan; } }
		protected Image SizeGrip { get { return sizeGrip; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void ClearControlFields() {
			this.imageControl = null;
			this.textControl = null;
			this.textSpan = null;
			this.hyperLinkControl = null;
			this.contentDiv = null;
			this.sizeGrip = null;
		}
		protected override void CreateControlHierarchy() { 
			if(AddFooterTemplate())
				return;
			CreateContent();
			CreateSizeGrip();
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected void CreateContent() {
			this.contentDiv = RenderUtils.CreateDiv();
			Controls.Add(ContentDiv);
			if(AddFooterContentTemplate())
				return;
			WebControl container = ContentDiv;
			if(!string.IsNullOrEmpty(NavigateUrl)) {
				this.hyperLinkControl = RenderUtils.CreateHyperLink(true, true);
				container.Controls.Add(HyperLinkControl);
				container = HyperLinkControl;
			}
			CreateImage(container);
			CreateTextSpan(container);
		}
		protected void CreateTextSpan(WebControl container) {
			this.textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			container.Controls.Add(TextSpan);
			this.textControl = RenderUtils.CreateLiteralControl();
			TextSpan.Controls.Add(TextControl);
		}
		protected void CreateImage(WebControl container) {
			if (!Image.IsEmpty) {
				this.imageControl = RenderUtils.CreateImage();
				container.Controls.Add(ImageControl);
			}
		}
		protected void CreateSizeGrip() {
			if(PopupControl.HasSizeGrip()) {
				this.sizeGrip = RenderUtils.CreateImage();
				Controls.Add(SizeGrip);
			}
		}
		protected override void PrepareControlHierarchy() {
			PopupControl.GetFooterStyle(Window).AssignToControl(this);
			RenderUtils.ResetWrap(this);
			RenderUtils.SetPaddings(ContentDiv ?? this, PopupControl.GetFooterPaddings(Window));
			if(ContentDiv != null) 
				RenderUtils.AppendDefaultDXClassName(ContentDiv, PopupControlStyles.FooterContentContainerCssClassName);
			if(HyperLinkControl != null) {
				RenderUtils.AppendDefaultDXClassName(HyperLinkControl, PopupControlStyles.InternalHyperLinkCssClassName);
				RenderUtils.PrepareHyperLink(HyperLinkControl, string.Empty, NavigateUrl,
					PopupControl.GetTarget(Window), string.Empty, true);
				PopupControl.GetFooterLinkStyle(Window).AssignToHyperLink(HyperLinkControl);
			}
			if(ImageControl != null) {
				RenderUtils.AppendDefaultDXClassName(ImageControl, PopupControlStyles.FooterImageCssClassName);
				RenderUtils.SetVerticalAlignClass(ImageControl, PopupControl.GetFooterStyle(Window).VerticalAlign);
				Image.AssignToControl(this.imageControl, DesignMode);
				RenderUtils.SetHorizontalMargins(ImageControl, Unit.Empty, ImageSpacing);
				RenderUtils.MergeImageWithItemToolTip(ImageControl, PopupControl.GetToolTip(Window));
			}
			if(TextControl != null) {
				RenderUtils.AppendDefaultDXClassName(TextSpan, PopupControlStyles.FooterTextCssClassName);
				RenderUtils.SetVerticalAlignClass(TextSpan, PopupControl.GetFooterStyle(Window).VerticalAlign);
				RenderUtils.SetWrap(TextSpan, PopupControl.GetFooterStyle(Window).Wrap);
				TextControl.Text = PopupControl.HtmlEncode(PopupControl.GetFooterText(Window));
			}
			if(SizeGrip != null) {
				RenderUtils.AppendDefaultDXClassName(SizeGrip, PopupControlStyles.SizeGripCssClassName);
				PopupControl.GetSizeGripImageProperties(Window).AssignToControl(SizeGrip, DesignMode);
				RenderUtils.SetMargins(SizeGrip, PopupControl.GetSizeGripPaddings(Window));
				RenderUtils.MergeImageWithItemToolTip(SizeGrip, ToolTip);
				RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this);
			}
		}
		protected bool AddFooterTemplate() {
			return AddTemplate(PopupControl.GetFooterTemplate(Window), this, PopupControl.GetFooterTemplateContainerID(Window));
		}
		protected bool AddFooterContentTemplate() {
			return AddTemplate(PopupControl.GetFooterContentTemplate(Window), ContentDiv, PopupControl.GetFooterContentTemplateContainerID(Window));
		}
	}
}
