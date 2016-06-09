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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Threading;
namespace DevExpress.Xpf.DocumentViewer {
	public enum UndoActionType {
		Zoom,
		Rotate,
		Scroll,
		DeferredScroll,
	}
	public class UndoState {
		protected bool Equals(UndoState other) {
			return Equals(State, other.State);
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((UndoState)obj);
		}
		public override int GetHashCode() {
			return (State != null ? State.GetHashCode() : 0);
		}
		public UndoActionType ActionType { get; set; }
		public NavigationState State { get; set; }
		public Action<NavigationState> Perform { get; set; }
	}
	public class NavigationState : ICloneable {
		protected bool Equals(NavigationState other) {
			return PageIndex == other.PageIndex && OffsetY.Equals(other.OffsetY) && OffsetX.Equals(other.OffsetX) && Angle.Equals(other.Angle) && ZoomFactor.AreClose(other.ZoomFactor);
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NavigationState)obj);
		}
		public override int GetHashCode() {
			unchecked {
				var hashCode = PageIndex;
				hashCode = (hashCode * 397) ^ OffsetY.GetHashCode();
				hashCode = (hashCode * 397) ^ OffsetX.GetHashCode();
				hashCode = (hashCode * 397) ^ Angle.GetHashCode();
				return hashCode;
			}
		}
		public int PageIndex { get; set; }
		public double OffsetX { get; set; }
		public double OffsetY { get; set; }
		public double Angle { get; set; }
		public double ZoomFactor { get; set; }
		object ICloneable.Clone() {
			return new NavigationState() { PageIndex = PageIndex, OffsetX = OffsetX, OffsetY = OffsetY, Angle = Angle, ZoomFactor = ZoomFactor };
		}
		public NavigationState Clone() {
			ICloneable clone = this;
			return (NavigationState)clone.Clone();
		}
	}
	public partial class UndoRedoManager : BindableBase {
		readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(300);
		readonly DispatcherTimer timer;
		readonly Locker undoRedoLocker;
		public bool CanUndo {
			get { return UndoStack.Count > 0 || DeferredQueue.Count > 0; }
		}
		public bool CanRedo {
			get { return RedoStack.Count > 0; }
		}
		Stack<UndoState> UndoStack { get; set; }
		UndoState CurrentState { get; set; }
		Stack<UndoState> RedoStack { get; set; }
		Queue<UndoState> DeferredQueue { get; set; }
		public UndoRedoManager(Dispatcher dispatcher) {
			UndoStack = new Stack<UndoState>();
			RedoStack = new Stack<UndoState>();
			DeferredQueue = new Queue<UndoState>();
			undoRedoLocker = new Locker();
			timer = new DispatcherTimer(timeSpan, DispatcherPriority.Normal, (sender, args) => TimerTick(), dispatcher);
		}
		void TimerTick() {
			timer.Stop();
			FlushDeferredQueue();
		}
		public void Undo() {
			FlushDeferredQueue();
			if (!CanUndo)
				return;
			UndoState action = UndoStack.Pop();
			undoRedoLocker.DoLockedAction(() => action.Perform(action.State));
			RedoStack.Push(CurrentState);
			CurrentState = action;
			RaiseUndoRedoPropertiesChanged();
		}
		public void Redo() {
			FlushDeferredQueue();
			if (!CanRedo)
				return;
			UndoState action = RedoStack.Pop();
			undoRedoLocker.DoLockedAction(() => action.Perform(action.State));
			UndoStack.Push(CurrentState);
			CurrentState = action;
			RaiseUndoRedoPropertiesChanged();
		}
		public void Flush() {
			CurrentState = null;
			UndoStack.Clear();
			RedoStack.Clear();
			DeferredQueue.Clear();
			RaiseUndoRedoPropertiesChanged();
		}
		public void RegisterAction(UndoState action) {
			if (undoRedoLocker.IsLocked)
				return;
			if (CurrentState == null) {
				CurrentState = action;
				return;
			}
			if (IsSameAsCurrentState(action))
				return;
			if (IsDeferredAction(action))
				RegisterDeferredAction(action);
			else
				RegisterImmediateAction(action);
		}
		bool IsSameAsCurrentState(UndoState action) {
			return action.Equals(CurrentState);
		}
		void FlushDeferredQueue() {
			var firstAction = DeferredQueue.FirstOrDefault();
			var lastAction = DeferredQueue.LastOrDefault();
			DeferredQueue.Clear();
			if (firstAction == null || lastAction == null)
				return;
			var action = new UndoState() { ActionType = firstAction.ActionType, State = lastAction.State, Perform = firstAction.Perform };
			RegisterImmediateAction(action);
		}
		void RegisterImmediateAction(UndoState action) {
			FlushDeferredQueue();
			CurrentState.Do(x => UndoStack.Push(x));
			CurrentState = action;
			RedoStack.Clear();
			RaiseUndoRedoPropertiesChanged();
		}
		void RegisterDeferredAction(UndoState action) {
			DeferredQueue.Enqueue(action);
			timer.Stop();
			timer.Start();
		}
		bool IsDeferredAction(UndoState action) {
			return action.ActionType == UndoActionType.DeferredScroll;
		}
		void RaiseUndoRedoPropertiesChanged() {
			RaisePropertyChanged(() => CanUndo);
			RaisePropertyChanged(() => CanRedo);
		}
	}
}
