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
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class ChartPrintLayoutCalculatorBase {
		public abstract Size CalculateChartRenderSize(Size chartSize, Size pageSize);
		public abstract Size CalculateChartImageSize(Size chartSize, Size pageSize);
		public Size CalculateActualChartSize(ChartControl chart) {
			double width = double.IsNaN(chart.Width) ? chart.ActualWidth : chart.Width;
			double height = double.IsNaN(chart.Height) ? chart.ActualHeight : chart.Height;
			return new Size(width, height);
		}
	}
	public class DefaultChartPrintLayoutCalculator : ChartPrintLayoutCalculatorBase {
		public override Size CalculateChartImageSize(Size chartSize, Size pageSize) {
			return chartSize;
		}
		public override Size CalculateChartRenderSize(Size chartSize, Size pageSize) {
			return chartSize;
		}
	}
	public class StretchChartPrintLayoutCalculator : ChartPrintLayoutCalculatorBase {
		public override Size CalculateChartImageSize(Size chartSize, Size pageSize) {
			return pageSize;
		}
		public override Size CalculateChartRenderSize(Size chartSize, Size pageSize) {
			return pageSize;
		}
	}
	public abstract class ZoomChartPrintLayoutCalculator : ChartPrintLayoutCalculatorBase {
		protected Size CalculateChartFinalSize(Size chartSize, Size usablePageSize) {
			if (chartSize.Width == 0.0 || chartSize.Height == 0.0) {
				double minSize = Math.Min(usablePageSize.Width, usablePageSize.Height);
				return new Size(minSize, minSize);
			}
			double widthRatio = usablePageSize.Width / chartSize.Width;
			double heightRatio = usablePageSize.Height / chartSize.Height;
			double minRatio = Math.Min(widthRatio, heightRatio);
			return new Size(chartSize.Width * minRatio, chartSize.Height * minRatio);
		}
		public override Size CalculateChartImageSize(Size chartSize, Size pageSize) {
			return CalculateChartFinalSize(chartSize, pageSize);
		}
	}
	public class NonProportionalZoomChartPrintLayoutCalculator : ZoomChartPrintLayoutCalculator {
		public override Size CalculateChartRenderSize(Size chartSize, Size pageSize) {
			return CalculateChartFinalSize(chartSize, pageSize);
		}
	}
	public class ProportionalZoomChartPrintLayoutCalculator : ZoomChartPrintLayoutCalculator {
		public override Size CalculateChartRenderSize(Size chartSize, Size pageSize) {
			return chartSize;
		}
	}
}
