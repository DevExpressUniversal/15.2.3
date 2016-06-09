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

#if !FREE
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Mvvm.UI.Native;
#endif
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using System.ComponentModel;
namespace DevExpress.Mvvm.UI {
	[TargetTypeAttribute(typeof(UserControl))]
	[TargetTypeAttribute(typeof(Window))]
	public class CurrentWindowService : WindowAwareServiceBase, ICurrentWindowService {
		public ICommand ClosingCommand {
			get { return (ICommand)GetValue(ClosingCommandProperty); }
			set { SetValue(ClosingCommandProperty, value); }
		}
		public static readonly DependencyProperty ClosingCommandProperty =
			DependencyProperty.Register("ClosingCommand", typeof(ICommand), typeof(CurrentWindowService), new PropertyMetadata(null));
		void ICurrentWindowService.Close() {
			ActualWindow.Do(x => x.Close());
		}
		void ICurrentWindowService.Activate() {
			ActualWindow.Do(x => x.Activate());
		}
		void ICurrentWindowService.Hide() {
			ActualWindow.Do(x => x.Hide());
		}
		void ICurrentWindowService.SetWindowState(WindowState state) {
			ActualWindow.Do(x => x.WindowState = state);
		}
		void ICurrentWindowService.Show() {
			ActualWindow.Do(x => x.Show());
		}
		protected override void OnActualWindowChanged(Window oldWindow) {
			oldWindow.Do(x => x.Closing -= OnClosing);
			ActualWindow.Do(x => x.Closing += OnClosing);
		}
		void OnClosing(object sender, CancelEventArgs e) {
			ClosingCommand.Do(x => x.Execute(e));
		}
	}
}
