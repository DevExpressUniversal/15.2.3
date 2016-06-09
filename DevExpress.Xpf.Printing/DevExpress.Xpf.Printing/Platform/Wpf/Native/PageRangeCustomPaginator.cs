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

using System.Windows;
using System.Windows.Documents;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing.Native {
	class PageRangeCustomPaginator : DocumentPaginator {
		readonly DocumentPaginator paginator;
		readonly int[] pageIndexes;
		public PageRangeCustomPaginator(DocumentPaginator paginator, int[] pageIndexes) {
			Guard.ArgumentNotNull(paginator, "paginator");
			Guard.ArgumentNotNull(pageIndexes, "pageIndexes");
			this.paginator = paginator;
			this.pageIndexes = pageIndexes;
		}
		public override int PageCount {
			get { return pageIndexes.Length; }
		}
		public override DocumentPage GetPage(int pageNumber) {
			return paginator.GetPage(pageIndexes[pageNumber]);
		}
		public override bool IsPageCountValid {
			get { return paginator.IsPageCountValid; }
		}
		public override Size PageSize {
			get { return paginator.PageSize; }
			set { paginator.PageSize = value; }
		}
		public override IDocumentPaginatorSource Source {
			get { return paginator.Source; }
		}
	}
}
