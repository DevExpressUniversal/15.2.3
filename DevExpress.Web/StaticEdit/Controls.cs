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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web;
using System.IO;
using System.Drawing;
namespace DevExpress.Web.Internal {
	public class InternalHyperLinkControl : InternalHyperLink {
		public InternalHyperLinkControl() {
			IsAlwaysHyperLink = true;
		}
		protected override object SaveViewState() {
			return null;
		}
		protected override object SaveControlState() {
			return null;
		}
	}
	public class HyperLinkDisplayControl : ASPxInternalWebControl {
		private AppearanceStyleBase controlStyle = null;
		private AppearanceStyleBase disabledStyle = null;
		private string navigateUrl = "";
		private string imageAltText = "";
		private string text = "";
		private string target = "";
		private string onClickScript = "";
		private InternalHyperLinkControl actualHyperLink = null;
		private InternalHyperLinkControl hyperLink = null;
		private System.Web.UI.WebControls.Image image = null;
		private ImageProperties imageProperties = null;
		private InternalHyperLinkControl imageHyperLink = null;
		internal new AppearanceStyleBase ControlStyle {
			get { return controlStyle; }
			set { controlStyle = value; }
		}
		public AppearanceStyleBase DisabledStyle {
			get { return disabledStyle; }
			set { disabledStyle = value; }
		}
		public string NavigateUrl {
			get { return navigateUrl; }
			set { navigateUrl = value; }
		}
		public string ImageUrl {
			get { return ImageProperties.Url; }
			set { ImageProperties.Url = value; }
		}
		public Unit ImageHeight {
			get { return ImageProperties.Height; }
			set { ImageProperties.Height = value; }
		}
		public Unit ImageWidth {
			get { return ImageProperties.Width; }
			set { ImageProperties.Width = value; }
		}
		public string ImageAltText {
			get { return imageAltText; }
			set { imageAltText = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public string Target {
			get { return target; }
			set { target = value; }
		}
		public string OnClickScript {
			get { return onClickScript; }
			set { onClickScript = value; }
		}
		protected InternalHyperLinkControl ActualHyperLink {
			get { return actualHyperLink; }
		}
		protected InternalHyperLinkControl HyperLink {
			get {
				if(hyperLink == null)
					hyperLink = new InternalHyperLinkControl();
				return hyperLink;
			}
		}
		protected System.Web.UI.WebControls.Image Image {
			get { return image; }
		}
		protected ImageProperties ImageProperties {
			get {
				if(imageProperties == null)
					imageProperties = new ImageProperties();
				return imageProperties;
			}
		}
		protected InternalHyperLinkControl ImageHyperLink {
			get { return imageHyperLink; }
		}
		protected override void ClearControlFields() {
			this.image = null;
			this.imageHyperLink = null;
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(HyperLink);
			this.imageHyperLink = new InternalHyperLinkControl();
			Controls.Add(ImageHyperLink);
			this.image = RenderUtils.CreateImage();
			ImageHyperLink.Controls.Add(Image);
		}
		protected override void PrepareControlHierarchy() {
			this.actualHyperLink = (ImageUrl != "") ? ImageHyperLink : HyperLink;
			HyperLink.Visible = (ActualHyperLink == HyperLink);
			ImageHyperLink.Visible = (ActualHyperLink == ImageHyperLink);
			if(!ActualHyperLink.HasControls() && Text != "")
				ActualHyperLink.Text = Text;
			ActualHyperLink.Enabled = IsEnabled();
			ActualHyperLink.NavigateUrl = NavigateUrl;
			ActualHyperLink.Target = Target;
			ActualHyperLink.ToolTip = ToolTip;
			if(IsEnabled())
				RenderUtils.SetStringAttribute(ActualHyperLink, "onclick", OnClickScript);
			if(Image != null) {
				ImageProperties.AssignToControl(Image, DesignMode);
				Image.AlternateText = !string.IsNullOrEmpty(ImageAltText) ? ImageAltText : Text;
				Image.ToolTip = ToolTip;
			}
			if(ControlStyle != null)
				ControlStyle.AssignToControl(ActualHyperLink);
		}
		protected internal void SetProperties(HyperLinkProperties properties) {
			ImageUrl = properties.ImageUrl;
			ImageWidth = properties.ImageWidth;
			ImageHeight = properties.ImageHeight;
		}
	}
	public class HyperLinkControl : HyperLinkDisplayControl {
		private ASPxHyperLink edit = null;
		public HyperLinkControl(ASPxHyperLink edit) {
			this.edit = edit;
		}
		public ASPxHyperLink Edit {
			get { return edit; }
		}
		protected override void PrepareControlHierarchy() {
			Enabled = Edit.Enabled;
			OnClickScript = Edit.GetOnClick();
			ToolTip = Edit.ToolTip;
			SetProperties(Edit.Properties);
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(Edit, ActualHyperLink, false, true);
			RenderUtils.SetVisibility(ActualHyperLink, Edit.IsClientVisible(), true);
			Edit.GetControlStyle().AssignToControl(ActualHyperLink);
			var requireBlockRender = Edit.IsClientVisible() && (!Edit.Width.IsEmpty || !Edit.Height.IsEmpty);
			if(requireBlockRender) {
				RenderUtils.SetStyleStringAttribute(ActualHyperLink, "display", "inline-block");
				RenderUtils.AllowEllipsisInText(ActualHyperLink, Edit.AllowEllipsisInText);
			}
		}
		protected internal ControlCollection GetControlCollection() {
			return HyperLink.Controls;
		}
	}
	public class ImageControlImageProperties : ImageProperties {
		public ImageControlImageProperties() : base() { }
		public new string SpriteUrl {
			get { return base.SpriteUrl; }
			set { base.SpriteUrl = value; }
		}
	}
	public class ImageDisplayControl : ASPxInternalWebControl {
		private ImageControlImageProperties imageProperties = null;
		private string onClickScript = "";
		public ImageDisplayControl() {
		}
		protected System.Web.UI.WebControls.Image Image {
			get; set;
		}
		protected virtual bool NeedRenderSizes { get { return true; } }
		public override string ToolTip {
			get {
				return ImageProperties.ToolTip;
			}
			set {
				ImageProperties.ToolTip = value;
			}
		}
		public virtual bool AssignEmptyImage { get { return true; } }
		public string AlternateText {
			get { return ImageProperties.AlternateText; }
			set { ImageProperties.AlternateText = value; }
		}
		public string DescriptionUrl {
			get { return ImageProperties.DescriptionUrl; }
			set { ImageProperties.DescriptionUrl = value; }
		}
		public ImageAlign ImageAlign {
			get { return ImageProperties.Align; }
			set { ImageProperties.Align = value; }
		}
		public Unit ImageHeight {
			get { return ImageProperties.Height; }
			set { ImageProperties.Height = value; }
		}
		public Unit ImageWidth {
			get { return ImageProperties.Width; }
			set { ImageProperties.Width = value; }
		}
		public string ImageUrl {
			get { return ImageProperties.Url; }
			set { ImageProperties.Url = value; }
		}
		[Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return ImageProperties.IsResourcePng; }
			set { ImageProperties.IsResourcePng = value; }
		}
		public Unit SpriteLeft {
			get { return ImageProperties.SpriteProperties.Left; }
			set { ImageProperties.SpriteProperties.Left = value; }
		}
		public Unit SpriteTop {
			get { return ImageProperties.SpriteProperties.Top; }
			set { ImageProperties.SpriteProperties.Top = value; }
		}
		public string SpriteUrl {
			get { return ImageProperties.SpriteUrl; }
			set { ImageProperties.SpriteUrl = value; }
		}
		public string SpriteCssClass {
			get { return ImageProperties.SpriteProperties.CssClass; }
			set { ImageProperties.SpriteProperties.CssClass = value; }
		}
		public string OnClickScript {
			get { return onClickScript; }
			set { onClickScript = value; }
		}
		protected internal ImageControlImageProperties ImageProperties {
			get {
				if(imageProperties == null)
					imageProperties = new ImageControlImageProperties();
				return imageProperties;
			}
		}
		protected override void ClearControlFields() {
			Image = null;
		}
		protected override void CreateControlHierarchy() {
			Image = RenderUtils.CreateImage();
			Controls.Add(Image);
		}
		protected override void PrepareControlHierarchy() {
			ImageProperties.AssignToControl(Image, DesignMode);
		}
		protected internal void SetProperties(string imageUrl, string spriteUrl, IImageEditProperties properties, IValueProvider valueProvider) {
			string alternateText = properties.AlternateText;
			string descriptionUrl = properties.DescriptionUrl;
			string toolTip = properties.ToolTip;
			if(valueProvider != null) {
				if(!string.IsNullOrEmpty(properties.AlternateTextField)) {
					object alternateTextValue = valueProvider.GetValue(properties.AlternateTextField);
					if(alternateTextValue != null)
						alternateText = string.Format(properties.AlternateTextFormatString, alternateTextValue.ToString());
				}
				if(!string.IsNullOrEmpty(properties.DescriptionUrlField)) {
					object descriptionUrlValue = valueProvider.GetValue(properties.DescriptionUrlField);
					if(descriptionUrlValue != null)
						descriptionUrl = string.Format(properties.DescriptionUrlFormatString, descriptionUrlValue.ToString());
				}
				if(!string.IsNullOrEmpty(properties.ToolTipField)) {
					object toolTipValue = valueProvider.GetValue(properties.ToolTipField);
					if(toolTipValue != null)
						toolTip = string.Format(properties.ToolTipFormatString, toolTipValue.ToString());
				}
			}
			ImageUrl = !string.IsNullOrEmpty(imageUrl) ?
				string.Format(properties.ImageUrlFormatString, imageUrl) : "";
			AlternateText = alternateText;
			ToolTip = toolTip;
			DescriptionUrl = descriptionUrl;
			if(properties.ApplyImageAlignToDisplayControl)
				ImageAlign = properties.ImageAlign;
			if(NeedRenderSizes) {
				ImageHeight = properties.ImageHeight;
				ImageWidth = properties.ImageWidth;
			}
			SpriteLeft = properties.SpriteLeft;
			SpriteTop = properties.SpriteTop;
			SpriteUrl = spriteUrl;
			SpriteCssClass = properties.SpriteCssClass;
			if(imageUrl == "" && !properties.EmptyImage.IsEmpty && AssignEmptyImage) {
				ImageProperties.CopyFrom(properties.EmptyImage);
				if(String.IsNullOrEmpty(ImageUrl) && !String.IsNullOrEmpty(ImageProperties.IconID)) {
					ImageWidth = Unit.Empty;
					ImageHeight = Unit.Empty;
				}
			}
		}
	}
	public class ImageControl<TEditor> : ImageDisplayControl 
		where TEditor : ASPxEditBase, IImageEdit, ISkinOwner {
		public ImageControl(TEditor control) {
			Control = control;
		}
		public ImageControl()
			: base() {
		}
		protected TEditor Control { get; set; }
		protected virtual string GetImageUrl() {
			return Control.GetImageUrl();
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Control, Image); 
			Control.GetControlStyle().AssignToControl(Image);
			PrepareControlHierarchyInternal();
			base.PrepareControlHierarchy();
		}
		protected void PrepareControlHierarchyInternal() {
			RenderUtils.SetVisibility(Image, Control.IsClientVisible(), true);
			OnClickScript = Control.GetOnClick();
			SetProperties(GetImageUrl(), Control.GetSpriteImageUrl(), Control.ImageEditProperties, null);
			RenderUtils.SetStringAttribute(Image, "onclick", OnClickScript);
			PrepareLoadingImage();
		}
		protected virtual void PrepareLoadingImage() {
			if(Control.ShowLoadingImage && !Control.DesignMode) {
				string backgroundImageUrl = "";
				string loadingImage = "";
				Image.Style.Remove("background-image");
				if(!string.IsNullOrEmpty(Control.BackgroundImage.ImageUrl)) {
					RenderUtils.AppendDefaultDXClassName(Image, Control.GetSysBackgroundCssClassName());
					backgroundImageUrl = Utils.UrlResolver.ResolveUrl(Control.BackgroundImage.ImageUrl);
				}
				if(!string.IsNullOrEmpty(Control.LoadingImageUrl)) {
					loadingImage = string.Format("url(\"{0}\")", Utils.UrlResolver.ResolveUrl(Control.LoadingImageUrl));
					Image.Style["background-image"] = loadingImage.Replace("\"", "'");
				}
				bool isOldIE = RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 9;
				RenderUtils.AppendDefaultDXClassName(Image, Control.GetLoadingImageCssClassName());
				string onASPxImageLoad = string.Format("ASPx.ASPxImageLoad.OnLoad(this,'{0}',{1},'{2}')", loadingImage, isOldIE ? 1 : 0, backgroundImageUrl);
				AssignClientEventHandlers(onASPxImageLoad);
			}
		}
		private void AssignClientEventHandlers(string onASPxImageLoad) {
			RenderUtils.SetStringAttribute(Image, "onload", onASPxImageLoad);
			RenderUtils.SetStringAttribute(Image, "onabort", onASPxImageLoad);
			RenderUtils.SetStringAttribute(Image, "onerror", onASPxImageLoad);
		}
	}
	public class LabelControl : ASPxInternalWebControl {
		public LabelControl(ASPxLabel edit) {
			Edit = edit;
		}
		protected internal ASPxLabel Edit { get; private set; }
		public string AssociatedControlID { get; set; }
		public virtual string Text { get; set; }
		public virtual string OnClick { get; set; }
		protected WebControl Label { get; private set; }
		protected LiteralControl TextControl { get; private set; }
		protected override void ClearControlFields() {
			Label = null;
			TextControl = null;
		}
		protected WebControl CreateMainElement() {
			HtmlTextWriterTag elementTag = (string.IsNullOrEmpty(AssociatedControlID) || Edit.IsAccessibilityAssociating)
				? HtmlTextWriterTag.Span
				: HtmlTextWriterTag.Label;
			return RenderUtils.CreateWebControl(elementTag);
		}
		protected override void CreateControlHierarchy() {
			Label = CreateMainElement();
			Controls.Add(Label);
			TextControl = RenderUtils.CreateLiteralControl();
			Label.Controls.Add(TextControl);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, Label);
			TextControl.Text = Text;
			TextControl.Visible = !string.IsNullOrEmpty(Text);
			if(!DesignMode) {
				if(IsEnabled())
					RenderUtils.SetStringAttribute(Label, "onclick", OnClick);
				if(!IsMvcRender() && !Edit.IsAccessibilityAssociating)
					RenderUtils.SetStringAttribute(Label, "for", AssociatedControlID);
				if(Edit.IsAccessibilityAssociating)
					RenderUtils.AppendDefaultDXClassName(Label, AccessibilityUtils.DefaultCursorCssClassName);
			}
		}
	}
	public class LabelEditControl : LabelControl {
		public LabelEditControl(ASPxLabel edit)
			: base(edit) { }
		public override string OnClick { get { return Edit.GetOnClick(); } }
		public override string Text { get { return Edit.GetText(); } }
		protected override void CreateControlHierarchy() {
			if(!DesignMode) {
				AssociatedControlID = Edit.GetAssociatedControlClientID();
			}
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Edit, this);
			Edit.GetControlStyle().AssignToControl(Label);
			Enabled = Edit.Enabled;
			base.PrepareControlHierarchy();
			RenderUtils.SetVisibility(Label, Edit.IsClientVisible(), true);
			Label.Height = Edit.GetHeight();
			Label.Width = Edit.GetWidth();
			var requireBlockRender = (!Label.Width.IsEmpty || !Label.Height.IsEmpty) && Edit.ClientVisible;
			if(requireBlockRender) {
				if(Browser.IsMozilla || Browser.IsNetscape || (Browser.IsFirefox && Browser.MajorVersion < 3))
					RenderUtils.SetStyleStringAttribute(Label, "display", "-moz-inline-box");
				else
					RenderUtils.SetStyleStringAttribute(Label, "display", "inline-block");
				RenderUtils.AllowEllipsisInText(Label, Edit.AllowEllipsisInText);
			}
		}
	}
}
