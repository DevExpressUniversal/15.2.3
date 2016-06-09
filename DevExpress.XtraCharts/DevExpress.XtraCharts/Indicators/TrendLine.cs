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
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(TrendLineTypeConverter))
	]
	public class TrendLine : FinancialIndicator {
		const bool DefaultExtrapolateToInfinity = true;
		bool extrapolateToInfinity = DefaultExtrapolateToInfinity;
		public override string IndicatorName {
			get { return ChartLocalizer.GetString(ChartStringId.IndTrendLine); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TrendLineExtrapolateToInfinity"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TrendLine.ExtrapolateToInfinity"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category("Behavior"),
		XtraSerializableProperty
		]
		public bool ExtrapolateToInfinity {
			get { return extrapolateToInfinity; }
			set {
				if (value != extrapolateToInfinity) {
					SendNotification(new ElementWillChangeNotification(this));
					extrapolateToInfinity = value;
					RaiseControlChanged();
				}
			}
		}
		public TrendLine(string name) : base(name) {
		}
		public TrendLine() : this(String.Empty) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeExtrapolateToInfinity() {
			return extrapolateToInfinity != DefaultExtrapolateToInfinity;
		}
		void ResetExtrapolateToInfinity() {
			ExtrapolateToInfinity = DefaultExtrapolateToInfinity;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "ExtrapolateToInfinity" ? ShouldSerializeExtrapolateToInfinity() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new TrendLine();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new TrendLineBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TrendLine trendLine = obj as TrendLine;
			if (trendLine != null)
				extrapolateToInfinity = trendLine.extrapolateToInfinity;
		}		
	}
}
namespace DevExpress.XtraCharts.Native {
	public class TrendLineBehavior : IndicatorBehavior {
		TrendLineCalculator calculator = new TrendLineCalculator();
		GRealLine2D indicatorLine;
		TrendLine TrendLine { get { return (TrendLine)Indicator; } }
		public TrendLineBehavior(TrendLine trendLine) : base(trendLine) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			FinancialIndicator financialIndicator = Indicator as FinancialIndicator;
			if (financialIndicator != null && financialIndicator.Visible) {
				FinancialIndicatorPoint point1 = financialIndicator.Point1;
				FinancialIndicatorPoint point2 = financialIndicator.Point2;
				AxisScaleTypeMap map = ((IAxisData)financialIndicator.View.ActualAxisX).AxisScaleTypeMap;
				indicatorLine = calculator.Calculate(seriesInfo, map, CultureInfo.InvariantCulture,
					point1.Argument, (ValueLevelInternal)point1.ValueLevel,
					point2.Argument, (ValueLevelInternal)point2.ValueLevel,
					TrendLine.ExtrapolateToInfinity, seriesInfo.MaxArgument);
				Visible = calculator.Calculated;
			}
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible)
				return null;
			GRealLine2D? truncated = TruncatedLineCalculator.Truncate(diagramMapping.GetScreenPointNoRound(indicatorLine.Start.X, indicatorLine.Start.Y),
				diagramMapping.GetScreenPointNoRound(indicatorLine.End.X, indicatorLine.End.Y),
				diagramMapping.Bounds, TrendLine.ExtrapolateToInfinity ? LineKind.Ray : LineKind.Segment);
			return truncated.HasValue ? new LineIndicatorLayout(TrendLine, truncated.Value) : null;
		}
	}
}
