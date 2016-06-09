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
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress {
	public class MissingAssemblyException : Exception {
		public MissingAssemblyException(string asmName)
			: base("Missing assembly: " + asmName + ".") {
		}
	}
}
namespace DevExpress.XtraReports.UI {
	public class ReportPrintContextNoninteractive : ReportPrintContext {
		public override IReportPrintTool CreateTool(IReport report) {
			Exception ex = PSNativeMethods.AspIsRunning ? new Exception("Calling this method is inappropriate in the Web context.") : 
				new MissingAssemblyException(AssemblyInfo.SRAssemblyPrinting);
			return new ReportPrintToolNoninteractive(report, ex);
		}
	}
	class ReportPrintToolNoninteractive : IReportPrintTool {
		Exception exception;
		IReport report;
		List<ParameterInfo> IReportPrintTool.ParametersInfo { get { return null; } }
		void IDisposable.Dispose() { 
		}
		bool IReportPrintTool.ApproveParameters(Parameter[] parameters, bool requestParameters) {
			return true;
		}
		public ReportPrintToolNoninteractive(IReport report, Exception exception) {
			this.report = report;
			this.exception = exception;
		}
		public void ClosePreview() {
			throw exception;
		}
		public void CloseRibbonPreview() {
			throw exception;
		}
		public void CreateDocument(bool buildPagesInBackground) {
			report.CreateDocument(buildPagesInBackground);
		}
		public void CreateDocument() {
			CreateDocument(false);
		}
		public void Print(string printerName) {
			throw exception;
		}
		public void Print() {
			throw exception;
		}
		public bool? PrintDialog() {
			throw exception;
		}
		public bool? ShowPageSetupDialog(object themeOwner) {
			throw exception;
		}
		public void ShowPreview(object themeOwner) {
			throw exception;
		}
		public void ShowPreview() {
			throw exception;
		}
		public void ShowPreviewDialog(object themeOwner) {
			throw exception;
		}
		public void ShowPreviewDialog() {
			throw exception;
		}
		public void ShowRibbonPreview(object themeOwner) {
			throw exception;
		}
		public void ShowRibbonPreview() {
			throw exception;
		}
		public void ShowRibbonPreviewDialog(object themeOwner) {
			throw exception;
		}
		public void ShowRibbonPreviewDialog() {
			throw exception;
		}
	}
}
