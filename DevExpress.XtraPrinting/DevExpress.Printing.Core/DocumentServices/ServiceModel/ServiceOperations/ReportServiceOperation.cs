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
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
namespace DevExpress.DocumentServices.ServiceModel.ServiceOperations {
	public abstract class ReportServiceOperation : ServiceOperationBase {
		#region Fields and Properties
		protected bool Aborted { get; private set; }
		protected new IReportServiceClient Client {
			get { return (IReportServiceClient)base.Client; }
		}
		public abstract bool CanStop { get; }
		public event EventHandler CanStopChanged;
		#endregion
		#region Constructors
		protected ReportServiceOperation(IReportServiceClient client, TimeSpan statusUpdateInterval, DocumentId documentId)
			: base(client, statusUpdateInterval) {
			this.documentId = documentId;
		}
		#endregion
		#region Methods
		public abstract void Start();
		public abstract void Stop();
		public void Abort() {
			Delayer.Abort();
			UnsubscribeClientEvents();
			Aborted = true;
		}
		protected void RaiseCanStopChanged() {
			if(CanStopChanged != null) {
				CanStopChanged(this, EventArgs.Empty);
			}
		}
		protected override bool TryProcessError(AsyncCompletedEventArgs e) {
			return base.TryProcessError(e) || Aborted;
		}
		[Conditional("DEBUG")]
		protected void AssertAlive() {
#if DEBUG
			Debug.Assert(!Aborted);
#endif
		}
		#endregion
	}
}
