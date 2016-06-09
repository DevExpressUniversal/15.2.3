﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Web;
namespace DevExpress.ExpressApp.Chart.Web {
	class WebChartControlWithEmptyArguments : WebChartControl {
		public WebChartControlWithEmptyArguments() {
			UnregisterSummaryFunction("COUNT");
			UnregisterSummaryFunction("SUM");
			UnregisterSummaryFunction("MIN");
			UnregisterSummaryFunction("MAX");
			UnregisterSummaryFunction("AVERAGE");
			SummaryFunctionArgumentDescription[] argumentDescriptions = new SummaryFunctionArgumentDescription[] { new SummaryFunctionArgumentDescription("argument") };
			SummaryFunctionArgumentDescription[] sumArgumentDescriptions = new SummaryFunctionArgumentDescription[] { new SummaryFunctionArgumentDescription("argument", ScaleType.Numerical) };
			RegisterSummaryFunction("MIN", "MIN", 1, argumentDescriptions, ChartControlWithEmptyArgumentsHelper.CalcMin);
			RegisterSummaryFunction("MAX", "MAX", 1, argumentDescriptions, ChartControlWithEmptyArgumentsHelper.CalcMax);
			RegisterSummaryFunction("SUM", "SUM", ScaleType.Numerical, 1, sumArgumentDescriptions, ChartControlWithEmptyArgumentsHelper.CalcSum);
			RegisterSummaryFunction("AVERAGE", "AVERAGE", ScaleType.Numerical, 1, sumArgumentDescriptions, ChartControlWithEmptyArgumentsHelper.CalcAverage);
			RegisterSummaryFunction("COUNT", "COUNT", ScaleType.Numerical, 1, null, ChartControlWithEmptyArgumentsHelper.CalcCount);
		}
	}
#if DebugTest
	public static class DebugTest_WebChartControlWithEmptyArgumentsCreator {
		public static WebChartControl Create() {
			return new WebChartControlWithEmptyArguments();
		}
	}
#endif
}
