#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting.InternalAccess;
namespace DevExpress.XtraReports.Web.Native {
	public class ContentUseDiv : ContentBase {
		public override bool ShouldAddLoadingPanel {
			get { return ReportViewer.PageByPage; }
		}
		public ContentUseDiv(ReportViewer reportViewer)
			: base(reportViewer) {
		}
		public override void PrepareControlHierarchy() {
			ReportViewer.AssignStyles(ReportViewer, DivControl);
			if(PageByPage) {
				PrepearePageByPageHierarchy();
			} else if(Report != null && (!AutoSize || PageCount == 0)) {
				DivControl.Style.Add("overflow", "auto");
			}
			Paddings.AssignToControl(DivControl);
		}
		public override void GetCreateClientObjectScript(string localVarName, string clientName) {
			ActionBuilder.AddAssignment("useIFrame", false);
			var reportHTML = string.Empty;
			if(PageByPage) {
				if(ReportViewer.RemoteMode && !ReportViewer.ShouldRequestParametersFirst) {
					reportHTML = ReportViewer.CallbackRemotePage();
				} else if(ReportViewer.IsReportReady(Report)) {
					var renderHelper = new ReportRenderHelper(ReportViewer);
					reportHTML = renderHelper.WritePage(CurrentPageIndex);
				}
				PrepearePageByPageHierarchy();
			} else if(Report != null) {
				PrintingSystemAccessor.ForceLoadDocument(Report.PrintingSystem);
				if(ReportViewer.IsReportReady(Report)) {
					var renderHelper = new ReportRenderHelper(ReportViewer);
					reportHTML = renderHelper.WriteWholeDocument();
				}
			}
			if(!string.IsNullOrEmpty(reportHTML)) {
				ActionBuilder.AddAction("OnPageChanged", HtmlConvertor.ToScript(reportHTML).Trim('\''));
			}
			if(PageByPage) {
				if(!ReportViewer.ShouldRequestParametersFirst) {
					AddPageLoadScript(CurrentPageIndex);
				}
			} else {
				ActionBuilder.AddAssignment("pageByPage", false);
				if(AutoSize) {
					ActionBuilder.AddAction("setViewSize");
				}
			}
		}
		void PrepearePageByPageHierarchy() {
			RemoveStyleAttributes(DivControl, "width", "height");
			if(AutoSize) {
				DivControl.Style.Add("overflow", "hidden");
				Size pageSize = GetPageSize(CurrentPageIndex);
				DivControl.Width = new Unit(pageSize.Width);
				DivControl.Height = new Unit(pageSize.Height);
			} else {
				DivControl.Style.Add("overflow", "auto");
				ApplySizeToDiv(DefaultWidth, DefaultHeight);
			}
		}
	}
}
