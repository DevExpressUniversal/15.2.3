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
using System.ComponentModel;
using System.Windows;
using DevExpress.Map.Native;
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class CustomizeMapItemEventArgs : EventArgs {
		readonly MapItem mapItem;
		readonly object sourceObject;
		public MapItem MapItem { get { return mapItem; } }
		public object SourceObject { get { return sourceObject; } }
		public CustomizeMapItemEventArgs(MapItem mapItem, object sourceObject) {
			this.mapItem = mapItem;
			this.sourceObject = sourceObject;
		}
	}
	public delegate void CustomizeMapItemEventHandler(object sender, CustomizeMapItemEventArgs e);
	[DataAccessMetadata("TypedDataSet,EntityFramework,LinqToSql,IEnumerable", DataSourceProperty = "DataSource", Platform="WPF", DataMemberProperty="DataMember")]
	public abstract class DataSourceAdapterBase : CoordinateSystemDataAdapterBase, IWeakEventListener, IMapItemSettingsListener {
		public static readonly DependencyProperty DataSourceProperty = DependencyPropertyManager.Register("DataSource",
			typeof(object), typeof(DataSourceAdapterBase), new PropertyMetadata(null, DataSourcePropertyChanged));
		public static readonly DependencyProperty DataMemberProperty = DependencyPropertyManager.Register("DataMember",
			typeof(string), typeof(DataSourceAdapterBase), new PropertyMetadata(null));
		static readonly DependencyPropertyKey AttributeMappingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("AttributeMappings",
			typeof(MapItemAttributeMappingCollection), typeof(DataSourceAdapterBase), new PropertyMetadata(null));
		public static readonly DependencyProperty AttributeMappingsProperty = AttributeMappingsPropertyKey.DependencyProperty;
		static void DataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataSourceAdapterBase adapter = d as DataSourceAdapterBase;
			if (adapter != null)
				adapter.UpdateData();
		}
		protected static void ItemSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataSourceAdapterBase dataAdapter = d as DataSourceAdapterBase;
			if (dataAdapter != null && e.NewValue != e.OldValue) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, dataAdapter);
				dataAdapter.LoadDataInternal();
			}
		}
		protected static void MappingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataSourceAdapterBase dataAdapter = d as DataSourceAdapterBase;
			if (dataAdapter != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as INotifyPropertyChanged, e.NewValue as INotifyPropertyChanged, dataAdapter);
				dataAdapter.OnMappingsChanged();
			}
		}
		[Category(Categories.Data)]
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		[Category(Categories.Data)]
		public string DataMember {
			get { return (string)GetValue(DataMemberProperty); }
			set { SetValue(DataMemberProperty, value); }
		}
		[Category(Categories.Data)]
		public MapItemAttributeMappingCollection AttributeMappings {
			get { return (MapItemAttributeMappingCollection)GetValue(AttributeMappingsProperty); }
		}
		readonly MapVectorItemCollection mapItems;
		readonly MapDataController dataController;
		protected internal abstract MapItemSettingsBase ActualItemSettings { get; }
		protected internal abstract MapItemMappingInfoBase DataMappings { get; }
		protected internal override MapVectorItemCollection ItemsCollection { get { return mapItems; } }
		protected internal MapDataController DataController { get { return dataController; } }
		protected internal virtual bool CanUpdateItemsOnly { get { return true; } }
		public event CustomizeMapItemEventHandler CustomizeMapItem;
		public DataSourceAdapterBase() {
			this.SetValue(AttributeMappingsPropertyKey, new MapItemAttributeMappingCollection());
			mapItems = new MapVectorItemCollection();
			this.dataController = new MapDataController(this);
		}
		void OnMappingsChanged() {
			DataController.OnMappingsChanged();
		}
		void UpdateItemsProperty(DependencyProperty property, object value) {
			foreach(MapItem item in ItemsCollection) {
				item.SetValue(property, value);
				RaiseCustomizeMapItem(item, item.Tag);
			}
		}
		void ApplyItemsSource() {
			foreach(MapItem item in ItemsCollection)
				ActualItemSettings.ApplySource(item, item.Tag);
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if (sender is MapItemMappingInfoBase) {
					DataController.OnMappingsChanged();
					success = true;
				}
			}
			return success;
		}
		#endregion
		#region IMapItemSettingsListener implementation
		void IMapItemSettingsListener.OnPropertyChanged(DependencyProperty property, object value) {
			UpdateItemsProperty(property, value);
		}
		void IMapItemSettingsListener.OnSourceChanged() {
			ApplyItemsSource();
		}
		#endregion
		protected void UpdateData() {
			if ((DataSource == null) && !System.Windows.Data.BindingOperations.IsDataBound(this, DataSourceProperty))
				ItemsCollection.Clear();
			DataController.DataSource = DataSource;
		}
		public override object GetItemSourceObject(MapItem item) {
			return DataSource != null ? DataController.GetItemSourceObject(item) : item;
		}
		protected internal virtual void OnDataLoaded() {
			if(Layer != null)
				Layer.OnDataLoaded();
		}
		protected internal virtual void FillActualMappings(MappingDictionary mappings) {
			if (DataMappings != null)
				DataMappings.FillActualMappings(mappings, SourceCoordinateSystem);
		}
		protected internal void RaiseCustomizeMapItem(MapItem mapItem, object sourceObject) {
			if (CustomizeMapItem != null)
				CustomizeMapItem(this, new CustomizeMapItemEventArgs(mapItem, sourceObject));
		}
		internal MapItem GetItemByRowIndex(int rowIndex) {
			return null;
		}
		protected internal abstract IMapDataEnumerator CreateDataEnumerator(MapDataController controller);
		protected internal virtual void Aggregate(MapDataController dataController) {
		}
		protected override void LoadDataCore() {
			DataController.LoadData();
		}
		protected internal void Clear() {
			ItemsCollection.Clear();
		}
	}
}
