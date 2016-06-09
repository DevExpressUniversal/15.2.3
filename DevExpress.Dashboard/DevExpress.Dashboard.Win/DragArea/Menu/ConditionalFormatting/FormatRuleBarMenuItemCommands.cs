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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class FormatRuleBarMenuItemCommand : FormatRuleMenuItemCommandBase {
		public override string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBar); } }
		public FormatRuleBarMenuItemCommand(IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(provider, dashboardItem, dataItem) {
		}
		public override void Execute() {
			using(Form form = FormatRuleViewFactory.CreateControlViewForm<FormatConditionBar>(ServiceProvider, DashboardItem, DataItem)) {
				form.ShowDialog(GuiContextService.Win32Window);
			}
		}
	}
	public class FormatRuleGradientRangeBarMenuItemCommand : FormatRuleMenuItemCommandBase {
		readonly FormatConditionRangeGradientPredefinedType rangeGradientType;
		public override string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeGradient); } }
		public FormatRuleGradientRangeBarMenuItemCommand(IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem, FormatConditionRangeGradientPredefinedType rangeGradientType)
			: base(provider, dashboardItem, dataItem) {
			this.rangeGradientType = rangeGradientType;
		}
		public override void Execute() {
			using(Form form = FormatRuleViewFactory.CreateControlViewForm<FormatConditionGradientRangeBar, FormatConditionRangeGradientPredefinedType>(ServiceProvider, DashboardItem, DataItem, rangeGradientType)) {
				form.ShowDialog(GuiContextService.Win32Window);
			}
		}
	}
	public class FormatRuleColorRangeBarMenuItemCommand : FormatRuleMenuItemCommandBase {
		readonly FormatConditionRangeSetPredefinedType rangeSetType;
		public override string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeSet); } }
		public FormatRuleColorRangeBarMenuItemCommand(IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem, FormatConditionRangeSetPredefinedType rangeSetType)
			: base(provider, dashboardItem, dataItem) {
			this.rangeSetType = rangeSetType;
		}
		public override void Execute() {
			using(Form form = FormatRuleViewFactory.CreateControlViewForm<FormatConditionColorRangeBar, FormatConditionRangeSetPredefinedType>(ServiceProvider, DashboardItem, DataItem, rangeSetType)) {
				form.ShowDialog(GuiContextService.Win32Window);
			}
		}
	}
}
