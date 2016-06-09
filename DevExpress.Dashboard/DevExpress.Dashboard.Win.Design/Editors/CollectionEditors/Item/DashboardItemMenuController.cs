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
using System;
namespace DevExpress.DashboardWin.Design {
	class DashboardItemMenuController : MenuItemController {
		const int DashboardItemCount = 17;
		static Type[] DashboardItemTypesList = new Type[DashboardItemCount] {
			typeof(CardDashboardItem),
			typeof(GaugeDashboardItem),
			typeof(PieDashboardItem),
			typeof(ChartDashboardItem),
			typeof(ScatterChartDashboardItem),
			typeof(GridDashboardItem),
			typeof(PivotDashboardItem),
			typeof(RangeFilterDashboardItem),
			typeof(ChoroplethMapDashboardItem),
			typeof(GeoPointMapDashboardItem),
			typeof(BubbleMapDashboardItem),
			typeof(PieMapDashboardItem),
			typeof(TextBoxDashboardItem),
			typeof(ImageDashboardItem),
			typeof(ComboBoxDashboardItem),
			typeof(ListBoxDashboardItem),
			typeof(TreeViewDashboardItem)
		};
		static string[] DashboardItemNamesList = new string[DashboardItemCount] {
			ActionNames.Cards,
			ActionNames.Gauges,
			ActionNames.Pies,
			ActionNames.Chart,
			ActionNames.Scatter,
			ActionNames.Grid,
			ActionNames.Pivot,
			ActionNames.RangeFilter,
			ActionNames.ChoroplethMap,
			ActionNames.GeoPointMap, 
			ActionNames.BubbleMap, 
			ActionNames.PieMap,
			ActionNames.TextBox,
			ActionNames.Image,
			ActionNames.ComboBox,
			ActionNames.ListBox,
			ActionNames.TreeView
		};
		protected override int Count { get { return DashboardItemCount; } }
		protected override Type[] TypesList { get { return DashboardItemTypesList; } }
		protected override string[] NamesList { get { return DashboardItemNamesList; } }
		public DashboardItemMenuController()
			: base() {
		}
	}
}
