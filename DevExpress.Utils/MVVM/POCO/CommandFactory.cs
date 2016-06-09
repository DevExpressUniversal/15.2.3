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

namespace DevExpress.Utils.MVVM.Internal {
	using System;
	using System.ComponentModel;
	using System.Threading;
	using System.Threading.Tasks;
	interface IDelegateCommand : System.Windows.Input.ICommand {
		void RaiseCanExecuteChanged();
	}
	interface IAsyncCommand : IDelegateCommand {
		bool IsExecuting { get; }
		CancellationTokenSource CancellationTokenSource { get; }
		bool IsCancellationRequested { get; }
		System.Windows.Input.ICommand CancelCommand { get; }
		void Wait(TimeSpan timeout);
	}
	abstract class CommandBase {
		public static bool DefaultUseCommandManager {
			get { return false; }
			set { }
		}
		protected static object TryCast(object value, Type targetType) {
			Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
			if(underlyingType.IsEnum) {
				if(value is string)
					return Enum.Parse(underlyingType, (string)value, false);
				if(value is IConvertible)
					return Enum.ToObject(underlyingType, value);
			}
			if(value is IConvertible && !targetType.IsAssignableFrom(value.GetType()))
				return Convert.ChangeType(value, underlyingType, System.Globalization.CultureInfo.InvariantCulture);
			if(value == null && targetType.IsValueType)
				return Activator.CreateInstance(targetType);
			return value;
		}
	}
	abstract class CommandBase<T> : CommandBase, System.Windows.Input.ICommand {
		bool System.Windows.Input.ICommand.CanExecute(object parameter) {
			return CanExecute(GetGenericParameter(parameter, true));
		}
		void System.Windows.Input.ICommand.Execute(object parameter) {
			Execute(GetGenericParameter(parameter));
		}
		public abstract void Execute(T parameter);
		public abstract bool CanExecute(T parameter);
		static T GetGenericParameter(object parameter, bool suppressCastException = false) {
			parameter = TryCast(parameter, typeof(T));
			if(parameter == null || parameter is T) return (T)parameter;
			if(suppressCastException) return default(T);
			throw new InvalidCastException(string.Format("CommandParameter: Unable to cast object of type '{0}' to type '{1}'", parameter.GetType().FullName, typeof(T).FullName));
		}
		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}
		protected virtual void OnCanExecuteChanged() {
			EventHandler handler = CanExecuteChanged;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
	}
	class DelegateCommand<T> : CommandBase<T>, IDelegateCommand {
		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null) {
		}
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			if(executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod");
			this.executeMethod = executeMethod;
			this.canExecuteMethod = canExecuteMethod;
		}
		Action<T> executeMethod = null;
		public override void Execute(T parameter) {
			if(!CanExecute(parameter))
				return;
			if(executeMethod != null)
				executeMethod(parameter);
		}
		Func<T, bool> canExecuteMethod = null;
		public override bool CanExecute(T parameter) {
			return (canExecuteMethod == null) || canExecuteMethod(parameter);
		}
	}
	class DelegateCommand : DelegateCommand<object> {
		public DelegateCommand(Action execute)
			: this(execute, null) {
		}
		public DelegateCommand(Action execute, Func<bool> canExecute)
			: base(
			(execute != null) ? (Action<object>)(o => execute()) : null,
			(canExecute != null) ? (Func<object, bool>)(o => canExecute()) : null) { }
	}
	class AsyncCommand<T> : CommandBase<T>, INotifyPropertyChanged, IAsyncCommand {
		public AsyncCommand(Func<T, Task> executeMethod)
			: this(executeMethod, null) {
		}
		public AsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod) {
			if(executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod");
			cancelCommand = new DelegateCommand(Cancel, CanCancel);
			this.executeMethod = executeMethod;
			this.canExecuteMethod = canExecuteMethod;
		}
		bool isExecuting;
		public bool IsExecuting {
			get { return isExecuting; }
			private set {
				if(isExecuting == value) return;
				isExecuting = value;
				OnIsExecutingChanged();
			}
		}
		CancellationTokenSource cancellationTokenSource;
		public CancellationTokenSource CancellationTokenSource {
			get { return cancellationTokenSource; }
			private set {
				if(cancellationTokenSource == value) return;
				cancellationTokenSource = value;
				OnCancellationTokenSourceChanged();
			}
		}
		public bool IsCancellationRequested {
			get {
				if(CancellationTokenSource == null) return false;
				return CancellationTokenSource.IsCancellationRequested;
			}
		}
		readonly DelegateCommand cancelCommand;
		System.Windows.Input.ICommand IAsyncCommand.CancelCommand {
			get { return cancelCommand; }
		}
		protected virtual void OnIsExecutingChanged() {
			RaisePropertyChanged("IsExecuting");
			cancelCommand.RaiseCanExecuteChanged();
			RaiseCanExecuteChanged();
		}
		protected virtual void OnCancellationTokenSourceChanged() {
			RaisePropertyChanged("CancellationTokenSource");
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if(handler != null) handler(this, new PropertyChangedEventArgs(propName));
		}
		Func<T, bool> canExecuteMethod = null;
		public override bool CanExecute(T parameter) {
			if(IsExecuting) return false;
			return (canExecuteMethod == null) || canExecuteMethod(parameter);
		}
		Func<T, Task> executeMethod;
		public override void Execute(T parameter) {
			if(!CanExecute(parameter) || executeMethod == null)
				return;
			IsExecuting = true;
			CancellationTokenSource = new CancellationTokenSource();
			executeTask = executeMethod(parameter)
				.ContinueWith(x => 
					IsExecuting = false, 
				TaskScheduler.FromCurrentSynchronizationContext());
		}
		Task executeTask;
		public void Wait(TimeSpan timeout) {
			if(executeTask == null || !IsExecuting) return;
			executeTask.Wait(timeout);
		}
		public void Cancel() {
			if(!CanCancel()) return;
			CancellationTokenSource.Cancel();
		}
		bool CanCancel() {
			return IsExecuting;
		}
	}
	class AsyncCommand : AsyncCommand<object> {
		public AsyncCommand(Func<Task> executeMethod)
			: this(executeMethod, null) {
		}
		public AsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
			: base(
			(executeMethod != null) ? (Func<object, Task>)(o => executeMethod()) : null,
			(canExecuteMethod != null) ? (Func<object, bool>)(o => canExecuteMethod()) : null) { }
	}
	public static class CommandFactory {
		public static System.Windows.Input.ICommand Create(Action execute, Func<bool> canExecute) {
			return new DelegateCommand(execute, canExecute);
		}
		public static System.Windows.Input.ICommand Create(Func<Task> execute, Func<bool> canExecute) {
			return new AsyncCommand(execute, canExecute);
		}
	}
	public static class CommandFactory<T> {
		public static System.Windows.Input.ICommand Create(Action<T> execute, Func<T, bool> canExecute) {
			return new DelegateCommand<T>(execute, canExecute);
		}
		public static System.Windows.Input.ICommand Create(Func<T, Task> execute, Func<T, bool> canExecute) {
			return new AsyncCommand<T>(execute, canExecute);
		}
	}
}
