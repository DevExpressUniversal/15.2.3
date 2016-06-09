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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.ViewInfo;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.ToolTip.ViewInfo;
using DevExpress.Utils.Text;
namespace DevExpress.Utils.ViewInfo {
	public class ToolTipViewInfoSuperTip : ToolTipViewInfoBase {
		SuperToolTip superTip;
		public ToolTipViewInfoSuperTip(SuperToolTip superTip) {
			this.superTip = superTip;
		}
		public SuperToolTip SuperTip { get { return superTip; } }
		protected override Size CalcBoundsSize(Size contentSize) {
			Size customSize = this.superTip.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ? contentSize : Size.Empty;
			if(customSize != Size.Empty) {
				SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(superTip.LookAndFeel)[CommonSkins.SkinToolTipWindow], Rectangle.Empty);
				GraphicsInfo gInfo = new GraphicsInfo();
				gInfo.AddGraphics(null);
				try {
					customSize = ObjectPainter.CalcBoundsByClientRectangle(gInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(Point.Empty, customSize)).Size;
				}
				finally { gInfo.ReleaseGraphics(); }
			}
			superTip.AdjustSize(customSize);
			return superTip.Bounds.Size;
		}
	}
	public abstract class BaseToolTipItemViewInfo {
		Rectangle bounds;
		GraphicsInfo ginfo;
		bool isReady;
		AppearanceObject paintAppearance;
		BaseToolTipObject item;
		public BaseToolTipItemViewInfo(BaseToolTipObject item) {
			this.item = item;
			this.paintAppearance = new AppearanceObject();
		}
		public BaseToolTipObject Item { get { return item; } }
		public abstract UserLookAndFeel LookAndFeel { get; }
		protected virtual AppearanceObject GetControllerAppearance() { return Item.Controller == null? null: Item.Controller.Appearance; }
		protected virtual void UpdatePaintAppearance() {
			PaintAppearance.Reset();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Item.Appearance, GetControllerAppearance() },
				DefaultAppearance);
			ToolTipItem item = Item as ToolTipItem;
			if(item != null && item.Container is SuperToolTip && item.Container.Appearance.TextOptions.HotkeyPrefix != HKeyPrefix.Default) {
				PaintAppearance.TextOptions.HotkeyPrefix = item.Container.Appearance.TextOptions.HotkeyPrefix;
				return;
			}
			if(Item.Appearance.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				PaintAppearance.TextOptions.HotkeyPrefix = HKeyPrefix.Hide;
		}
		public void CalcViewInfo() {
			this.bounds = Item.Bounds;
			UpdatePaintAppearance();
			GInfo.AddGraphics(null);
			try {
				CalcViewInfoCore();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.isReady = true;
		}
		public Size CalcActualContentSize() {
			UpdatePaintAppearance();
			GInfo.AddGraphics(null);
			try {
				return CalcActualContentSizeCore();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public AppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public virtual AppearanceDefault DefaultAppearance {
			get { return new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, SystemColors.ControlDark); }
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public bool IsReady { get { return isReady; } set { isReady = value; } }
		protected virtual void CalcViewInfoCore() {
		}
		protected virtual Size CalcActualContentSizeCore() { return Size.Empty; }
		protected internal virtual BaseToolTipItemInfoArgs GetInfoArgs(GraphicsCache cache) { return new BaseToolTipItemInfoArgs(cache, this, Rectangle.Empty); }
		protected internal virtual void LayoutItem() { }
		public GraphicsInfo GInfo {
			get {
				if(ginfo == null) ginfo = new GraphicsInfo();
				return ginfo;
			}
		}
	}
	public class BaseToolTipItemPainter : ObjectPainter {
		public BaseToolTipItemPainter() { }
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
		}
	}
	public class ToolTipTitleItemViewInfo : ToolTipItemViewInfo {
		public ToolTipTitleItemViewInfo(ToolTipItem item) : base(item) {
		}
		protected override AppearanceObject GetControllerAppearance() { return Item.Controller == null ? null : Item.Controller.AppearanceTitle; }
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault res = base.DefaultAppearance;
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) {
					res.Font = ResourceCache.DefaultCache.GetFont(AppearanceObject.DefaultFont, FontStyle.Bold);
				}
				return res;
			}
		}
		protected override string GetElementName() { return CommonSkins.SkinToolTipTitleItem; }
		protected override int LeftContentIndent { get { return 0; } }
	}
	public class ToolTipItemViewInfo : BaseToolTipItemViewInfo {
		Rectangle contentBounds;
		Rectangle imageBounds;
		Rectangle textBounds;
		StringInfo textInfo;
		public ToolTipItemViewInfo(ToolTipItem item) : base(item) {
			this.contentBounds = this.imageBounds = this.textBounds = Rectangle.Empty;
		}
		protected override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			PaintAppearance.TextOptions.RightToLeft = Item.Container == null? false: Item.Container.RightToLeft;
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					SkinElement element = GetElement();
					if(element != null) 
					return GetElement().GetAppearanceDefault(LookAndFeel);
				}
				return base.DefaultAppearance;
			}
		}
		protected SkinElement GetElement() {
			if(GetElement(GetElementName()) == null) return GetElement(CommonSkins.SkinToolTipItem);
			return GetElement(GetElementName());
		}
		SkinElement GetElement(string name) { return CommonSkins.GetSkin(LookAndFeel)[name]; }
		protected virtual string GetElementName() { return CommonSkins.SkinToolTipItem; }
		public override UserLookAndFeel LookAndFeel { get { return Item.LookAndFeel; } }
		public new ToolTipItem Item { get { return base.Item as ToolTipItem; } }
		public Rectangle ContentBounds { get { return contentBounds; } }
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; }  }
		public StringInfo TextInfo {
			get {
				if (textInfo == null) 
					textInfo = new StringInfo();
				return textInfo; 
			}
			set {
				textInfo = value;
			}
		}
		protected override void CalcViewInfoCore() {
			CalcContentBounds();
			CalcActualContentSize();
			LayoutItem();
		}
		protected internal override void LayoutItem() {
			LayoutImage();
			LayoutText();
			if(Item.Container.RightToLeft) {
				ImageBounds = ReverceBounds(ImageBounds, ContentBounds);
				TextBounds = ReverceBounds(TextBounds, ContentBounds);
				if(GetAllowHtmlText()) {
					TextInfo.SetLocation(TextBounds.Location);
				}
			}
		}
		private Rectangle ReverceBounds(Rectangle itemBounds, Rectangle bounds) {
			return new Rectangle(bounds.Right - (itemBounds.X - bounds.X) - itemBounds.Width, itemBounds.Y, itemBounds.Width, itemBounds.Height);
		}
		protected internal virtual Size CalcImageSize() {
			if(Item.Icon != null) return Item.Icon.Size;
			if(Item.Image != null) return Item.Image.Size;
			if(Images != null) return ImageCollection.GetImageListSize(Images);
			return Size.Empty;
		}
		public bool GetAllowHtmlText() {
			if (Item.AllowHtmlText != DefaultBoolean.Default) return Item.AllowHtmlText == DefaultBoolean.True;
			return Item.OwnerAllowHtmlText;
		}
		protected internal virtual Size CalcTextSize() {
			if (GetAllowHtmlText()) {
				StringCalculateArgs args = new StringCalculateArgs(GInfo.Graphics, PaintAppearance, TextOptions.DefaultOptionsMultiLine, Item.Text, new Rectangle(Point.Empty, new Size(CalcAvailableTextWidth(), 0)), null);
				args.HyperlinkColor = EditorsSkins.GetSkin(LookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
				args.AllowBaselineAlignment = false;
				TextInfo = StringPainter.Default.Calculate(args);
				return TextInfo.Bounds.Size;
			}
			Size size = PaintAppearance.CalcTextSize(GInfo.Graphics, PaintAppearance.GetStringFormat(TextOptions.DefaultOptionsMultiLine), Item.Text, CalcAvailableTextWidth()).ToSize();
			size.Height = Math.Max(PaintAppearance.CalcDefaultTextSize(GInfo.Graphics).Height, size.Height);
			return size;
		}
		protected internal virtual int CalcAvailableTextWidth() {
			int width = CalcAvailableContentWidth() - LeftContentIndent;
			if(HasImage) width -= (CalcImageSize().Width + IndentBetweenImageAndText);
			return Math.Max(width, 1);
		}
		protected internal virtual int CalcAvailableContentWidth() { return Item.MaxWidth; }
		protected override Size CalcActualContentSizeCore() {
			imageBounds.Size = CalcImageSize();
			textBounds.Size = CalcTextSize();
			Size res = Size.Empty;
			res.Height = Math.Max(imageBounds.Height, textBounds.Height);
			res.Width = textBounds.Width + LeftContentIndent;
			if(HasImage) res.Width += ImageBounds.Width + IndentBetweenImageAndText;
			return res;
		}
		protected internal virtual void CalcContentBounds() { this.contentBounds = Bounds; }
		protected internal virtual void LayoutImage() {
			if(!HasImage) return;
			Point location = Point.Empty;
			if(ImageFromLeft)
				location = new Point(ContentBounds.X + LeftContentIndent, ContentBounds.Top);
			else
				location = new Point(ContentBounds.Right - imageBounds.Width, ContentBounds.Top);
			ImageBounds = new Rectangle(location, ImageBounds.Size);
		}
		protected internal virtual void LayoutText() {
			Point location = new Point(ContentBounds.X + LeftContentIndent, ContentBounds.Y);
			if(ImageFromLeft && HasImage) {
				location.X = ImageBounds.Right + IndentBetweenImageAndText;
			}
			TextBounds = new Rectangle(location, TextBounds.Size);
			if (GetAllowHtmlText())
				TextInfo.SetLocation(TextBounds.Location);
		}
		protected internal virtual bool ImageFromLeft {
			get {
				return Item.ImageAlign == ToolTipImageAlignment.Default || Item.ImageAlign == ToolTipImageAlignment.Left;
			}
		}
		public Icon Icon { get { return Item.Icon; } }
		public object Images { get { return Item.Images; } }
		public int ImageIndex { get { return Item.ImageIndex; } }
		public Image Image { get { return Item.Image; } }
		public bool HasImage {
			get {
				return Icon != null || Item.Image != null || ImageCollection.IsImageListImageExists(Images, ImageIndex);
			}
		}
		protected virtual int LeftContentIndent {
			get { return Item.LeftIndent; } 
		}
		public string Text { get { return Item.Text; } }
		protected internal int IndentBetweenImageAndText {
			get {
				return Item.ImageToTextDistance;
			}
		}
		protected internal override BaseToolTipItemInfoArgs GetInfoArgs(GraphicsCache cache) { return new ToolTipItemInfoArgs(cache, this) as BaseToolTipItemInfoArgs; }
	}
	public class BaseToolTipItemInfoArgs : ObjectInfoArgs {
		BaseToolTipItemViewInfo viewInfo;
		public BaseToolTipItemInfoArgs(GraphicsCache cache, BaseToolTipItemViewInfo viewInfo, Rectangle bounds) : base(cache, bounds, ObjectState.Normal) { this.viewInfo = viewInfo; }
		public virtual BaseToolTipItemViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class ToolTipItemInfoArgs : BaseToolTipItemInfoArgs {
		public new ToolTipItemViewInfo ViewInfo { get { return base.ViewInfo as ToolTipItemViewInfo; } }
		public ToolTipItemInfoArgs(GraphicsCache cache, ToolTipItemViewInfo viewInfo) : base(cache, viewInfo, viewInfo.Bounds) { }
	}
	public class ToolTipItemPainter : BaseToolTipItemPainter {
		public ToolTipItemPainter() { }
		public override void DrawObject(ObjectInfoArgs e) {
			ToolTipItemInfoArgs infoArgs = e as ToolTipItemInfoArgs;
			if(ShouldDrawImage) DrawImage(infoArgs);
			if(ShouldDrawText) DrawText(infoArgs);
		}
		public virtual void DrawImage(ToolTipItemInfoArgs e) {
			if(e.ViewInfo.Icon != null) {
				e.Graphics.DrawIcon(e.ViewInfo.Icon, e.ViewInfo.ImageBounds);
				return;
			}
			ImageCollection.DrawImageListImage(e.Cache, e.ViewInfo.Image, e.ViewInfo.Images, e.ViewInfo.ImageIndex, e.ViewInfo.ImageBounds, true);
		}
		public virtual void DrawText(ToolTipItemInfoArgs e) {
			if (e.ViewInfo.GetAllowHtmlText())
				StringPainter.Default.DrawString(e.Cache, e.ViewInfo.TextInfo);
			else
				e.ViewInfo.PaintAppearance.DrawString(e.Cache, e.ViewInfo.Text, e.ViewInfo.TextBounds, e.ViewInfo.PaintAppearance.GetStringFormat(TextOptions.DefaultOptionsMultiLine));
		}
		public virtual bool ShouldDrawText { get { return true; } }
		public virtual bool ShouldDrawImage { get { return true; } }
	}
	public class ToolTipContainerViewInfo : BaseToolTipItemViewInfo {
		Rectangle contentBounds;
		public ToolTipContainerViewInfo(SuperToolTip container) : base(container) {		}
		public override UserLookAndFeel LookAndFeel { get { return Container.LookAndFeel; } }
		public SuperToolTip Container { get { return Item as SuperToolTip; } }
		public ObjectPainter BorderPainter {
			get {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinToolTipContainerBackgroundPainter(LookAndFeel);
				return new ToolTipContainerBackgroundPainter();
			}
		}
		protected override void CalcViewInfoCore() {
			CalcContentBounds();
			LayoutItems();
		}
		public Rectangle ContentBounds { get { return contentBounds; } }
		public virtual void CalcContentBounds() {
			this.contentBounds = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, BorderPainter, new ToolTipContainerInfoArgs(null, this));
#if DXWhidbey
			contentBounds.X += Container.Padding.Left;
			contentBounds.Y += Container.Padding.Top;
			contentBounds.Width -= Container.Padding.Size.Width;
			contentBounds.Height -= Container.Padding.Size.Height;
#endif
		}
		protected virtual Size CalcSizeByContentSize(Size content) {
			content.Width += Container.GetPaddingSize().Width;
			content.Height += Container.GetPaddingSize().Height;
			return BorderPainter.CalcBoundsByClientRectangle(new ToolTipContainerInfoArgs(null, this), new Rectangle(Point.Empty, content)).Size;
		}
		public virtual int CalcActualContentWidth() {
			if(Container.FixedTooltipWidth) return Container.MaxWidth;
			int maxWidth = 0;
			for(int itemIndex = 0; itemIndex < Container.Items.Count; itemIndex++) {
				if(Container.Items[itemIndex] is ToolTipSeparatorItem) continue;
				maxWidth = Math.Max(maxWidth, Container.Items[itemIndex].Bounds.Width);
			}
			return maxWidth;
		}
		protected virtual void LayoutItems() {
			int currY = ContentBounds.Top;
			for(int itemIndex = 0; itemIndex < Container.Items.Count; itemIndex++) {
				BaseToolTipItem item = Container.Items[itemIndex];
				item.SetBounds(new Rectangle(new Point(ContentBounds.X, currY), new Size(ContentBounds.Width, item.ViewInfo.CalcActualContentSize().Height) ));
				item.ViewInfo.CalcViewInfo();
				currY += item.Bounds.Height;
				if(!(item is ToolTipSeparatorItem) && itemIndex < Container.Items.Count - 1 && 
					!(Container.Items[itemIndex + 1] is ToolTipSeparatorItem)) currY += Container.DistanceBetweenItems;
			}
		}
		protected override Size CalcActualContentSizeCore() {
			if(Container.Items.Count == 0) return Size.Empty;
			bool hasPictures = false;
			Size resSize = Size.Empty;
			for(int itemIndex = 0; itemIndex < Container.Items.Count; itemIndex++) {
				BaseToolTipItem item = Container.Items[itemIndex];
				resSize.Height += item.Bounds.Height;
				if(item.HasBigImage) hasPictures = true;
				if(!(item is ToolTipSeparatorItem) && itemIndex < Container.Items.Count - 1 &&
					!(Container.Items[itemIndex + 1] is ToolTipSeparatorItem))  resSize.Height += Container.DistanceBetweenItems;
			}
			resSize.Width = CalcActualContentWidth();
			if(hasPictures) resSize.Height += 5;
			return resSize;
		}
		public virtual Size CalcSize() {
			GInfo.AddGraphics(null);
			try {
				for(int n = 0; n < Container.Items.Count; n++)
					Container.Items[n].AdjustSize();
				Size size = CalcActualContentSize();
				return CalcSizeByContentSize(size);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
	}
	public class ToolTipContainerInfoArgs : BaseToolTipItemInfoArgs {
		public ToolTipContainerInfoArgs(GraphicsCache cache, ToolTipContainerViewInfo viewInfo) : base(cache, viewInfo, viewInfo.Bounds) { }
		public new ToolTipContainerViewInfo ViewInfo {
			get { return base.ViewInfo as ToolTipContainerViewInfo; }
		}
	}
	public class ToolTipContainerBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ToolTipContainerInfoArgs se = e as ToolTipContainerInfoArgs;
			e.Cache.Paint.DrawRectangle(e.Graphics, e.Cache.GetSolidBrush(se.ViewInfo.PaintAppearance.BorderColor), se.ViewInfo.Bounds);
			Rectangle r = Rectangle.Inflate(se.ViewInfo.Bounds, -1, -1);
			se.ViewInfo.PaintAppearance.FillRectangle(e.Cache, r);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			client.Inflate(4, 2);
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle res = e.Bounds;
			res.Inflate(-4, -2);
			return res;
		}
	}
	public class ToolTipContainerPainter : BaseToolTipItemPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ToolTipContainerInfoArgs se = e as ToolTipContainerInfoArgs;
			ObjectPainter.DrawObject(se.Cache, se.ViewInfo.BorderPainter, se);
			for(int itemIndex = 0; itemIndex < se.ViewInfo.Container.Items.Count; itemIndex++) {
				ObjectPainter.DrawObject(se.Cache, se.ViewInfo.Container.Items[itemIndex].Painter, se.ViewInfo.Container.Items[itemIndex].ViewInfo.GetInfoArgs(se.Cache));
			}
		}
	}
	public class ToolTipSeparatorItemViewInfo : BaseToolTipItemViewInfo {
		public ToolTipSeparatorItemViewInfo(ToolTipSeparatorItem separator) : base(separator) {
		}
		public ToolTipSeparatorItem Separator { get { return base.Item as ToolTipSeparatorItem; } }
		public override UserLookAndFeel LookAndFeel { get { return Separator.LookAndFeel; } }
		public override AppearanceDefault DefaultAppearance {
			get {
				return new AppearanceDefault(SystemColors.Control, SystemColors.ControlDark);
			}
		}
		protected override void CalcViewInfoCore() {
			CalcActualContentSize();
		}
		protected override Size CalcActualContentSizeCore() {
			return new Size(Separator.Container.ContentBounds.Width, 
				ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, Item.Painter, new ToolTipSeparatorInfoArgs(this)).Height);
		}
		protected internal override void LayoutItem() { }
		protected internal override BaseToolTipItemInfoArgs GetInfoArgs(GraphicsCache cache) { return new ToolTipSeparatorInfoArgs(cache, this); }
	}
	public class ToolTipSeparatorInfoArgs : BaseToolTipItemInfoArgs {
		public ToolTipSeparatorInfoArgs(ToolTipSeparatorItemViewInfo viewInfo) : this(null, viewInfo) { }
		public ToolTipSeparatorInfoArgs(GraphicsCache cache, ToolTipSeparatorItemViewInfo viewInfo) : base(cache, viewInfo, viewInfo.Bounds) { }
		public new ToolTipSeparatorItemViewInfo ViewInfo { get { return base.ViewInfo as ToolTipSeparatorItemViewInfo; } }
	}
	public class SkinToolTipContainerBackgroundPainter : SkinCustomPainter {
		public SkinToolTipContainerBackgroundPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			ToolTipContainerInfoArgs le = e as ToolTipContainerInfoArgs;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(le.ViewInfo.LookAndFeel)[CommonSkins.SkinToolTipWindow], le.ViewInfo.Bounds);
			return info;
		}
	}
	public class SkinToolTipSeparatorItemPainter : SkinCustomPainter {
		public SkinToolTipSeparatorItemPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			ToolTipSeparatorInfoArgs le = e as ToolTipSeparatorInfoArgs;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(le.ViewInfo.LookAndFeel)[CommonSkins.SkinToolTipSeparator], le.ViewInfo.Bounds);
			return info;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo info = UpdateInfo(e);
			info.Bounds = GetObjectClientRectangle(e);
			SkinElementPainter.Default.DrawObject(info);
		}
	}
	public class ToolTipSeparatorItemPainter : BaseToolTipItemPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ToolTipSeparatorInfoArgs le = e as ToolTipSeparatorInfoArgs;
			Rectangle bounds = le.Bounds;
			bounds.Y += 3;
			le.Paint.DrawLine(le.Graphics, le.Cache.GetPen(le.ViewInfo.PaintAppearance.ForeColor), new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
			le.Paint.DrawLine(le.Graphics, le.Cache.GetPen(le.ViewInfo.PaintAppearance.BackColor), new Point(bounds.Left, bounds.Top + 1), new Point(bounds.Right, bounds.Top + 1));
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 0, 6);
		}
	}
}
