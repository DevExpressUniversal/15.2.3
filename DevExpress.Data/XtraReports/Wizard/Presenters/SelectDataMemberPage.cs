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
using DevExpress.Data.XtraReports.ServiceModel;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Data.XtraReports.Wizard.Native;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class SelectDataMemberPage<TModel> : ReportWizardServiceClientPage<ISelectDataMemberPageView, TModel> where TModel : ReportModel {
		public override bool MoveNextEnabled {
			get {
				return View.SelectedDataMemberName != null;
			}
		}
		public override Type GetNextPageType() {
			return typeof(SelectColumnsPage<TModel>);
		}
		public SelectDataMemberPage(ISelectDataMemberPageView view, IReportWizardServiceClient client)
			: base(view, client) {
			view.SelectedDataMemberChanged += view_SelectedDataMemberChanged;
		}
		public override void Begin() {
			base.Begin();
			View.ShowWaitIndicator(true);
			View.FillTables(null);
			View.FillViews(null);
			GetDataMembersOperation operation = new GetDataMembersOperation(Client);
			operation.GetDataMembersCompleted += operation_GetDataMembersCompleted;
			operation.GetDataMembersAsync(Model.DataSourceName, PageSessionId);
		}
		void operation_GetDataMembersCompleted(object sender, AsyncCompletedEventArgs e) {
			GetDataMembersOperation operation = (GetDataMembersOperation)sender;
			operation.GetDataMembersCompleted -= operation_GetDataMembersCompleted;
			if(!object.Equals(e.UserState, PageSessionId))
				return;
			View.ShowWaitIndicator(false);
			if(HandleError(e, "Failed to retrieve a list of available data members"))
				return;
			View.FillViews(operation.Views);
			View.FillTables(operation.Tables);
			View.SelectedDataMemberName = Model.DataMemberName;
			RaiseChanged();
		}
		public override void Commit() {
			Model.DataMemberName = View.SelectedDataMemberName;
			base.Commit();
		}
		void view_SelectedDataMemberChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
}
