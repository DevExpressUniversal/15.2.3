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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(IndicatorTypeConverter))]
	public class DetrendedPriceOscillator : SeparatePaneIndicator {
		const ValueLevel DefaultValueLevel = ValueLevel.Close;
		const int DefaultPointsCount = 20;
		ValueLevel valueLevel = DefaultValueLevel;
		int pointsCount = DefaultPointsCount;
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndDetrendedPriceOscillator); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DetrendedPriceOscillatorPointsCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DetrendedPriceOscillator.PointsCount"),
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
	DevExpressXtraChartsLocalizedDescription("DetrendedPriceOscillatorValueLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DetrendedPriceOscillator.ValueLevel"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
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
		public DetrendedPriceOscillator() : this(string.Empty) { }
		public DetrendedPriceOscillator(string name) : base(name) { }
		public DetrendedPriceOscillator(string name, ValueLevel valueLevel) : base(name) {
			this.valueLevel = valueLevel;
		}
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
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "PointsCount":
					return ShouldSerializePointsCount();
				case "ValueLevel":
					return ShouldSerializeValueLevel();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new DetrendedPriceOscillator();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new DetrendedPriceOscillatorBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var dpo = obj as DetrendedPriceOscillator;
			if (dpo != null) {
				this.pointsCount = dpo.pointsCount;
				this.valueLevel = dpo.valueLevel;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class DetrendedPriceOscillatorBehavior : SeparatePaneIndicatorBehavior {
		double minY, maxY;
		LineStrip dpoLineStrip;
		DetrendedPriceOscillator DetrendedPriceOscillator {
			get { return (DetrendedPriceOscillator)Indicator; }
		}
		protected override MinMaxValues MinMaxYByWholeXRange {
			get { return new MinMaxValues(minY, maxY); }
		}
		public DetrendedPriceOscillatorBehavior(DetrendedPriceOscillator indicator)
			: base(indicator) { }
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new DetrendedPriceOscillatorCalculator();
			List<GRealPoint2D> indicatorPoints = calculator.Calculate(seriesInfo, DetrendedPriceOscillator.PointsCount, (ValueLevelInternal)DetrendedPriceOscillator.ValueLevel);
			this.dpoLineStrip = new LineStrip(indicatorPoints);
			this.minY = calculator.MinY;
			this.maxY = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(dpoLineStrip, visualRangeOfOtherAxisForFiltering);
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || dpoLineStrip == null)
				return null;
			LineStrip screenPolyline = StripsUtils.MapStrip(diagramMapping, this.dpoLineStrip);
			return new MultilineIndicatorLayout(DetrendedPriceOscillator, screenPolyline);
		}
	}
}
