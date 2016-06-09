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

using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public class ContentArrangementCountCommand : DashboardItemCommand<DashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ContentArrangementCount; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandContentArrangementCountCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandContentArrangementCountDescription; } }
		public override string ImageName { get { return null; } }
		public ContentArrangementCountCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			DashboardItem selectedItem = DashboardItem;
			IElementContainer elementContainer = selectedItem != null ? selectedItem.ElementContainer : null;
			if(elementContainer == null) {
				state.Enabled = false;
				state.EditValue = 0;
			}
			else {
				state.EditValue = elementContainer.ContentLineCount;
				state.Enabled = elementContainer.ContentArrangementMode != ContentArrangementMode.Auto;
			}
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			int contentLineCount = (int)state.EditValue;
			DashboardItem dashboardItem = DashboardItem;
			IElementContainer elementContainer = dashboardItem != null ? dashboardItem.ElementContainer : null;
			if(elementContainer != null && contentLineCount != elementContainer.ContentLineCount) {
				ContentArrangementHistoryItem historyItem = new ContentArrangementHistoryItem(dashboardItem, elementContainer.ContentArrangementMode, contentLineCount);
				historyItem.Redo(Control);
				Control.History.Add(historyItem);
			}
		}
	}
}
