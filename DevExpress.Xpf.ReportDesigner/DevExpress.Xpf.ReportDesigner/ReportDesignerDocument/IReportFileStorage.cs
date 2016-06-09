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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public interface IReportProvider {
		XtraReport GetReport();
		XtraReport GetReport(string newReportTitle);
	}
	public interface IReportStorage {
		bool CanCreateNew();
		XtraReport CreateNew();
		XtraReport CreateNewSubreport();
		bool CanOpen();
		string Open(IReportDesignerUI designer);
		XtraReport Load(string reportID, IReportSerializer designerReportSerializer);
		string Save(string reportID, IReportProvider reportProvider, bool saveAs, string reportTitle, IReportDesignerUI designer);
		string GetErrorMessage(Exception exception);
	}
	public abstract class OperationFailedEventArgs : EventArgs {
		public OperationFailedEventArgs(Exception exception) {
			this.exception = exception;
			ErrorMessage = ExceptionHelper.GetInnerErrorMessage(exception);
			Rethrow = true;
			ShowErrorMessage = false;
		}
		readonly Exception exception;
		public Exception Exception { get { return exception; } }
		public string ErrorMessage { get; set; }
		public bool Rethrow { get; set; }
		public bool ShowErrorMessage { get; set; }
	}
	public abstract class SubreportLoadFailedEventArgs : OperationFailedEventArgs {
		public SubreportLoadFailedEventArgs(Exception exception, string reportSourceUrl)
			: base(exception) {
			this.reportSourceUrl = reportSourceUrl;
		}
		readonly string reportSourceUrl;
		public string ReportSourceUrl { get { return reportSourceUrl; } }
	}
	public class ReportStorageSubreportLoadFailedEventArgs : SubreportLoadFailedEventArgs {
		public ReportStorageSubreportLoadFailedEventArgs(Exception exception, string reportSourceUrl) : base(exception, reportSourceUrl) { }
		public Action<Action<IMessageBoxService>> MessageBoxService { get; set; }
	}
	public static class ReportStorageExtensions {
		public static void ResolveSubreports(this IReportStorage storage, XtraReport report, IReportSerializer designerReportSerializer, Action<ReportStorageSubreportLoadFailedEventArgs> onLoadError = null) {
			Guard.ArgumentNotNull(storage, "storage");
			foreach(var subreport in report.AllControls<XRSubreport>()) {
				if(string.IsNullOrEmpty(subreport.ReportSourceUrl) || (subreport.ReportSource != null && string.Equals(subreport.ReportSourceUrl, subreport.ReportSource.SourceUrl, StringComparison.Ordinal))) continue;
				XtraReport subreportReportSource;
				try {
					subreportReportSource = storage.Load(subreport.ReportSourceUrl, designerReportSerializer);
				} catch(Exception e) {
					var args = new ReportStorageSubreportLoadFailedEventArgs(e, subreport.ReportSourceUrl);
					args.Rethrow = false;
					args.ShowErrorMessage = true;
					args.ErrorMessage = storage.GetErrorMessage(e);
					if(onLoadError != null)
						onLoadError(args);
					if(args.Rethrow)
						throw;
					if(args.ShowErrorMessage)
						(args.MessageBoxService ?? (x => x(new DXMessageBoxService())))(x => x.ShowMessage(args.ErrorMessage, "Error", MessageButton.OK, MessageIcon.Error));
					subreportReportSource = new XtraReport();
				}
				subreportReportSource.SourceUrl = subreport.ReportSourceUrl;
				subreport.ReportSource = subreportReportSource;
			}
		}
	}
	public interface IReportSerializer {
		void Save(Stream stream, XtraReport report);
		XtraReport Load(Stream stream);
	}
	public static class ReportSerializerExtensions {
		public static XtraReport Load(this IReportSerializer serializer, byte[] data) {
			Guard.ArgumentNotNull(serializer, "serializer");
			return serializer.Load(new MemoryStream(data));
		}
		public static byte[] Save(this IReportSerializer serializer, XtraReport report) {
			Guard.ArgumentNotNull(serializer, "serializer");
			MemoryStream stream = new MemoryStream();
			serializer.Save(stream, report);
			return stream.ToArray();
		}
	}
}
