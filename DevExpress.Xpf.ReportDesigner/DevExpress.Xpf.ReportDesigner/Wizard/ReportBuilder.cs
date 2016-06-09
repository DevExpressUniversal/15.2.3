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

using DevExpress.XtraReports.Wizards.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.Data.XtraReports.Wizard;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using WizardModel = DevExpress.XtraReports.Wizards3.XtraReportModel;
using DevExpress.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.DataAccess.Wizard;
using DevExpress.Mvvm.Native;
using DevExpress.XtraReports.Wizards;
using DevExpress.DataAccess;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard {
	public class ReportBuilder : WizardReportBuilder {
		class ComponentFactory : IComponentFactory {
			public IComponent Create(Type type) {
				var components = new System.Drawing.Design.ToolboxItem(type).CreateComponents();
				return components.FirstOrDefault(x => x.GetType() == type);
			}
		}
		public ReportBuilder() : base(()=> new ComponentFactory(), false) { }
		protected override void BeforeBuildLabelReport(XtraReport report) {
			base.BeforeBuildLabelReport(report);
		}
		protected override void BuildLabelReport(XtraReport report, DevExpress.Data.XtraReports.Wizard.ReportModel model) {
			base.BuildLabelReport(report, model);
			report.Bands.ForEach(x => x.UpdateHeight());
			report.UpdateHeight();
		}
		protected override void BeforeBuildStandardReport(XtraReport report, DevExpress.Data.XtraReports.Wizard.ReportModel model) {
			var dataSource = new DataComponentCreator().CreateDataComponent((WizardModel)model);
			report.DataSource = dataSource;
			report.DataMember = dataSource.DataMember;
			base.BeforeBuildStandardReport(report, model);
		}
		protected override void AfterBuildStandardReport(XtraReport report) {
			base.AfterBuildStandardReport(report);
			report.Bands.ForEach(x => x.UpdateHeight());
			report.UpdateHeight();
		}
		protected override void BuildEmptyReport(XtraReport report) {
			base.BuildEmptyReport(report);
		}
	}
}
