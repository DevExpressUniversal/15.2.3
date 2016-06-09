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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Model {
	public enum ThemeFontType {
		None,
		MajorEastAsia,
		MajorBidi,
		MajorAscii,
		MajorHAnsi,
		MinorEastAsia,
		MinorBidi,
		MinorAscii,
		MinorHAnsi
	}
	#region RichEditFontInfo
	public class RichEditFontInfo {
		public string AsciiFontName { get; set; }
		public string CsFontName { get; set; }
		public string EastAsiaFontName { get; set; }
		public string HAnsiFontName { get; set; }
		public ThemeFontType AsciiThemeFont { get; set; }
		public ThemeFontType CsThemeFont { get; set; }
		public ThemeFontType EastAsiaThemeFont { get; set; }
		public ThemeFontType HAnsiThemeFont { get; set; }
		public FontTypeHint HintFont { get; set; }
		public override bool Equals(object obj) {
			RichEditFontInfo info = obj as RichEditFontInfo;
			if (info == null)
				return false;
			return AsciiFontName == info.AsciiFontName &&
				   CsFontName == info.CsFontName &&
				   EastAsiaFontName == info.EastAsiaFontName &&
				   HAnsiFontName == info.HAnsiFontName &&
				   AsciiThemeFont == info.AsciiThemeFont &&
				   CsThemeFont == info.CsThemeFont &&
				   EastAsiaThemeFont == info.EastAsiaThemeFont &&
				   HAnsiThemeFont == info.HAnsiThemeFont &&
				   HintFont == info.HintFont;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region FontContentType
	public enum FontTypeHint {
		Cs,
		EastAsia,
		Default,
		None,
	}
	#endregion
	#region Shading
	public class Shading {
		public ShadingPattern ShadingPattern { get; set; }
		public ColorModelInfo ColorInfo { get; set; }
		public ColorModelInfo FillInfo { get; set; }
		public static Shading Create() {
			Shading result = new Shading();
			result.ColorInfo = ColorModelInfo.Create(DXColor.Transparent);
			result.FillInfo = ColorModelInfo.Create(DXColor.Empty);
			result.ShadingPattern = ShadingPattern.Clear;
			return result;
		}
		public override bool Equals(object obj) {
			Shading info = obj as Shading;
			if (info == null)
				return false;
			return ColorInfo.Equals(info.ColorInfo) &&
				   FillInfo.Equals(info.FillInfo) &&
				   ShadingPattern == info.ShadingPattern;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
}
