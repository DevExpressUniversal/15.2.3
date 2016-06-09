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

using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using System;
using System.Windows;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Pdf;
namespace DevExpress.Xpf.PdfViewer {
	public enum PdfSelectionCommand {
		SelectUp,
		SelectDown,
		SelectLeft,
		SelectRight,
		SelectLineStart,
		SelectLineEnd,
		SelectDocumentStart,
		SelectDocumentEnd,
		SelectNextWord,
		SelectPreviousWord,
		MoveUp,
		MoveDown,
		MoveLeft,
		MoveRight,
		MoveLineStart,
		MoveLineEnd,
		MoveDocumentStart,
		MoveDocumentEnd,
		MoveNextWord,
		MovePreviousWord,
	}
	public class PdfBehaviorProvider : BehaviorProvider {
		public event EventHandler<EventArgs> FunctionalLimitsOccured;
		public bool ShouldTestFunctionalLimits { get; set; }
		public int CurrentPageNumber { get { return PageIndex + 1; } }
		public virtual void RaiseFunctionalLimitsOccured() {
			if (FunctionalLimitsOccured != null)
				FunctionalLimitsOccured(this, new EventArgs());
		}
	}
}
