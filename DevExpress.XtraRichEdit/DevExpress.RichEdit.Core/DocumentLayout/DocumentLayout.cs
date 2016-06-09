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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region RichEditHitTestRequest
	public class RichEditHitTestRequest : ICloneable<RichEditHitTestRequest>, ISupportsCopyFrom<RichEditHitTestRequest> {
		#region Fields
		readonly PieceTable pieceTable;
		Point physicalPoint;
		Point logicalPoint;
		DocumentLayoutDetailsLevel detailsLevel;
		HitTestAccuracy accuracy;
		bool searchAnyPieceTable;
		bool ignoreInvalidAreas;
		#endregion
		public RichEditHitTestRequest(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public Point PhysicalPoint { get { return physicalPoint; } set { physicalPoint = value; } }
		public Point LogicalPoint { get { return logicalPoint; } set { logicalPoint = value; } }
		public DocumentLayoutDetailsLevel DetailsLevel { get { return detailsLevel; } set { detailsLevel = value; } }
		public HitTestAccuracy Accuracy { get { return accuracy; } set { accuracy = value; } }
		public bool SearchAnyPieceTable { get { return searchAnyPieceTable; } set { searchAnyPieceTable = value; } }
		public bool IgnoreInvalidAreas { get { return ignoreInvalidAreas; } set { ignoreInvalidAreas = value; } }
		#endregion
		#region ICloneable<RichEditHitTestRequest> Members
		public RichEditHitTestRequest Clone() {
			RichEditHitTestRequest clone = new RichEditHitTestRequest(PieceTable);
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<RichEditHitTestRequest> Members
		public void CopyFrom(RichEditHitTestRequest value) {
			this.PhysicalPoint = value.PhysicalPoint;
			this.LogicalPoint = value.LogicalPoint;
			this.DetailsLevel = value.DetailsLevel;
			this.Accuracy = value.Accuracy;
			this.SearchAnyPieceTable = value.SearchAnyPieceTable;
		}
		#endregion
	}
	#endregion
	#region RichEditHitTestResult
	public class RichEditHitTestResult : DocumentLayoutPosition, ISupportsCopyFrom<RichEditHitTestResult> {
		#region Fields
		HitTestAccuracy accuracy;
		Point logicalPoint;
		Point physicalPoint;
		FloatingObjectBox floatingObjectBox;
		CommentViewInfo commentViewInfo;
		CommentLocationType commentLocation;
		#endregion
		public RichEditHitTestResult(DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, DocumentLogPosition.Zero) {
		}
		#region Properties
		public HitTestAccuracy Accuracy { get { return accuracy; } set { accuracy = value; } }
		public Point LogicalPoint { get { return logicalPoint; } set { logicalPoint = value; } }
		public Point PhysicalPoint { get { return physicalPoint; } set { physicalPoint = value; } }
		public FloatingObjectBox FloatingObjectBox { get { return floatingObjectBox; } set { floatingObjectBox = value; } }
		public CommentViewInfo CommentViewInfo { get { return commentViewInfo; } set { commentViewInfo = value; } }
		public CommentLocationType CommentLocation { get { return commentLocation; } set { commentLocation = value; } }
		#endregion
		#region ISupportsCopyFrom<RichEditHitTestResult> Members
		public void CopyFrom(RichEditHitTestResult value) {
			base.CopyFrom(value);
			this.Accuracy = value.Accuracy;
			this.LogicalPoint = value.LogicalPoint;
			this.PhysicalPoint = value.PhysicalPoint;
		}
		#endregion
	}
	#endregion
	#region CommentLocationType
	public enum CommentLocationType { 
		None,
		CommentContent,
		CommentMoreButton
	}
	#endregion
	#region DocumentLayout
	public class DocumentLayout {
		#region Fields
		readonly DocumentModel documentModel;
		readonly IBoxMeasurerProvider measurerProvider;
		readonly PageCollection pages;
		readonly Counters counters;
		int firstVisiblePageIndex = -1;
		int lastVisiblePageIndex = -1;
		#endregion
		public DocumentLayout(DocumentModel documentModel, IBoxMeasurerProvider measurerProvider) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(measurerProvider, "measurerProvider");
			this.documentModel = documentModel;
			this.measurerProvider = measurerProvider;
			this.pages = new PageCollection();
			this.counters = new Counters();
			counters.RegisterCounter(FootNote.FootNoteCounterId);
			counters.RegisterCounter(EndNote.EndNoteCounterId);
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return DocumentModel.LayoutUnitConverter; } }
		public IBoxMeasurerProvider MeasurerProvider { get { return measurerProvider; } }
		public BoxMeasurer Measurer { get { return measurerProvider.Measurer; } }
		public PageCollection Pages { get { return pages; } }
		public Counters Counters { get { return counters; } }
		public int FirstVisiblePageIndex { get { return firstVisiblePageIndex; } set { firstVisiblePageIndex = value; } }
		public int LastVisiblePageIndex { get { return lastVisiblePageIndex; } set { lastVisiblePageIndex = value; } }
		#endregion
		#region Events
		#region BeforeCreateDetailRow
		EventHandler onBeforeCreateDetailRow;
		public event EventHandler BeforeCreateDetailRow { add { onBeforeCreateDetailRow += value; } remove { onBeforeCreateDetailRow -= value; } }
		protected internal virtual void RaiseBeforeCreateDetailRow() {
			if (onBeforeCreateDetailRow != null)
				onBeforeCreateDetailRow(this, EventArgs.Empty);
		}
		#endregion
		#region AfterCreateDetailRow
		EventHandler onAfterCreateDetailRow;
		public event EventHandler AfterCreateDetailRow { add { onAfterCreateDetailRow += value; } remove { onAfterCreateDetailRow -= value; } }
		protected internal virtual void RaiseAfterCreateDetailRow() {
			if (onAfterCreateDetailRow != null)
				onAfterCreateDetailRow(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public DocumentLayoutPosition CreateLayoutPosition(PieceTable pieceTable, DocumentLogPosition logPosition, int preferredPageIndex) {
			if (pieceTable.IsHeaderFooter)
				return new HeaderFooterDocumentLayoutPosition(this, pieceTable, logPosition, preferredPageIndex);
			else if (pieceTable.IsTextBox)
				return new TextBoxDocumentLayoutPosition(this, (TextBoxContentType)pieceTable.ContentType, logPosition, preferredPageIndex);
			else if (pieceTable.IsComment)
				return new CommentDocumentLayoutPosition(this, (CommentContentType)pieceTable.ContentType, logPosition, preferredPageIndex);
			else
				return new DocumentLayoutPosition(this, pieceTable, logPosition);
		}
		protected internal virtual DetailRow CreateDetailRow(Row row) {
			RaiseBeforeCreateDetailRow();
			try {
				return CreateDetailRowCore(row);
			}
			finally {
				RaiseAfterCreateDetailRow();
			}
		}
		protected internal virtual DetailRow CreateDetailRowCore(Row row) {
			DetailRow detailRow = new DetailRow();
			CharacterBoxCollection chars = detailRow.Characters;
			CharacterBoxLevelDocumentLayoutExporter exporter = new CharacterBoxLevelDocumentLayoutExporter(documentModel, chars, Measurer);
			exporter.ExportRow(row);
			return detailRow;
		}
		protected internal virtual DetailRow CreateDetailRowForBox(Row row, Box box, bool suppressSuspendFormatting) {
			if (!suppressSuspendFormatting)
				RaiseBeforeCreateDetailRow();
			try {
				return CreateDetailRowForBoxCore(row, box);
			}
			finally {
				if (!suppressSuspendFormatting)
					RaiseAfterCreateDetailRow();
			}
		}
		protected internal virtual DetailRow CreateDetailRowForBoxCore(Row row, Box box) {
			DetailRow detailRow = new DetailRow();
			CharacterBoxCollection chars = detailRow.Characters;
			CharacterBoxLevelDocumentLayoutExporter exporter = new CharacterBoxLevelDocumentLayoutExporter(documentModel, chars, Measurer);
			exporter.ExportRowBox(row, box);
			return detailRow;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit { 
	#region DocumentLayoutUnit
	[System.Runtime.InteropServices.ComVisible(true)]
	public enum DocumentLayoutUnit {
		Document = DevExpress.Office.DocumentLayoutUnit.Document,
		Twip = DevExpress.Office.DocumentLayoutUnit.Twip,
		Pixel = DevExpress.Office.DocumentLayoutUnit.Pixel
	}
	#endregion
}
