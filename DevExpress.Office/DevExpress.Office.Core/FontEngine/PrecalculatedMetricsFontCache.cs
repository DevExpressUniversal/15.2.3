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
using DevExpress.Utils.Internal;
using DevExpress.Office.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Drawing {
	#region PrecalculatedMetricsFontCache
	public class PrecalculatedMetricsFontCache : FontCache {
		public PrecalculatedMetricsFontCache(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override FontInfoMeasurer CreateFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter) {
			return new PrecalculatedMetricsFontInfoMeasurer(unitConverter);
		}
		protected internal override FontInfo CreateFontInfoCore(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			return new PrecalculatedMetricsFontInfo(Measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
		}
		protected override List<FontCharacterRange> GetFontCharacterRanges(FontInfo fontInfo) {
			PrecalculatedMetricsFontInfo precalculatedFontInfo = (PrecalculatedMetricsFontInfo)fontInfo;
			List<FontCharacterRange> result = new List<FontCharacterRange>();
			TTFontInfo ttFontInfo = precalculatedFontInfo.FontDescriptor.FontInfo;
			if (ttFontInfo == null) {
				result.Add(new FontCharacterRange(0, 65536));
				return result;
			}
			Size[] segments = ttFontInfo.GetCharSegments();
			int count = segments.Length;
			for (int i = 0; i < count; i++)
				result.Add(new FontCharacterRange(segments[i].Width, segments[i].Height));
			return result;
		}
		protected internal override void PopulateNameToCharacterSetMap() {
			lock (this) {
				if (nameToCharacterMapPopulated)
					return;
				foreach (string familyName in FontManager.GetFontFamilyNames())
					GetFontCharacterSet(familyName); 
				nameToCharacterMapPopulated = true;
			}
		}
		public override FontCharacterSet GetFontCharacterSet(string fontName) {
			lock (NameToCharacterSetMap) {
				FontCharacterSet result;
				if (NameToCharacterSetMap.TryGetValue(fontName, out result))
					return result;
				else {
					result = CreateFontCharacterSet(fontName);
					if (result != null)
						NameToCharacterSetMap.Add(fontName, result);
					return result;
				}
			}
		}
		protected internal virtual FontCharacterSet CreateFontCharacterSet(string fontName) {
			using (PrecalculatedMetricsFontInfo fontInfo = new PrecalculatedMetricsFontInfo(Measurer, fontName, 20, false, false, false, false)) {
				TTFontInfo ttFontInfo = fontInfo.FontDescriptor.FontInfo;
				if (ttFontInfo == null)
					return null;
				List<FontCharacterRange> characterRanges = GetFontCharacterRanges(fontInfo);
				characterRanges.Add(new FontCharacterRange(0, 255));
				characterRanges.Add(new FontCharacterRange(0xF000, 0xF0FF));
				return new FontCharacterSet(characterRanges, ttFontInfo.Panose);
			}
		}
	}
	#endregion
}
