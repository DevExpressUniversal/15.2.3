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
	using DevExpress.Data.Filtering;
	public class CollectionValueBox<T> : ValueViewModel, ICollectionValueViewModel<T>, IFilterValueViewModel {
		readonly static IEnumerable<T> UnsetValues = new T[] { };
		readonly static object valuesKey = new object();
		public CollectionValueBox() {
			dataSourceCore = new Lazy<object>(() => null);
			lookupDataSourceCore = new Lazy<IEnumerable<KeyValuePair<object, string>>>(() => null);
		}
		public virtual IEnumerable<T> Values {
			get { return GetValue<IEnumerable<T>>(valuesKey, UnsetValues); }
			set {
				if(TrySetValues(valuesKey, value, () => Values))
					OnValuesChanged();
			}
		}
		protected void OnValuesChanged() {
			SetIsModified();
		}
		protected sealed override void ResetCore() {
			ResetValue(valuesKey, () => Values);
		}
		protected sealed override bool CanResetCore() {
			return HasValue(valuesKey);
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			(MetricAttributes as INotifyPropertyChanged)
				.@Do(npc => npc.PropertyChanged += OnMetricAttributes_PropertyChanged);
			ResetDataSourceAndLoadCommand();
		}
		void OnMetricAttributes_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "DataSource" || e.PropertyName == "Top" || e.PropertyName == "MaxCount") {
				ResetDataSourceAndLoadCommand();
				ResetValue(valuesKey, () => Values);
			}
		}
		void ResetDataSourceAndLoadCommand() {
			dataSourceCore = new Lazy<object>(() => QueryHelper.TakeDistinct<T>(MetricAttributes.DataSource, Top ?? MaxCount));
			UpdateLookupDataSource();
			NotifyDataSourceChanged();
			dataSourceLoaded = CanLoadData() ? new Nullable<bool>(false) : null;
			UpdateLoadCommands();
		}
		bool? dataSourceLoaded;
		public void LoadMore() {
			bool areAllValuesSelected = Values.Count() == QueryHelper.CountDistinct<T>(DataSource);
			dataSourceCore = new Lazy<object>(() => QueryHelper.TakeDistinct<T>(MetricAttributes.DataSource, MaxCount));
			UpdateLookupDataSource();
			dataSourceLoaded = true;
			NotifyDataSourceChanged();
			if(areAllValuesSelected)
				Values = Internal.QueryHelper.GetValues<T>(DataSource, ValueMember);
			UpdateLoadCommands();
		}
		public void LoadFewer() {
			bool areAllValuesSelected = Values.Count() == QueryHelper.CountDistinct<T>(DataSource);
			dataSourceCore = new Lazy<object>(() => QueryHelper.TakeDistinct<T>(MetricAttributes.DataSource, Top ?? MaxCount));
			UpdateLookupDataSource();
			dataSourceLoaded = false;
			NotifyDataSourceChanged();
			if(areAllValuesSelected)
				Values = Internal.QueryHelper.GetValues<T>(DataSource, ValueMember);
			UpdateLoadCommands();
		}
		void NotifyDataSourceChanged() {
			this.RaisePropertyChanged(() => DataSource);
			this.RaisePropertyChanged(() => LookupDataSource);
		}
		void UpdateLookupDataSource() {
			lookupDataSourceCore = new Lazy<IEnumerable<KeyValuePair<object, string>>>(() => CreateLookupDataSource(dataSourceCore.Value as IEnumerable));
		}
		IEnumerable<KeyValuePair<object, string>> CreateLookupDataSource(IEnumerable dataSource) {
			if(dataSource == null)
				yield break;
			foreach(var item in dataSource) {
				object value = item.@Get(x =>
					x.GetMemberValue(ValueMember) ?? item);
				string displayValue = item.@Get(x =>
					x.GetMemberValue(DisplayMember) as string ?? item.ToString(), NullName);
				if(value == null && !TypeHelper.AllowNull(typeof(T)))
					continue;
				yield return new KeyValuePair<object, string>(value, displayValue);
			}
		}
		bool CanLoadData() {
			return IsInitialized && Top.HasValue && (MetricAttributes.DataSource != null);
		}
		public bool CanLoadMore() {
			return CanLoadData() && dataSourceLoaded.HasValue && !dataSourceLoaded.Value;
		}
		public bool IsLoadMoreAvailable {
			get { return CanLoadData() && dataSourceLoaded.HasValue && !dataSourceLoaded.Value; }
		}
		public bool CanLoadFewer() {
			return CanLoadData() && dataSourceLoaded.HasValue && dataSourceLoaded.Value;
		}
		public bool IsLoadFewerAvailable {
			get { return CanLoadData() && dataSourceLoaded.HasValue && dataSourceLoaded.Value; }
		}
		void UpdateLoadCommands() {
			this.RaisePropertyChanged(() => IsLoadMoreAvailable);
			this.RaiseCanExecuteChanged(() => LoadMore());
			this.RaisePropertyChanged(() => IsLoadFewerAvailable);
			this.RaiseCanExecuteChanged(() => LoadFewer());
		}
		[Browsable(false)]
		Lazy<object> dataSourceCore;
		public object DataSource {
			get { return dataSourceCore.Value; }
		}
		[Browsable(false)]
		Lazy<IEnumerable<KeyValuePair<object, string>>> lookupDataSourceCore;
		public IEnumerable<KeyValuePair<object, string>> LookupDataSource {
			get { return lookupDataSourceCore.Value; }
		}
		protected ILookupMetricAttributes<T> MetricAttributes {
			get { return (ILookupMetricAttributes<T>)MetricViewModel.Metric.Attributes; }
		}
		[Browsable(false)]
		public string ValueMember {
			get { return MetricAttributes.ValueMember; }
		}
		[Browsable(false)]
		public string DisplayMember {
			get { return MetricAttributes.DisplayMember; }
		}
		[Browsable(false)]
		public int? Top {
			get { return MetricAttributes.Top; }
		}
		[Browsable(false)]
		public int? MaxCount {
			get { return MetricAttributes.MaxCount; }
		}
		[Browsable(false)]
		public bool UseFlags {
			get { return MetricAttributes.UseFlags; }
		}
		[Browsable(false)]
		public bool UseSelectAll {
			get { return MetricAttributes.UseSelectAll; }
		}
		[Browsable(false)]
		public string SelectAllName {
			get { return MetricAttributes.SelectAllName; }
		}
		[Browsable(false)]
		public string NullName {
			get { return MetricAttributes.NullName; }
		}
		CriteriaOperator IFilterValueViewModel.CreateFilterCriteria() {
			if(!IsModified)
				return null;
			if(!Values.Any())
				return null;
			if(MetricAttributes.DataSource != null && !MaxCount.HasValue) {
				if((dataSourceLoaded.HasValue && dataSourceLoaded.Value) || !Top.HasValue)
					if(Values.Count() == QueryHelper.CountDistinct<T>(MetricAttributes.DataSource))
						return null;
			}
			var prop = new OperandProperty(MetricViewModel.Metric.Path);
			var isNull = new UnaryOperator(UnaryOperatorType.IsNull, prop);
			bool hasNull = Values.Any(v => Equals(v, null));
			var values = Values
				.Where(v => !Equals(v, null))
				.Select(v => new OperandValue(v));
			CriteriaOperator op = (values.Count() == 1) ?
				(CriteriaOperator)new BinaryOperator(prop, values.First(), BinaryOperatorType.Equal) :
				(CriteriaOperator)new InOperator(prop, values);
			return hasNull ? GroupOperator.Or(op, isNull) : op;
		}
	}
}
