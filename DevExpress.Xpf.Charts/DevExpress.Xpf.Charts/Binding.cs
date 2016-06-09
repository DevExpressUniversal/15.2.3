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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public delegate void BindingHandler();
	public class BindingBehavior : IWeakEventListener {
		object dataSource;
		object actualDataSource;
		DataSet dataSet;
		DataSourceProvider dataSourceProvider;
		public event BindingHandler ActualDataSourceChanged;
		public object ActualDataSource { get { return actualDataSource; } }
		void ProcessActualDataSource() {
			ClearDataSet();
			if (dataSource is ICollectionView)
				actualDataSource = DataBindingHelper.ExtractDataSource(dataSource);
			else if ((dataSource is IList) || (dataSource is IListSource))
				actualDataSource = PerformListDataSource(dataSource);
			else {
				IEnumerable enumerable = dataSource as IEnumerable;
				if (enumerable == null)
					actualDataSource = null;
				else {
					XmlNode node = enumerable as XmlNode;
					if (node == null) {
						List<object> list = new List<object>();
						foreach (object value in enumerable)
							list.Add(value);
						actualDataSource = list;
					}
					else
						actualDataSource = PerformXmlNode(node);
				}
			}
		}
		void DataSourceChanged() {
			ProcessActualDataSource();
			ActualDataSourceChanged();
		}
		void ClearDataSource() {
			IBindingList bindingList = dataSource as IBindingList;
			if (bindingList != null)
				ListChangedEventManager.RemoveListener(bindingList, this);
			else {
				INotifyCollectionChanged notificator = dataSource as INotifyCollectionChanged;
				if (notificator != null)
					CollectionChangedEventManager.RemoveListener(notificator, this);
			}
			actualDataSource = null;
			dataSource = null;
			ClearDataSet();
		}
		void SetDataSource(object dataSource) {
			ClearDataSource();
			if (dataSource != null) {
				this.dataSource = dataSource;
				IBindingList bindingList = dataSource as IBindingList;
				if (bindingList != null)
					ListChangedEventManager.AddListener(bindingList, this);
				else {
					INotifyCollectionChanged notificator = dataSource as INotifyCollectionChanged;
					if (notificator != null)
						CollectionChangedEventManager.AddListener(notificator, this);
				}
				ProcessActualDataSource();
			}
		}
		void CreateDataSet(XmlNode node) {
			XmlNodeReader reader = new XmlNodeReader(node);
			try {
				dataSet = new DataSet();
				dataSet.ReadXml(reader);
			}
			finally {
				reader.Close();
			}
		}
		void DataProviderDataChanged() {
			if (dataSourceProvider != null)
				SetDataSource(dataSourceProvider.Data);
			ActualDataSourceChanged();
		}
		void ClearDataSet() {
			if (dataSet != null) {
				dataSet.Dispose();
				dataSet = null;
			}
		}
		void BeforeUpdateDataSource() {
			if (dataSourceProvider != null)
				DataChangedEventManager.RemoveListener(dataSourceProvider, this);
		}
		object PerformNewDataSource(object newDataSource) {
			dataSourceProvider = newDataSource as DataSourceProvider;
			if (dataSourceProvider != null) {
				DataChangedEventManager.AddListener(dataSourceProvider, this);
				return dataSourceProvider.Data;
			}
			MarkupExtension markupDataSource = newDataSource as MarkupExtension;
			if (markupDataSource != null)
				return markupDataSource.ProvideValue(null);
			return newDataSource;
		}
		object PerformListDataSource(object dataSource) {
			ICollection collection = dataSource as ICollection;
			if (collection != null && collection.Count == 1) {
				IEnumerator enumerator = collection.GetEnumerator();
				if (enumerator != null && enumerator.MoveNext()) {
					XmlNode node = enumerator.Current as XmlNode;
					if (node != null) {
						CreateDataSet(node);
						return dataSet;
					}
				}
			}
			return dataSource;
		}
		object PerformXmlNode(XmlNode node) {
			if (node == null || string.IsNullOrEmpty(node.Name))
				return null;
			XmlNode sourceNode = node.ParentNode != null ? node.ParentNode : node;
			CreateDataSet(sourceNode);
			return dataSet.Tables[node.Name];
		}
		public void UpdateActualDataSource(object newDataSource) {
			BeforeUpdateDataSource();
			if (newDataSource != null)
				SetDataSource(PerformNewDataSource(newDataSource));
			else
				ClearDataSource();
			ActualDataSourceChanged();
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(DataChangedEventManager)) {
				DataProviderDataChanged();
				return true;
			}
			if (managerType == typeof(ListChangedEventManager) ||
				managerType == typeof(CollectionChangedEventManager)) {
				DataSourceChanged();
				return true;
			}
			return false;
		}
	}
	public class CollectionChangedEventManager : WeakEventManager {
		static CollectionChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(CollectionChangedEventManager);
				CollectionChangedEventManager currentManager = (CollectionChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if (currentManager == null) {
					currentManager = new CollectionChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		public static void AddListener(INotifyCollectionChanged source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(INotifyCollectionChanged source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		CollectionChangedEventManager() {
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
			base.DeliverEvent(sender, args);
		}
		protected override void StartListening(object source) {
			INotifyCollectionChanged changed = (INotifyCollectionChanged)source;
			changed.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
		}
		protected override void StopListening(object source) {
			INotifyCollectionChanged changed = (INotifyCollectionChanged)source;
			changed.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
		}
	}
}
