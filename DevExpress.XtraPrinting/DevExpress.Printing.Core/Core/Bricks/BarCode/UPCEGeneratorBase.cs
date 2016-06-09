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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public abstract class UPCEGeneratorBase : UPCAGenerator {
		#region static
		internal static string ConvertUPCA2UPCE(string text) {
			if(text[0] != '0' && text[0] != '1')
				return null;
			StringBuilder sb = new StringBuilder(6);
			if(text[5] != '0') {
				if((int)text[10] < (int)'5')
					return null;
				if(text[6] != '0' || text[7] != '0' || text[8] != '0' || text[9] != '0')
					return null;
				for(int i = 1; i <= 5; i++)
					sb.Append(text[i]);
				sb.Append(text[10]);
			} else {
				if(text[4] == '0') {
					if(text[3] == '0' || text[3] == '1' || text[3] == '2') {
						if(text[6] != '0' || text[7] != '0')
							return null;
						sb.Append(text[1]);
						sb.Append(text[2]);
						sb.Append(text[8]);
						sb.Append(text[9]);
						sb.Append(text[10]);
						sb.Append(text[3]);
					} else {
						if(text[6] != '0' || text[7] != '0' || text[8] != '0')
							return null;
						sb.Append(text[1]);
						sb.Append(text[2]);
						sb.Append(text[3]);
						sb.Append(text[9]);
						sb.Append(text[10]);
						sb.Append('3');
					}
				} else {
					if(text[6] != '0' || text[7] != '0' || text[8] != '0' || text[9] != '0')
						return null;
					sb.Append(text[1]);
					sb.Append(text[2]);
					sb.Append(text[3]);
					sb.Append(text[4]);
					sb.Append(text[10]);
					sb.Append('4');
				}
			}
			return sb.ToString();
		}
		#endregion
		#region Fields & Properties
		[
		DefaultValue(false),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return false; }
			set { }
		}
		#endregion
		protected UPCEGeneratorBase() {
		}
		protected UPCEGeneratorBase(UPCEGeneratorBase source)
			: base(source) {
		}
		protected abstract char GetNumberSystemDigit();
		protected override int[] GetGuardBarsBounds() {
			return new int[] { 3, -1, -1, 28 };
		}
		protected override int GetMiddleIndex() {
			return -1;
		}
		protected override string FormatText(string text) {
			if(text.Length >= 11)
				return FormatText(text.Substring(1, 10), 10);
			else
				return FormatText(text, 10);
		}
		protected override bool IsValidTextFormat(string text) {
			text = GetNumberSystemDigit() + text;
			return ConvertUPCA2UPCE(text) != null;
		}
		protected override char[] PrepareText(string text) {
			text = GetNumberSystemDigit() + text;
			string textUPCE = ConvertUPCA2UPCE(text);
			char checkSum = CalcCheckDigit(text);
			int count = 0;
			if(textUPCE != null)
				count = textUPCE.Length;
			char[] chars = new char[count + 2];
			chars[0] = '*';
			string parity = GetParityString(checkSum);
			for(int i = 0, idx = 1; i < count; i++, idx++)
				chars[idx] = (char)(Char2Int(textUPCE[i]) + (int)parity[i]);
			chars[count + 1] = '<';
			return chars;
		}
		protected override float GetLeftSpacing(IBarCodeData data, IGraphicsBase gr) {
			return MeasureCharWidth(FinalText[0], data, sfRight, gr) + GetSpacing(gr);
		}
		protected override void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			string firstDigit = new string(GetNumberSystemDigit(), 1);
			string text = firstDigit + FinalText;
			string textUPCE = ConvertUPCA2UPCE(text);
			string checkDigit = new string(CalcCheckDigit(text), 1);
			RectangleF firstDigitBounds = bounds;
			firstDigitBounds.Width = GetLeftSpacing(data, gr);
			firstDigitBounds.X -= firstDigitBounds.Width;
			RectangleF checkDigitBounds = bounds;
			checkDigitBounds.Width = GetRightSpacing(data, gr) - GetSpacing(gr);
			checkDigitBounds.X = bounds.Right + GetSpacing(gr);
			gr.DrawString(textUPCE, data.Style.Font, gr.GetBrush(data.Style.ForeColor), bounds, sfCenter);
			gr.DrawString(firstDigit, data.Style.Font, gr.GetBrush(data.Style.ForeColor), firstDigitBounds, sfRight);
			gr.DrawString(checkDigit, data.Style.Font, gr.GetBrush(data.Style.ForeColor), checkDigitBounds, sfLeft);
		}
	}
}
