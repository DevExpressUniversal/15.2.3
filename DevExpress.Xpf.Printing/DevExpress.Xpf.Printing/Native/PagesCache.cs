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
using System.Windows;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class PagesCache {
		readonly int cacheSize;
		FrameworkElement firstPage;
		Dictionary<int, FrameworkElement> cachedPages;
		Dictionary<int, DateTime> timeMarkers;
		public int CacheSize { get { return cacheSize; } }
		public PagesCache(int cacheSize) {
			if(cacheSize <= 0)
				throw new ArgumentException("cacheSize");
			this.cacheSize = cacheSize;
			cachedPages = new Dictionary<int, FrameworkElement>();
			timeMarkers = new Dictionary<int, DateTime>();
		}
		public void AddPage(FrameworkElement page, int pageIndex) {
			if(pageIndex == 0) {
				firstPage = page;
				return;
			}
			bool cachedPagesContainsPageIndexKey = cachedPages.ContainsKey(pageIndex);
			if(!cachedPagesContainsPageIndexKey) {
				if(cachedPages.Count >= cacheSize - 1) {
					RemovePageCore(GetDeprecatedPageIndex());
				}
				if(cachedPagesContainsPageIndexKey) {
					RemovePageCore(pageIndex);
				}
				AddPageCore(page, pageIndex);
			}
		}
		public FrameworkElement GetPage(int pageIndex) {
			if(pageIndex == 0) {
				return firstPage;
			}
			if(!cachedPages.ContainsKey(pageIndex))
				return null;
			timeMarkers[pageIndex] = DateTime.Now;
			return cachedPages[pageIndex];
		}
		public void Clear() {
			firstPage = null;
			cachedPages.Clear();
			timeMarkers.Clear();
		}
		void AddPageCore(FrameworkElement page, int pageIndex) {
			cachedPages.Add(pageIndex, page);
			timeMarkers.Add(pageIndex, DateTime.Now);
		}
		void RemovePageCore(int pageIndex) {
			cachedPages.Remove(pageIndex);
			timeMarkers.Remove(pageIndex);
		}
		int GetDeprecatedPageIndex() {
			DateTime min = DateTime.MaxValue;
			int minIndex = -1;
			foreach(KeyValuePair<int, DateTime> pair in timeMarkers) {
				if(min > pair.Value) {
					min = pair.Value;
					minIndex = pair.Key;
				}
			}
			return minIndex;
		}
	}
}
