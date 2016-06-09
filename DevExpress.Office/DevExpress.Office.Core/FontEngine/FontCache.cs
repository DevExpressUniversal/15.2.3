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
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region FontCache (abstract class)
	public abstract class FontCache : IDisposable {
		#region Fields
		bool isDisposed;
		FontInfoMeasurer measurer;
		Dictionary<Int64, int> indexHash;
		Dictionary<string, int> charsets;
		List<FontInfo> fontInfoList;
		static readonly Dictionary<string, FontCharacterSet> nameToCharacterSetMap = new Dictionary<string, FontCharacterSet>();
		protected static bool nameToCharacterMapPopulated;
		readonly DocumentLayoutUnitConverter unitConverter;
		#endregion
		protected FontCache(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.indexHash = new Dictionary<Int64, int>();
			this.charsets = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.fontInfoList = new List<FontInfo>();
			this.measurer = CreateFontInfoMeasurer(unitConverter);
		}
#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		public FontInfo this[int index] { get { return fontInfoList[index]; } }
		internal List<FontInfo> FontInfoList { get { return fontInfoList; } }
		internal Dictionary<Int64, int> IndexHash { get { return indexHash; } }
		public FontInfoMeasurer Measurer { get { return measurer; } }
		protected internal virtual Dictionary<string, FontCharacterSet> NameToCharacterSetMap { get { return nameToCharacterSetMap; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
#endregion
		protected internal abstract FontInfoMeasurer CreateFontInfoMeasurer(DocumentLayoutUnitConverter unitConverter);
		protected internal abstract FontInfo CreateFontInfoCore(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline);
		public FontInfo CreateFontInfo(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			return CreateFontInfoCore(fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
		}
		public virtual int CalcFontIndex(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, CharacterFormattingScript script, bool fontStrikeout, bool fontUnderline) {
			int doubleFontSizeInLayoutFontUnits = unitConverter.PointsToFontUnits(doubleFontSize);
			switch (script) {
				case CharacterFormattingScript.Normal:
					return CalcNormalFontIndex(fontName, doubleFontSizeInLayoutFontUnits, fontBold, fontItalic, fontStrikeout, fontUnderline);
				case CharacterFormattingScript.Subscript:
					return CalcSubscriptFontIndex(fontName, doubleFontSizeInLayoutFontUnits, fontBold, fontItalic, fontStrikeout, fontUnderline);
				case CharacterFormattingScript.Superscript:
					return CalcSuperscriptFontIndex(fontName, doubleFontSizeInLayoutFontUnits, fontBold, fontItalic, fontStrikeout, fontUnderline);
				default:
					Exceptions.ThrowInternalException();
					return -1;
			}
		}
		public virtual bool ShouldUseDefaultFontToDrawInvisibleCharacter(FontInfo fontInfo, char character) {
			return true;
		}
		protected internal virtual int CalcSubscriptFontSize(int baseFontIndex) {
			FontInfo fontInfo = fontInfoList[baseFontIndex];
			return (int)Math.Round(fontInfo.SubscriptSize.Height * unitConverter.FontSizeScale);
		}
		protected internal virtual int CalcSuperscriptFontSize(int baseFontIndex) {
			FontInfo fontInfo = fontInfoList[baseFontIndex];
			return (int)Math.Round(fontInfo.SuperscriptSize.Height * unitConverter.FontSizeScale);
		}
		public virtual int CalcNormalFontIndex(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			return CalcFontIndexCore(fontName, doubleFontSize, fontBold, fontItalic, CharacterFormattingScript.Normal, fontStrikeout, fontUnderline);
		}
		protected internal virtual int CalcSuperscriptFontIndex(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			int baseFontIndex = CalcFontIndexCore(fontName, doubleFontSize, fontBold, fontItalic, CharacterFormattingScript.Normal, fontStrikeout, fontUnderline);
			int superscriptFontSize = CalcSuperscriptFontSize(baseFontIndex);
			int fontIndex = CalcFontIndexCore(fontName, superscriptFontSize * 2, fontBold, fontItalic, CharacterFormattingScript.Superscript, fontStrikeout, fontUnderline);
			FontInfo fontInfo = this[fontIndex];
			fontInfo.CalculateSuperscriptOffset(this[baseFontIndex]);
			fontInfo.BaseFontIndex = baseFontIndex;
			return fontIndex;
		}
		protected internal virtual int CalcSubscriptFontIndex(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			int baseFontIndex = CalcFontIndexCore(fontName, doubleFontSize, fontBold, fontItalic, CharacterFormattingScript.Normal, fontStrikeout, fontUnderline);
			int subscriptFontSize = CalcSubscriptFontSize(baseFontIndex);
			int fontIndex = CalcFontIndexCore(fontName, subscriptFontSize * 2, fontBold, fontItalic, CharacterFormattingScript.Subscript, fontStrikeout, fontUnderline);
			FontInfo fontInfo = this[fontIndex];
			fontInfo.CalculateSubscriptOffset(this[baseFontIndex]);
			fontInfo.BaseFontIndex = baseFontIndex;
			return fontIndex;
		}
		protected internal virtual int CalcFontIndexCore(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, CharacterFormattingScript script, bool fontStrikeout, bool fontUnderline) {
			Int64 hash = CalcHash(fontName, doubleFontSize, fontBold, fontItalic, script, fontStrikeout, fontUnderline);
			int result;
			if (!indexHash.TryGetValue(hash, out result)) {
				lock (this) {
					if (!indexHash.TryGetValue(hash, out result)) 
						return CreateFontInfo(hash, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
					else
						return result;
				}
			}
			else
				return result;
		}
		protected internal virtual Int64 CalcHash(string fontName, int doubleFontSize, bool fontBold, bool fontItalic, CharacterFormattingScript script, bool fontStrikeout, bool fontUnderline) {
			Int64 style = ((int)script | ((fontBold ? 0 : 1) << 3) | ((fontItalic ? 0 : 1) << 2) | ((fontStrikeout ? 0 : 1) << 4) | ((fontUnderline ? 0 : 1) << 5)); 
			Int64 fontNameHash = fontName.ToLower().GetHashCode();
			return ((doubleFontSize << 6) + style) + (fontNameHash << 32);
		}
		protected internal virtual int CreateFontInfo(Int64 hash, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			FontInfo fontInfo = CreateFontInfoCore(fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
			int result = fontInfoList.Count;
			fontInfoList.Add(fontInfo);
			if (!charsets.ContainsKey(fontName))
				charsets.Add(fontName, fontInfo.Charset);
			indexHash.Add(hash, result);
			return result;
		}
		protected abstract List<FontCharacterRange> GetFontCharacterRanges(FontInfo fontInfo);
		public int GetCharsetByFontName(string fontName) {
			int charset;
			if (!this.charsets.TryGetValue(fontName, out charset))
				CalcFontIndex(fontName, 24, false, false, CharacterFormattingScript.Normal, false, false);
			return charsets[fontName];
		}
#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposeContent();
				if (measurer != null) {
					measurer.Dispose();
					measurer = null;
				}
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
#endregion
		protected internal virtual void DisposeContent() {
			if (fontInfoList != null) {
				int count = fontInfoList.Count;
				for (int i = 0; i < count; i++)
					this[i].Dispose();
				fontInfoList.Clear();
				fontInfoList = null;
			}
			if (indexHash != null) {
				indexHash.Clear();
				indexHash = null;
			}
		}
		public virtual string FindSubstituteFont(string sourceFontName, char ch) {
			PopulateNameToCharacterSetMap();
			int minDistance = Int32.MaxValue;
			string result = sourceFontName;
			lock (NameToCharacterSetMap) {
				FontCharacterSet sourceCharacterSet = NameToCharacterSetMap[sourceFontName];
				foreach (KeyValuePair<string, FontCharacterSet> fontCharacterSetPair in NameToCharacterSetMap) {
					FontCharacterSet fontCharacterSet = fontCharacterSetPair.Value;
					if (fontCharacterSet.ContainsChar(ch)) {
						int distance = FontCharacterSet.CalculatePanoseDistance(sourceCharacterSet, fontCharacterSet);
						if (distance < minDistance) {
							minDistance = distance;
							result = fontCharacterSetPair.Key;
						}
					}
				}
			}
			return result;
		}
		protected internal abstract void PopulateNameToCharacterSetMap();
		public abstract FontCharacterSet GetFontCharacterSet(string fontName);
	}
#endregion
#region FontSizeSortedReadonlyList
	public class FontSizeSortedReadonlyList : IList<int> {
		readonly int count;
		public FontSizeSortedReadonlyList(int count) {
			Guard.ArgumentNonNegative(count, "count");
			this.count = count;
		}
#region IList<int> Members
		public int IndexOf(int item) {
			return item;
		}
		public void Insert(int index, int item) {
		}
		public void RemoveAt(int index) {
		}
		public int this[int index] { get { return index; } set { } }
#endregion
#region ICollection<int> Members
		public void Add(int item) {
		}
		public void Clear() {
		}
		public bool Contains(int item) {
			return item >= 0 && item < count;
		}
		public void CopyTo(int[] array, int arrayIndex) {
		}
		public int Count { get { return count; } }
		public bool IsReadOnly { get { return true; } }
		public bool Remove(int item) {
			return false;
		}
#endregion
#region IEnumerable<int> Members
		public IEnumerator<int> GetEnumerator() {
			return null;
		}
#endregion
#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return null;
		}
#endregion
	}
#endregion
#region ScriptFontSizeComparableBase (abstract class)
	public abstract class ScriptFontSizeComparableBase : IComparable<int> {
		readonly string fontName;
		readonly bool fontBold;
		readonly bool fontItalic;
		readonly bool fontStrikeout;
		readonly bool fontUnderline;
		readonly FontCache fontCache;
		readonly int formattingFontSize;
		readonly int doubleformattingFontSize;
		protected ScriptFontSizeComparableBase(FontCache fontCache, string fontName, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline, int scriptFontSize, int doublescriptFontSize) {
			Guard.ArgumentNotNull(fontCache, "fontCache");
			this.fontCache = fontCache;
			this.fontName = fontName;
			this.fontBold = fontBold;
			this.fontItalic = fontItalic;
			this.fontStrikeout = fontStrikeout;
			this.fontUnderline = fontUnderline;
			this.formattingFontSize = fontCache.UnitConverter.PointsToFontUnits(scriptFontSize);
			this.doubleformattingFontSize = fontCache.UnitConverter.PointsToFontUnits(doublescriptFontSize);
		}
#region IComparable<int> Members
		public int CompareTo(int doublebaseFontSize) {
			return Math.Sign(CalculateDistance(doublebaseFontSize));
		}
#endregion
		public int CalculateDistance(int doublebaseFontSize) {
			int index = fontCache.CalcFontIndex(fontName, doublebaseFontSize, fontBold, fontItalic, CharacterFormattingScript.Normal, fontStrikeout, fontUnderline);
			int scriptFontSize = CalculateScriptFontSize(fontCache, index);
			return scriptFontSize - formattingFontSize;
		}
		protected internal abstract int CalculateScriptFontSize(FontCache fontCache, int baseFontIndex);
	}
#endregion
#region SuperscriptFontSizeComparable
	public class SuperscriptFontSizeComparable : ScriptFontSizeComparableBase {
		public SuperscriptFontSizeComparable(FontCache fontCache, string fontName, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline, int scriptFontSize, int doublescriptFontSize)
			: base(fontCache, fontName, fontBold, fontItalic, fontStrikeout, fontUnderline, scriptFontSize, doublescriptFontSize) {
		}
		protected internal override int CalculateScriptFontSize(FontCache fontCache, int baseFontIndex) {
			return fontCache.CalcSuperscriptFontSize(baseFontIndex);
		}
	}
#endregion
#region SubscriptFontSizeComparable
	public class SubscriptFontSizeComparable : ScriptFontSizeComparableBase {
		public SubscriptFontSizeComparable(FontCache fontCache, string fontName, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline, int scriptFontSize, int doublescriptFontSize)
			: base(fontCache, fontName, fontBold, fontItalic, fontStrikeout, fontUnderline, scriptFontSize, doublescriptFontSize) {
		}
		protected internal override int CalculateScriptFontSize(FontCache fontCache, int baseFontIndex) {
			return fontCache.CalcSubscriptFontSize(baseFontIndex);
		}
	}
#endregion
}
