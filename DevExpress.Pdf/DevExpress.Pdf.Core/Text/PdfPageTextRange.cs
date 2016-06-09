#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

namespace DevExpress.Pdf.Native {
	public class PdfPageTextRange {
		public static bool AreEqual(PdfPageTextRange t1, PdfPageTextRange t2) {
			if (t1 == null)
				return t2 == null;
			if (t2 == null)
				return false;
			return t1.pageIndex == t2.pageIndex && t1.wholePage == t2.wholePage && 
				PdfPageTextPosition.AreEqual(t1.startTextPosition, t2.startTextPosition) && PdfPageTextPosition.AreEqual(t1.endTextPosition, t2.endTextPosition);
		}
		readonly int pageIndex;
		readonly PdfPageTextPosition startTextPosition;
		readonly PdfPageTextPosition endTextPosition;
		readonly bool wholePage;
		public int PageIndex { get { return pageIndex; } }
		public PdfPageTextPosition StartTextPosition { get { return startTextPosition; } }
		public PdfPageTextPosition EndTextPosition { get { return endTextPosition; } }
		public int StartWordNumber { get { return startTextPosition.WordNumber; } }
		public int EndWordNumber { get { return endTextPosition.WordNumber; } }
		public int StartOffset { get { return startTextPosition.Offset; } }
		public int EndOffset { get { return endTextPosition.Offset; } }
		public bool WholePage { get { return wholePage; } }
		public PdfPageTextRange(int pageIndex, PdfPageTextPosition startTextPosition, PdfPageTextPosition endTextPosition) {
			this.pageIndex = pageIndex;
			this.startTextPosition = startTextPosition;
			this.endTextPosition = endTextPosition;
		}
		public PdfPageTextRange(int pageIndex, int startWordNumber, int startOffset, int endWordNumber, int endOffset) 
			: this(pageIndex, new PdfPageTextPosition(startWordNumber, startOffset), new PdfPageTextPosition(endWordNumber, endOffset)) {
		}
		public PdfPageTextRange(int pageIndex) : this(pageIndex, new PdfPageTextPosition(0, 0), new PdfPageTextPosition(0, 0)) {
			this.wholePage = true;
		}
	}
}
