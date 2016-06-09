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
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.Docking {
	enum DelayedActionPriority { Default, Delayed };
	class DelayedActionsHelper : DependencyObject, IDisposable {
		List<DelayedAction> delayedActions = new List<DelayedAction>();
		public void AddDelayedAction(Action action, DelayedActionPriority priority) {
			DelayedAction delayed = new DelayedAction(action, priority);
			if(delayedActions.Contains(delayed)) delayedActions.Remove(delayed);
			delayedActions.Add(delayed);
		}
		public void AddDelayedAction(Action action) {
			AddDelayedAction(action, DelayedActionPriority.Default);
		}
		public void DoDelayedActions() {
			var actions = delayedActions.ToArray();
			delayedActions.Clear();
			for(int i = 0; i < actions.Length; i++) {
				if(actions[i] != null)
					if(actions[i].Priority == DelayedActionPriority.Delayed) Dispatcher.BeginInvoke(actions[i].Action);
					else
						actions[i].Action();
			}
		}
		#region IDisposable Members
		public void Dispose() {
			delayedActions.Clear();
			GC.SuppressFinalize(this);
		}
		#endregion
		class DelayedAction {
			public Action Action { get; set; }
			public DelayedActionPriority Priority { get; set; }
			readonly int _hash;
			public DelayedAction(Action action, DelayedActionPriority priority) {
				Action = action;
				Priority = priority;
				_hash = action.Method.GetHashCode();
			}
			public override bool Equals(object obj) {
				DelayedAction action = obj as DelayedAction;
				if((object)action == null) return false;
				return action.Action == Action;
			}
			public override int GetHashCode() {
				return _hash;
			}
			public static bool operator ==(DelayedAction left, DelayedAction right) {
				return object.Equals(left, right);
			}
			public static bool operator !=(DelayedAction left, DelayedAction right) {
				return !object.Equals(left, right);
			}
		}
	}
}
