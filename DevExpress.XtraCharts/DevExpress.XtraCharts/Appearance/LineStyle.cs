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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum DashStyle {
		Empty,
		Solid,
		Dash,
		Dot,
		DashDot,
		DashDotDot
	}
	[
	TypeConverter(typeof(ExpandableObjectConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class LineStyle : ChartElement {
		#region Nested class: XmlKeys
		class XmlKeys {
			public const string Thickness = "Thickness";
			public const string DashStyle = "DashStyle";
		}
		#endregion
		int defaultThickness;
		protected int lineThickness;
		DashStyle defaultDashStyle;
		DashStyle dashStyle;
		LineJoin lineJoin;
		bool antiAlias;
		internal IChartAppearance ActualAppearance {
			get {
				IChartAppearance appearance = CommonUtils.GetActualAppearance(Owner);
				if(appearance == null)
					appearance = AppearanceRepository.Default;
				return appearance;
			}
		}
		internal bool AntiAlias { get { return antiAlias; } }
		internal DashStyle ActualDashStyle {
			get {
				if(Owner is ConstantLine)
					return this.DashStyle == DashStyle.Empty ?
						ActualAppearance.ConstantLineAppearance.DashStyle :
						this.dashStyle;
				return this.dashStyle;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineStyleThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineStyle.Thickness"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public virtual int Thickness {
			get { return lineThickness; }
			set {
				if(value != lineThickness) {
					if(value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineThickness));
					SendNotification(new ElementWillChangeNotification(this));
					lineThickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineStyleDashStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineStyle.DashStyle"),
		TypeConverter(typeof(DashStyleTypeConterter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				if (value != dashStyle) {
					if (value == DashStyle.Empty && !(Owner is ConstantLine))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDashStyle));
					SendNotification(new ElementWillChangeNotification(this));
					dashStyle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("LineStyleLineJoin"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.LineStyle.LineJoin"),
		TypeConverter(typeof(LineJoinTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public LineJoin LineJoin {
			get { return lineJoin; }
			set {
				if (value != lineJoin) {
					SendNotification(new ElementWillChangeNotification(this));
					lineJoin = value;
					RaiseControlChanged();
				}
			}
		}
		internal LineStyle(ChartElement owner) : this(owner, 1) {
		}
		internal LineStyle(ChartElement owner, int thickness) : this(owner, thickness, true) {
		}
		internal LineStyle(ChartElement owner, int thickness, bool antiAlias) : base(owner) {
			InitializeThickness(thickness);
			InitializeDashStyle(DashStyle.Solid);
			this.antiAlias = antiAlias;
		}
		internal LineStyle(ChartElement owner, int thickness, bool antialias, DashStyle dashStyle) : this(owner, thickness, antialias) {
			InitializeDashStyle(dashStyle);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Thickness")
				return ShouldSerializeThickness();
			if(propertyName == "DashStyle")
				return ShouldSerializeDashStyle();
			if (propertyName == "LineJoin")
				return ShouldSerializeLineJoin();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeThickness() {
			return lineThickness != this.defaultThickness;
		}
		void ResetThickness() {
			Thickness = this.defaultThickness;
		}
		bool ShouldSerializeDashStyle() {
			return this.dashStyle != this.defaultDashStyle;
		}
		void ResetDashStyle() {
			DashStyle = this.defaultDashStyle;
		}
		bool ShouldSerializeLineJoin() {
			return this.lineJoin != System.Drawing.Drawing2D.LineJoin.Miter;
		}
		void ResetLineJoin() {
			LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeThickness() ||
				ShouldSerializeDashStyle() ||
				ShouldSerializeLineJoin();
		}
		#endregion
		void InitializeThickness(int thickness) {
			this.lineThickness = thickness;
			this.defaultThickness = thickness;
		}
		void InitializeDashStyle(DashStyle dashStyle) {
			this.dashStyle = dashStyle;
			this.defaultDashStyle = dashStyle;
		}		
		protected override ChartElement CreateObjectForClone() {
			return new LineStyle(null);
		}
		internal void SetLineStyle(Line line) {
			line.SetParameters(ActualDashStyle, Thickness);
		}
		internal void SetAntialiasing(bool antiAlias) {
			this.antiAlias = antiAlias;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			LineStyle style = obj as LineStyle;
			return style != null && lineThickness == style.lineThickness &&
				dashStyle == style.dashStyle &&
				lineJoin == style.lineJoin;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			LineStyle style = obj as LineStyle;
			if(style == null)
				return;
			lineThickness = style.lineThickness;
			dashStyle = style.dashStyle;
			lineJoin = style.lineJoin;
			antiAlias = style.antiAlias;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class DashStyleHelper {
		static System.Drawing.Drawing2D.DashStyle ConvertDashStyle(DashStyle dashStyle) {
			switch (dashStyle) {
				case DashStyle.Solid:
					return System.Drawing.Drawing2D.DashStyle.Solid;
				case DashStyle.Dash:
					return System.Drawing.Drawing2D.DashStyle.Dash;
				case DashStyle.Dot:
					return System.Drawing.Drawing2D.DashStyle.Dot;
				case DashStyle.DashDot:
					return System.Drawing.Drawing2D.DashStyle.DashDot;
				case DashStyle.DashDotDot:
					return System.Drawing.Drawing2D.DashStyle.DashDotDot;
				case DashStyle.Empty:
					return System.Drawing.Drawing2D.DashStyle.Custom;
				default:
					throw new DefaultSwitchException();
			}
		}
		public static void ApplyDashStyle(Pen pen, DashStyle dashStyle) {
			pen.Alignment = PenAlignment.Center;
			pen.DashStyle = ConvertDashStyle(dashStyle);
		}
	}
}
