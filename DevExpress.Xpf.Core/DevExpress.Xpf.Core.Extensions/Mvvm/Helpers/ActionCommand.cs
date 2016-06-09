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
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.MvvmSample.Helpers {
	public class ActionCommandBase : ICommand {
		bool allowExecute = true;
		public ActionCommandBase(Action<object> action, object owner) {
			Action = action;
			Owner = owner;
		}
		public bool AllowExecute {
			get { return allowExecute; }
			protected set {
				allowExecute = value;
				RaiseAllowExecuteChanged();
			}
		}
		public Action<object> Action { get; private set; }
		protected object Owner { get; private set; }
		public event EventHandler CanExecuteChanged;
		public bool CanExecute(object parameter) { return AllowExecute; }
		public void Execute(object parameter) {
			if(Action != null)
				Action(parameter);
		}
		void RaiseAllowExecuteChanged() {
			if(CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
	}
	public class SimpleActionCommand : ActionCommandBase {
		public SimpleActionCommand(Action<object> action, object owner) : base(action, owner) {}
		public SimpleActionCommand(Action<object> action) : this(action, null) { }
	}
	public class ExtendedActionCommandBase : ActionCommandBase {
		string allowExecutePropertyName;
		PropertyInfo allowExecuteProperty;
		public ExtendedActionCommandBase(Action<object> action, INotifyPropertyChanged owner, string allowExecuteProperty)
			: base(action, owner) {
			this.allowExecutePropertyName = allowExecuteProperty;
			if(Owner != null) {
				this.allowExecuteProperty = Owner.GetType().GetProperty(this.allowExecutePropertyName, BindingFlags.Public | BindingFlags.Instance);
				if(this.allowExecuteProperty == null)
					throw new ArgumentOutOfRangeException("allowExecuteProperty");
				((INotifyPropertyChanged)Owner).PropertyChanged += OnOwnerPropertyChanged;
			}
		}
		protected virtual void UpdateAllowExecute() {
			AllowExecute = Owner == null ? true : (bool)this.allowExecuteProperty.GetValue(Owner, null);
		}
		void OnOwnerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == this.allowExecutePropertyName)
				UpdateAllowExecute();
		}
	}
	public class ActionCommand : ExtendedActionCommandBase {
		public ActionCommand(Action<object> action, INotifyPropertyChanged owner, string allowExecuteProperty)
			: base(action, owner, allowExecuteProperty) {
			UpdateAllowExecute();
		}
	}
	public class ExtendedActionCommand : ExtendedActionCommandBase {
		Func<object, bool> allowExecuteCallback;
		object id;
		public ExtendedActionCommand(Action<object> action, INotifyPropertyChanged owner, string allowExecuteProperty, Func<object, bool> allowExecuteCallback, object id)
			: base(action, owner, allowExecuteProperty) {
			this.allowExecuteCallback = allowExecuteCallback;
			this.id = id;
			UpdateAllowExecute();
		}
		protected override void UpdateAllowExecute() {
			AllowExecute = this.allowExecuteCallback == null ? true : this.allowExecuteCallback(this.id);
		}
	}
}
