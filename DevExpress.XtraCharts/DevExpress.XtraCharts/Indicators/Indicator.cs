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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Indicator : ChartElementNamed, ISupportInitialize, IXtraSerializable, IHitTest, ILegendItem {
		const bool DefaultCheckedInLegend = true;
		const bool DefaultCheckableInLegend = true;
		const bool DefaultShowInLegend = false;
		const bool DefaultVisible = true;
		static readonly Color DefaultColor = Color.Empty;
		readonly HitTestState hitTestState = new HitTestState();
		readonly LineStyle lineStyle;
		Color color = DefaultColor;
		bool visible = DefaultVisible;
		bool showInLegend = DefaultShowInLegend;
		IndicatorBehavior indicatorBehavior;
		bool loading;
		bool checkedInLegend = DefaultCheckedInLegend;
		bool checkableInLegend = DefaultCheckableInLegend;
		bool UseTemplateHit {
			get {
				XYDiagram2DSeriesViewBase view = View;
				if (view == null)
					return false;
				Series series = view.Owner as Series;
				return series != null && series.UseTemplateHit;
			}
		}
		Indicator TemplateIndicator {
			get {
				XYDiagram2DSeriesViewBase view = View;
				if (view == null)
					return this;
				Chart chart = view.Chart;
				if (chart == null)
					return this;
				XYDiagram2DSeriesViewBase templateView = chart.DataContainer.SeriesTemplate.View as XYDiagram2DSeriesViewBase;
				return templateView == null ? this : templateView.Indicators[view.Indicators.IndexOf(this)];
			}
		}
		internal Series OwningSeries {
			get { return this.Owner.Owner as Series; }
		}
		internal HitTestController HitTestController {
			get { return View.Chart.HitTestController;
			}
		}
		internal IndicatorBehavior IndicatorBehavior {
			get { return indicatorBehavior; }
		}
		protected internal override bool Loading {
			get { return loading || base.Loading; }
		}
		protected internal bool ShouldBeDrawnOnDiagram {
			get {
				if (ChartContainer == null || OwningSeries == null)
					return false;
				bool useLegendCheckBox = ChartContainer.Chart.Legend.UseCheckBoxes && checkableInLegend;
				if (useLegendCheckBox)
					return OwningSeries.ShouldBeDrawnOnDiagram && checkedInLegend;
				else
					return visible && OwningSeries.ShouldBeDrawnOnDiagram;
			}
		}
		[
		Browsable(false),
		NonTestableProperty
		]
		public abstract string IndicatorName {
			get;
		}
		[
		Browsable(false),
		NonTestableProperty
		]
		public XYDiagram2DSeriesViewBase View {
			get { return (XYDiagram2DSeriesViewBase)Owner; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("IndicatorName"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Indicator.Name"),
		Localizable(true),
		XtraSerializableProperty
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable {
			get { return GetType().Name; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("IndicatorLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Indicator.LineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle {
			get { return lineStyle; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("IndicatorColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Indicator.Color"),
		Category(Categories.Appearance),
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
	DevExpressXtraChartsLocalizedDescription("IndicatorVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Indicator.Visible"),
		Category(Categories.Behavior),
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
	DevExpressXtraChartsLocalizedDescription("IndicatorCheckedInLegend"),
#endif
		DXDisplayNameIgnore,
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool CheckedInLegend {
			get { return checkedInLegend; }
			set {
				if (value != checkedInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkedInLegend = value;
					if (ContainerAdapter != null)
						ContainerAdapter.OnLegendItemChecked(new LegendItemCheckedEventArgs(this, value));
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("IndicatorCheckableInLegend"),
#endif
		DXDisplayNameIgnore,
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool CheckableInLegend {
			get { return checkableInLegend; }
			set {
				if (value != checkableInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkableInLegend = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("IndicatorShowInLegend"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Indicator.ShowInLegend"),
		Category(Categories.Behavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowInLegend {
			get { return showInLegend; }
			set {
				if (value != showInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					showInLegend = value;
					RaiseControlChanged();
				}
			}
		}
		protected Indicator(string name) : base(name) {
			lineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
			UpdateBehavior();
		}
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		#endregion
		#region IHitTest implementation
		object IHitTest.Object {
			get { return UseTemplateHit ? ((IHitTest)TemplateIndicator).Object : this; }
		}
		HitTestState IHitTest.State {
			get { return UseTemplateHit ? ((IHitTest)TemplateIndicator).State : hitTestState; }
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeName() {
			return !String.IsNullOrEmpty(Name);
		}
		void ResetName() {
			Name = string.Empty;
		}
		bool ShouldSerializeLineStyle() {
			return lineStyle.ShouldSerialize();
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeShowInLegend() {
			return showInLegend != DefaultShowInLegend;
		}
		void ResetShowInLegend() {
			ShowInLegend = DefaultShowInLegend;
		}
		bool ShouldSerializeCheckedInLegend() {
			return checkedInLegend != DefaultCheckedInLegend;
		}
		void ResetCheckedInLegend() {
			CheckedInLegend = DefaultCheckedInLegend;
		}
		bool ShouldSerializeCheckableInLegend() {
			return checkableInLegend != DefaultCheckableInLegend;
		}
		void ResetCheckableInLegend() {
			CheckableInLegend = DefaultCheckableInLegend;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Name":
					return ShouldSerializeName();
				case "TypeNameSerializable":
					return true;
				case "LineStyle":
					return ShouldSerializeLineStyle();
				case "Color":
					return ShouldSerializeColor();
				case "Visible":
					return ShouldSerializeVisible();
				case "CheckedInLegend":
					return ShouldSerializeCheckedInLegend();
				case "ShowInLegend":
					return ShouldSerializeShowInLegend();
				case "CheckableInLegend":
					return ShouldSerializeCheckableInLegend();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected void UpdateBehavior() {
			indicatorBehavior = CreateBehavior();
		}
		protected abstract IndicatorBehavior CreateBehavior();
		protected internal virtual void Validate(XYDiagram2DSeriesViewBase view, IRefinedSeries refinedSeries) { }
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Indicator indicator = obj as Indicator;
			if (indicator != null) {
				lineStyle.Assign(indicator.lineStyle);
				color = indicator.color;
				visible = indicator.visible;
				showInLegend = indicator.showInLegend;
				checkedInLegend = indicator.checkedInLegend;
				checkableInLegend = indicator.checkableInLegend;
			}
		}
	}
}
