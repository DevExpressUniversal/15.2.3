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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region RunFontInfoCache
	public class RunFontInfoCache : UniqueItemsCache<RunFontInfo> {
		internal const int DefaultItemIndex = 0;
		public RunFontInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override RunFontInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return RunFontInfo.CreateDefault();
		}
#if DEBUGTEST
		public static bool CheckDefaults2(RunFontInfoCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			RunFontInfo info = (RunFontInfo)collection.DefaultItem;
			result &= 0 == info.Charset;
			result &= ! info.Condense;
			result &= ! info.Extend;
			result &= ! info.Bold;
			result &= 2 == info.FontFamily;
			result &= ! info.Italic;
			result &= "Calibri" == info.Name;
			result &= ! info.Outline;
			result &= XlFontSchemeStyles.Minor == info.SchemeStyle;
			result &= XlScriptType.Baseline == info.Script;
			result &= ! info.Shadow;
			result &= 11 == info.Size;
			result &= ! info.StrikeThrough;
			result &= XlUnderlineType.None == info.Underline;
			return result;
		}
#endif
	}
	#endregion
	#region RunFontInfo
	public class RunFontInfo : XlFontBase, ICloneable<RunFontInfo>, ISupportsCopyFrom<RunFontInfo>, ISupportsSizeOf {
		#region Static Members
		internal static RunFontInfo CreateDefault() {
			RunFontInfo result = new RunFontInfo();
			result.Name = "Calibri";
			result.Size = 11;
			result.SchemeStyle = XlFontSchemeStyles.Minor;
			result.FontFamily = 2;
			return result;
		}
		#endregion 
		#region Fields
		int fontFamily;
		int colorIndex;
		int fontCacheIndex = -1;
		#endregion
		#region Properties
		public int FontFamily { get { return fontFamily; } set { fontFamily = value; } }
		public int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RunFontInfo info = (RunFontInfo)obj;
			return this.FontFamily == info.FontFamily && this.ColorIndex == info.ColorIndex;
		}
		public bool EqualsNoColorIndex(RunFontInfo info) {
			return base.Equals(info) && this.FontFamily == info.FontFamily;
		}
		internal bool EqualsForDifferentWorkbooks(RunFontInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel) {
			return EqualsNoColorIndex(otherInfo) && EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel);
		}
		bool EqualsByColorOnly(RunFontInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel) {
			return GetColorModelInfo(targetDocumentModel).Rgb == otherInfo.GetColorModelInfo(otherDocumentModel).Rgb;
		}
		public override int GetHashCode() {
			return Office.Utils.HashCodeCalculator.CalcHashCode32(base.GetHashCode(), colorIndex, fontFamily);
		}
		#region ICloneable<RunFontInfo> Members
		public RunFontInfo Clone() {
			RunFontInfo result = new RunFontInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<RunFontInfo> Members
		public void CopyFrom(RunFontInfo value) {
			base.CopyFrom(value);
			this.FontFamily = value.FontFamily;
			this.ColorIndex = value.ColorIndex;
		}
		#endregion
		public void CopyFrom(IActualRunFontInfo value) {
			this.Bold = value.Bold;
			this.Charset = value.Charset;
			this.ColorIndex = value.ColorIndex;
			this.Condense = value.Condense;
			this.Extend = value.Extend;
			this.FontFamily = value.FontFamily;
			this.Italic = value.Italic;
			this.Name = value.Name;
			this.Outline = value.Outline;
			this.SchemeStyle = value.SchemeStyle;
			this.Script = value.Script;
			this.Shadow = value.Shadow;
			this.Size = value.Size;
			this.StrikeThrough = value.StrikeThrough;
			this.Underline = value.Underline;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return 0;
		}
		#endregion
		public ColorModelInfo GetColorModelInfo(DocumentModel documentModel) {
			return documentModel.Cache.ColorModelInfoCache[ColorIndex];
		} 
		public FontInfo GetFontInfo(FontCache cache) {
			if (fontCacheIndex < 0)
				fontCacheIndex = cache.CalcFontIndex(Name, (int)Math.Round(Size) * 2, Bold, Italic, ConvertScript(Script), StrikeThrough, Underline != XlUnderlineType.None);
			return cache[fontCacheIndex];
		}
		protected internal FontInfo GetFontInfoNoCache(FontCache cache) {
			int index = cache.CalcFontIndex(Name, (int)Math.Round(Size) * 2, Bold, Italic, ConvertScript(Script), StrikeThrough, Underline != XlUnderlineType.None);
			return cache[index];
		}
		protected internal static FontInfo GetFontInfoNoCache(FontCache cache, string name, double size, bool bold, bool italic, XlScriptType script, bool strikeThrough, XlUnderlineType underline) {
			int index = cache.CalcFontIndex(name, (int)Math.Round(size) * 2, bold, italic, ConvertScript(script), strikeThrough, underline != XlUnderlineType.None);
			return cache[index];
		}
		protected internal static int GetFontInfoIndex(FontCache cache, string name, double size, bool bold, bool italic, XlScriptType script, bool strikeThrough, XlUnderlineType underline) {
			return cache.CalcFontIndex(name, (int)Math.Round(size) * 2, bold, italic, ConvertScript(script), strikeThrough, underline != XlUnderlineType.None);
		}
		public void ResetFontInfoIndex() {
			fontCacheIndex = -1;
		}
		static CharacterFormattingScript ConvertScript(XlScriptType script) {
			switch (script) {
				default:
				case XlScriptType.Baseline:
					return CharacterFormattingScript.Normal;
				case XlScriptType.Superscript:
					return CharacterFormattingScript.Superscript;
				case XlScriptType.Subscript:
					return CharacterFormattingScript.Subscript;
			}
		}
	}
	#endregion
	#region IRunFontInfo
	public interface IRunFontInfo {
		string Name { get; set; }
		Color Color { get; set; }
		bool Bold { get; set; }
		bool Condense { get; set; }
		bool Extend { get; set; }
		bool Italic { get; set; }
		bool Outline { get; set; }
		bool Shadow { get; set; }
		bool StrikeThrough { get; set; }
		int Charset { get; set; }
		int FontFamily { get; set; }
		double Size { get; set; }
		XlFontSchemeStyles SchemeStyle { get; set; }
		XlScriptType Script { get; set; }
		XlUnderlineType Underline { get; set; }
	}
	#endregion
	#region IActualRunFontInfo
	public interface IActualRunFontInfo {
		string Name { get; }
		Color Color { get; }
		bool Bold { get; }
		bool Condense { get; }
		bool Extend { get; }
		bool Italic { get; }
		bool Outline { get; }
		bool Shadow { get; }
		bool StrikeThrough { get; }
		int Charset { get; }
		int FontFamily { get; }
		double Size { get; }
		XlFontSchemeStyles SchemeStyle { get; }
		XlScriptType Script { get; }
		XlUnderlineType Underline { get; }
		int ColorIndex { get; }
		FontInfo GetFontInfo();
	}
	#endregion
	#region ImportFontInfo
	public class ImportFontInfo {
		public bool? Bold { get; set; }
		public bool? Condense { get; set; }
		public bool? Extend { get; set; }
		public bool? Italic { get; set; }
		public bool? Outline { get; set; }
		public bool? Shadow { get; set; }
		public bool? StrikeThrough { get; set; }
		public XlFontSchemeStyles? SchemeStyle { get; set; }
		public XlScriptType? Script { get; set; }
		public XlUnderlineType? Underline { get; set; }
		public int? Charset { get; set; }
		public int? FontFamily { get; set; }
		public double? Size { get; set; }
		public int? ColorIndex { get; set; }
		public string Name { get; set; }
	}
	#endregion
	#region SpreadsheetPackedValuesInfoObject
	public abstract class SpreadsheetPackedValuesInfoObject {
		uint packedValues;
		[CLSCompliant(false)]
		protected uint PackedValues { get { return packedValues; } set { packedValues = value; } }
		public override bool Equals(object obj) {
			SpreadsheetPackedValuesInfoObject info = obj as SpreadsheetPackedValuesInfoObject;
			if (info == null)
				return false;
			return EqualsCore(info);
		}
		public override int GetHashCode() {
			return GetHashCodeCore();
		}
		#region GetVal/SetVal helpers
		internal void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		internal bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		internal uint GetUInt(uint mask, int shift) {
			return (packedValues & mask) >> shift;
		}
		internal void SetUInt(uint value, uint mask, int shift) {
			packedValues &= ~mask;
			packedValues |= (value << shift) & mask;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public abstract bool EqualsCore(SpreadsheetPackedValuesInfoObject obj);
		public abstract int GetHashCodeCore();
		public override string ToString() {
			return base.ToString();
		}
	}
	#endregion
	#region SpreadsheetUndoableIndexBasedObject<T> (abstract class)
	public abstract class SpreadsheetUndoableIndexBasedObject<T> : UndoableIndexBasedObject<T, DocumentModelChangeActions>
		where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf {
		protected SpreadsheetUndoableIndexBasedObject(IDocumentModelPartWithApplyChanges part)
			: base(part) {
		}
		public new IDocumentModelPartWithApplyChanges DocumentModelPart { get { return (IDocumentModelPartWithApplyChanges) base.DocumentModelPart; } }
		public new DocumentModel DocumentModel { get { return (DocumentModel)DocumentModelBase; } }
		protected IDocumentModel DocumentModelBase { get { return base.DocumentModel; } }
		public virtual void CopyFrom(SpreadsheetUndoableIndexBasedObject<T> sourceItem) {
			if (Object.ReferenceEquals(sourceItem.DocumentModelBase, this.DocumentModelBase))
				base.CopyFrom(sourceItem);
			else {
				this.BeginUpdate();
				sourceItem.BeginUpdate();
				try {
					this.Info.CopyFrom(sourceItem.Info);
				}
				finally {
					this.EndUpdate();
					sourceItem.EndUpdate();
				}
			}
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModelPart.ApplyChanges(changeActions);
		}
	}
	#endregion
}
