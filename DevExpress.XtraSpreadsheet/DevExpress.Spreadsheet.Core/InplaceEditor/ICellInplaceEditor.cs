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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region ICellInplaceEditor
	public interface ICellInplaceEditor : IDisposable {
		bool IsVisible { get; set; }
		bool WrapText { get; set; }
		string Text { get; set; }
		bool Focused { get; }
		Color ForeColor { get; set; }
		Color BackColor { get; set; }
		int SelectionStart { get; }
		int SelectionLength { get; }
		bool Registered { get; set; }
		bool CurrentEditable { get; set; }
		void Close();
		void SetFocus();
		void SetSelection(int start, int length);
		void SetFont(FontInfo fontInfo, float zoomFactor);
		void SetHorizontalAlignment(XlHorizontalAlignment alignment);
		void SetVerticalAlignment(XlVerticalAlignment alignment);
		void SetBounds(InplaceEditorBoundsInfo boundsInfo);
		void Activate();
		void Deactivate();
		void Rollback();
		void Copy();
		void Cut();
		void Paste();
		event TextChangedEventHandler EditorTextChanged;
		event EventHandler EditorSelectionChanged;
		event EventHandler GotFocus;
	}
	#endregion
}
