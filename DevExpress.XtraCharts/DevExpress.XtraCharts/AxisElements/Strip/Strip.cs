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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(StripTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + 
	AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class Strip : ChartElementNamed, IStrip, ICheckableLegendItemData, ISupportInitialize, IXtraSerializable, ILegendItem {
		const bool DefaultCheckedInLegend = true;
		const bool DefaultCheckableInLegend = true;
		const bool DefaultShowInLegend = true;
		const bool DefaultVisible = true;
		const bool DefaultShowAxisLabel = false;
		static readonly Color DefaultColor = Color.Empty;
		readonly MinStripLimit minLimit;
		readonly MaxStripLimit maxLimit;
		readonly RectangleFillStyle fillStyle;
		bool visible = true;
		bool showInLegend = DefaultShowInLegend;
		bool showAxisLabel;
		bool loading;
		bool checkedInLegend = DefaultCheckedInLegend;
		bool checkableInLegend = DefaultCheckableInLegend;
		Color color = Color.Empty;
		string legendText = String.Empty;
		string axisLabelText = String.Empty;		
		Legend Legend {
			get { return Axis.Diagram.Chart.Legend; } 
		}
		StripAppearance Appearance { 
			get { return CommonUtils.GetActualAppearance(this).StripAppearance; } 
		}
		internal Color ActualColor { 
			get { return color == Color.Empty ? Appearance.Color : color; } 
		}
		internal RectangleFillStyle ActualFillStyle {
			get { return fillStyle.FillMode == FillMode.Empty ? Appearance.FillStyle : fillStyle; } 
		}
		internal Axis2D Axis { 
			get { return (Axis2D)base.Owner; } }
		protected internal override bool Loading { 
			get { return loading || base.Loading; } 
		}
		protected internal bool ShouldBeDrawnOnDiagram {
			get {
				if (Axis == null || Axis.Diagram == null || Axis.Diagram.Chart == null)
					return false;
				bool visibleByScaleTypeMap = CustomAxisElementsHelper.IsStripVisible(Axis.ScaleTypeMap, this);
				bool useLegendCheckBox = Axis.Diagram.Chart.Legend.UseCheckBoxes && checkableInLegend;
				bool visibleByLegendCheckBoxesAndVisibleProperty = useLegendCheckBox ? visible && checkedInLegend : visible;
				return visibleByScaleTypeMap && visibleByLegendCheckBoxesAndVisibleProperty;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripMinLimit"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.MinLimit"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public MinStripLimit MinLimit { get { return minLimit; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripMaxLimit"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.MaxLimit"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public MaxStripLimit MaxLimit { get { return maxLimit; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.Visible"),
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
	DevExpressXtraChartsLocalizedDescription("StripColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.Color"),
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
	DevExpressXtraChartsLocalizedDescription("StripShowInLegend"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.ShowInLegend"),
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
	DevExpressXtraChartsLocalizedDescription("StripLegendText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.LegendText"),
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
	DevExpressXtraChartsLocalizedDescription("StripShowAxisLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.ShowAxisLabel"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public bool ShowAxisLabel {
			get { return showAxisLabel; }
			set {
				if (value != showAxisLabel) {
					SendNotification(new ElementWillChangeNotification(this));
					showAxisLabel = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripAxisLabelText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Strip.AxisLabelText"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty]
		public string AxisLabelText {
			get { return axisLabelText; }
			set {
				if (value != axisLabelText) {
					SendNotification(new ElementWillChangeNotification(this));
					axisLabelText = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StripCheckedInLegend"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty]
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
	DevExpressXtraChartsLocalizedDescription("StripCheckableInLegend"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty]
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
		public Strip(string name)
			: base(name) {
			minLimit = new MinStripLimit(this);
			maxLimit = new MaxStripLimit(this);
			minLimit.SetMaxLimit(maxLimit);
			maxLimit.SetMinLimit(minLimit);
			fillStyle = new RectangleFillStyle(this);
		}
		public Strip()
			: this(String.Empty) {
		}
		public Strip(string name, double minValue, double maxValue)
			: this(name, (object)minValue, (object)maxValue) {
		}
		public Strip(string name, string minValue, string maxValue)
			: this(name, (object)minValue, (object)maxValue) {
		}
		public Strip(string name, DateTime minValue, DateTime maxValue)
			: this(name, (object)minValue, (object)maxValue) {
		}
		public Strip(string name, object minValue, object maxValue)
			: this(name) {
			SetAxisValues(minValue, maxValue);
		}
		#region IStrip implementation
		IStripLimit IStrip.MinLimit { get { return minLimit; } }
		IStripLimit IStrip.MaxLimit { get { return maxLimit; } }
		string IStrip.AxisLabelText {
			get {
				if (ShowAxisLabel)
					return String.IsNullOrEmpty(axisLabelText) ? Name : axisLabelText;
				return String.Empty;
			}
		}
		void IStrip.CorrectLimits() {
			CorrectLimits(Axis);
		}
		#endregion
		#region ICheckableLegendItem implementation
		bool ILegendItemData.DisposeFont { get { return false; } }
		bool ILegendItemData.DisposeMarkerImage { get { return false; } }
		bool ILegendItemData.MarkerVisible { get { return Legend.MarkerVisible; } }
		bool ILegendItemData.TextVisible { get { return Legend.TextVisible; } }
		bool ICheckableLegendItemData.Disabled { get { return false; } }
		Color ILegendItemData.TextColor { get { return Legend.ActualTextColor; } }
		Color ICheckableLegendItemData.MainColor { get { return Color.FromArgb(255, ActualColor.R, ActualColor.G, ActualColor.B); } }
		Image ILegendItemData.MarkerImage { get { return null; } }
		ChartImageSizeMode ILegendItemData.MarkerImageSizeMode { get { return ChartImageSizeMode.AutoSize; } }
		Font ILegendItemData.Font { get { return Legend.Font; } }
		Size ILegendItemData.MarkerSize { get { return Legend.MarkerSize; } }
		string ILegendItemData.Text { get { return String.IsNullOrEmpty(legendText) ? Name : legendText; } }
		object ILegendItemData.RepresentedObject { get { return this; } }
		bool ICheckableLegendItemData.UseCheckBox{ 
			get{ return Legend.UseCheckBoxes && checkableInLegend;}
		}
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) {
			bounds.Inflate(-1, -1);
			renderer.FillRectangle(bounds, ActualColor, ActualFillStyle);
		}
		#endregion
		#region ISupportInitialize implemantation
		void ISupportInitialize.BeginInit() {
			this.loading = true;
		}
		void ISupportInitialize.EndInit() {
			this.loading = false;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeMinLimit() {
			return minLimit.ShouldSerialize();
		}
		bool ShouldSerializeMaxLimit() {
			return maxLimit.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
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
			return !String.IsNullOrEmpty(legendText);
		}
		void ResetLegendText() {
			LegendText = string.Empty;
		}
		bool ShouldSerializeShowAxisLabel() {
			return showAxisLabel != DefaultShowAxisLabel;
		}
		void ResetShowAxisLabel() {
			ShowAxisLabel = DefaultShowAxisLabel;
		}
		bool ShouldSerializeAxisLabelText() {
			return !String.IsNullOrEmpty(axisLabelText);
		}
		void ResetAxisLabelText() {
			AxisLabelText = string.Empty;
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
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "MinLimit":
					return ShouldSerializeMinLimit();
				case "MaxLimit":
					return ShouldSerializeMaxLimit();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "Visible":
					return ShouldSerializeVisible();
				case "Color":
					return ShouldSerializeColor();
				case "ShowInLegend":
					return ShouldSerializeShowInLegend();
				case "LegendText":
					return ShouldSerializeLegendText();
				case "ShowAxisLabel":
					return ShouldSerializeShowAxisLabel();
				case "AxisLabelText":
					return ShouldSerializeAxisLabelText();
				case "CheckedInLegend":
					return ShouldSerializeCheckedInLegend();
				case "CheckableInLegend":
					return ShouldSerializeCheckableInLegend();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
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
		#endregion
		void SetAxisValues(object minAxisValue, object maxAxisValue) {
			if ((minAxisValue is double) && (maxAxisValue is double)) {
				double minValue = (double)minAxisValue;
				double maxValue = (double)maxAxisValue;
				if (minValue < maxValue) {
					((IAxisValueContainer)minLimit).SetValue(minValue);
					((IAxisValueContainer)maxLimit).SetValue(maxValue);
				} else {
					minAxisValue = ((IAxisValueContainer)minLimit).GetValue();
					maxAxisValue = ((IAxisValueContainer)maxLimit).GetValue();
				}
			} else if ((minAxisValue is string) && (maxAxisValue is string)) {
				string minValue = minAxisValue.ToString();
				string maxValue = maxAxisValue.ToString();
				if (minValue.CompareTo(maxValue) == 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectStripConstructorParameters));
			} else if ((maxAxisValue is DateTime) && (maxAxisValue is DateTime)) {
				DateTime minValue = (DateTime)minAxisValue;
				DateTime maxValue = (DateTime)maxAxisValue;
				if (minValue.CompareTo(maxValue) >= 0) {
					minAxisValue = DateTime.Now;
					maxAxisValue = DateTime.Now.AddDays(1);
				}
			} else
				throw new ArgumentException("Types mismatch.");
			((IAxisValueContainer)minLimit).SetAxisValue(minAxisValue);
			((IAxisValueContainer)maxLimit).SetAxisValue(maxAxisValue);
			minLimit.SyncAxisValueAndEnabled();
			maxLimit.SyncAxisValueAndEnabled();
		}
		void SwapLimits() {
			object tempValue = MinLimit.AxisValue;
			((IAxisValueContainer)MinLimit).SetAxisValue(MaxLimit.AxisValue);
			((IAxisValueContainer)MaxLimit).SetAxisValue(tempValue);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed)
				fillStyle.Dispose();
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new Strip();
		}
		internal void CorrectLimits(Axis2D axis) {
			if (axis == null || !MinLimit.Enabled || !MaxLimit.Enabled || MinLimit.AxisValue == null || MaxLimit.AxisValue == null)
				return;
			switch (axis.ScaleType) {
				case ActualScaleType.Numerical:
					if (!(MinLimit.AxisValue is double) || !(MaxLimit.AxisValue is double))
						return;
					if ((double)MinLimit.AxisValue > (double)MaxLimit.AxisValue)
						SwapLimits();
					break;
				case ActualScaleType.DateTime:
					if (!(MinLimit.AxisValue is DateTime) || !(MaxLimit.AxisValue is DateTime))
						return;
					if ((DateTime)MaxLimit.AxisValue < (DateTime)MinLimit.AxisValue)
						SwapLimits();
					break;
				default:
					string minLimit = MinLimit.AxisValue.ToString();
					string maxLimit = MaxLimit.AxisValue.ToString();
					double minValue = axis.ScaleTypeMap.NativeToInternal(minLimit);
					double maxValue = axis.ScaleTypeMap.NativeToInternal(maxLimit);
					if (double.IsNaN(minValue)|| double.IsNaN(maxValue))
						return;
					if (minValue > maxValue) {
						((IAxisValueContainer)MinLimit).SetAxisValue(maxLimit);
						((IAxisValueContainer)MaxLimit).SetAxisValue(minLimit);
					}
					break;
			}
		}	 
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Strip strip = obj as Strip;
			if (strip != null) {
				minLimit.Assign(strip.minLimit);
				maxLimit.Assign(strip.maxLimit);
				visible = strip.visible;
				color = strip.color;
				fillStyle.Assign(strip.fillStyle);
				legendText = strip.legendText;
				showInLegend = strip.showInLegend;
				axisLabelText = strip.axisLabelText;
				showAxisLabel = strip.showAxisLabel;
				checkedInLegend = strip.checkedInLegend;
				checkableInLegend = strip.checkableInLegend;
			}
		}
	}
}
