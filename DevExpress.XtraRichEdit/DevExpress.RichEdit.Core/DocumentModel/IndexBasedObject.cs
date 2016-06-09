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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Model {
	public interface IObtainAffectedRangeListener {
		void NotifyObtainAffectedRange(ObtainAffectedRangeEventArgs args);
	}
	#region RichEditIndexBasedObject<T>
	public abstract class RichEditIndexBasedObject<T> : UndoableIndexBasedObject<T, DocumentModelChangeActions> where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf {
		protected RichEditIndexBasedObject(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public PieceTable PieceTable { get { return (PieceTable)DocumentModelPart; } }
		#region Events
		#region ObtainAffectedRange
		ObtainAffectedRangeEventHandler onObtainAffectedRange;
		public event ObtainAffectedRangeEventHandler ObtainAffectedRange { add { onObtainAffectedRange += value; } remove { onObtainAffectedRange -= value; } }
		protected internal virtual void RaiseObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			if (onObtainAffectedRange != null)
				onObtainAffectedRange(this, args);
		}
		#endregion
		#endregion
		protected override UniqueItemsCache<T> GetCache(IDocumentModel documentModel) {
			return GetCache((DocumentModel)documentModel);
		}
		protected internal abstract UniqueItemsCache<T> GetCache(DocumentModel documentModel);
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			ObtainAffectedRangeEventArgs args = new ObtainAffectedRangeEventArgs();
			IObtainAffectedRangeListener listener = GetObtainAffectedRangeListener();
			if (listener != null && IsDirectNotificationsEnabled)
				listener.NotifyObtainAffectedRange(args);
			else
				RaiseObtainAffectedRange(args);
			if (args.Start >= RunIndex.Zero)
				PieceTable.ApplyChangesCore(changeActions, args.Start, args.End);
		}
		protected internal virtual IObtainAffectedRangeListener GetObtainAffectedRangeListener() {
			return null;
		}
	}
	#endregion
	#region IndexBasedObject<TInfo, TOptions> (abstract class)
	public abstract class IndexBasedObjectB<TInfo, TOptions> : IndexBasedObject<TInfo, TOptions>
		where TInfo : ICloneable<TInfo>, ISupportsCopyFrom<TInfo>, ISupportsSizeOf
		where TOptions : ICloneable<TOptions>, ISupportsCopyFrom<TOptions>, ISupportsSizeOf {
		#region Fields
		readonly PieceTable pieceTable;
		#endregion
		protected IndexBasedObjectB(PieceTable pieceTable, IDocumentModel documentModel, int formattingInfoIndex, int formattingOptionsIndex)
			: base(documentModel, formattingInfoIndex, formattingOptionsIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
	}
	#endregion
	public interface IPieceTableHistoryItem {
		PieceTable PieceTable { get; }
	}
	#region RunPropertiesChangedHistoryItemBase (abstract class)
	public abstract class RunPropertiesChangedHistoryItemBase : IndexChangedHistoryItemCore<DocumentModelChangeActions>, IPieceTableHistoryItem {
		readonly RunIndex runIndex;
		protected RunPropertiesChangedHistoryItemBase(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable) {
			if (runIndex < new RunIndex(0))
				Exceptions.ThrowArgumentException("runIndex", runIndex);
			this.runIndex = runIndex;
		}
		public RunIndex RunIndex { get { return runIndex; } }
		public PieceTable PieceTable { get { return (PieceTable)DocumentModelPart; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return GetProperties(PieceTable.Runs[RunIndex]);
		}
		protected abstract IIndexBasedObject<DocumentModelChangeActions> GetProperties(TextRunBase textRunBase);
	}
	#endregion
	#region RunCharacterPropertiesChangedHistoryItem
	public class RunCharacterPropertiesChangedHistoryItem : RunPropertiesChangedHistoryItemBase {
		public RunCharacterPropertiesChangedHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected override IIndexBasedObject<DocumentModelChangeActions> GetProperties(TextRunBase textRunBase) {
			return textRunBase.CharacterProperties;
		}
	}
	#endregion
	#region RunInlinePicturePropertiesChangedHistoryItem
	public class RunInlinePicturePropertiesChangedHistoryItem : RunPropertiesChangedHistoryItemBase {
		public RunInlinePicturePropertiesChangedHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected override IIndexBasedObject<DocumentModelChangeActions> GetProperties(TextRunBase textRunBase) {
			return ((InlinePictureRun)textRunBase).PictureProperties;
		}
	}
	#endregion
	#region TableCellPropertiesChangedHistoryItem
	public class TableCellPropertiesChangedHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions>, IPieceTableHistoryItem {
		readonly ParagraphIndex firstCellParagraphIndex;
		readonly int nestedLevel;
		public PieceTable PieceTable { get { return (PieceTable)DocumentModelPart; } }
		public TableCellPropertiesChangedHistoryItem(PieceTable pieceTable, ParagraphIndex firstCellParagraphIndex, int nestedLevel)
			: base(pieceTable) {
			if (firstCellParagraphIndex < ParagraphIndex.Zero)
				Exceptions.ThrowArgumentException("firstCellParagraphIndex", firstCellParagraphIndex);
			this.firstCellParagraphIndex = firstCellParagraphIndex;
			this.nestedLevel = nestedLevel;
		}
		public ParagraphIndex FirstCellParagraphIndex { get { return firstCellParagraphIndex; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			TableCell cell = PieceTable.Paragraphs[FirstCellParagraphIndex].GetCell();
			while (cell.Table.NestedLevel != nestedLevel)
				cell = cell.Table.ParentCell;
			return cell.Properties;
		}
	}
	#endregion
	#region TableCellInnerPropertiesChangedHistoryItem
	public class TableCellInnerPropertiesChangedHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions>, IPieceTableHistoryItem {
		readonly ParagraphIndex firstCellParagraphIndex;
		readonly int nestedLevel;
		readonly Properties property;
		public PieceTable PieceTable { get { return (PieceTable)DocumentModelPart; } }
		public TableCellInnerPropertiesChangedHistoryItem(PieceTable pieceTable, ParagraphIndex firstCellParagraphIndex, int nestedLevel, Properties property)
			: base(pieceTable) {
			if (firstCellParagraphIndex < ParagraphIndex.Zero)
				Exceptions.ThrowArgumentException("firstCellParagraphIndex", firstCellParagraphIndex);
			this.firstCellParagraphIndex = firstCellParagraphIndex;
			this.nestedLevel = nestedLevel;
			this.property = property;
		}
		public ParagraphIndex FirstCellParagraphIndex { get { return firstCellParagraphIndex; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			TableCell cell = PieceTable.Paragraphs[FirstCellParagraphIndex].GetCell();
			while (cell.Table.NestedLevel != nestedLevel)
				cell = cell.Table.ParentCell;
			TableCellProperties properties = cell.Properties;
			switch (property) {
				case Properties.PreferredWidth:
					return properties.PreferredWidth;
				case Properties.CellGeneralSettings:
					return properties.GeneralSettings;
				case Properties.TopMargin:
					return properties.CellMargins.Top;
				case Properties.BottomMargin:
					return properties.CellMargins.Bottom;
				case Properties.LeftMargin:
					return properties.CellMargins.Left;
				case Properties.RightMargin:
					return properties.CellMargins.Right;
				case Properties.BottomBorder:
					return properties.Borders.BottomBorder;
				case Properties.InsideHorizontalBorder:
					return properties.Borders.InsideHorizontalBorder;
				case Properties.InsideVerticalBorder:
					return properties.Borders.InsideVerticalBorder;
				case Properties.LeftBorder:
					return properties.Borders.LeftBorder;
				case Properties.RightBorder:
					return properties.Borders.RightBorder;
				case Properties.TopBorder:
					return properties.Borders.TopBorder;
				case Properties.TopLeftDiagonalBorder:
					return properties.Borders.TopLeftDiagonalBorder;
				case Properties.TopRightDiagonalBorder:
					return properties.Borders.TopRightDiagonalBorder;
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
	}
	#endregion
	#region ParagraphParagraphPropertiesChangedHistoryItem
	public class ParagraphParagraphPropertiesChangedHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions>, IPieceTableHistoryItem {
		readonly ParagraphIndex paragraphIndex;
		public PieceTable PieceTable { get { return (PieceTable)DocumentModelPart; } }
		public ParagraphParagraphPropertiesChangedHistoryItem(PieceTable pieceTable, ParagraphIndex paragraphIndex)
			: base(pieceTable) {
			if (paragraphIndex < new ParagraphIndex(0))
				Exceptions.ThrowArgumentException("paragraphIndex", paragraphIndex);
			this.paragraphIndex = paragraphIndex;
		}
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return PieceTable.Paragraphs[ParagraphIndex].ParagraphProperties;
		}
	}
	#endregion
}
