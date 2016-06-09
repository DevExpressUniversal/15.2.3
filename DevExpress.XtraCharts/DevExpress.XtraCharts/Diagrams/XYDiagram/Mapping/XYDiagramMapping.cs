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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IInterimValueCalculator {
		int CalculateInterimValue(AxisIntervalLayout intervalLayout, double value, ITransformation transformation);
	}
	public class XYDiagramMappingContainer : List<XYDiagramMappingBase> {
		static AxisIntervalLayout FindIntervalLayout(List<AxisIntervalLayout> intervalsLayout, double value) {
			foreach (AxisIntervalLayout intervalLayout in intervalsLayout)
				if (intervalLayout.ValueWithinRange(value))
					return intervalLayout;
			return null;
		}
		static IntMinMaxValues CalculateMinMaxInterimValues(IList<AxisIntervalLayout> intervalsLayout, IInterimValueCalculator interimValueCalculator, bool clipBySideLimits, double value1, double value2, ITransformation transformation) {
			IntMinMaxValues interimValues = null;
			for (int i = 0; i < intervalsLayout.Count; i++) {
				AxisIntervalLayout intervalLayout = intervalsLayout[i];
				int interimValue1;
				int interimValue2;
				if (interimValueCalculator != null) {
					interimValue1 = interimValueCalculator.CalculateInterimValue(intervalLayout, value1, transformation);
					interimValue2 = interimValueCalculator.CalculateInterimValue(intervalLayout, value2, transformation);
				}
				else {
					AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(intervalLayout, transformation);
					interimValue1 = (int)mapping.GetInterimCoord(value1, false, true);
					interimValue2 = (int)mapping.GetInterimCoord(value2, false, true);
				}
				int minInterimVal = Math.Min(interimValue1, interimValue2);
				int maxInterimVal = Math.Max(interimValue1, interimValue2);
				if (maxInterimVal < intervalLayout.Start)
					continue;
				if (minInterimVal > intervalLayout.End)
					continue;
				if ((clipBySideLimits || i != 0) && minInterimVal < intervalLayout.Start)
					minInterimVal = intervalLayout.Start;
				if ((clipBySideLimits || i != intervalsLayout.Count - 1) && maxInterimVal > intervalLayout.End)
					maxInterimVal = intervalLayout.End;
				if (interimValues == null)
					interimValues = new IntMinMaxValues(minInterimVal, maxInterimVal);
				else {
					interimValues.MinValue = Math.Min(interimValues.MinValue, minInterimVal);
					interimValues.MaxValue = Math.Max(interimValues.MaxValue, maxInterimVal);
				}
			}
			return interimValues;
		}
		readonly XYDiagram2D diagram;
		readonly Axis2D axisX;
		readonly Axis2D axisY;
		readonly AxisIntervalsLayoutRepository intervalsLayoutRepository;
		readonly List<AxisIntervalLayout> intervalsLayoutX;
		readonly List<AxisIntervalLayout> intervalsLayoutY;
		readonly Rectangle mappingBounds;
		readonly MappingTransform transform;
		public bool Rotated { get { return diagram.ActualRotated; } }
		public Axis2D AxisX { get { return axisX; } }
		public Axis2D AxisY { get { return axisY; } }
		public AxisIntervalsLayoutRepository IntervalsLayoutRepository { get { return intervalsLayoutRepository; } }
		public IList<AxisIntervalLayout> IntervalsLayoutX { get { return intervalsLayoutX; } }
		public IList<AxisIntervalLayout> IntervalsLayoutY { get { return intervalsLayoutY; } }
		public XYDiagramMappingBase MappingForScrolling {
			get {
				if (Count == 1)
					return this[0];
				else {
					ChartDebug.Fail("Only one mapping for scrolling expected.");
					return null;
				}
			}
		}
		public Rectangle MappingBounds { get { return mappingBounds; } }
		public MappingTransform Transform { get { return transform; } }
		public XYDiagramMappingContainer(AxisIntervalsLayoutRepository intervalsLayoutRepository, XYDiagram2D diagram, Axis2D axisX, Axis2D axisY) {
			this.diagram = diagram;
			this.axisX = axisX;
			this.axisY = axisY;
			this.intervalsLayoutRepository = intervalsLayoutRepository;
			intervalsLayoutX = intervalsLayoutRepository.GetIntervalsLayout(axisX);
			intervalsLayoutY = intervalsLayoutRepository.GetIntervalsLayout(axisY);
			foreach (AxisIntervalLayout layoutX in intervalsLayoutX)
				foreach (AxisIntervalLayout layoutY in intervalsLayoutY)
					Add(CreateDiagramMapping(layoutX, layoutY));
			mappingBounds = intervalsLayoutRepository.MappingBounds;
			transform = XYDiagramMappingHelper.CreateTransform(intervalsLayoutRepository.MappingBounds, diagram.ActualRotated, axisX.ActualReverse, axisY.ActualReverse);
		}
		public XYDiagramMappingBase CreateDiagramMapping(AxisIntervalLayout layoutX, AxisIntervalLayout layoutY) {
			return diagram.CreateDiagramMapping(this, layoutX, layoutY);
		}
		public AxisIntervalLayout GetIntervalLayoutX(double argument) {
			return FindIntervalLayout(intervalsLayoutX, argument);
		}
		public AxisIntervalLayout GetIntervalLayoutY(double value) {
			return FindIntervalLayout(intervalsLayoutY, value);
		}
		public XYDiagramMappingBase GetMapping(double argument, double value) {
			AxisIntervalLayout layoutX = GetIntervalLayoutX(argument);
			if (layoutX == null)
				return null;
			AxisIntervalLayout layoutY = GetIntervalLayoutY(value);
			if (layoutY == null)
				return null;
			return CreateDiagramMapping(layoutX, layoutY);
		}
		public IntMinMaxValues CalculateMinMaxInterimX(IInterimValueCalculator interimValueCalculator, bool clipBySideLimits, double argument1, double argument2) {
			return CalculateMinMaxInterimValues(intervalsLayoutX, interimValueCalculator, clipBySideLimits, argument1, argument2, axisX.ScaleTypeMap.Transformation);
		}
		public IntMinMaxValues CalculateMinMaxInterimY(IInterimValueCalculator interimValueCalculator, bool clipBySideLimits, double value1, double value2) {
			return CalculateMinMaxInterimValues(intervalsLayoutY, interimValueCalculator, clipBySideLimits, value1, value2, axisY.ScaleTypeMap.Transformation);
		}
	}
	public abstract class XYDiagramMappingBase : IXYDiagramMapping {
		readonly XYDiagramMappingContainer container;
		readonly AxisIntervalLayout layoutX;
		readonly AxisIntervalLayout layoutY;
		public abstract Rectangle Bounds { get; }
		public Rectangle InflatedBounds { get { return GraphicUtils.InflateRect(Bounds, EdgeGeometry.MaxCrosswiseStep, EdgeGeometry.MaxCrosswiseStep); } }
		public XYDiagramMappingContainer Container { get { return container; } }
		public AxisIntervalLayout LayoutX { get { return layoutX; } }
		public AxisIntervalLayout LayoutY { get { return layoutY; } }
		public bool Rotated { get { return container.Rotated; } }
		public Axis2D AxisX { get { return container.AxisX; } }
		public Axis2D AxisY { get { return container.AxisY; } }
		public bool XReverse { get { return container.AxisX.ActualReverse; } }
		public bool YReverse { get { return container.AxisY.ActualReverse; } }
		public Rectangle MappingBounds { get { return container.MappingBounds; } }
		public MappingTransform Transform { get { return container.Transform; } }
		public XYDiagramMappingBase(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY) {
			this.container = container;
			this.layoutX = layoutX;
			this.layoutY = layoutY;
		}
		protected abstract DiagramPoint GetScreenPoint(double argument, double value, bool round);
		public DiagramPoint GetScreenPoint(double argument, double value) {
			return GetScreenPoint(argument, value, true);
		}
		public DiagramPoint GetScreenPointNoRound(double argument, double value) {
			return GetScreenPoint(argument, value, false);
		}
		public DiagramPoint GetInterimPoint(double argument, double value, bool clip, bool round) {
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(layoutX, AxisX.ScaleTypeMap.Transformation);
			double x = mapping.GetInterimCoord(argument, clip, round);
			mapping = new AxisIntervalLayoutMapping(layoutY, AxisY.ScaleTypeMap.Transformation);
			double y = mapping.GetInterimCoord(value, clip, round);
			return new DiagramPoint(x, y);
		}
	}
	public class XYDiagramMapping : XYDiagramMappingBase {
		Rectangle? bounds;
		public override Rectangle Bounds {
			get {
				if (bounds == null) {
					DiagramPoint p1 = new DiagramPoint(LayoutX.Bounds.Start, LayoutY.Bounds.Start); ;
					DiagramPoint p2 = new DiagramPoint(LayoutX.Bounds.Start + LayoutX.Bounds.Length, LayoutY.Bounds.Start + LayoutY.Bounds.Length);
					p1 = Transform.Map(p1);
					p2 = Transform.Map(p2);
					p1 = MatrixTransform.Project(p1, MappingBounds);
					p2 = MatrixTransform.Project(p2, MappingBounds);
					bounds = GraphicUtils.MakeRectangle((Point)p1, (Point)p2);
				}
				return (Rectangle)bounds;
			}
		}
		public XYDiagramMapping(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY) : base(container, layoutX, layoutY) { }
		DiagramPoint GetDiagramPoint(double argument, double value, bool round) {
			DiagramPoint interimPoint = GetInterimPoint(argument, value, false, round);
			return Transform.Map(interimPoint);
		}
		protected override DiagramPoint GetScreenPoint(double argument, double value, bool round) {
			DiagramPoint diagramPoint = GetDiagramPoint(argument, value, round);
			return MatrixTransform.Project(diagramPoint, MappingBounds);
		}
	}
	public class SwiftPlotDiagramMapping : XYDiagramMappingBase {
		readonly int left;
		readonly int bottom;
		readonly double minX;
		readonly double minY;
		readonly double factorX;
		readonly double factorY;
		readonly Transformation valueTransformation;
		readonly Transformation argumentTransformation;
		public override Rectangle Bounds { get { return Container.IntervalsLayoutRepository.MappingBounds; } }
		public SwiftPlotDiagramMapping(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY)
			: base(container, layoutX, layoutY) {
			left = Bounds.Left;
			bottom = Bounds.Bottom;
			argumentTransformation = AxisX.ScaleTypeMap.Transformation;
			valueTransformation = AxisY.ScaleTypeMap.Transformation;
			minX = argumentTransformation.TransformForward(layoutX.Range.Min);
			minY = valueTransformation.TransformForward(layoutY.Range.Min);
			factorX = (double)layoutX.Bounds.Length / (argumentTransformation.TransformForward(layoutX.Range.Max) - minX);
			factorY = (double)layoutY.Bounds.Length / (valueTransformation.TransformForward(layoutY.Range.Max) - minY);
		}
		double GetInterimArgument(double argument) {
			return (argumentTransformation.TransformForward(argument) - minX) * factorX;
		}
		double GetInterimValue(double value) {
			return (valueTransformation.TransformForward(value) - minY) * factorY;
		}
		protected override DiagramPoint GetScreenPoint(double argument, double value, bool round) {
			double interimX = GetInterimArgument(argument); ;
			double interimY = GetInterimValue(value);
			if (round) {
				interimX = Math.Round(interimX);
				interimY = Math.Round(interimY);
			}
			return new DiagramPoint(left + interimX, bottom - interimY);
		}
		public Point GetRoundedScreenPoint(double argument, double value) {
			int interimX = (int)Math.Round(GetInterimArgument(argument));
			int interimY = (int)Math.Round(GetInterimValue(value));
			return new Point(left + interimX, bottom - interimY);
		}
	}
	public class XYDiagramWholeMapping {
		readonly bool rotated;
		readonly Axis2D axisX;
		readonly Axis2D axisY;
		readonly int axisXLength;
		readonly int axisYLength;
		readonly MappingTransform transform;
		readonly Rectangle mappingBounds;
		public Rectangle MappingBounds { get { return mappingBounds; } }
		public XYDiagramWholeMapping(Rectangle mappingBounds, bool rotated, Axis2D axisX, Axis2D axisY) {
			this.rotated = rotated;
			this.axisX = axisX;
			this.axisY = axisY;
			this.mappingBounds = mappingBounds;
			axisXLength = rotated ? mappingBounds.Height : mappingBounds.Width;
			axisYLength = rotated ? mappingBounds.Width : mappingBounds.Height;
			transform = XYDiagramMappingHelper.CreateTransform(mappingBounds, rotated, axisX.ActualReverse, axisY.ActualReverse);
		}
		DiagramPoint GetInterimPoint(double argument, double value, bool clip, bool round) {
			double x = AxisCoordCalculator.GetCoord(axisX.VisualRangeData, argument, axisXLength, clip);
			double y = AxisCoordCalculator.GetCoord(axisY.VisualRangeData, value, axisYLength, clip);
			if (round) {
				x = Math.Round(x);
				y = Math.Round(y);
			}
			return new DiagramPoint(x, y);
		}
		public DiagramPoint GetDiagramPoint(double argument, double value, bool clip, bool round) {
			DiagramPoint interimPoint = GetInterimPoint(argument, value, clip, round);
			return transform.Map(interimPoint);
		}
	}
}
