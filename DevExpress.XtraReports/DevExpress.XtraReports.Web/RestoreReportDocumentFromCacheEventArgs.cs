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

using System;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web {
	public class RestoreReportDocumentFromCacheEventArgs : EventArgs {
		XtraReport report;
		public string Key { get; private set; }
		internal bool IsRestored { get; private set; }
		internal XtraReport Report {
			get {
				if(report == null)
					report = new XtraReport();
				return report;
			}
		}
		public RestoreReportDocumentFromCacheEventArgs(XtraReport report, string key) {
			this.report = report;
			Key = key;
		}
		public void RestoreDocumentFromStream(Stream stream) {
			RestoreDocumentFromStream(stream, true);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RestoreDocumentFromStream(Stream stream, bool isVirtualDocument) {
			if(isVirtualDocument)
				PrintingSystemAccessor.LoadVirtualDocument(Report.PrintingSystem, stream);
			else
				Report.PrintingSystem.LoadDocument(stream);
			IsRestored = true;
		}
		public void RestoreDocumentFromFile(string fileName) {
			PrintingSystemAccessor.LoadVirtualDocument(Report.PrintingSystem, fileName);
			IsRestored = true;
		}
		public void RestoreExportOptionsFromStream(Stream stream) {
			if(stream.CanSeek) {
				stream.Position = 0;
			}
			Report.ExportOptions.RestoreFromStream(stream);
			IsRestored = true;
		}
		public void RestoreExportOptionsFromFile(string fileName) {
			using(FileStream stream = File.OpenRead(fileName)) {
				RestoreExportOptionsFromStream(stream);
			}
		}
	}
}
