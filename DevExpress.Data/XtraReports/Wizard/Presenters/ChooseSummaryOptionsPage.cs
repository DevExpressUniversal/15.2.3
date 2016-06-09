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
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class ChooseSummaryOptionsPage<TModel> : WizardPageBase<IChooseSummaryOptionsPageView, TModel> where TModel : ReportModel {
		#region Fields & Properties
		readonly List<ColumnInfoSummaryOptions> summaryOptions = new List<ColumnInfoSummaryOptions>();
		readonly IColumnInfoCache columnInfoCache;
		bool ignoreNullValues;
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return true; } }
		#endregion
		#region ctor
		public ChooseSummaryOptionsPage(IChooseSummaryOptionsPageView view, IColumnInfoCache columnInfoCache)
			: base(view) {
			Guard.ArgumentNotNull(columnInfoCache, "columnInfoCache");
			this.columnInfoCache = columnInfoCache;
		}
		#endregion
		#region Methods
		public override Type GetNextPageType() {
			return typeof(ChooseReportLayoutPage<TModel>);
		}
		public override void Begin() {
			ignoreNullValues = Model.IgnoreNullValuesForSummary;
			var options = Model.SummaryOptions.Select(x => new ColumnInfoSummaryOptions(columnInfoCache.Columns.First(c => c.Name == x.ColumnName), x.Flags));
			summaryOptions.Clear();
			summaryOptions.AddRange(options);
			RefreshView();
		}
		private void RefreshView() {
			View.FillSummaryOptions(summaryOptions.ToArray());
			View.IgnoreNullValues = ignoreNullValues;
		}
		public override void Commit() {
			Model.SummaryOptions = new HashSet<ColumnNameSummaryOptions>(
				summaryOptions.Select(x => new ColumnNameSummaryOptions(x.ColumnInfo.Name, x.Options.Flags)));
			Model.IgnoreNullValuesForSummary = ignoreNullValues;
		}
		#endregion
	}
}
