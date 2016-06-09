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
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocBoolWrapper
	public enum DocBoolWrapper {
		False = 0x00,
		True = 0x01,
		Leave = 0x80,
		Inverse = 0x81
	}
	#endregion
	#region DocCharacterFormattingInfo
	public class DocCharacterFormattingInfo : ICloneable<DocCharacterFormattingInfo>, ISupportsCopyFrom<DocCharacterFormattingInfo>, ISupportsSizeOf {
		public static readonly string DefaultFontName = "Times New Roman";
		public static DocCharacterFormattingInfo CreateDefault() {
			DocCharacterFormattingInfo result = new DocCharacterFormattingInfo();
			result.FontName = DefaultFontName;
			result.DoubleFontSize = 0x000c*2;
			result.Script = CharacterFormattingScript.Normal;
			return result;
		}
		#region Fields
		string fontName;
		int doubleFontSize;
		DocBoolWrapper fontBold;
		DocBoolWrapper fontItalic;
		DocBoolWrapper strike;
		DocBoolWrapper doubleStrike;
		UnderlineType fontUnderlineType;
		DocBoolWrapper allCaps;
		bool underlineWordsOnly;
		bool strikeoutWordsOnly;
		CharacterFormattingScript script;
		Color foreColor;
		Color backColor;
		Color underlineColor;
		Color strikeoutColor;
		DocBoolWrapper hidden;
		int styleIndex;
		#endregion
		public DocCharacterFormattingInfo() {
			this.fontName = String.Empty;
			this.foreColor = DXColor.Empty;
			this.backColor = DXColor.Transparent;
			this.underlineColor = DXColor.Empty;
			this.strikeoutColor = DXColor.Empty;
			this.styleIndex = 0x0a;
		}
		#region Properties
		public string FontName { get { return fontName; } set { fontName = value; } }
		public int DoubleFontSize {
			get { return doubleFontSize; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("DoubleFontSize", value);
				doubleFontSize = value;
			}
		}
		public DocBoolWrapper FontBold { get { return fontBold; } set { fontBold = value; } }
		public DocBoolWrapper FontItalic { get { return fontItalic; } set { fontItalic = value; } }
		public DocBoolWrapper Strike { get { return strike; } set { strike = value; } }
		public DocBoolWrapper DoubleStrike { get { return doubleStrike; } set { doubleStrike = value; } }
		public UnderlineType FontUnderlineType {
			get { return fontUnderlineType; }
			set {
				fontUnderlineType = value;
			}
		}
		public DocBoolWrapper AllCaps { get { return allCaps; } set { allCaps = value; } }
		public bool UnderlineWordsOnly { get { return underlineWordsOnly; } set { underlineWordsOnly = value; } }
		public bool StrikeoutWordsOnly { get { return strikeoutWordsOnly; } set { strikeoutWordsOnly = value; } }
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color UnderlineColor { get { return underlineColor; } set { underlineColor = value; } }
		public Color StrikeoutColor { get { return strikeoutColor; } set { strikeoutColor = value; } }
		public CharacterFormattingScript Script { get { return script; } set { script = value; } }
		public DocBoolWrapper Hidden { get { return hidden; } set { hidden = value; } }
		public int StyleIndex { get { return styleIndex; } set { styleIndex = value; } }
		#endregion
		public DocCharacterFormattingInfo Clone() {
			DocCharacterFormattingInfo result = new DocCharacterFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public override bool Equals(object obj) {
			DocCharacterFormattingInfo info = (DocCharacterFormattingInfo)obj;
			return
				this.DoubleFontSize == info.DoubleFontSize &&
				this.FontBold == info.FontBold &&
				this.FontItalic == info.FontItalic &&
				this.Strike == info.Strike &&
				this.DoubleStrike == info.DoubleStrike &&
				this.FontUnderlineType == info.FontUnderlineType &&
				this.AllCaps == info.AllCaps &&
				this.ForeColor == info.ForeColor &&
				this.BackColor == info.BackColor &&
				this.UnderlineColor == info.UnderlineColor &&
				this.StrikeoutColor == info.StrikeoutColor &&
				this.StrikeoutWordsOnly == info.StrikeoutWordsOnly &&
				this.UnderlineWordsOnly == info.UnderlineWordsOnly &&
				this.Script == info.Script &&
				this.Hidden == info.Hidden &&
				this.StyleIndex == info.StyleIndex &&
				StringExtensions.CompareInvariantCultureIgnoreCase(this.FontName, info.FontName) == 0;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public bool ContainsInvertedProperties() {
			return
				this.allCaps == DocBoolWrapper.Inverse ||
				this.fontBold == DocBoolWrapper.Inverse ||
				this.fontItalic == DocBoolWrapper.Inverse ||
				this.hidden == DocBoolWrapper.Inverse ||
				this.strike == DocBoolWrapper.Inverse ||
				this.doubleStrike == DocBoolWrapper.Inverse;
		}
		public void CopyFrom(DocCharacterFormattingInfo info) {
			FontName = info.FontName;
			DoubleFontSize = info.DoubleFontSize;
			FontBold = info.FontBold;
			FontItalic = info.FontItalic;
			Strike = info.Strike;
			DoubleStrike = info.DoubleStrike;
			FontUnderlineType = info.FontUnderlineType;
			AllCaps = info.AllCaps;
			ForeColor = info.ForeColor;
			BackColor = info.BackColor;
			UnderlineColor = info.UnderlineColor;
			StrikeoutColor = info.StrikeoutColor;
			UnderlineWordsOnly = info.UnderlineWordsOnly;
			StrikeoutWordsOnly = info.StrikeoutWordsOnly;
			Script = info.Script;
			Hidden = info.Hidden;
			StyleIndex = info.StyleIndex;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
}
