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

using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Web.Mvc.Internal;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Native;
namespace DevExpress.Web.Mvc {
	[ToolboxItem(false)]
	public class MVCxReportViewer : ReportViewer {
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public ImagesBase Images {
			get { return base.ImagesInternal; }
		}
		protected override bool AllowNullReport {
			get { return true; }
		}
		public MVCxReportViewer() {
			this.UseClientParameters = true;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			if(CallbackRouteValues != null)
				stb.Append(string.Concat(localVarName, ".callbackUrl=\"", DevExpress.Web.Mvc.Internal.Utils.GetUrl(CallbackRouteValues), "\";\n"));
			if(ExportRouteValues != null)
				stb.Append(string.Concat(localVarName, ".exportUrl=\"", DevExpress.Web.Mvc.Internal.Utils.GetUrl(ExportRouteValues), "\";\n"));
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientReportViewer";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ReportDocumentMap), WebResourceNames.DocumentMap.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDocumentViewer), WebResourceNames.DocumentViewer.ScriptResourceName);
			RegisterIncludeScript(typeof(MVCxReportViewer), DevExpress.Web.Mvc.Internal.Utils.ReportScriptResourceName);
		}
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			DialogHelper.ForceOnLoad(userControl);
#if DebugTest
			userControl.AppRelativeTemplateSourceDirectory = null;
#endif
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxReportViewerClientSideEvents();
		}
	}
}
