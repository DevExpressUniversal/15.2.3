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
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel.Design;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public class WizardTaskManager : TaskManager, IWizardTaskManager {
		MvvmConstructorContext context;
		Window window;
		public WizardTaskManager(MvvmConstructorContext context) {
			ServiceContainer = new ServiceCache();
			this.context = context != null ? context : new MvvmConstructorContext();
		}
		public ServiceCache ServiceContainer { get; private set; }
		protected virtual Window GetUIWindow() {
			DevExpress.Design.UI.DXDesignWindow window = new DevExpress.Design.UI.DXDesignWindow();
			MvvmDXDesignWindowViewModel viewModel = new MvvmDXDesignWindowViewModel(window, ServiceContainer, context);
			window.DataContext = viewModel;
			window.Content = new MvvmConstructorTaskView() { DataContext = viewModel.ContentViewModel };
			window.Width = 900;
			window.Height = 600;
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			return window;
		}
		protected virtual void Run() {
			this.window = GetUIWindow();
			Nullable<bool> result = window.ShowDialog();
			if(result.GetValueOrDefault())
			{
				if (!IsStarted)
				Start();
				if (!AllTasksCompleted)
					RunTasks();
			}
		}
		protected override bool? ShowDialog(Window dialog) {
			if(this.window != null)
				dialog.Owner = this.window;
			return base.ShowDialog(dialog);
		}
		public MvvmConstructorContext Context {
			get {
				if(this.context == null)
					context = new MvvmConstructorContext();
				return this.context;
			}
		}
		public Window MainWindow {
			get { return window; }
		}
		public void CloseMainWindow() {
			window.DialogResult = true;
			window = null;
		}
	}
}
