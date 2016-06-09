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

using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Gauges.Native {
	public class LinearScaleLayout : ScaleLayout {
		readonly Point anchorPoint;
		readonly Point scaleVector;
		public Point AnchorPoint { get { return anchorPoint; } }
		public Point ScaleVector { get { return scaleVector; } }
		public override Geometry Clip { get { return null; } }
		public override bool IsEmpty { get { return !(InitialBounds.Height > 0 && InitialBounds.Width > 0); } }
		public LinearScaleLayout(Rect bounds, Point anchorPoint, Point scaleVector)
			: base(bounds) {
			this.anchorPoint = anchorPoint;
			this.scaleVector = scaleVector;
		}
	}
	public class LinearScaleMapping : ScaleMapping {
		bool IsVerticalScale { get { return Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom; } }
		new public LinearScale Scale { get { return base.Scale as LinearScale; } }
		new public LinearScaleLayout Layout { get { return base.Layout as LinearScaleLayout; } }
		public LinearScaleMapping(LinearScale scale, Rect bounds)
			: base(scale, new LinearScaleLayoutCalculator(scale, bounds).Calculate()) {
		}
		Point GetPointOffset(double offset) {
			return IsVerticalScale ? new Point(offset, 0) : new Point(0, offset);
		}
		protected override double GetAngleByPoint(Point point) {
			return IsVerticalScale ? 0.0 : 90.0;
		}
		public override Point GetPointByPercent(double percent) {
			return GetPointByPercent(percent, 0);
		}
		public override Point GetPointByPercent(double percent, double offset) {
			Point pointOffset = GetPointOffset(offset);
			double x = Layout.AnchorPoint.X + percent * Layout.ScaleVector.X + pointOffset.X;
			double y = Layout.AnchorPoint.Y + percent * Layout.ScaleVector.Y + pointOffset.Y;
			return new Point(x, y);
		}
		public override double? GetValueByPoint(Point point) {
			if(Layout.IsEmpty)
				return null;
			double valueInPercent = IsVerticalScale ? (point.Y - Layout.AnchorPoint.Y) / Layout.ScaleVector.Y :
				(point.X - Layout.AnchorPoint.X) / Layout.ScaleVector.X;
			double result = Scale.StartValue + valueInPercent * Scale.ValuesRange;
			if(MathUtils.IsValueInRange(result, Scale.StartValue, Scale.EndValue))
				return result;
			return null;
		}
	}
	public class LinearScaleLayoutCalculator {
		readonly LinearScale scale;
		readonly Rect bounds;
		Point StartPoint {
			get {
				switch(scale.LayoutMode) {
					case LinearScaleLayoutMode.BottomToTop:
						return new Point(0.5 * (bounds.Left + bounds.Right), bounds.Bottom);
					case LinearScaleLayoutMode.TopToBottom:
						return new Point(0.5 * (bounds.Left + bounds.Right), bounds.Top);
					case LinearScaleLayoutMode.LeftToRight:
						return new Point(bounds.Left, 0.5 * (bounds.Top + bounds.Bottom));
					case LinearScaleLayoutMode.RightToLeft:
						return new Point(bounds.Right, 0.5 * (bounds.Top + bounds.Bottom));
					default:
						goto case LinearScaleLayoutMode.BottomToTop;
				}
			}
		}
		Point EndPoint {
			get {
				switch(scale.LayoutMode) {
					case LinearScaleLayoutMode.BottomToTop:
						return new Point(0.5 * (bounds.Left + bounds.Right), bounds.Top);
					case LinearScaleLayoutMode.TopToBottom:
						return new Point(0.5 * (bounds.Left + bounds.Right), bounds.Bottom);
					case LinearScaleLayoutMode.LeftToRight:
						return new Point(bounds.Right, 0.5 * (bounds.Top + bounds.Bottom));
					case LinearScaleLayoutMode.RightToLeft:
						return new Point(bounds.Left, 0.5 * (bounds.Top + bounds.Bottom));
					default:
						goto case LinearScaleLayoutMode.BottomToTop;
				}
			}
		}
		public LinearScaleLayoutCalculator(LinearScale scale, Rect bounds) {
			this.scale = scale;
			this.bounds = bounds;
		}
		public LinearScaleLayout Calculate() {
			Point scaleVector = new Point(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
			return new LinearScaleLayout(bounds, StartPoint, scaleVector);
		}
	}
}
