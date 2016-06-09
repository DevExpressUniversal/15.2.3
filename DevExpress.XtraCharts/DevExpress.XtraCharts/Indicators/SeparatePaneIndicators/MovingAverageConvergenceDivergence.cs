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
	public class MovingAverageConvergenceDivergence : SeparatePaneIndicator {
		const int DefaultLongPeriod = 26;
		const int DefaultShortPeriod = 12;
		const int DefaultSignalSmoothingPeriod = 9;
		const ValueLevel DefaultValueLevel = ValueLevel.Close;
		static readonly Color DefaultSignalLineColor = Color.Empty;
		int shortPeriod = DefaultShortPeriod;
		int longPeriod = DefaultLongPeriod;
		int signalSmoothingPeriod = DefaultSignalSmoothingPeriod;
		Color signalLineColor = DefaultSignalLineColor;
		LineStyle signalLineStyle;
		ValueLevel valueLevel = DefaultValueLevel;
		public override string IndicatorName {
			get { return ChartLocalizer.GetString(ChartStringId.IndMovingAverageConvergenceDivergence); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageConvergenceDivergenceLongPeriod"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.LongPeriod"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int LongPeriod {
			get { return longPeriod; }
			set {
				if (value != longPeriod) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					longPeriod = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageConvergenceDivergenceShortPeriod"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.ShortPeriod"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int ShortPeriod {
			get { return shortPeriod; }
			set {
				if (value != shortPeriod) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					shortPeriod = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageConvergenceDivergenceSignalSmoothingPeriod"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.SignalSmoothingPeriod"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int SignalSmoothingPeriod {
			get { return signalSmoothingPeriod; }
			set {
				if (value != signalSmoothingPeriod) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					signalSmoothingPeriod = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageConvergenceDivergenceSignalLineColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.SignalLineColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty]
		public Color SignalLineColor {
			get { return signalLineColor; }
			set {
				if (value != signalLineColor) {
					SendNotification(new ElementWillChangeNotification(this));
					signalLineColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.SignalLineStile"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public LineStyle SignalLineStyle { get { return signalLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageConvergenceDivergenceValueLevel"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverageConvergenceDivergence.ValueLevel"),
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
		public MovingAverageConvergenceDivergence() 
			: this(string.Empty) { }
		public MovingAverageConvergenceDivergence(string name) 
			: base(name) {
			signalLineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
		}
		public MovingAverageConvergenceDivergence(string name, ValueLevel valueLevel)
			: base(name) {
			signalLineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
			this.valueLevel = valueLevel;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeLongPeriod() {
			return this.longPeriod != DefaultLongPeriod;
		}
		void ResetLongPeriod() {
			LongPeriod = DefaultLongPeriod;
		}
		bool ShouldSerializeShortPeriod() {
			return this.shortPeriod != DefaultShortPeriod;
		}
		void ResetShortPeriod() {
			ShortPeriod = DefaultShortPeriod;
		}
		bool ShouldSerializeSignalLineColor() {
			return signalLineColor != DefaultSignalLineColor;
		}
		void ResetSignalLineColor() {
			SignalLineColor = DefaultSignalLineColor;
		}
		bool ShouldSerializeSignalLineStyle() {
			return this.signalLineStyle.ShouldSerialize();
		}
		bool ShouldSerializeValueLevel() {
			return this.valueLevel != DefaultValueLevel;
		}
		void ResetValueLevel() {
			ValueLevel = DefaultValueLevel;
		}
		bool ShouldSerializeSignalSmoothingPeriod() {
			return this.signalSmoothingPeriod != DefaultSignalSmoothingPeriod;
		}
		void ResetSignalSmoothingPeriod() {
			SignalSmoothingPeriod = DefaultSignalSmoothingPeriod;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LongPeriod":
					return ShouldSerializeLongPeriod();
				case "ShortPeriod":
					return ShouldSerializeShortPeriod();
				case "SignalLineColor":
					return ShouldSerializeSignalLineColor();
				case "SignalLineStyle":
					return ShouldSerializeSignalLineStyle();
				case "ValueLevel":
					return ShouldSerializeValueLevel();
				case "SignalSmoothingPeriod":
					return ShouldSerializeSignalSmoothingPeriod();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new MovingAverageConvergenceDivergence();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new MovingAverageConvergenceDivergenceBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var macd = obj as MovingAverageConvergenceDivergence;
			if (macd != null) {
				this.longPeriod = macd.longPeriod;
				this.shortPeriod = macd.shortPeriod;
				this.signalLineColor = macd.signalLineColor;
				this.signalLineStyle.Assign(macd.signalLineStyle);
				this.signalSmoothingPeriod = macd.signalSmoothingPeriod;
				this.valueLevel = macd.valueLevel;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class MovingAverageConvergenceDivergenceBehavior : SeparatePaneIndicatorBehavior {
		double minY, maxY;
		LineStrip macdLineStrip, signalLineStrip;
		MovingAverageConvergenceDivergence Macd {
			get { return (MovingAverageConvergenceDivergence)Indicator; }
		}
		Color SignalLineColor {
			get {
				Color signalLineColor = Macd.SignalLineColor;
				return signalLineColor.IsEmpty ? Color : signalLineColor;
			}
		}
		protected override MinMaxValues MinMaxYByWholeXRange {
			get { return new MinMaxValues(minY, maxY); }
		}
		public MovingAverageConvergenceDivergenceBehavior(MovingAverageConvergenceDivergence indicator)
			: base(indicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new MovingAverageConvergenceDivergenceCalculator();
			calculator.Calculate(seriesInfo, Macd.ShortPeriod, Macd.LongPeriod, Macd.SignalSmoothingPeriod, (ValueLevelInternal)Macd.ValueLevel);
			this.macdLineStrip = new LineStrip(calculator.MacdPoints);
			this.signalLineStrip = new LineStrip(calculator.SignalPoints);
			this.minY = calculator.MinY;
			this.maxY = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(macdLineStrip, visualRangeOfOtherAxisForFiltering);
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || macdLineStrip == null || signalLineStrip == null)
				return null;
			List<LineStripItem> lineStrips = new List<LineStripItem>();
			LineStrip screenMacdLineStrip = StripsUtils.MapStrip(diagramMapping, this.macdLineStrip);
			Color macdColor = GraphicUtils.CorrectColorByHitTestState(Color, ((IHitTest)Macd).State);
			LineStyle macdLineStyle = Macd.LineStyle;
			int macdThickness = GraphicUtils.CorrectThicknessByHitTestState(macdLineStyle.Thickness, ((IHitTest)Macd).State, 1);
			lineStrips.Add(new LineStripItem(screenMacdLineStrip, macdColor, macdLineStyle, macdThickness));
			LineStrip screenSignalLineStrip = StripsUtils.MapStrip(diagramMapping, this.signalLineStrip);
			Color signalLineColor = GraphicUtils.CorrectColorByHitTestState(SignalLineColor, ((IHitTest)Macd).State);
			LineStyle signalLineStyle = Macd.SignalLineStyle;
			int signalThickness = GraphicUtils.CorrectThicknessByHitTestState(signalLineStyle.Thickness, ((IHitTest)Macd).State, 1);
			lineStrips.Add(new LineStripItem(screenSignalLineStrip, signalLineColor, signalLineStyle, signalThickness));
			return new LineStripsIndicatorLayout(Macd, lineStrips);
		}
	}
}
