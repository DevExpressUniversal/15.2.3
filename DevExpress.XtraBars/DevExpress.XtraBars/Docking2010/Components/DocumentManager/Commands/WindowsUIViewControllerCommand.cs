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

namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public abstract class WindowsUIViewControllerCommand : BaseViewControllerCommand {
		protected override bool CanExecuteCore(object parameter) {
			WindowsUIView view;
			return Check(parameter, out view);
		}
		#region static
		public static readonly WindowsUIViewControllerCommand Back = new BackCommand();
		public static readonly WindowsUIViewControllerCommand Home = new HomeCommand();
		public static readonly WindowsUIViewControllerCommand Exit = new ExitCommand();
		public static readonly WindowsUIViewControllerCommand EnableFullScreenMode = new EnableFullScreenModeCommand();
		public static readonly WindowsUIViewControllerCommand DisableFullScreenMode = new DisableFullScreenModeCommand();
		#endregion static
		#region Commands
		[CommandGroup("WindowsUIView.Navigation", Order = 0, Index = 0)]
		class BackCommand : WindowsUIViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandHorizontalOrientation; }
			}
			protected override bool CanExecuteCore(object parameter) {
				WindowsUIView view;
				if(Check(parameter, out view)) {
					IContentContainer container = view.ActiveContentContainer;
					return container != null && container.Parent != null;
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((IWindowsUIViewController)controller).Back();
			}
		}
		[CommandGroup("WindowsUIView.Navigation", Order = 0, Index = 1)]
		class HomeCommand : WindowsUIViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandHome; }
			}
			protected override bool CanExecuteCore(object parameter) {
				WindowsUIView view;
				if(Check(parameter, out view)) {
					IContentContainer container = view.ActiveContentContainer;
					IContentContainer root = BaseContentContainer.GetRoot(container);
					return container != null && container != root && container.Parent != root;
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((IWindowsUIViewController)controller).Home();
			}
		}
		[CommandGroup("WindowsUIView.Application", Order = 100, Index = 100)]
		class ExitCommand : WindowsUIViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandExit; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((IWindowsUIViewController)controller).Exit();
			}
		}
		[CommandGroup("WindowsUIView.Application", Order = 100, Index = 0)]
		class EnableFullScreenModeCommand : WindowsUIViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandEnableFullScreenMode; }
			}
			protected override bool CanExecuteCore(object parameter) {
				WindowsUIView view;
				return Check(parameter, out view) && !WindowsUIViewController.IsFullScreenMode(view.Manager);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((IWindowsUIViewController)controller).EnableFullScreenMode(true);
			}
		}
		[CommandGroup("WindowsUIView.Application", Order = 100, Index = 1)]
		class DisableFullScreenModeCommand : WindowsUIViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandDisableFullScreenMode; }
			}
			protected override bool CanExecuteCore(object parameter) {
				WindowsUIView view;
				return Check(parameter, out view) && WindowsUIViewController.IsFullScreenMode(view.Manager);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((IWindowsUIViewController)controller).EnableFullScreenMode(false);
			}
		}
		#endregion Commands
	}
}
