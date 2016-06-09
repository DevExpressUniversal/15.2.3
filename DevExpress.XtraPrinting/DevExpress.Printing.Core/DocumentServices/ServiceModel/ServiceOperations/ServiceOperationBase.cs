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
using System.Diagnostics;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.DocumentServices.ServiceModel.ServiceOperations {
	public abstract class ServiceOperationBase {
		static IDelayerFactory delayerFactory = new SynchronizedDelayerFactory();
		protected readonly Guid instanceId = Guid.NewGuid();
		readonly IServiceClientBase client;
		readonly IDelayer delayer;
		protected DocumentId documentId;
		public static IDelayerFactory DelayerFactory {
			get {
				return delayerFactory;
			}
			set {
				Guard.ArgumentNotNull(value, "value");
				delayerFactory = value;
			}
		}
		protected IDelayer Delayer {
			get { return delayer; }
		}
		protected IServiceClientBase Client {
			get { return client; }
		}
		public ServiceOperationBase(IServiceClientBase client, TimeSpan statusUpdateInterval) {
			Guard.ArgumentNotNull(client, "client");
			this.delayer = DelayerFactory.Create(statusUpdateInterval);
			this.client = client;
		}
		protected virtual bool TryProcessError(AsyncCompletedEventArgs e) {
			return e.Cancelled || e.Error != null;
		}
		[Conditional("DEBUG")]
		protected void AssertInstanceId(object userState) {
			Debug.Assert(userState.Equals(instanceId));
		}
		protected abstract void UnsubscribeClientEvents();
		protected bool IsSameInstanceId(object userState) {
			return userState.Equals(instanceId);
		}
	}
}
