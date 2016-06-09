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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class BezierRangeStrip : RangeStrip, IBezierStrip {
		double lineTension;
		public BezierRangeStrip(double lineTension) {
			this.lineTension = lineTension;
		}
		void GetPointsForDrawingInternal(out LineStrip topPoints, out LineStrip bottomPoints) {
			topPoints = new LineStrip();
			bottomPoints = new LineStrip();
			topPoints.AddRange(((BezierStrip)TopStrip).GetPointsForDrawing(true, true));
			bottomPoints.AddRange(((BezierStrip)BottomStrip).GetPointsForDrawing(true, true));
			bottomPoints.Reverse();
		}
		protected override LineStrip CreateBorderStrip() {
			return new BezierStrip(lineTension);
		}
		public void GetPointsForDrawing(out LineStrip topPoints, out LineStrip bottomPoints) {
			GetPointsForDrawing(out topPoints, out bottomPoints, 0.5);
		}
		public void GetPointsForDrawing(out LineStrip topPoints, out LineStrip bottomPoints, double border) {
			GRealRect2D boundingRectangle = GetBoundingRectangle();
			if (boundingRectangle.Width < border || boundingRectangle.Height < border) {
				if (boundingRectangle.Width < border)
					boundingRectangle.Inflate(border, 0.0f);
				if (boundingRectangle.Height < border)
					boundingRectangle.Inflate(0.0f, border);
				topPoints = new LineStrip(4);
				topPoints.Add(new GRealPoint2D(boundingRectangle.Left, boundingRectangle.Top));
				topPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width / 3.0, boundingRectangle.Top));
				topPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width / 1.5, boundingRectangle.Top));
				topPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width, boundingRectangle.Top));
				bottomPoints = new LineStrip(4);
				bottomPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width, boundingRectangle.Top + boundingRectangle.Height));
				bottomPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width / 1.5, boundingRectangle.Top + boundingRectangle.Height));
				bottomPoints.Add(new GRealPoint2D(boundingRectangle.Left + boundingRectangle.Width / 3.0, boundingRectangle.Top + boundingRectangle.Height));
				bottomPoints.Add(new GRealPoint2D(boundingRectangle.Left, boundingRectangle.Top + boundingRectangle.Height));
			}
			else
				GetPointsForDrawingInternal(out topPoints, out bottomPoints);
		}
		public GRealRect2D GetBoundingRectangle() {
			if (Count == 0)
				return GRealRect2D.Empty;
			if (Count == 1) {
				GRealRect2D rect = GeometricUtils.CalcBounds(new GRealPoint2D[] { TopStrip[0], BottomStrip[0] });
				if (rect.Width < 0.5)
					rect.Inflate(0.5f, 0.0f);
				if (rect.Height < 0.5)
					rect.Inflate(0.0f, 0.5f);
				return rect;
			}
			LineStrip topPoints, bottomPoints;
			GetPointsForDrawingInternal(out topPoints, out bottomPoints);
			topPoints.AddRange(bottomPoints);
			return GeometricUtils.CalcBounds(topPoints);
		}
		public override RangeStrip CreateInstance() {
			return new BezierRangeStrip(lineTension);
		}
		public override void CompleteFilling() {
			EnsureStips();
			List<GRealPoint2D> vertices = new List<GRealPoint2D>();
			vertices.AddRange(TopStrip);
			vertices.AddRange(BottomStrip);
			base.CompleteFilling();
		}
		public MinMaxValues CalculateMinMaxValues() {
			LineStrip topStrip;
			LineStrip bottomStrip;
			GetPointsForRangeCorrection(out topStrip, out bottomStrip);
			double min = double.MaxValue;
			double max = double.MinValue;
			MinMaxValues stripMinMax = BezierStrip.CalculateMinMaxValues(topStrip);
			if (stripMinMax.Min < min)
				min = stripMinMax.Min;
			if (stripMinMax.Max > max)
				max = stripMinMax.Max;
			return new MinMaxValues(min, max);
		}
		public MinMaxValues CalculateMinMaxArguments() {
			LineStrip topStrip;
			LineStrip bottomStrip;
			GetPointsForRangeCorrection(out topStrip, out bottomStrip);
			double min = double.MaxValue;
			double max = double.MinValue;
			MinMaxValues stripMinMax = BezierStrip.CalculateMinMaxArguments(topStrip);
			if (stripMinMax.Min < min)
				min = stripMinMax.Min;
			if (stripMinMax.Max > max)
				max = stripMinMax.Max;
			stripMinMax = BezierStrip.CalculateMinMaxArguments(bottomStrip);
			if (stripMinMax.Min < min)
				min = stripMinMax.Min;
			if (stripMinMax.Max > max)
				max = stripMinMax.Max;
			return new MinMaxValues(min, max);
		}
		void GetPointsForRangeCorrection(out LineStrip topPoints, out LineStrip bottomPoints) {
			topPoints = new LineStrip();
			bottomPoints = new LineStrip();
			topPoints.AddRange(((BezierStrip)TopStrip).GetPointsForDrawingWithoutValidate(true, true));
			bottomPoints.AddRange(((BezierStrip)BottomStrip).GetPointsForDrawingWithoutValidate(true, true));
			bottomPoints.Reverse();
		}
	}
}
