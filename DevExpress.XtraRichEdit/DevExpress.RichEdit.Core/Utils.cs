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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.Localization;
using Debug = System.Diagnostics.Debug;
#if !DXPORTABLE
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
#if !SL
using System.Data;
using System.Windows.Forms;
#else
using System.Windows.Media;
#endif
#endif
namespace DevExpress.XtraRichEdit.Utils {
#region SortedList<T>
	public class SortedList<T> {
		readonly List<T> innerList;
		readonly IComparer<T> comparer;
		public SortedList() {
			this.innerList = new List<T>();
		}
		public SortedList(IComparer<T> comparer)
			: this() {
			Guard.ArgumentNotNull(comparer, "comparer");
			this.comparer = comparer;
		}
		public T this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
#region First
		public T First {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return innerList[0];
			}
		}
#endregion
#region Last
		public T Last {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return innerList[Count - 1];
			}
		}
#endregion
		public virtual void Add(T value) {
			int index = BinarySearch(value);
			if (index >= 0)
				return;
			Insert(~index, value);
		}
		public void Insert(int index, T value) {
			this.innerList.Insert(index, value);
		}
		public virtual bool Contains(T value) {
			return BinarySearch(value) >= 0;
		}
		public int BinarySearch(T value) {
			return this.innerList.BinarySearch(value, this.comparer);
		}
		public int BinarySearch(IComparable<T> predicate) {
			return Algorithms.BinarySearch(this.innerList, predicate);
		}
		public void RemoveAt(int index) {
			this.innerList.RemoveAt(index);
		}
		public virtual void Remove(T value) {
			int index = BinarySearch(value);
			if (index >= 0)
				this.RemoveAt(index);
		}
		public virtual void RemoveFrom(T value) {
			int startIndex = BinarySearch(value);
			if (startIndex < 0)
				startIndex = ~startIndex;
			for (int i = this.innerList.Count - 1; i >= startIndex; i--)
				this.RemoveAt(i);
		}
		public virtual void Clear() {
			this.innerList.Clear();
		}
		public virtual SortedList<T> Clone() {
			SortedList<T> result = CreateEmptyList();
			CopyCore(result);
			return result;
		}
		protected virtual void CopyCore(SortedList<T> destination) {
			destination.innerList.AddRange(innerList);
		}
		protected virtual SortedList<T> CreateEmptyList() {
			return new SortedList<T>();
		}
	}
#endregion
#region TableWidthUnitType
	public enum TableWidthUnitType {
		Fixed,
		Auto,
		Nil, 
		Pct 
	}
#endregion
#region TableWidthUnit
	public class TableWidthUnit : ICloneable<TableWidthUnit> {
		readonly TableWidthUnitType type;
		readonly int value;
		public TableWidthUnit()
			: this(TableWidthUnitType.Auto) {
		}
		public TableWidthUnit(int value)
			: this(value, TableWidthUnitType.Fixed) {
		}
		public TableWidthUnit(TableWidthUnitType type)
			: this(0, type) {
		}
		public TableWidthUnit(int value, TableWidthUnitType type) {
			this.value = value;
			this.type = type;
		}
		public TableWidthUnitType Type { get { return type; } }
		public int Value { get { return value; } }
		public static TableWidthUnit operator +(TableWidthUnit firstOperand, TableWidthUnit secondOperand) {
			if (firstOperand.Type != TableWidthUnitType.Fixed)
				Exceptions.ThrowArgumentException("firstOperand.Type", firstOperand.Type);
			if (secondOperand.Type != TableWidthUnitType.Fixed)
				Exceptions.ThrowArgumentException("secondOperand.Type", secondOperand.Type);
			return new TableWidthUnit(firstOperand.Value + secondOperand.Value);
		}
		public static TableWidthUnit operator -(TableWidthUnit firstOperand, TableWidthUnit secondOperand) {
			if (firstOperand.Type != TableWidthUnitType.Fixed)
				Exceptions.ThrowArgumentException("firstOperand.Type", firstOperand.Type);
			if (secondOperand.Type != TableWidthUnitType.Fixed)
				Exceptions.ThrowArgumentException("secondOperand.Type", secondOperand.Type);
			return new TableWidthUnit(firstOperand.Value - secondOperand.Value);
		}
		public override bool Equals(object obj) {
			TableWidthUnit unit = obj as TableWidthUnit;
			if (unit == null)
				return false;
			return (Value == unit.Value && Type == unit.Type);
		}
		public static bool operator ==(TableWidthUnit first, TableWidthUnit second) {
			if (Object.ReferenceEquals(first, second))
				return true;
			if (Object.ReferenceEquals(first, null) || Object.ReferenceEquals(second, null))
				return false;
			return first.Equals(second);
		}
		public static bool operator !=(TableWidthUnit first, TableWidthUnit second) {
			return !(first == second);
		}
		public override int GetHashCode() {
			return Value | ((int)Type << 28);
		}
		public TableWidthUnit Clone() {
			return new TableWidthUnit(Value, Type);
		}
	}
#endregion
#region TableHeightUnitType
	public enum TableHeightUnitType {
		Min,
		Auto,
		Exact
	}
#endregion
#region TableHeightUnit
	public class TableHeightUnit : ICloneable<TableHeightUnit> {
		readonly TableHeightUnitType type;
		readonly int value;
		public TableHeightUnit()
			: this(TableHeightUnitType.Auto) {
		}
		public TableHeightUnit(int value)
			: this(value, TableHeightUnitType.Exact) {
		}
		public TableHeightUnit(TableHeightUnitType type)
			: this(0, type) {
		}
		public TableHeightUnit(int value, TableHeightUnitType type) {
			this.value = value;
			this.type = type;
		}
		public TableHeightUnitType Type { get { return type; } }
		public int Value { get { return value; } }
		public static TableHeightUnit operator +(TableHeightUnit firstOperand, TableHeightUnit secondOperand) {
			if (firstOperand.Type != TableHeightUnitType.Exact)
				Exceptions.ThrowArgumentException("firstOperand.Type", firstOperand.Type);
			if (secondOperand.Type != TableHeightUnitType.Exact)
				Exceptions.ThrowArgumentException("secondOperand.Type", secondOperand.Type);
			return new TableHeightUnit(firstOperand.Value + secondOperand.Value);
		}
		public static TableHeightUnit operator -(TableHeightUnit firstOperand, TableHeightUnit secondOperand) {
			if (firstOperand.Type != TableHeightUnitType.Exact)
				Exceptions.ThrowArgumentException("firstOperand.Type", firstOperand.Type);
			if (secondOperand.Type != TableHeightUnitType.Exact)
				Exceptions.ThrowArgumentException("secondOperand.Type", secondOperand.Type);
			return new TableHeightUnit(firstOperand.Value - secondOperand.Value);
		}
		public override bool Equals(object obj) {
			TableHeightUnit unit = obj as TableHeightUnit;
			if (unit == null)
				return false;
			return (Value == unit.Value && Type == unit.Type);
		}
		public override int GetHashCode() {
			return Value | ((int)Type << 28);
		}
		public TableHeightUnit Clone() {
			return new TableHeightUnit(Value, Type);
		}
	}
#endregion
#region RichEditExceptions
	public static class RichEditExceptions {
		public static void ThrowInvalidOperationException(XtraRichEditStringId id) {
			Exceptions.ThrowInvalidOperationException(XtraRichEditLocalizer.GetString(id));
		}
	}
#endregion
#region ColorCollection
	public class ColorCollection : List<Color> {
	}
#endregion
#region TextManipulatorHelper
	public static class TextManipulatorHelper {
		public static void SetText(PieceTable table, string text) {
			SetText(table, text, String.Empty, 0);
		}
		public static void SetText(PieceTable table, string text, string defaultFontName, int doubleFontSize) {
			DocumentModel documentModel = table.DocumentModel;
			documentModel.BeginSetContent();
			try {
				if(!String.IsNullOrEmpty(defaultFontName))
					documentModel.DefaultCharacterProperties.FontName = defaultFontName;
				if(doubleFontSize > 0)
					documentModel.DefaultCharacterProperties.DoubleFontSize = doubleFontSize;
				SetTextCore(table, text);
			}
			finally {
				documentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
		}
		static String[] newLineStrings = new String[] { "\r\n", "\n\r", "\r", "\n" };
		public static string[] GetPlainTextLines(string text) {
			if (String.IsNullOrEmpty(text))
				return new String[] { String.Empty };
			return text.Split(newLineStrings, StringSplitOptions.None);
		}
		internal static void SetTextCore(PieceTable table, string text) {
			text = text.Replace(Characters.SectionMark, Characters.ParagraphMark);
			string[] lines = GetPlainTextLines(text);
			SetTextLinesCore(table, lines);
		}
		public static void SetTextForeColor(PieceTable table, Color foreColor) {
			DocumentModel documentModel = table.DocumentModel;
			documentModel.History.Clear();
			documentModel.SwitchToEmptyHistory(true);
			try {
				TextRunCollection runs = table.Runs;
				RunIndex runCount = new RunIndex(runs.Count);
				for (RunIndex i = new RunIndex(0); i < runCount; i++) {
					TextRun run = runs[i] as TextRun;
					if (run != null)
						run.ForeColor = foreColor;
				}
			}
			finally {
				documentModel.SwitchToNormalHistory(true);
			}
		}
		public static void SetTextLines(PieceTable table, string[] lines) {
			SetTextLines(table, lines, String.Empty, 0);
		}
		public static void SetTextLines(PieceTable table, string[] lines, string defaultFontName, int doubleFontSize) {
			DocumentModel documentModel = table.DocumentModel;
			documentModel.BeginSetContent();
			try {
				if(!String.IsNullOrEmpty(defaultFontName))
					documentModel.DefaultCharacterProperties.FontName = defaultFontName;
				if(doubleFontSize > 0)
					documentModel.DefaultCharacterProperties.DoubleFontSize = doubleFontSize;
				SetTextLinesCore(table, lines);
			}
			finally {
				documentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
		}
		static void SetTextLinesCore(PieceTable table, string[] lines) {
			int count = lines.Length;
			int maxIndex = count - 1;
			if (maxIndex < 0)
				return;
			InputPosition pos = new InputPosition(table);
			pos.CharacterFormatting.CopyFrom(table.Runs.First.CharacterProperties.Info);
			pos.CharacterStyleIndex = table.Runs.First.CharacterStyleIndex;
			for (int i = 0; i < maxIndex; i++) {
				InsertTextLine(table, pos, lines[i]);
				table.InsertParagraphCoreNoInheritParagraphRunStyle(pos);
			}
			InsertTextLine(table, pos, lines[maxIndex]);
		}
		static void InsertTextLine(PieceTable table, InputPosition pos, string text) {
			if (text != null && text.Length > 0)
				table.InsertTextCoreNoResetMergeNoApplyFormatting(pos, text, false);
		}
	}
#endregion
#region PositionConverter
	public static class PositionConverter {
		public static DocumentModelPosition ToDocumentModelPosition(PieceTable pieceTable, DocumentLogPosition pos) {
			DocumentModelPosition result = new DocumentModelPosition(pieceTable);
			result.LogPosition = pos;
			result.Update();
			return result;
		}
		public static DocumentModelPosition ToDocumentModelPosition(PieceTable pieceTable, FormatterPosition pos) {
			DocumentModelPosition result = new DocumentModelPosition(pieceTable);
			RunIndex runIndex = pos.RunIndex;
			result.RunIndex = runIndex;
			Paragraph paragraph = pieceTable.Runs[runIndex].Paragraph;
			result.ParagraphIndex = paragraph.Index;
			result.RunStartLogPosition = GetRunStartLogPosition(runIndex, paragraph);
			result.LogPosition = result.RunStartLogPosition + pos.Offset;
			return result;
		}
		static DocumentLogPosition GetRunStartLogPosition(RunIndex runIndex, Paragraph paragraph) {
			TextRunCollection runs = paragraph.PieceTable.Runs;
			RunIndex firstRunIndex = paragraph.FirstRunIndex;
			DocumentLogPosition result = paragraph.LogPosition;
			for (RunIndex i = firstRunIndex; i < runIndex; i++) {
				result += runs[i].Length;
			}
			return result;
		}
		public static FormatterPosition ToFormatterPosition(DocumentModelPosition pos) {
			return new FormatterPosition(pos.RunIndex, pos.RunOffset, 0);
		}
		public static FormatterPosition ToFormatterPosition(PieceTable pieceTable, DocumentLogPosition pos) {
			DocumentModelPosition modelPos = ToDocumentModelPosition(pieceTable, pos);
			return ToFormatterPosition(modelPos);
		}
	}
#endregion
#if !SL && !DXPORTABLE
	public static class DataHelper {
		static BindingContext bindingContext = new BindingContext();
		static BindingManagerBase GetBindingManager(object dataSource, string dataMember) {
			BindingManagerBase mgr = null;
			if (dataSource != null) {
				try {
					if (dataMember.Length > 0)
						mgr = bindingContext[dataSource, dataMember];
					else
						mgr = bindingContext[dataSource];
				}
				catch { }
			}
			return mgr;
		}
		public static PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember) {
			BindingManagerBase mgr = GetBindingManager(dataSource, dataMember);
			return (mgr != null) ? mgr.GetItemProperties() : null;
		}
		public static IList GetList(object dataSource, string dataMember) {
			if (dataMember == null)
				dataMember = String.Empty;
			bool useContext = !((dataSource is DataSet) && dataMember.Length == 0);
			return DevExpress.Data.Helpers.MasterDetailHelper.GetDataSource((useContext ? bindingContext : null), dataSource, dataMember);
		}
	}
#else
	public static class DataHelper {
		public static IList GetList(object dataSource, string dataMember) {
			return null;
		}
	}
#endif
#region SymbolProperties
	public class SymbolProperties {
#region Fields
		char unicodeChar;
		string fontName;
#endregion
		public SymbolProperties() {
			this.unicodeChar = ' ';
			this.fontName = String.Empty;
		}
		public SymbolProperties(char unicodeChar, string fontName) {
			this.unicodeChar = unicodeChar;
			this.fontName = fontName;
		}
#region Properties
		public char UnicodeChar { get { return unicodeChar; } set { unicodeChar = value; } }
		public string FontName { get { return fontName; } set { fontName = value; } }
#endregion
		public override bool Equals(object obj) {
			SymbolProperties symbolProperties = obj as SymbolProperties;
			if (symbolProperties == null)
				return false;
			bool isCharIdentical = symbolProperties.UnicodeChar == UnicodeChar;
			bool isFontNameIdentical = symbolProperties.FontName == FontName;
			return isCharIdentical && isFontNameIdentical;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
#endregion
#region ImeCompositionInfo
	public class ImeCompositionInfo {
		int length = 0;
		DocumentLogPosition startPos;
		public ImeCompositionInfo() {
			Initialize();
		}
		public int Length { get { return length; } set { length = value; } }
		public DocumentLogPosition StartPos { get { return startPos; } set { startPos = value; } }
		public void Initialize() {
			this.startPos = DocumentLogPosition.Zero;
			this.length = 0;
		}
		public virtual void Reset(DocumentModel model) {
			this.startPos = model.Selection.Start;
			this.length = 0;
		}
		public virtual RunInfo GetRunInfo(DocumentModel model) {
			if (Length == 0)
				return null;
			return model.ActivePieceTable.FindRunInfo(StartPos, Length);
		}
	}
#endregion
#region FontsExportHelper
	public class FontsExportHelper {
#region Fields
		const string defaultFontName = "Times New Roman";
		readonly int defaultFontIndex;
		readonly List<string> fontNamesCollection;
#endregion
		public FontsExportHelper() {
			this.fontNamesCollection = new List<string>();
			this.defaultFontIndex = GetFontIndexByName(defaultFontName);
		}
#region Properties
		public List<string> FontNamesCollection { get { return fontNamesCollection; } }
		public int DefaultFontIndex { get { return defaultFontIndex; } }
#endregion
		public int GetFontIndexByName(string fontName) {
			int count = fontNamesCollection.Count;
			for (int i = 0; i < count; i++) {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(fontName, fontNamesCollection[i]) == 0)
					return i;
			}
			fontNamesCollection.Add(fontName);
			return count;
		}
		public string GetFontNameByIndex(int index) {
			if (index >= this.fontNamesCollection.Count)
				return String.Empty;
			return this.fontNamesCollection[index];
		}
	}
#endregion
#region StringHelper
	public static class StringHelper {
		static char lastLowSpecial = '\x1f';
		static char firstHighSpecial = '\xffff';
		const int maxDocFontNameLength = 42;
		public static string RemoveSpecialSymbols(string text) {
			if (!ContainsSpecialSymbols(text))
				return text;
			StringBuilder sb = new StringBuilder();
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				char ch = text[i];
				if (ch > lastLowSpecial && ch < firstHighSpecial)
					sb.Append(ch);
				else {
					if (ch == '\x9' || ch == '\xA' || ch == '\xD')
						sb.Append(ch);
				}
			}
			return sb.ToString();
		}
		public static string ReplaceParagraphMarksWithLineBreaks(string text) {
			if (!ContainsParagraphMarksOrUnitSeparators(text))
				return text;
			StringBuilder sb = new StringBuilder();
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				char ch = text[i];
				if (ch != '\n' && ch != '\r' && ch != '\x1f')
					sb.Append(ch);
				else if (ch != '\x1f') {
					sb.Append(Characters.LineBreak);
					if (i != count - 1) {
						char nextCh = text[i + 1];
						if ((nextCh == '\n' || nextCh == '\r') && nextCh != ch)
							i++;						
					}
				}
			}
			return sb.ToString();
		}
		static bool ContainsParagraphMarksOrUnitSeparators(string text) {
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				char ch = text[i];
				if (ch == '\n' || ch == '\r' || ch == '\x1f')
					return true;
			}
			return false;
		}
		static bool ContainsSpecialSymbols(string text) {
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				char ch = text[i];
				if (ch <= lastLowSpecial || ch >= firstHighSpecial) {
					if (ch != '\x9' && ch != '\xA' && ch != '\xD')
						return true;
				}
			}
			return false;
		}
		public static string PrepareFontNameForDoc(string originalFontName) {
			originalFontName = RemoveSpecialSymbols(originalFontName);
			if (originalFontName.Length <= maxDocFontNameLength)
				return originalFontName;
			int trimIndex = originalFontName.IndexOf(',');
			if (trimIndex > 0)
				originalFontName = originalFontName.Substring(0, trimIndex);
			if (originalFontName.Length > maxDocFontNameLength)
				originalFontName = originalFontName.Substring(0, maxDocFontNameLength);
			return originalFontName;
		}
	}
#endregion
	public static class DateTimeUtils {
		public static DateTime FromDateTimeDTTM(int dateTime_DTTM) {
			if (dateTime_DTTM == 0)
				return DateTime.MinValue;
			int minute = Math.Min(59, dateTime_DTTM & 0x003f);
			int hour = Math.Min(23, (dateTime_DTTM & 0x07C0) >> 6);
			int month = Math.Min(12, Math.Max(1, (dateTime_DTTM & 0x000f0000) >> 16));
			int year = Math.Max(1, (dateTime_DTTM & 0x1ff00000) >> 20) + 1900;
			int daysOfMonth = Math.Min(DateTime.DaysInMonth(year, month), Math.Max(1, (dateTime_DTTM & 0xf800) >> 11));
			return new DateTime(year, month, daysOfMonth, hour, minute, 0);
		}
		public static int ToDateTimeDTTM(DateTime dateTime) {
			if (dateTime == DateTime.MinValue)
				return 0;
			int minute = dateTime.Minute;
			int hour = dateTime.Hour << 6;
			int dayOfMonth = dateTime.Day << 11;
			int month = dateTime.Month << 16;
			int year = (dateTime.Year - 1900) << 20;
			int dayOfWeek = Convert.ToInt32(dateTime.DayOfWeek) << 29;
			return minute | hour | dayOfMonth | month | year | dayOfWeek;
		}
		public static DateTime FromDateTimeISO8601(string dateTime_ISO8601) {
			DateTime result;
			string[] formats = new string[] { "yyyy-MM-ddTHH:mm:ssZ", "-yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss", "-yyyy-MM-ddTHH:mm:ss", "o", 
											"yyyy-MM-ddTHH:mm:ss.fff", "-yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.fffff", "-yyyy-MM-ddTHH:mm:ss.fffff"};
			if (DateTime.TryParseExact(dateTime_ISO8601, formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.None, out result))
				return result;
			DateTimeOffset resultOffset;
			if (DateTimeOffset.TryParse(dateTime_ISO8601, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out resultOffset))
				return resultOffset.DateTime - resultOffset.Offset; 
			return Comment.MinCommentDate;
		}
		public static string ToDateTimeISO8601(DateTime dateTime) {
			return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
		}
	}
	public class AutoColorUtils {
		public static int CalculateLumaY(Color color) {
			return 19595 * color.R + 38470 * color.G + 7471 * color.B;
		}
		public static bool IsDarkColor(Color color, int boundary) {			
			return CalculateLumaY(color) < boundary;
		}
		public static Color GetActualForeColor(Color backColor, Color foreColor, TextColors textColors) {
			if (foreColor != DXColor.Empty)
				return foreColor;
			if (backColor != DXColor.Empty && IsDarkColor(backColor, textColors.DarkBoundary))
				return textColors.LightTextColor;
			else
				return textColors.DarkTextColor;
		}
	}
	public class TextColors {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104")]
		public static readonly TextColors Defaults;
		public const int defaultBoundary = 3982098;
		static TextColors() {
			Defaults = FromSkinColors(DXSystemColors.Window, DXColor.Black); 
		}
		public static TextColors FromSkinColors(Color backgroundColor, Color textColor) {
			int backgroundLuma = AutoColorUtils.CalculateLumaY(backgroundColor);
			int textColorLuma = AutoColorUtils.CalculateLumaY(textColor);
			if (backgroundLuma < textColorLuma)
				return new TextColors(backgroundColor, textColor, DXColor.Black, Math.Max(backgroundLuma + 1, defaultBoundary));
			else
				return new TextColors(backgroundColor, DXColor.White, textColor, Math.Min(backgroundLuma - 1, defaultBoundary));
		}
		readonly Color defaultBackgroundColor;
		readonly Color lightTextColor;
		readonly Color darkTextColor;
		readonly int darkBoundary;
		public TextColors(Color defaultBackgroundColor, Color lightTextColor, Color darkTextColor, int darkBoundary) {
			this.defaultBackgroundColor = defaultBackgroundColor;
			this.lightTextColor = lightTextColor;
			this.darkTextColor = darkTextColor;
			this.darkBoundary = darkBoundary;
		}
		public Color LightTextColor { get { return lightTextColor; } }
		public Color DarkTextColor { get { return darkTextColor; } }
		public Color DefaultBackgroundColor { get { return defaultBackgroundColor; } }
		public int DarkBoundary { get { return darkBoundary; } }
	}
}
namespace DevExpress.XtraRichEdit.Internal {
#region CopyHelper
	public static class CopyHelper {
		public static void CopyDocumentModel(DocumentModel source, DocumentModel target, bool includeUnreferenced) {
			CopyDocumentModelProperties(source, target);
			CopyCore(source.MainPieceTable, target.MainPieceTable);
			CopySections(source.Sections, target.Sections);			
			if (includeUnreferenced)
				CopyUnreferencedNotes(source, target);
		}
		static void CopyDocumentModelProperties(DocumentModel source, DocumentModel target) {
			target.DefaultCharacterProperties.CopyFrom(source.DefaultCharacterProperties.Info);
			target.DefaultParagraphProperties.CopyFrom(source.DefaultParagraphProperties.Info);
			target.DefaultTableCellProperties.CopyFrom(source.DefaultTableCellProperties.Info);
			target.DefaultTableProperties.CopyFrom(source.DefaultTableProperties.Info);
			target.DefaultTableRowProperties.CopyFrom(source.DefaultTableRowProperties.Info);			
		}
		static void CopySections(SectionCollection source, SectionCollection target) {
			Debug.Assert(source.Count == target.Count);
			SectionIndex count = new SectionIndex(source.Count);
			for (SectionIndex i = new SectionIndex(0); i < count; i++)
				CopySectionCore(source[i], target[i]);
		}
		static void CopySectionCore(Section sourceSection, Section targetSection) {
			if (sourceSection.InnerFirstPageHeader == null)
				targetSection.Headers.LinkToPrevious(HeaderFooterType.First);
			else {
				targetSection.Headers.Create(HeaderFooterType.First);
				CopyCore(sourceSection.InnerFirstPageHeader.PieceTable, targetSection.InnerFirstPageHeader.PieceTable);
			}
			if (sourceSection.InnerOddPageHeader == null)
				targetSection.Headers.LinkToPrevious(HeaderFooterType.Odd);
			else {
				targetSection.Headers.Create(HeaderFooterType.Odd);
				CopyCore(sourceSection.InnerOddPageHeader.PieceTable, targetSection.InnerOddPageHeader.PieceTable);
			}
			if (sourceSection.InnerEvenPageHeader == null)
				targetSection.Headers.LinkToPrevious(HeaderFooterType.Even);
			else {
				targetSection.Headers.Create(HeaderFooterType.Even);
				CopyCore(sourceSection.InnerEvenPageHeader.PieceTable, targetSection.InnerEvenPageHeader.PieceTable);
			}
			if (sourceSection.InnerFirstPageFooter == null)
				targetSection.Footers.LinkToPrevious(HeaderFooterType.First);
			else {
				targetSection.Footers.Create(HeaderFooterType.First);
				CopyCore(sourceSection.InnerFirstPageFooter.PieceTable, targetSection.InnerFirstPageFooter.PieceTable);
			}
			if (sourceSection.InnerOddPageFooter == null)
				targetSection.Footers.LinkToPrevious(HeaderFooterType.Odd);
			else {
				targetSection.Footers.Create(HeaderFooterType.Odd);
				CopyCore(sourceSection.InnerOddPageFooter.PieceTable, targetSection.InnerOddPageFooter.PieceTable);
			}
			if (sourceSection.InnerEvenPageFooter == null)
				targetSection.Footers.LinkToPrevious(HeaderFooterType.Even);
			else {
				targetSection.Footers.Create(HeaderFooterType.Even);
				CopyCore(sourceSection.InnerEvenPageFooter.PieceTable, targetSection.InnerEvenPageFooter.PieceTable);
			}
			targetSection.CopyFromCore(sourceSection);
		}
		static void CopyUnreferencedNotes(DocumentModel source, DocumentModel target) {
			int count = source.FootNotes.Count;
			for (int i = 0; i < count; i++) {
				if (!source.FootNotes[i].IsReferenced) {
					FootNote note = new FootNote(target);
					target.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
					CopyCore(source.FootNotes[i].PieceTable, note.PieceTable);
					target.FootNotes.Add(note);
				}
			}
			count = source.EndNotes.Count;
			for (int i = 0; i < count; i++) {
				if (!source.EndNotes[i].IsReferenced) {
					EndNote note = new EndNote(target);
					target.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
					CopyCore(source.EndNotes[i].PieceTable, note.PieceTable);
					target.EndNotes.Add(note);
				}
			}
		}
		public static Field CopyAndWrapToField(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, DocumentLogPosition logPosition) {			
			Field field = targetPieceTable.CreateField(logPosition, 0);
			CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, logPosition + 1);
			return field;
		}
		public static void CopyQuoted(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, DocumentLogPosition logPosition) {
			targetPieceTable.InsertText(logPosition, "\"\"");
			CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, logPosition + 1);
		}
		public static void CopyCore(PieceTable sourcePieceTable, PieceTable targetPieceTable) {
			CopyCore(sourcePieceTable, targetPieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, sourcePieceTable.DocumentEndLogPosition - sourcePieceTable.DocumentStartLogPosition), DocumentLogPosition.Zero, false);
		}
		public static void CopyCore(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, DocumentLogPosition logPosition) {
			CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, logPosition, false);
		}
		public static void CopyCore(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, DocumentLogPosition logPosition, bool suppressJoinTables) {
			CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, logPosition, suppressJoinTables, true, UpdateFieldOperationType.Copy);
		}
		public static void CopyCore(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, DocumentLogPosition logPosition, bool suppressJoinTables, bool suppressFieldsUpdate, UpdateFieldOperationType updateFieldOperationType) {
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(sourcePieceTable, targetPieceTable, ParagraphNumerationCopyOptions.CopyAlways);
			copyManager.TargetPosition.LogPosition = logPosition;
			copyManager.TargetPosition.ParagraphIndex = targetPieceTable.FindParagraphIndex(logPosition);
			RunIndex runIndex;
			DocumentLogPosition rangeStartLogPosition = targetPieceTable.FindRunStartLogPosition(targetPieceTable.Paragraphs[copyManager.TargetPosition.ParagraphIndex], logPosition, out runIndex);
			if (rangeStartLogPosition != logPosition) {
				targetPieceTable.SplitTextRun(copyManager.TargetPosition.ParagraphIndex, runIndex, logPosition - rangeStartLogPosition);
				rangeStartLogPosition = logPosition;
				runIndex++;
			}
			copyManager.TargetPosition.RunStartLogPosition = rangeStartLogPosition;
			copyManager.TargetPosition.RunIndex = runIndex;
			CopySectionOperation operation = sourcePieceTable.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.SuppressFieldsUpdate = suppressFieldsUpdate;
			operation.SuppressJoinTables = suppressJoinTables;
			operation.UpdateFieldOperationType = updateFieldOperationType;
			operation.FixLastParagraph = false;
			operation.Execute(sourceInterval.Start, sourceInterval.Length, false);
		}
	}
#endregion
}
