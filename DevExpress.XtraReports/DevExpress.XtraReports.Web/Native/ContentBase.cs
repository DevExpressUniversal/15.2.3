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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native {
	public abstract class ContentBase {
		protected const int DefaultWidth = 330;
		protected const int DefaultHeight = 90;
		protected internal const int DefaultNoPageWidth = 600;
		protected internal const int DefaultNoPageHeight = 200;
		public ActionBuilder ActionBuilder { get; private set; }
		public abstract bool ShouldAddLoadingPanel { get; }
		protected ReportViewer ReportViewer { get; private set; }
		protected WebControl DivControl { get; private set; }
		protected bool AutoSize {
			get { return ReportViewer.AutoSize; }
		}
		protected bool PageByPage {
			get { return ReportViewer.PageByPage; }
		}
		protected XtraReport Report {
			get { return ReportViewer.ActualReport; }
		}
		protected int CurrentPageIndex {
			get { return ReportViewer.CurrentPageIndex; }
		}
		protected Paddings Paddings {
			get { return ReportViewer.Paddings; }
		}
		protected int PageCount {
			get { return ReportViewer.PageCount; }
		}
		protected ContentBase(ReportViewer reportViewer) {
			ReportViewer = reportViewer;
			ActionBuilder = new ActionBuilder();
		}
		public abstract void PrepareControlHierarchy();
		public abstract void GetCreateClientObjectScript(string localVarName, string clientName);
		public void CreateControlHierarchy() {
			DivControl = CreateDivControl("Div");
		}
		public void ResetControlHierarchy() {
			ReportViewer.Controls.Remove(DivControl);
			DivControl = null;
		}
		public virtual void AddPageLoadScript(int pageIndex) {
			if(AutoSize) {
				Size pageSize = GetPageSize(pageIndex);
				if(!pageSize.IsEmpty) {
					ActionBuilder.AddAction("setViewSize", pageSize.Width, pageSize.Height);
				}
			}
			if(!ReportViewer.RemoteMode) {
				ActionBuilder.AddAction("onPageLoad", PageCount);
			}
		}
		public virtual void AddSelectTextScript(string arguments) {
			ActionBuilder.AddAction("findText", arguments);
		}
		protected WebControl CreateDivControl(string id) {
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			div.ID = id;
			ReportViewer.Controls.Add(div);
			return div;
		}
		protected void ApplySizeToDiv(int defaultWidth, int defaultHeight) {
			var width = ReportViewer.Width;
			var height = ReportViewer.Height;
			DivControl.Width = !width.IsEmpty ? width : defaultWidth;
			DivControl.Height = !height.IsEmpty ? height : defaultHeight;
		}
		protected static void RemoveStyleAttributes(WebControl webControl, params string[] keys) {
			foreach(string key in keys) {
				webControl.Style.Remove(key);
			}
		}
		protected Size GetPageSize(int pageIndex) {
			Size pageSize = Size.Empty;
			if(Report != null) {
				DocumentAccessor.LoadPage(Report.PrintingSystem.Document, pageIndex);
				pageSize = Report.PrintingSystem.GetPageSize(pageIndex, !ReportViewer.EnableReportMargins);
				if(pageSize.IsEmpty)
					pageSize = Report.GetPageSize(!ReportViewer.EnableReportMargins);
			}
			return pageSize;
		}
	}
}
