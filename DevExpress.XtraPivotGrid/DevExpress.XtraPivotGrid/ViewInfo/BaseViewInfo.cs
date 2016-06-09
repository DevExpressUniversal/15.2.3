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

using DevExpress.Utils.Drawing;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public interface IViewInfoControl {
		Rectangle ClientRectangle { get; }
		void Invalidate(Rectangle bounds);
		void Update();
		Control ControlOwner {get; }
		void InvalidateScrollBars();
		void UpdateScrollBars();
		void EnableScrollBars(bool enabled);
		bool ScrollBarOverlap { get; }
		bool IsDesignMode { get; }
	}
	public class ViewInfoPaintArgs {
		Control control;
		Graphics graphics;
		Rectangle clipRectangle;
		GraphicsCache graphicsCache;
		public ViewInfoPaintArgs(Control control, PaintEventArgs e) {
			this.control = control;
			this.graphics = e.Graphics;
			this.clipRectangle = e.ClipRectangle;
			this.graphicsCache = new GraphicsCache(e.Graphics);
		}
		protected internal bool IsFocused { get { return control != null ? control.Focused : false; } }
		public Rectangle ClientRectangle { get { return control != null ? control.ClientRectangle : Rectangle.Empty; } }
		public Graphics Graphics { get { return graphics; } }
		public Rectangle ClipRectangle { get { return clipRectangle; } }
		public GraphicsCache GraphicsCache { get { return graphicsCache; } }
	}
	public class BaseViewInfo : IDisposable {
		bool isReady;
		BaseViewInfoCollection children;
		BaseViewInfo parent;
		Rectangle paintBounds;
		Size boundsOffset;
		BaseViewInfo activeViewInfo;
		BaseViewInfo hotTrackViewInfo;
		Rectangle bounds;
		Rectangle controlBounds = Rectangle.Empty;
		Rectangle originalControlBounds = Rectangle.Empty;
		public BaseViewInfo()
			: this(true) {
		}
		public BaseViewInfo(bool destroyChildrenOnClear) {
			this.boundsOffset = Size.Empty;
			this.isReady = false;
			this.parent = null;
			this.activeViewInfo = null;
			this.hotTrackViewInfo = null;
		}
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds == value) return;
				bounds = value;
				OnBoundsChanged();
			}
		}
		public Point Location {
			get { return Bounds.Location; }
			set {
				if(Location == value) return;
				SetBounds(value.X, value.Y, null, null); 
			}
		}
		public Size Size {
			get { return Bounds.Size; }
			set {
				if(Size == value) return;
				SetBounds(null, null, value.Width, value.Height);
			}
		}
		public int X {
			get { return Bounds.X; }
			set {
				if(X == value) return;
				SetBounds(value, null, null, null); 
			}
		}
		public int Y {
			get { return Bounds.Y; }
			set {
				if(Y == value) return;
				SetBounds(null, value, null, null);
			}
		}
		public int Width {
			get { return Bounds.Width; }
			set {
				if(Width == value) return;
				SetBounds(null, null, value, null);
			}
		}
		public int Height {
			get { return Bounds.Height; }
			set {
				if(Height == value) return;
				SetBounds(null, null, null, value);
			}
		}
		public Size BoundsOffset {
			get { return boundsOffset; }
			set {
				if(Parent != null && Parent != Root) throw new Exception("BoundsOffset can be set at the first level only");
				if(boundsOffset == value) return;
				boundsOffset = value;
				OnBoundsChanged();
			}
		}
		public virtual Rectangle ControlBounds {
			get {
				if(controlBounds.IsEmpty)
					controlBounds = CorrectControlBoundsBasedOnOffsets(OriginalControlBounds);
				return controlBounds;
			}
		}
		public virtual bool IsRightToLeft { get { return false; } }
		public Rectangle RightToLeftRect(Rectangle rectangle) {
			Rectangle rect = new Rectangle(rectangle.Location, rectangle.Size);
			if(IsRightToLeft) {
				rect.Offset(Root.Bounds.Width - 2 * rect.X - rect.Width, 0);
			}
			return rect;
		}
		public Rectangle OriginalControlBounds {
			get {
				if(originalControlBounds.IsEmpty)
					originalControlBounds = GetOriginalControlBounds();
				return originalControlBounds;
			}
		}
		Rectangle GetOriginalControlBounds() {
			Rectangle controlBounds = Bounds;
			controlBounds.Offset(BoundsOffset.Width, BoundsOffset.Height);
			if(parent == null)
				return controlBounds;
			Rectangle parentControlBounds = parent.OriginalControlBounds;
			controlBounds.X += parentControlBounds.Left;
			controlBounds.Y += parentControlBounds.Top;
			return controlBounds;
		}
		Rectangle CorrectControlBoundsBasedOnOffsets(Rectangle rect) {
			BaseViewInfo viewInfo = this;
			while(viewInfo != null) {
				if(viewInfo != this || !viewInfo.BoundsOffset.IsEmpty) {
					Rectangle controlBounds = viewInfo == this ? viewInfo.Bounds : viewInfo.ControlBounds;	
					if(controlBounds.X > rect.X) {
						rect.Width -= controlBounds.X - rect.X;
						rect.X = controlBounds.X;
					}
					if(controlBounds.Y > rect.Y) {
						rect.Height -= controlBounds.Y - rect.Y;
						rect.Y = controlBounds.Y;
					}
				}
				viewInfo = viewInfo.Parent;
			}
			return rect;
		}
		public Rectangle PaintBounds {
			get {
				if(paintBounds.IsEmpty) {
					paintBounds = CalculatePaintBounds();
				}
				return paintBounds;
			}
		}
		public virtual Rectangle PaintBoundsWithScroll { get { return PaintBounds; } }
		protected virtual Rectangle CalculatePaintBounds() {
			if((ControlBounds.Width <= 0) || (ControlBounds.Height <= 0))
				return Rectangle.Empty;
			return RightToLeftRect(ControlBounds);
		}
		public void EnsureIsCalculated() {
			if(!IsReady) 
				Calculate();
		}
		bool isCalculating;
		protected void Calculate() {
			if(this.isCalculating)
				throw new PivotViewInfoException("double entrance");
			this.isCalculating = true;
			try {
				OnBeforeCalculating();
				if(this.parent != null)
					this.parent.OnBeforeChildCalculating(this);
				CalculateChildren();
				InternalCalculate();
				if(this.parent != null)
					this.parent.OnAfterChildCalculated(this);
				this.isReady = true;
				OnAfterCalculated();
			} finally {
				this.isCalculating = false;
			}
		}
		public void Clear() {
			if(this.children != null) 
				ClearChildren();
			this.isReady = false;
			this.activeViewInfo = null;
			this.hotTrackViewInfo = null;
			Bounds = Rectangle.Empty;
			InternalClear();
		}
		protected internal void OnBoundsChanged() {
			this.originalControlBounds = Rectangle.Empty;
			this.controlBounds = Rectangle.Empty;
		}
		void OnChildrenBoundsChanged() {
			if(this.children == null) return;
			foreach(BaseViewInfo child in this.children) {
				child.OnBoundsChanged();
			}
		}
		public void SetBounds(int? x, int? y, int? width, int? height) {
			Rectangle bounds = Bounds;
			if(x != null)
				bounds.X = x.Value;
			if(y != null)
				bounds.Y = y.Value;
			if(width != null)
				bounds.Width = width.Value;
			if(height != null)
				bounds.Height = height.Value;
			Bounds = bounds;
		}
		public bool IsReady { get { return isReady; } }
		public virtual void KeyDown(KeyEventArgs e) { }
		public virtual void MouseMove(MouseEventArgs e) {
			if(ActiveViewInfo != null)
				ActiveViewInfo.MouseMoveCore(e);
			else {
				BaseViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y);
				HotTrackViewInfo = viewInfo;
				if(viewInfo != null) {
					viewInfo.MouseMoveCore(e);
				}
			}
		}
		public virtual void MouseDown(MouseEventArgs e) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y);
			if(viewInfo != null) {
				this.activeViewInfo = viewInfo.MouseDownCore(e);
				if(this.activeViewInfo != null)
					this.activeViewInfo.Invalidate();
			}
		}
		public virtual void MouseUp(MouseEventArgs e) {
			if(ActiveViewInfo != null)
				ActiveViewInfo.MouseUpCore(e);
			this.activeViewInfo = null;
		}
		public virtual void DoubleClick(MouseEventArgs e) {
			if(ActiveViewInfo != null)
				ActiveViewInfo.DoubleClick(e);
		}
		public void MouseEnter() {
			MouseEnterCore();
		}
		public void MouseLeave() {
			HotTrackViewInfo = null;
			MouseLeaveCore();
		}
		public virtual void OnGestureTwoFingerSelection(Point start, Point end) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(start);
			if(viewInfo != null)
				viewInfo.OnGestureTwoFingerSelectionCore(start, end);
		}
		public BaseViewInfo GetViewInfoAtPoint(int x, int y) {
			return GetViewInfoAtPoint(new Point(x, y), true);
		}
		public BaseViewInfo GetViewInfoAtPoint(int x, int y, bool recursive) {
			return GetViewInfoAtPoint(new Point(x, y), recursive);
		}
		public BaseViewInfo GetViewInfoAtPoint(Point pt) {
			return GetViewInfoAtPoint(pt, true);
		}
		public BaseViewInfo GetViewInfoAtPoint(Point pt, bool recursive) {
			if(!PaintBounds.Contains(pt)) return null;
			for(int i = ChildCount - 1; i >= 0; i--) {
				if(this[i].PaintBounds.Contains(pt)) {
					if(recursive)
						return this[i].GetViewInfoAtPoint(pt, recursive);
					else
						return this[i];
				}
			}
			return this;
		}
		protected virtual bool CheckControlBounds { get { return true; } }
		public void Paint(Control control, PaintEventArgs e) {
			Paint(new ViewInfoPaintArgs(control, e));
		}
		public void Paint(ViewInfoPaintArgs e) {
			if(!IsReady) EnsureIsCalculated();
			if(CheckControlBounds) {
				if(!e.ClipRectangle.IsEmpty && (!PaintBounds.IntersectsWith(e.ClipRectangle))) 
					return;
				if(e.ClientRectangle != Rectangle.Empty && !PaintBounds.IntersectsWith(e.ClientRectangle)) 
					return;
			}
			InternalPaint(e);
			PaintChildren(e);
			AfterPaint(e);
		}
		public void Invalidate() {
			Invalidate(PaintBounds);
		}
		protected virtual void Invalidate(Rectangle bounds) { }
		public void ResetPaintBounds() {
			this.paintBounds = Rectangle.Empty;
			for(int i = 0; i < ChildCount; i++) {
				this[i].ResetPaintBounds();
			}
			OnBoundsChanged();
		}
		public void AddChild(BaseViewInfo viewInfo) {
			if(children == null)
				children = new BaseViewInfoCollection();
			viewInfo.parent = this;
			children.Add(viewInfo);
		}
		public int ChildIndex { get { return parent != null ? Parent.ChildIndexOf(this) : -1; } }
		public int ChildIndexOf(BaseViewInfo viewInfo) { return HasChildren ? children.IndexOf(viewInfo) : -1; }
		public int ChildCount { get { return children != null ? children.Count : 0; } }
		public BaseViewInfo GetChild(int index) { return HasChildren ? children[index] : null; }
		public BaseViewInfo this[int index] { get { return GetChild(index); } }
		public bool HasChildren { get { return ChildCount > 0; } }
		public BaseViewInfo FirstChild { get { return HasChildren ? this[0] : null; } }
		public BaseViewInfo LastChild { get { return HasChildren ? this[ChildCount - 1] : null; } }
		public BaseViewInfo Parent { get { return parent; } }
		public virtual BaseViewInfo Root { get { return Parent == null ? this : Parent.Root; } }
		public virtual FieldMeasures FieldMeasures { get { return Root.FieldMeasures; } }
		public bool IsActive { get { return Root != null && Root.ActiveViewInfo == this; } }
		public BaseViewInfo ActiveViewInfo { get { return activeViewInfo; } }
		public bool IsHotTrack { get { return Root != null && Root.HotTrackViewInfo == this; } }
		public BaseViewInfo HotTrackViewInfo {
			get { return hotTrackViewInfo; }
			set {
				if(HotTrackViewInfo == value) return;
				if(HotTrackViewInfo != null)
					HotTrackViewInfo.MouseLeaveCore();
				this.hotTrackViewInfo = value;
				if(HotTrackViewInfo != null)
					HotTrackViewInfo.MouseEnterCore();
			}
		}
		protected virtual void MouseMoveCore(MouseEventArgs e) {
		}
		protected virtual BaseViewInfo MouseDownCore(MouseEventArgs e) {
			return null;
		}
		protected virtual void MouseUpCore(MouseEventArgs e) {
		}
		protected virtual void MouseEnterCore() { }
		protected virtual void MouseLeaveCore() { }
		protected virtual void OnGestureTwoFingerSelectionCore(Point start, Point end) { }
		protected virtual void InternalCalculate() { }
		protected virtual void InternalClear() { }
		protected virtual void InternalPaint(ViewInfoPaintArgs e) { }
		protected virtual void AfterPaint(ViewInfoPaintArgs e) { }
		protected virtual void OnAfterCalculated() { }
		protected virtual void OnBeforeCalculating() { }
		protected virtual void OnAfterChildrenCalculated() { }
		protected virtual void OnBeforeChildrenCalculating() { }
		protected virtual void OnAfterChildCalculated(BaseViewInfo viewInfo) { }
		protected virtual void OnBeforeChildCalculating(BaseViewInfo viewInfo) { }
		protected void ClearChildren() {
			for(int i = 0; i < children.Count; i++) {
				children[i].Dispose();
			}
			children.Clear();
		}
		protected void RemoveChildren(int index) {
			BaseViewInfo child = children[index];
			children.RemoveAt(index);
			child.Dispose();
		}
		void CalculateChildren() {
			if(this.children == null) return;
			this.children.SetBounds(Bounds);
			OnBeforeChildrenCalculating();
			this.children.CalculateViewInfo();
			OnAfterChildrenCalculated();
		}		
		protected virtual void PaintChildren(ViewInfoPaintArgs e) {
			if(this.children != null)
				this.children.Paint(e);
		}
		#region IDisposable Members
		public virtual void Dispose() {
			Clear();
		}
		#endregion
	}
	public enum PivotGridViewInfoState { Normal, FieldResizing };
	public class BaseViewInfoCollection : Collection<BaseViewInfo> {
		public BaseViewInfoCollection() { }
		public void CalculateViewInfo() {
			for(int i = 0; i < Count; i++)
				this[i].EnsureIsCalculated();
		}
		public void ClearViewInfo() {
			for(int i = 0; i < Count; i++)
				this[i].Clear();
		}
		public void Paint(ViewInfoPaintArgs e) {
			for(int i = 0; i < Count; i++)
				this[i].Paint(e);
		}
		public void SetBounds(Rectangle Bounds) {
			for(int i = 0; i < Count; i++)
				if(this[i].Bounds.IsEmpty) {
					this[i].Size = Bounds.Size;
				}
		}
	}
	internal class RectangleHelper {
		public static Rectangle Shrink(Rectangle rect, int dLeft, int dTop, int dRight, int dBottom) {
			Rectangle result = rect;
			result.X += dLeft;
			result.Width -= dLeft + dRight;
			result.Y += dTop;
			result.Height -= dTop + dBottom;
			return result;
		}
	}
	public class PivotViewInfoException : Exception {
		public PivotViewInfoException(string message)
			: base(message) {
		}
	}
}
