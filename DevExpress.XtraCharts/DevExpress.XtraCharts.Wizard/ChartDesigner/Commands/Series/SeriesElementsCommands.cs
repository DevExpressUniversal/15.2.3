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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AddSeriesTitleCommand : AddCommandBase<SeriesTitle> {
		readonly SeriesTitleCollection seriesTitleCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesTitleCollection; } }
		public AddSeriesTitleCommand(CommandManager commandManager, SeriesTitleCollection seriesTitleCollection)
			: base(commandManager) {
			this.seriesTitleCollection = seriesTitleCollection;
		}
		protected override SeriesTitle CreateChartElement(object parameter) {
			return new SeriesTitle();
		}
		protected override void AddToCollection(SeriesTitle chartElement) {
			seriesTitleCollection.Add(chartElement);
		}
	}
	public class DeleteSeriesTitleCommand : DeleteCommandBase<SeriesTitle> {
		readonly SeriesTitleCollection seriesTitleCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesTitleCollection; } }
		public DeleteSeriesTitleCommand(CommandManager commandManager, SeriesTitleCollection seriesTitleCollection)
			: base(commandManager) {
			this.seriesTitleCollection = seriesTitleCollection;
		}
		protected override void InsertIntoCollection(int index, SeriesTitle chartElement) {
			seriesTitleCollection.Insert(index, chartElement);
		}
	}
	public class AddIndicatorCommand : AddCommandBase<Indicator> {
		readonly IndicatorCollection indicatorCollection;
		protected override ChartCollectionBase ChartCollection { get { return indicatorCollection; } }
		public AddIndicatorCommand(CommandManager commandManager, IndicatorCollection indicatorCollection)
			: base(commandManager) {
			this.indicatorCollection = indicatorCollection;
		}
		protected override Indicator CreateChartElement(object parameter) {
			Indicator indicator = null;
			var selectedIndicatorType = parameter as Type;
			if (selectedIndicatorType != null)
				indicator = (Indicator)Activator.CreateInstance(selectedIndicatorType);
			if (parameter is FibonacciIndicatorKind)
				indicator = new FibonacciIndicator((FibonacciIndicatorKind)parameter);
			if (indicator != null)
				indicator.Name = indicatorCollection.GenerateName(indicator.GetType().Name + " ");
			return indicator;
		}
		protected override void GenerateName(Indicator chartElement) {
			if(chartElement != null) {
				string name = indicatorCollection.GenerateName(chartElement.IndicatorName + " ");
				chartElement.Name = name;
			}
		}
		protected override void AddToCollection(Indicator chartElement) {
			indicatorCollection.Add(chartElement);
		}
		public override bool CanExecute(object parameter) {
			return (parameter is Type && ((Type)parameter).IsSubclassOf(typeof(Indicator))) || parameter is FibonacciIndicatorKind;
		}
	}
	public class DeleteIndicatorCommand : DeleteCommandBase<Indicator> {
		readonly IndicatorCollection indicatorCollection;
		protected override ChartCollectionBase ChartCollection { get { return indicatorCollection; } }
		public DeleteIndicatorCommand(CommandManager commandManager, IndicatorCollection indicatorCollection)
			: base(commandManager) {
			this.indicatorCollection = indicatorCollection;
		}
		protected override void InsertIntoCollection(int index, Indicator chartElement) {
			indicatorCollection.Insert(index, chartElement);
		}
	}
	public class AddSeriesPointCommand : AddCommandBase<SeriesPoint> {
		readonly SeriesPointCollection pointCollection;
		protected override ChartCollectionBase ChartCollection { get { return pointCollection; } }
		public AddSeriesPointCommand(CommandManager commandManager, SeriesPointCollection pointCollection)
			: base(commandManager) {
			this.pointCollection = pointCollection;
		}
		protected override SeriesPoint CreateChartElement(object parameter) {
			throw new NotImplementedException();
		}
		protected override void AddToCollection(SeriesPoint chartElement) {
			pointCollection.Add(chartElement);
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			return new HistoryItem(this, null, parameter as SeriesPoint, null);
		}
	}
	public class DeleteSeriesPointCommand : DeleteCommandBase<SeriesPoint> {
		readonly SeriesPointCollection pointCollection;
		protected override ChartCollectionBase ChartCollection { get { return pointCollection; } }
		public DeleteSeriesPointCommand(CommandManager commandManager, SeriesPointCollection pointCollection)
			: base(commandManager) {
			this.pointCollection = pointCollection;
		}
		protected override void InsertIntoCollection(int index, SeriesPoint chartElement) {
			pointCollection.Insert(index, chartElement);
		}
	}
	public class AddSeriesPointFilterCommand : AddCommandBase<SeriesPointFilter> {
		readonly SeriesPointFilterCollection seriesPointFilterCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesPointFilterCollection; } }
		public AddSeriesPointFilterCommand(CommandManager commandManager, SeriesPointFilterCollection seriesPointFilterCollection)
			: base(commandManager) {
			this.seriesPointFilterCollection = seriesPointFilterCollection;
		}
		protected override SeriesPointFilter CreateChartElement(object parameter) {
			return parameter as SeriesPointFilter;
		}
		protected override void AddToCollection(SeriesPointFilter chartElement) {
			seriesPointFilterCollection.Add(chartElement);
		}
	}
	public class DeleteSeriesPointFilterCommand : DeleteCommandBase<SeriesPointFilter> {
		readonly SeriesPointFilterCollection seriesPointFilterCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesPointFilterCollection; } }
		public DeleteSeriesPointFilterCommand(CommandManager commandManager, SeriesPointFilterCollection seriesPointFilterCollection)
			: base(commandManager) {
			this.seriesPointFilterCollection = seriesPointFilterCollection;
		}
		protected override void InsertIntoCollection(int index, SeriesPointFilter chartElement) {
			seriesPointFilterCollection.Insert(index, chartElement);
		}
	}
	public class AddDataFilterCommand : AddCommandBase<DataFilter> {
		readonly DataFilterCollection dataFilterCollection;
		protected override ChartCollectionBase ChartCollection { get { return dataFilterCollection; } }
		public AddDataFilterCommand(CommandManager commandManager, DataFilterCollection dataFilterCollection)
			: base(commandManager) {
			this.dataFilterCollection = dataFilterCollection;
		}
		protected override DataFilter CreateChartElement(object parameter) {
			return parameter as DataFilter;
		}
		protected override void AddToCollection(DataFilter chartElement) {
			dataFilterCollection.Add(chartElement);
		}
	}
	public class DeleteDataFilterCommand : DeleteCommandBase<DataFilter> {
		readonly DataFilterCollection dataFilterCollection;
		protected override ChartCollectionBase ChartCollection { get { return dataFilterCollection; } }
		public DeleteDataFilterCommand(CommandManager commandManager, DataFilterCollection dataFilterCollection)
			: base(commandManager) {
			this.dataFilterCollection = dataFilterCollection;
		}
		protected override void InsertIntoCollection(int index, DataFilter chartElement) {
			dataFilterCollection.Insert(index, chartElement);
		}
	}
	public class AddRelationCommand : AddCommandBase<Relation> {
		readonly SeriesPointRelationCollection seriesPointRelationCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesPointRelationCollection; } }
		public AddRelationCommand(CommandManager commandManager, SeriesPointRelationCollection seriesPointRelationCollection)
			: base(commandManager) {
			this.seriesPointRelationCollection = seriesPointRelationCollection;
		}
		protected override Relation CreateChartElement(object parameter) {
			return new TaskLink((SeriesPoint)parameter);
		}
		protected override void AddToCollection(Relation chartElement) {
			seriesPointRelationCollection.Add(chartElement);
		}
	}
	public class DeleteRelationCommand : DeleteCommandBase<Relation> {
		readonly SeriesPointRelationCollection seriesPointRelationCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesPointRelationCollection; } }
		public DeleteRelationCommand(CommandManager commandManager, SeriesPointRelationCollection seriesPointRelationCollection)
			: base(commandManager) {
			this.seriesPointRelationCollection = seriesPointRelationCollection;
		}
		protected override void InsertIntoCollection(int index, Relation chartElement) {
			seriesPointRelationCollection.Insert(index, chartElement);
		}
		protected override void RemoveFromCollection(Relation relation) {
			SeriesPoint childPoint = relation.ChildPoint;
			base.RemoveFromCollection(relation);
			relation.ChildPoint = childPoint;
		}
	}
	public class AddKnownDateCommand : AddCommandBase<KnownDate> {
		readonly KnownDateCollection knownDateCollection;
		protected override ChartCollectionBase ChartCollection { get { return knownDateCollection; } }
		public AddKnownDateCommand(CommandManager commandManager, KnownDateCollection knownDateCollection)
			: base(commandManager) {
			this.knownDateCollection = knownDateCollection;
		}
		protected override void GenerateName(KnownDate chartElement) {
		}
		protected override KnownDate CreateChartElement(object parameter) {
			return parameter as KnownDate;
		}
		protected override void AddToCollection(KnownDate chartElement) {
			knownDateCollection.Add(chartElement);
		}
	}
	public class DeleteKnownDateCommand : DeleteCommandBase<KnownDate> {
		readonly KnownDateCollection knownDateCollection;
		protected override ChartCollectionBase ChartCollection { get { return knownDateCollection; } }
		public DeleteKnownDateCommand(CommandManager commandManager, KnownDateCollection knownDateCollection)
			: base(commandManager) {
			this.knownDateCollection = knownDateCollection;
		}
		protected override void InsertIntoCollection(int index, KnownDate chartElement) {
			knownDateCollection.Insert(index, chartElement);
		}
	}
	public class ClearKnownDatesCommand : ClearCommandBase<KnownDate> {
		readonly KnownDateCollection knownDateCollection;
		protected override ChartCollectionBase ChartCollection { get { return knownDateCollection; } }
		public ClearKnownDatesCommand(CommandManager commandManager, KnownDateCollection knownDateCollection)
			: base(commandManager) {
			this.knownDateCollection = knownDateCollection;
		}
		protected override void AddToCollection(KnownDate chartElement) {
			knownDateCollection.Add(chartElement);
		}
	}
	public class AddExplodedPointCommand : AddCommandBase<SeriesPoint> {
		readonly ExplodedSeriesPointCollection explodedSeriesPointCollection;
		protected override ChartCollectionBase ChartCollection { get { return explodedSeriesPointCollection; } }
		public AddExplodedPointCommand(CommandManager commandManager, ExplodedSeriesPointCollection explodedSeriesPointCollection)
			: base(commandManager) {
			this.explodedSeriesPointCollection = explodedSeriesPointCollection;
		}
		protected override SeriesPoint CreateChartElement(object parameter) {
			return parameter as SeriesPoint;
		}
		protected override void AddToCollection(SeriesPoint chartElement) {
			explodedSeriesPointCollection.Add(chartElement);
		}
	}
	public class DeleteExplodedPointCommand : DeleteCommandBase<SeriesPoint> {
		readonly ExplodedSeriesPointCollection explodedSeriesPointCollection;
		protected override ChartCollectionBase ChartCollection { get { return explodedSeriesPointCollection; } }
		public DeleteExplodedPointCommand(CommandManager commandManager, ExplodedSeriesPointCollection explodedSeriesPointCollection)
			: base(commandManager) {
			this.explodedSeriesPointCollection = explodedSeriesPointCollection;
		}
		protected override void InsertIntoCollection(int index, SeriesPoint chartElement) {
			explodedSeriesPointCollection.Add(chartElement);
		}
	}
	public class AddColorizerKeyCommand : ChartCommandBase {
		readonly KeyCollection collection;
		public AddColorizerKeyCommand(KeyCollection collection, CommandManager commandManager)
			: base(commandManager) {
			this.collection = collection;
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			if (parameter != null) {
				collection.Add(parameter);
				return new HistoryItem(this, null, parameter, null);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			collection.Remove(historyItem.NewValue);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			collection.Add(historyItem.NewValue);
		}
	}
	public class DeleteColorizerKeyCommand : ChartCommandBase {
		readonly KeyCollection collection;
		public DeleteColorizerKeyCommand(KeyCollection collection, CommandManager commandManager)
			: base(commandManager) {
			this.collection = collection;
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			if (parameter != null) {
				int index = collection.IndexOf(parameter);
				collection.Remove(parameter);
				return new HistoryItem(this, parameter, null, index);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			int index = (int)historyItem.Parameter;
			collection.Insert(index, historyItem.OldValue);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			collection.Remove(historyItem.OldValue);
		}
	}
}
