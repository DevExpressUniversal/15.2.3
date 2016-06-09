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
using System.Drawing;
using DevExpress.XtraPrinting.BarCode.Native;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.BarCode {
	public class IntelligentMailGenerator : BarCode2DGenerator {
		readonly IntelligentMailProcessor processor = new IntelligentMailProcessor();
		#region Static & Constants
		const short HightToWidthRatio = 8;
		static int GetPatternOffsetTable(char patternSymbol) {
			switch(patternSymbol) {
				case 'D':
				case 'T':
					return 1;
				case 'A':
				default:
					return 0;
			}
		}
		static int GetPatternLenghtTable(char patternSymbol) {
			switch(patternSymbol) {
				case 'A':
				case 'D':
					return 2;
				case 'T':
					return 1;
				default:
					return 3;
			}
		}
		static bool IsValidTextLength(string text) {
			return text.Length == 20 || text.Length == 25 || text.Length == 29 || text.Length == 31;
		}
		static bool IsValidValue(string text) {
			return text[1] < '5';
		}
		#endregion
		#region Fields & Properties
		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		protected override IPatternProcessor PatternProcessor {
			get { return processor; }
		}
		protected override bool IsSquareBarcode {
			get { return true; }
		}
		#endregion
		public IntelligentMailGenerator() {
		}
		IntelligentMailGenerator(IntelligentMailGenerator source)
			: base(source) {
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("IntelligentMailGeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.IntelligentMail; }
		}
		#region Methods
		public override void Update(string text, byte[] binaryData) {
			if(text != null && IsValidText(text))
				base.Update(text, binaryData);
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new IntelligentMailGenerator(this);
		}
		protected override float GetPatternWidth(char pattern) {
			return HightToWidthRatio;
		}
		protected override string GetValidCharSet() {
			return charSetDigits;
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			const int BarsCount = 65;
			int BarsWithSpaces = BarsCount * 2 - 1;
			return (float)((double)BarsWithSpaces * module);
		}
		protected override float CalcBarCodeHeight(ArrayList pattern, double module) {
			return (float)((double)HightToWidthRatio * module);
		}
		protected override bool IsValidText(string text) {
			return IsValidTextLength(text) && IsValidValue(text) && base.IsValidText(text);
		}
		protected override Hashtable GetPatternTable() {
			return new Hashtable();
		}
		protected override char[] PrepareText(string text) {
			return text.ToCharArray();
		}
		protected override bool TextCompactionMode() {
			return true;
		}
		protected override bool BinaryCompactionMode() {
			return false;
		}
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			const float SingleSegmentMultiplier = 1f / 3f;
			float x = barBounds.Left;
			float height = 0;
			float offset = 0;
			foreach(char symbol in Pattern) {
				var ratio = yModule * HightToWidthRatio * SingleSegmentMultiplier;
				offset = ratio * GetPatternOffsetTable(symbol);
				height = ratio * GetPatternLenghtTable(symbol);
				gr.FillRectangle(gr.GetBrush(data.Style.ForeColor), x, barBounds.Top + offset, xModule, height);
				x += 2 * xModule;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
		protected override RectangleF AlignBarcodeBounds(RectangleF barcodeBounds, float width, float height, TextAlignment align) {
			barcodeBounds = base.AlignBarcodeBounds(barcodeBounds, width, height, align);
			barcodeBounds = AlignVerticalBarcodeBound(barcodeBounds, height, align);
			return barcodeBounds;
		}
		#endregion
	}
}
