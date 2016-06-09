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
namespace DevExpress.Printing.Native {
	public class PageScope {
		public static readonly PageScope Empty;
		static PageScope() {
			Empty = new PageScope(0, 0);
		}
		int fromPage;
		int toPage;
		public int FromPage { get { return fromPage; } set { fromPage = value; } }
		public int ToPage { get { return toPage; } set { toPage = value; } }
		public string PageRange {
			get {
				if (IsEmpty)
					return string.Empty;
				return fromPage == toPage ? fromPage.ToString() : string.Format("{0}-{1}", fromPage, toPage);
			}
		}
		public bool IsEmpty {
			get {
				return fromPage == 0 && toPage == 0;
			}
		}
		public PageScope(int fromPage, int toPage) {
			this.fromPage = fromPage;
			this.toPage = toPage;
		}
		public PageScope(string pageRange, int maximumPage) {
			int[] pageIndices = PageRangeParser.GetIndices(pageRange, maximumPage);
			if (pageIndices.Length > 0) {
				fromPage = pageIndices[0] + 1;
				toPage = pageIndices[pageIndices.Length - 1] + 1;
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (obj is PageScope) {
				PageScope pageScope = (PageScope)obj;
				return FromPage == pageScope.FromPage && ToPage == pageScope.ToPage;
			}
			return base.Equals(obj);
		}
		public PageScope Validate(int pageCount) {
			PageScope pageScope = new PageScope(FromPage, ToPage);
			pageScope.FromPage = Math.Max(1, Math.Min(pageCount, pageScope.FromPage));
			pageScope.ToPage = Math.Max(1, Math.Min(pageCount, pageScope.ToPage));
			pageScope.ToPage = Math.Max(pageScope.FromPage, pageScope.ToPage);
			return pageScope;
		}
	}
}
