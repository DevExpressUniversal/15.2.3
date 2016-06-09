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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
namespace DevExpress.XtraRichEdit.API.RichTextBox {
	#region ITextBoxBase
	public interface ITextBoxBase {
		void AppendText(string text);
		bool CanUndo { get; }
		void Undo();
		void ClearUndo();
		void Clear();
		void Copy();
		void Cut();
		void Paste();
		void DeselectAll();
		 char GetCharFromPosition(Point pt);
		 int GetCharIndexFromPosition(Point pt);
		int GetFirstCharIndexFromLine(int lineNumber);
		int GetFirstCharIndexOfCurrentLine();
		 int GetLineFromCharIndex(int index);
		 Point GetPositionFromCharIndex(int index);
		void ScrollToCaret();
		void Select(int start, int length);
		void SelectAll();
		 string ToString();
		bool AcceptsTab { get; set; }
		 bool AutoSize { get; set; }
		 Color BackColor { get; set; }
		 Image BackgroundImage { get; set; }
		 ImageLayout BackgroundImageLayout { get; set; }
		BorderStyle BorderStyle { get; set; }
		 Color ForeColor { get; set; }
		bool HideSelection { get; set; }
		string[] Lines { get; set; }
		 int MaxLength { get; set; }
		bool Modified { get; set; }
		 bool Multiline { get; set; }
		Padding Padding { get; set; }
		int PreferredHeight { get; }
		bool ReadOnly { get; set; }
		 string SelectedText { get; set; }
		 int SelectionLength { get; set; }
		int SelectionStart { get; set; }
		 bool ShortcutsEnabled { get; set; }
		 string Text { get; set; }
		 int TextLength { get; }
		bool WordWrap { get; set; }
		event EventHandler AcceptsTabChanged;
		event EventHandler AutoSizeChanged;
		event EventHandler BackgroundImageChanged;
		event EventHandler BackgroundImageLayoutChanged;
		event EventHandler BorderStyleChanged;
		event EventHandler Click;
		event EventHandler HideSelectionChanged;
		event EventHandler ModifiedChanged;
		event MouseEventHandler MouseClick;
		event EventHandler MultilineChanged;
		event EventHandler PaddingChanged;
		event PaintEventHandler Paint;
		event EventHandler ReadOnlyChanged;
	}
	#endregion
}
