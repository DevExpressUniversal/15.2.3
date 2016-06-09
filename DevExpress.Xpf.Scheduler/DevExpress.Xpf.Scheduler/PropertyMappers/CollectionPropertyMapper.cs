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

using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler.Native {
	#region Collection mappers
	public abstract class CollectionPropertyMapperBase<T, U> : DependencyPropertyMapperBase
		where T : class
		where U : class {
		protected CollectionPropertyMapperBase(DependencyProperty property, DependencyObject owner)
			: base(property, owner) {
#if !SL
			if (property.PropertyType != typeof(ObservableCollection<T>))
				Exceptions.ThrowArgumentException("TargetCollection", null);
#endif
		}
		public ObservableCollection<T> TargetCollection { get { return (ObservableCollection<T>)Owner.GetValue(base.TargetProperty); } }
		protected abstract U GetInnerCollectionItem(T targetItem);
		protected abstract T GetTargetCollectionItem(U innerItem);
		protected abstract NotificationCollection<U> InnerCollection { get; }
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SubscribeTargetCollectionEvents();
			SubscribeInnerCollectionEvents();
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			UnsubscribeTargetCollectionEvents();
			UnsubscribeInnerCollectionEvents();
		}
		protected virtual void SubscribeTargetCollectionEvents() {
			TargetCollection.CollectionChanged += OnTargetCollectionChanged;
		}
		protected virtual void UnsubscribeTargetCollectionEvents() {
			TargetCollection.CollectionChanged -= OnTargetCollectionChanged;
		}
		protected virtual void SubscribeInnerCollectionEvents() {
			InnerCollection.CollectionChanged += OnInnerCollectionChanged;
		}
		protected virtual void UnsubscribeInnerCollectionEvents() {
			InnerCollection.CollectionChanged -= OnInnerCollectionChanged;
		}
		void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UnsubscribeEvents();
			try {
				RecreateInnerCollection(null, TargetCollection);
			} finally {
				SubscribeEvents();
			}
		}
		void OnInnerCollectionChanged(object sender, CollectionChangedEventArgs<U> e) {
			UnsubscribeEvents();
			try {
				UpdateOwnerPropertyValue();
			} finally {
				SubscribeEvents();
			}
		}
		protected override void UpdateOwnerPropertyValue() {
			LoadInnerCollectionDefaultsIfEmpty();
			object newValue = GetInnerPropertyValue();
			Owner.SetValue(TargetProperty, newValue);
		}
		protected virtual void LoadInnerCollectionDefaultsIfEmpty() {
		}
		protected virtual void FillInnerCollection() {
			InnerCollection.BeginUpdate();
			try {
				InnerCollection.Clear();
				foreach (T item in TargetCollection) {
					U innerItem = GetInnerCollectionItem(item);
					InnerCollection.Add(innerItem);
				}
			} finally {
				InnerCollection.EndUpdate();
			}
		}
		public override object GetInnerPropertyValue() {
			LoadInnerCollectionDefaultsIfEmpty();
			return ConvertFromInnerCollection();
		}
		protected virtual void RecreateInnerCollection(ObservableCollection<T> oldCollection, ObservableCollection<T> targetCollection) {
			FillInnerCollection();
			LoadInnerCollectionDefaultsIfEmpty();
		}
		protected virtual ObservableCollection<T> ConvertFromInnerCollection() {
			ObservableCollection<T> newTarget = new ObservableCollection<T>();
			int count = InnerCollection.Count;
			for (int i = 0; i < count; i++) {
				T item = GetTargetCollectionItem(InnerCollection[i]);
				newTarget.Add(item);
			}
			return newTarget;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			UnsubscribeInnerCollectionEvents();
			try {
				RecreateInnerCollection((ObservableCollection<T>)oldValue, (ObservableCollection<T>)newValue);
			} finally {
				SubscribeInnerCollectionEvents();
			}
		}
	}
	public class TimeScaleCollectionPropertyMapper : CollectionPropertyMapperBase<TimeScale, TimeScale> {
		TimelineView view;
		public TimeScaleCollectionPropertyMapper(DependencyProperty property, TimelineView view)
			: base(property, view) {
			this.view = view;
		}
		public TimelineView View { get { return view; } }
		protected override NotificationCollection<TimeScale> InnerCollection { get { return View.InnerScales; } }
		protected TimeScaleCollection ScaleCollection { get { return (TimeScaleCollection)InnerCollection; } }
		protected override void LoadInnerCollectionDefaultsIfEmpty() {
			if (ScaleCollection.Count == 0)
				ScaleCollection.LoadDefaults();
		}
		protected override TimeScale GetTargetCollectionItem(TimeScale innerItem) {
			return innerItem;
		}
		protected override TimeScale GetInnerCollectionItem(TimeScale targetItem) {
			return targetItem;
		}
	}
	#endregion
}
