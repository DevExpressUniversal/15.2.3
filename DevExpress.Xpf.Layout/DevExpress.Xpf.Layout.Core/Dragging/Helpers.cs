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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Layout.Core {
	public abstract class BaseResizeInfo {
		Point resizePointCore;
		Rect startRectCore;
		public Point ResizePoint { get { return resizePointCore; } }
		public Rect ItemRect { get { return startRectCore; } }
		protected BaseResizeInfo(Rect rect, Point pt) {
			this.startRectCore = rect;
			this.resizePointCore = pt;
		}
		public abstract bool ChangeLocationX { get; }
		public abstract bool ChangeLocationY { get; }
		public abstract bool Horizontal { get; }
		public abstract bool Vertical { get; }
		public abstract ResizeType HResizeType { get; }
		public abstract ResizeType VResizeType { get; }
		public override bool Equals(object obj) {
			if(!(obj is BaseResizeInfo)) return false;
			BaseResizeInfo info = obj as BaseResizeInfo;
			return info.HResizeType == HResizeType && info.VResizeType == VResizeType && info.ItemRect == ItemRect;
		}
		public override int GetHashCode() {
			return ItemRect.GetHashCode() ^ ResizePoint.GetHashCode();
		}
		public static bool operator ==(BaseResizeInfo left, BaseResizeInfo right) {
			if((object)left == (object)right) return true;
			return (object)left != null && left.Equals(right);
		}
		public static bool operator !=(BaseResizeInfo left, BaseResizeInfo right) {
			return !(left == right);
		}
	}
	public static class ResizeHelper {
		static Cursor[] SizingCursors = new Cursor[] { null , 
			Cursors.SizeWE, Cursors.SizeNS, Cursors.SizeNWSE, Cursors.SizeWE, 
			null, Cursors.SizeNESW, null, Cursors.SizeNS,
			Cursors.SizeNESW, null, null, Cursors.SizeNWSE
		};
		static SizingAction[] SizingActions = new SizingAction[] { SizingAction.None , 
			SizingAction.West, SizingAction.North, SizingAction.NorthWest, SizingAction.East, 
			SizingAction.None, SizingAction.NorthEast, SizingAction.None, SizingAction.South,
			SizingAction.SouthWest, SizingAction.None, SizingAction.None, SizingAction.SouthEast
		};
		public static BaseResizeInfo CalcResizeInfo(Rect rect, Point point) {
			return new ResizeInfo(rect, point);
		}
		public static SizingAction GetSizingAction(Rect rect, Point point, bool rightToLeft) {
			BaseResizeInfo ri = CalcResizeInfo(rect, point);
			int mode = 0;
			if(ri.HResizeType != ResizeType.None) mode |= (ri.HResizeType == ResizeType.Left) ?
				(rightToLeft ? 0x04 : 0x01) : (rightToLeft ? 0x01 : 0x04);
			if(ri.VResizeType != ResizeType.None) mode |= (ri.VResizeType == ResizeType.Top) ? 0x02 : 0x08;
			return SizingActions[mode];
		}
		public static Cursor GetResizeCursor(Rect rect, Point point) {
			return GetResizeCursor(rect, point, false);
		}
		public static Cursor GetResizeCursor(Rect rect, Point point, bool rightToLeft) {
			BaseResizeInfo ri = CalcResizeInfo(rect, point);
			int mode = 0;
			if(ri.HResizeType != ResizeType.None) mode |= (ri.HResizeType == ResizeType.Left) ?
				(rightToLeft ? 0x04 : 0x01) : (rightToLeft ? 0x01 : 0x04);
			if(ri.VResizeType != ResizeType.None) mode |= (ri.VResizeType == ResizeType.Top) ? 0x02 : 0x08;
			return SizingCursors[mode];
		}
		public static Rect CalcResizing(Rect startRect, Point startPoint, Point screenPoint, Size minSize) {
			BaseResizeInfo info = ResizeHelper.CalcResizeInfo(startRect, startPoint);
			double dx = info.Horizontal ? screenPoint.X - startPoint.X : 0;
			double dy = info.Vertical ? screenPoint.Y - startPoint.Y : 0;
			bool invertX = info.ChangeLocationX;
			bool invertY = info.ChangeLocationY;
			double w = Math.Max(minSize.Width, startRect.Width + (invertX ? -dx : dx));
			double h = Math.Max(minSize.Height, startRect.Height + (invertY ? -dy : dy));
			Point location = new Point(
					info.ChangeLocationX ? Math.Min(startRect.Right - minSize.Width, startRect.X + dx) : startRect.X,
					info.ChangeLocationY ? Math.Min(startRect.Bottom - minSize.Height, startRect.Y + dy) : startRect.Y
				);
			return new Rect(location, new Size(w, h));
		}
		public static Rect CalcResizing(Rect startRect, Point startPoint, Point screenPoint, Size minSize, Size maxSize) {
			BaseResizeInfo info = ResizeHelper.CalcResizeInfo(startRect, startPoint);
			double dx = info.Horizontal ? screenPoint.X - startPoint.X : 0;
			double dy = info.Vertical ? screenPoint.Y - startPoint.Y : 0;
			bool invertX = info.ChangeLocationX;
			bool invertY = info.ChangeLocationY;
			double w = Math.Max(minSize.Width, startRect.Width + (invertX ? -dx : dx));
			double h = Math.Max(minSize.Height, startRect.Height + (invertY ? -dy : dy));
			double w1 = w;
			double h1 = h;
			if(MathHelper.IsConstraintValid(maxSize.Width))
				w = Math.Min(w, maxSize.Width);
			if(MathHelper.IsConstraintValid(maxSize.Height))
				h = Math.Min(h, maxSize.Height);
			dx = -w + startRect.Width;
			dy = -h + startRect.Height;
			Point location = new Point(
					info.ChangeLocationX  ? Math.Min(startRect.Right - minSize.Width, startRect.X + dx) : startRect.X,
					info.ChangeLocationY  ? Math.Min(startRect.Bottom - minSize.Height, startRect.Y + dy) : startRect.Y
				);
			return new Rect(location, new Size(w, h));
		}
		class ResizeInfo : BaseResizeInfo {
			ResizeType hResize;
			ResizeType vResize;
			bool fHorizontal;
			bool fVertical;
			bool fChangeLocationX;
			bool fChangeLocationY;
			public ResizeInfo(Rect rect, Point pt)
				: base(rect, pt) {
				Calc(rect, pt);
			}
			double corner = 10.0;
			void Calc(Rect r, Point p) {
				this.hResize = ResizeType.None;
				this.vResize = ResizeType.None;
				bool inTopHalf = (p.Y < r.Top + r.Height * 0.5);
				bool inLeftHalf = (p.X < r.Left + r.Width * 0.5);
				bool inTopLeftCorner = (p.X < r.Left + corner) && (p.Y < r.Top + corner);
				bool inTopRightCorner = (p.X > r.Right - corner) && (p.Y < r.Top + corner);
				bool inBottomRightCorner = (p.X > r.Right - corner) && (p.Y > r.Bottom - corner);
				bool inBottomLeftCorner = (p.X < r.Left + corner) && (p.Y > r.Bottom - corner);
				bool inCorner = inTopLeftCorner || inTopRightCorner || inBottomRightCorner || inBottomLeftCorner;
				bool f1 = (p.Y - p.X) < (r.Top - r.Left);
				bool f2 = (p.Y + p.X) < (r.Top + r.Right);
				bool f3 = (p.Y - p.X) > (r.Bottom - r.Right);
				bool f4 = (p.Y + p.X) > (r.Bottom + r.Left);
				if(inCorner) {
					if(inTopLeftCorner) {
						vResize = ResizeType.Top;
						hResize = ResizeType.Left;
					}
					if(inTopRightCorner) {
						vResize = ResizeType.Top;
						hResize = ResizeType.Right;
					}
					if(inBottomRightCorner) {
						vResize = ResizeType.Bottom;
						hResize = ResizeType.Right;
					}
					if(inBottomLeftCorner) {
						vResize = ResizeType.Bottom;
						hResize = ResizeType.Left;
					}
				}
				else {
					if(f1 && f2 && inTopHalf) vResize = ResizeType.Top;
					if(f3 && f4 && !inTopHalf) vResize = ResizeType.Bottom;
					if(!f1 && !f4 && inLeftHalf) hResize = ResizeType.Left;
					if(!f2 && !f3 && !inLeftHalf) hResize = ResizeType.Right;
				}
				this.fHorizontal = (HResizeType == ResizeType.Left) || (HResizeType == ResizeType.Right);
				this.fVertical = (VResizeType == ResizeType.Top) || (VResizeType == ResizeType.Bottom);
				this.fChangeLocationX = (HResizeType == ResizeType.Left);
				this.fChangeLocationY = (VResizeType == ResizeType.Top);
			}
			public override ResizeType HResizeType { get { return hResize; } }
			public override ResizeType VResizeType { get { return vResize; } }
			public override bool Horizontal { get { return fHorizontal; } }
			public override bool Vertical { get { return fVertical; } }
			public override bool ChangeLocationX { get { return fChangeLocationX; } }
			public override bool ChangeLocationY { get { return fChangeLocationY; } }
		}
	}
	public abstract class BaseDropInfo {
		Point dropPointCore;
		Rect itemRectCore;
		protected BaseDropInfo(Rect rect, Point pt) {
			this.itemRectCore = rect;
			this.dropPointCore = pt;
		}
		public Point DropPoint { get { return dropPointCore; } }
		public Rect ItemRect { get { return itemRectCore; } }
		public abstract DropType Type { get; }
		public DockType DockType { get { return Type.ToDockType(); } }
		public MoveType MoveType { get { return Type.ToMoveType(); } }
		public abstract bool Horizontal { get; }
		public abstract bool Vertical { get; }
		public abstract Rect DropRect { get; }
		public virtual int InsertIndex { get { return -1; } }
		public override bool Equals(object obj) {
			if(!(obj is BaseDropInfo)) return false;
			BaseDropInfo info = obj as BaseDropInfo;
			return info.Type == Type && info.ItemRect == ItemRect;
		}
		public override int GetHashCode() {
			return ItemRect.GetHashCode() ^ Type.GetHashCode();   
		}
		public static bool operator ==(BaseDropInfo left, BaseDropInfo right) {
			if((object)left == (object)right) return true;
			return (object)left != null && left.Equals(right);
		}
		public static bool operator !=(BaseDropInfo left, BaseDropInfo right) {
			return !(left == right);
		}
	}
	public static class DockHelper {
		static double factor = 0.3;
		public static Rect GetDockRect(Rect rect, Size preferredSize, DockType type) {
			Rect dock = Rect.Empty;
			switch(type) {
				case DockType.Left: dock = GetLeftDock(rect, preferredSize.Width); break;
				case DockType.Right: dock = GetRightDock(rect, preferredSize.Width); break;
				case DockType.Top: dock = GetTopDock(rect, preferredSize.Height); break;
				case DockType.Bottom: dock = GetBottomDock(rect, preferredSize.Height); break;
				case DockType.Fill: dock = rect; break;
			}
			return dock;
		}
		public static Rect GetDockRect(Rect rect, DockType type) {
			return GetDockRect(rect, new Size(0, 0), type);
		}
		public static Rect GetLeftDock(Rect r, double preferredWidth) {
			return new Rect(r.Left, r.Top, GetWidth(r, preferredWidth), r.Height);
		}
		public static Rect GetRightDock(Rect r, double preferredWidth) {
			return new Rect(r.Right - GetWidth(r, preferredWidth), r.Top, GetWidth(r, preferredWidth), r.Height);
		}
		public static Rect GetTopDock(Rect r, double preferredHeight) {
			return new Rect(r.Left, r.Top, r.Width, GetHeight(r, preferredHeight));
		}
		public static Rect GetBottomDock(Rect r, double preferredHeight) {
			return new Rect(r.Left, r.Bottom - GetHeight(r, preferredHeight), r.Width, GetHeight(r, preferredHeight));
		}
		static double GetWidth(Rect r, double preferredWidth){
			return MathHelper.IsZero(preferredWidth) ? r.Width * factor : preferredWidth;
		}
		static double GetHeight(Rect r, double preferredHeight) {
			return MathHelper.IsZero(preferredHeight) ? r.Height * factor : preferredHeight;
		}
	}
	public static class DockTypeExtension {
		public static Orientation ToOrientation(this DockType type) {
			return (type == DockType.Left || type == DockType.Right) ? Orientation.Horizontal : Orientation.Vertical;
		}
		public static InsertType ToInsertType(this DockType type) {
			return (type == DockType.Left || type == DockType.Top) ? InsertType.Before : InsertType.After;
		}
		public static DockType ToDockType(this Dock type) {
			DockType result = DockType.None;
			switch(type) {
				case Dock.Left: result = DockType.Left; break;
				case Dock.Right: result = DockType.Right; break;
				case Dock.Top: result = DockType.Top; break;
				case Dock.Bottom: result = DockType.Bottom; break;
			}
			return result;
		}
		public static Dock ToDock(this DockType type) {
			switch(type) {
				case DockType.Right: return Dock.Right;
				case DockType.Top: return Dock.Top;
				case DockType.Bottom: return Dock.Bottom;
				default: return Dock.Left;
			}
		}
	}
	public static class MoveTypeExtension {
		public static InsertType ToInsertType(this MoveType type) {
			switch(type) {
				case MoveType.Bottom:
				case MoveType.Right:
					return InsertType.After;
				default:
					return InsertType.Before;
			}
		}
		public static Orientation ToOrientation(this MoveType type) {
			switch(type) {
				case MoveType.Left:
				case MoveType.Right:
					return Orientation.Horizontal;
				default:
					return Orientation.Vertical;
			}
		}
	}
	public static class DropTypeExtension {
		public static Orientation ToOrientation(this DropType type) {
			return (type == DropType.Left || type == DropType.Right) ? Orientation.Horizontal : Orientation.Vertical;
		}
	}
	public static class DropTypeHelper {
		public static DockType ToDockType(this DropType type) {
			DockType result = DockType.None;
			switch(type) {
				case DropType.Center: result = DockType.Fill; break;
				case DropType.Left: result = DockType.Left; break;
				case DropType.Right: result = DockType.Right; break;
				case DropType.Top: result = DockType.Top; break;
				case DropType.Bottom: result = DockType.Bottom; break;
			}
			return result;
		}
		public static MoveType ToMoveType(this DropType type) {
			switch(type) {
				case DropType.Left: return MoveType.Left;
				case DropType.Right: return MoveType.Right;
				case DropType.Top: return MoveType.Top;
				case DropType.Bottom: return MoveType.Bottom;
				case DropType.Center: return MoveType.InsideGroup;
			}
			return MoveType.None;
		}
		public static BaseDropInfo CalcCenterDropInfo(Rect rect, Point point, double centerZone) {
			return new DropInfo(rect, point, centerZone, true);
		}
		public static BaseDropInfo CalcSideDropInfo(Rect rect, Point point, double sideZone) {
			return new DropInfo(rect, point, sideZone, false);
		}
		public static BaseDropInfo CalcCenterDropInfo(Rect rect, Point point) {
			return new DropInfo(rect, point);
		}
		class DropInfo : BaseDropInfo {
			DropType dropTypeCore;
			bool fHorizontal = false;
			bool fVertical = false;
			Rect dropRectCore;
			public DropInfo(Rect rect, Point pt)
				: base(rect, pt) {
				if(ItemRect.Contains(DropPoint)) CalcWithCenterZone(ItemRect, DropPoint, 0.0);
				else CalcWithCenterZone(ItemRect, DropPoint);
			}
			public DropInfo(Rect rect, Point pt, double factor, bool useCenterZoneCalculation)
				: base(rect, pt) {
				if(useCenterZoneCalculation)
					CalcWithCenterZone(ItemRect, DropPoint, factor);
				else
					CalcWithRectangularSideZones(ItemRect, DropPoint, factor);
			}
			void CalcWithCenterZone(Rect r, Point p) {
				this.dropTypeCore = DropType.None;
				bool f1 = (p.Y - p.X) < (r.Top - r.Left);
				bool f2 = (p.Y + p.X) < (r.Top + r.Right);
				bool f3 = (p.Y - p.X) > (r.Bottom - r.Right);
				bool f4 = (p.Y + p.X) > (r.Bottom + r.Left);
				if(f1 && f2 && (p.Y < r.Top + r.Height * 0.5)) {
					dropTypeCore = DropType.Top;
					dropRectCore = new Rect(r.Left, r.Top, r.Width, r.Height * 0.5);
				}
				if(f3 && f4 && (p.Y > r.Top + r.Height * 0.5)) {
					dropTypeCore = DropType.Bottom;
					dropRectCore = new Rect(r.Left, r.Top + r.Height * 0.5, r.Width, r.Height * 0.5);
				}
				if(!f1 && !f4 && (p.X < r.Left + r.Width * 0.5)) {
					dropTypeCore = DropType.Left;
					dropRectCore = new Rect(r.Left, r.Top, r.Width * 0.5, r.Height);
				}
				if(!f2 && !f3 && (p.X > r.Left + r.Width * 0.5)) {
					dropTypeCore = DropType.Right;
					dropRectCore = new Rect(r.Left + r.Width * 0.5, r.Top, r.Width * 0.5, r.Height);
				}
				this.fHorizontal = (Type == DropType.Left) || (Type == DropType.Right);
				this.fVertical = (Type == DropType.Top) || (Type == DropType.Bottom);
			}
			void CalcWithCenterZone(Rect r, Point p, double centerFactor) {
				this.dropTypeCore = DropType.None;
				if(r.Contains(p)) {
					if(centerFactor != 0.0) {
						Rect center = new Rect(
								r.Left + r.Width * 0.5 * (1.0 - centerFactor), r.Top + r.Height * 0.5 * (1.0 - centerFactor),
								r.Width * centerFactor, r.Height * centerFactor
							);
						if(center.Contains(p)) {
							dropTypeCore = DropType.Center;
							dropRectCore = center;
							return;
						}
					}
					CalcWithCenterZone(r, p);
				}
			}
			void CalcWithRectangularSideZones(Rect r, Point p, double sideFactor) {
				this.dropTypeCore = DropType.None;
				if(r.Contains(p)) {
					double sideWidth = r.Width * 0.5 * sideFactor;
					Rect left = new Rect(r.Left, r.Top, sideWidth, r.Height);
					Rect right = new Rect(r.Right - sideWidth, r.Top, sideWidth, r.Height);
					fHorizontal = true;
					if(left.Contains(p)) {
						dropTypeCore = DropType.Left;
						dropRectCore = new Rect(r.Left, r.Top, r.Width * 0.5, r.Height);
						return;
					}
					if(right.Contains(p)) {
						dropTypeCore = DropType.Right;
						dropRectCore = new Rect(r.Left + r.Width * 0.5, r.Top, r.Width * 0.5, r.Height);
						return;
					}
					fHorizontal = false;
					fVertical = true;
					if(p.Y < r.Top + r.Height * 0.5) {
						dropTypeCore = DropType.Top;
						dropRectCore = new Rect(r.Left, r.Top, r.Width, r.Height * 0.5);
					}
					else {
						dropTypeCore = DropType.Bottom;
						dropRectCore = new Rect(r.Left, r.Top + r.Height * 0.5, r.Width, r.Height * 0.5);
					}
				}
			}
			public override DropType Type { get { return dropTypeCore; } }
			public override bool Horizontal { get { return fHorizontal; } }
			public override bool Vertical { get { return fVertical; } }
			public override Rect DropRect { get { return dropRectCore; } }
		}
	}
	public static class PlacementHelper {
		public static Rect Arrange(Size size, Rect targetRect, Alignment alignment, Point offset) {
			if(alignment == Alignment.Fill) return targetRect;
			double left = GetLeft(size.Width, targetRect, alignment) + offset.X;
			double top = GetTop(size.Height, targetRect, alignment) + offset.Y;
			return new Rect(new Point(left, top), size);
		}
		public static Rect Arrange(Size size, Rect targetRect, Alignment alignment) {
			if(alignment == Alignment.Fill) return targetRect;
			double left = GetLeft(size.Width, targetRect, alignment);
			double top = GetTop(size.Height, targetRect, alignment);
			return new Rect(new Point(left, top), size);
		}
		static double GetLeft(double width, Rect targetRect, Alignment alignment) {
			double left = targetRect.Left;
			switch(alignment) {
				case Alignment.TopCenter:
				case Alignment.MiddleCenter:
				case Alignment.BottomCenter:
					left = MathHelper.CenterRange(targetRect.Left, targetRect.Width, width);
					break;
				case Alignment.TopRight:
				case Alignment.MiddleRight:
				case Alignment.BottomRight:
					left = targetRect.Right - width;
					break;
			}
			return left;
		}
		static double GetTop(double height, Rect targetRect, Alignment alignment) {
			double top = targetRect.Top;
			switch(alignment) {
				case Alignment.MiddleLeft:
				case Alignment.MiddleCenter:
				case Alignment.MiddleRight:
					top = MathHelper.CenterRange(targetRect.Top, targetRect.Height, height);
					break;
				case Alignment.BottomLeft:
				case Alignment.BottomCenter:
				case Alignment.BottomRight:
					top = targetRect.Bottom - height;
					break;
			}
			return top;
		}
	}
	public static class MathHelper {
		const double Epsilon = 1E-10;
		public static bool IsEmpty(double value) {
			return double.IsNaN(value) || IsZero(value);
		}
		public static bool IsEmpty(Size value) {
			return IsEmpty(value.Width) || IsEmpty(value.Height);
		}
		public static bool IsEmpty(Point value) {
			return IsEmpty(value.X) || IsEmpty(value.Y);
		}
		public static bool IsConstraintValid(double value) {
			return !double.IsNaN(value) && !double.IsInfinity(value) && IsDimensionValid(value);
		}
		public static bool IsDimensionValid(double value) {
			return value > double.Epsilon;
		}
		public static bool IsZero(double value) {
			return IsZero(value, Epsilon);
		}
		public static double NormalizeConstraint(double value) {
			return IsConstraintValid(value) ? value : double.NaN;
		}
		public static Size MeasureSize(Size minSize, Size maxSize, Size measuredSize) {
			return new Size(
					MeasureDimension(minSize.Width, maxSize.Width, measuredSize.Width),
					MeasureDimension(minSize.Height, maxSize.Height, measuredSize.Height)
				);
		}
		public static Size ValidateSize(Size minSize, Size maxSize, Size measuredSize) {
			return new Size(
					ValidateDimension(minSize.Width, maxSize.Width, measuredSize.Width),
					ValidateDimension(minSize.Height, maxSize.Height, measuredSize.Height)
				);
		}
		public static Size MeasureMaxSize(Size[] maxSizes, bool fHorz) {
			double maxw = fHorz ? 0 : double.MaxValue;
			double maxh = fHorz ? double.MaxValue : 0;
			for(int i = 0; i < maxSizes.Length; i++) {
				if(fHorz) {
					maxw += CalcRealMaxConstraint(maxSizes[i].Width);
					maxh = Math.Min(maxh, CalcRealMaxConstraint(maxSizes[i].Height));
				}
				else {
					maxw = Math.Min(maxw, CalcRealMaxConstraint(maxSizes[i].Width));
					maxh += CalcRealMaxConstraint(maxSizes[i].Height);
				}
			}
			if(maxw == double.MaxValue) maxw = double.NaN;
			if(maxh == double.MaxValue) maxh = double.NaN;
			return new Size(maxw, maxh);
		}
		public static Size MeasureMinSize(Size[] minSizes, bool fHorz) {
			double minw = 0; double minh = 0;
			for(int i = 0; i < minSizes.Length; i++) {
				if(fHorz) {
					minw += CalcRealMinConstraint(minSizes[i].Width);
					minh = Math.Max(minh, CalcRealMinConstraint(minSizes[i].Height));
				}
				else {
					minw = Math.Max(minw, CalcRealMinConstraint(minSizes[i].Width));
					minh += CalcRealMinConstraint(minSizes[i].Height);
				}
			}
			return new Size(minw, minh);
		}
		public static Size MeasureMinSize(Size[] minSizes) {
			double maxw = 0; double maxh = 0;
			for(int i = 0; i < minSizes.Length; i++) {
				maxw = Math.Max(maxw, CalcRealMinConstraint(minSizes[i].Width));
				maxh = Math.Max(maxh, CalcRealMinConstraint(minSizes[i].Height));
			}
			return new Size(maxw, maxh);
		}
		public static Size MeasureMaxSize(Size[] maxSizes) {
			double minw = double.MaxValue; double minh = double.MaxValue;
			for(int i = 0; i < maxSizes.Length; i++) {
				minw = TopDimension(minw, CalcRealMaxConstraint(maxSizes[i].Width));
				minh = TopDimension(minh, CalcRealMaxConstraint(maxSizes[i].Height));
			}
			if(minw == double.MaxValue) minw = double.NaN;
			if(minh == double.MaxValue) minh = double.NaN;
			return new Size(minw, minh);
		}
		public static double ValidateDimension(double min, double max, double dim) {
			if(double.IsNaN(dim)) return dim;
			if(IsConstraintValid(max)) dim = Math.Min(max, dim);
			if(IsConstraintValid(min)) dim = Math.Max(min, dim);
			return dim;
		}
		public static double MeasureDimension(double min, double max, double dim) {
			if(IsConstraintValid(max)) dim = TopDimension(max, dim);
			if(IsConstraintValid(min)) dim = Math.Max(min, dim);
			return dim;
		}
		internal static double BottomDimension(double min, double dim) {
			if(double.IsNaN(dim)) return min;
			return (min > dim) ? min : dim;
		}
		internal static double TopDimension(double max, double dim) {
			if(double.IsNaN(dim)) return max;
			return (max < dim) ? max : dim;
		}
		public static double CalcRealDimension(double value) {
			double normalized = NormalizeConstraint(value);
			return double.IsNaN(normalized) ? 0.0 : normalized;
		}
		public static double CalcRealMinConstraint(double value) {
			double normalized = NormalizeConstraint(value);
			return double.IsNaN(normalized) ? 0.0 : normalized;
		}
		public static double CalcRealMaxConstraint(double value) {
			return NormalizeConstraint(value);
		}
		public static bool AreDoubleClose(double d1, double d2) {
			return AreDoubleClose(d1, d2, Epsilon);
		}
		public static bool AreDoubleClose(double d1, double d2, double precesion) {
			if(d1 == d2) return true;
			return IsZero(d1 - d2, precesion);
		}
		public static bool AreEqual(double d1, double d2) {
			return (d1 == d2) || (double.IsNaN(d1) && double.IsNaN(d2));
		}
		public static bool AreEqual(Size s1, Size s2) {
			return AreEqual(s1.Width, s2.Width) && AreEqual(s1.Height, s2.Height);
		}
		public static bool AreEqual(Thickness t1, Thickness t2) {
			return AreEqual(t1.Left, t2.Left) && AreEqual(t1.Top, t2.Top) &&
					AreEqual(t1.Right, t2.Right) && AreEqual(t1.Bottom, t2.Bottom);
		}
		public static bool IsZero(double value, double precesion) {
			return (value < precesion) && (value > -precesion);
		}
		public static int Round(double d) {
			return (d < 0.0) ? (int)(d - 0.5) : (int)(d + 0.5);
		}
		public static Point Round(Point p) {
			return new Point(Round(p.X), Round(p.Y));
		}
		public static Size Round(Size s) {
			return new Size(Round(s.Width), Round(s.Height));
		}
		public static double CenterRange(double targetStart, double targetRange, double range) {
			return targetStart + (targetRange - range) * 0.5;
		}
		public static Rect Round(Rect r) {
			double left = Round(r.Left);
			double right = Round(r.Right);
			double top = Round(r.Top);
			double bottom = Round(r.Bottom);
			return new Rect(left, top, right - left, bottom - top);
		}
		public static Rect Edge(Rect source, Rect target, bool horz) {
			if(horz) {
				if(target.Left > source.Left)
					return new Rect(source.Left, source.Top, target.Left - source.Left, source.Height);
				else
					return new Rect(target.Right, source.Top, source.Right - target.Right, source.Height);
			}
			else {
				if(target.Top > source.Top)
					return new Rect(source.Left, source.Top, source.Width, target.Top - source.Top);
				else
					return new Rect(source.Left, target.Bottom, source.Width, source.Bottom - target.Bottom);
			}
		}
	}
	public static class GeometryHelper {
		public static Rect CorrectBounds(Rect bounds, Rect constraints, double threshold) {
			Rect result = bounds;
			if(constraints.Width * constraints.Height != 0) {
				double left = bounds.Left;
				double top = bounds.Top;
				if(bounds.Left + threshold > constraints.Right) left -= (bounds.Left + threshold - constraints.Right);
				if(bounds.Top + threshold > constraints.Bottom) top -= (bounds.Top + threshold - constraints.Bottom);
				if(left + threshold < constraints.Left) left -= (left + threshold - constraints.Left);
				if(top + threshold < constraints.Top) top -= (top + threshold - constraints.Top);
				result = new Rect(left, top, bounds.Width, bounds.Height);
			}
			return result;
		}
		public static Rect CorrectBounds(Rect bounds, Rect constraints, Thickness threshold) {
			Rect result = bounds;
			if(constraints.Width * constraints.Height != 0) {
				double left = bounds.Left;
				double top = bounds.Top;
				if(bounds.Left + threshold.Right > constraints.Right) left -= (bounds.Left + threshold.Right - constraints.Right);
				if(bounds.Top + threshold.Bottom > constraints.Bottom) top -= (bounds.Top + threshold.Bottom - constraints.Bottom);
				if(left + threshold.Left < constraints.Left) left -= (left + threshold.Left - constraints.Left);
				if(top + threshold.Top < constraints.Top) top -= (top + threshold.Top - constraints.Top);
				result = new Rect(left, top, bounds.Width, bounds.Height);
			}
			return result;
		}
		public static Size CorrectSize(Size size, Size desired) {
			Size result = size;
			if(desired.Width * desired.Height != 0) {
				result = new Size(Math.Max(desired.Width, size.Width), Math.Max(desired.Height, size.Height));
			}
			return result;
		}
	}
	public static class SizingActionExtension {
		public static Cursor ToCursor(this SizingAction sa) {
			switch(sa) {
				case SizingAction.North:
				case SizingAction.South:
					return Cursors.SizeNS;
				case SizingAction.East:
				case SizingAction.West:
					return Cursors.SizeWE;
				case SizingAction.NorthEast:
				case SizingAction.SouthWest:
					return Cursors.SizeNESW;
				case SizingAction.NorthWest:
				case SizingAction.SouthEast:
					return Cursors.SizeNWSE;
				default:
					return Cursors.None;
			}
		}
	}
}
