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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CommentReferenceValidator
	public static class CommentReferenceValidator {
		public static CellPosition GetValidCommentReference(CellPosition position, MergedCellCollection mergedCells) {
			CellRange found = mergedCells.GetMergedCellRange(position.Column, position.Row);
			if (found != null)
				return found.TopLeft;
			return position;
		}
	}
	#endregion
	#region AddWorksheetCommentCommand
	public class AddWorksheetCommentCommand : SpreadsheetModelCommand {
		#region Fields
		readonly Comment comment;
		#endregion
		public AddWorksheetCommentCommand(Worksheet worksheet, Comment comment)
			: base(worksheet) {
			Guard.ArgumentNotNull(comment, "comment");
			this.comment = comment;
		}
		protected internal override void ExecuteCore() {
			CellPosition validPosition = CommentReferenceValidator.GetValidCommentReference(comment.Reference, Worksheet.MergedCells);
			Worksheet.Comments.CheckExistingComments(validPosition);
			if (!validPosition.EqualsPosition(comment.Reference))
				comment.SetReferenceCore(validPosition);
			Worksheet.Comments.Add(comment);
		}
	}
	#endregion
	#region InsertWorksheetCommentCommand
	public class InsertWorksheetCommentCommand : SpreadsheetModelCommand {
		Color color = DXColor.FromArgb(0xFF, 0xFF, 0xE1);
		CellPosition position;
		string authorName;
		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		public InsertWorksheetCommentCommand(Worksheet worksheet, CellPosition position, string authorName)
			: base(worksheet) {
			Guard.ArgumentIsNotNullOrEmpty(authorName, "authorName");
			this.position = CommentReferenceValidator.GetValidCommentReference(position, Worksheet.MergedCells);
			this.authorName = authorName;
		}
		protected internal override void ExecuteCore() {
			int authorId = DocumentModel.CommentAuthors.AddIfNotPresent(authorName);
			VmlShape shape = new VmlShape(Worksheet);
			shape.ClientData.SetReference(position);
			shape.IsHidden = true;
			shape.Filled = true;
			shape.Fillcolor = Color;
			shape.Fill.Color2 = Color;
			shape.Shadow.IsDefault = false;
			shape.Shadow.Color = DXColor.Black;
			shape.Shadow.Obscured = true;
			int shapeId = Worksheet.VmlDrawing.Shapes.RegisterShape(shape);
			Comment comment = new Comment(Worksheet, position, authorId, shapeId);
			Worksheet.Comments.CheckExistingComments(comment.Reference);
			Worksheet.Comments.Add(comment);
			Result = comment;
		}
	}
	#endregion
	#region DeleteWorksheetCommentCommand
	public class DeleteWorksheetCommentCommand : SpreadsheetModelCommand {
		int index;
		public DeleteWorksheetCommentCommand(Worksheet worksheet, int index)
			: base(worksheet) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		protected internal override void ExecuteCore() {
			Worksheet.Comments.RemoveAt(index);
		}
	}
	#endregion
	#region DeleteAllWorksheetCommentCommand
	public class DeleteAllWorksheetCommentCommand : SpreadsheetModelCommand {
		CommentCollection comments;
		public DeleteAllWorksheetCommentCommand(Worksheet worksheet, CommentCollection comments)
			: base(worksheet) {
			Guard.ArgumentNotNull(comments, "comments");
			this.comments = comments;
		}
		protected internal override void ExecuteCore() {
			comments.Clear();
		}
	}
	#endregion
	#region MoveCommentCommand
	public class MoveCommentCommand : SpreadsheetModelCommand {
		#region Fields
		readonly Comment comment;
		readonly float modelOffsetX;
		readonly float modelOffsetY;
		#endregion
		public MoveCommentCommand(Worksheet worksheet, int commentIndex, int offsetX, int offsetY)
			: base(worksheet) {
			Guard.ArgumentNonNegative(commentIndex, "commentIndex");
			comment = worksheet.Comments[commentIndex];
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			modelOffsetX = unitConverter.ToModelUnits((float)offsetX);
			modelOffsetY = unitConverter.ToModelUnits((float)offsetY);
		}
		protected internal override void ExecuteCore() {
			comment.Shape.ClientData.Anchor.Move(modelOffsetX, modelOffsetY);
		}
	}
	#endregion
	#region ResizeCommentCommand
	public class ResizeCommentCommand : SpreadsheetModelCommand {
		#region Fields
		readonly Comment comment;
		readonly float modelWidth;
		readonly float modelHeight;
		#endregion
		public ResizeCommentCommand(Worksheet worksheet, int commentIndex, int width, int height)
			: base(worksheet) {
			Guard.ArgumentNonNegative(commentIndex, "commentIndex");
			Guard.ArgumentNonNegative(width, "width");
			Guard.ArgumentNonNegative(height, "height");
			comment = worksheet.Comments[commentIndex];
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			modelWidth = unitConverter.ToModelUnits((float)width);
			modelHeight = unitConverter.ToModelUnits((float)height);
		}
		protected internal override void ExecuteCore() {
			comment.Shape.ClientData.Anchor.SetIndependentSize(modelWidth, modelHeight);
		}
	}
	#endregion
}
