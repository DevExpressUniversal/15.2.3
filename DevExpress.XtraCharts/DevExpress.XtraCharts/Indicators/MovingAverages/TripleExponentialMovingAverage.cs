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
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(MovingAverageTypeConverter))
	]
	public class TripleExponentialMovingAverageTema : MovingAverage { 
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndTripleExponentialMovingAverageTema); } }
		public TripleExponentialMovingAverageTema(string name, ValueLevel valueLevel) 
			: base(name, valueLevel) { }
		public TripleExponentialMovingAverageTema(string name) 
			: base(name) { }
		public TripleExponentialMovingAverageTema() 
			: this(String.Empty) { }
		protected override IndicatorBehavior CreateBehavior() {
			return new TripleExponentialMovingAverageTemaBehavior(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new TripleExponentialMovingAverageTema();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class TripleExponentialMovingAverageTemaBehavior : MovingAverageIndicatorBehavior {
		public TripleExponentialMovingAverageTemaBehavior(TripleExponentialMovingAverageTema exponentialMovingAverage)
			: base(exponentialMovingAverage) { }
		protected override void CalculateInternal(IRefinedSeries refinedSeries) {
			Visible = false;
			SubsetBasedIndicator indicator = (SubsetBasedIndicator)Indicator;
			if (indicator.Visible) {
				TripleExponentialMovingAverageCalculator calculator = new TripleExponentialMovingAverageCalculator();
				List<GRealPoint2D> movingAverageData = calculator.CalculateMovingAverageData(refinedSeries.Points, indicator.PointsCount, (ValueLevelInternal)indicator.ValueLevel);
				SetMovingAverageData(movingAverageData);
				Visible = calculator.MovingAverageDataCalculated;
			}
		}
	}
}
