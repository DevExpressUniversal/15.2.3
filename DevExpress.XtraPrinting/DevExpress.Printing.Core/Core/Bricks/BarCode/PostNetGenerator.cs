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

using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class PostNetGenerator : BarCodeGeneratorBase {
		#region static
		static string validCharSet = charSetDigits;
		static Hashtable charPattern = new Hashtable();
		static char CalcCheckDigit(string text) {
			int sum = 0;
			int count = text.Length;
			for(int i = 0; i < count; i++)
				sum += Char2Int(text[i]);
			sum %= 10;
			if(sum != 0)
				sum = 10 - sum;
			return (char)(sum + (int)'0');
		}
		#endregion
		#region Fields & Properties
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		#endregion
		#region static XRPostNetGenerator()
		static PostNetGenerator() {
			charPattern['0'] = "ffhhh";
			charPattern['1'] = "hhhff";
			charPattern['2'] = "hhfhf";
			charPattern['3'] = "hhffh";
			charPattern['4'] = "hfhhf";
			charPattern['5'] = "hfhfh";
			charPattern['6'] = "hffhh";
			charPattern['7'] = "fhhhf";
			charPattern['8'] = "fhhfh";
			charPattern['9'] = "fhfhh";
			charPattern['*'] = "f";
		}
		#endregion
		public PostNetGenerator() {
		}
		PostNetGenerator(PostNetGenerator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new PostNetGenerator(this);
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PostNetGeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.PostNet; }
		}
		protected override char[] PrepareText(string text) {
			if(CalcCheckSum)
				text += CalcCheckDigit(text);
			return ('*' + text + '*').ToCharArray();
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'f' ? 0 : 0.5f;
		}
		protected override string GetValidCharSet() { return validCharSet; }
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			float x = barBounds.Left;
			for(int i = 0; i < Pattern.Count; i++) {
				float h = barBounds.Height * (float)Pattern[i];
				gr.FillRectangle(gr.GetBrush(data.Style.ForeColor), x, barBounds.Top + h, xModule, barBounds.Height - h);
				x += 2 * xModule;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			return (float)(2 * (double)pattern.Count * module);
		}
	}
}
