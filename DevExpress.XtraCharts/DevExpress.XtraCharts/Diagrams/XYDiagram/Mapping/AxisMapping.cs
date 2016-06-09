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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class AxisMappingBase {
		protected readonly IAxisData axis;
		readonly AxisAlignment alignment;
		readonly Rectangle mappingBounds;
		protected readonly int length;
		readonly int normalLength;
		double? interimZero = null;
		public AxisAlignment Alignment { get { return alignment; } }
		public Rectangle MappingBounds { get { return mappingBounds; } }
		public int Lenght { get { return length; } }
		public double? InterimZero { get { return interimZero; } }
		protected AxisMappingBase(IAxisData axis, Rectangle mappingBounds, AxisAlignment alignment) {
			this.axis = axis;
			this.alignment = alignment;
			this.mappingBounds = mappingBounds;
			length = axis.IsVertical ? mappingBounds.Height : mappingBounds.Width;
			normalLength = axis.IsVertical ? mappingBounds.Width : mappingBounds.Height;
		}
		protected abstract DiagramPoint GetInterimPoint(double value, AxisAlignment alignment, bool isClampedCoord);
		protected internal DiagramPoint GetTransformPoint(DiagramPoint interimPoint) {
			if (axis.IsVertical) {
				double swap = interimPoint.Y;
				interimPoint.Y = interimPoint.X;
				interimPoint.X = swap;
				if (axis.Reverse)
					interimPoint.Y = interimPoint.Y * -1 + MappingBounds.Height;
			}
			else {
				if (axis.Reverse)
					interimPoint.X = interimPoint.X * -1 + MappingBounds.Width;
			}
			return interimPoint;
		}
		DiagramPoint GetInterimPoint(double value, AxisAlignment alignment, int offset, int normalOffset, bool isClampedCoord) {
			DiagramPoint interimPoint = GetInterimPoint(value, alignment, isClampedCoord);
			return new DiagramPoint(interimPoint.X + offset, interimPoint.Y + normalOffset, interimPoint.Z);
		}
		DiagramPoint GetDiagramPoint(double value, AxisAlignment alignment, int offset, int normalOffset, bool isClampedCoord) {
			DiagramPoint interimPoint = GetInterimPoint(value, alignment, offset, normalOffset, isClampedCoord);
			return GetTransformPoint(interimPoint);
		}
		protected DiagramPoint GetScreenPoint(double value, AxisAlignment alignment, int selfOffset, int normalOffset, bool isClampedCoord) {
			DiagramPoint diagramPoint = GetDiagramPoint(value, alignment, selfOffset, normalOffset, isClampedCoord);
			return MatrixTransform.Project(diagramPoint, MappingBounds);
		}
		public DiagramPoint GetScreenPoint(double value, int selfOffset, int normalOffset) {
			return GetScreenPoint(value, Alignment, selfOffset, normalOffset, true);
		}
		public DiagramPoint InterimPointToScreenPoint(DiagramPoint interimPoint) {
			DiagramPoint diagramPoint = GetTransformPoint(interimPoint);
			return MatrixTransform.Project(diagramPoint, MappingBounds);
		}
		protected double GetInterimNormal(AxisAlignment alignment) {
			switch (alignment) {
				case AxisAlignment.Near:
					return 0;
				case AxisAlignment.Far:
					return normalLength;
				case AxisAlignment.Zero:
					return interimZero ?? 0;
				default:
					ChartDebug.Fail("Unknown axis alignment.");
					return 0;
			}
		}
		public void InitializeInterimZero(double? interimZero) {
			this.interimZero = interimZero;
		}
	}
	public class AxisIntervalMapping : AxisMappingBase {
		readonly AxisIntervalLayout intervalLayout;
		public AxisIntervalLayout IntervalLayout { get { return intervalLayout; } }
		public IMinMaxValues Limits { get { return intervalLayout.Range; } }
		public AxisIntervalMapping(IAxisData axis, AxisIntervalLayout intervalLayout, Rectangle mappingBounds, AxisAlignment alignment)
			: base(axis, mappingBounds, alignment) {
			this.intervalLayout = intervalLayout;
		}
		protected override DiagramPoint GetInterimPoint(double value, AxisAlignment alignment, bool isClampedCoord) {
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(intervalLayout, axis.AxisScaleTypeMap.Transformation);
			double interimCoord = isClampedCoord ? mapping.GetClampedCoord(value) : mapping.GetCoord(value);
			double interimNormal = GetInterimNormal(alignment);
			return new DiagramPoint((int)Math.Round(interimCoord), (int)Math.Round(interimNormal));
		}
		double GetClampedCoord(double value) {
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(intervalLayout, axis.AxisScaleTypeMap.Transformation);
			return mapping.GetClampedCoord(value);
		}
		public double GetCoord(double value) {
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(intervalLayout, axis.AxisScaleTypeMap.Transformation);
			return mapping.GetCoord(value);
		}
		public DiagramPoint GetNearScreenPoint(double value, int selfOffset, int normalOffset) {
			return GetScreenPoint(value, AxisAlignment.Near, selfOffset, normalOffset, true);
		}
		public DiagramPoint GetFarScreenPoint(double value, int selfOffset, int normalOffset) {
			return GetScreenPoint(value, AxisAlignment.Far, selfOffset, normalOffset, true);
		}
		public DiagramPoint GetNotClampedScreenPoint(double value, int selfOffset, int normalOffset) {
			return GetScreenPoint(value, Alignment, selfOffset, normalOffset, false);
		}
	}
	public class AxisMapping : AxisMappingBase {
		public class InterimZeroCalculator {
			readonly List<AxisIntervalMapping> intervalMappings = new List<AxisIntervalMapping>();
			readonly IAxisData axis;
			readonly int length;
			double TotalMinLimit { get { return intervalMappings[0].IntervalLayout.Range.Min; } }
			double TotalMaxLimit { get { return intervalMappings[intervalMappings.Count - 1].IntervalLayout.Range.Max; } }
			double? GetCoord(double value) {
				AxisIntervalMapping intervalMapping = GetIntervalMapping(value);
				if (intervalMapping != null)
					return intervalMapping.GetCoord(value);
				else
					return null;
			}
			AxisIntervalMapping GetIntervalMapping(double value) {
				foreach (AxisIntervalMapping intervalMapping in intervalMappings)
					if (intervalMapping.IntervalLayout.ValueWithinRange(value))
						return intervalMapping;
				return null;
			}
			public InterimZeroCalculator(IAxisData axis, Rectangle mappingBounds, List<AxisIntervalLayout> intervalsLayout, AxisAlignment alignment) {
				length = axis.IsVertical ? mappingBounds.Height : mappingBounds.Width;
				this.axis = axis;
				if (intervalsLayout == null)
					intervalsLayout = AxisIntervalLayout.CreateIntervalsLayout(axis as IIntervalContainer, axis, length);
				foreach (AxisIntervalLayout intervalLayout in intervalsLayout)
					intervalMappings.Add(new AxisIntervalMapping(axis, intervalLayout, mappingBounds, alignment));
			}
			public double? CalculateInterimZero() {
				List<AxisIntervalMapping> intervalMappings = new List<AxisIntervalMapping>();
				double? interimZero;
				if (TotalMinLimit >= 0)
					interimZero = 0;
				else if (TotalMaxLimit <= 0)
					interimZero = length;
				else
					interimZero = GetCoord(0);
				if (interimZero != null && axis.Reverse)
					interimZero = length - interimZero;
				return interimZero;
			}
		}
		public AxisMapping(AxisIntervalsLayoutRepository intervalsLayoutRepository, Axis2D axis)
			: this(intervalsLayoutRepository, intervalsLayoutRepository.MappingBounds, axis, axis.Alignment) { }
		public AxisMapping(AxisIntervalsLayoutRepository intervalsLayoutRepository, Axis2D axis, AxisAlignment alignment)
			: this(intervalsLayoutRepository, intervalsLayoutRepository.MappingBounds, axis, alignment) { }
		public AxisMapping(Rectangle mappingBounds, Axis2D axis, AxisAlignment alignment) : this(null, mappingBounds, axis, alignment) { }
		AxisMapping(AxisIntervalsLayoutRepository intervalsLayoutRepository, Rectangle mappingBounds, Axis2D axis, AxisAlignment alignment)
			: base(axis, mappingBounds, alignment) { }
		protected override DiagramPoint GetInterimPoint(double value, AxisAlignment alignment, bool isClampedCoord) {
			double interimCoord = AxisCoordCalculator.GetCoord(axis.VisualRange, value, Lenght, isClampedCoord);
			double interimNormal = GetInterimNormal(alignment);
			return new DiagramPoint((int)Math.Round(interimCoord), (int)Math.Round(interimNormal));
		}
	}
}
