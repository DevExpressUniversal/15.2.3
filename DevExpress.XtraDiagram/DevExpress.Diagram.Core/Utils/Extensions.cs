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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
namespace DevExpress.Diagram.Core {
	public static class OrientationExtensions {
		public static Point OffsetPoint(this Orientation orientation, Point point, Point offset) {
			return orientation.OffsetPoint(point, orientation.GetPoint(offset));
		}
		public static Rect OffsetRect(this Orientation orientation, Rect rect, double value) {
			return rect.SetLocation(orientation.OffsetPoint(rect.Location, value));
		}
		public static Point OffsetPoint(this Orientation orientation, Point point, double offset) {
			if(orientation == Orientation.Horizontal)
				return new Point(point.X + offset, point.Y);
			else
				return new Point(point.X, point.Y + offset);
		}
		public static Point SetPoint(this Orientation orientation, Point point, double value) {
			if(orientation == Orientation.Horizontal)
				return new Point(value, point.Y);
			else
				return new Point(point.X, value);
		}
		public static Point MakePoint(this Orientation orientation, double primary, double secondary) {
			if(orientation == Orientation.Horizontal)
				return new Point(primary, secondary);
			else
				return new Point(secondary, primary);
		}
		public static Size MakeSize(this Orientation orientation, double primary, double secondary) {
			if(orientation == Orientation.Horizontal)
				return new Size(primary, secondary);
			else
				return new Size(secondary, primary);
		}
		public static Thickness MakeThickness(this Orientation orientation, double near, double far) {
			if(orientation == Orientation.Horizontal)
				return new Thickness(near, 0, far, 0);
			else
				return new Thickness(0, near, 0, far);
		}
		public static double GetPoint(this Orientation orientation, Point point) {
			if(orientation == Orientation.Horizontal)
				return point.X;
			else
				return point.Y;
		}
		public static double GetSize(this Orientation orientation, Size size) {
			if(orientation == Orientation.Horizontal)
				return size.Width;
			else
				return size.Height;
		}
		public static double GetSize(this Orientation orientation, Rect rect) {
			return orientation.GetSize(rect.Size);
		}
		public static Rect SetSize(this Orientation orientation, Rect rect, double size) {
			if(orientation == Orientation.Horizontal)
				rect.Width = size;
			else
				rect.Height = size;
			return rect;
		}
		public static Size SetSize(this Orientation orientation, Size value, double size) {
			if(orientation == Orientation.Horizontal)
				value.Width = size;
			else
				value.Height = size;
			return value;
		}
		public static Rect SetLocation(this Orientation orientation, Rect rect, double location) {
			if(orientation == Orientation.Horizontal)
				rect.X = location;
			else
				rect.Y = location;
			return rect;
		}
		public static double GetLocation(this Orientation orientation, Rect rect) {
			return orientation.GetPoint(rect.Location);
		}
		public static Orientation Rotate(this Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return Orientation.Vertical;
			else
				return Orientation.Horizontal;
		}
		public static double GetRotationAngle(this Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return 0;
			else
				return -90;
		}
		public static bool IsCompatibleWithResizeMode(this Orientation orientation, ResizeMode mode) {
			if(orientation == Orientation.Horizontal) {
				return mode.IsHorizontal();
			} else {
				return mode.IsVertical();
			}
		}
		public static double GetResizeBoundary(this Orientation orientation, Rect rect, ResizeMode mode) {
			if(orientation == Orientation.Horizontal) {
				if(mode.IsLeft())
					return rect.Left;
				if(mode.IsRight())
					return rect.Right;
			} else {
				if(mode.IsBottom())
					return rect.Bottom;
				if(mode.IsTop())
					return rect.Top;
			}
			throw new InvalidOperationException();
		}
		public static double GetSide(this Orientation orientation, Rect rect, Side side) {
			if(orientation == Orientation.Horizontal) {
				if(side == Side.Near)
					return rect.Left;
				if(side == Side.Center)
					return rect.GetCenter().X;
				return rect.Right;
			} else {
				if(side == Side.Near)
					return rect.Top;
				if(side == Side.Center)
					return rect.GetCenter().Y;
				return rect.Bottom;
			}
		}
		public static Rect SetSide(this Orientation orientation, Rect rect, Side side, double value) {
			if(orientation == Orientation.Horizontal) {
				if(side == Side.Near)
					return rect.SetLeft(value);
				if(side == Side.Far)
					return rect.SetRight(value);
				throw new InvalidOperationException();
			} else {
				if(side == Side.Near)
					return rect.SetTop(value);
				if(side == Side.Far)
					return rect.SetBottom(value);
				throw new InvalidOperationException();
			}
		}
	}
	public static class ResizeModeExtensions {
		public static bool IsTop(this ResizeMode mode) {
			return (mode & ResizeMode.Top) > 0;
		}
		public static bool IsBottom(this ResizeMode mode) {
			return (mode & ResizeMode.Bottom) > 0;
		}
		public static bool IsLeft(this ResizeMode mode) {
			return (mode & ResizeMode.Left) > 0;
		}
		public static bool IsRight(this ResizeMode mode) {
			return (mode & ResizeMode.Right) > 0;
		}
		public static bool IsHorizontal(this ResizeMode mode) {
			return mode.IsLeft() || mode.IsRight();
		}
		public static bool IsVertical(this ResizeMode mode) {
			return mode.IsTop() || mode.IsBottom();
		}
		public static IEnumerable<Direction> GetDirections(this ResizeMode mode) {
			if(mode.IsLeft())
				yield return Direction.Left;
			if(mode.IsTop())
				yield return Direction.Up;
			if(mode.IsBottom())
				yield return Direction.Down;
			if(mode.IsRight())
				yield return Direction.Right;
		}
		public static bool HasDirection(this ResizeMode mode, Direction direction) {
			switch(direction) {
				case Direction.Left:
					return mode.IsLeft();
				case Direction.Up:
					return mode.IsTop();
				case Direction.Right:
					return mode.IsRight();
				case Direction.Down:
					return mode.IsBottom();
				default:
					throw new InvalidOperationException();
			}
		}
		public static DiagramCursor GetCursor(this ResizeMode mode) {
			switch(mode) {
			case ResizeMode.Left:
			case ResizeMode.Right:
				return DiagramCursor.SizeWE;
			case ResizeMode.Top:
			case ResizeMode.Bottom:
				return DiagramCursor.SizeNS;
			case ResizeMode.TopRight:
			case ResizeMode.BottomLeft:
				return DiagramCursor.SizeNESW;
			case ResizeMode.TopLeft:
			case ResizeMode.BottomRight:
				return DiagramCursor.SizeNWSE;
			default:
				throw new InvalidOperationException();
			}
		}
	}
	public static class SidesExtensions {
		public static bool HasTop(this Sides sides) {
			return (sides & Sides.Top) > 0;
		}
		public static bool HasLeft(this Sides sides) {
			return (sides & Sides.Left) > 0;
		}
		public static bool HasRight(this Sides sides) {
			return (sides & Sides.Right) > 0;
		}
		public static bool HasBottom(this Sides sides) {
			return (sides & Sides.Bottom) > 0;
		}
		public static bool IsBoth(this Sides sides, Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return sides.HasLeft() && sides.HasRight();
			else
				return sides.HasTop() && sides.HasBottom();
		}
		public static bool IsNear(this Sides sides, Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return sides.HasLeft() && !sides.HasRight();
			else
				return sides.HasTop() && !sides.HasBottom();
		}
		public static bool IsFar(this Sides sides, Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return !sides.HasLeft() && sides.HasRight();
			else
				return !sides.HasTop() && sides.HasBottom();
		}
	}
	public static class DirectionExtensions {
		public static Orientation GetOrientation(this Direction direction) {
			return (direction == Direction.Down || direction == Direction.Up) ? Orientation.Vertical : Orientation.Horizontal;
		}
		public static LogicalDirection GetLogicalDirection(this Direction direction) {
			return (direction == Direction.Right || direction == Direction.Down) ? LogicalDirection.Forward: LogicalDirection.Backward;
		}
		public static Point GetOffset(this Direction direction, Size gridSize) {
			switch(direction) {
				case Direction.Left:
					return new Point(-gridSize.Width, 0);
				case Direction.Up:
					return new Point(0, -gridSize.Height);
				case Direction.Right:
					return new Point(gridSize.Width, 0);
				case Direction.Down:
					return new Point(0, gridSize.Height);
			}
			throw new InvalidOperationException();
		}
		public static bool IsRightDirection(this Direction direction, Point offset) {
			switch(direction) {
				case Direction.Left:
					return MathHelper.IsLessThan(offset.X, 0);
				case Direction.Up:
					return MathHelper.IsLessThan(offset.Y, 0);
				case Direction.Right:
					return MathHelper.IsGreaterThan(offset.X, 0);
				case Direction.Down:
					return MathHelper.IsGreaterThan(offset.Y, 0);
			}
			throw new InvalidOperationException();
		}
		public static double GetSide(this Direction direction, Rect rect) {
			switch(direction) {
				case Direction.Left:
					return rect.Left;
				case Direction.Up:
					return rect.Top;
				case Direction.Right:
					return rect.Right;
				case Direction.Down:
					return rect.Bottom;
				default:
					throw new InvalidOperationException();
			}
		}
		public static Rect SetSide(this Direction direction, Rect rect, double value) {
			switch(direction) {
				case Direction.Left:
					return rect.SetLeft(value);
				case Direction.Up:
					return rect.SetTop(value);
				case Direction.Right:
					return rect.SetRight(value);
				case Direction.Down:
					return rect.SetBottom(value);
				default:
					throw new InvalidOperationException();
			}
		}
		public static Direction Inverse(this Direction direction) {
			switch(direction) {
				case Direction.Left:
					return Direction.Right;
				case Direction.Up:
					return Direction.Down;
				case Direction.Right:
					return Direction.Left;
				case Direction.Down:
					return Direction.Up;
				default:
					throw new InvalidOperationException();
			}
		}
		public static bool IsNear(this Direction direction) {
			switch(direction) {
				case Direction.Left:
				case Direction.Up:
					return true;
				case Direction.Right:
				case Direction.Down:
					return false;
				default:
					throw new InvalidOperationException();
			}
		}
		public static bool IsFar(this Direction direction) {
			return !direction.IsNear();
		}
	}
	public static class LogicalDirectionExtensions {
		public static double GetDirectedValue(this LogicalDirection direction, double value) {
			return direction == LogicalDirection.Forward ? value : -value;
		}
		public static double ChooseDirectionStartValue(this LogicalDirection direction, double near, double far) {
			return direction == LogicalDirection.Forward ? near : far;
		}
	}
	public static class StreamExtensions {
		public static string GetString(this MemoryStream memoryStream) {
			memoryStream.Seek(0, SeekOrigin.Begin);
			using(StreamReader reader = new StreamReader(memoryStream)) {
				return reader.ReadToEnd();
			}
		}
	}
	public static class GeometryKindExtensions {
		public static bool IsClosed(this GeometryKind kind) {
			return (kind & GeometryKind.Closed) > 0;
		}
		public static bool IsFilled(this GeometryKind kind) {
			return (kind & GeometryKind.Filled) > 0;
		}
	}
}
