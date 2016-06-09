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
using System.ComponentModel;
using System.Collections.Specialized;
#if SILVERLIGHT && !FREE
using DevExpress.Data.Browsing;
#endif
#if MVVM
namespace DevExpress.Mvvm.UI.Native {
#else
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Utils {
#endif
	#region WeakEventHandler
	public interface IWeakEventHandler<THandler> {
		THandler Handler { get; }
	}
	public class WeakEventHandler<TOwner, TEventArgs, THandler> : IWeakEventHandler<THandler> where TOwner : class {
		WeakReference ownerReference;
		Action<WeakEventHandler<TOwner, TEventArgs, THandler>, object> onDetachAction;
		Action<TOwner, object, TEventArgs> onEventAction;
		public THandler Handler { get; private set; }
		public WeakEventHandler(TOwner owner, Action<TOwner, object, TEventArgs> onEventAction, Action<WeakEventHandler<TOwner, TEventArgs, THandler>, object> onDetachAction, Func<WeakEventHandler<TOwner, TEventArgs, THandler>, THandler> createHandlerFunction) {
			this.ownerReference = new WeakReference(owner);
			this.onEventAction = onEventAction;
			this.onDetachAction = onDetachAction;
			this.Handler = createHandlerFunction(this);
		}
		public void OnEvent(object source, TEventArgs eventArgs) {
			TOwner target = ownerReference.Target as TOwner;
			if(target != null) {
				onEventAction(target, source, eventArgs);
			} else {
				onDetachAction(this, source);
			}
		}
	}
	public class PropertyChangedWeakEventHandler<TOwner> : WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler> where TOwner : class {
		static Action<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, object> action = (h, o) => ((INotifyPropertyChanged)o).PropertyChanged -= h.Handler;
		static Func<WeakEventHandler<TOwner, PropertyChangedEventArgs, PropertyChangedEventHandler>, PropertyChangedEventHandler> create = h => new PropertyChangedEventHandler(h.OnEvent);
		public PropertyChangedWeakEventHandler(TOwner owner, Action<TOwner, object, PropertyChangedEventArgs> onEventAction)
			: base(owner, onEventAction, action, create) {
		}
	}
#if !MVVM
	public class CollectionChangedWeakEventHandler<TOwner> : WeakEventHandler<TOwner, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler> where TOwner : class {
		 static Action<WeakEventHandler<TOwner, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>, object> action = (h, o) => ((INotifyCollectionChanged)o).CollectionChanged -= h.Handler;
		 static Func<WeakEventHandler<TOwner, NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>, NotifyCollectionChangedEventHandler> create = h => new NotifyCollectionChangedEventHandler(h.OnEvent);
		public CollectionChangedWeakEventHandler(TOwner owner, Action<TOwner, object, NotifyCollectionChangedEventArgs> onEventAction)
			: base(owner, onEventAction, action, create) {
		}
	}
	public class ListChangedWeakEventHandler<TOwner> : WeakEventHandler<TOwner, ListChangedEventArgs, ListChangedEventHandler> where TOwner : class {
	static Action<WeakEventHandler<TOwner, ListChangedEventArgs, ListChangedEventHandler>, object> action = (h, o) => ((IBindingList)o).ListChanged -= h.Handler;
	static Func<WeakEventHandler<TOwner, ListChangedEventArgs, ListChangedEventHandler>, ListChangedEventHandler> create = h => new ListChangedEventHandler(h.OnEvent);
		public ListChangedWeakEventHandler(TOwner owner, Action<TOwner, object, ListChangedEventArgs> onEventAction)
			: base(owner, onEventAction, action, create) {
		}
	}
#endif
	#endregion
}
