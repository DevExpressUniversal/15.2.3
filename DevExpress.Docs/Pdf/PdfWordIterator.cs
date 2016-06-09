#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	class PdfWordIterator {
		PdfDocument document;
		int pageIndex;
		int wordIndex;
		IList<PdfWord> pageWords;
		bool wordIteratorEnded;
		IList<PdfPage> Pages { get { return document.Pages; } }
		bool RecognizePage() {
			if (pageIndex < 0 || pageIndex >= Pages.Count)
				return false;
			pageWords = PdfDataRecognizer.Recognize(Pages[pageIndex], false).Words;
			return true;
		}
		public PdfWordIterator(PdfDocument document) : this(document, 0, -1) {
			wordIteratorEnded = true;
		}
		public PdfWordIterator(PdfDocument document, int pageIndex, int wordIndex) {
			this.document = document;
			this.pageIndex = pageIndex;
			this.wordIndex = wordIndex;
			RecognizePage();
			wordIteratorEnded = false;
		}
		public void Reset() {
			wordIteratorEnded = true;
		}
		public PdfPageWord Next() {
			if (wordIteratorEnded) {
				pageIndex = 0;
				if (!RecognizePage())
					return null;
				wordIndex = -1;
			}
			while (++wordIndex >= pageWords.Count) {
				++pageIndex;
				if (!RecognizePage()) {
					Reset();
					return null;
				}
				wordIndex = -1;
			}
			wordIteratorEnded = false;
			return new PdfPageWord(pageWords[wordIndex], pageIndex + 1);
		}
		public PdfPageWord Prev() {
			IList<PdfPage> pages = document.Pages;
			if (wordIteratorEnded) {
				pageIndex = pages.Count - 1;
				if (!RecognizePage())
					return null;
				wordIndex = pageWords.Count;
			}
			while (--wordIndex < 0) {
				--pageIndex;
				if (!RecognizePage()) {
					Reset();
					return null;
				}
				wordIndex = pageWords.Count;
			}
			wordIteratorEnded = false;
			return new PdfPageWord(pageWords[wordIndex], pageIndex + 1);
		}
	}
}
