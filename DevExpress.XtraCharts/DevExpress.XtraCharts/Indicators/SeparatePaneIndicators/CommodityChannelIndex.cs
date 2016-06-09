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
	public class CommodityChannelIndex : SeparatePaneIndicator {
		const int DefaultPointsCount = 14;
		int pointsCount = DefaultPointsCount;
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndCommodityChannelIndex); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("CommodityChannelIndexPointsCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.CommodityChannelIndex.PointsCount"),
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
		public CommodityChannelIndex() 
			: this(string.Empty) { }
		public CommodityChannelIndex(string name) 
			: base(name) { }
		#region ShouldSerialize & Reset
		bool ShouldSerializePointsCount() {
			return this.pointsCount != DefaultPointsCount;
		}
		void ResetPointsCount() {
			PointsCount = DefaultPointsCount;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "PointsCount" ? ShouldSerializePointsCount() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new CommodityChannelIndex();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new CommodityChannelIndexBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var cci = obj as CommodityChannelIndex;
			if (cci != null)
				this.pointsCount = cci.pointsCount;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class CommodityChannelIndexBehavior : SeparatePaneIndicatorBehavior {
		double minValue, maxValue;
		LineStrip cciLineStrip;
		CommodityChannelIndex CommodityChannelIndex {
			get { return (CommodityChannelIndex)Indicator; }
		}
		protected override MinMaxValues MinMaxYByWholeXRange {
			get { return new MinMaxValues(minValue, maxValue); }
		}
		public CommodityChannelIndexBehavior(CommodityChannelIndex indicator)
			: base(indicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new CommodityChannelIndexCalculator();
			List<GRealPoint2D> indicatorPoints = calculator.Calculate(seriesInfo, CommodityChannelIndex.PointsCount);
			this.cciLineStrip = new LineStrip(indicatorPoints);
			this.minValue = calculator.MinY;
			this.maxValue = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(cciLineStrip, visualRangeOfOtherAxisForFiltering);
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || cciLineStrip == null)
				return null;
			LineStrip screenPolyLine = StripsUtils.MapStrip(diagramMapping, this.cciLineStrip);
			return new MultilineIndicatorLayout(CommodityChannelIndex, screenPolyLine);
		}
	}
}
