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
#if DXWhidbey
using System.Collections.Generic;
#endif
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.Accessibility;
using System.Windows.Forms.Design;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Drawing.Helpers;
using System.Reflection;
using DevExpress.Utils.Text.Internal;
using System.Security;
using DevExpress.XtraEditors.Native;
using System.Security.Permissions;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.ViewInfo {
	public class LabelControlViewInfo : BaseStyleControlViewInfo, IAnimatedItem, ISupportXtraAnimation, IServiceProvider, IStringImageProvider {
		static int HorizontalIndentBetweenTextAndLine { get { return 2; } }
		static int VerticalIndentBetweenTextAndLine { get { return 3; } }
		static int DefaultIndentBetweenImageAndText { get { return 3; } }
		static int ImageSpacing { get { return 2; } }
		Point[] line1;
		Point[] line2;
		Rectangle textRect; 
		Point textBaseline;
		LabelInfoArgs labelInfoArgs;
		Rectangle paddedRect;
		Rectangle imageBounds;
		LabelControlObjectPainter labelPainter;
		int textCenterY;
		public LabelControlViewInfo(BaseStyleControl obj)
			: base(obj) { 
			labelInfoArgs = new LabelInfoArgs(this, null,Bounds);
			labelInfoArgs.Assign(this);
			UpdatePainters();
			this.textRect = Rectangle.Empty;
			this.textBaseline = Point.Empty;
			this.imageBounds = Rectangle.Empty;
			this.textCenterY = 0;
			this.line1 = new Point[2];
			this.line2 = new Point[2];
		}
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(OwnerControl == null || OwnerControl.HtmlImages == null) return null;
			return OwnerControl.HtmlImages.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return OwnerControl == null ? null : OwnerControl.Site;
			return null;
		}
		#endregion
		public new LabelControlAppearanceObject Appearance {
			get { return base.Appearance as LabelControlAppearanceObject; }
			set { base.Appearance = value; }
		}
		AnimatedImageHelper imageHelper;
		protected internal AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(null);
				return imageHelper;
			}
		}
		protected internal virtual int IndentBetweenImageAndText {
			get {
				if(OwnerControl.IndentBetweenImageAndText == -1)
					return LabelControlViewInfo.DefaultIndentBetweenImageAndText;
				return OwnerControl.IndentBetweenImageAndText;
			}
		}
		public DashStyle LineStyle { get { return OwnerControl.LineStyle; } }
		public bool ShowLineShadow { get { return OwnerControl.ShowLineShadow; } }
		public Rectangle ImageBounds { get { return imageBounds; } }
		bool paintAppearanceDirty;
		public bool IsPaintAppearanceDirty {
			get { return paintAppearanceDirty; }
		}
		public void SetPaintAppearanceDirty() {
			paintAppearanceDirty = true;
			ResetAppearanceDefault();
		}
		protected virtual LabelControlAppearanceObject CreateAppearance() { return new LabelControlAppearanceObject(); }
		public override void UpdatePaintAppearance() {
			Appearance = GetAppearance();
			base.UpdatePaintAppearance();
			LabelControlAppearanceObject paintApp = PaintAppearance as LabelControlAppearanceObject;
			if(paintApp != null) {
				AppearanceObject styleApp = StyleController == null ? null : StyleController.Appearance;
				paintApp.Combine(new AppearanceObject[] { GetAppearance(), OwnerControl.Appearance, styleApp }, DefaultAppearance);
				paintApp.TextOptions.RightToLeft = RightToLeft;
			}
			if(ImageHelper.Image != OwnerControl.Appearance.GetImage()) {
				ImageHelper.Image = OwnerControl.Appearance.GetImage();
				OwnerControl.StopAnimation();
				OwnerControl.StartAnimation();
			}
			paintAppearanceDirty = false;
			UpdateStringInfoAppearance();
			LabelInfoArgs.DrawImageEnabled = OwnerControl.Enabled || OwnerControl.AppearanceDisabled.GetImage() != null;
		}
		private LabelControlAppearanceObject GetAppearance() {
			if(State == ObjectState.Disabled)
				return OwnerControl.AppearanceDisabled;
			else if(State == ObjectState.Hot)
				return OwnerControl.AppearanceHovered;
			else if(State == ObjectState.Pressed)
				return OwnerControl.AppearancePressed;
			return OwnerControl.Appearance;
		}
		protected virtual void UpdateStringInfoAppearance() {
			if(StringInfo != null) {
				StringInfo.UpdateAppearanceColors(PaintAppearance);
			}
		}
		public override bool AllowHtmlString { 
			get { return OwnerControl != null ? OwnerControl.AllowHtmlString : false; } 
		}
		protected override AppearanceObject CreatePaintAppearance() {
			return new LabelControlAppearanceObject();
		}
		Color lastForeColor;
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault defaultAppearance = LabelPainter.GetDefaultAppearance(OwnerControl);
				if(lastForeColor != defaultAppearance.ForeColor) SetPaintAppearanceDirty();
				if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && OwnerControl != null && OwnerControl.Enabled) {
					defaultAppearance.ForeColor = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabel].Color.GetForeColor();
					if(defaultAppearance.ForeColor.IsEmpty || defaultAppearance.ForeColor == Color.Transparent)
						defaultAppearance.ForeColor = GetForeColor();
				}
				lastForeColor = defaultAppearance.ForeColor;
				return defaultAppearance;
			}
		}
		protected virtual Color GetForeColor() {
			return LookAndFeelHelper.GetTransparentForeColor(LookAndFeel, OwnerControl, OwnerControl.Enabled);
		}
		public virtual int IndentBetweenTextAndLine {
			get {
				if(OwnerControl.LineOrientation == LabelLineOrientation.Vertical)return VerticalIndentBetweenTextAndLine;
				return HorizontalIndentBetweenTextAndLine;
			}
		}
		public LabelControlObjectPainter LabelPainter { get { return labelPainter; } }
		public Rectangle PaddedRect { get { return paddedRect; } }
		public LabelInfoArgs LabelInfoArgs { get { return labelInfoArgs; } }
		public new Size TextSize {
			get {
				return textRect.Size;
			}
		}
		public LabelControlAppearanceObject LabelPaintAppearance { get { return (LabelControlAppearanceObject)PaintAppearance; } }
		public Size CalcContentSizeByTextSize(Size textSize) {
			int h, w;
			if(!LabelPaintAppearance.HasImage) {
				w = textSize.Width;
				h = textSize.Height;
			}
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.None && LabelPaintAppearance.HasImage) {
				w = Math.Max(textSize.Width, LabelPaintAppearance.GetImageSize().Width);
				h = Math.Max(textSize.Height, LabelPaintAppearance.GetImageSize().Height);
			}
			else {
				w = LabelPaintAppearance.GetImageSize().Width + IndentBetweenImageAndText + textSize.Width + ImageSpacing;
				if(ImageTopAligned || ImageBottomAligned) {
					h = textSize.Height + LabelPaintAppearance.GetImageSize().Height + IndentBetweenImageAndText;
					w = Math.Max(textSize.Width, LabelPaintAppearance.GetImageSize().Width);
				}
				else if(textSize.Height < LabelPaintAppearance.GetImageSize().Height + ImageSpacing * 2)
					h = LabelPaintAppearance.GetImageSize().Height + ImageSpacing * 2;
				else h = textSize.Height;
			}
			if(OwnerControl.LineVisible) {
				if(OwnerControl.LineLocation == LineLocation.Left || OwnerControl.LineLocation == LineLocation.Right) w += IndentBetweenTextAndLine + 1;
				else if(OwnerControl.LineLocation == LineLocation.Top || OwnerControl.LineLocation == LineLocation.Bottom) h += IndentBetweenTextAndLine + 1;
			}
			Size size = CalcBorderSize();
			w += size.Width;
			h += size.Height;
			return new Size(w, h);
		}
		public Size CalcDefaultTextSize() {
			bool shouldReleaseGraphics = false;
			if(GInfo.Graphics == null) {
				shouldReleaseGraphics = true;
				GInfo.AddGraphics(null);
			}
			try {
				return PaintAppearance.CalcTextSize(GInfo.Graphics, "W", 0).ToSize();
			}
			finally {
				if(shouldReleaseGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		public override Size CalcBestFit(Graphics g) {
			return CalcContentSizeByTextSize(CalcTextSize(DisplayText, true, LabelAutoSizeMode.Horizontal));
		}
		public Size ContentSize {
			get {
				return CalcContentSizeByTextSize(TextSize);
			}
		}
		public Rectangle TextRect { 
			get {
				return textRect; 
			} 
		}
		public Point TextBaseline { get { return textBaseline; } }
		public Point[] Line1 { get { return line1; } }
		public Point[] Line2 { get { return line2; } }
		public new LabelControl OwnerControl { get { return base.OwnerControl as LabelControl; } }
		public override string DisplayText {
			get {
				if(OwnerControl == null)
					return "";
				return OwnerControl.Text;
			}
		}
		int GetTextAscentHeight() {
			if(AllowHtmlString)
				return StringInfo.Bounds.Height;
			int fontHeight;
			Graphics g = this.GInfo.AddGraphics(null);
			try {
				fontHeight = TextUtils.GetFontAscentHeight(g, this.PaintAppearance.Font);
			}
			finally {
				this.GInfo.ReleaseGraphics();
			}
			return fontHeight;
		}
		protected internal Size CalcTextSize(string Text, bool useHotkeyPrefix) {
			return CalcTextSize(Text, useHotkeyPrefix, OwnerControl.RealAutoSizeMode);
		}
		protected internal Size CalcTextSize(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode) {
			return CalcTextSize(Text, useHotkeyPrefix, mode, PaddedRect.Width, PaddedRect.Height);
		}
		protected internal HKeyPrefix GetHKeyPrefix(HotkeyPrefix prefix) {
			if(prefix == HotkeyPrefix.Hide)
				return HKeyPrefix.Hide;
			if(prefix == HotkeyPrefix.Show)
				return HKeyPrefix.Show;
			return HKeyPrefix.None;
		}
		protected internal Size CalcTextSize(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth) {
			return CalcTextSize(Text, useHotkeyPrefix, mode, predWidth, int.MaxValue);
		}
		protected internal virtual Size CalcTextSize(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth, int predHeight) {
			return AllowHtmlString ? CalcHtmlTextSize(Text, useHotkeyPrefix, mode, predWidth) :
				CalcSimpleTextSize(Text, useHotkeyPrefix, mode, predWidth, predHeight);
		}
		object GetHtmlDrawContext() { return this; }
		protected virtual bool UnderlineLinks { get { return true; } }
		protected internal DevExpress.Utils.Text.StringInfo CalcStringInfo(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth) {
			int width = 0;
			using(AppearanceObject obj = PaintAppearance.Clone() as AppearanceObject) {
				bool addGraphics = false;
				if(GInfo.Graphics == null) {
					GInfo.AddGraphics(null);
					addGraphics = true;
				}
				try {
					if(useHotkeyPrefix)
						obj.TextOptions.HotkeyPrefix = GetHKeyPrefix(OwnerControl.HotkeyPrefixState);
					else
						obj.TextOptions.HotkeyPrefix = HKeyPrefix.Hide;
					if(mode == LabelAutoSizeMode.Vertical || mode == LabelAutoSizeMode.None) {
						width = predWidth;
						if((ImageLeftAligned || ImageRightAligned) && LabelPaintAppearance.HasImage)
							width -= GetImageWidth();
					}
					if(mode == LabelAutoSizeMode.Vertical || obj.TextOptions.WordWrap == WordWrap.Wrap)
						obj.TextOptions.WordWrap = WordWrap.Wrap;
					return StringPainter.Default.Calculate(GInfo.Graphics, obj, obj.GetTextOptions(), new HyperlinkSettings() { Color = GetHyperLinkColor(), Underline = UnderlineLinks }, Text, width, null, GetHtmlDrawContext());
				}
				finally {
					if(addGraphics) GInfo.ReleaseGraphics();
				}
			}
		}
		protected internal Size CalcHtmlTextSize(string text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth) {
			StringInfo = CalcStringInfo(text, useHotkeyPrefix, mode, predWidth);
			this.textCenterY = StringInfo.Bounds.Height / 2;
			return StringInfo.Bounds.Size;
		}
		protected virtual Color GetHyperLinkColor() {
			return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor, Color.Blue);
		}
		protected internal Size CalcSimpleTextSize(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth) {
			return CalcSimpleTextSize(Text, useHotkeyPrefix, mode, predWidth, int.MaxValue);
		}
		protected internal Size CalcSimpleTextSize(string Text, bool useHotkeyPrefix, LabelAutoSizeMode mode, int predWidth, int predHeight) {
			using(StringFormat format = PaintAppearance.GetStringFormat().Clone() as StringFormat) {
				int width = 0;
				if(useHotkeyPrefix)
					format.HotkeyPrefix = OwnerControl.HotkeyPrefixState;
				else
					format.HotkeyPrefix = HotkeyPrefix.Hide;
				if(mode == LabelAutoSizeMode.Vertical || mode == LabelAutoSizeMode.None) {
					width = predWidth;
					if((ImageLeftAligned || ImageRightAligned) && LabelPaintAppearance.HasImage)
						width -= GetImageWidth();
					if(mode == LabelAutoSizeMode.Vertical)
						format.FormatFlags &= ~StringFormatFlags.NoWrap;
				}
				bool addGraphics = false;
				if(this.GInfo.Graphics == null) {
					this.GInfo.AddGraphics(null);
					addGraphics = true;
				}
				try {
					SizeF sizef = Size.Empty;
					if(ShouldUsePredefinedHeightWithCalcTextSize(format.FormatFlags, mode))
						sizef = this.PaintAppearance.CalcTextSize(this.GInfo.Graphics, format, Text, width, predHeight, out isCroppedCore);
					else {
						SizeF bestSize = this.PaintAppearance.CalcTextSize(this.GInfo.Graphics, format, Text, 0);
						sizef = this.PaintAppearance.CalcTextSize(this.GInfo.Graphics, format, Text, width);
						isCroppedCore = OwnerControl.AutoSizeMode != LabelAutoSizeMode.Vertical && bestSize.Width > sizef.Width;
					}
					sizef.Width += 0.5f;
					sizef.Height += 0.5f;
					this.textCenterY = (int)(OwnerControl.Appearance.Font.GetHeight(this.GInfo.Graphics) / 2);
					return sizef.ToSize();
				}
				finally {
					if(addGraphics) this.GInfo.ReleaseGraphics();
				}
			}
		}
		bool isCroppedCore;
		public bool IsCropped { get { return isCroppedCore; } set { isCroppedCore = value; } }
		bool ShouldUsePredefinedHeightWithCalcTextSize(StringFormatFlags flags, LabelAutoSizeMode mode) {
			if(mode == LabelAutoSizeMode.Vertical) return false;
			if((flags & StringFormatFlags.NoWrap) == 0) return true;
			return false;
		}
		void CalcTextSize(bool useHotkeyPrefix) {
			textRect.Size = CalcTextSize(DisplayText, useHotkeyPrefix);
		}
		int GetImageWidth() {
			if(!LabelPaintAppearance.HasImage) return 0;
			return IndentBetweenImageAndText + LabelPaintAppearance.GetImageSize().Width + ImageSpacing;
		}
		bool ImageLeftAligned {
			get {
				return OwnerControl.ImageAlignToText == ImageAlignToText.LeftTop ||
					OwnerControl.ImageAlignToText == ImageAlignToText.LeftCenter ||
					OwnerControl.ImageAlignToText == ImageAlignToText.LeftBottom;
			}
		}
		bool ImageRightAligned {
			get {
				return OwnerControl.ImageAlignToText == ImageAlignToText.RightTop ||
					OwnerControl.ImageAlignToText == ImageAlignToText.RightCenter ||
					OwnerControl.ImageAlignToText == ImageAlignToText.RightBottom;
			}
		}
		bool ImageTopAligned {
			get {
				return OwnerControl.ImageAlignToText == ImageAlignToText.TopLeft ||
					OwnerControl.ImageAlignToText == ImageAlignToText.TopCenter ||
					OwnerControl.ImageAlignToText == ImageAlignToText.TopRight;
			}
		}
		bool ImageBottomAligned {
			get {
				return OwnerControl.ImageAlignToText == ImageAlignToText.BottomLeft ||
					OwnerControl.ImageAlignToText == ImageAlignToText.BottomCenter ||
					OwnerControl.ImageAlignToText == ImageAlignToText.BottomRight;
			}
		}
		VertAlignment GetTextVertAlign() {
			if(LabelPaintAppearance.Image == null || LabelPaintAppearance.TextOptions.VAlignment != VertAlignment.Default || OwnerControl.AutoSizeMode == LabelAutoSizeMode.None)
				return LabelPaintAppearance.TextOptions.VAlignment;
			if(ImageTopAligned)
				return VertAlignment.Bottom;
			if(ImageBottomAligned)
				return VertAlignment.Top;
			return LabelPaintAppearance.TextOptions.VAlignment;
		}
		void CalcTextTopLeftPoint() {
			int addWidth = 0;
			int addHeight = 0;
			if(AllowHtmlString)
				textRect = StringInfo.Bounds;
			switch(GetTextVertAlign()) {
				case VertAlignment.Top:
					if(OwnerControl.LineVisible && OwnerControl.LineLocation == LineLocation.Top) 
						addHeight += IndentBetweenTextAndLine + 1; 
					if(ImageTopAligned) 
						addHeight += ImageBounds.Height;
					textRect.Y = PaddedRect.Y + addHeight;
					break;
				case VertAlignment.Center:
				case VertAlignment.Default:
					textRect.Y = PaddedRect.Y + (PaddedRect.Height - textRect.Height) / 2;
				break;
				case VertAlignment.Bottom:
					if(OwnerControl.LineVisible && OwnerControl.LineLocation == LineLocation.Bottom) addHeight += IndentBetweenTextAndLine + 1; 
					if(ImageBottomAligned) addHeight += ImageBounds.Height;
					textRect.Y = PaddedRect.Bottom - addHeight - TextRect.Height;
					break;
			}
			switch(LabelPaintAppearance.FinalAlign(LabelPaintAppearance.HAlignment, OwnerControl.RightToLeft)) {
				case HorzAlignment.Near:
				case HorzAlignment.Default:
					if(OwnerControl.LineVisible && OwnerControl.LineLocation == LineLocation.Left) addWidth += IndentBetweenTextAndLine + 1;
					if(ImageLeftAligned) addWidth += GetImageWidth();
					textRect.X = PaddedRect.X + addWidth;
					break;
				case HorzAlignment.Center:
					if(ImageLeftAligned) addWidth = GetImageWidth();
					else if(ImageRightAligned) addWidth = - GetImageWidth();
					textRect.X = PaddedRect.X + (PaddedRect.Width - textRect.Width + addWidth) / 2;
					break;
				case HorzAlignment.Far:
					if(OwnerControl.LineVisible && OwnerControl.LineLocation == LineLocation.Right) addWidth += IndentBetweenTextAndLine + 1;
					if(ImageRightAligned) addWidth += GetImageWidth();
					textRect.X = Math.Max(PaddedRect.Right - addWidth - textRect.Width, 0);
					break;
			}
			LabelInfoArgs.TextRect = textRect;
			if(AllowHtmlString)
				StringPainter.Default.UpdateLocation(StringInfo, TextRect);
		}
		protected virtual Rectangle ItemRect {
			get {
				if(!LabelPaintAppearance.HasImage) return TextRect;
				Rectangle rect = Rectangle.Empty;
				rect.X = Math.Min(TextRect.X, ImageBounds.X);
				rect.Y = Math.Min(TextRect.Y, ImageBounds.Y);
				rect.Width = Math.Max(TextRect.Right, ImageBounds.Right) - rect.X;
				rect.Height = Math.Max(TextRect.Bottom, ImageBounds.Bottom) - rect.Y;
				return rect;
			}
		}
		protected virtual bool EmptyLabel { get { return !LabelPaintAppearance.HasImage && DisplayText == string.Empty; } }
		void CalcLines() {
			int linesX, linesY;
			if(OwnerControl.LineLocation == LineLocation.Default || OwnerControl.LineLocation == LineLocation.Center) {
				if(OwnerControl.LineOrientation != LabelLineOrientation.Vertical) {
					if(TextRect.Height == 0) 
						linesY = PaddedRect.Y + PaddedRect.Height / 2;
					else 
					linesY = TextRect.Bottom - this.textCenterY;
					this.line1[0] = new Point(PaddedRect.Left, linesY);
					if(EmptyLabel) {
						this.line1[1] = new Point(PaddedRect.Right, linesY);
						this.line2[0] = this.line2[1] = Point.Empty;
					}
					else {
						this.line1[1] = new Point(Math.Max(this.line1[0].X, ItemRect.X - IndentBetweenTextAndLine), linesY);
						this.line2[0] = new Point(ItemRect.Right + IndentBetweenTextAndLine, linesY);
						this.line2[1] = new Point(PaddedRect.Right, linesY);
					}
				}
				else {
					if(ItemRect.Width == 0) 
						linesX = PaddedRect.X + PaddedRect.Width / 2;
					else 
					linesX = ItemRect.X + ItemRect.Width / 2;
					this.line1[0] = new Point(linesX, PaddedRect.Top);
					if(EmptyLabel) {
						this.line1[1] = new Point(linesX, PaddedRect.Bottom);
						this.line2[0] = this.line2[1] = Point.Empty;
					}
					else {
						this.line1[1] = new Point(linesX, ItemRect.Y - IndentBetweenTextAndLine);
						this.line2[0] = new Point(linesX, ItemRect.Bottom + IndentBetweenTextAndLine);
						this.line2[1] = new Point(linesX, PaddedRect.Bottom);
					}
				}
			}
			else if(OwnerControl.LineLocation == LineLocation.Top || OwnerControl.LineLocation == LineLocation.Bottom) {
				linesY = OwnerControl.LineLocation == LineLocation.Top? ItemRect.Y - IndentBetweenTextAndLine: ItemRect.Bottom + IndentBetweenTextAndLine;
				this.line1[0] = new Point(PaddedRect.Left, linesY);
				this.line1[1] = new Point(PaddedRect.Right, linesY);
				this.line2[0] = this.line2[1] = Point.Empty;
			}
			else if(OwnerControl.LineLocation == LineLocation.Left || OwnerControl.LineLocation == LineLocation.Right) {
				linesX = OwnerControl.LineLocation == LineLocation.Left ? ItemRect.X - IndentBetweenTextAndLine: ItemRect.Right + IndentBetweenTextAndLine;
				this.line1[0] = new Point(linesX, PaddedRect.Top);
				this.line1[1] = new Point(linesX, PaddedRect.Bottom);
				this.line2[0] = this.line2[1] = Point.Empty;
			}
		}
		void CalcTextBaseline() {
			textBaseline.X = textRect.X;
			textBaseline.Y = textRect.Y + GetTextAscentHeight() + 1;
		}
		void CalcTextPoints() {
			CalcTextSize(true);
			CalcTextTopLeftPoint();
			CalcTextBaseline();
		}
		public override TextOptions DefaultTextOptions { 
			get {
				WordWrap wrap = WordWrap.Default;
				VertAlignment valign = VertAlignment.Default;
#if !DXWhidbey
				wrap = WordWrap.Wrap;
				valign = VertAlignment.Top;
#endif                
				Trimming trimming = Trimming.Default;
				if(OwnerControl.RealAutoSizeMode == LabelAutoSizeMode.Vertical) wrap = WordWrap.Wrap;
				if(OwnerControl.AutoEllipsis) trimming = Trimming.EllipsisCharacter;
				return new TextOptions(HorzAlignment.Default, valign, wrap, trimming);
			} 
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			paddedRect = ClientRect;
#if DXWhidbey
			paddedRect.X += OwnerControl.Padding.Left;
			paddedRect.Y += OwnerControl.Padding.Top;
			paddedRect.Width -= OwnerControl.Padding.Left + OwnerControl.Padding.Right;
			paddedRect.Height -= OwnerControl.Padding.Top + OwnerControl.Padding.Bottom;
#endif
			CalcImageSize();
			CalcTextPoints();
			CalcImageDrawCoords();
			CalcLines();
			LabelInfoArgs.Assign(this);
		}
		void CalcImageSize() {
			imageBounds.Width = ((LabelControlAppearanceObject)PaintAppearance).GetImageSize().Width;
			imageBounds.Height = ((LabelControlAppearanceObject)PaintAppearance).GetImageSize().Height;
		}
		void CalcImageDrawCoords() {
			if(OwnerControl.ImageAlignToText != ImageAlignToText.None)
				CalcAttachedImageDrawCoords();
			else
				CalcUnattachedImageDrawCoords();
		}
		void CalcAttachedImageDrawCoords() {
			if(OwnerControl.ImageAlignToText == ImageAlignToText.LeftTop ||
				OwnerControl.ImageAlignToText == ImageAlignToText.RightTop) {
				imageBounds.Y = TextRect.Y;
			}
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.LeftCenter ||
				OwnerControl.ImageAlignToText == ImageAlignToText.RightCenter) {
				imageBounds.Y = TextRect.Y - (imageBounds.Height - TextRect.Height) / 2;
			}
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.LeftBottom ||
				OwnerControl.ImageAlignToText == ImageAlignToText.RightBottom) {
				imageBounds.Y = TextRect.Bottom - ImageBounds.Height;
			}
			else if(ImageTopAligned) imageBounds.Y = TextRect.Top - ImageBounds.Height - IndentBetweenImageAndText;
			else if(ImageBottomAligned) imageBounds.Y = TextRect.Bottom + IndentBetweenImageAndText;
			if(ImageLeftAligned)
				imageBounds.X = TextRect.X - IndentBetweenImageAndText - imageBounds.Width;
			else if(ImageRightAligned)
				imageBounds.X = TextRect.Right + IndentBetweenImageAndText;
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.TopLeft ||
				OwnerControl.ImageAlignToText == ImageAlignToText.BottomLeft)
				imageBounds.X = TextRect.X;
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.TopCenter ||
				OwnerControl.ImageAlignToText == ImageAlignToText.BottomCenter)
				imageBounds.X = TextRect.X + (TextRect.Width - ImageBounds.Width) / 2;
			else if(OwnerControl.ImageAlignToText == ImageAlignToText.TopRight ||
				OwnerControl.ImageAlignToText == ImageAlignToText.BottomRight)
				imageBounds.X = TextRect.Right - ImageBounds.Width;
		}
		void CalcUnattachedImageDrawCoords() {
			if(!LabelPaintAppearance.HasImage) return;
			Size size = LabelPaintAppearance.GetImageSize();
			int xPos = PaddedRect.X + ImageSpacing;
			int yPos = PaddedRect.Y + ImageSpacing;
			switch(LabelPaintAppearance.FinalAlign(LabelPaintAppearance.ImageAlign, OwnerControl.RightToLeft)) {
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					xPos = PaddedRect.X + PaddedRect.Width - ImageSpacing - size.Width;
					break;
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					xPos = PaddedRect.X + (PaddedRect.Width - size.Width) / 2;
					break;
			}
			switch(LabelPaintAppearance.FinalAlign(LabelPaintAppearance.ImageAlign, OwnerControl.RightToLeft)) {
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomRight:
					yPos = PaddedRect.Y + PaddedRect.Height - ImageSpacing - size.Height;
					break;
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
					yPos = PaddedRect.Y + (PaddedRect.Height - size.Height) / 2;
					break;
			}
			imageBounds.X = xPos;
			imageBounds.Y = yPos;
			imageBounds.Width = size.Width;
			imageBounds.Height = size.Height;
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			labelPainter = GetLabelPainter();
		}
		protected virtual LabelControlObjectPainter GetLabelPainter() {
			return LabelPaintersHelper.GetPainter(OwnerControl.BorderStyle, OwnerControl.LookAndFeel);
		}
		protected override void CalcClientRect(Rectangle bounds) {
			if(OwnerControl.BorderStyle == BorderStyles.Default)
				this.fClientRect = bounds;
			else
				base.CalcClientRect(bounds);
		}
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds {
			get { return ImageBounds; }
		}
		int IAnimatedItem.AnimationInterval { get { return ImageHelper.AnimationInterval; } }
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner {
			get { return OwnerControl; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return ImageHelper.FramesCount > 1; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return OwnerControl; }
		}
		#endregion
		protected internal virtual void OnStateChanged(ObjectState newState, bool updateLayout) {
			if(State == newState)
				return;
			State = newState;
			ForceStateChanged();
			if(updateLayout)
				OwnerControl.LayoutChanged();
		}
		protected internal virtual void ForceStateChanged() {
			LabelInfoArgs.Assign(this);
			SetPaintAppearanceDirty();
			OwnerControl.Invalidate();
		}
		protected override BorderPainter GetBorderPainter() {
			if(OwnerControl != null && OwnerControl.BorderStyle == BorderStyles.Default)
				return new EmptyBorderPainter();
			return base.GetBorderPainter();
		}
		protected internal virtual Size CalcBorderSize() {
			Rectangle rect = BorderPainter.CalcBoundsByClientRectangle(GetBorderArgs(new Rectangle(0, 0, 100, 100)));
			return new Size(rect.Width - 100, rect.Height - 100);
		}
		public bool AllowGlyphSkinning { get { return OwnerControl.AllowGlyphSkinning == DefaultBoolean.True; } }
	}
	public class HyperlinkLabelControlViewInfo : LabelControlViewInfo {
		public HyperlinkLabelControlViewInfo(BaseStyleControl control) : base(control) { }
		internal string HyperLinkText { get; set; }
		public override string DisplayText {
			get {
				if(OwnerControl == null)
					return null;
				if(HyperLinkText == null) {
					List<StringBlock> blocks = StringPainter.Default.Parse(PaintAppearance, base.DisplayText);
					if(blocks.Find(block => block.Type == StringBlockType.Link) != null)
						HyperLinkText = base.DisplayText;
					else 
						HyperLinkText = "<href>" + base.DisplayText + "</href>";
				}
				return HyperLinkText;
			}
		}
		protected override LabelControlAppearanceObject CreateAppearance() {
			return new HyperlinkLabelControlAppearanceObject();
		}
		LinkBehavior GetIELinkBehavior() {
			try {
				Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\Main");
				if(registryKey == null)
					return LinkBehavior.AlwaysUnderline;
				string hyperlinkUnderline = (string)registryKey.GetValue("Anchor Underline");
				registryKey.Close();
				if(hyperlinkUnderline != null && string.Compare(hyperlinkUnderline, "no", true, CultureInfo.InvariantCulture) == 0)
					return LinkBehavior.NeverUnderline;
				return hyperlinkUnderline != null && string.Compare(hyperlinkUnderline, "hover", true, CultureInfo.InvariantCulture) == 0 ? LinkBehavior.HoverUnderline : LinkBehavior.AlwaysUnderline;
			}
			finally {
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(PaintAppearance.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				PaintAppearance.TextOptions.HotkeyPrefix = HKeyPrefix.Hide;
			UpdateStringInfoAppearance();
		}
		LinkBehavior? SystemLinkBehavior { get; set; }
		protected override bool UnderlineLinks {
			get {
				if(HyperLinkOwner == null)
					return false;
				LinkBehavior behavior = HyperLinkOwner.LinkBehavior;
				if(behavior == LinkBehavior.SystemDefault)  {
					if(!SystemLinkBehavior.HasValue)
						SystemLinkBehavior = GetIELinkBehavior();
					behavior = SystemLinkBehavior.Value;
				}
				if(behavior == LinkBehavior.AlwaysUnderline)
					return true;
				if(behavior == LinkBehavior.NeverUnderline)
					return false;
				if(behavior == LinkBehavior.HoverUnderline)
					return StringInfo != null && StringInfo.GetLinkByPoint(HyperLinkOwner.PointToClient(Control.MousePosition)) != null;
				return base.UnderlineLinks;
			}
		}
		public override bool AllowHtmlString {
			get {
				return true;
			}
		}
		protected override Color GetHyperLinkColor() {
			if(!HyperLinkOwner.Enabled)
				return DisabledColor;
			else if(Control.MouseButtons == System.Windows.Forms.MouseButtons.Left && ClientRect.Contains(HyperLinkOwner.PointToClient(Control.MousePosition)))
				return PressedColor;
			else if(HyperLinkOwner.LinkVisited)
				return VisitedColor;
			return LinkColor;
		}
		protected override void UpdateStringInfoAppearance() {
			if(StringInfo == null)
				return;
			bool addGraphics = GInfo.Graphics == null;
			if(addGraphics)
				GInfo.AddGraphics(null);
			try {
				if(OwnerControl.RealAutoSizeMode == LabelAutoSizeMode.Vertical && PaintAppearance.TextOptions.WordWrap == WordWrap.Default)
					PaintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
				if(StringInfo != null && GInfo.Graphics != null) {
					StringInfo = StringPainter.Default.Calculate(GInfo.Graphics, PaintAppearance, new HyperlinkSettings() { Color = GetHyperLinkColor(), Underline = UnderlineLinks }, DisplayText, StringInfo.Bounds);
				}
			}
			finally {
				if(addGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		protected HyperlinkLabelControl HyperLinkOwner { get { return (HyperlinkLabelControl)OwnerControl; } }
		protected virtual Color DefaultLinkColor {
			get {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return Color.Purple;
				return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor, Color.Blue);
			}
		}
		protected virtual Color DefaultLinkVisitedColor {
			get {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return Color.Purple;
				return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColorVisited, Color.Purple);
			}
		}
		protected virtual Color DefaultLinkPressedColor {
			get {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return Color.Red;
				return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColorPressed, Color.Red);
			}
		}
		protected virtual Color DefaultLinkDisabledColor {
			get {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return SystemColors.GrayText;
				return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.GrayText);
			}
		}
		protected virtual Color LinkColor {
			get {
				return !HyperLinkOwner.Appearance.Options.UseLinkColor || HyperLinkOwner.Appearance.LinkColor.IsEmpty ? DefaultLinkColor : HyperLinkOwner.Appearance.LinkColor;
			}
		}
		protected virtual Color PressedColor {
			get {
				return !HyperLinkOwner.Appearance.Options.UsePressedColor || HyperLinkOwner.Appearance.PressedColor.IsEmpty ? DefaultLinkPressedColor : HyperLinkOwner.Appearance.PressedColor;
			}
		}
		protected virtual Color VisitedColor {
			get {
				return !HyperLinkOwner.Appearance.Options.UseVisitedColor || HyperLinkOwner.Appearance.VisitedColor.IsEmpty ? DefaultLinkVisitedColor : HyperLinkOwner.Appearance.VisitedColor;
			}
		}
		protected virtual Color DisabledColor {
			get {
				return !HyperLinkOwner.Appearance.Options.UseDisabledColor || HyperLinkOwner.Appearance.DisabledColor.IsEmpty ? DefaultLinkDisabledColor : HyperLinkOwner.Appearance.DisabledColor;
			}
		}
	}
	internal static class ImageAndTextLayoutHelper {
		public static bool ImageLeftAligned(ImageAlignToText align) {
				return align == ImageAlignToText.LeftTop ||
					align == ImageAlignToText.LeftCenter ||
					align == ImageAlignToText.LeftBottom;
		}
		public static bool ImageRightAligned(ImageAlignToText align) {
				return align == ImageAlignToText.RightTop ||
					align == ImageAlignToText.RightCenter ||
					align == ImageAlignToText.RightBottom;
		}
		public static bool ImageTopAligned(ImageAlignToText align) {
				return align == ImageAlignToText.TopLeft ||
					align == ImageAlignToText.TopCenter ||
					align == ImageAlignToText.TopRight;
		}
		public static bool ImageBottomAligned(ImageAlignToText align) {
			return align == ImageAlignToText.BottomLeft ||
				align == ImageAlignToText.BottomCenter ||
				align == ImageAlignToText.BottomRight;
		}
		public static Rectangle CalcImageAttachedToTextBounds(Rectangle textBounds, Size imageSize, ImageAlignToText align, int indentBetweenImageAndText) {
			Rectangle imageBounds = new Rectangle(Point.Empty, imageSize);
			if(align == ImageAlignToText.LeftTop ||
				align == ImageAlignToText.RightTop) {
				imageBounds.Y = textBounds.Y;
			}
			else if(align == ImageAlignToText.LeftCenter ||
				align == ImageAlignToText.RightCenter) {
					imageBounds.Y = textBounds.Y - (imageBounds.Height - textBounds.Height) / 2;
			}
			else if(align == ImageAlignToText.LeftBottom ||
				align == ImageAlignToText.RightBottom) {
					imageBounds.Y = textBounds.Bottom - imageBounds.Height;
			}
			else if(ImageTopAligned(align)) imageBounds.Y = textBounds.Top - imageBounds.Height - indentBetweenImageAndText;
			else if(ImageBottomAligned(align)) imageBounds.Y = textBounds.Bottom + indentBetweenImageAndText;
			if(ImageLeftAligned(align))
				imageBounds.X = textBounds.X - indentBetweenImageAndText - imageBounds.Width;
			else if(ImageRightAligned(align))
				imageBounds.X = textBounds.Right + indentBetweenImageAndText;
			else if(align == ImageAlignToText.TopLeft || align == ImageAlignToText.BottomLeft)
				imageBounds.X = textBounds.X;
			else if(align == ImageAlignToText.TopCenter || align == ImageAlignToText.BottomCenter)
				imageBounds.X = textBounds.X + (textBounds.Width - imageBounds.Width) / 2;
			else if(align == ImageAlignToText.TopRight || align == ImageAlignToText.BottomRight)
				imageBounds.X = textBounds.Right - imageBounds.Width;
			return imageBounds;
		}
		public static Rectangle CalcTextBounds(Rectangle contentBounds, Size textSize, Size imageSize, ImageAlignToText align, int indentBetweenImageAndText, AppearanceObject textAppearance) {
			Rectangle textBounds = new Rectangle(Point.Empty, textSize);
			if(textAppearance.TextOptions.HAlignment == HorzAlignment.Near) {
				if(ImageLeftAligned(align) && imageSize.Width > 0)
					textBounds.X = contentBounds.X + imageSize.Width + indentBetweenImageAndText;
				else
					textBounds.X = contentBounds.X;
			}
			else if(textAppearance.TextOptions.HAlignment == HorzAlignment.Far) {
				if(ImageRightAligned(align) && imageSize.Width > 0)
					textBounds.X = contentBounds.Right - imageSize.Width - indentBetweenImageAndText - textSize.Width;
				else
					textBounds.X = contentBounds.Right - textSize.Width;
			}
			else {
				int textWidth = textSize.Width;
				int imageWidth = 0;
				if((ImageLeftAligned(align) || ImageRightAligned(align)) && imageSize.Width > 0) {
					imageWidth = indentBetweenImageAndText + imageSize.Width;
					textWidth = textWidth + imageWidth;
					if(textWidth > contentBounds.Width) {
						textWidth = contentBounds.Width - imageWidth;
					}
				}
				textBounds.X = contentBounds.X + (contentBounds.Width - textWidth) / 2;
				if(ImageLeftAligned(align))
					textBounds.X += imageWidth;
			}
			if(textAppearance.TextOptions.VAlignment == VertAlignment.Top) {
				if(ImageTopAligned(align) && imageSize.Height > 0)
					textBounds.Y = contentBounds.Y + imageSize.Height + indentBetweenImageAndText;
				else
					textBounds.Y = contentBounds.Y;
			}
			else if(textAppearance.TextOptions.VAlignment == VertAlignment.Bottom) {
				if(ImageBottomAligned(align) && imageSize.Height > 0)
					textBounds.Y = contentBounds.Bottom - imageSize.Height - indentBetweenImageAndText - textSize.Height;
				else
					textBounds.Y = contentBounds.Bottom - textSize.Height;
			}
			else {
				int textHeight = textSize.Height;
				int imageHeight = 0;
				if((ImageTopAligned(align) || ImageBottomAligned(align)) && imageSize.Height > 0) {
					imageHeight = indentBetweenImageAndText + imageSize.Height;
					textHeight += imageHeight;
				}
				textBounds.Y = contentBounds.Y + (contentBounds.Height - textHeight) / 2;
				if(ImageTopAligned(align))
					textBounds.Y += imageHeight;
			}
			return textBounds;
		}
	}
	public class LabelInfoArgs : ObjectInfoArgs {
		LabelControlViewInfo viewInfo;
		public LabelControlViewInfo ViewInfo { get { return viewInfo; } }
		public LabelInfoArgs(LabelControlViewInfo vInfo, GraphicsCache cache, Rectangle rect):base(cache, rect, ObjectState.Normal) {
			viewInfo = vInfo;
			DrawImageEnabled = true;
		}
		public virtual void Assign(LabelControlViewInfo vInfo) {
			BorderStyle = ViewInfo.OwnerControl.BorderStyle;
			LineColor = ViewInfo.OwnerControl.LineColor;
			Enabled = ViewInfo.OwnerControl.Enabled;
			LabelPainter = ViewInfo.LabelPainter;
			Line1 = ViewInfo.Line1;
			Line2 = ViewInfo.Line2;
			LineLocation = ViewInfo.OwnerControl.LineLocation;
			Appearance = ViewInfo.PaintAppearance as LabelControlAppearanceObject;
			DefaultTextOptions = ViewInfo.DefaultTextOptions;
			TextRect = ViewInfo.TextRect;
			HotkeyPrefixState = ViewInfo.OwnerControl.HotkeyPrefixState;
			DisplayText = ViewInfo.DisplayText;
			PaintAppearance = ViewInfo.PaintAppearance;
			ImageBounds = ViewInfo.ImageBounds;
			LineVisible = ViewInfo.OwnerControl.LineVisible;
			LineOrientation = ViewInfo.OwnerControl.LineOrientation;
			State = ViewInfo.State;
			LineStyle = ViewInfo.LineStyle;
			ShowLineShadow = ViewInfo.ShowLineShadow;
			AllowGlyphSkinning = ViewInfo.AllowGlyphSkinning;
			StringInfo = ViewInfo.StringInfo;
		}
		public bool DrawImageEnabled { get; set; }
		DevExpress.Utils.Text.StringInfo stringInfoCore;
		public DevExpress.Utils.Text.StringInfo StringInfo {
			get { return stringInfoCore; }
			set { stringInfoCore = value; }
		}
		bool allowHtmlStringCore = false;
		public bool AllowHtmlString { get { return allowHtmlStringCore; } set { allowHtmlStringCore = value; } }
		public bool AllowGlyphSkinning { get; set; }
		BorderStyles borderStyle;
		public BorderStyles BorderStyle { get { return borderStyle; } set { borderStyle = value; } }
		Color lineColor;
		public Color LineColor { get { return lineColor; } set { lineColor = value; } }
		private bool enabled;
		public bool Enabled { get { return enabled; } set { enabled = value; } }
		private LabelControlObjectPainter labelPainter;
		public LabelControlObjectPainter LabelPainter { get { return labelPainter; } set { labelPainter = value; } }
		private Point[] line1;
		public Point[] Line1 { get { return line1; } set { line1 = value; } }
		private Point[] line2;
		public Point[] Line2 { get { return line2; } set { line2 = value; } }
		private LineLocation lineLocation;
		public LineLocation LineLocation { get { return lineLocation; } set { lineLocation = value; } }
		private LabelControlAppearanceObject appearance;
		public LabelControlAppearanceObject Appearance { get { return appearance; } set { appearance = value; } }
		private TextOptions defaultTextOptions;
		public TextOptions DefaultTextOptions { get { return defaultTextOptions; } set { defaultTextOptions = value; } }
		private Rectangle textRect;
		public Rectangle TextRect { get { return textRect; } set { textRect = value; } }
		private HotkeyPrefix hotkeyPrefixState;
		public HotkeyPrefix HotkeyPrefixState { get { return hotkeyPrefixState; } set { hotkeyPrefixState = value; } }
		private string displayText;
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		private AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance { get { return paintAppearance; } set { paintAppearance = value; } }
		private Rectangle imageBounds;
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		private bool lineVisible;
		public bool LineVisible { get { return lineVisible; } set { lineVisible = value; } }
		private LabelLineOrientation lineOrientation;
		public LabelLineOrientation LineOrientation { get { return lineOrientation; } set { lineOrientation = value; } }
		DashStyle lineStyle;
		public DashStyle LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		bool showLineShadow = true;
		public bool ShowLineShadow { get { return showLineShadow; } set { showLineShadow = value; } }
		public bool GetAllowGlyphSkinning() {
			return Enabled && AllowGlyphSkinning;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {	
	public class LabelPaintersHelper {
		public static LabelControlObjectPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFill) {
			if(lookAndFill == null) {
				lookAndFill = UserLookAndFeel.Default;
			}
			switch(lookAndFill.ActiveStyle) {
				case ActiveLookAndFeelStyle.Office2003:
				case ActiveLookAndFeelStyle.WindowsXP:
					return new LabelControlWindowsXPObjectPainter();
				case ActiveLookAndFeelStyle.Skin:
					return new LabelControlSkinObjectPainter(lookAndFill);
			}
			return new LabelControlObjectPainter();
		}
		public static LabelControlObjectPainter GetPainter(BorderStyles style) {
			return LabelPaintersHelper.GetPainter(style, null);
		}
	}
	public class LabelControlWindowsXPObjectPainter : LabelControlObjectPainter {
		public override Color GetLineColor(ObjectInfoArgs e) {
			LabelControlViewInfo vi = ((LabelInfoArgs)e).ViewInfo;
			if(vi.OwnerControl.LineColor == Color.Empty) return SystemColors.ControlDark;
			return vi.OwnerControl.LineColor;
		}
		public override Color GetLineColor2(ObjectInfoArgs e) {
			return Color.Empty;
		}
	}
	public class LabelControlObjectPainter : StyleObjectPainter {
		public virtual Color GetLineColor(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			if(le.LineColor == Color.Empty)
				return SystemColors.ControlDark;
			return le.LineColor;
		}
		public virtual Color GetLineColor2(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			if(!le.ShowLineShadow)
				return Color.Empty;
			if(le.Enabled == false || le.LineColor == Color.Empty)
				return SystemColors.ControlLightLight;
			return SystemColors.ControlLightLight;
		}
		protected virtual Point NearPoint(LabelLineOrientation lo, Point pt) {
			if(lo == LabelLineOrientation.Vertical) return new Point(pt.X + 1, pt.Y);
			return new Point(pt.X, pt.Y + 1); 
		}
		protected virtual Point NearPoint(LabelControlViewInfo vi, Point pt) {
			return NearPoint(vi.OwnerControl.LineOrientation, pt);
		}
		protected virtual Pen GetLinePen(LabelInfoArgs le) {
			Pen res = new Pen(le.LabelPainter.GetLineColor(le), 0.5f);
			res.DashStyle = le.LineStyle;
			return res;
		}
		public virtual void DrawLabelLines(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			using(Pen pen = GetLinePen(le)) {
				e.Paint.DrawLine(e.Graphics, pen, le.Line1[0], le.Line1[1]);
				if(le.LabelPainter.GetLineColor2(e) != Color.Empty)
					e.Paint.DrawLine(e.Graphics, pen, NearPoint(le.LineOrientation, le.Line1[0]), NearPoint(le.LineOrientation, le.Line1[1]));
				if(le.LineLocation == LineLocation.Default || le.LineLocation == LineLocation.Center) {
					e.Paint.DrawLine(e.Graphics, pen, le.Line2[0], le.Line2[1]);
					if(le.LabelPainter.GetLineColor2(e) != Color.Empty)
						e.Paint.DrawLine(e.Graphics, pen, NearPoint(le.LineOrientation, le.Line2[0]), NearPoint(le.LineOrientation, le.Line2[1]));
				}
			}
		}
		protected virtual void DrawSimpleString(LabelInfoArgs le) {
			StringFormat format = le.Appearance.GetStringFormat(le.DefaultTextOptions);
			HotkeyPrefix prefix = format.HotkeyPrefix;
			PointF tlPoint = new Point(le.TextRect.X, le.TextRect.Y);
			format.HotkeyPrefix = le.HotkeyPrefixState;
			le.PaintAppearance.DrawString(le.Cache, le.DisplayText, le.TextRect, format);
			format.HotkeyPrefix = prefix;
		}
		protected virtual void DrawHtmlString(LabelInfoArgs le) {
			if (le.ViewInfo != null) {
				StringPainter.Default.UpdateLocation(le.ViewInfo.StringInfo, le.TextRect);
				StringPainter.Default.DrawString(le.Cache, le.ViewInfo.StringInfo);
			} else {
				if (le.StringInfo != null) {
					StringPainter.Default.UpdateLocation(le.StringInfo, le.TextRect);
					StringPainter.Default.DrawString(le.Cache, le.StringInfo);
				}
			}
		}
		public virtual void DrawLabelText(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			if (le.ViewInfo != null && le.ViewInfo.AllowHtmlString || le.AllowHtmlString)
				DrawHtmlString(le);
			else
				DrawSimpleString(le);
		}
		public virtual void DrawBackgroundImage(ObjectInfoArgs e) { 
			LabelInfoArgs le = e as LabelInfoArgs;
			Image img = le.Appearance.GetImage(ObjectState.Normal);
			if(img != null) {
				Rectangle srcImgBounds = new Rectangle(Point.Empty, le.Appearance.GetImageSize());
				if(!le.GetAllowGlyphSkinning()) {
					e.Cache.Paint.DrawImage(e.Graphics, img, le.ImageBounds, srcImgBounds, le.DrawImageEnabled);
				}
				else {
					var coloredAttributes = ImageColorizer.GetColoredAttributes(le.Appearance.ForeColor);
					e.Cache.Paint.DrawImage(e.Graphics, img, le.ImageBounds, srcImgBounds, coloredAttributes);
				}
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			e.Graphics.FillRectangle(le.Appearance.GetBackBrush(e.Cache, le.Bounds), le.Bounds);
			if(le != null && le.Appearance.HasImage)
				DrawBackgroundImage(e);
			DrawLabelText(e);
			if(le != null && le.LineVisible)
				DrawLabelLines(e);
		}
		protected internal virtual AppearanceDefault GetDefaultAppearance(Control ownerControl) {
			return new AppearanceDefault(ownerControl.Enabled ? SystemColors.WindowText : SystemColors.GrayText, Color.Transparent);
		}
	}
	public class LabelControlSkinObjectPainter : LabelControlObjectPainter { 
		ISkinProvider skinProvider;
		public LabelControlSkinObjectPainter(ISkinProvider skinProvider) {
			this.skinProvider = skinProvider;
		}
		public ISkinProvider Provider { get { return skinProvider; } }
		public override Color GetLineColor(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			return GetLineColor(e, le.ViewInfo.GetSystemColor(SystemColors.ControlDark));
		}
		public virtual Color GetLineColor(ObjectInfoArgs e, Color controlDark) {
			LabelInfoArgs le = e as LabelInfoArgs;
			if(le.LineColor == Color.Empty) return controlDark;
			return le.LineColor;
		}
		public override Color GetLineColor2(ObjectInfoArgs e) {
			return Color.Empty;
		}
		protected internal override AppearanceDefault GetDefaultAppearance(Control ownerControl) {
			SkinElement element = CommonSkins.GetSkin(Provider)[CommonSkins.SkinLabel];
			if(element == null) 
				return base.GetDefaultAppearance(ownerControl);
			AppearanceDefault res = element.GetAppearanceDefault();
			if(res.ForeColor == Color.Transparent) {
				res.ForeColor = LookAndFeelHelper.GetTransparentForeColor(Provider, ownerControl);
			}
			if(!ownerControl.Enabled) res.ForeColor = LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.GrayText);
			return res;
		}
		public override void DrawLabelLines(ObjectInfoArgs e) {
			LabelInfoArgs le = e as LabelInfoArgs;
			if(le.LineColor != Color.Empty) {
				base.DrawLabelLines(e);
				return;
			}
			bool isVertical = le.LineOrientation == LabelLineOrientation.Vertical;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[isVertical ? CommonSkins.SkinLabelLineVert : CommonSkins.SkinLabelLine], Rectangle.Empty);
			Size size = ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, info).Size;
			DrawLineHelper(e, info, le.Line1[0], le.Line1[1], isVertical ? size.Width : size.Height);
			DrawLineHelper(e, info, le.Line2[0], le.Line2[1], isVertical ? size.Width : size.Height);
		}
		void DrawLineHelper(ObjectInfoArgs e, SkinElementInfo info, Point p1, Point p2, int height) {
			LabelInfoArgs le = e as LabelInfoArgs;
			Rectangle line = Rectangle.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
			if(le.LineOrientation == LabelLineOrientation.Vertical)
				line.Width = height;
			else
				line.Height = height;
			if(line.Width < 1 || line.Height < 1) return;
			info.Bounds = line;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class LabelControlPainter : BaseControlPainter {
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
			LabelControlViewInfo labelInfo = info.ViewInfo as LabelControlViewInfo;
			if(labelInfo.OwnerControl.BorderStyle != BorderStyles.Default)
				base.DrawBorder(info);
		}
		protected override void  DrawContent(ControlGraphicsInfoArgs info){
			LabelControlViewInfo labelViewInfo = info.ViewInfo as LabelControlViewInfo;
			labelViewInfo.LabelInfoArgs.Cache = info.Cache;
			labelViewInfo.LabelInfoArgs.Bounds = info.Bounds;
			labelViewInfo.LabelPainter.DrawObject(labelViewInfo.LabelInfoArgs);
			labelViewInfo.LabelInfoArgs.Cache = null;
		}
	}
}
namespace DevExpress.XtraEditors {			
	public enum LabelAutoSizeMode { Default, None, Horizontal, Vertical }
	public class HyperlinkLabelControlAppearanceOptions : LabelControlAppearanceOptions {
		bool useLinkColor, usePressedColor, useVisitedColor, useDisabledColor;
		protected override void ResetOptions() {
			base.ResetOptions();
			this.useLinkColor = false;
			this.usePressedColor = false;
			this.useVisitedColor = false;
			this.useDisabledColor = false;
		}
		public override bool IsEqual(AppearanceOptions options) {
			bool res = base.IsEqual(options);
			HyperlinkLabelControlAppearanceOptions opt = options as HyperlinkLabelControlAppearanceOptions;
			if(!res || opt == null)
				return false;
			return this.useLinkColor == opt.useLinkColor && this.usePressedColor == opt.usePressedColor && this.useDisabledColor == opt.useDisabledColor && this.useVisitedColor == opt.useVisitedColor;
		}
		protected override bool GetOptionValue(string name) {
			if(IsEqual(name, HyperlinkLabelControlAppearanceObject.optLinkColor))
				return UseLinkColor;
			if(IsEqual(name, HyperlinkLabelControlAppearanceObject.optPressedColor))
				return UsePressedColor;
			if(IsEqual(name, HyperlinkLabelControlAppearanceObject.optVisitedColor))
				return UseVisitedColor;
			if(IsEqual(name, HyperlinkLabelControlAppearanceObject.optDisabledColor))
				return UseDisabledColor;
			return base.GetOptionValue(name);
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseLinkColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseLinkColor {
			get { return useLinkColor; }
			set {
				if(UseLinkColor == value) return;
				bool prevValue = UseLinkColor;
				useLinkColor = value;
				OnChanged(HyperlinkLabelControlAppearanceObject.optLinkColor, prevValue, UseLinkColor);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UsePressedColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UsePressedColor {
			get { return usePressedColor; }
			set {
				if(UsePressedColor == value) return;
				bool prevValue = UsePressedColor;
				usePressedColor = value;
				OnChanged(HyperlinkLabelControlAppearanceObject.optPressedColor, prevValue, UsePressedColor);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseVisitedColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseVisitedColor {
			get { return useVisitedColor; }
			set {
				if(UseVisitedColor == value) return;
				bool prevValue = UseVisitedColor;
				useVisitedColor = value;
				OnChanged(HyperlinkLabelControlAppearanceObject.optVisitedColor, prevValue, UseVisitedColor);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseDisabledColor"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseDisabledColor {
			get { return useDisabledColor; }
			set {
				if(UseDisabledColor == value) return;
				bool prevValue = UseDisabledColor;
				useDisabledColor = value;
				OnChanged(HyperlinkLabelControlAppearanceObject.optDisabledColor, prevValue, UseDisabledColor);
			}
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			HyperlinkLabelControlAppearanceOptions lo = options as HyperlinkLabelControlAppearanceOptions;
			if(lo != null) {
				this.useLinkColor = lo.UseLinkColor;
				this.usePressedColor = lo.UsePressedColor;
				this.useVisitedColor = lo.UseVisitedColor;
				this.useDisabledColor = lo.UseDisabledColor;
			}
		}
	}
	public class LabelControlAppearanceOptions : AppearanceOptions {
		bool useHoverImage, usePressedImage, useDisabledImage;
		bool useImageList, useImageIndex, useImageAlign, useHoverImageIndex, usePressedImageIndex, useDisabledImageIndex;
		public LabelControlAppearanceOptions() : base() {
		}
		protected override void ResetOptions() {
			base.ResetOptions();
			this.useHoverImage = false;
			this.usePressedImage = false;
			this.useDisabledImage = false;
			this.useImageList = false;
			this.useImageIndex = false;
			this.useImageAlign = false;
			this.useHoverImageIndex = false;
			this.usePressedImageIndex = false;
			this.useDisabledImageIndex = false;
		}
		public override bool IsEqual(AppearanceOptions options) {
			bool res = base.IsEqual(options);
			LabelControlAppearanceOptions lo = options as LabelControlAppearanceOptions;
			if(!res || lo == null)
				return res;
			return this.useHoverImage == lo.useHoverImage && this.usePressedImage == lo.usePressedImage && this.useDisabledImage == lo.useDisabledImage;
		}
		protected override bool GetOptionValue(string name) {
			if(IsEqual(name, LabelControlAppearanceObject.optHoverImage))
				return UseHoverImage;
			if(IsEqual(name, LabelControlAppearanceObject.optPressedImage))
				return UsePressedImage;
			if(IsEqual(name, LabelControlAppearanceObject.optDisabledImage))
				return UseDisabledImage;
			return base.GetOptionValue(name);
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			LabelControlAppearanceOptions lo = options as LabelControlAppearanceOptions;
			if(lo != null) {
				this.useHoverImage = lo.UseHoverImage;
				this.usePressedImage = lo.UsePressedImage;
				this.useDisabledImage = lo.UseDisabledImage;
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseHoverImage"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseHoverImage {
			get { return useHoverImage; }
			set {
				if(UseHoverImage == value) return;
				bool prevValue = UseHoverImage;
				useHoverImage = value;
				OnChanged(LabelControlAppearanceObject.optHoverImage, prevValue, UseHoverImage);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UsePressedImage"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UsePressedImage {
			get { return usePressedImage; }
			set {
				if(UsePressedImage == value) return;
				bool prevValue = UsePressedImage;
				usePressedImage = value;
				OnChanged(LabelControlAppearanceObject.optPressedImage, prevValue, UsePressedImage);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseDisabledImage"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseDisabledImage {
			get { return useDisabledImage; }
			set {
				if(UseDisabledImage == value) return;
				bool prevValue = UseDisabledImage;
				useDisabledImage = value;
				OnChanged(LabelControlAppearanceObject.optDisabledImage, prevValue, UseDisabledImage);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseImageIndex"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseImageIndex {
			get { return useImageIndex; }
			set {
				if(UseImageIndex == value) return;
				bool prevValue = UseImageIndex;
				useImageIndex = value;
				OnChanged(LabelControlAppearanceObject.optImageIndex, prevValue, UseImageIndex);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseHoveredImageIndex"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseHoveredImageIndex {
			get { return useHoverImageIndex; }
			set {
				if(UseHoveredImageIndex == value) return;
				bool prevValue = UseHoveredImageIndex;
				useHoverImageIndex = value;
				OnChanged(LabelControlAppearanceObject.optHoverImageIndex, prevValue, UseHoveredImageIndex);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UsePressedImageIndex"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UsePressedImageIndex {
			get { return usePressedImageIndex; }
			set {
				if(UsePressedImageIndex == value) return;
				bool prevValue = UsePressedImageIndex;
				usePressedImageIndex = value;
				OnChanged(LabelControlAppearanceObject.optPressedImageIndex, prevValue, UsePressedImageIndex);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseDisabledImageIndex"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseDisabledImageIndex {
			get { return useDisabledImageIndex; }
			set {
				if(UseDisabledImageIndex == value) return;
				bool prevValue = UseDisabledImageIndex;
				useDisabledImageIndex = value;
				OnChanged(LabelControlAppearanceObject.optDisabledImageIndex, prevValue, UseDisabledImageIndex);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseImageList"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseImageList {
			get { return useImageList; }
			set {
				if(UseImageList == value) return;
				bool prevValue = UseImageList;
				useImageList = value;
				OnChanged(LabelControlAppearanceObject.optImageList, prevValue, UseImageList);
			}
		}
		[
		 DefaultValue(false), XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceOptions.UseImageAlign"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseImageAlign {
			get { return useImageAlign; }
			set {
				if(UseImageAlign == value) return;
				bool prevValue = UseImageAlign;
				useImageAlign = value;
				OnChanged(LabelControlAppearanceObject.optImageAlign, prevValue, UseImageAlign);
			}
		}
	}
	public class HyperlinkLabelControlAppearanceObject : LabelControlAppearanceObject {
		internal static readonly string optLinkColor = "UseLinkColor";
		internal static readonly string optPressedColor = "UsePressedColor";
		internal static readonly string optVisitedColor = "UseVisitedColor";
		internal static readonly string optDisabledColor = "UseDisabledColor";
		protected override AppearanceOptions CreateOptions() {
			return new HyperlinkLabelControlAppearanceOptions();
		}
		public new HyperlinkLabelControlAppearanceOptions Options { get { return (HyperlinkLabelControlAppearanceOptions)base.Options; } }
		Color linkColor;
		void ResetLinkColor() { LinkColor = Color.Empty; }
		protected bool ShouldSerializeLinkColor() { return LinkColor != Color.Empty; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.LinkLabelControlAppearanceObject.LinkColor"),
		XtraSerializableProperty(), Localizable(true)
		]
		public Color LinkColor {
			get { return linkColor; }
			set {
				if(LinkColor == value) return;
				linkColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseLinkColor = linkColor != Color.Empty; }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		Color visitedColor;
		void ResetVisitedColor() { VisitedColor = Color.Empty; }
		protected bool ShouldSerializeVisitedColor() { return VisitedColor != Color.Empty; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.LinkLabelControlAppearanceObject.VisitedColor"),
		XtraSerializableProperty(), Localizable(true)
		]
		public Color VisitedColor {
			get { return visitedColor; }
			set {
				if(VisitedColor == value) return;
				visitedColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseVisitedColor = visitedColor != Color.Empty; }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		Color pressedColor;
		void ResetPressedColor() { PressedColor = Color.Empty; }
		protected bool ShouldSerializePressedColor() { return PressedColor != Color.Empty; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.LinkLabelControlAppearanceObject.PressedColor"),
		XtraSerializableProperty(), Localizable(true)
		]
		public Color PressedColor {
			get { return pressedColor; }
			set {
				if(PressedColor == value) return;
				pressedColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UsePressedColor = pressedColor != Color.Empty; }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		Color disabledColor;
		void ResetDisabledColor() { DisabledColor = Color.Empty; }
		protected bool ShouldSerializeDisabledColor() { return DisabledColor != Color.Empty; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.LinkLabelControlAppearanceObject.DisabledColor"),
		XtraSerializableProperty(), Localizable(true)
		]
		public Color DisabledColor {
			get { return disabledColor; }
			set {
				if(DisabledColor == value) return;
				disabledColor = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseDisabledColor = disabledColor != Color.Empty; }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
	}
	public class LabelControlAppearanceObject : AppearanceObject {
		object imageCollection;
		ContentAlignment imageAlign;
		int imageIndex = -1;
		int hoverImageIndex = -1;
		int pressedImageIndex = -1;
		int disabledImageIndex = -1;
		Image hoverImage;
		Image pressedImage;
		Image disabledImage;
		internal static string
			optHoverImage = "UseHoverImage",
			optPressedImage = "UsePressedImage",
			optDisabledImage = "UseDisabledImage",
			optImageIndex = "ImageIndex",
			optHoverImageIndex = "HoveredImageIndex",
			optPressedImageIndex = "PressedImageIndex",
			optImageList = "ImageList",
			optDisabledImageIndex = "DisabledImageIndex",
			optImageAlign = "ImageAlign";
		public override void Dispose() {
			base.Dispose();
		}
		protected override AppearanceOptions CreateOptions() {
			return new LabelControlAppearanceOptions();
		}
		public override void Reset() {
			base.Reset();
			this.imageCollection = null;
			this.imageAlign = ContentAlignment.MiddleCenter;
			this.imageIndex = -1;
			this.hoverImage = null;
			this.hoverImageIndex = -1;
			this.pressedImage = null;
			this.pressedImageIndex = -1;
			this.disabledImage = null;
			this.disabledImageIndex = -1;
		}
		public void Combine(AppearanceObject[] appearances, AppearanceDefault defaultAppearance) {
			Reset();
			AppearanceHelper.Combine(this, appearances, defaultAppearance);
			for(int i = appearances.Length - 1; i >= 0; i--) {
				LabelControlAppearanceObject lapp = appearances[i] as LabelControlAppearanceObject;
				if(lapp != null) {
					if(lapp.Options.UseImageList) ImageList = lapp.ImageList;
					if(lapp.Options.UseImageAlign) ImageAlign = lapp.ImageAlign;
					if(lapp.Options.UseImage) Image = lapp.Image;
					if(lapp.Options.UseImageIndex) ImageIndex = lapp.ImageIndex;
					if(lapp.Options.UseHoverImage) this.HoverImageCore = lapp.HoverImageCore;
					if(lapp.Options.UseHoveredImageIndex) HoverImageIndexCore = lapp.HoverImageIndexCore;
					if(lapp.Options.UsePressedImage) PressedImageCore = lapp.PressedImageCore;
					if(lapp.Options.UsePressedImageIndex) PressedImageIndexCore = lapp.PressedImageIndexCore;
					if(lapp.Options.UseDisabledImage) DisabledImageCore = lapp.DisabledImageCore;
					if(lapp.Options.UseDisabledImageIndex) DisabledImageIndexCore = lapp.DisabledImageIndexCore;
				}
			}
		}
		public new LabelControlAppearanceOptions Options {
			get { return (LabelControlAppearanceOptions)base.Options; }
		}
		protected internal Size GetImageSize() {
			if(!HasImage) return Size.Empty;
			if(Image != null) return Image.Size;
			if(ImageIndex == -1)
				return Size.Empty;
			return ImageCollection.GetImageListSize(ImageList);
		}
		public override Image GetImage() {
			if(!Options.UseImage) return null;
			Image img = base.GetImage();
			if(img != null) return img;
			return ImageCollection.GetImageListImage(ImageList, ImageIndex);
		}
		protected Image GetImage(bool useImage, Image image, int imageIndex) {
			if(image != null)
				return useImage ? image : null;
			return ImageCollection.GetImageListImage(ImageList, imageIndex);
		}
		public Image GetImage(ObjectState state) {
			Image img = null;
			switch(state) {
				case ObjectState.Hot: 
					img = GetImage(Options.UseHoverImage, this.hoverImage, HoverImageIndexCore);
					break;
				case ObjectState.Pressed: 
					img = GetImage(Options.UsePressedImage, this.pressedImage, PressedImageIndexCore);
					break;
				case ObjectState.Disabled: 
					img = GetImage(Options.UseDisabledImage, this.disabledImage, DisabledImageIndexCore);
					break;
			}
			return img ?? GetImage(Options.UseImage, Image, ImageIndex);
		}
		protected internal bool HasImage {
			get { return Image != null || ImageCollection.IsImageListImageExists(ImageList, ImageIndex); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearanceObjectImageList"),
#endif
 DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList {
			get { return imageCollection; }
			set {
				if(imageCollection == value) return;
				imageCollection = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseImageList = ImageList != null; }
					finally { Options.CancelUpdate(); }
				}
				OnChanged();
			}
		}
		internal Image HoverImageCore { get { return this.hoverImage; } set { this.hoverImage = value; } }
		internal Image PressedImageCore { get { return this.pressedImage; } set { this.pressedImage = value; } }
		internal Image DisabledImageCore { get { return this.disabledImage; } set { this.disabledImage = value; } }
		[
		 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceObject.HoverImage"), Localizable(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Please use AppearanceHovered instead.")]
		public virtual Image HoverImage {
			get { return hoverImage; }
			set {
				if(this.hoverImage == value) return;
				hoverImage = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseHoverImage = hoverImage != null; }
					finally { Options.CancelUpdate(); }
				}
				OnHoverImageChanged();
			}
		}
		protected virtual void OnHoverImageChanged() {
			OnPaintChanged();
		}
		[
		 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceObject.PressedImage"), Localizable(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Please use AppearancePressed instead.")]
		public virtual Image PressedImage {
			get { return pressedImage; }
			set {
				if(this.pressedImage == value) return;
				pressedImage = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UsePressedImage = pressedImage != null; }
					finally { Options.CancelUpdate(); }
				}
				OnPressedImageChanged();
			}
		}
		protected virtual void OnPressedImageChanged() {
			OnPaintChanged();
		}
		[
		 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LabelControlAppearanceObject.DisabledImage"), Localizable(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Please use AppearanceDisabled instead.")]
		public virtual Image DisabledImage {
			get { return disabledImage; }
			set {
				if(this.disabledImage == value) return;
				disabledImage = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseDisabledImage = disabledImage != null; }
					finally { Options.CancelUpdate(); }
				}
				OnDisabledImageChanged();
			}
		}
		protected virtual void OnDisabledImageChanged() {
			OnPaintChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearanceObjectImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), 
		ImageList("ImageList")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(imageIndex == value) return;
				if(value < -1)
					value = -1;
				imageIndex = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseImageIndex = ImageIndex > -1; }
					finally { Options.CancelUpdate(); }
				}
				OnImageIndexChanged();
			}
		}
		internal int HoverImageIndexCore { get { return this.hoverImageIndex; } set { this.hoverImageIndex = value; } }
		internal int PressedImageIndexCore { get { return this.pressedImageIndex; } set { this.pressedImageIndex = value; } }
		internal int DisabledImageIndexCore { get { return this.disabledImageIndex; } set { this.disabledImageIndex = value; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearanceObjectHoverImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Use AppearanceHovered instead."),
		ImageList("ImageList")]
		public int HoverImageIndex {
			get { return hoverImageIndex; }
			set {
				if(hoverImageIndex == value) return;
				if(value < -1)
					value = -1;
				hoverImageIndex = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseHoveredImageIndex = this.hoverImageIndex > -1; }
					finally { Options.CancelUpdate(); }
				}
				OnHoverImageIndexChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearanceObjectPressedImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Use AppearancePressed instead."),
		ImageList("ImageList")]
		public int PressedImageIndex {
			get { return pressedImageIndex; }
			set {
				if(pressedImageIndex == value) return;
				if(value < -1)
					value = -1;
				pressedImageIndex = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UsePressedImageIndex = this.pressedImageIndex > -1; }
					finally { Options.CancelUpdate(); }
				}
				OnPressedImageIndexChanged();
			}
		}
		[ DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Obsolete("Use AppearanceDisabled instead."),
		ImageList("ImageList")]
		public int DisabledImageIndex {
			get { return disabledImageIndex; }
			set {
				if(disabledImageIndex == value) return;
				if(value < -1)
					value = -1;
				disabledImageIndex = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseDisabledImageIndex = this.disabledImageIndex > -1; }
					finally { Options.CancelUpdate(); }
				}
				OnDisabledImageIndexChanged();
			}
		}
		protected LabelControl OwnerLabel { get { return Owner as LabelControl; } }
		protected virtual void OnImageIndexChanged() {
			OnChanged();
		}
		protected virtual void OnHoverImageIndexChanged() {
			OnChanged();
		}
		protected virtual void OnPressedImageIndexChanged() {
			OnChanged();
		}
		protected virtual void OnDisabledImageIndexChanged() {
			OnChanged();
		}
		protected internal HorzAlignment FinalAlign(HorzAlignment align, RightToLeft rightToLeft) {
			if(rightToLeft != RightToLeft.Yes || align == HorzAlignment.Center )
				return align;
			if(align == HorzAlignment.Near||
				align == HorzAlignment.Default)
				return HorzAlignment.Far;
			return HorzAlignment.Near;
		}
		public ContentAlignment FinalAlign(ContentAlignment align, RightToLeft rightToLeft) {
			if(rightToLeft != RightToLeft.Yes)
				return align;
			switch(align) {
				case ContentAlignment.TopLeft:
					return ContentAlignment.TopRight;
				case ContentAlignment.MiddleLeft:
					return ContentAlignment.MiddleRight;
				case ContentAlignment.BottomLeft:
					return ContentAlignment.BottomRight;
				case ContentAlignment.TopRight:
					return ContentAlignment.TopLeft;
				case ContentAlignment.MiddleRight:
					return ContentAlignment.MiddleLeft;
				case ContentAlignment.BottomRight:
					return ContentAlignment.BottomLeft;
			}
			return align;
		}
		public override void Assign(AppearanceObject val) {
			base.Assign(val);
			LabelControlAppearanceObject labelAppearance = val as LabelControlAppearanceObject;
			if(labelAppearance != null) {
				BeginUpdate();
				try {
					this.ImageAlign = labelAppearance.ImageAlign;
					this.ImageIndex = labelAppearance.ImageIndex;
					this.ImageList = labelAppearance.ImageList;
					this.HoverImageCore = labelAppearance.HoverImageCore;
					this.HoverImageIndexCore = labelAppearance.HoverImageIndexCore;
					this.PressedImageCore = labelAppearance.PressedImageCore;
					this.PressedImageIndexCore = labelAppearance.PressedImageIndexCore;
					this.DisabledImageCore = labelAppearance.DisabledImageCore;
					this.DisabledImageIndexCore = labelAppearance.DisabledImageIndexCore;
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected void OnImageCollectionChange(object sender, EventArgs e) {
			OnChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearanceObjectImageAlign"),
#endif
 DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment ImageAlign {
			get {
				if(Enum.GetName(typeof(ContentAlignment),imageAlign) == null)
					return ContentAlignment.MiddleCenter;
				return imageAlign; 
			}
			set {
				if(imageAlign == value) return ;
				imageAlign = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseImageAlign = ImageAlign != ContentAlignment.MiddleCenter; }
					finally { Options.CancelUpdate(); }
				}
				OnChanged();
			}
		}
	}
}
namespace DevExpress.Accessibility {
	public class LabelControlAccessible : BaseControlAccessible {
		LabelControl ownerLabel;
		public LabelControlAccessible(LabelControl owner)
			: base(owner) {
			this.ownerLabel = owner;
		}
		public LabelControl OwnerLabel { get { return ownerLabel; } }
		protected override AccessibleRole GetRole() {
			AccessibleRole role = OwnerLabel.AccessibleRole;
			if(role != AccessibleRole.Default)
				return role;
			return AccessibleRole.StaticText;
		}
	}
}
namespace DevExpress.XtraEditors {
	class LabelControlUnsafeNativeMethods {
		public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong) {
			if(IntPtr.Size == 4) {
				return EditorsNativeMethods.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return EditorsNativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}
		public static void UpdateParentUICuesState(LabelControl label) {
			int wParam = 0x00030001;
			if(label.Parent == null || !label.Parent.IsHandleCreated) return;
			EditorsNativeMethods.SendMessage(new HandleRef(label.Parent, label.Parent.Handle), 0x127, (IntPtr)(wParam), IntPtr.Zero);
		}
	}
	public enum ImageAlignToText { None, LeftTop, LeftCenter, LeftBottom, RightTop, RightCenter, RightBottom, TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight }
	public enum LineLocation { Default, Top, Center, Bottom, Left, Right }
	public enum LabelLineOrientation { Default, Horizontal, Vertical }
	[DXToolboxItem(DXToolboxItemKind.Free),
	 DefaultEvent("Click"),
	 DefaultProperty("Text"),
	 Designer("DevExpress.XtraEditors.Design.LabelControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays descriptive text and optionally an image next to the text."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagFilter(typeof(LabelControlFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "LabelControl")
	]
	public class LabelControl : BaseStyleControl, IMouseWheelSupportIgnore {
		static readonly object sizeableChanged = new object();
		static readonly object hyperlinkClick = new object();
		bool useMnemonic;
		bool autoEllipsis;
		bool showLine;
		Color lineColor;
		ImageAlignToText imageAlignToText;
		LabelAutoSizeMode autoSizeMode;
		LabelLineOrientation lineOrientation;
		LineLocation lineLocation;
		bool allowHtmlString;
		DefaultBoolean allowGlyphSkinning;
		DashStyle lineStyle;
		ImageCollection htmlImages;
		LabelControlAppearanceObject appearanceHovered, appearancePressed, appearanceDisabled;
		public LabelControl() {
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserMouse | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
			lineOrientation = LabelLineOrientation.Default;
			lineLocation = LineLocation.Default;
			useMnemonic = true; 
			TabStop = false;
			imageAlignToText = ImageAlignToText.None;
			autoSizeMode = LabelAutoSizeMode.Default;
			AutoSizeMode = AutoSizeMode;
			this.allowHtmlString = DefaultAllowHtmlString;
			this.allowGlyphSkinning = DefaultBoolean.Default;
			this.lineStyle = DashStyle.Solid;
			MethodInfo mi = CommonProperties.GetMethod("SetSelfAutoSizeInDefaultLayout", BindingFlags.NonPublic | BindingFlags.Static);
			mi.Invoke(null, new object[] { this, true });
			this.appearanceHovered = (LabelControlAppearanceObject)CreateAppearance();
			this.appearancePressed = (LabelControlAppearanceObject)CreateAppearance();
			this.appearanceDisabled = (LabelControlAppearanceObject)CreateAppearance();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				DestroyAppearance(this.appearanceHovered);
				DestroyAppearance(this.appearancePressed);
				DestroyAppearance(this.appearanceDisabled);
			}
		}
		bool ShouldSerializeAppearanceHovered() { return AppearanceHovered.ShouldSerialize(); }
		void ResetAppearanceHovered() { AppearanceHovered.Reset(); }
		[ DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LabelControlAppearanceObject AppearanceHovered { get { return appearanceHovered; } }
		bool ShouldSerializeAppearancePressed() { return AppearancePressed.ShouldSerialize(); }
		void ResetAppearancePressed() { AppearancePressed.Reset(); }
		[ DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LabelControlAppearanceObject AppearancePressed { get { return appearancePressed; } }
		bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled.ShouldSerialize(); }
		void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		[ DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual LabelControlAppearanceObject AppearanceDisabled { get { return appearanceDisabled; } }
		protected virtual bool DefaultAllowHtmlString { get { return false; } }
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			if(info != null && (info.SuperTip != null || info.Text != String.Empty || info.Title != String.Empty)) return info;
			if(!ViewInfo.IsCropped) return info;
			ToolTipControlInfo res = new ToolTipControlInfo();
			res.Object = this;
			res.SuperTip = new SuperToolTip();
			res.SuperTip.Items.Add(new ToolTipItem() { Text = this.Text });
			return res;
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new LabelControlViewInfo(this); }
		protected override BaseControlPainter CreatePainter() { return new LabelControlPainter(); }
		protected override AppearanceObject CreateAppearance() {
			LabelControlAppearanceObject res = new LabelControlAppearanceObject();
			res.Changed += new EventHandler(OnStyleChanged);
			res.SizeChanged += new EventHandler(OnStyleChanged);
			res.PaintChanged += new EventHandler(OnStyleChanged);
			return res;
		}
		protected override void OnStyleChanged(object sender, EventArgs e) {
			ViewInfo.SetPaintAppearanceDirty();
			base.OnStyleChanged(sender, e);
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(ViewInfo);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = ViewInfo;
			if(DesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(null, ViewInfo, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = ViewInfo;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(Appearance.GetImage() == null || info == null) return;
			if(!info.IsFinalFrame) {
				Appearance.GetImage().SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				Invalidate(animItem.AnimationBounds);
			}
			else {
				StopAnimation();
				StartAnimation();
			}
		}
		protected internal virtual void LayoutChanged(bool isVisualUpdate) {
			if(IsDisposing)
				return;
			if(!IsHandleCreated) return;
			ViewInfo.CalcViewInfo(null, Control.MouseButtons, PointToClient(Control.MousePosition), ClientRectangle);
			if(!isVisualUpdate) AdjustSize();
			Invalidate();
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlHtmlImages")
#else
	Description("")
#endif
]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -= new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
				LayoutChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(AllowHtmlString) {
				LayoutChanged();
			}
		}
		protected void RaiseHyperlinkClick(HyperlinkClickEventArgs e) {
			HyperlinkClickEventHandler handler = Events[hyperlinkClick] as HyperlinkClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { Events.AddHandler(hyperlinkClick, value); }
			remove { Events.RemoveHandler(hyperlinkClick, value); }
		}
		bool showLineShadow = true;
		[DefaultValue(true),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlShowLineShadow"),
#endif
 DXCategory(CategoryName.Appearance)]
		public bool ShowLineShadow {
			get { return showLineShadow; }
			set {
				if(ShowLineShadow == value)
					return;
				this.showLineShadow = value;
				if(LineVisible)
					LayoutChanged();
			}
		}
		[DefaultValue(DashStyle.Solid),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlLineStyle"),
#endif
 DXCategory(CategoryName.Appearance), SmartTagProperty("Line Style", "")]
		public DashStyle LineStyle { 
			get { return lineStyle; } 
			set {
				if(LineStyle == value)
					return;
				lineStyle = value;
				if(LineVisible)
					LayoutChanged();
			} 
		}
		protected internal override void LayoutChanged() { LayoutChanged(false); }
		[ DefaultValue(LineLocation.Default), DXCategory(CategoryName.Appearance), SmartTagProperty("Line Location", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public LineLocation LineLocation {
			get { return lineLocation; }
			set {
				if(LineLocation == value) return;
				lineLocation = value;
				LayoutChanged();
			}
		}
		int indentBetweenImageAndText = -1;
		[DefaultValue(-1)]
		public int IndentBetweenImageAndText {
			get { return indentBetweenImageAndText; }
			set {
				if(IndentBetweenImageAndText == value)
					return;
				indentBetweenImageAndText = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlPlainText"),
#endif
 DXCategory(CategoryName.Appearance)]
		[Browsable(false)]
		public string PlainText {
			get;
			private set;
		}
		private void UpdatePlainText() {
			if(!AllowHtmlString)
				PlainText = Text;
			else {
				if((ViewInfo.StringInfo == null))
					ViewInfo.CalcViewInfo(null);
				DevExpress.Utils.Text.StringInfo strInfo = CalcStringInfoCore();
				PlainText = GetPlaintTextFromHTML(strInfo);
			}
		}
		protected virtual Utils.Text.StringInfo CalcStringInfoCore() {
			DevExpress.Utils.Text.StringInfo strInfo = ViewInfo.CalcStringInfo(Text, false, LabelAutoSizeMode.Default, 10000);
			return strInfo;
		}
		protected virtual string GetPlaintTextFromHTML (DevExpress.Utils.Text.StringInfo info) {
			if(info.Blocks == null)
				return string.Empty;
			StringBuilder sBuilder = new StringBuilder();
			foreach (var block in info.Blocks)
			{
				sBuilder.Append(block.Text);
			}
			return sBuilder.ToString();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAllowHtmlString"),
#endif
 DefaultValue(false), DXCategory(CategoryName.Appearance)]
		public virtual bool AllowHtmlString {
			get { return allowHtmlString; }
			set {
				if(AllowHtmlString == value) return;
				allowHtmlString = value;
				LayoutChanged();
				UpdatePlainText();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), DXCategory(CategoryName.Appearance)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlLineOrientation"),
#endif
 DefaultValue(LabelLineOrientation.Default), DXCategory(CategoryName.Appearance), SmartTagProperty("Line Orientation", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public LabelLineOrientation LineOrientation {
			get { return lineOrientation; }
			set {
				if(LineOrientation == value) return;
				lineOrientation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlImageAlignToText"),
#endif
 DefaultValue(ImageAlignToText.None), DXCategory(CategoryName.Appearance)]
		public ImageAlignToText ImageAlignToText {
			get { return imageAlignToText; }
			set {
				if(ImageAlignToText == value) return;
				imageAlignToText = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new LabelControlAppearanceObject Appearance { get { return base.Appearance as LabelControlAppearanceObject; } }
		[Browsable(false)
		,EditorBrowsable(EditorBrowsableState.Never)
		,DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
#if DXWhidbey        
		[Browsable(false)
		,EditorBrowsable(EditorBrowsableState.Never)
		,DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
#endif
		internal int WindowStyle {
			get {
				return (int)((long)EditorsNativeMethods.GetWindowLong(new HandleRef(this, this.Handle), -16));
			}
			set {
				LabelControlUnsafeNativeMethods.SetWindowLong(new HandleRef(this, this.Handle), -16, new HandleRef(null, (IntPtr)value));
			}
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
		}
		protected override void OnParentForeColorChanged(EventArgs e) {
			base.OnParentForeColorChanged(e);
			LayoutChanged();
		}
		protected override void OnChangeUICues(UICuesEventArgs e) {
			LayoutChanged(true);
			base.OnChangeUICues(e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlUseMnemonic"),
#endif
 DefaultValue(true)
		, DXCategory(CategoryName.Appearance), SmartTagProperty("Use Mnemonic", "", 1, SmartTagActionType.None)
		]
		public bool UseMnemonic {
			get { return useMnemonic; }
			set {
				if(useMnemonic == value) return;
				useMnemonic = value;
				if(base.IsHandleCreated) {
					int num1 = WindowStyle;
					if(UseMnemonic) {
						num1 |= 0x80;
					}
					else {
						num1 &= -129;
					}
					WindowStyle = num1;
				}
				LayoutChanged();
			}
		}
		[Browsable(false)
		, EditorBrowsable(EditorBrowsableState.Never)
		, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override RightToLeft RightToLeft {
			get {
				return base.RightToLeft;
			}
			set {
				base.RightToLeft = value;
			}
		}
		protected internal HotkeyPrefix HotkeyPrefixState {
			get {
				if(!UseMnemonic)
					return HotkeyPrefix.None;
				else if(ShowKeyboardCues || DesignMode)
					return HotkeyPrefix.Show;
				else
					return HotkeyPrefix.Hide;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlLineVisible"),
#endif
 DefaultValue(false), DXCategory(CategoryName.Appearance), SmartTagProperty("Line Visible", "", 0, SmartTagActionType.RefreshAfterExecute)]
		public bool LineVisible {
			get { return showLine; }
			set {
				if(showLine == value) return;
				showLine = value;
				LayoutChanged();
				AdjustSize();
			}
		}
		void ResetLineColor() { LineColor = Color.Empty; }
		bool ShouldSerializeLineColor() { return LineColor != Color.Empty; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlLineColor"),
#endif
 DXCategory(CategoryName.Appearance),Localizable(true)]
		public Color LineColor {
			get { return lineColor; }
			set {
				if(lineColor == value) return ;
				lineColor = value;
				LayoutChanged();
			}
		}
		[DefaultValue(false), Browsable(false)
		,EditorBrowsable(EditorBrowsableState.Never)
		]
		public new bool TabStop {
			get { return base.TabStop; }
			set {
				if(base.TabStop == value) return;
				base.TabStop = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAutoEllipsis"),
#endif
 DefaultValue(false), EditorBrowsable(EditorBrowsableState.Always), DXCategory(CategoryName.Behavior)]
		public bool AutoEllipsis {
			get { return autoEllipsis; }
			set {
				if(autoEllipsis == value) return;
				autoEllipsis = value;
				if(autoEllipsis && (Appearance.TextOptions.Trimming == Trimming.None || (AllowHtmlString && Appearance.TextOptions.Trimming == Trimming.Default)))	   
					Appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
				else if(autoEllipsis == false)
					Appearance.TextOptions.Trimming = Trimming.Default;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlAutoSizeMode"),
#endif
 DefaultValue(LabelAutoSizeMode.Default), System.ComponentModel.Category("Layout"), Localizable(true), SmartTagProperty("Auto Size Mode", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public LabelAutoSizeMode AutoSizeMode {
			get { return autoSizeMode; }
			set {
				autoSizeMode = value;
#if DXWhidbey
				base.AutoSize = autoSizeMode == LabelAutoSizeMode.Horizontal || autoSizeMode == LabelAutoSizeMode.Default;
#endif                
				LayoutChanged();
				RaiseSizeableChanged();
			}
		}
#if DXWhidbey        
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LabelControlText"),
#endif
 Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), SettingsBindable(true), SmartTagProperty("Text", "", SmartTagActionType.RefreshBoundsAfterExecute)
]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
#endif
		protected virtual bool IsParentFormMinimized { 
			get { 
				Form parentForm = FindForm();
				return parentForm != null && parentForm.WindowState == FormWindowState.Minimized;
			} 
		}
		void AdjustSize() {
			AdjustSize(false);
		}
		void AdjustSize(bool skipCheckHandleCreated) {
			if((!IsHandleCreated && !skipCheckHandleCreated) || IsParentFormMinimized) return;
			Size finalSize = new Size( Size.Width, Size.Height );
			if(RealAutoSizeMode != LabelAutoSizeMode.None) {
				finalSize.Height = ViewInfo.ContentSize.Height + ViewInfo.ClientRect.Top * 2;
#if DXWhidbey				
				finalSize.Height += Padding.Top + Padding.Bottom;
#endif
			}
			if(RealAutoSizeMode == LabelAutoSizeMode.Horizontal)	{
				finalSize.Width = ViewInfo.ContentSize.Width + ViewInfo.ClientRect.Left * 2;
#if DXWhidbey								
				finalSize.Width += Padding.Left + Padding.Right;
#endif
			}
			if(IsInLayoutControl) {
				if(finalSize.Height != Size.Height) {
					RaiseSizeableChanged();
				}
			}
			else {
				if(Size != finalSize)
				Size = finalSize;
			}
		}
		bool isInLayoutInitialized = false;
		bool isInLayout = false;
		protected internal virtual bool IsInLayoutControl {
			get {
				if(!isInLayoutInitialized)
					isInLayout = (Parent != null) && Parent.GetType().Name.EndsWith("LayoutControl");
				return isInLayout;
			}
		}
		internal void SetAutoSizeInLayoutControl(bool value) {
			isInLayoutInitialized = value;
			isInLayout = value;
		}
		[Browsable(false)]
		public virtual LabelAutoSizeMode RealAutoSizeMode {
			get { 
#if DXWhidbey
				if(AutoSizeMode == LabelAutoSizeMode.Default) return LabelAutoSizeMode.Horizontal;
				return AutoSizeMode;
#else
				if(AutoSizeMode == LabelAutoSizeMode.Default) return LabelAutoSizeMode.None;
				return AutoSizeMode;
#endif                
			}
		}
		public override Color BackColor {
			get {
				if(ViewInfo.PaintAppearance != null && !ViewInfo.PaintAppearance.BackColor2.IsEmpty)
					return Color.Transparent;
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				ViewInfo.GInfo.AddGraphics(e.Graphics);
				if(ViewInfo.IsPaintAppearanceDirty)
					ViewInfo.UpdatePaintAppearance();
				if(!string.IsNullOrEmpty(Text) && ViewInfo.TextRect.Width == 0)
					ViewInfo.CalcViewInfo(e.Graphics);
				ViewInfo.GInfo.ReleaseGraphics();
				Painter.Draw(new ControlGraphicsInfoArgs(ViewInfo, cache, ViewInfo.Bounds));
				RaisePaintEvent(this, e);
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			LabelControlUnsafeNativeMethods.UpdateParentUICuesState(this);
			LayoutChanged();
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			if(IsHandleCreated)
				LayoutChanged();
			else
				AdjustSizeCore();
			RaiseSizeableChanged();
			UpdatePlainText();
		}
		protected virtual void AdjustSizeCore() {
			if(SelfSizingCore) {
				if(RealAutoSizeMode == LabelAutoSizeMode.Horizontal) {
					Size = PreferredSize;
				}
				else if(RealAutoSizeMode == LabelAutoSizeMode.Vertical) {
					Size = GetPreferredSize(Size);
				}
			}
		}
#if DXWhidbey
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			LayoutChanged();
			RaiseSizeableChanged();
			isInLayoutInitialized = false;
		}
#endif
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			ViewInfo.OnStateChanged(Enabled? ObjectState.Normal: ObjectState.Disabled, true);
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			LayoutChanged();
		}
		protected internal override void OnPropertiesChanged() {
			bool shrsc = shouldRaiseSizeableChanged;
			base.OnPropertiesChanged();
			if(shrsc) RaiseSizeableChanged();
			AdjustSize();
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(!useMnemonic || !IsMnemonic(charCode, Text) || !CanProcessMnemonic())
				return false;
			if(this.Parent != null) {
				if(Parent.SelectNextControl(this, true, false, true, false) && !Parent.ContainsFocus)
					Parent.Focus();
			}
			return true;
		}
#if DEBUGTEST  
		public bool ProcessMnemonicTest(char charCode) {
			return this.ProcessMnemonic(charCode);
		}
#endif
		bool CanProcessMnemonic() {
			return Visible == true && Enabled == true;
		}
		protected virtual Size CalcIXtraResizableControlMinMaxSize(bool calcMin) {
			LabelAutoSizeMode mode = RealAutoSizeMode;
			ViewInfo.UpdatePaintAppearance();
			Size defTextSize = ViewInfo.CalcContentSizeByTextSize(ViewInfo.CalcDefaultTextSize());
			Size size = CalcContentSizeByTextSizeCore(mode, Text);
			size.Height += Padding.Vertical;
			size.Width += Padding.Horizontal;
			if(size.Height == 0) size.Height += 1;
			if(mode == LabelAutoSizeMode.None) return new Size(calcMin ? defTextSize.Width : 0, size.Height);
			if(mode == LabelAutoSizeMode.Horizontal) return size;
			if(mode == LabelAutoSizeMode.Vertical) return new Size(calcMin ? defTextSize.Width : 0, size.Height);
			return Size.Empty;
		}
		protected virtual System.Drawing.Size CalcContentSizeByTextSizeCore(LabelAutoSizeMode mode, string text){
			Size size = ViewInfo.CalcContentSizeByTextSize(ViewInfo.CalcTextSize(text, true, mode));
			return size;
		}
		protected override Size CalcSizeableMaxSize() {
			return CalcIXtraResizableControlMinMaxSize(false);
		}
		protected override Size CalcSizeableMinSize() {
			return CalcIXtraResizableControlMinMaxSize(true);
		}
		static Type commonProperties;
		Type CommonProperties {
			get {
				if(commonProperties == null)
					commonProperties = GetCommonProperties();
				return commonProperties;
			}
		}
		bool SelfSizingCore {
			get {
				System.Reflection.MethodInfo mi = CommonProperties.GetMethod("ShouldSelfSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				bool res = (bool)mi.Invoke(null, new object[] { this });
				return res;
			}
		}
		static Type GetCommonProperties() {
			AssemblyName[] asmList = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			foreach(AssemblyName a in asmList) {
				if(a.Name.Contains("System.Windows.Forms")) {
					Assembly asm = Assembly.Load(a);
					return asm.GetType("System.Windows.Forms.Layout.CommonProperties");
				}
			}
			return null;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(!IsInLayoutControl) {
				Size sz = GetPreferredSize(new Size(width, height));
				if(SelfSizingCore) {
					if(AutoSizeMode == LabelAutoSizeMode.Default ||
						AutoSizeMode == LabelAutoSizeMode.Horizontal) {
						width = sz.Width;
						height = sz.Height;
					}
					else if(AutoSizeMode == LabelAutoSizeMode.Vertical) {
						height = sz.Height;
					}
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		Size ConstrainByMinMax(Size size) {
			if(MaximumSize.Width > 0)
				size.Width = Math.Min(MaximumSize.Width, size.Width);
			if(MaximumSize.Height > 0)
				size.Height = Math.Min(MaximumSize.Height, size.Height);
			size.Width = Math.Max(MinimumSize.Width, size.Width);
			size.Height = Math.Max(MinimumSize.Height, size.Height);
			return size;
		}
		public override Size GetPreferredSize(Size proposedSize) {
			LabelAutoSizeMode mode = RealAutoSizeMode;
			Size pSize = proposedSize;
			int borderSize = 0;
			if(mode == LabelAutoSizeMode.Horizontal) {
				if(pSize.Width == 1)
					pSize.Width = 0;
				if(pSize.Height == 1)
					pSize.Height = 0;
			}
			else if(mode == LabelAutoSizeMode.Vertical) {
				if(pSize.Width == int.MaxValue || pSize.Width == 1)
					pSize.Width = Width;
				pSize.Width -= Padding.Horizontal;
				borderSize = ViewInfo.CalcBorderSize().Width;
				pSize.Width -= borderSize;
			}
			ViewInfo.UpdatePaintAppearance();
			int height = (AutoSizeMode == LabelAutoSizeMode.Vertical) ? int.MaxValue : pSize.Height;
			Size size = ViewInfo.CalcContentSizeByTextSize(ViewInfo.CalcTextSize(ViewInfo.DisplayText, true, mode, pSize.Width, height));
			size.Width += Padding.Horizontal;
			size.Height += Padding.Vertical;
			size = ConstrainByMinMax(size);
			pSize.Width += Padding.Horizontal;
			pSize.Width += borderSize;
			if(mode == LabelAutoSizeMode.None) return new Size(proposedSize.Width, size.Height);
			if(mode == LabelAutoSizeMode.Horizontal) return size;
			if(mode == LabelAutoSizeMode.Vertical) return new Size(pSize.Width, size.Height);
			return base.GetPreferredSize(size);
		}
		protected override BaseAccessible CreateAccessibleInstance() {
			return new LabelControlAccessible(this);
		}
		bool ShouldSerializeAppearance() {
			return this.Appearance.ShouldSerialize();
		}
		protected new internal LabelControlViewInfo ViewInfo { get { return base.ViewInfo as LabelControlViewInfo; } }
		public int GetTextBaselineY() { return ViewInfo.TextBaseline.Y; }
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			LabelControlUnsafeNativeMethods.UpdateParentUICuesState(this);
			if(!IsHandleCreated) {
				if(SelfSizingCore)
					AdjustSize(true);
			}
			else
				LayoutChanged();
		}
		bool IsMouseInClient {
			get {
				Point pt = PointToClient(Control.MousePosition);
				return ClientRectangle.Contains(pt);
			}
		}
		ObjectState GetState(ObjectState state) {
			if(!Enabled)
				return ObjectState.Disabled;
			if(!IsMouseInClient)
				return ObjectState.Normal;
			return state;
		}
		bool IMouseWheelSupportIgnore.Ignore { get { return true; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(IsDisposed)
				return;
			ViewInfo.OnStateChanged(GetState(ObjectState.Pressed), true);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(IsDisposed)
				return;
			ObjectState state = e.Button == MouseButtons.Left ? ObjectState.Pressed : ObjectState.Hot;
			ViewInfo.OnStateChanged(GetState(state), true);
			UpdateHyperlinkCursor(e);
		}
		protected virtual bool HasHyperlink {
			get { return AllowHtmlString && ViewInfo.StringInfo.HasHyperlink; }
		}
		protected Cursor PrevCursor { get; set; }
		StringBlock PrevBlock { get; set; }
		protected virtual void UpdateHyperlinkCursor(MouseEventArgs e) {
			if(e.Button != MouseButtons.None)
				return;
			if(HasHyperlink) {
				StringBlock block = ViewInfo.StringInfo.GetLinkByPoint(e.Location);
				if(block == null) {
					if(PrevCursor != null)
						Cursor = PrevCursor;
					PrevCursor = null;
				}
				else {
					if(PrevCursor == null)
						PrevCursor = Cursor;
					Cursor = Cursors.Hand;
				}
				if(PrevBlock != block) {
					PrevBlock = block;
					ViewInfo.ForceStateChanged();
				}
			}
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			TryClickHyperlink(e);
		}
		protected virtual void TryClickHyperlink(MouseEventArgs e) {
			if(!HasHyperlink)
				return;
			StringBlock block = ViewInfo.StringInfo.GetLinkByPoint(e.Location);
			if(block != null)
				RaiseHyperlinkClick(new HyperlinkClickEventArgs() { Text = block.Text, Link = block.Link, MouseArgs = e });
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(IsDisposed)
				return;
			ViewInfo.OnStateChanged(GetState(ObjectState.Hot), true);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(IsDisposed)
				return;
			ViewInfo.OnStateChanged(GetState(ObjectState.Hot), true);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(IsDisposed)
				return;
			ViewInfo.OnStateChanged(GetState(ObjectState.Normal), true);
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free),
	ToolboxTabName(AssemblyInfo.DXTabCommon)]
	public class HyperlinkLabelControl : LabelControl {
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new HyperlinkLabelControlViewInfo(this);
		}
		protected override void OnTextChanged(EventArgs e) {
			if(ViewInfo != null)
				((HyperlinkLabelControlViewInfo)ViewInfo).HyperLinkText = null;
			base.OnTextChanged(e);
		}
		LinkBehavior linkBehavior = LinkBehavior.SystemDefault;
		[DefaultValue(LinkBehavior.SystemDefault)]
		public LinkBehavior LinkBehavior {
			get { return linkBehavior; }
			set {
				if(LinkBehavior == value)
					return;
				linkBehavior = value;
				OnPropertiesChanged();
			}
		}
		protected override AppearanceObject CreateAppearance() {
			HyperlinkLabelControlAppearanceObject res = new HyperlinkLabelControlAppearanceObject();
			res.Changed += new EventHandler(OnStyleChanged);
			res.SizeChanged += new EventHandler(OnStyleChanged);
			res.PaintChanged += new EventHandler(OnStyleChanged);
			return res;
		}
		protected override Utils.Text.StringInfo CalcStringInfoCore() {
			DevExpress.Utils.Text.StringInfo strInfo = ViewInfo.CalcStringInfo(ViewInfo == null ? Text : ViewInfo.DisplayText, false, LabelAutoSizeMode.Default, 10000);
			return strInfo;
		}
		protected override Size CalcContentSizeByTextSizeCore(LabelAutoSizeMode mode, string text)		{
			return base.CalcContentSizeByTextSizeCore(mode, ViewInfo == null ? text : ViewInfo.DisplayText);
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public new HyperlinkLabelControlAppearanceObject Appearance { get { return base.Appearance as HyperlinkLabelControlAppearanceObject; } }
		[ DefaultValue(true), DXCategory(CategoryName.Appearance),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowHtmlString {
			get { return base.AllowHtmlString; }
			set { base.AllowHtmlString = value; }
		}
		protected override bool HasHyperlink {
			get { return true; }
		}
		protected override bool DefaultAllowHtmlString {
			get { return true; }
		}
		bool linkVisited;
		[DefaultValue(false)]
		public bool LinkVisited {
			get { return linkVisited; }
			set {
				if(LinkVisited == value)
					return;
				linkVisited = value;
				ViewInfo.SetPaintAppearanceDirty();
				LayoutChanged();
			}
		}
	}
}
