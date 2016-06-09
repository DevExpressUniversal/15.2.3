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
namespace DevExpress.Charts.Native {
	[Flags]
	internal enum ChartUpdateType {
		Empty = 0,
		UpdateScaleMap = 1,
		UpdateInteraction = 2,
		UpdateSeriesGroupsInteraction = 4,
		UpdateCrosshair = 16,
		UpdateSideMargins = 32,
		ResetSelectedItems = 64,
		DeserializeRange = 128,
		UpdateArgumentScale = 256,
		UpdatePointsIndices = 512
	}
	internal class ChartUpdateAggregator {
		readonly List<ChartUpdate> updates = new List<ChartUpdate>();
		ChartUpdateType aggregatedUpdateType = ChartUpdateType.Empty;
		public IList<ChartUpdate> Updates { get { return updates; } }
		public bool ShouldUpdateArgumentScale { get { return (aggregatedUpdateType & ChartUpdateType.UpdateArgumentScale) > 0; } }
		public bool ShouldUpdatePointsIndices { get { return (aggregatedUpdateType & ChartUpdateType.UpdatePointsIndices) > 0; } }
		public bool ShouldUpdateScaleMap { get { return (aggregatedUpdateType & ChartUpdateType.UpdateScaleMap) > 0; } }
		public bool ShouldUpdateInteraction { get { return (aggregatedUpdateType & ChartUpdateType.UpdateInteraction) > 0; } }
		public bool ShouldUpdateSeriesGroupsInteraction { get { return ShouldUpdateInteraction || (aggregatedUpdateType & ChartUpdateType.UpdateSeriesGroupsInteraction) > 0; } }
		public bool ShouldUpdateCrosshair { get { return (aggregatedUpdateType & ChartUpdateType.UpdateCrosshair) > 0; } }
		public bool ShouldDeserializeRange { get { return (aggregatedUpdateType & ChartUpdateType.DeserializeRange) > 0; } }
		public int Count {
			get {
				return updates.Count;
			}
		}
		public void Add(ChartUpdate update) {
			if (update == null)
				return;
			aggregatedUpdateType = aggregatedUpdateType | update.UpdateType;
			updates.Add(update);
		}
		public void AddRange(IEnumerable<ChartUpdate> updates) {
			foreach (ChartUpdate update in updates)
				Add(update);
		}
		public bool HasUpdates<T>() where T : ChartUpdate {
			foreach (ChartUpdate update in updates)
				if (update is T)
					return true;
			return false;
		}
		public IEnumerable<T> GetUpdates<T>() where T : ChartUpdate {
			foreach (ChartUpdate update in updates)
				if (update is T)
					yield return (T)update;
		}
	}
	internal class ChartUpdate {
		ChartUpdateType updateType;
		public ChartUpdateType UpdateType { get { return updateType; } }
		public bool ShouldUpdateScaleMap { get { return (updateType & ChartUpdateType.UpdateScaleMap) > 0; } }
		public bool ShouldUpdateInteraction { get { return (updateType & ChartUpdateType.UpdateInteraction) > 0; } }
		public ChartUpdate(ChartUpdateType updateType) {
			this.updateType = updateType;
		}
		public void AddUpdate(ChartUpdateType updateType) {
			this.updateType = this.updateType | updateType;
		}
	}
	internal class RangeResetUpdate : ChartUpdate {
		public IAxisRangeData Range { get; private set; }
		internal RangeResetUpdate(IAxisRangeData range)
			: base(ChartUpdateType.Empty) {
			this.Range = range;
		}
	}
	internal class BatchSeriesUpdate : ChartUpdate {
		readonly ICollection<RefinedSeries> refinedSeriesList;
		public ICollection<RefinedSeries> RefinedSeriesList { get { return refinedSeriesList; } }
		internal BatchSeriesUpdate(ICollection<RefinedSeries> refinedSeriesList, ChartUpdateType updateType)
			: base(updateType) {
			this.refinedSeriesList = refinedSeriesList;
		}
	}
	internal class PointsAggregationUpdate : BatchSeriesUpdate {
		readonly RefinedSeriesGroup groupToUpdate;
		readonly IAxisData axis;
		public RefinedSeriesGroup GroupToUpdate { get { return groupToUpdate; } }
		public IAxisData Axis { get { return axis; } }
		internal PointsAggregationUpdate(IAxisData axis, RefinedSeriesGroup groupToUpdate, ICollection<RefinedSeries> refinedSeriesList, ChartUpdateType updateType)
			: base(refinedSeriesList, updateType) {
			this.axis = axis;
			this.groupToUpdate = groupToUpdate;
		}
	}
	internal class ScaleSortingUpdate : ChartUpdate {
		readonly RefinedSeriesGroup groupToUpdate;
		public RefinedSeriesGroup GroupToUpdate { get { return groupToUpdate; } }
		internal ScaleSortingUpdate(RefinedSeriesGroup groupToUpdate, ChartUpdateType updateType)
			: base(updateType) {
			this.groupToUpdate = groupToUpdate;
		}
	}
	internal class SingleSeriesUpdate : ChartUpdate {
		readonly RefinedSeries refinedSeries;
		public RefinedSeries RefinedSeries { get { return refinedSeries; } }
		internal SingleSeriesUpdate(RefinedSeries refinedSeries, ChartUpdateType updateType)
			: base(updateType) {
			this.refinedSeries = refinedSeries;
		}
	}
	internal class SinglePointUpdate : SingleSeriesUpdate {
		readonly ChartCollectionOperation operation;
		readonly RefinedPoint affectedPoint;
		readonly RefinedPoint deprectedPoint;
		public RefinedPoint AffectedPoint { get { return affectedPoint; } }
		public RefinedPoint DeprectedPoint { get { return deprectedPoint; } }
		public ChartCollectionOperation Operation { get { return operation; } }
		internal SinglePointUpdate(ChartCollectionOperation operation, RefinedSeries refinedSeries, RefinedPoint affectedPoint)
			: this(operation, refinedSeries, ChartUpdateType.Empty, affectedPoint, null) {
		}
		internal SinglePointUpdate(ChartCollectionOperation operation, RefinedSeries refinedSeries, ChartUpdateType updateType, RefinedPoint affectedPoint)
			: this(operation, refinedSeries, updateType, affectedPoint, null) {
		}
		internal SinglePointUpdate(ChartCollectionOperation operation, RefinedSeries refinedSeries, ChartUpdateType updateType, RefinedPoint affectedPoint, RefinedPoint deprectedPoint)
			: base(refinedSeries, updateType) {
			this.operation = operation;
			this.affectedPoint = affectedPoint;
			this.deprectedPoint = deprectedPoint;
		}
	}
	internal class BatchPointsUpdate : SingleSeriesUpdate {
		readonly ChartCollectionOperation operation;
		readonly RefinedPoint[] affectedPoints;
		public RefinedPoint[] AffectedPoints { get { return affectedPoints; } }
		public ChartCollectionOperation Operation { get { return operation; } }
		public BatchPointsUpdate(ChartCollectionOperation operation, RefinedSeries refinedSeries, RefinedPoint[] affectedPoints)
			: this(operation, refinedSeries, ChartUpdateType.Empty, affectedPoints) {
		}
		public BatchPointsUpdate(ChartCollectionOperation operation, RefinedSeries refinedSeries, ChartUpdateType updateType, RefinedPoint[] affectedPoints)
			: base(refinedSeries, updateType) {
			this.operation = operation;
			this.affectedPoints = affectedPoints;
		}
	}
	public abstract class ChartUpdateInfoBase {
		readonly object sender;
		public object Sender {
			get {
				return sender;
			}
		}
		public ChartUpdateInfoBase(object sender) {
			this.sender = sender;
		}
	}
	public abstract class CollectionUpdateInfo : ChartUpdateInfoBase {
		readonly ChartCollectionOperation operation;
		readonly int oldIndex;
		readonly int newIndex;
		readonly object oldItem;
		readonly object newItem;
		public ChartCollectionOperation Operation { get { return operation; } }
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public object OldItem { get { return oldItem; } }
		public object NewItem { get { return newItem; } }
		public CollectionUpdateInfo(object sender, ChartCollectionOperation operation, int oldIndex, int newIndex, object oldItem, object newItem)
			: base(sender) {
			this.operation = operation;
			this.oldIndex = oldIndex;
			this.newIndex = newIndex;
			this.oldItem = oldItem;
			this.newItem = newItem;
		}
	}
	public class CollectionUpdateInfo<T> : CollectionUpdateInfo {
		public new T OldItem { get { return (T)base.OldItem; } }
		public new  T NewItem { get { return (T)base.NewItem; } }
		public CollectionUpdateInfo(object sender, ChartCollectionOperation operation, T oldItem, int oldIndex, T newItem, int newIndex)
			: base(sender, operation, oldIndex, newIndex, oldItem, newItem) {			
		}
	}
	public class SeriesCollectionUpdateInfo : CollectionUpdateInfo<ISeries> {
		public SeriesCollectionUpdateInfo(object sender, ChartCollectionOperation operation, ISeries oldItem, int oldIndex, ISeries newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) { }
	}
	public class SeriesCollectionBatchUpdateInfo : CollectionUpdateInfo<ICollection<ISeries>> {
		public SeriesCollectionBatchUpdateInfo(object sender, ChartCollectionOperation operation, ICollection<ISeries> oldItem, int oldIndex, ICollection<ISeries> newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) { }
	}
	public class SeriesPointCollectionUpdateInfo : CollectionUpdateInfo<ISeriesPoint> {
		readonly ISeries series;
		public ISeries Series { get { return series; } }
		public SeriesPointCollectionUpdateInfo(object sender, ChartCollectionOperation operation, ISeries series, ISeriesPoint oldItem, int oldIndex, ISeriesPoint newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) {
			this.series = series;
		}
	}
	public class SeriesPointCollectionBatchUpdateInfo : CollectionUpdateInfo<ICollection<ISeriesPoint>> {
		public static SeriesPointCollectionBatchUpdateInfo CreateInsertInfo(object owner, ISeries series, ICollection<ISeriesPoint> newItem, int newIndex) {
			return new SeriesPointCollectionBatchUpdateInfo(owner, ChartCollectionOperation.InsertItem, series, null, -1, newItem, newIndex);
		}
		readonly ISeries series;
		public ISeries Series { get { return series; } }
		public SeriesPointCollectionBatchUpdateInfo(object sender, ChartCollectionOperation operation, ISeries series, ICollection<ISeriesPoint> oldItem, int oldIndex, ICollection<ISeriesPoint> newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) {
			this.series = series;
		}
	}
	public class RandomPointsBatchUpdateInfo : SeriesPointCollectionBatchUpdateInfo {
		public static RandomPointsBatchUpdateInfo CreateResetInfo(ISeries series) {
			return new RandomPointsBatchUpdateInfo(series, ChartCollectionOperation.Reset, series, null, -1, null, -1);
		}
		public static RandomPointsBatchUpdateInfo CreateInsertInfo(ISeries series) {
			IList<ISeriesPoint> randomPoints = null;
			if (series.SeriesView != null)
				randomPoints = series.SeriesView.GenerateRandomPoints(series.ArgumentScaleType, series.ValueScaleType);
			return new RandomPointsBatchUpdateInfo(series, ChartCollectionOperation.InsertItem, series, null, -1, randomPoints, 0);
		}
		public RandomPointsBatchUpdateInfo(object sender, ChartCollectionOperation operation, ISeries series, ICollection<ISeriesPoint> oldItem, int oldIndex, ICollection<ISeriesPoint> newItem, int newIndex)
			: base(sender, operation, series, oldItem, oldIndex, newItem, newIndex) { }
	}
	public class AxisCollectionUpdateInfo : CollectionUpdateInfo<IAxisData> {
		public AxisCollectionUpdateInfo(object sender, ChartCollectionOperation operation, IAxisData oldItem, int oldIndex, IAxisData newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) { }
	}
	public class AxisCollectionBatchUpdateInfo : CollectionUpdateInfo<ICollection<IAxisData>> {
		public AxisCollectionBatchUpdateInfo(object sender, ChartCollectionOperation operation, ICollection<IAxisData> oldItem, int oldIndex, ICollection<IAxisData> newItem, int newIndex)
			: base(sender, operation, oldItem, oldIndex, newItem, newIndex) { }
	}
	public class PropertyUpdateInfo : ChartUpdateInfoBase {
		readonly string name;
		public string Name { get { return name; } }		
		public PropertyUpdateInfo(object sender, string name)
			: base(sender) {
			this.name = name;
		}
	}
	public class PropertyUpdateInfo<T> : PropertyUpdateInfo {
		readonly T oldValue;
		readonly T newValue;
		public T OldValue {
			get {
				return oldValue;
			}
		}
		public T NewValue {
			get {
				return newValue;
			}
		}
		public PropertyUpdateInfo(object sender, string name, T oldValue, T newValue)
			: base(sender, name) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public class PropertyUpdateInfo<O, T> : PropertyUpdateInfo<T> {
		readonly O owner;
		public O Owner { get { return owner; } }
		public PropertyUpdateInfo(object sender, string name, T oldValue, T newValue, O owner)
			: base(sender, name, oldValue, newValue) {
			this.owner = owner;
		}
	}
	public class AxisElementUpdateInfo : PropertyUpdateInfo {
		readonly IAxisData axis;
		public IAxisData Axis { get { return axis; } }
		public AxisElementUpdateInfo(object sender, IAxisData axis) :
			base(sender, string.Empty) {
			this.axis = axis;
		}
	}
	public class RangeResetUpdateInfo : ChartUpdateInfoBase {
		public IAxisRangeData Range { get { return (IAxisRangeData)Sender; } }
		public RangeResetUpdateInfo(IAxisRangeData sender) : base(sender) { }
	}
	public class LightUpdateInfo : ChartUpdateInfoBase {
		public LightUpdateInfo(object sender)
			: base(sender) { }
	}
	public class RangeChangedUpdateInfo : LightUpdateInfo {
		readonly bool isRangeControl;
		readonly bool isArgumentAxis;
		readonly IAxisData axis;
		public bool IsRangeControl { get { return isRangeControl; } }
		public bool IsArgumentAxis { get { return isArgumentAxis; } }
		public IAxisData Axis { get { return axis; } }
		public RangeChangedUpdateInfo(IAxisRangeData sender, IAxisData axis, bool needUpdate, bool isArgumentAxis)
			: base(sender) {
			this.axis = axis;
			this.isRangeControl = needUpdate;
			this.isArgumentAxis = isArgumentAxis;
		}
	}
	public class OnLoadEndUpdateInfo : ChartUpdateInfoBase {
		public OnLoadEndUpdateInfo(object sender) : base(sender) { }
	}
	public class SeriesGroupsInteractionUpdateInfo : ChartUpdateInfoBase {
		public SeriesGroupsInteractionUpdateInfo(object sender) : base(sender) { }
	}
	public class RefreshDataUpdateInfo : ChartUpdateInfoBase {
		public RefreshDataUpdateInfo(object sender)
			: base(sender) { }
	}
	public class ReprocessPointsUpdate : ChartUpdateInfoBase {
		public ISeries Series {
			get {
				return (ISeries)Sender;
			}
		}
		public ReprocessPointsUpdate(ISeries series)
			: base(series) {
		}
	}
	public class DataAggregationUpdate : ChartUpdateInfoBase {
		public new IAxisData Sender { get { return (IAxisData)base.Sender; } }
		public DataAggregationUpdate(IAxisData axis) : base(axis) { }
	}
	public class SeriesBindingUpdate : ChartUpdateInfoBase {
		public new ISeries Sender { get { return (ISeries)base.Sender; } }
		public SeriesBindingUpdate(ISeries series) : base(series) { }
	}
}
