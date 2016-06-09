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

using DevExpress.XtraReports.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Web.Extensions {
	public abstract class ReportStorageWebExtension : IReportStorageTool, IReportStorageWebTool {
		public static void RegisterExtensionGlobal(ReportStorageWebExtension extention) {
			ReportStorageService.RegisterTool(extention);
			ReportStorageWebService.RegisterTool(extention);
		}
		IReportStorageWebTool webTool;
		IReportStorageTool tool;
		IReportStorageWebTool WebTool {
			get {
				if(webTool == null)
					webTool = new ReportStorageWebTool();
				return webTool;
			}
		}
		IReportStorageTool Tool {
			get {
				if(tool == null)
					tool = new ReportStorageTool();
				return tool;
			}
		}
		public virtual Dictionary<string, string> GetUrls() {
			return WebTool.GetUrls();
		}
		public virtual bool IsValidUrl(string url) {
			return Tool.IsValidUrl(url);
		}
		public virtual void SetData(UI.XtraReport report, string url) {
			Tool.SetData(report, url);
		}
		public virtual byte[] GetData(string url) {
			return Tool.GetData(url);
		}
		public virtual bool CanSetData(string url) {
			return Tool.CanSetData(url);
		}
		public virtual void SetData(UI.XtraReport report, System.IO.Stream stream) {
			throw new NotImplementedException();
		}
		public virtual string SetNewData(UI.XtraReport report, string defaultUrl) {
			return WebTool.SetNewData(report, defaultUrl);
		}
	}
}
