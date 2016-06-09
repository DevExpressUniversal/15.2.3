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
namespace DevExpress.XtraPrinting.BarCode {
	public abstract class UPCSupplementalNGeneratorBase : BarCodeGeneratorBase {
		static string validCharSet = charSetDigits;
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
		protected UPCSupplementalNGeneratorBase() {
		}
		protected UPCSupplementalNGeneratorBase(UPCSupplementalNGeneratorBase source)
			: base(source) {
		}
		protected override string FormatText(string text) {
			return EAN13Generator.FormatText(text, GetMaxCharsCount());
		}
		protected override string GetValidCharSet() { return validCharSet; }
		protected override Hashtable GetPatternTable() {
			return EAN13Generator.charPattern;
		}
		protected override char[] PrepareText(string text) {
			int count = text.Length;
			System.Diagnostics.Debug.Assert(count == GetMaxCharsCount());
			char[] chars = new char[count * 2];
			chars[0] = '>';
			string parity = GetParityString(CalcCheckDigit(text));
			for(int i = 0, idx = 1; i < count; i++, idx += 2) {
				chars[idx] = (char)(Char2Int(text[i]) + (int)parity[i]);
				if(i + 1 < count)
					chars[idx + 1] = '-';
			}
			return chars;
		}
		protected abstract int CalcCheckDigit(string text);
		protected abstract int GetMaxCharsCount();
		protected abstract string GetParityString(int checkDigit);
	}
}
