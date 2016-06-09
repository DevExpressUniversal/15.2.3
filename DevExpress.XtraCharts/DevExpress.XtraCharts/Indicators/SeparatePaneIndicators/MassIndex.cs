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
	public class MassIndex : SeparatePaneIndicator {
		const int DefaultMaPointsCount = 9;  
		const int DefaultSumPointsCount = 25;
		int maPointsCount = DefaultMaPointsCount;
		int sumPointsCount = DefaultSumPointsCount;
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndMassIndex); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MassIndexMovingAveragePointsCount"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MassIndex.MovingAveragePointsCount"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int MovingAveragePointsCount {
			get { return maPointsCount; }
			set {
				if (value != maPointsCount) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					maPointsCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MassIndexSumPointsCount"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MassIndex.SumPointsCount"),
		 Category(Categories.Behavior),
		 XtraSerializableProperty]
		public int SumPointsCount {
			get { return sumPointsCount; }
			set {
				if (value != sumPointsCount) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					sumPointsCount = value;
					RaiseControlChanged();
				}
			}
		}
		public MassIndex() : this(string.Empty) {
		}
		public MassIndex(string name) : base(name) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeMovingAveragePointsCount() {
			return this.maPointsCount != DefaultMaPointsCount;
		}
		void ResetMovingAveragePointsCount() {
			MovingAveragePointsCount = DefaultMaPointsCount;
		}
		bool ShouldSerializeSumPointsCount() {
			return this.sumPointsCount != DefaultSumPointsCount;
		}
		void ResetSumPointsCount() {
			SumPointsCount = DefaultSumPointsCount;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "MovingAveragePointsCount":
					return ShouldSerializeMovingAveragePointsCount();
				case "SumPointsCount":
					return ShouldSerializeSumPointsCount();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new MassIndex();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new MassIndexBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var mi = obj as MassIndex;
			if (mi != null) {
				this.maPointsCount = mi.maPointsCount;
				this.sumPointsCount = mi.sumPointsCount;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class MassIndexBehavior : SeparatePaneIndicatorBehavior {
		double minY, maxY;
		LineStrip massIndexLineStrip;
		MassIndex MassIndex {
			get { return (MassIndex)Indicator; }
		}
		protected override MinMaxValues MinMaxYByWholeXRange {
			get { return new MinMaxValues(minY, maxY); }
		}
		public MassIndexBehavior(MassIndex indicator)
			: base(indicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new MassIndexCalculator();
			List<GRealPoint2D> indicatorPoints = calculator.Calculate(seriesInfo, MassIndex.MovingAveragePointsCount, MassIndex.SumPointsCount);
			this.massIndexLineStrip = new LineStrip(indicatorPoints);
			this.minY = calculator.MinY;
			this.maxY = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(massIndexLineStrip, visualRangeOfOtherAxisForFiltering);
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || massIndexLineStrip == null)
				return null;
			LineStrip screenPolyline = StripsUtils.MapStrip(diagramMapping, this.massIndexLineStrip);
			return new MultilineIndicatorLayout(MassIndex, screenPolyline);
		}
	}
}
