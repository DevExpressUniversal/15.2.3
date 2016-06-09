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
using System.Text;
using DevExpress.Data.WizardFramework;
namespace DevExpress.XtraReports.ReportGeneration.Wizard.Presenters {
	public class WizardOptionsGeneralPage<TModel> : WizardPageBase<IWizardViewOptionsGeneral, TModel> where TModel : ReportGridDataModel {
		public WizardOptionsGeneralPage(IWizardViewOptionsGeneral view) : base(view){
			View.PageLoad += view_PageLoad;
		}
		void view_PageLoad(object sender, EventArgs e){
			UpdateReportPreview(sender);
		}
		protected virtual void UpdateReportPreview(object sender){
			((IWizardViewOptionsGeneral) sender).Report = Model.GetFakedReport(View.Options);
		}
		void view_OptionsChanged(object sender, EventArgs e){
			RaiseChanged();
			Model.Options = View.Options;
			UpdateReportPreview(sender);
		}
		public override void Begin(){
			View.OptionsChanged += view_OptionsChanged;
		}
		public override void Commit(){
		}
		public override Type GetNextPageType() {
			return Model.ViewGrouped ? typeof(WizardOptionsGroupingPage<TModel>) : typeof(WizardOptionsStylesPage<TModel>);
		}
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return false; } }
	}
}
