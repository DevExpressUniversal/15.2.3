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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	public abstract class ValueViewModel : IValueViewModel {
		[Browsable(false)]
		public virtual bool IsModified {
			get;
			protected set;
		}
		protected void SetIsModified() {
			if(IsInitialized && lockSetIsModified > 0) return;
			IsModified = true;
			RaiseChanged();
		}
		protected void ResetIsModified() {
			if(IsInitialized && lockSetIsModified > 0) return;
			IsModified = false;
			RaiseChanged();
		}
		protected void OnIsModifiedChanged() {
			this.RaiseCanExecuteChanged(() => Reset());
		}
		protected IEndUserFilteringMetricViewModel MetricViewModel {
			get;
			private set;
		}
		protected bool IsInitialized {
			get { return MetricViewModel != null; }
		}
		void IValueViewModel.Initialize(IEndUserFilteringMetricViewModel metricViewModel) {
			MetricViewModel = metricViewModel;
			if(IsInitialized) OnInitialized();
		}
		void IValueViewModel.Release() {
			if(IsInitialized)
				OnReleasing();
			MetricViewModel = null;
		}
		protected virtual void OnInitialized() {
			SubscribeAttributes(MetricViewModel.Metric.Attributes);
			lockSetIsModified++;
			ResetCore();
			lockSetIsModified--;
			IsModified = false;
			this.RaiseCanExecuteChanged(() => Reset());
		}
		protected virtual void OnReleasing() {
			UnsubscribeAttributes(MetricViewModel.Metric.Attributes);
		}
		void SubscribeAttributes(IMetricAttributes attributes) {
			(attributes as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged += Attributes_PropertyChanged);
		}
		void UnsubscribeAttributes(IMetricAttributes attributes) {
			(attributes as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged -= Attributes_PropertyChanged);
		}
		void Attributes_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			lockSetIsModified++;
			OnMetricAttributesChanged(e.PropertyName);
			lockSetIsModified--;
			IsModified = CanResetCore();
			if(IsModified)
				RaiseChanged();
		}
		protected virtual void OnMetricAttributesChanged(string propertyName) {
			this.RaisePropertyChanged(propertyName);
		}
		int lockSetIsModified = 0;
		[Browsable(false)]
		public void Reset() {
			if(!IsInitialized) return;
			lockSetIsModified++;
			ResetCore();
			lockSetIsModified--;
			ResetIsModified();
		}
		public bool CanReset() {
			return IsInitialized && IsModified && CanResetCore();
		}
		protected abstract void ResetCore();
		protected abstract bool CanResetCore();
		#region Changed
		EventHandler changedCore;
		public event EventHandler Changed {
			add { changedCore += value; }
			remove { changedCore -= value; }
		}
		protected void RaiseChanged() {
			var handler = (EventHandler)changedCore;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		#endregion Changed
		#region Values
		Hashtable valueHash = new Hashtable();
		void SetValueCore<T>(object valueKey, object value, Expression<Func<T>> selector) {
			valueHash[valueKey] = value;
			this.RaisePropertyChanged(selector);
		}
		protected T GetValue<T>(object valueKey, T defaultValue) where T : class {
			return valueHash[valueKey] as T ?? defaultValue;
		}
		protected T? GetValue<T>(object valueKey) where T : struct {
			if(!valueHash.ContainsKey(valueKey))
				return null;
			object value = valueHash[valueKey];
			if(ReferenceEquals(value, null))
				return null;
			return new Nullable<T>((T)value);
		}
		protected bool TrySetValue<T>(object valueKey, object value, Expression<Func<T>> selector) {
			if(Equals(valueHash[valueKey], value))
				return false;
			SetValueCore(valueKey, value, selector);
			return true;
		}
		protected bool TrySetValues<T>(object valuesKey, IEnumerable<T> values, Expression<Func<IEnumerable<T>>> selector) {
			if(Equals(valueHash[valuesKey], values))
				return false;
			if(!ReferenceEquals(values, null) && !ReferenceEquals(valueHash[valuesKey], null)) {
				if(values.SequenceEqual(valueHash[valuesKey] as IEnumerable<T>))
					return false;
			}
			SetValueCore(valuesKey, values, selector);
			return true;
		}
		protected bool ResetValue<T>(object valueKey, Expression<Func<T>> selector) {
			if(!valueHash.ContainsKey(valueKey))
				return false;
			valueHash.Remove(valueKey);
			this.RaisePropertyChanged(selector);
			return true;
		}
		protected bool HasValue(object valueKey) {
			return valueHash.ContainsKey(valueKey);
		}
		#endregion Values
	}
}
