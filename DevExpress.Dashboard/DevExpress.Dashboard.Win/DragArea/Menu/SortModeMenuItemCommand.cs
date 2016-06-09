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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using System;
namespace DevExpress.DashboardWin.Native {
	public class SortModeMenuItemCommand : DimensionMenuItemCommand {
		static string GetSortModeCaption(DimensionSortMode sortMode) {
			switch(sortMode) {
				case DimensionSortMode.DisplayText:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortModeDisplayText);
				case DimensionSortMode.Value:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortModeValue);
				case DimensionSortMode.ID:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortModeID);
				case DimensionSortMode.Key:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortModeKey);
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(sortMode));
			}
		}
		readonly DimensionSortMode sortMode;
		public override bool Checked { get { return Dimension.ActualSortMode == sortMode; } }
		public override string Caption { get { return GetSortModeCaption(sortMode); } }
		public SortModeMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, DimensionSortMode sortMode)
			: base(designer, dashboardItem, dimension) {
			this.sortMode = sortMode;
		}
		public override void Execute() {
			Dimension dimension = Dimension;
			DimensionSortOrder newSortOrder = dimension.SortOrder == DimensionSortOrder.None ? DimensionSortOrder.Ascending : dimension.SortOrder;
			Measure newMeasure = null;
			if(newSortOrder != dimension.SortOrder || newMeasure != dimension.SortByMeasure || sortMode != dimension.SortMode) {
				SortingHistoryItem historyItem = new SortingHistoryItem(DashboardItem, dimension, newSortOrder, newMeasure, sortMode);
				Designer.History.RedoAndAdd(historyItem);
			}
		}
	}
}
