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
	public class SortOrderMenuItemCommand : DimensionMenuItemCommand {
		static string GetSortOrderCaption(DimensionSortOrder sortOrder) {
			switch(sortOrder) {
				case DimensionSortOrder.Ascending:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortAscending);
				case DimensionSortOrder.Descending:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortDescending);
				case DimensionSortOrder.None:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDimensionSortNone);
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(sortOrder));
			}
		}
		readonly DimensionSortOrder sortOrder;
		public override bool CanExecute { get { return base.CanExecute && !Dimension.TopNOptions.Enabled; } }
		public override bool Checked { get { return Dimension.ActualSortOrder == sortOrder; } }
		public override string Caption { get { return GetSortOrderCaption(sortOrder); } }
		public SortOrderMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, DimensionSortOrder sortOrder)
			: base(designer, dashboardItem, dimension) {
			this.sortOrder = sortOrder;
		}
		public override void Execute() {
			Dimension dimension = Dimension;
			if (sortOrder != dimension.SortOrder) {
				SortOrderHistoryItem historyItem = new SortOrderHistoryItem(DashboardItem, dimension, sortOrder);
				Designer.History.RedoAndAdd(historyItem);
			}
		}
	}
}
