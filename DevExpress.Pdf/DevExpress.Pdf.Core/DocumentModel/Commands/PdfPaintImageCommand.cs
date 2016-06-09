#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf {
	public class PdfPaintImageCommand : PdfCommand {
		readonly PdfImage image;
		public PdfImage Image { get { return image; } }
		internal PdfPaintImageCommand(PdfImage image) {
			this.image = image;
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			interpreter.DrawImage(image);
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteString("BI");
			writer.WriteSpace();
			PdfColorSpace colorSpace = image.ColorSpace;
			if (colorSpace != null) {
				writer.WriteName(new PdfName(PdfImage.ColorSpaceDictionaryAbbreviation));
				writer.WriteSpace();
				object colorSpaceObject = resources.FindColorSpaceName(colorSpace);
				if (colorSpaceObject == null) {
					colorSpaceObject = colorSpace.ToWritableObject(null);
					PdfName colorSpaceName = colorSpaceObject as PdfName;
					if (colorSpaceName != null)
						switch (colorSpaceName.Name) {
							case PdfDeviceColorSpace.GrayName:
								colorSpaceObject = new PdfName(PdfDeviceColorSpace.GrayColorSpaceAbbreviation);
								break;
							case PdfDeviceColorSpace.RGBName:
								colorSpaceObject = new PdfName(PdfDeviceColorSpace.RgbColorSpaceAbbreviation);
								break;
							case PdfDeviceColorSpace.CMYKName:
								colorSpaceObject = new PdfName(PdfDeviceColorSpace.CmykColorSpaceAbbreviation);
								break;
						}
				}
				writer.WriteObject(colorSpaceObject, -1);
				writer.WriteSpace();
			}
			writer.WriteName(new PdfName(PdfImage.WidthDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteInt(image.Width);
			writer.WriteSpace();
			writer.WriteName(new PdfName(PdfImage.HeightDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteInt(image.Height);
			writer.WriteSpace();
			writer.WriteName(new PdfName(PdfImage.BitsPerComponentDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteInt(image.BitsPerComponent);
			writer.WriteSpace();
			writer.WriteName(new PdfName(PdfImage.DecodeDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteObject(PdfRange.ToArray(image.Decode), -1);
			writer.WriteSpace();
			writer.WriteName(new PdfName(PdfImage.ImageMaskDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteObject(image.IsMask, -1);
			writer.WriteSpace();
			if (image.Intent.HasValue) {
				writer.WriteName(new PdfName(PdfImage.IntentDictionaryKey));
				writer.WriteSpace();
				writer.WriteName(new PdfName(PdfEnumToStringConverter.Convert(image.Intent.Value, false)));
				writer.WriteSpace();
			}
			writer.WriteName(new PdfName(PdfImage.InterpolateDictionaryAbbreviation));
			writer.WriteSpace();
			writer.WriteObject(image.Interpolate, -1);
			writer.WriteSpace();
			PdfDictionary filtersDictionary = new PdfDictionary();
			image.AddFiltersToDictionary(null, filtersDictionary);
			object filters;
			if (filtersDictionary.TryGetValue(PdfReaderStream.FilterDictionaryKey, out filters)) {
				writer.WriteName(new PdfName("F"));
				writer.WriteSpace();
				IEnumerable list = filters as IEnumerable;
				if (list == null)
					filters = ConvertFilter(filters);
				else {
					IList<object> result = new List<object>();
					foreach (object filter in list)
						result.Add(ConvertFilter(filter));
					filters = new PdfWritableArray(result);
				}
				writer.WriteObject(filters, -1);
				writer.WriteSpace();
			}
			object filterParams;
			if (filtersDictionary.TryGetValue(PdfReaderStream.DecodeParametersDictionaryKey, out filterParams)) {
				IEnumerable filterParamsList = filterParams as IEnumerable;
				bool writeParams = false;
				foreach (object param in filterParamsList)
					if (param != null) {
						writeParams = true;
						break;
					}
				if (writeParams) {
					writer.WriteName(new PdfName("DP"));
					writer.WriteSpace();
					writer.WriteObject(filterParams, -1);
					writer.WriteSpace();
				}
			}
			writer.WriteString("ID");
			writer.WriteSpace();
			writer.WriteBytes(image.Data);
			writer.WriteSpace();
			writer.WriteString("EI");
		}
		object ConvertFilter(object obj) {
			PdfName filterName = obj as PdfName;
			if (filterName != null) 
				switch (filterName.Name) {
					case PdfASCIIHexDecodeFilter.Name:
						return new PdfName(PdfASCIIHexDecodeFilter.ShortName);
					case PdfASCII85DecodeFilter.Name:
						return new PdfName(PdfASCII85DecodeFilter.ShortName);
					case PdfLZWDecodeFilter.Name:
						return new PdfName(PdfLZWDecodeFilter.ShortName);
					case PdfFlateDecodeFilter.Name:
						return new PdfName(PdfFlateDecodeFilter.ShortName);
					case PdfRunLengthDecodeFilter.Name:
						return new PdfName(PdfRunLengthDecodeFilter.ShortName);
					case PdfCCITTFaxDecodeFilter.Name:
						return new PdfName(PdfCCITTFaxDecodeFilter.ShortName);
					case PdfDCTDecodeFilter.Name:
						return new PdfName(PdfDCTDecodeFilter.ShortName);
				}
			return obj;
		}
	}
}
