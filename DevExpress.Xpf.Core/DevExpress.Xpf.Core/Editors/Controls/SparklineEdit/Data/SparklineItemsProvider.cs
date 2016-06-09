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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Editors.Internal {
	public class SparklineItemsProvider : FrameworkElement, IItemsSourceSupport {
		const SparklineSortOrder defaultSortOrder = SparklineSortOrder.Ascending;
		ItemsProviderChangedEventHandler itemsProviderChanged;
		SparklinePointCollection points;
		SortedSparklinePointCollection pointsSortedByArgument;
		IList ListSource;
		ListSourceDataController DataController;
		object itemsSource;
		SparklineSortOrder pointArgumentSortOrder = SparklineSortOrder.Ascending;
		string pointArgumentMember;
		string pointValueMember;
		CriteriaOperator filterCriteria;
		SparklineDataClient DataControllerData { get { return DataController.DataClient as SparklineDataClient; } }
		public SparklinePointCollection Points { get { return pointsSortedByArgument; } }
		public object ItemsSource {
			get { return itemsSource; }
			set {
				if (itemsSource != value) {
					itemsSource = value;
					OnItemsSourceChanged();
				}
			}
		}
		public SparklineSortOrder PointArgumentSortOrder {
			get { return pointArgumentSortOrder; }
			set {
				if (pointArgumentSortOrder != value) {
					pointArgumentSortOrder = value;
					pointsSortedByArgument.Sort(new SparklinePointArgumentComparer(pointArgumentSortOrder == SparklineSortOrder.Ascending));
					OnItemsSourceSettingsChanged();
				}
			}
		}
		public string PointArgumentMember {
			get { return pointArgumentMember; }
			set {
				if (pointArgumentMember != value) {
					pointArgumentMember = value;
					OnItemsSourceSettingsChanged();
				}
			}
		}
		public string PointValueMember {
			get { return pointValueMember; }
			set {
				if (pointValueMember != value) {
					pointValueMember = value;
					OnItemsSourceSettingsChanged();
				}
			}
		}
		public CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				if (!ReferenceEquals(filterCriteria, value)) {
					filterCriteria = value;
					OnItemsSourceSettingsChanged();
				}
			}
		}
		public event ItemsProviderChangedEventHandler ItemsProviderChanged {
			add { itemsProviderChanged += value; }
			remove { itemsProviderChanged -= value; }
		}
		public SparklineItemsProvider() {
			points = new SparklinePointCollection();
			pointsSortedByArgument = new SortedSparklinePointCollection(new SparklinePointArgumentComparer());
		}
		void OnItemsSourceChanged() {
			RecreateDataController();
			FillItemCollection();
		}
		void OnItemsSourceSettingsChanged() {
			OnItemsSourceChanged();
		}
		void RecreateDataController() {
			if (DataController != null) {
				DataController.ListChanged -= DataController_ListChanged;
				DataController.Dispose();
			}
			DataController = new ListSourceDataController();
			DataController.DataClient = new SparklineDataClient(this);
			DataController.ListChanged += DataController_ListChanged;
			ListSource = ExtractDataSource();
			DataControllerData.ResetDescriptors();
			DataController.SetDataSource(ListSource);
			FilterDataSource();
		}
		void FilterDataSource() {
			try {
				DataController.BeginUpdate();
				DataController.FilterCriteria = FilterCriteria;
			}
			finally {
				DataController.EndUpdate();
			}
		}
		void FillItemCollection() {
			points.Clear();
			int index = 0;
			foreach (var row in DataController.GetAllFilteredAndSortedRows()) {
				SparklinePoint item = CreateSparklineItem(row, index);
				if (item != null)
					points.Add(item);
				index++;
			}
			pointsSortedByArgument.Clear();
			pointsSortedByArgument.AddRange(points);
		}
		SparklinePoint CreateSparklineItem(object row, int index) {
			SparklinePoint item = null;
			SparklineScaleType valueScaleType;
			SparklineScaleType argumentScaleType;
			double? value = GetPointValue(row, out valueScaleType);
			if (value != null) {
				item = new SparklinePoint(index, (double)value);
				item.ValueScaleType = valueScaleType;
			}
			if (item != null && !string.IsNullOrEmpty(PointArgumentMember)) {
				double? argument = GetPointArgument(row, out argumentScaleType);
				if (argument != null) {
					item.Argument = (double)argument;
					item.ArgumentScaleType = argumentScaleType;
				}
			}
			return item;
		}
		double? GetPointArgument(object row, out SparklineScaleType scaleType) {
			object value = DataControllerData.ArgumentColumnDescriptor.GetValue(row);
			return GetValue(value, out scaleType);
		}
		double? GetPointValue(object row, out SparklineScaleType scaleType) {
			object value = DataControllerData.ValueColumnDescriptor.GetValue(row);
			return GetValue(value, out scaleType);
		}
		double? GetValue(object value, out SparklineScaleType scaleType) {
			if (!SparklinePropertyDescriptorBase.IsUnsetValue(value)) {
				if (value == null) {
					scaleType = SparklineScaleType.Unknown;
					return null;
				}
				return SparklineMathUtils.ConvertToDouble(value, out scaleType);
			}
			scaleType = SparklineScaleType.Unknown;
			return null;
		}
		IList ExtractDataSource() {
			object itemsSource = ItemsSource;
			IList source = ExtractSimpleDataSource(itemsSource);
			return source;
		}
		IList ExtractSimpleDataSource(object itemsSource) {
			ICollectionView view = itemsSource as ICollectionView;
			if (view != null)
				return DataBindingHelper.ExtractDataSourceFromCollectionView(view);
			return DataBindingHelper.ExtractDataSource(itemsSource);
		}
		void DataController_ListChanged(object sender, ListChangedEventArgs e) {
			int listSourceIndex = e.NewIndex;
			if (e.ListChangedType == ListChangedType.ItemChanged)
				UpdateItem(listSourceIndex);
			else if (e.ListChangedType == ListChangedType.ItemAdded)
				AddItem(listSourceIndex);
			else if (e.ListChangedType == ListChangedType.ItemDeleted)
				RemoveItem(listSourceIndex);
			else
				FillItemCollection();
			RaiseDataChanged(e.ListChangedType, listSourceIndex, e.PropertyDescriptor);
		}
		void UpdateItem(int listSourceIndex) {
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow || controllerIndex < 0 || controllerIndex >= points.Count)
				return;
			object row = DataController.GetRowByListSourceIndex(listSourceIndex);
			SparklinePoint item = CreateSparklineItem(row, controllerIndex);
			SparklinePoint oldItem = points[controllerIndex];
			if (item != null) {
				points[controllerIndex] = item;
				pointsSortedByArgument.Update(oldItem, item);
			}
		}
		void AddItem(int listSourceIndex) {
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow || controllerIndex < 0 || controllerIndex > points.Count)
				return;
			object row = DataController.GetRowByListSourceIndex(listSourceIndex);
			SparklinePoint item = CreateSparklineItem(row, controllerIndex);
			if (item != null) {
				if (controllerIndex == points.Count)
					points.Add(item);
				else
					points.Insert(controllerIndex, item);
				pointsSortedByArgument.Add(item);
				SetAutoArguments(controllerIndex);
			}
		}
		void RemoveItem(int listSourceIndex) {
			int controllerIndex = DataController.GetControllerRow(listSourceIndex);
			if (controllerIndex == Data.DataController.InvalidRow)
				return;
			pointsSortedByArgument.Remove(points[controllerIndex]);
			points.RemoveAt(controllerIndex);
			SetAutoArguments(controllerIndex);
		}
		void SetAutoArguments(int controllerIndex) {
			if (string.IsNullOrEmpty(PointArgumentMember))
				for (int i = controllerIndex; i < points.Count; i++)
					points[i].Argument = i;
		}
		public void RaiseDataChanged(ListChangedType changedType = ListChangedType.Reset, int newIndex = -1, PropertyDescriptor descriptor = null) {
			RaiseDataChanged(new ItemsProviderDataChangedEventArgs(changedType, newIndex, descriptor));
		}
		public void RaiseDataChanged(ItemsProviderDataChangedEventArgs args) {
			if (itemsProviderChanged != null)
				itemsProviderChanged(this, args);
		}
	}
}
