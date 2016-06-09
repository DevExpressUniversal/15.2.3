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
using System.Windows.Forms;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010 {
	internal static class Ref {
		[System.Diagnostics.DebuggerStepThrough]
		public static void Dispose<T>(ref T refToDispose)
			where T : class, IDisposable {
			DisposeCore(refToDispose);
			refToDispose = null;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public static void Clear<TKey, TValue>(ref IDictionary<TKey, TValue> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public static void DisposeItemsAndClear<TKey, TValue>(ref IDictionary<TKey, TValue> refToClear) {
			DisposeItemsAndClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public static void Clear<T>(ref IList<T> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public static void Clear<T>(ref ICollection<T> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		static void DisposeCore(IDisposable refToDispose) {
			if(refToDispose != null) refToDispose.Dispose();
		}
		static void DisposeItemsAndClearCore<TKey, TValue>(IDictionary<TKey, TValue> refToClear) {
			if(refToClear != null && refToClear.Count > 0) {
				KeyValuePair<TKey, TValue>[] items = new KeyValuePair<TKey, TValue>[refToClear.Count];
				refToClear.CopyTo(items, 0);
				refToClear.Clear();
				for(int i = 0; i < items.Length; i++)
					DisposeCore(items[i].Value as IDisposable);
			}
		}
		static void ClearCore<T>(ICollection<T> refToClear) {
			if(refToClear != null) refToClear.Clear();
		}
	}
	internal sealed class SharedRef<T> : IDisposable
		where T : class, IDisposable {
		T source;
		public T Target {
			get { return source; }
		}
		public SharedRef(T target) {
			refCounter++;
			this.source = target;
		}
#if DEBUGTEST
		internal int refCounter;
#else
		int refCounter;
#endif
		public void Share(ref SharedRef<T> targetRef) {
			if(targetRef != this) {
				refCounter++;
				bool sharingAllowed = true;
				ISharedRefTarget<T> r = Target as ISharedRefTarget<T>;
				if(r != null) {
					sharingAllowed = r.CanShare(targetRef.Target);
					if(sharingAllowed)
						r.Share(targetRef.Target);
				}
				if(sharingAllowed) {
					Ref.Dispose(ref targetRef);
					targetRef = this;
				}
			}
		}
		void IDisposable.Dispose() {
			if(--refCounter == 0)
				Ref.Dispose(ref source);
		}
	}
	internal interface ISharedRefTarget<T> where T : class {
		bool CanShare(T target);
		void Share(T target);
	}
	internal sealed class DisposableObjectsContainer : IDisposable {
		List<IDisposable> disposableObjects;
		public DisposableObjectsContainer() {
			disposableObjects = new List<IDisposable>(8);
		}
		void IDisposable.Dispose() {
			OnDisposing();
			GC.SuppressFinalize(this);
		}
		void OnDisposing() {
			foreach(IDisposable disposable in disposableObjects)
				disposable.Dispose();
			disposableObjects.Clear();
		}
		[System.Diagnostics.DebuggerStepThrough]
		public T Register<T>(T obj) where T : IDisposable {
			if(!object.Equals(obj, null) && !disposableObjects.Contains(obj))
				disposableObjects.Add(obj);
			return obj;
		}
	}
	public sealed class BatchUpdate : Base.IBatchUpdate {
		ISupportBatchUpdate Component;
		bool cancelUpdate;
		BatchUpdate(ISupportBatchUpdate component, bool cancelUpdate) {
			Component = component;
			this.cancelUpdate = cancelUpdate;
			Component.BeginUpdate();
		}
		void IDisposable.Dispose() {
			if(cancelUpdate)
				Component.CancelUpdate();
			else Component.EndUpdate();
			Component = null;
		}
		public void Cancel() {
			cancelUpdate = true;
		}
		public static Base.IBatchUpdate Enter(ISupportBatchUpdate component) {
			return (component != null) ? new BatchUpdate(component, false) : null;
		}
		public static Base.IBatchUpdate Enter(ISupportBatchUpdate component, bool cancelUpdate) {
			return (component != null) ? new BatchUpdate(component, cancelUpdate) : null;
		}
	}
	internal class ControlMethodBuilder {
		internal static Func<Control, TParam, TResult> BuildControlInvoke<TParam, TResult>(string methodName) {
			var method = typeof(Control).GetMethod(methodName,
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			var target = System.Linq.Expressions.Expression.Parameter(typeof(Control), "control");
			var param = System.Linq.Expressions.Expression.Parameter(typeof(TParam), "param");
			return System.Linq.Expressions.Expression.Lambda<Func<Control, TParam, TResult>>(
						System.Linq.Expressions.Expression.Call(target, method, param),
						target, param
					).Compile();
		}
	}
}
namespace DevExpress.XtraBars.Docking2010.Base {
	public static class ServiceLocator<Service>
	where Service : class {
		delegate Service CreateService();
		static CreateService createInstance;
		public static void Register<ServiceProvider>()
			where ServiceProvider : Service, new() {
			if(createInstance == null)
				createInstance = delegate {
					return new ServiceProvider();
				};
		}
		public static Service Resolve() {
			AssertionException.IsNotNull(createInstance,
					string.Format("The {0} service is not registered", typeof(Service).Name)
				);
			return createInstance();
		}
	}
}
