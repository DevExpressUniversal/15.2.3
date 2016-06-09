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

using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class DataMatrixGS1Generator : DataMatrixGenerator {
		internal const char fnc1Char = (char)232;
		const string defaultFNC1Subst = "#";
		string fnc1Subst = defaultFNC1Subst;
		bool decodeText = true;
		[
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataMatrixGS1Generator.FNC1Substitute"),
		DefaultValue(defaultFNC1Subst),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public string FNC1Substitute {
			get { return fnc1Subst; }
			set { 
				fnc1Subst = value;
				RefreshPatternProcessor();
			}
		}
		[
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataMatrixGS1Generator.HumanReadableText"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool HumanReadableText {
			get { return decodeText; }
			set {
				decodeText = value;
				RefreshPatternProcessor();			
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DataMatrixCompactionMode CompactionMode {
			get {
				return base.CompactionMode;
			}
			set {
				base.CompactionMode = value;
			}
		}
		public DataMatrixGS1Generator() {
		}
		public DataMatrixGS1Generator(DataMatrixGS1Generator source) : base(source) {
			FNC1Substitute = source.FNC1Substitute;
			HumanReadableText = source.HumanReadableText;
		}
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.DataMatrixGS1; }
		}
		protected override string MakeDisplayText(string text) {
			return GS1Helper.MakeDisplayText(text, fnc1Char, FNC1Substitute, decodeText);
		}
		protected override string ProcessText(string text) {		  
			string fnc1string = new string(fnc1Char, 1);
			if(!string.IsNullOrEmpty(fnc1Subst))
				text = text.Replace(fnc1Subst, fnc1string);
			if(!string.IsNullOrEmpty(text) && text[0] != fnc1Char)
				text = fnc1string + text;
			return base.ProcessText(text);
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new DataMatrixGS1Generator(this);
		}
	}
}
