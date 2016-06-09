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
using System.Linq;
using System.Text;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
namespace DevExpress.XtraGrid.Views.WinExplorer {
	public class WinExplorerViewScrollingCache {
		int[] cachedPositions;
		WinExplorerViewInfo viewInfo;
		public WinExplorerViewScrollingCache(WinExplorerViewInfo viewInfo) {
			IsDirty = true;
			this.viewInfo = viewInfo;
			this.cachedPositions = null;
		}
		public bool IsEmpty { get { return CachedPositions.Length == 0; } }
		public int Length { get { return CachedPositions.Length; } }
		public void CheckCache() {
			if(this.cachedPositions != null && IsDataSourceChanged()) {
				ReCreateCache();
			}
			if(IsDirty)
				InitializeCache();
		}
		public int this[int index] {
			get { return CachedPositions[index]; }
		}
		public int CalcScrollableAreaHeightGroup() {
			return CachedPositions[CachedPositions.Length - 1] + ViewInfo.CalcGroupHeight(-CachedPositions.Length);
		}
		public int CalcGroupLocationByHandle(int groupRowHandle) {
			return CachedPositions[-groupRowHandle - 1];
		}
		protected bool IsDataSourceChanged() {
			int rowsCount = ViewInfo.View.DataController.GroupRowCount;
			return this.cachedPositions.Length != rowsCount;
		}
		protected virtual void InitializeCache() {
			IsDirty = false;
			int groupRowHandle = -1, index = 1;
			int offset = 0;
			while(ViewInfo.View.IsValidRowHandle(groupRowHandle)) {
				offset += ViewInfo.CalcGroupHeight(groupRowHandle);
				if(index < CachedPositions.Length) {
					CachedPositions[index] = offset;
					index++;
				}
				groupRowHandle--;
			}
		}
		protected virtual void ReCreateCache() {
			this.cachedPositions = new int[ViewInfo.View.DataController.GroupRowCount];
			InitializeCache();
		}
		protected int[] CachedPositions {
			get {
				if(this.cachedPositions == null || IsDirty) {
					ReCreateCache();
				}
				return this.cachedPositions;
			}
		}
		public bool IsDirty { get; protected internal set; }
		public WinExplorerViewInfo ViewInfo { get { return viewInfo; } }
		protected internal virtual void UpdateCacheFromHandle(int groupRowHandle) {
			if(IsDirty)
				return;
			int cacheIndex = -groupRowHandle - 1;
			if(cacheIndex == CachedPositions.Length - 1)
				return;
			int groupHeight = ViewInfo.CalcGroupHeight(groupRowHandle);
			int delta = groupHeight - (CachedPositions[cacheIndex + 1] - CachedPositions[cacheIndex]);
			for(int i = cacheIndex + 1; i < CachedPositions.Length; i++) {
				CachedPositions[i] += delta;
			}
		}
	}
	public class WinExplorerViewListScrollingCache : WinExplorerViewScrollingCache {
		public WinExplorerViewListScrollingCache(WinExplorerViewInfo viewInfo) : base(viewInfo) { }
		protected override void InitializeCache() {
			IsDirty = false;
			int groupRowHandle = -1, index = 1;
			int offset = 0;
			while(ViewInfo.View.IsValidRowHandle(groupRowHandle)) {
				offset += ViewInfo.CalcGroupColumnCount(groupRowHandle);
				if(index < CachedPositions.Length) {
					CachedPositions[index] = offset;
					index++;
				}
				groupRowHandle--;
			}
		}
		public int CalcScrollableAreaColumnCount() {
			return CachedPositions[CachedPositions.Length - 1] + ViewInfo.CalcGroupColumnCount(-CachedPositions.Length);
		}
	}
}
