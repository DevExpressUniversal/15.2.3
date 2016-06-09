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
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraEditors.ButtonPanel {
	public class BaseButtonInfo : ObjectInfoArgs {
		Image imageCore;
		Rectangle imageBoundsCore;
		Rectangle textBoundsCore;
		public BaseButtonInfo(IBaseButton button, bool verticalRotated = false) {
			Button = button;
			PaintAppearance = new FrozenAppearance();
			Row = 0;
			Column = 0;
			verticalRotatedElementCore = verticalRotated;
		}
		public int Row { get; set; }
		public int Column { get; set; }
		public IBaseButton Button { get; private set; }
		public AppearanceObject PaintAppearance { get; private set; }
		public string Caption { get { return Button.Properties.Caption; } }		
		public Image Image { get { return imageCore; } }
		public Rectangle Content { get; protected set; }
		public Rectangle ImageBounds {
			get { return imageBoundsCore; }
			protected set { imageBoundsCore = value; }
		}
		public Rectangle TextBounds {
			get { return textBoundsCore; }
			protected set { textBoundsCore = value; }
		}
		public bool HasImage { get { return imageCore != null; } }
		public bool HasCaption { get { return CheckCaption(Button.Properties); } }
		protected virtual void UpdateActualImage(BaseButtonPainter painter) {
			Button.Properties.BeginUpdate();
			imageCore = GetImage(painter, Button);
			Button.Properties.CancelUpdate();
		}
		bool verticalRotatedElementCore;
		protected internal virtual bool VerticalRotatedElement { get { return verticalRotatedElementCore; } }
		protected internal virtual bool IsVertical {
			get { return VerticalRotatedElement && ButtonOwner != null && !ButtonOwner.IsHorizontal; }
		}
		protected virtual Size GetActualSize(Size size) {
			if(IsVertical)
				return new Size(size.Height, size.Width);
			return size;
		}
		public virtual Size CalcMinSize(Graphics g, BaseButtonPainter painter) {
			if(!Button.Properties.Visible)
				return Size.Empty;
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size textSize = GetTextSize(g, hasText);
			Size imageSize = GetImageSize(hasImage);
			int interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : GetImageToTextInterval(painter);
			Size minSize = GetMinSize(painter, hasText, hasImage);
			Size contentSize = GetContentSize(textSize, imageSize, interval, minSize);
			Rectangle client = new Rectangle(Point.Empty, contentSize);
			return painter.CalcBoundsByClientRectangle(this, client).Size;
		}
		public virtual void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal) {
			Calc(g, painter, offset, maxRect, isHorizontal, false);
		}
		protected virtual int GetImageToTextInterval(BaseButtonPainter painter) {
			return painter.ImageToTextInterval;
		}
		public virtual void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
			TextBounds = ImageBounds = Content = Rectangle.Empty;
			if(!Button.Properties.Visible) return;
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size textSize = GetTextSize(g, hasText);
			Size imageSize = GetImageSize(hasImage);
			int interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : GetImageToTextInterval(painter);
			Size minSize = GetMinSize(painter, hasText, hasImage);
			Size contentSize = GetContentSize(textSize, imageSize, interval, minSize);
			Bounds = maxRect;
			if(isHorizontal)
				contentSize.Height = painter.GetObjectClientRectangle(this).Height;
			else
				contentSize.Width = painter.GetObjectClientRectangle(this).Width;
			Rectangle r = painter.CalcBoundsByClientRectangle(this, new Rectangle(Point.Empty, contentSize));
			if(isHorizontal && !calcIsLeft && !(ButtonOwner != null && ButtonOwner.RightToLeft)) offset.X -= r.Size.Width;
			Bounds = new Rectangle(offset, r.Size);
			Content = painter.GetObjectClientRectangle(this);
			CalcImageAndTextBounds(Content.Location, textSize, imageSize, interval, Content);
		}
		protected virtual Size GetMinSize(BaseButtonPainter painter, bool hasText, bool hasImage) {
			return painter.CalcObjectMinBounds(this).Size;
		}
		public virtual void UpdatePaintAppearance(BaseButtonPainter painter) {
			AppearanceHelper.Combine(PaintAppearance,
				new AppearanceObject[] { Button.Properties.Appearance }, painter.DefaultAppearance);
		}
		protected virtual Size GetImageSize(bool hasImage) {
			Size imageSize = hasImage ? Image.Size : Size.Empty;
			return GetActualSize(imageSize);
		}
		protected internal virtual AppearanceObject GetStateAppearance() {
			return GetStateAppearance(State);
		}
		protected internal virtual AppearanceObject GetStateAppearance(ObjectState state) {
			if(ButtonPanelOwner == null) return null;
			if((state & ObjectState.Pressed) != 0)
				return ButtonPanelOwner.AppearanceButton.Pressed;
			if((state & ObjectState.Hot) != 0)
				return ButtonPanelOwner.AppearanceButton.Hovered;
			return ButtonPanelOwner.AppearanceButton.Normal;
		}
		protected internal IButtonsPanel ButtonOwner {
			get {
				if(Button is BaseButton)
					return (Button as BaseButton).GetOwner();
				return null;
			}
		}
		protected internal Control ParentControl { get { return ButtonPanelOwner is Control ? (Control)ButtonPanelOwner : null; } }
		public IButtonsPanelOwner ButtonPanelOwner { get { return GetButtonPanelOwner(); } }
		protected virtual IButtonsPanelOwner GetButtonPanelOwner() {
			BaseButton baseButton = Button as BaseButton;
			if(baseButton == null || baseButton.GetOwner() == null) return null;
			return baseButton.GetOwner().Owner as IButtonsPanelOwner;
		}
		protected internal IButtonsPanelGlyphSkinningOwner ButtonPanelGlyphSkinningOwner {
			get {
				BaseButton baseButton = Button as BaseButton;
				if(baseButton == null || baseButton.GetOwner() == null) return null;
				return baseButton.GetOwner().Owner as IButtonsPanelGlyphSkinningOwner;
			}
		}
		protected virtual Size GetTextSize(Graphics g, bool hasText) {
			if(!hasText) return Size.Empty;
			Size result = Size.Empty;
			using(GraphicsCache cache = new GraphicsCache(g)) {
				if(ButtonPanelOwner == null || ButtonPanelOwner.AppearanceButton == null) return CalcButtonTextSize(cache, PaintAppearance);
				CalcAppearances();
				foreach(AppearanceObject app in AppearanceList) {
					Size textSize = CalcButtonTextSize(cache, app);
					result.Width = Math.Max(textSize.Width, result.Width);
					result.Height = Math.Max(textSize.Height, result.Height);
				}
			}
			return result;
		}
		protected virtual Size CalcButtonTextSize(GraphicsCache cache, AppearanceObject appearance) {
			DevExpress.Utils.Text.StringInfo info = null;
			if(ButtonPanelOwner != null && ButtonPanelOwner.AllowHtmlDraw)
				info = DevExpress.Utils.Text.StringPainter.Default.Calculate(cache.Graphics, appearance, appearance.TextOptions, Caption, Rectangle.Empty, cache.Paint, this);
			Size textSize = info != null ? info.Bounds.Size : Size.Round(appearance.CalcTextSize(cache, Caption, 0));
			return GetActualSize(textSize);
		}
		protected void CalcAppearances() {
			if(AppearanceList == null) AppearanceList = new System.Collections.Generic.List<AppearanceObject>();
			SetAppearance(AppearanceList, PaintAppearance);
			SetAppearance(AppearanceList, GetStateAppearance(ObjectState.Normal));
			SetAppearance(AppearanceList, GetStateAppearance(ObjectState.Hot));
			SetAppearance(AppearanceList, GetStateAppearance(ObjectState.Pressed));
		}
		System.Collections.Generic.List<AppearanceObject> appearanceListCore;
		protected internal System.Collections.Generic.List<AppearanceObject> AppearanceList {
			get { return appearanceListCore; }
			set { appearanceListCore = value; }
		}
		protected void SetAppearance(System.Collections.Generic.List<AppearanceObject> list, AppearanceObject app) {
			list.Add(app);
		}
		protected virtual ImageLocation GetActualImageLocation(ImageLocation imageLocation) {
			if(ButtonOwner != null){ 
				if(ButtonOwner.RightToLeft){
					if(imageLocation == ImageLocation.AfterText)
						return ImageLocation.BeforeText;
					if(imageLocation == ImageLocation.BeforeText || imageLocation == ImageLocation.Default)
						return ImageLocation.AfterText;
				}
				if(ButtonOwner.ButtonRotationAngle == RotationAngle.Rotate90){
					if(imageLocation == ImageLocation.AboveText)
						return ImageLocation.BelowText;
					if(imageLocation == ImageLocation.BelowText)
						return ImageLocation.AboveText;
				}
			}
			return imageLocation;
		}
		protected virtual void CalcImageAndTextBounds(Point offset, Size textSize, Size imageSize, int interval, Rectangle content) {
			imageBoundsCore.Size = imageSize;
			textBoundsCore.Size = textSize;
			ImageLocation imageLocation = GetActualImageLocation(Button.Properties.ImageLocation);
			switch(imageLocation) {
				case ImageLocation.Default:
					CalcDefault(ref imageBoundsCore, ref textBoundsCore, offset, interval, content);
					break;
				case ImageLocation.BeforeText:
					CalcVertical(ref imageBoundsCore, ref textBoundsCore, offset, interval, content);
					break;
				case ImageLocation.AfterText:
					CalcVertical(ref textBoundsCore, ref imageBoundsCore, offset, interval, content);
					break;
				case ImageLocation.AboveText:
					CalcHorizontal(ref imageBoundsCore, ref textBoundsCore, offset, interval, content);
					break;
				case ImageLocation.BelowText:
					CalcHorizontal(ref textBoundsCore, ref imageBoundsCore, offset, interval, content);
					break;
			}
		}
		protected virtual void CalcDefault(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			CalcVertical(ref first, ref second, offset, interval, content);
		}
		protected void CalcVertical(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			if(IsVertical) {
				first.Location = new Point(offset.X + (content.Width - first.Width) / 2, offset.Y);
				second.Location = new Point(offset.X + (content.Width - second.Width) / 2, offset.Y + first.Height + interval);
			}
			else {
				first.Location = new Point(offset.X, offset.Y + (content.Height - first.Height) / 2);
				second.Location = new Point(offset.X + first.Width + interval, offset.Y + (content.Height - second.Height) / 2);
			}
		}
		protected void CalcHorizontal(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			if(IsVertical) {
				first.Location = new Point(offset.X, offset.Y + (content.Height - first.Height) / 2);
				second.Location = new Point(offset.X + first.Width + interval, offset.Y + (content.Height - second.Height) / 2);
			}
			else {
				first.Location = new Point(offset.X + (content.Width - first.Width) / 2, offset.Y);
				second.Location = new Point(offset.X + (content.Width - second.Width) / 2, offset.Y + first.Height + interval);
			}
		}
		protected void CalcCenter(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			first.Location = new Point(offset.X + (content.Width - first.Width) / 2, offset.Y + (content.Height - first.Height - second.Height) / 2);
			second.Location = new Point(offset.X + (content.Width - second.Width) / 2, offset.Y + (first.Bottom - offset.Y) + interval);
		}
		protected virtual Size GetContentSize(Size textSize, Size imageSize, int interval, Size minSize) {
			bool isSide = IsSideImageLocation(Button.Properties.ImageLocation);
			int width, height;
			if(IsVertical) {
				width = isSide ? Math.Max(textSize.Width, imageSize.Width) : textSize.Width + imageSize.Width + interval;
				height = isSide ? textSize.Height + imageSize.Height + interval : Math.Max(textSize.Height, imageSize.Height); 
			}
			else {
				width = isSide ? textSize.Width + imageSize.Width + interval : Math.Max(textSize.Width, imageSize.Width);
				height = isSide ? Math.Max(textSize.Height, imageSize.Height) : textSize.Height + imageSize.Height + interval;
			}
			return new Size(Math.Max(width, minSize.Width), Math.Max(height, minSize.Height));
		}
		public virtual bool Disabled {
			get { return GetState(ObjectState.Disabled); }
			set { SetState(ObjectState.Disabled, value); }
		}
		public virtual bool Hot {
			get { return GetState(ObjectState.Hot); }
			set { SetState(ObjectState.Hot, value); }
		}
		public virtual bool Pressed {
			get { return GetState(ObjectState.Pressed); }
			set { SetState(ObjectState.Pressed, value); }
		}
		public virtual bool Selected {
			get { return GetState(ObjectState.Selected); }
			set { SetState(ObjectState.Selected, value); }
		}
		bool GetState(ObjectState state) {
			return (State & state) != 0;
		}
		void SetState(ObjectState state, bool value) {
			if(value)
				State |= state;
			else
				State &= ~state;
		}
		protected virtual bool IsSideImageLocation(ImageLocation location) {
			return location == ImageLocation.Default || location == ImageLocation.AfterText || location == ImageLocation.BeforeText;
		}
		protected bool CheckImage(IButtonProperties properties) {
			return properties.UseImage && HasImage;
		}
		protected bool CheckCaption(IButtonProperties properties) {
			return properties.UseCaption && !string.IsNullOrEmpty(properties.Caption);
		}
		Image GetImage(IButtonProperties properties) {
			if(properties.Glyphs != null)
				return ImageCollection.GetImageListImage(properties.Glyphs, 0);
			return GetImageByUri() ?? properties.Image ?? ImageCollection.GetImageListImage(properties.Images, properties.ImageIndex);
		}
		internal Image GetImageByUri() {
			if(Button is IDxImageUriProvider)
				return GetImageByUriCore((Button as IDxImageUriProvider).ImageUri);
			return null;
		}
		protected virtual Image GetImageByUriCore(DxImageUri imageUri){
			if(imageUri.HasDefaultImage)
				return imageUri.GetDefaultImage();
			if(imageUri.HasLargeImage)
				return imageUri.GetLargeImage();
			return imageUri.GetImage();
		}
		Image GetImage(BaseButtonPainter painter, IBaseButton Button) {
			BaseButtonPainter buttonPainter = painter.GetButtonPainter(Button.GetType());
			if(Button.Properties.Glyphs == null)
				Button.Properties.Glyphs = buttonPainter.GetGlyphs(Button);
			return GetImage(Button.Properties);
		}
		protected virtual internal bool GetAllowGlyphSkinning() {
			return !(Button is IDefaultButton) && (ButtonPanelOwner != null) && ButtonPanelOwner.AllowGlyphSkinning && !Disabled;
		}
		protected virtual internal Color GetGlyphSkinningColor() {
			if(Button != null && !Button.Properties.Appearance.ForeColor.IsEmpty)
				return Button.Properties.Appearance.ForeColor;
			return (ButtonPanelGlyphSkinningOwner != null) ?
				ButtonPanelGlyphSkinningOwner.GetGlyphSkinningColor(this) : PaintAppearance.GetForeColor();
		}
		protected virtual internal bool GetAllowHtmlDraw() {
			return (ButtonPanelOwner != null) && ButtonPanelOwner.AllowHtmlDraw;
		}
	}
	public class LayoutViewCardExpandButtonViewInfo : BaseButtonInfo {
		public LayoutViewCardExpandButtonViewInfo(IBaseButton button)
			: base(button,true) {
		}
		public override ObjectState State {
			get {
				var button = Button as ButtonsPanelControl.LayoutViewCardExpandButton;
				if(button != null && button.GroupObjectInfoArgs != null)
					return button.GroupObjectInfoArgs.ButtonState;
				return ObjectState.Normal;
			}
			set { }
		}
	}
	public class RibbonPageGroupButtonInfo : BaseButtonInfo {
		public RibbonPageGroupButtonInfo(IBaseButton button)
			: base(button) {
		}
		public override ObjectState State {
			get {
				var button = Button as ButtonsPanelControl.RibbonPageGroupButton;
				if(button != null && button.GroupObjectInfoArgs != null)
					return button.GroupObjectInfoArgs.ButtonState;
				return ObjectState.Normal;
			}
			set { }
		}
		public override void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
			painter = painter.GetButtonPainter(Button.GetType());
			TextBounds = ImageBounds = Content = Rectangle.Empty;
			if(!Button.Properties.Visible) return;
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size textSize = GetTextSize(g, hasText);
			Size imageSize = GetImageSize(hasImage);
			int interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : GetImageToTextInterval(painter);
			Size minSize = GetMinSize(painter, hasText, hasImage);
			Size contentSize = GetContentSize(textSize, imageSize, interval, minSize);
			Bounds = maxRect;
			if(isHorizontal)
				contentSize.Height = painter.GetObjectClientRectangle(this).Height;
			else
				contentSize.Width = painter.GetObjectClientRectangle(this).Width;
			Rectangle r = painter.CalcBoundsByClientRectangle(this, new Rectangle(Point.Empty, contentSize));
			if(isHorizontal && !calcIsLeft) offset.X -= r.Size.Width;
			Bounds = new Rectangle(offset, r.Size);
			Content = painter.GetObjectClientRectangle(this);
			CalcImageAndTextBounds(Content.Location, textSize, imageSize, interval, Content);
		}
		public override Size CalcMinSize(Graphics g, BaseButtonPainter painter) {
			painter = painter.GetButtonPainter(Button.GetType());
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size textSize = GetTextSize(g, hasText);
			Size imageSize = GetImageSize(hasImage);
			int interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : GetImageToTextInterval(painter);
			Size minSize = GetMinSize(painter, hasText, hasImage);
			Size contentSize = GetContentSize(textSize, imageSize, interval, minSize);
			Rectangle client = new Rectangle(Point.Empty, contentSize);
			return painter.CalcBoundsByClientRectangle(this, client).Size;
		}
	}
	public class BaseSeparatorInfo : BaseButtonInfo {
		public BaseSeparatorInfo(IBaseButton separator)
			: base(separator) {
		}
		public override ObjectState State {
			get { return ObjectState.Normal; }
			set { }
		}
		public override void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
			painter = painter.GetButtonPainter(Button.GetType());
			ImageBounds = Content = Rectangle.Empty;
			UpdateActualImage(painter);
			bool hasImage = CheckImage(Button.Properties);
			Size imageSize = GetImageSize(hasImage);
			if(!isHorizontal) imageSize = new Size(imageSize.Height, imageSize.Width);
			Size minSize = GetMinSize(painter, false, hasImage);
			Bounds = maxRect;
			if(isHorizontal)
				imageSize.Height = painter.GetObjectClientRectangle(this).Height;
			else
				imageSize.Width = painter.GetObjectClientRectangle(this).Width;
			Rectangle r = painter.CalcBoundsByClientRectangle(this, new Rectangle(Point.Empty, imageSize));
			if(isHorizontal && !calcIsLeft) offset.X -= r.Size.Width;
			Bounds = new Rectangle(offset, r.Size);
			Content = painter.GetObjectClientRectangle(this);
			ImageBounds = Content;
		}
		public override Size CalcMinSize(Graphics g, BaseButtonPainter painter) {
			painter = painter.GetButtonPainter(Button.GetType());
			UpdateActualImage(painter);
			bool hasImage = CheckImage(Button.Properties);
			Size imageSize = GetImageSize(hasImage);
			if(((BaseButton)Button).GetOwner().Orientation == Orientation.Vertical)
				imageSize = new Size(1, imageSize.Width);
			else
				imageSize = new Size(imageSize.Width, 1);
			Rectangle client = new Rectangle(Point.Empty, imageSize);
			Size resultSize = painter.CalcBoundsByClientRectangle(this, client).Size;
			return resultSize;
		}
	}
	public class BaseButtonsPanelViewInfo : ObjectInfoArgs, IButtonsPanelViewInfo {
		IList<BaseButtonInfo> buttonsCore;
		public BaseButtonsPanelViewInfo(IButtonsPanel panel) {
			Panel = panel;
			PaintAppearance = new FrozenAppearance();
			PaintAppearance.BackColor = Color.Black;
		}
		public bool IsReady { get { return buttonsCore != null; } }
		public Size MinSize { get; set; }
		internal Rectangle InnerContent { get; set; }
		public IButtonsPanel Panel { get; private set; }
		public AppearanceObject PaintAppearance { get; private set; }
		public IList<BaseButtonInfo> Buttons {
			get { return buttonsCore; }
			protected set { buttonsCore = value; }
		}
		public Rectangle Content { get; protected set; }
		public BaseButtonInfo CalcHitInfo(Point point) {
			if(IsReady) {
				foreach(BaseButtonInfo buttonInfo in Buttons) {
					if(buttonInfo.Bounds.Contains(point) && !buttonInfo.Disabled)
						return buttonInfo;
				}
			}
			return null;
		}
		public void SetDirty() {
			buttonsCore = null;
		}
		public virtual Size CalcMinSize(Graphics g) {
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			if(IsReady && !Content.Size.IsEmpty)
				return painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, Content.Size)).Size;
			Size result = new Size(0, 0);
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			int visibleButtons = 0;
			foreach(IBaseButton button in Panel.Buttons) {
				BaseButtonInfo buttonInfo = CreateButtonInfo(button);
				Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
				visibleButtons += buttonSize.Width != 0 ? 1 : 0;
				result.Width = Panel.IsHorizontal ? result.Width + buttonSize.Width : Math.Max(buttonSize.Width, result.Width);
				result.Height = Panel.IsHorizontal ? Math.Max(buttonSize.Height, result.Height) : result.Height + buttonSize.Height;
			}
			if(Panel.IsHorizontal)
				result.Width += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			else
				result.Height += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			MinSize = painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, result)).Size;
			return MinSize;
		}
		public virtual void Calc(Graphics g, Rectangle bounds) {
			if(IsReady) return;
			Bounds = bounds;
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			Rectangle client = painter.GetObjectClientRectangle(this);
			buttonsCore = EnsureButtons(g, client, buttonPainter);
			int interval = Panel.ButtonInterval;
			bool horz = Panel.IsHorizontal;
			Stack<BaseButtonInfo> leftButtons = new Stack<BaseButtonInfo>();
			List<BaseButtonInfo> buttons = new List<BaseButtonInfo>();
			bool hasWrap = false;
			foreach(BaseButtonInfo bInfo in Buttons) {
				BaseButton button = bInfo.Button as BaseButton;
				if(horz && button != null && button.IsLeft)
					leftButtons.Push(bInfo);
				else buttons.Add(bInfo);
				hasWrap |= bInfo.Column > 0 || bInfo.Row > 0;
			}
			Point offset = Content.Location;
			if(Panel.RightToLeft) {
				if(horz)
					offset.X = Content.Right;
				else
					offset.Y = Content.Bottom;
			}
			if(hasWrap) {
				offset = CalcButtonInfos(g, buttonPainter, interval, horz, Buttons, offset);
			}
			else {
				while(leftButtons.Count > 0) {
					BaseButtonInfo bInfo = leftButtons.Pop();
					bInfo.Calc(g, buttonPainter, offset, Content, horz, true);
					offset.X += (Panel.RightToLeft ? -1 : 1) * (bInfo.Bounds.Width + interval);
				}
				offset = GetPointOffset(horz, offset, InnerContent);
				CalcButtonInfos(g, buttonPainter, interval, horz, buttons, offset);
			}
			Bounds = painter.CalcBoundsByClientRectangle(this, Content);
			Panel.BeginUpdate();
			Panel.Bounds = Bounds;
			Panel.CancelUpdate();
		}
		protected virtual Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			foreach(BaseButtonInfo buttonInfo in buttons) {
				buttonInfo.Calc(g, buttonPainter, offset, Content, horz);
				if(horz)
					offset.X -= (Panel.RightToLeft ? -1 : 1) * (buttonInfo.Bounds.Width + interval);
				else
					offset.Y += (Panel.RightToLeft ? -1 : 1) * (buttonInfo.Bounds.Height + interval);
			}
			return offset;
		}
		protected virtual Point GetPointOffset(bool horz, Point offset, Rectangle innerContent) {
			offset = Content.Location;
			if(horz && !Panel.RightToLeft)
				offset.X += Content.Width;
			return offset;
		}
		protected virtual IBaseButton[] SortButtonList(BaseButtonCollection buttons) {
			List<IBaseButton> positiveList = new List<IBaseButton>();
			List<IBaseButton> negativeList = new List<IBaseButton>();
			foreach(IBaseButton button in buttons) {
				if(button.Properties.VisibleIndex < 0)
					negativeList.Add(button);
				else
					positiveList.Add(button);
			}
			SortButtonListCore(positiveList);
			positiveList.AddRange(negativeList);
			return positiveList.ToArray();
		}
		protected virtual void SortButtonListCore(List<IBaseButton> positiveList) {
			positiveList.Sort(PositiveCompare);
			positiveList.Reverse();
		}
		protected static int PositiveCompare(IBaseButton x, IBaseButton y) {
			if(x == y) return 0;
			return x.Properties.VisibleIndex.CompareTo(y.Properties.VisibleIndex);
		}
		protected IList<BaseButtonInfo> EnsureButtons(Graphics g, Rectangle bounds, BaseButtonPainter buttonPainter) {
			bool horz = Panel.IsHorizontal;
			int width = 0, height = 0;
			IList<BaseButtonInfo> buttonInfos = new List<BaseButtonInfo>();
			IBaseButton[] buttons = SortButtonList(Panel.Buttons);
			CalcButtonsCore(g, buttonPainter, horz, bounds, ref width, ref height, buttonInfos, buttons);
			InnerContent = PlacementHelper.Arrange(new Size(width, height), bounds, Panel.ContentAlignment);
			width = horz && HasLeftButtons(buttonInfos) ? bounds.Width : width;
			Content = PlacementHelper.Arrange(new Size(width, height), bounds, Panel.ContentAlignment);
			return buttonInfos;
		}
		bool HasLeftButtons(IEnumerable<BaseButtonInfo> buttonInfos) {
			foreach(BaseButtonInfo bInfo in buttonInfos) {
				BaseButton button = bInfo.Button as BaseButton;
				if(button != null && button.IsLeft)
					return true;
			}
			return false;
		}
		protected virtual void CalcButtonsCore(Graphics g, BaseButtonPainter buttonPainter, bool horz, Rectangle maxRect, ref int width, ref int height, IList<BaseButtonInfo> buttonInfos, IBaseButton[] buttons) {
			List<IBaseButton> checkedButtons = new List<IBaseButton>();
			List<IBaseButton> simpleButtons = new List<IBaseButton>();
			int maxSize = horz ? maxRect.Width : maxRect.Height;
			foreach(IBaseButton button in buttons) {
				if(!button.Properties.Visible) continue;
				if(button.IsChecked.HasValue && button.IsChecked.Value && button.Properties.GroupIndex != -1)
					checkedButtons.Add(button);
				else
					simpleButtons.Add(button);
			}
			bool allCheckedButtonVisible = false;
			int tempMinSize = maxSize;
			Size buttonPanelMinSize = CalcMinSize(g);
			while(!allCheckedButtonVisible) {
				allCheckedButtonVisible = false;
				width = 0;
				height = 0;
				maxSize = tempMinSize;
				int tempWidth = 0;
				int tempRow = 0;
				int tempHeight = 0;
				int tempColumn = 0;
				buttonInfos.Clear();
				for(int i = 0; i < buttons.Length; i++) {
					IBaseButton button = buttons[i];
					if(!simpleButtons.Contains(button) && !checkedButtons.Contains(button)) continue;
					BaseButtonInfo buttonInfo = CreateButtonInfo(button);
					Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
					if(buttonSize.Height == 0 || buttonSize.Width == 0)
						continue;
					int interval = (buttonInfos.Count == 0) ? 0 : Panel.ButtonInterval;
					if(horz) {
						maxSize -= (buttonSize.Width + interval);
						if(maxSize >= 0) {
							width += (buttonSize.Width + interval);
							buttonInfo.Row = tempRow;
							buttonInfos.Add(buttonInfo);
						}
						else {
							if((maxRect.Height - (height + Panel.ButtonInterval)) > buttonPanelMinSize.Height && Panel.WrapButtons) {
								if(maxRect.Width - buttonSize.Width < 0) continue;
								maxSize = maxRect.Width - buttonSize.Width;
								height += buttonPanelMinSize.Height + interval;
								tempWidth = Math.Max(tempWidth, width);
								width = buttonSize.Width;
								buttonInfo.Row = (height - tempRow * Panel.ButtonInterval) / buttonPanelMinSize.Height - 1;
								tempRow = buttonInfo.Row;
								buttonInfos.Add(buttonInfo);
							}
							else {
								simpleButtons.Remove(button);
								maxSize += (buttonSize.Width + interval);
							}
						}
						height = Math.Max(buttonPanelMinSize.Height, height);
					}
					else {
						maxSize -= (buttonSize.Height + interval);
						if(maxSize >= 0) {
							height += (buttonSize.Height + interval);
							buttonInfo.Column = tempColumn;
							buttonInfos.Add(buttonInfo);
						}
						else {
							if((maxRect.Width - (width + Panel.ButtonInterval)) > buttonPanelMinSize.Width && Panel.WrapButtons) {
								if(maxRect.Height - (buttonSize.Height) < 0) continue;
								maxSize = maxRect.Height - buttonSize.Height;
								width += buttonPanelMinSize.Width + interval;
								tempHeight = Math.Max(tempHeight, height);
								height = buttonSize.Height;
								buttonInfo.Column = (width - tempColumn * Panel.ButtonInterval) / buttonPanelMinSize.Width - 1;
								tempColumn = buttonInfo.Column;
								buttonInfos.Add(buttonInfo);
							}
							else {
								simpleButtons.Remove(button);
								maxSize += (buttonSize.Height + interval);
							}
						}
						width = Math.Max(buttonPanelMinSize.Width, width);
					}
				}
				width = Math.Max(width, tempWidth);
				height = Math.Max(height, tempHeight);
				allCheckedButtonVisible = CheckGroup(buttonInfos, checkedButtons);
				if(!allCheckedButtonVisible) {
					if(simpleButtons.Count != 0)
						simpleButtons.RemoveAt(0);
					else
						checkedButtons.RemoveAt(0);
				}
			}
		}
		protected virtual BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			if(button is BaseSeparator)
				return new BaseSeparatorInfo(button);
			return new BaseButtonInfo(button);
		}
		protected bool CheckGroup(IList<BaseButtonInfo> buttonInfos, List<IBaseButton> checkedButton) {
			int index = 0;
			foreach(BaseButtonInfo info in buttonInfos) {
				if(checkedButton.Contains(info.Button)) index++;
			}
			return index == checkedButton.Count;
		}
	}
	public class GroupBoxButtonsPanelViewInfo : BaseButtonsPanelViewInfo {
		public GroupBoxButtonsPanelViewInfo(IButtonsPanel panel) : base(panel) {}
		protected override Point GetPointOffset(bool horz, Point offset, Rectangle innerContent) {
			if(horz)
				return new Point(offset.X - Content.X + innerContent.X, offset.Y);
			return new Point(offset.X, offset.Y - Content.Y + innerContent.Y);
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			if(button is BaseSeparator)
				return new BaseSeparatorInfo(button);
			if(button is DevExpress.XtraEditors.ButtonsPanelControl.RibbonPageGroupButton)
				return new RibbonPageGroupButtonInfo(button);
			if(button is DevExpress.XtraEditors.ButtonsPanelControl.LayoutViewCardExpandButton)
				return new LayoutViewCardExpandButtonViewInfo(button);
			return new BaseButtonInfo(button, true);
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			return Panel.RightToLeft ? CalcButtonInfosCoreRTL(g, buttonPainter, interval, horz, buttons, offset) : CalcButtonInfosCore(g, buttonPainter, interval, horz, buttons, offset); 
		}
		protected virtual Point CalcButtonInfosCoreRTL(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset){
			 foreach(BaseButtonInfo buttonInfo in buttons) {
				Size minSize = buttonInfo.CalcMinSize(g, buttonPainter);
				if(horz)
					offset.X -= (minSize.Width + interval);
				else
					offset.Y -= (minSize.Height + interval);
				buttonInfo.Calc(g, buttonPainter, offset, Content, horz, true);
			 }
			 return offset;
		}
		protected virtual Point CalcButtonInfosCore(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			foreach(BaseButtonInfo buttonInfo in buttons) {
				buttonInfo.Calc(g, buttonPainter, offset, Content, horz, true);
				if(horz)
					offset.X += (buttonInfo.Bounds.Width + interval);
				else
					offset.Y += (buttonInfo.Bounds.Height + interval);
			}
			return offset;
		}
		protected override void SortButtonListCore(List<IBaseButton> positiveList) {
			positiveList.Sort(PositiveCompare);
		}
	}
}
namespace DevExpress.XtraEditors.ButtonsPanelControl {
	public interface IButtonPanelControlAppearanceOwner : IAppearanceOwner {
		IButtonsPanelControlAppearanceProvider CreateAppearanceProvider();
	}
	public class ButtonControlInfo : BaseButtonControlInfo {
		public ButtonControlInfo(IBaseButton button)
			: base(button) {
		}
	}
	public class BaseButtonControlInfo : BaseButtonInfo {
		public BaseButtonControlInfo(IBaseButton button)
			: base(button) {
		}
		public override void Calc(System.Drawing.Graphics g, BaseButtonPainter painter, System.Drawing.Point offset, System.Drawing.Rectangle maxRect, bool isHorizontal) {
			Calc(g, painter, offset, maxRect, isHorizontal, true);
		}
	}
	public class ButtonsPanelControlViewInfo : BaseButtonsPanelViewInfo {
		public ButtonsPanelControlViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return new ButtonControlInfo(button);
		}
		protected override Point GetPointOffset(bool horz, Point offset, Rectangle innerContent) {
			if(horz) {
				return new Point(offset.X - Content.X + innerContent.X, offset.Y);
			}
			return offset;
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			int rowOffset = MinSize.Height;
			int columnOffset = MinSize.Width;
			Point tempOffset = offset;
			int previousRow = 0;
			int previousColumn = 0;
			foreach(BaseButtonInfo buttonInfo in buttons) {
				if(previousRow != buttonInfo.Row) {
					offset.X = tempOffset.X;
					offset.Y = tempOffset.Y + buttonInfo.Row * rowOffset + interval * buttonInfo.Row;
				}
				if(previousColumn != buttonInfo.Column) {
					offset.Y = tempOffset.Y;
					offset.X = tempOffset.X + buttonInfo.Column * columnOffset + interval * buttonInfo.Column;
				}
				if(horz) {
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(Content.Width, rowOffset)), horz);
					offset.X += (buttonInfo.Bounds.Width + interval);
					previousRow = buttonInfo.Row;
				}
				else {
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(columnOffset, Content.Height)), horz);
					offset.Y += (buttonInfo.Bounds.Height + interval);
					previousColumn = buttonInfo.Column;
				}
			}
			return offset;
		}
		protected override void SortButtonListCore(List<IBaseButton> positiveList) {
			positiveList.Sort(PositiveCompare);
		}
	}
}
