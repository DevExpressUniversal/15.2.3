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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using Mask = DevExpress.XtraRichEdit.Model.CharacterFormattingOptions.Mask;
using System.Drawing;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public abstract class WebTextRunBase : IHashtableProvider {
		#region Fields
		int startIndex;
		int length = 1;
		int mergedCharacterFormattingCacheIndex = -1;
		int characterPropertiesIndex = -1;
		int characterStyleIndex;
		WebParagraph paragraph;
		#endregion
		protected WebTextRunBase(WebParagraph paragraph)
			: this(paragraph, 0, 1) {
		}
		protected WebTextRunBase(WebParagraph paragraph, int startIndex, int length) {
			this.StartIndex = startIndex;
			this.Length = length;
			this.Paragraph = paragraph;
		}
		#region Properties
		public int StartIndex {
			get { return startIndex; }
			set {
				if(value < 0)
					Exceptions.ThrowArgumentException("StartIndex", value);
				startIndex = value;
			}
		}
		public virtual int Length {
			get { return length; }
			set {
				if(value <= 0)
					Exceptions.ThrowArgumentException("Length", value);
				length = value;
			}
		}
		public int CharacterStyleIndex { get { return characterStyleIndex; } protected internal set { characterStyleIndex = value; } }
		public int MergedCharacterFormattingCacheIndex { get { return mergedCharacterFormattingCacheIndex; } protected internal set { mergedCharacterFormattingCacheIndex = value; } }
		public int CharacterPropertiesIndex { get { return characterPropertiesIndex; } protected internal set { characterPropertiesIndex = value; } }
		public WebParagraph Paragraph {
			get { return paragraph; }
			set {
				Guard.ArgumentNotNull(value, "value");
				paragraph = value;
			}
		}
		#endregion
		public abstract WebRunType RunType { get; }
		public virtual void ApplyFormatting(TextRunBase source) {
			MergedCharacterFormattingCacheIndex = source.MergedCharacterFormattingCacheIndex;
			CharacterPropertiesIndex = source.CharacterProperties.Index;
			CharacterStyleIndex = source.CharacterStyleIndex;
		}
		public virtual void FillHashtable(Hashtable result) {
			result.Add("type", (int)RunType);
			result.Add("startIndex", this.startIndex);
			result.Add("length", this.length);
			result.Add("maskedCharacterPropertiesCacheIndex", this.characterPropertiesIndex);
			result.Add("mergedCharacterFormattingCacheIndex", this.mergedCharacterFormattingCacheIndex);
			result.Add("characterStyleIndex", this.characterStyleIndex);
		}
	}
	public abstract class WebSinglePositionRun : WebTextRunBase {
		protected WebSinglePositionRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		protected WebSinglePositionRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override int Length {
			get { return base.Length; }
			set {
				if(value != 1)
					Exceptions.ThrowArgumentException("Length", value);
			}
		}
	}
	public class HashtableCollection<T> : List<T>, IConvertToHashtableCollection where T : IHashtableProvider {
		public IEnumerable<Hashtable> ToHashtableCollection() {
			List<Hashtable> result = new List<Hashtable>(Count);
			for(int i = 0; i < Count; i++) {
				var ht = new Hashtable();
				this[i].FillHashtable(ht);
				result.Add(ht);
			}
			return result;
		}
	}
	public class TextRunHashtableCollection : HashtableCollection<WebTextRunBase> {
	}
	public enum WebRunType {
		Undefined = -1,
		TextRun = 1,
		ParagraphRun = 2,
		LineNumberCommonRun = 3,
		CustomRun = 4,
		DataContainerRun = 5,
		FieldCodeStartRun = 6,
		FieldCodeEndRun = 7,
		FieldResultEndRun = 8,
		LayoutDependentTextRun = 9,
		FootNoteRun = 10,
		EndNoteRun = 11,
		InlineCustomObjectRun = 12,
		InlinePictureRun = 13,
		SectionRun = 14,
		SeparatorTextRun = 15,
		FloatingObjectAnchorRun = 16
	}
	public static class WebTextRunFactory {
		static Dictionary<Type, ConstructorInfo> constructors;
		static WebTextRunFactory() {
			constructors = new Dictionary<Type, ConstructorInfo>();
			AddConstructor(typeof(TextRun), typeof(WebTextRun));
			AddConstructor(typeof(ParagraphRun), typeof(WebParagraphRun));
			AddConstructor(typeof(LineNumberCommonRun), typeof(WebLineNumberCommonRun));
			AddConstructor(typeof(CustomRun), typeof(WebCustomRun));
			AddConstructor(typeof(DataContainerRun), typeof(WebDataContainerRun));
			AddConstructor(typeof(FieldResultEndRun), typeof(WebFieldResultEndRun));
			AddConstructor(typeof(FieldCodeStartRun), typeof(WebFieldCodeStartRun));
			AddConstructor(typeof(FieldCodeEndRun), typeof(WebFieldCodeEndRun));
			AddConstructor(typeof(LayoutDependentTextRun), typeof(WebLayoutDependentTextRun));
			AddConstructor(typeof(FootNoteRun), typeof(WebFootNoteRun));
			AddConstructor(typeof(EndNoteRun), typeof(WebEndNoteRun));
			AddConstructor(typeof(InlineCustomObjectRun), typeof(WebInlineCustomObjectRun));
			AddConstructor(typeof(InlinePictureRun), typeof(WebInlinePictureRun));
			AddConstructor(typeof(SectionRun), typeof(WebSectionRun));
			AddConstructor(typeof(SeparatorTextRun), typeof(WebSeparatorTextRun));
			AddConstructor(typeof(FloatingObjectAnchorRun), typeof(WebFloatingObjectAnchorRun));
		}
		static void AddConstructor(Type sourceType, Type resultType) {
			constructors.Add(sourceType, resultType.GetConstructor(new Type[] { typeof(WebParagraph), typeof(int), typeof(int) }));
		}
		public static WebTextRunBase CreateRun(Type sourceType, WebParagraph paragraph, int startIndex, int length) {
			ConstructorInfo constuctor;
			if(constructors.TryGetValue(sourceType, out constuctor))
				return constuctor.Invoke(new object[] { paragraph, startIndex, length }) as WebTextRunBase;
			Exceptions.ThrowInternalException();
			return null;
		}
	}
	public class WebTextRun : WebTextRunBase {
		public WebTextRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebTextRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.TextRun; } }
	}
	public class WebParagraphRun : WebSinglePositionRun {
		public WebParagraphRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebParagraphRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.ParagraphRun; } }
	}
	public class WebSectionRun : WebSinglePositionRun {
		public WebSectionRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebSectionRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.SectionRun; } }
	}
	public class WebCustomRun : WebSinglePositionRun {
		public WebCustomRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebCustomRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.CustomRun; } }
	}
	public class WebDataContainerRun : WebSinglePositionRun {
		public WebDataContainerRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebDataContainerRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.DataContainerRun; } }
	}
	public class WebEndNoteRun : WebSinglePositionRun {
		public WebEndNoteRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebEndNoteRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.EndNoteRun; } }
	}
	public class WebFieldCodeEndRun : WebSinglePositionRun {
		public WebFieldCodeEndRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebFieldCodeEndRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.FieldCodeEndRun; } }
	}
	public class WebFieldCodeStartRun : WebSinglePositionRun {
		public WebFieldCodeStartRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebFieldCodeStartRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.FieldCodeStartRun; } }
	}
	public class WebFieldResultEndRun : WebSinglePositionRun {
		public WebFieldResultEndRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebFieldResultEndRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.FieldResultEndRun; } }
	}
	public class WebFloatingObjectAnchorRun : WebSinglePositionRun {
		public WebFloatingObjectAnchorRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebFloatingObjectAnchorRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.FloatingObjectAnchorRun; } }
	}
	public class WebFootNoteRun : WebSinglePositionRun {
		public WebFootNoteRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebFootNoteRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.FootNoteRun; } }
	}
	public class WebInlineCustomObjectRun : WebSinglePositionRun {
		public WebInlineCustomObjectRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebInlineCustomObjectRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.InlineCustomObjectRun; } }
	}
	public class WebInlinePictureRun : WebSinglePositionRun {
		public WebInlinePictureRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebInlinePictureRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.InlinePictureRun; } }
		public Size OriginalSize { get; set; }
		public bool LockAspectRatio { get; set; }
		public float ScaleX { get; set; }
		public float ScaleY { get; set; }
		public int ID { get; set; }
		public override void ApplyFormatting(TextRunBase source) {
			base.ApplyFormatting(source);
			var src = source as InlinePictureRun;
			OriginalSize = src.OriginalSize;
			LockAspectRatio = src.LockAspectRatio;
			ScaleX = src.ScaleX;
			ScaleY = src.ScaleY;
			ID = src.Image.ImageCacheKey;
		}
		public override void FillHashtable(Hashtable result) {
			base.FillHashtable(result);
			result.Add("originalWidth", OriginalSize.Width);
			result.Add("originalHeight", OriginalSize.Height);
			result.Add("lockAspectRatio", LockAspectRatio);
			result.Add("scaleX", ScaleX);
			result.Add("scaleY", ScaleY);
			result.Add("id", ID);
		}
	}
	public class WebLayoutDependentTextRun : WebSinglePositionRun {
		public WebLayoutDependentTextRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebLayoutDependentTextRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.LayoutDependentTextRun; } }
	}
	public class WebLineNumberCommonRun : WebTextRunBase {
		public WebLineNumberCommonRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebLineNumberCommonRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.LineNumberCommonRun; } }
	}
	public class WebSeparatorTextRun : WebSinglePositionRun {
		public WebSeparatorTextRun(WebParagraph paragraph)
			: base(paragraph) {
		}
		public WebSeparatorTextRun(WebParagraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override WebRunType RunType { get { return WebRunType.SeparatorTextRun; } }
	}
	public abstract class WebBookmarkBase : IHashtableProvider {
		public DocumentLogPosition Start { get; private set; }
		public int Length { get; private set; }
		public bool CanExpand { get; set; }
		protected WebBookmarkBase(DocumentLogPosition start, int length) {
			Start = start;
			Length = length;
			CanExpand = true;
		}
		public void FillHashtable(Hashtable result) {
			ToHashtableCore(result);
		}
		protected virtual void ToHashtableCore(Hashtable hashtable) {
			hashtable.Add("start", Start);
			hashtable.Add("length", Length);
		}
	}
	public class WebBookmark : WebBookmarkBase {
		public string Name { get; set; }
		public WebBookmark(DocumentLogPosition start, int length, string name)
			: base(start, length) {
			Name = name;
		}
		public static WebBookmark FromModelBookmark(Bookmark bookmark) {
			return new WebBookmark(bookmark.Start, bookmark.Length, bookmark.Name);
		}
		protected override void ToHashtableCore(Hashtable hashtable) {
			base.ToHashtableCore(hashtable);
			hashtable.Add("name", Name);
		}
	}
	public class WebComment : WebBookmarkBase {
		public string Name { get; set; }
		public string Author { get; set; }
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public CommentContentType ContentType { get; set; }
		public WebComment(DocumentLogPosition start, int length, string name, string author, int id, DateTime date, CommentContentType contentType)
			: base(start, length) {
			Name = name;
			Author = author;
			Id = id;
			Date = date;
			ContentType = contentType;
		}
		public static WebComment FromModelComment(Comment comment) {
			return new WebComment(comment.Start, comment.Length, comment.Name, comment.Author, comment.Index, comment.Date, comment.Content);
		}
		protected override void ToHashtableCore(Hashtable hashtable) {
			base.ToHashtableCore(hashtable);
			hashtable.Add("name", Name);
			hashtable.Add("author", Author);
			hashtable.Add("id", Id);
			hashtable.Add("date", Date);
			hashtable.Add("contentType", ContentType);
		}
	}
	public class WebRangePermission : WebBookmarkBase {
		public string UserName { get; set; }
		public string Group { get; set; }
		public WebRangePermission(DocumentLogPosition start, int length)
			: base(start, length) { }
		public static WebRangePermission FromModelRangePermission(RangePermission rangePermission) {
			WebRangePermission result = new WebRangePermission(rangePermission.Start, rangePermission.Length);
			result.UserName = rangePermission.UserName;
			result.Group = rangePermission.Group;
			return result;
		}
		protected override void ToHashtableCore(Hashtable hashtable) {
			base.ToHashtableCore(hashtable);
			hashtable.Add("userName", UserName);
			hashtable.Add("group", Group);
		}
	}
	public class DocumentRange : WebBookmarkBase {
		public DocumentRange(DocumentLogPosition start, int length) : base(start, length) { }
	}
	public class WebField : IHashtableProvider {
		private int startPosition;
		private int separatorPosition;
		private int endPosition;
		private HyperlinkInfo hyperlinkInfo;
		public int StartPosition { get { return startPosition; } }
		public int SeparatorPosition { get { return separatorPosition; } }
		public int EndPosition { get { return endPosition; } }
		public HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } }
		public static WebField FromModelField(Field field) {
			HyperlinkInfoCollection collection = field.PieceTable.HyperlinkInfos;
			return new WebField(
				((IConvertToInt<DocumentLogPosition>)field.PieceTable.GetRunLogPosition(field.Code.Start)).ToInt(),
				((IConvertToInt<DocumentLogPosition>)field.PieceTable.GetRunLogPosition(field.Code.End)).ToInt(),
				((IConvertToInt<DocumentLogPosition>)field.PieceTable.GetRunLogPosition(field.Result.End)).ToInt() + 1,
				collection.IsHyperlink(field.Index) ? collection[field.Index] : null);
		}
		public WebField(int startPosition, int separatorPosition, int endPosition, HyperlinkInfo hyperlinkInfo = null) {
			this.startPosition = startPosition;
			this.separatorPosition = separatorPosition;
			this.endPosition = endPosition;
			this.hyperlinkInfo = hyperlinkInfo;
		}
		public void FillHashtable(Hashtable result) {
			result.Add("start", this.startPosition);
			result.Add("separator", this.separatorPosition);
			result.Add("end", this.endPosition);
			if(this.hyperlinkInfo != null) {
				result.Add("uri", this.hyperlinkInfo.NavigateUri);
				result.Add("anchor", this.hyperlinkInfo.Anchor);
				result.Add("tip", this.hyperlinkInfo.ToolTip);
				result.Add("visited", this.hyperlinkInfo.Visited);
			}
		}
	}
	public class WebTable : IHashtableProvider {
		protected WebTablePropertiesCache TablePropertiesCache { get; private set; }
		protected WebTableRowPropertiesCache TableRowPropertiesCache { get; private set; }
		protected WebTableCellPropertiesCache TableCellPropertiesCache { get; private set; }
		public WebTable(Table table, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowPropertiesCache, WebTableCellPropertiesCache tableCellPropertiesCache) {
			TablePropertiesCache = tablePropertiesCache;
			TableRowPropertiesCache = tableRowPropertiesCache;
			TableCellPropertiesCache = tableCellPropertiesCache;
			StyleIndex = table.StyleIndex;
			NestedLevel = table.NestedLevel;
			TablePropertiesIndex = tablePropertiesCache.AddItem(new WebTableProperties(table.TableProperties));
			Rows = table.Rows;
			ParentCell = table.ParentCell;
			Index = table.Index;
			PreferredWidth = table.PreferredWidth;
			LookTypes = table.TableLook;
		}
		public int StyleIndex { get; private set; }
		public int NestedLevel { get; private set; }
		public int TablePropertiesIndex { get; private set; }
		public TableRowCollection Rows { get; private set; }
		public TableCell ParentCell { get; private set; }
		public int Index { get; private set; }
		public PreferredWidth PreferredWidth { get; private set; }
		public TableLookTypes LookTypes { get; private set; }
		public void FillHashtable(Hashtable result) {
			result.Add("styleIndex", this.StyleIndex);
			result.Add("nestedLevel", this.NestedLevel);
			result.Add("tablePropertiesIndex", this.TablePropertiesIndex);
			result.Add("index", Index);
			result.Add("preferredWidth", WidthUnitExporter.ToHashtable(PreferredWidth));
			result.Add("lookTypes", (int)LookTypes);
			if(ParentCell != null) {
				result.Add("parentCell", new Hashtable() {
					{ "cellIndex", ParentCell.IndexInRow },
					{ "rowIndex", ParentCell.Row.IndexInTable },
					{ "tableIndex", ParentCell.Table.Index }
				});
			}
			List<Hashtable> rowsList = new List<Hashtable>();
			for(int rowIndex = 0; rowIndex < this.Rows.Count; rowIndex++) {
				rowsList.Add(new WebTableRow(Rows[rowIndex], TablePropertiesCache, TableRowPropertiesCache, TableCellPropertiesCache).ToHashtable());
			}
			result.Add("rows", rowsList);
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
	}
	public class WebTableRow : IHashtableProvider {
		protected WebTableCellPropertiesCache TableCellPropertiesCache { get; private set; }
		public int TableRowPropertiesIndex { get; private set; }
		public int GridBefore { get; private set; }
		public int GridAfter { get; private set; }
		public WidthUnit WidthBefore { get; private set; }
		public WidthUnit WidthAfter { get; private set; }
		public TableCellCollection Cells { get; private set; }
		public int TablePropertiesExceptionIndex { get; private set; }
		public HeightUnit Height { get; private set; }
		public WebTableRow(TableRow row, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowPropertiesCache, WebTableCellPropertiesCache tableCellPropertiesCache) {
			TableCellPropertiesCache = tableCellPropertiesCache;
			TableRowPropertiesIndex = tableRowPropertiesCache.AddItem(new WebTableRowProperties(row.Properties));
			GridBefore = row.Properties.GridBefore;
			GridAfter = row.Properties.GridAfter;
			WidthBefore = row.Properties.WidthBefore;
			WidthAfter = row.Properties.WidthAfter;
			Cells = row.Cells;
			TablePropertiesExceptionIndex = tablePropertiesCache.AddItem(new WebTableProperties(row.TablePropertiesException));
			Height = row.Properties.Height;
		}
		public void FillHashtable(Hashtable result) {
			result.Add("gridBefore", this.GridBefore);
			result.Add("gridAfter", this.GridAfter);
			result.Add("widthAfter", WidthUnitExporter.ToHashtable(this.WidthAfter));
			result.Add("widthBefore", WidthUnitExporter.ToHashtable(this.WidthBefore));
			result.Add("tableRowPropertiesIndex", this.TableRowPropertiesIndex);
			result.Add("tablePropertiesExceptionIndex", this.TablePropertiesExceptionIndex);
			result.Add("height", HeightUnitExporter.ToHashtable(this.Height));
			List<Hashtable> cellsList = new List<Hashtable>();
			for(int cellIndex = 0; cellIndex < this.Cells.Count; cellIndex++) {
				cellsList.Add(new WebTableCell(TableCellPropertiesCache, Cells[cellIndex]).ToHashtable());
			}
			result.Add("cells", cellsList);
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
	}
	public class WebTableCell : IHashtableProvider {
		public int StyleIndex { get; private set; }
		public int TableCellPropertiesIndex { get; private set; }
		public int ColumnSpan { get; private set; }
		public MergingState VerticalMergingState { get; private set; }
		public int StartParagraphPosition { get; private set; }
		public int EndParagraphPosition { get; private set; }
		public PreferredWidth PreferredWidth { get; private set; }
		public WebTableCell(WebTableCellPropertiesCache tablePropertiesCache, TableCell cell) {
			ParagraphCollection paragraphs = cell.Row.Table.PieceTable.Paragraphs;
			StyleIndex = cell.StyleIndex;
			TableCellPropertiesIndex = tablePropertiesCache.AddItem(new WebTableCellProperties(cell.Properties));
			ColumnSpan = cell.Properties.ColumnSpan;
			PreferredWidth = cell.Properties.PreferredWidth;
			VerticalMergingState = cell.Properties.VerticalMerging;
			StartParagraphPosition = ((IConvertToInt<DocumentLogPosition>)paragraphs[cell.StartParagraphIndex].GetLogPosition()).ToInt();
			EndParagraphPosition = ((IConvertToInt<DocumentLogPosition>)paragraphs[cell.EndParagraphIndex + 1].GetLogPosition()).ToInt();
		}
		public void FillHashtable(Hashtable result) {
			result.Add("styleIndex", StyleIndex);
			result.Add("tableCellPropertiesIndex", TableCellPropertiesIndex);
			result.Add("columnSpan", ColumnSpan);
			result.Add("preferredWidth", WidthUnitExporter.ToHashtable(PreferredWidth));
			result.Add("verticalMerging", (int)VerticalMergingState);
			result.Add("startParagraphPosition", StartParagraphPosition);
			result.Add("endParagraphPosition", EndParagraphPosition);
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
	}
}
