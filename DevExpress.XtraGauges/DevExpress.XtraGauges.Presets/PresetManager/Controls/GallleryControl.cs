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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGauges.Core.Base;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public delegate Image GetImage();
	public class GalleryItem : ISupportAcceptOrder {
		int numCore;
		Image imageCore;
		GetImage imageGetter;
		string nameCore;
		public Image Image {
			get {
				if(imageCore == null && imageGetter != null)
					imageCore = imageGetter();
				return imageCore;
			}
			set { imageCore = value; }
		}
		public string Name {
			get { return nameCore; }
			set { nameCore = value; }
		}
		public GalleryItem(string name, Image img) {
			this.imageCore = img;
			this.nameCore = name;
		}
		public GalleryItem(string name, GetImage imageGetter) {
			this.imageGetter = imageGetter;
			this.nameCore = name;
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return numCore; }
			set { numCore = value; }
		}
	}
	public class GalleryItemCollection :
		BaseChangeableList<GalleryItem> {
		public GalleryItemCollection()
			: this(null) {
		}
		public GalleryItemCollection(GalleryItem[] items) {
			if(items != null) AddRange(items);
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class GalleryControl : Control, ISupportInitialize, IMouseWheelSupport {
		GalleryViewInfo viewInfoCore;
		GalleryItemCollection itemsCore;
		GalleryItemCollection EmptyItems;
		ScrollBarBase scrollCore;
		int selectedIndexCore;
		Brush DescriptionBrush;
		Font DescriptionFont;
		StringFormat DescriptionFormat;
		BorderPainter BorderPainter;
		public GalleryControl() {
			this.selectedIndexCore = -1;
			SetStyle(
					ControlStyles.SupportsTransparentBackColor |
					ControlConstants.DoubleBuffer |
					ControlStyles.ResizeRedraw |
					ControlStyles.AllPaintingInWmPaint |
					ControlStyles.ResizeRedraw |
					ControlStyles.UserMouse |
					ControlStyles.UserPaint,
					true
				);
			OnCreate();
		}
		protected virtual void OnCreate() {
			this.EmptyItems = new GalleryItemCollection();
			this.itemsCore = EmptyItems;
			DescriptionBrush = Brushes.Black;
			DescriptionFont = new Font(FontFamily.GenericSansSerif, 8);
			DescriptionFormat = new StringFormat();
			DescriptionFormat.Alignment = StringAlignment.Center;
			DescriptionFormat.LineAlignment = StringAlignment.Center;
			this.viewInfoCore = CreateViewInfo();
			if(UseScrolls) {
				this.scrollCore = CreateScrollBar();
				Scroll.Parent = this;
				Scroll.Maximum = 0;
				Scroll.Value = 0;
				Scroll.ValueChanged += OnScrollValueChanged;
			}
			this.BorderPainter = CreateBorderPainter();
		}
		protected internal virtual bool UseScrolls {
			get { return true; }
		}
		protected virtual ScrollBarBase CreateScrollBar() {
			return new DevExpress.XtraEditors.HScrollBar();
		}
		protected virtual DevExpress.XtraEditors.Controls.BorderStyles BorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; }
		}
		protected virtual BorderPainter CreateBorderPainter() {
			return BorderHelper.GetPainter(BorderStyle, LookAndFeel);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ViewInfo != null) {
					ViewInfo.Dispose();
					viewInfoCore = null;
				}
				if(Scroll != null) {
					Scroll.ValueChanged -= OnScrollValueChanged;
					Scroll.Parent = null;
					Scroll.Dispose();
					scrollCore = null;
				}
				this.itemsCore = null;
				this.EmptyItems = null;
			}
			base.Dispose(disposing);
		}
		protected UserLookAndFeel LookAndFeel {
			get { return UserLookAndFeel.Default; }
		}
		protected internal DevExpress.XtraEditors.ScrollBarBase Scroll {
			get { return scrollCore; }
		}
		protected virtual ImageCollection StateImages {
			get { return Presets.Resources.UIHelper.GalleryItemImages; }
		}
		public GalleryItemCollection Items {
			get { return itemsCore; }
			set {
				if(value == null) value = EmptyItems;
				if(Items == value) return;
				this.itemsCore = value;
				this.selectedIndexCore = -1;
				LayoutChanged();
				Invalidate();
			}
		}
		Size itemSizeCore = Size.Empty;
		public Size ItemSize {
			get { return itemSizeCore; }
			set {
				if(ItemSize == value) return;
				itemSizeCore = value;
				LayoutChanged();
				Invalidate();
			}
		}
		float itemTextVOffsetCore = 1f;
		public float ItemTextVerticalOffset {
			get { return itemTextVOffsetCore; }
			set {
				if(itemTextVOffsetCore == value) return;
				itemTextVOffsetCore = value;
				LayoutChanged();
				Invalidate();
			}
		}
		float itemImageScaleFactorCore = 0.95f;
		public float ItemImageScaleFactor {
			get { return itemImageScaleFactorCore; }
			set {
				if(itemTextVOffsetCore == value) return;
				itemImageScaleFactorCore = value;
				LayoutChanged();
				Invalidate();
			}
		}
		protected virtual GalleryViewInfo CreateViewInfo() {
			return new GalleryViewInfo(this);
		}
		protected GalleryViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected internal int ItemsCount {
			get { return Items.Count; }
		}
		public event EventHandler SelectedIndexChanged;
		public event EventHandler ItemDoubleClick;
		public GalleryItem SelectedItem {
			get { return SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null; }
		}
		public int SelectedIndex {
			get { return selectedIndexCore; }
			set {
				if(SelectedIndex == value) return;
				InvalidateItem(selectedIndexCore);
				this.selectedIndexCore = value;
				RaiseSelectedIndexChanged();
				InvalidateItem(selectedIndexCore);
				LayoutChanged();
			}
		}
		protected void RaiseSelectedIndexChanged() {
			if(SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
		}
		protected void RaiseItemDoubleClick() {
			if(ItemDoubleClick != null) ItemDoubleClick(this, EventArgs.Empty);
		}
		void InvalidateItem(int index) {
			if(index >= 0 && index < ViewInfo.Rects.Items.Length) {
				Rectangle r = Rectangle.Round(ViewInfo.Rects.Items[index]);
				r.Inflate(5, 5);
				Invalidate(ViewInfo.MapRect(r));
			}
		}
		void OnScrollValueChanged(object sender, EventArgs e) {
			if(lockUpdateScroll > 0) return;
			LayoutChanged();
			Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				UpdateBeforePaint(ea);
				DrawBackground(ea);
				DrawItems(ea);
			}
		}
		protected void UpdateBeforePaint(GraphicsInfoArgs e) {
			if(!ViewInfo.IsReady) {
				ViewInfo.CalcInfo(e.Graphics, Bounds);
				UpdateScroll();
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		int lockLayoutChanged = 0;
		public void LayoutChanged() {
			if(lockLayoutChanged > 0) return;
			lockLayoutChanged++;
			ViewInfo.SetDirty();
			ViewInfo.SetPressedInfo(GalleryHitInfo.Empty);
			ViewInfo.SetHotTrackedInfo(GalleryHitInfo.Empty);
			ViewInfo.CalcInfo(null, Bounds);
			UpdateScroll();
			lockLayoutChanged--;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.SetHotTrackedInfo(GalleryHitInfo.Empty);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			GalleryHitInfo downInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				ViewInfo.SetPressedInfo(downInfo.InItem ? downInfo : GalleryHitInfo.Empty);
				if(!downInfo.InItem) ResetSelection();
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			GalleryHitInfo clickInfo = ViewInfo.CalcHitInfo(e.Location);
			if(clickInfo.InItem && e.Button == MouseButtons.Left) RaiseItemDoubleClick();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode == Keys.Escape) ResetSelection();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			GalleryHitInfo upInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				if(ViewInfo.States.Pressed.IsEquals(upInfo)) DoClickAction(upInfo);
			}
			ViewInfo.SetPressedInfo(GalleryHitInfo.Empty);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			GalleryHitInfo moveInfo = ViewInfo.CalcHitInfo(e.Location);
			if(!ViewInfo.States.Pressed.IsEquals(moveInfo)) {
				ViewInfo.SetHotTrackedInfo(moveInfo.InItem ? moveInfo : GalleryHitInfo.Empty);
			}
		}
		protected void DoClickAction(GalleryHitInfo hitInfo) {
			if(hitInfo.InItem) SelectedIndex = hitInfo.HitItemIndex;
		}
		int lockUpdateScroll = 0;
		protected void UpdateScroll() {
			if(!UseScrolls || !IsInitialized || lockUpdateScroll > 0) return;
			lockUpdateScroll++;
			try {
				bool v = Scroll.Visible;
				int imagesCount = ViewInfo.Rects.ImagesCount;
				int max = ViewInfo.ScrollMax;
				int view = ViewInfo.ScrollView;
				Scroll.SmallChange = ViewInfo.ScrollChange;
				Scroll.LargeChange = view;
				Scroll.Minimum = 0;
				Scroll.Maximum = max;
				Scroll.Visible = max > view && view > 0;
				if(max - Scroll.Value < view)
					Scroll.Value = Math.Max(0, max - view);
				if(v != Scroll.Visible)
					LayoutChanged();
				if(Scroll.Bounds != ViewInfo.Rects.Scroll)
					Scroll.Bounds = ViewInfo.Rects.Scroll;
			}
			finally { lockUpdateScroll--; }
		}
		public void ResetSelection() {
			SelectedIndex = -1;
		}
		protected internal Rectangle CalcClientRect(Rectangle bounds) {
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(null, AppearanceObject.ControlAppearance, bounds, ObjectState.Normal);
			return BorderPainter.GetObjectClientRectangle(info);
		}
		protected void DrawBackground(GraphicsInfoArgs e) {
			e.Graphics.FillRectangle(Brushes.White, ViewInfo.Rects.Canvas);
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(e.Cache, AppearanceObject.ControlAppearance,
				ViewInfo.Rects.Bounds, ObjectState.Normal);
			BorderPainter.DrawObject(info);
		}
		protected void DrawItems(GraphicsInfoArgs e) {
			e.Graphics.SetClip(ViewInfo.Rects.Canvas);
			for(int i = 0; i < ViewInfo.Rects.Items.Length; i++) {
				DrawItemBackground(e, i);
				DrawItemImage(e, i);
				DrawItemText(e, i);
			}
			e.Graphics.ResetClip();
		}
		protected void DrawItemImage(GraphicsInfoArgs e, int i) {
			RectangleF r = ViewInfo.MapRect(ViewInfo.Rects.Images[i]);
			if(e.Graphics.ClipBounds.IntersectsWith(r)) {
				Image img = Items[i].Image;
				if(img != null) e.Graphics.DrawImage(img, r);
			}
		}
		protected void DrawItemText(GraphicsInfoArgs e, int i) {
			string text = Items[i].Name;
			RectangleF r = ViewInfo.MapRect(ViewInfo.Rects.Descriptions[i]);
			e.Graphics.DrawString(text, DescriptionFont, DescriptionBrush, r, DescriptionFormat);
		}
		protected void DrawItemBackground(GraphicsInfoArgs e, int i) {
			Image img = null;
			if((ViewInfo.States.Items[i] & ObjectState.Hot) != 0) img = StateImages.Images[0];
			if((ViewInfo.States.Items[i] & ObjectState.Pressed) != 0) img = StateImages.Images[1];
			if(img != null) {
				RectangleF r = ViewInfo.MapRect(ViewInfo.Rects.Items[i]);
				e.Graphics.DrawImageUnscaled(img, Point.Round(r.Location));
			}
		}
		#region ISupportInitialize Members
		int initializing = 0;
		public bool IsInitializing {
			get { return initializing > 0; }
		}
		int initialized = 0;
		public bool IsInitialized {
			get { return initialized > 0; }
		}
		void ISupportInitialize.BeginInit() {
			initializing++;
		}
		void ISupportInitialize.EndInit() {
			if(--initializing == 0) {
				initialized++;
				LayoutChanged();
			}
		}
		#endregion
		#region IMouseWheelSupport Members
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			if(UseScrolls) {
				ViewInfo.SetHotTrackedInfo(GalleryHitInfo.Empty);
				Scroll.Value = Math.Max(0, Math.Min(Scroll.Maximum - Scroll.LargeChange, Scroll.Value - e.Delta));
				ViewInfo.SetHotTrackedInfo(ViewInfo.CalcHitInfo(e.Location));
			}
		}
		#endregion
	}
	public class GalleryViewInfo : BaseViewInfo {
		GalleryControl ownerCore;
		GalleryViewRects viewRectsCore;
		GalleryViewStates viewStatesCore;
		public GalleryViewInfo(GalleryControl owner) {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.viewRectsCore = new GalleryViewRects();
			this.viewStatesCore = new GalleryViewStates();
		}
		protected override void OnDispose() {
			this.viewRectsCore = null;
			this.viewStatesCore = null;
			this.ownerCore = null;
			base.OnDispose();
		}
		protected GalleryControl Owner {
			get { return ownerCore; }
		}
		public GalleryViewRects Rects {
			get { return viewRectsCore; }
		}
		public GalleryViewStates States {
			get { return viewStatesCore; }
		}
		protected override void CalcViewRects(Rectangle bounds) {
			Rects.Clear();
			scrollMaxCore = 0;
			bool hScroll = Owner.UseScrolls && Owner.Scroll.ScrollBarType == ScrollBarType.Horizontal;
			int scrollSize = Owner.UseScrolls ? (hScroll ? Owner.Scroll.GetHeight() : Owner.Scroll.GetWidth()) : 0;
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(null, AppearanceObject.ControlAppearance, bounds, ObjectState.Normal);
			Rectangle client = Owner.CalcClientRect(bounds);
			Rectangle scroll, canvas;
			if(hScroll) {
				scroll = (client.Height < scrollSize) ? Rectangle.Empty :
						new Rectangle(client.Left, client.Bottom - scrollSize, client.Width, scrollSize);
				canvas = new Rectangle(client.Left, client.Top, client.Width, client.Height - scrollSize);
			}
			else {
				scroll = (client.Width < scrollSize) ? Rectangle.Empty :
						new Rectangle(client.Right - scrollSize, client.Top, scrollSize, client.Height);
				canvas = new Rectangle(client.Left, client.Top, client.Width - scrollSize, client.Height);
			}
			Rects.Bounds = bounds;
			Rects.Scroll = scroll;
			Rects.Canvas = canvas;
			Size itemSize = Owner.ItemSize.IsEmpty ? new Size(182, 200) : Owner.ItemSize;
			Rects.Items = CalcItems(canvas, itemSize, hScroll);
			Rects.Descriptions = CalcItemDescriptions();
			Rects.Images = CalcItemImages();
			States.Items = new ObjectState[Rects.ImagesCount];
		}
		int scrollMaxCore = 0;
		public int ScrollMax {
			get { return scrollMaxCore; }
		}
		int scrollViewCore = 0;
		public int ScrollView {
			get { return scrollViewCore; }
		}
		int scrollChangeCore;
		public int ScrollChange {
			get { return scrollChangeCore; }
		}
		public int Offset {
			get { return Owner.UseScrolls ? Owner.Scroll.Value : 0; }
		}
		public Rectangle MapRect(Rectangle rect) {
			if(!Owner.UseScrolls) return rect;
			if(Owner.Scroll.ScrollBarType == ScrollBarType.Horizontal)
				return new Rectangle(rect.X - Owner.Scroll.Value, rect.Y, rect.Width, rect.Height);
			else return new Rectangle(rect.X, rect.Y - Owner.Scroll.Value, rect.Width, rect.Height);
		}
		public Point MapPoint(Point point) {
			if(!Owner.UseScrolls) return point;
			if(Owner.Scroll.ScrollBarType == ScrollBarType.Horizontal)
				return new Point(point.X + Owner.Scroll.Value, point.Y);
			else return new Point(point.X, point.Y + Owner.Scroll.Value);
		}
		public RectangleF MapRect(RectangleF rect) {
			if(!Owner.UseScrolls) return rect;
			if(Owner.Scroll.ScrollBarType == ScrollBarType.Horizontal)
				return new RectangleF(rect.X - (float)Owner.Scroll.Value, rect.Y, rect.Width, rect.Height);
			else return new RectangleF(rect.X, rect.Y - (float)Owner.Scroll.Value, rect.Width, rect.Height);
		}
		RectangleF[] CalcItems(Rectangle canvas, Size itemSize, bool horizontal) {
			int w = canvas.Width / itemSize.Width;
			int h = canvas.Height / itemSize.Height;
			return (h > 0) ? CalcItemsNormal(canvas, itemSize, w, h, horizontal)
				: CalcItemsPartial(canvas, itemSize, w, h, horizontal);
		}
		RectangleF[] CalcItemsPartial(Rectangle canvas, Size itemSize, int w, int h, bool horizontal) {
			Rects.ImagesCount = Math.Min(w, Owner.ItemsCount);
			float dx = (float)(canvas.Width % itemSize.Width) / ((float)(w + 1));
			RectangleF[] items = new RectangleF[Rects.ImagesCount];
			for(int i = 0; i < Rects.ImagesCount; i++) {
				items[i] = new RectangleF(
					(float)(i * itemSize.Width) + dx * ((float)(i + 1)), 3,
					(float)itemSize.Width, (float)itemSize.Height);
			}
			return items;
		}
		RectangleF[] CalcItemsNormal(Rectangle canvas, Size itemSize, int w, int h, bool horizontal) {
			Rects.ImagesCount = Owner.ItemsCount;
			float dx = (float)(canvas.Width % itemSize.Width) / ((float)(w + 1));
			float dy = (float)(canvas.Height % itemSize.Height) / ((float)(h + 1));
			RectangleF[] items = new RectangleF[Rects.ImagesCount];
			int xIndex, yIndex;
			for(int i = 0; i < Rects.ImagesCount; i++) {
				if(horizontal) {
					xIndex = i / h; yIndex = i % h;
				}
				else {
					xIndex = i % w; yIndex = i / w;
				}
				items[i] = new RectangleF(
					((float)(xIndex * itemSize.Width) + dx * ((float)xIndex + 1f)),
					((float)(yIndex * itemSize.Height) + dy * ((float)yIndex + 1f)),
					(float)itemSize.Width, (float)itemSize.Height);
				scrollMaxCore = Math.Max((int)(
					horizontal ? (items[i].Right + dx) : (items[i].Bottom + dy) + 0.5f), scrollMaxCore);
				scrollViewCore = Math.Min(horizontal ? canvas.Width : canvas.Height, scrollMaxCore);
				scrollChangeCore = horizontal ? (itemSize.Width + (int)(dx + 0.5f)) : (itemSize.Height + (int)(dy + 0.5f));
			}
			return items;
		}
		RectangleF[] CalcItemDescriptions() {
			RectangleF[] descriptions = new RectangleF[Rects.ImagesCount];
			float offset = Owner.ItemTextVerticalOffset;
			for(int i = 0; i < Rects.ImagesCount; i++) {
				SizeF size = Rects.Items[i].Size;
				descriptions[i] = new RectangleF(
						Rects.Items[i].Left,
						Rects.Items[i].Top + size.Height * offset,
						Rects.Items[i].Width, size.Height * 0.1f
					);
			}
			return descriptions;
		}
		RectangleF[] CalcItemImages() {
			RectangleF[] images = new RectangleF[Rects.ImagesCount];
			float f = 1f - Owner.ItemImageScaleFactor;
			for(int i = 0; i < Rects.ImagesCount; i++) {
				float size = Rects.Items[i].Width;
				images[i] = new RectangleF(
						Rects.Items[i].Left,
						Rects.Items[i].Top,
						Rects.Items[i].Width, size
					);
				if(f > 0.001f)
					images[i].Inflate(-images[i].Width * f, -images[i].Height * f);
			}
			return images;
		}
		protected override void CalcViewStates() {
			States.Clear();
			CheckAndSetState(States.Pressed, ObjectState.Pressed);
			CheckAndSetState(States.HotTracked, ObjectState.Hot);
			int pos = Owner.SelectedIndex;
			if(pos >= 0 && pos < States.Items.Length)
				States.Items[pos] |= ObjectState.Pressed;
		}
		public void SetPressedInfo(GalleryHitInfo hitInfo) {
			GalleryHitInfo prevHitInfo = States.Pressed;
			if(hitInfo.IsEquals(prevHitInfo)) return;
			States.HotTracked = GalleryHitInfo.Empty;
			States.Pressed = hitInfo;
			CalcViewStates();
			InvalidateHitObject(prevHitInfo);
			InvalidateHitObject(States.Pressed);
		}
		public void SetHotTrackedInfo(GalleryHitInfo hitInfo) {
			GalleryHitInfo prevHitInfo = States.HotTracked;
			if(hitInfo.IsEquals(prevHitInfo)) return;
			States.HotTracked = hitInfo;
			CalcViewStates();
			InvalidateHitObject(prevHitInfo);
			InvalidateHitObject(States.HotTracked);
		}
		protected void InvalidateHitObject(GalleryHitInfo hitInfo) {
			if(hitInfo != null && !hitInfo.IsEmpty)
				Owner.Invalidate(hitInfo.HitRect);
		}
		protected void CheckAndSetState(GalleryHitInfo hitInfo, ObjectState state) {
			if(hitInfo != null) {
				if(hitInfo.InItem && hitInfo.HitItemIndex >= 0)
					States.Items[hitInfo.HitItemIndex] = state;
			}
		}
		public GalleryHitInfo CalcHitInfo(Point pt) {
			GalleryHitInfo hitInfo = new GalleryHitInfo(pt);
			if(hitInfo.CheckAndSetHitTest(Rects.Bounds, -1, GalleryHitTest.Bounds)) {
				bool inCanvas = hitInfo.CheckAndSetHitTest(Rects.Canvas, -1, GalleryHitTest.Canvas);
				if(inCanvas) {
					for(int i = 0; i < Rects.ImagesCount; i++) {
						if(hitInfo.CheckAndSetHitTest(
								MapRect(Rectangle.Round(Rects.Items[i])), i,
								GalleryHitTest.Item)
							) return hitInfo;
					}
				}
				else hitInfo.CheckAndSetHitTest(Rects.Scroll, -1, GalleryHitTest.Scroll);
			}
			return hitInfo;
		}
	}
	public class GalleryViewRects {
		public Rectangle Bounds;
		public Rectangle Canvas;
		public Rectangle Scroll;
		public int ImagesCount;
		public RectangleF[] Items;
		public RectangleF[] Descriptions;
		public RectangleF[] Images;
		public GalleryViewRects() {
			Clear();
		}
		public void Clear() {
			Bounds = Canvas = Scroll = Rectangle.Empty;
			ImagesCount = 0;
			Items = new RectangleF[ImagesCount];
			Descriptions = new RectangleF[ImagesCount];
			Images = new RectangleF[ImagesCount];
		}
	}
	public class GalleryViewStates {
		public GalleryHitInfo HotTracked;
		public GalleryHitInfo Pressed;
		public ObjectState[] Items;
		public GalleryViewStates() {
			HotTracked = GalleryHitInfo.Empty;
			Pressed = GalleryHitInfo.Empty;
			Items = new ObjectState[0];
			Clear();
		}
		public void Clear() {
			for(int i = 0; i < Items.Length; i++) Items[i] = ObjectState.Normal;
		}
	}
	public enum GalleryHitTest {
		None, Bounds, Scroll, Canvas, Item
	}
	public class GalleryHitInfo {
		public static readonly GalleryHitInfo Empty;
		static GalleryHitInfo() {
			Empty = new GalleryEmptyHitInfo();
		}
		class GalleryEmptyHitInfo : GalleryHitInfo {
			public GalleryEmptyHitInfo() : base(new Point(-10000, -10000)) { }
		}
		GalleryHitTest hitTestCore;
		Rectangle hitRectCore;
		Point hitPointCore;
		int hitItemIndexCore;
		public GalleryHitInfo(Point hitPoint) {
			this.hitPointCore = hitPoint;
			this.hitTestCore = GalleryHitTest.None;
		}
		public int HitItemIndex {
			get { return hitItemIndexCore; }
		}
		public Rectangle HitRect {
			get { return hitRectCore; }
		}
		public Point HitPoint {
			get { return hitPointCore; }
		}
		public GalleryHitTest HitTest {
			get { return hitTestCore; }
		}
		public bool IsEmpty {
			get { return this is GalleryEmptyHitInfo; }
		}
		public bool CheckAndSetHitTest(Rectangle bounds, int num, GalleryHitTest hitTest) {
			bool contains = !bounds.IsEmpty && bounds.Contains(HitPoint);
			if(contains) {
				hitRectCore = bounds;
				hitTestCore = hitTest;
				hitItemIndexCore = num;
			}
			return contains;
		}
		public bool InBounds {
			get { return (HitTest == GalleryHitTest.Bounds) || InCanvas || InScroll; }
		}
		public bool InScroll {
			get { return (HitTest == GalleryHitTest.Scroll); }
		}
		public bool InCanvas {
			get { return (HitTest == GalleryHitTest.Canvas); }
		}
		public bool InItem {
			get { return (HitTest == GalleryHitTest.Item); }
		}
		public virtual bool IsEquals(GalleryHitInfo hitInfo) {
			return (hitInfo != null && (this.HitTest == hitInfo.HitTest) && (this.HitRect == hitInfo.HitRect));
		}
	}
}
