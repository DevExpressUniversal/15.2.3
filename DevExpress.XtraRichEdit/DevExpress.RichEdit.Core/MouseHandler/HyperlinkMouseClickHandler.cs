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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	#region BeginMouseDragHyperlinkClickHandleHelperState
	public class BeginMouseDragHyperlinkClickHandleHelperState : RichEditBeginMouseDragHelperState {
		public BeginMouseDragHyperlinkClickHandleHelperState(RichEditMouseHandler mouseHandler, MouseHandlerState dragState, Point point)
			: base(mouseHandler, dragState, point) {
		}
		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			HandleMouseUp(e);
		}
		void HandleMouseUp(MouseEventArgs e) {
			IRichEditControl control = ((RichEditMouseHandler)MouseHandler).Control;
			ValidateCursorPosition(control);	  
			HyperlinkMouseClickHandler handler = new HyperlinkMouseClickHandler(control);
			handler.HandleMouseUp(e);
		}
		void ValidateCursorPosition(IRichEditControl control) {
			DocumentModel documentModel = control.InnerControl.DocumentModel;
			Selection selection = documentModel.Selection;
			if (selection.Length != 0)
				return;
			DocumentLogPosition cursorPos = selection.Start;
			PieceTable activePieceTable = documentModel.ActivePieceTable;
			if (cursorPos <= activePieceTable.DocumentStartLogPosition)
				return;
			RunIndex runIndex;
			ParagraphIndex paragraphIndex = activePieceTable.FindParagraphIndex(cursorPos);
			DocumentLogPosition runStartPosition = activePieceTable.FindRunStartLogPosition(activePieceTable.Paragraphs[paragraphIndex], cursorPos, out runIndex);
			if (runStartPosition != cursorPos || runIndex == RunIndex.Zero)
				return;
			TextRunBase run = activePieceTable.Runs[runIndex - 1];
			if (!(run is FieldCodeRunBase))
				return;
			IVisibleTextFilter textFilter = activePieceTable.VisibleTextFilter;
			cursorPos = textFilter.GetPrevVisibleLogPosition(cursorPos + 1, true);
			if (cursorPos < activePieceTable.DocumentStartLogPosition || cursorPos == selection.Start)
				return;
			control.BeginUpdate();
			try {
				selection.Start = cursorPos;
				selection.End = cursorPos;
			}
			finally {
				control.EndUpdate();
			}
		}
	}
	#endregion
	#region HyperlinkMouseClickHandler
	public class HyperlinkMouseClickHandler {
		readonly IRichEditControl control;
		public HyperlinkMouseClickHandler(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public IRichEditControl Control { get { return control; } }
		public virtual void HandleMouseUp(MouseEventArgs e) {
			RichEditHitTestResult hitTestResult = Control.InnerControl.ActiveView.CalculateHitTest(new Point(e.X, e.Y), DocumentLayoutDetailsLevel.Box);
			if (hitTestResult == null || !hitTestResult.IsValid(DocumentLayoutDetailsLevel.Box))
				return;
			if (e.Button == MouseButtons.Left)
				TryProcessHyperlinkClick(hitTestResult);
		}
		protected internal virtual bool TryProcessHyperlinkClick(RichEditHitTestResult hitTestResult) {
			Field field = Control.InnerControl.ActiveView.GetHyperlinkField(hitTestResult);
			if (field == null || field.IsCodeView)
				return false;
			return Control.InnerControl.OnHyperlinkClick(field, true);
		}
	}
	#endregion
}
