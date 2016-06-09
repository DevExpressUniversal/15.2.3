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
using DevExpress.XtraBars.Docking.Paint;
namespace DevExpress.XtraBars.Docking.Helpers {
	public abstract class BaseZone {
		DockLayout owner;
		protected BaseZone(DockLayout owner) {
			this.owner = owner;
		}
		public virtual bool Contains(Point pt) {
			return Bounds.Contains(pt);
		}
		protected internal Point PointToScreen(Point pt) {
			return Owner.Panel.PointToScreen(pt);
		}
		protected internal Point PointToClient(Point pt) {
			return Owner.Panel.PointToClient(pt);
		}
		protected internal Rectangle RectangleToScreen(Rectangle bounds) {
			return Owner.Panel.RectangleToScreen(bounds);
		}
		protected internal Rectangle RectangleToClient(Rectangle bounds) {
			return Owner.Panel.RectangleToClient(bounds);
		}
		protected virtual void FireChanged() {
			Owner.Panel.FireChanged();
		}
		public abstract Rectangle Bounds { get; }
		protected DockLayout Owner { get { return owner; } }
		protected internal virtual Cursor Cursor { get { return Cursors.Default; } }
	}
	public class ResizeZone : BaseZone {
		DockingStyle side;
		Point sizingPos;
		public ResizeZone(DockLayout owner, DockingStyle side)
			: base(owner) {
			this.side = side;
			this.sizingPos = LayoutConsts.InvalidPoint;
		}
		bool? isRightToLeftLayoutCore;
		protected bool IsRightToLeftLayout {
			get {
				if(!isRightToLeftLayoutCore.HasValue)
					isRightToLeftLayoutCore = CalcIsRightToLeftLayout();
				return isRightToLeftLayoutCore.Value;
			}
		}
		bool CalcIsRightToLeftLayout() {
			Form form = (Owner.Panel != null) ? Owner.Panel.FindForm() : null;
			return (form != null) && DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeftLayout(form);
		}
		protected internal virtual void StartSizing(Point pt) {
			pt = GetSizingPos(pt);
			var adorner = GetSizingAdorer(SizingPanel);
			if(adorner != null)
				BeginSplit(adorner, PointToScreen(pt));
			else
				DrawSizer(pt);
			SizingPos = pt;
		}
		protected internal virtual void DoSizing(Point pt) {
			pt = GetSizingPos(pt);
			if(pt == SizingPos) return;
			var adorner = GetSizingAdorer(SizingPanel);
			if(adorner == null)
				DrawSizer(SizingPos);
			int change = IsHorizontal ? (pt.X - SizingPos.X) : (pt.Y - SizingPos.Y);
			SizingPos = pt;
			if(adorner != null)
				UpdateSplit(adorner, change);
			else
				DrawSizer(SizingPos);
		}
		protected internal virtual void EndSizing(Point startPoint, Point endPoint) {
			if(endPoint != SizingPos) endPoint = SizingPos;
			endPoint = GetSizingPos(endPoint);
			var adorner = GetSizingAdorer(SizingPanel);
			if(adorner != null)
				ResetSplit(adorner);
			else
				DrawSizer(endPoint);
			Resize(startPoint, endPoint);
		}
		protected internal virtual void Resize(Point startPoint, Point endPoint) {
			Size newSize = GetNewSize(startPoint, endPoint);
			Size oldSize = Owner.Size;
			Owner.SavedSize = newSize;
			Owner.Size = newSize;
			if(Owner.Size != oldSize)
				FireChanged();
		}
		protected internal virtual void DrawSizer(Point pt) {
			ElementPainter.FillReversibleRectangle(GetSizerBounds(PointToScreen(pt), true), DockConsts.SelectionColor);
		}
		protected internal virtual Rectangle GetSizerBounds(Point screenPos, bool checkDPISettings) {
			DirectionRectangle sizerBounds = new DirectionRectangle(Owner.Panel.RectangleToScreen(Bounds), IsHorizontal);
			sizerBounds.SetSize(DockConsts.ResizeSelectionFrameWidth);
			sizerBounds.SetLocation((IsHorizontal ? screenPos.X : screenPos.Y) - DockConsts.ResizeSelectionFrameWidth / 2);
			Rectangle frameBounds = CheckFrameBounds(Owner.Panel, sizerBounds.Bounds);
			Rectangle screenRect = checkDPISettings ? DPISettings.CheckRelativeScreenBounds(Owner.Panel, frameBounds) : frameBounds;
			return screenRect;
		}
		static Rectangle CheckFrameBounds(DockPanel panel, Rectangle frameBounds) {
			FloatForm parentFloatForm = panel.FindForm() as FloatForm;
			if(parentFloatForm != null)
				return CheckFrameBoundsCore(parentFloatForm, frameBounds);
			else {
				ContainerControl container = panel.DockManager.Form;
				frameBounds = CheckFrameBoundsCore(container, frameBounds);
				Form containerForm = container as Form;
				if(containerForm == null)
					containerForm = container.FindForm();
				if(containerForm != null) {
					frameBounds = CheckFrameBoundsCore(containerForm, frameBounds);
					if(containerForm.IsMdiChild)
						frameBounds = CheckFrameBoundsCore(containerForm.MdiParent, frameBounds);
				}
			}
			return frameBounds;
		}
		static Rectangle CheckFrameBoundsCore(Control container, Rectangle frameBounds) {
			if(container == null || !container.IsHandleCreated) return frameBounds;
			Rectangle containerScreeenRect = container.RectangleToScreen(container.ClientRectangle);
			return Rectangle.Intersect(frameBounds, containerScreeenRect);
		}
		protected internal virtual Point GetSizingPos(Point pt) {
			Rectangle screenRect = Owner.GetBoundingResizeRectangle();
			pt = PointToScreen(pt);
			if(pt.X < screenRect.Left) pt.X = screenRect.Left;
			if(pt.X > screenRect.Right) pt.X = screenRect.Right;
			if(pt.Y < screenRect.Top) pt.Y = screenRect.Top;
			if(pt.Y > screenRect.Bottom) pt.Y = screenRect.Bottom;
			return PointToClient(pt);
		}
		public virtual Size GetNewSize(Point startPoint, Point endPoint) {
			Size result = Owner.Size;
			if(IsHorizontal) {
				if(IsRTLAware())
					result.Width += (IsHead ? endPoint.X - startPoint.X : startPoint.X - endPoint.X);
				else
					result.Width += (IsHead ? startPoint.X - endPoint.X : endPoint.X - startPoint.X);
			}
			else
				result.Height += (IsHead ? startPoint.Y - endPoint.Y : endPoint.Y - startPoint.Y);
			return result;
		}
		public virtual void Release() {
			DrawSizer(SizingPos);
			SizingPos = LayoutConsts.InvalidPoint;
		}
		public override Rectangle Bounds {
			get {
				DirectionRectangle dBounds = new DirectionRectangle(Owner.Panel.ClientRectangle,
					IsRTLAware() ? GetOppositeSide() : Side);
				return dBounds.GetSideRectangle(Owner.Panel.GetResizeZoneWidth(), 0);
			}
		}
		bool IsRTLAware() {
			return IsHorizontal && IsRightToLeftLayout && (Owner.Level == 0 || (Owner.Panel != null && Owner.Panel.ParentAutoHideControl != null));
		}
		bool IsHorizontal {
			get { return LayoutRectangle.GetIsHorizontal(Side); }
		}
		bool IsHead {
			get { return LayoutRectangle.GetIsHead(Side); }
		}
		protected Point SizingPos {
			get { return sizingPos; }
			set { sizingPos = value; }
		}
		protected internal DockingStyle Side {
			get { return side; }
		}
		DockingStyle GetOppositeSide() {
			switch(side) {
				case DockingStyle.Left:
					return DockingStyle.Right;
				case DockingStyle.Right:
					return DockingStyle.Left;
			}
			return side;
		}
		protected internal override Cursor Cursor {
			get { return (IsHorizontal ? Cursors.VSplit : Cursors.HSplit); }
		}
		public virtual DockPanel SizingPanel {
			get { return Owner.Panel; }
		}
		SizingAdorer GetSizingAdorer(DockPanel panel) {
			if(panel == null || panel.IsMdiDocument)
				return null;
			DockManager dockManager = (panel != null) ? panel.DockManager : null;
			FloatForm floatForm = panel.FindForm() as FloatForm;
			if(floatForm != null)
				return floatForm.SizingAdorner;
			return (dockManager != null) ? dockManager.SizingAdorner : null;
		}
		void BeginSplit(SizingAdorer adorner, Point screenPoint) {
			adorner.StartSizing(GetSizerBounds(screenPoint, false));
		}
		void UpdateSplit(SizingAdorer adorner, int change) {
			adorner.Sizing(IsHorizontal, change);
		}
		void ResetSplit(SizingAdorer adorner) {
			adorner.EndSizing();
		}
	}
	public class ResizeZoneReference : ResizeZone {
		ResizeZone zoneReference;
		public ResizeZoneReference(DockLayout owner, ResizeZone resizeZone)
			: base(owner, resizeZone.Side) {
			this.zoneReference = resizeZone;
		}
		protected internal override void StartSizing(Point pt) {
			if(Side == DockingStyle.Float) return;
			base.StartSizing(pt);
		}
		protected internal override void EndSizing(Point startPoint, Point endPoint) {
			if(Side == DockingStyle.Float) return;
			base.EndSizing(startPoint, endPoint);
		}
		protected internal override void DoSizing(Point pt) {
			if(Side == DockingStyle.Float) return;
			base.DoSizing(pt);
		}
		protected internal override void Resize(Point startPoint, Point endPoint) {
			ZoneReference.Resize(startPoint, endPoint);
		}
		protected internal override void DrawSizer(Point pt) {
			ZoneReference.DrawSizer(PointToParent(pt));
		}
		protected internal override Rectangle GetSizerBounds(Point screenPos, bool checkDPISettings) {
			return ZoneReference.GetSizerBounds(screenPos, checkDPISettings);
		}
		protected internal override Point GetSizingPos(Point pt) {
			return PointToItself(ZoneReference.GetSizingPos(PointToParent(pt)));
		}
		public override Size GetNewSize(Point startPoint, Point endPoint) {
			return ZoneReference.GetNewSize(startPoint, endPoint);
		}
		Point PointToParent(Point pt) { return ZoneReference.PointToClient(PointToScreen(pt)); }
		Point PointToItself(Point pt) { return PointToClient(ZoneReference.PointToScreen(pt)); }
		public override Rectangle Bounds {
			get {
				Rectangle result = RectangleToClient(ZoneReference.RectangleToScreen(ZoneReference.Bounds));
				return Rectangle.Intersect(result, Owner.Panel.ClientRectangle);
			}
		}
		protected internal override Cursor Cursor {
			get {
				if(Side == DockingStyle.Float) return Cursor.Current;
				return base.Cursor;
			}
		}
		protected ResizeZone ZoneReference { get { return zoneReference; } }
		public override DockPanel SizingPanel { get { return ZoneReference.SizingPanel; } }
	}
	public class FloatResizeZone : ResizeZone {
		FloatResizeZonePosition position;
		Rectangle initialBounds;
		Point initialPoint;
		public FloatResizeZone(DockLayout owner, FloatResizeZonePosition position)
			: base(owner, DockingStyle.Float) {
			this.position = position;
			this.initialBounds = LayoutConsts.InvalidRectangle;
			this.initialPoint = LayoutConsts.InvalidPoint;
		}
		protected internal override void StartSizing(Point pt) {
			if(FloatForm == null) return;
			this.initialBounds = FloatForm.Bounds;
			this.initialPoint = pt;
			Owner.LockCalcResizeZones();
		}
		protected internal override void DoSizing(Point pt) {
			FloatFormBounds = GetNewFloatBounds(pt);
		}
		protected virtual Rectangle GetNewFloatBounds(Point pt) {
			Size floatSize = LayoutConsts.InvalidSize;
			Point floatLocation = InitialBounds.Location;
			int width = Math.Max(FloatForm.MinimumSize.Width, InitialBounds.Width + (pt.X - InitialPoint.X));
			int height = Math.Max(FloatForm.MinimumSize.Height, InitialBounds.Height + (pt.Y - InitialPoint.Y));
			int offsetX = GetOffset(InitialBounds.Width, FloatForm.MinimumSize.Width, FloatForm.MaximumSize.Width, Owner.Panel.PointToScreen(pt).X, floatLocation.X);
			int offsetY = GetOffset(InitialBounds.Height, FloatForm.MinimumSize.Height, FloatForm.MaximumSize.Height, Owner.Panel.PointToScreen(pt).Y, floatLocation.Y);
			switch(Position) {
				case FloatResizeZonePosition.LeftTop:
					floatSize = new Size(InitialBounds.Width - offsetX, InitialBounds.Height - offsetY);
					floatLocation.Offset(offsetX, offsetY);
					break;
				case FloatResizeZonePosition.RightBottom:
					floatSize = new Size(width, height);
					break;
				case FloatResizeZonePosition.RightTop:
					floatSize = new Size(width, InitialBounds.Height - offsetY);
					floatLocation.Offset(0, offsetY);
					break;
				case FloatResizeZonePosition.LeftBottom:
					floatSize = new Size(InitialBounds.Width - offsetX, height);
					floatLocation.Offset(offsetX, 0);
					break;
				case FloatResizeZonePosition.Left:
					floatSize = new Size(InitialBounds.Width - offsetX, InitialBounds.Height);
					floatLocation.Offset(offsetX, 0);
					break;
				case FloatResizeZonePosition.Right:
					floatSize = new Size(width, InitialBounds.Height);
					break;
				case FloatResizeZonePosition.Top:
					floatSize = new Size(InitialBounds.Width, InitialBounds.Height - offsetY);
					floatLocation.Offset(0, offsetY);
					break;
				case FloatResizeZonePosition.Bottom:
					floatSize = new Size(InitialBounds.Width, height);
					break;
			}
			return new Rectangle(floatLocation, floatSize);
		}
		public override Size GetNewSize(Point startPoint, Point endPoint) {
			return GetNewFloatBounds(endPoint).Size;
		}
		int GetOffset(int size, int minSize, int maxSize, int x, int beginX) {
			int result = x - beginX;
			if(size - result < minSize)
				result = size - minSize;
			if(size - result > maxSize && maxSize != 0)
				result = maxSize - size;
			return result;
		}
		protected internal override void EndSizing(Point startPoint, Point endPoint) {
			Owner.UnlockCalcResizeZones();
			Owner.CalcViewInfo();
		}
		protected internal override Cursor Cursor {
			get {
				switch(Position) {
					case FloatResizeZonePosition.LeftTop:
					case FloatResizeZonePosition.RightBottom:
						return Cursors.SizeNWSE;
					case FloatResizeZonePosition.RightTop:
					case FloatResizeZonePosition.LeftBottom:
						return Cursors.SizeNESW;
					case FloatResizeZonePosition.Left:
					case FloatResizeZonePosition.Right:
						return Cursors.SizeWE;
					case FloatResizeZonePosition.Top:
					case FloatResizeZonePosition.Bottom:
						return Cursors.SizeNS;
				}
				return Cursors.Default;
			}
		}
		public override Rectangle Bounds {
			get {
				DockingStyle[] sides = null;
				switch(Position) {
					case FloatResizeZonePosition.LeftTop:
						sides = new DockingStyle[] { DockingStyle.Left, DockingStyle.Top };
						break;
					case FloatResizeZonePosition.RightBottom:
						sides = new DockingStyle[] { DockingStyle.Right, DockingStyle.Bottom };
						break;
					case FloatResizeZonePosition.RightTop:
						sides = new DockingStyle[] { DockingStyle.Right, DockingStyle.Top };
						break;
					case FloatResizeZonePosition.LeftBottom:
						sides = new DockingStyle[] { DockingStyle.Left, DockingStyle.Bottom };
						break;
					default:
						sides = new DockingStyle[] { (DockingStyle)((int)Position - 3) };
						break;
				}
				Rectangle result = Owner.Panel.ClientRectangle;
				int width = DockConsts.ResizeZoneWidth * sides.Length;
				for(int i = 0; i < sides.Length; i++) {
					DirectionRectangle dBounds = new DirectionRectangle(result, sides[i]);
					result = dBounds.GetSideRectangle(width);
				}
				return result;
			}
		}
		protected FloatForm FloatForm {
			get {
				if(Owner == null || Owner.Panel == null) return null;
				return Owner.Panel.FloatForm;
			}
		}
		protected internal FloatResizeZonePosition Position { get { return position; } }
		protected Rectangle InitialBounds { get { return initialBounds; } }
		protected Point InitialPoint { get { return initialPoint; } }
		protected Rectangle FloatFormBounds {
			get { return FloatForm.Bounds; }
			set {
				if(FloatFormBounds == value) return;
				FloatForm.SetFloatBoundsCore(value);
			}
		}
	}
	public class DockZone : BaseZone {
		int index;
		DockingStyle dock;
		IDockZonesOwner zonesOwner;
		public DockZone(IDockZonesOwner zonesOwner, DockingStyle dock, int index)
			: base(zonesOwner as DockLayout) {
			this.index = index;
			this.dock = dock;
			this.zonesOwner = zonesOwner;
		}
		public virtual void DrawPointer(DockLayout source) {
			if(!IsDockingAllowed(source)) return;
			DrawPointerCore(source);
		}
		protected virtual void DrawPointerCore(DockLayout source) {
			ElementPainter.DrawReversibleRectangle(GetPointerBounds(source));
		}
		protected internal virtual bool IsDockingAllowed(DockLayout source) {
			return source.Panel.Options.GetOptionByDockingStyle(DockStyle);
		}
		protected virtual Rectangle GetPointerBounds(DockLayout source) {
			return GetRectangleBounds(ZonesOwner.GetSourceDockSize(source, DockStyle), DockSide, 0);
		}
		public virtual void Dock(DockLayout source) {
			if(!IsDockingAllowed(source)) return;
			DockLayout dest = ZonesOwner as DockLayout;
			if(dest != null)
				dest.SetFloatVerticalCore(FloatVertical);
			source.DockTo((BaseLayoutInfo)ZonesOwner, DockStyle, Index);
			FireChanged();
		}
		protected override void FireChanged() {
			if(Owner != null)
				base.FireChanged();
			else
				LayoutManager.FireChanged();
		}
		protected internal virtual void MouseOver(DockLayout source, Point pt) { }
		protected internal virtual void MouseLeave(DockLayout source) { }
		protected internal virtual void BeginDock() { }
		protected internal virtual void EndDock() { }
		protected DockLayoutManager LayoutManager { get { return ZonesOwner as DockLayoutManager; } }
		protected virtual int LevelIndent { get { return ZonesOwner.DockZones.GetDockIndex(this) * DockConsts.DockZoneWidth; } }
		protected internal IDockZonesOwner ZonesOwner { get { return zonesOwner; } }
		protected bool FloatVertical { get { return !LayoutRectangle.GetIsVertical(DockSide); } }
		protected int IndexCore { get { return index; } set { index = value; } }
		protected internal int Index { get { return IndexCore; } }
		protected internal virtual DockingStyle DockSide { get { return DockStyle; } }
		protected internal virtual DockingStyle DockStyle { get { return dock; } }
		protected internal virtual Control TargetControl { get { return Owner.Panel; } }
		protected internal virtual bool TargetTabbed { get { return false; } }
		public override Rectangle Bounds { get { return GetRectangleBounds(DockConsts.DockZoneWidth, DockSide, LevelIndent); } }
		protected virtual Rectangle GetRectangleBounds(int size, DockingStyle dock, int indent) {
			DirectionRectangle dBounds = new DirectionRectangle(ZonesOwner.ScreenRectangle, dock);
			return dBounds.GetSideRectangle(size, indent);
		}
	}
	public class LayoutManagerDockZone : DockZone {
		DockLayout rootLayout;
		public LayoutManagerDockZone(DockLayoutManager zonesOwner, DockingStyle dock, int index, DockLayout rootLayout)
			: base(zonesOwner, dock, index) {
			this.rootLayout = rootLayout;
		}
		protected override Rectangle GetRectangleBounds(int size, DockingStyle dock, int indent) {
			if(RootLayout != null && RootLayout.Parent == null) return LayoutConsts.InvalidRectangle;
			DirectionRectangle dBounds = new DirectionRectangle(GetZoneScreenBounds(), dock);
			return dBounds.GetSideRectangle(size, indent);
		}
		Rectangle GetZoneScreenBounds() {
			Rectangle result = (RootLayout == null ? (Index == LayoutManager.Count ? LayoutManager.GetRestBounds(null) : LayoutManager.ClientBounds) : RootLayout.Bounds);
			if(TargetControl.IsHandleCreated)
				return TargetControl.RectangleToScreen(result);
			return Rectangle.Empty;
		}
		protected internal override Control TargetControl { get { return LayoutManager.DockManager.Form; } }
		protected internal override DockingStyle DockSide {
			get { return (InsertBehind ? LayoutRectangle.GetOppositeDockingStyle(base.DockSide) : base.DockSide); }
		}
		bool InsertBehind {
			get {
				if(RootLayout == null || RootLayout.Parent == null) return false;
				return (RootLayout.Index < Index);
			}
		}
		protected override int LevelIndent {
			get {
				if(Index != 0) {
					return (Index == LayoutManager.Count ? 1 : 0);
				}
				return base.LevelIndent;
			}
		}
		protected internal DockLayout RootLayout { get { return rootLayout; } }
	}
	public class DockZoneReference : DockZone {
		DockZone dockZone;
		public DockZoneReference(IDockZonesOwner zonesOwner, DockZone dockZone)
			: base(zonesOwner, dockZone.DockStyle, dockZone.Index) {
			this.dockZone = dockZone;
		}
		public override void Dock(DockLayout source) {
			DockZone.Dock(source);
		}
		public override void DrawPointer(DockLayout source) {
			DockZone.DrawPointer(source);
		}
		protected internal override bool IsDockingAllowed(DockLayout source) {
			return DockZone.IsDockingAllowed(source);
		}
		protected internal override DockingStyle DockSide { get { return DockZone.DockSide; } }
		protected DockZone DockZone { get { return dockZone; } }
	}
	public class FloatDockZone : DockZone {
		Point location;
		public FloatDockZone(DockLayout owner, Point location)
			: base(owner, DockingStyle.Float, -1) {
			this.location = location;
		}
		public override void Dock(DockLayout source) {
			if(!IsDockingAllowed(source)) return;
			Point floatLocation = new Point(location.X, Math.Max(0, location.Y));
			Rectangle workingAreaRect = GetWorkingArea(floatLocation);
			if(floatLocation.Y >= workingAreaRect.Bottom)
				floatLocation.Y = Math.Max(0, Math.Min(workingAreaRect.Bottom - FloatSize.Height, floatLocation.Y));
			source.Panel.MakeFloat(floatLocation);
			FireChanged();
		}
		protected static Rectangle GetWorkingArea(Point point) {
			if(SystemInformation.MonitorCount < 2) return SystemInformation.WorkingArea;
			Screen screen = Screen.FromPoint(point);
			return screen.WorkingArea;
		}
		protected override Rectangle GetRectangleBounds(int size, DockingStyle dock, int indent) { return new Rectangle(location, FloatSize); }
		protected internal override Control TargetControl { get { return null; } }
		protected Size FloatSize { get { return Owner.FloatSize; } }
		protected internal override Cursor Cursor {
			get {
				if(!Owner.Panel.Options.AllowFloating) return Owner.DockManager.DockingOptions.CursorFloatCanceled == null ? Cursors.No : Owner.DockManager.DockingOptions.CursorFloatCanceled;
				return base.Cursor;
			}
		}
	}
	public class NestedDockZone : DockZone {
		DockingPointerMap map;
		public NestedDockZone(IDockZonesOwner zonesOwner, DockingStyle dock, int index)
			: base(zonesOwner, dock, index) {
			this.map = new DockingPointerMap(Owner);
		}
		protected override Rectangle GetPointerBounds(DockLayout source) {
			return RectangleToScreen(Map.GetPointerBounds(source, DockStyle, Index, FloatVertical));
		}
		protected override Rectangle GetRectangleBounds(int size, DockingStyle dock, int indent) {
			int index = Index;
			int offset = 0;
			if(Owner.HasChildren && !Owner.Tabbed) {
				if(!IsHead(dock))
					index--;
				offset = this.Owner[index].BorderBounds.Y;
			}
			Rectangle rect = RectangleToScreen(Map.GetRectangleBounds(index));
			if(dock == DockingStyle.Top)
				rect.Y += offset;
			DirectionRectangle dBounds = new DirectionRectangle(rect, dock);
			return dBounds.GetSideRectangle(size, indent);
		}
		protected internal override bool IsDockingAllowed(DockLayout source) {
			return source.Panel.Options.AllowDockFill;
		}
		protected internal override DockingStyle DockSide { get { return base.DockStyle; } }
		protected internal override DockingStyle DockStyle {
			get {
				if(Owner.Count > 0 && Owner.Tabbed) return base.DockStyle;
				return DockingStyle.Fill;
			}
		}
		bool IsHead(DockingStyle dock) { return LayoutRectangle.GetIsHead(dock); }
		protected DockingPointerMap Map { get { return map; } }
		protected class DockingPointerMap : LayoutInfo {
			LayoutInfo info;
			Size minSize;
			Rectangle bounds;
			public DockingPointerMap(LayoutInfo info)
				: this(info.MinSize, 1.0, info.Dock) {
				this.info = info;
				Reset();
			}
			public DockingPointerMap(Size minSize, double sizeFactor, DockingStyle dock)
				: base(dock) {
				this.info = null;
				this.minSize = minSize;
				this.bounds = LayoutConsts.InvalidRectangle;
				this.SizeFactor = sizeFactor;
			}
			public void Reset() {
				Clear();
				this.bounds = Info.Bounds;
				this.Dock = GetMapDock();
				this.SetFloatVerticalCore(Info.FloatVertical);
				this.DockVertical = Info.DockVertical;
				this.SetActiveChildCore(Info.ActiveChild);
				for(int i = 0; i < Info.Count; i++) {
					DockingPointerMap child = new DockingPointerMap(Info[i].MinSize, Info[i].SizeFactor, Info[i].Dock);
					InnerList.Add(child);
					SetParent(child, true);
				}
				SetTabbedCore(Info.Tabbed);
			}
			public Rectangle GetPointerBounds(DockLayout source, DockingStyle dock, int index, bool floatVertical) {
				DockingPointerMap child = new DockingPointerMap(source.MinSize, source.SizeFactor, source.Dock);
				SetFloatVerticalCore(floatVertical);
				child.DockTo(this, dock, index);
				Rectangle result = ToClientBounds(child.Bounds);
				Reset();
				return result;
			}
			public Rectangle GetRectangleBounds(int index) {
				Rectangle result = (index > Count - 1 ? Bounds : this[index].Bounds);
				return ToClientBounds(result);
			}
			Rectangle ToClientBounds(Rectangle rArgs) {
				return new Rectangle(new Point(rArgs.Left - Bounds.Left, rArgs.Top - Bounds.Top), rArgs.Size);
			}
			DockingStyle GetMapDock() {
				if(Info.Dock != DockingStyle.Fill) return Info.Dock;
				return (Info.IsHorizontal ? DockingStyle.Left : DockingStyle.Top);
			}
			protected override LayoutInfo CreateLayout(DockingStyle dock) { return new DockingPointerMap(Info.MinSize, 1.0, DockingStyle.Fill); }
			protected override void UpdateAutoHideInfo() { }
			public override Size MinSize { get { return minSize; } }
			public override Rectangle Bounds { get { return (bounds == LayoutConsts.InvalidRectangle ? base.Bounds : bounds); } }
			protected override Size DefaultSize { get { return DockConsts.DefaultDockPanelSize; } }
			protected LayoutInfo Info { get { return info; } }
		}
	}
	public class BaseTabDockZone : DockZone, ILayoutInfoInitializer {
		public BaseTabDockZone(IDockZonesOwner zonesOwner, DockingStyle dock, int index) : base(zonesOwner, dock, index) { }
		public override void Dock(DockLayout source) {
			if(!IsDockingAllowed(source)) return;
			DockCore(source);
		}
		protected virtual void DockCore(DockLayout source) {
			source.Initializer = this;
			try {
				if(!IsDockingAllowed(source)) return;
				LayoutInfo dest = ZonesOwner as LayoutInfo;
				if(dest == null) return;
				source.DockAsTab(dest, Index);
				FireChanged();
			}
			finally {
				source.Initializer = null;
			}
		}
		protected internal override bool TargetTabbed { get { return true; } }
		void ILayoutInfoInitializer.InitializeBeforeAssignContent(LayoutInfo info) {
		}
		void ILayoutInfoInitializer.InitializeAfterAssignContent(LayoutInfo info) {
			info.SetTabbedCore(true);
		}
	}
	public class TabCaptionDockZone : BaseTabDockZone {
		public TabCaptionDockZone(IDockZonesOwner zonesOwner, DockingStyle dock) : base(zonesOwner, dock, 0) { }
		protected override void DrawPointerCore(DockLayout source) {
			Owner.DrawTabDockPointer(source);
		}
		protected override void DockCore(DockLayout source) {
			if(!Owner.HasChildren)
				Owner.CanDockSourceChildren = true;
			try {
				base.DockCore(source);
			}
			finally {
				Owner.CanDockSourceChildren = false;
			}
		}
		public override Rectangle Bounds { get { return RectangleToScreen(Owner.CaptionBounds); } }
	}
	public abstract class TabVisualDockZone : BaseTabDockZone {
		DockLayout nativeActiveChild;
		int nativeActiveChildIndex;
		int nativeCount;
		Rectangle[] nativeTabsBounds;
		Rectangle headerRectCore;
		public TabVisualDockZone(IDockZonesOwner zonesOwner, DockingStyle dock)
			: base(zonesOwner, dock, LayoutConsts.InvalidIndex) {
			this.nativeActiveChild = Owner.ActiveChild;
			this.nativeActiveChildIndex = Owner.ActiveChildIndex;
			this.nativeCount = Owner.Count;
			this.nativeTabsBounds = Owner.TabsBounds;
			this.headerRectCore = Rectangle.Empty;
		}
		protected internal Rectangle HeaderRect {
			get { return headerRectCore; }
			set { headerRectCore = value; }
		}
		protected override void DrawPointerCore(DockLayout source) { }
		public override void Dock(DockLayout source) {
			if(source.Parent == Owner) {
				source.Index = Index;
				FireChanged();
			}
			else
				base.Dock(source);
		}
		protected void CheckDecreaseIndex(DockLayout source) {
			if(source.Parent == Owner && IndexCore == NativeCount)
				IndexCore--;
		}
		protected internal override void MouseOver(DockLayout source, Point pt) {
			int newIndex = GetZoneIndex(pt);
			if(newIndex == IndexCore) return;
			IndexCore = newIndex;
			CheckDecreaseIndex(source);
			if(IndexCore < 0 || IndexCore > Owner.Count ) return;
			if(IsDockingAllowed(source) && (CanEmulateDocking || !source.Float)) {
				DockEmulator.EmulateDocking(source, IndexCore);
			}
		}
		protected internal override void MouseLeave(DockLayout source) {
			if(IsDockingAllowed(source) && (CanEmulateDocking || !source.Float)) {
				source.DockManager.canImmediateRepaint = false;
				try {
					DockEmulator.Undock(source, NativeActiveChild, NativeActiveChildIndex, NativeTabsBounds);
				}
				finally {
					source.DockManager.canImmediateRepaint = true;
				}
			}
		}
		int GetZoneIndex(Point pt) {
			if(!Contains(pt)) return LayoutConsts.InvalidIndex;
			return GetZoneIndexCore(pt);
		}
		protected internal override void BeginDock() { DockEmulator.BeginUpdate(); }
		protected internal override void EndDock() { DockEmulator.EndUpdate(); }
		protected abstract int GetZoneIndexCore(Point pt);
		protected DockLayout NativeActiveChild { get { return nativeActiveChild; } }
		protected int NativeActiveChildIndex { get { return nativeActiveChildIndex; } }
		protected int NativeCount { get { return nativeCount; } }
		protected Rectangle[] NativeTabsBounds { get { return nativeTabsBounds; } }
		protected internal IDockEmulator DockEmulator { get { return Owner; } }
		protected internal bool CanEmulateDocking {
			get {
				return Owner != null && Owner.DockManager != null &&
					Owner.DockManager.DockingOptions.DockPanelInTabContainerTabRegion != DockPanelInTabContainerTabRegion.HighlightDockPosition;
			}
		}
	}
	public class TabVisualCaptionDockZone : TabVisualDockZone {
		public TabVisualCaptionDockZone(IDockZonesOwner zonesOwner, DockingStyle dock) : base(zonesOwner, dock) { }
		protected override int GetZoneIndexCore(Point pt) { return 0; }
		protected void UpdateHotTabBounds() {
			if(NativeTabsBounds == null || NativeTabsBounds.Length == 0)
				HeaderRect = new Rectangle(Point.Empty, DefaultSize);
			else
				HeaderRect = NativeTabsBounds[0];
		}
		static Size DefaultSize = new Size(50, 50);
		public override Rectangle Bounds { get { return RectangleToScreen(Owner.CaptionBounds); } }
		protected internal override void MouseOver(DockLayout source, Point pt) { UpdateHotTabBounds(); IndexCore = 0; }
		protected internal override void MouseLeave(DockLayout source) { UpdateHotTabBounds(); }
		protected override void DockCore(DockLayout source) {
			if(!Owner.HasChildren)
				Owner.CanDockSourceChildren = true;
			try {
				base.DockCore(source);
			}
			finally {
				Owner.CanDockSourceChildren = false;
			}
		}
	}
	public class TabVisualTabPanelDockZone : TabVisualDockZone {
		Rectangle[] screenTabsBounds;
		public TabVisualTabPanelDockZone(IDockZonesOwner zonesOwner, DockingStyle dock)
			: base(zonesOwner, dock) {
			this.screenTabsBounds = new Rectangle[NativeTabsBounds.Length];
			for(int i = 0; i < NativeTabsBounds.Length; i++)
				this.screenTabsBounds[i] = RectangleToScreen(NativeTabsBounds[i]);
		}
		protected override int GetZoneIndexCore(Point pt) {
			bool isHorizontal = !LayoutRectangle.GetIsHorizontal(DockLayoutUtils.ConvertToDockingStyle(Owner.TabsPosition));
			int curPos = (isHorizontal ? pt.X : pt.Y);
			for(int i = 0; i < ScreenTabsBounds.Length; i++) {
				DirectionRectangle dRect = new DirectionRectangle(ScreenTabsBounds[i], isHorizontal);
				if(curPos < dRect.Right) {
					HeaderRect = NativeTabsBounds[i];
					return i;
				}
			}
			if(NativeTabsBounds != null && NativeTabsBounds.Length != 0) {
				Rectangle rect = NativeTabsBounds[NativeCount - 1];
				rect.Location = new Point(NativeTabsBounds[NativeCount - 1].Right, NativeTabsBounds[NativeCount - 1].Bottom);
				HeaderRect = rect;
			}
			return NativeCount;
		}
		protected Rectangle[] ScreenTabsBounds { get { return screenTabsBounds; } }
		public override Rectangle Bounds { get { return RectangleToScreen(Owner.TabPanelBounds); } }
	}
	public class RestoreTabbedLayoutInfoInitializer : ILayoutInfoInitializer {
		public bool Tabbed { get; private set; }
		public RestoreTabbedLayoutInfoInitializer(bool tabbed) {
			Tabbed = tabbed;
		}
		public void InitializeBeforeAssignContent(LayoutInfo info) { }
		public void InitializeAfterAssignContent(LayoutInfo info) { info.SetTabbedCore(Tabbed); }
	}
	public class SizingAdorer : Docking2010.Customization.Adorner {
		Docking2010.Customization.AdornerElementInfo AdornerInfo;
		public SizingAdorer(Control adornedControl)
			: base(adornedControl) {
			var splitArgs = new Docking2010.Customization.SplitAdornerInfoArgs();
			var splitPainter = new Docking2010.Customization.SplitAdornerPainter();
			AdornerInfo = new Docking2010.Customization.AdornerElementInfo(splitPainter, splitArgs);
			AdornerPadding = Size.Empty;
		}
		public void StartSizing(Rectangle sizerBounds) {
			var splitArgs = AdornerInfo.InfoArgs as Docking2010.Customization.SplitAdornerInfoArgs;
			Show(AdornerInfo);
			splitArgs.Bounds = AdornedControl.RectangleToClient(sizerBounds);
			if(IsRightToLeftLayout)
				splitArgs.Bounds = new Rectangle((AdornedControl.ClientSize.Width - PaintOffset.X) - splitArgs.Bounds.Right, splitArgs.Bounds.Y, splitArgs.Bounds.Width, splitArgs.Bounds.Height);
			InvalidateTransparentLayer();
		}
		public void Sizing(bool horz, int change) {
			var splitArgs = AdornerInfo.InfoArgs as Docking2010.Customization.SplitAdornerInfoArgs;
			if(horz)
				AdornerLocation.Offset(change, 0);
			else
				AdornerLocation.Offset(0, change);
			TransparentLayer.Show(AdornerLocation);
		}
		public void EndSizing() {
			Reset(AdornerInfo);
			InvalidateTransparentLayer();
		}
	}
}
