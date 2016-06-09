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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Native {
	public class GetDataMembersOperation {
		readonly IReportWizardServiceClient client;
		string dataSourceName;
		public IEnumerable<TableInfo> Tables { get; private set; }
		public IEnumerable<TableInfo> Views { get; private set; }
		public IEnumerable<TableInfo> StoredProcedures { get; private set; }
		public event EventHandler<AsyncCompletedEventArgs> GetDataMembersCompleted;
		public GetDataMembersOperation(IReportWizardServiceClient client) {
			Guard.ArgumentNotNull(client, "client");
			this.client = client;
		}
		public void GetDataMembersAsync(string dataSourceName, object asyncState) {
			this.dataSourceName = dataSourceName;
			client.GetDataMembersCompleted += client_GetDataMembersCompleted;
			client.GetDataMembersAsync(dataSourceName, asyncState);
		}
		void client_GetDataMembersCompleted(object sender, ScalarOperationCompletedEventArgs<IEnumerable<TableInfo>> e) {
			client.GetDataMembersCompleted -= client_GetDataMembersCompleted;
			if(ErrorOrCancelled(e))
				return;
			Tables = e.Result.Where(x => x.DataMemberType == DataMemberType.Table);
			Views = e.Result.Where(x => x.DataMemberType == DataMemberType.View);
			StoredProcedures = e.Result.Where(x => x.DataMemberType == DataMemberType.StoredProcedure);
			RaiseCompleted(e);
		}
		bool ErrorOrCancelled(AsyncCompletedEventArgs e) {
			if(e.Error != null || e.Cancelled) {
				RaiseCompleted(e);
				return true;
			}
			return false;
		}
		private void RaiseCompleted(AsyncCompletedEventArgs e) {
			if(GetDataMembersCompleted != null) {
				GetDataMembersCompleted(this, e);
			}
		}
	}
}
