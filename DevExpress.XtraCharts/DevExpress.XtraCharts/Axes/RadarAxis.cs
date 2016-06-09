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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class RadarAxis : AxisBase {
		PolygonFillStyle interlacedFillStyle;
		RadarDiagramAppearance DiagramAppearance { get { return CommonUtils.GetActualAppearance(this).RadarDiagramAppearance; } }
		internal new RadarDiagram Diagram { get { return (RadarDiagram)base.Diagram; } }
		internal Color ActualInterlacedColor {
			get {
				Color interlacedColor = InterlacedColor;
				return interlacedColor.IsEmpty ? DiagramAppearance.InterlacedColor : interlacedColor;
			}
		}
		internal PolygonFillStyle ActualInterlacedFillStyle {
			get { return interlacedFillStyle.FillMode == FillMode.Empty ? (PolygonFillStyle)DiagramAppearance.InterlacedFillStyle : interlacedFillStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisInterlacedFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxis.InterlacedFillStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle InterlacedFillStyle { get { return interlacedFillStyle; } }
		protected RadarAxis(string name, RadarDiagram diagram) : base(name, diagram) {
			interlacedFillStyle = new PolygonFillStyle(this);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeInterlacedFillStyle() {
			return interlacedFillStyle.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeInterlacedFillStyle();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "InterlacedFillStyle" ? ShouldSerializeInterlacedFillStyle() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing && interlacedFillStyle != null) {
				interlacedFillStyle.Dispose();
				interlacedFillStyle = null;
			}
			base.Dispose(disposing);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RadarAxis axis = obj as RadarAxis;
			if (axis != null)
				interlacedFillStyle.Assign(axis.interlacedFillStyle);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarAxisX : RadarAxis {
		protected internal override bool IsValuesAxis { get { return false; } }
		protected override bool IsRadarAxis { get { return true; } }
		protected override int GridSpacingFactor { get { return 45; } }
		protected override int DefaultMinorCount { get { return 4; } }
		protected internal override bool IsVertical { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisXLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisX.Label"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new RadarAxisXLabel Label { get { return ActualLabel as RadarAxisXLabel; } }
		internal RadarAxisX(RadarDiagram diagram) : base(ChartLocalizer.GetString(ChartStringId.PrimaryAxisXName), diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarAxisX(null);
		}
		protected override AxisLabel CreateAxisLabel() {
			return new RadarAxisXLabel(this);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new RadarAxisXRange(this);
		}
		protected override GridLines CreateGridLines() {
			return new RadarGridLinesX(this);
		}
		public override string ToString() {
			return "(RadarAxisX)";
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarAxisY : RadarAxis {
		const int DefaultThickness = 1;
		const bool DefaultVisible = true;
		const bool DefaultTopLevel = true;
		static readonly Color DefaultColor = Color.Empty;
		bool visible = DefaultVisible;
		bool topLevel = DefaultTopLevel;
		int thickness = DefaultThickness;
		Color color = DefaultColor;
		RadarTickmarksY tickmarks;
		new RadarDiagram Diagram { get { return base.Diagram as RadarDiagram; } }
		Color ActualColor { get { return color.IsEmpty ? CommonUtils.GetActualAppearance(this).RadarDiagramAppearance.AxisColor : color; } }
		internal Color DrawingColor { get { return GraphicUtils.GetColor(ActualColor, ((IHitTest)this).State); } }
		protected internal override bool IsValuesAxis { get { return true; } }
		protected override int GridSpacingFactor { get { return 50; } }
		protected override int DefaultMinorCount { get { return 4; } }
		protected internal override bool IsVertical { get { return true; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYTopLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.TopLevel"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category("Behavior"),
		XtraSerializableProperty
		]
		public bool TopLevel {
			get { return topLevel; }
			set {
				if (value != topLevel) {
					SendNotification(new ElementWillChangeNotification(this));
					topLevel = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.Thickness"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return thickness; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisThickness));
				if (value != thickness) {
					SendNotification(new ElementWillChangeNotification(this));
					thickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.Color"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if (value != color) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYTickmarks"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.Tickmarks"),
		Category("Elements"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RadarTickmarksY Tickmarks { get { return tickmarks; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisYLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisY.Label"),
		Category("Elements"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new RadarAxisYLabel Label { get { return ActualLabel as RadarAxisYLabel; } }
		internal RadarAxisY(RadarDiagram diagram) : base(ChartLocalizer.GetString(ChartStringId.PrimaryAxisYName), diagram) {
			tickmarks = new RadarTickmarksY(this);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeTopLevel() {
			return topLevel != DefaultTopLevel;
		}
		void ResetTopLevel() {
			TopLevel = DefaultTopLevel;
		}
		bool ShouldSerializeThickness() {
			return thickness != DefaultThickness;
		}
		void ResetThickness() {
			Thickness = DefaultThickness;
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeTickmarks() {
			return tickmarks.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeVisible() || ShouldSerializeTopLevel() ||
				ShouldSerializeThickness() || ShouldSerializeColor() || ShouldSerializeTickmarks();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Visible":
					return ShouldSerializeVisible();
				case "TopLevel":
					return ShouldSerializeTopLevel();
				case "Thickness":
					return ShouldSerializeThickness();
				case "Color":
					return ShouldSerializeColor();
				case "Tickmarks":
					return ShouldSerializeTickmarks();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RadarAxisY(null);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new RadarAxisYRange(this);
		}
		protected override GridLines CreateGridLines() {
			return new RadarGridLinesY(this);
		}
		protected override AxisLabel CreateAxisLabel() {
			return new RadarAxisYLabel(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RadarAxisY axis = obj as RadarAxisY;
			if (axis != null) {
				visible = axis.visible;
				topLevel = axis.topLevel;
				thickness = axis.thickness;
				color = axis.color;
				tickmarks.Assign(axis.tickmarks);
			}
		}
		public override string ToString() {
			return "(RadarAxisY)";
		}
	}
}
