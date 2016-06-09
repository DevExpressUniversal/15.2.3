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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraPrinting.Native {
	public static class Helper {
		public static TimeSpan DefaultStatusUpdateInterval = TimeSpan.FromMilliseconds(500);
		static DataContractSerializer pagesContractSerializer;
		static readonly object pagesContractSerializerLock = new object();
		static DataContractSerializer PagesContractSerializer {
			get {
				lock (pagesContractSerializerLock) {
					if(pagesContractSerializer == null) {
						pagesContractSerializer = new DataContractSerializer(typeof(string[]));
					}
					return pagesContractSerializer;
				}
			}
		}
		static XtraSerializer serializer;
		static Dictionary<ExportFormat, Type> exportOptionTypesByFormat;
		static Dictionary<ExportFormat, Type> ExportOptionTypesByFormat {
			get {
				if(exportOptionTypesByFormat == null) {
					exportOptionTypesByFormat = new Dictionary<ExportFormat, Type> {
						{ ExportFormat.Pdf, typeof(PdfExportOptions) },
						{ ExportFormat.Htm, typeof(HtmlExportOptions) },
						{ ExportFormat.Mht, typeof(MhtExportOptions) },
						{ ExportFormat.Rtf, typeof(RtfExportOptions) },
						{ ExportFormat.Xls, typeof(XlsExportOptions) },
						{ ExportFormat.Xlsx, typeof(XlsxExportOptions) },
						{ ExportFormat.Csv, typeof(CsvExportOptions) },
						{ ExportFormat.Txt, typeof(TextExportOptions) },
						{ ExportFormat.Image, typeof(ImageExportOptions) },
						{ ExportFormat.Xps, typeof(XpsExportOptions) },
						{ ExportFormat.Prnx, typeof(NativeFormatOptions)}
					};
				}
				return exportOptionTypesByFormat;
			}
		}
		internal static XtraSerializer Serializer {
			get {
				if(serializer == null)
					serializer = new XmlXtraSerializer();
				return serializer;
			}
		}
		public static bool IsEmpty(this IEnumerable enumerable) {
			return !enumerable.GetEnumerator().MoveNext();
		}
		public static ReportParameter[] ToParameterStubs(this IParameterContainer parameters) {
			var clientParameterContainer = parameters as ClientParameterContainer;
			if(clientParameterContainer == null)
				return new ReportParameter[0];
			return clientParameterContainer.ClientParameters.Select(p => new ReportParameter() {
				Name = p.Name,
				Path = p.Path,
				Value = p.Value,
				Visible = p.Visible,
				Description = p.Description
			}).ToArray();
		}
		public static DocumentExportArgs CreateDocumentExportArgs(ExportFormat format, ExportOptions exportOptions, object customArgs) {
			return new DocumentExportArgs {
				Format = format,
				SerializedExportOptions = SerializeExportOptions(exportOptions.GetByFormat(format)),
				CustomArgs = customArgs
			};
		}
		public static byte[] SerializeExportOptions(ExportOptionsBase exportOptions) {
			return SerializeObject(exportOptions, typeof(ExportOptions).Name);
		}
		public static void DeserializeExportOptions(ExportOptions instance, byte[] serializedData) {
			using(var stream = new MemoryStream(serializedData)) {
				instance.RestoreFromStream(stream);
			}
		}
		public static byte[] SerializePageSettings(XtraPageSettingsBase instance) {
			if(instance == null) {
				return null;
			}
			return SerializeObject(new ReadonlyPageData(instance.Data), typeof(PageData).Name);
		}
		public static void DeserializePageSettings(XtraPageSettingsBase instance, byte[] serializedData) {
			if(serializedData == null)
				return;
			var emptyPageData = new PageData();
			using(var stream = new MemoryStream(serializedData)) {
				Serializer.DeserializeObject(emptyPageData, stream, typeof(PageData).Name);
			}
			instance.Assign(emptyPageData);
		}
		static byte[] SerializeObject(object obj, string name) {
			if(obj == null) {
				return null;
			}
			using(var stream = new MemoryStream()) {
				Serializer.SerializeObject(obj, stream, name);
				return stream.ToArray();
			}
		}
		public static ExportOptionsBase GetByFormat(this ExportOptions options, ExportFormat format) {
			Type exportOptionType = null;
			if(!ExportOptionTypesByFormat.TryGetValue(format, out exportOptionType)) {
				throw new ArgumentException("format");
			}
			return options.Options[exportOptionType];
		}
		public static T[] Exclude<T>(this IEnumerable<T> list, T exclude) {
			return list.Where(element => !element.Equals(exclude)).ToArray();
		}
		public static Exception ToException(this ServiceFault fault) {
			return new Exception(fault.Message, new Exception(fault.FullMessage));
		}
		public static IEnumerable<ExportOptionKind> FlagsToList(this ExportOptionKind kinds) {
			return GetEnumValues<ExportOptionKind>()
				.Where(x => kinds.HasFlag(x));
		}
		static IEnumerable<T> GetEnumValues<T>()
			where T : struct {
			Type type = typeof(T);
			if(!type.IsEnum) {
				throw new ArgumentException(string.Format("Type '{0}' is not an enum", type.Name));
			}
			return type.GetFields()
				.Where(x => x.IsLiteral)
				.Select(x => (T)x.GetValue(type));
		}
		public static ReportBuildArgs CreateReportBuildArgs(IParameterContainer parameters, XtraPageSettingsBase pageSettings, Watermark watermark, object customArgs) {
			return CreateReportBuildArgs(parameters.ToParameterStubs(), pageSettings, watermark, customArgs);
		}
		public static ReportBuildArgs CreateReportBuildArgs(IList<Parameter> parameters, XtraPageSettingsBase pageSettings, Watermark watermark, object customArgs) {
			ReportParameter[] reportParameters = parameters.Select(x => new ReportParameter {
				Description = x.Description,
				Name = x.Name,
				Value = x.Value,
				Visible = x.Visible
			}).ToArray();
			return CreateReportBuildArgs(reportParameters, pageSettings, watermark, customArgs);
		}
		static ReportBuildArgs CreateReportBuildArgs(ReportParameter[] parameterDtoArray, XtraPageSettingsBase pageSettings, Watermark watermark, object customArgs) {
			return new ReportBuildArgs {
				Parameters = parameterDtoArray,
				SerializedPageData = SerializePageSettings(pageSettings),
				SerializedWatermark = SerializeWatermark(watermark),
				CustomArgs = customArgs
			};
		}
		static byte[] SerializeWatermark(Watermark watermark) {
			if(watermark == null)
				return null;
			using(var stream = new MemoryStream()) {
				watermark.SaveToStream(stream);
				return stream.ToArray();
			}
		}
		public static string[] DeserializePages(byte[] bytes) {
			using(var stream = new MemoryStream(bytes))
			using(var compressedStream = new GZipStream(stream, CompressionMode.Decompress))
			using(var reader = XmlDictionaryReader.CreateBinaryReader(compressedStream, XmlDictionaryReaderQuotas.Max)) {
				return (string[])PagesContractSerializer.ReadObject(reader);
			}
		}
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue) {
			TValue result;
			if(dict.TryGetValue(key, out result))
				return result;
			return defaultValue;
		}
	}
}
