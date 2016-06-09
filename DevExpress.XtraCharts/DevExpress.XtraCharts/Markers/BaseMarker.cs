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
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum MarkerKind {
		Square,
		Diamond,
		Triangle,
		InvertedTriangle,
		Circle,
		Plus,
		Cross,
		Star,
		Pentagon,
		Hexagon
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(BaseMarkerTypeConverter))
	]
	public class MarkerBase : ChartElement {
		#region Nested classes: CalculatorBase, CalculatorCircle, CalculatorPlus, CalculatorCross, CalculatorPolygon, CalculatorSquare, CalculatorDiamond, CalculatorTriangle, CalculatorInvertedTriangle, CalculatorStar, CalculatorPentagon, CalculatorHexagon, MarkerKindEditor
		abstract class CalculatorBase {
			static CalculatorBase CreateInstance(MarkerKind kind, int starPointCount, GRealPoint2D center, float size) {
				CalculatorBase calculator;
				switch (kind) {
					case MarkerKind.Circle:
						calculator = new CalculatorCircle();
						break;
					case MarkerKind.Cross:
						calculator = new CalculatorCross();
						break;
					case MarkerKind.Diamond:
						calculator = new CalculatorDiamond();
						break;
					case MarkerKind.Hexagon:
						calculator = new CalculatorHexagon();
						break;
					case MarkerKind.InvertedTriangle:
						calculator = new CalculatorInvertedTriangle();
						break;
					case MarkerKind.Pentagon:
						calculator = new CalculatorPentagon();
						break;
					case MarkerKind.Plus:
						calculator = new CalculatorPlus();
						break;
					case MarkerKind.Square:
						calculator = new CalculatorSquare();
						break;
					case MarkerKind.Star:
						calculator = new CalculatorStar();
						break;
					case MarkerKind.Triangle:
						calculator = new CalculatorTriangle();
						break;
					default:
						throw new DefaultSwitchException();
				}
				calculator.Initialize(kind, starPointCount, center, size);
				return calculator;
			}
			public static CalculatorBase CreateInstance(MarkerKind kind, int starPointCount, RectangleF bounds) {
				GRealPoint2D center = new GRealPoint2D(bounds.Left + bounds.Width / 2.0f, bounds.Top + bounds.Height / 2.0f - 1.0f);
				float size = Math.Min(bounds.Width, bounds.Height);
				return CreateInstance(kind, starPointCount, center, size);
			}
			public static CalculatorBase CreateInstance(MarkerBase marker, GRealPoint2D pt, bool isSelected, int size) {
				if (isSelected)
					size += 3;
				return CreateInstance(marker.Kind, marker.StarPointCount, pt, size);
			}
			MarkerKind kind;
			int starPointCount;
			GRealPoint2D center;
			float size;
			RectangleF bounds;
			protected MarkerKind Kind { get { return kind; } }
			protected int StarPointCount { get { return starPointCount; } }
			protected float Size { get { return size; } }
			public IPolygon Polygon {
				get {
					Rectangle rect = Rectangle.Round(bounds);
					if (kind == MarkerKind.Circle)
						return new CirclePolygon(center, size / 2.0f);
					else
						return new VariousPolygon(GetPolygon(), rect);
				}
			}
			void Initialize(MarkerKind kind, int starPointCount, GRealPoint2D center, float size) {
				this.kind = kind;
				this.starPointCount = starPointCount;
				this.center = center;
				this.size = size;
				bounds = new RectangleF(new PointF((float)center.X, (float)center.Y), SizeF.Empty);
				bounds.Inflate(size / 2.0f, size / 2.0f);
			}
			protected LineStrip OffsetPoints(GRealPoint2D pt, LineStrip points) {
				LineStrip result = new LineStrip();
				foreach (GRealPoint2D point in points)
					result.Add(new GRealPoint2D(point.X + pt.X, pt.Y - point.Y));
				return result;
			}
			public LineStrip GetPolygon() {
				return OffsetPoints(center, CalcMarkerPolygon());
			}
			protected virtual LineStrip CalcMarkerPolygon() {
				return new LineStrip();
			}
		}
		class CalculatorCircle : CalculatorBase {
		}
		class CalculatorPlus : CalculatorBase {
			const float widthRatio = 0.4f;
			protected override LineStrip CalcMarkerPolygon() {
				float semiSize = Size / 2.0f;
				float semiWidth = semiSize * widthRatio;
				LineStrip points = new LineStrip(12);
				points.Add(new GRealPoint2D(-semiWidth, -semiSize));
				points.Add(new GRealPoint2D(semiWidth, -semiSize));
				points.Add(new GRealPoint2D(semiWidth, -semiWidth));
				points.Add(new GRealPoint2D(semiSize, -semiWidth));
				points.Add(new GRealPoint2D(semiSize, semiWidth));
				points.Add(new GRealPoint2D(semiWidth, semiWidth));
				points.Add(new GRealPoint2D(semiWidth, semiSize));
				points.Add(new GRealPoint2D(-semiWidth, semiSize));
				points.Add(new GRealPoint2D(-semiWidth, semiWidth));
				points.Add(new GRealPoint2D(-semiSize, semiWidth));
				points.Add(new GRealPoint2D(-semiSize, -semiWidth));
				points.Add(new GRealPoint2D(-semiWidth, -semiWidth));
				return points;
			}
		}
		class CalculatorCross : CalculatorPlus {
			static LineStrip RotateVectors(double rotateAngle, LineStrip points) {
				LineStrip result = new LineStrip(points.Count);
				foreach (GRealPoint2D point in points) {
					double magnitude = Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
					double angle = Math.Atan2(point.Y, point.X) + rotateAngle;
					double x = magnitude * Math.Cos(angle);
					double y = magnitude * Math.Sin(angle);
					result.Add(new GRealPoint2D(x, y));
				}
				return result;
			}
			protected override LineStrip CalcMarkerPolygon() {
				LineStrip points = base.CalcMarkerPolygon();
				return RotateVectors(Math.PI / 4, points);
			}
		}
		abstract class CalculatorPolygon : CalculatorBase {
			protected LineStrip CalcPolygon(double startAngle, int angleCount) {
				return GraphicUtils.CalcPolygon(startAngle, angleCount, new GRealPoint2D(), Size / 2.0f);
			}
		}
		class CalculatorSquare : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(Math.PI / 4.0, 4);
			}
		}
		class CalculatorDiamond : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(0, 4);
			}
		}
		class CalculatorTriangle : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(Math.PI / 2.0, 3);
			}
		}
		class CalculatorInvertedTriangle : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(-Math.PI / 2.0, 3);
			}
		}
		class CalculatorStar : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				int pointCount = StarPointCount * 2;
				float extSize = Size / 2.0f;
				float intSize = extSize * (StarPointCount <= 3 ? 0.25f : 0.43f);
				LineStrip extPoints = GraphicUtils.CalcPolygon(Math.PI / 2.0, StarPointCount, new GRealPoint2D(), extSize);
				LineStrip intPoints = GraphicUtils.CalcPolygon(Math.PI / 2.0 + Math.PI / StarPointCount, StarPointCount, new GRealPoint2D(), intSize);
				LineStrip points = new LineStrip(pointCount);
				for (int i = 0; i < pointCount; i++)
					points.Add(i % 2 == 0 ? extPoints[i / 2] : intPoints[i / 2]);
				return points;
			}
		}
		class CalculatorPentagon : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(Math.PI / 2.0, 5);
			}
		}
		class CalculatorHexagon : CalculatorPolygon {
			protected override LineStrip CalcMarkerPolygon() {
				return CalcPolygon(Math.PI / 2.0, 6);
			}
		}
		class MarkerKindEditor : UITypeEditor {
			public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
				return true;
			}
			public override void PaintValue(PaintValueEventArgs e) {
				Rectangle bounds = e.Bounds;
				bounds.Inflate(-2, -2);
				MarkerBase marker = e.Context.Instance as MarkerBase;
				if (marker != null) {
					GdiPlusRenderer renderer = new GdiPlusRenderer();
					renderer.Reset(e.Graphics, bounds);
					MarkerBase.Render(renderer, (MarkerKind)e.Value, marker.StarPointCount, bounds, null, Color.Black, Color.Empty, Color.Empty, 1);
					renderer.Present();
				}
			}
		}
		#endregion
		const MarkerKind DefaultKind = MarkerKind.Circle;
		const int DefaultStarPointCount = 5;
		static void Render(IRenderer renderer, IPolygon polygon, PolygonFillStyle fillStyle, Color color, Color color2, Color borderColor, int borderThickness) {
			renderer.EnableAntialiasing(true);
			try {
				using (GraphicsPath path = polygon.GetPath()) {
					if (fillStyle != null)
						polygon.Render(renderer, fillStyle.Options, color, color2, borderColor, borderThickness);
					else {
						using (Brush brush = new SolidBrush(color))
							renderer.FillPath(path, color);
					}					
				}
			}
			finally {
				renderer.RestoreAntialiasing();
			}
		}
		static internal void Render(IRenderer renderer, MarkerKind kind, int starPointCount, Rectangle bounds, PolygonFillStyle fillStyle, Color color, Color color2, Color borderColor, int borderThickness) {
			Render(renderer, CalculatorBase.CreateInstance(kind, starPointCount, bounds).Polygon, fillStyle, color, color2, borderColor, borderThickness);
		}
		MarkerKind kind = DefaultKind;
		int starPointCount = DefaultStarPointCount;
		PolygonFillStyle fillStyle;
		CustomBorder border;
		internal CustomBorder Border { get { return this.border; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerBaseKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BaseMarker.Kind"),
		RefreshProperties(RefreshProperties.All),
		Editor(typeof(MarkerKindEditor), typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty
		]
		public MarkerKind Kind {
			get { return this.kind; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.kind = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerBaseStarPointCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BaseMarker.StarPointCount"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int StarPointCount {
			get { return this.starPointCount; }
			set {
				if (value < 3 || value > 100)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMarkerStarPointCount));
				SendNotification(new ElementWillChangeNotification(this));
				this.starPointCount = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerBaseFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BaseMarker.FillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle FillStyle { get { return this.fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerBaseBorderVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BaseMarker.BorderVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool BorderVisible {
			get { return this.border.ActualVisibility; }
			set { this.border.Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MarkerBaseBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BaseMarker.BorderColor"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return this.border.Color; }
			set { this.border.Color = value; }
		}
		internal MarkerBase(ChartElement owner) : base(owner) {
			this.fillStyle = new PolygonFillStyle(this);
			this.border = new CustomBorder(this, true, Color.Empty);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "BorderColor")
				return ShouldSerializeBorderColor();
			if (propertyName == "Kind")
				return ShouldSerializeKind();
			if (propertyName == "StarPointCount")
				return ShouldSerializeStarPointCount();
			if (propertyName == "BorderVisible")
				return ShouldSerializeBorderVisible();
			if (propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBorderColor() {
			return !this.border.Color.IsEmpty;
		}
		void ResetBorderColor() {
			BorderColor = Color.Empty;
		}
		bool ShouldSerializeKind() {
			return this.kind != DefaultKind;
		}
		void ResetKind() {
			Kind = DefaultKind;
		}
		bool ShouldSerializeStarPointCount() {
			return starPointCount != DefaultStarPointCount;
		}
		void ResetStarPointCount() {
			StarPointCount = DefaultStarPointCount;
		}
		bool ShouldSerializeBorderVisible() {
			return !BorderVisible;
		}
		void ResetBorderVisible() {
			BorderVisible = true;
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBorderColor() ||
				ShouldSerializeKind() ||
				ShouldSerializeStarPointCount() ||
				ShouldSerializeBorderVisible() ||
				ShouldSerializeFillStyle();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new MarkerBase(null);
		}
		protected virtual internal IPolygon CalculatePolygon(Rectangle bounds) {
			RectangleF floatBounds = new RectangleF(bounds.Location, bounds.Size);
			floatBounds.Inflate(-1.5f, -1.5f);
			IPolygon polygon = CalculatorBase.CreateInstance(kind, starPointCount, floatBounds).Polygon;
			return polygon.Rect.AreWidthAndHeightPositive() ? polygon : null;
		}
		protected virtual internal void Render(IRenderer renderer, IPolygon polygon, Color color, Color color2, Color borderColor, int borderThickness) {
			renderer.EnablePolygonAntialiasing(true);
			if (fillStyle != null) {
				polygon.Render(renderer, fillStyle.Options, color, color2, borderColor, borderThickness);   
			}
			else {
				renderer.FillPolygon(polygon.Vertices, color);
				if (borderThickness > 0)
					renderer.DrawPolygon(polygon.Vertices, borderColor, borderThickness);
			}
			renderer.RestorePolygonAntialiasing();
		}
		protected internal IPolygon CalculatePolygon(GRealPoint2D origin, bool isSelected, int size) {
			return CalculatorBase.CreateInstance(this, origin, isSelected, size).Polygon;
		}
		protected internal ZPlaneRectangle GetActualBounds(DiagramPoint point, int size) {
			return MathUtils.MakeRectangle(point, new SizeF(size, size));
		}
		public override string ToString() {
			return "(Marker)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			MarkerBase marker = obj as MarkerBase;
			if (marker == null)
				return;
			this.kind = marker.kind;
			this.starPointCount = marker.starPointCount;
			this.fillStyle.Assign(marker.fillStyle);
			this.border.Assign(marker.border);
		}
		public override bool Equals(object obj) {
			MarkerBase marker = obj as MarkerBase;
			return marker != null && marker.GetType().Equals(GetType()) &&
				this.kind == marker.kind &&
				this.starPointCount == marker.starPointCount &&
				this.fillStyle.Equals(marker.fillStyle) &&
				this.border.Equals(marker.border);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
