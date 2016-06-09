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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections;
using System.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	#region AppointmentCellIndexes
	public class AppointmentCellIndexes {
		int firstCellIndex;
		int lastCellIndex;
		public AppointmentCellIndexes() {
			firstCellIndex = 0;
			lastCellIndex = 0;
		}
		public AppointmentCellIndexes(int firstCellIndex, int lastCellIndex) {
			this.firstCellIndex = firstCellIndex;
			this.lastCellIndex = lastCellIndex;
		}
		public int FirstCellIndex { get { return firstCellIndex; } set { firstCellIndex = value; } }
		public int LastCellIndex { get { return lastCellIndex; } set { lastCellIndex = value; } }
	}
	#endregion
	#region AppointmentCellIndexesCollection
	public class AppointmentCellIndexesCollection : DXCollection<AppointmentCellIndexes> {
	}
	#endregion AppointmentCellIndexesCollection
	#region PreparedAppointments
	public class PreparedAppointments {
		AppointmentBaseCollection appointments;
		IVisuallyContinuousCellsInfoCore cellsInfo;
		public PreparedAppointments(AppointmentBaseCollection appointments, IVisuallyContinuousCellsInfoCore cellsInfo) {
			Appointments = appointments;
			CellsInfo = cellsInfo;
		}
		protected internal virtual AppointmentBaseCollection Appointments {
			get { return appointments; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("appointments", appointments);
				else
					appointments = value;
			}
		}
		protected internal virtual IVisuallyContinuousCellsInfoCore CellsInfo {
			get { return cellsInfo; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("cellsInfo", cellsInfo);
				else
					cellsInfo = value;
			}
		}
	}
	#endregion
	#region PreparedAppointmentsCollection
	public class PreparedAppointmentsCollection : List<PreparedAppointments> {
	}
	#endregion
	#region IAppointmentsLayoutResult
	public interface IAppointmentsLayoutResult {
		void Merge(IAppointmentsLayoutResult layoutResult);
		IAppointmentViewInfoCollection AppointmentViewInfos { get; }
	}
	#endregion
	#region AppointmentsLayoutResultCore (abstract class)
	public abstract class AppointmentsLayoutResultCore<TAppointmentViewInfoCollection>
		where TAppointmentViewInfoCollection : IAppointmentViewInfoCollection, new() {
		TAppointmentViewInfoCollection appointmentViewInfos;
		protected AppointmentsLayoutResultCore() {
			appointmentViewInfos = new TAppointmentViewInfoCollection();
		}
		public TAppointmentViewInfoCollection AppointmentViewInfos {
			get { return appointmentViewInfos; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("value", value);
				appointmentViewInfos = value;
			}
		}
		public virtual void Merge(AppointmentsLayoutResultCore<TAppointmentViewInfoCollection> layoutResult) {
			AppointmentViewInfos.AddRange(layoutResult.AppointmentViewInfos);
		}
	}
	#endregion
	#region IAppointmentView
	public interface IAppointmentView {
		Appointment Appointment { get; }
	}
	#endregion
	#region IAppointmentViewInfo
	public interface IAppointmentViewInfo : IAppointmentView {
		TimeInterval AppointmentInterval { get; }
		TimeInterval Interval { get; set; }
		Resource Resource { get; set; }
		bool HasTopBorder { get; set; }
		bool HasBottomBorder { get; set; }
		bool HasLeftBorder { get; set; }
		bool HasRightBorder { get; set; }
		AppointmentViewInfoOptions Options { get; }
		bool IsLongTime();
	}
	#endregion
	#region IAppointmentIntermediateLayoutViewInfo
	public interface IAppointmentIntermediateLayoutViewInfo {
		AppointmentCellIndexes CellIndexes { get; set; }
		TimeInterval Interval { get; }
		int FirstIndexPosition { get; set; }
		int FirstCellIndex { get; }
		int LastCellIndex { get; }
		int MaxIndexInGroup { get; set; }
		int LastIndexPosition { get; set; }
	}
	#endregion
	#region AppointmentIntermediateViewInfoCore
	public abstract class AppointmentIntermediateViewInfoCore : IAppointmentIntermediateLayoutViewInfo, INotifyPropertyChanged {
		#region Fields
		IAppointmentViewInfo viewInfo;
		AppointmentCellIndexes cellIndexes;
		bool hasStartBorder;
		bool hasEndBorder;
		int firstIndexPosition;
		int lastIndexPosition;
		int maxIndexInGroup;
		int startRelativeOffset;
		int endRelativeOffset;
		#endregion
		#region Properties
		public bool HasStartBorder {
			get { return hasStartBorder; }
			set {
				if (HasStartBorder == value)
					return;
				hasStartBorder = value;
				RaisePropertyChanged("HasStartBorder");
			}
		}
		public bool HasEndBorder {
			get { return hasEndBorder; }
			set {
				if (HasEndBorder == value)
					return;
				hasEndBorder = value;
				RaisePropertyChanged("HasEndBorder");
			}
		}
		public int FirstCellIndex {
			get { return cellIndexes.FirstCellIndex; }
			set {
				if (CellIndexes.FirstCellIndex == value)
					return;
				cellIndexes.FirstCellIndex = value;
				RaisePropertyChanged("FirstCellIndex");
			}
		}
		public int LastCellIndex {
			get { return cellIndexes.LastCellIndex; }
			set {
				if (cellIndexes.LastCellIndex == value)
					return;
				cellIndexes.LastCellIndex = value;
				RaisePropertyChanged("LastCellIndex");
			}
		}
		public AppointmentCellIndexes CellIndexes {
			get { return cellIndexes; }
			set {
				if (CellIndexes == value)
					return;
				cellIndexes = value;
				RaisePropertyChanged("CellIndexes");
			}
		}
		public IAppointmentViewInfo ViewInfo { get { return viewInfo; } }
		public Appointment Appointment { get { return viewInfo.Appointment; } }
		public TimeInterval AppointmentInterval { get { return viewInfo.AppointmentInterval; } }
		public int FirstIndexPosition {
			get { return firstIndexPosition; }
			set {
				if (FirstIndexPosition == value)
					return;
				firstIndexPosition = value;
				RaisePropertyChanged("FirstIndexPosition");
			}
		}
		public int LastIndexPosition {
			get { return lastIndexPosition; }
			set {
				if (LastIndexPosition == value)
					return;
				lastIndexPosition = value;
				RaisePropertyChanged("LastIndexPosition");
			}
		}
		public int MaxIndexInGroup {
			get { return maxIndexInGroup; }
			set {
				if (MaxIndexInGroup == value)
					return;
				maxIndexInGroup = value;
				RaisePropertyChanged("MaxIndexInGroup");
			}
		}
		public int StartRelativeOffset {
			get { return startRelativeOffset; }
			set {
				if (StartRelativeOffset == value)
					return;
				startRelativeOffset = value;
				RaisePropertyChanged("StartRelativeOffset");
			}
		}
		public int EndRelativeOffset {
			get { return endRelativeOffset; }
			set {
				if (EndRelativeOffset == value)
					return;
				endRelativeOffset = value;
				RaisePropertyChanged("EndRelativeOffset");
			}
		}
		#endregion
		protected AppointmentIntermediateViewInfoCore(IAppointmentViewInfo viewInfo) {
			cellIndexes = new AppointmentCellIndexes();
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
			this.viewInfo = viewInfo;
		}
		#region IAppointmentIntermediateLayoutViewInfo Members
		public TimeInterval Interval { get { return ViewInfo.Interval; } }
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	#region AppointmentIntermediateViewInfoCoreComparer
	public class AppointmentIntermediateViewInfoCoreComparer<TAppointmentIntermediateViewInfo> : IComparer<TAppointmentIntermediateViewInfo>
		where TAppointmentIntermediateViewInfo : AppointmentIntermediateViewInfoCore {
		AppointmentBaseComparer aptComparer;
		public AppointmentIntermediateViewInfoCoreComparer(AppointmentBaseComparer aptComparer) {
			Guard.ArgumentNotNull(aptComparer, "aptComparer");
			this.aptComparer = aptComparer;
		}
		public int Compare(TAppointmentIntermediateViewInfo viewInfoX, TAppointmentIntermediateViewInfo viewInfoY) {
			return aptComparer.Compare(viewInfoX.Appointment, viewInfoY.Appointment);
		}
	}
	#endregion
	#region IAppointmentIntermediateLayoutViewInfoCoreCollection
	public interface IAppointmentIntermediateLayoutViewInfoCoreCollection {
		IAppointmentIntermediateLayoutViewInfo this[int index] { get; }
		int Count { get; }
	}
	#endregion
	#region IAppointmentIntermediateViewInfoCoreCollection
	public interface IAppointmentIntermediateViewInfoCoreCollection {
		AppointmentIntermediateViewInfoCore this[int index] { get; }
		int Count { get; }
		ManualResetEventSlim Signal { get; }
		int Add(AppointmentIntermediateViewInfoCore value);
		bool Remove(AppointmentIntermediateViewInfoCore value);
		void Sort(AppointmentBaseComparer aptComparer);
		void Clear();
		Resource Resource { get; }
		TimeInterval Interval { get; }
		List<IAppointmentIntermediateLayoutViewInfoCoreCollection> GroupedViewInfos { get; }
	}
	#endregion
	public interface IAppointmentViewInfoCollection {
		void AddRange(IAppointmentViewInfoCollection value);
	}
	public interface IAppointmentViewInfoContainer<out TViewInfoCollection> where TViewInfoCollection : IAppointmentViewInfoCollection {
		TViewInfoCollection AppointmentViewInfos { get; }
	}
	#region AppointmentIntermediateViewInfoCoreCollection
	public class AppointmentIntermediateViewInfoCoreCollection : DXCollection<AppointmentIntermediateViewInfoCore>, IAppointmentIntermediateViewInfoCoreCollection, IAppointmentIntermediateLayoutViewInfoCoreCollection {
		Resource resource;
		TimeInterval interval;
		List<IAppointmentIntermediateLayoutViewInfoCoreCollection> groupedViewInfos;
		ManualResetEventSlim signal;
		public AppointmentIntermediateViewInfoCoreCollection(Resource resource, TimeInterval interval) {
			this.resource = resource;
			this.interval = interval;
			groupedViewInfos = new List<IAppointmentIntermediateLayoutViewInfoCoreCollection>();
			signal = new ManualResetEventSlim();
		}
		public AppointmentIntermediateViewInfoCoreCollection() {
		}
		public void Sort(AppointmentBaseComparer aptComparer) {
			base.Sort(new AppointmentIntermediateViewInfoCoreComparer<AppointmentIntermediateViewInfoCore>(aptComparer));
		}
		#region IAppointmentIntermediateLayoutViewInfoCoreCollection Members
		IAppointmentIntermediateLayoutViewInfo IAppointmentIntermediateLayoutViewInfoCoreCollection.this[int index] { get { return this[index]; } }
		#endregion
		#region IAppointmentIntermediateViewInfoCoreCollection members
		public Resource Resource { get { return this.resource; } }
		public TimeInterval Interval { get { return this.interval; } }
		public List<IAppointmentIntermediateLayoutViewInfoCoreCollection> GroupedViewInfos { get { return this.groupedViewInfos; } }
		public ManualResetEventSlim Signal {
			get { return signal; }
		}
		#endregion
	}
	#endregion
	#region DateTimeCollection
	public class DateTimeCollection : DXCollection<DateTime> {
		public DateTimeCollection() {
		}
		public int BinarySearch(DateTime dateTime) {
			return BinarySearch(dateTime, Comparer<DateTime>.Default);
		}
	}
	#endregion
	#region BusyIndexCollection
	public class BusyIndexCollection : DXCollection<int> {
		protected override void InsertIfNotAlreadyInserted(int index, int obj) {
			if (!UniquenessProvider.LookupObject(obj))
				InsertCore(index, obj);
		}
		public int BinarySearch(int obj) {
			return base.BinarySearch(obj, Comparer<int>.Default);
		}
		public void Sort() {
			base.Sort(Comparer<int>.Default);
		}
	}
	#endregion
	#region AppointmentIndexesCalculator
	public class AppointmentIndexesCalculator {
		BusyIndexCollection[] busyIndexes;
		int nextAvailableIndex;
		public AppointmentIndexesCalculator() {
			nextAvailableIndex = 1;
		}
		protected internal int NextAvailableIndex { get { return nextAvailableIndex; } set { nextAvailableIndex = value; } }
		internal BusyIndexCollection[] BusyIndexes { get { return busyIndexes; } set { busyIndexes = value; } }
		public virtual void CalculateAppointmentIndexes(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			AppointmentCellIndexesCollection previousCellIndexes = CreateAppointmentCellIndexesCollection(viewInfos);
			AdjustAppointmentCellIndexes(token, viewInfos);
			CalculateAppointmentIndexesCore(token, viewInfos);
			RestoreCellIndexes(token, viewInfos, previousCellIndexes);
		}
		public virtual void AdjustAppointmentCellIndexes(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			DateTimeCollection viewInfosDateTime = CreateViewInfosDateTimeCollection(viewInfos);
			CalculateAdjustedCellIndexes(token, viewInfos, viewInfosDateTime);
		}
		public virtual AppointmentCellIndexesCollection CreateAppointmentCellIndexesCollection(IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			AppointmentCellIndexesCollection cellIndexes = new AppointmentCellIndexesCollection();
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				cellIndexes.Add(viewInfos[i].CellIndexes);
			return cellIndexes;
		}
		public virtual void RestoreCellIndexes(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos, AppointmentCellIndexesCollection cellindexes) {
			if (!token.IsCancellationRequested)
				XtraSchedulerDebug.Assert(viewInfos.Count == cellindexes.Count);
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				viewInfos[i].CellIndexes = cellindexes[i];
			}
		}
		protected internal virtual DateTimeCollection CreateViewInfosDateTimeCollection(IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			int count = viewInfos.Count;
			DateTimeCollection dateTimeCollection = new DateTimeCollection();
			for (int i = 0; i < count; i++) {
				TimeInterval viewInfoInterval = viewInfos[i].Interval;
				dateTimeCollection.Add(viewInfoInterval.Start);
				dateTimeCollection.Add(viewInfoInterval.End);
			}
			dateTimeCollection.Sort(Comparer<DateTime>.Default);
			return dateTimeCollection;
		}
		protected internal virtual void CalculateAdjustedCellIndexes(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos, DateTimeCollection viewInfosDateTime) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				CalculateAdjustedCellIndexesCore(viewInfos[i], viewInfosDateTime);
			}
		}
		protected internal virtual void CalculateAdjustedCellIndexesCore(IAppointmentIntermediateLayoutViewInfo intermediateViewInfo, DateTimeCollection viewInfosDateTimeCollection) {
			TimeInterval interval = intermediateViewInfo.Interval;
			int firstCellIndex = viewInfosDateTimeCollection.BinarySearch(interval.Start);
			int lastCellIndex = viewInfosDateTimeCollection.BinarySearch(interval.End) - 1;
			intermediateViewInfo.CellIndexes = new AppointmentCellIndexes(firstCellIndex, lastCellIndex);
		}
		protected internal virtual void CreateBusyIndexes(int count) {
			busyIndexes = new BusyIndexCollection[count];
			for (int i = 0; i < busyIndexes.Length; i++)
				busyIndexes[i] = new BusyIndexCollection();
		}
		protected internal virtual void CalculateAppointmentIndexesCore(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			int count = viewInfos.Count;
			CreateBusyIndexes(2 * count);
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				IAppointmentIntermediateLayoutViewInfo viewInfo = viewInfos[i];
				int index = FindMinimalAvailableIndex(viewInfo);
				viewInfo.FirstIndexPosition = index;
				nextAvailableIndex = MakeIndexBusy(viewInfo, index);
			}
		}
		protected internal virtual int FindMinimalAvailableIndex(IAppointmentIntermediateLayoutViewInfo viewInfo) {
			for (int i = 0; i < nextAvailableIndex; i++) {
				if (IsIndexAvailable(viewInfo, i))
					return i;
			}
			return nextAvailableIndex;
		}
		protected internal virtual int MakeIndexBusy(IAppointmentIntermediateLayoutViewInfo viewInfo, int index) {
			int from = viewInfo.FirstCellIndex;
			int to = viewInfo.LastCellIndex;
			for (int i = from; i <= to; i++)
				busyIndexes[i].Add(index);
			return Math.Max(index + 1, nextAvailableIndex);
		}
		protected internal virtual bool IsIndexAvailable(IAppointmentIntermediateLayoutViewInfo viewInfo, int index) {
			int from = viewInfo.FirstCellIndex;
			int to = viewInfo.LastCellIndex;
			for (int i = from; i <= to; i++) {
				if (busyIndexes[i].Contains(index))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region VerticalAppointmentIndexesCalculator
	public class VerticalAppointmentIndexesCalculator : AppointmentIndexesCalculator {
		protected internal override void CalculateAppointmentIndexesCore(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			base.CalculateAppointmentIndexesCore(token, viewInfos);
			SortBusyIndexes(token);
			CalculateVerticalAppointmentIndexes(token, viewInfos);
		}
		protected internal virtual void CalculateVerticalAppointmentIndexes(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				IAppointmentIntermediateLayoutViewInfo viewInfo = viewInfos[i];
				viewInfo.MaxIndexInGroup = NextAvailableIndex - 1;
				viewInfo.LastIndexPosition = FindMaxLastIndex(viewInfo);
			}
		}
		protected internal virtual int FindMaxLastIndex(IAppointmentIntermediateLayoutViewInfo viewInfo) {
			int firstIndex = viewInfo.FirstIndexPosition;
			int maxLastIndex = viewInfo.MaxIndexInGroup;
			for (int i = viewInfo.FirstCellIndex; i <= viewInfo.LastCellIndex; i++) {
				int firstIndexPosition = BusyIndexes[i].BinarySearch(firstIndex);
				int lastIndex = (firstIndexPosition < BusyIndexes[i].Count - 1) ? BusyIndexes[i][firstIndexPosition + 1] - 1 : viewInfo.MaxIndexInGroup;
				maxLastIndex = Math.Min(maxLastIndex, lastIndex);
			}
			return maxLastIndex;
		}
		protected internal virtual void SortBusyIndexes(CancellationToken token) {
			int count = BusyIndexes.Length;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				BusyIndexes[i].Sort(Comparer<int>.Default);
			}
		}
	}
	#endregion
	#region TimeScaleHelper
	internal class AppointmentTimeScaleHelper {
		internal struct TimeOffset {
			public float startOffset;
			public float endOffset;
			public TimeOffset(float startOffset, float endOffset) {
				this.startOffset = startOffset;
				this.endOffset = endOffset;
			}
		}
		public static TimeInterval SnapToScale(TimeInterval interval, TimeScale scale) {
			DateTime start = scale.Floor(interval.Start);
			DateTime end = scale.Floor(interval.End);
			TimeInterval result = new TimeInterval(start, end);
			AdjustInterval(result, interval, scale);
			return result;
		}
		static void AdjustInterval(TimeInterval snappedInterval, TimeInterval interval, TimeScale scale) {
			if ((snappedInterval.Duration == TimeSpan.Zero) || (snappedInterval.End != interval.End))
				snappedInterval.End = scale.GetNextDate(snappedInterval.End);
		}
		public static TimeInterval SnapToScale(TimeInterval interval, TimeScaleFixedInterval scale, DateTime baseDate) {
			DateTime start = scale.Floor(interval.Start, baseDate);
			DateTime end = scale.Floor(interval.End, baseDate);
			TimeInterval result = new TimeInterval(start, end);
			AdjustInterval(result, interval, scale);
			return result;
		}
		internal static TimeOffset CalculateOffset(TimeInterval baseInterval, TimeInterval interval) {
			float startOffset = CalculateStartOffsetFloat(interval.Start, baseInterval);
			float endOffset = CalculateEndOffsetFloat(interval.End, baseInterval);
			return new TimeOffset(startOffset, endOffset);
		}
		public static int CalculateStartOffset(DateTime start, TimeInterval interval) {
			return (int)Math.Round(CalculateStartOffsetFloat(start, interval));
		}
		public static int CalculateEndOffset(DateTime start, TimeInterval interval) {
			return (int)Math.Round(CalculateEndOffsetFloat(start, interval));
		}
		public static float CalculateStartOffsetFloat(DateTime start, TimeInterval interval) {
			float duration = interval.Duration.Ticks;
			if (duration != 0) {
				float startOffset = ((float)(start.Ticks - interval.Start.Ticks) * 100 / duration);
				return Math.Max(0, startOffset);
			} else
				return 0;
		}
		public static float CalculateEndOffsetFloat(DateTime end, TimeInterval interval) {
			float duration = interval.Duration.Ticks;
			if (duration != 0) {
				float endOffset = ((float)(interval.End.Ticks - end.Ticks) * 100 / interval.Duration.Ticks);
				return Math.Max(0, endOffset);
			} else
				return 0;
		}
		public static DateTime CalculateStartTimeByOffset(TimeInterval baseInterval, int startOffset) {
			long offset = (long)baseInterval.Duration.Ticks * startOffset / 100;
			return baseInterval.Start.AddTicks(offset);
		}
		public static DateTime CalculateEndTimeByOffset(TimeInterval baseInterval, int endOffset) {
			long offset = -(long)baseInterval.Duration.Ticks * endOffset / 100;
			return baseInterval.End.AddTicks(offset);
		}
		public static Rectangle ScaleRectangleHorizontally(Rectangle rect, TimeInterval baseInterval, TimeInterval interval) {
			TimeOffset offset = CalculateOffset(baseInterval, interval);
			int left = rect.Left + (int)(rect.Width * offset.startOffset * 0.01);
			int right = rect.Right - (int)(rect.Width * offset.endOffset * 0.01);
			return Rectangle.FromLTRB(left, rect.Top, right, rect.Bottom);
		}
		public static Rectangle ScaleRectangleVertically(Rectangle rect, TimeInterval baseInterval, TimeInterval interval) {
			TimeOffset offset = CalculateOffset(baseInterval, interval);
			int top = rect.Top + (int)(rect.Height * offset.startOffset * 0.01);
			int bottom = rect.Bottom - (int)(rect.Height * offset.endOffset * 0.01);
			return Rectangle.FromLTRB(rect.Left, top, rect.Right, bottom);
		}
	}
	#endregion
	#region AppointmentIntermediateLayoutCalculatorCore (abstract)
	public abstract class AppointmentIntermediateLayoutCalculatorCore {
		#region Fields
		TimeScale scale;
		AppointmentSnapToCellsMode snapToCellsMode;
		TimeZoneHelper timeZoneHelper;
		#endregion
		protected AppointmentIntermediateLayoutCalculatorCore(TimeScale scale, AppointmentSnapToCellsMode snapToCellsMode, TimeZoneHelper timeZoneEngine) {
			if (scale == null)
				Exceptions.ThrowArgumentNullException("scale");
			if (timeZoneEngine == null)
				Exceptions.ThrowArgumentNullException("timeZoneHelper");
			this.scale = scale;
			this.snapToCellsMode = snapToCellsMode;
			this.timeZoneHelper = timeZoneEngine;
		}
		#region Properties
		protected internal virtual TimeScale Scale { get { return scale; } }
		protected internal virtual AppointmentSnapToCellsMode SnapToCellsMode { get { return snapToCellsMode; } }
		protected internal TimeZoneHelper TimeZoneHelper { get { return timeZoneHelper; } }
		#endregion
		protected internal abstract void CalculateFinalCellIndexes(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval);
		protected internal abstract AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo);
		protected internal abstract IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection();
		public abstract IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(Resource resource, TimeInterval interval);
		public virtual void CalculateIntermediateAppointmentViewInfos(CancellationToken token, IAppointmentIntermediateViewInfoCoreCollection viewInfos, AppointmentBaseCollection apts, IVisuallyContinuousCellsInfoCore cellsInfo) {
			CalculateIntermediateAppointmentViewInfosCore(token, viewInfos, apts, cellsInfo);
			RemoveInvisibleViewInfos(token, viewInfos);
		}
		protected internal virtual void CalculateIntermediateAppointmentViewInfosCore(CancellationToken token, IAppointmentIntermediateViewInfoCoreCollection viewInfos, AppointmentBaseCollection apts, IVisuallyContinuousCellsInfoCore cellsInfo) {
			int count = apts.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				Appointment apt = apts[i];
				if (CanLayoutAppointment(apt)) {
					AppointmentIntermediateViewInfoCore intermediateViewInfo = CalculateIntermediateAppointmentViewInfo(apts[i], cellsInfo);
					viewInfos.Add(intermediateViewInfo);
				}
			}
		}
		protected internal virtual bool CanLayoutAppointment(Appointment apt) {
			return true;
		}
		protected internal virtual void RemoveInvisibleViewInfos(CancellationToken token, IAppointmentIntermediateViewInfoCoreCollection viewInfos) {
			IAppointmentIntermediateViewInfoCoreCollection filteredViewInfos = CreateIntermediateViewInfoCollection(viewInfos.Resource, viewInfos.Interval);
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				AppointmentIntermediateViewInfoCore viewInfo = viewInfos[i];
				if (viewInfo.FirstCellIndex <= viewInfo.LastCellIndex)
					filteredViewInfos.Add(viewInfo);
			}
			viewInfos.Clear();
			count = filteredViewInfos.Count;
			for (int i = 0; i < count; i++)
				viewInfos.Add(filteredViewInfos[i]);
		}
		protected internal virtual AppointmentIntermediateViewInfoCore CalculateIntermediateAppointmentViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			AppointmentIntermediateViewInfoCore intermediateViewInfo = CreateAppointmentIntermediateViewInfo(apt, cellsInfo);
			if (ShouldSnapToCells(intermediateViewInfo, cellsInfo))
				CalculateViewInfoPropertiesSnapToCell(intermediateViewInfo, cellsInfo);
			else
				CalculateViewInfoPropertiesExactTime(intermediateViewInfo, cellsInfo);
			return intermediateViewInfo;
		}
		protected internal virtual bool ShouldSnapToCells(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo) {
			switch (SnapToCellsMode) {
				case AppointmentSnapToCellsMode.Never:
					return false;
				case AppointmentSnapToCellsMode.Disabled:
					return false;
				case AppointmentSnapToCellsMode.Auto:
					return IsViewInfoLessThanACell(intermediateViewInfo, cellsInfo);
				case AppointmentSnapToCellsMode.Always:
				default:
					return true;
			}
		}
		protected internal virtual void CalculateViewInfoPropertiesSnapToCell(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo) {
			TimeInterval viewInfoInterval = SnapToCells(intermediateViewInfo.AppointmentInterval, cellsInfo.Interval);
			CalculateViewInfoPropertiesCore(intermediateViewInfo, cellsInfo, viewInfoInterval);
		}
		protected internal virtual void CalculateViewInfoPropertiesExactTime(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo) {
			TimeInterval aptInterval = intermediateViewInfo.AppointmentInterval;
			CalculateViewInfoPropertiesCore(intermediateViewInfo, cellsInfo, aptInterval);
		}
		protected internal virtual TimeInterval SnapToCells(TimeInterval viewInfoInterval, TimeInterval cellsInterval) {
			return AppointmentTimeScaleHelper.SnapToScale(viewInfoInterval, Scale);
		}
		protected internal virtual bool IsViewInfoLessThanACell(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo) {
			TimeInterval viewInfoInterval = TimeInterval.Intersect(intermediateViewInfo.AppointmentInterval, cellsInfo.Interval);
			TimeInterval snappedInterval = SnapToCells(viewInfoInterval, cellsInfo.Interval);
			CalculatePreliminaryCellIndexes(intermediateViewInfo, cellsInfo, snappedInterval);
			return intermediateViewInfo.LastCellIndex - intermediateViewInfo.FirstCellIndex < 2;
		}
		protected internal virtual void CalculateViewInfoPropertiesCore(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval) {
			TimeInterval snappedInterval = SnapToCells(viewInfoInterval, cellsInfo.Interval);
			CalculatePreliminaryCellIndexes(intermediateViewInfo, cellsInfo, snappedInterval);
			intermediateViewInfo.HasStartBorder = intermediateViewInfo.FirstCellIndex >= 0;
			intermediateViewInfo.HasEndBorder = intermediateViewInfo.LastCellIndex >= 0;
			CalculateFinalCellIndexes(intermediateViewInfo, cellsInfo, snappedInterval);
			CalculateViewInfoIntervalAndOffset(intermediateViewInfo, cellsInfo, viewInfoInterval);
		}
		protected internal virtual void CalculateViewInfoIntervalAndOffset(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval) {
			int lastIndex = intermediateViewInfo.LastCellIndex;
			int firstIndex = intermediateViewInfo.FirstCellIndex;
			if (firstIndex <= lastIndex) {
				TimeInterval firstCellInterval = cellsInfo.GetIntervalByIndex(firstIndex);
				TimeInterval lastCellsInterval = cellsInfo.GetIntervalByIndex(lastIndex);
				TimeInterval cellsInterval = new TimeInterval(firstCellInterval.Start, lastCellsInterval.End);
				viewInfoInterval = TimeInterval.Intersect(viewInfoInterval, cellsInterval);
				int startRelativeOffset = AppointmentTimeScaleHelper.CalculateStartOffset(viewInfoInterval.Start, firstCellInterval);
				int endRelativeOffset = AppointmentTimeScaleHelper.CalculateEndOffset(viewInfoInterval.End, lastCellsInterval);
				AssignViewInfoIntervalAndOffset(intermediateViewInfo, viewInfoInterval, startRelativeOffset, endRelativeOffset);
			}
		}
		protected internal virtual void AssignViewInfoIntervalAndOffset(AppointmentIntermediateViewInfoCore intermediateViewInfo, TimeInterval viewInfoInterval, int startRelativeOffset, int endRelativeOffset) {
			intermediateViewInfo.ViewInfo.Interval = viewInfoInterval;
			intermediateViewInfo.StartRelativeOffset = startRelativeOffset;
			intermediateViewInfo.EndRelativeOffset = endRelativeOffset;
		}
		protected internal virtual void CalculatePreliminaryCellIndexes(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval) {
			intermediateViewInfo.FirstCellIndex = cellsInfo.GetIndexByStartDate(viewInfoInterval.Start);
			intermediateViewInfo.LastCellIndex = cellsInfo.GetIndexByEndDate(viewInfoInterval.End);
		}
	}
	#endregion
	#region HorizontalAppointmentIntermediateLayoutCalculatorCore
	public abstract class HorizontalAppointmentIntermediateLayoutCalculatorCore
		: AppointmentIntermediateLayoutCalculatorCore {
		protected HorizontalAppointmentIntermediateLayoutCalculatorCore(TimeScale scale, AppointmentSnapToCellsMode appointmentsSnapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, appointmentsSnapToCells, timeZoneEngine) {
		}
		protected internal virtual void RecalcFirstCellIndex(AppointmentIntermediateViewInfoCore intermediateViewInfo, TimeInterval viewInfoInterval, IVisuallyContinuousCellsInfoCore cellsInfo) {
			if (viewInfoInterval.Start > cellsInfo.Interval.Start)
				intermediateViewInfo.FirstCellIndex = Math.Max(0, cellsInfo.GetNextCellIndexByStartDate(viewInfoInterval.Start));
			else
				intermediateViewInfo.FirstCellIndex = 0;
		}
		protected internal virtual void RecalcLastCellIndex(AppointmentIntermediateViewInfoCore intermediateViewInfo, TimeInterval viewInfoInterval, IVisuallyContinuousCellsInfoCore cellsInfo) {
			int lastCellIndex = cellsInfo.Count - 1;
			if (viewInfoInterval.End < cellsInfo.Interval.End) {
				int previousCellIndex = cellsInfo.GetPreviousCellIndexByEndDate(viewInfoInterval.End);
				intermediateViewInfo.LastCellIndex = previousCellIndex < 0 ? lastCellIndex : previousCellIndex;
			} else
				intermediateViewInfo.LastCellIndex = lastCellIndex;
		}
		protected internal override void CalculateFinalCellIndexes(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval) {
			if (intermediateViewInfo.FirstCellIndex < 0)
				RecalcFirstCellIndex(intermediateViewInfo, viewInfoInterval, cellsInfo);
			if (intermediateViewInfo.LastCellIndex < 0)
				RecalcLastCellIndex(intermediateViewInfo, viewInfoInterval, cellsInfo);
		}
	}
	#endregion
	#region VerticalAppointmentIntermediateLayoutCalculatorCore
	public abstract class VerticalAppointmentIntermediateLayoutCalculatorCore
		: AppointmentIntermediateLayoutCalculatorCore {
		protected VerticalAppointmentIntermediateLayoutCalculatorCore(TimeScaleFixedInterval scale, AppointmentSnapToCellsMode appointmentsSnapToCells, TimeZoneHelper timeZoneHelper)
			: base(scale, appointmentsSnapToCells, timeZoneHelper) {
		}
		protected internal override void CalculateFinalCellIndexes(AppointmentIntermediateViewInfoCore intermediateViewInfo, IVisuallyContinuousCellsInfoCore cellsInfo, TimeInterval viewInfoInterval) {
			intermediateViewInfo.FirstCellIndex = Math.Max(0, intermediateViewInfo.FirstCellIndex);
			intermediateViewInfo.LastCellIndex = intermediateViewInfo.LastCellIndex < 0 ? cellsInfo.Count - 1 : intermediateViewInfo.LastCellIndex;
		}
		protected internal override TimeInterval SnapToCells(TimeInterval viewInfoInterval, TimeInterval cellsInterval) {
			return AppointmentTimeScaleHelper.SnapToScale(viewInfoInterval, (TimeScaleFixedInterval)Scale, cellsInterval.Start);
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculatorHelperCore
	public class VerticalAppointmentLayoutCalculatorHelperCore<TVisuallyContinuousCellsInfo>
		where TVisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfoCore {
		public virtual List<IAppointmentIntermediateLayoutViewInfoCoreCollection> LayoutViewInfos(CancellationToken token, TVisuallyContinuousCellsInfo cellsInfo, IAppointmentIntermediateViewInfoCoreCollection intermediateResult) {
			List<IAppointmentIntermediateLayoutViewInfoCoreCollection> groupedViewInfosList = GroupIntersectedViewInfos(intermediateResult);
			for (int i = 0; i < groupedViewInfosList.Count; i++) {
				if (token.IsCancellationRequested)
					return groupedViewInfosList;
				IAppointmentIntermediateLayoutViewInfoCoreCollection groupedViewInfos = groupedViewInfosList[i];
				LayoutGroupedViewInfosHorizontally(token, groupedViewInfos, cellsInfo);
			}
			return groupedViewInfosList;
		}
		protected internal virtual List<IAppointmentIntermediateLayoutViewInfoCoreCollection> GroupIntersectedViewInfos(IAppointmentIntermediateViewInfoCoreCollection viewInfos) {
			List<IAppointmentIntermediateLayoutViewInfoCoreCollection> groupedViewInfos = new List<IAppointmentIntermediateLayoutViewInfoCoreCollection>();
			AppointmentIntermediateViewInfoCoreCollection viewInfosCollection = new AppointmentIntermediateViewInfoCoreCollection();
			int count = viewInfos.Count;
			if (count == 0)
				return groupedViewInfos;
			DateTime groupEnd = viewInfos[0].ViewInfo.Interval.Start;
			for (int i = 0; i < count; i++) {
				AppointmentIntermediateViewInfoCore intermediateViewInfo = viewInfos[i];
				TimeInterval viewInfoInterval = intermediateViewInfo.ViewInfo.Interval;
				if (viewInfoInterval.Start >= groupEnd) {
					viewInfosCollection = new AppointmentIntermediateViewInfoCoreCollection();
					groupedViewInfos.Add(viewInfosCollection);
				}
				viewInfosCollection.Add(intermediateViewInfo);
				groupEnd = DateTimeHelper.Max(viewInfoInterval.End, groupEnd);
			}
			return groupedViewInfos;
		}
		protected internal virtual void LayoutGroupedViewInfosHorizontally(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection groupedViewInfos, TVisuallyContinuousCellsInfo cellsInfo) {
			VerticalAppointmentIndexesCalculator indexCalculator = new VerticalAppointmentIndexesCalculator();
			indexCalculator.CalculateAppointmentIndexes(token, groupedViewInfos);
		}
	}
	#endregion
	#region AppointmentLayoutCalculatorCore (abstract class)
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	public abstract class AppointmentLayoutCalculatorCore<TAppointmentIntermediateViewInfoCollection, TVisuallyContinuousCellsInfo, TAppointmentsLayoutResult, TAppointmentViewInfoCollection>
		where TAppointmentIntermediateViewInfoCollection : IAppointmentIntermediateViewInfoCoreCollection
		where TVisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfoCore
		where TAppointmentsLayoutResult : IAppointmentsLayoutResult
		where TAppointmentViewInfoCollection : IAppointmentViewInfoCollection, new() {
		#region Fields
		TimeScale scale;
		ISupportAppointmentsBase viewInfo;
		bool isCellHeightVariable;
		int preparedAppointmentsToCalculateCount;
		int calculatedPreparedAppointmentsCount;
		#endregion
		protected AppointmentLayoutCalculatorCore(ISupportAppointmentsBase viewInfo) {
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
			this.viewInfo = viewInfo;
			scale = CreateScale();
		}
		#region Properties
		public bool IsCellHeightVariable { get { return isCellHeightVariable; } set { isCellHeightVariable = value; } }
		protected internal ISupportAppointmentsBase ViewInfo { get { return viewInfo; } }
		protected internal virtual TimeScale Scale { get { return scale; } }
		protected internal virtual AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return ViewInfo.AppointmentDisplayOptions.SnapToCellsMode; } }
		#endregion
		protected internal abstract TimeScale CreateScale();
		protected internal abstract AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator();
		protected internal abstract void PreliminaryContentLayout(TAppointmentIntermediateViewInfoCollection intermediateResul, TVisuallyContinuousCellsInfo cellsInfo);
		protected internal abstract void LayoutViewInfos(TVisuallyContinuousCellsInfo cellsInfo, TAppointmentIntermediateViewInfoCollection intermediateResult, bool isTwoPassLayout);
		protected internal abstract void FinalContentLayout(TAppointmentViewInfoCollection viewInfos);
		protected internal abstract TAppointmentsLayoutResult SnapToCells(TAppointmentIntermediateViewInfoCollection intermediateResult, TVisuallyContinuousCellsInfo cellsInfo);
		protected internal abstract void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult);
		protected internal abstract TimeInterval GetAppointmentInterval(Appointment appointment);
		protected internal virtual AppointmentBaseComparer CreateAppointmentComparer() {
			return ViewInfo.AppointmentComparerProvider.CreateAppointmentComparer();
		}
		public virtual void CalculateLayout(TAppointmentsLayoutResult result, AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			PreparedAppointmentsCollection preparedAppointments = PrepareAppointments(appointments, cellsInfos);
			CalculateLayoutCore(result, preparedAppointments, true);
		}
		public virtual List<TAppointmentIntermediateViewInfoCollection> CalculatePreliminaryLayout(AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			PreparedAppointmentsCollection preparedAppointments = PrepareAppointments(appointments, cellsInfos);
			return CalculatePreliminaryLayoutCore(preparedAppointments);
		}
		protected internal virtual void CalculateLayoutCore(TAppointmentsLayoutResult result, PreparedAppointmentsCollection preparedAppointments, bool isTwoPassLayout) {
			int count = preparedAppointments.Count;
			for (int i = 0; i < count; i++) {
				TVisuallyContinuousCellsInfo cellsInfo = (TVisuallyContinuousCellsInfo)preparedAppointments[i].CellsInfo;
				TAppointmentsLayoutResult singleCellsInfolayoutResult = CalculateLayoutCoreSingleCellsInfo(preparedAppointments[i].Appointments, cellsInfo, isTwoPassLayout);
				result.Merge(singleCellsInfolayoutResult);
			}
		}
		protected virtual List<TAppointmentIntermediateViewInfoCollection> CalculatePreliminaryLayoutCore(PreparedAppointmentsCollection preparedAppointments) {
			List<TAppointmentIntermediateViewInfoCollection> result = new List<TAppointmentIntermediateViewInfoCollection>();
			IEnumerable<PreparedAppointments> preparedAppointmentsToCalculation = GetPreparedAppointmentsToCalculation(preparedAppointments);
			preparedAppointmentsToCalculateCount = preparedAppointmentsToCalculation.Count();
			calculatedPreparedAppointmentsCount = 0;
			IViewAsyncAccessor viewAccessor = ViewInfo as IViewAsyncAccessor;
			if (viewAccessor == null)
				return result;
			foreach (PreparedAppointments preparedAppointment in preparedAppointmentsToCalculation) {
				AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
				TAppointmentIntermediateViewInfoCollection intermediateResult = (TAppointmentIntermediateViewInfoCollection)intermediateCalculator.CreateIntermediateViewInfoCollection(preparedAppointment.CellsInfo.Resource, preparedAppointment.CellsInfo.Interval);
				result.Add(intermediateResult);
				viewAccessor.View.ThreadManager.Run((preparedApt, iRes) => {
					try {
						iRes.Signal.Reset();
						CalculateIntermediateResult(iRes, preparedApt, true);
						Interlocked.Increment(ref calculatedPreparedAppointmentsCount);
						if (calculatedPreparedAppointmentsCount == preparedAppointmentsToCalculateCount && !ViewInfo.CancellationToken.IsCancellationRequested)
							AfterPreliminaryLayoutCalculation();
						iRes.Signal.Set();
					} catch (Exception e) {
						throw e;
					}
				}, preparedAppointment, intermediateResult);
			}
			if (preparedAppointmentsToCalculateCount == 0 && !ViewInfo.CancellationToken.IsCancellationRequested)
				AfterPreliminaryLayoutCalculation();
			if (!ViewInfo.UseAsyncMode)
				viewAccessor.View.ThreadManager.WaitForAllThreads();
			return result;
		}
		protected virtual void AfterPreliminaryLayoutCalculation() {
		}
		protected virtual IEnumerable<PreparedAppointments> GetPreparedAppointmentsToCalculation(PreparedAppointmentsCollection preparedAppointments) {
			return preparedAppointments;
		}
		protected internal virtual TAppointmentsLayoutResult CalculateLayoutCoreSingleCellsInfo(AppointmentBaseCollection appointments, TVisuallyContinuousCellsInfo cellsInfo, bool isTwoPassLayout) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			TAppointmentIntermediateViewInfoCollection intermediateResult = (TAppointmentIntermediateViewInfoCollection)intermediateCalculator.CreateIntermediateViewInfoCollection(cellsInfo.Resource, cellsInfo.Interval);
			CalculateIntermediateViewInfos(intermediateResult, appointments, cellsInfo);
			PreliminaryContentLayout(intermediateResult, cellsInfo);
			LayoutViewInfos(cellsInfo, intermediateResult, isTwoPassLayout);
			TAppointmentsLayoutResult result = SnapToCells(intermediateResult, cellsInfo);
			lock (cellsInfo.ScrollContainer.AppointmentViewInfos)
				cellsInfo.ScrollContainer.AppointmentViewInfos.AddRange(result.AppointmentViewInfos);
			FinalContentLayout((TAppointmentViewInfoCollection)result.AppointmentViewInfos);
			return result;
		}
		protected void CalculateIntermediateResult(TAppointmentIntermediateViewInfoCollection intermediateResult, PreparedAppointments preparedAppointments, bool isTwoPassLayout) {
			CalculateIntermediateViewInfos(intermediateResult, preparedAppointments.Appointments, (TVisuallyContinuousCellsInfo)preparedAppointments.CellsInfo);
			LayoutViewInfos((TVisuallyContinuousCellsInfo)preparedAppointments.CellsInfo, intermediateResult, isTwoPassLayout);
		}
		protected internal virtual PreparedAppointmentsCollection PrepareAppointments(AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			PreparedAppointmentsCollection result = new PreparedAppointmentsCollection();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++) {
				ResourceVisuallyContinuousCellsInfos resourceCellsInfos = cellsInfos[i];
				AppointmentBaseCollection filteredAppointments = FilterAppointmentsByResource(appointments, resourceCellsInfos.Resource);
				result.AddRange(PrepareAppointmentsForSingleResource(filteredAppointments, resourceCellsInfos.CellsInfoCollection));
			}
			return result;
		}
		protected internal virtual AppointmentBaseCollection FilterAppointmentsByResource(AppointmentBaseCollection appointments, Resource resource) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			if (ResourceBase.IsEmptyResourceId(resource.Id)) {
				result.AddRange(appointments);
				return result;
			}
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (AppointmentMatchResource(apt, resource))
					result.Add(apt);
			}
			return result;
		}
		protected internal virtual bool AppointmentMatchResource(Appointment apt, Resource resource) {
			return ResourceBase.InternalMatchIds(apt.ResourceId, resource.Id) || apt.ResourceIds.Contains(resource.Id);
		}
		protected internal virtual PreparedAppointmentsCollection PrepareAppointmentsForSingleResource(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfoCollection cellsInfos) {
			PreparedAppointmentsCollection result = new PreparedAppointmentsCollection();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++) {
				IVisuallyContinuousCellsInfoCore cellsInfo = cellsInfos[i];
				AppointmentBaseCollection filteredAppointments = FilterAppointmentsByDate(appointments, cellsInfo);
				result.Add(new PreparedAppointments(filteredAppointments, cellsInfo));
			}
			return result;
		}
		protected internal virtual AppointmentBaseCollection FilterAppointmentsByDate(AppointmentBaseCollection appointments, IVisuallyContinuousCellsInfoCore cellsInfo) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment appointment = appointments[i];
				TimeInterval aptInterval = GetAppointmentInterval(appointment);
				if (IsAppointmentBelongsToCells(aptInterval, cellsInfo))
					result.Add(appointment);
			}
			return result;
		}
		protected internal virtual bool IsAppointmentBelongsToCells(TimeInterval appointmentInterval, IVisuallyContinuousCellsInfoCore cellsInfo) {
			TimeInterval cellsInterval = cellsInfo.Interval;
			TimeInterval intersectionInterval = TimeInterval.Intersect(cellsInterval, appointmentInterval);
			if (intersectionInterval.Duration == TimeSpan.Zero) {
				if (intersectionInterval.Start == DateTime.MinValue)
					return false;
				if (intersectionInterval.Start == cellsInterval.End)
					return false;
				if (intersectionInterval.Start == cellsInterval.Start && appointmentInterval.Start != cellsInterval.Start)
					return false;
			}
			return true;
		}
		protected internal virtual bool IsDateTimeBelongToCells(DateTime dateTime, TVisuallyContinuousCellsInfo cells) {
			return ((dateTime >= cells.Interval.Start) && (dateTime <= cells.Interval.End));
		}
		protected internal virtual void CalculateIntermediateViewInfos(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, AppointmentBaseCollection appointments, TVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			intermediateCalculator.CalculateIntermediateAppointmentViewInfos(ViewInfo.CancellationToken.Token, intermediateResult, appointments, cellsInfo);
			intermediateResult.Sort(CreateAppointmentComparer());
			IAppointmentIntermediateViewInfoCoreCollection filteredResult = FilterAppointmentIntermediateViewInfoResult(intermediateResult, cellsInfo, intermediateCalculator);
			if (intermediateResult != filteredResult) {
				intermediateResult.Clear();
				for (int i = 0; i < filteredResult.Count; i++)
					intermediateResult.Add(filteredResult[i]);
			}
		}
		protected virtual IAppointmentIntermediateViewInfoCoreCollection FilterAppointmentIntermediateViewInfoResult(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, TVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateLayoutCalculatorCore intermediateCalculator) {
			if (IsCellHeightVariable)
				return intermediateResult;
			int aptCountPerCell = CalculateMaxAppointmentsLevel(intermediateResult, cellsInfo);
			if (aptCountPerCell < 0)
				return intermediateResult;
			IAppointmentIntermediateViewInfoCoreCollection intermediateFilteredResult = intermediateCalculator.CreateIntermediateViewInfoCollection();
			AppointmenPerCellReductionPredicate predicate = new AppointmenPerCellReductionPredicate(cellsInfo, aptCountPerCell);
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return intermediateResult;
				if (predicate.Calculate(intermediateResult[i].AppointmentInterval))
					intermediateFilteredResult.Add(intermediateResult[i]);
			}
			return intermediateFilteredResult;
		}
		protected virtual int CalculateMaxAppointmentsLevel(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, TVisuallyContinuousCellsInfo cellsInfo) {
			int appointmentHeight = CalculateMinAppointmentHeight(intermediateResult);
			int maxCellHeight = cellsInfo.GetMaxCellHeight();
			if (maxCellHeight == Int32.MaxValue || maxCellHeight <= 0 || appointmentHeight <= 0)
				return -1;
			return cellsInfo.GetMaxCellHeight() / appointmentHeight + 1;
		}
		protected virtual int CalculateMinAppointmentHeight(IAppointmentIntermediateViewInfoCoreCollection appointmentIntermediateViewInfo) {
			return 0;
		}
		protected internal virtual void CalculateViewInfoProperties(AppointmentIntermediateViewInfoCore intermediateResult, TVisuallyContinuousCellsInfo cellsInfo) {
			intermediateResult.ViewInfo.Resource = cellsInfo.Resource;
			CalculateViewInfoBorders(intermediateResult);
		}
	}
	#endregion
	#region AppointmentViewInfoOptionsSet
	[Flags]
	public enum AppointmentViewInfoOptionsSet {
		None = 0x00,
		ShowBell = 0x01,
		ShowStartTime = 0x02,
		ShowEndTime = 0x04,
		ShowTimeAsClock = 0x08,
		ShowRecurrence = 0x10
	}
	#endregion
	#region AppointmentViewInfoOptions
	public class AppointmentViewInfoOptions : INotifyPropertyChanged {
		AppointmentViewInfoOptionsSet options;
		AppointmentStatusDisplayType statusDisplayType = AppointmentStatusDisplayType.Never;
		PercentCompleteDisplayType percentCompleteDisplayType = PercentCompleteDisplayType.None;
		AppointmentContinueArrowDisplayType startContinueItemDisplayType = AppointmentContinueArrowDisplayType.Never;
		AppointmentContinueArrowDisplayType endContinueItemDisplayType = AppointmentContinueArrowDisplayType.Never;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsStartContinueItemDisplayType")]
#endif
		public AppointmentContinueArrowDisplayType StartContinueItemDisplayType {
			get { return startContinueItemDisplayType; }
			set {
				if (StartContinueItemDisplayType == value)
					return;
				startContinueItemDisplayType = value;
				RaisePropertyChanged("StartContinueItemDisplayType");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsEndContinueItemDisplayType")]
#endif
		public AppointmentContinueArrowDisplayType EndContinueItemDisplayType {
			get { return endContinueItemDisplayType; }
			set {
				if (EndContinueItemDisplayType == value)
					return;
				endContinueItemDisplayType = value;
				RaisePropertyChanged("EndContinueItemDisplayType");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsStatusDisplayType")]
#endif
		public AppointmentStatusDisplayType StatusDisplayType {
			get { return statusDisplayType; }
			set {
				if (StatusDisplayType == value)
					return;
				statusDisplayType = value;
				RaisePropertyChanged("StatusDisplayType");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsShowBell")]
#endif
		public bool ShowBell {
			get { return GetValue(AppointmentViewInfoOptionsSet.ShowBell); }
			set {
				if (ShowBell == value)
					return;
				SetValue(AppointmentViewInfoOptionsSet.ShowBell, value);
				RaisePropertyChanged("ShowBell");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsShowStartTime")]
#endif
		public bool ShowStartTime {
			get { return GetValue(AppointmentViewInfoOptionsSet.ShowStartTime); }
			set {
				if (ShowStartTime == value)
					return;
				SetValue(AppointmentViewInfoOptionsSet.ShowStartTime, value);
				RaisePropertyChanged("ShowStartTime");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsShowEndTime")]
#endif
		public bool ShowEndTime {
			get { return GetValue(AppointmentViewInfoOptionsSet.ShowEndTime); }
			set {
				if (ShowEndTime == value)
					return;
				SetValue(AppointmentViewInfoOptionsSet.ShowEndTime, value);
				RaisePropertyChanged("ShowEndTime");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsShowTimeAsClock")]
#endif
		public bool ShowTimeAsClock {
			get { return GetValue(AppointmentViewInfoOptionsSet.ShowTimeAsClock); }
			set {
				if (ShowTimeAsClock == value)
					return;
				SetValue(AppointmentViewInfoOptionsSet.ShowTimeAsClock, value);
				RaisePropertyChanged("ShowTimeAsClock");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsShowRecurrence")]
#endif
		public bool ShowRecurrence {
			get { return GetValue(AppointmentViewInfoOptionsSet.ShowRecurrence); }
			set {
				if (ShowRecurrence == value)
					return;
				SetValue(AppointmentViewInfoOptionsSet.ShowRecurrence, value);
				RaisePropertyChanged("ShowRecurrence");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsPercentCompleteDisplayType")]
#endif
		public PercentCompleteDisplayType PercentCompleteDisplayType {
			get { return percentCompleteDisplayType; }
			set {
				if (PercentCompleteDisplayType == value)
					return;
				percentCompleteDisplayType = value;
				RaisePropertyChanged("PercentCompleteDisplayType");
			}
		}
		protected bool GetValue(AppointmentViewInfoOptionsSet mask) {
			return (options & mask) != 0;
		}
		protected void SetValue(AppointmentViewInfoOptionsSet mask, bool value) {
			if (value)
				options |= mask;
			else
				options &= ~mask;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentViewInfoOptionsPropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void RaisePropertyChanged(string name) {
			PropertyChangedEventHandler handler = onPropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(name));
		}
		#endregion
		internal bool Compare(AppointmentViewInfoOptions viewInfoOptions) {
			return this.options == viewInfoOptions.options && StatusDisplayType == viewInfoOptions.StatusDisplayType;
		}
	}
	#endregion
	#region NonVisualAppointmentViewInfo
	public class NonVisualAppointmentViewInfo : IAppointmentViewInfo {
		public NonVisualAppointmentViewInfo(Appointment apt) {
			Guard.ArgumentNotNull(apt, "apt");
			Appointment = apt;
			Interval = AppointmentInterval;
			Resource = ResourceBase.Empty;
		}
		public TimeInterval AppointmentInterval { get { return ((IInternalAppointment)Appointment).CreateInterval(); } }
		public bool HasBottomBorder { get; set; }
		public bool HasLeftBorder { get; set; }
		public bool HasRightBorder { get; set; }
		public bool HasTopBorder { get; set; }
		public TimeInterval Interval { get; set; }
		public bool IsLongTime() { return false; }
		public AppointmentViewInfoOptions Options { get { return null; } }
		public Resource Resource { get; set; }
		public Appointment Appointment { get; private set; }
	}
	#endregion
}
