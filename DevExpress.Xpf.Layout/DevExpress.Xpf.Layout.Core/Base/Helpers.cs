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
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Layout.Core {
	public static class Ref {
		public static void Dispose<T>(ref T refToDispose)
			where T : class, IDisposable {
			DisposeCore(refToDispose);
			refToDispose = null;
		}
		public static void Clear<TKey, TValue>(ref IDictionary<TKey, TValue> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		public static void Clear<T>(ref IList<T> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		public static void Clear<T>(ref ICollection<T> refToClear) {
			ClearCore(refToClear);
			DisposeCore(refToClear as IDisposable);
			refToClear = null;
		}
		static void DisposeCore(IDisposable refToDispose) {
			if(refToDispose != null) refToDispose.Dispose();
		}
		static void ClearCore<T>(ICollection<T> refToClear) {
			if(refToClear != null) refToClear.Clear();
		}
	}
	public static class CollectionHelper {
		public static void Accept<T>(this ICollection<T> collection, VisitDelegate<T> visit) where T : class {
			foreach(T item in collection) visit(item);
		}
		public static bool IsValidIndex<T>(this ICollection<T> collection, int index) where T : class {
			return index >= 0 && index < collection.Count - 1;
		}
	}
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
