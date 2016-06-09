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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon {
	public class AppMenuFileLabelPainter : BaseControlPainter {
		AppMenuFileLabelObjectPainter objectPainter = null;
		public AppMenuFileLabelPainter() {
			this.objectPainter = CreateLabelObjectPainter();
		}
		protected virtual AppMenuFileLabelObjectPainter CreateLabelObjectPainter() { return new AppMenuFileLabelObjectPainter(); }
		public AppMenuFileLabelObjectPainter LabelObjectPainter { get { return objectPainter; } }
		public override void Draw(ControlGraphicsInfoArgs info) {
			AppMenuFileLabelInfoArgs args = new AppMenuFileLabelInfoArgs(info.ViewInfo as AppMenuFileLabelViewInfo, info);
			LabelObjectPainter.DrawObject(args);
		}
	}
	public class AppMenuFileLabelObjectPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppMenuFileLabelInfoArgs le = e as AppMenuFileLabelInfoArgs;
			if(le == null) return;
			DrawBackgraund(le);
			DrawCaption(le);
			DrawText(le);
			DrawDescription(le);
			DrawImage(le);
			DrawGlyph(le);
		}
		protected virtual void DrawCaption(AppMenuFileLabelInfoArgs e) { 
			using(StringFormat s = e.ViewInfo.PaintAppearance.GetStringFormat().Clone() as StringFormat){
				s.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				e.ViewInfo.PaintAppearance.DrawString(e.Cache, e.ViewInfo.Label.Caption, e.ViewInfo.CaptionRect, s);  
			}
		}
		protected virtual void DrawText(AppMenuFileLabelInfoArgs e) {
			using(StringFormat s = e.ViewInfo.PaintAppearance.GetStringFormat().Clone() as StringFormat) {
				s.Trimming = StringTrimming.EllipsisCharacter;
				e.ViewInfo.PaintAppearance.DrawString(e.Cache, e.ViewInfo.Label.Text, e.ViewInfo.TextRect, s);
			}
		}
		protected virtual void DrawDescription(AppMenuFileLabelInfoArgs e) {
			using(StringFormat s = e.ViewInfo.AppearanceDescription.GetStringFormat().Clone() as StringFormat) {
				s.Trimming = StringTrimming.EllipsisCharacter;
				e.ViewInfo.AppearanceDescription.DrawString(e.Cache, e.ViewInfo.Label.Description, e.ViewInfo.DescriptionRect, s);
			}
		}
		protected virtual void DrawGlyph(AppMenuFileLabelInfoArgs e) {
			if(e.ViewInfo.Label.Glyph == null) return;
			e.Graphics.DrawImage(e.ViewInfo.Label.Glyph, e.ViewInfo.GlyphRect); 
		}
		protected virtual void DrawImage(AppMenuFileLabelInfoArgs e) {
			if(!e.ViewInfo.Label.ShowCheckButton || e.ViewInfo.Label.GetImage() == null) return;
			e.Graphics.DrawImage(e.ViewInfo.Label.GetImage(), e.ViewInfo.ImageRect); 
		}
		protected virtual SkinElementInfo GetBackgroundInfo(AppMenuFileLabelInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(e.ViewInfo.LookAndFeel.ActiveLookAndFeel)[RibbonSkins.SkinPopupGalleryPopupButton], e.ViewInfo.HitInfo.ObjectRect);
			if(e.ViewInfo.HitInfo.HitTest != AppMenuFileLabelHitTest.None) {
				info.State = ObjectState.Hot;
				info.ImageIndex = 1;
				info.RightToLeft = e.ViewInfo.Label.IsRightToLeft;
			}
			return info;
		}
		protected virtual void DrawBackgraund(AppMenuFileLabelInfoArgs e) {
			SkinElementInfo info = GetBackgroundInfo(e);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class AppMenuFileLabelInfoArgs : ObjectInfoArgs {
		AppMenuFileLabelViewInfo viewInfo = null;
		public AppMenuFileLabelInfoArgs(AppMenuFileLabelViewInfo viewInfo, ControlGraphicsInfoArgs e) : this(e.Cache, viewInfo.Bounds, viewInfo.GetObjectState()) {
			this.viewInfo = viewInfo; 
		}
		public AppMenuFileLabelInfoArgs(GraphicsCache cache, Rectangle bounds, ObjectState state) : base(cache, bounds, state) { }
		public AppMenuFileLabelInfoArgs(GraphicsCache cache) : base(cache) { }
		public AppMenuFileLabelViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
	}
	public enum AppMenuFileLabelHitTest { None, Label, LabelImage }
	public class AppMenuFileLabelHitInfo {
		Point hitPoint;
		MouseButtons buttons;
		AppMenuFileLabelHitTest hitTest;
		Rectangle objRect;
		public AppMenuFileLabelHitInfo() {
			this.hitPoint = Point.Empty;
			this.hitTest = AppMenuFileLabelHitTest.None;
			this.objRect = Rectangle.Empty;
			this.buttons = MouseButtons.None;
		}
		public MouseButtons Buttons { get { return buttons; } set { buttons = value; } } 
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public AppMenuFileLabelHitTest HitTest { get { return hitTest; } set { hitTest = value; } }
		public Rectangle ObjectRect { get { return objRect; } set { objRect = value; } }
		public bool ContainsSet(Rectangle rect, AppMenuFileLabelHitTest hitTest) {
			if(rect.Contains(HitPoint)) {
				HitTest = hitTest;
				ObjectRect = rect;
				return true;
			}
			return false;
		}
	}
	public class AppMenuFileLabelViewInfo : BaseStyleControlViewInfo {
		public static int IndentBetweenCaptionAndText = 6;
		public static int IndentBetweenTextAndDescription = 6;
		public static int ImageIndent = 6;
		public static int ContentIndent = 3;
		Rectangle captionRect, textRect, buttonRect, imageRect, glyphRect, descriptionRect;
		AppMenuFileLabelHitInfo hitInfo = null;
		AppearanceObject appearanceDescription;
		public AppMenuFileLabelViewInfo(AppMenuFileLabel owner) : base(owner) {
			this.captionRect = this.textRect = this.buttonRect = Rectangle.Empty;
		}
		public override AppearanceDefault DefaultAppearance {
			get { return new AppearanceDefault(Label.Enabled ? GetForeColor() : LookAndFeelHelper.GetSystemColorEx(LookAndFeel.ActiveLookAndFeel, SystemColors.GrayText), Color.Transparent); }
		}
		Color GetForeColor() {
			Color res = LookAndFeelHelper.GetTransparentForeColor(Label.LookAndFeel, Label);
			if(res.IsSystemColor) res = RibbonSkins.GetSkin(Label.LookAndFeel.ActiveLookAndFeel).GetSystemColor(res);
			return res;
		}
		Color GetDescriptionColor() { 
			object obj = RibbonSkins.GetSkin(Label.LookAndFeel).Properties[RibbonSkins.OptAppMenuFileLabelDescriptionColor];
			Color res = obj == null? LookAndFeelHelper.GetTransparentForeColor(Label.LookAndFeel.ActiveLookAndFeel, Label) : (Color)obj;
			if(res.IsSystemColor) res = RibbonSkins.GetSkin(Label.LookAndFeel).GetSystemColor(res);
			return res;
		}
		public virtual AppearanceDefault DefaultAppearanceDescription {
			get { return new AppearanceDefault(!Label.Enabled? LookAndFeelHelper.GetSystemColorEx(Label.LookAndFeel.ActiveLookAndFeel, SystemColors.GrayText): GetDescriptionColor(), Color.Transparent);}
		}
		protected virtual Size CalcTextSize(StringFormat sFormat, string text, int scopeWidth) {
			return Appearance.CalcTextSize(GInfo.Graphics, sFormat, text, scopeWidth).ToSize();
		}
		public AppearanceObject AppearanceDescription {
			get {
				if(appearanceDescription == null)
					appearanceDescription = new AppearanceObject();
				return appearanceDescription;
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			this.appearanceDescription = null;
			AppearanceHelper.Combine(AppearanceDescription, new AppearanceObject[] { Label.AppearanceDescription }, DefaultAppearanceDescription);
			AppearanceDescription.TextOptions.RightToLeft = Label.IsRightToLeft;
		}
		public virtual AppMenuFileLabel Label { get { return Owner as AppMenuFileLabel; } }
		protected override void CalcContentRect(Rectangle bounds) {
			fContentRect = bounds;
			fContentRect.Inflate(-3, -3);
		}
		protected virtual void CalcSizes() {
			CalcImageSizes();
			CalcTextSizes();
		}
		protected virtual void CalcImageSizes() {
			if(!Label.ShowCheckButton) {
				this.imageRect.Size = Size.Empty;
				this.buttonRect.Size = Size.Empty;
			}
			else if(Label.GetImage() == null) {
				this.imageRect.Size = new Size(0, ContentRect.Height);
				this.buttonRect.Size = new Size(ImageIndent * 2, Bounds.Height);
			}
			else {
				this.imageRect.Size = Label.GetImage().Size;
				this.buttonRect.Size = new Size(ImageRect.Width + ImageIndent * 2, Bounds.Height);
			}
			if(Label.Glyph == null)
				this.glyphRect.Size = Size.Empty;
			else this.glyphRect.Size = Label.Glyph.Size;
		}
		protected virtual void CalcTextSizes() {
			using(StringFormat s = Appearance.GetStringFormat().Clone() as StringFormat) {
				s.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				this.captionRect.Size = CalcTextSize(s, Label.Caption, 0);
				s.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
				s.Trimming = StringTrimming.EllipsisCharacter;
				int scopeWidth = CalcDescriptionAvaliableWidth();
				s.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
				this.textRect.Size = CalcTextSize(s, Label.Text, scopeWidth);
				this.descriptionRect.Size = CalcDescriptionSize(scopeWidth);
			}
		}
		protected virtual Size CalcDescriptionSize(int scopeWidth) {
			if(string.IsNullOrEmpty(Label.Description))
				return Size.Empty;
			using(StringFormat format = AppearanceDescription.GetStringFormat().Clone() as StringFormat) {
				format.Trimming = StringTrimming.EllipsisCharacter;
				return AppearanceDescription.CalcTextSize(GInfo.Graphics, format, Label.Description, scopeWidth).ToSize();
			}
		}
		protected virtual int CalcDescriptionAvaliableWidth() {
			return Label.Width - GlyphRect.Width - CaptionRect.Width - ImageRect.Width - IndentBetweenCaptionAndText - 2 * ImageIndent - 2 * ContentIndent;
		}
		protected virtual void CalcLabelRects() {
			CalcSizes();
			this.buttonRect = new Rectangle(new Point(Bounds.Right - ButtonRect.Width, Bounds.Y), ButtonRect.Size);
			this.glyphRect.Height = Label.Glyph != null ? Label.Glyph.Height : 0;
			this.glyphRect.X = ContentRect.X;
			this.glyphRect.Y = ContentRect.Y + (ContentRect.Height - GlyphRect.Height) / 2;
			this.imageRect.X = ButtonRect.Width != 0? ButtonRect.X + ImageIndent: ButtonRect.X;
			this.imageRect.Y = ContentRect.Y + (ContentRect.Height - ImageRect.Height) / 2;
			this.imageRect.Width = ButtonRect.Width != 0? ButtonRect.Width - 2 * ImageIndent: 0;
			this.imageRect.Height = Label.GetImage() != null ? Label.GetImage().Height : 0;
			int textAndDescriptionHeight = !string.IsNullOrEmpty(Label.Description) ? TextRect.Height + DescriptionRect.Height + IndentBetweenTextAndDescription : TextRect.Height;
			this.captionRect.Location = GetCaptionLocation(Label.AutoHeight ? VertAlignment.Default : Appearance.TextOptions.VAlignment, textAndDescriptionHeight);
			this.captionRect.Width = Math.Min(CaptionRect.Width, ImageRect.X - CaptionRect.X);
			this.textRect.Location = GetTextLocation(textAndDescriptionHeight);
			this.textRect.Width = Math.Min(TextRect.Width, ButtonRect.X - TextRect.X);
			this.descriptionRect.Location = GetDescriptionLocation(textAndDescriptionHeight);
			this.descriptionRect.Width = Math.Min(DescriptionRect.Width, ButtonRect.X - TextRect.X);
			RotateRects();
		}
		void RotateRects() {
			if(Label.IsRightToLeft) {
				this.buttonRect = BarUtilites.ConvertBoundsToRTL(buttonRect, Bounds);
				this.glyphRect = BarUtilites.ConvertBoundsToRTL(glyphRect, Bounds);
				this.imageRect = BarUtilites.ConvertBoundsToRTL(imageRect, Bounds);
				this.captionRect = BarUtilites.ConvertBoundsToRTL(captionRect, Bounds);
				this.textRect = BarUtilites.ConvertBoundsToRTL(textRect, Bounds);
				this.descriptionRect = BarUtilites.ConvertBoundsToRTL(descriptionRect, Bounds);
			}
		}
		protected virtual Point GetCaptionLocation(VertAlignment alighment, int textBlockHeight) {
			int xPt = GlyphRect.Width != 0? GlyphRect.Right + IndentBetweenCaptionAndText : ContentRect.X;
			int yPt = 0;
			switch(alighment) {
				case VertAlignment.Top:
					yPt = ContentRect.Y;
					break;
				case VertAlignment.Center:
				case VertAlignment.Default:
					yPt = ContentRect.Y + (ContentRect.Height - textBlockHeight) / 2;
					break;
				case VertAlignment.Bottom:
					yPt = ContentRect.Bottom - textBlockHeight;
					break;
			}
			return new Point(xPt, yPt);
		}
		protected virtual Point GetTextLocation(int textBlockHeight) {
			return new Point(CaptionRect.Right + IndentBetweenCaptionAndText, CaptionRect.Y);
		}
		protected virtual Point GetDescriptionLocation(int textBlockHeight) {
			return new Point(TextRect.X, TextRect.Bottom + IndentBetweenTextAndDescription);
		}
		protected override void CalcRects() {
			base.CalcRects();
			CalcLabelRects();
		}
		public Rectangle CaptionRect { get { return captionRect; } }
		public Rectangle DescriptionRect { get { return descriptionRect; } }
		public Rectangle TextRect { get { return textRect; } }
		public Rectangle ButtonRect { get { return buttonRect; } }
		public Rectangle ImageRect { get { return imageRect; } }
		public Rectangle GlyphRect { get { return glyphRect; } }
		public virtual AppMenuFileLabelHitInfo CalcHitInfo(Point pt, MouseButtons bt) {
			AppMenuFileLabelHitInfo info = new AppMenuFileLabelHitInfo();
			info.HitPoint = pt;
			info.Buttons = bt;
			if(info.ContainsSet(Bounds, AppMenuFileLabelHitTest.Label)) {
				info.ContainsSet(ButtonRect, AppMenuFileLabelHitTest.LabelImage);
			}
			return info;
		}
		public virtual int CalcBestHeight() {
			if(Bounds.Height == 0) {
				GInfo.AddGraphics(null);
				try {
					UpdatePaintAppearance();
					CalcRects();
				}
				finally { GInfo.ReleaseGraphics(); }
			}
			int textAndDescHeight = DescriptionRect.Height > 0 ? TextRect.Height + DescriptionRect.Height + IndentBetweenTextAndDescription : TextRect.Height;
			int height = Math.Max(Math.Max(Math.Max(CaptionRect.Height, textAndDescHeight), ImageRect.Height), GlyphRect.Height);
			return height + ContentIndent * 2;
		}
		public AppMenuFileLabelHitInfo HitInfo {
			get {
				if(hitInfo == null) hitInfo = new AppMenuFileLabelHitInfo();
				return hitInfo;
			}
			set { hitInfo = value; }
		}
		protected internal virtual ObjectState GetObjectState() {
			if(HitInfo.HitTest == AppMenuFileLabelHitTest.None) return ObjectState.Normal;
			return ObjectState.Hot;
		}
	}
	public class AppMenuFileLabelAccessible : BaseAccessible {
		AppMenuFileLabel label;
		public AppMenuFileLabelAccessible(AppMenuFileLabel label) : base() {
			this.label = label;
		}
		public AppMenuFileLabel Label { get { return label; } }
		protected override string GetDefaultAction() {
			return base.GetString(AccStringId.ActionPress);
		}
		protected override void DoDefaultAction() {
			if(Label != null) {
				Label.RaiseLabelClick();
				return;
			}
			base.DoDefaultAction();
		}
		public override Control GetOwnerControl() { return Label; }
	}
	public class AppMenuFileLabel : BaseStyleControl {
		private static readonly object labelClick = new object();
		private static readonly object labelImageClick = new object();
		private static readonly object checkedChanged = new object();
		string caption = string.Empty;
		Image image = null;
		Image selectedImage = null;
		Image glyph = null;
		bool bChecked = false;
		bool showCheckButton = true;
		string imageToolTip;
		string imageToolTipTitle;
		SuperToolTip imageSuperTip;
		ToolTipIconType imageToolTipIconType;
		string selectedImageToolTip;
		string selectedImageToolTipTitle;
		SuperToolTip selectedImageSuperTip;
		ToolTipIconType selectedImageToolTipIconType;
		string description;
		AppearanceObject descriptionAppearance;
		public AppMenuFileLabel() : base() {
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
			SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
			this.imageToolTipTitle = this.imageToolTip = "";
			this.imageToolTipIconType = ToolTipIconType.None;
			this.selectedImageToolTipTitle = this.selectedImageToolTip = "";
			this.selectedImageToolTipIconType = ToolTipIconType.None;
			this.descriptionAppearance = new AppearanceObject();
			this.descriptionAppearance.Changed += new EventHandler(OnDescriptionAppearanceChanged);
		}
		protected virtual void OnDescriptionAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelImageToolTip"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string ImageToolTip {
			get { return imageToolTip; }
			set {
				if(value == null) value = string.Empty;
				if(ImageToolTip == value) return;
				imageToolTip = value;
			}
		}
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelImageToolTipTitle"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string ImageToolTipTitle {
			get { return imageToolTipTitle; }
			set {
				if(value == null) value = string.Empty;
				if(ImageToolTipTitle == value) return;
				imageToolTipTitle = value;
			}
		}
		internal bool ShouldSerializeImageSuperTip() { return ImageSuperTip != null && !ImageSuperTip.IsEmpty; }
		[Localizable(true), Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelImageSuperTip"),
#endif
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public virtual SuperToolTip ImageSuperTip {
			get { return imageSuperTip; }
			set { imageSuperTip = value; }
		}
		public void ResetImageSuperTip() { ImageSuperTip = null; }
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelImageToolTipIconType"),
#endif
 DefaultValue(ToolTipIconType.None), Localizable(true)]
		public virtual ToolTipIconType ImageToolTipIconType {
			get { return imageToolTipIconType; }
			set { imageToolTipIconType = value; }
		}
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelSelectedImageToolTip"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string SelectedImageToolTip {
			get { return selectedImageToolTip; }
			set {
				if(value == null) value = string.Empty;
				if(SelectedImageToolTip == value) return;
				selectedImageToolTip = value;
			}
		}
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelSelectedImageToolTipTitle"),
#endif
 DefaultValue(""), Localizable(true)]
		public virtual string SelectedImageToolTipTitle {
			get { return selectedImageToolTipTitle; }
			set {
				if(value == null) value = string.Empty;
				if(SelectedImageToolTipTitle == value) return;
				selectedImageToolTipTitle = value;
			}
		}
		internal bool ShouldSerializeSelectedImageSuperTip() { return SelectedImageSuperTip != null && !SelectedImageSuperTip.IsEmpty; }
		[Localizable(true), Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelSelectedImageSuperTip"),
#endif
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public virtual SuperToolTip SelectedImageSuperTip {
			get { return selectedImageSuperTip; }
			set { selectedImageSuperTip = value; }
		}
		public void ResetSelectedImageSuperTip() { SelectedImageSuperTip = null; }
		[Category("Tooltip"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelSelectedImageToolTipIconType"),
#endif
 DefaultValue(ToolTipIconType.None), Localizable(true)]
		public virtual ToolTipIconType SelectedImageToolTipIconType {
			get { return selectedImageToolTipIconType; }
			set { selectedImageToolTipIconType = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelText"),
#endif
 SettingsBindable(true), DefaultValue(""), Category("Appearance")]
		public override string Text {
			get { return base.Text; }
			set {
				if(Text == value) return;
				base.Text = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelDescription"),
#endif
 SettingsBindable(true), Category("Appearance")]
		public string Description {
			get { return description; }
			set {
				if(Description == value)
					return;
				description = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelCaption"),
#endif
 SettingsBindable(true), DefaultValue(""), Category("Appearance")]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnPropertiesChanged();
			}
		}
		void ResetAppearanceDescription() { AppearanceDescription.Reset(); }
		bool ShouldSerializeAppearanceDescription() { return !AppearanceDescription.IsEqual(AppearanceObject.EmptyAppearance); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelAppearanceDescription"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SettingsBindable(true), Category("Appearance")]
		public AppearanceObject AppearanceDescription {
			get { return descriptionAppearance; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelChecked"),
#endif
 DefaultValue(false), Category("Behavior")]
		public bool Checked {
			get { return bChecked; }
			set {
				if(Checked == value) return;
				bChecked = value;
				RaiseCheckedChanged();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelImage"),
#endif
 DefaultValue((string)null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelSelectedImage"),
#endif
 DefaultValue((string)null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image SelectedImage {
			get { return selectedImage; }
			set {
				if(SelectedImage == value) return;
				selectedImage = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelShowCheckButton"),
#endif
 DefaultValue(true), Category("Appearance")]
		public bool ShowCheckButton {
			get { return showCheckButton; }
			set {
				if(ShowCheckButton == value) return;
				showCheckButton = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelGlyph"),
#endif
 DefaultValue((string)null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Glyph {
			get { return glyph; }
			set {
				if(Glyph == value) return;
				glyph = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AppMenuFileLabelAutoHeight"),
#endif
 DefaultValue(true), Category("Appearance")]
		public bool AutoHeight {
			get { return AutoSize; }
			set {
				if(AutoSize == value) return;
				AutoSize = value;
				OnPropertiesChanged();
			}
		}
		internal Image GetImage() {
			if(!Checked) return Image;
			else return SelectedImage;
		}
		protected new AppMenuFileLabelViewInfo ViewInfo { get { return base.ViewInfo as AppMenuFileLabelViewInfo; } }
		protected override DevExpress.XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new AppMenuFileLabelViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() { return new AppMenuFileLabelPainter() as BaseControlPainter; }
		public override Size GetPreferredSize(Size proposedSize) {
			if(!AutoHeight) return base.GetPreferredSize(proposedSize);
			else return new Size(Bounds.Width, ViewInfo.CalcBestHeight());
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			AppMenuFileLabelHitInfo info = ViewInfo.CalcHitInfo(e.Location, e.Button);
			AppMenuFileLabelHitTest prevTest = ViewInfo.HitInfo.HitTest;
			ViewInfo.HitInfo = info;
			if(ViewInfo.HitInfo.HitTest != prevTest) {
				Invalidate();
			}
		}
		public event EventHandler CheckedChanged {
			add { Events.AddHandler(checkedChanged, value); }
			remove { Events.RemoveHandler(checkedChanged, value); }
		}
		public event EventHandler LabelClick {
			add { Events.AddHandler(labelClick, value); }
			remove { Events.RemoveHandler(labelClick, value); }
		}
		public event CancelEventHandler LabelImageClick {
			add { Events.AddHandler(labelImageClick, value); }
			remove { Events.RemoveHandler(labelImageClick, value); }
		}
		internal virtual void RaiseLabelClick() {
			EventHandler handler = Events[labelClick] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		internal virtual void RaiseLabelImageClick(CancelEventArgs e) {
			CancelEventHandler handler = Events[labelImageClick] as CancelEventHandler;
			if(handler != null) handler(this, e);
		}
		internal virtual void RaiseCheckedChanged() {
			EventHandler handler = Events[checkedChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void PerformLabelImageClick() {
			CancelEventArgs ce = new CancelEventArgs();
			RaiseLabelImageClick(ce);
			if(!ce.Cancel) Checked = !Checked;
		}
		protected void PerformLabelClick() {
			RaiseLabelClick();
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			AppMenuFileLabelHitTest prevTest = ViewInfo.HitInfo.HitTest;
			ViewInfo.HitInfo = new AppMenuFileLabelHitInfo();
			if(prevTest != ViewInfo.HitInfo.HitTest) Invalidate();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			AppMenuFileLabelHitInfo info = ViewInfo.CalcHitInfo(e.Location, e.Button);
			ViewInfo.HitInfo = info;
			if(e.Button == MouseButtons.Left) {
				if(ViewInfo.HitInfo.HitTest == AppMenuFileLabelHitTest.Label) {
					RaiseLabelClick();
				}
				else if(ViewInfo.HitInfo.HitTest == AppMenuFileLabelHitTest.LabelImage) {
					CancelEventArgs ce = new CancelEventArgs();
					RaiseLabelImageClick(ce);
					if(!ce.Cancel) Checked = !Checked;
				}
			}
		}
		protected override BaseAccessible CreateAccessibleInstance() {
			return new AppMenuFileLabelAccessible(this);
		}
		object buttonObject = new object();
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			if(!ViewInfo.Bounds.Contains(point)) return null;
			if(ViewInfo.ButtonRect.Contains(point)) {
				if(info == null)
					info = new ToolTipControlInfo();
				if(Checked)
					InitializeToolTipControlInfo(info, SelectedImageSuperTip, SelectedImageToolTip, SelectedImageToolTipTitle, SelectedImageToolTipIconType);
				else
					InitializeToolTipControlInfo(info, ImageSuperTip, ImageToolTip, ImageToolTipTitle, ImageToolTipIconType);
				info.Object = buttonObject;
			}
			return info;
		}
		protected virtual void InitializeToolTipControlInfo(ToolTipControlInfo info, SuperToolTip superTip, string toolTip, string toolTipTitle, ToolTipIconType iconType) {
			if(info == null) return;
			info.SuperTip = superTip;
			info.Title = toolTipTitle;
			info.Text = toolTip;
			info.IconType = iconType;
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected internal new bool IsRightToLeft {
			get { return base.IsRightToLeft; }
		}
	}
	[
	DXToolboxItem(true),
	Designer("DevExpress.XtraBars.Design.AppPopupMenuDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	Description("Represents the application's main menu displayed at the top left corner of a RibbonForm."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "ApplicationMenu")
	]
	public class ApplicationMenu : PopupMenu, IDXDropDownControlEx {
		static int MinRightPaneWidth = 240;
		bool showRightPane = false;
		int rightPaneWidth = MinRightPaneWidth;
		PopupControlContainer rightPaneControlContainer;	
		PopupControlContainer bottomPaneControlContainer;
		DefaultBoolean rightToLeft = DefaultBoolean.Default;
		public ApplicationMenu(IContainer container)
			: this() {
			container.Add(this);
		}
		public ApplicationMenu() {
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("ApplicationMenuRightToLeft"),
#endif
 DefaultValue(DefaultBoolean.Default), Category("Behavior")]
		public virtual DefaultBoolean RightToLeft {
			get { return rightToLeft; }
			set {
				if(value == RightToLeft) return;
				rightToLeft = value;
				CheckRightToLeft();
			}
		}
		bool isRightToLeft = false;
		protected internal void CheckRightToLeft() {
			bool rightToLeft = false;
			if(RightToLeft == DefaultBoolean.True) {
				rightToLeft = true;
			}
			else {
				if(RightToLeft == DefaultBoolean.Default) {
					if(Ribbon != null) rightToLeft = WindowsFormsSettings.GetIsRightToLeft(Ribbon);
					if(Manager != null) rightToLeft = Manager.IsRightToLeft;
				}
			}
			if(rightToLeft == isRightToLeft) return;
			this.isRightToLeft = rightToLeft;
		}
		protected override void OnBeforePopup(CancelEventArgs e) {
			base.OnBeforePopup(e);
			CheckRightToLeft();
		}
		internal bool IsRightToLeft { get { return isRightToLeft; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("ApplicationMenuShowRightPane"),
#endif
 DefaultValue(false), Category("Appearance")]
		public bool ShowRightPane {
			get { return showRightPane; }
			set { showRightPane = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("ApplicationMenuRightPaneWidth"),
#endif
 DefaultValue(240), Category("Behavior")]
		public int RightPaneWidth {
			get { return rightPaneWidth; }
			set { rightPaneWidth = Math.Max(value, MinRightPaneWidth); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("ApplicationMenuRightPaneControlContainer"),
#endif
 DefaultValue((string)null)]
		public PopupControlContainer RightPaneControlContainer {
			get { return rightPaneControlContainer; }
			set { rightPaneControlContainer = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("ApplicationMenuBottomPaneControlContainer"),
#endif
 DefaultValue((string)null)]
		public PopupControlContainer BottomPaneControlContainer {
			get { return bottomPaneControlContainer; }
			set { bottomPaneControlContainer = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarManager Manager {
			get { return base.Manager; }
			set { base.Manager = value; }
		}
		protected override PopupMenuBarControl CreatePopupControl(BarManager manager) { return new AppMenuBarControl(manager, this); }
		protected override SubMenuControlForm CreateForm(BarManager manager, PopupMenuBarControl pc) { return new AppMenuForm(this, manager, pc); }
		protected virtual bool SkinnedMenu { 
			get {
				if(Ribbon == null) return true;
				BarAndDockingController controller = Ribbon.GetController();
				if(controller == null) return true;
				if(controller.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return false;
				return true;	
			} 
		}
		protected internal override LocationInfo CalcLocationInfo(Point p) {
			LocationInfo res = base.CalcLocationInfo(p);
			if(Ribbon == null) return res;
			int bh = Ribbon.ViewInfo.ApplicationButtonSize.Height;
			int px = IsRightToLeft ? p.X + 4 : p.X - 4;
			if(Screen.FromPoint(p) != Screen.FromPoint(new Point(px, p.Y)))
				px = p.X;
			LocationInfo info = new LocationInfo(new Point(px, p.Y - 16), new Point(px, p.Y - bh), true, IsRightToLeft);
			Rectangle appButton = Ribbon.RectangleToScreen(Ribbon.ViewInfo.ApplicationButton.Bounds);
			if(appButton.X != p.X || appButton.Bottom != p.Y) info = new LocationInfo(p, p, true, IsRightToLeft);
			else if(!SkinnedMenu) info = new LocationInfo(new Point(appButton.X, appButton.Bottom), new Point(appButton.X, appButton.Bottom), true, IsRightToLeft);
			info.WindowSize = res.WindowSize;
			return info;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("ApplicationMenuCanShowPopup")]
#endif
public override bool CanShowPopup { get { return Ribbon != null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string MenuCaption {
			get { return base.MenuCaption; }
			set { base.MenuCaption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ShowCaption {
			get { return base.ShowCaption; }
			set { base.ShowCaption = value; }
		}
		protected override MenuAppearance CreateMenuAppearance() { return new ApplicationMenuAppearance(this) as MenuAppearance; }
		void IDXDropDownControl.Show(IDXMenuManager manager, Control control, Point pos) {
			if(manager == null || control == null) return;
			pos = control.PointToScreen(pos);
			if(manager is RibbonControl) ShowPopup((manager as RibbonControl).Manager, pos);
		}
	}
	public class ApplicationMenuAppearance : MenuAppearance {
		public ApplicationMenuAppearance() : base() { }
		public ApplicationMenuAppearance(IAppearanceOwner owner) : base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceObject MenuCaption { get { return base.MenuCaption; } }
	}
	public class AppMenuKeyTipManager : ContainerKeyTipManager {
		public AppMenuKeyTipManager(RibbonControl ribbon, CustomLinksControl linksControl, BarItemLinkReadOnlyCollection links) : base(ribbon, linksControl, links) { }
		public ApplicationMenu AppMenu {
			get {
				AppMenuBarControl barControl = LinksControl as AppMenuBarControl;
				if(barControl == null) return null;
				return barControl.Menu as ApplicationMenu;
			}
		}
		public PopupControlContainer RightPaneControlContainer {
			get {
				if(AppMenu == null) return null;
				return AppMenu.RightPaneControlContainer;
			}
		}
		public PopupControlContainer BottomPaneControlContainer {
			get {
				if(AppMenu == null) return null;
				return AppMenu.BottomPaneControlContainer;
			}
		}
		Control mnemonicControl = null;
		protected Control MnemonicControl { get { return mnemonicControl; } set { mnemonicControl = value; } }
		public override bool IsNeededChar(char c) {
			bool isNeeded = base.IsNeededChar(c);
			if(!isNeeded && FilterString.Length == 0) {
				Control ctrl = GetControlByMnemonicChar(c);
				if(ctrl != null) {
					MnemonicControl = ctrl;
					return true;
				}
			}
			return isNeeded;
		}
		protected override void OnFilterStringChanged() {
			if(MnemonicControl != null && MnemonicControl.AccessibilityObject != null) {
				HideKeyTips();
				MnemonicControl.AccessibilityObject.DoDefaultAction();
				return;
			}
			base.OnFilterStringChanged();
		}
		Control GetControlByMnemonicChar(Control ctrl, char c) {
			Control childControl;
			if(ctrl == null) return null;
			AppMenuFileLabel fileLabel = ctrl as AppMenuFileLabel;
			if(fileLabel != null) {
				if(Control.IsMnemonic(c, fileLabel.Caption)) return fileLabel;
				return null;
			}
			if(ctrl.Controls == null) return null;
			foreach(Control cc in ctrl.Controls) {
				childControl = GetControlByMnemonicChar(cc, c);
				if(childControl != null) return childControl;
			}
			return null;
		}
		internal Control GetControlByMnemonicChar(char c) {
			Control ctrl = null;
			ctrl = GetControlByMnemonicChar(RightPaneControlContainer, c);
			if(ctrl != null) return ctrl;
			return GetControlByMnemonicChar(BottomPaneControlContainer, c);
		}
		internal override bool ShouldCheckInterceptKey {
			get { return true; }
		}
		internal bool ProcessMnemonicCode(char c) { 
			Control ctrl = GetControlByMnemonicChar(c);
			if(ctrl != null && ctrl.AccessibilityObject != null) {
				ctrl.AccessibilityObject.DoDefaultAction();
				return true;
			}
			return false;
		}
	}
}
namespace DevExpress.XtraBars.Controls {
	public class AppMenuBarControl : PopupMenuBarControl {
		internal AppMenuBarControl(BarManager barManager, ApplicationMenu menu)
			: base(barManager, menu) {
		}
		protected override LinksNavigation CreateLinksNavigator() {
			return new AppMenuLinksNavigation(this);
		}
		public ApplicationMenu AppMenu { get { return (ApplicationMenu)Menu; } }
		protected override AnimationType GetAnimationType() { return AnimationType.Fade; }
		protected override Rectangle PopupChildForceBoundsCore { 
			get {
				if(Form != null && Form.ViewInfo != null && !Form.ViewInfo.RightPaneClientBounds.IsEmpty)
					return Form.RectangleToScreen(Form.ViewInfo.RightPaneClientBounds);
				return Rectangle.Empty; 
			} 
		}
		public ApplicationMenu ApplicationMenu { get { return Menu as ApplicationMenu; } }
		public override void OpenPopup(LocationInfo locInfo, IPopup parentPopup) {
			base.OpenPopup(locInfo, parentPopup);
		}
		new AppMenuForm Form { get { return base.Form as AppMenuForm; } }
		protected override ContainerKeyTipManager CreateKeyTipManager() {
			return new AppMenuKeyTipManager(Menu.Ribbon, this, VisibleLinks);
		}
		AppMenuKeyTipManager AppMenuKeyTipManager { get { return KeyTipManager as AppMenuKeyTipManager; } }
		protected override bool ProcessKeyPressCore(KeyPressEventArgs e) {
			if(KeyTipManager.Show) return base.ProcessKeyPressCore(e);
			if(AppMenuKeyTipManager != null) return AppMenuKeyTipManager.ProcessMnemonicCode(e.KeyChar);
			return false;
		}
	}
}
namespace DevExpress.XtraBars.ViewInfo {
	public class AppMenuControlViewInfo : PopupMenuBarControlViewInfo {
		public new AppMenuBarControl BarControl { get { return base.BarControl as AppMenuBarControl; } }
		public AppMenuControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar)
			: base(manager, parameters, bar) {
		}
		protected override MenuDrawMode GetDefaultMenuDrawMode() {
			if(BarControl.AppMenu.ShowRightPane) return MenuDrawMode.LargeImagesText;
			return MenuDrawMode.LargeImagesTextDescription;
		}
		public override Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			Size sz = base.CalcBarSize(g, sourceObject, width, maxHeight);
			if(BarControl.AppMenu.ShowRightPane) {
				sz.Width += BarControl.AppMenu.RightPaneWidth + AppMenuFormViewInfo.ClientRightIndent;
			}
			if(BarControl.AppMenu.BottomPaneControlContainer != null)
				sz.Width = Math.Max(sz.Width, BarControl.AppMenu.BottomPaneControlContainer.Width);
			return sz;
		}
	}
	public class AppMenuFormViewInfo : ControlFormViewInfo {
		Rectangle bottomBounds, topBounds, centerBounds;
		Rectangle rightPaneBounds, rightPaneClientBounds;
		Rectangle bottomPaneBounds, bottomPaneClientBounds;
		public AppMenuFormViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) { }
		public override void Clear() {
			base.Clear();
			this.bottomPaneClientBounds = this.bottomPaneBounds = this.rightPaneClientBounds = this.rightPaneBounds = this.bottomBounds = this.topBounds = Rectangle.Empty;
		}
		public ApplicationMenu Menu { get { return ControlForm.AppMenu; } }
		public new AppMenuForm ControlForm { get { return (AppMenuForm)base.ControlForm; } }
		public Rectangle RightPaneBounds { get { return rightPaneBounds; } set { rightPaneBounds = value; } }
		public Rectangle RightPaneClientBounds { get { return rightPaneClientBounds; } set { rightPaneClientBounds = value; } }
		public Rectangle BottomBounds { get { return bottomBounds; } set { bottomBounds = value; } }
		public Rectangle TopBounds { get { return topBounds; } set { topBounds = value; } }
		public Rectangle CenterBounds { get { return centerBounds; } set { centerBounds = value; } }
		public Rectangle BottomPaneBounds { get { return bottomPaneBounds; } set { bottomPaneBounds = value; } }
		public Rectangle BottomPaneClientBounds { get { return bottomPaneClientBounds; } set { bottomPaneClientBounds = value; } }
		public override Size CalcMinSize() {
			GInfo.AddGraphics(null);
			try {
				SkinElementInfo infoBottom = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundBottom]);
				Size bottomSize = SkinElementPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, infoBottom, new Rectangle(Point.Empty, CalcBottomContentSize(GInfo.Graphics))).Size;
				SkinElementInfo topInfo = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundTop]);
				Size topSize = topInfo.Element == null? Size.Empty: SkinElementPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, topInfo, new Rectangle(Point.Empty, new Size(100, 100))).Size;
				if(topSize != Size.Empty) {
					topSize.Width -= 100; topSize.Height -= 100;
					int headerHeight = ((RibbonBarManager)Manager).Ribbon.ViewInfo.Header.Bounds.Height + 1;
					topSize.Height = Math.Max(topSize.Height, headerHeight);
				}
				SkinElementInfo centerInfo = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackground]);
				Size centerSize = SkinElementPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, centerInfo, new Rectangle(Point.Empty, new Size(100, 100))).Size;
				centerSize.Width -= 100; centerSize.Height -= 100;
				return new Size(Math.Max(topSize.Width, centerSize.Width), topSize.Height + centerSize.Height + bottomSize.Height);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public override bool IsShowCaption { get { return false; } }
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle bounds) {
			Clear();
			this.WindowRect = bounds;
			if(bounds.IsEmpty) return;
			SkinElementInfo infoBottom = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundBottom], bounds);
			Size bottomSize = SkinElementPainter.CalcBoundsByClientRectangle(g, SkinElementPainter.Default, infoBottom, new Rectangle(Point.Empty, CalcBottomContentSize(g))).Size;
			this.topBounds = bounds;
			this.topBounds.Height -= bottomSize.Height;
			this.centerBounds = topBounds;
			this.bottomBounds = new Rectangle(bounds.X, this.topBounds.Bottom, bounds.Width, bottomSize.Height);
			SkinElementInfo topInfo = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundTop], TopBounds);
			if(topInfo.Element != null) {
				this.topBounds.Height = ((RibbonBarManager)Manager).Ribbon.ViewInfo.Header.Bounds.Height + 1;
				this.centerBounds.Height -= this.topBounds.Height;
				this.centerBounds.Y = this.topBounds.Bottom;
			}
			Rectangle rect = topInfo.Element == null ? TopBounds : CenterBounds;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackground], rect);
			bounds = ContentRect = ObjectPainter.GetObjectClientRectangle(g, SkinElementPainter.Default, info);
			if(ControlForm.AppMenu != null && ControlForm.AppMenu.BottomPaneControlContainer != null && ControlForm.AllowContainers) {
				PopupControlContainer bottomCont = ControlForm.AppMenu.BottomPaneControlContainer;
				SkinElementInfo bottomInfo = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundBottom], BottomBounds);
				this.bottomPaneClientBounds = ObjectPainter.GetObjectClientRectangle(g, SkinElementPainter.Default, bottomInfo);
				this.bottomPaneClientBounds.X = ContentRect.Right - bottomCont.Width;
				this.bottomPaneClientBounds.Width = bottomCont.Width;
				this.bottomPaneBounds = BottomPaneClientBounds;
			}
			ControlRect = CalcControlBounds(g, bounds); 
			int rightWidth = CalcRightPaneWidth(g);
			if(rightWidth == 0) return;
			bounds.Width -= rightWidth;
			ControlRect = CalcControlBounds(g, bounds);
			Rectangle right = bounds;
			right.X = bounds.Right;
			right.Width = rightWidth;
			RightPaneBounds = right;
			RightPaneClientBounds = ObjectPainter.GetObjectClientRectangle(g, SkinElementPainter.Default, new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuPane], RightPaneBounds));
			if(Menu.IsRightToLeft)
				RotateRects();
		}
		private void RotateRects() {
			topBounds.X = ControlForm.ClientRectangle.Right - (TopBounds.X + TopBounds.Width);
			centerBounds.X = ControlForm.ClientRectangle.Right - (CenterBounds.X + CenterBounds.Width);
			bottomBounds.X = ControlForm.ClientRectangle.Right - (bottomBounds.X + bottomBounds.Width);
			rightPaneBounds.X = ControlForm.ClientRectangle.Right - (rightPaneBounds.X + rightPaneBounds.Width);
			rightPaneClientBounds.X = ControlForm.ClientRectangle.Right - (rightPaneClientBounds.X + rightPaneClientBounds.Width);
			ControlRect.X = ControlForm.ClientRectangle.Right - (ControlRect.X + ControlRect.Width);
		}
		internal const int ClientRightIndent = 1;
		protected virtual Rectangle CalcControlBounds(Graphics g, Rectangle bounds) {
			bounds.Width -= ClientRightIndent;
			return bounds; 
		} 
		protected virtual Size CalcBottomContentSize(Graphics g) { 
			if(ControlForm.AppMenu == null || ControlForm.AppMenu.BottomPaneControlContainer == null || !ControlForm.AllowContainers) return new Size(0, 6);
			return new Size(0, ControlForm.AppMenu.BottomPaneControlContainer.Bounds.Height);
		} 
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		public virtual int CalcRightPaneWidth(Graphics g) {
			return Menu.ShowRightPane ? Menu.RightPaneWidth : 0;
		}
	}
}
namespace DevExpress.XtraBars.Forms {
	public class AppMenuForm : SubMenuControlForm {
		ApplicationMenu appMenu;
		public AppMenuForm(ApplicationMenu appMenu, BarManager manager, PopupMenuBarControl pc)
			: base(manager, pc, FormBehavior.SubMenu) {
			this.appMenu = appMenu;
			Initialize();
		}
		public new AppMenuFormViewInfo ViewInfo { get { return base.ViewInfo as AppMenuFormViewInfo; } }
		protected override bool AllowInitialize { get { return false; } }
		public ApplicationMenu AppMenu { get { return appMenu; } }
		int CornerRadius {
			get { 
				object res = RibbonSkins.GetSkin(Manager.GetController().LookAndFeel.ActiveLookAndFeel).Properties[RibbonSkins.OptApplicationMenuCornerRadius];
				if(res == null) return 4;
				return (int)res;
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), CornerRadius);
		}
		protected override void Dispose(bool disposing) {
			if(AppMenu != null ) {
				if(AppMenu.RightPaneControlContainer != null && Controls.Contains(AppMenu.RightPaneControlContainer))
					Controls.Remove(AppMenu.RightPaneControlContainer);
				if(AppMenu.BottomPaneControlContainer != null && Controls.Contains(AppMenu.BottomPaneControlContainer))
					Controls.Remove(AppMenu.BottomPaneControlContainer);	
			}
			base.Dispose(disposing);
		}
		protected virtual void AddControlContainer(PopupControlContainer cont) {
			if(cont == null || !AllowContainers) return;
			Controls.Add(cont);
			cont.Visible = true;
		}
		protected internal override void Initialize() {
			base.Initialize();
			if(AppMenu == null) return;
			AddControlContainer(AppMenu.RightPaneControlContainer);
			AddControlContainer(AppMenu.BottomPaneControlContainer);
		}
		protected internal override void UpdateViewInfo() {
			base.UpdateViewInfo();
			if(AppMenu == null) return;
			if(AppMenu.RightPaneControlContainer != null && AllowContainers)
				AppMenu.RightPaneControlContainer.Bounds = ViewInfo.RightPaneClientBounds;
			if(AppMenu.BottomPaneControlContainer != null && AllowContainers)
				AppMenu.BottomPaneControlContainer.Bounds = ViewInfo.BottomPaneBounds;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				if(GetApplicationButtonBounds().Contains(e.X, e.Y)) {
					AppMenu.HidePopup();
					return;
				}
			}
			base.OnMouseDown(e);
		}
		protected internal Rectangle GetApplicationButtonBounds() {
			RibbonControl ribbon = ((RibbonBarManager)Manager).Ribbon;
			Rectangle buttonBounds = ribbon.ViewInfo.ApplicationButton.Bounds;
			if(buttonBounds.IsEmpty) return Rectangle.Empty;
			return RectangleToClient(ribbon.RectangleToScreen(buttonBounds));
		}
		protected override void WndProc(ref Message msg) {
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT = (-1);
			if(msg.Msg == WM_NCHITTEST) {
				Point pt = PointToClient(new Point((int)msg.LParam));
				if(GetApplicationButtonBounds().Contains(pt)) {
					msg.Result = new IntPtr(HTTRANSPARENT);
					return;
				}
			}
			base.WndProc(ref msg);
		}
		protected internal virtual bool AllowContainers { 
			get { 
				if(AppMenu == null || AppMenu.Ribbon == null) return false;
				return !AppMenu.Ribbon.IsDesignMode; 
			} 
		}
	}
}
namespace DevExpress.XtraBars.Painters {
	public class SkinAppMenuFormPainter : SkinControlFormPainter {
		public SkinAppMenuFormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			AppMenuFormViewInfo vi = (AppMenuFormViewInfo)info;
			SkinElementInfo infoTop = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundTop], vi.TopBounds);
			SkinElementInfo infoCenter = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackground], vi.CenterBounds);
			SkinElementInfo infoBottom = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuBackgroundBottom], vi.BottomBounds);
			infoTop.RightToLeft = vi.Menu.IsRightToLeft;
			infoCenter.RightToLeft = vi.Menu.IsRightToLeft;
			infoBottom.RightToLeft = vi.Menu.IsRightToLeft;
			e.Graphics.ExcludeClip(vi.ControlRect);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, infoTop);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, infoCenter);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, infoBottom);
			DrawApplicationButton(e, vi);
			DrawRightPane(e, vi);
		}
		protected virtual void DrawRightPane(GraphicsInfoArgs e, AppMenuFormViewInfo vi) {
			if(vi.RightPaneBounds.IsEmpty || !e.Cache.IsNeedDrawRect(vi.RightPaneBounds)) return;
			SkinElementInfo infoPane = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinAppMenuPane], vi.RightPaneBounds);
			infoPane.RightToLeft = vi.Menu.IsRightToLeft;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, infoPane);
		}
		protected virtual bool ShouldDrawApplicationButton(RibbonControl ribbon, AppMenuFormViewInfo vi, Rectangle buttonBounds) {
			if(ribbon.GetRibbonStyle() == RibbonControlStyle.Office2007)
				return (vi.TopBounds.IntersectsWith(buttonBounds) && vi.TopBounds.Y > buttonBounds.Y);
			else
				return vi.TopBounds.IntersectsWith(buttonBounds);
		}
		protected virtual void DrawApplicationButton(GraphicsInfoArgs e, AppMenuFormViewInfo vi) {
			RibbonControl ribbon = ((RibbonBarManager)vi.Manager).Ribbon;
			Rectangle buttonBounds = vi.ControlForm.GetApplicationButtonBounds();
			if(buttonBounds.IsEmpty) return;
			if(ShouldDrawApplicationButton(ribbon, vi, buttonBounds)) {
				RibbonApplicationButtonInfo buttonInfo = ribbon.ViewInfo.GetApplicationButtonInfo();
				Point offset = new Point(buttonInfo.TextBounds.X - buttonInfo.Bounds.X, buttonInfo.TextBounds.Y - buttonInfo.Bounds.Y);
				Rectangle savedBounds = buttonInfo.Bounds;
				Rectangle savedTextBounds = buttonInfo.TextBounds;
				buttonInfo.Bounds = buttonBounds;
				buttonInfo.TextBounds = new Rectangle(buttonBounds.X + offset.X, buttonBounds.Y + offset.Y, buttonInfo.TextBounds.Width, buttonInfo.TextBounds.Height);
				buttonInfo.State |= (ObjectState.Pressed);
				ObjectPainter.DrawObject(e.Cache, new RibbonApplicationButtonPainter(), buttonInfo);
				ObjectPainter.DrawObject(e.Cache, new RibbonApplicationButtonTextPainter(), buttonInfo);
				buttonInfo.Bounds = savedBounds;
				buttonInfo.TextBounds = savedTextBounds;
			}
		}
	}
	public class AppMenuLinksNavigation : VerticalLinksNavigation {
		public AppMenuLinksNavigation(AppMenuBarControl control) : base(control) { }
		AppMenuBarControl AppMenuBarControl { get { return (AppMenuBarControl)Popup.CustomControl; } }
		Control GetFocusedControl() {
			Control focused = null;
			IntPtr handle = BarNativeMethods.GetFocus();
			if(handle != IntPtr.Zero) {
				focused = Control.FromHandle(handle);
			}
			return focused;
		}
		bool IsChildOf(Control child, Control parent) {
			while(child != null) {
				if(child == parent)
					return true;
				child = child.Parent;
			}
			return false;
		}
		bool IsChildOfAppMenuControlContainer(Control c, PopupControlContainer parent) {
			return parent != null && IsChildOf(c, parent);
		}
		public override bool IsInterceptKey(KeyEventArgs e, BarItemLink activeLink) {
			if(IsPaneControlFocused())
				return false;
			return base.IsInterceptKey(e, activeLink);
		}
		bool IsPaneControlFocused() {
			Control focused = GetFocusedControl();
			if(focused != null) {
				if(IsChildOfAppMenuControlContainer(focused, AppMenuBarControl.ApplicationMenu.RightPaneControlContainer))
					return true;
				if(IsChildOfAppMenuControlContainer(focused, AppMenuBarControl.ApplicationMenu.BottomPaneControlContainer))
					return true;
			}
			return false;
		}
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) {
			if(IsPaneControlFocused())
				return false;
			return base.IsNeededKey(e, activeLink);
		}
	}
}
