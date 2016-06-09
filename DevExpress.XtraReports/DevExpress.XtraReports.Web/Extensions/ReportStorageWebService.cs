﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Web.Extensions {
	public interface IReportStorageWebTool {
		Dictionary<string, string> GetUrls();
		string SetNewData(XtraReport report, string defaultUrl);
	}
	class ReportStorageWebTool : IReportStorageWebTool {
		public Dictionary<string, string> GetUrls() {
			return new Dictionary<string, string>();
		}
		public string SetNewData(XtraReport report, string defaultUrl) {
			return defaultUrl;
		}
	}
	public static class ReportStorageWebService {
		static readonly object padlock = new object();
		static IReportStorageWebTool tool;
		static bool isRegister = false;
		public static bool IsRegister {
			get { return isRegister; }
		}
		static IReportStorageWebTool Tool {
			get {
				if(tool == null)
					tool = new ReportStorageWebTool();
				return tool;
			}
		}
		public static void RegisterTool(IReportStorageWebTool tool) {
			lock(padlock) {
				isRegister = true;
				ReportStorageWebService.tool = tool;
			}
		}
		public static Dictionary<string, string> GetUrls() {
			lock(padlock) {
				return Tool.GetUrls();
			}
		}
		public static string SetNewData(XtraReport report, string defaultUrl) {
			lock(padlock) {
				return Tool.SetNewData(report, defaultUrl);
			}
		}
	}
}
