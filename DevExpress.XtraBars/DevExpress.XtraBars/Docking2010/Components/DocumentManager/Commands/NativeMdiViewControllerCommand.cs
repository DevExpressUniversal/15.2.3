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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	public abstract class NativeMdiViewControllerCommand : BaseViewControllerCommand {
		protected override bool CanExecuteCore(object parameter) {
			NativeMdiView view;
			return Check(parameter, out view);
		}
		#region static
		public static readonly NativeMdiViewControllerCommand Cascade = new CascadeCommand();
		public static readonly NativeMdiViewControllerCommand TileHorizontal = new TileHorizontalCommand();
		public static readonly NativeMdiViewControllerCommand TileVertical = new TileVerticalCommand();
		public static readonly NativeMdiViewControllerCommand MinimizeAll = new MinimizeAllCommand();
		public static readonly NativeMdiViewControllerCommand ArrangeIcons = new ArrangeIconsCommand();
		public static readonly NativeMdiViewControllerCommand RestoreAll = new RestoreAllCommand();
		#endregion static
		#region Commands
		[CommandGroup("NativeMdiView.Layout", Visibility = CommandGroupVisibility.Always)]
		class CascadeCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandCascade; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).Cascade();
			}
		}
		[CommandGroup("NativeMdiView.Layout", Visibility = CommandGroupVisibility.Always)]
		class TileHorizontalCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileHorizontal; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).TileHorizontal();
			}
		}
		[CommandGroup("NativeMdiView.Layout", Visibility = CommandGroupVisibility.Always)]
		class TileVerticalCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileVertical; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).TileVertical();
			}
		}
		[CommandGroup("NativeMdiView.Layout", Visibility = CommandGroupVisibility.Always)]
		class MinimizeAllCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandMinimizeAll; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).MinimizeAll();
			}
		}
		[CommandGroup("NativeMdiView.Icons", Visibility = CommandGroupVisibility.Always)]
		class ArrangeIconsCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandArrangeIcons; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).ArrangeIcons();
			}
		}
		[CommandGroup("NativeMdiView.Icons", Visibility = CommandGroupVisibility.Always)]
		class RestoreAllCommand : NativeMdiViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandRestoreAll; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((INativeMdiViewController)controller).RestoreAll();
			}
		}
		#endregion Commands
	}
}
