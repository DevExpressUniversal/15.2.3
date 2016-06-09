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
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class HyperLinkProperties : StaticEditProperties {
		public HyperLinkProperties()
			: base() {
		}
		public HyperLinkProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), Bindable(true), UrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(UITypeEditor)), AutoFormatDisable]
		public string ImageUrl {
			get { return GetStringProperty("ImageUrl", ""); }
			set {
				SetStringProperty("ImageUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesImageHeight"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", Unit.Empty); }
			set { SetUnitProperty("ImageHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesImageWidth"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", Unit.Empty); }
			set { SetUnitProperty("ImageWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true),
		Bindable(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesTarget"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesImageUrlField"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set { SetStringProperty("ImageUrlField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesNavigateUrlFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue("{0}"), Localizable(false), AutoFormatDisable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", "{0}"); }
			set { SetStringProperty("NavigateUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesTextField"),
#endif
		Category("Data"), NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string TextField {
			get { return GetStringProperty("TextField", ""); }
			set { SetStringProperty("TextField", "", value); }
		}
		bool ShouldSerializeDisplayFormatString() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesDisplayFormatString"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue("{0}"), AutoFormatDisable]
		public override string DisplayFormatString {
			get { return TextFormatString; }
			set { TextFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HyperLinkPropertiesTextFormatString"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue("{0}"), AutoFormatDisable]
		public string TextFormatString {
			get { return GetStringProperty("TextFormatString", "{0}"); }
			set { SetStringProperty("TextFormatString", "{0}", value); }
		}
		[AutoFormatEnable, DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowEllipsisInText {
			get { return base.AllowEllipsisInText; }
			set { base.AllowEllipsisInText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				string prevDisplayFormatString = DisplayFormatString;
				base.Assign(source);
				DisplayFormatString = prevDisplayFormatString; 
				HyperLinkProperties src = source as HyperLinkProperties;
				if(src != null) {
					ImageUrl = src.ImageUrl;
					ImageHeight = src.ImageHeight;
					ImageWidth = src.ImageWidth;
					Text = src.Text;
					Target = src.Target;
					ImageUrlField = src.ImageUrlField;
					NavigateUrlFormatString = src.NavigateUrlFormatString;
					TextField = src.TextField;
					TextFormatString = src.TextFormatString;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxTextBox();
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue) && string.IsNullOrEmpty(Text))
				return base.CreateDisplayControlInstance(args);
			else {
				HyperLinkDisplayControl hyperLink = new HyperLinkDisplayControl();
				hyperLink.NavigateUrl = GetNavigateUrl(args);
				hyperLink.Target = Target;
				hyperLink.Text = RenderUtils.CheckEmptyRenderText(GetDisplayText(args));
				hyperLink.ImageAltText = GetDisplayTextCore(args, false);
				hyperLink.ImageUrl = GetDisplayImageUrl(args);
				hyperLink.ControlStyle = GetControlStyle();
				hyperLink.ImageHeight = ImageHeight;
				hyperLink.ImageWidth = ImageWidth;
				return hyperLink;
			}
		}
		private AppearanceStyleBase controlStyle;
		private AppearanceStyleBase GetControlStyle() {
			if(this.controlStyle == null || !UseCachedObjects()) {
				this.controlStyle = new AppearanceStyleBase();
				this.controlStyle.CopyFrom(Styles.GetDefaultHyperlinkStyle());
				MergeParentSkinOwnerControlStyle(this.controlStyle);
				this.controlStyle.CopyFrom(Styles.Hyperlink);
				this.controlStyle.CopyFrom(Styles.Style);
			}
			return this.controlStyle;
		}
		public string GetNavigateUrl(CreateDisplayControlArgs args) {
			return !CommonUtils.IsNullValue(args.EditValue) ?
				string.Format(NavigateUrlFormatString, CommonUtils.ValueToString(args.EditValue)) : "";
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue) && string.IsNullOrEmpty(this.Text))
				return base.GetDisplayTextCore(args, encode);
			else {
				object editValue = "";
				if(args.ValueProvider != null && !string.IsNullOrEmpty(TextField)) {
					object textValue = args.ValueProvider.GetValue(TextField);
					if(textValue != null)
						editValue = textValue; 
				}
				else if(!string.IsNullOrEmpty(Text))
					editValue = Text;
				else
					editValue = args.EditValue;
				if(encode && (editValue is string))
					editValue = HttpUtility.HtmlEncode((string)editValue);
				return string.Format(CommonUtils.GetFormatString(TextFormatString), editValue); 
			}
		}
		protected string GetDisplayImageUrl(CreateDisplayControlArgs args) {
			string ret = "";
			if(args.ValueProvider != null && !string.IsNullOrEmpty(ImageUrlField)) {
				object imageUrlValue = args.ValueProvider.GetValue(ImageUrlField);
				if(imageUrlValue != null)
					ret = imageUrlValue.ToString();
			}
			else if(!string.IsNullOrEmpty(ImageUrl))
				ret = ImageUrl;
			return ret;
		}
		public override string GetExportNavigateUrl(CreateDisplayControlArgs args) {
			return GetNavigateUrl(args);
		}
		public override object GetExportValue(CreateDisplayControlArgs args) {
			return null;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("Text"), ControlValueProperty("NavigateUrl"),
	ToolboxData("<{0}:ASPxHyperLink runat=\"server\" Text=\"ASPxHyperLink\" />"),
	DataBindingHandler("DevExpress.Web.Design.HyperLinkDataBindingHandler, " + AssemblyInfo.SRAssemblyWebDesignFull),
	Designer("DevExpress.Web.Design.ASPxHyperLinkDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxHyperLink.bmp")
	]
	public class ASPxHyperLink : ASPxStaticEdit {
		private HyperLinkControl hyperLink = null;
		public ASPxHyperLink()
			: base() {
		}
		protected internal new HyperLinkProperties Properties {
			get { return (HyperLinkProperties)base.Properties; }
		}
		[AutoFormatDisable, Category("Layout"), DefaultValue(false)]
		public bool AllowEllipsisInText {
			get { return Properties.AllowEllipsisInText; }
			set { Properties.AllowEllipsisInText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkEncodeHtml"),
#endif
		Category("Behavior"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatDisable]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkImageUrl"),
#endif
		DefaultValue(""), Localizable(false), Bindable(true), UrlProperty, AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(UITypeEditor))]
		public string ImageUrl {
			get { return Properties.ImageUrl; }
			set { Properties.ImageUrl = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkImageHeight"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit ImageHeight {
			get { return Properties.ImageHeight; }
			set { Properties.ImageHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkImageWidth"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit ImageWidth {
			get { return Properties.ImageWidth; }
			set { Properties.ImageWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkNavigateUrl"),
#endif
		DefaultValue(""), Localizable(false), Bindable(true), UrlProperty, AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(UITypeEditor))]
		public string NavigateUrl {
			get { return CommonUtils.ValueToString(Value); }
			set {
				Value = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkText"),
#endif
		DefaultValue(""), Localizable(true), Bindable(true), AutoFormatDisable]
		public string Text {
			get { return Properties.Text; }
			set {
				if(HasControls())
					Controls.Clear();
				Properties.Text = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkTarget"),
#endif
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return Properties.Target; }
			set { Properties.Target = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkWrap"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean Wrap {
			get { return ((AppearanceStyle)ControlStyle).Wrap; }
			set { ((AppearanceStyle)ControlStyle).Wrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHyperLinkRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxHyperLinkControls")]
#endif
		public new ControlCollection Controls {
			get { return HyperLink.GetControlCollection(); }
		}
		protected new ControlCollection ControlsBase {
			get { return base.Controls; }
		}
		protected HyperLinkControl HyperLink {
			get {
				if(hyperLink == null)
					hyperLink = new HyperLinkControl(this);
				return hyperLink;
			}
		}
		protected override void ClearControlFields() {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ControlsBase.Add(HyperLink);
		}
		protected override void PrepareControlHierarchy() {
			hyperLink.Text = EncodeHtml ? HtmlEncode(Text) : Text;
			hyperLink.NavigateUrl = NavigateUrl;
			hyperLink.Target = Target;
			hyperLink.ImageUrl = ImageUrl;
			hyperLink.ImageWidth = ImageWidth;
			hyperLink.ImageHeight = ImageHeight;
			hyperLink.ImageAltText = Text;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new HyperLinkProperties(this);
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.GetDefaultHyperlinkStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(RenderStyles.Hyperlink);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || AllowEllipsisInText;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(AllowEllipsisInText)
				stb.AppendFormat("{0}.enableEllipsis=true;\n", localVarName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientHyperLink";
		}
	}
}
