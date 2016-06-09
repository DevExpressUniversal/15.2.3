#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
namespace DevExpress.ExpressApp.Utils {
	public class LockedChangedEventArgs : EventArgs {
		private bool locked;
		private IList<string> pendingCalls;
		public LockedChangedEventArgs(bool locked, IList<string> pendingCalls) {
			this.locked = locked;
			this.pendingCalls = pendingCalls;
		}
		public bool Locked {
			get { return locked; }
		}
		public IList<string> PendingCalls {
			get {
				return pendingCalls;
			}
		}
	}
	public class Locker {
		private int lockCount;
		private List<string> pendingCalls = new List<string>();
		protected virtual void OnLockedChanged() {
			List<string> pendingCallsCopy = new List<String>(pendingCalls);
			if(!Locked) {
				pendingCalls.Clear();
			}
			LockedChangedEventArgs args = new LockedChangedEventArgs(Locked, pendingCallsCopy.AsReadOnly());
			if(LockedChanged != null) {
				LockedChanged(this, args);
			}
		}
		public int LockCount {
			get { return lockCount; }
		}
		public bool Locked {
			get { return lockCount > 0; }
		}
		public void Lock() {
			lockCount++;
			if(lockCount == 1) {
				OnLockedChanged();
			}
		}
		public void Unlock() {
			lockCount--;
			if(lockCount < 0) {
				throw new InvalidOperationException("Lock count is negative");
			}
			if(lockCount == 0) {
				OnLockedChanged();
			}
		}
		public void Call(string callName) {
			if(Locked && !pendingCalls.Contains(callName)) {
				pendingCalls.Add(callName);
			}
		}
		public void ClearPendingCall(string callName) {
			if(pendingCalls.Contains(callName)) {
				pendingCalls.Remove(callName);
			}
		}
		public event EventHandler<LockedChangedEventArgs> LockedChanged;
	}
}
