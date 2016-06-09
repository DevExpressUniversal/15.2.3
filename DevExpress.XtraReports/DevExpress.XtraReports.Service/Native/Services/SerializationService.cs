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
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using ExportOptions = DevExpress.XtraPrinting.ExportOptions;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class SerializationService : ISerializationService {
		static readonly string PageDataSerializationName = typeof(PageData).Name;
		static readonly Dictionary<ExportFormat, Func<ExportOptionsBase>> ExportFormatsByOptions = new Dictionary<ExportFormat, Func<ExportOptionsBase>> {
			{ ExportFormat.Csv, () => new CsvExportOptions() },
			{ ExportFormat.Htm, () => new HtmlExportOptions() },
			{ ExportFormat.Image, () => new ImageExportOptions() },
			{ ExportFormat.Mht, () => new MhtExportOptions() },
			{ ExportFormat.Rtf, () => new RtfExportOptions() },
			{ ExportFormat.Txt, () => new TextExportOptions() },
			{ ExportFormat.Xls, () => new XlsExportOptions() },
			{ ExportFormat.Xlsx, () => new XlsxExportOptions() },
			{ ExportFormat.Xps, () => new XpsExportOptions() },
			{ ExportFormat.Pdf, () => new PdfExportOptions() },
			{ ExportFormat.Prnx, () => new NativeFormatOptions() }
		};
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly DataContractSerializer arrayOfStringSerializer = new DataContractSerializer(typeof(string[]));
		readonly XtraSerializer printingSystemSerializer = new PrintingSystemXmlSerializer();
		readonly XtraSerializer xtraSerializer = new XmlXtraSerializer();
#if DEBUGTEST
		public DocumentExportArgs SerializeToDocumentExportArgsTEST(ExportOptionsBase exportOptions) {
			return new DocumentExportArgs {
				Format = ExportOptionsHelper.GetFormat(exportOptions),
				SerializedExportOptions = Serialize(exportOptions, typeof(ExportOptions).Name)
			};
		}
#endif
		#region ISerializationService
		public XtraSerializer PrintingSystemSerializer {
			get { return printingSystemSerializer; }
		}
		public byte[] Serialize(ReadonlyPageData pageData) {
			return Serialize(pageData, PageDataSerializationName);
		}
		public byte[] Serialize(PageData pageData) {
			return Serialize(pageData, PageDataSerializationName);
		}
		public PageData DeserializePageData(byte[] value) {
			if(value == null) {
				return null;
			}
			var result = new PageData();
			Deserialize(result, value, PageDataSerializationName);
			return result;
		}
		public void Serialize(string[] strings, Stream stream) {
			using(var compressedStream = new GZipStream(stream, CompressionMode.Compress, true))
			using(var writer = XmlDictionaryWriter.CreateBinaryWriter(compressedStream, null, null, false)) {
				try {
					arrayOfStringSerializer.WriteObject(writer, strings);
				} catch(Exception e) {
					Logger.Error("SerializeStrings: " + e);
					throw;
				}
				writer.Flush();
			}
		}
		public string[] DeserializeStrings(Stream stream) {
			using(var compressedStream = new GZipStream(stream, CompressionMode.Decompress))
			using(var reader = XmlDictionaryReader.CreateBinaryReader(compressedStream, XmlDictionaryReaderQuotas.Max)) {
				return (string[])arrayOfStringSerializer.ReadObject(reader);
			}
		}
		public byte[] Serialize(params int[] values) {
			var result = new byte[values.Length * sizeof(ushort)];
			var resultIndex = 0;
			foreach(var value in values) {
				try {
					ushort ushortValue = 0;
					unchecked {
						ushortValue = (ushort)value;
					}
					foreach(var singleByte in BitConverter.GetBytes(ushortValue)) {
						result[resultIndex++] = singleByte;
					}
				} catch(Exception e) {
					Logger.Error("SerializeIndexes: " + e);
					throw;
				}
			}
			return result;
		}
		public int[] DeserializeIndexes(byte[] bytes) {
			const int ElementSize = sizeof(ushort);
			var result = new int[bytes.Length / ElementSize];
			var resultIndex = 0;
			try {
				for(var valueIndex = 0; valueIndex < bytes.Length; valueIndex += ElementSize) {
					result[resultIndex++] = BitConverter.ToUInt16(bytes, valueIndex);
				}
			} catch(Exception e) {
				Logger.Error("DeserializeIndexes: " + e);
				throw;
			}
			return result;
		}
		public ExportOptionsBase Deserialize(DocumentExportArgs exportArgs) {
			Func<ExportOptionsBase> createExportOptions = null;
			ExportOptionsBase result = null;
			if(ExportFormatsByOptions.TryGetValue(exportArgs.Format, out createExportOptions)) {
				result = createExportOptions();
			} else {
				Logger.Error("Undefined export format: " + exportArgs.Format);
				result = new PdfExportOptions();
			}
			var serializedExportOptions = exportArgs.SerializedExportOptions;
			if(serializedExportOptions == null || serializedExportOptions.Length == 0) {
				return result;
			}
			Deserialize(result, serializedExportOptions, typeof(ExportOptions).Name);
			return result;
		}
		public byte[] Serialize(Watermark watermark) {
			using(var xpfWatermark = new XpfWatermark())
			using(var stream = new MemoryStream()) {
				xpfWatermark.CopyFrom(watermark);
				xpfWatermark.SaveToStream(stream);
				return stream.ToArray();
			}
		}
		public Watermark DeserializeWatermark(byte[] data) {
			if(data == null) {
				return null;
			}
			var result = new XpfWatermark();
			using(var stream = new MemoryStream(data)) {
				try {
					result.RestoreFromStream(stream);
				} catch(Exception e) {
					Logger.Error("XpfWatermark.RestoreFromStream: " + e);
				}
			}
			return result;
		}
		#endregion
		byte[] Serialize(object obj, string name) {
			using(var ms = new MemoryStream()) {
				try {
					xtraSerializer.SerializeObject(obj, ms, name);
				} catch(Exception e) {
					Logger.Error("Serialize '{0}': {1}", name, e);
					throw;
				}
				return ms.ToArray();
			}
		}
		void Deserialize(object obj, byte[] data, string name) {
			using(var ms = new MemoryStream(data)) {
				try {
					xtraSerializer.DeserializeObject(obj, ms, name);
				} catch(Exception e) {
					Logger.Error("Deserialize '{0}': {1}", name, e);
					throw;
				}
			}
		}
		public byte[] Serialize(Dictionary<string, bool> drillDownKeys) {
			var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, bool>));
			using(var stream = new MemoryStream()) {
				serializer.WriteObject(stream, drillDownKeys);
				return stream.ToArray();
			}
		}
		public Dictionary<string, bool> DeserializeDrillDownKeys(byte[] data) {
			var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, bool>));
			using(XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(data, 0, data.Length, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null)) {
				return (Dictionary<string, bool>)serializer.ReadObject(reader, false);
			}
		}
	}
}
