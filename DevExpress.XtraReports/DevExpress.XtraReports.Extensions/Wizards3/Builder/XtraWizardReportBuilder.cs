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
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards;
using DevExpress.XtraReports.Wizards.Builder;
namespace DevExpress.XtraReports.Wizards3.Builder {
	public class XtraWizardReportBuilder : WizardReportBuilder {
		static Func<IComponentFactory> GetComponentFactoryConstructor(IDesignerHost designerHost) {
			return () => new ComponentFactory(designerHost);
		}
		readonly IDesignerHost designerHost;
		readonly object dataSource;
		readonly string dataMember;
		public XtraWizardReportBuilder(IDesignerHost designerHost, object dataSource, string dataMember)
			: base(GetComponentFactoryConstructor(designerHost), false) {
			this.designerHost = designerHost;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		protected override void BeforeBuildStandardReport(XtraReport report, ReportModel reportModel) {
			if(dataSource != null) {
				report.DataSource = dataSource;
				report.DataMember = dataMember;
				var component = dataSource as IComponent;
				if(component != null)
					DesignToolHelper.ForceAddToContainer(designerHost.Container, component, reportModel.DataSourceName);
			}
		}
		protected override void AfterBuildStandardReport(XtraReport report) {
			foreach(XRControlStyle style in report.StyleSheet) {
				try {
					designerHost.Container.Add(style, style.Name);
				} catch {
				}
			}
		}
		protected override void BuildEmptyReport(XtraReport report) {
			var dataSourcesList = designerHost.Container.Components.OfType<SqlDataSource>();
			foreach(var dataSource in dataSourcesList)
				DesignToolHelper.RemoveFromContainer(designerHost, dataSource);
		}
	}
}
