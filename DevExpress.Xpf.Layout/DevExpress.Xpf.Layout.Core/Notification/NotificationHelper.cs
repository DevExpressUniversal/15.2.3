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
namespace DevExpress.Xpf.Layout.Core {
	public static class NotificationHelper {
		internal static Dictionary<object, Tracker<NotificationEventArgs>> trackers =
			new Dictionary<object, Tracker<NotificationEventArgs>>();
		public static IDisposable Subscribe(object source, IObserver<NotificationEventArgs> observer) {
			if(observer == null || source == null) return null;
			lock(trackers) {
				Tracker<NotificationEventArgs> tracker;
				if(!trackers.TryGetValue(source, out tracker)) {
					CollectUnusedTrackers(trackers);
					tracker = new Tracker<NotificationEventArgs>();
					trackers.Add(source, tracker);
				}
				return tracker.Subscribe(observer);
			}
		}
		public static void Notify(object source, NotificationEventArgs action) {
			lock(trackers) {
				Tracker<NotificationEventArgs> tracker;
				if(trackers.TryGetValue(source, out tracker)) {
					if(tracker.InUse)
						tracker.Notify(action);
					else trackers.Remove(source);
				}
			}
		}
		public static void EndNotification(object source) {
			lock(trackers) {
				Tracker<NotificationEventArgs> tracker;
				if(trackers.TryGetValue(source, out tracker)) {
					tracker.EndNotification();
				}
				trackers.Remove(source);
			}
		}
		static void CollectUnusedTrackers(IDictionary<object, Tracker<NotificationEventArgs>> trackers) {
			ICollection<object> toRemove = new List<object>();
			foreach(var pair in trackers) {
				if(!pair.Value.InUse)
					toRemove.Add(pair.Key);
			}
			foreach(object source in toRemove) {
				trackers.Remove(source);
			}
		}
	}
	class Tracker<T> : IObservable<T> {
		ICollection<IObserver<T>> observers;
		public Tracker() {
			observers = new List<IObserver<T>>();
		}
		public IDisposable Subscribe(IObserver<T> observer) {
			if(!observers.Contains(observer))
				observers.Add(observer);
			return new Unsubscriber(observers, observer);
		}
		public bool InUse {
			get { return observers.Count > 0; }
		}
		public void Notify(T element) {
			Exception e = CheckError(element);
			IObserver<T>[] observersArray = GetObservers();
			foreach(IObserver<T> observer in observersArray) {
				if(e != null)
					observer.OnError(e);
				else
					observer.OnNext(element);
			}
		}
		protected virtual Exception CheckError(T element) {
			return null;
		}
		public void EndNotification() {
			IObserver<T>[] observersArray = GetObservers();
			foreach(IObserver<T> observer in observersArray) {
				if(observers.Contains(observer))
					observer.OnCompleted();
			}
			observers.Clear();
		}
		IObserver<T>[] GetObservers() {
			IObserver<T>[] observersArray = new IObserver<T>[observers.Count];
			observers.CopyTo(observersArray, 0);
			return observersArray;
		}
		class Unsubscriber : IDisposable {
			ICollection<IObserver<T>> observers;
			IObserver<T> observer;
			public Unsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer) {
				this.observers = observers;
				this.observer = observer;
			}
			public void Dispose() {
				if(observer != null && observers.Contains(observer))
					observers.Remove(observer);
			}
		}
	}
}
