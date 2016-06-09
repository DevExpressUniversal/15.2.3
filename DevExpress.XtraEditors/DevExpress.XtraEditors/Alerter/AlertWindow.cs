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
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.Skins.XtraForm;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.Utils.Win;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
namespace DevExpress.XtraBars.Alerter {
	class DefaultSkinProvider : ISkinProvider {
		public string SkinName {
			get { return "Blue"; }
		}
	}
	public enum AlertFormLocation { TopLeft, TopRight, BottomLeft, BottomRight };
	public enum AlertFormDisplaySpeed { Fast, Moderate, Slow };
	public enum AlertFormControlBoxPosition { Top, Right };
	public enum AlertFormCloseReason { None, TimeUp, UserClosing };
	enum AlertFormImagePosition { Top, Center };
	enum AlertTimerState { None, Start, Live, Hide };
	public class AlertViewInfo {
		protected bool isRightToLeft = false;
		protected const int indent = 15;
		protected const int textIndent = 10;
		protected int topElementHeight = 10;
		Point topElementOffset = new Point(1, 11);
		protected Point buttonsOffset = new Point(2, 2);
		AlertFormCore owner;
		Rectangle imageRectangle = Rectangle.Empty;
		Rectangle captionRectangle = Rectangle.Empty;
		Rectangle topElementRectangle = Rectangle.Empty;
		Rectangle textRectangle = Rectangle.Empty;
		Rectangle maxTextRectangle = Rectangle.Empty;
		StringInfo textInfo, captionInfo;
		DefaultSkinProvider defaultSkinProvider = new DefaultSkinProvider();
		internal AppearanceCaptionObject appearanceCaption = new AppearanceCaptionObject();
		internal AppearanceObject appearanceText = new AppearanceObject();
		internal AppearanceSelectedObject appearanceHotTrackedText = new AppearanceSelectedObject();
		public AlertViewInfo(AlertFormCore form) {
			isRightToLeft = WindowsFormsSettings.GetIsRightToLeft(form);
			this.owner = form;
			AssignAppearance();
		}
		public virtual int RealFormHeight {
			get {
				return Owner.Height;
			}
		}
		Color GetHotTrackedTextColor() {
			Skin skin = BarSkins.GetSkin(CurrentSkinProvider);
			Color ret = skin.Colors.GetColor("AlertWindowHotTrackedForeColor", Color.Empty);
			if(ret != Color.Empty) return ret;
			skin = EditorsSkins.GetSkin(CurrentSkinProvider);
			ret = skin.Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
			if(ret == Color.Empty) return Color.Blue;
			return ret;
		}
		Color GetAlertColor(string element) {
			SkinElement se = BarSkins.GetSkin(CurrentSkinProvider)[element];
			Color ret = se.Color.GetForeColor();
			if(ret == Color.Empty) ret = Color.Black;
			return ret;
		}
		internal void AssignAppearance() {
			appearanceCaption.Assign(owner.AlertControl != null ? owner.AlertControl.AppearanceCaption : new AppearanceCaptionObject());
			appearanceCaption.TextOptions.VAlignment = VertAlignment.Top;
			if(appearanceCaption.TextOptions.WordWrap == WordWrap.Default) appearanceCaption.TextOptions.WordWrap = WordWrap.NoWrap;
			if(appearanceCaption.TextOptions.Trimming == Trimming.Default) appearanceCaption.TextOptions.Trimming = Trimming.EllipsisCharacter;
			if(appearanceCaption.ForeColor == Color.Empty) appearanceCaption.ForeColor = GetAlertColor(BarSkins.SkinAlertWindow); 
			appearanceCaption.TextOptions.RightToLeft = isRightToLeft;
			appearanceText.Assign(owner.AlertControl != null ? owner.AlertControl.AppearanceText : new AppearanceObject());
			if(appearanceText.TextOptions.VAlignment == VertAlignment.Default) appearanceText.TextOptions.VAlignment = VertAlignment.Top;
			if(appearanceText.TextOptions.WordWrap == WordWrap.Default) appearanceText.TextOptions.WordWrap = WordWrap.Wrap;
			if(appearanceText.TextOptions.Trimming == Trimming.Default) appearanceText.TextOptions.Trimming = Trimming.EllipsisCharacter;
			if(appearanceText.ForeColor == Color.Empty) appearanceText.ForeColor = GetAlertColor(BarSkins.SkinAlertWindow);
			appearanceText.TextOptions.RightToLeft = isRightToLeft;
			appearanceHotTrackedText.Assign(owner.AlertControl != null ? owner.AlertControl.AppearanceHotTrackedText : new AppearanceSelectedObject());
			if(appearanceHotTrackedText.TextOptions.VAlignment == VertAlignment.Default) appearanceHotTrackedText.TextOptions.VAlignment = VertAlignment.Top;
			if(appearanceHotTrackedText.TextOptions.WordWrap == WordWrap.Default) appearanceHotTrackedText.TextOptions.WordWrap = WordWrap.Wrap;
			if(appearanceHotTrackedText.TextOptions.Trimming == Trimming.Default) appearanceHotTrackedText.TextOptions.Trimming = Trimming.EllipsisCharacter;
			if(appearanceHotTrackedText.ForeColor == Color.Empty) appearanceHotTrackedText.ForeColor = GetHotTrackedTextColor();
			appearanceHotTrackedText.TextOptions.RightToLeft = isRightToLeft;
		}
		protected AlertInfo Info { get { return owner.Info; } }
		Image Image {
			get {
				if(owner == null || owner.Info == null) return null;
				return owner.Info.Image;
			}
		}
		protected AlertFormCore Owner { get { return owner; } }
		public virtual void Calculate() {
			CalculateSkinElementsInfo();
			CalculateTopElementRectangle();
			CalculateImageRectangle();
			CalculateCaptionRectangle();
			CalculateButtons();
			CalculateTextRectangle();
		}
		public void CalculateTextRectangle(GraphicsCache cache) {
			if(owner.AllowHtmlText)
				TextRectangle = CalculateHtmlTextRectangleCore(cache);
			else
				TextRectangle = CalculateTextRectangleCore(cache);
		}
		public void CalculateTextRectangle() {
			Graphics gr = Graphics.FromHwnd(Owner.Handle);
			GraphicsCache cache = new GraphicsCache(gr);
			try {
				CalculateTextRectangle(cache);
			} finally {
				cache.Dispose();
				gr.Dispose();
			}
		}
		protected virtual void CalculateButtons() {
			int pIndex = 0, fabricateWidth = 0;
			foreach(AlertButton item in Owner.Buttons) {
				if(item.Predefined)
					item.Bounds = GetControlBoxElementRectangle(pIndex++, item.GetButtonSize());
				else {
					item.Bounds = GetButtonElementRectangle(item.GetButtonSize(), fabricateWidth);
					fabricateWidth += item.Bounds.Width;
				}
			}
		}
		private Rectangle CalculateHtmlTextRectangleCore(GraphicsCache cache) {
			return CalcHtmlTextSize(cache.Graphics);
		}
		private Rectangle CalculateTextRectangleCore(GraphicsCache cache) {
			return CalcTextSize(cache);
		}
		protected void CalculateCaptionRectangle() {
			Graphics gr = Graphics.FromHwnd(Owner.Handle);
			GraphicsCache cache = new GraphicsCache(gr);
			try {
				if(owner.AllowHtmlText)
					captionRectangle = CalculateHtmlCaptionRectangleCore(cache);
				else
					captionRectangle = CalculateCaptionRectangleCore(cache);
			} finally {
				cache.Dispose();
				gr.Dispose();
			}
		}
		private Rectangle CalculateHtmlCaptionRectangleCore(GraphicsCache cache) {
			int top = GetTopTextPosition();
			SizeF captionSize = SizeF.Empty;
			int x = RightTextIndent;
			int w = GetRightTextPosition() - RightTextIndent;
			if(isRightToLeft) {
				x = x + w;
				w = -w;
			}
			Rectangle result = CalcTextRectangle(x, top, w, 0);
			if(Info == null) return result;
			if(Info.Caption.Trim() != string.Empty) {
				CaptionInfo = StringPainter.Default.Calculate(cache.Graphics, appearanceCaption, Info.Caption, GetRightTextPosition() - RightTextIndent);
				result = CalcTextRectangle(x, top, w, Convert.ToInt32(CaptionInfo.Bounds.Height));
				CaptionInfo.SetLocation(result.X, result.Y);
				return result;
			}
			return result;
		}
		private Rectangle CalculateCaptionRectangleCore(GraphicsCache cache) {
			int top = GetTopTextPosition();
			SizeF captionSize = SizeF.Empty;
			int x = RightTextIndent;
			int w = GetRightTextPosition() - RightTextIndent;
			if(isRightToLeft) {
				x = x + w;
				w = -w;
			}
			Rectangle result = CalcTextRectangle(x, top, w, 0);
			if(Info == null) return result;
			if(Info.GetCaption().Trim() != string.Empty) {
				captionSize = appearanceCaption.CalcTextSize(cache, Info.Caption, owner.ClientRectangle.Width - 2 * textIndent);
				return CalcTextRectangle(x, top, w, Convert.ToInt32(captionSize.Height));
			}
			return result;
		}
		protected void CalculateTopElementRectangle() {
			topElementRectangle = CalculateTopElementRectangleCore();
		}
		protected virtual Rectangle CalculateTopElementRectangleCore() {
			Rectangle r = owner.ClientRectangle;
			return new Rectangle(r.X, r.Y, r.Width, topElementHeight);
		}
		protected void CalculateImageRectangle() {
			imageRectangle = CalculateImageRectangleCore();
		}
		protected virtual Rectangle CalculateImageRectangleCore() {
			Rectangle r = owner.ClientRectangle;
			if(Image != null) {
				Size imageSize = ImageLayoutHelper.GetImageBounds(r, Image.Size, ImageLayoutMode.Squeeze).Size;
				return new Rectangle(isRightToLeft ? r.Right - indent - imageSize.Width : r.X + indent,
					GetImageTop(r, imageSize), imageSize.Width, imageSize.Height);
			}
			return Rectangle.Empty;
		}
		public void CalculateInfo() {
			Calculate();
		}
		#region CalcTextRect
		int RightTextIndent {
			get {
				return isRightToLeft ? imageRectangle.Left - textIndent : imageRectangle.Right + textIndent;
			}
		}
		private void CalcTextRectangle() {
			int top = GetTopTextPosition();
			captionRectangle = new Rectangle(RightTextIndent, top,
				GetRightTextPosition() - RightTextIndent, GetBottomTextPosition() - top);
		}
		protected int GetTopTextPosition() {
			int ret = owner.ClientRectangle.Y + topElementHeight + textIndent;
			if(owner.Buttons != null && owner.ControlBoxPosition == AlertFormControlBoxPosition.Top) {
				AlertButton item = owner.Buttons.GetFirstPredefinedButton();
				if(item != null) {
					ret = Math.Max(ret, GetControlBoxElementRectangle(0, item.GetButtonSize()).Bottom);
				}
			}
			return ret;
		}
		int GetRightTextPosition() {
			int ret = isRightToLeft ? textIndent : owner.ClientRectangle.Width - textIndent;
			if(owner.Buttons != null && owner.ControlBoxPosition == AlertFormControlBoxPosition.Right) {
				AlertButton item = owner.Buttons.GetFirstPredefinedButton();
				if(item != null) {
					if(isRightToLeft)
						ret = Math.Max(ret, item.Bounds.Right);
					else ret = Math.Min(ret, item.Bounds.Left);
				}
			}
			return ret;
		}
		int GetBottomTextPosition() {
			int ret = owner.ClientRectangle.Height - textIndent;
			if(owner.Buttons != null) {
				AlertButton item = owner.Buttons.GetFirstCustomButton(RightTextIndent);
				if(item != null) {
					ret = Math.Min(ret, item.Bounds.Top);
				}
			}
			return ret;
		}
		#endregion
		public virtual ISkinProvider CurrentSkinProvider {
			get {
				if(owner.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return defaultSkinProvider;
				return owner.LookAndFeel.ActiveLookAndFeel;
			}
		}
		protected void CalculateSkinElementsInfo() {
			SkinElement element = BarSkins.GetSkin(CurrentSkinProvider)[BarSkins.SkinAlertCaptionTop];
			if(element != null) {
				topElementHeight = element.ContentMargins.Height;
				if(element.Glyph != null && element.Glyph.Image != null)
					topElementHeight += element.Glyph.Image.Height;
				topElementOffset = element.Offset.Offset;
			}
			element = BarSkins.GetSkin(CurrentSkinProvider)[BarSkins.SkinAlertWindow];
			if(element != null)
				buttonsOffset = element.Offset.Offset;
		}
		int GetImageTop(Rectangle clientRect, Size imageSize) {
			int cTop = (clientRect.Y + clientRect.Height - imageSize.Height) / 2;
			int tTop = clientRect.Y + indent + topElementOffset.Y;
			if(tTop > cTop) tTop = cTop;
			if(owner.ImagePosition == AlertFormImagePosition.Center)
				return cTop;
			else return tTop;
		}
		public Rectangle ImageRectangle { get { return imageRectangle; } }
		public Rectangle CaptionRectangle { get { return captionRectangle; } }
		public Rectangle TextRectangle {
			get { return textRectangle; }
			set {
				if(TextRectangle.Equals(value)) return;
				if(value.Height > TextRectangle.Height ||
					value.Width > TextRectangle.Width) maxTextRectangle = value;
				textRectangle = value;
			}
		}
		internal Rectangle MaxTextRectangle { get { return maxTextRectangle; } }
		public Rectangle TopElementRectangle { get { return topElementRectangle; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Rectangle GetControlBoxElementRectangle(int count, Size size) {
			return
				new Rectangle(
					isRightToLeft ?
					topElementOffset.X + (owner.ControlBoxPosition == AlertFormControlBoxPosition.Top ? size.Width * count : 0) :
					owner.ClientRectangle.Right - topElementOffset.X - size.Width - (owner.ControlBoxPosition == AlertFormControlBoxPosition.Top ? size.Width * count : 0),
					owner.ClientRectangle.Top + topElementOffset.Y + (owner.ControlBoxPosition == AlertFormControlBoxPosition.Right ? size.Height * count : 0),
					size.Width, size.Height);
		}
		protected virtual Rectangle GetButtonElementRectangle(Size size, int indent) {
			if(size == Size.Empty) return Rectangle.Empty;
			return
				new Rectangle(
					isRightToLeft ?
					owner.ClientRectangle.Right - size.Width - buttonsOffset.X - indent :
					owner.ClientRectangle.X + buttonsOffset.X + indent,
					owner.ClientRectangle.Bottom - buttonsOffset.Y - size.Height,
					size.Width, size.Height);
		}
		protected virtual Rectangle CalcTextRectangle(int x, int y, int width, int height) {
			int bottomTextPosition = GetBottomTextPosition();
			if(y + height < bottomTextPosition)
				return new Rectangle(x, y, width, height);
			else
				return new Rectangle(x, y, width, bottomTextPosition - y);
		}
		internal AppearanceObject TextAppearance {
			get {
				if(owner.Linking) return appearanceHotTrackedText;
				return appearanceText;
			}
		}
		protected Rectangle CalcTextSize(GraphicsCache graphicsCache) {
			SizeF captionSize = SizeF.Empty, textSize = SizeF.Empty;
			if(Info == null) return Rectangle.Empty;
			if(Info.GetText().Trim() != string.Empty) {
				textSize = TextAppearance.CalcTextSize(graphicsCache, Text, CaptionRectangle.Width);
				return CalcTextRectangle(CaptionRectangle.Left, CaptionRectangle.Bottom, CaptionRectangle.Width, Convert.ToInt32(textSize.Height));
			}
			return Rectangle.Empty;
		}
		#region HtmlText
		internal string Text {
			get {
				string ret = string.Empty;
				if(owner.Linking && !string.IsNullOrEmpty(Info.HotTrackedText))
					ret = Info.HotTrackedText;
				else {
					if(owner.Linking && owner.AllowHtmlText)
						ret = string.Format("<u>{0}", Info.Text);
					else
						ret = Info.Text;
				}
				if(ret == null) return string.Empty;
				return ret;
			}
		}
		public StringInfo TextInfo {
			get {
				if(textInfo == null)
					textInfo = new StringInfo();
				return textInfo;
			}
			set {
				textInfo = value;
			}
		}
		public StringInfo CaptionInfo {
			get {
				if(captionInfo == null)
					captionInfo = new StringInfo();
				return captionInfo;
			}
			set {
				captionInfo = value;
			}
		}
		protected virtual Rectangle CalcHtmlTextSize(Graphics g) {
			if(Info.Text.Trim() != string.Empty) {
				Rectangle maxRect = new Rectangle(CaptionRectangle.Left, CaptionRectangle.Bottom, CaptionRectangle.Width, GetBottomTextPosition() - CaptionRectangle.Bottom);
				StringCalculateArgs args = new StringCalculateArgs(g, appearanceText, Text, maxRect, null);
				args.AllowBaselineAlignment = false;
				TextInfo = StringPainter.Default.Calculate(args);
				Rectangle result = CalcTextRectangle(CaptionRectangle.Left, CaptionRectangle.Bottom, CaptionRectangle.Width, Convert.ToInt32(textInfo.Bounds.Height));
				textInfo.SetLocation(result.X, result.Y);
				return result;
			}
			return Rectangle.Empty;
		}
		#endregion
	}
	public class AlertAutoHeightViewInfo : AlertViewInfo {
		public AlertAutoHeightViewInfo(AlertFormCore form)
			: base(form) {
		}
		public override void Calculate() {
			CalculateSkinElementsInfo();
			CalculateTopElementRectangle();
			CalculateImageRectangle();
			CalculateCaptionRectangle();
			CalculateTextRectangle();
			CalculateButtons();
			PatchFormHeight();
		}
		public override int RealFormHeight {
			get {
				return Math.Max(CaptionRectangle.Height + TextRectangle.Height, ImageRectangle.Height) +
				GetTopTextPosition() +  
				GetButtonsBarHeight() + 2 * buttonsOffset.Y;
			}
		}
		protected virtual void PatchFormHeight() {
			if(!Owner.LockLocationUpdate)
				Owner.Height = RealFormHeight;
		}
		private int GetButtonsBarHeight() {
			AlertButton button = Owner.Buttons.GetFirstCustomButton(0);
			if(button != null)
				return button.Bounds.Height;
			return 0;
		}
		protected override Rectangle CalcTextRectangle(int x, int y, int width, int height) {
			return new Rectangle(x, y, width, height);
		}
		protected override Rectangle CalcHtmlTextSize(Graphics g) {
			if(Info.Text.Trim() != string.Empty) {
				StringCalculateArgs args = new StringCalculateArgs(g, appearanceText, Text, new Rectangle(0, 0, CaptionRectangle.Width, 0), null);
				args.AllowBaselineAlignment = false;
				TextInfo = StringPainter.Default.Calculate(args);
				Rectangle result = CalcTextRectangle(CaptionRectangle.Left, CaptionRectangle.Bottom, CaptionRectangle.Width, Convert.ToInt32(TextInfo.Bounds.Height));
				TextInfo.SetLocation(result.X, result.Y);
				return result;
			}
			return Rectangle.Empty;
		}
		protected override Rectangle GetButtonElementRectangle(Size size, int indent) {
			if(size == Size.Empty) return Rectangle.Empty;
			int top = Math.Max(ImageRectangle.Bottom, TextRectangle.Bottom);
			return new Rectangle(
				isRightToLeft ?
					Owner.ClientRectangle.Right - size.Width - buttonsOffset.X - indent :
					Owner.ClientRectangle.X + buttonsOffset.X + indent,
					top + buttonsOffset.Y,
					size.Width, size.Height);
		}
	}
	public class AlertPainter {
		AlertFormCore owner;
		public AlertPainter(AlertFormCore form) {
			this.owner = form;
		}
		protected AlertFormCore Owner { get { return owner; } }
		protected AlertInfo Info { get { return owner.Info; } }
		protected AlertViewInfo ViewInfo { get { return owner.ViewInfo; } }
		Image Image { get { return owner.Info.Image; } }
		public virtual void Draw(PaintEventArgs e) {
			Skin skin = BarSkins.GetSkin(ViewInfo.CurrentSkinProvider);
			GraphicsCache cache = new GraphicsCache(e);
			DrawContent(cache, skin);
			DrawTopElement(cache, skin);
			if(ViewInfo.ImageRectangle != Rectangle.Empty)
				e.Graphics.DrawImage(Image, ViewInfo.ImageRectangle);
			DrawText(cache);
		}
		protected virtual void DrawContent(GraphicsCache graphicsCache, Skin skin) {
			SkinElement element = skin[BarSkins.SkinAlertWindow];
			SkinElementInfo eInfo = new SkinElementInfo(element, owner.ClientRectangle);
			ObjectPainter.DrawObject(graphicsCache, SkinElementPainter.Default, eInfo);
		}
		protected virtual void DrawTopElement(GraphicsCache graphicsCache, Skin skin) {
			SkinElement element = skin[BarSkins.SkinAlertCaptionTop];
			SkinElementInfo eInfo = new SkinElementInfo(element, ViewInfo.TopElementRectangle);
			ObjectPainter.DrawObject(graphicsCache, SkinElementPainter.Default, eInfo);
		}
		void DrawText(GraphicsCache graphicsCache) {
			if(owner.AllowHtmlText)
				DrawHtmlText(graphicsCache);
			else
				DrawAppearanceText(graphicsCache);
		}
		protected virtual void DrawHtmlText(GraphicsCache graphicsCache) {
			ViewInfo.CalculateTextRectangle();
			if(Info == null) return;
			if(Info.GetCaption().Trim() != string.Empty)
				StringPainter.Default.DrawString(graphicsCache, ViewInfo.CaptionInfo);
			if(ViewInfo.Text.Trim() != string.Empty)
				StringPainter.Default.DrawString(graphicsCache, ViewInfo.TextInfo);
		}
		protected virtual void DrawAppearanceText(GraphicsCache graphicsCache) {
			ViewInfo.CalculateTextRectangle();
			if(Info == null) return;
			if(Info.GetCaption().Trim() != string.Empty) {
				ViewInfo.appearanceCaption.DrawString(graphicsCache, Info.Caption, ViewInfo.CaptionRectangle);
			}
			if(ViewInfo.Text.Trim() != string.Empty) {
				ViewInfo.TextAppearance.DrawString(graphicsCache, ViewInfo.Text, ViewInfo.TextRectangle);
			}
		}
		internal void UpdateRegion() {
			SkinElement se = BarSkins.GetSkin(ViewInfo.CurrentSkinProvider)[BarSkins.SkinAlertWindow];
			if(se == null) {
				owner.Region = null;
				return;
			}
			int cornerRadius = se.Properties.GetInteger(BarSkins.SkinAlertWindowCornerRadius);
			if(cornerRadius == 0) owner.Region = null;
			else owner.Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, owner.Size), cornerRadius);
		}
	}
	public enum AlertFormShowingEffect { FadeIn, SlideVertical, SlideHorizontal, MoveVertical, MoveHorizontal };
	[ToolboxItem(false)]
	class AlertTimer : Timer {
		AlertTimerState state = AlertTimerState.None;
		AlertFormCore form;
		int height, width, top, left;
		int moveCount = 1;
		public AlertTimer(AlertFormCore form) {
			this.form = form;
			switch(form.FormShowingEffect) {
				case AlertFormShowingEffect.SlideVertical:
					form.Opacity = 1;
					InitBounds();
					form.Height = 0;
					break;
				case AlertFormShowingEffect.SlideHorizontal:
					form.Opacity = 1;
					InitBounds();
					form.Width = 0;
					break;
				case AlertFormShowingEffect.MoveVertical:
					form.Opacity = 1;
					InitBounds();
					if(FormBottom)
						form.Top = form.currentScreen.WorkingArea.Bottom;
					else
						form.Top = form.currentScreen.WorkingArea.Top - height;
					break;
				case AlertFormShowingEffect.MoveHorizontal:
					form.Opacity = 1;
					InitBounds();
					if(FormRight)
						form.Left = form.currentScreen.WorkingArea.Right;
					else
						form.Left = form.currentScreen.WorkingArea.Left - width;
					break;
			}
		}
		bool FormBottom { get { return form.FormLocation == AlertFormLocation.BottomLeft || form.FormLocation == AlertFormLocation.BottomRight; } }
		bool FormRight { get { return form.FormLocation == AlertFormLocation.BottomRight || form.FormLocation == AlertFormLocation.TopRight; } }
		void InitBounds() {
			height = form.Height;
			width = form.Width;
			top = form.GetTop();
			left = form.GetLeft();
		}
		public void SetLiveState() {
			form.Opacity = form.DefaultOpacity;
			SetFormHeight(height, 0);
			SetFormWidth(width, 0);
			SetFormTop(top);
			SetFormLeft(left);
			form.UpdateViewInfo();
			this.State = AlertTimerState.Live;
			form.UpdateViewInfo();
			moveCount = 1;
		}
		public AlertTimerState State {
			get { return state; }
			set {
				if(form == null) return;
				if(state == value) return;
				if(value == AlertTimerState.Hide) {
					AlertFormClosingEventArgs args = new AlertFormClosingEventArgs(form, form.CustomClosing ? AlertFormCloseReason.UserClosing : AlertFormCloseReason.TimeUp);
					if(form.AlertControl != null)
						form.AlertControl.RaiseFormClosing(args);
					if(args.Cancel) {
						form.CustomClosing = false;
						return;
					}
					InitBounds();
				}
				state = value;
				this.Stop();
				if(state == AlertTimerState.Live)
					this.Interval = form.AutoFormDelay;
				else
					this.Interval = AppearanceInterval;
				if(state != AlertTimerState.None) this.Start();
			}
		}
		int Shift {
			get {
				int ret = 2;
				if(form.FormDisplaySpeed == AlertFormDisplaySpeed.Slow) ret = 1;
				if(form.FormDisplaySpeed == AlertFormDisplaySpeed.Fast) ret = 3;
				return ret * AlertFormCore.DisplaySpeedPointCount * (form.FormShowingEffect == AlertFormShowingEffect.SlideHorizontal ? 2 : 1);
			}
		}
		int AppearanceInterval {
			get {
				int ret = 60;
				if(form.FormDisplaySpeed == AlertFormDisplaySpeed.Slow) ret = 120;
				if(form.FormDisplaySpeed == AlertFormDisplaySpeed.Fast) ret = 30;
				if(form.CustomClosing || form.FormShowingEffect != AlertFormShowingEffect.FadeIn) ret = 5;
				return ret;
			}
		}
		protected override void OnTick(EventArgs e) {
			base.OnTick(e);
			if(form == null) return;
			if(State == AlertTimerState.Hide) SetFormEffect(false);
			if(State == AlertTimerState.Live)
				this.State = AlertTimerState.Hide;
			if(State == AlertTimerState.Start) SetFormEffect(true);
		}
		void SetMoveVerticalEffect(bool start) {
			if(start) {
				if((FormBottom && form.Top <= top) || (!FormBottom && form.Top >= top)) {
					SetLiveState();
				} else {
					SetFormTop(form.Top + MoveShift * (FormBottom ? -1 : 1));
				}
			} else {
				if((FormBottom && form.Top >= form.currentScreen.WorkingArea.Bottom) || (!FormBottom && form.Top <= (form.currentScreen.WorkingArea.Top - height))) {
					CloseForm();
					return;
				}
				SetFormTop(form.Top + MoveShift * (FormBottom ? 1 : -1));
			}
		}
		void SetMoveHorizontalEffect(bool start) {
			if(start) {
				if((FormRight && form.Left <= left) || (!FormRight && form.Left >= left)) {
					SetLiveState();
				} else {
					SetFormLeft(form.Left + MoveShift * (FormRight ? -1 : 1));
				}
			} else {
				if((FormRight && form.Left >= form.currentScreen.WorkingArea.Right) || (!FormRight && form.Left <= (form.currentScreen.WorkingArea.Left - width))) {
					CloseForm();
					return;
				}
				SetFormLeft(form.Left + MoveShift * (FormRight ? 1 : -1));
			}
		}
		int MoveShift { get { return Shift * moveCount++; } }
		void SetFormEffect(bool start) {
			if(form.FormShowingEffect == AlertFormShowingEffect.MoveVertical) {
				SetMoveVerticalEffect(start);
				return;
			}
			if(form.FormShowingEffect == AlertFormShowingEffect.MoveHorizontal) {
				SetMoveHorizontalEffect(start);
				return;
			}
			if(start) {
				if(form.Opacity >= form.DefaultOpacity && form.Height >= height && form.Width >= width) {
					SetLiveState();
				}
				if(form.Opacity < form.DefaultOpacity) form.Opacity += 0.1;
				if(form.Height < height) {
					SetFormHeight(form.Height + Shift, Shift);
					form.UpdateViewInfo();
				}
				if(form.Width < width) {
					SetFormWidth(form.Width + Shift, Shift);
					form.UpdateViewInfo();
				}
			} else {
				if(form.Opacity <= 0 || form.Height <= Shift || form.Width <= Shift) {
					CloseForm();
					return;
				}
				if(form.FormShowingEffect == AlertFormShowingEffect.FadeIn) form.Opacity -= 0.1;
				if(form.FormShowingEffect == AlertFormShowingEffect.SlideVertical)
					SetFormHeight(form.Height - Shift, 0);
				if(form.FormShowingEffect == AlertFormShowingEffect.SlideHorizontal)
					SetFormWidth(form.Width - Shift, 0);
			}
		}
		void SetFormHeight(int height, int shift) {
			if(form.FormShowingEffect != AlertFormShowingEffect.SlideVertical) return;
			if(FormBottom) {
				form.Top = top + (this.height - height) + shift;
			}
			form.Height = height;
		}
		void SetFormWidth(int width, int shift) {
			if(form.FormShowingEffect != AlertFormShowingEffect.SlideHorizontal) return;
			if(FormRight) {
				form.Left = left + (this.width - width) + shift;
			}
			form.Width = width;
		}
		void SetFormTop(int top) {
			if(form.FormShowingEffect != AlertFormShowingEffect.MoveVertical) return;
			if((FormBottom && top < this.top) ||
				(!FormBottom && top > this.top)) top = this.top;
			form.Top = top;
		}
		void SetFormLeft(int left) {
			if(form.FormShowingEffect != AlertFormShowingEffect.MoveHorizontal) return;
			if((FormRight && left < this.left) ||
				(!FormRight && left > this.left)) left = this.left;
			form.Left = left;
		}
		void CloseForm() {
			if(form == null) return;
			form.Close();
			form.Dispose();
		}
	}
	public class StickForm : TopFormBase {
		protected internal int DefaultStickLength = 20;
		protected override void WndProc(ref Message m) {
			const int Wm_WindowPosChanging = 0x46;
			if(m.Msg == Wm_WindowPosChanging && WmWindowPosChanging(m.LParam)) return;
			base.WndProc(ref m);
		}
		internal Rectangle GetCurrentScreenArea() {
			return GetCurrentScreenArea(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
		}
		Rectangle GetCurrentScreenArea(int x, int y) {
			return Screen.GetWorkingArea(new Point(x, y));
		}
		[System.Security.SecuritySafeCritical]
		bool WmWindowPosChanging(IntPtr lParam) {
			if(DefaultStickLength == 0) return true;
			if(LockLocationUpdate) return true;
			int offset = IntPtr.Size * 2;
			int x = Marshal.ReadInt32(lParam, offset);
			int y = Marshal.ReadInt32(lParam, offset + 4);
			int width = Marshal.ReadInt32(lParam, offset + 8);
			int height = Marshal.ReadInt32(lParam, offset + 12);
			Rectangle screenArea = GetCurrentScreenArea(x + width / 2, y + height / 2);
			if(Math.Abs(screenArea.Left - x) < DefaultStickLength) Marshal.WriteInt32(lParam, offset, screenArea.Left);
			if(Math.Abs(screenArea.Top - y) < DefaultStickLength) Marshal.WriteInt32(lParam, offset + 4, screenArea.Top);
			if(Math.Abs(screenArea.Right - (x + width)) < DefaultStickLength) Marshal.WriteInt32(lParam, offset, screenArea.Right - width);
			if(Math.Abs(screenArea.Bottom - (y + height)) < DefaultStickLength) Marshal.WriteInt32(lParam, offset + 4, screenArea.Bottom - height);
			return false;
		}
		protected internal virtual bool LockLocationUpdate { get { return false; } }
	}
	public class AlertFormCore : StickForm, IToolTipControlClient, IAnimatedItem, ISupportXtraAnimation, ISupportToolTipsForm {
		public static int DisplaySpeedPointCount = 5;
		bool formMoving = false, isMoved = false;
		int MouseDownX, MouseDownY;
		int autoFormDelay = 7000;
		protected Point customLocation = Point.Empty;
		protected internal Screen currentScreen;
		protected internal double DefaultOpacity = 0.8;
		internal bool CustomClosing = false;
		Form ownerForm = null;
		AlertInfo info;
		AlertFormLocation formLocation = AlertFormLocation.BottomRight;
		AlertFormDisplaySpeed formDisplaySpeed = AlertFormDisplaySpeed.Moderate;
		AlertFormControlBoxPosition controlBoxPosition = AlertFormControlBoxPosition.Top;
		AlertFormImagePosition imagePosition = AlertFormImagePosition.Top;
		AlertFormShowingEffect formShowingEffect = AlertFormShowingEffect.FadeIn;
		AlertTimer alertTimer = null;
		IAlertControl control;
		AlertViewInfo viewInfo;
		AlertPainter painter;
		AlertButtonCollection buttonElements;
		ToolTipController toolTipController = new ToolTipController();
		bool showToolTips = true;
		bool showCloseButton = true;
		bool showPinButton = true;
		bool allowHtmlText = false;
		bool allowHotTrack = true;
		bool styleChanged = false;
		public AlertFormCore(IAlertControl control) : this(new Point(0, 0), control) { }
		public AlertFormCore(Point location, IAlertControl control) : this(location, control, null) { }
		public AlertFormCore(Point location, IAlertControl control, AlertInfo info) {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
			this.control = control;
			InitAlertControlProperties(control);
			currentScreen = CalcCurrentScreen(location);
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.Manual;
			this.ShowInTaskbar = false;
			this.Opacity = 0;
			this.Size = DevExpress.Utils.ScaleUtils.GetScaleSize(DefaultSize);
			UpdateLocation();
			this.Padding = new Padding(2);
			this.info = info;
			toolTipController.ToolTipType = ToolTipType.SuperTip;
			toolTipController.AddClientControl(this);
			this.LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			if(this.Info != null)
				this.Info.InfoChanged += new EventHandler(Info_InfoChanged);
		}
		void Info_InfoChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		internal void UpdateViewInfo() {
			if(viewInfo != null) {
				viewInfo.CalculateInfo();
				viewInfo.AssignAppearance();
			}
			UpdateRegion();
		}
		internal void UpdateRegion() {
			if(painter != null)
				painter.UpdateRegion();
		}
		public UserLookAndFeel LookAndFeel {
			get {
				if(control != null)
					return control.LookAndFeel;
				return UserLookAndFeel.Default;
			}
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			styleChanged = true;
			try {
				UpdateViewInfo();
				Invalidate();
			} finally {
				styleChanged = false;
			}
		}
		public void LayoutChanged() {
			if(viewInfo != null)
				viewInfo.Calculate();
			if(painter != null) {
				painter.UpdateRegion();
				Invalidate();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(selectedTimer != null) {
					selectedTimer.Tick -= new EventHandler(OnSelectedTimerTick);
				}
				if(buttonElements != null)
					buttonElements.Dispose();
				toolTipController.Dispose();
				if(this.LookAndFeel != null)
					this.LookAndFeel.StyleChanged -= new EventHandler(LookAndFeel_StyleChanged);
				if(this.Info != null)
					this.Info.InfoChanged -= new EventHandler(Info_InfoChanged);
				UnsubscribeChildControls();
				if(alertTimer != null) alertTimer.Dispose();
				if(selectedTimer != null) selectedTimer.Dispose();
				StopAnimation();
				imageHelper = null;
			}
			base.Dispose(disposing);
		}
		protected virtual int GetDesiredFormWidth() {
			AlertFormWidthEventArgs args = new AlertFormWidthEventArgs();
			if(control != null)
				return control.RaiseGetDesiredAlertFormWidth(args);
			return Size.Width;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			viewInfo = CreateViewInfo();
			buttonElements = CreateButtons();
			viewInfo.Calculate();
			painter = CreatePainter();
			painter.UpdateRegion();
			if(control != null)
				control.RaiseFormLoad(this);
			StartAnimation();
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(this);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(DesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(Info.Image == null || info == null) return;
			if(!info.IsFinalFrame) {
				Info.Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				this.Invalidate(animItem.AnimationBounds);
			} else {
				StopAnimation();
				StartAnimation();
			}
		}
		internal void UpdateButtons() {
			if(buttonElements != null) {
				viewInfo.Calculate();
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(!IsHandleCreated)
				return;
			if(!customLocation.IsEmpty)
				currentScreen = CalcCurrentScreen(customLocation);
			else
				currentScreen = CalcCurrentScreen(Location);
			if(!LockLocationUpdate && !styleChanged)
				UpdateLocation();
		}
		protected internal void UpdateLocation() {
			if(Bounds.Right > currentScreen.WorkingArea.Right || Bounds.Bottom > currentScreen.WorkingArea.Bottom)
				this.Location = CalcAlertFromLocation(true);
			else
				this.Location = CalcAlertFromLocation();
		}
		protected internal override bool LockLocationUpdate {
			get {
				if(FormShowingEffect == AlertFormShowingEffect.FadeIn) return false;
				return alertTimer != null && (alertTimer.State == AlertTimerState.Hide || alertTimer.State == AlertTimerState.Start);
			}
		}
		protected virtual void CreatePopupMenuPutton(AlertButtonCollection collection) { }
		protected virtual AlertButtonCollection CreateButtons() {
			AlertButtonCollection ret = new AlertButtonCollection();
			if(showCloseButton)
				ret.Add(new AlertCloseButton(this, ret));
			if(showPinButton)
				ret.Add(new AlertPinButton(this, ret));
			CreatePopupMenuPutton(ret);
			if(control != null)
				foreach(AlertButton item in control.Buttons) {
					AlertButton button = new AlertButton();
					button.AssignFrom(item, control.Images);
					button.SetOwner(this);
					ret.Add(button);
				}
			return ret;
		}
		protected virtual AlertViewInfo CreateViewInfo() {
			if(control != null && control.AutoHeight)
				return new AlertAutoHeightViewInfo(this);
			else
				return new AlertViewInfo(this);
		}
		protected virtual AlertPainter CreatePainter() {
			return new AlertPainter(this);
		}
		internal AlertPainter Painter { get { return painter; } }
		[Browsable(false)]
		public IAlertControl AlertControl { get { return control; } }
		[Browsable(false)]
		public double OpacityLevel {
			get { return DefaultOpacity; }
			set {
				if(DefaultOpacity == value) return;
				if(value < 0.1) value = 0.1;
				if(value > 0.99) value = 0.99;
				DefaultOpacity = value;
			}
		}
		[Browsable(false)]
		public int StickBorderLength {
			get { return DefaultStickLength; }
			set {
				if(DefaultStickLength == value) return;
				if(value < 0) value = 0;
				DefaultStickLength = value;
			}
		}
		[Browsable(false)]
		public AlertInfo AlertInfo { get { return info; } }
		[Browsable(false)]
		public AlertButtonCollection Buttons { get { return buttonElements; } }
		void InitAlertControlProperties(IAlertControl control) {
			if(control == null) {
				this.allowHotTrack = false;
				return;
			}
			this.ControlBoxPosition = control.ControlBoxPosition;
			this.showToolTips = control.ShowToolTips;
			this.showCloseButton = control.ShowCloseButton;
			this.showPinButton = control.ShowPinButton;
			this.formLocation = control.FormLocation;
			this.formDisplaySpeed = control.FormDisplaySpeed;
			this.formShowingEffect = control.FormShowingEffect;
			this.autoFormDelay = control.AutoFormDelay;
			this.allowHtmlText = control.AllowHtmlText;
			this.allowHotTrack = control.AllowHotTrack;
		}
		protected virtual Screen CalcCurrentScreen(Point location) {
			return Screen.FromPoint(location);
		}
		protected virtual new Size DefaultSize { get { return new Size(250, 100); } }
		public void ShowForm(Form owner, Point location) {
			this.ownerForm = owner;
			if(location != Point.Empty)
				customLocation = location;
			int formWidth = GetDesiredFormWidth();
			if(formWidth != -1)
				Width = formWidth;
			CreateAlertTimer();
			this.Show();
			UpdateToolTipPosition();
			SubscribeChildControls();
		}
		public void ShowForm(Form owner) {
			ShowForm(owner, Point.Empty);
		}
		private void CreateAlertTimer() {
			if(alertTimer != null) return;
			alertTimer = new AlertTimer(this);
			alertTimer.State = AlertTimerState.Start;
		}
		public new void Close() { CloseForm(); }
		internal void CloseForm() {
			if(alertTimer == null) return;
			CustomClosing = true;
			alertTimer.State = AlertTimerState.Hide;
		}
		#region SubscribeChildControls
		void SubscribeChildControls() {
			foreach(Control ctrl in this.Controls) {
				ctrl.MouseEnter += new EventHandler(ctrl_MouseEnter);
				ctrl.MouseLeave += new EventHandler(ctrl_MouseLeave);
			}
		}
		void UnsubscribeChildControls() {
			foreach(Control ctrl in this.Controls) {
				ctrl.MouseEnter -= new EventHandler(ctrl_MouseEnter);
				ctrl.MouseLeave -= new EventHandler(ctrl_MouseLeave);
			}
		}
		void ctrl_MouseLeave(object sender, EventArgs e) {
			MouseFormLeave();
		}
		void ctrl_MouseEnter(object sender, EventArgs e) {
			MouseFormEnter();
		}
		#endregion
		#region Locations
		protected virtual Point CalcAlertFromLocation() {
			return CalcAlertFromLocation(false);
		}
		protected virtual Point CalcAlertFromLocation(bool force) {
			if(customLocation != Point.Empty && !force) return customLocation;
			int y = GetDeltaHeight();
			if(y < currentScreen.WorkingArea.Top || (y + this.Height) > currentScreen.WorkingArea.Bottom) y = currentScreen.WorkingArea.Top;
			if(y == currentScreen.WorkingArea.Top && !this.IsTopForm)
				y = currentScreen.WorkingArea.Bottom - this.Height;
			int x = currentScreen.WorkingArea.Left;
			if(!this.IsLeftForm)
				x = currentScreen.WorkingArea.Right - this.Width;
			return new Point(x, y);
		}
		protected virtual int GetDeltaHeight() {
			int ret = currentScreen.WorkingArea.Top;
			for(int i = Application.OpenForms.Count - 1; i >= 0; i--) {
				AlertFormCore aForm = Application.OpenForms[i] as AlertFormCore;
				if(aForm == null || aForm.IsMoved || aForm == this) continue;
				if(aForm.FormLocation != this.FormLocation) continue;
				if(!aForm.currentScreen.Equals(this.currentScreen)) continue;
				if(aForm.LockLocationUpdate) {
					if(aForm.alertTimer.State == AlertTimerState.Start) aForm.alertTimer.SetLiveState();
				}
				if(aForm.IsTopForm)
					ret = aForm.Bounds.Bottom;
				else
					ret = aForm.Bounds.Top - this.Height;
				break;
			}
			return ret;
		}
		#endregion
		#region Link
		bool linking = false;
		internal bool Linking {
			get { return linking; }
			set {
				if(linking == value) return;
				linking = value;
				if(linking)
					this.Cursor = Cursors.Hand;
				else this.Cursor = Cursors.Default;
				this.Invalidate(ViewInfo.MaxTextRectangle);
			}
		}
		private void CalcLinking(Point point) {
			if(ViewInfo == null || formMoving || !allowHotTrack) return;
			Linking = ViewInfo.TextRectangle.Contains(point);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			if(Linking) {
				bool activateOwner = true;
				if(AlertControl != null) {
					activateOwner = AlertControl.RaiseAlertClick(AlertInfo, this);
				}
				if(this.OwnerForm != null && activateOwner) OwnerForm.Activate();
			}
		}
		#endregion
		#region From Moving
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			bool isButton = buttonElements.OnMouseDown(e);
			if(Linking) return;
			if(e.Button == MouseButtons.Left && !isButton) {
				StartMoving(e.X, e.Y);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			buttonElements.OnMouseUp(e);
			if(e.Button == MouseButtons.Left) {
				StopMoving();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			CalcLinking(new Point(e.X, e.Y));
			buttonElements.OnMouseMove(e);
			MoveTo();
		}
		internal void StartMoving(int x, int y) {
			formMoving = true;
			MouseDownX = x;
			MouseDownY = y;
		}
		internal void StopMoving() {
			formMoving = false;
		}
		internal void MoveTo() {
			if(formMoving) {
				this.Location = new Point(Cursor.Position.X - MouseDownX, Cursor.Position.Y - MouseDownY);
				UpdateToolTipPosition();
				isMoved = true;
			}
		}
		void UpdateToolTipPosition() {
			if(this.Location.Y + this.Height == GetCurrentScreenArea().Bottom)
				toolTipController.ToolTipLocation = ToolTipLocation.TopRight;
			else toolTipController.ToolTipLocation = ToolTipLocation.Default;
		}
		#endregion
		#region Selected
		bool selected = false;
		bool pin = false;
		Timer selectedTimer = null;
		Timer SelectedTimer {
			get {
				if(selectedTimer == null) {
					selectedTimer = new Timer();
					selectedTimer.Interval = 20;
					selectedTimer.Tick += new EventHandler(OnSelectedTimerTick);
				}
				return selectedTimer;
			}
		}
		void OnSelectedTimerTick(object sender, EventArgs e) {
			if(Selected) {
				Opacity = 0.99;
				SelectedTimer.Stop();
				return;
			}
			this.Opacity -= 0.02;
			if(Opacity <= DefaultOpacity) {
				if(alertTimer.State != AlertTimerState.Hide) Opacity = DefaultOpacity;
				if(alertTimer.State == AlertTimerState.Live) alertTimer.Start();
				SelectedTimer.Stop();
			}
		}
		internal bool Selected {
			get { return selected; }
			set {
				if(alertTimer == null) return;
				if(value == false && Pin) return;
				if(value == selected) return;
				selected = value;
				if(selected) {
					if(alertTimer.State == AlertTimerState.Start) alertTimer.SetLiveState();
					if(alertTimer.State != AlertTimerState.Hide) this.Opacity = 0.99;
					if(alertTimer.State == AlertTimerState.Live) alertTimer.Stop();
				} else {
					SelectedTimer.Start();
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Pin {
			get { return pin; }
			set {
				if(value == pin) return;
				pin = value;
				if(pin && !selected)
					Selected = true;
				if(!pin && !this.Bounds.Contains(Cursor.Position)) {
					Selected = false;
				}
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			MouseFormLeave();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			MouseFormEnter();
		}
		void MouseFormLeave() {
			if(control != null)
				if(control.RaiseMouseFromLeave(new AlertFormEventArgs(this))) return;
			Linking = false;
			buttonElements.OnMouseLeave();
			Selected = false;
		}
		void MouseFormEnter() {
			if(control != null)
				if(control.RaiseMouseFromEnter(new AlertFormEventArgs(this))) return;
			Selected = true;
		}
		#endregion
		#region Internal Properties
		internal bool IsTopForm {
			get { return FormLocation == AlertFormLocation.TopLeft || FormLocation == AlertFormLocation.TopRight; }
		}
		internal bool IsLeftForm {
			get { return FormLocation == AlertFormLocation.TopLeft || FormLocation == AlertFormLocation.BottomLeft; }
		}
		internal bool IsMoved {
			get { return isMoved; }
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Form OwnerForm { get { return ownerForm; } }
		[Browsable(false)]
		public AlertFormLocation FormLocation {
			get { return formLocation; }
			set {
				if(formLocation == value) return;
				formLocation = value;
				this.Location = CalcAlertFromLocation();
			}
		}
		[Browsable(false)]
		public AlertFormShowingEffect FormShowingEffect {
			get { return formShowingEffect; }
			set {
				if(formShowingEffect == value) return;
				formShowingEffect = value;
			}
		}
		[Browsable(false)]
		public AlertFormDisplaySpeed FormDisplaySpeed {
			get { return formDisplaySpeed; }
			set {
				if(formDisplaySpeed == value) return;
				formDisplaySpeed = value;
			}
		}
		[Browsable(false)]
		public int AutoFormDelay {
			get { return autoFormDelay; }
			set {
				if(autoFormDelay == value) return;
				autoFormDelay = value;
			}
		}
		[Browsable(false)]
		public AlertFormControlBoxPosition ControlBoxPosition {
			get { return controlBoxPosition; }
			set {
				if(controlBoxPosition == value) return;
				controlBoxPosition = value;
			}
		}
		[Browsable(false)]
		public AlertViewInfo ViewInfo { get { return viewInfo; } }
		[Browsable(false)]
		public AlertInfo Info { get { return info; } }
		[Browsable(false)]
		internal AlertFormImagePosition ImagePosition {
			get { return imagePosition; }
			set {
				if(imagePosition == value) return;
				imagePosition = value;
			}
		}
		[Browsable(false)]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
			}
		}
		#endregion
		#region IToolTipControlClient Members
		public ToolTipControlInfo GetObjectInfo(Point point) {
			AlertButton item = buttonElements.GetButtonByPoint(point);
			if(item == null || item.Hint == string.Empty) return null;
			return new ToolTipControlInfo(item, item.Hint);
		}
		[Browsable(false)]
		public bool ShowToolTips {
			get { return showToolTips; }
		}
		#endregion
		protected override void OnPaint(PaintEventArgs e) {
			painter.Draw(e);
			base.OnPaint(e);
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Info != null ? Info.Image : null);
				return imageHelper;
			}
		}
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate { get { return ImageHelper.FramesCount > 1; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		#endregion
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds { get { return ViewInfo.ImageRectangle; } }
		int IAnimatedItem.AnimationInterval { get { return ImageHelper.AnimationInterval; } }
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		DevExpress.Utils.Drawing.Animation.AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) { return ImageHelper.GetAnimationInterval(frameIndex); }
		bool IAnimatedItem.IsAnimated { get { return ImageHelper.IsAnimated; } }
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner { get { return this; } }
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) { ImageHelper.UpdateAnimation(info); }
		#endregion
		internal int GetLeft() {
			return customLocation.IsEmpty ? Left : customLocation.X;
		}
		internal int GetTop() {
			return customLocation.IsEmpty ? Top : customLocation.Y;
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return true; }
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return true;
		}
	}
	public class AlertMessageViewInfo : AlertAutoHeightViewInfo {
		public AlertMessageViewInfo(AlertFormCore form)
			: base(form) {
		}
		protected override void PatchFormHeight() {
			base.PatchFormHeight();
			Owner.Height += topElementHeight;
		}
	}
	public class AlertFormMessage : AlertFormCore {
		Point messageLocation;
		public AlertFormMessage(Point location, IAlertControl control, AlertInfo info)
			: base(location, control, info) {
			messageLocation = location;
		}
		protected override Size DefaultSize {
			get {
				return new Size(300, 70);
			}
		}
		protected override AlertViewInfo CreateViewInfo() {
			return new AlertMessageViewInfo(this);
		}
		protected override Point CalcAlertFromLocation(bool force) {
			Point ret = messageLocation;
			ret.Y -= Size.Height;
			if(ret.X + Size.Width > currentScreen.WorkingArea.Right)
				ret.X = currentScreen.WorkingArea.Right - Size.Width;
			if(ret.X < currentScreen.WorkingArea.Left) ret.X = currentScreen.WorkingArea.Left;
			if(ret.Y < currentScreen.WorkingArea.Top)
				ret.Y = currentScreen.WorkingArea.Top;
			if(ret.Y > currentScreen.WorkingArea.Bottom) ret.Y = currentScreen.WorkingArea.Bottom - Size.Height;
			return ret;
		}
	}
}
