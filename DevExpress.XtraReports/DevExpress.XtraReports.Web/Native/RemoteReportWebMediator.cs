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
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Web.Native.DocumentViewer;
namespace DevExpress.XtraReports.Web.Native {
	public class RemoteReportWebMediator : ReportWebMediator {
		static readonly Dictionary<string, Func<ExportOptionsBase>> formatsByNames = new Dictionary<string, Func<ExportOptionsBase>>(StringComparer.OrdinalIgnoreCase) {
			{ "pdf", () => new PdfExportOptions() },
			{ "xls", () => new XlsExportOptions() },
			{ "xlsx", () => new XlsxExportOptions() },
			{ "rtf", () => new RtfExportOptions() },
			{ "mht", () => new MhtExportOptions() },
			{ "html",() => new HtmlExportOptions { EmbedImagesInHTML = true } },
			{ "txt", () => new TextExportOptions() },
			{ "csv", () => new CsvExportOptions() },
			{ "png", () => new ImageExportOptions(ImageFormat.Png) },
			{ "tiff", () => new ImageExportOptions(ImageFormat.Tiff) },
			{ "gif", () => new ImageExportOptions(ImageFormat.Gif) },
			{ "jpeg", () => new ImageExportOptions(ImageFormat.Jpeg) },
			{ "jpg", () => new ImageExportOptions(ImageFormat.Jpeg) },
			{ "bmp", () => new ImageExportOptions(ImageFormat.Bmp) }
		};
		readonly IReportWebRemoteMediator mediator;
		readonly Func<DocumentId> getDocumentId;
		public override Dictionary<string, bool> GetDrillDownKeys() {
			if(ClientParametersWereChanged || ClientDrillDownKeys == null) {
				return new Dictionary<string, bool>();
			}
			return ClientDrillDownKeys
				.Cast<DictionaryEntry>()
				.ToDictionary(x => (string)x.Key, x => (bool)x.Value);
		}
		public RemoteReportWebMediator(IReportWebRemoteMediator mediator, Hashtable clientParameters, Hashtable clientDrillDownKeys, ReportParameter[] remoteParameters, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, Func<DocumentId> getDocumentId)
			: base(null, clientParameters, clientDrillDownKeys, deserializeClientParameter, new RemoteParameterTypeProvider(remoteParameters).GetParameterType) {
			this.mediator = mediator;
			this.getDocumentId = getDocumentId;
		}
		protected override ExportOptionsBase GenerateExportOptions(string format) {
			Func<ExportOptionsBase> exportOptionsFactory;
			if(!formatsByNames.TryGetValue(format, out exportOptionsFactory)) {
				throw new ArgumentException(string.Format("Format '{0} is not supported'", format), "format");
			}
			return exportOptionsFactory();
		}
		protected override Task<ExportStreamInfo> GenerateExportInfoAsync(ExportOptionsBase options, string format, string responseContentDisposition) {
			var exportOptions = new ExportOptions();
			ExportFormat exportFormat = AssignExportOptionsAndGetFormat(exportOptions, options);
			return mediator.ExportDocumentAsync(getDocumentId(), exportFormat, exportOptions)
				.ContinueWith(t => {
					var exportedDocument = t.Result;
					var info = ExportStreamCache.CreateExportStreamInfo(options, format, responseContentDisposition);
					info.Stream = new MemoryStream(exportedDocument);
					return info;
				});
		}
		static ExportFormat AssignExportOptionsAndGetFormat(ExportOptions exportOptions, ExportOptionsBase subOptions) {
			Guard.ArgumentNotNull(exportOptions, "exportOptions");
			Guard.ArgumentNotNull(subOptions, "subOptions");
			ExportFormat? result = SafeAssignExportFormat(exportOptions, subOptions, x => x.Pdf, ExportFormat.Pdf)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Xls, ExportFormat.Xls)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Xlsx, ExportFormat.Xlsx)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Rtf, ExportFormat.Rtf)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Mht, ExportFormat.Mht)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Html, ExportFormat.Htm)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Text, ExportFormat.Txt)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Csv, ExportFormat.Csv)
				?? SafeAssignExportFormat(exportOptions, subOptions, x => x.Image, ExportFormat.Image);
			if(result == null) {
				throw new ArgumentException("Unsupported type: " + subOptions.GetType().FullName, "subOptions");
			}
			return result.Value;
		}
		static ExportFormat? SafeAssignExportFormat<T>(ExportOptions exportOptions, ExportOptionsBase subExportOptions, Func<ExportOptions, T> selectSpecificOptions, ExportFormat result)
			where T : ExportOptionsBase {
			if(subExportOptions is T) {
				selectSpecificOptions(exportOptions).Assign(subExportOptions);
				return result;
			}
			return null;
		}
	}
}
