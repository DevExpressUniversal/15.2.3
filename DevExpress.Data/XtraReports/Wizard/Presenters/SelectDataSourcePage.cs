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
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.ServiceModel;
using DevExpress.Data.XtraReports.Wizard.Views;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class SelectDataSourcePage<TModel> : ReportWizardServiceClientPage<ISelectDataSourcePageView, TModel> where TModel : ReportModel {
		IEnumerable<DataSourceInfo> dataSources;
		public SelectDataSourcePage(ISelectDataSourcePageView view, IReportWizardServiceClient client)
			: base(view, client) {
			view.SelectedDataSourceChanged += view_SelectedDataSourceChanged;
		}
		public override bool MoveNextEnabled {
			get {
				return dataSources != null && View.SelectedDataSourceName != null;
			}
		}
		public override Type GetNextPageType() {
			Debug.Assert(View.SelectedDataSourceName != null);
			var info = dataSources.First(x => x.Name == View.SelectedDataSourceName);
			return info.TablesOrViewsSupported ? typeof(SelectDataMemberPage<TModel>) : typeof(SelectHierarchicalDataSourceColumnsPage<TModel>);
		}
		void view_SelectedDataSourceChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void client_GetDataSourcesCompleted(object sender, ScalarOperationCompletedEventArgs<IEnumerable<DataSourceInfo>> e) {
			Client.GetDataSourcesCompleted -= client_GetDataSourcesCompleted;
			if(!object.Equals(e.UserState, PageSessionId))
				return;
			View.ShowWaitIndicator(false);
			if(HandleError(e, "Failed to retrieve a list of available data sources."))
				return;
			dataSources = e.Result;
			View.FillDataSourceList(dataSources);
			View.SelectedDataSourceName = Model.DataSourceName;
			RaiseChanged();
		}
		public override void Begin() {
			base.Begin();
			View.ShowWaitIndicator(true);
			dataSources = null;
			View.FillDataSourceList(dataSources);
			Client.GetDataSourcesCompleted += client_GetDataSourcesCompleted;
			Client.GetDataSourcesAsync(PageSessionId);
		}
		public override void Commit() {
			Model.DataSourceName = View.SelectedDataSourceName;
			base.Commit();
		}
	}
}
