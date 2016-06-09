#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraCharts.Native;
using System.IO;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.Linq;
namespace DevExpress.ExpressApp.PivotChart {
	public class ChartSettingsHelper {
		public static void LoadChartSettings(Chart chart, IAnalysisInfo analysisInfo) {
			if(analysisInfo != null && chart != null) {
				if(analysisInfo.ChartSettingsContent != null && analysisInfo.ChartSettingsContent.Length > 0) {
					int realLength = analysisInfo.ChartSettingsContent.Length;
					using(MemoryStream stream = new MemoryStream(analysisInfo.ChartSettingsContent, 0, realLength)) {
						chart.LoadLayout(stream);
					}
				}
			}
		}
		public static bool HasChartSettings(IAnalysisInfo analysisInfo) {
			Guard.ArgumentNotNull(analysisInfo, "analysisInfo");
			return analysisInfo.ChartSettingsContent != null && analysisInfo.ChartSettingsContent.Length > 0;
		}
		public static bool SaveChartSettings(Chart chart, IAnalysisInfo analysisInfo) {
			if(analysisInfo != null && chart != null) {
				using(MemoryStream stream = new MemoryStream()) {
					chart.SaveLayout(stream);
					byte[] layoutSettings = stream.GetBuffer();
					if(analysisInfo.ChartSettingsContent == null || !layoutSettings.SequenceEqual(analysisInfo.ChartSettingsContent)) {
						analysisInfo.ChartSettingsContent = layoutSettings;
						return true;
					}
				}
			}
			return false;
		}
	}
}
