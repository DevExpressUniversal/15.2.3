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

using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Keyboard;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InnerCommentInplaceEditor
	public class InnerCommentInplaceEditor {
		#region Fields
		readonly InnerSpreadsheetControl innerControl;
		CommentKeyboardHandler keyboardHandler;
		ICommentInplaceEditor editor;
		CellPosition commentReference;
		bool isActive;
		bool needHideComment;
		#endregion
		public InnerCommentInplaceEditor(InnerSpreadsheetControl innerControl) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
			InitializeKeyboardHandler();
		}
		#region Properties
		public InnerSpreadsheetControl InnerControl { get { return innerControl; } }
		public bool IsActive { get { return isActive; } }
		#endregion
		#region KeyboardHandler
		void InitializeKeyboardHandler() {
			this.keyboardHandler = new CommentKeyboardHandler();
			AppendKeyboardShortcuts(keyboardHandler, new SpreadsheetKeyHashProvider());
		}
		void AppendKeyboardShortcuts(CommentKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, SpreadsheetCommandId.CommentCloseEditor);
		}
		#endregion
		public void Activate(CellPosition commentReference, bool isHiddenComment) {
			CommentBox box = FindCommentBox(commentReference);
			if (box == null)
				return;
			Activate(box);
			needHideComment = isHiddenComment;
		}
		CommentBox FindCommentBox(CellPosition reference) {
			IList<Page> pages = innerControl.InnerDocumentLayout.Pages;
			foreach (Page page in pages) {
				CommentBox box = FindCommentBoxCore(page.CommentBoxes, reference);
				if (box != null)
					return box;
			}
			return null;
		}
		CommentBox FindCommentBoxCore(List<CommentBox> boxes, CellPosition reference) {
			foreach (CommentBox box in boxes) {
				if (box.Reference.EqualsPosition(reference))
					return box;
			}
			return null;
		}
		public void Activate(CommentBox box) {
			this.commentReference = box.Reference;
			Activate(box.Bounds, box.FillColor, box.GetNormalizedPlainText());
		}
		void Activate(Rectangle bounds, Color backColor, string text) {
			InnerControl.SetNewKeyboardHandler(keyboardHandler);
			this.editor = CreateEditor();
			this.editor.SetBounds(bounds);
			this.editor.SetBackColor(backColor);
			this.editor.Text = text;
			this.editor.Activate();
			this.editor.SetFocus();
			this.editor.SetSelection();
			this.isActive = true;
		}
		ICommentInplaceEditor CreateEditor() {
			return InnerControl.CreateCommentInplaceEditor();
		}
		public void UpdateBounds() {
			if (!IsActive)
				return;
			CommentBox box = FindCommentBox(commentReference);
			Rectangle bounds = box != null ? box.Bounds : Rectangle.Empty;
			this.editor.SetBounds(bounds);
		}
		public void Commit() {
			Worksheet activeSheet = InnerControl.DocumentModel.ActiveSheet;
			int index = activeSheet.Selection.SelectedCommentIndex;
			Comment comment = activeSheet.Comments[index];
			string text = this.editor.Text;
			if (string.Compare(text, comment.GetNormalizedLineBreaksPlainText()) != 0)
				comment.SetPlainText(text);
			if (needHideComment)
				comment.Visible = false;
		}
		public void Deactivate() {
			InnerControl.RestoreKeyboardHandler();
			this.editor.Deactivate();
			this.isActive = false;
			this.needHideComment = false;
		}
	}
	#endregion
}
