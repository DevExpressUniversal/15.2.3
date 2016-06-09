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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Linq.Expressions;
	public abstract class EventToCommandBehaviorBase<TViewModel, TEventArgs> :
		EventTriggerBase<TEventArgs>
		where TEventArgs : EventArgs
		where TViewModel : class {
		Expression<Action<TViewModel>> commandSelector;
		Predicate<TEventArgs> eventFilter;
		protected EventToCommandBehaviorBase(string eventName, Expression<Action<TViewModel>> commandSelector)
			: base(eventName) {
			if(commandSelector == null || !(commandSelector.Body is MethodCallExpression))
				throw new NotSupportedException("commandSelector");
			this.commandSelector = commandSelector;
		}
		protected EventToCommandBehaviorBase(string eventName, Expression<Action<TViewModel>> commandSelector, Predicate<TEventArgs> eventFilter)
			: base(eventName) {
			if(commandSelector == null || !(commandSelector.Body is MethodCallExpression))
				throw new NotSupportedException("commandSelector");
			this.commandSelector = commandSelector;
			this.eventFilter = eventFilter;
		}
		protected TViewModel ViewModel {
			get { return GetViewModel<TViewModel>(); }
		}
		Func<bool> canExecute;
		Action execute;
		protected sealed override void OnEvent() {
			if(!CanProcessEvent(Args)) return;
			if(execute == null) {
				TViewModel viewModel = GetViewModel<TViewModel>();
				Func<object> queryCommandParameter = GetQueryCommandParameter(viewModel);
				Type commandType;
				object command = CommandHelper.GetCommand(commandSelector, viewModel, out commandType);
				canExecute = CommandHelper.GetCanExecute(command, commandType, queryCommandParameter);
				execute = CommandHelper.GetExecute(command, commandType, queryCommandParameter);
			}
			if(canExecute())
				execute();
		}
		protected virtual bool CanProcessEvent(TEventArgs args) {
			return (eventFilter == null) || eventFilter(args);
		}
		protected abstract Func<object> GetQueryCommandParameter(TViewModel viewModel);
	}
	public class EventToCommandBehavior<TViewModel, TEventArgs> :
		EventToCommandBehaviorBase<TViewModel, TEventArgs>
		where TEventArgs : EventArgs
		where TViewModel : class {
		Func<TEventArgs, object> convertArgs;
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector)
			: base(eventName, commandSelector) {
			this.convertArgs = ((args) => args);
		}
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector,
			Func<TEventArgs, object> eventArgsToCommandParameterConverter)
			: base(eventName, commandSelector) {
			this.convertArgs = eventArgsToCommandParameterConverter ?? ((args) => args);
		}
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector,
			Predicate<TEventArgs> eventFilter)
			: base(eventName, commandSelector, eventFilter) {
			this.convertArgs = ((args) => args);
		}
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector,
			Func<TEventArgs, object> eventArgsToCommandParameterConverter,
			Predicate<TEventArgs> eventFilter)
			: base(eventName, commandSelector, eventFilter) {
			this.convertArgs = eventArgsToCommandParameterConverter ?? ((args) => args);
		}
		protected virtual object GetCommandParameter() {
			return convertArgs(Args);
		}
		protected sealed override Func<object> GetQueryCommandParameter(TViewModel viewModel) {
			return () => GetCommandParameter();
		}
	}
	public class EventToCommandBehavior<TViewModel, T, TEventArgs> :
		EventToCommandBehaviorBase<TViewModel, TEventArgs>
		where TEventArgs : EventArgs
		where TViewModel : class {
		Expression<Func<TViewModel, T>> commandParameterSelector;
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector)
			: base(eventName, commandSelector) {
			if(commandParameterSelector == null || !(commandParameterSelector.Body is MemberExpression))
				throw new NotSupportedException("commandParameterSelector");
			this.commandParameterSelector = commandParameterSelector;
		}
		public EventToCommandBehavior(string eventName, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector, Predicate<TEventArgs> eventFilter)
			: base(eventName, commandSelector, eventFilter) {
			if(commandParameterSelector == null || !(commandParameterSelector.Body is MemberExpression))
				throw new NotSupportedException("commandParameterSelector");
			this.commandParameterSelector = commandParameterSelector;
		}
		protected sealed override Func<object> GetQueryCommandParameter(TViewModel viewModel) {
			return CommandHelper.GetQueryCommandParameter<TViewModel, T>(commandParameterSelector, viewModel);
		}
	}
}
