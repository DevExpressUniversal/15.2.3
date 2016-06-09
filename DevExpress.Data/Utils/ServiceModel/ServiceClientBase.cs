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
using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Threading;
using DevExpress.Data.Utils.ServiceModel.Native;
using DevExpress.Utils;
namespace DevExpress.Data.Utils.ServiceModel {
	public abstract class ServiceClientBase : IServiceClientBase {
		#region Fields
		readonly IChannel channel;
		SynchronizationContext syncContext;
		#endregion
		#region Properties
		protected IChannel Channel {
			get { return channel; }
		}
		#endregion
		#region ctor
		protected ServiceClientBase(IChannel channel) {
			Guard.ArgumentNotNull(channel, "channel");
			this.channel = channel;
			syncContext = SynchronizationContext.Current;
		}
		#endregion
		#region Methods
		public void SetSynchronizationContext(SynchronizationContext syncContext) {
			this.syncContext = syncContext;
		}
		public void Abort() {
			IChannel channel = (IChannel)this.channel;
			channel.Abort();
		}
		public void CloseAsync() {
			IChannel channel = (IChannel)this.channel;
			AsyncCallback callback = ar => EndVoidOperation(ar, channel.EndClose, null);
			channel.BeginClose(callback, null);
		}
		protected void EndVoidOperation(IAsyncResult ar, Action<IAsyncResult> endOperation, Func<EventHandler<AsyncCompletedEventArgs>> getCompletedEvent) {
			try {
				endOperation(ar);
				RaiseVoidOperationCompleted(getCompletedEvent, null, false, ar.AsyncState);
			}
			catch(Exception exception) {
				if(exception.IsFatal())
					throw;
				RaiseVoidOperationCompleted(getCompletedEvent, exception, false, ar.AsyncState);
			}
		}
		protected void EndScalarOperation<T>(IAsyncResult ar, Func<IAsyncResult, T> endOperation, Func<EventHandler<ScalarOperationCompletedEventArgs<T>>> getCompletedEvent) {
			try {
				T result = endOperation(ar);
				RaiseScalarOperationCompleted(getCompletedEvent, result, null, false, ar.AsyncState);
			}
			catch(Exception exception) {
				if(exception.IsFatal())
					throw;
				RaiseScalarOperationCompleted(getCompletedEvent, null, exception, false, ar.AsyncState);
			}
		}
		protected void RaiseVoidOperationCompleted(Func<EventHandler<AsyncCompletedEventArgs>> getCompletedEvent, Exception exception, bool cancelled, object asyncState) {
			if(getCompletedEvent == null)
				return;
			RaiseOperationCompleted(getCompletedEvent, () => new AsyncCompletedEventArgs(exception, cancelled, asyncState));
		}
		protected void RaiseScalarOperationCompleted<T>(Func<EventHandler<ScalarOperationCompletedEventArgs<T>>> getCompletedEvent, object result, Exception exception, bool cancelled, object asyncState) {
			Guard.ArgumentNotNull(getCompletedEvent, "getCompletedEvent");
			RaiseOperationCompleted(getCompletedEvent, () => new ScalarOperationCompletedEventArgs<T>(result, exception, cancelled, asyncState));
		}
		void RaiseOperationCompleted<TEventArgs>(Func<EventHandler<TEventArgs>> getCompletedEvent, Func<TEventArgs> getEventArgs) where TEventArgs : AsyncCompletedEventArgs {
			SendOrPostCallback callback = s => {
				if(getCompletedEvent() != null) {
					getCompletedEvent()(this, getEventArgs());
				}
			};
			if(syncContext != null) {
				syncContext.Post(callback, null);
			} else {
				callback(null);
			}
		}
		#endregion
	}
}
