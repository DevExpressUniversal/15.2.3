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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class InnerColorListControl : InnerColorPickControlBase, IToolTipControlClient {
		const int DefaultItemHeight = 21;
		const int DefaultItemGlyphIndent = 3;
		static readonly Size DefaultItemGlyphSize = new Size(16, 13);
		int topItem;
		int itemHeight;
		int itemGlyphIndent;
		Size itemGlyphSize;
		InnerColorListControlScrollController scrollController;
		ColorItemCollection colors;
		public InnerColorListControl() {
			this.topItem = 0;
			this.itemHeight = DefaultItemHeight;
			this.itemGlyphIndent = DefaultItemGlyphIndent;
			this.itemGlyphSize = DefaultItemGlyphSize;
			this.colors = new ColorItemCollection();
			this.colors.ListChanged += OnColorListChanged;
			this.scrollController = CreateScrollController();
			this.scrollController.AddControls(this);
			this.scrollController.VScrollValueChanged += OnVScrollValueChanged;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
		protected override BaseControlPainter CreatePainter() {
			return new InnerColorListControlPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new InnerColorListControlViewInfo(this);
		}
		protected virtual InnerColorListControlScrollController CreateScrollController() {
			return new InnerColorListControlScrollController(this);
		}
		InnerColorListControlHandler handler = null;
		protected InnerColorListControlHandler Handler {
			get {
				if(this.handler == null) {
					this.handler = CreateHandler();
				}
				return this.handler;
			}
		}
		protected virtual InnerColorListControlHandler CreateHandler() {
			return new InnerColorListControlHandler(this);
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right) return true;
			return base.IsInputKey(keyData);
		}
		protected override Padding DefaultPadding {
			get { return new Padding(2); }
		}
		void OnColorListChanged(object sender, ListChangedEventArgs e) {
			OnColorListChanged(e);
		}
		protected virtual void OnColorListChanged(ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			Handler.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			CheckTopItem();
		}
		protected virtual void CheckTopItem() {
			if(TopItem > MaxTopItem) TopItem = MaxTopItem;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorItemCollection Colors {
			get { return colors; }
		}
		[DefaultValue(DefaultItemHeight)]
		public int ItemHeight {
			get { return itemHeight; }
			set {
				if(ItemHeight == value)
					return;
				itemHeight = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(DefaultItemGlyphIndent)]
		public int ItemGlyphIndent {
			get { return itemGlyphIndent; }
			set {
				if(ItemGlyphIndent == value)
					return;
				itemGlyphIndent = value;
				OnPropertiesChanged();
			}
		}
		public Size ItemGlyphSize {
			get { return itemGlyphSize; }
			set {
				if(ItemGlyphSize == value)
					return;
				itemGlyphSize = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeItemGlyphSize() { return ItemGlyphSize != DefaultItemGlyphSize; }
		void ResetItemGlyphSize() { ItemGlyphSize = DefaultItemGlyphSize; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopItem {
			get { return topItem; }
			set {
				if(TopItem == value)
					return;
				topItem = value;
				OnPropertiesChanged();
			}
		}
		protected int MaxTopItem { get { return Math.Max(0, Colors.Count - VisibleItemCount); } }
		public void SetTopItem(int topItem) {
			TopItem = Math.Min(Math.Max(0, topItem), MaxTopItem);
		}
		[Browsable(false)]
		public int VisibleItemCount {
			get {
				if(ItemHeight == 0) return 0;
				return Height / ItemHeight + ((Height % ItemHeight > ItemHeight * 2 / 3) ? 1 : 0);
			}
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			TopItem = ScrollController.VScrollPosition;
		}
		int lockScrollUpdate = 0;
		protected virtual void UpdateScrollBars() {
			if(this.lockScrollUpdate != 0) return;
			this.lockScrollUpdate++;
			try {
				ScrollController.IsRightToLeft = IsRightToLeft;
				ScrollController.VScrollVisible = ViewInfo.IsVScrollVisible;
				ScrollController.ClientRect = ViewInfo.ClientRect;
				if(ScrollController.VScrollVisible) ScrollController.VScrollArgs = ViewInfo.CalcVScrollArgs();
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
		protected internal override void LayoutChanged() {
			base.LayoutChanged();
			if(!ViewInfo.IsVScrollVisible) {
				TopItem = 0;
			}
			UpdateScrollBars();
		}
		public override void SetColor(Color color, bool raiseEvent) {
			ScrollIntoView(color);
			base.SetColor(color, raiseEvent);
		}
		public void ScrollIntoView(Color color) {
			ViewInfo.ScrollInto(color);
		}
		public override bool ContainsColor(Color color) {
			return Colors.ContainsColor(color, true);
		}
		public override ColorItem GetColorItemByColor(Color color) {
			return Colors.GetItem(color, true);
		}
		protected internal InnerColorListControlScrollController ScrollController { get { return scrollController; } }
		protected internal new InnerColorListControlViewInfo ViewInfo { get { return base.ViewInfo as InnerColorListControlViewInfo; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.colors != null) {
					this.colors.ListChanged -= OnColorListChanged;
				}
				this.colors = null;
				if(this.handler != null) {
					this.handler = null;
				}
				this.handler = null;
				if(ScrollController != null) {
					ScrollController.VScrollValueChanged -= OnVScrollValueChanged;
					ScrollController.RemoveControls(this);
					ScrollController.Dispose();
				}
				this.scrollController = null;
			}
			base.Dispose(disposing);
		}
	}
	public class ColorListItemInfo : ColorItemInfo {
		Rectangle glyphBounds;
		Rectangle descriptionBounds;
		public ColorListItemInfo(ColorItem item) : base(item) {
			this.glyphBounds = this.descriptionBounds = Rectangle.Empty;
		}
		public void SetGlyphBounds(Rectangle bounds) {
			this.glyphBounds = bounds;
		}
		public void SetDescriptionBounds(Rectangle bounds) {
			this.descriptionBounds = bounds;
		}
		public override bool IsMatch(Color other) {
			return base.IsMatch(other) && Color.Name == other.Name;
		}
		public string Description { get { return Color.Name; } }
		public Rectangle GlyphBounds { get { return glyphBounds; } }
		public Rectangle DescriptionBounds { get { return descriptionBounds; } }
	}
	public enum InnerColorListControlHitTest {
		None, Item
	}
	public class InnerColorListControlHitInfo {
		Point hitPoint;
		object hitObject;
		InnerColorListControlHitTest hitTest;
		public InnerColorListControlHitInfo() : this(Point.Empty) { }
		public InnerColorListControlHitInfo(Point hitPoint) {
			this.hitPoint = hitPoint;
			this.hitObject = null;
			this.hitTest = InnerColorListControlHitTest.None;
		}
		public void SetHitTest(InnerColorListControlHitTest hitTest) {
			this.hitTest = hitTest;
		}
		public void SetHitObject(object hitObject) {
			this.hitObject = hitObject;
		}
		public void Reset() {
			this.hitPoint = Point.Empty;
			this.hitObject = null;
		}
		public static readonly InnerColorListControlHitInfo Empty = new InnerColorListControlHitInfo();
		public bool IsEmpty {
			get { return Empty.Equals(this); }
		}
		public override bool Equals(object obj) {
			InnerColorListControlHitInfo other = obj as InnerColorListControlHitInfo;
			if(other == null) return false;
			return other.HitTest == HitTest && other.hitObject == hitObject;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public ColorListItemInfo ColorItem { get { return (ColorListItemInfo)hitObject; } }
		public Point HitPoint { get { return hitPoint; } }
		public InnerColorListControlHitTest HitTest { get { return hitTest; } }
	}
	public class InnerColorListControlHotObject {
		InnerColorListControlHitInfo hotObject;
		public InnerColorListControlHotObject() {
			this.hotObject = InnerColorListControlHitInfo.Empty;
		}
		public void SetHotObject(InnerColorListControlHitInfo newObj) {
			if(newObj.Equals(this.hotObject)) return;
			InnerColorListControlHitInfo prev = this.hotObject;
			this.hotObject = newObj;
			OnHotObjectChanged(new InnerColorListControlHotObjectChangedEventArgs(prev, newObj));
		}
		public void Reset() {
			if(this.hotObject.IsEmpty) return;
			InnerColorListControlHitInfo prev = this.hotObject;
			this.hotObject = InnerColorListControlHitInfo.Empty;
			OnHotObjectChanged(new InnerColorListControlHotObjectChangedEventArgs(prev, this.hotObject));
		}
		protected virtual void OnHotObjectChanged(InnerColorListControlHotObjectChangedEventArgs e) {
			if(HotObjectChanged != null) HotObjectChanged(this, e);
		}
		public event InnerColorListControlHotObjectChangedHandler HotObjectChanged;
	}
	public delegate void InnerColorListControlHotObjectChangedHandler(object sender, InnerColorListControlHotObjectChangedEventArgs e);
	public class InnerColorListControlHotObjectChangedEventArgs : EventArgs {
		InnerColorListControlHitInfo prev;
		InnerColorListControlHitInfo next;
		public InnerColorListControlHotObjectChangedEventArgs(InnerColorListControlHitInfo prev, InnerColorListControlHitInfo next) {
			this.prev = prev;
			this.next = next;
		}
		public InnerColorListControlHitInfo Prev { get { return prev; } }
		public InnerColorListControlHitInfo Next { get { return next; } }
	}
	public class InnerColorListControlViewInfo : BaseStyleControlViewInfo {
		bool isVScrollVisible;
		Hashtable items;
		InnerColorListControlHotObject hotObject;
		public InnerColorListControlViewInfo(InnerColorListControl owner) : base(owner) {
			this.items = new Hashtable();
			this.isVScrollVisible = false;
			this.hotObject = new InnerColorListControlHotObject();
			this.hotObject.HotObjectChanged += OnHotObjectChanged;
		}
		public InnerColorListControlHitInfo CalcHitInfo(Point pt) {
			InnerColorListControlHitInfo hitInfo = new InnerColorListControlHitInfo(pt);
			foreach(ColorListItemInfo item in VisibleItems) {
				if(item.Bounds.Contains(pt)) {
					hitInfo.SetHitTest(InnerColorListControlHitTest.Item);
					hitInfo.SetHitObject(item);
					break;
				}
			}
			return hitInfo;
		}
		protected internal void CheckHotObject() { CheckHotObject(GetMousePoint()); }
		protected internal void CheckHotObject(Point pt) {
			InnerColorListControlHitInfo hi = CalcHitInfo(pt);
			this.hotObject.SetHotObject(hi);
		}
		protected internal void ResetHotObject() {
			this.hotObject.Reset();
		}
		protected Point GetMousePoint() { return OwnerControl.PointToClient(Control.MousePosition); }
		void OnHotObjectChanged(object sender, InnerColorListControlHotObjectChangedEventArgs e) {
			OnHotObjectChanged(e);
		}
		protected virtual void OnHotObjectChanged(InnerColorListControlHotObjectChangedEventArgs e) {
			if(e.Prev.HitTest == InnerColorListControlHitTest.Item) {
				UpdateState(e.Prev.ColorItem);
			}
			if(e.Next.HitTest == InnerColorListControlHitTest.Item) {
				UpdateState(e.Next.ColorItem);
			}
			OwnerControl.Invalidate();
		}
		protected internal virtual void UpdateState(Point pt) {
			InnerColorListControlHitInfo hi = CalcHitInfo(pt);
			if(hi.HitTest == InnerColorListControlHitTest.Item) {
				UpdateState(hi.ColorItem);
				OwnerControl.Invalidate();
			}
		}
		protected virtual void UpdateState(ColorListItemInfo colorItem) {
			UpdateColorItemState(colorItem);
			UpdateColorItemAppearance(colorItem);
		}
		protected virtual void UpdateColorItemState(ColorItemInfo itemInfo) {
			if(!OwnerControl.Enabled) {
				itemInfo.SetState(ObjectState.Disabled);
				return;
			}
			if(OwnerControl.IsDesignMode) return;
			ObjectState state = ObjectState.Normal;
			if(Owner.SelectedColorItem != null && Owner.SelectedColorItem.Equals(itemInfo.ColorItem)) {
				state = ObjectState.Selected;
			}
			if(itemInfo.Bounds.Contains(GetMousePoint())) {
				state |= ObjectState.Hot;
				if((Control.MouseButtons & MouseButtons.Left) != 0) state |= ObjectState.Pressed;
			}
			itemInfo.SetState(state);
		}
		protected virtual void UpdateColorItemAppearance(ColorItemInfo itemInfo) {
			itemInfo.PaintAppearance.Assign(PaintAppearance);
		}
		protected virtual int CalcItemTextHeight() {
			return PaintAppearance.CalcTextSizeInt(GInfo.Graphics, "Wq", 0).Height;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContent();
			CalcItems();
		}
		protected virtual void AdjustContent() {
			ApplyPadding();
			bool vScrollVisible = CalcVScrollBarVisibility();
			if(vScrollVisible) {
				if(!RightToLeft) {
					this.fContentRect.Width -= Owner.ScrollController.VScrollWidth;
				}
				else {
					this.fContentRect.X += Owner.ScrollController.VScrollWidth;
					this.fContentRect.Width -= Owner.ScrollController.VScrollWidth;
				}
			}
			this.isVScrollVisible = vScrollVisible;
		}
		protected void ApplyPadding() {
			Padding padding = OwnerControl.Padding;
			this.fContentRect.X += padding.Left;
			this.fContentRect.Width -= (padding.Left + padding.Right);
			this.fContentRect.Y += padding.Top;
			this.fContentRect.Height -= (padding.Top + padding.Bottom);
		}
		protected virtual bool CalcVScrollBarVisibility() {
			return Owner.Colors.Count > Owner.VisibleItemCount;
		}
		public virtual ScrollArgs CalcVScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.SmallChange = 1;
			args.Maximum = Owner.Colors.Count - 1;
			args.LargeChange = Owner.VisibleItemCount;
			args.Value = Owner.TopItem;
			return args;
		}
		protected virtual void CalcItems() {
			this.items.Clear();
			ColorItemCollection col = Owner.Colors;
			if(col.Count == 0) return;
			for(int i = Owner.TopItem, j = 0; i < col.Count; i++, j++) {
				ColorItem colorItem = col[i];
				ColorListItemInfo colorItemInfo = CreateItem(j, colorItem);
				this.items.Add(colorItem, colorItemInfo);
				UpdateState(colorItemInfo);
			}
		}
		public ICollection VisibleItems { get { return items.Values; } }
		public bool IsHotItemInitialized() {
			return GetSelectedItem() != null;
		}
		public ColorListItemInfo GetSelectedItem() {
			foreach(ColorListItemInfo colorItem in VisibleItems) {
				if(colorItem.State.HasFlag(ObjectState.Selected)) return colorItem;
			}
			return null;
		}
		protected internal void SetSelectedItem(ColorItem colorItem) {
			ColorItemInfo item = GetItem(colorItem);
			if(item != null) {
				ResetStates();
				item.SetState(ObjectState.Selected);
			}
		}
		public void ScrollInto(Color color) {
			ColorItem colorItem = Owner.Colors.GetItem(color, true);
			if(colorItem != null) {
				ScrollInto(colorItem, true);
			}
		}
		public void ScrollInto(ColorItem colorItem, bool topAnchor) {
			int itemPos = Owner.Colors.IndexOf(colorItem);
			if(itemPos == -1 || ItemInViewPort(colorItem)) return;
			int newPos = itemPos;
			if(!topAnchor) {
				newPos = Math.Max(0, itemPos - Owner.VisibleItemCount + 1);
			}
			Owner.TopItem = newPos;
		}
		public bool ItemInViewPort(ColorItem colorItem) {
			int itemPos = Owner.Colors.IndexOf(colorItem);
			if(itemPos == -1) return false;
			return itemPos >= Owner.TopItem && itemPos < Owner.TopItem + Owner.VisibleItemCount;
		}
		protected ColorItemInfo GetItem(ColorItem item) {
			foreach(ColorListItemInfo colorItem in VisibleItems) {
				if(object.ReferenceEquals(colorItem.ColorItem, item)) return colorItem;
			}
			return null;
		}
		protected void ResetStates() {
			foreach(ColorListItemInfo colorItem in VisibleItems) colorItem.SetState(ObjectState.Normal);
		}
		protected virtual ColorListItemInfo CreateItem(int pos, ColorItem colorItem) {
			ColorListItemInfo colorItemInfo = new ColorListItemInfo(colorItem);
			Rectangle itemBounds = CalcItemBounds(pos);
			colorItemInfo.SetBounds(itemBounds);
			colorItemInfo.SetGlyphBounds(CalcItemGlyphBounds(itemBounds));
			colorItemInfo.SetDescriptionBounds(CalcItemDescriptionBounds(colorItem.Color.Name, itemBounds));
			return colorItemInfo;
		}
		protected virtual Rectangle CalcItemBounds(int pos) {
			return new Rectangle(ContentRect.X, ContentRect.Y + pos * Owner.ItemHeight, ContentRect.Width, CalcItemHeight());
		}
		protected virtual int CalcItemHeight() {
			int minHeight = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetButtonElementInfo(ObjectState.Normal, Rectangle.Empty)).Height;
			return Math.Max(minHeight, Owner.ItemHeight);
		}
		protected virtual Rectangle CalcItemGlyphBounds(Rectangle itemBounds) {
			Rectangle rect = new Rectangle(Point.Empty, Owner.ItemGlyphSize);
			if(!RightToLeft) {
				rect.X += itemBounds.X + Owner.ItemGlyphIndent;
			}
			else {
				rect.X += itemBounds.Right - Owner.ItemGlyphIndent - Owner.ItemGlyphSize.Width - 1;
			}
			rect.Y = itemBounds.Y + (itemBounds.Height - Owner.ItemGlyphSize.Height) / 2;
			return rect;
		}
		protected virtual Rectangle CalcItemDescriptionBounds(string description, Rectangle itemBounds) {
			Size textSize = PaintAppearance.CalcTextSizeInt(GInfo.Graphics, description, 0);
			Rectangle rect = new Rectangle(Point.Empty, textSize);
			if(!RightToLeft) {
				rect.X = itemBounds.X + Owner.ItemGlyphIndent * 2 + Owner.ItemGlyphSize.Width;
			}
			else {
				rect.X = CalcItemGlyphBounds(itemBounds).Left - 1 - textSize.Width - 1;
			}
			rect.Y = itemBounds.Y + (itemBounds.Height - textSize.Height) / 2;
			return rect;
		}
		public SkinElementInfo GetButtonElementInfo(ObjectState state, Rectangle bounds) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetRibbonSkinElement(RibbonSkins.SkinGalleryButton), bounds);
			elementInfo.ImageIndex = GetButtonElementImageIndex(state);
			return elementInfo;
		}
		protected SkinElement GetRibbonSkinElement(string elementName) {
			return RibbonSkins.GetSkin(LookAndFeel)[elementName];
		}
		protected virtual int GetButtonElementImageIndex(ObjectState state) {
			if(state == ObjectState.Normal) return 0;
			if(state == ObjectState.Selected) return 3;
			if((state & ObjectState.Selected) != 0) return 4;
			if(state == ObjectState.Hot) return 1;
			return 2;
		}
		public bool IsVScrollVisible { get { return isVScrollVisible; } }
		protected new InnerColorListControl Owner { get { return base.Owner as InnerColorListControl; } }
	}
	public class InnerColorListControlPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawItems(info);
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs info) {
			InnerColorListControlViewInfo viewInfo = (InnerColorListControlViewInfo)info.ViewInfo;
			foreach(ColorListItemInfo colorItem in viewInfo.VisibleItems) {
				DoDrawItem(info, colorItem);
			}
		}
		protected virtual void DoDrawItem(ControlGraphicsInfoArgs info, ColorListItemInfo colorItem) {
			InnerColorListControlViewInfo viewInfo = (InnerColorListControlViewInfo)info.ViewInfo;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetButtonElementInfo(colorItem.State, colorItem.Bounds));
			info.Cache.DrawRectangle(Pens.Black, colorItem.GlyphBounds);
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(colorItem.Color), Rectangle.Inflate(colorItem.GlyphBounds, -1, -1));
			viewInfo.PaintAppearance.DrawString(info.Cache, colorItem.Description, colorItem.DescriptionBounds);
		}
	}
	public class InnerColorListControlHandler : IDisposable {
		InnerColorListControl owner;
		InnerColorListControlKeyboardHandler keyboardHandler;
		public InnerColorListControlHandler(InnerColorListControl owner) {
			this.owner = owner;
			this.keyboardHandler = CreateKeyboardHandler(owner);
		}
		protected virtual InnerColorListControlKeyboardHandler CreateKeyboardHandler(InnerColorListControl owner) {
			return new InnerColorListControlKeyboardHandler(owner);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			Owner.ViewInfo.UpdateState(e.Location);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			Owner.ViewInfo.UpdateState(e.Location);
			DoCheckClick(e.Location);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			Owner.ViewInfo.CheckHotObject(e.Location);
		}
		public virtual void OnMouseEnter(EventArgs e) {
			Owner.ViewInfo.CheckHotObject();
		}
		public virtual void OnMouseLeave(EventArgs e) {
			Owner.ViewInfo.ResetHotObject();
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			if(!Owner.ViewInfo.IsVScrollVisible) return;
			int wheelChange = 1;
			Owner.SetTopItem(Owner.TopItem + ((e.Delta > 0) ? -wheelChange : wheelChange));
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			KeyboardHandler.OnKeyDown(e);
			if(IsPostKey(e)) {
				ColorItemInfo colorItem = Owner.ViewInfo.GetSelectedItem();
				if(colorItem != null) DoPostValue(colorItem.ColorItem);
			}
		}
		protected virtual bool IsPostKey(KeyEventArgs e) {
			return e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space;
		}
		public InnerColorListControlKeyboardHandler KeyboardHandler { get { return keyboardHandler; } }
		protected virtual void DoCheckClick(Point pt) {
			InnerColorListControlHitInfo hitInfo = Owner.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.HitTest == InnerColorListControlHitTest.Item) {
				DoPostValue(hitInfo.ColorItem.ColorItem);
			}
		}
		protected virtual void DoPostValue(ColorItem colorItem) {
			Owner.SelectedColorItem = colorItem;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		public InnerColorListControl Owner { get { return owner; } }
	}
	public class InnerColorListControlKeyboardHandler {
		InnerColorListControl owner;
		public InnerColorListControlKeyboardHandler(InnerColorListControl owner) {
			this.owner = owner;
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(Owner.Colors.Count == 0) return;
			switch(e.KeyCode) {
				case Keys.Up: DoMoveUp(); break;
				case Keys.Down: DoMoveDown(); break;
				case Keys.PageUp: DoMovePageUp(); break;
				case Keys.PageDown: DoMovePageDown(); break;
				case Keys.Home: DoMoveHome(); break;
				case Keys.End: DoMoveEnd(); break;
			}
		}
		public virtual void DoMoveUp() {
			if(!Owner.ViewInfo.IsHotItemInitialized()) return;
			ColorItemInfo colorItem = Owner.ViewInfo.GetSelectedItem();
			if(colorItem != null) DoSetPrev(colorItem);
		}
		public virtual void DoMoveDown() {
			if(!Owner.ViewInfo.IsHotItemInitialized()) {
				DoSetTop();
			}
			else {
				ColorItemInfo colorItem = Owner.ViewInfo.GetSelectedItem();
				if(colorItem != null) DoSetNext(colorItem);
			}
		}
		public virtual void DoMovePageUp() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetSelectedItem();
			if(colorItem != null) DoSetPrevPage(colorItem);
		}
		public virtual void DoMovePageDown() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetSelectedItem();
			if(colorItem != null) DoSetNextPage(colorItem);
		}
		public virtual void DoMoveHome() {
			DoSetTop();
		}
		public virtual void DoMoveEnd() {
			DoSetBottom();
		}
		protected virtual void DoSetTop() {
			if(Owner.Colors.Count != 0) DoSetItem(Owner.Colors[0], true);
		}
		protected virtual void DoSetBottom() {
			if(Owner.Colors.Count != 0) DoSetItem(Owner.Colors[Owner.Colors.Count - 1], false);
		}
		protected virtual void DoSetNext(ColorItemInfo colorItem) {
			int pos = Owner.Colors.IndexOf(colorItem.ColorItem);
			if(pos == -1 || pos == Owner.Colors.Count - 1) return;
			DoSetItem(Owner.Colors[pos + 1], false);
		}
		protected virtual void DoSetPrev(ColorItemInfo colorItem) {
			int pos = Owner.Colors.IndexOf(colorItem.ColorItem);
			if(pos > 0) DoSetItem(Owner.Colors[pos - 1], true);
		}
		protected virtual void DoSetNextPage(ColorItemInfo colorItem) {
			int pos = Owner.Colors.IndexOf(colorItem.ColorItem);
			if(pos == -1 || pos == Owner.Colors.Count - 1) return;
			int newPos = Math.Min(Owner.Colors.Count - 1, pos + Owner.VisibleItemCount);
			DoSetItem(Owner.Colors[newPos], false);
		}
		protected virtual void DoSetPrevPage(ColorItemInfo colorItem) {
			int pos = Owner.Colors.IndexOf(colorItem.ColorItem);
			if(pos <= 0) return;
			int newPos = Math.Max(0, pos - Owner.VisibleItemCount);
			DoSetItem(Owner.Colors[newPos], false);
		}
		protected virtual void DoSetItem(ColorItem colorItem, bool topAnchor) {
			Owner.ViewInfo.ScrollInto(colorItem, topAnchor);
			Owner.ViewInfo.SetSelectedItem(colorItem);
			Owner.Invalidate();
		}
		public InnerColorListControl Owner { get { return owner; } }
	}
	public class InnerColorListControlScrollController : ScrollControllerBase {
		public InnerColorListControlScrollController(InnerColorListControl owner)
			: base(owner) {
		}
		public new InnerColorListControl Owner { get { return base.Owner as InnerColorListControl; } }
	}
}
