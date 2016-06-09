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
	using System.Collections.Generic;
	using System.ComponentModel;
	using DevExpress.Data.Filtering;
	sealed class DefaultEndUserFilteringMetricViewModelFactory : IEndUserFilteringMetricViewModelFactory {
		internal static readonly IEndUserFilteringMetricViewModelFactory Instance = new DefaultEndUserFilteringMetricViewModelFactory();
		DefaultEndUserFilteringMetricViewModelFactory() { }
		IDictionary<Type, Func<IEndUserFilteringMetric, IMetricAttributesQuery, IValueViewModel, Type, IEndUserFilteringMetricViewModel>> initializers =
			new Dictionary<Type, Func<IEndUserFilteringMetric, IMetricAttributesQuery, IValueViewModel, Type, IEndUserFilteringMetricViewModel>>() { 
			{ typeof(IRangeMetricAttributes<>), (metric, query, value, valueType) => new EndUserFilteringRangeMetricViewModel(metric, query, value, valueType) },
			{ typeof(ILookupMetricAttributes<>), (metric, query, value, valueType) => new EndUserFilteringLookupMetricViewModel(metric, query, value, valueType) },
			{ typeof(IChoiceMetricAttributes<>), (metric, query, value, valueType) => new EndUserFilteringChoiceMetricViewModel(metric, query, value, valueType) },
			{ typeof(IEnumChoiceMetricAttributes<>), (metric, query, value, valueType) => new EndUserFilteringEnumChoiceMetricViewModel(metric, query, value, valueType) },
		};
		public IEndUserFilteringMetricViewModel Create(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType) {
			Func<IEndUserFilteringMetric, IMetricAttributesQuery, IValueViewModel, Type, IEndUserFilteringMetricViewModel> initializer;
			if(initializers.TryGetValue(metric.AttributesTypeDefinition, out initializer))
				return initializer(metric, query, value, valueType);
			throw new NotSupportedException(metric.AttributesTypeDefinition.ToString());
		}
	}
	public abstract class EndUserFilteringMetricViewModel : IEndUserFilteringMetricViewModel, INotifyPropertyChanged, IDisposable {
		readonly IEndUserFilteringMetric metricCore;
		readonly IMetricAttributesQuery queryCore;
		readonly IValueViewModel valueCore;
		readonly Type valueTypeCore;
		protected EndUserFilteringMetricViewModel(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType) {
			this.metricCore = metric;
			this.queryCore = query;
			this.valueCore = value;
			this.valueTypeCore = valueType;
			Value.Changed += Value_Changed;
			this.filterCriteriaCore = new Lazy<CriteriaOperator>(() => CreateFilterCriteria());
		}
		void IDisposable.Dispose() {
			ResetCriteriaOperator();
			Value.Changed -= Value_Changed;
			Value.Release();
		}
		void Value_Changed(object sender, System.EventArgs e) {
			OnValueChanged();
		}
		internal const string FilterCriteriaNotify = ".FilterCriteria";
		protected virtual void OnValueChanged() {
			ResetCriteriaOperator();
			this.RaisePropertyChanged(() => HasValue);
			this.RaisePropertyChanged(() => FilterCriteria);
			ParentViewModel.RaisePropertyChanged(Metric.Path + FilterCriteriaNotify);
		}
		void ResetCriteriaOperator() {
			if(filterCriteriaCore.IsValueCreated)
				filterCriteriaCore = new Lazy<CriteriaOperator>(() => CreateFilterCriteria());
		}
		WeakReference parentViewModelRef;
		protected object ParentViewModel {
			get { return parentViewModelRef.@Get(r => r.Target); }
			set {
				if(ParentViewModel == value) return;
				this.parentViewModelRef = new WeakReference(value);
				OnParentViewModelChanged();
			}
		}
		protected virtual void OnParentViewModelChanged() {
			Metric.Attributes.UpdateMemberBindings(ParentViewModel, null, Query);
			Value.Initialize(this);
		}
		public IEndUserFilteringMetric Metric {
			get { return metricCore; }
		}
		public IMetricAttributesQuery Query {
			get { return queryCore; }
		} 
		public bool HasValue {
			get { return valueCore.IsModified; }
		}
		public IValueViewModel Value {
			get { return valueCore; }
		}
		public Type ValueType {
			get { return valueTypeCore; }
		}
		Lazy<CriteriaOperator> filterCriteriaCore;
		public CriteriaOperator FilterCriteria {
			get { return filterCriteriaCore.Value; }
		}
		protected virtual CriteriaOperator CreateFilterCriteria() {
			return (Value as IFilterValueViewModel)
				.@Get(fvm => fvm.CreateFilterCriteria());
		}
		public Func<T, bool> GetWhereClause<T>() {
			return (T e) => true; 
		}
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			var handler = (PropertyChangedEventHandler)PropertyChanged;
			if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	public class EndUserFilteringRangeMetricViewModel : EndUserFilteringMetricViewModel {
		public EndUserFilteringRangeMetricViewModel(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType)
			: base(metric, query, value, valueType) {
		}
	}
	public class EndUserFilteringLookupMetricViewModel : EndUserFilteringMetricViewModel {
		public EndUserFilteringLookupMetricViewModel(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType)
			: base(metric, query, value, valueType) {
		}
	}
	public class EndUserFilteringChoiceMetricViewModel : EndUserFilteringMetricViewModel {
		public EndUserFilteringChoiceMetricViewModel(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType)
			: base(metric, query, value, valueType) {
		}
	}
	public class EndUserFilteringEnumChoiceMetricViewModel : EndUserFilteringMetricViewModel {
		public EndUserFilteringEnumChoiceMetricViewModel(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType)
			: base(metric, query, value, valueType) {
		}
	}
}
