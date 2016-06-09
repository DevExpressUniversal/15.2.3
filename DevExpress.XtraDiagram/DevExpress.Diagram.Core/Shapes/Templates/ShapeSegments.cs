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

using DevExpress.Data.Filtering;
using DevExpress.Diagram.Core.Shapes.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core.Shapes {
	public class BaseShapePoint {
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator X { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Y { get; set; }
	}
	public abstract class Segment : BaseShapePoint {
		internal abstract ShapeSegment CreateShapeSegment(ShapeLayoutCalculator context);
	}
	public enum PointKind {
		Absolute,
		Relative
	}
	public class ShapePoint : BaseShapePoint {
		public PointKind Kind { get; set; }
		public ShapePoint() {
			Kind = PointKind.Relative;
		}
		internal Point GetPoint(ShapeLayoutCalculator context) {
			switch(Kind) {
				case PointKind.Absolute:
					return new Point(Convert.ToDouble(context.Evaluate(X, null)), Convert.ToDouble(context.Evaluate(Y, null)));
				case PointKind.Relative:
					return context.GetPoint(X, Y);
				default:
					throw new NotImplementedException();
			}
		}
	}
	public class Start : Segment {
		public GeometryKind Kind { get; set; }
		public bool IsSmoothJoin { get; set; }
		public float FillBrightness { get; set; }
		public Color? FillColor { get; set; }
		public Color? StrokeColor { get; set; }
		public double? StrokeThickness { get; set; }
		public bool IsNewShape { get; set; }
		public Start() {
			Kind = GeometryKind.ClosedFilled;
			IsNewShape = true;
		}
		internal override ShapeSegment CreateShapeSegment(ShapeLayoutCalculator context) {
			return new StartSegment(context.GetPoint(X, Y), Kind, IsSmoothJoin, IsNewShape, new StartSegmentStyle(FillBrightness, FillColor, StrokeColor, StrokeThickness));
		}
	}
	public class Line : Segment {
		internal override ShapeSegment CreateShapeSegment(ShapeLayoutCalculator context) {
			return new LineSegment(context.GetPoint(X, Y));
		}
	}
	public class Arc : Segment {
		public SweepDirection Direction { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Size { get; set; }
		internal override ShapeSegment CreateShapeSegment(ShapeLayoutCalculator context) {
			Size? size = null;
			if(!object.ReferenceEquals(Size, null))
				size = (Size)context.Evaluate(Size, new List<ICustomFunctionOperator> { new CreateSizeFunction() });
			return ArcSegment.Create(context.GetPoint(X, Y), size, Direction);
		}
	}
}
