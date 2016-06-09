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

extern alias Platform;
using DevExpress.Design.UI;
using DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Linq;
using System.Windows.Input;
namespace DevExpress.Design.SmartTags {
	public class NestedPropertyLinesViewModel : SmartTagLineViewModelBase {
		IPropertyLineContext nestedContext;
		public IPropertyLineContext NestedContext {
			get { return nestedContext; }
			set { SetProperty(ref nestedContext, value, () => NestedContext); }
		}
		public string NestedProperty { get; private set; }
		public NestedPropertyLinesViewModel(IPropertyLineContext context, string nestedProperty)
			: base(context) {
			this.NestedProperty = nestedProperty;
			UpdateNestedViewModel();
		}
		void UpdateNestedViewModel() {
			IModelProperty property = SelectedItem.Properties[NestedProperty];
			var nestedContext = property.IsSet ? Context.CreateContext(property.Value) : null;
			var propertiesViewModel = nestedContext as FrameworkElementSmartTagPropertiesViewModel;
			if(propertiesViewModel != null && !propertiesViewModel.Lines.Any())
				nestedContext = null;
			NestedContext = nestedContext;
		}
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			if(propertyName == NestedProperty)
				UpdateNestedViewModel();
		}
	}
	public class ActionPropertyLineViewModel : SmartTagLineViewModelBase {
		public ICommand Command {
			get { return command; }
			protected set { SetProperty(ref command, value, () => Command); }
		}
		public string Text { get; protected set; }
		public Action<object> CommandAction {
			get { return commandAction; }
			set { SetProperty(ref commandAction, value, () => CommandAction, OnCommandActionChanged); }
		}
		public Func<object, bool> CanExecuteCommandAction {
			get { return canExecuteCommand; }
			set { SetProperty(ref canExecuteCommand, value, () => CanExecuteCommandAction, OnCanExecuteCommandChanged); }
		}
		protected ActionPropertyLineViewModel(IPropertyLineContext context)
			: base(context) { }
		public static ActionPropertyLineViewModel CreateLine(IPropertyLineCommandProvider provider) {
			ActionPropertyLineViewModel line = new ActionPropertyLineViewModel(provider.Context);
			line.CommandAction = provider.CommandAction;
			line.CanExecuteCommandAction = provider.CanExecuteAction != null ? (o => provider.CanExecuteAction(o)) : (Func<object, bool>)null;
			line.Text = provider.CommandText;
			return line;
		}
		public void UpdateCommand() {
			Command = new WpfDelegateCommand<object>(CommandAction, CanExecuteCommandAction);
		}
		void OnCommandActionChanged() {
			UpdateCommand();
		}
		void OnCanExecuteCommandChanged() {
			UpdateCommand();
		}
		Action<object> commandAction;
		Func<object, bool> canExecuteCommand;
		ICommand command;
	}
}
