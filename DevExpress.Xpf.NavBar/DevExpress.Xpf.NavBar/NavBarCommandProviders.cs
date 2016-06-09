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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.NavBar {
	public class SelectItemCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavBarCommands.SelectItem;
		}
	}
	public class ChangeGroupExpandedCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavBarCommands.ChangeGroupExpanded;
		}
	}
	public class SetActiveGroupCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavBarCommands.SetActiveGroup;
		}
	}
	public class ChangeNavPaneExpandedCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavigationPaneCommands.ChangeNavPaneExpanded;
		}
	}
	public class ShowMoreGroupsCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavigationPaneCommands.ShowMoreGroups;
		}
	}
	public class ShowFewerGroupsCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return NavigationPaneCommands.ShowFewerGroups;
		}
	}
	public class ScrollUpCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return SmoothScroller.ScrollUpCommand;
		}
	}
	public class ScrollDownCommandProvider : ICommandProvider {
		ICommand ICommandProvider.GetCommand() {
			return SmoothScroller.ScrollDownCommand;
		}
	}
}
