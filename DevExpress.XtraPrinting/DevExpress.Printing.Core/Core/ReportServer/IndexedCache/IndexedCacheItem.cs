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
namespace DevExpress.ReportServer.IndexedCache {
	class IndexedCacheItem<T> : IDisposable {
		public T Value { get; private set; }
		public IndexedCacheItemState State { get; set; }
		readonly HashSet<Action<int[]>> itemCachedCallbacks;
		public IndexedCacheItem(T value) {
			State = IndexedCacheItemState.NotRequested;
			Value = value;
			itemCachedCallbacks = new HashSet<Action<int[]>>();
		}
		public void AddItemCachedCallback(Action<int[]> callback) {
			if(!itemCachedCallbacks.Contains(callback))
				itemCachedCallbacks.Add(callback);
		}
		public Action<int[]>[] GetItemCachedCallbacks() {
			Action<int[]>[] result = new Action<int[]>[itemCachedCallbacks.Count];
			itemCachedCallbacks.CopyTo(result);
			return result;
		}
		public void SetRealValue(T value) {
			DisposeValue();
			Value = value;
			State = IndexedCacheItemState.Cached;
			itemCachedCallbacks.Clear();
		}
		public void Dispose() {
			DisposeValue();
			itemCachedCallbacks.Clear();
		}
		void DisposeValue() {
			IDisposable disposableValue = Value as IDisposable;
			if(disposableValue != null)
				disposableValue.Dispose();
		}
	}
}
