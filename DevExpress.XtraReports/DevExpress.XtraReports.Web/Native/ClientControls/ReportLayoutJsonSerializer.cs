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

using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class ReportLayoutJsonSerializer {
		public static bool ShouldClearScripts { get; set; }
		public static string GetExportOptionsJson(ExportOptions exportOptions) {
			using(var stream = new MemoryStream()) {
				exportOptions.SaveToStream(stream);
				return JsonConverter.XmlToJson(stream, includeRootTag: false);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		public static string GenerateReportLayoutJson(XtraReport report, out XtraReportsSerializationContext serializationContext) {
			FixReport(report);
			using(var stream = new MemoryStream()) {
				var serializer = new XtraReportsXmlSerializer(report);
				serializer.SerializeRootObject(report, stream);
				serializationContext = serializer.Context;
				stream.Position = 0;
				return JsonConverter.XmlToJson(stream);
			}
		}
		public static XtraReport CreateReportFromJson(string json) {
			var xmlReportLayoutBytes = JsonConverter.JsonToXml(json);
			using(var stream = new MemoryStream(xmlReportLayoutBytes)) {
				var report = XtraReport.FromStream(stream, true);
				SafeProcessScripts(report);
				return report;
			}
		}
		public static byte[] LoadFromJsonAndReturnXml(string json) {
			if(!ShouldClearScripts) {
				return JsonConverter.JsonToXml(json);
			}
			using(var report = CreateReportFromJson(json))
			using(var stream = new MemoryStream()) {
				report.SaveLayoutToXml(stream);
				return stream.ToArray();
			}
		}
		static void FixReport(XtraReport report) {
			if(string.IsNullOrEmpty(report.Name)) {
				report.Name = report.GetType().Name;
			}
		}
		static void SafeProcessScripts(XtraReport report) {
			if(ShouldClearScripts) {
				report.ScriptsSource = "";
			}
		}
	}
}
