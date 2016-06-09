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
using System.Xml;
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
	public abstract class FillOptionsBase : ChartElement {
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FillOptionsBaseHiddenSerializableString"),
#endif
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DefaultValue(""),
		Obsolete("This property is obsolete now.")
		]
		public string HiddenSerializableString { get { return String.Empty; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		protected FillOptionsBase() : base() {
		}
		protected internal abstract void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2);
		protected internal abstract void RenderRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected internal abstract void Render(IRenderer renderer, LineStrip vertices, RectangleF boundedRectangle, Color color, Color color2);
		protected internal abstract void Render(IRenderer renderer, BezierRangeStrip strip, Color color, Color color2);
		protected internal abstract void RenderCircle(IRenderer renderer, PointF center, float radius, Color color, Color color2);
		protected internal abstract void RenderEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, Color color, Color color2);
		protected internal abstract void RenderPie(IRenderer renderer, PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		protected internal abstract void RenderPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		protected internal abstract GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected internal abstract GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundedRectangle, Color color, Color color2);
		protected internal abstract GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		protected internal abstract PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2);		
		protected internal abstract void ReadFromXml(XmlReader xmlReader);		
		internal void SetStyle(FillStyleBase style) {
			Owner = style;
		}		
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			return obj.GetType().Equals(GetType());
		}
	}
	[
	TypeConverter(typeof(SolidFillOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
				   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public class SolidFillOptions : FillOptionsBase {
		public SolidFillOptions() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new SolidFillOptions();
		}
		protected internal override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			using(Brush brush = new SolidBrush(color)) {
				gr.FillPath(brush, polygon.GetPath());
			}
		}
		protected internal override void RenderRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, color);
		}
		protected internal override void Render(IRenderer renderer, LineStrip vertices, RectangleF boundedRectangle, Color color, Color color2) {
			renderer.FillPolygon(vertices, color);
		}
		protected internal override void Render(IRenderer renderer, BezierRangeStrip strip, Color color, Color color2) {
			renderer.FillBezier(strip, color);
		}
		protected internal override void RenderCircle(IRenderer renderer, PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(new Point((int)center.X, (int)center.Y), (int)radius, color);
		}
		protected internal override void RenderEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(new Point((int)center.X, (int)center.Y), (int)semiAxisX, (int)semiAxisY, color);
		}
		protected internal override void RenderPie(IRenderer renderer, PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, color);
		}
		protected internal override void RenderPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, color);
		}
		protected internal override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new SolidRectangleGraphicsCommand(rect, color);
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundedRectangle, Color color, Color color2) {
			return new SolidPolygonGraphicsCommand(vertices, color);
		}
		protected internal override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new SolidPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, color);
		}
		protected internal override PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			polygon.SameColors = true;
			polygon.Color = color;
			return new PlanePolygon[] { polygon };
		}
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadString();
		}
		public override string ToString() {
			return "(None)";
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			SolidFillOptions options = obj as SolidFillOptions;
			return options != null && base.Equals(obj);
		}
	}
	public abstract class FillOptionsColor2Base : FillOptionsBase {
		#region Nested class: XmlKeys
		class XmlKeys {
			public const string Color2 = "Color2";
		}
		#endregion
		Color defaulColor2;
		Color color2;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FillOptionsColor2BaseColor2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FillOptionsColor2Base.Color2"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color2 {
			get { return color2; }
			set {
				if (value != color2) {
					SendNotification(new ElementWillChangeNotification(this));
					color2 = value;
					RaiseControlChanged();
				}
			}
		}
		public FillOptionsColor2Base()
			: base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Color2")
				return ShouldSerializeColor2();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor2() {
			return !this.color2.Equals(this.defaulColor2);
		}
		void ResetColor2() {
			Color2 = this.defaulColor2;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeColor2();
		}
		#endregion
		protected internal Color CalculateActualColor2(Color color, Color color2) {
			Color actualColor2 =
				this.color2 == Color.Empty ?
				color2 :
				this.color2;
			if (actualColor2 == Color.Empty)
				actualColor2 = color;
			return actualColor2;
		}
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			color2 = XmlUtils.ReadColor(xmlReader, XmlKeys.Color2);
		}
		internal void SetColor2AndInitDefaultColor2(Color color2) {
			this.color2 = color2;
			this.defaulColor2 = color2;
		}
		internal void SetColor2(Color color2) {
			this.color2 = color2;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FillOptionsColor2Base options = obj as FillOptionsColor2Base;
			if (options == null)
				return;
			this.color2 = options.color2;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			FillOptionsColor2Base options = obj as FillOptionsColor2Base;
			return
				options != null &&
				base.Equals(obj) &&
				this.color2 == options.color2;
		}
	}
	public abstract class GradientFillOptionsBase : FillOptionsColor2Base {
		protected GradientFillOptionsBase() : base() {
		}
		protected abstract IGradientPainter GetPainter(IRenderer renderer);
		protected abstract IGradientPainter GetPainter();
		protected internal override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			GetPainter(null).FillPolygon(gr, polygon, color, CalculateActualColor2(color, CalculateActualColor2(color, color2)));
		}
		protected internal override void RenderRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			GetPainter(renderer).RenderRectangle(rect, gradientRect, color, CalculateActualColor2(color, color2));
		}
		protected internal override void Render(IRenderer renderer, LineStrip vertices, RectangleF boundedRectangle, Color color, Color color2) {
			GetPainter(renderer).RenderStrip(vertices, boundedRectangle, color, CalculateActualColor2(color, color2));   
		}
		protected internal override void Render(IRenderer renderer, BezierRangeStrip strip, Color color, Color color2) {
			GetPainter(renderer).RenderBezier(strip, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderCircle(IRenderer renderer, PointF center, float radius, Color color, Color color2) {
			GetPainter(renderer).RenderCircle(center, radius, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			GetPainter(renderer).RenderEllipse(center, semiAxisX, semiAxisY, color, color2);
		}
		protected internal override void RenderPie(IRenderer renderer, PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			GetPainter(renderer).RenderPie(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			GetPainter(renderer).RenderPath(path, gradientRect, color, color2);
		}
		protected internal override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return GetPainter().CreateRectangleGraphicsCommand(rect, gradientRect, color, CalculateActualColor2(color, color2));
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundedRectangle, Color color, Color color2) {
			return GetPainter().CreateGraphicsCommand(vertices, boundedRectangle, color, CalculateActualColor2(color, color2));
		}
		protected internal override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return GetPainter().CreatePieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, CalculateActualColor2(color, color2));
		}
		protected internal override PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			return GetPainter().FillPlanePolygon(polygon, gradientRect, color, CalculateActualColor2(color, color2));
		}
		public override string ToString() {
			return "(Gradient)";
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RectangleGradientMode {
		TopToBottom,
		BottomToTop,
		LeftToRight,
		RightToLeft,
		TopLeftToBottomRight,
		BottomRightToTopLeft,
		TopRightToBottomLeft,
		BottomLeftToTopRight,
		FromCenterHorizontal,
		ToCenterHorizontal,
		FromCenterVertical,
		ToCenterVertical
	}
	[
	TypeConverter(typeof(RectangleGradientFillOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RectangleGradientFillOptions : GradientFillOptionsBase, IGradientFillOptions<RectangleGradientMode> {
		class XmlKeys {
			public const string GradientMode = "RectangleGradientMode";
		}
		internal const RectangleGradientMode DefaultGradientMode = RectangleGradientMode.TopToBottom;
		RectangleGradientMode gradientMode = DefaultGradientMode;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleGradientFillOptionsGradientMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleGradientFillOptions.GradientMode"),
		Editor("DevExpress.XtraCharts.Design.RectangleGradientModeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public RectangleGradientMode GradientMode {
			get { return gradientMode; }
			set {
				if(value != gradientMode) {
					SendNotification(new ElementWillChangeNotification(this));
					gradientMode = value;
					RaiseControlChanged();
				}
			}
		}
		public RectangleGradientFillOptions() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "GradientMode")
				return ShouldSerializeGradientMode();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeGradientMode() {
			return this.gradientMode != DefaultGradientMode;
		}
		void ResetGradientMode() {
			GradientMode = DefaultGradientMode;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeGradientMode();
		}
		#endregion
		#region IGradientFillOptions implementation
		IGradientPainter IGradientFillOptions<RectangleGradientMode>.GetPainter(RectangleGradientMode gradientMode, IRenderer renderer) {
			return GetPainter(gradientMode, renderer);
		}
		#endregion
		protected override IGradientPainter GetPainter(IRenderer renderer) {
			return GetPainter(this.gradientMode, renderer);
		}
		protected override IGradientPainter GetPainter() {
			return GetPainter(this.gradientMode, null);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RectangleGradientFillOptions();
		}
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			base.ReadFromXml(xmlReader);
			gradientMode = (RectangleGradientMode)XmlUtils.ReadEnum(xmlReader, XmlKeys.GradientMode, typeof(RectangleGradientMode));
		}
		IGradientPainter GetPainter(RectangleGradientMode gradientMode, IRenderer renderer) {
			switch (gradientMode) {
				case RectangleGradientMode.TopToBottom:
					return new TopToBottomGradientPainter(renderer);
				case RectangleGradientMode.BottomToTop:
					return new BottomToTopGradientPainter(renderer);
				case RectangleGradientMode.LeftToRight:
					return new LeftToRightGradientPainter(renderer);
				case RectangleGradientMode.RightToLeft:
					return new RightToLeftGradientPainter(renderer);
				case RectangleGradientMode.TopLeftToBottomRight:
					return new TopLeftToBottomRightGradientPainter(renderer);
				case RectangleGradientMode.BottomRightToTopLeft:
					return new BottomRightToTopLeftGradientPainter(renderer);
				case RectangleGradientMode.TopRightToBottomLeft:
					return new TopRightToBottomLeftGradientPainter(renderer);
				case RectangleGradientMode.BottomLeftToTopRight:
					return new BottomLeftToTopRightGradientPainter(renderer);
				case RectangleGradientMode.FromCenterHorizontal:
					return new FromCenterHorizontalGradientPainter(renderer);
				case RectangleGradientMode.ToCenterHorizontal:
					return new ToCenterHorizontalGradientPainter(renderer);
				case RectangleGradientMode.FromCenterVertical:
					return new FromCenterVerticalGradientPainter(renderer);
				case RectangleGradientMode.ToCenterVertical:
					return new ToCenterVerticalGradientPainter(renderer);
				default:
					throw new DefaultSwitchException();
			}
		}
		internal void RotateGradientMode() {
			switch (gradientMode) {
				case RectangleGradientMode.TopToBottom:
					gradientMode = RectangleGradientMode.RightToLeft;
					break;
				case RectangleGradientMode.BottomToTop:
					gradientMode = RectangleGradientMode.LeftToRight;
					break;
				case RectangleGradientMode.LeftToRight:
					gradientMode = RectangleGradientMode.TopToBottom;
					break;
				case RectangleGradientMode.RightToLeft:
					gradientMode = RectangleGradientMode.BottomToTop;
					break;
				case RectangleGradientMode.FromCenterHorizontal:
					gradientMode = RectangleGradientMode.FromCenterVertical;
					break;
				case RectangleGradientMode.ToCenterHorizontal:
					gradientMode = RectangleGradientMode.ToCenterVertical;
					break;
				case RectangleGradientMode.FromCenterVertical:
					gradientMode = RectangleGradientMode.FromCenterHorizontal;
					break;
				case RectangleGradientMode.ToCenterVertical:
					gradientMode = RectangleGradientMode.ToCenterHorizontal;
					break;
				case RectangleGradientMode.TopLeftToBottomRight:
					gradientMode = RectangleGradientMode.TopRightToBottomLeft;
					break;
				case RectangleGradientMode.TopRightToBottomLeft:
					gradientMode = RectangleGradientMode.BottomRightToTopLeft;
					break;
				case RectangleGradientMode.BottomLeftToTopRight:
					gradientMode = RectangleGradientMode.TopLeftToBottomRight;
					break;
				case RectangleGradientMode.BottomRightToTopLeft:
					gradientMode = RectangleGradientMode.BottomLeftToTopRight;
					break;
			}
		}
		internal void InvertGradientMode() {
			switch (gradientMode) {
				case RectangleGradientMode.TopToBottom:
					gradientMode = RectangleGradientMode.BottomToTop;
					break;
				case RectangleGradientMode.BottomToTop:
					gradientMode = RectangleGradientMode.TopToBottom;
					break;
				case RectangleGradientMode.BottomLeftToTopRight:
					gradientMode = RectangleGradientMode.TopLeftToBottomRight;
					break;
				case RectangleGradientMode.TopLeftToBottomRight:
					gradientMode = RectangleGradientMode.BottomLeftToTopRight;
					break;
				case RectangleGradientMode.BottomRightToTopLeft:
					gradientMode = RectangleGradientMode.TopRightToBottomLeft;
					break;
				case RectangleGradientMode.TopRightToBottomLeft:
					gradientMode = RectangleGradientMode.BottomRightToTopLeft;
					break;
				case RectangleGradientMode.FromCenterVertical:
					gradientMode = RectangleGradientMode.ToCenterVertical;
					break;
				case RectangleGradientMode.ToCenterVertical:
					gradientMode = RectangleGradientMode.FromCenterVertical;
					break;
			}
		}
		internal void RestoreGradientOrientation(RectangleGradientMode gradientMode) {
			this.gradientMode = gradientMode;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RectangleGradientFillOptions options = obj as RectangleGradientFillOptions;
			if(options == null)
				return;
			this.gradientMode = options.gradientMode;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			RectangleGradientFillOptions options = obj as RectangleGradientFillOptions;
			return options != null && base.Equals(obj) && 
				gradientMode == options.gradientMode;
		}
	}
	[
	TypeConverter(typeof(PolygonGradientModeTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum PolygonGradientMode {
		TopToBottom,
		BottomToTop,
		LeftToRight,
		RightToLeft,
		TopLeftToBottomRight,
		BottomRightToTopLeft,
		TopRightToBottomLeft,
		BottomLeftToTopRight,
		ToCenter,
		FromCenter
	}
	[
	TypeConverter(typeof(PolygonGradientFillOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
				   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PolygonGradientFillOptions : GradientFillOptionsBase, IGradientFillOptions<PolygonGradientMode> {
		class XmlKeys {
			public const string GradientMode = "PolygonGradientMode";
		}
		const PolygonGradientMode DefaultGradientMode = PolygonGradientMode.TopToBottom;
		PolygonGradientMode gradientMode = DefaultGradientMode;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PolygonGradientFillOptionsGradientMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PolygonGradientFillOptions.GradientMode"),
		Editor("DevExpress.XtraCharts.Design.PolygonGradientModeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]		
		public PolygonGradientMode GradientMode {
			get { return gradientMode; }
			set {
				if(value != gradientMode) {
					SendNotification(new ElementWillChangeNotification(this));
					if(Owner != null && (Owner.Owner is AreaSeriesView) && (value == PolygonGradientMode.ToCenter || value == PolygonGradientMode.FromCenter))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidGradientMode));
					gradientMode = value;
					RaiseControlChanged();
				}
			}
		}
		public PolygonGradientFillOptions() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "GradientMode")
				return ShouldSerializeGradientMode();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeGradientMode() {
			return this.gradientMode != DefaultGradientMode;
		}
		void ResetGradientMode() {
			GradientMode = DefaultGradientMode;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeGradientMode();
		}
		#endregion
		#region IGradientFillOptions implementation
		IGradientPainter IGradientFillOptions<PolygonGradientMode>.GetPainter(PolygonGradientMode gradientMode, IRenderer renderer) {
			return GetPainter(gradientMode, renderer);
		}
		#endregion
		protected override IGradientPainter GetPainter(IRenderer renderer) {
			return GetPainter(this.gradientMode, renderer);
		}
		protected override IGradientPainter GetPainter() {
			return GetPainter(this.gradientMode, null);
		}
		internal void RotateGradientMode() {
			switch (gradientMode) {
				case PolygonGradientMode.TopToBottom:
					gradientMode = PolygonGradientMode.RightToLeft;
					break;
				case PolygonGradientMode.BottomToTop:
					gradientMode = PolygonGradientMode.LeftToRight;
					break;
				case PolygonGradientMode.LeftToRight:
					gradientMode = PolygonGradientMode.TopToBottom;
					break;
				case PolygonGradientMode.RightToLeft:
					gradientMode = PolygonGradientMode.BottomToTop;
					break;
				case PolygonGradientMode.TopLeftToBottomRight:
					gradientMode = PolygonGradientMode.TopRightToBottomLeft;
					break;
				case PolygonGradientMode.TopRightToBottomLeft:
					gradientMode = PolygonGradientMode.BottomRightToTopLeft;
					break;
				case PolygonGradientMode.BottomLeftToTopRight:
					gradientMode = PolygonGradientMode.TopLeftToBottomRight;
					break;
				case PolygonGradientMode.BottomRightToTopLeft:
					gradientMode = PolygonGradientMode.BottomLeftToTopRight;
					break;
			}
		}
		internal IGradientPainter GetPainter(PolygonGradientMode gradientMode, IRenderer renderer) {
			switch(gradientMode) {
				case PolygonGradientMode.TopToBottom:
					return new TopToBottomGradientPainter(renderer);
				case PolygonGradientMode.BottomToTop:
					return new BottomToTopGradientPainter(renderer);
				case PolygonGradientMode.LeftToRight:
					return new LeftToRightGradientPainter(renderer);
				case PolygonGradientMode.RightToLeft:
					return new RightToLeftGradientPainter(renderer);
				case PolygonGradientMode.TopLeftToBottomRight:
					return new TopLeftToBottomRightGradientPainter(renderer);
				case PolygonGradientMode.BottomRightToTopLeft:
					return new BottomRightToTopLeftGradientPainter(renderer);
				case PolygonGradientMode.TopRightToBottomLeft:
					return new TopRightToBottomLeftGradientPainter(renderer);
				case PolygonGradientMode.BottomLeftToTopRight:
					return new BottomLeftToTopRightGradientPainter(renderer);
				case PolygonGradientMode.FromCenter:
					return new FromCenterGradientPainter(renderer);
				case PolygonGradientMode.ToCenter:
					return new ToCenterGradientPainter(renderer);
				default:
					throw new DefaultSwitchException();
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new PolygonGradientFillOptions();
		}
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			base.ReadFromXml(xmlReader);
			this.gradientMode = (PolygonGradientMode)XmlUtils.ReadEnum(xmlReader, XmlKeys.GradientMode, typeof(PolygonGradientMode));
		}		
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PolygonGradientFillOptions options = obj as PolygonGradientFillOptions;
			if(options == null)
				return;
			this.gradientMode = options.gradientMode;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			PolygonGradientFillOptions options = obj as PolygonGradientFillOptions;
			return 
				options != null && 
				base.Equals(obj) && 
				gradientMode == options.gradientMode;
		}
	}
	[
	TypeConverter(typeof(HatchFillOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
				   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public class HatchFillOptions : FillOptionsColor2Base {
		class XmlKeys {
			public const string HatchStyle = "HatchStyle";
		}
		const HatchStyle DefaultHatchStyle = HatchStyle.BackwardDiagonal;
		HatchStyle hatchStyle = DefaultHatchStyle;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("HatchFillOptionsHatchStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.HatchFillOptions.HatchStyle"),
		TypeConverter(typeof(HatchStyleTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.HatchStyleTypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public HatchStyle HatchStyle {
			get { return hatchStyle; }
			set {
				if(value != hatchStyle) {
					SendNotification(new ElementWillChangeNotification(this));
					hatchStyle = value;
					RaiseControlChanged();
				}
			}
		}
		internal HatchFillOptions(Color color2) : this() {
			SetColor2AndInitDefaultColor2(color2);
		}
		public HatchFillOptions() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "HatchStyle")
				return ShouldSerializeHatchStyle();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeHatchStyle() {
			return this.hatchStyle != DefaultHatchStyle;
		}
		void ResetHatchStyle() {
			HatchStyle = DefaultHatchStyle;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeHatchStyle();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new HatchFillOptions();
		}
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			base.ReadFromXml(xmlReader);
			this.hatchStyle = (HatchStyle)XmlUtils.ReadEnum(xmlReader, XmlKeys.HatchStyle, typeof(HatchStyle));
		}
		protected internal override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			using(Brush brush = new HatchBrush(hatchStyle, color, CalculateActualColor2(color, color2))) {
				gr.FillPath(brush, polygon.GetPath());
			}
		}
		protected internal override void RenderRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override void Render(IRenderer renderer, LineStrip vertices, RectangleF boundedRectangle, Color color, Color color2) {
			renderer.FillPolygon(vertices, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override void Render(IRenderer renderer, BezierRangeStrip strip, Color color, Color color2) {
			renderer.FillBezier(strip, hatchStyle, color, CalculateActualColor2(color, color2)); 
		}
		protected internal override void RenderCircle(IRenderer renderer, PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderPie(IRenderer renderer, PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override void RenderPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new HatchedRectangleGraphicsCommand(rect, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundedRectangle, Color color, Color color2) {
			return new HatchedPolygonGraphicsCommand(vertices, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new HatchedPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, hatchStyle, color, CalculateActualColor2(color, color2));
		}
		protected internal override PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			return new PlanePolygon[] { polygon };
		}
		public override string ToString() {
			return "(Hatch)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			HatchFillOptions options = obj as HatchFillOptions;
			if(options == null)
				return;
			this.hatchStyle = options.hatchStyle;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			HatchFillOptions options = obj as HatchFillOptions;
			return 
				options != null && 
				base.Equals(obj) && 
				hatchStyle == options.hatchStyle;
		}
	}
}
