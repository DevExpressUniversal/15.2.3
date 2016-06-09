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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region DocumentLayoutPosition
	public class DocumentLayoutPosition : ICloneable<DocumentLayoutPosition>, ISupportsCopyFrom<DocumentLayoutPosition> {
		#region Fields
		readonly DocumentLayout documentLayout;
		DocumentLayoutDetailsLevel detailsLevel = DocumentLayoutDetailsLevel.None;
		Page page;
		DrawingBox drawingBox;
		CommentBox commentBox;
		ICellTextBox cellTextBox;
		CellPosition cellPosition;
		#endregion
		public DocumentLayoutPosition(DocumentLayout documentLayout, CellPosition cellPosition) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(cellPosition, "cellPosition");
			this.documentLayout = documentLayout;
			this.cellPosition = cellPosition;
		}
		#region Properties
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public DocumentModel DocumentModel { get { return DocumentLayout.DocumentModel; } }
		public DocumentLayoutDetailsLevel DetailsLevel { get { return detailsLevel; } set { detailsLevel = value; } }
		public Page Page { get { return page; } set { page = value; } }
		public DrawingBox PictureBox { get { return drawingBox; } set { drawingBox = value; } }
		public CommentBox CommentBox { get { return commentBox; } set { commentBox = value; } }
		public ICellTextBox CellTextBox { get { return cellTextBox; } set { cellTextBox = value; } }
		public CellPosition CellPosition { get { return cellPosition; } }
		#endregion
		protected internal void SetCellPosition(CellPosition position) {
			this.cellPosition = position;
		}
		public bool IsValid(DocumentLayoutDetailsLevel detailsLevel) {
			return detailsLevel <= DetailsLevel;
		}
		protected internal void Invalidate() {
			detailsLevel = DocumentLayoutDetailsLevel.None;
		}
		#region ICloneable<DocumentLayoutPosition> Members
		public DocumentLayoutPosition Clone() {
			DocumentLayoutPosition clone = new DocumentLayoutPosition(DocumentLayout, CellPosition);
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<DocumentLayoutPosition> Members
		public virtual void CopyFrom(DocumentLayoutPosition value) {
			this.detailsLevel = value.detailsLevel;
			this.page = value.page;
			this.drawingBox = value.drawingBox;
			this.cellTextBox = value.cellTextBox;
			this.cellPosition = value.cellPosition;
		}
		#endregion
	}
	#endregion
}
