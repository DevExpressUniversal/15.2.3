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
	public abstract class PdfColorSpace : PdfObject {
		internal const string GrayColorSpaceAbbreviation = "G";
		internal const string RgbColorSpaceAbbreviation = "RGB";
		internal const string CmykColorSpaceAbbreviation = "CMYK";
		const string indexedColorSpaceAbbreviation = "I";
		static void CheckSingleElementArray(IList<object> array) {
			if (array.Count != 1)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		static double GetDecodedValue(double value, PdfRange decodeArrayEntry) {
			double min = decodeArrayEntry.Min;
			double max = decodeArrayEntry.Max;
			if (min < max)
				value = min + (value - min) / (max - min);
			else
				value = 1.0 - max - (value - max) / (min - max);
			if (value < 0)
				value = 0;
			else if (value > 1)
				value = 1;
			return value;
		}
		internal static PdfColorSpace CreateColorSpace(string name, PdfResources resources = null) {
			switch (name) {
				case PdfDeviceColorSpace.GrayName:
				case GrayColorSpaceAbbreviation:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
				case PdfDeviceColorSpace.RGBName:
				case RgbColorSpaceAbbreviation:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.RGB);
				case PdfDeviceColorSpace.CMYKName:
				case CmykColorSpaceAbbreviation:
					return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.CMYK);
				case PdfPatternColorSpace.Name:
					return new PdfPatternColorSpace();
				default:
					return resources == null ? null : resources.GetColorSpace(name);
			}
		}
		internal static PdfColorSpace Parse(PdfObjectCollection objects, object value, PdfResources resources = null) {
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.ResolveObject<PdfColorSpace>(reference.Number, () => Parse(objects, objects.GetObjectData(reference.Number), resources));
			PdfName name = value as PdfName;
			if (name == null) {
				IList<object> array = value as IList<object>;
				if (array == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				if (array.Count == 2) {
					name = array[1] as PdfName;
					if (name != null && name.Name == PdfPatternColorSpace.Name) {
						object value2 = array[0];
						array[0] = name;
						array[1] = value2;
					}
				}
				name = array[0] as PdfName;
				if (name == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				switch (name.Name) {
					case GrayColorSpaceAbbreviation:
						if (resources == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						goto case PdfDeviceColorSpace.GrayName;
					case PdfDeviceColorSpace.GrayName:
						CheckSingleElementArray(array);
						return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
					case RgbColorSpaceAbbreviation:
						if (resources == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						goto case PdfDeviceColorSpace.RGBName;
					case PdfDeviceColorSpace.RGBName:
						CheckSingleElementArray(array);
						return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.RGB);
					case CmykColorSpaceAbbreviation:
						if (resources == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						goto case PdfDeviceColorSpace.CMYKName;
					case PdfDeviceColorSpace.CMYKName:
						CheckSingleElementArray(array);
						return new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.CMYK);
					case PdfCalGrayColorSpace.TypeName:
						return PdfCalGrayColorSpace.Create(objects, array);
					case PdfCalRGBColorSpace.TypeName:
						return PdfCalRGBColorSpace.Create(objects, array);
					case PdfLabColorSpace.TypeName:
						return PdfLabColorSpace.Create(objects, array);
					case PdfICCBasedColorSpace.TypeName:
						return new PdfICCBasedColorSpace(objects, array);
					case indexedColorSpaceAbbreviation:
						if (resources == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						goto case PdfIndexedColorSpace.TypeName;
					case PdfIndexedColorSpace.TypeName:
						return new PdfIndexedColorSpace(objects, array, resources);
					case PdfPatternColorSpace.Name:
						switch (array.Count) {
							case 1:
								return new PdfPatternColorSpace();
							case 2:
								return new PdfPatternColorSpace(Parse(objects, array[1]));
							default:
								PdfDocumentReader.ThrowIncorrectDataException();
								return null;
						}
					case PdfSeparationColorSpace.TypeName:
						return new PdfSeparationColorSpace(objects, array);
					case PdfDeviceNColorSpace.TypeName:
						if (array.Count != 5)
							return new PdfDeviceNColorSpace(objects, array);
						value = array[4];
						PdfReaderDictionary attributesDictionary = value as PdfReaderDictionary;
						if (attributesDictionary == null) {
							reference = value as PdfObjectReference;
							if (reference == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							attributesDictionary = objects.GetDictionary(reference.Number);
							if (attributesDictionary == null)
								PdfDocumentReader.ThrowIncorrectDataException();
						}
						string typeName = attributesDictionary.GetName(PdfDictionary.DictionarySubtypeKey);
						if (typeName == null)
							return new PdfDeviceNColorSpace(objects, array);
						switch (typeName) {
							case PdfDeviceNColorSpace.TypeName:
								return new PdfDeviceNColorSpace(objects, array);
							case PdfNChannelColorSpace.TypeName:
								return new PdfNChannelColorSpace(objects, array, attributesDictionary);
							default:
								PdfDocumentReader.ThrowIncorrectDataException();
								return null;
						}
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						return null;
				}
			}
			else {
				PdfColorSpace colorSpace = CreateColorSpace(name.Name, resources);
				if (colorSpace == null && resources == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return colorSpace;
			}
		}
		public abstract int ComponentsCount { get; }
		internal PdfColorSpaceTransformResult Transform(PdfImage image, byte[] data) {
			int bitsPerComponent = image.BitsPerComponent;
			IList<PdfRange> decodeArray = image.Decode;
			PdfRange[] defaultDecodeArray = CreateDefaultDecodeArray(bitsPerComponent);
			int componentsCount = ComponentsCount;
			int length = data.Length;
			int size = defaultDecodeArray.Length;
			byte startMask = (byte)(0xFF - (0xFF >> bitsPerComponent));
			int startShift = 8 - bitsPerComponent;
			if (size == decodeArray.Count) {
				double maxPossibleValue = Math.Pow(2, bitsPerComponent) - 1;
				for (int i = 0; i < size; i++)
					if (!defaultDecodeArray[i].IsSame(decodeArray[i])) {
						if (bitsPerComponent == 8)
							for (int j = 0, componentIndex = 0; j < length; j++) {
								data[j] = PdfMathUtils.ToByte(GetDecodedValue(data[j] / maxPossibleValue, decodeArray[componentIndex]) * maxPossibleValue);
								if (++componentIndex == componentsCount)
									componentIndex = 0;
							}
						else {
							if (bitsPerComponent == 1) {
								bool revert = true;
								foreach (PdfRange range in decodeArray)
									if (range.Min < range.Max) {
										revert = false;
										break;
									}
								if (revert) {
									for (int j = 0; j < length; j++)
										data[j] = (byte)(data[j] ^ 0xff);
									return Transform(data, image.Width, image.Height, bitsPerComponent, image.ColorKeyMask);
								}
							}
							for (int j = 0, componentIndex = 0; j < length; j++) {
								byte result = 0;
								byte mask = startMask;
								for (int shift = startShift; shift >= 0; shift -= bitsPerComponent, mask >>= bitsPerComponent) {
									double value = GetDecodedValue(((data[j] & mask) >> shift) / maxPossibleValue, decodeArray[componentIndex]) * maxPossibleValue;
									result += (byte)(PdfMathUtils.ToByte(value) << shift);
									if (++componentIndex == componentsCount)
										componentIndex = 0;
								}
								data[j] = result;
							}
						}
						break;
					}
			}
			return Transform(data, image.Width, image.Height, bitsPerComponent, image.ColorKeyMask);
		}
		protected internal virtual PdfColor Transform(PdfColor color) {
			return null;
		}
		protected internal virtual PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			return null;
		}
		protected internal virtual PdfRange[] CreateDefaultDecodeArray(int bitsPerComponent) {
			int componentsCount = ComponentsCount;
			PdfRange[] array = new PdfRange[componentsCount];
			for (int i = 0; i < componentsCount; i++)
				array[i] = new PdfRange(0, 1);
			return array;
		}
		protected internal abstract object Write(PdfObjectCollection collection);
	}
}
