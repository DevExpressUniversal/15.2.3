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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class ButtonNativeControl: ASPxInternalWebControl {
		private string name = "";
		private string text = "";
		private bool useSubmitBehavior = true;
		WebControl button = null;
		private ButtonControlStyle buttonStyle = null;
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public bool UseSubmitBehavior {
			get { return useSubmitBehavior; }
			set { useSubmitBehavior = value; }
		}
		public ButtonControlStyle ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
		protected WebControl Button {
			get { return button; }
		}
		protected override void ClearControlFields() {
			this.button = null;
		}
		protected override void CreateControlHierarchy() {
			this.button = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			Controls.Add(Button);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, Button);
			if(ButtonStyle != null) {
				ButtonStyle.AssignToControl(Button, true);
				RenderUtils.SetWrap(Button, ButtonStyle.Wrap);
			}
			RenderUtils.SetAttribute(Button, "value", Text, "dummy");
			if(UseSubmitBehavior) {
				if(!string.IsNullOrEmpty(Name))
					RenderUtils.SetStringAttribute(Button, "name", Name);
				RenderUtils.SetStringAttribute(Button, "type", "submit");
			} else
				RenderUtils.SetStringAttribute(Button, "type", "button");
			if(!Enabled)
				RenderUtils.SetStringAttribute(Button, "disabled", "disabled");
		}
	}
	public class ButtonTextContainer : ASPxInternalWebControl {
		private string text;
		public string Text {
			get { return text; }
			set { text = value; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Span; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new LiteralControl());
		}
		protected override void PrepareControlHierarchy() {
			LiteralControl literal = (Controls.Count > 0) ? Controls[0] as LiteralControl : null;
			if(literal != null) literal.Text = Text;
		}
	}
	public class ButtonInternalControlBase : ASPxInternalWebControl {
		private bool allowFocus = false;
		private CheckedButtonImageProperties image = null;
		private ImagePosition imagePosition = ImagePosition.Left;
		private Unit imageSpacing = Unit.Empty;
		private HorizontalAlign horizontalAlign = HorizontalAlign.NotSet;
		private bool useSubmitBehavior = false;
		private string name = "";
		private string text = "";
		private string valueStr = "";
		private bool mainCellStretched;
		private bool rtl;
		private string onFocusScript = string.Empty;
		private string onBlurScript = string.Empty;
		private string onClickScript = string.Empty;
		private ButtonControlStyle buttonStyle = null;
		private AppearanceStyle buttonContentDivStyle = null;
		private Image imageControl = null;
		private ButtonTextContainer textControl = null;
		public ButtonInternalControlBase()
			: this(string.Empty) {
		}
		public ButtonInternalControlBase(string text)
			: this(text, new CheckedButtonImageProperties(), ImagePosition.Left, true, true, HorizontalAlign.NotSet) {
		}
		public ButtonInternalControlBase(string text, CheckedButtonImageProperties image, ImagePosition imagePosition, bool allowFocus,
			bool useSubmitBehavior, HorizontalAlign horizontalAlign)
			: base() {
			this.text = text;
			this.image = image;
			this.imagePosition = imagePosition;
			this.allowFocus = allowFocus;
			this.useSubmitBehavior = useSubmitBehavior;
			this.horizontalAlign = horizontalAlign;
		}
		public bool AllowFocus {
			get { return allowFocus; }
			set { 
				allowFocus = value;
				ResetControlHierarchy();
			}
		}
		public bool IsRightToLeft {
			get { return rtl; }
			set { rtl = value; }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string Text {
			get { return text; }
			set { 
				text = value;
				ResetControlHierarchy();
			}
		}
		public string Value {
			get { return valueStr; }
			set { valueStr = value; }
		}
		public bool MainCellStretched {
			get { return mainCellStretched; }
			set { mainCellStretched = value; }
		}
		public CheckedButtonImageProperties Image {
			get { return image; }
			set { 
				image = value;
				ResetControlHierarchy();
			}
		}
		public ImagePosition ImagePosition {
			get { return imagePosition; }
			set {
				imagePosition = value;
				ResetControlHierarchy();
			}
		}
		public Unit ImageSpacing {
			get { return imageSpacing; }
			set { imageSpacing = value; }
		}
		public HorizontalAlign HorizontalAlign {
			get { return horizontalAlign; }
			set {
				horizontalAlign = value;
				ResetControlHierarchy();
			}
		}
		public bool UseSubmitBehavior {
			get { return useSubmitBehavior; }
			set { 
				useSubmitBehavior = value;
				ResetControlHierarchy();
			}
		}
		public string OnFocusScript {
			get { return onFocusScript; }
			set { onFocusScript = value; }
		}
		public string OnBlurScript {
			get { return onBlurScript; }
			set { onBlurScript = value; }
		}
		public string OnClickScript {
			get { return onClickScript; }
			set { onClickScript = value; }
		}
		public ButtonControlStyle ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
		public AppearanceStyle ButtonContentDivStyle {
			get { return buttonContentDivStyle; }
			set { buttonContentDivStyle = value; }
		}
		protected Image ImageControl {
			get { return imageControl; }
		}
		protected ButtonTextContainer TextControl {
			get { return textControl; }
		}
		protected override void ClearControlFields() {
			this.imageControl = null;
			this.textControl = null;
		}
		protected virtual void CreateImage(WebControl parent) {
			this.imageControl = RenderUtils.CreateImage();
			parent.Controls.Add(ImageControl);
		}
		protected virtual void CreateText(WebControl parent) {
			this.textControl = new ButtonTextContainer();
			parent.Controls.Add(TextControl);
		}
		protected virtual void CreateImageAndText(WebControl parent) {
			if(Image.IsEmpty)
				CreateText(parent);
			else {
				if(IsImageAside()) {
					if(!Image.IsEmpty && ImagePosition == ImagePosition.Left)
						CreateImage(parent);
					if(!string.IsNullOrEmpty(Text))
						CreateText(parent);
					if(!Image.IsEmpty && ImagePosition == ImagePosition.Right)
						CreateImage(parent);
				}
				else {
					if(!Image.IsEmpty && ImagePosition == ImagePosition.Top) {
						CreateImage(parent);
						if(!string.IsNullOrEmpty(Text))
							parent.Controls.Add(RenderUtils.CreateBr());
					}
					if(!string.IsNullOrEmpty(Text))
						CreateText(parent);
					if(!Image.IsEmpty && ImagePosition == ImagePosition.Bottom) {
						if(!string.IsNullOrEmpty(Text))
							parent.Controls.Add(RenderUtils.CreateBr());
						CreateImage(parent);
					}
				}
			}
		}
		protected virtual void PrepareImage() {
			Image.AssignToControl(ImageControl, DesignMode, !Enabled);
			if(TextControl != null)
				RenderUtils.SetMargins(ImageControl, GetImagePaddings());
			if(IsImageAside())
				RenderUtils.SetVerticalAlignClass(ImageControl, ButtonStyle.VerticalAlign);
		}
		protected virtual void PrepareText() {
			TextControl.Text = string.IsNullOrEmpty(Text) ? "&nbsp;" : Text;
			if(IsImageAside())
				RenderUtils.SetVerticalAlignClass(TextControl, ButtonStyle.VerticalAlign);
			if(ButtonStyle != null)
				RenderUtils.SetWrap(TextControl, ButtonStyle.Wrap, DesignMode);
		}
		protected bool IsImageAside() {
			return ImagePosition == ImagePosition.Left || ImagePosition == ImagePosition.Right;
		}
		protected Paddings GetImagePaddings() {
			Paddings paddings = new Paddings();
			switch(ImagePosition) {
				case ImagePosition.Top:
					paddings.PaddingBottom = ImageSpacing;
					break;
				case ImagePosition.Bottom:
					paddings.PaddingTop = ImageSpacing;
					break;
				case ImagePosition.Right:
					if(IsRightToLeft)
						paddings.PaddingRight = ImageSpacing;
					else
						paddings.PaddingLeft = ImageSpacing;
					break;
				case ImagePosition.Left:
					if(IsRightToLeft)
						paddings.PaddingLeft = ImageSpacing;
					else
						paddings.PaddingRight = ImageSpacing;
					break;
			}
			return paddings;
		}
	}
	public class ButtonInternalControl : ButtonInternalControlBase {
		private WebControl mainDiv = null;
		private WebControl internalButton = null;
		private WebControl internalButtonDiv = null;
		private WebControl contentDiv = null;
		bool isAccessibilityCompliant = false;
		public ButtonInternalControl()
			: base() {
		}
		public ButtonInternalControl(string text)
			: base(text) {
		}
		public ButtonInternalControl(string text, CheckedButtonImageProperties image, ImagePosition imagePosition, bool allowFocus,
			bool useSubmitBehavior, HorizontalAlign horizontalAlign, bool isAccessibilityCompliant)
			: base(text, image, imagePosition, allowFocus, useSubmitBehavior, horizontalAlign) {
				this.isAccessibilityCompliant = isAccessibilityCompliant;
		}
		protected WebControl MainDiv {
			get { return mainDiv; }
		}
		protected WebControl InternalButton {
			get { return internalButton; }
		}
		protected WebControl InternalButtonDiv {
			get { return internalButtonDiv; }
		}
		protected WebControl ContentDiv {
			get { return contentDiv; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainDiv = null;
			this.internalButton = null;
			this.internalButtonDiv = null;
			this.contentDiv = null;
		}
		protected override void CreateControlHierarchy() {
			WebControl contentContainer;
			if(DesignMode) {
				this.mainDiv = RenderUtils.CreateTable();
				Controls.Add(MainDiv);
				TableRow row = RenderUtils.CreateTableRow();
				MainDiv.Controls.Add(row);
				TableCell cell = RenderUtils.CreateTableCell();
				row.Controls.Add(cell);
				contentContainer = cell;
			}
			else {
				this.mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(MainDiv);
				contentContainer = MainDiv;
			}
			CreateContentDiv(contentContainer);
			if(NeedRenderHiddenButton())
				CreateInternalButton(ContentDiv);
			CreateImageAndText(ContentDiv);
		}
		protected virtual void CreateInternalButton(WebControl parent) {
			if(HiddenButtonWrapDivRequired()) {
				this.internalButtonDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parent.Controls.Add(InternalButtonDiv);
				parent = InternalButtonDiv;
			}
			this.internalButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			this.InternalButton.ID = ASPxButton.ButtonInputPostfix;
			parent.Controls.Add(this.internalButton);
		}
		protected virtual void CreateContentDiv(WebControl parent) {
			this.contentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(ContentDiv);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, MainDiv);
			MainDiv.TabIndex = 0;
			MainDiv.AccessKey = "";
			MainDiv.CssClass = CssClass;
			RenderUtils.SetPreventSelectionAttribute(MainDiv);
			if(ButtonStyle != null) {
				ButtonStyle.AssignToControl(MainDiv, !DesignMode);
				RenderUtils.ResetWrap(MainDiv);
				if(DesignMode) {
					TableCell cell = (TableCell)MainDiv.Controls[0].Controls[0];
					ButtonStyle.Paddings.AssignToControl(cell);
				}
			}
			if(!string.IsNullOrEmpty(OnClickScript))
				RenderUtils.SetStringAttribute(MainDiv, "onclick", OnClickScript);
			RenderUtils.AppendDefaultDXClassName(MainDiv, ButtonControlStyles.ButtonSystemClassName);
			if(!Browser.IsSafari)
				RenderUtils.AppendDefaultDXClassName(MainDiv, ButtonControlStyles.ButtonTableSystemClassName);
			if(ButtonContentDivStyle != null)
				ButtonContentDivStyle.AssignToControl(ContentDiv, true);
			if(TextControl == null)
				RenderUtils.SetStyleStringAttribute(ContentDiv, "font-size", "0px");
			if(InternalButton != null)
				PrepareInternalButton();
			if(ImageControl != null)
				PrepareImage();
			if(TextControl != null)
				PrepareText();
		}
		protected virtual void PrepareInternalButton() {
			InternalButton.TabIndex = TabIndex;
			InternalButton.ToolTip = ToolTip;
			InternalButton.AccessKey = AccessKey;
			InternalButton.CssClass = this.isAccessibilityCompliant ? AccessibilityUtils.InvisibleFocusableElementCssClassName : "dxb-hb";
			string inputValue = string.IsNullOrEmpty(Value) ? "submit" : Value;
			RenderUtils.SetAttribute(InternalButton, "value", inputValue, "dummy");
			if(!string.IsNullOrEmpty(OnFocusScript))
				RenderUtils.SetStringAttribute(InternalButton, "onfocus", OnFocusScript);
			if(!string.IsNullOrEmpty(OnBlurScript))
				RenderUtils.SetStringAttribute(InternalButton, "onblur", OnBlurScript);
			RenderUtils.SetStringAttribute(InternalButton, "type", UseSubmitBehavior ? "submit" : "button");
			if(!string.IsNullOrEmpty(Name))
				RenderUtils.SetStringAttribute(InternalButton, "name", Name);
			if(Browser.IsOpera)
				RenderUtils.SetOpacity(InternalButton, 1);
			if(InternalButtonDiv != null) {
				if(!RenderUtils.IsHtml5Mode(this))
					RenderUtils.SetStringAttribute(InternalButton, "readonly", "readonly");
				InternalButtonDiv.CssClass = "dxb-hbc";
			}
		}
		protected bool NeedRenderHiddenButton() {
			return Enabled && (AllowFocus || UseSubmitBehavior) && !DesignMode;
		}
		protected bool HiddenButtonWrapDivRequired() {
			return Browser.Family.IsWebKit;
		}
	}
	public class ButtonLinkInternalControl : ButtonInternalControlBase {
		private InternalHyperLinkControl link = null;
		public ButtonLinkInternalControl()
			: base() {
		}
		public ButtonLinkInternalControl(string text)
			: base(text) {
		}
		public ButtonLinkInternalControl(string text, CheckedButtonImageProperties image, ImagePosition imagePosition, bool allowFocus,
			bool useSubmitBehavior, HorizontalAlign horizontalAlign)
			: base(text, image, imagePosition, allowFocus, useSubmitBehavior, horizontalAlign) {
		}
		public InternalHyperLinkControl Link {
			get { return link; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.link = null;
		}
		protected override void CreateControlHierarchy() {
			this.link = new InternalHyperLinkControl();
			Controls.Add(Link);
			CreateImageAndText(Link);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, Link);
			Link.TabIndex = TabIndex;
			Link.ToolTip = ToolTip;
			Link.AccessKey = AccessKey;
			Link.Enabled = IsEnabled();
			Link.CssClass = CssClass;
			if(ButtonStyle != null)
				ButtonStyle.AssignToControl(Link, true);
			RenderUtils.AppendDefaultDXClassName(Link, ButtonControlStyles.ButtonSystemClassName);
			if(!string.IsNullOrEmpty(OnClickScript))
				RenderUtils.SetStringAttribute(Link, "onclick", OnClickScript);
			if(!string.IsNullOrEmpty(OnFocusScript))
				RenderUtils.SetStringAttribute(Link, "onfocus", OnFocusScript);
			if(!string.IsNullOrEmpty(OnBlurScript))
				RenderUtils.SetStringAttribute(Link, "onblur", OnBlurScript);
			if(ImageControl != null)
				PrepareImage();
			if(TextControl != null)
				PrepareText();
		}
		protected override void PrepareText() {
			base.PrepareText();
			if(Image.IsEmpty)
				TextControl.CssClass = string.Empty; 
			if(ButtonStyle != null)
				RenderUtils.PrepareHyperLinkChildStyle(TextControl, ButtonStyle);
		}
	}
	public class ButtonControl: ASPxInternalWebControl {
		private ASPxButton button = null;
		private ButtonInternalControlBase buttonInternal = null;
		private ButtonNativeControl buttonNative = null;
		public ButtonControl(ASPxButton button)
			: base() {
			this.button = button;
		}
		protected ASPxButton Button {
			get { return this.button; }
		}
		protected bool IsRightToLeft {
			get { return (Button as ISkinOwner).IsRightToLeft(); }
		}
		protected ButtonInternalControlBase ButtonInternal {
			get { return this.buttonInternal; }
		}
		protected ButtonNativeControl ButtonNative {
			get { return this.buttonNative; }
		}
		protected override void ClearControlFields() {
			this.buttonInternal = null;
		}
		protected override void CreateControlHierarchy() {
			if(Button.IsNativeRender())
				CreateNativeButtonControl();
			else if(Button.IsLink())
				CreateLinkButtonControl();
			else
				CreateButtonControl();
		}
		protected override void PrepareControlHierarchy() {
			if(Button.IsNativeRender())
				PrepareNativeButtonControl();
			else
				PrepareInternalButtonControl();
		}
		protected void CreateButtonControl() {
			this.buttonInternal = new ButtonInternalControl(Button.GetText(), Button.GetImage(),
				Button.ImagePosition, Button.AllowFocus, Button.UseSubmitBehavior, Button.HorizontalAlign,
				Button.IsAccessibilityCompliantRender());
			ButtonInternal.IsRightToLeft = IsRightToLeft; 
			Controls.Add(ButtonInternal);
		}
		protected void CreateLinkButtonControl() {
			this.buttonInternal = new ButtonLinkInternalControl(Button.Text, Button.GetImage(),
				Button.ImagePosition, Button.AllowFocus, Button.UseSubmitBehavior, Button.HorizontalAlign);
			ButtonInternal.IsRightToLeft = IsRightToLeft;
			Controls.Add(ButtonInternal);
		}
		protected void CreateNativeButtonControl() {
			this.buttonNative = new ButtonNativeControl();
			Controls.Add(ButtonNative);
		}
		protected void PrepareInternalButtonControl() {
			ButtonControlStyle buttonStyle = Button.GetButtonStyle();
			RenderUtils.AssignAttributes(Button, ButtonInternal);
			RenderUtils.SetVisibility(ButtonInternal, Button.IsClientVisible(), true);
			ButtonInternal.Name = Button.UniqueID;
			ButtonInternal.Enabled = Button.IsEnabled();
			ButtonInternal.ButtonStyle = buttonStyle;
			ButtonInternal.ButtonContentDivStyle = Button.GetButtonContentDivStyle();
			ButtonInternal.ImageSpacing = Button.GetImageSpacing();
			ButtonInternal.TabIndex = Button.GetTabIndex();
			ButtonInternal.Text = Button.GetText();
			ButtonInternal.Value = Button.GetValue();
			ButtonInternal.OnClickScript = Button.GetOnClickScript();
			ButtonInternal.ToolTip = Button.ToolTip;
			ButtonInternal.MainCellStretched = Browser.IsOpera && !buttonStyle.Width.IsEmpty;
		}
		protected void PrepareNativeButtonControl() {
			RenderUtils.AssignAttributes(Button, ButtonNative);
			RenderUtils.SetVisibility(ButtonNative, Button.IsClientVisible(), true);
			ButtonNative.Enabled = Button.IsEnabled();
			ButtonNative.TabIndex = Button.GetTabIndex();
			ButtonNative.Name = Button.UniqueID;
			ButtonNative.Text = Button.Text;
			ButtonNative.UseSubmitBehavior = Button.UseSubmitBehavior;
			ButtonNative.ButtonStyle = Button.GetButtonStyle();
		}
	}
}
