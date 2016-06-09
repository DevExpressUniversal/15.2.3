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
	public class PivotGridSettingsHelper {
		public static void LoadPivotGridSettings(IPivotGridSettingsStore pivotGridSettingsStore, IAnalysisInfo analysisInfo) {
			if(analysisInfo != null && pivotGridSettingsStore != null) {
				if(HasPivotGridSettings(analysisInfo)) {
					int realLength = analysisInfo.PivotGridSettingsContent.Length;
					using(MemoryStream stream = new MemoryStream(analysisInfo.PivotGridSettingsContent, 0, realLength)) {
						pivotGridSettingsStore.LoadPivotGridSettings(stream);
					}
				}
			}
		}
		public static bool HasPivotGridSettings(IAnalysisInfo analysisInfo) {
			Guard.ArgumentNotNull(analysisInfo, "analysisInfo");
			return analysisInfo.PivotGridSettingsContent != null && analysisInfo.PivotGridSettingsContent.Length > 0;
		}
		public static bool SavePivotGridSettings(IPivotGridSettingsStore pivotGridSettingsStore, IAnalysisInfo analysisInfo) {
			if(analysisInfo != null && pivotGridSettingsStore != null) {
				using(MemoryStream stream = new MemoryStream()) {
					pivotGridSettingsStore.SavePivotGridSettings(stream);
					byte[] layoutSettings = stream.GetBuffer();
					if(analysisInfo.PivotGridSettingsContent == null || !layoutSettings.SequenceEqual(analysisInfo.PivotGridSettingsContent)) {
						analysisInfo.PivotGridSettingsContent = layoutSettings;
						return true;
					}
				}
			}
			return false;
		}
	}
}
