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
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.XtraDiagram.Utils {
	public enum SpinDirection {
		Clockwise, CounterClockwise
	}
	public enum VectorDirection {
		None, LeftToRight, RightToLeft, DownToUp, UpToDown, NorthWest, NorthEast, SouthWest, SouthEast
	}
	public struct ArcDrawParams {
		readonly Rect rect;
		readonly float angle;
		readonly float sweepAngle;
		public ArcDrawParams(Rect rect, float angle, float sweepAngle) {
			this.rect = rect;
			this.angle = angle;
			this.sweepAngle = sweepAngle;
		}
		public Rect Rect { get { return rect; } }
		public float Angle { get { return angle; } }
		public float SweepAngle { get { return sweepAngle; } }
	}
	public class PlatformPointUtils {
		public static VectorDirection GetVectorDirection(Point start, Point end) {
			if(MathUtils.IsEquals(start.Y, end.Y) && MathUtils.IsLessThan(start.X, end.X))
				return VectorDirection.LeftToRight;
			if(MathUtils.IsEquals(start.Y, end.Y) && MathUtils.IsGreaterThan(start.X, end.X))
				return VectorDirection.RightToLeft;
			if(MathUtils.IsEquals(start.X, end.X) && MathUtils.IsLessThan(start.Y, end.Y))
				return VectorDirection.UpToDown;
			if(MathUtils.IsEquals(start.X, end.X) && MathUtils.IsGreaterThan(start.Y, end.Y))
				return VectorDirection.DownToUp;
			if(MathUtils.IsGreaterThan(start.X, end.X) && MathUtils.IsGreaterThan(start.Y, end.Y))
				return VectorDirection.NorthWest;
			if(MathUtils.IsLessThan(start.X, end.X) && MathUtils.IsGreaterThan(start.Y, end.Y))
				return VectorDirection.NorthEast;
			if(MathUtils.IsGreaterThan(start.X, end.X) && MathUtils.IsLessThan(start.Y, end.Y))
				return VectorDirection.SouthWest;
			return VectorDirection.SouthEast;
		}
		public static ArcDrawParams GetArcParams(Point start, Point end, Size size, SpinDirection spinDirection) {
			VectorDirection direction = GetVectorDirection(start, end);
			ArcParamsCalculationStrategyBase strategy = ArcParamsCalculationStrategyBase.Create(spinDirection, size);
			switch(direction) {
				case VectorDirection.LeftToRight:
					return strategy.GetLeftToRightArcParams(start, end);
				case VectorDirection.RightToLeft:
					return strategy.GetRightToLeftArcParams(start, end);
				case VectorDirection.DownToUp:
					return strategy.GetDownToUpArcParams(start, end);
				case VectorDirection.UpToDown:
					return strategy.GetUpToDownArcParams(start, end);
				case VectorDirection.NorthWest:
					return strategy.GetNorthWestArcParams(start, end);
				case VectorDirection.NorthEast:
					return strategy.GetNorthEastArcParams(start, end);
				case VectorDirection.SouthWest:
					return strategy.GetSouthWestArcParams(start, end);
				case VectorDirection.SouthEast:
					return strategy.GetSouthEastArcParams(start, end);
				default:
					throw new InvalidOperationException(string.Format("{0} direction is not supported", direction.ToString()));
			}
		}
		#region ArcParamsCalculationStrategy
		abstract class ArcParamsCalculationStrategyBase {
			Size itemSize;
			public ArcParamsCalculationStrategyBase(Size itemSize) {
				this.itemSize = itemSize;
			}
			public static ArcParamsCalculationStrategyBase Create(SpinDirection direction, Size itemSize) {
				if(direction == SpinDirection.Clockwise) {
					return new ClockwiseArcParamsCalculationStrategy(itemSize);
				}
				return new CounterClockwiseArcParamsCalculationStrategy(itemSize);
			}
			protected double ItemWidth { get { return ItemSize.Width; } }
			protected double ItemHeight { get { return ItemSize.Height; } }
			protected bool IsDiagonalArc(Point start, Point end) {
				double horzRange = GetHorzRange(start, end);
				double vertRange = GetVertRange(start, end);
				return MathUtils.IsNotEquals(horzRange, ItemWidth) && MathUtils.IsNotEquals(vertRange, ItemHeight);
			}
			protected double GetHorzRange(Point start, Point end) {
				return Math.Abs(start.X - end.X);
			}
			protected double GetVertRange(Point start, Point end) {
				return Math.Abs(start.Y - end.Y);
			}
			protected Rect GetSquareBounds(Point centerPoint, Point start) {
				double dx = Math.Abs(centerPoint.X - start.X);
				double dy = Math.Abs(centerPoint.Y - start.Y);
				double R = Math.Sqrt(dx * dx + dy * dy);
				return new Rect(centerPoint.X - R, centerPoint.Y - R, 2 * R, 2 * R);
			}
			protected float GetDiagonalAngle(Point centerPoint, Point start) {
				double dx = Math.Abs(centerPoint.X - start.X);
				double dy = Math.Abs(centerPoint.Y - start.Y);
				return (float)(Math.Atan(dy / dx) * 180 / Math.PI);
			}
			protected Size ItemSize { get { return itemSize; } }
			public abstract ArcDrawParams GetLeftToRightArcParams(Point start, Point end);
			public abstract ArcDrawParams GetRightToLeftArcParams(Point start, Point end);
			public abstract ArcDrawParams GetDownToUpArcParams(Point start, Point end);
			public abstract ArcDrawParams GetUpToDownArcParams(Point start, Point end);
			public abstract ArcDrawParams GetNorthWestArcParams(Point start, Point end);
			public abstract ArcDrawParams GetNorthEastArcParams(Point start, Point end);
			public abstract ArcDrawParams GetSouthWestArcParams(Point start, Point end);
			public abstract ArcDrawParams GetSouthEastArcParams(Point start, Point end);
		}
		class ClockwiseArcParamsCalculationStrategy : ArcParamsCalculationStrategyBase {
			public ClockwiseArcParamsCalculationStrategy(Size itemSize)
				: base(itemSize) {
			}
			public override ArcDrawParams GetLeftToRightArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X, start.Y - ItemHeight, end.X - start.X, ItemHeight * 2);
				return new ArcDrawParams(rect, 180, 180);
			}
			public override ArcDrawParams GetRightToLeftArcParams(Point start, Point end) {
				double height = ItemHeight - end.Y;
				if(MathUtils.IsEquals(height, 0) || height < 0 || IsDiagonalArc(start, end)) {
					return GetRightToLeftArcParamsCore(start, end);
				}
				return GetLowSteepnessHorzArcParams(end, height);
			}
			protected ArcDrawParams GetRightToLeftArcParamsCore(Point start, Point end) {
				Rect rect = new Rect(end.X, end.Y - ItemHeight, start.X - end.X, ItemHeight * 2);
				return new ArcDrawParams(rect, 0, 180);
			}
			protected ArcDrawParams GetLowSteepnessHorzArcParams(Point end, double height) {
				Rect rect = MathUtils.GetBoundingRect(end, ItemWidth, height, true);
				return new ArcDrawParams(rect, MathUtils.GetArcStartAngle(height, ItemWidth, true), MathUtils.GetArcSweepAngle(height, ItemWidth, true));
			}
			public override ArcDrawParams GetDownToUpArcParams(Point start, Point end) {
				Rect rect = new Rect(end.X - ItemWidth, end.Y, ItemWidth * 2, start.Y - end.Y);
				return new ArcDrawParams(rect, 90, 180);
			}
			public override ArcDrawParams GetUpToDownArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X - ItemWidth, start.Y, ItemWidth * 2, end.Y - start.Y);
				return new ArcDrawParams(rect, -90, 180);
			}
			public override ArcDrawParams GetNorthWestArcParams(Point start, Point end) {
				Rect rect = new Rect(end.X, end.Y - ItemHeight, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 90, 90);
			}
			public override ArcDrawParams GetNorthEastArcParams(Point start, Point end) {
				if(IsDiagonalArc(start, end)) {
					Point center = new Point(start.X + (end.X - start.X) / 2, end.Y + (start.Y - end.Y) / 2);
					return new ArcDrawParams(GetSquareBounds(center, start), -180 - GetDiagonalAngle(center, start), 180);
				}
				Rect rect = new Rect(start.X, end.Y, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 180, 90);
			}
			public override ArcDrawParams GetSouthWestArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X - 2 * ItemWidth, start.Y - ItemHeight, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 0, 90);
			}
			public override ArcDrawParams GetSouthEastArcParams(Point start, Point end) {
				if(IsDiagonalArc(start, end)) {
					Point centerPoint = new Point(start.X + (end.X - start.X) / 2, start.Y + (end.Y - start.Y) / 2);
					return new ArcDrawParams(GetSquareBounds(centerPoint, start), -180 + GetDiagonalAngle(centerPoint, start), 180);
				}
				Rect rect = new Rect(start.X - ItemWidth, start.Y, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, -90, 90);
			}
		}
		class CounterClockwiseArcParamsCalculationStrategy : ArcParamsCalculationStrategyBase {
			public CounterClockwiseArcParamsCalculationStrategy(Size itemSize)
				: base(itemSize) {
			}
			public override ArcDrawParams GetLeftToRightArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X, start.Y - ItemHeight, end.X - start.X, ItemHeight * 2);
				return new ArcDrawParams(rect, 180, -180);
			}
			public override ArcDrawParams GetRightToLeftArcParams(Point start, Point end) {
				double height = ItemHeight - end.Y;
				if(MathUtils.IsEquals(height, 0) || height < 0 || IsDiagonalArc(start, end)) {
					return GetRightToLeftArcParamsCore(start, end);
				}
				return GetLowSteepnessHorzArcParams(end, height);
			}
			protected ArcDrawParams GetLowSteepnessHorzArcParams(Point end, double height) {
				Rect rect = MathUtils.GetBoundingRect(end, ItemWidth, height, false);
				return new ArcDrawParams(rect, MathUtils.GetArcStartAngle(height, ItemWidth, false), MathUtils.GetArcSweepAngle(height, ItemWidth, false));
			}
			protected ArcDrawParams GetRightToLeftArcParamsCore(Point start, Point end) {
				Rect rect = new Rect(end.X, end.Y - ItemHeight, start.X - end.X, ItemHeight * 2);
				return new ArcDrawParams(rect, 0, -180);
			}
			public override ArcDrawParams GetDownToUpArcParams(Point start, Point end) {
				Rect rect = new Rect(end.X - ItemWidth, end.Y, ItemWidth * 2, start.Y - end.Y);
				return new ArcDrawParams(rect, 90, -180);
			}
			public override ArcDrawParams GetUpToDownArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X - ItemWidth, start.Y, ItemWidth * 2, end.Y - start.Y);
				return new ArcDrawParams(rect, -90, -180);
			}
			public override ArcDrawParams GetNorthWestArcParams(Point start, Point end) {
				if(IsDiagonalArc(start, end)) {
					Point center = new Point(start.X + (end.X - start.X) / 2, end.Y + (start.Y - end.Y) / 2);
					return new ArcDrawParams(GetSquareBounds(center, start), GetDiagonalAngle(center, start), -180);
				}
				Rect rect = new Rect(end.X - ItemWidth, end.Y, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 0, -90);
			}
			public override ArcDrawParams GetNorthEastArcParams(Point start, Point end) {
				Rect rect = new Rect(end.X - 2 * ItemWidth, end.Y - ItemHeight, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 90, -90);
			}
			public override ArcDrawParams GetSouthWestArcParams(Point start, Point end) {
				if(IsDiagonalArc(start, end)) {
					Point center = new Point(start.X + (end.X - start.X) / 2, end.Y + (start.Y - end.Y) / 2);
					return new ArcDrawParams(GetSquareBounds(center, start), - GetDiagonalAngle(center, start), -180);
				}
				Rect rect = new Rect(start.X - ItemWidth, start.Y, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, -90, -90);
			}
			public override ArcDrawParams GetSouthEastArcParams(Point start, Point end) {
				Rect rect = new Rect(start.X, start.Y - ItemHeight, ItemWidth * 2, ItemHeight * 2);
				return new ArcDrawParams(rect, 180, -90);
			}
		}
		#endregion
	}
}
