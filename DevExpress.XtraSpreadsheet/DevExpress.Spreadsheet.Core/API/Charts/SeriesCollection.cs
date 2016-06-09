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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public interface SeriesCollection : ISimpleCollection<Series> {
		Series Add(ChartData arguments, ChartData values);
		Series Add(string seriesName, ChartData arguments, ChartData values);
		Series Add(Range arguments, Range values);
		Series Add(Range seriesName, Range arguments, Range values);
		void Remove(Series series);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Series series);
		int IndexOf(Series series);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Collections;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.Utils;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Office.Model;
	#region NativeSeriesCollection
	partial class NativeSeriesCollection : NativeObjectBase, SeriesCollection {
		#region Fields
		readonly NativeChart nativeChart;
		readonly List<NativeSeries> innerList;
		#endregion
		public NativeSeriesCollection(NativeChart nativeChart) {
			this.nativeChart = nativeChart;
			this.innerList = new List<NativeSeries>();
			Initialize();
		}
		#region Properties
		protected internal Model.IChart ModelChart { get { return nativeChart.ModelChart; } }
		protected internal List<NativeSeries> InnerList { get { return innerList; } }
		Model.IChartView TargetModelView { get { return ModelChart.Views.Last; } }
		Model.ChartViewCollection ModelViewCollection { get { return ModelChart.Views; } }
		#endregion
		#region Internal
		protected internal void Initialize() {
			ModelViewCollection.ForEach(RegisterSeries);
			SubscribeEvents();
		}
		void RegisterSeries(Model.IChartView modelView) {
			modelView.Series.ForEach(RegisterSeries);
		}
		void RegisterSeries(Model.ISeries series) {
			innerList.Add(CreateNativeSeries(series));
		}
		void RegisterView(Model.IChartView view) {
			view.Series.ForEach(RegisterSeries);
			SubscribeSeriesEvents(view);
		}
		NativeSeries CreateNativeSeries(Model.ISeries modelItem) {
			return new NativeSeries(modelItem, nativeChart);
		}
		void InvalidateSeries(NativeSeries nativeSeries) {
			if (nativeSeries != null)
				nativeSeries.IsValid = false;
		}
		void InvalidateSeriesCollection() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				InvalidateSeries(innerList[i]);
		}
		void InvalidateAndRemoveSeries(Model.SeriesCollection seriesCollection) {
			seriesCollection.ForEach(InvalidateAndRemoveSeries);
		}
		void InvalidateAndRemoveSeries(Model.ISeries series) {
			NativeSeries nativeSeries = FindNativeSeries(series);
			if (nativeSeries != null) {
				nativeSeries.IsValid = false;
				innerList.Remove(nativeSeries);
			}
		}
		NativeSeries FindNativeSeries(Model.ISeries series) {
			int count = innerList.Count;
			for (int i = 0; i < count; i++) {
				NativeSeries nativeSeries = innerList[i];
				if (Object.ReferenceEquals(series, nativeSeries.ModelSeries))
					return nativeSeries;
			}
			return null;
		}
		void InsertSeriesFromView(Model.SeriesCollection modelSeriesCollection, int allSeriesCount) {
			int viewSeriesCount = modelSeriesCollection.Count;
			for (int i = 0; i < viewSeriesCount; i++)
				innerList.Insert(allSeriesCount + i, CreateNativeSeries(modelSeriesCollection[i]));
		}
		int GetPreviousSeriesCount(Model.ChartViewCollection collection, int viewIndex) {
			int result = 0;
			for (int i = 0; i < viewIndex; i++)
				result += collection[i].Series.Count;
			return result;
		}
		#endregion
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			ModelViewCollection.OnAdd += OnViewAdd;
			ModelViewCollection.OnRemoveAt += OnViewRemoveAt;
			ModelViewCollection.OnInsert += OnViewInsert;
			ModelViewCollection.OnClear += OnViewsClear;
			ModelViewCollection.OnAddRange += OnViewsAddRange;
			SubscribeAllSeriesEvents();
		}
		void SubscribeAllSeriesEvents() {
			ModelViewCollection.ForEach(SubscribeSeriesEvents);
		}
		void SubscribeSeriesEvents(Model.IChartView modelView) {
			Model.SeriesCollection modelCollection = modelView.Series;
			modelCollection.OnAdd += OnSeriesAdd;
			modelCollection.OnRemoveAt += OnSeriesRemoveAt;
			modelCollection.OnInsert += OnSeriesInsert;
			modelCollection.OnClear += OnSeriesClear;
			modelCollection.OnAddRange += OnSeriesAddRange;
		}
		protected internal void UnsubscribeEvents() {
			ModelViewCollection.OnAdd -= OnViewAdd;
			ModelViewCollection.OnRemoveAt -= OnViewRemoveAt;
			ModelViewCollection.OnInsert -= OnViewInsert;
			ModelViewCollection.OnClear -= OnViewsClear;
			ModelViewCollection.OnAddRange -= OnViewsAddRange;
			UnsubscribeAllSeriesEvents();
		}
		void UnsubscribeAllSeriesEvents() {
			ModelViewCollection.ForEach(UnsubscribeSeriesEvents);
		}
		void UnsubscribeSeriesEvents(Model.IChartView modelView) {
			Model.SeriesCollection modelCollection = modelView.Series;
			modelCollection.OnAdd -= OnSeriesAdd;
			modelCollection.OnRemoveAt -= OnSeriesRemoveAt;
			modelCollection.OnInsert -= OnSeriesInsert;
			modelCollection.OnClear -= OnSeriesClear;
			modelCollection.OnAddRange -= OnSeriesAddRange;
		}
		void OnSeriesAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.ISeries> modelArgs = e as UndoableCollectionAddEventArgs<Model.ISeries>;
			if (modelArgs != null)
				RegisterSeries(modelArgs.Item);
		}
		void OnViewAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.IChartView> modelArgs = e as UndoableCollectionAddEventArgs<Model.IChartView>;
			if (modelArgs != null) {
				Model.IChartView view = modelArgs.Item;
				RegisterSeries(view);
				SubscribeSeriesEvents(view);
			}
		}
		void OnSeriesRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			Model.SeriesCollection modelSeries = sender as Model.SeriesCollection;
			if (modelSeries == null)
				return;
			int nativeIndex = e.Index;
			int viewCount = ModelViewCollection.Count;
			for (int i = 0; i < viewCount; i++) {
				Model.IChartView modelView = ModelViewCollection[i];
				Model.SeriesCollection currentModelSeries = modelView.Series;
				if (Object.ReferenceEquals(currentModelSeries, modelSeries))
					RemoveNativeSeriesCore(nativeIndex);
				nativeIndex += currentModelSeries.Count;
			}
		}
		void RemoveNativeSeriesCore(int index) {
			InvalidateSeries(innerList[index]);
			innerList.RemoveAt(index);
		}
		void OnViewRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			Model.IChartView view = ModelViewCollection[index];
			InvalidateAndRemoveSeries(view.Series);
			UnsubscribeSeriesEvents(view);
		}
		void OnSeriesInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.ISeries> modelArgs = e as UndoableCollectionInsertEventArgs<Model.ISeries>;
			Model.SeriesCollection modelSeriesCollection = sender as Model.SeriesCollection;
			if (modelArgs == null || modelSeriesCollection == null)
				return;
			Model.ChartViewCollection modelCollection = modelSeriesCollection.Parent.Views;
			Model.ISeries modelSeries = modelArgs.Item;
			int viewIndex = modelCollection.IndexOf(modelSeries.View);
			innerList.Insert(modelArgs.Index + GetPreviousSeriesCount(modelCollection, viewIndex), CreateNativeSeries(modelSeries));
		}
		void OnViewInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.IChartView> modelArgs = e as UndoableCollectionInsertEventArgs<Model.IChartView>;
			Model.ChartViewCollection modelViewCollection = sender as Model.ChartViewCollection;
			if (modelArgs == null || modelViewCollection == null)
				return;
			Model.IChartView modelView = modelArgs.Item;
			int allSeriesCount = GetPreviousSeriesCount(modelViewCollection, modelArgs.Index);
			InsertSeriesFromView(modelView.Series, allSeriesCount);
			SubscribeSeriesEvents(modelView);
		}
		void OnSeriesClear(object sender) {
			Model.SeriesCollection series = sender as Model.SeriesCollection;
			if (series != null)
				InvalidateAndRemoveSeries(series);
		}
		void OnViewsClear(object sender) {
			InvalidateSeriesCollection();
			innerList.Clear();
			UnsubscribeAllSeriesEvents();
		}
		void OnSeriesAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.ISeries> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.ISeries>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.ISeries> collection = modelArgs.Collection;
			foreach (Model.ISeries modelSeries in collection)
				RegisterSeries(modelSeries);
		}
		void OnViewsAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.IChartView> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.IChartView>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.IChartView> collection = modelArgs.Collection;
			foreach (Model.IChartView modelView in collection)
				RegisterView(modelView);
		}
		#endregion
		#region ISimpleCollection<Series> Members
		public IEnumerator<Series> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<Series, NativeSeries>(innerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get {
				CheckValid();
				return ((IList)this.innerList).IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				CheckValid();
				return ((IList)this.innerList).SyncRoot;
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateSeriesCollection();
				UnsubscribeEvents();
			}
		}
		#region SeriesCollection Members
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public Series this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		public Series Add(ChartData arguments, ChartData values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(TargetModelView, arguments, values);
			return innerList[Count - 1];
		}
		public Series Add(string seriesName, ChartData arguments, ChartData values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(TargetModelView, seriesName, arguments, values);
			return innerList[Count - 1];
		}
		public Series Add(Range arguments, Range values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(TargetModelView, arguments, values);
			return innerList[Count - 1];
		}
		public Series Add(Range seriesName, Range arguments, Range values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(TargetModelView, seriesName, arguments, values);
			return innerList[Count - 1];
		}
		public bool Contains(Series series) {
			CheckValid();
			return NativeSeriesCollectionHelper.Contains(innerList, series);
		}
		public int IndexOf(Series series) {
			CheckValid();
			return NativeSeriesCollectionHelper.IndexOf(innerList, series);
		}
		public void Remove(Series series) {
			CheckValid();
			NativeSeriesCollectionHelper.RemoveSeries(innerList, series);
		}
		public void RemoveAt(int index) {
			CheckValid();
			NativeSeriesCollectionHelper.RemoveAtSeries(innerList, index);
		}
		public void Clear() {
			CheckValid();
			Model.ClearAllSeriesCommand command = new Model.ClearAllSeriesCommand(ApiErrorHandler.Instance, ModelChart);
			command.Execute();
		}
		#endregion
	}
	#endregion
	#region NativeSeriesCollectionHelper
	public static class NativeSeriesCollectionHelper {
		internal static void AddSeries(Model.IChartView view, ChartData arguments, ChartData values) {
			Guard.ArgumentNotNull(values, "values");
			AddSeriesCore(view, null, arguments, values);
		}
		internal static void AddSeries(Model.IChartView view, string seriesName, ChartData arguments, ChartData values) {
			Guard.ArgumentNotNull(values, "values");
			Model.DocumentModel documentModel = view.Parent.DocumentModel;
			documentModel.BeginUpdate();
			try {
				ExecuteCommands(documentModel, view, CreateChartText(view.Parent, seriesName), arguments, values);
			} finally {
				documentModel.EndUpdate();
			}
		}
		internal static void AddSeries(Model.IChartView view, Range argumentsRange, Range valuesRange) {
			Guard.ArgumentNotNull(valuesRange, "valuesRange");
			AddSeries(view, null, argumentsRange, valuesRange);
		}
		internal static void AddSeries(Model.IChartView view, Range seriesNameRange, Range argumentsRange, Range valuesRange) {
			Guard.ArgumentNotNull(valuesRange, "valuesRange");
			ChartData arguments = argumentsRange != null ? ChartData.FromRange(argumentsRange) : ChartData.Empty;
			ChartData values = ChartData.FromRange(valuesRange);
			if (seriesNameRange == null)
				AddSeries(view, arguments, values);
			else 
				AddSeries(view, seriesNameRange, arguments, values);
		}
		internal static void RemoveSeries(IList<NativeSeries> collection, Series series) {
			NativeSeries nativeSeries = series as NativeSeries;
			RemoveModelSeries(nativeSeries.ModelSeries);
		}
		internal static void RemoveAtSeries(IList<NativeSeries> collection, int index) {
			RemoveModelSeries(collection[index].ModelSeries);
		}
		internal static bool Contains(IList<NativeSeries> collection, Series series) {
			NativeSeries nativeSeries = series as NativeSeries;
			if (nativeSeries == null || !nativeSeries.IsValid)
				return false;
			return collection.IndexOf(nativeSeries) != -1;
		}
		internal static int IndexOf(IList<NativeSeries> collection, Series series) {
			NativeSeries nativeSeries = series as NativeSeries;
			if (nativeSeries == null || !nativeSeries.IsValid)
				return -1;
			return collection.IndexOf(nativeSeries);
		}
		#region Internal
		static void AddSeries(Model.IChartView view, Range seriesNameRange, ChartData arguments, ChartData values) {
			Model.DocumentModel documentModel = view.Parent.DocumentModel;
			documentModel.BeginUpdate();
			try {
				NativeRange seriesName = seriesNameRange.GetRangeWithAbsoluteReference() as NativeRange;
				ExecuteCommands(documentModel, view, seriesName == null ? null : Model.ChartTextRef.Create(view.Parent, seriesName.ModelRange), arguments, values);
			} finally {
				documentModel.EndUpdate();
			}
		}
		static void AddSeriesCore(Model.IChartView view, Model.IChartText seriesName, ChartData arguments, ChartData values) {
			Model.DocumentModel documentModel = view.Parent.DocumentModel;
			documentModel.BeginUpdate();
			try {
				ExecuteCommands(documentModel, view, seriesName, arguments, values);
			} finally {
				documentModel.EndUpdate();
			}
		}
		static void ExecuteCommands(Model.DocumentModel documentModel, Model.IChartView view, Model.IChartText seriesName, ChartData arguments, ChartData values) {
			Model.IChart chart = view.Parent;
			SetSeriesDirection(chart, values);
			Model.ChartViewSeriesDirection direction = chart.Views[0].SeriesDirection;
			Model.IDataReference argumentsReference = ChartDataConverter.ToDataReference(arguments, documentModel, direction, ChartDataConverter.IsNumber(arguments));
			Model.IDataReference valuesReference = ChartDataConverter.ToDataReference(values, documentModel, direction, true);
			Model.InsertSeriesCommand command = new Model.InsertSeriesCommand(ApiErrorHandler.Instance, view, argumentsReference, valuesReference, seriesName);
			command.Execute();
		}
		static void SetSeriesDirection(Model.IChart chart, ChartData values) {
			if (values.IsRange) {
				Model.ChangeSeriesDirectionCommand changeSeriesDirectionCommand = new Model.ChangeSeriesDirectionCommand(ApiErrorHandler.Instance, chart, values.ModelVariantValue.CellRangeValue);
				changeSeriesDirectionCommand.Execute();
			}
		}
		static Model.IChartText CreateChartText(Model.IChart chart, string seriesName) {
			if (seriesName[0] == '=')
				return Model.ChartTextRef.Create(chart, seriesName);
			return Model.ChartTextValue.Create(chart, seriesName);
		}
		static void RemoveModelSeries(Model.ISeries series) {
			Model.RemoveSeriesCommand command = new Model.RemoveSeriesCommand(ApiErrorHandler.Instance, series);
			command.Execute();
		}
		#endregion
	}
	#endregion
	#region NativeViewSeriesCollection
	partial class NativeViewSeriesCollection : NativeChartCollectionBase<Series, NativeSeries, Model.ISeries>, SeriesCollection {
		#region Fields
		readonly NativeChart nativeChart;
		readonly Model.IChartView modelView;
		#endregion
		public NativeViewSeriesCollection(Model.IChartView modelView, NativeChart nativeChart)
			: base(modelView.Series) {
			this.modelView = modelView;
			this.nativeChart = nativeChart;
			SubscribeViewsEvents();
		}
		Model.ChartViewCollection ModelViews { get { return modelView.Parent.Views; } }
		#region SubscribeViewsEvents
		void SubscribeViewsEvents() {
			ModelViews.OnRemoveAt += OnViewRemoveAt;
		}
		void UnsubscribeViewsEvents() {
			ModelViews.OnRemoveAt -= OnViewRemoveAt;
		}
		void OnViewRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = modelView.IndexOfView;
			if (e.Index == index && index != 0)
				SetIsValid(false);
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value)
				UnsubscribeViewsEvents();
		}
		#region NativeChartCollectionBase<Series, NativeSeries, Model.ISeries> Members
		protected override NativeSeries CreateNativeObject(Model.ISeries modelItem) {
			return new NativeSeries(modelItem, nativeChart);
		}
		#endregion
		#region SeriesCollection Members
		public Series Add(ChartData arguments, ChartData values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(modelView, arguments, values);
			return InnerList[Count - 1];
		}
		public Series Add(string seriesName, ChartData arguments, ChartData values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(modelView, seriesName, arguments, values);
			return InnerList[Count - 1];
		}
		public Series Add(Range arguments, Range values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(modelView, arguments, values);
			return InnerList[Count - 1];
		}
		public Series Add(Range seriesName, Range arguments, Range values) {
			CheckValid();
			NativeSeriesCollectionHelper.AddSeries(modelView, seriesName, arguments, values);
			return InnerList[Count - 1];
		}
		public void Remove(Series series) {
			CheckValid();
			NativeSeriesCollectionHelper.RemoveSeries(InnerList, series);
		}
		public override void RemoveAt(int index) {
			CheckValid();
			NativeSeriesCollectionHelper.RemoveAtSeries(InnerList, index);
		}
		public bool Contains(Series series) {
			CheckValid();
			return NativeSeriesCollectionHelper.Contains(InnerList, series);
		}
		public int IndexOf(Series series) {
			CheckValid();
			return NativeSeriesCollectionHelper.IndexOf(InnerList, series);
		}
		public override void Clear() {
			CheckValid();
			Model.ClearViewSeriesCommand command = new Model.ClearViewSeriesCommand(ApiErrorHandler.Instance, modelView);
			command.Execute();
		}
		#endregion
	}
	#endregion
}
