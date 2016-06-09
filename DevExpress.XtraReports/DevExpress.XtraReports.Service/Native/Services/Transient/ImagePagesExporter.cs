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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class ImagePagesExporter : IPagesExporter {
		ImageFormat imageFormat = ImageFormat.Png;
		int resolution = 96;
		public ImageFormat ImageFormat {
			get { return imageFormat; }
			set {
				Guard.ArgumentNotNull(value, "ImageFormat");
				imageFormat = value;
			}
		}
		public int Resolution {
			get { return resolution; }
			set {
				if(value <= 0) {
					throw new ArgumentOutOfRangeException("value");
				}
				resolution = value;
			}
		}
		#region IExportWrapper Members
		public bool ExclusivelyDocumentUsing {
			get { return true; }
		}
		public string Export(Document document, int pageIndex) {
			return Convert.ToBase64String(ExportCore(document, pageIndex));
		}
		public byte[] Export(Document document, int[] pageIndexes) {
			var result = new byte[pageIndexes.Length][];
			for(int i = 0; i < pageIndexes.Length; i++) {
				var pageIndex = pageIndexes[i];
				result[i] = ExportCore(document, pageIndex);
			}
			return Serialize(result);
		}
		#endregion
		byte[] ExportCore(Document document, int pageIndex) {
			var exportOptions = new ImageExportOptions(ImageFormat) {
				ExportMode = ImageExportMode.SingleFilePageByPage,
				PageRange = (pageIndex + 1).ToString(),
				PageBorderColor = Color.Transparent,
				PageBorderWidth = 0
			};
			exportOptions.Resolution = Resolution;
			var ps = document.PrintingSystem;
			using(var stream = new MemoryStream()) {
				ps.ReplaceService<IBrickPublisher>(new DefaultBrickPublisher());
				try {
					ps.ExportToImage(stream, exportOptions);
				} finally {
					ps.RemoveService<IBrickPublisher>();
				}
				return stream.ToArray();
			}
		}
		static byte[] Serialize(byte[][] graph) {
			var serializer = new DataContractSerializer(typeof(byte[][]));
			using(var stream = new MemoryStream())
			using(var writer = XmlDictionaryWriter.CreateBinaryWriter(stream)) {
				serializer.WriteObject(writer, graph);
				writer.Flush();
				return stream.ToArray();
			}
		}
	}
}
