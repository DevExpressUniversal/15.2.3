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

using DevExpress.Data.Async;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncNotifyBusyChangedResultReceiver : IAsyncResultReceiver {
		IAsyncResultReceiverBusyChangedListener busyChangedListener;
		public AsyncNotifyBusyChangedResultReceiver(IAsyncResultReceiverBusyChangedListener busyChangedListener) {
			this.busyChangedListener = busyChangedListener;
		}
		public void Canceled(Command command) {
		}
		public void Visit(CommandGetTotals command) {
		}
		public void Visit(CommandGetRow command) {
		}
		public void Visit(CommandApply command) {
		}
		public void Visit(CommandRefresh command) {
		}
		public void Visit(CommandGetRowIndexByKey command) {
		}
		public void Visit(CommandGetGroupInfo command) {
		}
		public void Visit(CommandGetUniqueColumnValues command) {
		}
		public void Visit(CommandFindIncremental command) {
		}
		public void Visit(CommandLocateByValue command) {
		}
		public void Visit(CommandGetAllFilteredAndSortedRows command) {
		}
		public void Visit(CommandPrefetchRows command) {
		}
		public void Notification(NotificationInconsistencyDetected notification) {
		}
		public void Notification(NotificationExceptionThrown exception) {
		}
		public void BusyChanged(bool busy) {
			busyChangedListener.ProcessBusyChanged(busy);
		}
		public void Refreshing(CommandRefresh refreshCommand) {
		}
		public void PropertyDescriptorsRenewed() {
		}
	}
}
