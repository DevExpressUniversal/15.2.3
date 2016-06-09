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
namespace DevExpress.Charts.Native {
	public class SeriesGeometry {
		readonly StackedGeometry stackedGeometry;
		readonly ISeries series;
		IList<IGeometryStrip> geometry;
		public StackedGeometry StackedGeometry { get { return stackedGeometry; } }
		public ISeries Series { get { return series; } }
		public IList<IGeometryStrip> Geometry { get { return geometry; } }
		public SeriesGeometry(StackedGeometry stackedGeometry, ISeries series) {
			this.stackedGeometry = stackedGeometry;
			this.series = series;
		}
		LineStrip CreateConstantStrip(IGeometryStripCreator stripCreator, double constant, IList<double> arguments) {
			if (stripCreator == null)
				return null;
			RangeStrip rangeStrip = stripCreator.CreateStrip() as RangeStrip;
			if (rangeStrip == null)
				return null;
			LineStrip strip = rangeStrip.BottomStrip.CreateInstance();
			foreach (double argument in arguments)
				strip.Add(new GRealPoint2D(argument, constant));
			strip.CompleteFilling();
			return strip;
		}
		public void SetGeometry(IList<IGeometryStrip> geometry, IList<double> arguments) {
			this.geometry = geometry;
			LineStrip borderStrip = stackedGeometry.BorderStrip;
			if (borderStrip == null) {
				borderStrip = CreateConstantStrip(series.SeriesView as IGeometryStripCreator, 0.0, arguments);
				stackedGeometry.BorderStrip = borderStrip;
			}
			if (borderStrip != null && borderStrip.Count > 0) {
				foreach (RangeStrip strip in geometry)
					stackedGeometry.UpdateBottomSegment(strip);
				foreach (RangeStrip strip in geometry)
					stackedGeometry.ModifyBorderStrip(strip);
			}
		}
		public void SubtractStrips(IList<LineStrip> strips, double borderValue) {
			foreach (RangeStrip strip in geometry) {
				LineStrip topStrip = strip.TopStrip;
				double minX = topStrip[0].X;
				double maxX = topStrip[topStrip.Count - 1].X;
				for (int stripIndex = 0; stripIndex < strips.Count; stripIndex++) {
					LineStrip borderStrip = strips[stripIndex];
					if (borderStrip.IsEmpty)
						continue;
					int lastIndex = topStrip.Count - 1;
					int borderIndex = borderStrip.Count - 1;
					double minBorderX = borderStrip[0].X;
					double maxBorderX = borderStrip[borderIndex].X;
					double minCommonX = Math.Max(minX, minBorderX);
					double maxCommonX = Math.Min(maxX, maxBorderX);
					if (maxCommonX >= minCommonX) {
						int startIndex = -1, endIndex = -1;
						for (int i = 0; i <= lastIndex; i++) {
							double x = topStrip[i].X;
							if (x == minCommonX)
								startIndex = i;
							if (x == maxCommonX) {
								endIndex = i;
								break;
							}
						}
						if (startIndex >= 0 && endIndex >= startIndex) {
							LineStrip leftStripToInsert = null;
							LineStrip rightStripToInsert = null;
							GRealPoint2D endPoint = topStrip[endIndex];
							int previousIndex = endIndex - 1;
							if (previousIndex >= 0 && endPoint.Y != borderValue && endPoint.Y != 0.0) {
								GRealPoint2D previousPoint = topStrip[previousIndex];
								GRealPoint2D intermediatePoint = new GRealPoint2D(endPoint.X, borderValue);
								rightStripToInsert = topStrip.CreateInstance();
								rightStripToInsert.Add(previousPoint);
								rightStripToInsert.Add(previousPoint);
								rightStripToInsert.Add(intermediatePoint);
								rightStripToInsert.Add(intermediatePoint);
								rightStripToInsert.Add(endPoint);
								rightStripToInsert.Add(endPoint);
								rightStripToInsert.CompleteFilling();
							}
							GRealPoint2D startPoint = topStrip[startIndex];
							int nextIndex = startIndex + 1;
							if (nextIndex <= lastIndex && startPoint.Y != borderValue && startPoint.Y != 0.0) {
								GRealPoint2D nextPoint = topStrip[nextIndex];
								GRealPoint2D intermediatePoint = new GRealPoint2D(startPoint.X, borderValue);
								leftStripToInsert = topStrip.CreateInstance();
								leftStripToInsert.Add(startPoint);
								leftStripToInsert.Add(startPoint);
								leftStripToInsert.Add(intermediatePoint);
								leftStripToInsert.Add(intermediatePoint);
								leftStripToInsert.Add(nextPoint);
								leftStripToInsert.Add(nextPoint);
								leftStripToInsert.CompleteFilling();
							}
							if (rightStripToInsert != null && leftStripToInsert != null && previousIndex == startIndex) {
								LineStrip stripToInsert = topStrip.CreateInstance();
								stripToInsert.AddRange(leftStripToInsert.GetRange(0, 4));
								stripToInsert.AddRange(rightStripToInsert.GetRange(2, 4));
								stripToInsert.CompleteFilling();
								topStrip.Substiture(startIndex, endIndex, stripToInsert);
							} else {
								if (rightStripToInsert != null)
									topStrip.Substiture(previousIndex, endIndex, rightStripToInsert);
								if (leftStripToInsert != null)
									topStrip.Substiture(startIndex, nextIndex, leftStripToInsert);
							}
							if (minCommonX > minBorderX) {
								if (maxCommonX < maxBorderX) {
									startIndex = 0;
									endIndex = borderIndex;
									for (int i = 0; i < borderIndex; i++) {
										double x = borderStrip[i].X;
										if (x == minCommonX)
											startIndex = i;
										if (x == maxCommonX) {
											endIndex = i;
											break;
										}
									}
									strips.RemoveAt(stripIndex);
									strips.Insert(stripIndex, borderStrip.ExtractSubStrip(endIndex, borderIndex));
									strips.Insert(stripIndex, borderStrip.ExtractSubStrip(0, startIndex));
									stripIndex++;
								} else
									while (borderStrip[borderIndex].X > minCommonX) {
										borderStrip.RemoveAt(borderIndex);
										borderIndex--;
									}
							} else if (maxCommonX < maxBorderX)
								while (borderStrip[0].X < maxCommonX)
									borderStrip.RemoveAt(0);
							else
								borderStrip.Clear();
						}
					}
				}
			}
			for (int i = 0; i < strips.Count; )
				if (strips[i].IsEmpty)
					strips.RemoveAt(i);
				else
					i++;
		}
	}
	public class StackedGeometry {
		readonly double topValue;
		readonly List<SeriesGeometry> seriesGeometry = new List<SeriesGeometry>();
		LineStrip borderStrip;
		public double TopValue { get { return topValue; } }
		public LineStrip BorderStrip {
			get { return borderStrip; }
			set { borderStrip = value; }
		}
		public StackedGeometry(double topValue) {
			this.topValue = topValue;
		}
		public void Add(ISeries series) {
			seriesGeometry.Add(new SeriesGeometry(this, series));
		}
		public SeriesGeometry FindSeriesGeometry(ISeries series) {
			foreach (SeriesGeometry geometry in seriesGeometry)
				if (geometry.Series == series)
					return geometry;
			return null;
		}
		public void UpdateBottomSegment(RangeStrip strip) {
			LineStrip bottomSegment = borderStrip.CreateInstance();
			int count = strip.Count;
			if (count == 0)
				return;
			double minX = strip.TopStrip[0].X;
			double maxX = strip.TopStrip[count - 1].X;
			if (maxX != minX) {
				int startIndex = -1, endIndex = -1;
				for (int i = 0; i < borderStrip.Count; i++) {
					GRealPoint2D bottomPoint = borderStrip[i];
					if (startIndex == -1 && bottomPoint.X == minX)
						startIndex = i;
					if (bottomPoint.X > maxX)
						break;
					if (bottomPoint.X == maxX)
						endIndex = i;
				}
				if (endIndex - startIndex > 0)
					strip.BottomStrip = borderStrip.ExtractSubStrip(startIndex, endIndex);
			}
		}
		public void ModifyBorderStrip(RangeStrip strip) {
			LineStrip topStrip = strip.TopStrip;
			if (topStrip.Count < 2 || borderStrip.Count < 2)
				return;
			double minStripX = topStrip[0].X;
			double maxStripX = topStrip[topStrip.Count - 1].X;
			double minBorderX = borderStrip[0].X;
			double maxBorderX = borderStrip[borderStrip.Count - 1].X;
			LineStrip stripToInsert = topStrip.ExtractSubStrip(0, topStrip.Count - 1);
			if (minBorderX != minStripX)
				for (int i = 0; i < borderStrip.Count; i++) {
					GRealPoint2D borderPoint = borderStrip[i];
					if (borderPoint.X == minStripX) {
						stripToInsert.Extend(borderPoint, true);
						break;
					}
				}
			if (maxBorderX != maxStripX)
				for (int i = borderStrip.Count - 1; i >= 0; i--) {
					GRealPoint2D borderPoint = borderStrip[i];
					if (borderPoint.X == maxStripX) {
						stripToInsert.Extend(borderPoint, false);
						break;
					}
				}
			int startBorderIndex = -1, borderIndex;
			for (borderIndex = 0; borderIndex < borderStrip.Count; borderIndex++) {
				GRealPoint2D point = borderStrip[borderIndex];
				if (point.X < minStripX)
					continue;
				else if (startBorderIndex < 0)
					startBorderIndex = borderIndex;
				else if (point.X > maxStripX)
					break;
			}
			if (startBorderIndex >= 0)
				borderStrip.Substiture(startBorderIndex, borderIndex - 1, stripToInsert);
		}
	}
	public class GeometryCalculator {
		internal static IList<IGeometryStrip> CreateStrips(IRefinedSeries refinedSeries, GeometryStripCreator stripCreator) {
			IList<RefinedPoint> points = refinedSeries.GetDrawingPoints();
			List<IGeometryStrip> strips = new List<IGeometryStrip>(points.Count);
			if (stripCreator != null && points.Count > 0) {
				int dropCount = stripCreator.ShouldAddEmptyStrip ? 0 : 1;
				RefinedPoint firstPointInfo = stripCreator.Closed ? points[0] : null;
				IGeometryStrip currentStrip = stripCreator.CreateStrip();
				int firsIndex = stripCreator.FilteringPointsSupported ? refinedSeries.MinVisiblePointIndex : 0;
				int lastIndex = stripCreator.FilteringPointsSupported ? refinedSeries.MaxVisiblePointIndex : points.Count - 1;
				for (int i = firsIndex; i < lastIndex + 1; i++) {
					RefinedPoint pointInfo = points[i];
					if (pointInfo.IsEmpty) {
						if (currentStrip.Count > dropCount) {
							currentStrip.CompleteFilling();
							strips.Add(currentStrip);
						}
						currentStrip = stripCreator.CreateStrip();
					} else
						stripCreator.AddStripElement(currentStrip, pointInfo);
				}
				if (firstPointInfo != null && !firstPointInfo.IsEmpty)
					stripCreator.AddStripElement(currentStrip, firstPointInfo);
				if (currentStrip.Count > dropCount) {
					currentStrip.CompleteFilling();
					strips.Add(currentStrip);
				}
			}
			return strips;
		}
		readonly Dictionary<StackedInteractionContainer, SplineStackedAreaGeometryStripCreator> stackedSplineCreators = new Dictionary<StackedInteractionContainer, SplineStackedAreaGeometryStripCreator>();
		public GeometryCalculator() {
		}
		IList<IGeometryStrip> CreateStackedSplineStrips(RefinedSeries series, IGeometryHolder geometryHolder) {
			StackedInteractionContainer interactionContainer = (StackedInteractionContainer)series.InteractionContainer;
			SplineStackedAreaGeometryStripCreator stripCreator;
			if (!stackedSplineCreators.ContainsKey(interactionContainer)) {
				stripCreator = (SplineStackedAreaGeometryStripCreator)geometryHolder.CreateStripCreator();
				stripCreator.Calculate(interactionContainer);
				stackedSplineCreators.Add(interactionContainer, stripCreator);
			}
			else
				stripCreator = stackedSplineCreators[interactionContainer];
			return stripCreator.GetStrips(series);
		}
		public IList<IGeometryStrip> CreateStrips(IRefinedSeries refinedSeries) {
			RefinedSeries series = (RefinedSeries)refinedSeries;
			IGeometryHolder holder = series.SeriesView as IGeometryHolder;
			if (holder != null) {
				if (series.SeriesView is IStackedSplineView)
					return CreateStackedSplineStrips(series, holder);
				GeometryStripCreator stripCreator = holder.CreateStripCreator();
				return CreateStrips(refinedSeries, stripCreator);
			}
			return null;
		}
	}
	public abstract class GeometryStripCreator {
		protected internal abstract bool Closed { get; }
		protected internal abstract bool ShouldAddEmptyStrip { get; }
		protected internal virtual bool FilteringPointsSupported { get { return false; } }
		protected internal abstract IGeometryStrip CreateStrip();
		protected internal abstract void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint);
	}
	public class LineGeometryStripCreator : GeometryStripCreator {
		readonly bool closed;
		protected internal override bool Closed { get { return closed; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		protected internal override bool FilteringPointsSupported { get { return !closed; } }
		public LineGeometryStripCreator(bool closed) {
			this.closed = closed;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new LineStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			((LineStrip)strip).Add(new GRealPoint2D(point.Argument, point.Value));
		}
	}
	public class StackedLineGeometryStripCreator : GeometryStripCreator {
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return false; } }
		protected internal override IGeometryStrip CreateStrip() {
			return new LineStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IStackedPoint point = (IStackedPoint)refinedPoint;
			((LineStrip)strip).Add(new GRealPoint2D(point.Argument, point.MaxValue));
		}
	}
	public class SplineGeometryStripCreator : GeometryStripCreator {
		readonly double lineTension;
		ITransformation transformationX;
		ITransformation transformationY;
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		public SplineGeometryStripCreator(double lineTension, ITransformation transformationX, ITransformation transformationY) {
			this.lineTension = lineTension;
			this.transformationX = transformationX;
			this.transformationY = transformationY;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new BezierStrip(lineTension, transformationX, transformationY);
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			((BezierStrip)strip).Add(new GRealPoint2D(point.Argument, point.Value));
		}
	}
	public class StepLineGeometryStripCreator : GeometryStripCreator {
		readonly bool invertedStep;
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return false; } }
		protected internal override bool FilteringPointsSupported { get { return true; } }
		public StepLineGeometryStripCreator(bool invertedStep) {
			this.invertedStep = invertedStep;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new LineStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			((LineStrip)strip).AddStepToPoint(new GRealPoint2D(point.Argument, point.Value), invertedStep);
		}
	}
	public class RadarAreaGeometryStripCreator : GeometryStripCreator {
		readonly bool closed;
		protected internal override bool Closed { get { return closed; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		public RadarAreaGeometryStripCreator(bool closed) {
			this.closed = closed;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new RangeStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.Value);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, 0.0);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
	}
	public class AreaGeometryStripCreator : GeometryStripCreator {
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		protected internal override IGeometryStrip CreateStrip() {
			return new RangeStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.Value);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, 0.0);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
	}
	public class RangeAreaGeometryStripCreator : GeometryStripCreator {
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		protected internal override IGeometryStrip CreateStrip() {
			return new RangeStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IRangePoint point = (IRangePoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.Value2);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, point.Value1);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
	}
	public class StackedAreaGeometryStripCreator : GeometryStripCreator {
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		protected internal override IGeometryStrip CreateStrip() {
			return new RangeStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IStackedPoint point = (IStackedPoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.MaxValue);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, point.MinValue);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
	}
	public class SplineAreaGeometryStripCreator : GeometryStripCreator {
		readonly double lineTension;
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		public SplineAreaGeometryStripCreator(double lineTension) {
			this.lineTension = lineTension;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new BezierRangeStrip(lineTension);
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.Value);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, 0.0);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
	}
	public class SplineStackedAreaGeometryStripCreator : SplineAreaGeometryStripCreator {
		static bool IsSameVertical(GRealPoint2D point1, GRealPoint2D point2) {
			return point1.X == point2.X && point1.Y != point2.Y;
		}
		static bool ShouldSplitByVertical(LineStrip lineStrip, GRealPoint2D newPoint) {
			if (lineStrip.Count >= 2) {
				GRealPoint2D lastPoint = lineStrip[lineStrip.Count - 1];
				return IsSameVertical(lastPoint, newPoint) ^ IsSameVertical(lastPoint, lineStrip[lineStrip.Count - 2]);
			}
			return false;
		}
		readonly List<IList<IGeometryStrip>> stripsCache = new List<IList<IGeometryStrip>>();
		StackedInteractionContainer interactionContainer;
		public SplineStackedAreaGeometryStripCreator(double lineTension) : base(lineTension) {
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IStackedPoint point = (IStackedPoint)refinedPoint;
			GRealPoint2D topPoint = new GRealPoint2D(point.Argument, point.MaxValue);
			GRealPoint2D bottomPoint = new GRealPoint2D(point.Argument, point.MinValue);
			BezierRangeStrip bezierStrip = (BezierRangeStrip)strip;
			if (ShouldSplitByVertical(bezierStrip.TopStrip, topPoint) || ShouldSplitByVertical(bezierStrip.BottomStrip, bottomPoint))
				bezierStrip.Add(new StripRange(bezierStrip.TopStrip[bezierStrip.TopStrip.Count - 1], bezierStrip.BottomStrip[bezierStrip.BottomStrip.Count - 1]));
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
		internal void Calculate(StackedInteractionContainer interactionContainer) {
			this.interactionContainer = interactionContainer;
			List<double> arguments = new List<double>();
			for (int i = 0; i < interactionContainer.SortedInteractionsByArgument.Count; i++) {
				double argument = interactionContainer.SortedInteractionsByArgument[i].Argument;
				arguments.Add(argument);
			}
			StackedGeometry stackedGeometry = new StackedGeometry(interactionContainer.Max);
			for (int i = 0; i < interactionContainer.Series.Count; i++) {
				RefinedSeries series = interactionContainer.Series[i];
				SeriesGeometry geometry = new SeriesGeometry(stackedGeometry, series.Series);
				geometry.SetGeometry(GeometryCalculator.CreateStrips(series, this), arguments);
				stripsCache.Add(geometry.Geometry);
			}
		}
		internal IList<IGeometryStrip> GetStrips(RefinedSeries series) {
			int index = interactionContainer.GetSeriesIndex(series);
			return stripsCache[index];
		}
	}
	public class StepAreaGeometryStripCreator : GeometryStripCreator {
		readonly bool invertedStep;
		protected internal override bool Closed { get { return false; } }
		protected internal override bool ShouldAddEmptyStrip { get { return true; } }
		public StepAreaGeometryStripCreator(bool invertedStep) {
			this.invertedStep = invertedStep;
		}
		protected internal override IGeometryStrip CreateStrip() {
			return new RangeStrip();
		}
		protected internal override void AddStripElement(IGeometryStrip strip, RefinedPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			RangeStrip rangeStrip = ((RangeStrip)strip);
			rangeStrip.TopStrip.AddStepToPoint(new GRealPoint2D(point.Argument, point.Value), invertedStep);
			rangeStrip.BottomStrip.Add(new GRealPoint2D(point.Argument, 0));
		}
	}
}
