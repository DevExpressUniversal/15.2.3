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
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Globalization;
using DevExpress.Xpf.Core.ServerMode;
using System.Windows.Controls;
#if SILVERLIGHT
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PLinqInstantFeedbackDataSource = DevExpress.Xpf.Core.ServerMode.LinqToObjectsInstantFeedbackDataSource;
using TypeConverter = System.ComponentModel.TypeConverter;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Core.DataSources {
	public abstract class SimpleDataSourceBase : DXDesignTimeControl, IDesignDataUpdater {
		protected SimpleDataSourceBase() {
			DesignDataManager.RegisterUpdater(GetType(), new StandartDesignDataUpdater());
		}
		object CreateDesignTimeDataSource() {
			if(!CanUpdateFromDesignData()) return null;
			return CreateDesignTimeDataSourceCore();
		}
		protected internal abstract object DataCore { get; set; }
		protected abstract object CreateDesignTimeDataSourceCore();
		protected abstract object UpdateDataCore();
		protected virtual bool CanUpdateFromDesignData() {
			return DesignData != null && DesignData.RowCount > 0;
		}
		protected void UpdateData() {
			if(DesignerProperties.GetIsInDesignMode(this)) {
				DataCore = CreateDesignTimeDataSource();
				return;
			}
			DataCore = UpdateDataCore();
		}
		public IDesignDataSettings DesignData {
			get { return DesignDataManager.GetDesignData(this); }
			set { DesignDataManager.SetDesignData(this, value); }
		}
		static ResourceDictionary GetResourceDictionary() {
			string resourceUri = String.Format("/{0};component/DataSources/Resources/Resources.xaml", AssemblyInfo.SRAssemblyXpfCore);
			return new ResourceDictionary() { Source = new Uri(resourceUri, UriKind.Relative) };
		}
		protected override ControlTemplate CreateControlTemplate() {
			return (ControlTemplate)GetResourceDictionary()["DataSourceTemplate"];
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.DataSource.png";
		}
		#region IDesignDataUpdater members
		void IDesignDataUpdater.UpdateDesignData(DependencyObject element) {
			UpdateData();
		}
		#endregion
	}
	public abstract class DataSourceBase : SimpleDataSourceBase, IDataSource {
		#region static
		public static readonly DependencyProperty ContextTypeProperty;
		public static readonly DependencyProperty PathProperty;
		static DataSourceBase() {
			Type ownerType = typeof(DataSourceBase);
			ContextTypeProperty = DependencyPropertyManager.Register("ContextType", typeof(Type), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((DataSourceBase)d).UpdateData()));
			PathProperty = DependencyPropertyManager.Register("Path", typeof(string), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((DataSourceBase)d).OnPathChanged()));
		}
		#endregion
		#region fields
		DataSourceStrategyBase strategy;
		readonly BaseDataSourceStrategySelector strategySelector;
		private object contextInstance;
		#endregion
		protected DataSourceStrategyBase Strategy { get { return this.strategy; } }
		public DataSourceBase() {
			Initialize();
			this.strategy = CreateDataSourceStrategy();
			this.strategySelector = CreateDataSourceStrategySelector();
		}
		protected virtual DataSourceStrategyBase CreateDataSourceStrategy() {
			return new DataSourceStrategyBase(this);
		}
		protected virtual BaseDataSourceStrategySelector CreateDataSourceStrategySelector() {
			return new BaseDataSourceStrategySelector();
		}
		protected virtual object CreateData(object value) {
			return Strategy.CreateData(value);
		}
		protected override object CreateDesignTimeDataSourceCore() {
			return new BaseGridDesignTimeDataSource(Strategy.GetDataObjectType(), DesignData.RowCount, DesignData.UseDistinctValues, null, null, Strategy.GetDesignTimeProperties());
		}
		protected virtual void Initialize() { }
		protected override bool CanUpdateFromDesignData() {
			return base.CanUpdateFromDesignData() && Strategy.CanGetDesignData();
		}
		protected override object UpdateDataCore() {
			this.strategy = this.strategySelector.SelectStrategy(this, Strategy);
			if(!Strategy.CanUpdateData()) return null;
			this.contextInstance = Strategy.CreateContextIstance();
			object dataPropertyValue = Strategy.GetDataMemberValue(this.contextInstance);
			return CreateData(dataPropertyValue);
		}
		protected virtual void OnPathChanged() {
			UpdateData();
		}
		#region IDataSource
		public Type ContextType {
			get { return (Type)GetValue(ContextTypeProperty); }
			set { SetValue(ContextTypeProperty, value); }
		}
		object IDataSource.ContextInstance { get { return this.contextInstance; } }
		object IDataSource.Data { get { return DataCore; } }
		public string Path {
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}
		#endregion
#if DEBUGTEST
		internal Type DataObjectType { get { return Strategy.GetDataObjectType(); } }
#endif
	}
	public abstract class EnumerableDataSourceBase : DataSourceBase {
		private static readonly DependencyPropertyKey DataPropertyKey;
		public static readonly DependencyProperty DataProperty;
		static EnumerableDataSourceBase() {
			Type ownerType = typeof(EnumerableDataSourceBase);
			DataPropertyKey = DependencyPropertyManager.RegisterReadOnly("Data", typeof(IEnumerable), ownerType, new FrameworkPropertyMetadata());
			DataProperty = DataPropertyKey.DependencyProperty;
		}
		public IEnumerable Data {
			get { return (IEnumerable)GetValue(DataProperty); }
			protected set { this.SetValue(DataPropertyKey, value); }
		}
		protected internal override object DataCore {
			get { return Data; }
			set { Data = value as IEnumerable; }
		}
	}
	public abstract class ListSourceDataSourceBase : DataSourceBase {
		private static readonly DependencyPropertyKey DataPropertyKey;
		public static readonly DependencyProperty DataProperty;
		static ListSourceDataSourceBase() {
			Type ownerType = typeof(ListSourceDataSourceBase);
			DataPropertyKey = DependencyPropertyManager.RegisterReadOnly("Data", typeof(IListSource), ownerType, new FrameworkPropertyMetadata());
			DataProperty = DataPropertyKey.DependencyProperty;
		}
		public IListSource Data {
			get { return (IListSource)GetValue(DataProperty); }
			protected set { this.SetValue(DataPropertyKey, value); }
		}
		protected internal override object DataCore {
			get { return Data; }
			set { Data = value as IListSource; }
		}
	}
	public class CultureConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				try {
					return new CultureInfo((string)value);
				} catch(Exception) {
					return CultureInfo.CurrentCulture;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public abstract class CollectionViewDataSourceBase : EnumerableDataSourceBase {
		public static readonly DependencyProperty CollectionViewTypeProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty GroupDescriptionsProperty;
#if SL
		public static readonly DependencyProperty PageSizeProperty;
#endif
		public static readonly DependencyProperty SortDescriptionsProperty;
		static CollectionViewDataSourceBase() {
			Type ownerType = typeof(CollectionViewDataSourceBase);
#if SL
			PageSizeProperty = DependencyPropertyManager.Register("PageSize", typeof(int), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((CollectionViewDataSourceBase)d).UpdateView()));
			Type defaultCollectionViewType = typeof(CollectionViewSource);
#else
			Type defaultCollectionViewType = typeof(ListCollectionView);
#endif
			CollectionViewTypeProperty = DependencyPropertyManager.Register("CollectionViewType", typeof(Type), ownerType,
				new FrameworkPropertyMetadata(defaultCollectionViewType, (d, e) => ((CollectionViewDataSourceBase)d).UpdateData()), ValidateCollectionViewType);
			CultureProperty = DependencyPropertyManager.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((CollectionViewDataSourceBase)d).UpdateView()));
			GroupDescriptionsProperty = DependencyPropertyManager.Register("GroupDescriptions", typeof(ObservableCollection<GroupDescription>), ownerType,
				new FrameworkPropertyMetadata(new ObservableCollection<GroupDescription>(), (d, e) => ((CollectionViewDataSourceBase)d).OnDescriptionsChanged(e.OldValue, e.NewValue)));
			SortDescriptionsProperty = DependencyPropertyManager.Register("SortDescriptions", typeof(SortDescriptionCollection), ownerType,
				new FrameworkPropertyMetadata(new SortDescriptionCollection(), (d, e) => ((CollectionViewDataSourceBase)d).OnDescriptionsChanged(e.OldValue, e.NewValue)));
		}
		private static bool ValidateCollectionViewType(object value) {
			Type type = (Type)value;
#if !SL
			return type.GetInterfaces().Contains(typeof(ICollectionView)) && type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Count() > 0;
#else
			return type == typeof(CollectionViewSource) || type == typeof(PagedCollectionView);
#endif
		}
		public CollectionViewDataSourceBase() {
			GroupDescriptions = new ObservableCollection<GroupDescription>();
			SortDescriptions = new SortDescriptionCollection();
			((INotifyCollectionChanged)SortDescriptions).CollectionChanged += OnDescriptionsCollectionChanged;
			((INotifyCollectionChanged)GroupDescriptions).CollectionChanged += OnDescriptionsCollectionChanged;
		}
		public Type CollectionViewType {
			get { return (Type)GetValue(CollectionViewTypeProperty); }
			set { SetValue(CollectionViewTypeProperty, value); }
		}
#if SL
		public int PageSize {
			get { return (int)GetValue(PageSizeProperty); }
			set { SetValue(PageSizeProperty, value); }
		}
#endif
		[TypeConverter(typeof(CultureConverter))]
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public ObservableCollection<GroupDescription> GroupDescriptions {
			get { return (ObservableCollection<GroupDescription>)GetValue(GroupDescriptionsProperty); }
			set { SetValue(GroupDescriptionsProperty, value); }
		}
		public SortDescriptionCollection SortDescriptions {
			get { return (SortDescriptionCollection)GetValue(SortDescriptionsProperty); }
			set { SetValue(SortDescriptionsProperty, value); }
		}
		private static void Copy<T>(IEnumerable source, IList<T> target, bool isValid) {
			if(source == null || target == null || !isValid) return;
			target.Clear();
			foreach(T item in source)
				target.Add(item);
		}
		protected override object CreateDesignTimeDataSourceCore() {
			IEnumerable baseSource = new CollectionViewDesignTimeDataSource(Strategy.GetDataObjectType(), DesignData.RowCount, DesignData.UseDistinctValues);
			CollectionViewSource cvs = new CollectionViewSource() { Source = baseSource };
			UpdateView(cvs.View);
			return cvs.View;
		}
		static IList ToIList(IEnumerable target, Type elementType) {
			MethodInfo toList = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(new Type[] { elementType });
			return (IList)toList.Invoke(null, new object[] { target });
		}
		protected override object CreateData(object value) {
			if(value == null) return null;
			IEnumerable data = (IEnumerable)Strategy.CreateData(value);
			IEnumerable view;
#if !SL
			CollectionViewSource source = new CollectionViewSource();
			ISupportInitialize sourceSupportInitialize = (ISupportInitialize)source;
			sourceSupportInitialize.BeginInit();
			source.CollectionViewType = CollectionViewType;
			data = data is IList ? data : ToIList(data, Strategy.GetDataObjectType());
			source.Source = data;
			sourceSupportInitialize.EndInit();
			view = source.View;
#else
			IList listData = data.ToIList(Strategy.GetDataObjectType());
			view = CollectionViewType == typeof(PagedCollectionView) ? new PagedCollectionView(listData) : new CollectionViewSource() { Source = listData }.View;
#endif
			UpdateView(view);
			return view;
		}
		private void OnDescriptionsChanged(object oldValue, object newValue) {
			if(oldValue != null)
				((INotifyCollectionChanged)oldValue).CollectionChanged -= OnDescriptionsCollectionChanged;
			((INotifyCollectionChanged)newValue).CollectionChanged += OnDescriptionsCollectionChanged;
			UpdateView();
		}
		private void OnDescriptionsCollectionChanged(object d, NotifyCollectionChangedEventArgs e) {
			UpdateView();
		}
		private void UpdateView() {
			UpdateView(Data);
		}
		private void UpdateView(object collectionView) {
			if(collectionView == null || !(collectionView is ICollectionView)) return;
			ICollectionView data = (ICollectionView)collectionView;
			Copy<SortDescription>(SortDescriptions, data.SortDescriptions, data.CanSort);
			Copy<GroupDescription>(GroupDescriptions, data.GroupDescriptions, data.CanGroup);
			if(Culture != null)
				data.Culture = Culture;
#if SL
			if(collectionView is IPagedCollectionView)
				((IPagedCollectionView)collectionView).PageSize = PageSize;
#endif
		}
	}
	public abstract class SimpleDataSource : EnumerableDataSourceBase { }
#if !SL
	public abstract class PLinqServerModeDataSourceBase : ListSourceDataSourceBase {
		public static readonly DependencyProperty DefaultSortingProperty;
		static PLinqServerModeDataSourceBase() {
			Type ownerType = typeof(PLinqServerModeDataSourceBase);
			DefaultSortingProperty = DependencyPropertyManager.Register("DefaultSorting", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, (d, e) => ((PLinqServerModeDataSourceBase)d).UpdateData()));
		}
		PLinqServerModeDataSource pLinqSource;
		protected override void Initialize() {
			base.Initialize();
			this.pLinqSource = new PLinqServerModeDataSource();
			this.pLinqSource.SetBinding(PLinqServerModeDataSource.DefaultSortingProperty, new Binding("DefaultSorting") { Source = this });
		}
		protected override object CreateData(object value) {
			this.pLinqSource.ElementType = Strategy.GetDataObjectType();
			this.pLinqSource.ItemsSource = Strategy.CreateData(value) as IEnumerable;
			return this.pLinqSource.Data;
		}
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
#if DEBUGTEST
		internal PLinqServerModeDataSource SourceInternal { get { return this.pLinqSource; } }
#endif
	}
#endif
	public abstract class PLinqInstantFeedbackDataSourceBase : ListSourceDataSourceBase {
		public static readonly DependencyProperty DefaultSortingProperty;
		static PLinqInstantFeedbackDataSourceBase() {
			Type ownerType = typeof(PLinqInstantFeedbackDataSourceBase);
			DefaultSortingProperty = DependencyPropertyManager.Register("DefaultSorting", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, (d, e) => ((PLinqInstantFeedbackDataSourceBase)d).UpdateData()));
		}
		PLinqInstantFeedbackDataSource pLinqSource;
		protected override void Initialize() {
			base.Initialize();
			this.pLinqSource = new PLinqInstantFeedbackDataSource();
			this.pLinqSource.SetBinding(PLinqInstantFeedbackDataSource.DefaultSortingProperty, new Binding("DefaultSorting") { Source = this });
		}
		protected override object CreateData(object value) {
			this.pLinqSource.ItemsSource = Strategy.CreateData(value) as IEnumerable;
			return this.pLinqSource.Data;
		}
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
#if DEBUGTEST
		internal PLinqInstantFeedbackDataSource SourceInternal { get { return this.pLinqSource; } }
#endif
	}
	public abstract class ItemsSourceDataSourceBase : SimpleDataSourceBase {
		public static readonly DependencyProperty ItemsSourceProperty;
		static ItemsSourceDataSourceBase() {
			Type ownerType = typeof(ItemsSourceDataSourceBase);
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(IEnumerable), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((ItemsSourceDataSourceBase)d).UpdateData()));
		}
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
	}
	public abstract class PLinqDataSourceBase : ItemsSourceDataSourceBase {
		private static readonly DependencyPropertyKey DataPropertyKey;
		public static readonly DependencyProperty DataProperty;
		static PLinqDataSourceBase() {
			Type ownerType = typeof(PLinqDataSourceBase);
			DataPropertyKey = DependencyPropertyManager.RegisterReadOnly("Data", typeof(IListSource), ownerType, new FrameworkPropertyMetadata());
			DataProperty = DataPropertyKey.DependencyProperty;
		}
		public IListSource Data {
			get { return (IListSource)GetValue(DataProperty); }
			protected set { this.SetValue(DataPropertyKey, value); }
		}
		protected internal override object DataCore {
			get { return Data; }
			set { Data = value as IListSource; }
		}
		protected abstract Type GetDataObjectType();
		protected override object CreateDesignTimeDataSourceCore() {
			Type dataObjectType = GetDataObjectType();
			return dataObjectType != null ? new BaseGridDesignTimeDataSource(dataObjectType, DesignData.RowCount, DesignData.UseDistinctValues, DesignData.FlattenHierarchy) : null;
		}
	}
	static class DataSourceHelper {
		public static Type ExtractEnumerableType(IEnumerable obj) {
			if(obj == null) return null;
			IEnumerable<Type> interfaces = obj.GetType().GetInterfaces().Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if(interfaces.Count() == 0) return null;
			return interfaces.ElementAt(0).GetGenericArguments()[0];
		}
	}
}
