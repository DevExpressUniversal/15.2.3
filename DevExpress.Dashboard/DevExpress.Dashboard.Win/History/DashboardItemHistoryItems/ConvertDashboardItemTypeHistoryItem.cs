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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ConvertDashboardItemTypeHistoryItem : DashboardItemLayoutHistoryItem {
		readonly DataDashboardItem sourceItem;
		readonly DataDashboardItem destinationItem;
		int itemIndex;
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemConvertDashboardItemType), sourceItem.Name, destinationItem.GetType().Name); } }
		protected override DashboardItem InsertedDashboardItem { get { return destinationItem; } }
		protected override DashboardItem RemovedDashboardItem { get { return sourceItem; } }
		protected override IEnumerable<string> PropertyNames { get { return new[] { "Items" }; } }
		public ConvertDashboardItemTypeHistoryItem(DataDashboardItem sourceItem, DataDashboardItem destinationItem) {
			this.sourceItem = sourceItem;
			this.destinationItem = destinationItem;
			destinationItem.AssignDashboardItemDataDescription(sourceItem.CreateDashboardItemDataDescription());
		}
		protected override void PerformUndoAction(DashboardDesigner designer) {
			designer.Dashboard.Items.Remove(destinationItem);
			designer.Dashboard.Items.Insert(itemIndex, sourceItem);
		}
		protected override void PerformRedoAction(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			itemIndex = dashboard.Items.IndexOf(sourceItem);
			dashboard.Items.Remove(sourceItem);
			dashboard.Items.Insert(itemIndex, destinationItem);
		}
		protected override void RebuildLayout(Dashboard dashboard, int width, int height) {
			if(dashboard.LayoutRoot == null) {
				dashboard.RebuildLayout(width, height);
			} else {
				DashboardLayoutItem sourceLayoutItem = dashboard.LayoutRoot.FindRecursive(sourceItem);
				DashboardLayoutItem destinationLayoutItem = new DashboardLayoutItem(destinationItem);
				destinationLayoutItem.Weight = sourceLayoutItem.Weight;
				int index = sourceLayoutItem.Parent.ChildNodes.IndexOf(sourceLayoutItem);
				sourceLayoutItem.Parent.ChildNodes.Insert(index, destinationLayoutItem);
			}
		}
	}
}
