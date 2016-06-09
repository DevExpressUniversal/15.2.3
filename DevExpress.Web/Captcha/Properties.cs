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

using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Captcha {
	public enum ControlPosition { Left, Right, Top, Bottom }
	public enum CaptchaFontStyle { Regular, Bold, Italic }
	public class RefreshButtonProperties : PropertiesBase, IPropertiesOwner {
		ImagePropertiesEx image;
		public RefreshButtonProperties(ASPxCaptcha owner)
			: base(owner) {
			this.image = new ImagePropertiesEx(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImagePropertiesEx Image {
			get { return image; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesImagePosition"),
#endif
		NotifyParentProperty(true), DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", ImagePosition.Left); }
			set {
				SetEnumProperty("ImagePosition", ImagePosition.Left, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesShowImage"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatEnable]
		public bool ShowImage {
			get { return GetBoolProperty("ShowImage", true); }
			set {
				SetBoolProperty("ShowImage", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.Captcha_RefreshText),
		Localizable(true), AutoFormatEnable]
		public string Text {
			get {
				return GetStringProperty("Text",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_RefreshText));
			}
			set {
				SetStringProperty("Text",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_RefreshText), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesPosition"),
#endif
		NotifyParentProperty(true), DefaultValue(ControlPosition.Bottom), AutoFormatEnable]
		public ControlPosition Position {
			get { return (ControlPosition)GetEnumProperty("Position", ControlPosition.Bottom); }
			set {
				SetEnumProperty("Position", ControlPosition.Bottom, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RefreshButtonPropertiesVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				RefreshButtonProperties src = source as RefreshButtonProperties;
				if(src == null)
					return;
				Image.Assign(src.Image);
				ImagePosition = src.ImagePosition;
				ShowImage = src.ShowImage;
				Text = src.Text;
				Position = src.Position;
				Visible = src.Visible;
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Image });
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class CaptchaTextBoxProperties : PropertiesBase, IPropertiesOwner {
		public CaptchaTextBoxProperties(ASPxCaptcha owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxPropertiesNullText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable,
		Localizable(true)]
		public string NullText {
			get { return GetStringProperty("NullText", string.Empty); }
			set {
				SetStringProperty("NullText", string.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxPropertiesVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxPropertiesPosition"),
#endif
		NotifyParentProperty(true), DefaultValue(ControlPosition.Right), AutoFormatEnable]
		public ControlPosition Position {
			get { return (ControlPosition)GetEnumProperty("Position", ControlPosition.Right); }
			set {
				SetEnumProperty("Position", ControlPosition.Right, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxPropertiesShowLabel"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatEnable]
		public bool ShowLabel {
			get { return GetBoolProperty("ShowLabel", true); }
			set {
				SetBoolProperty("ShowLabel", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaTextBoxPropertiesLabelText"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue(StringResources.Captcha_DefaultTextBoxLabelText),
		AutoFormatEnable]
		public string LabelText {
			get {
				return GetStringProperty("LabelText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultTextBoxLabelText));
			}
			set {
				SetStringProperty("LabelText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultTextBoxLabelText), value);
				Changed();
			}
		}
		protected internal void AssignToControl(ASPxTextBox textBox) {
			textBox.Visible = Visible;
			textBox.NullText = NullText;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CaptchaTextBoxProperties src = source as CaptchaTextBoxProperties;
				if(src == null)
					return;
				NullText = src.NullText;
				Visible = src.Visible;
				ShowLabel = src.ShowLabel;
				LabelText = src.LabelText;
				Position = src.Position;
			} finally {
				EndUpdate();
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class CaptchaImageProperties : PropertiesBase, IPropertiesOwner {
		protected internal const string DefaultFontFamily = "Times New Roman";
		const int DefaultWidth = 200;
		const int DefaultHeight = 80;
		const int DefaultBorderWidth = 2;
		const int MinWidth = 10;
		const int MinHeight = 25;
		const string StrDefaultForegroundColor = "#000000";
		const string StrDefaultBackgroundColor = "#eeeeee";
		const string StrDefaultBorderColor = "#a8a8a8";
		BackgroundImage backgroundImage;
		public CaptchaImageProperties(ASPxCaptcha owner)
			: base(owner) {
			this.backgroundImage = new BackgroundImage();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal ASPxCaptcha Captcha {
			get { return (ASPxCaptcha)Owner; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultWidth),
		AutoFormatEnable]
		public int Width {
			get { return GetIntProperty("Width", DefaultWidth); }
			set {
				CommonUtils.CheckMinimumValue(value, MinWidth, "Width");
				SetIntProperty("Width", DefaultWidth, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultHeight),
		AutoFormatEnable]
		public int Height {
			get { return GetIntProperty("Height", DefaultHeight); }
			set {
				CommonUtils.CheckMinimumValue(value, MinHeight, "Height");
				SetIntProperty("Height", DefaultHeight, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesBorderWidth"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBorderWidth),
		AutoFormatEnable]
		public int BorderWidth {
			get { return GetIntProperty("BorderWidth", DefaultBorderWidth); }
			set {
				CommonUtils.CheckMinimumValue(value, 0, "BorderWidth");
				SetIntProperty("BorderWidth", DefaultBorderWidth, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesForegroundColor"),
#endif
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		DefaultValue(typeof(Color), StrDefaultForegroundColor), AutoFormatEnable]
		public Color ForegroundColor {
			get { return GetColorProperty("ForegroundColor", Color.FromArgb(0x00, 0x00, 0x00)); }
			set {
				SetColorProperty("ForegroundColor", Color.FromArgb(0x00, 0x00, 0x00), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesBackgroundColor"),
#endif
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		DefaultValue(typeof(Color), StrDefaultBackgroundColor), AutoFormatEnable]
		public Color BackgroundColor {
			get { return GetColorProperty("BackgroundColor", Color.FromArgb(0xee, 0xee, 0xee)); }
			set {
				SetColorProperty("BackgroundColor", Color.FromArgb(0xee, 0xee, 0xee), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesBorderColor"),
#endif
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		DefaultValue(typeof(Color), StrDefaultBorderColor), AutoFormatEnable]
		public Color BorderColor {
			get { return GetColorProperty("BorderColor", Color.FromArgb(0xa8, 0xa8, 0xa8)); }
			set {
				SetColorProperty("BorderColor", Color.FromArgb(0xa8, 0xa8, 0xa8), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesFontFamily"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultFontFamily),
		Editor("DevExpress.Web.Design.FontFamilyEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		AutoFormatDisable, Localizable(false)]
		public string FontFamily {
			get { return GetStringProperty("FontFamily", DefaultFontFamily); }
			set {
				SetStringProperty("FontFamily", DefaultFontFamily, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesFontStyle"),
#endif
		NotifyParentProperty(true), DefaultValue(CaptchaFontStyle.Italic), AutoFormatEnable]
		public CaptchaFontStyle FontStyle {
			get { return (CaptchaFontStyle)GetEnumProperty("FontStyle", CaptchaFontStyle.Italic); }
			set {
				SetEnumProperty("FontStyle", CaptchaFontStyle.Italic, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesAlternateText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.Captcha_DefaultImageAlternateText),
		Localizable(true), AutoFormatDisable]
		public string AlternateText {
			get {
				return GetStringProperty("AlternateText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultImageAlternateText));
			}
			set {
				SetStringProperty("AlternateText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultImageAlternateText), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesToolTip"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true), AutoFormatDisable]
		public virtual string ToolTip {
			get { return GetStringProperty("ToolTip", string.Empty); }
			set {
				SetStringProperty("ToolTip", string.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesBackgroundImage"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public virtual BackgroundImage BackgroundImage {
			get { return this.backgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaImagePropertiesBinaryStorageMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(false), DefaultValue(BinaryStorageMode.Default)]
		public BinaryStorageMode BinaryStorageMode
		{
			get { return (BinaryStorageMode)GetEnumProperty("StorageMode", BinaryStorageMode.Default); }
			set { SetEnumProperty("StorageMode", BinaryStorageMode.Default, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			BeginUpdate();
			try {
				base.Assign(source);
				CaptchaImageProperties src = source as CaptchaImageProperties;
				if(src == null)
					return;
				Width = src.Width;
				Height = src.Height;
				ForegroundColor = src.ForegroundColor;
				BackgroundColor = src.BackgroundColor;
				BorderColor = src.BorderColor;
				BorderWidth = src.BorderWidth;
				FontFamily = src.FontFamily;
				FontStyle = src.FontStyle;
				AlternateText = src.AlternateText;
				ToolTip = src.ToolTip;
				BinaryStorageMode = src.BinaryStorageMode;
				BackgroundImage.Assign(src.BackgroundImage);
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				 new IStateManager[] { BackgroundImage });
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class CaptchaSettingsLoadingPanel : SettingsLoadingPanel {
		public CaptchaSettingsLoadingPanel(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), AutoFormatEnable, DefaultValue(""), Localizable(true)]
		public override string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set {
				SetStringProperty("Text", string.Empty, value);
				Changed();
			}
		}
	}
}
