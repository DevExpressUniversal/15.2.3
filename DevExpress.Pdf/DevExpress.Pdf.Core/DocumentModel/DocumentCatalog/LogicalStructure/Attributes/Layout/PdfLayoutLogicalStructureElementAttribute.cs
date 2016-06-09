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
using DevExpress.Pdf.Native;
using System.Collections.Generic;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfLayoutLogicalStructureElementAttributePlacement.Inline)]
	public enum PdfLayoutLogicalStructureElementAttributePlacement { Block, Inline, Before, Start, End }
	[PdfDefaultField(PdfLayoutLogicalStructureElementAttributeWritingMode.LeftToRight)]
	public enum PdfLayoutLogicalStructureElementAttributeWritingMode {
		[PdfFieldName("LrTb")]
		LeftToRight,
		[PdfFieldName("RlTb")]
		RightToLeft,
		[PdfFieldName("TbRl")]
		TopToBottom
	}
	[PdfDefaultField(PdfLayoutLogicalStructureElementAttributeBorderStyle.None)]
	public enum PdfLayoutLogicalStructureElementAttributeBorderStyle { None, Hidden, Dotted, Dashed, Solid, Double, Groove, Ridge, Inset, Outset }
	public class PdfLayoutLogicalStructureElementAttribute : PdfLogicalStructureElementAttribute {
		internal const string Owner = "Layout";
		const string placementKey = "Placement";
		const string writingModeKey = "WritingMode";
		const string backgroundColorKey = "BackgroundColor";
		const string borderColorKey = "BorderColor";
		const string borderStyleKey = "BorderStyle";
		const string borderThicknessKey = "BorderThickness";
		const string paddingKey = "Padding";
		const string colorKey = "Color";
		readonly PdfLayoutLogicalStructureElementAttributePlacement placement;
		readonly PdfLayoutLogicalStructureElementAttributeWritingMode writingMode;
		readonly PdfColor backgroundColor = null;
		readonly PdfColor borderColorBefore;
		readonly PdfColor borderColorAfter;
		readonly PdfColor borderColorStart;
		readonly PdfColor borderColorEnd;
		readonly PdfLayoutLogicalStructureElementAttributeBorderStyle borderStyleBefore = PdfLayoutLogicalStructureElementAttributeBorderStyle.None;
		readonly PdfLayoutLogicalStructureElementAttributeBorderStyle borderStyleAfter = PdfLayoutLogicalStructureElementAttributeBorderStyle.None;
		readonly PdfLayoutLogicalStructureElementAttributeBorderStyle borderStyleStart = PdfLayoutLogicalStructureElementAttributeBorderStyle.None;
		readonly PdfLayoutLogicalStructureElementAttributeBorderStyle borderStyleEnd = PdfLayoutLogicalStructureElementAttributeBorderStyle.None;
		readonly double borderThicknessBefore = 0.0;
		readonly double borderThicknessAfter = 0.0;
		readonly double borderThicknessStart = 0.0;
		readonly double borderThicknessEnd = 0.0;
		readonly double paddingBefore = 0.0;
		readonly double paddingAfter = 0.0;
		readonly double paddingStart = 0.0;
		readonly double paddingEnd = 0.0;
		readonly PdfColor colorText = null;
		public PdfLayoutLogicalStructureElementAttributePlacement Placement { get { return placement; } }
		public PdfLayoutLogicalStructureElementAttributeWritingMode WritingMode { get { return writingMode; } }
		public PdfColor BackgroundColor { get { return backgroundColor; } }
		public PdfColor BorderColorBefore { get { return borderColorBefore; } }
		public PdfColor BorderColorAfter { get { return borderColorAfter; } }
		public PdfColor BorderColorStart { get { return borderColorStart; } }
		public PdfColor BorderColorEnd { get { return borderColorEnd; } }
		public PdfLayoutLogicalStructureElementAttributeBorderStyle BorderStyleBefore { get { return borderStyleBefore; } }
		public PdfLayoutLogicalStructureElementAttributeBorderStyle BorderStyleAfter { get { return borderStyleAfter; } }
		public PdfLayoutLogicalStructureElementAttributeBorderStyle BorderStyleStart { get { return borderStyleStart; } }
		public PdfLayoutLogicalStructureElementAttributeBorderStyle BorderStyleEnd { get { return borderStyleEnd; } }
		public double BorderThicknessBefore { get { return borderThicknessBefore; } }
		public double BorderThicknessAfter { get { return borderThicknessAfter; } }
		public double BorderThicknessStart { get { return borderThicknessStart; } }
		public double BorderThicknessEnd { get { return borderThicknessEnd; } }
		public double PaddingBefore { get { return paddingBefore; } }
		public double PaddingAfter { get { return paddingAfter; } }
		public double PaddingStart { get { return paddingStart; } }
		public double PaddingEnd { get { return paddingEnd; } }
		public PdfColor ColorText { get { return colorText; } }
		internal static PdfLayoutLogicalStructureElementAttribute ParseAttribute(PdfReaderDictionary dictionary) {
			foreach (KeyValuePair<string, object> pair in dictionary) {
				if (Array.IndexOf(PdfBLSELayoutLogicalStructureElementAttribute.Keys, pair.Key) > -1)
					return new PdfBLSELayoutLogicalStructureElementAttribute(dictionary);
				if (Array.IndexOf(PdfILSELayoutLogicalStructureElementAttribute.Keys, pair.Key) > -1)
					return new PdfILSELayoutLogicalStructureElementAttribute(dictionary);
				if (Array.IndexOf(PdfColumnLayoutLogicalStructureElementAttribute.Keys, pair.Key) > -1)
					return new PdfColumnLayoutLogicalStructureElementAttribute(dictionary);
			}
			return new PdfLayoutLogicalStructureElementAttribute(dictionary);
		}
		protected PdfLayoutLogicalStructureElementAttribute(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			placement = PdfEnumToStringConverter.Parse<PdfLayoutLogicalStructureElementAttributePlacement>(dictionary.GetName(placementKey));
			writingMode = PdfEnumToStringConverter.Parse<PdfLayoutLogicalStructureElementAttributeWritingMode>(dictionary.GetName(writingModeKey));
			backgroundColor = ConvertToColor(dictionary.GetDoubleArray(backgroundColorKey));
			IList<PdfColor> colors = GetEdgeColors(dictionary, borderColorKey);
			if (colors != null) {
				borderColorBefore = colors[0];
				borderColorAfter = colors[1];
				borderColorStart = colors[2];
				borderColorEnd = colors[3];
			}
			IList<PdfLayoutLogicalStructureElementAttributeBorderStyle> bs = GetEdgeOptions(dictionary, borderStyleKey, 
				o => PdfEnumToStringConverter.Parse<PdfLayoutLogicalStructureElementAttributeBorderStyle>(ConvertToString(o)));
			if (bs != null) {
				borderStyleBefore = bs[0];
				borderStyleAfter = bs[1];
				borderStyleStart = bs[2];
				borderStyleEnd = bs[3];
			}
			IList<double> doubleValues = GetEdgeOptions(dictionary, borderThicknessKey, ConvertToDouble);
			if (doubleValues != null) {
				borderThicknessBefore = doubleValues[0];
				borderThicknessAfter = doubleValues[1];
				borderThicknessStart = doubleValues[2];
				borderThicknessEnd = doubleValues[3];
			}
			doubleValues = GetEdgeOptions(dictionary, paddingKey, ConvertToDouble);
			if (doubleValues != null) {
				paddingBefore = doubleValues[0];
				paddingAfter = doubleValues[1];
				paddingStart = doubleValues[2];
				paddingEnd = doubleValues[3];
			}
			colorText = ConvertToColor(dictionary.GetDoubleArray(colorKey));
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.AddName(OwnerKey, Owner);
			dictionary.AddEnumName(placementKey, placement);
			dictionary.AddEnumName(writingModeKey, writingMode);
			dictionary.Add(backgroundColorKey, backgroundColor);
			dictionary.AddIfPresent(borderColorKey, WriteEdgeParams(borderColorBefore, borderColorAfter, borderColorStart, borderColorEnd, (o) => o.ToWritableObject(), null));
			object borderStyle = WriteEdgeParams(borderStyleBefore, borderStyleAfter, borderStyleStart, borderStyleEnd, 
					(o) => new PdfName(PdfEnumToStringConverter.Convert(o, false)), PdfLayoutLogicalStructureElementAttributeBorderStyle.None);
			dictionary.AddIfPresent(borderStyleKey, borderStyle);
			dictionary.AddIfPresent(borderThicknessKey, WriteEdgeParams(borderThicknessBefore, borderThicknessAfter, borderThicknessStart, borderThicknessEnd, (o) => o, 0.0));
			dictionary.AddIfPresent(paddingKey, WriteEdgeParams(paddingBefore, paddingAfter, paddingStart, paddingEnd, (o) => o, 0.0));
			dictionary.Add(colorKey, colorText);
			return dictionary;
		}
		protected PdfColor ConvertToColor(IList<double> components) {
			if (components == null)
				return null;
			if (components.Count != 3)
				PdfDocumentReader.ThrowIncorrectDataException();
			foreach (double value in components)
				if (value < 0.0 || value > 1.0)
					PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfColor(components[0], components[1], components[2]);
		}
		double ConvertToDouble(object value) {
			double result = PdfDocumentReader.ConvertToDouble(value);
			if (result < 0.0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return result;
		}
		PdfColor ConvertToColor(IList<object> components) {
			IList<double> comp = null;
			if (components != null) {
				comp = new List<double>();
				foreach (object value in components)
					comp.Add(PdfDocumentReader.ConvertToDouble(value));
			}
			return ConvertToColor(comp);
		}
		IList<PdfColor> GetEdgeColors(PdfReaderDictionary dictionary, string key) {
			return GetEdgeOptions<PdfColor>(dictionary, key, o => {
				if (o == null)
					return null;
				IList<object> components = o as IList<object>;
				if (components == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return ConvertToColor(components);
			});
		}
		IList<T> GetEdgeOptions<T>(PdfReaderDictionary dictionary, string key, Func<object, T> convertFunction) {
			object value;
			if (!dictionary.TryGetValue(key, out value))
				return null;
			IList<object> array = value as IList<object>;
			if (array == null || array.Count != 4)
				array = new List<object>() { value, value, value, value };
			List<T> result = new List<T>(4);
			foreach (object v in array)
				result.Add(convertFunction(dictionary.Objects.TryResolve(v)));
			return result;
		}
		object WriteEdgeParams<T>(T before, T after, T start, T end, Func<T, object> prepareFunction, T defaultValue) {
			if (before == null && after == null && start == null && end == null)
				return null;
			Func<T, object> function = (o) => { return o == null ? null : prepareFunction((T)o); };
			if (before != null && before.Equals(after) && after.Equals(start) && start.Equals(end))
				return before.Equals(defaultValue) ? null : function(before);
			return new object[] { function(before), function(after), function(start), function(end) };
		}
		string ConvertToString(object value) {
			PdfName name = value as PdfName;
			if (name != null)
				return name.Name;
			byte[] bytes = value as byte[];
			if (bytes == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfDocumentReader.ConvertToString(bytes);
		}
	}
}
