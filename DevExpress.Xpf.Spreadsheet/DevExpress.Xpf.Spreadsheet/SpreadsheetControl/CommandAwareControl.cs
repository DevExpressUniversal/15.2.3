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
using DevExpress.XtraSpreadsheet.Commands;
using System.ComponentModel;
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl {
		Command ICommandAwareControl<SpreadsheetCommandId>.CreateCommand(SpreadsheetCommandId commandId) {
			return this.CreateCommand(commandId);
		}
		public virtual SpreadsheetCommand CreateCommand(SpreadsheetCommandId commandId) {
			if (InnerControl != null)
				return InnerControl.CreateCommand(commandId);
			else
				return null;
		}
		bool ICommandAwareControl<SpreadsheetCommandId>.HandleException(Exception e) {
			return this.HandleException(e);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (InnerControl != null)
				return InnerControl.RaiseUnhandledException(e);
			else
				return false;
		}
		void ICommandAwareControl<SpreadsheetCommandId>.Focus() {
			this.Focus();
		}
		void ICommandAwareControl<SpreadsheetCommandId>.CommitImeContent() {
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
		public event EventHandler BeforeDispose { add { } remove { }
		}
#endif
		#endregion
		#endregion
	}
	#endregion
}
