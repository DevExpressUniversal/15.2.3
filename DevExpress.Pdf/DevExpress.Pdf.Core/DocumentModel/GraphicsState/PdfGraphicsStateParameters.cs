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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfGraphicsStateParameters : PdfObject {
		const string dictionaryType = "ExtGState";
		const string lineWidthDictionaryKey = "LW";
		const string lineCapDictionaryKey = "LC";
		const string lineJoinDictionaryKey = "LJ";
		const string miterLimitDictionaryKey = "ML";
		const string dashPatternDictionaryKey = "D";
		const string renderingIntentDictionaryKey = "RI";
		const string strokingOverprintDictionaryKey = "OP";
		const string nonStrokingOverprintDictionaryKey = "op";
		const string overprintModeDictionaryKey = "OPM";
		const string fontDictionaryKey = "Font";
		const string blackGenerationFunctionDictionaryKey = "BG2";
		const string undercolorRemovalFunctionDictionaryKey = "UCR2";
		const string transferFunctionDictionaryKey = "TR2";
		const string halftoneDictionaryKey = "HT";
		const string flatnessToleranceDictionaryKey = "FL";
		const string smoothnessToleranceDictionaryKey = "SM";
		const string strokeAdjustmentDictionaryKey = "SA";
		const string softMaskDictionaryKey = "SMask";
		const string strokingAlphaConstantDictionaryKey = "CA";
		const string nonStrokingAlphaConstantDictionaryKey = "ca";
		const string alphaSourceDictionaryKey = "AIS";
		const string textKnockoutDictionaryKey = "TK";
		const string blendModeDictionaryKey = "BM";
		static PdfFunction ResolveFunction(PdfReaderDictionary dictionary, string key, bool expectDefault) {
			object value;
			return dictionary.TryGetValue(key, out value) ? PdfFunction.Parse(dictionary.Objects, value, expectDefault) : null;
		}
		static PdfFunction[] ResolveFunctions(PdfReaderDictionary dictionary, string key, bool expectDefault) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			PdfObjectCollection objects = dictionary.Objects;
			IList<object> array = value as IList<object>;
			if (array == null)
				return new PdfFunction[] { PdfFunction.Parse(objects, value, expectDefault) };
			int count = array.Count;
			PdfFunction[] result = new PdfFunction[count];
			for (int i = 0; i < count; i++)
				result[i] = PdfFunction.Parse(objects, array[i], expectDefault);
			return result;
		}
		double? lineWidth;
		PdfLineCapStyle? lineCap;
		PdfLineJoinStyle? lineJoin;
		double? miterLimit;
		PdfLineStyle lineStyle;
		PdfRenderingIntent? renderingIntent;
		bool? strokingOverprint;
		bool? nonStrokingOverprint;
		PdfOverprintMode? overprintMode;
		PdfFont font;
		double? fontSize;
		PdfFunction blackGenerationFunction;
		PdfFunction undercolorRemovalFunction;
		PdfFunction[] transferFunction;
		PdfHalftone halftone;
		double? flatnessTolerance;
		double? smoothnessTolerance;
		bool? strokeAdjustment;
		PdfBlendMode? blendMode;
		PdfSoftMask softMask;
		double? strokingAlphaConstant;
		double? nonStrokingAlphaConstant;
		bool? alphaSource;
		bool? textKnockout;
		public double? LineWidth {
			get { return lineWidth; }
			set { lineWidth = value; }
		}
		public PdfLineCapStyle? LineCap {
			get { return lineCap; }
			set { lineCap = value; }
		}
		public PdfLineJoinStyle? LineJoin {
			get { return lineJoin; }
			set { lineJoin = value; }
		}
		public double? MiterLimit {
			get { return miterLimit; }
			set { miterLimit = value; }
		}
		public PdfLineStyle LineStyle {
			get { return lineStyle; }
			set { lineStyle = value; }
		}
		public PdfRenderingIntent? RenderingIntent {
			get { return renderingIntent; }
			set { renderingIntent = value; }
		}
		public bool? StrokingOverprint {
			get { return strokingOverprint; }
			set { strokingOverprint = value; }
		}
		public bool? NonStrokingOverprint {
			get { return nonStrokingOverprint; }
			set { nonStrokingOverprint = value; }
		}
		public PdfOverprintMode? OverprintMode {
			get { return overprintMode; }
			set { overprintMode = value; }
		}
		public PdfFont Font {
			get { return font; }
			set { font = value; }
		}
		public double? FontSize {
			get { return fontSize; }
			set { fontSize = value; }
		}
		public PdfFunction BlackGenerationFunction {
			get { return blackGenerationFunction; }
			set { blackGenerationFunction = value; }
		}
		public PdfFunction UndercolorRemovalFunction {
			get { return undercolorRemovalFunction; }
			set { undercolorRemovalFunction = value; }
		}
		public PdfFunction[] TransferFunction {
			get { return transferFunction; }
			set { transferFunction = value; }
		}
		public PdfHalftone Halftone {
			get { return halftone; }
			set { halftone = value; }
		}
		public double? FlatnessTolerance {
			get { return flatnessTolerance; }
			set { flatnessTolerance = value; }
		}
		public double? SmoothnessTolerance {
			get { return smoothnessTolerance; }
			set { smoothnessTolerance = value; }
		}
		public bool? StrokeAdjustment {
			get { return strokeAdjustment; }
			set { strokeAdjustment = value; }
		}
		public PdfBlendMode? BlendMode {
			get { return blendMode; }
			set { blendMode = value; }
		}
		public PdfSoftMask SoftMask {
			get { return softMask; }
			set { softMask = value; }
		}
		public double? StrokingAlphaConstant {
			get { return strokingAlphaConstant; }
			set { strokingAlphaConstant = value; }
		}
		public double? NonStrokingAlphaConstant {
			get { return nonStrokingAlphaConstant; }
			set { nonStrokingAlphaConstant = value; }
		}
		public bool? AlphaSource {
			get { return alphaSource; }
			set { alphaSource = value; }
		}
		public bool? TextKnockout {
			get { return textKnockout; }
			set { textKnockout = value; }
		}
		internal PdfGraphicsStateParameters() {
		}
		internal PdfGraphicsStateParameters(PdfReaderDictionary dictionary) : base(dictionary.Number)  {
			PdfObjectCollection objects = dictionary.Objects;
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != dictionaryType)
				PdfDocumentReader.ThrowIncorrectDataException();
			lineWidth = dictionary.GetNumber(lineWidthDictionaryKey);
			int? lc = dictionary.GetInteger(lineCapDictionaryKey);
			if (lc.HasValue)
				lineCap = PdfSetLineCapStyleCommand.ConvertToLineCapStyle(lc.Value);
			int? lj = dictionary.GetInteger(lineJoinDictionaryKey);
			if (lj.HasValue)
				lineJoin = PdfSetLineJoinStyleCommand.ConvertToLineJoinStyle(lj.Value);
			miterLimit = dictionary.GetNumber(miterLimitDictionaryKey);
			IList<object> d = dictionary.GetArray(dashPatternDictionaryKey);
			if (d != null)
				lineStyle = PdfLineStyle.Parse(d);
			string ri = dictionary.GetName(renderingIntentDictionaryKey);
			if (ri != null)
				renderingIntent = PdfEnumToStringConverter.Parse<PdfRenderingIntent>(ri);
			strokingOverprint = dictionary.GetBoolean(strokingOverprintDictionaryKey);
			nonStrokingOverprint = dictionary.GetBoolean(nonStrokingOverprintDictionaryKey);
			int? opm = dictionary.GetInteger(overprintModeDictionaryKey);
			if (opm.HasValue)
				overprintMode = PdfEnumToValueConverter.Parse<PdfOverprintMode>(opm.Value);
			IList<object> fontArray = dictionary.GetArray(fontDictionaryKey);
			if (fontArray != null) {
				if (fontArray.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfObjectReference reference = fontArray[0] as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				font = objects.GetFont(reference.Number);
				if (font == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				fontSize = PdfDocumentReader.ConvertToDouble(fontArray[1]);
			}
			blackGenerationFunction = ResolveFunction(dictionary, blackGenerationFunctionDictionaryKey, true) ?? ResolveFunction(dictionary, "BG", false);
			undercolorRemovalFunction = ResolveFunction(dictionary, undercolorRemovalFunctionDictionaryKey, true) ?? ResolveFunction(dictionary, "UCR", false);
			transferFunction = ResolveFunctions(dictionary, transferFunctionDictionaryKey, true) ?? ResolveFunctions(dictionary, "TR", false);
			object halftoneValue;
			halftone = dictionary.TryGetValue(halftoneDictionaryKey, out halftoneValue) ? objects.GetHalftone(halftoneValue) : null;
			flatnessTolerance = dictionary.GetNumber(flatnessToleranceDictionaryKey);
			smoothnessTolerance = dictionary.GetNumber(smoothnessToleranceDictionaryKey);
			strokeAdjustment = dictionary.GetBoolean(strokeAdjustmentDictionaryKey);
			string blendModeName = dictionary.GetName(blendModeDictionaryKey);
			if (blendModeName != null)
				blendMode = PdfEnumToStringConverter.Parse<PdfBlendMode>(blendModeName, false);
			object softMaskValue;
			if (dictionary.TryGetValue(softMaskDictionaryKey, out softMaskValue))
				softMask = PdfSoftMask.Create(objects.DocumentCatalog, softMaskValue);
			strokingAlphaConstant = dictionary.GetNumber(strokingAlphaConstantDictionaryKey);
			nonStrokingAlphaConstant = dictionary.GetNumber(nonStrokingAlphaConstantDictionaryKey);
			alphaSource = dictionary.GetBoolean(alphaSourceDictionaryKey);
			textKnockout = dictionary.GetBoolean(textKnockoutDictionaryKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryType));
			dictionary.AddNullable(lineWidthDictionaryKey, lineWidth);
			if (lineCap.HasValue)
				dictionary.Add(lineCapDictionaryKey, (int)lineCap.Value);
			if (lineJoin.HasValue)
				dictionary.Add(lineJoinDictionaryKey, (int)lineJoin.Value);
			dictionary.AddNullable(miterLimitDictionaryKey, miterLimit);
			if (lineStyle != null)
				dictionary.Add(dashPatternDictionaryKey, lineStyle.Data);
			if (renderingIntent.HasValue)
				dictionary.AddEnumName(renderingIntentDictionaryKey, renderingIntent.Value, false);
			dictionary.AddNullable(strokingOverprintDictionaryKey, strokingOverprint);
			dictionary.AddNullable(nonStrokingOverprintDictionaryKey, nonStrokingOverprint);
			if (overprintMode.HasValue)
				dictionary.Add(overprintModeDictionaryKey, (int)overprintMode.Value);
			if (font != null)
				dictionary.Add(fontDictionaryKey, new List<object>() { objects.AddObject(font), fontSize.Value });
			if (blackGenerationFunction != null)
				dictionary.Add(blackGenerationFunctionDictionaryKey, blackGenerationFunction.Write(objects));
			if (undercolorRemovalFunction != null)
				dictionary.Add(undercolorRemovalFunctionDictionaryKey, undercolorRemovalFunction.Write(objects));
			if (transferFunction != null) {
				if (transferFunction.Length == 1)
					dictionary.Add(transferFunctionDictionaryKey, transferFunction[0].Write(objects));
				else
					dictionary.AddList(transferFunctionDictionaryKey, transferFunction, value => ((PdfFunction)value).Write(objects));
			}
			if (halftone != null)
				dictionary.Add(halftoneDictionaryKey, halftone.CreateWriteableObject(objects));
			dictionary.AddNullable(flatnessToleranceDictionaryKey, flatnessTolerance);
			dictionary.AddNullable(smoothnessToleranceDictionaryKey, smoothnessTolerance);
			dictionary.AddNullable(strokeAdjustmentDictionaryKey, strokeAdjustment);
			if (blendMode.HasValue)
				dictionary.AddEnumName(blendModeDictionaryKey, blendMode.Value);
			if (softMask != null)
				dictionary.Add(softMaskDictionaryKey, softMask.Write(objects));
			dictionary.AddNullable(strokingAlphaConstantDictionaryKey, strokingAlphaConstant);
			dictionary.AddNullable(nonStrokingAlphaConstantDictionaryKey, nonStrokingAlphaConstant);
			dictionary.AddNullable(alphaSourceDictionaryKey, alphaSource);
			dictionary.AddNullable(textKnockoutDictionaryKey, textKnockout);
			return dictionary;
		}
	}
}
