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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class PointValuesCache {
		readonly DateTime[] dateTimeValues;
		readonly double[] values;
		public DateTime[] DateTimeValues { get { return CopyArray<DateTime>(dateTimeValues); } }
		public double[] Values { get { return CopyArray<double>(values); } }
		public PointValuesCache(DateTime[] dateTimeValues, double[] values) {
			this.values = CopyArray<double>(values);
			this.dateTimeValues = CopyArray<DateTime>(dateTimeValues);
		}
		TArray[] CopyArray<TArray>(TArray[] source) {
			TArray[] copy = new TArray[source.Length];
			Array.Copy(source, copy, source.Length);
			return copy;
		}
	}
	public class SetPointIsEmptyCommand : ChartCommandBase {
		readonly SeriesPoint seriesPoint;
		ScaleType ScaleType {
			get {
				Series series = ((IOwnedElement)seriesPoint).Owner as Series;
				return series != null ? series.ValueScaleType : ScaleType.Numerical;
			}
		}
		protected internal override bool CanUpdatePointsGrid { get { return true; } }
		public SetPointIsEmptyCommand(CommandManager commandManager, SeriesPoint seriesPoint)
			: base(commandManager) {
			this.seriesPoint = seriesPoint;
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			if (parameter is bool) {
				bool oldValue = seriesPoint.IsEmpty;
				bool newValue = (bool)parameter;
				PointValuesCache valuesCache = new PointValuesCache(seriesPoint.DateTimeValues, seriesPoint.Values);
				seriesPoint.IsEmpty = newValue;
				return new HistoryItem(this, oldValue, newValue, valuesCache);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			PointValuesCache valuesCache = (PointValuesCache)historyItem.Parameter;
			seriesPoint.IsEmpty = (bool)historyItem.OldValue;
			if (ScaleType == ScaleType.DateTime)
				seriesPoint.DateTimeValues = valuesCache.DateTimeValues;
			else
				seriesPoint.Values = valuesCache.Values;
		}
		public override void RedoInternal(HistoryItem historyItem) {
			seriesPoint.IsEmpty = (bool)historyItem.NewValue;
		}
	}
	public class SetPointValuesCommandCache<TValues> {
		readonly bool isEmpty;
		readonly TValues[] values;
		public bool IsEmpty { get { return isEmpty; } }
		public TValues[] Values { get { return CopyArray(values); } }
		public SetPointValuesCommandCache(bool isEmpty, TValues[] values) {
			this.values = CopyArray(values);
			this.isEmpty = isEmpty;
		}
		TValues[] CopyArray(TValues[] source) {
			TValues[] copy = new TValues[source.Length];
			Array.Copy(source, copy, source.Length);
			return copy;
		}
	}
	public abstract class SetPointValuesCommandBase<TValues> : ChartCommandBase {
		readonly SeriesPoint seriesPoint;
		protected SeriesPoint SeriesPoint { get { return seriesPoint; } }
		protected abstract TValues[] Values { get; set; }
		protected internal override bool CanUpdatePointsGrid { get { return true; } }
		public SetPointValuesCommandBase(CommandManager commandManager, SeriesPoint seriesPoint)
			: base(commandManager) {
			this.seriesPoint = seriesPoint;
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			TValues[] values = parameter as TValues[];
			if (values != null) {
				SetPointValuesCommandCache<TValues> oldValue = new SetPointValuesCommandCache<TValues>(seriesPoint.IsEmpty, Values);
				Values = values;
				SetPointValuesCommandCache<TValues> newValue = new SetPointValuesCommandCache<TValues>(seriesPoint.IsEmpty, Values);
				return new HistoryItem(this, oldValue, newValue, null);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			SetPointValuesCommandCache<TValues> valuesCache = (SetPointValuesCommandCache<TValues>)historyItem.OldValue;
			Values = valuesCache.Values;
			seriesPoint.IsEmpty = valuesCache.IsEmpty;
		}
		public override void RedoInternal(HistoryItem historyItem) {
			SetPointValuesCommandCache<TValues> valuesCache = (SetPointValuesCommandCache<TValues>)historyItem.NewValue;
			Values = valuesCache.Values;
			seriesPoint.IsEmpty = valuesCache.IsEmpty;
		}
	}
	public class SetPointValuesCommand : SetPointValuesCommandBase<double> {
		public SetPointValuesCommand(CommandManager commandManager, SeriesPoint seriesPoint)
			: base(commandManager, seriesPoint) {
		}
		protected override double[] Values {
			get { return SeriesPoint.Values; }
			set { SeriesPoint.Values = value; }
		}
	}
	public class SetPointDateTimeValuesCommand : SetPointValuesCommandBase<DateTime> {
		public SetPointDateTimeValuesCommand(CommandManager commandManager, SeriesPoint seriesPoint)
			: base(commandManager, seriesPoint) {
		}
		protected override DateTime[] Values {
			get { return SeriesPoint.DateTimeValues; }
			set { SeriesPoint.DateTimeValues = value; }
		}
	}
}
