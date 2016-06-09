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

using DevExpress.Mvvm;
using DevExpress.Pdf;
namespace DevExpress.Xpf.PdfViewer {
	public enum PdfSelectionAction {
		SelectViaArea,
		ClearSelection,
		SelectAllText,
		StartSelection,
		ContinueSelection,
		EndSelection,
		SelectWord,
		SelectLine,
		SelectPage,
		SelectImage,
		SetSelection,
		SelectLeft,
		SelectUp,
		SelectRight,
		SelectDown,
		SelectLineStart,
		SelectLineEnd,
		SelectNextWord,
		SelectPreviousWord,
		SelectDocumentStart,
		SelectDocumentEnd,
		MoveLeft,
		MoveUp,
		MoveRight,
		MoveDown,
		MoveLineStart,
		MoveLineEnd,
		MoveNextWord,
		MovePreviousWord,
		MoveDocumentStart,
		MoveDocumentEnd,
	}
	public class PdfDocumentSelectionParameter : BindableBase {
		PdfSelectionAction selectionAction;
		PdfDocumentPosition position;
		PdfDocumentArea area;
		public PdfSelectionAction SelectionAction {
			get { return selectionAction; }
			set { SetProperty(ref selectionAction, value, () => SelectionAction); }
		}
		public PdfDocumentArea Area {
			get { return area; }
			set { SetProperty(ref area, value, () => Area); }
		}
		public PdfDocumentPosition Position {
			get { return position; }
			set { SetProperty(ref position, value, () => Position); }
		}
	}
}
