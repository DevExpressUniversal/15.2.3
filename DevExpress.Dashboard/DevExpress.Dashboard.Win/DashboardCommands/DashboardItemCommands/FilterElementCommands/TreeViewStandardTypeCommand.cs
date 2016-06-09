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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Commands {
	public class TreeViewStandardTypeCommand : DashboardItemInteractionCommand<TreeViewDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.TreeViewTypeStandard; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandComboBoxStandardTypeCaption; } } 
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandComboBoxStandardTypeDescription; } } 
		public override string ImageName { get { return "FilterElementRadioMode"; } }
		public TreeViewStandardTypeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(TreeViewDashboardItem item) {
			return item.TreeViewType == TreeViewDashboardItemType.Standard;
		}
		protected override IHistoryItem CreateHistoryItem(TreeViewDashboardItem dashboardItem, bool enabled) {
			return new TreeViewTypeHistoryItem(dashboardItem, TreeViewDashboardItemType.Standard);
		}
	}
}
