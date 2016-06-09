#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public interface IModelReportDesignerViewItem : IModelViewItem { }
	public class ReportDesignerDetailItem : ViewItem {
		internal static IDictionary<Type, string> ParametersTypes { get; set; }
		private Action<ASPxReportDesigner> LoadReport;
		private string reportTitle;
		public ASPxReportDesigner ReportDesigner {
			get { return Control as ASPxReportDesigner; }
		}
		public ReportDesignerDetailItem(IModelViewItem model, Type objectType)
			: base(objectType, model.Id) {
				LoadReport = (designer) => { designer.OpenReport(new XtraReport()); };
		}
		public void OpenReport(XtraReport report, string reportTitle) {
			this.reportTitle = reportTitle;
			LoadReport = (designer) => { designer.OpenReport(report); };
		}
		public void OpenReport(string handle) {
			LoadReport = (designer) => { designer.OpenReport(handle); };
		}
		protected override object CreateControlCore() {
			ASPxReportDesigner designer = CreateReportDesigner();
			LoadReport(designer);
			return designer;
		}
		protected virtual ASPxReportDesigner CreateReportDesigner() {
			ASPxReportDesigner designer = new XafReportDesigner();
			designer.ShouldDisposeReport = true;
			designer.ClientInstanceName = "xafReportDesigner";
			designer.CssClass = "dx-viewport";
			designer.CustomJSProperties += designer_CustomJSProperties;
			ApplyCurrentTheme(designer);
			return designer;
		}
		protected virtual void ApplyCurrentTheme(ASPxReportDesigner designer) {
			if(BaseXafPage.CurrentTheme != null && BaseXafPage.CurrentTheme.ToLower().Contains("black")) {
				designer.ColorScheme = "dark";
			}
		}
		private void designer_CustomJSProperties(object sender, DevExpress.Web.CustomJSPropertiesEventArgs e) {
			e.Properties.Add("cpHiddenMenuItems", new string[] { "New", "New via Wizard", "Save As" });
			e.Properties.Add("cpParametersTypes", ParametersTypes);
			if(reportTitle != null) {
				e.Properties.Add("cpWizardReportTitle", reportTitle);
			}
		}
		#region Obsolete 15.2
		[Obsolete("This member is not used any longer.", true)]
		public void Setup(IObjectSpace objectSpace, XafApplication application) { }
		[Obsolete("This member is not used any longer.", true)]
		protected virtual void LoadSubReports(XtraReport mainReport) { }
		[Obsolete("This member is not used any longer.", true)]
		protected virtual void FillSubReportsList(ASPxReportDesigner designer) { }
		[Obsolete("This member is not used any longer.", true)]
		protected virtual void OnQueryReport(QueryReportEventArgs args) { }
		[Obsolete("This member is not used any longer.", true)]
		protected virtual void OnSaveReportLayout(SaveReportEventArgs args) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the ReportsStorage method instead.", true)]
		public event EventHandler<QueryReportEventArgs> QueryReport {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the ReportsStorage method instead.", true)]
		public event EventHandler<SaveReportEventArgs> SaveReport {
			add { }
			remove { }
		}
		#endregion
	}
	internal class XafReportDesigner : ASPxReportDesigner {
		protected override void GetCreateClientObjectScript(StringBuilder scriptContainer, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(scriptContainer, localVarName, clientName);
			scriptContainer.Append(string.Format("{0}.CustomizeMenuActions.AddHandler(xafCustomizeMenuActions);", localVarName));
			scriptContainer.Append(string.Format("{0}.Init.AddHandler(xafInitReportDesigner);", localVarName));
		}
	}
}
