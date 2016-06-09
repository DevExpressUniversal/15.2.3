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
	public abstract class ContentArrangeCommand : DashboardItemCommand<DashboardItem> {
		protected abstract ContentArrangementMode ArrangeMode { get; }
		protected ContentArrangeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			DashboardItem selectedItem = DashboardItem;
			IElementContainer elementContainer = selectedItem != null ? selectedItem.ElementContainer : null;
			state.Checked = elementContainer != null ? elementContainer.ContentArrangementMode == ArrangeMode : false;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			DashboardItem dashboardItem = DashboardItem;
			IElementContainer elementContainer = dashboardItem != null ? dashboardItem.ElementContainer : null;
			if (elementContainer != null) {
				ContentArrangementHistoryItem historyItem = new ContentArrangementHistoryItem(dashboardItem, ArrangeMode, elementContainer.ContentLineCount);
				historyItem.Redo(Control);
				Control.History.Add(historyItem);
			}
		}
	}
	public class ContentAutoArrangeCommand : ContentArrangeCommand {
		protected override ContentArrangementMode ArrangeMode { get { return ContentArrangementMode.Auto; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.ContentAutoArrange; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandContentAutoArrangeCaption; } } 
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandContentAutoArrangeDescription; } }
		public override string ImageName { get { return "ContentAutoArrange"; } }
		public ContentAutoArrangeCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ContentArrangeInColumnsCommand : ContentArrangeCommand {
		protected override ContentArrangementMode ArrangeMode { get { return ContentArrangementMode.FixedColumnCount; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.ContentArrangeInColumns; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandContentArrangeInColumnsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandContentArrangeInColumnsDescription; } }
		public override string ImageName { get { return "ContentArrangeInColumns"; } }
		public ContentArrangeInColumnsCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class ContentArrangeInRowsCommand : ContentArrangeCommand {
		protected override ContentArrangementMode ArrangeMode { get { return ContentArrangementMode.FixedRowCount; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.ContentArrangeInRows; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandContentArrangeInRowsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandContentArrangeInRowsDescription; } }
		public override string ImageName { get { return "ContentArrangeInRows"; } }
		public ContentArrangeInRowsCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
