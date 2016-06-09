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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Input;
namespace DevExpress.Design.UI {
	public abstract class DelegateCommandBase<T> : ICommand<T> {
		readonly Action<T> execute;
		readonly Func<T, bool> canExecute;
		public DelegateCommandBase(Action<T> execute)
			: this(execute, null) {
		}
		public DelegateCommandBase(Action<T> execute, Func<T, bool> canExecute) {
			AssertionException.IsFalse((execute == null) && (canExecute == null));
			this.execute = execute;
			this.canExecute = canExecute;
		}
		[DebuggerStepThrough]
		bool CanExecuteCore(T parameter) {
			return (canExecute == null) || canExecute(parameter);
		}
		[DebuggerStepThrough]
		protected void ExecuteCore(T parameter) {
			if(execute != null)
				execute(parameter);
		}
		[DebuggerStepThrough]
		public bool CanExecute(object parameter) {
			return CanExecuteCore(CastParameter(parameter));
		}
		[DebuggerStepThrough]
		bool ICommand<T>.CanExecute(T parameter) {
			return CanExecuteCore(parameter);
		}
		[DebuggerStepThrough]
		public void Execute(object parameter) {
			ExecuteCore(CastParameter(parameter));
		}
		[DebuggerStepThrough]
		void ICommand<T>.Execute(T parameter) {
			ExecuteCore(parameter);
		}
		[DebuggerStepThrough]
		protected T CastParameter(object parameter) {
			return (parameter is T) ? (T)parameter : default(T);
		}
	}
	public abstract class DelegateCommandBase : DelegateCommandBase<object> {
		public DelegateCommandBase(Action execute)
			: this(execute, null) {
		}
		public DelegateCommandBase(Action execute, Func<bool> canExecute)
			: base(
			execute != null ? new Action<object>(x => execute()) : null,
			canExecute != null ? new Func<object, bool>(x => canExecute()) : null) {
		}
	}
	class DelegateCommandICommandImplementation {
		readonly bool UseCommandManager;
		public DelegateCommandICommandImplementation(bool useCommandManager) {
			UseCommandManager = useCommandManager;
		}
		event EventHandler canExecuteChanged;
		public void AddCanExecuteChangedEventHandler(EventHandler eventHandler) {
			if(UseCommandManager) CommandManager.RequerySuggested += eventHandler;
			else canExecuteChanged += eventHandler;
		}
		public void RemoveCanExecuteChangedEventHandler(EventHandler eventHandler) {
			if(UseCommandManager) CommandManager.RequerySuggested -= eventHandler;
			else canExecuteChanged -= eventHandler;
		}
		public void RaiseCanExecuteChanged() {
			if(UseCommandManager) CommandManager.InvalidateRequerySuggested();
			else {
				if(canExecuteChanged != null)
					canExecuteChanged(this, EventArgs.Empty);
			}
		}
	}
	public class WpfDelegateCommand<T> : DelegateCommandBase<T>, ICommand {
		DelegateCommandICommandImplementation ICommandImplementation;
		public WpfDelegateCommand(Action<T> execute, bool useCommandManager = true)
			: this(execute, null, useCommandManager) {
		}
		public WpfDelegateCommand(Action<T> execute, Func<T, bool> canExecute, bool useCommandManager = true)
			: base(execute, canExecute) {
			ICommandImplementation = new DelegateCommandICommandImplementation(useCommandManager);
		}
		public void RaiseCanExecuteChanged() {
			ICommandImplementation.RaiseCanExecuteChanged();
		}
		public event EventHandler CanExecuteChanged {
			add { ICommandImplementation.AddCanExecuteChangedEventHandler(value); }
			remove { ICommandImplementation.RemoveCanExecuteChangedEventHandler(value); }
		}
	}
	public class WpfDelegateCommand : DelegateCommandBase, ICommand {
		DelegateCommandICommandImplementation ICommandImplementation;
		public WpfDelegateCommand(Action execute, bool useCommandManager = true)
			: this(execute, null, useCommandManager) {
		}
		public WpfDelegateCommand(Action execute, Func<bool> canExecute, bool useCommandManager = true)
			: base(execute, canExecute) {
			ICommandImplementation = new DelegateCommandICommandImplementation(useCommandManager);
		}
		public void RaiseCanExecuteChanged() {
			ICommandImplementation.RaiseCanExecuteChanged();
		}
		public event EventHandler CanExecuteChanged {
			add { ICommandImplementation.AddCanExecuteChangedEventHandler(value); }
			remove { ICommandImplementation.RemoveCanExecuteChangedEventHandler(value); }
		}
	}
	public abstract class WpfBindableBase : INotifyPropertyChanged {
		public static string GetPropertyName<T>(Expression<Func<T>> expression) {
			MemberExpression memberExpression = expression.Body as MemberExpression;
			return memberExpression.Member.Name;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback = null) {
			if(object.Equals(storage, value)) return false;
			T oldValue = storage;
			storage = value;
			if(changedCallback != null)
				changedCallback();
			RaisePropertyChanged(propertyName);
			return true;
		}
		protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action changedCallback = null) {
			string propName = GetPropertyName(expression);
			return SetProperty(ref storage, value, propName, changedCallback);
		}
		protected void RaisePropertyChanged(string propName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
		protected void RaisePropertyChanged() {
			RaisePropertiesChanged(null);
		}
		protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
			RaisePropertyChanged(GetPropertyName(expression));
		}
		protected void RaisePropertiesChanged(params string[] propertyNames) {
			if(propertyNames == null) {
				RaisePropertyChanged(string.Empty);
				return;
			}
			foreach(string propertyName in propertyNames) {
				RaisePropertyChanged(propertyName);
			}
		}
	}
	public abstract class WpfViewModelBase : WpfBindableBase, IViewModelBase {
		IViewModelBase parentViewModel;
		public WpfViewModelBase(IViewModelBase parentViewModel) {
			this.parentViewModel = parentViewModel;
		}
		#region IViewModelBase Members
		IServiceContainer serviceContainerCore;
		public IServiceContainer ServiceContainer {
			get {
				if(serviceContainerCore == null)
					serviceContainerCore = CreateServiceContainer();
				return serviceContainerCore;
			}
		}
		protected virtual IServiceContainer CreateServiceContainer() {
			return new ServiceContainer(GetParentServiceContainer());
		}
		protected IServiceContainer GetParentServiceContainer() {
			return (parentViewModel != null) ? parentViewModel.ServiceContainer : Platform.ServiceContainer;
		}
		public T GetParentViewModel<T>() where T : class, IViewModelBase {
			T result = parentViewModel as T;
			if(result != null)
				return result;
			if(parentViewModel != null)
				return parentViewModel.GetParentViewModel<T>();
			return null;
		}
		#endregion
	}
}
