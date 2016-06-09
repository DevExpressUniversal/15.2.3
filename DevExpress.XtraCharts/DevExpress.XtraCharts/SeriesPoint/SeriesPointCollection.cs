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
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPointCollection : ChartCollectionBase, ICloneable, IList<ISeriesPoint>, ITypedList, IBindingList {
		#region Nested Classes: SeriesPointEnumerator, ArgumentPropertyDescriptor, ValuePropertyDescriptor, ButtonPropertyDescriptor, AnnotationsPropertyDescriptor, LinksPropertyDescriptor
		class SeriesPointEnumerator : IEnumerator<ISeriesPoint> {
			readonly SeriesPointCollection collection;
			int currentIndex;
			public object Current { get { return collection[currentIndex]; } }
			ISeriesPoint IEnumerator<ISeriesPoint>.Current { get { return (ISeriesPoint)Current; } }
			public SeriesPointEnumerator(SeriesPointCollection collection) {
				this.collection = collection;
				Reset();
			}
			public void Dispose() {
			}
			public void Reset() {
				currentIndex = -1;
			}
			public bool MoveNext() {
				return ++currentIndex < collection.List.Count;
			}
		}
		class ArgumentPropertyDescriptor : PropertyDescriptor {
			public override Type ComponentType { get { return typeof(SeriesPoint); } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool IsReadOnly { get { return false; } }
			public override bool IsBrowsable { get { return true; } }
			public ArgumentPropertyDescriptor() : base(ChartLocalizer.GetString(ChartStringId.ArgumentMember), new Attribute[] { }) {
			}
			void ValidateDateTimeArgument(SeriesPoint seriesPoint) {
				IChartContainer chartContainer = seriesPoint.ChartContainer;
				if (chartContainer != null && chartContainer.DesignMode && seriesPoint.ArgumentScaleType == ScaleType.DateTime &&
					(DateTime.Compare(MinPointDateTimeValue, seriesPoint.DateTimeArgument) > 0))
					seriesPoint.DateTimeArgument = MinPointDateTimeValue;
			}
			public override object GetValue(object component) {
				SeriesPoint seriesPoint = (SeriesPoint)component;
				Series series = seriesPoint.Series;
				if (series.ActualArgumentScaleType == ScaleType.DateTime) {
					string format = "G";
					if (series.Label != null)
						format = PatternUtils.GetArgumentFormat(series.Label.ActualTextPattern);
					return PatternUtils.Format(seriesPoint.DateTimeArgument, format);
				}
				return seriesPoint.Argument;
			}
			public override void SetValue(object component, object value) {
				SeriesPoint point = component as SeriesPoint;
				string argument = value as string;
				if (point != null && argument != null) {
					Series series = point.Series;
					int indexPoint = series.Points.IndexOf(point);
					if (argument == String.Empty && (!point.IsEmpty || indexPoint != series.Points.Count - 1))
						throw new ArgumentException("argument");
					object oldArgument = point.Argument;
					if (!argument.Equals(oldArgument)) {
						point.SetArgument(argument, true, true);
						ValidateDateTimeArgument(point);
						if (!series.IsCompatible(point)) {
							point.SetArgument(oldArgument, true, true);
							throw new Exception();
						}
						point.RaiseControlChanged();
					}
				}
			}
			public override bool CanResetValue(object component) {
				return ((SeriesPoint)component).Argument != null;
			}
			public override void ResetValue(object component) {
				((SeriesPoint)component).SetArgument(string.Empty, false, true);
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		class ValuePropertyDescriptor : PropertyDescriptor {
			int index;
			public override Type ComponentType { get { return typeof(SeriesPoint); } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool IsReadOnly { get { return false; } }
			public override bool IsBrowsable { get { return true; } }
			public ValuePropertyDescriptor(string name, int index) : base(name, new Attribute[] { }) {
				this.index = index;
			}
			internal static void ValidateDateTimeValues(SeriesPoint seriesPoint) {
				IChartContainer chartContainer = seriesPoint.ChartContainer;
				DateTime[] values = seriesPoint.DateTimeValues;
				if (chartContainer != null && chartContainer.DesignMode && seriesPoint.ValueScaleType == ScaleType.DateTime && values != null) {
					for (int i = 0; i < values.Length; i++) {
						if (DateTime.Compare(MinPointDateTimeValue, values[i]) > 0)
							values[i] = MinPointDateTimeValue;
					}
				}
			}
			public override object GetValue(object component) {
				SeriesPoint point = (SeriesPoint)component;
				return point.IsEmpty ? String.Empty : point.GetValueString(index);
			}
			public override void SetValue(object component, object value) {
				SeriesPoint point = (SeriesPoint)component;
				string str = value as string;
				if (String.IsNullOrEmpty(str))
					point.IsEmpty = true;
				else {
					bool prevEmptyState = point.IsEmpty;
					point.IsEmpty = false;
					if (point.ValueScaleType == ScaleType.DateTime) {
						DateTime newValue = Convert.ToDateTime(value);
						if (newValue != point.DateTimeValues[index] || prevEmptyState) {
							point.SendNotification(new ChartElement.ElementWillChangeNotification(this));
							point.DateTimeValues[index] = newValue;
							ValidateDateTimeValues(point);
							point.RaisePropertyChanged(SeriesPoint.ValuesProperty);
						}
					}
					else {
						double newValue = Convert.ToDouble(value);
						if (newValue != point.Values[index] || prevEmptyState) {
							point.SendNotification(new ChartElement.ElementWillChangeNotification(this));
							point.Values[index] = newValue;
							point.RaisePropertyChanged(SeriesPoint.ValuesProperty);
						}
					}
				}
			}
			public override bool CanResetValue(object component) {
				return ((SeriesPoint)component).Values[index] != 0.0;
			}
			public override void ResetValue(object component) {
				((SeriesPoint)component).Values[index] = 0.0;
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		class ButtonPropertyDescriptor : PropertyDescriptor {
			bool browsable;
			public override Type ComponentType { get { return typeof(SeriesPoint); } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool IsReadOnly { get { return false; } }
			public override bool IsBrowsable { get { return browsable; } }
			public ButtonPropertyDescriptor(bool browsable, string name) : base(name, new Attribute[0]) {
				this.browsable = browsable;
			}
			public override object GetValue(object component) {
				return "...";
			}
			public override void SetValue(object component, object value) {
			}
			public override bool CanResetValue(object component) {
				return true;
			}
			public override void ResetValue(object component) {
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		class AnnotationsPropertyDescriptor : ButtonPropertyDescriptor {
			public AnnotationsPropertyDescriptor(bool browsable) : base(browsable, ChartLocalizer.GetString(ChartStringId.ColumnAnnotations)) { }
		}
		class ColorPropertyDescriptor : ButtonPropertyDescriptor {
			public ColorPropertyDescriptor(bool browsable) : base(browsable, ChartLocalizer.GetString(ChartStringId.ColumnColor)) { }
		}
		class LinksPropertyDescriptor : ButtonPropertyDescriptor {
			public LinksPropertyDescriptor(bool browsable) : base(browsable, ChartLocalizer.GetString(ChartStringId.ColumnLinks)) {
			}
		}
		#endregion
		static readonly DateTime MinPointDateTimeValue = new DateTime(1, 1, 8);
		int currentMaxId = -1;
		ListChangedEventHandler onListChanged;
		bool unlock;
		SeriesPoint emptyDesignPoint = null;
		bool Loading { get { return Owner != null ? Owner.Loading : false; } }
		Series Series { get { return (Series)base.Owner; } }
		protected override bool UpdateLocked { get { return base.UpdateLocked || Loading; } }
		internal bool Unlock { get { return unlock; } set { unlock = value; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SeriesPointCollectionOwner")]
#endif
		public new Series Owner { get { return (Series)base.Owner; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SeriesPointCollectionItem")]
#endif
		public SeriesPoint this[int index] { get { return (SeriesPoint)List[index]; } }
		internal SeriesPointCollection(Series series) : base() {
			base.Owner = series;
		}
		#region ICloneable implementation
		object ICloneable.Clone() {
			SeriesPointCollection coll = new SeriesPointCollection(null);
			coll.InnerList.AddRange(InnerList);
			coll.SetOwner(Owner);
			return coll;
		}
		#endregion
		#region IEnumerable<ISeriesPoint> implementation
		IEnumerator<ISeriesPoint> IEnumerable<ISeriesPoint>.GetEnumerator() {
			return new SeriesPointEnumerator(this);
		}
		#endregion
		#region ICollection<ISeriesPoint> implementation
		bool ICollection<ISeriesPoint>.IsReadOnly { get { return false; } }
		bool ICollection<ISeriesPoint>.Contains(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			return point != null && Contains(point);
		}
		void ICollection<ISeriesPoint>.Add(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			if (point != null)
				Add(point);
		}
		bool ICollection<ISeriesPoint>.Remove(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			if (point == null || !Contains(point))
				return false;
			Remove(point);
			return true;
		}
		void ICollection<ISeriesPoint>.CopyTo(ISeriesPoint[] array, int arrayIndex) {
			int count = Count;
			for (int i = 0, j = arrayIndex; i < count; i++, j++)
				array[j] = this[i];
		}
		#endregion
		#region IList<ISeriesPoint> implementation
		ISeriesPoint IList<ISeriesPoint>.this[int index] {
			get { return this[index]; }
			set { ((IList)this)[index] = value as SeriesPoint; }
		}
		int IList<ISeriesPoint>.IndexOf(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			return point == null ? -1 : IndexOf(point);
		}
		void IList<ISeriesPoint>.Insert(int index, ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			if (point != null)
				Insert(index, point);
		}
		#endregion
		#region ITypedList implementation
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return "SeriesPoints";
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if (Owner == null || Owner.View == null)
				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			int dimension = Owner.View.PointDimension;
			PropertyDescriptor[] descriptor = new PropertyDescriptor[dimension + 4];
			descriptor[0] = new ArgumentPropertyDescriptor();
			for (int i = 0; i < dimension; i++)
				descriptor[i + 1] = new ValuePropertyDescriptor(Owner.View.GetValueCaption(i), i);
			IChartContainer container = ((Series)Owner).ChartContainer;
			descriptor[descriptor.Length - 3] = new ColorPropertyDescriptor(true);
			descriptor[descriptor.Length - 2] = new AnnotationsPropertyDescriptor(container != null && !container.IsDesignControl);
			descriptor[descriptor.Length - 1] = new LinksPropertyDescriptor(((Series)Owner).View.IsSupportedRelations);
			return new PropertyDescriptorCollection(descriptor);
		}
		#endregion
		#region IBindingList implementation
		event ListChangedEventHandler IBindingList.ListChanged {
			add { onListChanged = (ListChangedEventHandler)Delegate.Combine(onListChanged, value); }
			remove { onListChanged = (ListChangedEventHandler)Delegate.Remove(onListChanged, value); }
		}
		bool IBindingList.AllowEdit { get { return Owner.UnboundMode; } }
		bool IBindingList.AllowNew { get { return Owner.UnboundMode; } }
		bool IBindingList.AllowRemove { get { return Owner.UnboundMode; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		bool IBindingList.IsSorted {
			get { throw new NotSupportedException(); }
		}
		ListSortDirection IBindingList.SortDirection {
			get { throw new NotSupportedException(); }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { throw new NotSupportedException(); }
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		void IBindingList.RemoveSort() {
			throw new NotSupportedException();
		}
		object IBindingList.AddNew() {
			ValidateEmptyArgument();
			SeriesPoint point = CreateNewSeriesPoint();
			SetID(point);
			AddNoTesting(point);
			return point;
		}
		#endregion
		void SetOwner(Series owner) {
			base.Owner = owner;
		}
		void OnListChanged(ListChangedType type, int newIndex, int oldIndex) {
			if (List.Count == 0)
				currentMaxId = -1;
			if (onListChanged != null)
				onListChanged(this, new ListChangedEventArgs(type, newIndex, oldIndex));
		}
		void TestPoint(SeriesPoint point) {
			if (string.IsNullOrEmpty(point.Argument))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			Series series = Owner;
			if (series == null) {
				if (point.Values.Length == 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArrayOfValues));
			}
			else {
				if (!AxisScaleTypeMap.CheckArgumentScaleType(point, (Scale)series.ArgumentScaleType)) {
					string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleArgument), point.Argument);
					throw new ArgumentException(message);
				}
				if (!point.CheckValueScaleType(series.ValueScaleType)) {
					string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleValue),
						point.ValueScaleType == ScaleType.Numerical ? point.Values[0].ToString() : point.DateTimeValues[0].ToString());
					throw new ArgumentException(message);
				}
				int pointDimension = series.View.PointDimension;
				if (pointDimension > point.Values.Length)
					if (point.IsEmpty)
						point.IncreaseDimension(pointDimension);
					else
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectValueDataMemberCount), pointDimension.ToString()));
			}
		}
		void SetID(SeriesPoint point) {
			if (point.HasRelations)
				point.Relations.ClearInternal();
			point.SetID(GetNextId());
		}
		int AddNoTesting(SeriesPoint point) {
			return base.Add(point);
		}
		int GetNextId() {
			if (currentMaxId == -1)
				foreach (SeriesPoint point in List)
					if (point.SeriesPointID > currentMaxId)
						currentMaxId = point.SeriesPointID;
			return ++currentMaxId;
		}
		void CheckIDs() {
			List<int> ids = new List<int>();
			foreach (SeriesPoint point in this) {
				int index = ids.BinarySearch(point.SeriesPointID);
				if (index >= 0)
					throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgSeriesPointIDNotUnique));
				ids.Insert(~index, point.SeriesPointID);
				if (point.HasRelations)
					point.Relations.ValidateIDs();
			}
		}
		void ValidateIDs() {
			if (Count == 0)
				return;
			foreach (SeriesPoint point in this) {
				if (point.SeriesPointID == -1)
					point.SetID(GetNextId());
			}
			foreach (SeriesPoint point in this)
				if (point.HasRelations)
					foreach (Relation relation in point.Relations)
						relation.InitializeChildPointId();
			CheckIDs();
		}
		void TestUnboundMode() {
			if (!this.unlock && Owner != null && !Owner.UnboundMode)
				throw new SeriesPointCollectionChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointCollection));
		}
		SeriesPoint CreateNewSeriesPoint() {
			SeriesPoint seriesPoint = new SeriesPoint(Owner.View.PointDimension, Owner.ArgumentScaleType, Owner.ValueScaleType);
			IChartContainer container = Owner != null ? Owner.ChartContainer : null;
			if (container != null && container.DesignMode) {
				ScaleType actualArgumentScaleType = Owner.ActualArgumentScaleType;
				if (this.Count > 0) {
					SeriesPoint prevPoint = this[this.Count - 1];
					if (actualArgumentScaleType == ScaleType.DateTime)
						seriesPoint.DateTimeArgument = prevPoint.DateTimeArgument;
					else
						seriesPoint.Argument = prevPoint.Argument;
				}
				else if (actualArgumentScaleType == ScaleType.DateTime)
					seriesPoint.DateTimeArgument = DateTime.Now;
				else
					seriesPoint.Argument = "0";
				emptyDesignPoint = (SeriesPoint)seriesPoint.Clone();
			}
			return seriesPoint;
		}
		bool IsEmptyDesignPoint(ISeriesPoint seriesPoint) {
			IChartContainer container = Owner.ChartContainer;
			return container != null && container.DesignMode && seriesPoint.Equals(emptyDesignPoint);
		}
		void ValidateLastPoint(Predicate<SeriesPoint> shouldRemovePoint) {
			if (Count > 0) {
				SeriesPoint point = this[Count - 1];
				if (shouldRemovePoint(point))
					RemoveAt(Count - 1);
			}
		}
		void ValidateEmptyArgument() {
			ValidateLastPoint(point => point.Argument == String.Empty);
		}
		protected override ICollection GetUpdateInfoSequence(int index) {
			return UpdateHelper.GetUpdateInfoSequence<ISeriesPoint>(index);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new SeriesPointCollectionUpdateInfo(this, operation, Owner, oldItem as ISeriesPoint, oldIndex, newItem as ISeriesPoint, newIndex);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new SeriesPointCollectionBatchUpdateInfo(this, operation, Owner, oldItems as ICollection<ISeriesPoint>, oldIndex, newItems as ICollection<ISeriesPoint>, newIndex);
		}
		protected override void OnClear() {
			foreach (SeriesPoint point in this)
				point.ClearAnnotations();
			TestUnboundMode();
			base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnListChanged(ListChangedType.Reset, -1, -1);
		}
		protected override void OnInsert(int index, object value) {
			TestUnboundMode();
			base.OnInsert(index, value);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SeriesPoint point = value as SeriesPoint;
			if (point != null)
				point.UpdateAnnotationRepository();
			OnListChanged(ListChangedType.ItemAdded, index, -1);
		}
		protected override void OnRemove(int index, object value) {
			SeriesPoint point = value as SeriesPoint;
			if (point != null)
				point.ClearAnnotations();
			TestUnboundMode();
			for (int i = 0; i < Count; i++) {
				if (i != index) {
					point = this[i];
					if (point != null && point.Relations != null) {
						Relation relation = point.Relations.GetByChildSeriesPoint(value as SeriesPoint);
						if (relation != null)
							point.Relations.Remove(relation);
					}
				}
			}
			base.OnRemove(index, value);
		}
		protected override void OnSet(int index, object oldValue, object newValue) {
			TestUnboundMode();
			SeriesPoint point = newValue as SeriesPoint;
			if (point != null)
				point.UpdateAnnotationRepository();
			base.OnSet(index, oldValue, newValue);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnListChanged(ListChangedType.ItemDeleted, index, -1);
		}
		protected override void OnBeginUpdate() {
			base.OnBeginUpdate();
			Series.BeginPointsUpdate();
		}
		protected override void OnEndUpdate() {
			base.OnEndUpdate();
			Series.EndPointsUpdate();
		}
		protected internal override void RaiseControlChanged(ChartUpdateInfoBase changeInfo) {
			if (NotificationCenter != null)
				base.RaiseControlChanged(changeInfo);
		}
		internal bool ArePointEquals(IList<ISeriesPoint> points) {
			if (Count != points.Count)
				return false;
			for (int i = 0; i < Count; i++)
				if (!this[i].Equals(points[i]))
					return false;
			return true;
		}
		internal SeriesPoint GetByID(int id) {
			if (id < 0)
				return null;
			foreach (SeriesPoint point in this) {
				if (point.SeriesPointID == id)
					return point;
			}
			return null;
		}
		internal SeriesPoint GetByID(SeriesPoint point) {
			if (point.SeriesPointID < 0)
				return null;
			foreach (SeriesPoint collectionPoint in this) {
				if (collectionPoint.SeriesPointID == point.SeriesPointID)
					return collectionPoint;
				foreach (SeriesPoint sourcePoint in point.SourcePoints)
					if (collectionPoint.SeriesPointID == sourcePoint.SeriesPointID)
						return collectionPoint;
			}
			return null;
		}
		internal void OnEndLoading(bool validatePointsId) {
			EndUpdate(false);
			foreach (SeriesPoint point in this)
				point.OnEndLoading();
			if (validatePointsId)
				ValidateIDs();
		}
		protected internal override void EndUpdate(bool bindingProcessing) {
			if (!Loading)
				base.EndUpdate(bindingProcessing);
		}
		public void Validate() {
			ValidateLastPoint(point => point.Argument == String.Empty || IsEmptyDesignPoint(point));
			emptyDesignPoint = null;
		}
		public void Move(int oldIndex, int newIndex) {
			if (newIndex != oldIndex) {
				object obj = this[oldIndex];
				InnerList.RemoveAt(oldIndex);
				InnerList.Insert(newIndex, obj);
				RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.MoveItem, null, oldIndex, obj, newIndex));
			}
		}
		public int Add(SeriesPoint point) {
			bool scaleChanged = false;
			if (Owner != null && !Owner.Loading) {
				TestPoint(point);
				SetID(point);
				ScaleType scaleType = Owner.ActualArgumentScaleType;
				scaleChanged = scaleType != Owner.ActualArgumentScaleType;
			}
			return base.Add(point);
		}
		public void AddRange(SeriesPoint[] points) {
			bool scaleChanged = false;
			if (Owner != null && !Owner.Loading) {
				ScaleType scaleType = Owner.ActualArgumentScaleType;
				foreach (SeriesPoint point in points) {
					TestPoint(point);
					SetID(point);
				}
				scaleChanged = scaleType != Owner.ActualArgumentScaleType;
			}
			base.AddRange(points);
		}
		public void Insert(int index, SeriesPoint point) {
			bool scaleChanged = false;
			if (Owner != null && !Owner.Loading) {
				TestPoint(point);
				SetID(point);
				ScaleType scaleType = Owner.ActualArgumentScaleType;
				scaleChanged = scaleType != Owner.ActualArgumentScaleType;
			}
			base.Insert(index, point);
		}
		public void RemoveRange(int index, int count) {
			ISeriesPoint[] pointsToRemove = null;
			if (index >= 0 && count >= 0 && (index + count) <= InnerList.Count) {
				pointsToRemove = new ISeriesPoint[count];
				InnerList.CopyTo(index, pointsToRemove, 0, count);
			}
			if (pointsToRemove != null) {
				foreach (ChartElement item in pointsToRemove)
					DisposeItemBeforeRemove(item);
			}
			if (pointsToRemove != null)
				SendControlChanging();
			InnerList.RemoveRange(index, count);
			if (pointsToRemove != null) {
				RaiseControlChanged(CreateBatchUpdateInfo(ChartCollectionOperation.RemoveItem, pointsToRemove, index, null, -1));
			}
		}
		public void Remove(SeriesPoint point) {
			base.Remove(point);
		}
		public new void RemoveAt(int index) {
			base.RemoveAt(index);
		}
		public void Swap(SeriesPoint point1, SeriesPoint point2) {
			base.Swap(point1, point2);
		}
		public int IndexOf(SeriesPoint point) {
			return base.IndexOf(point);
		}
		public new SeriesPoint[] ToArray() {
			return (SeriesPoint[])InnerList.ToArray(typeof(SeriesPoint));
		}
		public new bool Contains(ChartElement item) {
			if (base.Contains(item))
				return true;
			SeriesPoint point = item as SeriesPoint;
			if (point != null)
				foreach (SeriesPoint sourcePoint in point.SourcePoints)
					if (base.Contains(sourcePoint))
						return true;
			return false;
		}
		public override void Assign(ChartCollectionBase collection) {
			base.Assign(collection);
			SeriesPointCollection pointCollection = collection as SeriesPointCollection;
			if (pointCollection != null)
				emptyDesignPoint = pointCollection.emptyDesignPoint;
		}
	}
}
