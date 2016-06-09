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

using System.Collections.Generic;
using DevExpress.Data.Async;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncSharedResultReceiver : IAsyncResultReceiver {
		readonly IAsyncResultReceiver mainReceiver;
		readonly IList<IAsyncResultReceiver> additionalReceivers;
		public AsyncSharedResultReceiver(IAsyncResultReceiver mainReceiver) {
			this.mainReceiver = mainReceiver;
			this.additionalReceivers = new List<IAsyncResultReceiver>();
		}
		public void AddReceiver(IAsyncResultReceiver receiver) {
			if (receiver == null)
				return;
			if (additionalReceivers.Contains(receiver))
				return;
			additionalReceivers.Add(receiver);
		}
		public void Canceled(Command command) {
			mainReceiver.Canceled(command);
			additionalReceivers.ForEach(x => x.Canceled(command));
		}
		public void Visit(CommandGetTotals command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandGetRow command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandApply command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandRefresh command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandGetRowIndexByKey command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandGetGroupInfo command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandGetUniqueColumnValues command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandFindIncremental command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandLocateByValue command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandGetAllFilteredAndSortedRows command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Visit(CommandPrefetchRows command) {
			mainReceiver.Visit(command);
			additionalReceivers.ForEach(x => x.Visit(command));
		}
		public void Notification(NotificationInconsistencyDetected notification) {
			mainReceiver.Notification(notification);
			additionalReceivers.ForEach(x => x.Notification(notification));
		}
		public void Notification(NotificationExceptionThrown exception) {
			mainReceiver.Notification(exception);
			additionalReceivers.ForEach(x => x.Notification(exception));
		}
		public void BusyChanged(bool busy) {
			mainReceiver.BusyChanged(busy);
			additionalReceivers.ForEach(x => x.BusyChanged(busy));
		}
		public void Refreshing(CommandRefresh refreshCommand) {
			mainReceiver.Refreshing(refreshCommand);
			additionalReceivers.ForEach(x => x.Refreshing(refreshCommand));
		}
		public void PropertyDescriptorsRenewed() {
			mainReceiver.PropertyDescriptorsRenewed();
			additionalReceivers.ForEach(x => x.PropertyDescriptorsRenewed());
		}
	}
}
