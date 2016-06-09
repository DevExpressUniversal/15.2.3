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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region GdiPlusFontCache
	public class GdiPlusFontCache : FontCache {
		readonly UnicodeRangeInfo unicodeRangeInfo;
		public GdiPlusFontCache(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
			this.unicodeRangeInfo = new UnicodeRangeInfo();
		}
		protected internal override FontInfoMeasurer CreateFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter) {
			return new GdiPlusFontInfoMeasurer(unitConverter);
		}
		protected internal override FontInfo CreateFontInfoCore(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			return new GdiPlusFontInfo(Measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
		}
		protected override List<FontCharacterRange> GetFontCharacterRanges(FontInfo fontInfo) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			GdiPlusFontInfoMeasurer measurer = (GdiPlusFontInfoMeasurer)Measurer;			
			return gdiPlusFontInfo.GetFontUnicodeRanges(measurer.MeasureGraphics);
		}
		public override bool ShouldUseDefaultFontToDrawInvisibleCharacter(FontInfo fontInfo, char character) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			GdiPlusFontInfoMeasurer measurer = (GdiPlusFontInfoMeasurer)Measurer;
			return gdiPlusFontInfo.CanDrawCharacter(unicodeRangeInfo, measurer.MeasureGraphics, character);
		}
		protected internal override void PopulateNameToCharacterSetMap() {
			lock (this) {
				if (nameToCharacterMapPopulated)
					return;
				FontFamily[] fontFamilies = FontFamily.Families;
				int count = fontFamilies.Length;
				for (int i = 0; i < count; i++) {
					FontFamily family = fontFamilies[i];
					if (family.IsStyleAvailable(FontStyle.Regular))
						GetFontCharacterSet(family.Name); 
				}
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
			using (GdiPlusFontInfo fontInfo = (GdiPlusFontInfo)this.CreateFontInfoCore(fontName, 20, false, false, false, false)) {
				byte[] panose = GetPanose(fontInfo);
				if (panose == null)
					return null;
				List<FontCharacterRange> characterRanges = GetFontCharacterRanges(fontInfo);
				characterRanges.Add(new FontCharacterRange(0, 255));
				characterRanges.Add(new FontCharacterRange(0xF000, 0xF0FF));
				return new FontCharacterSet(characterRanges, panose);
			}
		}
		protected virtual byte[] GetPanose(GdiPlusFontInfo fontInfo) {
			GdiPlusFontInfoMeasurer measurer = (GdiPlusFontInfoMeasurer)Measurer;
			PInvokeSafeNativeMethods.OUTLINETEXTMETRIC? otm = fontInfo.GetOutlineTextMetrics(measurer.MeasureGraphics);
			return (otm != null) ? otm.Value.otmPanoseNumber.ToByteArray() : null;
		}
	}
	#endregion
	#region GdiFontCache
	public class GdiFontCache : GdiPlusFontCache {
		public GdiFontCache(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override FontInfoMeasurer CreateFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter) {
			return new GdiFontInfoMeasurer(unitConverter);
		}
		protected internal override FontInfo CreateFontInfoCore(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			return new GdiFontInfo(Measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
		}
	}
	#endregion
	#region PureGdiFontCache
	public class PureGdiFontCache : GdiFontCache {
		public PureGdiFontCache(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override FontInfoMeasurer CreateFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter) {
			return new PureGdiFontInfoMeasurer(unitConverter);
		}
	}
	#endregion
	#region FontCacheManager
	public abstract partial class FontCacheManager {
		public static FontCacheManager CreateDefault(DocumentLayoutUnitConverter unitConverter) {
			if (PrecalculatedMetricsFontCacheManager.ShouldUse())
				return new PrecalculatedMetricsFontCacheManager(unitConverter);
			else
				return new GdiPlusFontCacheManager(unitConverter);
		}
	}
	#endregion
	#region GdiPlusFontCacheManager
	public class GdiPlusFontCacheManager : FontCacheManager {
		public GdiPlusFontCacheManager(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public override FontCache CreateFontCache() {
			return new GdiPlusFontCache(UnitConverter);
		}
	}
	#endregion
	#region GdiFontCacheManager
	public class GdiFontCacheManager : FontCacheManager {
		public GdiFontCacheManager(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public override FontCache CreateFontCache() {
			return new GdiFontCache(UnitConverter);
		}
	}
	#endregion
	#region PureGdiFontCacheManager
	public class PureGdiFontCacheManager : GdiFontCacheManager {
		public PureGdiFontCacheManager(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public override FontCache CreateFontCache() {
			return new PureGdiFontCache(UnitConverter);
		}
	}
	#endregion
}
