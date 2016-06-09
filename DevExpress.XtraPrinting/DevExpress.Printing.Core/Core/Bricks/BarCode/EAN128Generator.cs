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

using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class EAN128Generator : Code128Generator {
		#region static
		#endregion
		#region Fields & Properties
		protected const string defaultFNC1Subst = "#"; 
		string fnc1Subst = defaultFNC1Subst;
		bool decodeText = true;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EAN128GeneratorFNC1Substitute"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.EAN128Generator.FNC1Substitute"),
		DefaultValue(defaultFNC1Subst),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public string FNC1Substitute {
			get { return fnc1Subst; }
			set { fnc1Subst = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EAN128GeneratorHumanReadableText"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.EAN128Generator.HumanReadableText"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public virtual bool HumanReadableText {
			get { return decodeText; }
			set { decodeText = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("EAN128GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.EAN128;
			}
		}
		#endregion
		public EAN128Generator() {
		}
		protected EAN128Generator(EAN128Generator source)
			: base(source) {
			fnc1Subst = source.fnc1Subst;
			decodeText = source.decodeText;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new EAN128Generator(this);
		}
		protected override void InsertControlCharsIndexes(ArrayList text) {
			text.Add(FNC1);
		}
		protected override char[] PrepareText(string text) {
			if(!String.IsNullOrEmpty(fnc1Subst))
				text = text.Replace(fnc1Subst,  fnc1Char);
			return base.PrepareText(text);
		}
		protected override string MakeDisplayText(string text) {
			return GS1Helper.MakeDisplayText(text, fnc1Char[0], FNC1Substitute, decodeText);			
		}
		protected override bool IsValidTextFormat(string text) {
			if(CharacterSet == DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC &&
				!System.Text.RegularExpressions.Regex.Match(text, @"^(?:(?:\d{2})+" + this.FNC1Substitute + @")*(?:\d{2})+$").Success)
				return false;
			try {
				MakeDisplayText(text);
				return true;
			} catch {
				return false;
			}
		}
		protected override string GetValidCharSet() {
			string charSet = base.GetValidCharSet();
			if(CharacterSet == Code128Charset.CharsetC)
				return charSet += fnc1Subst;
			return charSet;
		}
	}
}
