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
using System.Linq;
using System.Text;
namespace DevExpress.XtraCharts {
	public class ChartRangeControlClientNumericOptions {
		readonly ChartRangeControlClientNumericGridOptions rangeControlNumericGridOptions;
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.RangeControlNumericGridOptions"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DRangeControlNumericGridOptions")
#else
	Description("")
#endif
		]
		public ChartRangeControlClientNumericGridOptions RangeControlNumericGridOptions { get { return rangeControlNumericGridOptions; } }
		public ChartRangeControlClientNumericOptions(ChartRangeControlClientNumericGridOptions rangeControlNumericGridOptions) {
			this.rangeControlNumericGridOptions = rangeControlNumericGridOptions;
		}
		public override string ToString() {
			return String.Format("({0})", GetType().Name);
		}
	}
	public class ChartRangeControlClientDateTimeOptions {
		readonly ChartRangeControlClientDateTimeGridOptions rangeControlDateTimeGridOptions;
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2D.RangeControlDateTimeGridOptions"),
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DRangeControlDateTimeGridOptions"),
#endif
		]
		public ChartRangeControlClientDateTimeGridOptions RangeControlDateTimeGridOptions { get { return rangeControlDateTimeGridOptions; } }
		public ChartRangeControlClientDateTimeOptions(ChartRangeControlClientDateTimeGridOptions rangeControlDateTimeGridOptions) {
			this.rangeControlDateTimeGridOptions = rangeControlDateTimeGridOptions;
		}
		public override string ToString() {
			return String.Format("({0})", GetType().Name);
		}
	}
}
