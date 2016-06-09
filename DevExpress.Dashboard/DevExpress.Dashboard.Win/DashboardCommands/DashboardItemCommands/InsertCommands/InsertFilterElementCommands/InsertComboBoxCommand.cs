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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class InsertFilterElementBaseCommand : DashboardItemCommand<FilterElementDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.SelectFilterElementType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertFilterElementsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertFilterElementsDescription; } }
		public override string ImageName { get { return "InsertFilterElement"; } }
		public InsertFilterElementBaseCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public class InsertComboBoxCommand : InsertItemCommand<ComboBoxDashboardItem> {
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameComboBoxItem; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertComboboxCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertComboboxDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertComboBox; } }
		public override string ImageName { get { return "InsertComboBox"; } }
		public InsertComboBoxCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class InsertListBoxCommand : InsertItemCommand<ListBoxDashboardItem> {
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameListBoxItem; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertListBoxCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertListBoxDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertListBox; } }
		public override string ImageName { get { return "InsertListBox"; } }
		public InsertListBoxCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class InsertTreeViewCommand : InsertItemCommand<TreeViewDashboardItem> {
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameTreeViewItem; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertTreeViewCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertTreeViewDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertTreeView; } }
		public override string ImageName { get { return "InsertTreeView"; } }
		public InsertTreeViewCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
