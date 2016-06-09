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
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Shapes.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core.Shapes {
	[ContentProperty("Segments")]
	public class ShapeTemplate {
		readonly List<Segment> segmentsCore;
		readonly List<ShapePoint> connectionPointsCore;
		readonly List<Parameter> parametersCore;
		public List<Segment> Segments { get { return segmentsCore; } }
		public List<ShapePoint> ConnectionPoints { get { return connectionPointsCore; } }
		public List<Parameter> Parameters { get { return parametersCore; } }
		public Size DefaultSize { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator EditorBounds { get; set; }
		public DiagramItemStyleId Style { get; set; }
		public bool IsQuick { get; set; }
		public bool UseBackgroundAsForeground { get; set; }
		UnitCollection rowsCore;
		public UnitCollection Rows {
			get { return rowsCore ?? UnitCollection.Default; }
			set { rowsCore = value; }
		}
		UnitCollection columnsCore;
		public UnitCollection Columns {
			get { return columnsCore ?? UnitCollection.Default; }
			set { columnsCore = value; }
		}
		public double[] DefaultParameters { get { return Parameters.Select(x => x.DefaultValue).ToArray(); } }
#if DEBUGTEST
		public static int InstanceCountForTests { get; private set; }
#endif
		public ShapeTemplate() {
#if DEBUGTEST
			InstanceCountForTests++;
#endif
			segmentsCore = new List<Segment>();
			connectionPointsCore = new List<ShapePoint>();
			parametersCore = new List<Parameter>();
		}
		ShapeLayoutCalculator CreateContext(Size size, double[] parameters) {
			double[] normalizedParameters = GetNormalizedParameters(size, parameters);
			return new ShapeLayoutCalculator(Rows, Columns, size, normalizedParameters);
		}
		double[] GetNormalizedParameters(Size size, double[] parameters) {
			if(parameters == null)
				return null;
			double[] normalized = new double[parameters.Length];
			for(int index = 0; index < parameters.Length; index++) {
				Parameter param = Parameters[index];
				normalized[index] = param.NormalizeParameter(size, parameters, parameters[index]);
			}
			return normalized;
		}
		public ShapeGeometry GetShape(Size size, double[] parameters) {
			List<ShapeSegment> shapeSegments = new List<ShapeSegment>();
			ShapeLayoutCalculator context = CreateContext(size, parameters);
			foreach(Segment segment in Segments) {
				shapeSegments.Add(segment.CreateShapeSegment(context));
			}
			return new ShapeGeometry(shapeSegments.ToArray());
		}
		public IEnumerable<Point> GetConnectionPoints(Size size, double[] parameters) {
			ShapeLayoutCalculator context = CreateContext(size, parameters);
			return ConnectionPoints.Select(x => x.GetPoint(context));
		}
		public ParameterCollection GetParameterCollection() {
			var parameters = Parameters.Select(param => param.CreateParameterDescription()).ToArray();
			return new ParameterCollection(parameters);
		}
		public Rect GetEditorBounds(Size size, double[] parameters) {
			if(object.ReferenceEquals(EditorBounds, null))
				return new Rect(new Point(), size);
			ShapeContext context = new ShapeContext(size, parameters);
			return (Rect)context.Evaluate(EditorBounds, new List<ICustomFunctionOperator> { new CreateRectFunction() });
		}
	}
	public class DiagramResourceKey : ResourceKey {
		[ThreadStatic]
		static int idCore;
		readonly int id;
		readonly string resourceKey;
		public int Id { get { return id; } }
		public string ResourceKey { get { return resourceKey; } }
		public override System.Reflection.Assembly Assembly {
			get { return typeof(DiagramToolboxRegistrator).Assembly; }
		}
		public DiagramResourceKey(string resourceKey) {
			this.id = idCore++;
			this.resourceKey = resourceKey;
		}
	}
	public class ShapeKey : DiagramResourceKey {
		public ShapeKey(string resourceKey)
			: base(resourceKey) {
		}
	}
	public class Parameter {
		public double DefaultValue { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Value { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Point { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Min { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Max { get; set; }
		public ParameterDescription CreateParameterDescription() {
			return new ParameterDescription(GetParameterValue, GetParameterPoint, DefaultValue);
		}
		static double Evaluate(CriteriaOperatorContextBase context, CriteriaOperator op) {
			return Convert.ToDouble(context.Evaluate(op, null));
		}
		double GetParameterValue(double width, double height, double[] parameters, Point localPoint) {
			ValueGetterContext valueContext = new ValueGetterContext(new Size(width, height), localPoint);
			double value = Evaluate(valueContext, Value);
			return NormalizeParameter(new Size(width, height), parameters, value);
		}
		internal double NormalizeParameter(Size size, double[] parameters, double value) {
			var context = new ShapeContext(size, parameters);
			if(!object.ReferenceEquals(Min, null))
				value = Math.Max(value, Evaluate(context, Min));
			if(!object.ReferenceEquals(Max, null))
				value = Math.Min(value, Evaluate(context, Max));
			return value;
		}
		Point GetParameterPoint(double width, double height, double[] parameters, double value) {
			value = NormalizeParameter(new Size(width, height), parameters, value);
			PointGetterContext context = new PointGetterContext(new Size(width, height), value);
			return (Point)context.Evaluate(Point, new List<ICustomFunctionOperator> { new CreatePointFunction() });
		}
	}
	internal class ShapeReference {
		public Size DefaultSize { get; set; }
		public bool IsQuick { get; set; }
		public bool UseBackgroundAsForeground { get; set; }
		public bool HasParameters { get; set; }
		public bool HasCustomEditorBounds { get; set; }
	}
	public enum ArrowConnectorKind {
		Left,
		Right,
		Center
	}
	[ContentProperty("Segments")]
	public class ArrowTemplate {
		readonly List<Segment> segmentsCore;
		public List<Segment> Segments { get { return segmentsCore; } }
		public ArrowConnectorKind ConnectorKind { get; set; }
		public ArrowTemplate() {
			segmentsCore = new List<Segment>();
		}
		public ShapeGeometry GetShape(Size size) {
			List<ShapeSegment> shapeSegments = new List<ShapeSegment>();
			ShapeLayoutCalculator context = CreateContext(size);
			foreach(Segment segment in Segments) {
				shapeSegments.Add(segment.CreateShapeSegment(context));
			}
			return new ShapeGeometry(shapeSegments.ToArray());
		}
		ShapeLayoutCalculator CreateContext(Size size) {
			return new ShapeLayoutCalculator(UnitCollection.Default, UnitCollection.Default, size, null);
		}
	}
}
