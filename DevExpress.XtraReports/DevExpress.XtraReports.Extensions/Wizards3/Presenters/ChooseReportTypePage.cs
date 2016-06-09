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
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Presenters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Wizards3.Views;
namespace DevExpress.XtraReports.Wizards3.Presenters {
	public class ChooseReportTypePageEx<TReportModel> : ChooseReportTypePage<TReportModel> 
		where TReportModel : XtraReportModel {
		readonly bool singleDataSourceType;
		DataSourceType dataSourceType;
		readonly DataSourceTypes dataSourceTypes;
		readonly IEnumerable<SqlDataConnection> dataConnections;
		public override bool MoveNextEnabled {
			get { return View.ReportType != ReportType.Empty; }
		}
		public override bool FinishEnabled {
			get { return View.ReportType == ReportType.Empty; }
		}
		public ChooseReportTypePageEx(IChooseReportTypePageViewExtended view, IEnumerable<SqlDataConnection> dataConnections, DataSourceTypes dataSourceTypes)
			: base(view) {
			this.dataSourceTypes = dataSourceTypes;
			this.dataConnections = dataConnections;
			singleDataSourceType = this.dataSourceTypes.Count == 0;
		}
		public override void Begin() {
			base.Begin();
			((IChooseReportTypePageViewExtended)View).ReportTypeChanged += view_ReportTypeChanged;
		}
		public override void Commit() {
			base.Commit();
			if(singleDataSourceType) {
				Model.DataSourceType = DataSourceType;
			}
		}
		public override Type GetNextPageType() {
			return View.ReportType == ReportType.Standard
				? singleDataSourceType
					? DataSourceTypeHelper.GetNextPageType<TReportModel>(DataSourceType, dataConnections.Any())
					: typeof(ChooseDataSourceTypePage<TReportModel>)
				: typeof(SelectLabelTypePage<TReportModel>);
		}
		DataSourceType DataSourceType {
			get { return dataSourceType ?? (dataSourceType = dataSourceTypes.First()); }
		}
		void view_ReportTypeChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
}
