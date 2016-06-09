#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils.Controls;
using System;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ComponentNameControllerBase<T> : IDisposable where T : class {
		readonly IObjectNameGenerator context;
		readonly NotifyingCollection<T> collection;
		public IEnumerable<string> ComponentNames {
			get {
				foreach(T item in collection)
					yield return GetComponentName(item);
			}
		}
		protected IObjectNameGenerator Context { get { return context; } }
		protected ComponentNameControllerBase(NotifyingCollection<T> collection, IObjectNameGenerator context) {
			this.collection = collection;
			this.context = context;
			SubscribeCollectionEvents();
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeCollectionEvents();
				UnsubscribeItemsEvents(collection);
			}
		}
		protected abstract string GetComponentName(T item);
		protected abstract void SetComponentName(T item, string componentName);
		protected abstract void SubscribeItemEvents(T item);
		protected abstract void UnsubscribeItemEvents(T item);
		protected abstract void CheckComponentName(string componentName);
		void SubscribeCollectionEvents() {
			collection.BeforeItemAdded += OnBeforeItemAdded;
			collection.CollectionChanged += OnCollectionChanged;
		}
		void UnsubscribeCollectionEvents() {
			collection.BeforeItemAdded -= OnBeforeItemAdded;
			collection.CollectionChanged -= OnCollectionChanged;
		}
		void SubscribeItemsEvents(IEnumerable<T> items) {
			foreach(T item in items)
				SubscribeItemEvents(item);
		}
		void UnsubscribeItemsEvents(IEnumerable<T> items) {
			foreach(T item in items)
				UnsubscribeItemEvents(item);
		}
		void OnCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<T> e) {
			UnsubscribeItemsEvents(e.RemovedItems);
			SubscribeItemsEvents(e.AddedItems);
		}
		void OnBeforeItemAdded(object sender, NotifyingCollectionBeforeItemAddedEventArgs<T> e) {
			string name = GetComponentName(e.Item);
			if(string.IsNullOrEmpty(name))
				SetComponentName(e.Item, context.GenerateName(e.Item));
			else
				CheckComponentName(name);
		}
	}
}
