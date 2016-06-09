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
using System.Linq;
using System.Text;
namespace DevExpress.Web.Office.Internal {
	public enum WorkSessionState { NotInitialized, Loaded, Hibernated, WakingUp }
	public class WorkSessionStateController {
		public WorkSessionState State { get; private set; }
		public WorkSessionStateController() {
			State = WorkSessionState.NotInitialized;
		}
		public void OnModelCreated() {
			ChangeState(WorkSessionState.NotInitialized, WorkSessionState.Loaded);
		}
		public void OnInitializedHibernated() {
			ChangeState(WorkSessionState.Loaded, WorkSessionState.Hibernated);
		}
		public void OnHibernated() {
			ChangeState(WorkSessionState.Loaded, WorkSessionState.Hibernated);
		}
		internal void OnBeforeWakeUp() {
			ChangeState(
				new WorkSessionState[] { 
					WorkSessionState.NotInitialized,
					WorkSessionState.Hibernated
				}, 
				WorkSessionState.WakingUp);
		}
		public void OnAfterWakeUp() {
			ChangeState(WorkSessionState.WakingUp, WorkSessionState.Loaded);
		}
		void ChangeState(WorkSessionState nextState) {
			ChangeState((WorkSessionState[])(Enum.GetValues(typeof(WorkSessionState))), nextState);
		}
		void ChangeState(WorkSessionState prevState, WorkSessionState nextState) {
			ChangeState(new WorkSessionState[]{ prevState }, nextState);
		}
		void ChangeState(WorkSessionState[] prevStates, WorkSessionState nextState) {
			if(prevStates.Contains(State))
				State = nextState;
			else
				throw new InvalidOperationException(string.Format("Invalid work session state change {0} -> {1}", prevStates, nextState));
		}
	}
}
