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
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
namespace DevExpress.Design.UI {
	class MessageDialogViewModel : WpfBindableBase {
		IEnumerable<ButtonViewModel> items;
		public class ButtonViewModel {
			public ButtonViewModel(MessageDialogViewModel parent, MessageBoxResult result) {
				Result = result;
				Parent = parent;
				OnClick = new WpfDelegateCommand<Window>(window => {
					Parent.Result = this.Result;
					window.DialogResult = true;
				});
			}
			MessageDialogViewModel Parent { get; set; }
			public MessageBoxResult Result { get; private set; }
			public ICommand OnClick { get; private set; }
		}
		internal MessageDialogViewModel():this("For test only"){
		}
		public MessageDialogViewModel(string message)
			: this(message, MessageBoxButton.OKCancel) {			
		}
		public MessageDialogViewModel(string message, MessageBoxButton button) {
			Message = message;
			Button = button;
			Result = MessageBoxResult.None;
		}
		void UpdateItems() { 
			Items = GetItems();
		}
		IEnumerable<ButtonViewModel> GetItems() {
			switch(Button) {
				case MessageBoxButton.OK:
					return new ButtonViewModel[] { new ButtonViewModel(this, MessageBoxResult.OK) };
				case MessageBoxButton.OKCancel:
					return new ButtonViewModel[] { new ButtonViewModel(this, MessageBoxResult.OK), new ButtonViewModel(this, MessageBoxResult.Cancel) };
				case MessageBoxButton.YesNoCancel:
					return new ButtonViewModel[] { new ButtonViewModel(this, MessageBoxResult.Yes), new ButtonViewModel(this, MessageBoxResult.No), new ButtonViewModel(this, MessageBoxResult.Cancel) };
				case MessageBoxButton.YesNo:
					return new ButtonViewModel[] { new ButtonViewModel(this, MessageBoxResult.Yes), new ButtonViewModel(this, MessageBoxResult.No) };
			}
			return new ButtonViewModel[] { new ButtonViewModel(this, MessageBoxResult.OK) };
		}
		public string Title {
			get { return "Title"; }
		}
		public IEnumerable<ButtonViewModel> Items {
			get {
				return items;
			}
			set {
				SetProperty<IEnumerable<ButtonViewModel>>(ref items, value, "Items");
			}
		}
		private MessageBoxButton button;
		public MessageBoxButton Button {
			get {
				return button;
			}
			set {
				if(SetProperty<MessageBoxButton>(ref button, value, "Button"))
					UpdateItems();
			}
		}
		private string message;
		public string Message {
			get {
				return message;
			}
			set {				
				SetProperty<string>(ref message, value, "Message");
			}
		}
		public MessageBoxResult Result {
			get;
			set;
		}
	}
}
