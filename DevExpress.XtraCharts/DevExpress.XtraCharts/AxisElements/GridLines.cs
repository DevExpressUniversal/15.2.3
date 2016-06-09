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
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public abstract class GridLines : ChartElement {
		const bool DefaultMinorVisible = false;
		static readonly Color DefaultColor = Color.Empty;
		static readonly Color DefaultMinorColor = Color.Empty;
		bool visible;
		bool minorVisible = DefaultMinorVisible;
		LineStyle lineStyle;
		LineStyle minorLineStyle;
		Color color = DefaultColor;
		Color minorColor = DefaultMinorColor;
		internal AxisBase Axis { get { return (AxisBase)base.Owner; } }
		protected abstract bool DefaultVisible { get; }
		protected virtual bool DefaultAntialias { get { return false; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GridLinesVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
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
	DevExpressXtraChartsLocalizedDescription("GridLinesMinorVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.MinorVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool MinorVisible {
			get { return minorVisible; }
			set {
				if (value != minorVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					minorVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GridLinesLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.LineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle { get { return lineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GridLinesMinorLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.MinorLineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle MinorLineStyle { get { return minorLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GridLinesColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.Color"),
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
	DevExpressXtraChartsLocalizedDescription("GridLinesMinorColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GridLines.MinorColor"),
		XtraSerializableProperty
		]
		public Color MinorColor {
			get { return minorColor; }
			set {
				if (value != minorColor) {
					SendNotification(new ElementWillChangeNotification(this));
					minorColor = value;
					RaiseControlChanged();
				}
			}
		}
		protected GridLines(AxisBase axis) : base(axis){
			visible = DefaultVisible;
			lineStyle = new LineStyle(this, 1, DefaultAntialias);
			minorLineStyle = new LineStyle(this, 1, DefaultAntialias);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeMinorVisible() {
			return minorVisible != DefaultMinorVisible;
		}
		void ResetMinorVisible() {
			MinorVisible = DefaultMinorVisible;
		}
		bool ShouldSerializeLineStyle() {
			return lineStyle.ShouldSerialize();
		}
		bool ShouldSerializeMinorLineStyle() {
			return minorLineStyle.ShouldSerialize();
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeMinorColor() {
			return minorColor != DefaultMinorColor;
		}
		void ResetMinorColor() {
			MinorColor = DefaultMinorColor;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeVisible() || ShouldSerializeMinorVisible() ||
				ShouldSerializeLineStyle() || ShouldSerializeMinorLineStyle() || ShouldSerializeColor() || ShouldSerializeMinorColor();
		}
		#endregion
		#region XtraSeriealization
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Visible":
					return ShouldSerializeVisible();
				case "MinorVisible":
					return ShouldSerializeMinorVisible();
				case "LineStyle":
					return ShouldSerializeLineStyle();
				case "MinorLineStyle":
					return ShouldSerializeMinorLineStyle();
				case "Color":
					return ShouldSerializeColor();
				case "MinorColor":
					return ShouldSerializeMinorColor();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		internal Color GetActualColor(DiagramAppearance appearance) {
			if (!color.IsEmpty)
				return color;
			return appearance == null ? Color.Empty : appearance.GridLinesColor;
		}
		internal Color GetActualMinorColor(DiagramAppearance appearance) {
			if (!minorColor.IsEmpty)
				return minorColor;
			return appearance == null ? Color.Empty : appearance.MinorGridLinesColor;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			GridLines gridLines = obj as GridLines;
			if (gridLines != null) {
				visible = gridLines.visible;
				minorVisible = gridLines.minorVisible;
				lineStyle.Assign(gridLines.lineStyle);
				minorLineStyle.Assign(gridLines.minorLineStyle);
				color = gridLines.color;
				minorColor = gridLines.minorColor;
			}
		}
		public override string ToString() {
			return "(GridLines)";
		}
	}
	[Obsolete("This class is now obsolete.")]
	public abstract class GridLinesXBase : GridLines {
		internal GridLinesXBase(AxisBase axis) : base(axis) { }
	}
	[Obsolete("This class is now obsolete.")]
	public abstract class GridLinesYBase : GridLines {
		internal GridLinesYBase(AxisBase axis) : base(axis) { }
	}
	public sealed class GridLinesX : GridLines {
		protected override bool DefaultVisible { get { return false; } }
		internal GridLinesX() : base(null) { }
		internal GridLinesX(AxisX axis) : base(axis) {	}
		internal GridLinesX(SwiftPlotDiagramAxisX axis) : base(axis) { }
		internal GridLinesX(AxisX3D axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new GridLinesX();
		}
	}
	public sealed class GridLinesY : GridLines {
		protected override bool DefaultVisible { get { return true; } }
		internal GridLinesY() : base(null) { }
		internal GridLinesY(AxisY axis) : base(axis) { }
		internal GridLinesY(SwiftPlotDiagramAxisY axis) : base(axis) { }
		internal GridLinesY(AxisY3D axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new GridLinesY();
		}
	}
	public sealed class SecondaryGridLinesX : GridLines {
		protected override bool DefaultVisible { get { return false; } }
		internal SecondaryGridLinesX() : base(null) { }
		internal SecondaryGridLinesX(SecondaryAxisX axis) : base(axis) { }
		internal SecondaryGridLinesX(SwiftPlotDiagramSecondaryAxisX axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new SecondaryGridLinesX();
		}
	}
	public sealed class SecondaryGridLinesY : GridLines {
		protected override bool DefaultVisible { get { return false; } }
		internal SecondaryGridLinesY() : base(null) { }
		internal SecondaryGridLinesY(SecondaryAxisY axis) : base(axis) { }
		internal SecondaryGridLinesY(SwiftPlotDiagramSecondaryAxisY axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new SecondaryGridLinesY();
		}
	}
	public sealed class RadarGridLinesX : GridLines {
		protected override bool DefaultVisible { get { return true; } }
		internal RadarGridLinesX(RadarAxisX axis) : base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarGridLinesX(null);
		}
	}
	public sealed class RadarGridLinesY : GridLines {
		protected override bool DefaultVisible { get { return true; } }
		protected override bool DefaultAntialias { get { return true; } }
		internal RadarGridLinesY(RadarAxisY axis) : base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarGridLinesY(null);
		}
	}
}
