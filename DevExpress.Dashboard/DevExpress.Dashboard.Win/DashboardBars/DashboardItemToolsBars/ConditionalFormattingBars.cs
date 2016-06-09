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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Bars {
	public class ClearDataItemFormatConditionRulesBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		readonly ClearDataItemFormatConditionRulesMenuItemCommand command;
		public ClearDataItemFormatConditionRulesBarItem(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base() {
			this.command = new ClearDataItemFormatConditionRulesMenuItemCommand(designer, dashboardItem, dataItem);
			Caption = command.Caption;
			Enabled = command.CanExecute;
			Glyph = ImageHelper.GetEditorsMenuImage("ClearRules_16x16.png");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			command.Execute();
		}
	}
	public class ManagerRulesBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		readonly FormatRuleManagerRulesMenuItemCommand command;
		public ManagerRulesBarItem(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base() {
			this.command = new FormatRuleManagerRulesMenuItemCommand(designer, dashboardItem, dataItem);
			Caption = command.Caption;
			Enabled = command.CanExecute;
			Glyph = ImageHelper.GetEditorsMenuImage("ManageRules_16x16.png");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			command.Execute();
		}
	}
}
