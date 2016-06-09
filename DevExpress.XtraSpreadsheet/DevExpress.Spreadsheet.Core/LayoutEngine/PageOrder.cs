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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region PageOrder (abstract class)
	public abstract class PageOrder {
		public abstract int GetColumnGridIndex(int pageIndex);
		public abstract int GetRowGridIndex(int pageIndex);
		public abstract Page GetPreviousPageHorizontalDirection(List<Page> pages, Page page);
		public abstract Page GetNextPageHorizontalDirection(List<Page> pages, Page page);
		public abstract Page GetPreviousPageVerticalDirection(List<Page> pages, Page page);
		public abstract Page GetNextPageVerticalDirection(List<Page> pages, Page page);
		public abstract void ReorderPages(List<Page> pages);
	}
	#endregion
	#region PageOrderDownThenOver
	public class PageOrderDownThenOver : PageOrder {
		readonly int columnCount;
		readonly int rowCount;
		public override int GetColumnGridIndex(int pageIndex) {
			return pageIndex / rowCount;
		}
		public override int GetRowGridIndex(int pageIndex) {
			return pageIndex % rowCount;
		}
		public PageOrderDownThenOver(int columnCount, int rowCount) {
			this.columnCount = columnCount;
			this.rowCount = rowCount;
		}
		public override Page GetPreviousPageVerticalDirection(List<Page> pages, Page page) {
			int index = (page.Index % rowCount) - 1;
			if(index < 0)
				return null;
			else
				return pages[index];
		}
		public override Page GetNextPageVerticalDirection(List<Page> pages, Page page) {
			int index = (page.Index % rowCount) + 1;
			if(index >= rowCount)
				return null;
			else
				return pages[index];
		}
		public override Page GetPreviousPageHorizontalDirection(List<Page> pages, Page page) {
			int index = page.Index - rowCount;
			if(index < 0)
				return null;
			else
				return pages[index];
		}
		public override Page GetNextPageHorizontalDirection(List<Page> pages, Page page) {
			int index = page.Index + rowCount;
			if(index >= pages.Count)
				return null;
			else
				return pages[index];
		}
		public override void ReorderPages(List<Page> pages) {
			Page[] originalPages = pages.ToArray();
			int i = 0;
			for(int x = 0; x < columnCount; x++)
				for(int y = 0; y < rowCount; y++, i++) {
					pages[i] = originalPages[x + y * columnCount];
				}
		}
	}
	#endregion
	#region PageOrderOverThenDown
	public class PageOrderOverThenDown : PageOrder {
		readonly int columnCount;
		public PageOrderOverThenDown(int columnCount, int rowCount) {
			this.columnCount = columnCount;
		}
		public override int GetColumnGridIndex(int pageIndex) {
			return pageIndex % columnCount;
		}
		public override int GetRowGridIndex(int pageIndex) {
			return pageIndex / columnCount;
		}
		public override Page GetPreviousPageVerticalDirection(List<Page> pages, Page page) {
			int index = page.Index - columnCount;
			if(index < 0)
				return null;
			else
				return pages[index];
		}
		public override Page GetNextPageVerticalDirection(List<Page> pages, Page page) {
			int index = page.Index + columnCount;
			if(index >= pages.Count)
				return null;
			else
				return pages[index];
		}
		public override Page GetPreviousPageHorizontalDirection(List<Page> pages, Page page) {
			int index = (page.Index % columnCount) - 1;
			if(index < 0)
				return null;
			else
				return pages[index];
		}
		public override Page GetNextPageHorizontalDirection(List<Page> pages, Page page) {
			int index = (page.Index % columnCount) + 1;
			if(index >= columnCount)
				return null;
			else
				return pages[index];
		}
		public override void ReorderPages(List<Page> pages) {
		}
	}
	#endregion
}
