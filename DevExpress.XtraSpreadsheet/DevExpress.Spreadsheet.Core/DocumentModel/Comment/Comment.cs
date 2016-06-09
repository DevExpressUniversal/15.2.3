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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Comment
	public class Comment {
		#region Fields
		public static int MinSizeInPixels = 5; 
		public static Font DefaultFont = CreateDefaultCommentFont();
		public static Color DefaultForeColor = Color.Black;
		readonly CommentRunCollection runs;
		readonly int shapeId;
		readonly Worksheet worksheet;
		CellPosition reference;
		int authorId;
		#endregion
		static Font CreateDefaultCommentFont() {
			return new Font("Tahoma", 9);
		}
		public Comment(Worksheet worksheet, int shapeId) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.worksheet = worksheet;
			this.shapeId = shapeId;
			this.runs = new CommentRunCollection(this);
		}
		public Comment(Worksheet worksheet, CellPosition reference, int authorId, int shapeId)
			: this(worksheet, shapeId) {
			this.reference = reference;
			this.authorId = authorId;
		}
		#region Properties
		public DocumentModel Workbook { get { return worksheet.Workbook; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public CommentRunCollection Runs { get { return runs; } }
		public float Width { get { return Shape.ClientData.Anchor.Width; } set { Shape.ClientData.Anchor.Width = value; } } 
		public float Height { get { return Shape.ClientData.Anchor.Height; } set { Shape.ClientData.Anchor.Height = value; } } 
		public bool Visible { get { return !Shape.IsHidden; } set { Shape.IsHidden = !value; } }
		#region Reference
		public CellPosition Reference {
			get { return reference; }
			set {
				if (reference.Equals(value))
					return;
				Workbook.BeginUpdate();
				try {
					DocumentHistory history = Workbook.History;
					CommentChangeReferenceHistoryItem historyItem = new CommentChangeReferenceHistoryItem(this, reference, value);
					history.Add(historyItem);
					historyItem.Execute();
				}
				finally {
					Workbook.EndUpdate();
				}
			}
		}
		#endregion
		#region AuthorId
		public int AuthorId {
			get { return authorId; }
			set {
				if (value == authorId)
					return;
				CommentChangeAuthorIdHistoryItem historyItem = new CommentChangeAuthorIdHistoryItem(this, authorId, value);
				Workbook.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		#endregion
		public VmlShape Shape { get { return worksheet.VmlDrawing.Shapes[shapeId]; } }
		#endregion
		public void SetAuthorIdCore(int value) {
			authorId = value;
		}
		public void SetReferenceCore(CellPosition value) {
			reference = value;
			if (Shape != null)
				Shape.ClientData.SetReference(value);
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			ParsedThingRef referenceExpression = new ParsedThingRef(reference);
			WorkbookDataContext dataContext = notificationContext.Range.Worksheet.Workbook.DataContext;
			ParsedThingRef processedExpression = notificationContext.Visitor.ProcessRef(referenceExpression) as ParsedThingRef;
			if (!object.Equals(processedExpression.Position, reference))
				Reference = processedExpression.Position;
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			ParsedThingRef referenceExpression = new ParsedThingRef(reference);
			ParsedThingRef processedExpression = notificationContext.Visitor.ProcessRef(referenceExpression) as ParsedThingRef;
			if (processedExpression != null && !object.Equals(processedExpression.Position, reference))
				Reference = processedExpression.Position;
		}
		public void CopyFrom(Comment source) {
			Runs.CopyFrom(source.Runs);
			SetReferenceCore(source.Reference);
			authorId = source.AuthorId;
		}
		public string GetPlainText() {
			StringBuilder result = new StringBuilder();
			int count = runs.Count;
			for (int i = 0; i < count; i++)
				result.Append(runs[i].Text);
			return result.ToString();
		}
		public string GetNormalizedLineBreaksPlainText() {
			string result = GetPlainText();
			return Regex.Replace(result, "(?<!\r)\n", Environment.NewLine);
		}
		public void SetPlainText(string text) {
			Workbook.BeginUpdate();
			try {
				Runs.Clear();
				CommentRun run = new CommentRun(worksheet, text);
				run.FontInfo.Name = DefaultFont.Name;
				run.FontInfo.Size = DefaultFont.Size;
				Runs.Add(run);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
	}
	#endregion
}
