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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfImage : PdfXObject {
		internal const string Type = "Image";
		internal const string WidthDictionaryKey = "Width";
		internal const string HeightDictionaryKey = "Height";
		internal const string ColorSpaceDictionaryKey = "ColorSpace";
		internal const string BitsPerComponentDictionaryKey = "BitsPerComponent";
		internal const string WidthDictionaryAbbreviation = "W";
		internal const string HeightDictionaryAbbreviation = "H";
		internal const string ColorSpaceDictionaryAbbreviation = "CS";
		internal const string BitsPerComponentDictionaryAbbreviation = "BPC";
		internal const string DecodeDictionaryKey = "Decode";
		internal const string DecodeDictionaryAbbreviation = "D";
		internal const string IntentDictionaryKey = "Intent";
		internal const string ImageMaskDictionaryKey = "ImageMask";
		internal const string ImageMaskDictionaryAbbreviation = "IM";
		internal const string InterpolateDictionaryKey = "Interpolate";
		internal const string InterpolateDictionaryAbbreviation = "I";
		const string maskDictionaryKey = "Mask";
		const string sMaskDictionaryKey = "SMask";
		const string matteDictionaryKey = "Matte";
		static byte[] ToRGB(byte[] data, PdfPixelFormat pixelFormat) {
			switch (pixelFormat) {
				case PdfPixelFormat.Gray1bit:
					return null;
				case PdfPixelFormat.Gray8bit:
					int size = data.Length;
					byte[] rgbData = new byte[size * 3];
					for (int i = 0, index = 0; i < size; i++) {
						byte b = data[i];
						rgbData[index++] = b;
						rgbData[index++] = b;
						rgbData[index++] = b;
					}
					return rgbData;
				default:
					return data;
			}
		}
		static PdfImageData ApplyMask(int width, int height, byte[] data, byte[] maskData, int maskStride) {
			int stride = width * 4;
			byte[] imageData = new byte[stride * height];
			for (int y = 0, src = 0, targetPosition = 0, maskPosition = 0; y < height; y++, maskPosition += maskStride) {
				for (int x = 0, m = maskPosition; x < width; x++, m++) {
					byte red = data[src++];
					byte green = data[src++];
					byte blue = data[src++];
					imageData[targetPosition++] = blue;
					imageData[targetPosition++] = green;
					imageData[targetPosition++] = red;
					imageData[targetPosition++] = maskData[m];
				}
			}
			return new PdfImageData(imageData, width, height, stride, PdfPixelFormat.Argb32bpp, null);
		}
		readonly IList<PdfFilter> filters;
		readonly int width;
		readonly int height;
		readonly PdfColorSpace colorSpace;
		readonly int bitsPerComponent;
		readonly PdfRenderingIntent? intent;
		readonly bool isMask;
		readonly PdfImage mask;
		readonly List<PdfRange> colorKeyMask;
		readonly IList<PdfRange> decode;
		readonly bool interpolate;
		readonly PdfImage sMask;
		readonly IList<double> matte;
		byte[] data;
		public IList<PdfFilter> Filters { get { return filters; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public PdfColorSpace ColorSpace { get { return colorSpace; } }
		public int BitsPerComponent { get { return bitsPerComponent; } }
		public PdfRenderingIntent? Intent { get { return intent; } }
		public bool IsMask { get { return isMask; } }
		public PdfImage Mask { get { return mask; } }
		public IList<PdfRange> ColorKeyMask { get { return colorKeyMask; } }
		public IList<PdfRange> Decode { get { return decode; } }
		public bool Interpolate { get { return interpolate; } }
		public PdfImage SMask { get { return sMask; } }
		public IList<double> Matte { get { return matte; } }
		public byte[] Data { get { return data; } }
		internal PdfImage(PdfReaderStream stream) : base(stream.Dictionary) {
			filters = stream.Filters;
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? imageWidth = dictionary.GetInteger(WidthDictionaryKey);
			int? imageHeight = dictionary.GetInteger(HeightDictionaryKey);
			if (!imageWidth.HasValue || !imageHeight.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			width = imageWidth.Value;
			height = imageHeight.Value;
			if (width <= 0 || height <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			object colorSpaceValue;
			if (!dictionary.TryGetValue(ColorSpaceDictionaryKey, out colorSpaceValue))
				colorSpaceValue = null;
			int? bitsPerComponentValue = dictionary.GetInteger(BitsPerComponentDictionaryKey);
			intent = CreateIntent(dictionary);
			isMask = dictionary.GetBoolean(ImageMaskDictionaryKey) ?? false;
			PdfObjectCollection objects = dictionary.Objects;
			object maskValue = dictionary.TryGetValue(maskDictionaryKey, out maskValue) ? objects.TryResolve(maskValue) : null;
			int componentsCount;
			if (isMask) {
				bitsPerComponent = bitsPerComponentValue.HasValue ? bitsPerComponentValue.Value : 1;
				if (bitsPerComponent != 1 || maskValue != null)
					PdfDocumentReader.ThrowIncorrectDataException();
				if (colorSpaceValue == null)
					colorSpace = new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
				else {
					colorSpace = PdfColorSpace.Parse(objects, colorSpaceValue);
					PdfDeviceColorSpace deviceColorSpace = colorSpace as PdfDeviceColorSpace;
					if (deviceColorSpace == null || deviceColorSpace.Kind != PdfDeviceColorSpaceKind.Gray)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				componentsCount = 1;
			}
			else {
				if (colorSpaceValue == null || !bitsPerComponentValue.HasValue) {
					int lastFilterIndex = filters.Count - 1;
					if (lastFilterIndex < 0 || !(filters[lastFilterIndex] is PdfJPXDecodeFilter))
						PdfDocumentReader.ThrowIncorrectDataException();
					componentsCount = 0;
				}
				else {
					colorSpace = PdfColorSpace.Parse(objects, colorSpaceValue);
					componentsCount = colorSpace.ComponentsCount;
					bitsPerComponent = bitsPerComponentValue.Value;
					if (bitsPerComponent != 1 && bitsPerComponent != 2 && bitsPerComponent != 4 && bitsPerComponent != 8 && bitsPerComponent != 16)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				if (maskValue != null) {
					IList<object> array = maskValue as IList<object>;
					if (array == null) {
						mask = objects.GetXObject(maskValue, null, PdfImage.Type) as PdfImage;
						if (mask == null || !mask.IsMask)
							PdfDocumentReader.ThrowIncorrectDataException();
					}
					else {
						if (bitsPerComponent == 0 || componentsCount == 0 || array.Count != componentsCount * 2)
							PdfDocumentReader.ThrowIncorrectDataException();
						int maxMaskValue = (1 << bitsPerComponent) - 1;
						colorKeyMask = new List<PdfRange>(componentsCount);
						for (int i = 0, index = 0; i < componentsCount; i++) {
							object minValue = array[index++];
							object maxValue = array[index++];
							if (!(minValue is int) || !(maxValue is int))
								PdfDocumentReader.ThrowIncorrectDataException();
							int min = (int)minValue;
							int max = (int)maxValue;
							min = min < 0 ? min = 0 : min;
							max = max > maxMaskValue ? maxMaskValue : max;
							if (max < min)
								PdfDocumentReader.ThrowIncorrectDataException();
							colorKeyMask.Add(new PdfRange(min, max));
						}
					}
				}
			}
			decode = CreateDecode(dictionary.GetArray(DecodeDictionaryKey));
			interpolate = dictionary.GetBoolean(InterpolateDictionaryKey) ?? false;
			object sMaskValue;
			if (dictionary.TryGetValue(sMaskDictionaryKey, out sMaskValue)) {
				PdfXObject xObject = objects.GetXObject(sMaskValue, null, PdfImage.Type);
				if (xObject != null) {
					sMask = xObject as PdfImage;
					PdfDeviceColorSpace deviceColorSpace = sMask.ColorSpace as PdfDeviceColorSpace;
					IList<double> matte = sMask.Matte;
					if (sMask == null || componentsCount == 0 || deviceColorSpace == null || deviceColorSpace.Kind != PdfDeviceColorSpaceKind.Gray || (matte != null && matte.Count != componentsCount))
						PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
			IList<object> matteArray = stream.Dictionary.GetArray(matteDictionaryKey);
			if (matteArray != null) {
				matte = new List<double>(matteArray.Count);
				foreach (object matteComponent in matteArray)
					matte.Add(PdfDocumentReader.ConvertToDouble(matteComponent));
			}
			data = stream.DecryptedData;
		}
		internal PdfImage(IList<PdfFilter> filters, int width, int height, PdfColorSpace colorSpace, int bitsPerComponent, bool isMask, byte[] data, PdfReaderDictionary dictionary, PdfResources resources) {
			this.filters = filters;
			this.width = width;
			this.height = height;
			this.colorSpace = colorSpace;
			this.bitsPerComponent = bitsPerComponent;
			this.isMask = isMask;
			this.data = data;
			intent = CreateIntent(dictionary);
			if (isMask)
				if (colorSpace == null)
					colorSpace = new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
				else {
					PdfDeviceColorSpace deviceColorSpace = colorSpace as PdfDeviceColorSpace;
					if (deviceColorSpace == null || deviceColorSpace.Kind != PdfDeviceColorSpaceKind.Gray)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
			else if (colorSpace == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			decode = CreateDecode(dictionary.GetArray(DecodeDictionaryAbbreviation) ?? dictionary.GetArray(DecodeDictionaryKey));
			interpolate = dictionary.GetBoolean(InterpolateDictionaryAbbreviation) ?? dictionary.GetBoolean(InterpolateDictionaryKey) ?? false;
		}
		internal void AddFiltersToDictionary(PdfObjectCollection objects, PdfDictionary dictionary) {
			int filterCount = filters.Count;
			if (filterCount > 0) {
				object[] names = new object[filterCount];
				object[] parameters = new object[filterCount];
				for (int i = 0; i < filterCount; i++) {
					PdfFilter filter = filters[i];
					names[i] = new PdfName(filter.FilterName);
					parameters[i] = filter.Write(objects);
				}
				dictionary.Add(PdfReaderStream.FilterDictionaryKey, new PdfWritableArray(names));
				dictionary.Add(PdfReaderStream.DecodeParametersDictionaryKey, new PdfWritableArray(parameters));
			}
		}
		internal PdfDecodedImageData DecodeData() {
			byte[] result = data;
			PdfImageDataType dataType;
			int lastFilterIndex = filters.Count - 1;
			if (lastFilterIndex >= 0) {
				for (int i = 0; i < lastFilterIndex; i++)
					result = filters[i].Decode(result);
				PdfFilter lastFilter = filters[lastFilterIndex];
				dataType = lastFilter.ImageDataType;
				if (dataType == PdfImageDataType.Raw)
					result = lastFilter.Decode(result);
			}
			else
				dataType = PdfImageDataType.Raw;
			if (dataType == PdfImageDataType.Raw) {
				int lineWidth = width;
				if (colorSpace != null)
					lineWidth *= colorSpace.ComponentsCount;
				if (bitsPerComponent == 1 || bitsPerComponent == 2 || bitsPerComponent == 4) {
					int divider = 8 / bitsPerComponent;
					lineWidth = lineWidth / divider + (lineWidth % divider == 0 ? 0 : 1);
				}
				else
					lineWidth *= bitsPerComponent / 8;
				if (result.Length < lineWidth * height)
					return null;
			}
			return new PdfDecodedImageData(dataType, result);
		}
		internal PdfImageData GetActualData() {
			byte[] data = PdfImageDataDecoder.Decode(this);
			if (data == null)
				return null;
			PdfPixelFormat pixelFormat;
			byte[] maskData;
			if (isMask) {
				pixelFormat = PdfPixelFormat.Gray1bit;
				maskData = null;
			}
			else {
				PdfColorSpaceTransformResult transformResult;
				transformResult = colorSpace.Transform(this, data);
				if (transformResult == null)
					return null;
				data = transformResult.Data;
				pixelFormat = transformResult.PixelFormat;
				maskData = transformResult.MaskData;
			}
			int actualWidth;
			int sourceStride;
			PdfImageColor[] palette;
			int resultComponentsCount;
			switch (pixelFormat) {
				case PdfPixelFormat.Gray1bit:
					actualWidth = width / 8;
					if (width % 8 > 0)
						actualWidth++;
					sourceStride = actualWidth;
					palette = new PdfImageColor[] { PdfImageColor.FromArgb(255, 0, 0, 0), PdfImageColor.FromArgb(255, 255, 255, 255) };
					resultComponentsCount = 1;
					break;
				case PdfPixelFormat.Gray8bit:
					palette = new PdfImageColor[256];
					for (int i = 0; i < 256; i++)
						palette[i] = PdfImageColor.FromArgb(255, (byte)i, (byte)i, (byte)i);
					resultComponentsCount = 1;
					actualWidth = width;
					sourceStride = width;
					break;
				default:
					if (maskData != null)
						return ApplyStencilMask(ApplyMask(width, height, data, maskData, width));
					palette = null;
					resultComponentsCount = 3;
					actualWidth = width;
					sourceStride = width * 3;
					break;
			}
			if (sMask != null) {
				PdfImageData result = sMask.ApplySoftMask(width, height, data, pixelFormat);
				if (result != null)
					return result;
			}
			int temp = actualWidth * resultComponentsCount;
			int remain = temp % 4;
			int stride = remain > 0 ? (temp + 4 - remain) : temp;
			int lastResultComponentIndex = resultComponentsCount - 1;
			byte[] resultComponents = new byte[resultComponentsCount];
			byte[] actualData = new byte[stride * height];
			for (int y = 0, sourcePosition = 0, targetPosition = 0; y < height; y++, sourcePosition += sourceStride, targetPosition += stride)
				for (int x = 0, trg = targetPosition, src = sourcePosition; x < actualWidth; x++) {
					for (int i = 0; i < resultComponentsCount; i++)
						resultComponents[i] = data[src++];
					for (int i = lastResultComponentIndex; i >= 0; i--)
						actualData[trg++] = resultComponents[i];
				}
			return ApplyStencilMask(new PdfImageData(actualData, width, height, stride, pixelFormat, palette));
		}
		PdfRenderingIntent? CreateIntent(PdfReaderDictionary dictionary) {
			string intentName = dictionary.GetName(IntentDictionaryKey);
			return String.IsNullOrEmpty(intentName) ? null : (PdfRenderingIntent?)PdfEnumToStringConverter.Parse<PdfRenderingIntent>(intentName);
		}
		IList<PdfRange> CreateDecode(IList<object> decodeArray) {
			if (decodeArray == null)
				return colorSpace == null ? new PdfRange[] { new PdfRange(0, 1) } : colorSpace.CreateDefaultDecodeArray(bitsPerComponent);
			int componentsCount = colorSpace == null ? 1 : colorSpace.ComponentsCount;
			if (decodeArray.Count < componentsCount * 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfRange[] result = new PdfRange[componentsCount];
			for (int i = 0, index = 0; i < componentsCount; i++) {
				double min = PdfDocumentReader.ConvertToDouble(decodeArray[index++]);
				double max = PdfDocumentReader.ConvertToDouble(decodeArray[index++]);
				result[i] = new PdfRange(min, max);
			}
			return result;
		}
		PdfImageData ApplyStencilMask(PdfImageData imageData) {
			if (mask == null)
				return imageData;
			int maskWidth = mask.Width;
			int maskHeight = mask.Height;
			int actualWidth = Math.Max(width, maskWidth);
			int actualHeight = Math.Max(height, maskHeight);
			byte[] data = ToRGB(imageData.Data, imageData.PixelFormat);
			PdfDecodedImageData decodedMaskData = mask.DecodeData();
			if (data == null || decodedMaskData == null || decodedMaskData.DataType != PdfImageDataType.Raw)
				return imageData;
			int sourceStride = data.Length / imageData.Height;
			byte[] maskData = decodedMaskData.Data;
			int componentsCount = imageData.PixelFormat == PdfPixelFormat.Argb32bpp ? 4 : 3;
			if (actualWidth > width) {
				data = PdfImageInterpolator.HorizontalInterpolation(data, width, sourceStride, actualWidth, height, componentsCount);
				sourceStride = actualWidth * componentsCount;
			}
			if (actualHeight > height)
				data = PdfImageInterpolator.VerticalInterpolation(data, actualWidth * componentsCount, height, actualHeight);
			IOpacityStream opacityStream = new PdfOpacityStream(maskWidth, mask.Decode[0], maskData);			
			if (actualWidth > maskWidth || actualHeight > maskHeight) { 
				int maskDataLength = maskWidth * maskHeight;
				byte[] interpolatedMaskData = new byte[maskDataLength];
				for (int resultPosition = 0; resultPosition < maskDataLength;) 
					interpolatedMaskData[resultPosition++] = opacityStream.GetNextValue();
				if (actualWidth > maskWidth)
					interpolatedMaskData = PdfOpacityInterpolator.HorizontalInterpolation(interpolatedMaskData, maskWidth, maskWidth, actualWidth, maskHeight, 1);
				if (actualHeight > maskHeight)
					interpolatedMaskData = PdfOpacityInterpolator.VerticalInterpolation(interpolatedMaskData, actualWidth, maskHeight, actualHeight);
				opacityStream = new PdfInterpolatedOpacityStream(interpolatedMaskData);
			}
			int stride = actualWidth * 4;
			byte[] result = new byte[stride * actualHeight];
			for (int y = 0, targetPosition = 0; y < actualHeight; y++) 
				for (int x = 0, src = sourceStride * y; x < actualWidth; x++) {
					result[targetPosition++] = data[src++];
					result[targetPosition++] = data[src++];
					result[targetPosition++] = data[src++];
					result[targetPosition++] = opacityStream.GetNextValue();
			}
			return new PdfImageData(result, actualWidth, actualHeight, stride, PdfPixelFormat.Argb32bpp, null);
		}
		PdfImageData ApplySoftMask(int imageWidth, int imageHeight, byte[] data, PdfPixelFormat pixelFormat) {
			data = ToRGB(data, pixelFormat);
			PdfDecodedImageData sMaskImageData = DecodeData();
			if (data == null || sMaskImageData == null)
				return null;
			byte[] softMaskData;
			int softMaskStride;
			switch (sMaskImageData.DataType) {
				case PdfImageDataType.DCTDecode:
					PdfDCTDecodeResult decodeResult = PdfDCTDecoder.Decode(sMaskImageData.Data, width, height);
					softMaskData = decodeResult.Data;
					softMaskStride = decodeResult.Stride;
					break;
				case PdfImageDataType.JPXDecode:
					return null;
				default:
					softMaskData = sMaskImageData.Data;
					softMaskStride = width;
					if (bitsPerComponent == 1) {
						byte[] result = new byte[softMaskData.Length * 8];
						int position = 0;
						int rowPosition = 0;
						foreach (byte currentByte in softMaskData) {
							byte b = currentByte;
							for (int bit = 0; bit < 8; bit++, b <<= 1) {
								result[position++] = (b & 0x80) != 0 ? (byte)255 : (byte)0;
								if (++rowPosition == width) {
									rowPosition = 0;
									break;
								}
							}
						}
						softMaskData = result;
					}
					break;
			}
			PdfColorSpaceTransformResult transformResult = colorSpace.Transform(this, softMaskData);
			if (transformResult == null)
				return null;
			softMaskData = transformResult.Data;
			int fullWidth = Math.Max(width, imageWidth);
			int fullHeight = Math.Max(height, imageHeight);
			if (fullWidth > imageWidth)
				data = PdfImageInterpolator.HorizontalInterpolation(data, imageWidth, imageWidth * 3, fullWidth, imageHeight, 3);
			else if (fullWidth > width) {
				softMaskData = PdfImageInterpolator.HorizontalInterpolation(softMaskData, width, softMaskStride, fullWidth, height, 1);
				softMaskStride = fullWidth;
			}
			if (fullHeight > imageHeight)
				data = PdfImageInterpolator.VerticalInterpolation(data, fullWidth * 3, imageHeight, fullHeight);
			else if (fullHeight > height)
				softMaskData = PdfImageInterpolator.VerticalInterpolation(softMaskData, softMaskStride, height, fullHeight);
			return ApplyMask(fullWidth, fullHeight, data, softMaskData, softMaskStride);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionarySubtypeKey, Type);
			dictionary.Add(WidthDictionaryKey, width);
			dictionary.Add(HeightDictionaryKey, height);
			if (isMask)
				dictionary.Add(ImageMaskDictionaryKey, isMask);
			else {
				if (colorSpace != null)
					dictionary.Add(ColorSpaceDictionaryKey, colorSpace.Write(objects));
				dictionary.Add(BitsPerComponentDictionaryKey, bitsPerComponent, 0);
				if (mask != null)
					dictionary.Add(maskDictionaryKey, mask);
				else if (colorKeyMask != null)
					dictionary.Add(maskDictionaryKey, PdfRange.ToArray(colorKeyMask));
			}
			if (intent.HasValue)
				dictionary.AddName(IntentDictionaryKey, PdfEnumToStringConverter.Convert(intent.Value, false));
			dictionary.Add(DecodeDictionaryKey, PdfRange.ToArray(decode));
			dictionary.Add(InterpolateDictionaryKey, interpolate);
			dictionary.Add(sMaskDictionaryKey, sMask);
			dictionary.Add(matteDictionaryKey, matte);
			AddFiltersToDictionary(objects, dictionary);
			return dictionary;
		}
		protected override PdfStream CreateStream(PdfObjectCollection objects) {
			return new PdfStream(CreateDictionary(objects), data);
		}
	}
}
