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

using DevExpress.Web;
namespace DevExpress.XtraReports.Web.Native {
	public class ContentUseIFrame : ContentBase {
		public override bool ShouldAddLoadingPanel {
			get { return true; }
		}
		public ContentUseIFrame(ReportViewer reportViewer)
			: base(reportViewer) {
		}
		public override void PrepareControlHierarchy() {
			ReportViewer.AssignStyles(ReportViewer, DivControl);
			if(PageByPage) {
				RemoveStyleAttributes(DivControl, "width", "height");
				DivControl.Style.Add("overflow", "hidden");
				if(AutoSize) {
					ApplySizeToDiv(DefaultNoPageWidth, DefaultNoPageHeight);
				} else {
					ApplySizeToDiv(DefaultWidth, DefaultHeight);
				}
			} else if(Report != null) {
				ApplySizeToDiv(DefaultNoPageWidth, DefaultNoPageHeight);
			}
		}
		public override void GetCreateClientObjectScript(string localVarName, string clientName) {
			ActionBuilder.AddAssignment("padding", Paddings.GetPaddingTop().ToString(), Paddings.GetPaddingRight().ToString(), Paddings.GetPaddingBottom().ToString(), Paddings.GetPaddingLeft().ToString());
			ActionBuilder.AddAssignment("autosize", AutoSize);
			if(!PageByPage) {
				ActionBuilder.AddAssignment("pageByPage", false);
			}
			if(ReportViewer.ShouldRequestParametersFirst || (ReportViewer.ForcedReport == null && !ReportViewer.AllowNullReport)) {
				ActionBuilder.AddAssignment("loadPage", false);
			}
		}
		public override void AddPageLoadScript(int pageIndex) {
			base.AddPageLoadScript(pageIndex);
			if(!AutoSize) {
				ActionBuilder.AddAction("setViewSize");
			}
		}
	}
}
