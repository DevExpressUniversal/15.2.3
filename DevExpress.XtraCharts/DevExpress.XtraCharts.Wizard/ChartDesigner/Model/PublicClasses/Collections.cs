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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[Editor("DevExpress.XtraCharts.Designer.Native.DesignerModelCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor))]
	public abstract class ChartCollectionBaseModel : DesignerChartElementModelBase, ICollection, IList {
		readonly ChartCollectionBase chartCollectionBase;
		readonly List<DesignerChartElementModelBase> innerList;
		readonly SwapCommand swapCommand;
		protected internal override ChartElement ChartElement { get { return null; } }
		protected internal ChartCollectionBase ChartCollection { get { return chartCollectionBase; } }
		protected DesignerChartElementModelBase this[int index] { get { return innerList[index]; } }
		internal int Count { get { return innerList.Count; } }
		[Browsable(false)]
		public new object Tag { get; set; }
		public ChartCollectionBaseModel(ChartCollectionBase chartCollectionBase, CommandManager commandManager) : this(chartCollectionBase, commandManager, null) {
		}
		public ChartCollectionBaseModel(ChartCollectionBase chartCollectionBase, CommandManager commandManager, Chart chart)
			: base(commandManager, chart) {
			this.innerList = new List<DesignerChartElementModelBase>();
			this.chartCollectionBase = chartCollectionBase;
			this.swapCommand = new SwapCommand(commandManager, chartCollectionBase);
		}
		#region ICollection implementation
		void ICollection.CopyTo(Array array, int index) {
			((ICollection)innerList).CopyTo(array, index);
		}
		bool ICollection.IsSynchronized { get { return ((ICollection)innerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((ICollection)innerList).SyncRoot; } }
		int ICollection.Count { get { return Count; } }
		IEnumerator IEnumerable.GetEnumerator() { return innerList.GetEnumerator(); }
		#endregion
		#region IList implementation
		int IList.Add(object value) {
			SetItemOwner(value as DesignerChartElementModelBase, this);
			return ((IList)innerList).Add(value);
		}
		bool IList.Contains(object value) {
			DesignerChartElementModelBase chartElement = value as DesignerChartElementModelBase;
			return chartElement != null ? Contains(chartElement) : false;
		}
		int IList.IndexOf(object value) {
			DesignerChartElementModelBase chartElement = value as DesignerChartElementModelBase;
			return chartElement != null ? IndexOf(chartElement) : -1;
		}
		void IList.Insert(int index, object value) {
			InsertCore(index, value as DesignerChartElementModelBase);
		}
		bool IList.IsFixedSize { get { return ((IList)innerList).IsFixedSize; } }
		bool IList.IsReadOnly { get { return ((IList)innerList).IsReadOnly; } }
		void IList.Remove(object value) {
			DesignerChartElementModelBase chartElement = value as DesignerChartElementModelBase;
			SetItemOwner(chartElement, null);
			if(chartElement != null)
				Remove(chartElement);
		}
		void IList.Clear() {
			ClearCore();
		}
		void IList.RemoveAt(int index) {
			RemoveAtCore(index);
		}
		object IList.this[int index] {
			get { return (DesignerChartElementModelBase)this[index]; }
			set {
				DesignerChartElementModelBase chartElement = value as DesignerChartElementModelBase;
				if(chartElement != null)
					innerList[index] = chartElement;
			}
		}
		#endregion
		void SetItemOwner(DesignerChartElementModelBase chartElement, ChartCollectionBaseModel owner) {
			if (chartElement != null)
				chartElement.ParentCollection = owner;
		}		
		void Insert(int index, DesignerChartElementModelBase chartElement) {
			InsertCore(index, chartElement);
			InsertComplete(index, chartElement);
		}
		void InsertCore(int index, DesignerChartElementModelBase chartElement) {
			if (chartElement != null) {
				innerList.Insert(index, chartElement);
				SetItemOwner(chartElement, this);
			}
		}
		void RemoveAt(int index) {
			DesignerChartElementModelBase chartElement = RemoveAtCore(index);
			RemoveComplete(index, chartElement);
		}
		DesignerChartElementModelBase RemoveAtCore(int index) {
			DesignerChartElementModelBase chartElement = (DesignerChartElementModelBase)innerList[index];
			SetItemOwner(chartElement, null);
			innerList.RemoveAt(index);
			return chartElement;
		}
		void Clear() {
			ClearCore();
			ClearComplete();
		}
		void ClearCore() {
			foreach (DesignerChartElementModelBase chartElement in innerList) {
				SetItemOwner(chartElement, null);
			}
			innerList.Clear();
		}
		void SwapCore(int oldIndex, int newIndex) {
			DesignerChartElementModelBase item1 = innerList[oldIndex];
			DesignerChartElementModelBase item2 = innerList[newIndex];
			innerList.RemoveAt(oldIndex);
			innerList.Insert(oldIndex, item2);
			innerList.RemoveAt(newIndex);
			innerList.Insert(newIndex, item1);
		}
		bool Contains(DesignerChartElementModelBase chartElement) {
			return innerList.Contains(chartElement);
		}
		bool Remove(DesignerChartElementModelBase chartElement) {
			SetItemOwner(chartElement, null);
			return innerList.Remove(chartElement);
		}
		void SynchronizeInsertOperation() {
			for (int itemIndex = 0; itemIndex < chartCollectionBase.Count; itemIndex++) {
				ChartElement chartElement = chartCollectionBase.GetElementByIndex(itemIndex);
				DesignerChartElementModelBase model = itemIndex < innerList.Count ? innerList[itemIndex] : null;
				if (model == null || chartElement != model.ChartElement)
					Insert(itemIndex, CreateModelItem(chartElement));
			}
		}
		void SynchronizeRemoveOperation() {
			if (chartCollectionBase.Count == 0) {
				Clear();
				return;
			}
			int itemIndex = 0;
			while (itemIndex < innerList.Count) {
				ChartElement chartElement = itemIndex < chartCollectionBase.Count ? chartCollectionBase.GetElementByIndex(itemIndex) : null;
				DesignerChartElementModelBase model = innerList[itemIndex];
				if (chartElement == null || chartElement != model.ChartElement)
					RemoveAt(itemIndex);
				else
					itemIndex++;
			}
		}
		void SynchronizeSwapOperation() {
			List<int> notSynchronizedIndices = new List<int>();
			for (int itemIndex = 0; itemIndex < chartCollectionBase.Count; itemIndex++) {
				ChartElement chartElement = chartCollectionBase.GetElementByIndex(itemIndex);
				DesignerChartElementModelBase model = innerList[itemIndex];
				if (!chartElement.Equals(model.ChartElement))
					notSynchronizedIndices.Add(itemIndex);
			}
			int indicesCount = notSynchronizedIndices.Count;
			if (indicesCount == 2)
				SwapCore(notSynchronizedIndices[0], notSynchronizedIndices[1]);
			else if (indicesCount != 0)
				DevExpress.Charts.Native.ChartDebug.Fail("Unknown collection operation");
		}
		protected void Add(DesignerChartElementModelBase chartElement) {
			if (chartElement != null) {
				innerList.Add(chartElement);
				SetItemOwner(chartElement, this);
			}
		}
		protected abstract DesignerChartElementModelBase CreateModelItem(ChartElement chartElement);
		protected override void AddChildren() {
			Children.AddRange(innerList);
			base.AddChildren();
		}
		protected virtual void RemoveComplete(int index, DesignerChartElementModelBase chartElement) { }
		protected virtual void InsertComplete(int index, DesignerChartElementModelBase chartElement) { }
		protected virtual void ClearComplete() { }
		public int IndexOf(DesignerChartElementModelBase chartElement) {
			return innerList.IndexOf(chartElement);
		}
		public override void Update() {
			if (chartCollectionBase.Count > innerList.Count)
				SynchronizeInsertOperation();
			else if (chartCollectionBase.Count < innerList.Count)
				SynchronizeRemoveOperation();
			else
				SynchronizeSwapOperation();
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public abstract void AddNewElement(object parameter);
		public abstract void DeleteElement(object parameter);
		public virtual void ClearElements() { }
		public void Swap(int oldIndex, int newIndex) {
			swapCommand.Execute(new SwapCommandParameter(oldIndex, newIndex));
		}
	}
	public abstract class DockableTitleCollectionBaseModel : ChartCollectionBaseModel {
		protected new DockableTitleModel this[int index] { get { return (DockableTitleModel)base[index]; } }
		public DockableTitleCollectionBaseModel(DockableTitleCollectionBase collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
		}
	}
	[ModelOf(typeof(SeriesTitleCollection))]
	public class SeriesTitleCollectionModel : DockableTitleCollectionBaseModel {
		readonly AddSeriesTitleCommand addCommand;
		readonly DeleteSeriesTitleCommand deleteCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.SeriesTitleKey; } }
		public new SeriesTitleModel this[int index] { get { return (SeriesTitleModel)base[index]; } }
		public SeriesTitleCollectionModel(SeriesTitleCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
				addCommand = new AddSeriesTitleCommand(commandManager, collection);
				deleteCommand = new DeleteSeriesTitleCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SeriesTitleModel((SeriesTitle)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			SeriesTitleModel titleModel = parameter as SeriesTitleModel;
			if (titleModel != null)
				deleteCommand.Execute(titleModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeCollectionElement(this);
		}
		public override string ToString() {
			return "Titles (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SeriesPointFilterCollection))]
	public class SeriesPointFilterCollectionModel : ChartCollectionBaseModel {
		readonly AddSeriesPointFilterCommand addCommand;
		readonly DeleteSeriesPointFilterCommand deleteCommand;
		public new SeriesPointFilterModel this[int index] { get { return (SeriesPointFilterModel)base[index]; } }
		protected SeriesPointFilterCollection SeriesPointFilterCollection { get { return (SeriesPointFilterCollection)ChartCollection; } }
		[PropertyForOptions]
		public ConjunctionTypes ConjunctionMode {
			get { return SeriesPointFilterCollection.ConjunctionMode; }
			set { SetProperty("ConjunctionMode", value); }
		}
		public SeriesPointFilterCollectionModel(SeriesPointFilterCollection collection, CommandManager commandManager)
			: base(collection, commandManager) {
			addCommand = new AddSeriesPointFilterCommand(commandManager, collection);
			deleteCommand = new DeleteSeriesPointFilterCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SeriesPointFilterModel((SeriesPointFilter)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
	}
	[ModelOf(typeof(ExplodedSeriesPointCollection))]
	public class ExplodedSeriesPointCollectionModel : ChartCollectionBaseModel {
		readonly AddExplodedPointCommand addCommand;
		readonly DeleteExplodedPointCommand deleteCommand;
		public new SeriesPointModel this[int index] { get { return (SeriesPointModel)base[index]; } }
		protected ExplodedSeriesPointCollection ExplodedSeriesPointCollection { get { return (ExplodedSeriesPointCollection)ChartCollection; } }
		public ExplodedSeriesPointCollectionModel(ExplodedSeriesPointCollection collection, CommandManager commandManager)
			: base(collection, commandManager) {
			addCommand = new AddExplodedPointCommand(commandManager, collection);
			deleteCommand = new DeleteExplodedPointCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SeriesPointModel((SeriesPoint)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
	}
	[ModelOf(typeof(ChartTitleCollection))]
	public class ChartTitleCollectionModel : DockableTitleCollectionBaseModel {
		readonly AddChartTitleCommand addChartTitleCommand;
		readonly DeleteChartTitleCommand deleteChartTitleCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.TitleKey; } }
		public new ChartTitleModel this[int index] { get { return (ChartTitleModel)base[index]; } }
		public ChartTitleCollectionModel(ChartTitleCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addChartTitleCommand = new AddChartTitleCommand(CommandManager, collection);
			deleteChartTitleCommand = new DeleteChartTitleCommand(CommandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new ChartTitleModel((ChartTitle)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addChartTitleCommand.Execute(null);
		}
		public override void DeleteElement(object parameter) {
			ChartTitleModel titleModel = parameter as ChartTitleModel;
			if (titleModel != null)
				deleteChartTitleCommand.Execute(titleModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeCollectionElement(this);
		}
		public override string ToString() {
			return "Titles (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SeriesCollection))]
	public class SeriesCollectionModel : ChartCollectionBaseModel {
		readonly AddSeriesCommand addSeriesCommand;
		DeleteSeriesCommand deleteSeriesCommand;
		internal DeleteSeriesCommand DeleteSeriesCommand { get { return deleteSeriesCommand; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.SeriesKey; } }
		protected internal SeriesCollection SeriesCollection { get { return (SeriesCollection)base.ChartCollection; } }
		public new DesignerSeriesModel this[int index] { get { return (DesignerSeriesModel)base[index]; } }
		public SeriesCollectionModel(SeriesCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addSeriesCommand = new AddSeriesCommand(CommandManager, Chart);
			UpdateDeleteCommand(collection);
		}
		void UpdateDeleteCommand(SeriesCollection collection) {
			deleteSeriesCommand = collection.Count != 1 ? new DeleteSeriesCommand(CommandManager, collection) : new DeleteOnlySeriesCommand(CommandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new DesignerSeriesModel((Series)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			if(parameter != null)
				addSeriesCommand.Execute((ViewType)parameter);
		}
		public override void DeleteElement(object parameter) {
			DesignerSeriesModel seriesModel = parameter as DesignerSeriesModel;
			if(seriesModel != null)
				deleteSeriesCommand.Execute(seriesModel.Series);
		}
		protected override void InsertComplete(int index, DesignerChartElementModelBase chartElement) {
			base.InsertComplete(index, chartElement);
			UpdateDeleteCommand(SeriesCollection);
		}
		protected override void RemoveComplete(int index, DesignerChartElementModelBase chartElement) {
			base.RemoveComplete(index, chartElement);
			UpdateDeleteCommand(SeriesCollection);
		}
		public bool IsCompartibleViewType(ViewType viewType) {
			return addSeriesCommand.CanExecute(viewType);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeSeriesCollectionElement(this);
		}
		public override string ToString() {
			return "Series Collection (" + this.Count.ToString() + ")";
		}
	}
	public abstract class ChartElementNamedCollectionModel : ChartCollectionBaseModel {
		public ChartElementNamedCollectionModel(ChartElementNamedCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
		}
	}
	[ModelOf(typeof(IndicatorCollection))]
	public class IndicatorCollectionModel : ChartElementNamedCollectionModel {
		readonly AddIndicatorCommand addCommand;
		readonly DeleteIndicatorCommand deleteCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.IndicatorKey; } }
		public new IndicatorModel this[int index] { get { return (IndicatorModel)base[index]; } }
		public IndicatorCollectionModel(IndicatorCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddIndicatorCommand(commandManager, collection);
			deleteCommand = new DeleteIndicatorCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return ModelHelper.CreateIndicatorModelInstance(chartElement as Indicator, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			IndicatorModel indicatorModel = parameter as IndicatorModel;
			if (indicatorModel != null)
				deleteCommand.Execute(indicatorModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeIndicatorCollectionElement(this);
		}
		public override string ToString() {
			return "Indicators (" + this.Count.ToString() + ")";
		}
	}
	public abstract class SecondaryAxisCollectionModel : ChartElementNamedCollectionModel {
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AxisKey; } }
		public new Axis2DModel this[int index] { get { return (Axis2DModel)base[index]; } }
		public SecondaryAxisCollectionModel(SecondaryAxisCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeCollectionElement(this);
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramSecondaryAxisXCollection))]
	public class SwiftPlotDiagramSecondaryAxisXCollectionModel : SecondaryAxisCollectionModel {
		readonly AddSwiftPlotSecondaryAxisXCommand addCommand;
		readonly DeleteSwiftPlotSecondaryAxisXCommand deleteCommand;
		public new SwiftPlotDiagramSecondaryAxisXModel this[int index] { get { return (SwiftPlotDiagramSecondaryAxisXModel)base[index]; } }
		public SwiftPlotDiagramSecondaryAxisXCollectionModel(SwiftPlotDiagramSecondaryAxisXCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddSwiftPlotSecondaryAxisXCommand(commandManager, collection);
			deleteCommand = new DeleteSwiftPlotSecondaryAxisXCommand(commandManager, collection, chart);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SwiftPlotDiagramSecondaryAxisXModel((SwiftPlotDiagramSecondaryAxisX)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			SwiftPlotDiagramSecondaryAxisXModel secondaryAxisModel = parameter as SwiftPlotDiagramSecondaryAxisXModel;
			if (secondaryAxisModel != null)
				deleteCommand.Execute(secondaryAxisModel.ChartElement);
		}
		public override string ToString() {
			return "Secondary Axes X (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SwiftPlotDiagramSecondaryAxisYCollection))]
	public class SwiftPlotDiagramSecondaryAxisYCollectionModel : SecondaryAxisCollectionModel {
		readonly AddSwiftPlotSecondaryAxisYCommand addCommand;
		readonly DeleteSwiftPlotSecondaryAxisYCommand deleteCommand;
		public new SwiftPlotDiagramSecondaryAxisYModel this[int index] { get { return (SwiftPlotDiagramSecondaryAxisYModel)base[index]; } }
		public SwiftPlotDiagramSecondaryAxisYCollectionModel(SwiftPlotDiagramSecondaryAxisYCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddSwiftPlotSecondaryAxisYCommand(commandManager, collection);
			deleteCommand = new DeleteSwiftPlotSecondaryAxisYCommand(commandManager, collection, chart);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SwiftPlotDiagramSecondaryAxisYModel((SwiftPlotDiagramSecondaryAxisY)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			SwiftPlotDiagramSecondaryAxisYModel secondaryAxisModel = parameter as SwiftPlotDiagramSecondaryAxisYModel;
			if (secondaryAxisModel != null)
				deleteCommand.Execute(secondaryAxisModel.ChartElement);
		}
		public override string ToString() {
			return "Secondary Axes Y (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(XYDiagramPaneCollection))]
	public class XYDiagramPaneCollectionModel : ChartElementNamedCollectionModel {
		readonly AddPaneCommand addCommand;
		readonly DeletePaneCommand deleteCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.PaneKey; } }
		public new XYDiagramPaneModel this[int index] { get { return (XYDiagramPaneModel)base[index]; } }
		public XYDiagramPaneCollectionModel(XYDiagramPaneCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddPaneCommand(commandManager, collection);
			deleteCommand = new DeletePaneCommand(commandManager, collection, chart);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new XYDiagramPaneModel((XYDiagramPane)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			XYDiagramPaneModel paneModel = parameter as XYDiagramPaneModel;
			if (paneModel != null)
				deleteCommand.Execute(paneModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreePaneCollectionElement(this);
		}
		public override string ToString() {
			return "Additional Panes (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SecondaryAxisXCollection))]
	public class SecondaryAxisXCollectionModel : SecondaryAxisCollectionModel {
		readonly AddSecondaryAxisXCommand addCommand;
		readonly DeleteSecondaryAxisXCommand deleteCommand;
		public new SecondaryAxisXModel this[int index] { get { return (SecondaryAxisXModel)base[index]; } }
		public SecondaryAxisXCollectionModel(SecondaryAxisXCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddSecondaryAxisXCommand(commandManager, collection);
			deleteCommand = new DeleteSecondaryAxisXCommand(commandManager, collection, chart);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SecondaryAxisXModel((SecondaryAxisX)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			SecondaryAxisXModel secondaryAxisModel = parameter as SecondaryAxisXModel;
			if (secondaryAxisModel != null)
				deleteCommand.Execute(secondaryAxisModel.ChartElement);
		}
		public override string ToString() {
			return "Secondary Axes X (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SecondaryAxisYCollection))]
	public class SecondaryAxisYCollectionModel : SecondaryAxisCollectionModel {
		readonly AddSecondaryAxisYCommand addCommand;
		readonly DeleteSecondaryAxisYCommand deleteCommand;
		public new SecondaryAxisYModel this[int index] { get { return (SecondaryAxisYModel)base[index]; } }
		public SecondaryAxisYCollectionModel(SecondaryAxisYCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddSecondaryAxisYCommand(commandManager, collection);
			deleteCommand = new DeleteSecondaryAxisYCommand(commandManager, collection, chart);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SecondaryAxisYModel((SecondaryAxisY)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			SecondaryAxisYModel secondaryAxisModel = parameter as SecondaryAxisYModel;
			if (secondaryAxisModel != null)
				deleteCommand.Execute(secondaryAxisModel.ChartElement);
		}
		public override string ToString() {
			return "Secondary Axes Y (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(StripCollection))]
	public class StripCollectionModel : ChartElementNamedCollectionModel {
		AddStripCommand addCommand;
		DeleteStripCommand deleteCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.StripKey; } }
		public new StripModel this[int index] { get { return (StripModel)base[index]; } }
		public StripCollectionModel(StripCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddStripCommand(commandManager, collection);
			deleteCommand = new DeleteStripCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new StripModel((Strip)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			StripModel stripModel = parameter as StripModel;
			if (stripModel != null)
				deleteCommand.Execute(stripModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeStripCollectionElement(this);
		}
		public override string ToString() {
			return "Strips (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(ConstantLineCollection))]
	public class ConstantLineCollectionModel : ChartElementNamedCollectionModel {
		readonly AddConstantLineCommand addCommand;
		readonly DeleteConstantLineCommand deleteCommand; 
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ConstantLineKey; } }
		public new ConstantLineModel this[int index] { get { return (ConstantLineModel)base[index]; } }
		public ConstantLineCollectionModel(ConstantLineCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddConstantLineCommand(commandManager, collection);
			deleteCommand = new DeleteConstantLineCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new ConstantLineModel((ConstantLine)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			ConstantLineModel constantLineModel = parameter as ConstantLineModel;
			if (constantLineModel != null)
				deleteCommand.Execute(constantLineModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeConstantLineCollectionElement(this);
		}
		public override string ToString() {
			return "Constant Lines (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(CustomAxisLabelCollection))]
	public class CustomAxisLabelCollectionModel : ChartElementNamedCollectionModel {
		readonly AddCustomAxisLabelCommand addCommand;
		readonly DeleteCustomAxisLabelCommand deleteCommand; 
		public new CustomAxisLabelModel this[int index] { get { return (CustomAxisLabelModel)base[index]; } }
		public CustomAxisLabelCollectionModel(CustomAxisLabelCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
				addCommand = new AddCustomAxisLabelCommand(commandManager, collection);
				deleteCommand = new DeleteCustomAxisLabelCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new CustomAxisLabelModel((CustomAxisLabel)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
	}
	[ModelOf(typeof(ScaleBreakCollection))]
	public class ScaleBreakCollectionModel : ChartElementNamedCollectionModel {
		readonly AddScaleBreakCommand addCommand;
		readonly DeleteScaleBreakCommand deleteCommand; 
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ScaleBreakKey; } }
		public new ScaleBreakModel this[int index] { get { return (ScaleBreakModel)base[index]; } }
		public ScaleBreakCollectionModel(ScaleBreakCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddScaleBreakCommand(commandManager, collection);
			deleteCommand = new DeleteScaleBreakCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new ScaleBreakModel((ScaleBreak)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			ScaleBreakModel scaleBreakModel = parameter as ScaleBreakModel;
			if (scaleBreakModel != null)
				deleteCommand.Execute(scaleBreakModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeScaleBreakCollectionElement(this);
		}
		public override string ToString() {
			return "Scale Breaks (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(KnownDateCollection))]
	public class KnownDateCollectionModel : ChartCollectionBaseModel {
		readonly AddKnownDateCommand addCommand;
		readonly DeleteKnownDateCommand deleteCommand;
		readonly ClearKnownDatesCommand clearCommand;
		public new KnownDateModel this[int index] { get { return (KnownDateModel)base[index]; } }
		public KnownDateCollectionModel(KnownDateCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddKnownDateCommand(commandManager, collection);
			deleteCommand = new DeleteKnownDateCommand(commandManager, collection);
			clearCommand = new ClearKnownDatesCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new KnownDateModel((KnownDate)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
		public override void ClearElements() {
			base.ClearElements();
			clearCommand.Execute(null);
		}
	}
	[ModelOf(typeof(AnnotationRepository))]
	public class AnnotationRepositoryModel : ChartElementNamedCollectionModel {
		readonly AddAnnotationCommand addCommand;
		readonly DeleteAnnotationCommand deleteCommand;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.AnnotationKey; } }
		public new AnnotationModel this[int index] { get { return (AnnotationModel)base[index]; } }
		public AnnotationRepositoryModel(AnnotationRepository collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddAnnotationCommand(commandManager, collection);
			deleteCommand = new DeleteAnnotationCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return ModelHelper.CreateAnnotationModelInstance(chartElement as Annotation, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			AnnotationModel annotationModel = parameter as AnnotationModel;
			if (annotationModel != null)
				deleteCommand.Execute(annotationModel.ChartElement);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeAnnotationCollectionElement(this);
		}
		public override string ToString() {
			return "Annotations (" + this.Count.ToString() + ")";
		}
	}
	[ModelOf(typeof(SeriesPointCollection))]
	public class SeriesPointCollectionModel : ChartCollectionBaseModel, IBindingList {
		readonly AddSeriesPointCommand addCommand;
		readonly DeleteSeriesPointCommand deleteCommand;
		IBindingList SourceList { get { return (IBindingList)this.ChartCollection; } }
		internal override string ChartTreeText { get { return "Points"; } }
		public new SeriesPointModel this[int index] { get { return (SeriesPointModel)base[index]; } }
		public event ListChangedEventHandler ListChanged;
		public SeriesPointCollectionModel(SeriesPointCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddSeriesPointCommand(commandManager, collection);
			deleteCommand = new DeleteSeriesPointCommand(commandManager, collection);
		}
		void RaiseListChanged(ListChangedType changeType, int itemIndex) {
			if (ListChanged != null)
				ListChanged(this, new ListChangedEventArgs(changeType, itemIndex));
		}
		protected internal override bool IsSupportsDataControl(bool isDesignTime) {
			return true;
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new SeriesPointModel((SeriesPoint)chartElement, CommandManager);
		}
		protected override void RemoveComplete(int index, DesignerChartElementModelBase chartElement) {
			base.RemoveComplete(index, chartElement);
			RaiseListChanged(ListChangedType.ItemDeleted, index);
		}
		protected override void InsertComplete(int index, DesignerChartElementModelBase chartElement) {
			base.InsertComplete(index, chartElement);
			RaiseListChanged(ListChangedType.ItemAdded, index);
		}
		protected override void ClearComplete() {
			base.ClearComplete();
			RaiseListChanged(ListChangedType.Reset, -1);
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeSeriesPointsElement(this);
		}
		public override void AddNewElement(object parameter) {
			SeriesPointModel pointModel = parameter as SeriesPointModel;
			if (pointModel != null)
				addCommand.Execute(pointModel.ChartElement);
		}
		public override void DeleteElement(object parameter) {
			SeriesPointModel pointModel = parameter as SeriesPointModel;
			if (pointModel != null)
				deleteCommand.Execute(pointModel.ChartElement);
		}
		#region IBindingList Members
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		bool IBindingList.IsSorted { get { return false; } }
		bool IBindingList.AllowEdit { get { return SourceList.AllowEdit; } }
		bool IBindingList.AllowNew { get { return SourceList.AllowNew; } }
		bool IBindingList.AllowRemove { get { return SourceList.AllowRemove; } }
		ListSortDirection IBindingList.SortDirection { get { throw new NotImplementedException(); } }
		PropertyDescriptor IBindingList.SortProperty { get { throw new NotImplementedException(); } }
		object IBindingList.AddNew() {
			SeriesPoint point = (SeriesPoint)SourceList.AddNew();
			SeriesPointModel model = new SeriesPointModel(point, CommandManager);
			this.Add(model);
			return model;
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new NotImplementedException();
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotImplementedException();
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotImplementedException();
		}
		void IBindingList.RemoveSort() {
			throw new NotImplementedException();
		}
		#endregion
	}
	[ModelOf(typeof(SeriesPointRelationCollection))]
	public class SeriesPointRelationCollectionModel : ChartCollectionBaseModel {
		readonly AddRelationCommand addCommand;
		readonly DeleteRelationCommand deleteCommand;
		protected internal override ChartElement ChartElement {
			get {
				return base.ChartElement;
			}
		}
		public new RelationModel this[int index] { get { return (RelationModel)base[index]; } }
		public SeriesPointRelationCollectionModel(SeriesPointRelationCollection collection, SeriesPointModel point, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddRelationCommand(commandManager, collection);
			deleteCommand = new DeleteRelationCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return ModelHelper.CreateRelationModel(chartElement as Relation, Parent.ChartElement as SeriesPoint, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
	}
	[ModelOf(typeof(DataFilterCollection))]
	public class DataFilterCollectionModel : ChartCollectionBaseModel {
		readonly AddDataFilterCommand addCommand;
		readonly DeleteDataFilterCommand deleteCommand;
		public new DataFilterModel this[int index] { get { return (DataFilterModel)base[index]; } }
		protected DataFilterCollection DataFilterCollection { get { return (DataFilterCollection)base.ChartCollection; } }
		[PropertyForOptions]
		public ConjunctionTypes ConjunctionMode {
			get { return DataFilterCollection.ConjunctionMode; }
			set { SetProperty("ConjunctionMode", value); }
		}
		public DataFilterCollectionModel(DataFilterCollection collection, CommandManager commandManager, Chart chart)
			: base(collection, commandManager, chart) {
			addCommand = new AddDataFilterCommand(commandManager, collection);
			deleteCommand = new DeleteDataFilterCommand(commandManager, collection);
		}
		protected override DesignerChartElementModelBase CreateModelItem(ChartElement chartElement) {
			return new DataFilterModel((DataFilter)chartElement, CommandManager);
		}
		public override void AddNewElement(object parameter) {
			addCommand.Execute(parameter);
		}
		public override void DeleteElement(object parameter) {
			deleteCommand.Execute(parameter);
		}
	}
}
