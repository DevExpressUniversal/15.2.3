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
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
namespace DevExpress.Xpf.Map.Native {
	public delegate void BindingHandler();
	public class BindingBehavior {
		DataSet dataSet;
		DataSourceProvider dataSourceProvider;
		object dataSource;
		object actualDataSource;
		[SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event BindingHandler ActualDataSourceChanged;
		public object ActualDataSource { get { return actualDataSource; } }
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
		void DataProviderDataChanged(object sender, EventArgs e) {
			if(dataSourceProvider != null)
				SetDataSource(dataSourceProvider.Data);
			ActualDataSourceChanged();
		}
		void ClearDataSet() {
			if(dataSet != null) {
				dataSet.Dispose();
				dataSet = null;
			}
		}
		object PerformListDataSource(object dataSource) {
			ICollection collection = dataSource as ICollection;
			if(collection != null && collection.Count == 1) {
				IEnumerator enumerator = collection.GetEnumerator();
				if(enumerator != null && enumerator.MoveNext()) {
					XmlNode node = enumerator.Current as XmlNode;
					if(node != null) {
						CreateDataSet(node);
						return dataSet;
					}
				}
			}
			return dataSource;
		}
		object PerformXmlNode(XmlNode node) {
			string tableName = String.Empty;
			while(node.ParentNode != null && !(node.ParentNode is XmlDocument)) {
				if(String.IsNullOrEmpty(tableName))
					tableName = node.Name;
				else
					tableName = node.Name + '.' + tableName;
				node = node.ParentNode;
			}
			CreateDataSet(node);
			return String.IsNullOrEmpty(tableName) ? dataSet.Tables[0] : dataSet.Tables[tableName];
		}
		void BeforeUpdateDataSource() {
			if(dataSourceProvider != null)
				dataSourceProvider.DataChanged -= DataProviderDataChanged;
			ICollectionView view = actualDataSource as ICollectionView;
			if (view != null)
				view.CollectionChanged -= OnCollectionViewChanged;
		}
		object PerformNewDataSource(object newDataSource) {
			dataSourceProvider = newDataSource as DataSourceProvider;
			if(dataSourceProvider != null) {
				dataSourceProvider.DataChanged += DataProviderDataChanged;
				return dataSourceProvider.Data;
			}
			MarkupExtension markupDataSource = newDataSource as MarkupExtension;
			if(markupDataSource != null)
				return markupDataSource.ProvideValue(null);
			ICollectionView view = newDataSource as ICollectionView;
			if (view != null)
				view.CollectionChanged += OnCollectionViewChanged;
			return newDataSource;
		}
		void ProcessActualDataSource() {
			ClearDataSet();
			if((dataSource is IList) || (dataSource is IListSource))
				actualDataSource = PerformListDataSource(dataSource);
			else {
				IEnumerable enumerable = dataSource as IEnumerable;
				if(enumerable == null)
					actualDataSource = null;
				else {
					XmlNode node = enumerable as XmlNode;
					actualDataSource = node == null  ? enumerable : PerformXmlNode(node);
				}
			}
		}
		void OnCollectionViewChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			ActualDataSourceChanged();
		}
		void ClearDataSource() {
			actualDataSource = null;
			dataSource = null;
			ClearDataSet();
		}
		void SetDataSource(object dataSource) {
			ClearDataSource();
			if(dataSource != null) {
				this.dataSource = dataSource;
				ProcessActualDataSource();
			}
		}
		public void UpdateActualDataSource(object newDataSource) {
			BeforeUpdateDataSource();
			if(newDataSource != null)
				SetDataSource(PerformNewDataSource(newDataSource));
			else
				ClearDataSource();
			ActualDataSourceChanged();
		}
	}
}
