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

using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Commands {
	public abstract class ChartAxisSettingsCommand<T, F, D> : DashboardItemCommand<D>
		where T : DashboardItemHistoryItem<D>
		where F : DashboardForm
		where D : DashboardItem {
		protected ChartAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected abstract T CreateHistoryItem(D item);
		protected abstract F CreateAxisSettingsForm(T historyItem, UserLookAndFeel lookAndFeel);
		protected override void ExecuteInternal(ICommandUIState state) {
			D dashboardItem = DashboardItem;
			if(dashboardItem != null) {
				T historyItem = CreateHistoryItem(dashboardItem);
				using(F form = CreateAxisSettingsForm(historyItem, Control.LookAndFeel)) {
					DialogResult dialogResult = form.ShowDialog(Control.FindForm());
					if(dialogResult == DialogResult.OK) {
						historyItem.Redo(Control);
						Control.History.Add(historyItem);
					}
					else
						historyItem.Undo(Control);
				}
			}
		}
	}
	public class ChartXAxisSettingsCommand : ChartAxisSettingsCommand<ChartXAxisSettingsHistoryItem, AxisXSettingsForm, ChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChartXAxisSettings; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChartXAxisSettingsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChartXAxisSettingsDescription; } }
		public override string ImageName { get { return "ChartXAxisSettings"; } }
		public ChartXAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override ChartXAxisSettingsHistoryItem CreateHistoryItem(ChartDashboardItem item) {
			return new ChartXAxisSettingsHistoryItem(item, ((ChartDashboardItemViewModel)Control.Viewer.SelectedLayoutItem.ItemViewer.ViewModel).Argument.Type != ChartArgumentType.String);
		}
		protected override AxisXSettingsForm CreateAxisSettingsForm(ChartXAxisSettingsHistoryItem historyItem, UserLookAndFeel lookAndFeel) {
			return new AxisXSettingsForm(historyItem, lookAndFeel);
		}
	}
}
