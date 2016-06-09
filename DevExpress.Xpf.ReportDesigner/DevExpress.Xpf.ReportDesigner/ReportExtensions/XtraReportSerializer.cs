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
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions {
	public static class XtraReportSerializer {
		public static void Save(Stream stream, XtraReport report) {
			VerifyXtraReport(report);
			VerifyIsNotInDesignMode(report);
			report.SaveLayoutToXml(stream);
		}
		public static byte[] Save(XtraReport report) {
			MemoryStream stream = new MemoryStream();
			Save(stream, report);
			return stream.ToArray();
		}
		public static XtraReport Load(Stream stream) {
			var tracer = Tracer.GetSource(NativeSR.TraceSource, SourceLevels.Error);
			XtraReport report = null;
			IEnumerable<Exception> exceptions = ExceptionListener.DoAndCollectExceptions(() => report = XtraReport.FromStream(stream, true), tracer);
			if(exceptions.Any())
				throw new AggregateException(exceptions);
			return report;
		}
		public static XtraReport Load(byte[] data) {
			return Load(new MemoryStream(data));
		}
		public static XtraReport Clone(this XtraReport report) {
			VerifyXtraReport(report);
			XtraReport clone = null;
			DesignSite.DoWithDesignMode(report, x => {
				clone = x.CloneReport(true);
				if(!(clone.DataSource is DevExpress.DataAccess.DataComponentBase)) {
					clone.CopyDataProperties(x);
				}
			});
			return clone;
		}
		static void VerifyXtraReport(XtraReport report) {
			if(report == null)
				throw new ArgumentNullException("report");
		}
		static void VerifyIsNotInDesignMode(XtraReport report) {
			if(report.Site != null)
				throw new ArgumentException("", "report");
		}
	}
}
