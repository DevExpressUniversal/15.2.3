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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region PageArea
	public class PageArea : NoPositionBox {
		#region Fields
		readonly ColumnCollection columns;
		readonly LineNumberBoxCollection lineNumbers;
		readonly ContentTypeBase pieceTable;
		readonly Section section;
		#endregion
		public PageArea(ContentTypeBase pieceTable, Section section) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(section, "section");
			this.pieceTable = pieceTable;
			this.section = section;
			this.columns = new ColumnCollection();
			this.lineNumbers = new LineNumberBoxCollection();
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.PageArea; } }
		protected internal override HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactPageArea; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public ColumnCollection Columns { get { return columns; } }
		public LineNumberBoxCollection LineNumbers { get { return lineNumbers; } }
		public bool IsEmpty {
			get {
				return Columns.Count <= 0 || Columns[0].IsEmpty;
			}
		}
		public PieceTable PieceTable { get { return pieceTable.PieceTable; } }
		public Section Section { get { return section; } }
		#endregion
		public override FormatterPosition GetFirstFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Columns.First.GetFirstFormatterPosition();
		}
		public override FormatterPosition GetLastFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Columns.Last.GetLastFormatterPosition();
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else
				return Columns.First.GetFirstPosition(pieceTable);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else
				return Columns.Last.GetLastPosition(pieceTable);
		}
		public override Box CreateBox() {
			Exceptions.ThrowInternalException();
			return new PageArea(PieceTable.ContentType, Section);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportPageArea(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreatePageAreaHitTestManager(this);
		}
		public override void MoveVertically(int deltaY) {
			base.MoveVertically(deltaY);
			Columns.MoveVertically(deltaY);
			LineNumbers.MoveVertically(deltaY);
		}
		public override string GetText(PieceTable table) {
			Exceptions.ThrowInternalException();
			return String.Empty;
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return null;
		}
#if DEBUG
		public override string ToString() {
			return String.Format("PageArea. Bounds: {0}", Bounds);
		}
#endif
	}
	#endregion
	#region PageAreaCollection
	public class PageAreaCollection : BoxCollectionBase<PageArea> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, PageArea item) {
			calculator.Result.PageArea = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.PageArea);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.PageArea = null;
		}
	}
	#endregion
	#region HeaderFooterPageAreaBase
	public abstract class HeaderFooterPageAreaBase : PageArea {
		Rectangle contentBounds;
		protected HeaderFooterPageAreaBase(SectionHeaderFooterBase headerFooter, Section section)
			: base(headerFooter, section) {
		}
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
	}
	#endregion
	#region HeaderPageArea
	public class HeaderPageArea : HeaderFooterPageAreaBase {
		public HeaderPageArea(SectionHeader header, Section section)
			: base(header, section) {
		}
		public SectionHeader Header { get { return (SectionHeader)PieceTable.ContentType; } }
	}
	#endregion
	#region FooterPageArea
	public class FooterPageArea : HeaderFooterPageAreaBase {
		public FooterPageArea(SectionFooter footer, Section section)
			: base(footer, section) {
		}
		public SectionFooter Footer { get { return (SectionFooter)PieceTable.ContentType; } }
	}
	#endregion
	public class FootNotePageArea : PageArea {
		readonly RunIndex referenceRunIndex;
		public FootNotePageArea(FootNote footNote, Section section, RunIndex referenceRunIndex)
			: base(footNote, section) {
			this.referenceRunIndex = referenceRunIndex;
		}
		public FootNote Note { get { return (FootNote)PieceTable.ContentType; } }
		public RunIndex ReferenceRunIndex { get { return referenceRunIndex; } }
	}
}
