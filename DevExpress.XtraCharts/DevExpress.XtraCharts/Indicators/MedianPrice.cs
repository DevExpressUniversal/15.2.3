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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(IndicatorTypeConverter))
	]
	public class MedianPrice : Indicator {
		public override string IndicatorName { get { return ChartLocalizer.GetString(ChartStringId.IndMedianPrice); } }
		public MedianPrice(string name) : base(name) {
		}
		public MedianPrice() : this(string.Empty) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new MedianPrice();
		}
		protected override IndicatorBehavior CreateBehavior() {
			return new MedianPriceBehavior(this);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class MedianPriceBehavior : IndicatorBehavior {
		LineStrip medLineStrip;
		MedianPrice MedianPrice { get { return (MedianPrice)Indicator; } }
		public MedianPriceBehavior(MedianPrice indicator)
			: base(indicator) {
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible || medLineStrip == null)
				return null;
			LineStrip screenPolyline = StripsUtils.MapStrip(diagramMapping, this.medLineStrip);
			return new MultilineIndicatorLayout(MedianPrice, screenPolyline);
		}
		protected override void Calculate(IRefinedSeries refinedSeries) {
			Visible = false;
			var calculator = new MedianPriceCalculator();
			List<GRealPoint2D> indicatorPoints = calculator.Calculate(refinedSeries);
			this.medLineStrip = new LineStrip(indicatorPoints);
			Visible = calculator.Calculated;
		}
	}
}
