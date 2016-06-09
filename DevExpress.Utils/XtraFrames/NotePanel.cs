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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils.Frames {
	internal class NotePanelSizes {
		int width, textHeight, captionBorderHeight, captionBorderWidth, substrateBorderHeight, substrateBorderWidth;
		public NotePanelSizes(int width) {
			this.width = width;
			textHeight = captionBorderHeight = captionBorderWidth = substrateBorderHeight = substrateBorderWidth = 0;
		}
		public int TextHeight { get { return textHeight;} set { textHeight = value; }}
		public int CaptionBorderHeight { get { return captionBorderHeight;} set { captionBorderHeight = value; }}
		public int CaptionBorderWidth { get { return captionBorderWidth;} set { captionBorderWidth = value; }}
		public int SubstrateBorderHeight { get { return substrateBorderHeight;} set { substrateBorderHeight = value; }}
		public int SubstrateBorderWidth { get { return substrateBorderWidth;} set { substrateBorderWidth = value; }}
		public int BorderHeight { get { return CaptionBorderHeight + SubstrateBorderHeight + width * 2;}}
		public int BorderWidth { get { return CaptionBorderWidth + SubstrateBorderWidth + width * 2;}}
	}
	[ToolboxItem(false)]
	public class FrameControl : Control, ISupportLookAndFeel {
		string text = "";
		protected StringFormat TextStringFormat;
		BorderStyles captionBorderStyle;
		Color backColor2;
		LinearGradientMode gradientMode;
		bool parentAutoHeight;
		DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel;
		public FrameControl() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			TextStringFormat = new StringFormat();
			TextStringFormat.Trimming = StringTrimming.EllipsisCharacter;
			backColor2 = Color.Empty;
			gradientMode = LinearGradientMode.BackwardDiagonal;
			captionBorderStyle = BorderStyles.NoBorder;
			parentAutoHeight = false;
			this.TabStop = false;
			this.ForeColor = Color.Black;
			ResetFont();
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DevExpress.LookAndFeel.UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected override void Dispose(bool disposing) {
			if(this.lookAndFeel != null) {
				lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				LookAndFeel.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			this.Invalidate();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[DefaultValue(BorderStyles.NoBorder)]
		public BorderStyles BorderStyle{
			get { return captionBorderStyle; }
			set { 
				captionBorderStyle = value; 
				BeginResize();
			}
		}
		protected virtual void ResetBackColor2() {
			BackColor2 = Color.Empty;
		}
		protected virtual bool ShouldSerializeBackColor2() {
			return BackColor2 != Color.Empty;
		}
		public virtual Color BackColor2 {
			get { return backColor2; }
			set { 
				backColor2 = value; 
				BeginResize();
			}
		}
		[DefaultValue(LinearGradientMode.BackwardDiagonal)]
		public virtual LinearGradientMode GradientMode {
			get { return gradientMode; }
			set { 
				gradientMode = value; 
				BeginResize();
			}
		}
		public override void ResetBackColor() {
			BackColor = SystemColors.Info;
		}
		protected virtual bool ShouldSerializeBackColor() {
			return BackColor != SystemColors.Info;
		}
		public override Color BackColor {
			get { return base.BackColor; }
			set { 
				base.BackColor = value; 
				BeginResize();
			}
		}
		public override void ResetFont() { Font = null;	}
		protected virtual bool ShouldSerializeFont() { return !Font.Equals(AppearanceObject.DefaultFont); }
		public override Font Font {
			get { return base.Font; }
			set { 
				if(value == null) value = AppearanceObject.DefaultFont;
				base.Font = value; 
				BeginResize();
			}
		}
		[DefaultValue(false)]
		public virtual bool ParentAutoHeight {
			get { return parentAutoHeight; }
			set { 
				parentAutoHeight = value; 
				BeginResize();
			}
		}
		[DefaultValue("")]	
		public override string Text {
			get { return text; }
			set { 
				text = value; 
				BeginResize();
			}
		}
		public void SetText(string text) {
			this.text = text;
			BeginResize();
		}
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string Caption {
			get { return Text; }
			set { Text = value; }
		}
		protected bool NeedResize = false;
		protected void BeginResize() {
			BeginResize(true);
		}
		protected void BeginResize(bool needRefresh) {
			NeedResize = true;
			if(needRefresh)
				Refresh();
			else
				Invalidate();
		}
		protected void EndResize() { NeedResize = false; }
		protected virtual bool IsParentResize {
			get {
				if(Dock == DockStyle.Fill && (Parent is Panel || Parent is XtraEditors.XtraPanel) && ParentAutoHeight) return true; 
				return false;
			}
		}
		protected virtual void ResizeParent() {
			Control p = Parent as Control; 
			int hCaption = p.Height - p.DisplayRectangle.Height;
			p.ClientSize = new Size(p.ClientSize.Width, this.Height + hCaption);
		}
		protected override void OnResize(EventArgs e) {
			if(Dock != DockStyle.Fill) 
				BeginResize(false);
			else if(IsParentResize) {
				BeginResize();
				return;
			}
			Invalidate();
		}
		public bool IgnoreChildren {
			get { return true; }
		}
	}
	public class FrameHelper {
		public enum Language { CS, VB }
		public enum SkinDefinitionReason { Default, Rules }
		public static bool IsDarkSkin(UserLookAndFeel lf) {
			return (lf.ActiveStyle == ActiveLookAndFeelStyle.Skin && IsDarkSkin(lf.ActiveSkinName));
		}
		public static bool IsDarkSkin(UserLookAndFeel lf, SkinDefinitionReason reason) {
			return (lf.ActiveStyle == ActiveLookAndFeelStyle.Skin && IsDarkSkin(lf.ActiveSkinName, reason));
		}
		public static bool IsDarkSkin(string skinName) {
			return IsDarkSkin(skinName, SkinDefinitionReason.Default);
		}
		public static bool IsDarkSkin(string skinName, SkinDefinitionReason reason) {
			bool ret = !string.IsNullOrEmpty(skinName) && ";Darkroom;High Contrast;Sharp;Sharp Plus;Pumpkin;Dark Side;Office 2010 Black;DevExpress Dark Style;Blueprint;Metropolis Dark;Visual Studio 2013 Dark;Office 2016 Dark;".Contains(string.Concat(";", skinName, ";"));
			if(ret && reason == SkinDefinitionReason.Rules)
				ret = ret && !"Office 2010 Black".Equals(skinName) && !"Sharp".Equals(skinName);
			return ret;
		}
		public static Language GetLanguage(Assembly asm) {
			foreach(Type type in asm.GetTypes()) {
				if(IsVBSpecificType(type))
					return Language.VB;
			}
			return Language.CS;
		}
		static bool IsVBSpecificType(Type type) {
			Attribute[] attributes = Attribute.GetCustomAttributes(type);
			for(int i = 0; i < attributes.Length; i++) {
				if(IsVBSpecificAttribute(attributes[i]))
					return true;
			}
			return false;
		}
		static bool IsVBSpecificAttribute(Attribute attribute) {
			string attributeName = attribute.GetType().Name;
			return attributeName == "HideModuleNameAttribute" || attributeName == "StandardModuleAttribute";
		}
	}
	public class ApplicationCaption8_1 : ApplicationCaption {
		Color lightColor = Color.FromArgb(220, 220, 220), darkColor = Color.FromArgb(54, 54, 54);
		public ApplicationCaption8_1() : base() {
			ForeColor = darkColor;
			this.LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			CheckForeColor();
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			CheckForeColor();
		}
		Color GetDefaultColor() {
			if(FrameHelper.IsDarkSkin(this.LookAndFeel))
				return lightColor;
			return darkColor;
		}
		void CheckForeColor() {
			if(ForeColor == darkColor || ForeColor == lightColor)
				ForeColor = GetDefaultColor();
		}
		static Image fImageLogoEx = null;
		static Image fImageLogoEx_light = null;
		public static Image GetImageLogoEx(UserLookAndFeel lf) {
			if(fImageLogoEx == null)
				fImageLogoEx = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.dx-logo.png", typeof(ApplicationCaption).Assembly);
			if(fImageLogoEx_light == null)
				fImageLogoEx_light = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.dx-logo-light.png", typeof(ApplicationCaption).Assembly);
			return FrameHelper.IsDarkSkin(lf) ? fImageLogoEx_light : fImageLogoEx;
		}
		public override void ResetForeColor() {
			ForeColor = GetDefaultColor();
		}
		protected override bool ShouldSerializeForeColor() {
			return ForeColor != GetDefaultColor();
		}
		protected override Image DXLogo {
			get { return GetImageLogoEx(this.LookAndFeel); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor2 {
			get { return base.BackColor2; }
			set { }
		}
		static Font defaultFont3 = null;
		static Font defaultFont4 = null;
		static Font DefaultFont3 {
			get {
				if(defaultFont3 == null) defaultFont3 = CreateDefaultFont("Tahoma", 25, FontStyle.Regular);
				return defaultFont3;
			}
		}
		static Font DefaultFont4 {
			get {
				if(defaultFont4 == null) defaultFont4 = CreateDefaultFont("Tahoma", 12, FontStyle.Regular);
				return defaultFont4;
			}
		}
		protected override bool ShouldSerializeFont() { return !Font.Equals(DefaultFont3); }
		public override Font Font {
			get { return base.Font; }
			set {
				if(value == null) value = DefaultFont3;
				base.Font = value;
				BeginResize();
			}
		}
		protected override bool ShouldSerializeFont2() { return !Font2.Equals(DefaultFont4); }
		public override Font Font2 {
			get { return base.Font2; }
			set {
				if(value == null) value = DefaultFont4;
				base.Font2 = value;
				BeginResize();
			}
		}
		protected override int deltaX { get { return 15; } }
		protected override int CaptionFontHeight { get { return Font.Height + 6; } }
		protected override int DXLogoHeight { get { return DXLogo.Height + 6; } }
		protected override void FillBackground(GraphicsCache cache, Rectangle r) {
			if(LookAndFeel.ActiveLookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				SkinElement element = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinForm];
				SkinElementInfo info = new SkinElementInfo(element, r);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
			else {
				cache.Graphics.FillRectangle(SystemBrushes.Control, r);
			}
		}
		string GetText2() {
			if(Text2.Length > 0 && Text2[0] != '(')
				return string.Format("({0})", Text2);
			return Text2;
		}
		protected override void DrawCaptions(GraphicsCache cache, Rectangle r, int textLeft) {
			using(SolidBrush foreBrush = new SolidBrush(this.ForeColor)) {
				int textTop = (this.Height - Font.Height) / 2 - 1;
				r = new Rectangle(textLeft, textTop, this.Width - textLeft - DXLogo.Width - deltaX, Font.Height);
				cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				if(r.Width > 0)
					cache.Graphics.DrawString(Text, this.Font, foreBrush, r, TextStringFormat);
				int textWidth1 = (int)cache.Graphics.MeasureString(Text, this.Font, new PointF(0, 0), TextStringFormat).Width;
				r = new Rectangle(textLeft + textWidth1, textTop + (Font.Height - Font2.Height) - 5, this.Width - textLeft - DXLogo.Width - deltaX - textWidth1, Font2.Height);
				if(r.Width > 0)
					cache.Graphics.DrawString(GetText2(), this.Font2, foreBrush, r, TextStringFormat);
			}
		}
		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = GetNewText(value);
				base.Text2 = GetNewText2(value);
			}
		}
		string GetNewText2(string value) {
			if (String.IsNullOrEmpty(value))
				return String.Empty;
			int n = value.IndexOf('(');
			if(n == -1) return "";
			return value.Substring(n);
		}
		string GetNewText(string value) {
			if (String.IsNullOrEmpty(value))
				return String.Empty;
			int n = value.IndexOf('(');
			if(n == -1) return value;
			return value.Substring(0, n);
		}
	}
	public class ApplicationCaption : FrameControl {
		Image image;
		Font font2;
		string text2;
		bool showLogo;
		public ApplicationCaption() : base() {
			image = null;
			base.BackColor = defaultBackColor;
			base.BackColor2 = defaultBackColor2;
			base.GradientMode = LinearGradientMode.Vertical;
			base.ForeColor = Color.White;
			text2 = "";
			showLogo = true;
			ResetFont2();
		}
		protected static Image fImageLogo = null;
		public static Image ImageLogo {
			get {
				if(fImageLogo == null)
					fImageLogo = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.logo.png", typeof(ApplicationCaption).Assembly);
				return fImageLogo;
			}
		}
		protected virtual Image DXLogo {
			get { return ImageLogo; }
		}
		static Font defaultFont1 = null;
		static Font defaultFont2 = null;
		static string defaultFontName = "Arial Narrow";
		static Font DefaultFont1 { 
			get { 
				if(defaultFont1 == null) defaultFont1 = CreateDefaultFont(defaultFontName, 18, FontStyle.Bold);
				if(defaultFont1.Name != defaultFontName) defaultFont1 = CreateDefaultFont(AppearanceObject.DefaultFont.Name, 16, FontStyle.Bold);
				return defaultFont1;
			}
		}
		static Font DefaultFont2 { 
			get { 
				if(defaultFont2 == null) defaultFont2 = CreateDefaultFont(defaultFontName, 10, FontStyle.Regular);
				if(defaultFont2.Name != defaultFontName) defaultFont2 = CreateDefaultFont(AppearanceObject.DefaultFont.Name, 9, FontStyle.Regular);
				return defaultFont2;
			}
		}
		public static Font CreateDefaultFont(string name, int fontSize, FontStyle fontStyle) {
			try {
				return new Font(new FontFamily(name), fontSize, fontStyle);
			}
			catch { }
			return Control.DefaultFont;
		}
		Color defaultBackColor = Color.FromArgb(255, 195, 113);
		Color defaultBackColor2 = Color.FromArgb(255, 131, 12);
		public override void ResetBackColor() { BackColor = defaultBackColor; }
		protected override bool ShouldSerializeBackColor() { return BackColor != defaultBackColor; }
		protected override void ResetBackColor2() { BackColor2 = defaultBackColor2;	}
		protected override bool ShouldSerializeBackColor2() { return BackColor2 != defaultBackColor2; }
		public void ShowLogo(bool show) {
			showLogo = show;
			this.Invalidate();
		}
		[DefaultValue(LinearGradientMode.Vertical)]
		public override LinearGradientMode GradientMode {
			get { return base.GradientMode; }
			set { 
				base.GradientMode = value; 
				BeginResize();
			}
		}
		public override void ResetForeColor() {
			ForeColor = Color.White;
		}
		protected virtual bool ShouldSerializeForeColor() {
			return ForeColor != Color.White;
		}
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { 
				base.ForeColor = value; 
				BeginResize();
			}
		}
		protected override bool ShouldSerializeFont() { return !Font.Equals(DefaultFont1); }
		public override Font Font {
			get { return base.Font; }
			set { 
				if(value == null) value = DefaultFont1;
				base.Font = value; 
				BeginResize();
			}
		}
		protected virtual void ResetFont2() { Font2 = null;	}
		protected virtual bool ShouldSerializeFont2() { return !Font2.Equals(DefaultFont2); }
		public virtual Font Font2 {
			get { return font2; }
			set { 
				if(value == null) value = DefaultFont2;
				font2 = value; 
				BeginResize();
			}
		}
		[DefaultValue("")]
		public string Text2 {
			get { return text2; }
			set {
				text2 = value;
				BeginResize();
			}
		}
		protected virtual void ResetImage() {
			Image = null;
		}
		protected virtual bool ShouldSerializeImage() {
			return Image != null;
		}
		public Image Image {
			get { return image; }
			set { 
				image = value; 
				BeginResize();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ParentAutoHeight {
			get { return false; }
			set {}
		}
		protected virtual int deltaX { get { return 10; }}
		protected virtual int deltaY { get { return -3; } }
		protected virtual int deltaText2 { get { return 3; } }
		protected virtual int CaptionFontHeight { get { return Font.Height + Font2.Height + deltaY + deltaX; } }
		protected virtual int DXLogoHeight { get { return DXLogo.Height + deltaX; } }
		void SetHeight() {
			int h = Math.Max(DXLogoHeight, CaptionFontHeight);
			if(image != null) h = Math.Max(h, image.Height + deltaX );
			if(Height < h) Height = h;
		}
		protected virtual void FillBackground(GraphicsCache cache, Rectangle r) {
			Brush brush = cache.GetGradientBrush(r, BackColor, (BackColor2 == Color.Empty ? BackColor : BackColor2), GradientMode);
			cache.Graphics.FillRectangle(brush, r);
			BorderHelper.GetPainter(BorderStyle).DrawObject(new BorderObjectInfoArgs(cache, null, r));
		}
		protected virtual void DrawCaptions(GraphicsCache cache, Rectangle r, int textLeft) {
			using(SolidBrush foreBrush = new SolidBrush(this.ForeColor)) {
				int textTop = (this.Height - (Font.Height + (Text2 != "" ? Font2.Height + deltaY : 0))) / 2 - 1;
				r = new Rectangle(textLeft, textTop, this.Width - textLeft - DXLogo.Width - deltaX, Font.Height);
				cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				if(r.Width > 0)
					cache.Graphics.DrawString(Text, this.Font, foreBrush, r, TextStringFormat);
				r = new Rectangle(textLeft + deltaText2, textTop + Font.Height + deltaY, this.Width - textLeft - DXLogo.Width - deltaX - deltaText2, Font2.Height);
				if(r.Width > 0)
					cache.Graphics.DrawString(Text2, this.Font2, foreBrush, r, TextStringFormat);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			if(NeedResize) SetHeight();
			Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
			FillBackground(cache, r);
			int textLeft = deltaX;
			if(image != null && this.Width > deltaX * 2 + image.Width + DXLogo.Width) { 
				e.Graphics.DrawImage(image, deltaX, (this.Height - image.Height) / 2);
				textLeft += deltaX + image.Width;
			}
			if(showLogo)
				e.Graphics.DrawImage(DXLogo, this.Width - DXLogo.Width - deltaX, (this.Height - DXLogo.Height) / 2);
			DrawCaptions(cache, r, textLeft);
			cache.Dispose();
		}
	}
	public class NotePanel8_1 : NotePanelEx {
		public NotePanel8_1()
			: base() {
			ArrowImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.description2.png", typeof(NotePanelEx).Assembly);
			ChangeColors();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
		}
		void ChangeColors() {
			BackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Info);
			ForeColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText);
		}
	}
	public class NotePanelEx : NotePanel {
		public NotePanelEx() : base() {
			ArrowImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.XtraFrames.description.png", typeof(NotePanelEx).Assembly);
		}
		protected override bool ShouldSerializeArrowImage() {
			return false;
		}
	}
	public class DescriptionPanel : NotePanel, ISupportLookAndFeel {
		public DescriptionPanel()
			: base() {
			ArrowImage = null;
			Font = DescriptionDefaultFont;
		}
		static Font descriptionFontCore = null;
		public static Font DescriptionDefaultFont {
			get {
				if(descriptionFontCore == null) {
					descriptionFontCore = new Font("Segoe UI", 12, GraphicsUnit.Pixel);
				}
				return descriptionFontCore;
			}
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			UpdateColors();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateColors();
		}
		UserLookAndFeel lookAndFeelCore;
		private void UpdateColors() {
			XtraEditors.XtraForm form = FindForm() as XtraEditors.XtraForm;
			lookAndFeelCore = form != null ? form.LookAndFeel.ActiveLookAndFeel : LookAndFeel;
			ForeColor = GetSystemColor(SystemColors.ControlText);
			BackColor = GetSystemColor(SystemColors.Control);
		}
		Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(lookAndFeelCore, color);
		}
		protected override void OnBackColorChanged(EventArgs e) {
			base.OnBackColorChanged(e);
		}
		protected override Image Arrow {
			get { return null; }
		}
		protected override bool ShouldSerializeArrowImage() {
			return false;
		}
		protected override int GetCaptionBorderWidth(int width) {
			return 0;
		}
		protected override int GetCaptionBorderHeight(int height) {
			return height + 8;
		}
		protected override int deltaX { get { return 0; } }
	}
	public class NotePanel : FrameControl, IXtraResizableControl {
		int substrateWidth = 0;
		int maxRows = 3;
		int imageIndex = 0;
		BorderStyles substrateBorderStyle;
		Color substrateBackColor, substrateBackColor2;
		LinearGradientMode substrateGradientMode;
		ImageCollection imc = null;
		Image arrowImage = null;
		protected virtual Image Arrow {
			get {
				if(arrowImage == null)
					return imc.Images[ImageIndex];
				return arrowImage;
			}
		}
		public NotePanel() : base() {
			imc = CreateImageCollection();
			System.Diagnostics.Debug.Assert(imc != null && imc.Images.Count > 0);
			base.BackColor = SystemColors.Info;
			substrateBackColor = SystemColors.ControlDark;
			substrateBackColor2 = SystemColors.ControlDarkDark;
			substrateGradientMode = LinearGradientMode.ForwardDiagonal;
			substrateBorderStyle = BorderStyles.NoBorder;
		}
		protected virtual ImageCollection CreateImageCollection() {
			return DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.XtraFrames.Arrows.bmp", typeof(NotePanel).Assembly, new Size(12, 17), Color.Magenta);
		} 
		protected override void Dispose(bool disposing) {
			if(imc != null) {
				imc.Dispose();
				imc = null;
			}
			base.Dispose(disposing);
		}
		protected virtual int deltaY { get { return 4; } }
		protected virtual int deltaX { get { return Arrow != null ? Arrow.Width + 4 : 4; } }
		private int MaxHeight(int h) {
			return h * MaxRows;
		}
		[DefaultValue(0)]
		public int ImageIndex {
			get { return imageIndex; }
			set { 
				imageIndex = Math.Max(0, Math.Min(value, imc.Images.Count - 1)); 
				BeginResize();
			}
		}
		[DefaultValue(3)]
		public int MaxRows {
			get { return maxRows; }
			set { 
				if(value < 1) value = 1;
				maxRows = value; 
				BeginResize();
			}
		}
		[DefaultValue(0)]
		public int SubstrateWidth {
			get { return substrateWidth; }
			set { 
				if(value < 0) value = 0;
				substrateWidth = value; 
				BeginResize();
			}
		}
		[DefaultValue(BorderStyles.NoBorder)]
		public BorderStyles SubstrateBorderStyle {
			get { return substrateBorderStyle; }
			set { 
				substrateBorderStyle = value; 
				BeginResize();
			}
		}
		public Image ArrowImage {
			get { return arrowImage; }
			set { 
				arrowImage = value; 
				if(arrowImage != null && arrowImage is Bitmap) ((Bitmap)arrowImage).SetResolution(96, 96);
				BeginResize();
			}
		}
		protected virtual bool ShouldSerializeArrowImage() {
			return arrowImage != null;
		}
		protected virtual void ResetArrowImage() {
			ArrowImage = null;
		}
		protected virtual bool ShouldSerializeCaption() {
			return false;
		}
		protected virtual void ResetSubstrateBackColor() {
			SubstrateBackColor = SystemColors.ControlDark;
		}
		protected virtual bool ShouldSerializeSubstrateBackColor() {
			return SubstrateBackColor != SystemColors.ControlDark;
		}
		public Color SubstrateBackColor {
			get { return substrateBackColor; }
			set { 
				substrateBackColor = value; 
				BeginResize();
			}
		}
		protected virtual void ResetSubstrateBackColor2() {
			SubstrateBackColor2 = SystemColors.ControlDarkDark;
		}
		protected virtual bool ShouldSerializeSubstrateBackColor2() {
			return SubstrateBackColor2 != SystemColors.ControlDarkDark;
		}
		public Color SubstrateBackColor2 {
			get { return substrateBackColor2; }
			set { 
				substrateBackColor2 = value; 
				BeginResize();
			}
		}
		[DefaultValue(LinearGradientMode.ForwardDiagonal)]
		public LinearGradientMode SubstrateGradientMode {
			get { return substrateGradientMode; }
			set { 
				substrateGradientMode = value; 
				BeginResize();
			}
		}
		NotePanelSizes sizes = null;
		NotePanelSizes CalcSizes(Graphics g) {
			NotePanelSizes ret = new NotePanelSizes(SubstrateWidth);
			ObjectPainter painter = BorderHelper.GetPainter(BorderStyle);
			BorderObjectInfoArgs be = new BorderObjectInfoArgs(null, null, new Rectangle(0, 0, 10, 10));
			Rectangle r = ObjectPainter.CalcBoundsByClientRectangle(g, painter, be, be.Bounds);
			ret.CaptionBorderHeight = GetCaptionBorderHeight(r.Height);
			ret.CaptionBorderWidth = GetCaptionBorderWidth(r.Width);
			if(SubstrateWidth > 0) {
				painter = BorderHelper.GetPainter(SubstrateBorderStyle);
				r = ObjectPainter.CalcBoundsByClientRectangle(g, painter, be, be.Bounds);
				ret.SubstrateBorderHeight = r.Height - 10;
				ret.SubstrateBorderWidth = r.Width - 10;
			}
			SizeF size = g.MeasureString(Caption, this.Font, this.Width - deltaX - ret.BorderWidth);
			int height = Math.Min(Math.Max((int)size.Height, this.Font.Height), MaxHeight(this.Font.Height));
			ret.TextHeight = height;
			height += deltaY * 2 + ret.SubstrateBorderHeight + ret.CaptionBorderHeight + SubstrateWidth * 2;
			height = Math.Max(height, Arrow != null ? Arrow.Height + deltaY : deltaY);
			if(Height != height) { 
				Height = height;
				if(IsParentResize)
					BeginInvoke(new MethodInvoker(ResizeParent));
			}
			if(Width < ret.BorderWidth + deltaX) Width = ret.BorderWidth + deltaX;
			EndResize();
			return ret;
		}
		protected virtual int GetCaptionBorderWidth(int length) {
			return length - 10;
		}
		protected virtual int GetCaptionBorderHeight(int length) {
			return length - 10;
		}
		protected override void OnPaint(PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			if(sizes == null || NeedResize) {
				sizes = CalcSizes(e.Graphics);
				if(!DesignMode && Text != string.Empty)
					RaiseSizeChanged();
			}
			Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
			Brush brush = cache.GetGradientBrush(r, SubstrateBackColor, SubstrateBackColor2, SubstrateGradientMode);
			e.Graphics.FillRectangle(brush, r);
			ObjectPainter painter = BorderHelper.GetPainter(SubstrateBorderStyle);
			BorderObjectInfoArgs be = new BorderObjectInfoArgs(cache, null, r);
			if(SubstrateWidth > 0)
				painter.DrawObject(be);
			r = new Rectangle(sizes.SubstrateBorderWidth / 2 + SubstrateWidth, sizes.SubstrateBorderHeight / 2 + SubstrateWidth, 
				this.Width - (sizes.SubstrateBorderWidth + SubstrateWidth * 2), this.Height - (sizes.SubstrateBorderHeight + SubstrateWidth * 2));
			brush = cache.GetGradientBrush(r, BackColor, (BackColor2 == Color.Empty ? BackColor : BackColor2), GradientMode);
			e.Graphics.FillRectangle(brush, r);
			painter = BorderHelper.GetPainter(BorderStyle);
			be = new BorderObjectInfoArgs(cache, null, r);
			painter.DrawObject(be);
			r = new Rectangle(
				sizes.BorderWidth / 2 + deltaX,  
				sizes.BorderHeight / 2 + deltaY, 
				this.Width - deltaX - sizes.BorderWidth, 
				sizes.TextHeight); 
			int imageIndent = deltaY / 2;
			if(Arrow != null)
				e.Graphics.DrawImage(Arrow, sizes.BorderWidth / 2, sizes.BorderHeight / 2 + imageIndent, Arrow.Width, Arrow.Height);
			if(r.Width > 0) {
				using(Brush foreBrush = new SolidBrush(this.ForeColor)) 
					e.Graphics.DrawString(Caption, this.Font, foreBrush, r, TextStringFormat);
			}
			cache.Dispose();
		}
		#region IXtraResizableControl Members
		int GetHeight() {
			if(sizes == null) return 50;
			return Math.Max(sizes.TextHeight + sizes.BorderHeight * 2 + sizes.CaptionBorderHeight * 2, deltaY + (ArrowImage != null ? ArrowImage.Height : 0));
		}
		public event EventHandler Changed;
		protected virtual void RaiseSizeChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public bool IsCaptionVisible {
			get { return false; }
		}
		public Size MaxSize {
			get { return new Size(1000, GetHeight()); }
		}
		public Size MinSize {
			get { return new Size(200, GetHeight()); }
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class PictureButton : PictureBox {
		public event EventHandler ButtonClick;
		protected Image normal = null, hover = null, pressed = null;
		string processStartLink = string.Empty;
		public PictureButton(Control parent, Image normal, Image hover, Image pressed, string processStartLink) {
			this.BackColor = Color.Transparent;
			this.SizeMode = PictureBoxSizeMode.AutoSize;
			this.normal = normal;
			this.hover = hover;
			this.pressed = pressed;
			this.Image = normal;
			parent.SizeChanged += new EventHandler(parent_SizeChanged);
			parent.Controls.Add(this);
			this.processStartLink = processStartLink;
			UpdateLocation();
		}
		void parent_SizeChanged(object sender, EventArgs e) {
			UpdateLocation();
		}
		void UpdateLocation() {
			this.Location = new Point((this.Parent.Width - this.Width) / 2, (this.Parent.Height - this.Height) / 2);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image Image {
			get { return base.Image; }
			set { base.Image = value; }
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			SetActive(Active, true);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			SetActive(normal, false);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left) return;
			SetActive(Pressed, true);
		}
		Image Active { get { return hover == null ? normal : hover; } }
		Image Pressed { get { return pressed == null ? Active : pressed; } }
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button != MouseButtons.Left) return;
			if(new Rectangle(0, 0, this.Width, this.Height).Contains(new Point(e.X, e.Y))) {
				SetActive(Active, true);
				RaiseButtonClick();
			} else {
				SetActive(normal, false);
			}
		}
		protected virtual void SetActive(Image image, bool active) {
		}
		protected virtual void RaiseButtonClick() {
			if(ButtonClick != null)
				ButtonClick(this, EventArgs.Empty);
			if(!string.IsNullOrEmpty(processStartLink))
				ProcessStart(processStartLink);
		}
		public static void ProcessStart(string name) {
			ProcessStart(name, string.Empty);
		}
		public static void ProcessStart(string name, string arguments) {
			try {
				var process = new System.Diagnostics.Process();
				process.StartInfo.FileName = name;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.Verb = "Open";
				process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
				process.Start();
			}
			catch { }
		}
	}
}
