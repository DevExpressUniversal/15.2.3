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
using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class SimpleBridgeReadonlyObservableCollection<T, Key> : ReadOnlyObservableCollection<T>, IWeakEventListener where T : class {
		public static SimpleBridgeReadonlyObservableCollection<T, Key> Create(IList<Key> keys, Func<Key, T> cast) {
			ObservableCollectionCore<T> coreCollection = new ObservableCollectionCore<T>();
			return new SimpleBridgeReadonlyObservableCollection<T, Key>(coreCollection, keys, cast);
		}
		readonly ObservableCollection<T> coreCollection;
		readonly Func<Key, T> cast;
		readonly IList<Key> keys;
		SimpleBridgeReadonlyObservableCollection(ObservableCollection<T> coreCollection, IList<Key> keys, Func<Key, T> cast)
			: base(coreCollection) {
			this.coreCollection = coreCollection;
			this.cast = cast;
			this.keys = keys;
			SyncCollectionHelper.PopulateCore(coreCollection, (IList)keys, o => cast((Key)o), null);
			if(keys is INotifyCollectionChanged)
				CollectionChangedEventManager.AddListener((INotifyCollectionChanged)keys, this);
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				SyncCollectionHelper.SyncCollection((NotifyCollectionChangedEventArgs)e, coreCollection, (IList)keys, o => cast((Key)o));
				return true;
			}
			return false;
		}
	}
}
