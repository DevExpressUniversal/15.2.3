#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
namespace DevExpress.Mvvm.Native {
	public class EventFireCounter<TObject, TEventArgs> where TEventArgs : EventArgs {
		readonly Action<EventHandler> unsubscribe;
		protected readonly EventHandler handler;
		public int FireCount { get; private set; }
		public TEventArgs LastArgs { get; private set; }
		public EventFireCounter(Action<EventHandler> subscribe, Action<EventHandler> unsubscribe) {
			this.unsubscribe = unsubscribe;
			handler = new EventHandler(OnEvent);
			subscribe(OnEvent);
		}
		void OnEvent(object source, EventArgs eventArgs) {
			FireCount++;
			LastArgs = (TEventArgs)eventArgs;
		}
		public void Unsubscribe() {
			unsubscribe(OnEvent);
		}
	}
	public class CanExecuteChangedCounter : EventFireCounter<ICommand, EventArgs> {
		public CanExecuteChangedCounter(ICommand command)
			: base(h => command.CanExecuteChanged += h, h => command.CanExecuteChanged -= h) {
		}
	}
	public class CollectionChangedCounter : EventFireCounter<INotifyCollectionChanged, NotifyCollectionChangedEventArgs> {
		public CollectionChangedCounter(INotifyCollectionChanged collection)
			: base(h => collection.CollectionChanged += new NotifyCollectionChangedEventHandler((o, e) => { h(o, e); }), null) {
		}
	}
	public class PropertyChangedCounter : EventFireCounter<INotifyPropertyChanged, PropertyChangedEventArgs> {
		public PropertyChangedCounter(INotifyPropertyChanged obj, string propertyName)
			: base(h => obj.PropertyChanged += new PropertyChangedEventHandler((o, e) => { if(e.PropertyName == propertyName) h(o, e); }), null) {
		}
	}
}
