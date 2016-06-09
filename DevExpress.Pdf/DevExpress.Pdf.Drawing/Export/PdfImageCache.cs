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

using DevExpress.Pdf.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace DevExpress.Pdf.Drawing {
	public class PdfImageCache {
		const float pdfDpi = 72f;
		static byte[] ReadToEnd(Stream stream) {
			int length = (int)stream.Length;
			byte[] data = new byte[length];
			stream.Read(data, 0, length);
			return data;
		}
		static PdfReaderDictionary GetImageDictionary(int width, int height, int colors, string filter) {
			PdfObjectCollection collection = new PdfObjectCollection(null);
			PdfReaderDictionary imageDictionary = new PdfReaderDictionary(collection, PdfObject.DirectObjectNumber, 0);
			imageDictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName("Image"));
			imageDictionary.Add("Width", width);
			imageDictionary.Add("Height", height);
			imageDictionary.Add("BitsPerComponent", 8);
			imageDictionary.Add("ColorSpace", new PdfName(colors == 1 ? "DeviceGray" : "DeviceRGB"));
			imageDictionary.Add("Filter", new PdfName(filter));
			if (!filter.Equals("DCTDecode")) {
				PdfReaderDictionary parameters = new PdfReaderDictionary(collection, PdfObject.DirectObjectNumber, 0);
				parameters.Add("Predictor", 12);
				parameters.Add("Columns", width);
				parameters.Add("Colors", colors);
				parameters.Add("BitsPerComponent", 8);
				imageDictionary.Add("DecodeParms", parameters);
			}
			return imageDictionary;
		}
		static PdfImage CreatePdfImage(Stream stream, Func<byte[]> getData, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality, Size size) {
			if (!size.IsEmpty)
				return new PdfImage(new PdfReaderStream(GetImageDictionary(size.Width, size.Height, 3, "DCTDecode"), getData()));
			using (Image image = Image.FromStream(stream))
				return CreatePdfImage(image, convert, jpegCodec, jpegImageQuality);
		}
		public static PdfImage CreatePdfImage(byte[] data, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality, Size size) {
			using (MemoryStream stream = new MemoryStream(data))
				return CreatePdfImage(stream, () => data, convert, jpegCodec, jpegImageQuality, size);
		}
		public static PdfImage CreatePdfImage(Stream stream, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality, Size size) {
			return CreatePdfImage(stream, () => ReadToEnd(stream), convert, jpegCodec, jpegImageQuality, size);
		}
		public static PdfImage CreatePdfImage(Image img, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality) {
			int width = img.Width;
			int height = img.Height;
			if (convert) {
				using (MemoryStream ms = new MemoryStream()) {
					EncoderParameters encoderParms = new EncoderParameters(1);
					encoderParms.Param[0] = new EncoderParameter(Encoder.Quality, (long)jpegImageQuality);
					img.Save(ms, jpegCodec, encoderParms);
					ms.Position = 0;
					return new PdfImage(new PdfReaderStream(GetImageDictionary(width, height, 3, "DCTDecode"), ms.ToArray()));
				}
			}
			else {
				PdfImageConverter imageConverter = PdfImageConverter.Create(img);
				byte[] imageData = PdfFlateEncoder.Encode(imageConverter.ImageData);
				PdfReaderDictionary image = GetImageDictionary(width, height, 3, "FlateDecode");
				if (imageConverter.HasMask) {
					byte[] sMask = PdfFlateEncoder.Encode(imageConverter.SMask);
					image.Add("SMask", new PdfReaderStream(GetImageDictionary(width, height, 1, "FlateDecode"), sMask));
				}
				return new PdfImage(new PdfReaderStream(image, imageData));
			}
		}
		readonly Dictionary<int, List<Tuple<WeakReference, int>>> cache = new Dictionary<int, List<Tuple<WeakReference, int>>>();
		readonly PdfDocumentCatalog documentCatalog;
		internal PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		public PdfImageCache(PdfObjectCollection objects) {
			this.documentCatalog = objects.DocumentCatalog;
		}
		public int GetPdfFormObjectNumber(Metafile metafile) {
			return CacheXObject<Metafile>(metafile, () => {
				using (EmfMetafile meta = new EmfMetafile(metafile)) {
					PdfObjectCollection objects = documentCatalog.Objects;
					float factorX = meta.DpiX / pdfDpi;
					float factorY = meta.DpiY / pdfDpi;
					PdfForm form = new PdfForm(objects.DocumentCatalog, new PdfRectangle(0, 0, metafile.Width / factorX, metafile.Height / factorY));
					using (PdfEditableFontDataCache fontCache = new PdfEditableFontDataCache(objects)) {
						PdfGraphicsFormContentCommandConstructor constructor = new PdfGraphicsFormContentCommandConstructor(form, factorX, factorY, fontCache, this);
						meta.Draw(constructor);
						fontCache.UpdateFonts();
						return form;
					}
				}
			});
		}
		public int GetPdfImageObjectNumber(byte[] data, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality, Size size) {
			return GetPdfImageObjectNumber(data, () => CreatePdfImage(data, convert, jpegCodec, jpegImageQuality, size));
		}
		public int GetPdfImageObjectNumber(Stream stream, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality, Size size) {
			return GetPdfImageObjectNumber(stream, () => CreatePdfImage(stream, convert, jpegCodec, jpegImageQuality, size));
		}
		public int GetPdfImageObjectNumber(Image image, bool convert, ImageCodecInfo jpegCodec, long jpegImageQuality) {
			return GetPdfImageObjectNumber(image, () => CreatePdfImage(image, convert, jpegCodec, jpegImageQuality));
		}
		public int GetPdfImageObjectNumber<T>(T source, Func<PdfXObject> create) where T : class {
			return CacheXObject<T>(source, create);
		}
		public bool CheckCollectionId(Guid id) {
			return documentCatalog == null ? true : documentCatalog.Objects.Id == id;
		}
		int CacheXObject<T>(T image, Func<PdfXObject> createItem) where T : class {
			PdfObjectCollection objects = documentCatalog.Objects;
			int hash = image.GetHashCode();
			List<Tuple<WeakReference, int>> list;
			if (cache.TryGetValue(hash, out list)) {
				foreach (Tuple<WeakReference, int> tuple in list) {
					WeakReference reference = tuple.Item1;
					T existing = reference.Target as T;
					if (reference.IsAlive && image == existing)
						return tuple.Item2;
				}
				int res = objects.AddResolvedObject(createItem());
				list.Add(Tuple.Create(new WeakReference(image), res));
				return res;
			}
			int objectNumber = objects.AddResolvedObject(createItem());
			cache.Add(hash, new List<Tuple<WeakReference, int>>() { Tuple.Create(new WeakReference(image), objectNumber) });
			return objectNumber;
		}
	}
}
