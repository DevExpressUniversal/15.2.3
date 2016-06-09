#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Web.Mvc;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.DashboardWeb.Mvc {
	[ToolboxItem(false)]
	public class MVCxDashboardViewer : ASPxDashboardViewer {
		internal const string MvcDashboardViewerScriptResourceName = "DevExpress.DashboardWeb.Mvc.Scripts.DashboardViewer.js";
		FileStreamResult fileStreamResult;
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public MVCxDashboardViewer()
			: base() {
		}
		internal MVCxDashboardViewer(bool needService) 
			: base(needService) { 
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + DevExpress.Web.Mvc.Internal.Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(ExportRouteValues != null)
				stb.AppendFormat(localVarName + ".exportUrl=\"" + DevExpress.Web.Mvc.Internal.Utils.GetUrl(ExportRouteValues) + "\";\n");
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(DevExpress.Web.Mvc.DevExpressHelper), DevExpress.Web.Mvc.Internal.Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxDashboardViewer), MvcDashboardViewerScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientDashboardViewer";
		}
		protected override void StreamToResponse(Stream stream, string fileName, bool saveAsFile, string fileFormat, string contentType) {
			fileStreamResult = new FileStreamResult(stream, contentType);
			ExportUtils.PrepareDownloadResult(null, fileName, saveAsFile, fileFormat, ref fileStreamResult);
		}
		public FileStreamResult PerformExport(string data) {
			ProcessClientRequest((Hashtable)HtmlConvertor.FromJSON(data));
			return fileStreamResult;
		}
	}
}
