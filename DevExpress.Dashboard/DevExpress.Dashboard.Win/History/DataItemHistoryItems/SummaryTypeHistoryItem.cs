#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class SummaryTypeHistoryItem : DataItemHistoryItem {
		readonly SummaryType summaryType;
		readonly SummaryType previousSummaryType;
		readonly GridMeasureColumn measureColumn;
		readonly bool needResetBarDisplayMode;
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemMeasureSummaryType),  DataItemCaption); }  }
		public SummaryTypeHistoryItem(DataDashboardItem dashboardItem, Measure measure, SummaryType summaryType)
			: base(dashboardItem, measure) {
			this.summaryType = summaryType;
			this.previousSummaryType = measure.SummaryType;
			this.measureColumn = DataItem.Context as GridMeasureColumn;
			if(measureColumn != null && measureColumn.DisplayMode == GridMeasureColumnDisplayMode.Bar) {
				DataFieldType dataType = DataItem != null ? DataItem.DataFieldType : DataFieldType.Unknown;
				bool allowBarMode = GridMeasureColumn.IsBarModeAllowed(dataType, measure, summaryType, DashboardItem.DataSource != null && DashboardItem.DataSource.GetIsOlap());
				this.needResetBarDisplayMode = !allowBarMode;
			}
			else
				this.needResetBarDisplayMode = false;
		}
		protected override void PerformUndo() {
			((Measure)DataItem).SummaryType = previousSummaryType;
			if(needResetBarDisplayMode)
				measureColumn.DisplayMode = GridMeasureColumnDisplayMode.Bar;
		}
		protected override void PerformRedo() {
			((Measure)DataItem).SummaryType = summaryType;
			if(needResetBarDisplayMode)
				measureColumn.DisplayMode = GridMeasureColumnDisplayMode.Value;
		}
	}
}
