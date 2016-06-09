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
using System.Linq;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	class ChooseRemoteReportPresenter<TView> : RemoteDocumentSourcePagePresenterBase<IPageView>
		where TView : IChooseRemoteReportView {
		ReportDto selectedReportDto;
		protected new IChooseRemoteReportView View { get { return (IChooseRemoteReportView)base.View; } }
		public override bool FinishEnabled {
			get { return false; }
		}
		public override bool MoveNextEnabled {
			get { return View.SelectedReport != null; }
		}
		public ChooseRemoteReportPresenter(TView view)
			: base(view) {
		}
		public override void Begin() {
			View.SelectedCategoryChanged += View_SelectedCategoryChanged;
			View.SelectedReportChanged += View_SelectedReportChanged;
			RequestReports();
		}
		void RequestReports() {
			if(Model.Client != null)
				if(Model.ReportName != null) {
					RequestReports(true);
				} else
					RequestCategories();
		}
		void View_SelectedReportChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void View_SelectedCategoryChanged(object sender, EventArgs e) {
			View.FillReports(null);
			RaiseChanged();
			RequestReports(false);
		}
		public override void Commit() {
			if(selectedReportDto != null) {
				Model.ReportId = selectedReportDto.Id;
				Model.ReportName = selectedReportDto.Name;
			}
			View.SelectedCategoryChanged -= View_SelectedCategoryChanged;
			View.SelectedReportChanged -= View_SelectedReportChanged;
		}
		public override Type GetNextPageType() {
			return typeof(SetReportServerEndpointPresenter<ISetReportServerEndpointView>);
		}
		public override void ValidatePage(Action ifValidAction) {
			Model.Client.LoadReport(View.SelectedReport.Id, null, args => {
				if(args.Error != null) {
					RaiseError(args.Error.Message);
					return;
				}
				selectedReportDto = args.Result;
				ifValidAction();
			});
		}
		void RequestCategories(int selectedCategoryId = -1) {
			Model.Client.GetCategories(null, args => {
				if(args.Error != null) {
					RaiseError(args.Error.Message);
					return;
				}
				if(Model == null)
					return;
				var categorySource = new List<CategoryDto>(args.Result);
				categorySource.Sort((x, y) => {
					return x.Name.CompareTo(y.Name);
				});
				View.FillCategories(categorySource);
				if(selectedCategoryId != -1) {
					var selectedCategory = args.Result.FirstOrDefault(x => x.Id == selectedCategoryId);
					if(selectedCategory != null)
						View.SelectedCategory = selectedCategory;
				}
			});
		}
		int selectedReportId = -1;
		void RequestReports(bool findSelectedReport) {
			int selectedCategoryId = -1;
			Model.Client.GetReports(null, args => {
				if(args.Error != null) {
					RaiseError(args.Error.Message);
					return;
				}
				if(Model == null)
					return;
				var reportSource = new List<ReportCatalogItemDto>(args.Result);
				reportSource.ForEach(x => x.ModifiedDateTime = DateTime.SpecifyKind(x.ModifiedDateTime, DateTimeKind.Utc).ToLocalTime());
				reportSource.OrderBy(x => x.Id);
				if(findSelectedReport) {
					if(!string.IsNullOrWhiteSpace(Model.ReportName)) {
						var selectedReport = reportSource.FirstOrDefault(x => x.Name == Model.ReportName);
						if(selectedReport != null) {
							selectedCategoryId = selectedReport.CategoryId;
							selectedReportId = selectedReport.Id;
						}
					} else if(Model.ReportId > 0) {
						var selectedReport = reportSource.FirstOrDefault(x => x.Id == Model.ReportId);
						if(selectedReport != null) {
							selectedCategoryId = selectedReport.CategoryId;
							selectedReportId = selectedReport.Id;
						}
					}
					RequestCategories(selectedCategoryId);
					return;
				}
				var reports = new List<ReportCatalogItemDto>(args.Result.Where(x => x.CategoryId == View.SelectedCategory.Id));
				View.FillReports(reports);
				if(selectedReportId != -1) {
					var selectedReport = args.Result.FirstOrDefault(x => x.Id == selectedReportId);
					if(selectedReport != null)
						View.SelectedReport = selectedReport;
				}
			});
		}
	}
}
