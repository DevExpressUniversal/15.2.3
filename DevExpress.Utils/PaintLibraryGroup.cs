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
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonPanel;
using System.Drawing.Printing;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.Utils.Drawing {
	public interface IPanelControlOwner {
		void OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e);
		Color GetForeColor();
	}
	public class NullPanellControlOwner : IPanelControlOwner {
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) { }
		Color IPanelControlOwner.GetForeColor() { return SystemColors.ControlText; }
	}
	public class GroupObjectInfoArgs : StyleObjectInfoArgs, IStringImageProvider, IDisposable {
		AppearanceObject appearanceCaption;
		ObjectState buttonState;
		BaseButtonsPanel buttonsPanelCore;
		IBaseButton expandButtonCore;
		IGroupBoxButtonsPanelOwner panelOwnerCore;
		object tag;
		bool showBackgroundImage, showCaption, showCaptionImage, showButton, isReady, expanded;
		bool autoSize;
		Locations captionLocation;
		GroupElementLocation captionImageLocation, buttonLocation;
		Image captionImage;
		Image contentImage;
		Image backgroundImage;
		ImageLayout backgroundImageLayout;
		ContentAlignment contentImageAlignment;
		Rectangle captionBounds, textBounds, clientBounds, controlClientBounds, captionImageBounds, buttonsPanelBounds;
		Rectangle contentImageRectangle;
		BorderStyles borderStyle;
		string caption;
		GraphicsInfo gInfo;
		int stateIndex;
		ImageAttributes attributes;
		Padding captionImageMargin;
		Padding captionImagePadding = new Padding(1, 2, 1, 2);
		public GroupObjectInfoArgs() {
			this.AllowHtmlText = false;
			this.expanded = true;
			this.gInfo = new GraphicsInfo();
			this.caption = string.Empty;
			this.borderStyle = BorderStyles.Default;
			this.appearanceCaption = AppearanceObject.ControlAppearance;
			this.buttonState = ObjectState.Normal;
			this.showCaption = true;
			this.showBackgroundImage = true;
			this.captionLocation = Locations.Default;
			this.captionImageMargin = new Padding(0);
			backgroundImageLayout = ImageLayout.Stretch;
			captionImageLocation = GroupElementLocation.Default;
			buttonLocation = GroupElementLocation.Default;
			this.LastBackColor = Color.Empty;
		}
		public void SetButtonsPanelOwner(IGroupBoxButtonsPanelOwner buttonsPanelOwner, bool raiseEvents = true) {
			this.panelOwnerCore = buttonsPanelOwner;
			this.expandButtonCore = CreateExpandButton();
			this.buttonsPanelCore = CreateButtonsPanel(raiseEvents);
			ButtonsPanel.BeginUpdate();
			ButtonsPanel.Buttons.Add(ExpandButton);
			ButtonsPanel.CancelUpdate();
			ButtonsPanel.Changed += OnButtonsPanelChanged;
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			ButtonsPanelOwner.LayoutChanged();
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			OffsetRectangleRef(ref captionBounds, x, y);
			OffsetRectangleRef(ref textBounds, x, y);
			OffsetRectangleRef(ref buttonsPanelBounds, x, y);
			OffsetRectangleRef(ref clientBounds, x, y);
			OffsetRectangleRef(ref controlClientBounds, x, y);
			OffsetRectangleRef(ref captionImageBounds, x, y);
			OffsetRectangleRef(ref contentImageRectangle, x, y);
		}
		public bool DrawUserBackground { get; set; }
		protected virtual BaseButtonsPanel CreateButtonsPanel(bool raiseEvents = true) {
			return new GroupBoxButtonsPanel(ButtonsPanelOwner) { RaiseEvents = raiseEvents };
		}
		protected virtual IBaseButton CreateExpandButton() {
			return new ExpandButton() { Checked = Expanded };
		}
		protected internal IGroupBoxButtonsPanelOwner ButtonsPanelOwner {
			get { return panelOwnerCore; }
		}
		public BaseButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		public Color LastBackColor;
		public bool AllowBorderColorBlending { get; set; }
		public bool AllowHtmlText { get; set; }
		public bool AllowGlyphSkinning { get; set; }
		public ImageCollection HtmlImages { get; set; }
		public Padding CaptionImagePadding { get { return captionImagePadding; } set { captionImagePadding = value; } }
		public Padding CaptionImageMargin { get { return captionImageMargin; } set { captionImageMargin = value; } }
		public object Tag { get { return tag; } set { tag = value; } }
		public int StateIndex { get { return stateIndex; } set { stateIndex = value; } }
		public ObjectState ButtonState { get { return buttonState; } set { buttonState = value; } }
		public Image BackgroundImage { get { return backgroundImage; } set { backgroundImage = value; } }
		public ImageLayout BackgroundImageLayout { get { return backgroundImageLayout; } set { backgroundImageLayout = value; } }
		public Image CaptionImage { get { return captionImage; } set { captionImage = value; } }
		public Image ContentImage { get { return contentImage; } set { contentImage = value; } }
		public ContentAlignment ContentImageAlignment { get { return contentImageAlignment; } set { contentImageAlignment = value; } }
		public Rectangle ContentImageRectangle { get { return contentImageRectangle; } set { contentImageRectangle = value; } }
		public bool Expanded {
			get { return expanded; }
			set {
				expanded = value;
				OnExpandedStateChanged();
			}
		}
		public bool IsReady { get { return isReady; } set { isReady = value; } }
		public ImageAttributes Attributes { get { return attributes; } set { attributes = value; } } 
		public GraphicsInfo GInfo { get { return gInfo; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle ControlClientBounds { get { return controlClientBounds; } set { controlClientBounds = value; } }
		public Rectangle CaptionBounds { get { return captionBounds; } set { captionBounds = value; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		public Rectangle ButtonBounds { get { return GetExpandButtonBounds(); } set { } }
		public Rectangle ButtonsPanelBounds { get { return buttonsPanelBounds; } set { buttonsPanelBounds = value; } }
		public Rectangle CaptionImageBounds { get { return captionImageBounds; } set { captionImageBounds = value; } }
		public IBaseButton ExpandButton { get { return expandButtonCore; } }
		public bool HasCustomButtons { get { return ButtonsPanelOwner != null && ButtonsPanelOwner.CustomHeaderButtons.Count > 0; } }
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaption; }
			set { appearanceCaption = value; }
		}
		public BorderStyles BorderStyle { get { return borderStyle; } set { borderStyle = value; } }
		public bool AutoSize {
			get { return autoSize; }
			set { autoSize = value; }
		}
		public bool ShowCaption {
			get { return showCaption; }
			set { showCaption = value; }
		}
		public bool ShowBackgroundImage {
			get { return showBackgroundImage; }
			set { showBackgroundImage = value; }
		}
		public bool ShowButton {
			get { return showButton; }
			set { showButton = value; }
		}
		public bool ShowButtonsPanel {
			get { return ButtonsPanel != null && ButtonsPanel.Buttons.Count != 0; }
		}
		public bool ShowCaptionImage {
			get { return showCaptionImage; }
			set { showCaptionImage = value; }
		}
		public GroupElementLocation CaptionImageLocation {
			get { return captionImageLocation; }
			set { captionImageLocation = value; }
		}
		public GroupElementLocation ButtonLocation {
			get { return buttonLocation; }
			set { buttonLocation = value; }
		}
		public Locations CaptionLocation {
			get { return captionLocation; }
			set { captionLocation = value; }
		}
		public bool ShouldDrawAllowHtmlText {
			get { return AllowHtmlText && (CaptionLocation == Locations.Default || CaptionLocation == Locations.Bottom || CaptionLocation == Locations.Top); }
		}
		protected virtual void OnExpandedStateChanged() {
			if(ExpandButton != null) {
				ButtonsPanel.BeginUpdate();
				ExpandButton.Properties.Checked = !Expanded;
				ButtonsPanel.CancelUpdate();
			}
		}
		#region IStringImageProvider
		Image IStringImageProvider.GetImage(string id) {
			if(HtmlImages == null) return null;
			return HtmlImages.Images[id];
		}
		#endregion
		public bool ShouldUseGlyphSkinning {
			get { return AllowGlyphSkinning && (State != ObjectState.Disabled); }
		}
		public Rectangle GetExpandButtonBounds() {
			if(ButtonsPanel == null || ButtonsPanel.ViewInfo.Buttons == null) return Rectangle.Empty;
			BaseButtonInfo bInfo = ButtonsPanel.ViewInfo.Buttons.SingleOrDefault(x => x.Button == ExpandButton);
			if(bInfo == null)
				bInfo = ButtonsPanel.ViewInfo.Buttons.SingleOrDefault(x => (x.Button is RibbonPageGroupButton));
			return bInfo == null ? Rectangle.Empty : bInfo.Bounds;
		}
		public GroupElementLocation GetCaptionImageLocation() {
			if(!RightToLeft) return CaptionImageLocation;
			switch(CaptionImageLocation) {
				case GroupElementLocation.Default:
				case GroupElementLocation.BeforeText:
					return GroupElementLocation.AfterText;
				default:
					return GroupElementLocation.BeforeText;
			}
		}
		public HorzAlignment GetCaptionTextLocation() {
			var alignment = AppearanceCaption.TextOptions.HAlignment;
			if(!RightToLeft) return alignment;
			switch(alignment) {
				case HorzAlignment.Default:
				case HorzAlignment.Near: return HorzAlignment.Far;
				case HorzAlignment.Far: return HorzAlignment.Near;
				case HorzAlignment.Center: return HorzAlignment.Center;
			}
			return alignment;
		}
		public GroupElementLocation GetButtonLocation() {
			if(!RightToLeft) return ButtonLocation;
			switch(ButtonLocation) {
				case GroupElementLocation.Default:
				case GroupElementLocation.BeforeText:
					return GroupElementLocation.AfterText;
				default:
					return GroupElementLocation.BeforeText;
			}
		}
		bool isDisposing = false;
		public void Dispose() {
			if(!isDisposing) {
				OnDispose();
				isDisposing = true;
			}
		}
		public bool IsDisposing { get { return isDisposing; } }
		protected virtual void OnDispose() {
			if(buttonsPanelCore == null) return;
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			ButtonsPanel.Buttons.ClearMergedButtons();
			ButtonsPanel.Dispose();
			buttonsPanelCore = null;
		}
	}
	public class LayoutViewCardGroupObjectInfoArgs : GroupObjectInfoArgs {
		protected override IBaseButton CreateExpandButton() {
			return new LayoutViewCardExpandButton(this) { Checked = !Expanded };
		}
	}
	public class WindowsXPGroupObjectPainter : FlatGroupObjectPainter {
		public static int DefaultMargins = -2;
		public WindowsXPGroupObjectPainter(IPanelControlOwner owner) : base(owner) { }
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.BorderStyle == BorderStyles.Default) {
				XPButtonPainter res = new XPButtonPainter();
				res.DrawArgs = new DevExpress.Utils.WXPaint.WXPPainterArgs("button", DevExpress.Utils.WXPaint.XPConstants.BP_GROUPBOX, 0);
				return res;
			}
			return base.GetBorderPainter(e);
		}
		protected override Rectangle GetBorderBounds(GroupObjectInfoArgs info) {
			Locations loc = GetCaptionLocation(info);
			Rectangle border = info.Bounds;
			if(!info.ShowCaption) return border;
			switch(loc) {
				case Locations.Left:
				case Locations.Right:
					border.Width -= (info.CaptionBounds.Width / 2 + 4);
					if(loc == Locations.Left)
						border.X += (info.CaptionBounds.Width / 2 + 4);
					break;
				case Locations.Bottom:
					border.Height -= (info.CaptionBounds.Height / 2 + 4);
					break;
				case Locations.Top:
					border.Height -= (info.CaptionBounds.Height / 2 + 4);
					border.Y += (info.CaptionBounds.Height / 2 + 4);
					break;
			}
			return border;
		}
		protected override Rectangle CalcControlBounds(GroupObjectInfoArgs info) {
			return DefaultMargins == 0 ? info.ClientBounds : Rectangle.Inflate(info.ClientBounds, DefaultMargins, DefaultMargins);
		}
		public override AppearanceDefault DefaultAppearanceCaption {
			get {
				return new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			}
		}
		Color GetCaptionColor() {
			DevExpress.Utils.WXPaint.WXPPainterArgs args = new DevExpress.Utils.WXPaint.WXPPainterArgs("button", DevExpress.Utils.WXPaint.XPConstants.BP_GROUPBOX, 0);
			Color c = DevExpress.Utils.WXPaint.WXPPainter.Default.GetThemeColor(args, DevExpress.Utils.WXPaint.XPConstants.TMT_TEXTCOLOR);
			return c;
		}
	}
	public class FlatGroupObjectPainter : GroupObjectPainter {
		public FlatGroupObjectPainter(IPanelControlOwner owner) : base(owner) { }
		public override AppearanceDefault DefaultAppearance {
			get {
				return new AppearanceDefault(Color.Transparent, SkinElementPainter.DefaultColor);
			}
		}
		public override AppearanceDefault DefaultAppearanceCaption {
			get {
				return new AppearanceDefault(Color.Transparent, SkinElementPainter.DefaultColor);
			}
		}
		protected override int CalcCaptionTextExtraIndent() { return 4; }
		public override void DrawVString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle rect, int angle) {
			if(angle == 270 || angle == 90)
				rect.Inflate(0, -2);
			else
				rect.Inflate(-2, 0);
			base.DrawVString(cache, appearance, text, rect, angle);
		}
		protected override Rectangle GetBorderBounds(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return info.Bounds;
			Locations loc = GetCaptionLocation(info);
			Rectangle border = info.Bounds;
			switch(loc) {
				case Locations.Left:
				case Locations.Right:
					border.Width -= (info.CaptionBounds.Width / 2 + 1);
					if(loc == Locations.Left)
						border.X += info.CaptionBounds.Width / 2 + 1;
					break;
				case Locations.Bottom:
					border.Y++;
					border.Height -= (info.CaptionBounds.Height / 2 + 1);
					break;
				case Locations.Top:
					border.Height -= (info.CaptionBounds.Height / 2 + 1);
					border.Y += info.CaptionBounds.Height / 2 + 1;
					break;
			}
			return border;
		}
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			info.Appearance.DrawBackground(info.Cache, info.Bounds, true);
		}
		protected override void DrawBorder(GroupObjectInfoArgs info) {
			Rectangle t = info.TextBounds;
			GraphicsClipState clipState = info.Cache.ClipInfo.SaveClip();
			info.Cache.ClipInfo.ExcludeClip(info.TextBounds);
			info.Cache.ClipInfo.ExcludeClip(info.CaptionImageBounds);
			info.Cache.ClipInfo.ExcludeClip(info.ButtonsPanelBounds);
			Rectangle border = GetBorderBounds(info);
			ObjectInfoArgs bInfo = GetBorderInfo(info, border);
			ObjectPainter.DrawObject(info.Cache, GetBorderPainter(info), bInfo);
			info.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		protected override void DrawCaption(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return;
			if(RaiseCustomDrawCaption(info)) return;
			DrawButtonsPanel(info);
			DrawVString(info.Cache, info.AppearanceCaption, info.Caption, info.TextBounds, GetRotateAngle(info));
		}
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.BorderStyle != BorderStyles.Default) return base.GetBorderPainter(e);
			return new FlatGroupBorderPainter();
		}
		protected class FlatGroupBorderPainter : BorderPainter {
			public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
				Rectangle r = e.Bounds;
				r.Inflate(-2, -2);
				return r;
			}
			public override void DrawObject(ObjectInfoArgs e) {
				Color color = GetBorderColor(e);
				Pen light = e.Cache.GetPen(ControlPaint.Light(color, 1f));
				Pen dark = e.Cache.GetPen(ControlPaint.Dark(color, 0f));
				Rectangle r = e.Bounds;
				e.Paint.DrawLine(e.Graphics, dark, new Point(r.X, r.Y), new Point(r.Right - 2, r.Y));
				e.Paint.DrawLine(e.Graphics, light, new Point(r.X + 1, r.Y + 1), new Point(r.Right - 3, r.Y + 1));
				e.Paint.DrawLine(e.Graphics, dark, new Point(r.Right - 2, r.Y + 1), new Point(r.Right - 2, r.Bottom - 2));
				e.Paint.DrawLine(e.Graphics, light, new Point(r.Right - 1, r.Y), new Point(r.Right - 1, r.Bottom - 1));
				e.Paint.DrawLine(e.Graphics, dark, new Point(r.X, r.Bottom - 2), new Point(r.Right - 2, r.Bottom - 2));
				e.Paint.DrawLine(e.Graphics, light, new Point(r.X, r.Bottom - 1), new Point(r.Right - 1, r.Bottom - 1));
				e.Paint.DrawLine(e.Graphics, dark, new Point(r.X, r.Bottom - 2), new Point(r.X, r.Y));
				e.Paint.DrawLine(e.Graphics, light, new Point(r.X + 1, r.Bottom - 3), new Point(r.X + 1, r.Y + 1));
			}
			public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
				Rectangle r = client;
				r.Inflate(2, 2);
				return r;
			}
		}
	}
	public class RotateObjectPaintHelper {
		public RotateFlipType GetButtonRotateFlipType(GroupObjectInfoArgs info) {
			bool flip = (info.ButtonLocation == GroupElementLocation.AfterText) && !info.Expanded;
			switch(info.CaptionLocation) {
				case Locations.Left:
					return flip ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate270FlipNone;
				case Locations.Right:
					return flip ? RotateFlipType.Rotate90FlipY : RotateFlipType.Rotate90FlipNone;
				default:
					return flip ? RotateFlipType.RotateNoneFlipX : RotateFlipType.RotateNoneFlipNone;
			}
		}
		public RotateFlipType RotateFlipTypeByRotationAngle(RotationAngle angle, bool shouldFlip = false) {
			return RotateFlipTypeByRotationAngle((int)angle, shouldFlip);
		}
		public RotateFlipType RotateFlipTypeByRotationAngle(int angle, bool shouldFlip) {
			switch(angle) {
				case 270:
					return shouldFlip ? RotateFlipType.Rotate270FlipY : RotateFlipType.Rotate270FlipNone;
				case 90:
					return shouldFlip ? RotateFlipType.Rotate90FlipY : RotateFlipType.Rotate90FlipNone;
				default:
					return shouldFlip ? RotateFlipType.RotateNoneFlipX : RotateFlipType.RotateNoneFlipNone;
			}
		}
		public RotateFlipType RotateFlipTypeByCaptionLocation(Locations location, bool flip = false) {
			switch(location) {
				case Locations.Left:
					return RotateFlipType.Rotate270FlipNone;
				case Locations.Right:
					return RotateFlipType.Rotate90FlipNone;
				case Locations.Bottom:
					return flip ? RotateFlipType.RotateNoneFlipY : RotateFlipType.RotateNoneFlipNone;
				default :
					return RotateFlipType.RotateNoneFlipNone;
			}
		}
		public void DrawRotated(GraphicsCache cache, ObjectInfoArgs info, ObjectPainter painter, RotateFlipType rotate) {
			DrawRotated(cache, info, painter, rotate, true);
		}
		protected virtual bool ShouldSwapElementSize(RotateFlipType rotate) {
			return (
					rotate == RotateFlipType.Rotate90FlipNone ||
					rotate == RotateFlipType.Rotate90FlipX ||
					rotate == RotateFlipType.Rotate90FlipXY ||
					rotate == RotateFlipType.Rotate90FlipY
				);
		}
		public virtual void DrawRotated(GraphicsCache cache, ObjectInfoArgs info, ObjectPainter painter, RotateFlipType rotate, bool alwaysCreate) {
			if(info.Bounds.IsEmpty || info.Bounds.Width < 1 || info.Bounds.Height < 1) return;
			if(rotate == RotateFlipType.RotateNoneFlipNone) {
				ObjectPainter.DrawObject(cache, painter, info);
				return;
			}
			if(painter is SkinElementPainter && info is SkinElementInfo) {
				DrawRotatedSkinElement(cache, (SkinElementPainter)painter, (SkinElementInfo)info, rotate);
				return;
			}
			GraphicsCache save = info.Cache;
			Rectangle saveBounds = info.Bounds;
			try {
				Size topSize = info.Bounds.Size;
				if(ShouldSwapElementSize(rotate)) {
					topSize.Width = info.Bounds.Height;
					topSize.Height = info.Bounds.Width;
				}
				Bitmap bmp = BitmapRotate.CreateBufferBitmap(topSize, alwaysCreate);
				info.Bounds = new Rectangle(Point.Empty, topSize);
				BitmapRotate.ClearGraphics(Color.Transparent);
				ObjectPainter.DrawObject(BitmapRotate.BufferCache, painter, info);
				BitmapRotate.RotateBitmap(rotate);
				info.Bounds = saveBounds;
				cache.Paint.DrawImage(cache.Graphics, BitmapRotate.BufferBitmap, info.Bounds, new Rectangle(Point.Empty, info.Bounds.Size), true);
			}
			finally {
				info.Bounds = saveBounds;
				info.Cache = save;
			}
		}
		protected virtual void DrawRotatedSkinElement(GraphicsCache cache, SkinElementPainter painter, SkinElementInfo info, RotateFlipType rotate) {
			string name = info.Element.ElementName + rotate.ToString();
			SkinElement prevElement = info.Element;
			SkinElement element = info.Element.Owner[name];
			if(element == null)
				element = CreateRotatedElement(info.Element, name, rotate);
			info.Element = element;
			try {
				ObjectPainter.DrawObject(cache, painter, info);
			}
			finally { info.Element = prevElement; }
		}
		protected virtual SkinElement CreateRotatedElement(SkinElement source, string name, RotateFlipType rotate) {
			return CreateRotatedElement(source, name, rotate, true);
		}
		protected virtual SkinElement CreateRotatedElement(SkinElement source, string name, RotateFlipType rotate, bool rotateContentMargins) {
			SkinElement res = source.Copy(source.Owner, name);
			res.Original = source;
			res.IsCustomElement = true;
			SetRotatedInfo(res.Info, source.Info, rotate, rotateContentMargins);
			foreach(object key in source.ScaledInfo.Keys) {
				if(res.ScaledInfo.ContainsKey((float)key))
					SetRotatedInfo(res.ScaledInfo[(float)key], source.ScaledInfo[(float)key], rotate, rotateContentMargins);
			}
			source.Owner.AddElement(res);
			return res;
		}
		private void SetRotatedInfo(SkinBuilderElementInfo info, SkinBuilderElementInfo sourceInfo, RotateFlipType rotate, bool rotateContentMargins) {
			info.Border = RotateSkinBorder(sourceInfo.Border, rotate);
			if(rotateContentMargins)
				info.ContentMargins = RotateSkinPaddingEdges(sourceInfo.ContentMargins, rotate);
			info.Size = RotateSkinSize(sourceInfo.Size, rotate);
			info.Image = RotateSkinImage(sourceInfo.Image, rotate);
			info.Glyph = RotateSkinGlyph(sourceInfo.Glyph, rotate);
		}
		protected SkinGlyph RotateSkinGlyph(SkinGlyph skinGlyph, RotateFlipType rotate) {
			return (SkinGlyph)RotateSkinImage(skinGlyph, rotate);
		}
		protected SkinImage RotateSkinImage(SkinImage skinImage, RotateFlipType rotate) {
			if(skinImage == null)
				return null;
			if(skinImage.Image == null)
				return skinImage.New(null);
			Size sz = RotateSize(skinImage.GetImageSize(), rotate);
			Bitmap img = new Bitmap(skinImage.Image, sz.Width, sz.Height * skinImage.ImageCount);
			img.SetResolution(skinImage.Image.HorizontalResolution, skinImage.Image.VerticalResolution);
			RotateFlipType prev = BitmapRotate.GetMirroredRotateFlipType(rotate);
			int imageIndex = 0;
			using(Graphics g = Graphics.FromImage(img)) {
				g.Clear(Color.Transparent);
				ImageCollection coll = skinImage.GetImages();
				foreach(Image simg in coll.Images) {
					Bitmap tmp = new Bitmap(simg);
					tmp.SetResolution(simg.HorizontalResolution, simg.VerticalResolution);
					using(Graphics tmpg = Graphics.FromImage(tmp)) {
						tmpg.Clear(Color.Transparent);
						tmpg.DrawImage(simg, Point.Empty);
						tmp.RotateFlip(rotate);
						g.DrawImage(tmp, new Rectangle(0, tmp.Height * imageIndex, tmp.Width, tmp.Height));
						imageIndex++;
					}
					tmp.Dispose();
				}
			}
			SkinImage si = (SkinImage)skinImage.GetType().GetConstructor(new Type[] { typeof(Image) }).Invoke(new object[] { img });
			si.ImageCount = skinImage.ImageCount;
			si.SetImageNameCore(skinImage.ImageName);
			si.Layout = skinImage.Layout;
			si.SizingMargins = RotateSkinPaddingEdges(skinImage.SizingMargins, rotate);
			si.Stretch = skinImage.Stretch;
			si.TransparentColor = skinImage.TransparentColor;
			si.UseOwnImage = skinImage.UseOwnImage;
			return si;
		}
		protected SkinBorder RotateSkinBorder(SkinBorder source, RotateFlipType rotate) {
			SkinBorder border = new SkinBorder();
			if(rotate == RotateFlipType.Rotate90FlipNone) {
				border.Left = source.Bottom;
				border.Top = source.Left;
				border.Right = source.Top;
				border.Bottom = source.Right;
			}
			else if(rotate == RotateFlipType.Rotate180FlipNone) {
				border.Left = border.Right;
				border.Top = border.Bottom;
				border.Right = border.Left;
				border.Bottom = border.Top;
			}
			else if(rotate == RotateFlipType.Rotate270FlipNone) {
				border.Left = border.Top;
				border.Top = border.Right;
				border.Right = border.Bottom;
				border.Bottom = border.Left;
			}
			else if(rotate == RotateFlipType.Rotate180FlipY) {
				border.Left = border.Right;
				border.Top = border.Top;
				border.Right = border.Left;
				border.Bottom = border.Bottom;
			}
			else if(rotate == RotateFlipType.Rotate90FlipX) {
				border.Left = source.Top;
				border.Top = source.Left;
				border.Right = source.Bottom;
				border.Bottom = source.Right;
			}
			else if(rotate == RotateFlipType.RotateNoneFlipY) {
				border.Left = source.Left;
				border.Top = source.Bottom;
				border.Right = source.Right;
				border.Bottom = source.Top;
			}
			else if(rotate == RotateFlipType.Rotate90FlipY) {
				border.Left = source.Bottom;
				border.Top = source.Right;
				border.Right = source.Top;
				border.Bottom = source.Left;
			}
			return border;
		}
		protected SkinPaddingEdges RotateSkinPaddingEdges(SkinPaddingEdges source, RotateFlipType rotate) {
			SkinPaddingEdges padding = new SkinPaddingEdges();
			if(rotate == RotateFlipType.Rotate90FlipNone) {
				padding.Left = source.Bottom;
				padding.Top = source.Left;
				padding.Right = source.Top;
				padding.Bottom = source.Right;
			}
			else if(rotate == RotateFlipType.Rotate180FlipNone) {
				padding.Left = source.Right;
				padding.Top = source.Bottom;
				padding.Right = source.Left;
				padding.Bottom = source.Top;
			}
			else if(rotate == RotateFlipType.Rotate270FlipNone) {
				padding.Left = source.Top;
				padding.Top = source.Right;
				padding.Right = source.Bottom;
				padding.Bottom = source.Left;
			}
			else if(rotate == RotateFlipType.Rotate180FlipY) {
				padding.Left = source.Right;
				padding.Top = source.Top;
				padding.Right = source.Left;
				padding.Bottom = source.Bottom;
			}
			else if(rotate == RotateFlipType.Rotate90FlipX) {
				padding.Left = source.Top;
				padding.Top = source.Left;
				padding.Right = source.Bottom;
				padding.Bottom = source.Right;
			}
			else if(rotate == RotateFlipType.RotateNoneFlipY) {
				padding.Left = source.Left;
				padding.Top = source.Bottom;
				padding.Right = source.Right;
				padding.Bottom = source.Top;
			}
			else if(rotate == RotateFlipType.Rotate90FlipY) {
				padding.Left = source.Bottom;
				padding.Top = source.Right;
				padding.Right = source.Top;
				padding.Bottom = source.Left;
			}
			return padding;
		}
		protected Size RotateSize(Size source, RotateFlipType rotate) {
			if(rotate == RotateFlipType.Rotate90FlipNone || rotate == RotateFlipType.Rotate270FlipNone || rotate == RotateFlipType.Rotate90FlipX || rotate == RotateFlipType.Rotate90FlipY) {
				return new Size(source.Height, source.Width);
			}
			return source;
		}
		protected SkinSize RotateSkinSize(SkinSize source, RotateFlipType rotate) {
			SkinSize res = new SkinSize();
			res.MinSize = RotateSize(source.MinSize, rotate);
			res.AllowHGrow = source.AllowHGrow;
			res.AllowVGrow = source.AllowVGrow;
			if(rotate == RotateFlipType.Rotate90FlipNone || rotate == RotateFlipType.Rotate270FlipNone ||
				rotate == RotateFlipType.Rotate90FlipX || rotate == RotateFlipType.Rotate90FlipY) {
				res.AllowHGrow = source.AllowVGrow;
				res.AllowVGrow = source.AllowHGrow;
			}
			return res;
		}
	}
	public class ImagePainterInfo : StyleObjectInfoArgs {
		protected Size imgRealSizeCore;
		protected Image imageCore;
		public ImagePainterInfo(GraphicsCache cache, Rectangle bounds, Size imgRealSize, Image image, AppearanceObject appearance)
			: base(cache, bounds, appearance) {
			this.imgRealSizeCore = imgRealSize;
			this.imageCore = image;
		}
		public Size ImgRealSize { get { return imgRealSizeCore; } set { imgRealSizeCore = value; } }
		public Image Image { get { return imageCore; } set { imageCore = value; } }
		public ImageAttributes ImageAttributes { get; set; }
	}
	public class ImagePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ImagePainterInfo info = e as ImagePainterInfo;
			if(info != null) {
				info.Cache.Paint.DrawImage(info.Cache.Graphics, info.Image, info.Bounds, new Rectangle(Point.Empty, info.ImgRealSize), info.State != ObjectState.Disabled);
			}
		}
	}
	public class ColoredImagePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ImagePainterInfo info = e as ImagePainterInfo;
			if(info != null) {
				info.Cache.Paint.DrawImage(info.Cache.Graphics, info.Image, info.Bounds, new Rectangle(Point.Empty, info.ImgRealSize), info.ImageAttributes);
			}
		}
	}
	public class GroupObjectPainter : StyleObjectPainter {
		IPanelControlOwner owner;
		public virtual int ButtonToBorderDistance { get { return 4; } }
		public virtual int ButtonToTextBlockDistance { get { return 7; } }
		public virtual int CaptionImageToTextDistance { get { return 3; } }
		public GroupObjectPainter(IPanelControlOwner owner) {
			this.owner = owner;
		}
		public virtual ObjectPainter ButtonPainter {
			get {
				return CreateButtonPainter();
			}
		}
		protected virtual ObjectPainter CreateButtonPainter() { return new ExplorerBarOpenCloseButtonObjectPainter(); }
		protected virtual ObjectInfoArgs CreateButtonInfo(GroupObjectInfoArgs info) {
			ExplorerBarOpenCloseButtonInfoArgs btn = new ExplorerBarOpenCloseButtonInfoArgs(null, info.ButtonBounds, info.AppearanceCaption, info.ButtonState, info.Expanded);
			return btn;
		}
		public virtual bool IsAllowParentBackColor { get { return false; } }
		public IPanelControlOwner Owner { get { return owner; } }
		public override AppearanceDefault DefaultAppearance { get { return new AppearanceDefault(Color.Black, SystemColors.Window, Office2003Colors.Default[Office2003Color.NavPaneBorderColor]); } }
		public virtual AppearanceDefault DefaultAppearanceCaption { get { return new AppearanceDefault(SystemColors.Window, ControlPaint.Light(Office2003Colors.Default[Office2003Color.NavBarNavPaneHeaderBackColor]), Color.Empty, ControlPaint.Dark(Office2003Colors.Default[Office2003Color.NavBarNavPaneHeaderBackColor], 0.05f), LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center, new Font(AppearanceObject.DefaultFont, FontStyle.Bold)); } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.IsReady) return info.ClientBounds;
			CalcObjectBounds(e);
			return info.ControlClientBounds;
		}
		protected internal void UpdateAppearance(GroupObjectInfoArgs info) {
			info.AppearanceCaption.RotateBackColors(GetRotateAngle(info));
		}
		protected virtual Rectangle GetBorderBounds(GroupObjectInfoArgs info) { return info.Bounds; }
		protected virtual ObjectInfoArgs GetBorderInfo(GroupObjectInfoArgs info, Rectangle border) {
			return new BorderObjectInfoArgs(info.Cache, border, info.Appearance, ObjectState.Selected);
		}
		public virtual int CalcCenterValue(int boundBegin, int boundSize, int itemSize) {
			return boundBegin + (boundSize - itemSize) / 2;
		}
		public virtual Rectangle CalcContentImageRectangle(GroupObjectInfoArgs info) {
			if(info.ContentImage == null) return Rectangle.Empty;
			Rectangle imageRect = new Rectangle(info.ControlClientBounds.Location, info.ContentImage.Size);
			switch(info.ContentImageAlignment) {
				case ContentAlignment.TopCenter:
					imageRect.X = CalcCenterValue(info.ControlClientBounds.X, info.ControlClientBounds.Width, imageRect.Width);
					break;
				case ContentAlignment.TopRight:
					imageRect.X = info.ControlClientBounds.Right - imageRect.Width;
					break;
				case ContentAlignment.MiddleLeft:
					imageRect.Y = CalcCenterValue(info.ControlClientBounds.Y, info.ControlClientBounds.Height, imageRect.Height);
					break;
				case ContentAlignment.MiddleCenter:
					imageRect.Y = CalcCenterValue(info.ControlClientBounds.Y, info.ControlClientBounds.Height, imageRect.Height);
					imageRect.X = CalcCenterValue(info.ControlClientBounds.X, info.ControlClientBounds.Width, imageRect.Width);
					break;
				case ContentAlignment.MiddleRight:
					imageRect.X = info.ControlClientBounds.Right - imageRect.Width;
					imageRect.Y = CalcCenterValue(info.ControlClientBounds.Y, info.ControlClientBounds.Height, imageRect.Height);
					break;
				case ContentAlignment.BottomLeft:
					imageRect.Y = info.ControlClientBounds.Bottom - imageRect.Height;
					break;
				case ContentAlignment.BottomCenter:
					imageRect.X = CalcCenterValue(info.ControlClientBounds.X, info.ControlClientBounds.Width, imageRect.Width);
					imageRect.Y = info.ControlClientBounds.Bottom - imageRect.Height;
					break;
				case ContentAlignment.BottomRight:
					imageRect.Y = info.ControlClientBounds.Bottom - imageRect.Height;
					imageRect.X = info.ControlClientBounds.Right - imageRect.Width;
					break;
			}
			return imageRect;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(!info.IsReady) CalcObjectBounds(e);
			int xd = info.ControlClientBounds.X - info.Bounds.X;
			Rectangle res = client;
			res.X -= (info.ControlClientBounds.X - info.Bounds.X);
			res.Width += (info.ControlClientBounds.X - info.Bounds.X) + (info.Bounds.Right - info.ControlClientBounds.Right);
			res.Y -= (info.ControlClientBounds.Y - info.Bounds.Y);
			res.Height += (info.ControlClientBounds.Y - info.Bounds.Y) + (info.Bounds.Bottom - info.ControlClientBounds.Bottom);
			return res;
		}
		protected Rectangle MoveRectangle(Rectangle source, int x, int y) {
			if(source == Rectangle.Empty) return source;
			return new Rectangle(new Point(source.X + x, source.Y + y), source.Size);
		}
		protected virtual void DrawButton(GroupObjectInfoArgs info) { }
		protected virtual Size CalcCaptionButtonSize(GroupObjectInfoArgs info) { return Size.Empty; }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			bool createGraphics = false;
			if(info.Cache == null || info.Cache.Graphics == null) {
				info.GInfo.AddGraphics(null);
				info.Cache = info.GInfo.Cache;
				createGraphics = true;
			}
			try {
				if(info.ButtonsPanel != null) {
					info.ButtonsPanel.ViewInfo.SetDirty();
					UpdateExpandButtonVisibility(info);
					info.ButtonsPanel.Buttons.Merge(info.ButtonsPanelOwner.CustomHeaderButtons);
				}
				info.TextBounds = info.CaptionBounds = info.ButtonsPanelBounds = info.CaptionImageBounds = Rectangle.Empty;
				info.ClientBounds = GetBoundsWithoutBorders(info, info.Bounds);
				if(info.ShowCaption) {
					ApplyButtonsPanelProperties(info);
					CalcCaptionNew(info);
				}
				info.ControlClientBounds = CalcControlBounds(info);
				info.ContentImageRectangle = CalcContentImageRectangle(info);
			}
			finally {
				if(createGraphics) {
					info.GInfo.ReleaseGraphics();
					info.Cache = null;
				}
			}
			info.IsReady = true;
			return e.Bounds;
		}
		protected virtual void ApplyButtonsPanelProperties(GroupObjectInfoArgs info) {
			if(!info.ShowButtonsPanel) return;
			info.ButtonsPanel.BeginUpdate();
			info.ButtonsPanel.RightToLeft = (info.CaptionLocation == Locations.Left) ^ info.ButtonsPanelOwner.IsRightToLeft;
			if(info.CaptionLocation == Locations.Left || info.CaptionLocation == Locations.Right) {
				info.ButtonsPanel.Orientation = Orientation.Vertical;
				info.ButtonsPanel.ButtonRotationAngle = (RotationAngle)GetRotateAngle(info);
			}
			else {
				info.ButtonsPanel.Orientation = Orientation.Horizontal;
				info.ButtonsPanel.ButtonRotationAngle = (RotationAngle)GetRotateAngle(info);
			}
			info.ButtonsPanel.CancelUpdate();
		}
		protected internal Rectangle GetBoundsWithoutBorders(GroupObjectInfoArgs e, Rectangle client) {
			return GetBorderPainter(e).GetObjectClientRectangle(GetBorderInfo(e, client));
		}
		protected virtual ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			BorderStyles border = info.BorderStyle;
			if(border == BorderStyles.Default) border = BorderStyles.Simple;
			return BorderHelper.GetPainter(border);
		}
		protected internal virtual Locations GetCaptionLocation(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.CaptionLocation != Locations.Default) return info.CaptionLocation;
			return Locations.Top;
		}
		protected virtual Rectangle CalcControlBounds(GroupObjectInfoArgs info) {
			if(GetBorderPainter(info).GetObjectClientRectangle(GetBorderInfo(info, info.Bounds)) == info.Bounds) return info.ClientBounds;
			return Rectangle.Inflate(info.ClientBounds, -1, -1);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override void DrawObject(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = e as GroupObjectInfoArgs;
			DrawBackground(info);
			DrawBorder(info);
			DrawBackgroundImage(info);
			DrawContentImage(info);
			if(info.ClientBounds.Contains(e.Cache.PaintArgs.ClipRectangle)) return;
			DrawCaption(info);
			DrawCaptionImage(info);
		}
		protected virtual void DrawContentImage(GroupObjectInfoArgs info) {
			if(info.ContentImage == null) return;
			using(Region saveClip = info.Graphics.Clip) {
				info.Graphics.Clip = new Region(info.ControlClientBounds);
				info.Cache.Paint.DrawImage(info.Graphics, info.ContentImage, info.ContentImageRectangle);
				info.Graphics.Clip = saveClip;
			}
		}
		protected virtual void DrawCaptionImage(GroupObjectInfoArgs info) {
			if(!info.ShowCaption || info.CaptionImageBounds.IsEmpty || info.CaptionImage == null) return;
			Size imgRealSize = info.CaptionImage.Size;
			Rectangle imageRect = new Rectangle(new Point(info.CaptionImageBounds.X + (IsVerticalCaption(info) ? info.CaptionImagePadding.Top : info.CaptionImagePadding.Left), info.CaptionImageBounds.Y + (IsVerticalCaption(info) ? info.CaptionImagePadding.Left : info.CaptionImagePadding.Top)), IsVerticalCaption(info) ? new Size(info.CaptionImageBounds.Size.Width - info.CaptionImagePadding.Size.Width, info.CaptionImageBounds.Size.Height - info.CaptionImagePadding.Size.Height) : imgRealSize);
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			ImagePainterInfo painterInfo = new ImagePainterInfo(info.Cache, imageRect, imgRealSize, info.CaptionImage, AppearanceObject.EmptyAppearance);
			if(info.ShouldUseGlyphSkinning)
				painterInfo.ImageAttributes = ImageColorizer.GetColoredAttributes(info.AppearanceCaption.ForeColor);
			painterInfo.State = info.State;
			ObjectPainter imagePainter = info.ShouldUseGlyphSkinning ?
				(ObjectPainter)new ColoredImagePainter() : (ObjectPainter)new ImagePainter();
			rotateHelper.DrawRotated(info.Cache, painterInfo, imagePainter, rotateHelper.RotateFlipTypeByCaptionLocation(info.CaptionLocation));
		}
		protected virtual void DrawButtonsPanel(GroupObjectInfoArgs info) {
			if(info.ButtonsPanelBounds.IsEmpty) return;
			ObjectPainter.DrawObject(info.Cache, info.ButtonsPanelOwner.GetPainter(), (ObjectInfoArgs)info.ButtonsPanel.ViewInfo);
		}
		protected virtual void DrawBackgroundImage(GroupObjectInfoArgs info) {
			if(info.BackgroundImage != null && info.ShowBackgroundImage) {
				ObjectPainter borderPainter = GetBorderPainter(info);
				Rectangle rect = borderPainter.GetObjectClientRectangle(GetBorderInfo(info, GetBorderBounds(info)));
				BackgroundImagePainter.DrawBackgroundImage(info.Graphics, info.BackgroundImage, info.Appearance.BackColor, info.BackgroundImageLayout, rect, rect, Point.Empty, RightToLeft.No);
			}
		}
		protected virtual void DrawBorder(GroupObjectInfoArgs info) {
			GetBorderPainter(info).DrawObject(GetBorderInfo(info, info.Bounds));
		}
		protected bool RaiseCustomDrawCaption(GroupObjectInfoArgs info) {
			GroupCaptionCustomDrawEventArgs e = new GroupCaptionCustomDrawEventArgs(info.Cache, this, info);
			string caption = e.Info.Caption;
			if(Owner != null)
				Owner.OnCustomDrawCaption(e);
			if(caption != e.Info.Caption && e.Info.ShowCaption) {
				var paintArgs = info.Cache.PaintArgs;
				CalcCaptionElements(info, info.CaptionBounds);
				info.Cache.SetPaintArgs(paintArgs);
			}
			return e.Handled;
		}
		protected virtual void DrawCaption(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return;
			info.AppearanceCaption.DrawBackground(info.Cache, info.CaptionBounds);
			if(RaiseCustomDrawCaption(info)) return;
			DrawCaptionText(info);
			DrawButtonsPanel(info);
		}
		protected virtual void DrawCaptionText(GroupObjectInfoArgs info) {
			if(!info.TextBounds.IsEmpty)
				DrawVString(info.Cache, info.AppearanceCaption, info.Caption, info.TextBounds, GetRotateAngle(info));
		}
		protected virtual void DrawBackground(GroupObjectInfoArgs info) {
			info.Appearance.FillRectangle(info.Cache, info.ClientBounds);
		}
		protected virtual int CalcCaptionContentHeight(GroupObjectInfoArgs info) {
			bool panelIsHorizontal = info.ShowButtonsPanel ? info.ButtonsPanel.Orientation == Orientation.Horizontal : false;
			int contentHeight = Math.Max(CalcCaptionTextSize(info).Height, panelIsHorizontal ? CalcCaptionButtonsPanelMinSize(info).Height : CalcCaptionButtonsPanelMinSize(info).Width);
			return Math.Max(CalcCaptionImageSize(info).Height, contentHeight) + 4;
		}
		protected virtual Size CalcCaptionTextSize(GroupObjectInfoArgs info) {
			if(info.ShouldDrawAllowHtmlText)
				return StringPainter.Default.Calculate(info.Graphics, info.AppearanceCaption, info.AppearanceCaption.GetTextOptions(), info.Caption, info.CaptionBounds.Width, null, info).Bounds.Size;
			return info.AppearanceCaption.CalcDefaultTextSize(info.Graphics);
		}
		protected virtual Size CalcCaptionImageSize(GroupObjectInfoArgs info) {
			if(info.CaptionImage == null || !info.ShowCaptionImage) return Size.Empty;
			Size imageSize = new Size(info.CaptionImage.Width + info.CaptionImagePadding.Horizontal, info.CaptionImage.Height + info.CaptionImagePadding.Vertical);
			return imageSize;
		}
		protected virtual Size CalcCaptionButtonsPanelSize(GroupObjectInfoArgs info, Rectangle clientBounds) {
			if(!info.ShowButtonsPanel)
				return Size.Empty;
			info.ButtonsPanel.ViewInfo.Calc(info.Graphics, IsVerticalCaption(info) ? new Rectangle(clientBounds.Location, new Size(clientBounds.Height, clientBounds.Width)) : clientBounds);
			return info.ButtonsPanel.ViewInfo.Bounds.Size;
		}
		protected virtual Size CalcCaptionButtonsPanelMinSize(GroupObjectInfoArgs info) {
			if(!info.ShowButtonsPanel)
				return Size.Empty;
			return info.ButtonsPanel.ViewInfo.CalcMinSize(info.Graphics);
		}
		protected virtual Rectangle InternalUpdateCaptionBounds(GroupObjectInfoArgs info, Rectangle client) {
			return client;
		}
		protected virtual void UpdateExpandButtonVisibility(GroupObjectInfoArgs info) {
			info.ButtonsPanel.BeginUpdate();
			info.ExpandButton.Properties.Visible = info.ShowButton;
			info.ButtonsPanel.CancelUpdate();
		}
		protected virtual void CalcCaptionNew(GroupObjectInfoArgs info) {
			int contentHeight = CalcCaptionContentHeight(info);
			Rectangle client = info.ClientBounds;
			Rectangle caption = InternalUpdateCaptionBounds(info, client);
			Rectangle originalCaption = caption;
			Locations location = GetCaptionLocation(info);
			switch(location) {
				case Locations.Left:
				case Locations.Right:
					caption.Width = contentHeight;
					client.Width -= contentHeight;
					if(location == Locations.Left)
						client.X += contentHeight;
					else
						caption.X = originalCaption.Right - caption.Width;
					break;
				case Locations.Top:
				case Locations.Bottom:
					caption.Height = contentHeight;
					client.Height -= contentHeight;
					if(location == Locations.Top)
						client.Y += contentHeight;
					else
						caption.Y = originalCaption.Bottom - caption.Height;
					break;
			}
			info.CaptionBounds = caption;
			info.ClientBounds = client;
			CalcCaptionElements(info, caption);
		}
		protected virtual int CalcCaptionTextExtraIndent() { return 1; }
		protected virtual void CalcCaptionElements(GroupObjectInfoArgs info, Rectangle caption) {
			switch(info.CaptionLocation) {
				case Locations.Top:
				case Locations.Bottom:
				case Locations.Default:
					CalcCaptionElementsNormal(info, caption);
					break;
				case Locations.Left:
					CalcCaptionElements270(info, caption);
					break;
				case Locations.Right:
					CalcCaptionElements90(info, caption);
					break;
			}
			CalcButtonsPanel(info);
		}
		protected virtual void CalcButtonsPanel(GroupObjectInfoArgs info) {
			if(info.ShowButtonsPanel) {
				info.ButtonsPanel.ViewInfo.SetDirty();
				info.ButtonsPanel.ViewInfo.Calc(info.Graphics, info.ButtonsPanelBounds);
			}
		}
		protected virtual void CalcButtonsPanelLocation(GroupObjectInfoArgs info, ref Rectangle caption, Margins margins) {
			if(!info.ButtonsPanelBounds.Size.IsEmpty) {
				bool isHorizontal = info.ButtonsPanel.Orientation == Orientation.Horizontal;
				int buttonToTextBlockDistance = ButtonToTextBlockDistance;
				int panelWidth = isHorizontal ? info.ButtonsPanelBounds.Width : info.ButtonsPanelBounds.Height;
				int panelHeight = isHorizontal ? info.ButtonsPanelBounds.Height : info.ButtonsPanelBounds.Width;
				int offsetY = (int)((caption.Height - panelHeight) / 2.0 + 0.5);
				if(info.GetButtonLocation() == GroupElementLocation.AfterText) {
					info.ButtonsPanelBounds = MoveRectangle(info.ButtonsPanelBounds, caption.Right - panelWidth, caption.Y + offsetY);
					caption.Width -= (panelWidth + buttonToTextBlockDistance);
				}
				else {
					info.ButtonsPanelBounds = MoveRectangle(info.ButtonsPanelBounds, caption.X, caption.Y + offsetY);
					caption.Width -= (panelWidth + buttonToTextBlockDistance);
					caption.X += (panelWidth + buttonToTextBlockDistance);
				}
			}
		}
		protected virtual void CalcTextAndImageLocation(GroupObjectInfoArgs info, ref Rectangle caption, Margins margins) {
			Rectangle textAndImageRect = new Rectangle(Point.Empty,
				new Size(
					info.TextBounds.Width + info.CaptionImageBounds.Width + (info.CaptionImageBounds.Width != 0 ? CaptionImageToTextDistance : 0),
					Math.Max(info.CaptionImageBounds.Height, info.TextBounds.Height))
				);
			switch(info.GetCaptionTextLocation()) {
				case HorzAlignment.Near:
				case HorzAlignment.Default:
					textAndImageRect = PlacementHelper.Arrange(textAndImageRect.Size, caption, ContentAlignment.MiddleLeft);
					break;
				case HorzAlignment.Far:
					textAndImageRect = PlacementHelper.Arrange(textAndImageRect.Size, caption, ContentAlignment.MiddleRight);
					break;
				case HorzAlignment.Center:
					textAndImageRect = PlacementHelper.Arrange(textAndImageRect.Size, caption, ContentAlignment.MiddleCenter);
					break;
			}
			if(textAndImageRect.Size == Size.Empty) {
				textAndImageRect = Rectangle.Empty;
			}
			if(info.CaptionImageBounds.Size != Size.Empty) {
				GroupElementLocation imageLocation = info.GetCaptionImageLocation();
				if(imageLocation != GroupElementLocation.AfterText) {
					info.CaptionImageBounds = PlacementHelper.Arrange(info.CaptionImageBounds.Size, textAndImageRect, ContentAlignment.MiddleLeft);
					info.TextBounds = PlacementHelper.Arrange(info.TextBounds.Size, textAndImageRect, ContentAlignment.MiddleRight, RoundMode.Down);
				}
				else {
					info.CaptionImageBounds = PlacementHelper.Arrange(info.CaptionImageBounds.Size, textAndImageRect, ContentAlignment.MiddleRight);
					info.TextBounds = PlacementHelper.Arrange(info.TextBounds.Size, textAndImageRect, ContentAlignment.MiddleLeft, RoundMode.Down);
				}
			}
			else {
				info.TextBounds = textAndImageRect;
			}
		}
		protected virtual void CalcCaptionElementsNormal(GroupObjectInfoArgs info, Rectangle caption) {
			Margins margins = GetCorrectMarginsByButtonsPanelLocation(info, caption, GetContentMargins(info));
			CalcElementsSize(info, caption, margins);
			Rectangle captionClientRect = new Rectangle(caption.X + margins.Left, caption.Y + margins.Top, caption.Width - margins.Left - margins.Right, caption.Height - margins.Top - margins.Bottom);
			CalcButtonsPanelLocation(info, ref captionClientRect, margins);
			CalcTextAndImageLocation(info, ref captionClientRect, margins);
		}
		protected virtual void CalcElementsSize(GroupObjectInfoArgs info, Rectangle caption, Margins margins) {
			Rectangle captionClientBounds = new Rectangle(caption.X + margins.Left, caption.Y + margins.Top, caption.Width - margins.Left - margins.Right, caption.Height - margins.Top - margins.Bottom);
			Size imageSize = CalcCaptionImageSize(info);
			int buttonToTextBlockDistance = ButtonToTextBlockDistance;
			int imageToTextDistance = CaptionImageToTextDistance;
			info.ButtonsPanelBounds = new Rectangle(Point.Empty, CalcCaptionButtonsPanelSize(info, captionClientBounds));
			if(info.ButtonsPanelBounds.Width > 0 && info.ButtonsPanelBounds.Height > 0) {
				captionClientBounds.Width -= (IsVerticalCaption(info) ? info.ButtonsPanelBounds.Height : info.ButtonsPanelBounds.Width);
				captionClientBounds.Width -= buttonToTextBlockDistance;
			}
			else
				info.ButtonsPanelBounds = Rectangle.Empty;
			if(!imageSize.IsEmpty && captionClientBounds.Width >= imageSize.Width) {
				info.CaptionImageBounds = new Rectangle(Point.Empty, CalcCaptionImageSize(info));
				if(info.ShowCaptionImage) {
					captionClientBounds.Width -= info.CaptionImageBounds.Size.Width + imageToTextDistance;
				}
			}
			Size textSize;
			if(info.ShouldDrawAllowHtmlText)
				textSize = CalcCaptionTextSize(info);
			else if(IsVerticalCaption(info))
				textSize = info.Cache.Graphics.MeasureString(info.Caption, info.AppearanceCaption.Font).ToSize();
			else
				textSize = info.AppearanceCaption.CalcTextSize(info.Graphics, info.AppearanceCaption.GetStringFormat(info.Appearance.TextOptions), info.Caption, captionClientBounds.Width).ToSize();
			if(textSize.Width > captionClientBounds.Width)
				textSize.Width = captionClientBounds.Width >= 0 ? captionClientBounds.Width : 0;
			if(textSize.Width == 0 || textSize.Height == 0)
				textSize = Size.Empty;
			if(!textSize.IsEmpty)
				textSize.Width += CalcCaptionTextExtraIndent();
			info.TextBounds = new Rectangle(Point.Empty, textSize);
		}
		public Margins GetActualMargins(GroupObjectInfoArgs info) {
			if(!info.IsReady) CalcObjectBounds(info);
			return GetCorrectMarginsByButtonsPanelLocation(info, info.CaptionBounds, GetContentMargins(info));
		}
		protected internal virtual void CalcContentMargins(GroupObjectInfoArgs info, out int left, out int top, out int right, out int bottom) {
			left = 4; top = 4; right = 4; bottom = 4;
		}
		protected internal virtual Margins GetContentMargins(GroupObjectInfoArgs info) {
			int left, top, right, bottom;
			CalcContentMargins(info, out left, out top, out right, out bottom);
			return GetCorrectMarginsByRTL(info, new Margins(left, right, top, bottom));
		}
		protected internal virtual Margins GetCorrectMarginsByButtonsPanelLocation(GroupObjectInfoArgs info, Rectangle caption, Margins margins) {
			Margins originalMargins = new Margins(margins.Left, margins.Right, margins.Top, margins.Bottom);
			if(info.ButtonsPanel != null && info.ButtonsPanel.Buttons.Count == 1 && !info.ShowButton) return margins;
			if(info.GetButtonLocation() == GroupElementLocation.AfterText)
				margins.Right = ButtonToBorderDistance;
			else
				margins.Left = ButtonToBorderDistance;
			return margins;
		}
		protected virtual bool CanUseRotateDrawing {
			get { return true; }
		}
		protected virtual Margins GetCorrectMarginsByRTL(GroupObjectInfoArgs info, Margins margins) {
			if(info.RightToLeft) {
				int left = margins.Left;
				margins.Left = margins.Right;
				margins.Right = left;
			}
			if(info.CaptionLocation == Locations.Bottom && CanUseRotateDrawing) {
				int top = margins.Top;
				margins.Top = margins.Bottom;
				margins.Bottom = top;
			}
			return margins;
		}
		protected void RotateMatrix(Matrix m, int angle) {
			m.Rotate(angle);
			if(angle == 90) {
				m.Elements[0] = 0;
				m.Elements[1] = 1;
				m.Elements[2] = -1;
				m.Elements[3] = 0;
				m.Elements[4] = 0;
				m.Elements[5] = 0;
			}
			if(angle == 270) {
				m.Elements[0] = 0;
				m.Elements[1] = -1;
				m.Elements[2] = 1;
				m.Elements[3] = 0;
				m.Elements[4] = 0;
				m.Elements[5] = 0;
			}
		}
		protected virtual void CalcCaptionElements270(GroupObjectInfoArgs info, Rectangle caption) {
			CalcCaptionElementsNormal(info, new Rectangle(Point.Empty, new Size(caption.Height, caption.Width)));
			Point[] points = new Point[] { info.TextBounds.Location, info.CaptionImageBounds.Location, info.ButtonsPanelBounds.Location };
			Matrix transformMatrix = new Matrix();
			RotateMatrix(transformMatrix, 270);
			transformMatrix.TransformPoints(points);
			transformMatrix.Reset();
			transformMatrix.Translate(caption.Location.X, caption.Location.Y + caption.Height);
			transformMatrix.TransformPoints(points);
			if(!info.TextBounds.Size.IsEmpty)
				info.TextBounds = new Rectangle(new Point(points[0].X, points[0].Y - info.TextBounds.Width), new Size(info.TextBounds.Height, info.TextBounds.Width));
			if(!info.CaptionImageBounds.Size.IsEmpty)
				info.CaptionImageBounds = new Rectangle(new Point(points[1].X, points[1].Y - info.CaptionImageBounds.Width), new Size(info.CaptionImageBounds.Height, info.CaptionImageBounds.Width));
			if(!info.ButtonsPanelBounds.Size.IsEmpty) {
				info.ButtonsPanelBounds = new Rectangle(new Point(points[2].X, points[2].Y - info.ButtonsPanelBounds.Height), info.ButtonsPanelBounds.Size);
			}
		}
		protected virtual void CalcCaptionElements90(GroupObjectInfoArgs info, Rectangle caption) {
			CalcCaptionElementsNormal(info, new Rectangle(Point.Empty, new Size(caption.Height, caption.Width)));
			Point[] points = new Point[] { info.TextBounds.Location, info.CaptionImageBounds.Location, info.ButtonsPanelBounds.Location };
			Matrix transformMatrix = new Matrix();
			RotateMatrix(transformMatrix, 90);
			transformMatrix.TransformPoints(points);
			transformMatrix.Reset();
			transformMatrix.Translate(caption.Location.X + caption.Width, caption.Location.Y);
			transformMatrix.TransformPoints(points);
			if(!info.TextBounds.Size.IsEmpty)
				info.TextBounds = new Rectangle(new Point(points[0].X - info.TextBounds.Height, points[0].Y), new Size(info.TextBounds.Height, info.TextBounds.Width));
			if(!info.CaptionImageBounds.Size.IsEmpty)
				info.CaptionImageBounds = new Rectangle(new Point(points[1].X - info.CaptionImageBounds.Height, points[1].Y), new Size(info.CaptionImageBounds.Height, info.CaptionImageBounds.Width));
			if(!info.ButtonsPanelBounds.Size.IsEmpty) {
				info.ButtonsPanelBounds = new Rectangle(new Point(points[2].X - info.ButtonsPanelBounds.Width, points[2].Y), info.ButtonsPanelBounds.Size);
			}
		}
		protected virtual bool IsVerticalCaption(ObjectInfoArgs e) { return GetRotateAngle(e) != 0; }
		protected virtual int GetRotateAngle(ObjectInfoArgs e) {
			Locations loc = GetCaptionLocation(e);
			switch(loc) {
				case Locations.Left: return 270;
				case Locations.Right: return 90;
			}
			return 0;
		}
		protected virtual TextOptions GetTextOptions() { return new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.None, HKeyPrefix.Show); }
		public virtual void DrawVString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle rect, int angle) {
			using(StringFormat format = appearance.GetStringFormat(appearance.TextOptions).Clone() as StringFormat) {
				appearance.DrawVString(cache, text, appearance.GetFont(), appearance.GetForeBrush(cache), rect, format, angle);
			}
		}
	}
	public class BackgroundImagePainter {
		static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout) {
			Rectangle rectangle = bounds;
			if(backgroundImage != null) {
				switch(imageLayout) {
					case ImageLayout.None:
						rectangle.Size = backgroundImage.Size;
						return rectangle;
					case ImageLayout.Tile:
						return rectangle;
					case ImageLayout.Center: {
							rectangle.Size = backgroundImage.Size;
							Size size = bounds.Size;
							if(size.Width > rectangle.Width) {
								rectangle.X = (size.Width - rectangle.Width) / 2;
							}
							if(size.Height > rectangle.Height) {
								rectangle.Y = (size.Height - rectangle.Height) / 2;
							}
							return rectangle;
						}
					case ImageLayout.Stretch:
						rectangle.Size = bounds.Size;
						return rectangle;
					case ImageLayout.Zoom: {
							Size size2 = backgroundImage.Size;
							float num = ((float)bounds.Width) / ((float)size2.Width);
							float num2 = ((float)bounds.Height) / ((float)size2.Height);
							if(num >= num2) {
								rectangle.Height = bounds.Height;
								rectangle.Width = (int)((size2.Width * num2) + 0.5);
								if(bounds.X >= 0) {
									rectangle.X = (bounds.Width - rectangle.Width) / 2;
								}
								return rectangle;
							}
							rectangle.Width = bounds.Width;
							rectangle.Height = (int)((size2.Height * num) + 0.5);
							if(bounds.Y >= 0) {
								rectangle.Y = (bounds.Height - rectangle.Height) / 2;
							}
							return rectangle;
						}
				}
			}
			return rectangle;
		}
		public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset, RightToLeft rightToLeft) {
			if(backgroundImageLayout == ImageLayout.Tile) {
				using(TextureBrush brush = new TextureBrush(backgroundImage, WrapMode.Tile)) {
					if(scrollOffset != Point.Empty) {
						Matrix transform = brush.Transform;
						transform.Translate((float)scrollOffset.X, (float)scrollOffset.Y);
						brush.Transform = transform;
					}
					g.FillRectangle(brush, clipRect);
					return;
				}
			}
			Rectangle rect = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
			if((rightToLeft == RightToLeft.Yes) && (backgroundImageLayout == ImageLayout.None)) {
				rect.X += clipRect.Width - rect.Width;
			}
			using(SolidBrush brush2 = new SolidBrush(backColor)) {
				g.FillRectangle(brush2, clipRect);
			}
			if(!clipRect.Contains(rect)) {
				if((backgroundImageLayout == ImageLayout.Stretch) || (backgroundImageLayout == ImageLayout.Zoom)) {
					rect.Intersect(clipRect);
					g.DrawImage(backgroundImage, rect);
				}
				else if(backgroundImageLayout == ImageLayout.None) {
					rect.Offset(clipRect.Location);
					Rectangle destRect = rect;
					destRect.Intersect(clipRect);
					Rectangle rectangle3 = new Rectangle(Point.Empty, destRect.Size);
					g.DrawImage(backgroundImage, destRect, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, GraphicsUnit.Pixel);
				}
				else {
					Rectangle rectangle4 = rect;
					rectangle4.Intersect(clipRect);
					Rectangle rectangle5 = new Rectangle(new Point(rectangle4.X - rect.X, rectangle4.Y - rect.Y), rectangle4.Size);
					g.DrawImage(backgroundImage, rectangle4, rectangle5.X, rectangle5.Y, rectangle5.Width, rectangle5.Height, GraphicsUnit.Pixel);
				}
			}
			else {
				ImageAttributes imageAttr = new ImageAttributes();
				imageAttr.SetWrapMode(WrapMode.TileFlipXY);
				g.DrawImage(backgroundImage, rect, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, imageAttr);
				imageAttr.Dispose();
			}
		}
	}
	public class SkinGroupObjectPainter : GroupObjectPainter {
		ISkinProvider provider;
		string groupPanelSkinElementName;
		public override bool IsAllowParentBackColor { get { return false; } }
		public string GroupPanelSkinElementName { get { return groupPanelSkinElementName; } }
		public override int ButtonToBorderDistance {
			get { return GetPanelCaptionSkinElementCore().Properties.GetInteger(CommonSkins.SkinGroupPanelButtonToBorderDistance, base.ButtonToBorderDistance); }
		}
		public override int ButtonToTextBlockDistance {
			get { return GetPanelCaptionSkinElementCore().Properties.GetInteger(CommonSkins.SkinGroupPanelButtonToTextBlockDistance, base.ButtonToTextBlockDistance); }
		}
		public override int CaptionImageToTextDistance {
			get { return GetPanelCaptionSkinElementCore().Properties.GetInteger(CommonSkins.SkinGroupPanelCaptionImageToTextDistance, base.CaptionImageToTextDistance); }
		}
		public SkinGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider, string groupPanelSkinElementName)
			: base(owner) {
			this.provider = provider;
			this.groupPanelSkinElementName = groupPanelSkinElementName;
		}
		public SkinGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider)
			: this(owner, provider, CommonSkins.SkinGroupPanel) { }
		public override AppearanceDefault DefaultAppearance {
			get {
				SkinElement element = null;
				if(!string.IsNullOrEmpty(groupPanelSkinElementName))
					element = GetSkin()[GroupPanelSkinElementName];
				else
					element = GetPanelCaptionSkinElementCore();
				return new AppearanceDefault(element.Color.GetForeColor(), element.Color.GetBackColor());
			}
		}
		public override AppearanceDefault DefaultAppearanceCaption {
			get {
				return GetPanelCaptionSkinElementCore().Apply(new AppearanceDefault(), Provider);
			}
		}
		public ISkinProvider Provider { get { return provider; } }
		protected virtual SkinElement GetExpandButtonSkinElement(GroupObjectInfoArgs info) {
			return GetSkin()[CommonSkins.SkinGroupPanelExpandButton];
		}
		protected override int CalcCaptionContentHeight(GroupObjectInfoArgs info) {
			int maxHeight = CalcCaptionTextSize(info).Height;
			bool panelIsHorizontal = info.ButtonsPanel.Orientation == Orientation.Horizontal;
			maxHeight = Math.Max(maxHeight, panelIsHorizontal ? CalcCaptionButtonsPanelMinSize(info).Height : CalcCaptionButtonsPanelMinSize(info).Width);
			maxHeight = Math.Max(maxHeight, CalcCaptionImageSize(info).Height);
			Rectangle contentBounds = new Rectangle(0, 0, 0, maxHeight);
			return CalcCaptionSizeWithMargins(info, ref contentBounds).Height;
		}
		protected virtual Size CalcCaptionSizeWithMargins(GroupObjectInfoArgs info, ref Rectangle contentBounds) {
			SkinElementInfo elInfo = new SkinElementInfo(GetPanelCaptionSkinElement(info), contentBounds);
			return ObjectPainter.CalcBoundsByClientRectangle(info.Graphics, SkinElementPainter.Default, elInfo, elInfo.Bounds).Size;
		}
		protected internal override void CalcContentMargins(GroupObjectInfoArgs info, out int left, out int top, out int right, out int bottom) {
			SkinElement e = GetPanelCaptionSkinElement(info);
			if(e != null) {
				left = e.ContentMargins.Left;
				top = e.ContentMargins.Top;
				right = e.ContentMargins.Right;
				bottom = e.ContentMargins.Bottom;
			}
			else base.CalcContentMargins(info, out left, out top, out right, out bottom);
		}
		protected virtual SkinElement GetPanelCaptionSkinElement(GroupObjectInfoArgs info) {
			return GetPanelCaptionSkinElementCore();
		}
		protected SkinElement GetPanelCaptionSkinElementCore() {
			return GetSkin()[CommonSkins.SkinGroupPanelCaptionTop];
		}
		protected Skin GetSkin() { return CommonSkins.GetSkin(Provider); }
		protected Color GetCaptionBackColor(GroupObjectInfoArgs info) {
			SkinElement element = GetPanelCaptionSkinElement(info);
			return (element != null) ? element.Color.GetBackColor() : Color.Empty;
		}
		protected SkinElementInfo GetCaptionElement(GroupObjectInfoArgs info) {
			SkinElementInfo e = new SkinElementInfo(GetPanelCaptionSkinElement(info), info.CaptionBounds);
			e.Attributes = info.Attributes;
			e.State = info.State;
			e.ImageIndex = info.StateIndex;
			e.RightToLeftFlipType = info.CaptionLocation == Locations.Left || info.CaptionLocation == Locations.Right ? FlipType.VerticalFlip : FlipType.Default;
			e.RightToLeft = info.RightToLeft;
			return e;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = e as GroupObjectInfoArgs;
			if(info.AllowBorderColorBlending) {
				using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(GetActualCaptionBackColor(info))) {
					base.DrawObject(e);
				}
			}
			else
				base.DrawObject(e);
		}
		protected virtual void DrawCaptionSkinElement(GroupObjectInfoArgs info) {
			SkinElementInfo e = GetCaptionElement(info);
			bool useCustomDraw = e.Element.Properties.GetBoolean(CommonSkins.SkinUseCustomGroupCaptionDrawing, false);
			if(useCustomDraw) {
				UpdateCaptionSkinBounds(info, e);
			}
			if(CanUseRotateDrawing)
				DrawRotatedCaptionSkinElement(info, e);
			else {
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, e);
			}
		}
		protected virtual void DrawRotatedCaptionSkinElement(GroupObjectInfoArgs info, SkinElementInfo skinElementInfo) {
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			RotateFlipType rotateFlipType;
			if(info.CaptionLocation == Locations.Bottom)
				rotateFlipType = RotateFlipType.RotateNoneFlipY;
			else
				rotateFlipType = rotateHelper.RotateFlipTypeByCaptionLocation(info.CaptionLocation);
			rotateHelper.DrawRotated(info.Cache, skinElementInfo, SkinElementPainter.Default , rotateFlipType);
		}
		protected virtual void UpdateCaptionSkinBounds(GroupObjectInfoArgs info, SkinElementInfo e) {
			Size prefferedSize = new Size(e.Bounds.Width, e.Element.Image.Image.Size.Height);
			float customDrawOffset = (float)e.Element.Properties.GetInteger(CommonSkins.SkinCustomDrawCaptionOffset, 1);
			int delta = (int)((float)(e.Bounds.Height - prefferedSize.Height) / 2 + 0.5f - customDrawOffset);
			switch(GetCaptionLocation(info)) {
				case Locations.Default:
				case Locations.Top:
					prefferedSize.Height += delta;
					e.Bounds = PlacementHelper.Arrange(prefferedSize, e.Bounds, ContentAlignment.BottomCenter);
					break;
				case Locations.Left:
					prefferedSize = new Size(e.Element.Image.Image.Size.Width, e.Bounds.Height);
					delta = (int)((float)(e.Bounds.Width - prefferedSize.Width) / 2 + 0.5f - customDrawOffset);
					prefferedSize.Width += delta;
					e.Bounds = PlacementHelper.Arrange(prefferedSize, e.Bounds, ContentAlignment.MiddleRight);
					break;
				case Locations.Right:
					prefferedSize = new Size(e.Element.Image.Image.Size.Width, e.Bounds.Height);
					delta = (int)((float)(e.Bounds.Width - prefferedSize.Width) / 2 + 0.5f - customDrawOffset);
					prefferedSize.Width += delta;
					e.Bounds = PlacementHelper.Arrange(prefferedSize, e.Bounds, ContentAlignment.MiddleLeft);
					break;
				case Locations.Bottom:
					delta = (int)((float)(e.Bounds.Height - prefferedSize.Height) / 2 + 0.5f - customDrawOffset);
					prefferedSize.Height += delta;
					e.Bounds = PlacementHelper.Arrange(prefferedSize, e.Bounds, ContentAlignment.TopCenter);
					break;
			}
		}
		protected override void DrawCaption(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return;
			DrawCaptionSkinElement(info);
			if(RaiseCustomDrawCaption(info)) return;
			DrawCaptionText(info);
			DrawButtonsPanel(info);
		}
		protected override void DrawCaptionText(GroupObjectInfoArgs info) {
			if(info.TextBounds.IsEmpty || info.TextBounds.Height == 0 || info.TextBounds.Width == 0) return;
			Rectangle clippedTextRect = info.TextBounds;
			if(clippedTextRect.Right > info.Bounds.Right) {
				clippedTextRect.Width -= (clippedTextRect.Right - info.Bounds.Right + 2);
			}
			Color textBackground = GetActualCaptionTextBackColor(info);
			if(!textBackground.IsEmpty & info.TextBounds.Width > 0) {
				Rectangle rect = clippedTextRect;
				int textPadding = GetPanelCaptionSkinElement(info).Properties.GetInteger(CommonSkins.OptGroupCaptionTextPadding);
				if(GetCaptionLocation(info) == Locations.Left || GetCaptionLocation(info) == Locations.Right)
					rect.Inflate(0, textPadding);
				else
					rect.Inflate(textPadding, 0);
				info.Cache.FillRectangle(info.Cache.GetSolidBrush(textBackground), rect);
			}
			if(info.ShouldDrawAllowHtmlText && info.TextBounds.Width > 0) {
				StringInfo si = StringPainter.Default.Calculate(info.Graphics, info.AppearanceCaption, info.Appearance.GetTextOptions(), info.Caption, info.TextBounds.Width, null, info);
				si.SetLocation(info.TextBounds.Location);
				StringPainter.Default.DrawString(info.Cache, si);
			}
			else
				DrawVString(info.Cache, info.AppearanceCaption, info.Caption, clippedTextRect, GetRotateAngle(info));
		}
		protected virtual Color GetActualCaptionTextBackColor(GroupObjectInfoArgs info) {
			return info.AllowBorderColorBlending ? SkinImageColorizer.ColorsSmartBlending(GetCaptionBackColor(info), GetActualCaptionBackColor(info)) : GetCaptionBackColor(info);
		}
		protected virtual Color GetActualCaptionBackColor(GroupObjectInfoArgs info) {
			if(!info.AllowBorderColorBlending) return Color.Empty;
			return info.AppearanceCaption.BorderColor.IsEmpty ? info.Appearance.BorderColor : info.AppearanceCaption.BorderColor;
		}
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.BorderStyle == BorderStyles.NoBorder && !info.ShowCaption) return new SkinGroupEmptyBorderPainter(Provider);
			return new SkinGroupBorderPainter(this, Provider);
		}
		protected override Rectangle InternalUpdateCaptionBounds(GroupObjectInfoArgs info, Rectangle client) {
			return info.Bounds;
		}
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			base.DrawBackground(info); 
		}
		protected override void DrawBorder(GroupObjectInfoArgs info) {
			Rectangle border = GetBorderBounds(info);
			ObjectPainter borderPainter = GetBorderPainter(info);
			ObjectInfoArgs borderInfo = GetBorderInfo(info, border);
			ObjectPainter.DrawObject(info.Cache, borderPainter, borderInfo);
		}
		protected override ObjectInfoArgs GetBorderInfo(GroupObjectInfoArgs info, Rectangle border) {
			SkinGroupBorderObjectInfoArgs res = new SkinGroupBorderObjectInfoArgs(info, info.Cache, info.Appearance, border, ObjectState.Selected);
			res.Attributes = info.Attributes;
			return res;
		}
		protected override Rectangle GetBorderBounds(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return info.Bounds;
			Rectangle res = info.Bounds;
			if(IsVerticalCaption(info)) {
				int size = info.CaptionBounds.Width;
				if(GetCaptionLocation(info) == Locations.Left)
					res.X += size;
				res.Width -= size;
			}
			else {
				int size = info.CaptionBounds.Height;
				if(GetCaptionLocation(info) == Locations.Top)
					res.Y += size;
				res.Height -= size;
			}
			return res;
		}
	}
	public class SkinGroupBorderObjectInfoArgs : BorderObjectInfoArgs {
		GroupObjectInfoArgs groupInfo;
		ImageAttributes attributes;
		public SkinGroupBorderObjectInfoArgs(GroupObjectInfoArgs groupInfo, GraphicsCache cache, AppearanceObject appearance, Rectangle bounds, ObjectState state)
			:
			base(cache, appearance, bounds, state) {
			this.groupInfo = groupInfo;
		}
		public GroupObjectInfoArgs GroupInfo { get { return groupInfo; } }
		public ImageAttributes Attributes { get { return attributes; } set { attributes = value; } }
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(groupInfo != null) groupInfo.OffsetContent(x, y);
		}
	}
	public class SkinGroupEmptyBorderPainter : EmptyBorderPainter {
		ISkinProvider provider;
		public SkinGroupEmptyBorderPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider {
			get { return provider; }
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinGroupBorderObjectInfoArgs info = (SkinGroupBorderObjectInfoArgs)e;
			SkinElementInfo elementInfo = new SkinElementInfo(GetNoBorderGroupSkinElement(info.GroupInfo), e.Bounds);
			elementInfo.BackAppearance = info.Appearance;
			DrawNoBorderBackground(e, elementInfo);
		}
		protected virtual void DrawNoBorderBackground(ObjectInfoArgs e, SkinElementInfo elementInfo) {
			SkinGroupBorderObjectInfoArgs info = (SkinGroupBorderObjectInfoArgs)e;
			if(info.GroupInfo.DrawUserBackground)
				return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual SkinElement GetNoBorderGroupSkinElement(GroupObjectInfoArgs info) {
			return GetSkin()[CommonSkins.SkinGroupPanelNoBorder];
		}
		Skin GetSkin() { return CommonSkins.GetSkin(Provider); }
	}
	public class SkinGroupBorderPainter : SkinBorderPainter {
		SkinGroupObjectPainter groupPainter;
		public SkinGroupBorderPainter(SkinGroupObjectPainter groupPainter, ISkinProvider provider)
			: base(provider) {
			this.groupPainter = groupPainter;
		}
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			SkinGroupBorderObjectInfoArgs info = (SkinGroupBorderObjectInfoArgs)e;
			SkinElementInfo res = new SkinElementInfo(GetPanelSkinElement(info.GroupInfo));
			res.Attributes = info.Attributes;
			return res;
		}
		protected override void DrawObjectCore(SkinElementInfo info, ObjectInfoArgs e) {
			var ea = e as SkinGroupBorderObjectInfoArgs;
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			if(ea != null && ea.GroupInfo.AllowBorderColorBlending) {
				using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(info, GetActualBorderColor(ea.GroupInfo))) {
					rotateHelper.DrawRotated(e.Cache, info, SkinElementPainter.Default, rotateHelper.RotateFlipTypeByCaptionLocation(ea.GroupInfo.CaptionLocation, true));
				}
			}
			else
				rotateHelper.DrawRotated(e.Cache, info, SkinElementPainter.Default, rotateHelper.RotateFlipTypeByCaptionLocation(ea.GroupInfo.CaptionLocation, true));
		}
		protected virtual Color GetActualBorderColor(GroupObjectInfoArgs info) {
			if(!info.AllowBorderColorBlending) return Color.Empty;
			return info.AppearanceCaption.BorderColor.IsEmpty ? info.Appearance.BorderColor : info.AppearanceCaption.BorderColor;
		}
		protected virtual SkinElement GetPanelSkinElement(GroupObjectInfoArgs info) {
			if(info.ShowCaption)
				return GetSkin()[CommonSkins.SkinGroupPanelTop];
			return GetSkin()[CommonSkins.SkinGroupPanel];
		}
		Skin GetSkin() { return CommonSkins.GetSkin(Provider); }
	}
}
