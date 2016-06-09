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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraReports.Configuration;
namespace DevExpress.XtraReports.Extensions {
	public abstract class ReportStorageExtension : IReportStorageTool, IReportStorageToolInteractive {
		public static void RegisterExtensionGlobal(ReportStorageExtension extension) {
			ReportStorageService.RegisterTool(extension);
			ReportStorageServiceInteractive.RegisterTool(extension);
		}
		IReportStorageToolInteractive interactiveTool;
		IReportStorageTool tool;
		IReportStorageTool Tool {
			get {
				if(tool == null)
					tool = new ReportStorageTool(RootDirectory);
				return tool;
			}
		}
		IReportStorageToolInteractive InteractiveTool {
			get {
				if(interactiveTool == null)
					interactiveTool = new ReportStorageToolInteractive(RootDirectory);
				return interactiveTool;
			}
		}
		protected virtual string RootDirectory {
			get { return Settings.Default.StorageOptions.RootDirectory; }
		}
		#region IReportStorageToolInteractive
		public virtual string GetNewUrl() {
			return InteractiveTool.GetNewUrl();
		}
		public virtual string SetNewData(XtraReport report, string defaultUrl) {
			return InteractiveTool.SetNewData(report, defaultUrl);
		}
		public virtual string[] GetStandardUrls(ITypeDescriptorContext context) {
			return InteractiveTool.GetStandardUrls(context);
		}
		public virtual bool GetStandardUrlsSupported(ITypeDescriptorContext context) {
			return InteractiveTool.GetStandardUrlsSupported(context);
		}
		#endregion
		#region IReportStorageTool
		public virtual void SetData(XtraReport report, Stream stream) {
			Tool.SetData(report, stream);
		}
		public virtual byte[] GetData(string url) {
			return Tool.GetData(url);
		}
		public virtual void SetData(XtraReport report, string url) {
			Tool.SetData(report, url);
		}
		public virtual bool CanSetData(string url) {
			return Tool.CanSetData(url);
		}
		public virtual bool IsValidUrl(string url) {
			return Tool.IsValidUrl(url);
		}
		#endregion
	}
}
