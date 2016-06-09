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
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Linq;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Collections;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Core.DataSources {
	public class DesignDataSettings : IDesignDataSettings {
		public bool FlattenHierarchy { get; set; }
		public int RowCount { get; set; }
		public bool UseDistinctValues { get; set; }
		public Type DataObjectType { get; set; }
	}
	public interface IDesignDataSettings {
		bool FlattenHierarchy { get; set; }
		int RowCount { get; set; }
		bool UseDistinctValues { get; set; }
		Type DataObjectType { get; set; }
	}
	public class DesignDataSettingsExtension : MarkupExtension, IDesignDataSettings {
		public bool FlattenHierarchy { get; set; }
		public int RowCount { get; set; }
		public bool UseDistinctValues { get; set; }
		public Type DataObjectType { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new DesignDataSettings() {
				RowCount = this.RowCount,
				UseDistinctValues = this.UseDistinctValues,
				DataObjectType = this.DataObjectType,
				FlattenHierarchy = this.FlattenHierarchy
			};
		}
	}
	public class ListenerPropertyChangedEventArgs : EventArgs {
		readonly DependencyObject owner;
		readonly string path;
		public DependencyObject Owner { get { return this.owner; } }
		public string Path { get { return this.path; } }
		public ListenerPropertyChangedEventArgs(DependencyObject owner, string path) {
			this.owner = owner;
			this.path = path;
		}
	}
	public class DependencyPropertyChangedListener : FrameworkElement {
		bool shouldRaiseEvent;
		readonly Dictionary<DependencyObject, List<DependencyProperty>> propsStorage = new Dictionary<DependencyObject, List<DependencyProperty>>();
		readonly Dictionary<DependencyProperty, DependencyObject> storage = new Dictionary<DependencyProperty, DependencyObject>();
		public event EventHandler<ListenerPropertyChangedEventArgs> DependencyPropertyChanged;
		public void Register(DependencyObject source, Binding binding) {
			DependencyProperty property = DependencyPropertyManager.Register(Guid.NewGuid().ToString(), typeof(object), typeof(DependencyPropertyChangedListener),
				new FrameworkPropertyMetadata(null, (d, e) => ((DependencyPropertyChangedListener)d).OnDependencyPropertyChanged(e)));
			if(!this.propsStorage.ContainsKey(source)) this.propsStorage[source] = new List<DependencyProperty>();
			this.storage.Add(property, source);
			this.propsStorage[source].Add(property);
			this.shouldRaiseEvent = false;
			BindingOperations.SetBinding(this, property, binding);
			this.shouldRaiseEvent = true;
		}
		public void Unregister(DependencyObject source) {
			if(!this.propsStorage.ContainsKey(source)) return;
			ClearProperties(source);
			this.propsStorage.Remove(source);
		}
		public void UnregisterAll() {
			foreach(DependencyObject source in this.propsStorage.Keys)
				ClearProperties(source);
			this.propsStorage.Clear();
			this.storage.Clear();
		}
		public bool IsRegistered(DependencyObject source) {
			return this.propsStorage.ContainsKey(source);
		}
		private void ClearProperties(DependencyObject source) {
			foreach(DependencyProperty prop in this.propsStorage[source]) {
				ClearValue(prop);
				this.storage.Remove(prop);
			}
		}
		void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(!this.shouldRaiseEvent) return;
			this.shouldRaiseEvent = false;
			RaiseDependencyPropertyChanged(new ListenerPropertyChangedEventArgs(this.storage[e.Property], GetBindingExpression(e.Property).ParentBinding.Path.Path));
			this.shouldRaiseEvent = true;
		}
		void RaiseDependencyPropertyChanged(ListenerPropertyChangedEventArgs args) {
			if(this.DependencyPropertyChanged != null)
				DependencyPropertyChanged(this, args);
		}
	}
	class CollectionViewSourceDesignDataManager : IDesignDataUpdater {
		readonly DependencyPropertyChangedListener listener = new DependencyPropertyChangedListener();
		void OnCollectionViewSourceChanged(object sender, ListenerPropertyChangedEventArgs e) {
			UpdateSubscriptions(e.Owner);
			if(!DesignerProperties.GetIsInDesignMode(e.Owner)) return;
			UpdateCollectionViewDesignData((CollectionViewSource)e.Owner);
		}
		void UpdateSubscriptions(DependencyObject d) {
			listener.DependencyPropertyChanged -= OnCollectionViewSourceChanged;
			if(!DesignerProperties.GetIsInDesignMode(d))
				listener.UnregisterAll();
			else
				listener.DependencyPropertyChanged += OnCollectionViewSourceChanged;
		}
		void UpdateCollectionViewDesignData(CollectionViewSource collectionView) {
			UpdateSubscriptions(collectionView);
			IDesignDataSettings settings = DesignDataManager.GetDesignData(collectionView);
			if(settings == null || !DesignerProperties.GetIsInDesignMode(collectionView)) return;
			if(!listener.IsRegistered(collectionView))
				listener.Register(collectionView, new Binding() { Source = collectionView });
			Type dataObjectType = settings.DataObjectType ?? DataSourceHelper.ExtractEnumerableType(collectionView.Source as IEnumerable);
			if(dataObjectType == null) return;
			ISupportInitialize supportInitialize = (ISupportInitialize)collectionView;
			supportInitialize.BeginInit();
			collectionView.Source = new CollectionViewDesignTimeDataSource(dataObjectType, settings.RowCount, settings.UseDistinctValues, settings.FlattenHierarchy);
			supportInitialize.EndInit();
		}
		public void UpdateDesignData(DependencyObject element) {
			CollectionViewSource collectionView = element as CollectionViewSource;
			if(collectionView == null) return;
			UpdateCollectionViewDesignData(collectionView);
		}
	}
	class StandartDesignDataUpdater : IDesignDataUpdater {
		public void UpdateDesignData(DependencyObject element) {
			IDesignDataUpdater dataSource = element as IDesignDataUpdater;
			if(dataSource == null) return;
			dataSource.UpdateDesignData(element);
		}
	}
#if SL
	public class DomainDataSourceDesignDataUpdater : IDesignDataUpdater {
		readonly DependencyPropertyChangedListener listener = new DependencyPropertyChangedListener();
		void UpdateSubscription(DependencyObject domainDataSource) {
			this.listener.DependencyPropertyChanged -= OnDomainDataSourceChanged;
			if(!DesignerProperties.GetIsInDesignMode(domainDataSource)) {
				this.listener.UnregisterAll();
				return;
			}
			if(!this.listener.IsRegistered(domainDataSource)) {
				this.listener.Register(domainDataSource, new Binding("QueryName") { Source = domainDataSource });
				this.listener.Register(domainDataSource, new Binding("DomainContext") { Source = domainDataSource });
			}
			this.listener.DependencyPropertyChanged += OnDomainDataSourceChanged;
		}
		public void UpdateDesignData(DependencyObject element) {
			if(element.GetType().FullName != "System.Windows.Controls.DomainDataSource") return;
			UpdateDesignDataCore(element);
		}
		void UpdateDesignDataCore(DependencyObject dds) {
			UpdateSubscription(dds);
			DesignDataSettings settings = DesignDataManager.GetDesignData(dds);
			string queryName = dds.GetType().GetProperty("QueryName").GetValue(dds, null) as string;
			object domainContext = dds.GetType().GetProperty("DomainContext").GetValue(dds, null);
			if(settings == null || settings.RowCount == 0 || string.IsNullOrEmpty(queryName) || domainContext == null || !DesignerProperties.GetIsInDesignMode(dds)) {
				SetDomainDataSourceDesignData(dds, null);
				return;
			}
			Type dataObjectType = domainContext.GetType().GetMethod(queryName).ReturnType.GetGenericArguments()[0];
			BaseGridDesignTimeDataSource source = new BaseGridDesignTimeDataSource(dataObjectType, settings.RowCount, settings.UseDistinctValues);
			SetDomainDataSourceDesignData(dds, source);
		}
		static void SetDomainDataSourceDesignData(DependencyObject dds, BaseGridDesignTimeDataSource source) {
			DependencyProperty designDataProperty = (DependencyProperty)dds.GetType().GetField("DesignDataProperty").GetValue(dds);
			dds.SetValue(designDataProperty, source);
		}
		void OnDomainDataSourceChanged(object sender, ListenerPropertyChangedEventArgs e) {
			UpdateDesignDataCore(e.Owner);
		}
	}
#endif
	public class DesignDataManager : DependencyObject {
		public static readonly DependencyProperty DesignDataProperty;
		static readonly Dictionary<Type, IDesignDataUpdater> updaters = new Dictionary<Type, IDesignDataUpdater>();
		static DesignDataManager() {
			Type ownerType = typeof(DesignDataManager);
			DesignDataProperty = DependencyPropertyManager.RegisterAttached("DesignData", typeof(IDesignDataSettings), ownerType, new FrameworkPropertyMetadata(OnDesignDataChanged));
			updaters[typeof(CollectionViewSource)] = new CollectionViewSourceDesignDataManager();
			RegisterDomainDataSourceUpdater();
		}
		static void RegisterDomainDataSourceUpdater() {
#if SL
			Type ddsType = Type.GetType("System.Windows.Controls.DomainDataSource, System.Windows.Controls.DomainServices");
			if(ddsType == null) return;
			updaters[ddsType] = new DomainDataSourceDesignDataUpdater();
#endif
		}
		public static IDesignDataSettings GetDesignData(DependencyObject sender) {
			if(sender == null) throw new ArgumentNullException("sender");
			return (IDesignDataSettings)sender.GetValue(DesignDataProperty);
		}
		public static void SetDesignData(DependencyObject sender, IDesignDataSettings value) {
			if(sender == null) throw new ArgumentNullException("sender");
			sender.SetValue(DesignDataProperty, value);
		}
		public static void RegisterUpdater(Type type, IDesignDataUpdater updater) {
			updaters[type] = updater;
		}
		static void OnDesignDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!DesignerProperties.GetIsInDesignMode(d)) return;
			Type type = d.GetType();
			if(!updaters.ContainsKey(type)) return;
			updaters[type].UpdateDesignData(d);
		}
	}
}
