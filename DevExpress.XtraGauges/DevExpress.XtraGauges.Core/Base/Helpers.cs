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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraGauges.Core.Base {
	public sealed class AffinityHelper {
		public static void SetCompositeAffinity<T>(IComposite<T> parent, BaseElement<T> element)
				where T : class, IElement<T> {
			if(AffinityHelperException.Assert(element) && element.parentCore != parent) {
				element.parentCore = parent;
			}
		}
	}
	public sealed class NameLockHelper {
		public static void LockName<T>(BaseElement<T> element)
				where T : class, IElement<T> {
			if(element == null || element.IsNameLocked) return;
			element.nameLockedCore = true;
		}
		public static void UnlockName<T>(BaseElement<T> element)
				where T : class, IElement<T> {
			if(element == null || !element.IsNameLocked) return;
			element.nameLockedCore = false;
		}
	}
	public sealed class SimpleCache : BaseObject, ISupportCaching {
		Hashtable cacheCore;
		int lockCounter;
		protected override void OnCreate() {
			this.cacheCore = new Hashtable();
		}
		protected override void OnDispose() {
			if(cacheCore != null) {
				cacheCore.Clear();
				cacheCore = null;
			}
		}
		public void LockCaching() {
			lockCounter++;
		}
		public void UnlockCaching() {
			lockCounter--;
		}
		public bool IsCachingLocked {
			get { return lockCounter > 0; }
		}
		public void CacheValue(object key, object value) {
			if(IsCachingLocked) return;
			cacheCore[key] = value;
		}
		public bool HasValue(object key) {
			return cacheCore.ContainsKey(key);
		}
		public object TryGetValue(object key) {
			return IsCachingLocked ? null : cacheCore[key];
		}
		public void ResetCache(object key) {
			IDisposable disposableValue = cacheCore[key] as IDisposable;
			cacheCore.Remove(key);
			if(disposableValue != null) disposableValue.Dispose();
		}
		public void ResetCache() {
			foreach(object value in cacheCore.Values) {
				IDisposable disposableValue = value as IDisposable;
				if(disposableValue != null) disposableValue.Dispose();
			}
			cacheCore.Clear();
		}
	}
	public sealed class AcceptComparer<T> : IComparer<T>
		where T : class, ISupportAcceptOrder {
		public int Compare(T x, T y) {
			return x.AcceptOrder.CompareTo(y.AcceptOrder);
		}
	}
	internal static class Ref {
		[System.Diagnostics.DebuggerStepThrough]
		public static void Dispose<T>(ref T refToDispose)
			where T : class, IDisposable {
			DisposeCore(refToDispose);
			refToDispose = null;
		}
		static void DisposeCore(IDisposable refToDispose) {
			if(refToDispose != null) refToDispose.Dispose();
		}
	}
}
