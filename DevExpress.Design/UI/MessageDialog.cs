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
using System.Windows;
using System.Collections.Generic;
namespace DevExpress.Design.UI {
	public class MessageDialog : Window {
		static MessageDialog() {
			var dProp = new DependencyPropertyRegistrator<MessageDialog>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		private MessageDialog() {
			ShowInTaskbar = false;
			WindowStyle = System.Windows.WindowStyle.None;
			AllowsTransparency = true;
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			ResizeMode = System.Windows.ResizeMode.NoResize;
			SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			this.DataContext = new MessageDialogViewModel();
		}
		public MessageDialog(string message, MessageBoxButton button):this() {			
			ViewModel.Message = message;
			ViewModel.Button = button;
		}
		public MessageBoxResult Result {
			get {
				return ViewModel.Result;
			}
		}
		MessageDialogViewModel ViewModel {
			get { return (MessageDialogViewModel)this.DataContext; }
		}
		public static MessageBoxResult Show(Window owner, string message, MessageBoxButton button) {
			MessageDialog window = new MessageDialog(message, button);
			if(owner != null)
				window.Owner = owner;
			window.ShowDialog();
			return window.Result;
		}
		public static MessageBoxResult Show(string message, MessageBoxButton button) {
			return Show(null, message, button);
		}
	}
}
