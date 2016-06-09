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
	[PdfDefaultField(PdfIncludedImageQuality.High)]
	public enum PdfIncludedImageQuality { 
		[PdfFieldValue(1)]
		Low,
		[PdfFieldValue(2)]
		Normal,
		[PdfFieldValue(3)]
		High
	}
	public class PdfOpenPrepressInterface : PdfObject {
		const string dictionaryType = "OPI";
		const string version13DictionaryKey = "1.3";
		const string version20DictionaryKey = "2.0";
		const string versionDictionaryKey = "Version";
		const string fileSpecificationDictionaryKey = "F";
		const string sizeDictionaryKey = "Size";
		const string cropRectDictionaryKey = "CropRect";
		const string overprintDictionaryKey = "Overprint";
		const string cropFixedDictionaryKey = "CropFixed";
		const string positionDictionaryKey = "Position";
		const string resolutionDictionaryKey = "Resolution";
		const string tintDictionaryKey = "Tint";
		const string imageTypeDictionaryKey = "ImageType";
		const string transparencyDictionaryKey = "Transparency";
		const string inksDictionaryKey = "Inks";
		const string includedImageQualityDictionaryKey = "IncludedImageQuality";
		const string monochromeInkName = "monochrome";
		internal static PdfOpenPrepressInterface Create(PdfReaderDictionary dictionary) {
			if (dictionary == null || dictionary.Count < 1)
				return null;
			object value;
			if (!dictionary.TryGetValue(version13DictionaryKey, out value) && !dictionary.TryGetValue(version20DictionaryKey, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfReaderDictionary opiDictionary = value as PdfReaderDictionary;
			if (opiDictionary != null)
				return new PdfOpenPrepressInterface(opiDictionary);
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfObjectCollection objects = dictionary.Objects;
			return objects.ResolveObject<PdfOpenPrepressInterface>(reference.Number, () => {
				PdfReaderDictionary resolvedObject = objects.GetObjectData(reference.Number) as PdfReaderDictionary;
				if (resolvedObject == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return new PdfOpenPrepressInterface(resolvedObject);
			});
		}
		readonly double version;
		readonly PdfFileSpecification fileSpecification;
		readonly double width;
		readonly double height;
		readonly PdfRectangle cropRect;
		readonly bool overprint;
		readonly PdfRectangle cropFixed;
		readonly PdfParallelogram position;
		readonly double horizontalResolution;
		readonly double verticalResolution;
		readonly double tint = 1.0;
		readonly int samplesPerPixel;
		readonly int bitsPerSample;
		readonly bool transparency = true;
		readonly string inksName;
		readonly Dictionary<string, double> inks;
		readonly PdfIncludedImageQuality includedImageQuality = PdfIncludedImageQuality.High;
		public PdfFileSpecification FileSpecification { get { return fileSpecification; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public PdfRectangle CropRect { get { return cropRect; } }
		public bool Overprint { get { return overprint; } }
		public PdfRectangle CropFixed { get { return cropFixed; } }
		public PdfParallelogram Position { get { return position; } }
		public double HorizontalResolution { get { return horizontalResolution; } }
		public double VerticalResolution { get { return verticalResolution; } }
		public double Tint { get { return tint; } }
		public int SamplesPerPixel { get { return samplesPerPixel; } }
		public int BitsPerSample { get { return bitsPerSample; } }
		public bool Transparency { get { return transparency; } }
		public string InksName { get { return inksName; } }
		public IDictionary<string, double> Inks { get { return inks; } }
		public PdfIncludedImageQuality IncludedImageQuality { get { return includedImageQuality; } }
		PdfOpenPrepressInterface(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			double? versionValue = dictionary.GetNumber(versionDictionaryKey);
			if ((type != null && type != dictionaryType) || !versionValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			version = versionValue.Value;
			bool isVersion13 = version == 1.3;
			if (!isVersion13 && version != 2.0)
				PdfDocumentReader.ThrowIncorrectDataException();
			fileSpecification = PdfFileSpecification.Parse(dictionary, fileSpecificationDictionaryKey, true);
			IList<object> s = dictionary.GetArray(sizeDictionaryKey);
			if (s == null) {
				if (isVersion13)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				if (s.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				width = PdfDocumentReader.ConvertToDouble(s[0]);
				height = PdfDocumentReader.ConvertToDouble(s[1]);
			}
			IList<object> cr = dictionary.GetArray(cropRectDictionaryKey);
			if (cr == null) {
				if (isVersion13)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else
				cropRect = new PdfRectangle(cr);
			overprint = dictionary.GetBoolean(overprintDictionaryKey) ?? false;
			if (isVersion13) {
				IList<object> cf = dictionary.GetArray(cropFixedDictionaryKey);
				if (cf != null)
					cropFixed = new PdfRectangle(cf);
				IList<object> pos = dictionary.GetArray(positionDictionaryKey);
				if (pos == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				position = new PdfParallelogram(pos);
				IList<object> resolution = dictionary.GetArray(resolutionDictionaryKey);
				if (resolution != null) {
					if (resolution.Count != 2)
						PdfDocumentReader.ThrowIncorrectDataException();
					horizontalResolution = PdfDocumentReader.ConvertToDouble(resolution[0]);
					verticalResolution = PdfDocumentReader.ConvertToDouble(resolution[1]);
				}
				double? t = dictionary.GetNumber(tintDictionaryKey);
				if (t.HasValue) {
					tint = t.Value;
					if (tint < 0.0 || tint > 1.0)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				IList<object> imageType = dictionary.GetArray(imageTypeDictionaryKey);
				if (imageType != null) {
					if (imageType.Count != 2)
						PdfDocumentReader.ThrowIncorrectDataException();
					object samplesPerPixelValue = imageType[0];
					object bitsPerSampleValue = imageType[1];
					if (!(samplesPerPixelValue is int) || !(bitsPerSampleValue is int))
						PdfDocumentReader.ThrowIncorrectDataException();
					samplesPerPixel = (int)samplesPerPixelValue;
					bitsPerSample = (int)bitsPerSampleValue;
				}
				transparency = dictionary.GetBoolean(transparencyDictionaryKey) ?? true;
			}
			else {
				object value;
				if (dictionary.TryGetValue(inksDictionaryKey, out value)) {
					IList<object> inksArray = value as IList<object>;
					if (inksArray == null) {
						PdfName name = value as PdfName;
						if (name == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						inksName = name.Name;
					}
					else {
						int count = inksArray.Count;
						if (count == 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						count -= 1;
						if (count % 2 != 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						count /= 2;
						PdfName monochromeInk = inksArray[0] as PdfName;
						if (monochromeInk == null || monochromeInk.Name != monochromeInkName)
							PdfDocumentReader.ThrowIncorrectDataException();
						inks = new Dictionary<string, double>(count);
						for (int i = 0, index = 1; i < count; i++) {
							byte[] str = inksArray[index++] as byte[];
							if (str == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							inks.Add(PdfDocumentReader.ConvertToString(str), PdfDocumentReader.ConvertToDouble(inksArray[index++]));
						}
					}
				}
				includedImageQuality = PdfEnumToValueConverter.Parse<PdfIncludedImageQuality>(dictionary.GetInteger(includedImageQualityDictionaryKey), PdfIncludedImageQuality.High);
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(versionDictionaryKey, version);
			dictionary.Add(fileSpecificationDictionaryKey, fileSpecification);
			dictionary.Add(overprintDictionaryKey, overprint, false);
			if (version == 2.0) {
				if (width != 0 || height != 0)
					dictionary.Add(sizeDictionaryKey, new double[] { width, height });
				if (cropRect != null)
					dictionary.Add(cropRectDictionaryKey, cropRect);
				if (String.IsNullOrEmpty(inksName)) {
					if (inks != null) {
						IList<object> inksArray = new List<object>();
						inksArray.Add(new PdfName(monochromeInkName));
						foreach (KeyValuePair<string, double> ink in inks) {
							inksArray.Add(ink.Key);
							inksArray.Add(ink.Value);
						}
						dictionary.Add(inksDictionaryKey, inksArray);
					}
				}
				else 
					dictionary.AddName(inksDictionaryKey, inksName);
				dictionary.Add(includedImageQualityDictionaryKey, PdfEnumToValueConverter.Convert(includedImageQuality, PdfIncludedImageQuality.High));
			}
			else {
				dictionary.Add(sizeDictionaryKey, new object[] { (int)width, (int)height });
				dictionary.Add(cropRectDictionaryKey, new object[] { (int)cropRect.Left, (int)cropRect.Bottom, (int)cropRect.Right, (int)cropRect.Top });
				dictionary.Add(cropFixedDictionaryKey, cropFixed);
				dictionary.Add(positionDictionaryKey, position.ToWriteableObject());
				if (horizontalResolution != 0 && verticalResolution != 0)
					dictionary.Add(resolutionDictionaryKey, new double[] { horizontalResolution, verticalResolution });
				dictionary.Add(tintDictionaryKey, tint, 1.0);
				if (samplesPerPixel != 0 || bitsPerSample != 0)
					dictionary.Add(imageTypeDictionaryKey, new object[] { samplesPerPixel, bitsPerSample });
				dictionary.Add(transparencyDictionaryKey, transparency, true);
			}
			PdfDictionary versionDictionary = new PdfDictionary();
			versionDictionary.Add(version == 1.3 ? version13DictionaryKey : version20DictionaryKey, dictionary);
			return versionDictionary;
		}
	}
}
