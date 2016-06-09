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
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.Wizard.Views;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class ChooseReportLayoutPage<TModel> : WizardPageBase<IChooseReportLayoutPageView, TModel> where TModel : ReportModel {
		public override bool FinishEnabled { get { return true; } }
		public override bool MoveNextEnabled { get { return true; } }
		public ChooseReportLayoutPage(IChooseReportLayoutPageView view)
			: base(view) {
		}
		public override Type GetNextPageType() {
			return typeof(ChooseReportStylePage<TModel>);
		}
		public override void Begin() {
			View.IsGroupedReport = Model.IsGrouped();
			View.Portrait = Model.Portrait;
			View.AdjustFieldWidth = Model.AdjustFieldWidth;
			View.ReportLayout = Model.Layout;
		}
		public override void Commit() {
			Model.Portrait = View.Portrait;
			Model.AdjustFieldWidth = View.AdjustFieldWidth;
			Model.Layout = View.ReportLayout;
		}
	}
}
