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
namespace DevExpress.Xpf.Core.Native {
	public class PostponedAction {
		readonly Func<bool> postponeDelegate;
		Action action;
		public EventHandler ActionPerformed;
		public PostponedAction(Func<bool> postponeDelegate) {
			this.postponeDelegate = postponeDelegate;
		}
		public void PerformPostpone(Action action) {
			if(postponeDelegate()) {
				this.action = action;
			}
			else {
				this.action = null;
				if(action != null) {
					action();
					RaiseActionPerformed();
				}
			}
		}
		void RaiseActionPerformed() {
			if (ActionPerformed != null)
				ActionPerformed(this, EventArgs.Empty);
		}
		public void Perform() {
			PerformPostpone(this.action);
		}
		public void PerformForce(Action action) {
			this.action = null;
			if (action != null) {
				action();
				RaiseActionPerformed();
			}
		}
		public void PerformForce() {
			if (action != null) 
				action();
			action = null;
			RaiseActionPerformed();
		}
	}
	public class EndInitPostponedAction {
		bool performaActionOnEndInit;
		readonly Func<bool> isLoadingDelegate;
		Action action;
		public EndInitPostponedAction(Func<bool> isLoadingDelegate) {
			this.isLoadingDelegate = isLoadingDelegate;
		}
		public void PerformIfNotLoading(Action action, Action actionWhenLoading = null) {
			if(isLoadingDelegate()) {
				performaActionOnEndInit = true;
				this.action = action;
				if(actionWhenLoading != null)
					actionWhenLoading();
			}
			else
				action();
		}
		public void PerformActionOnEndInitIfNeeded(Action action) {
			if(performaActionOnEndInit) {
				performaActionOnEndInit = false;
				action();
				this.action = null;
			}
		}
		public void PerformActionOnEndInitIfNeeded() {
			PerformActionOnEndInitIfNeeded(action);
		}
	}
	public class LockedPostponedAction {
		readonly EndInitPostponedAction endInitPostponedAction;
		readonly Locker locker = new Locker();
		public LockedPostponedAction() {
			endInitPostponedAction = new EndInitPostponedAction(() => locker.IsLocked);
		}
		public void PerformLockedAction(Action primaryAction) {
			locker.DoLockedAction(primaryAction);
			locker.DoIfNotLocked(endInitPostponedAction.PerformActionOnEndInitIfNeeded);
		}
		public void PerformIfNotInProgress(Action action) {
			endInitPostponedAction.PerformIfNotLoading(action);
		}
	}
}
