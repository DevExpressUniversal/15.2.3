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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public static class DefaultAlternateTextBuilder {
		public static string BuildTextForChart(Chart chart) {
			string titleText = String.Empty;
			string seriesText = String.Empty;
			ChartTitleCollection titles = chart.Titles;
			if (titles.Count > 0) {
				string[] lines = titles[0].Lines;
				if (lines.Length > 0)
					foreach (string line in lines)
						if (!String.IsNullOrEmpty(line)) {
							titleText = " " + line;
							break;
						}
			}
			if (chart.ViewController.ActiveRefinedSeries.Count > 0) {
				string seriesPattern = ChartLocalizer.GetString(ChartStringId.AlternateTextSeriesText);
				seriesText = String.Format(seriesPattern, chart.ViewController.ActiveRefinedSeries[0].Series);
				for (int i = 1; i < chart.ViewController.ActiveRefinedSeries.Count; i++)
					seriesText += ", " + String.Format(seriesPattern, chart.ViewController.ActiveRefinedSeries[i].Series);
				seriesText = String.Format(ChartLocalizer.GetString(ChartStringId.AlternateTextSeriesPlaceholder), seriesText);
			}
			return String.Format(ChartLocalizer.GetString(ChartStringId.AlternateTextPlaceholder), titleText, seriesText);
		}
	}
}
