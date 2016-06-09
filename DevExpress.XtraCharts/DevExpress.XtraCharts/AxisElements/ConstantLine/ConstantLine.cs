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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
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
	[TypeConverter(typeof(ConstantLineTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + 
	AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class ConstantLine : ChartElementNamed, IConstantLine, IHitTest, ICheckableLegendItemData, ILegendItem {
		const int DefaultThickness = 3;
		const double DefaultValue = 1.0;
		const bool DefaultCheckedInLegend = true;
		const bool DefaultCheckableInLegend = true;
		const string DefaultLegendText = "";
		const bool DefaultVisible = true;
		const bool DefaultShowInLegend = true;
		const bool DefaultShowBehind = false;
		static readonly Color DefaultColor = Color.Empty;
		object axisValue;
		string axisValueSerializable;
		string legendText = DefaultLegendText;
		double value = DefaultValue;
		bool visible = true;
		bool checkedInLegend = DefaultCheckedInLegend;
		bool checkableInLegend = DefaultCheckableInLegend;
		bool showInLegend = true;
		bool showBehind;
		LineStyle lineStyle;
		Color color = DefaultColor;
		ConstantLineTitle title;
		HitTestState hitTestState = new HitTestState();
		Legend Legend { 
			get { return Axis.Diagram.Chart.Legend; } 
		}
		internal Color ActualColor {
			get {
				if (color != Color.Empty)
					return color;
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance == null ? Color.Empty : actualAppearance.ConstantLineAppearance.Color;
			}
		}
		internal Axis2D Axis { 
			get { return (Axis2D)base.Owner; }
		}
		internal HitTestController HitTestController {
			get { return Axis.Diagram.Chart.HitTestController; } 
		}
		internal bool ActualShowInLegend {
			get {
				if (Axis.ChartContainer.Chart.Legend.UseCheckBoxes)
					return showInLegend;
				else
					return showInLegend && ((IConstantLine)this).Visible && CustomAxisElementsHelper.IsAxisValueVisible((IAxisData)Axis, ((IConstantLine)this).GetAxisValue());
			}
		}
		protected internal bool ShouldBeDrawnOnDiagram {
			get {
				if (Axis == null || Axis.Diagram == null || Axis.Diagram.Chart == null)
					return false;
				object axisValue = ((IAxisValueContainer)this).GetAxisValue();
				bool isInViewport = CustomAxisElementsHelper.IsAxisValueVisible((IAxisData)Axis, axisValue);
				bool useLegendCheckBox = Axis.Diagram.Chart.Legend.UseCheckBoxes && checkableInLegend;
				bool isVisibleByLegendCheckboxes = useLegendCheckBox ? visible && checkedInLegend : visible;
				return isInViewport && isVisibleByLegendCheckboxes;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineAxisValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.AxisValue"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public object AxisValue {
			get { return axisValue; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectConstantLineAxisValue));
				if (value != axisValue) {
					SendNotification(new ElementWillChangeNotification(this));
					axisValue = (Axis == null || Loading) ? value : Axis.ConvertBasedOnScaleType(value);
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
					RaiseControlChanged();
				}
			}
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty]
		public string AxisValueSerializable {
			get { return SerializingUtils.ConvertToSerializable(AxisValue); }
			set {
				AxisValue = value;
				if (Axis == null || Loading)
					axisValueSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.LineStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public LineStyle LineStyle { 
			get { return lineStyle; } 
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.Color"),
		Category(Categories.Appearance),
		XtraSerializableProperty]
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
	DevExpressXtraChartsLocalizedDescription("ConstantLineShowInLegend"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.ShowInLegend"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
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
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineLegendText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.LegendText"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty]
		public string LegendText {
			get { return legendText; }
			set {
				if (value != legendText) {
					SendNotification(new ElementWillChangeNotification(this));
					legendText = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineTitle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.Title"),
		Category(Categories.Elements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ConstantLineTitle Title { get { return title; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineShowBehind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLine.ShowBehind"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public bool ShowBehind {
			get { return showBehind; }
			set {
				if (value != showBehind) {
					SendNotification(new ElementWillChangeNotification(this));
					showBehind = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineCheckedInLegend"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		DXDisplayNameIgnore]
		public bool CheckedInLegend {
			get { return checkedInLegend; }
			set {
				if (value != checkedInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkedInLegend = value;
					if (ContainerAdapter != null)
						ContainerAdapter.OnLegendItemChecked(new LegendItemCheckedEventArgs(this, value));
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineCheckableInLegend"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		DXDisplayNameIgnore]
		public bool CheckableInLegend {
			get { return checkableInLegend; }
			set {
				if (value != checkableInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkableInLegend = value;
					RaiseControlChanged(new AxisElementUpdateInfo(this, Axis as IAxisData));
					RaiseControlChanged();
				}
			}
		}
		public ConstantLine(string name)
			: base(name) {
			lineStyle = new LineStyle(this, 1, false, DashStyle.Empty);
			title = new ConstantLineTitle(this);
			value = 1.0;
		}
		public ConstantLine()
			: this(String.Empty) { }
		public ConstantLine(string name, double value)
			: this(name, (object)value) { }
		public ConstantLine(string name, DateTime value)
			: this(name, (object)value) { }
		public ConstantLine(string name, string value)
			: this(name, (object)value) { }
		public ConstantLine(string name, object axisValue)
			: this(name) {
			if (axisValue == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectConstantLineAxisValue));
			this.axisValue = axisValue;
			try {
				value = Convert.ToDouble(axisValue);
			}
			catch {
			}
		}
		#region IHitTest implementation
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		#region ICheckableLegendItem implementation
		bool ILegendItemData.DisposeFont { get { return false; } }
		bool ILegendItemData.DisposeMarkerImage { get { return false; } }
		bool ILegendItemData.MarkerVisible { get { return Legend.MarkerVisible; } }
		bool ILegendItemData.TextVisible { get { return Legend.TextVisible; } }
		bool ICheckableLegendItemData.Disabled { get { return false; } }
		Color ILegendItemData.TextColor { get { return Legend.ActualTextColor; } }
		Color ICheckableLegendItemData.MainColor { get { return ActualColor; } }
		Font ILegendItemData.Font { get { return Legend.Font; } }
		Image ILegendItemData.MarkerImage { get { return null; } }
		ChartImageSizeMode ILegendItemData.MarkerImageSizeMode { get { return ChartImageSizeMode.AutoSize; } }
		Size ILegendItemData.MarkerSize { get { return Legend.MarkerSize; } }
		string ILegendItemData.Text { get { return String.IsNullOrEmpty(legendText) ? Name : legendText; } }
		object ILegendItemData.RepresentedObject { get { return this; } }
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) {
			int lineLevel = bounds.Top + bounds.Height / 2;
			Point p1 = new Point(bounds.Left + 1, lineLevel);
			Point p2 = new Point(bounds.Right - 1, lineLevel);
			Color color = GraphicUtils.CorrectColorByHitTestState(ActualColor, hitTestState);
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawLine(p1, p2, color, 2, lineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
			renderer.ProcessHitTestRegion(HitTestController, this, null, new HitRegion(bounds));
		}
		bool ICheckableLegendItemData.UseCheckBox {
			get { return Legend.UseCheckBoxes && checkableInLegend; }
		}
		#endregion
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return Visible; } }
		CultureInfo IAxisValueContainer.Culture {
			get {
				return (axisValueSerializable != null && axisValueSerializable.Equals(axisValue)) ?
					CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
			}
		}
		object IAxisValueContainer.GetAxisValue() {
			return axisValue;
		}
		void IAxisValueContainer.SetAxisValue(object axisValue) {
			if (!Object.ReferenceEquals(axisValue, this.axisValue)) {
				this.axisValue = axisValue;
				axisValueSerializable = null;
			}
		}
		double IAxisValueContainer.GetValue() {
			return value;
		}
		void IAxisValueContainer.SetValue(double value) {
			this.value = value;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Visible":
					return ShouldSerializeVisible();
				case "CheckedInLegend":
					return ShouldSerializeCheckedInLegend();
				case "CheckableInLegend":
					return ShouldSerializeCheckableInLegend();
				case "Color":
					return ShouldSerializeColor();
				case "ShowInLegend":
					return ShouldSerializeShowInLegend();
				case "LegendText":
					return ShouldSerializeLegendText();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
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
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeShowInLegend() {
			return showInLegend != DefaultShowInLegend;
		}
		void ResetShowInLegend() {
			ShowInLegend = DefaultShowInLegend;
		}
		bool ShouldSerializeLegendText() {
			return legendText != DefaultLegendText;
		}
		void ResetLegendText() {
			LegendText = DefaultLegendText;
		}
		bool ShouldSerializeShowBehind() {
			return showBehind != DefaultShowBehind;
		}
		void ResetShowBehind() {
			ShowBehind = DefaultShowBehind;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ConstantLine();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ConstantLine constLine = obj as ConstantLine;
			if (constLine != null) {
				axisValue = constLine.axisValue;
				value = constLine.value;
				visible = constLine.visible;
				lineStyle.Assign(constLine.lineStyle);
				color = constLine.color;
				showInLegend = constLine.showInLegend;
				legendText = constLine.legendText;
				title.Assign(constLine.title);
				showBehind = constLine.showBehind;
				axisValueSerializable = null;
				checkedInLegend = constLine.checkedInLegend;
				checkableInLegend = constLine.checkableInLegend;
			}
		}
	}
}
