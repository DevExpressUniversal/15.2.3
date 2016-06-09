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
	#region IRichTextBox
	public interface IRichTextBox : ITextBoxBase {
		bool CanPaste(DataFormats.Format clipFormat);
		void Paste(DataFormats.Format clipFormat);
		void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);
		int Find(char[] characterSet);
		int Find(string str);
		int Find(string str, RichTextBoxFinds options);
		int Find(char[] characterSet, int start);
		int Find(string str, int start, RichTextBoxFinds options);
		int Find(char[] characterSet, int start, int end);
		int Find(string str, int start, int end, RichTextBoxFinds options);
		void LoadFile(string path);
		void LoadFile(Stream data, RichTextBoxStreamType fileType);
		void LoadFile(string path, RichTextBoxStreamType fileType);
		void SaveFile(string path);
		void SaveFile(Stream data, RichTextBoxStreamType fileType);
		void SaveFile(string path, RichTextBoxStreamType fileType);
		bool CanRedo { get; }
		void Redo();
		string RedoActionName { get; }
		string UndoActionName { get; }
		 bool AllowDrop { get; set; }
		bool AutoWordSelection { get; set; }
		int BulletIndent { get; set; }
		bool DetectUrls { get; set; }
		bool EnableAutoDragDrop { get; set; }
		 Font Font { get; set; }
		RichTextBoxLanguageOptions LanguageOption { get; set; }
		bool RichTextShortcutsEnabled { get; set; }
		int RightMargin { get; set; }
		string Rtf { get; set; }
		RichTextBoxScrollBars ScrollBars { get; set; }
		string SelectedRtf { get; set; }
		HorizontalAlignment SelectionAlignment { get; set; }
		Color SelectionBackColor { get; set; }
		bool SelectionBullet { get; set; }
		int SelectionCharOffset { get; set; }
		Color SelectionColor { get; set; }
		Font SelectionFont { get; set; }
		int SelectionHangingIndent { get; set; }
		int SelectionIndent { get; set; }
		bool SelectionProtected { get; set; }
		int SelectionRightIndent { get; set; }
		int[] SelectionTabs { get; set; }
		RichTextBoxSelectionTypes SelectionType { get; }
		bool ShowSelectionMargin { get; set; }
		float ZoomFactor { get; set; }
		event ContentsResizedEventHandler ContentsResized;
		event DragEventHandler DragDrop;
		event DragEventHandler DragEnter;
		event EventHandler DragLeave;
		event DragEventHandler DragOver;
		event GiveFeedbackEventHandler GiveFeedback;
		event EventHandler HScroll;
		event EventHandler ImeChange;
		event LinkClickedEventHandler LinkClicked;
		event EventHandler Protected;
		event QueryContinueDragEventHandler QueryContinueDrag;
		event EventHandler SelectionChanged;
		event EventHandler VScroll;
	}
	#endregion
}
