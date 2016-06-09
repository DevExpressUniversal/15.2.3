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
using DevExpress.Utils;
using System.Linq;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public abstract class UndoManagerBase {
		#region inner classes
		protected interface IUndoAction {
			IUndoAction TryMerge(IUndoAction action);
			IRedoAction Undo();
		}
		protected interface IRedoAction {
			IUndoAction Redo();
		}
		class RedoAction : IRedoAction {
			readonly Func<IUndoAction> redo;
			bool executed;
			public RedoAction(Func<IUndoAction> redo) {
				this.redo = redo;
			}
			IUndoAction IRedoAction.Redo() {
				if(executed)
					throw new InvalidOperationException();
				executed = true;
				return redo();
			}
		}
		class UndoAction<TState> : IUndoAction where TState : class {
			readonly TState state;
			readonly Func<TState, IRedoAction> undo;
			readonly Func<TState, TState, TState> merge;
			public UndoAction(TState state, Func<TState, IRedoAction> undo, Func<TState, TState, TState> merge) {
				this.state = state;
				this.undo = undo;
				this.merge = merge;
			}
			bool executed;
			IRedoAction IUndoAction.Undo() {
				if(executed)
					throw new InvalidOperationException();
				executed = true;
				return undo(state);
			}
			IUndoAction IUndoAction.TryMerge(IUndoAction action) {
				if(executed)
					throw new InvalidOperationException();
				var other = action as UndoAction<TState>;
				if(merge != null && other != null)
					return merge(state, other.state).With(x => CreateUndoAction(x, undo, merge));
				return null;
			}
		}
		#endregion
		#region static
		protected const bool DefaultAllowMerge = true;
		static IUndoAction CommitCore(Transaction transaction, bool allowMerge) {
			if(transaction.IsEmpty())
				return null;
			var action = Pack(transaction.undoActions, allowMerge);
			transaction.ClearUndoActions();
			return action;
		}
		static IUndoAction Pack(IEnumerable<IUndoAction> actions, bool allowMerge) {
			return CreateUndoAction(new { Actions = actions.ToArray(), AllowMerge = allowMerge },
				x => {
					var undoActions = x.Actions.Select(y => y.Redo()).ToArray();
					return new { Actions = undoActions.Reverse().ToArray(), AllowMerge = x.AllowMerge };
				},
				x => {
					var redoActionsNew = x.Actions.Select(y => y.Undo()).ToArray();
					return new { Actions = redoActionsNew.Reverse().ToArray(), AllowMerge = x.AllowMerge };
				},
				(x1, x2) => {
					if(!x1.AllowMerge || !x2.AllowMerge || x1.Actions.Length != x2.Actions.Length)
						return null;
					IUndoAction[] mergedActions = new IUndoAction[x1.Actions.Length];
					for(int i = 0;
					i < x1.Actions.Length;
					i++) {
						mergedActions[i] = x1.Actions[i].TryMerge(x2.Actions[i]);
						if(mergedActions[i] == null)
							return null;
					}
					return new { Actions = mergedActions, AllowMerge = true };
				}
			);
		}
		static IUndoAction CreateUndoAction<TState>(TState state, Func<TState, IRedoAction> undo, Func<TState, TState, TState> merge) where TState : class {
			return new UndoAction<TState>(state, undo, merge);
		}
		protected static IRedoAction CreateRedoAction<TRedoState, TUndoState>(TRedoState redoState, Func<TRedoState, TUndoState> redo, Func<TUndoState, TRedoState> undo, Func<TUndoState, TUndoState, TUndoState> merge)
			where TUndoState : class
			where TRedoState : class {
			return new RedoAction(() => {
				var undoState = redo(redoState);
				return CreateUndoAction(undoState, redo, undo, merge);
			});
		}
		static IUndoAction CreateUndoAction<TRedoState, TUndoState>(TUndoState undoState, Func<TRedoState, TUndoState> redo, Func<TUndoState, TRedoState> undo, Func<TUndoState, TUndoState, TUndoState> merge)
			where TRedoState : class
			where TUndoState : class {
			return CreateUndoAction(undoState, x => {
				var redoStateNew = undo(x);
				return CreateRedoAction(redoStateNew, redo, undo, merge);
			}, merge);
		} 
		#endregion
		readonly Stack<IUndoAction> undoActions = new Stack<IUndoAction>();
		public bool Commit(Transaction transaction, bool allowMerge = DefaultAllowMerge) {
			var undoAction = CommitCore(transaction, allowMerge);
			if(undoAction == null)
				return false;
			TryMergePushAction(undoAction);
			OnTransactionCommited();
			return true;
		}
		public bool Execute(Action<Transaction> action, bool allowMerge = DefaultAllowMerge) {
			var transaction = new Transaction();
			action(transaction);
			return Commit(transaction, allowMerge);
		}
		protected virtual void OnTransactionCommited() {
		}
		protected void TryMergePushAction(IUndoAction undoAction) {
			if(undoActions.Any()) {
				var mergedAction = undoActions.Peek().TryMerge(undoAction);
				if(mergedAction != null) {
					undoActions.Pop();
					undoActions.Push(mergedAction);
					return;
				}
			}
			PushAction(undoAction);
		}
		protected void PushAction(IUndoAction undoAction) {
			undoActions.Push(undoAction);
		}
		protected IRedoAction UndoCore() {
			return undoActions.Pop().Undo();
		}
		protected void ClearUndoActions() {
			undoActions.Clear();
		}
		protected bool IsEmpty() {
			return !undoActions.Any();
		}
	}
	public class UndoManager : UndoManagerBase {
		public static UndoManager New(Action updateCommands = null) {
			return new UndoManager(updateCommands ?? (() => { }));
		}
		readonly Stack<IRedoAction> redoActions = new Stack<IRedoAction>();
		readonly Action updateCommands;
		protected UndoManager(Action updateCommands) {
			this.updateCommands = updateCommands;
		}
		public void Clear() {
			ClearUndoActions();
			redoActions.Clear();
			updateCommands();
		}
		protected override void OnTransactionCommited() {
			base.OnTransactionCommited();
			redoActions.Clear();
			updateCommands();
		}
		public void Undo() {
			if(!CanUndo())
				return;
			PerformClearOnExceptionAction(() => {
				redoActions.Push(UndoCore());
			});
			updateCommands();
		}
		public bool CanUndo() {
			return !IsEmpty();
		}
		public void Redo() {
			if(!CanRedo())
				return;
			PerformClearOnExceptionAction(() => {
				PushAction(redoActions.Pop().Redo());
			});
			updateCommands();
		}
		public bool CanRedo() {
			return redoActions.Any();
		}
		void PerformClearOnExceptionAction(Action action) {
			try {
				action();
			} catch {
				Clear();
			}
		}
	}
}
