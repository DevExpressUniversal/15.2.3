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
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region HitTestCalculator
	public class HitTestCalculator {
		#region Fields
		readonly SpreadsheetHitTestRequest request;
		readonly SpreadsheetHitTestResult result;
		#endregion
		public HitTestCalculator(SpreadsheetHitTestRequest request, SpreadsheetHitTestResult result) {
			Guard.ArgumentNotNull(request, "request");
			Guard.ArgumentNotNull(result, "result");
			this.request = request;
			this.result = result;
			result.PhysicalPoint = request.PhysicalPoint;
			result.LogicalPoint = request.LogicalPoint;
		}
		#region Properties
		public SpreadsheetHitTestRequest Request { get { return request; } }
		public SpreadsheetHitTestResult Result { get { return result; } }
		#endregion
		protected internal virtual void CalculateHitTest(Page page) {
			result.PictureBox = GetDrawingBox(page);
			result.CommentBox = GetCommentBox(page);
			if (request.DetailsLevel == DocumentLayoutDetailsLevel.Cell)
				CalculateCellHitTest(page);
		}
		protected internal virtual void CalculateHitTest(HeaderTextBox headerBox) {
			if(request.DetailsLevel == DocumentLayoutDetailsLevel.Header)
				CalculateHeaderHitTest(headerBox);
		}
		protected internal virtual void CalculateHitTest(OutlineLevelBox groupBox) {
			if (request.DetailsLevel == DocumentLayoutDetailsLevel.GroupArea)
				CalculateGroupHitTest(groupBox);
		}
		DrawingBox GetDrawingBox(Page page) {
			List<DrawingBox> boxes = page.DrawingBoxes;
			for (int i = boxes.Count - 1; i >= 0; i--) {
				DrawingBox currentBox = boxes[i];
				Point transformPoint = currentBox.TransformPointBackward(Request.LogicalPoint);
				if (currentBox.Bounds.Contains(transformPoint))
					return currentBox;
			}
			return null;
		}
		CommentBox GetCommentBox(Page page) {
			List<CommentBox> boxes = page.CommentBoxes;
			for (int i = boxes.Count - 1; i >= 0; i--) {
				CommentBox currentBox = boxes[i];
				if (!currentBox.IsHidden && currentBox.Bounds.Contains(Request.LogicalPoint))
					return currentBox;
			}
			return null;
		}
		void CalculateCellHitTest(Page page) {
			Point logicalPoint = Request.LogicalPoint;
			int columnIndex = page.GridColumns.LookupItemModelIndexByPosition(logicalPoint.X);
			int rowIndex = page.GridRows.LookupItemModelIndexByPosition(logicalPoint.Y);
			CellPosition position = new CellPosition(columnIndex, rowIndex);
			result.SetCellPosition(position);
			if(rowIndex >= 0 && columnIndex >= 0) {
				result.DetailsLevel = DocumentLayoutDetailsLevel.Cell;
			}
		}
		void CalculateHeaderHitTest(HeaderTextBox headerBox) {
			result.HeaderBox = headerBox;
			result.DetailsLevel = DocumentLayoutDetailsLevel.Header;
		}
		void CalculateGroupHitTest(OutlineLevelBox groupBox) {
			result.GroupBox = groupBox;
			result.DetailsLevel = DocumentLayoutDetailsLevel.GroupArea;
		}
	}
	#endregion
}
