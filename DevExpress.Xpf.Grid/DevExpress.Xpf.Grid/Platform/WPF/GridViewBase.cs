#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid {
	public partial class GridViewBase {
		public static readonly DependencyProperty ClipboardCopyMaxRowCountInServerModeProperty;
		static partial void RegisterClassCommandBindings() {
			Type ownerType = typeof(GridViewBase);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.ChangeGroupExpanded, (d, e) => ((GridViewBase)d).ChangeGroupExpanded(e.Parameter)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.ExpandAllGroups, (d, e) => ((GridViewBase)d).ExpandAllGroups(e), (d, e) => ((GridViewBase)d).OnCanExpandCollapseAll(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.CollapseAllGroups, (d, e) => ((GridViewBase)d).CollapseAllGroups(e), (d, e) => ((GridViewBase)d).OnCanExpandCollapseAll(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.MoveParentGroupRow, (d, e) => ((GridViewBase)d).MoveParentGroupRow(), (d, e) => e.CanExecute = ((GridViewBase)d).CanMoveGroupParentRow()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.ClearGrouping, (d, e) => ((GridViewBase)d).ClearGrouping(), (d, e) => ((GridViewBase)d).OnCanClearGrouping(e)));
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridViewBaseClipboardCopyMaxRowCountInServerMode"),
#endif
 Category(Categories.OptionsCopy)]
		public int ClipboardCopyMaxRowCountInServerMode {
			get { return (int)GetValue(ClipboardCopyMaxRowCountInServerModeProperty); }
			set { SetValue(ClipboardCopyMaxRowCountInServerModeProperty, value); }
		}
		void OnCanExpandCollapseAll(CanExecuteRoutedEventArgs e) {
			e.CanExecute = GetIsGrouped();
		}
		void OnCanClearGrouping(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanClearGrouping();
		}
	}
}
