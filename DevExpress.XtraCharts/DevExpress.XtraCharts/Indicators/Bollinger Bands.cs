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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(IndicatorTypeConverter))]
	public class BollingerBands : Indicator, IAffectsAxisRange {
		const ValueLevel DefaultValueLevel = ValueLevel.Close;
		const int DefaultPointsCount = 20;
		const double DefaultStandardDeviationMultiplier = 2.0;
		static readonly Color DefaultBandsColor = Color.Empty;
		ValueLevel valueLevel = DefaultValueLevel;
		int pointsCount = DefaultPointsCount;
		Color bandsColor = DefaultBandsColor;
		double standardDeviationMultiplier = DefaultStandardDeviationMultiplier;
		LineStyle bandsLineStyle;
		BollingerBandsBehavior BollingerBandsBehavior {
			get { return (BollingerBandsBehavior)base.IndicatorBehavior; }
		}
		public override string IndicatorName {
			get { return ChartLocalizer.GetString(ChartStringId.IndBollingerBands); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BollingerBandsPointsCount"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BollingerBands.PointsCount"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int PointsCount {
			get { return pointsCount; }
			set {
				if (value != pointsCount) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					pointsCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BollingerBandsValueLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BollingerBands.ValueLevel"),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public ValueLevel ValueLevel {
			get { return valueLevel; }
			set {
				if (value != valueLevel) {
					if (!Loading) {
						XYDiagram2DSeriesViewBase view = View;
						if (view != null)
							view.CheckValueLevel(value);
					}
					SendNotification(new ElementWillChangeNotification(this));
					valueLevel = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BollingerBandsBandsColor"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BollingerBands.BandsColor"),
		 Category(Categories.Appearance),
		 XtraSerializableProperty]
		public Color BandsColor {
			get { return bandsColor; }
			set {
				if (value != bandsColor) {
					SendNotification(new ElementWillChangeNotification(this));
					bandsColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BollingerBandsBandsLineStyle"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BollingerBands.BandsLineStyle"),
		 TypeConverter(typeof(ExpandableObjectConverter)),
		 Category(Categories.Appearance),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		 NestedTagProperty,
		 XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public LineStyle BandsLineStyle { get { return bandsLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BollingerBandsStandardDeviationMultiplier"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BollingerBands.StandardDeviationMultiplier"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public double StandardDeviationMultiplier {
			get { return standardDeviationMultiplier; }
			set {
				if (value != pointsCount) {
					if (value <= 0 || double.IsNaN(value) || double.IsInfinity(value))
						throw new ArgumentException("The StandardDeviationMultiplier property value should be greater than 0."); 
					SendNotification(new ElementWillChangeNotification(this));
					standardDeviationMultiplier = value;
					RaiseControlChanged();
				}
			}
		}
		public BollingerBands() : this(string.Empty) { }
		public BollingerBands(string name) : this(name, DefaultValueLevel) { }
		public BollingerBands(string name, ValueLevel valueLevel) : base(name) {
			this.valueLevel = valueLevel;
			this.bandsLineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
		}
		#region IAffectsAxisRange
		MinMaxValues IAffectsAxisRange.GetMinMaxValues(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return BollingerBandsBehavior.GetFilteredMinMaxY(visualRangeOfOtherAxisForFiltering);
		}
		IAxisData IAffectsAxisRange.AxisYData { get { return ((XYDiagramSeriesViewBase)this.Owner).ActualAxisY; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePointsCount() {
			return this.pointsCount != DefaultPointsCount;
		}
		void ResetPointsCount() {
			PointsCount = DefaultPointsCount;
		}
		bool ShouldSerializeValueLevel() {
			return valueLevel != DefaultValueLevel;
		}
		void ResetValueLevel() {
			ValueLevel = DefaultValueLevel;
		}
		bool ShouldSerializeBandsColor() {
			return bandsColor != DefaultBandsColor;
		}
		void ResetBandsColor() {
			BandsColor = DefaultBandsColor;
		}
		bool ShouldSerializeBandsLineStyle() {
			return bandsLineStyle.ShouldSerialize();
		}
		bool ShouldSerializeStandardDeviationMultiplier() {
			return this.standardDeviationMultiplier != DefaultStandardDeviationMultiplier;
		}
		void ResetStandardDeviationMultiplier() {
			StandardDeviationMultiplier = DefaultStandardDeviationMultiplier;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "PointsCount":
					return ShouldSerializePointsCount();
				case "ValueLevel":
					return ShouldSerializeValueLevel();
				case "BandsColor":
					return ShouldSerializeBandsColor();
				case "BandsLineStyle":
					return ShouldSerializeBandsLineStyle();
				case "StandardDeviationMultiplier":
					return ShouldSerializeStandardDeviationMultiplier();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new BollingerBands();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new BollingerBandsBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var bb = obj as BollingerBands;
			if (bb != null) {
				this.bandsColor = bb.bandsColor;
				this.bandsLineStyle.Assign(bb.bandsLineStyle);
				this.pointsCount = bb.pointsCount;
				this.standardDeviationMultiplier = bb.standardDeviationMultiplier;
				this.valueLevel = bb.valueLevel;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class BollingerBandsBehavior : IndicatorBehavior {
		LineStrip movingAverageStrip;
		LineStrip upperBandStrip;
		LineStrip lowerBandStrip;
		double minYByWholeXRange;
		double maxYByWholeXRange;
		BollingerBands BollingerBands { get { return (BollingerBands)Indicator; } }
		Color BandsColor {
			get {
				Color signalLineColor = BollingerBands.BandsColor;
				return signalLineColor.IsEmpty ? Color : signalLineColor;
			}
		}
		public BollingerBandsBehavior(BollingerBands indicator)
			: base(indicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new BollingerBandsCalculator();
			calculator.Calculate(seriesInfo, BollingerBands.PointsCount, BollingerBands.StandardDeviationMultiplier, (ValueLevelInternal)BollingerBands.ValueLevel);
			this.movingAverageStrip = new LineStrip(calculator.MovingAveragePoints);
			this.upperBandStrip = new LineStrip(calculator.UpperBandPoints);
			this.lowerBandStrip = new LineStrip(calculator.LowerBandPoints);
			this.minYByWholeXRange = calculator.MinY;
			this.maxYByWholeXRange = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || movingAverageStrip == null || upperBandStrip == null || lowerBandStrip == null)
				return null;
			List<LineStripItem> lineStrips = new List<LineStripItem>();
			LineStrip screenMovingAverageLineStrip = StripsUtils.MapStrip(diagramMapping, this.movingAverageStrip);
			Color macdColor = GraphicUtils.CorrectColorByHitTestState(Color, ((IHitTest)BollingerBands).State);
			LineStyle movingAverageLineStyle = BollingerBands.LineStyle;
			int macdThickness = GraphicUtils.CorrectThicknessByHitTestState(movingAverageLineStyle.Thickness, ((IHitTest)BollingerBands).State, 1);
			lineStrips.Add(new LineStripItem(screenMovingAverageLineStrip, macdColor, movingAverageLineStyle, macdThickness));
			LineStrip screenUpperBandLineStrip = StripsUtils.MapStrip(diagramMapping, this.upperBandStrip);
			Color bandsColor = GraphicUtils.CorrectColorByHitTestState(BandsColor, ((IHitTest)BollingerBands).State);
			LineStyle bandsLineStyle = BollingerBands.BandsLineStyle;
			int bandThickness = GraphicUtils.CorrectThicknessByHitTestState(bandsLineStyle.Thickness, ((IHitTest)BollingerBands).State, 1);
			lineStrips.Add(new LineStripItem(screenUpperBandLineStrip, bandsColor, bandsLineStyle, bandThickness));
			LineStrip screenLowerBandLineStrip = StripsUtils.MapStrip(diagramMapping, this.lowerBandStrip);
			lineStrips.Add(new LineStripItem(screenLowerBandLineStrip, bandsColor, bandsLineStyle, bandThickness));
			return new LineStripsIndicatorLayout(BollingerBands, lineStrips);
		}
		public MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			bool isRangeEmpty = double.IsNaN(visualRangeOfOtherAxisForFiltering.Delta);
			if (isRangeEmpty)
				return new MinMaxValues(this.minYByWholeXRange, this.maxYByWholeXRange);
			if (upperBandStrip == null || lowerBandStrip == null)
				return MinMaxValues.Empty;
			int minIndex = -1;
			ChartDebug.Assert(upperBandStrip.Count == lowerBandStrip.Count);
			for (int i = 0; i < upperBandStrip.Count; i++) {
				if (upperBandStrip[i].X >= visualRangeOfOtherAxisForFiltering.Min) {
					minIndex = i;
					break;
				}
			}
			int maxIndex = -1;
			for (int i = upperBandStrip.Count-1; i > -1; i--) {
				if (upperBandStrip[i].X <= visualRangeOfOtherAxisForFiltering.Max) {
					maxIndex = i;
					break;
				}
			}
			if (minIndex == -1 || maxIndex == -1)
				return MinMaxValues.Empty;
			double minValue = lowerBandStrip[minIndex].Y;
			double maxValue = upperBandStrip[minIndex].Y;
			for (int i = minIndex + 1; i <= maxIndex; i++) {
				if (lowerBandStrip[i].Y < minValue)
					minValue = lowerBandStrip[i].Y;
				if (upperBandStrip[i].Y > maxValue)
					maxValue = upperBandStrip[i].Y;
			}
			return new MinMaxValues(minValue, maxValue);
		}
	}
}
