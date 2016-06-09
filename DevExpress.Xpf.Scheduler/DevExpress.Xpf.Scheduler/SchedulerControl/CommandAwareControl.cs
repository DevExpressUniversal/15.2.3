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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Commands;
using System.ComponentModel;
using DevExpress.Utils.KeyboardHandler;
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerControl
	public partial class SchedulerControl {
		CommandBasedKeyboardHandler<SchedulerCommandId> ICommandAwareControl<SchedulerCommandId>.KeyboardHandler { get { return InnerControl != null ? InnerControl.KeyboardHandler as CommandBasedKeyboardHandler<SchedulerCommandId> : null; } }
		Command ICommandAwareControl<SchedulerCommandId>.CreateCommand(SchedulerCommandId commandId) {
			return this.CreateCommand(commandId);
		}
		public virtual SchedulerCommand CreateCommand(SchedulerCommandId commandId) {
			if (InnerControl != null)
				return InnerControl.CreateCommand(commandId);
			else
				return null;
		}
		bool ICommandAwareControl<SchedulerCommandId>.HandleException(Exception e) {
			return this.HandleException(e);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (InnerControl != null)
				return InnerControl.RaiseUnhandledException(e);
			else
				return false;
		}
		void ICommandAwareControl<SchedulerCommandId>.Focus() {
			this.Focus();
		}
		void ICommandAwareControl<SchedulerCommandId>.CommitImeContent() {
		}
		#region Events
		#region BeforeDispose
#if !SL && !WPF
		static readonly object onBeforeDispose = new object();
		public event EventHandler BeforeDispose {
			add { Events.AddHandler(onBeforeDispose, value); }
			remove { Events.RemoveHandler(onBeforeDispose, value); }
		}
		protected internal virtual void RaiseBeforeDispose() {
			EventHandler handler = (EventHandler)Events[onBeforeDispose];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
#else
		public event EventHandler BeforeDispose {
			add { }
			remove { }
		}
#endif
		#endregion
		#endregion
	}
	#endregion
}
