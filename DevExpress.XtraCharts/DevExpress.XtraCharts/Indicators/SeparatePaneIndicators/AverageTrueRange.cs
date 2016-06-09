﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public class AverageTrueRange : SeparatePaneIndicator {
		const int DefaultPointsCount = 14;  
		int pointsCount = DefaultPointsCount;
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndAverageTrueRange); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AverageTrueRangePointsCount"),
#endif
		 DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AverageTrueRange.PointsCount"),
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
		public AverageTrueRange()
			: this(string.Empty) { }
		public AverageTrueRange(string name)
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
			return new AverageTrueRange();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new AverageTrueRangeBehavior(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			var indicator = obj as AverageTrueRange;
			if (indicator != null)
				this.pointsCount = indicator.pointsCount;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class AverageTrueRangeBehavior : SeparatePaneIndicatorBehavior {
		double minY, maxY;
		LineStrip atrLineStrip;
		AverageTrueRange AverageTrueRange {
			get { return (AverageTrueRange)Indicator; }
		}
		protected override MinMaxValues MinMaxYByWholeXRange {
			get { return new MinMaxValues(minY, maxY); }
		}
		public AverageTrueRangeBehavior(AverageTrueRange indicator)
			: base(indicator) {
		}
		protected override void Calculate(IRefinedSeries seriesInfo) {
			Visible = false;
			var calculator = new AverageTrueRangeCalculator();
			List<GRealPoint2D> indicatorPoints = calculator.Calculate(seriesInfo, AverageTrueRange.PointsCount);
			this.atrLineStrip = new LineStrip(indicatorPoints);
			this.minY = calculator.MinY;
			this.maxY = calculator.MaxY;
			Visible = calculator.Calculated;
		}
		public override MinMaxValues GetFilteredMinMaxY(IMinMaxValues visualRangeOfOtherAxisForFiltering) {
			return GetFilteredMinMaxY(atrLineStrip, visualRangeOfOtherAxisForFiltering);
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || atrLineStrip == null)
				return null;
			LineStrip screenPolyline = StripsUtils.MapStrip(diagramMapping, this.atrLineStrip); 
			return new MultilineIndicatorLayout(AverageTrueRange, screenPolyline);
		}
	}
}
