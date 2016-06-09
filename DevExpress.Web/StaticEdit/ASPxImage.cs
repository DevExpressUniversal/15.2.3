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

namespace DevExpress.Web {
	using System;
	using System.ComponentModel;
	using System.Text;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Collections.Generic;
	using DevExpress.Web.Internal;
	public interface IImageExportSettings {
		int Width { get; }
		int Height { get; }
		XtraPrinting.ImageSizeMode SizeMode { get; }
	}
	public abstract class ImageEditPropertiesBase : StaticEditProperties, IImageExportSettings {
		const int DefaultExportWidth = 60;
		const int DefaultExportHeight = 40;
		ExportBinaryImageSettings exportImageSettings;
		protected internal override EditorStyles CreateEditorStyles() {
			return new EditorStyles(this);
		}
		public ImageEditPropertiesBase()
			: base() {
		}
		public ImageEditPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseShowLoadingImage"),
#endif
		Category("Layout"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowLoadingImage {
			get { return GetBoolProperty("ShowLoadingImage", false); }
			set { SetBoolProperty("ShowLoadingImage", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseLoadingImageUrl"),
#endif
		Category("Layout"), DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string LoadingImageUrl {
			get { return GetStringProperty("LoadingImageUrl", ""); }
			set { SetStringProperty("LoadingImageUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseAlternateText"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string AlternateText {
			get { return GetStringProperty("AlternateText", ""); }
			set { SetStringProperty("AlternateText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseToolTip"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseDescriptionUrl"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string DescriptionUrl {
			get { return GetStringProperty("DescriptionUrl", ""); }
			set { SetStringProperty("DescriptionUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseEmptyImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties EmptyImage {
			get { return Images.ImageEmpty; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseImageAlign"),
#endif
		Category("Layout"), DefaultValue(ImageAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public ImageAlign ImageAlign {
			get { return (ImageAlign)GetEnumProperty("ImageAlign", ImageAlign.NotSet); }
			set { SetEnumProperty("ImageAlign", ImageAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseImageHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", Unit.Empty); }
			set { SetUnitProperty("ImageHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseImageWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", Unit.Empty); }
			set { SetUnitProperty("ImageWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseIsPng"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true),
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return GetBoolProperty("IsPng", false); }
			set { SetBoolProperty("IsPng", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseImageUrlFormatString"),
#endif
		DefaultValue("{0}"), AutoFormatDisable, NotifyParentProperty(true),
		Localizable(false)]
		public string ImageUrlFormatString {
			get { return GetStringProperty("ImageUrlFormatString", "{0}"); }
			set { SetStringProperty("ImageUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseAlternateTextField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string AlternateTextField {
			get { return GetStringProperty("AlternateTextField", ""); }
			set { SetStringProperty("AlternateTextField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseToolTipField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set { SetStringProperty("ToolTipField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseAlternateTextFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string AlternateTextFormatString {
			get { return GetStringProperty("AlternateTextFormatString", "{0}"); }
			set { SetStringProperty("AlternateTextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseToolTipFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ToolTipFormatString {
			get { return GetStringProperty("ToolTipFormatString", "{0}"); }
			set { SetStringProperty("ToolTipFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseDescriptionUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string DescriptionUrlField {
			get { return GetStringProperty("DescriptionUrlField", ""); }
			set { SetStringProperty("DescriptionUrlField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseDescriptionUrlFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string DescriptionUrlFormatString {
			get { return GetStringProperty("DescriptionUrlFormatString", "{0}"); }
			set { SetStringProperty("DescriptionUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesBaseExportImageSettings"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ExportBinaryImageSettings ExportImageSettings {
			get {
				if(exportImageSettings == null)
					exportImageSettings = new ExportBinaryImageSettings(this);
				return exportImageSettings;
			}
		}
		protected internal string SpriteCssClass {
			get { return GetStringProperty("SpriteCssClass", ""); }
			set {
				SetStringProperty("SpriteCssClass", "", value);
			}
		}		
		protected internal Unit SpriteLeft {
			get { return GetUnitProperty("SpriteLeft", Unit.Empty); }
			set {
				SetUnitProperty("SpriteLeft", Unit.Empty, value);
			}
		}
		protected internal Unit SpriteTop {
			get { return GetUnitProperty("SpriteTop", Unit.Empty); }
			set {
				SetUnitProperty("SpriteTop", Unit.Empty, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageEditPropertiesBase src = source as ImageEditPropertiesBase;
				if(src != null) {
					ShowLoadingImage = src.ShowLoadingImage;
					LoadingImageUrl = src.LoadingImageUrl;
					AlternateText = src.AlternateText;
					DescriptionUrl = src.DescriptionUrl;
					EmptyImage.Assign(src.EmptyImage);
					ImageAlign = src.ImageAlign;
					ImageHeight = src.ImageHeight;
					ImageWidth = src.ImageWidth;
					AlternateTextField = src.AlternateTextField;
					ToolTip = src.ToolTip;
					ToolTipField = src.ToolTipField;
					ToolTipFormatString = src.ToolTipFormatString;
					DescriptionUrlField = src.DescriptionUrlField;
					AlternateTextFormatString = src.AlternateTextFormatString;
					DescriptionUrlFormatString = src.DescriptionUrlFormatString;
					ImageUrlFormatString = src.ImageUrlFormatString;
					SpriteLeft = src.SpriteLeft;
					SpriteTop = src.SpriteTop;
					SpriteCssClass = src.SpriteCssClass;
					ExportImageSettings.Assign(src.ExportImageSettings);
				}
			} finally {
				EndUpdate();
			}
		}
		public override HorizontalAlign GetDisplayControlDefaultAlign() {
			return HorizontalAlign.Center;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(ExportImageSettings);
			return list.ToArray();
		}
		protected internal int GetExportHeight() {
			if(ExportImageSettings.Height > 0)
				return ExportImageSettings.Height;
			if(ImageHeight.Type == UnitType.Pixel && ImageHeight.Value > 0)
				return (int)ImageHeight.Value;
			return DefaultExportHeight;
		}
		protected internal int GetExportWidth() {
			if(ExportImageSettings.Width > 0)
				return ExportImageSettings.Width;
			if(ImageWidth.Type == UnitType.Pixel && ImageWidth.Value > 0)
				return (int)ImageWidth.Value;
			return DefaultExportWidth;
		}
		int IImageExportSettings.Width { get { return GetExportWidth(); } }
		int IImageExportSettings.Height { get { return GetExportHeight(); } }
		XtraPrinting.ImageSizeMode IImageExportSettings.SizeMode { get { return ExportImageSettings.SizeMode; } }
	}
	public class ImageEditProperties : ImageEditPropertiesBase, IImageEditProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesSpriteLeft"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public new Unit SpriteLeft {
			get { return base.SpriteLeft; }
			set { base.SpriteLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesSpriteTop"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public new Unit SpriteTop {
			get { return base.SpriteTop; }
			set { base.SpriteTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageEditPropertiesSpriteCssClass"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public new string SpriteCssClass {
			get { return base.SpriteCssClass; }
			set { base.SpriteCssClass = value; }
		}
		bool IImageEditProperties.ApplyImageAlignToDisplayControl { get { return true; } }
		public ImageEditProperties()
			: base() {
		}
		public ImageEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxImage();
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			if(args.DisplayText != null ||
			   (CommonUtils.IsNullValue(args.EditValue) && EmptyImage.IsEmpty))
				return base.CreateDisplayControlInstance(args);
			else {
				ImageDisplayControl imageDisplayControl = new ImageDisplayControl();
				string spriteUrl = (args.SkinOwner != null) ? args.SkinOwner.GetSpriteImageUrl() : string.Empty;
				imageDisplayControl.SetProperties(CommonUtils.ValueToString(args.EditValue), spriteUrl,
					this, args.ValueProvider);
				return imageDisplayControl;
			}
		}
	}
	public class ExportBinaryImageSettings : PropertiesBase {
		public ExportBinaryImageSettings() : base() { }
		public ExportBinaryImageSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("ExportBinaryImageSettingsWidth"),
#endif
		Category("ExportSettings"), AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int Width
		{
			get { return GetIntProperty("Width", 0); }
			set
			{
				CommonUtils.CheckNegativeValue(value, "Width");
				SetIntProperty("Width", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ExportBinaryImageSettingsHeight"),
#endif
			Category("ExportSettings"), AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int Height
		{
			get { return GetIntProperty("Height", 0); }
			set
			{
				CommonUtils.CheckNegativeValue(value, "Height");
				SetIntProperty("Height", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ExportBinaryImageSettingsSizeMode"),
#endif
		Category("ExportSettings"), AutoFormatDisable, NotifyParentProperty(true), DefaultValue(DevExpress.XtraPrinting.ImageSizeMode.Squeeze)]
		public DevExpress.XtraPrinting.ImageSizeMode SizeMode
		{
			get { return (DevExpress.XtraPrinting.ImageSizeMode)GetEnumProperty("SizeMode", DevExpress.XtraPrinting.ImageSizeMode.Squeeze); }
			set { SetEnumProperty("SizeMode", DevExpress.XtraPrinting.ImageSizeMode.Squeeze, value); }
		}
		public override void Assign(PropertiesBase source) {
			ExportBinaryImageSettings sourceSettings = source as ExportBinaryImageSettings;
			if(sourceSettings == null)
				return;
			Width = sourceSettings.Width;
			Height = sourceSettings.Height;
			SizeMode = sourceSettings.SizeMode;
		}
	}
	[Designer("DevExpress.Web.Design.ASPxImageBaseDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public abstract class ASPxImageBase : ASPxStaticEdit, IImageEdit {
		const string LoadingImageCssClass = "loadingImage";
		const string SysBackground = "backgroundSys";
		protected WebControl ImageControl {
			get; private set;
		}
		protected virtual EditorStyles Styles {
			get { return (EditorStyles)Properties.Styles; }
		}
		string IImageEdit.GetLoadingImageCssClassName() {
			return Styles.GetCssClassNamePrefix() + "-" + LoadingImageCssClass;
		}
		string IImageEdit.GetSysBackgroundCssClassName() {
			return Styles.GetCssClassNamePrefix() + "-" + SysBackground;
		}
		public ASPxImageBase()
			: base() {
		}
		protected override void RegisterStandaloneScriptBlocks() {
			RegisterIncludeScript(typeof(ASPxImageBase), ImageScriptResourceName);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseAlternateText"),
#endif
		DefaultValue(""), Category("Accessibility"), Bindable(true), Localizable(true), AutoFormatDisable]
		public string AlternateText {
			get { return Properties.AlternateText; }
			set { Properties.AlternateText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseToolTip"),
#endif
		DefaultValue(""), Category("Accessibility"), Bindable(true), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return Properties.ToolTip; }
			set { Properties.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseDescriptionUrl"),
#endif
		DefaultValue(""), Category("Accessibility"), Localizable(false), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string DescriptionUrl {
			get { return Properties.DescriptionUrl; }
			set { Properties.DescriptionUrl = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseEmptyImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties EmptyImage {
			get { return Properties.EmptyImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseIsPng"),
#endif
		DefaultValue(false), Localizable(false), Bindable(true), AutoFormatDisable,
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return Properties.IsPng; }
			set { Properties.IsPng = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseShowLoadingImage"),
#endif
		Category("Layout"), DefaultValue(false), AutoFormatEnable]
		public bool ShowLoadingImage {
			get { return Properties.ShowLoadingImage; }
			set { Properties.ShowLoadingImage = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseLoadingImageUrl"),
#endif
		DefaultValue(""), Category("Layout"), Localizable(false), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string LoadingImageUrl {
			get { return Properties.LoadingImageUrl; }
			set { Properties.LoadingImageUrl = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseImageAlign"),
#endif
		Category("Layout"), DefaultValue(ImageAlign.NotSet), AutoFormatEnable]
		public ImageAlign ImageAlign {
			get { return Properties.ImageAlign; }
			set { Properties.ImageAlign = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxImageBaseWidth")]
#endif
		public override Unit Width {
			get { return Properties.ImageWidth; }
			set { Properties.ImageWidth = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxImageBaseHeight")]
#endif
		public override Unit Height {
			get { return Properties.ImageHeight; }
			set { Properties.ImageHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new StaticEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new ImageEditPropertiesBase Properties {
			get { return (ImageEditPropertiesBase)base.Properties; }
		}
		IImageEditProperties IImageEdit.ImageEditProperties {
			get { return (IImageEditProperties) Properties; }
		}
		protected override bool IsDesignTimeDataBindingRequired() {
			return false;
		}
		protected abstract WebControl CreateImageControl();
		protected override bool HasSpriteCssFile() {
			return false;
		}
		protected override bool HasRenderCssFile() {
			return !string.IsNullOrEmpty(Caption) || ShowLoadingImage;
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.Image);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			style.CopyFrom(Styles.GetDefaultImageStyle());
			MergeDisableStyle(style);
		}
		protected virtual WebControl ImageContainer {
			get { return this; }
		}
		protected override void ClearControlFields() {
			ImageControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ImageControl = CreateImageControl();
			ImageContainer.Controls.Add(ImageControl);
		}
		string IImageEdit.GetImageUrl() { return GetImageUrl(); }
		string IImageEdit.GetOnClick() { return GetOnClick(); }
		protected internal abstract string GetImageUrl();
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("ImageUrl"), 
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxData(@"<{0}:ASPxImage runat=""server"" ShowLoadingImage=""true""></{0}:ASPxImage>"),
	Designer("DevExpress.Web.Design.ASPxImageDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxImage.bmp")
	]
	public class ASPxImage : ASPxImageBase {
		public ASPxImage()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageImageUrl"),
#endif
		DefaultValue(""), Localizable(false), Bindable(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string ImageUrl {
			get { return CommonUtils.ValueToString(Value); }
			set { Value = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageSpriteLeft"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit SpriteLeft {
			get { return Properties.SpriteLeft; }
			set { Properties.SpriteLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageSpriteTop"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit SpriteTop {
			get { return Properties.SpriteTop; }
			set { Properties.SpriteTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxImageSpriteCssClass"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string SpriteCssClass {
			get { return Properties.SpriteCssClass; }
			set { Properties.SpriteCssClass = value; }
		}
		public void AssignImageProperties(ImageProperties imageProperties) {
			ImageUrl = imageProperties.Url;
			AlternateText = imageProperties.AlternateText;
			ToolTip = imageProperties.ToolTip;
			DescriptionUrl = imageProperties.DescriptionUrl;
			if(!imageProperties.Width.IsEmpty)
				Width = imageProperties.Width;
			if(!imageProperties.Height.IsEmpty)
				Height = imageProperties.Height;
		}
		protected internal new ImageEditProperties Properties {
			get { return (ImageEditProperties)base.Properties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new ImageEditProperties(this);
		}
		protected override WebControl CreateImageControl() { return new ImageControl<ASPxImage>(this); }
		protected internal override string GetImageUrl() { return ImageUrl; }
		protected override bool HasSpriteCssFile() { return !string.IsNullOrEmpty(SpriteCssFilePath); }
		protected override string GetClientObjectClassName() {
			return "ASPxClientImage";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(string.IsNullOrEmpty(ImageUrl))
				stb.Append(localVarName + ".isEmpty = true;\n");
			if(!string.IsNullOrEmpty(EmptyImage.Url)){
				stb.Append(localVarName + ".emptyImageUrl = " + HtmlConvertor.ToScript(ResolveClientUrl(EmptyImage.Url)) + ";\n");
				if(!EmptyImage.Width.IsEmpty)
					stb.Append(localVarName + ".emptyImageWidth = " + HtmlConvertor.ToScript(EmptyImage.Width.ToString()) + ";\n");
				if(!EmptyImage.Height.IsEmpty)
					stb.Append(localVarName + ".emptyImageHeight = " + HtmlConvertor.ToScript(EmptyImage.Height.ToString()) + ";\n");
				if(!string.IsNullOrEmpty(EmptyImage.AlternateText))
					stb.Append(localVarName + ".emptyImageAlt = " + HtmlConvertor.ToScript(EmptyImage.AlternateText) + ";\n");
				if(!string.IsNullOrEmpty(EmptyImage.ToolTip))
					stb.Append(localVarName + ".emptyImageToolTip = " + HtmlConvertor.ToScript(EmptyImage.ToolTip) + ";\n");
				if(!Width.IsEmpty)
					stb.Append(localVarName + ".imageWidth = " + HtmlConvertor.ToScript(Width.ToString()) + ";\n");
				if(!Height.IsEmpty)
					stb.Append(localVarName + ".imageHeight = " + HtmlConvertor.ToScript(Height.ToString()) + ";\n");
				if(!string.IsNullOrEmpty(AlternateText))
					stb.Append(localVarName + ".imageAlt = " + HtmlConvertor.ToScript(AlternateText) + ";\n");
				if(!string.IsNullOrEmpty(ToolTip))
					stb.Append(localVarName + ".imageToolTip = " + HtmlConvertor.ToScript(ToolTip) + ";\n");
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	using System;
	using System.Web.UI.WebControls;
	public interface IImageEdit {
		string GetImageUrl();
		string GetLoadingImageCssClassName();
		string GetOnClick();
		string GetSysBackgroundCssClassName();
		IImageEditProperties ImageEditProperties { get; }
		bool ShowLoadingImage { get; }
		string LoadingImageUrl { get; }
	}
	public interface IImageEditProperties {
		string AlternateText { get; set; }
		string AlternateTextField { get; set; }
		string AlternateTextFormatString { get; set; }
		string DescriptionUrl { get; set; }
		string DescriptionUrlField { get; set; }
		string DescriptionUrlFormatString { get; set; }
		ImageProperties EmptyImage { get; }
		ImageAlign ImageAlign { get; set; }
		Unit ImageHeight { get; set; }
		string ImageUrlFormatString { get; set; }
		Unit ImageWidth { get; set; }
		[Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		bool IsPng { get; set; }
		bool ApplyImageAlignToDisplayControl { get; }
		string SpriteCssClass { get; set; }
		Unit SpriteLeft { get; set; }
		Unit SpriteTop { get; set; }
		string ToolTip { get; set; }
		string ToolTipField { get; set; }
		string ToolTipFormatString { get; set; }
	}
}
