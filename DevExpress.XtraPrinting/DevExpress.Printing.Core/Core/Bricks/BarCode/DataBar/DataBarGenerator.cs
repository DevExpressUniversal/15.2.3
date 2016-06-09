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
using System.ComponentModel;
using DevExpress.XtraPrinting.BarCode.Native;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.BarCode {
	public class PatternElement {
		public int height;
		public List<int> pattern;
		public Boolean startBarBlack;
		public PatternElement() {
			this.pattern = new List<int>();
		}
		public PatternElement(int height, List<int> pattern, Boolean startBarBlack) {
			this.height = height;
			this.pattern = pattern;
			this.startBarBlack = startBarBlack;
		}
	};
	public class DataBarGenerator : BarCodeGeneratorBase {
		const string defaultFNC1Subst = "#";
		const char fnc1Char = (char)232;
		const int fixedNumberDigits = 14;
		static string validCharCharSet = charSetDigits + charSetUpperCase + charSetLowerCase + "*,-./!'%&()+:;<>=?_ \"" ;
		static Hashtable charPattern = new Hashtable();
		DataBarType type = DataBarType.Omnidirectional;
		int segmentPairsInRow = 10;
		string fnc1Subst = defaultFNC1Subst;
		DataBarPatternProcessor dataBarPatternProcessor;
		[DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("DataBarGeneratorType"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataBarGenerator.Type"),
		DefaultValue(DataBarType.Omnidirectional),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		NotifyParentProperty(true)
		]
		public DataBarType Type {
			get { return type; }
			set {
				if(value != type) {
					type = value;
					DataBarPatternProcessor processor = DataBarPatternProcessor.CreateInstance(type);
					if(dataBarPatternProcessor != null)
						((DataBarPatternProcessor)processor).Assign(dataBarPatternProcessor);
					dataBarPatternProcessor = processor;
				}
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("DataBarGeneratorSegmentsInRow"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataBarGenerator.SegmentsInRow"),
#if !WINRT && !WP
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataBarExpandedWidthConverter)),
#endif
		DefaultValue(20),
		RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true),
		XtraSerializableProperty
		]
		public int SegmentsInRow {
			get { return ((DataBarPatternProcessor)PatternProcessor).SegmentsInRow; }
			set {
				((DataBarPatternProcessor)PatternProcessor).SegmentsInRow = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("DataBarGeneratorFNC1Substitute"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataBarGenerator.FNC1Substitute"),
		DefaultValue(defaultFNC1Subst),
		RefreshProperties(RefreshProperties.All),
#if !WINRT && !WP
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataBarExpandedFNC1Converter)),
#endif
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public string FNC1Substitute {
			get { return ((DataBarPatternProcessor)PatternProcessor).FNC1Substitute; }
			set {
				((DataBarPatternProcessor)PatternProcessor).FNC1Substitute = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DataBarGeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.DataBar;
			}
		}
		protected DataBarPatternProcessor PatternProcessor {
			get {
				if(dataBarPatternProcessor == null) {
					dataBarPatternProcessor = DataBarPatternProcessor.CreateInstance(type);
				}
				return dataBarPatternProcessor;
			}
		}
		public DataBarGenerator() {
		}
		DataBarGenerator(DataBarGenerator source)
			: base(source) {
			type = source.type;
			segmentPairsInRow = source.segmentPairsInRow;
			fnc1Subst = source.fnc1Subst;
			PatternProcessor.Assign(source.PatternProcessor);
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new DataBarGenerator(this);
		}
		protected override string FormatText(string text) {
			if(Type != DataBarType.Expanded && Type != DataBarType.ExpandedStacked && text.Length <= fixedNumberDigits && text != "")
				return text.Length < fixedNumberDigits - 1 ? text + Industrial2of5Generator.CalcCheckDigit(text) :
					text.Substring(0, fixedNumberDigits - 1) + Industrial2of5Generator.CalcCheckDigit(text.Substring(0, fixedNumberDigits - 1));
			else return text;
		}
		protected override string MakeDisplayText(string text) {
			if(Type == DataBarType.Expanded || Type == DataBarType.ExpandedStacked)
				return GS1Helper.MakeDisplayText(text, fnc1Char, FNC1Substitute, true);
			if(text.Length < fixedNumberDigits) { text = text.PadLeft(fixedNumberDigits, '0'); }
			return "(01)" + text;
		}
		protected override Hashtable GetPatternTable() {
			return new Hashtable();
		}
		protected override char[] PrepareText(string text) {
			throw new NotImplementedException();
		}
		protected override string GetValidCharSet() {
			return validCharCharSet + fnc1Subst;
		}
		protected override bool IsValidTextFormat(string text) {
			return ((DataBarPatternProcessor)PatternProcessor).IsValidTextFormat(text);
		}
		protected override ArrayList MakeBarCodePattern(string text) {
			ArrayList result = ((DataBarPatternProcessor)PatternProcessor).GetPattern(text);
			return result;
		}
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			Brush brush = gr.GetBrush(data.Style.ForeColor);
			int count = Pattern.Count;
			float heightDraw = barBounds.Top;
			for(int i = 0; i < count; i++) {
				float x = barBounds.Left;
				PatternElement currentPattern = (PatternElement)Pattern[i];
				for(int j = 0; j < currentPattern.pattern.Count; j++) {
					float w = xModule * (float)(int)currentPattern.pattern[j];
					if(((currentPattern.startBarBlack) && (j % 2 == 0)) || ((!currentPattern.startBarBlack) && (j % 2 == 1))) {
						gr.FillRectangle(brush, x, heightDraw, w, xModule * currentPattern.height);
					}
					x += w;
				}
				heightDraw += xModule * currentPattern.height;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data, heightDraw,xModule);
		}
		void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data, float heightBarCode, float xModule) {
			if(data.Style.TextAlignment == TextAlignment.MiddleCenter || data.Style.TextAlignment == TextAlignment.MiddleLeft ||
				data.Style.TextAlignment == TextAlignment.MiddleRight || data.Style.TextAlignment == TextAlignment.MiddleJustify) {
				bounds.Y = heightBarCode;
			}
			gr.DrawString(DisplayText, data.Style.Font, gr.GetBrush(data.Style.ForeColor), bounds, data.Style.StringFormat.Value);
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			float totalPatternWidth = 0;
			PatternElement newFormatPattern = new PatternElement();
			if (pattern.Count != 0) newFormatPattern = (PatternElement)pattern[0];
			int count = newFormatPattern.pattern.Count;
			for(int i = 0; i < count; i++)
				totalPatternWidth += (float)(int)newFormatPattern.pattern[i];
			return (float)((double)totalPatternWidth * module);
		}
		protected override double CalcAutoModuleX(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			return Math.Min(base.CalcAutoModuleX(data, clientBounds, gr), base.CalcAutoModuleY(data, clientBounds, gr));
		}
		protected override void JustifyBarcodeBounds(IBarCodeData data, ref float barCodeWidth, ref float barCodeHeight, ref RectangleF barBounds) { 
		}
		protected override float CalcBarCodeHeight(ArrayList pattern, double module) {
			float totalPatternHeight = 0;
			for(int i = 0; i < pattern.Count; i++) {
				totalPatternHeight += ((PatternElement)pattern[i]).height;
			}
			return (float)((double)totalPatternHeight * module);
		}
	}
}
