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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Spreadsheet.Charts;
namespace DevExpress.Spreadsheet {
	#region SparklineGroupType
	public enum SparklineGroupType {
		Line = DevExpress.XtraSpreadsheet.Model.SparklineGroupType.Line,
		Column = DevExpress.XtraSpreadsheet.Model.SparklineGroupType.Column,
		Stacked = DevExpress.XtraSpreadsheet.Model.SparklineGroupType.Stacked
	}
	#endregion
	#region SparklineAxisType
	public enum SparklineAxisScaling {
		Individual = DevExpress.XtraSpreadsheet.Model.SparklineAxisScaling.Individual,
		Custom = DevExpress.XtraSpreadsheet.Model.SparklineAxisScaling.Custom,
		Group = DevExpress.XtraSpreadsheet.Model.SparklineAxisScaling.Group
	}
	#endregion
	#region SparklineGroupCollection
	public interface SparklineGroupCollection : ISimpleCollection<SparklineGroup> {
		SparklineGroup Add(Range position, Range dataRange, SparklineGroupType type);
		SparklineGroup Add(IList<Sparkline> sparklines, SparklineGroupType type);
		bool Remove(SparklineGroup item);
		void RemoveAt(int index);
		void Clear();
		bool Contains(SparklineGroup item);
		int IndexOf(SparklineGroup item);
		SparklineGroup GetSparklineGroup(Cell cell);
		IList<SparklineGroup> GetSparklineGroups(Range range);
	}
	#endregion
	#region SparklineGroup
	public interface SparklineGroup {
		SparklineCollection Sparklines { get; }
		SparklineVerticalAxis VerticalAxis { get; }
		SparklineHorizontalAxis HorizontalAxis { get; }
		SparklinePoints Points { get; }
		SparklineGroupType Type { get; set; }
		Range Position { get; }
		Range DateRange { get; set; }
		Color SeriesColor { get; set; }
		DisplayBlanksAs DisplayBlanksAs { get; set; }
		bool ShowHidden { get; set; }
		double LineWeight { get; set; }
		void UnGroup();
		void Delete();
	}
	public interface SparklineHorizontalAxis : SparklineColor {
		bool IsDateAxis { get; }
		bool RightToLeft { get; set; }
	}
	public interface SparklineColor {
		bool IsVisible { get; set; }
		Color Color { get; set; }
	}
	public interface SparklineVerticalAxis {
		SparklineAxisScaling MaxScaleType { get; set; }
		SparklineAxisScaling MinScaleType { get; set; }
		double MaxCustomValue { get; set; }
		double MinCustomValue { get; set; }
	}
	public interface SparklinePoints {
		SparklineColor First { get; }
		SparklineColor Last { get; }
		SparklineColor Highest { get; }
		SparklineColor Lowest { get; }
		SparklineColor Markers { get; }
		SparklineColor Negative { get; }
	}
	#endregion
	#region SparklineCollection
	public interface SparklineCollection : ISimpleCollection<Sparkline> {
		Sparkline Add(int rowIndex, int columnIndex, Range dataRange);
		bool Remove(Sparkline item);
		void RemoveAt(int index);
		bool Contains(Sparkline item);
		int IndexOf(Sparkline item);
		Sparkline GetSparkline(Cell cell);
		IList<Sparkline> GetSparklines(Range range);
	}
	#endregion
	#region Sparkline
	public interface Sparkline {
		Range DataRange { get; set; }
		Range Position { get; }
		void SetPosition(int columnIndex, int rowIndex);
		void MoveTo(SparklineGroup sparklineGroup);
		void Delete();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Office.Model;
	#region NativeSparklinCollectionBase
	abstract partial class NativeSparklinCollectionBase<T, N, M> : NativeObjectBase, ISimpleCollection<T> where N : NativeObjectBase, T where M : class {
		#region Fields
		readonly List<N> innerList;
		readonly NativeWorksheet nativeWorksheet;
		readonly UndoableCollection<M> modelCollection;
		#endregion
		protected NativeSparklinCollectionBase(NativeWorksheet nativeWorksheet, UndoableCollection<M> modelCollection) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Guard.ArgumentNotNull(modelCollection, "modelCollection");
			this.innerList = new List<N>();
			this.nativeWorksheet = nativeWorksheet;
			this.modelCollection = modelCollection;
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return InnerList.Count;
			}
		}
		public T this[int index] {
			get {
				CheckValid();
				return InnerList[index];
			}
		}
		protected internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		protected internal Model.Worksheet ModelWorksheet { get { return nativeWorksheet.ModelWorksheet; } }
		protected internal Model.DocumentModel ModelWorkbook { get { return nativeWorksheet.ModelWorksheet.Workbook; } }
		protected internal UndoableCollection<M> ModelCollection { get { return modelCollection; } }
		protected internal List<N> InnerList { get { return innerList; } }
		#endregion
		#region Internal
		protected internal void Populate() {
			modelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(M item) {
			innerList.Add(CreateNativeObject(item));
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateItems();
				UnsubscribeEvents();
			}
		}
		void InvalidateItems() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				innerList[i].IsValid = false;
		}
		public virtual bool Remove(T item) {
			int index = IndexOf(item);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public virtual void RemoveAt(int index) {
			CheckValid();
			modelCollection.RemoveAt(index);
		}
		public virtual bool Contains(T item) {
			return IndexOf(item) != -1;
		}
		public virtual int IndexOf(T item) {
			CheckValid();
			N nativeItem = (N)item;
			if (nativeItem != null)
				return InnerList.IndexOf(nativeItem);
			return -1;
		}
		protected abstract N CreateNativeObject(M modelItem);
		#endregion
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			modelCollection.OnAdd += OnAdd;
			modelCollection.OnRemoveAt += OnRemoveAt;
			modelCollection.OnInsert += OnInsert;
			modelCollection.OnClear += OnClear;
			modelCollection.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			modelCollection.OnAdd -= OnAdd;
			modelCollection.OnRemoveAt -= OnRemoveAt;
			modelCollection.OnInsert -= OnInsert;
			modelCollection.OnClear -= OnClear;
			modelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<M> modelArgs = e as UndoableCollectionAddEventArgs<M>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				innerList[index].IsValid = false;
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<M> modelArgs = e as UndoableCollectionInsertEventArgs<M>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<M> modelArgs = e as UndoableCollectionAddRangeEventArgs<M>;
			if (modelArgs == null)
				return;
			IEnumerable<M> collection = modelArgs.Collection;
			foreach (M modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<T> Members
		public IEnumerator<T> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<T, N>(innerList.GetEnumerator());
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
	}
	#endregion
	#region NativeSparklineGroupCollection
	partial class NativeSparklineGroupCollection : NativeSparklinCollectionBase<SparklineGroup, NativeSparklineGroup, Model.SparklineGroup>, SparklineGroupCollection {
		public NativeSparklineGroupCollection(NativeWorksheet nativeWorksheet)
			: base(nativeWorksheet, nativeWorksheet.ModelWorksheet.SparklineGroups) {
			Populate();
		}
		#region SparklineGroupCollection Members
		public SparklineGroup Add(Range position, Range dataRange, SparklineGroupType type) {
			CheckValid();
			Model.CellRangeBase modelDataRange = dataRange == null ? null : ((NativeWorksheet)dataRange.Worksheet).GetModelRange(dataRange);
			Model.CellRangeBase modelPosition = NativeWorksheet.GetModelRange(position);
			Model.SparklineGroupAddCommand command = new Model.SparklineGroupAddCommand(ModelWorksheet, ApiErrorHandler.Instance, modelDataRange, modelPosition, (Model.SparklineGroupType)type);
			if (!command.Execute())
				return null;
			return InnerList[Count - 1];
		}
		public SparklineGroup Add(IList<Sparkline> sparklines, SparklineGroupType type) {
			CheckValid();
			int count = sparklines.Count;
			if (count == 0)
				return null;
		   List<Model.Sparkline> modelSparklines = new List<Model.Sparkline>();
		   ModelWorkbook.BeginUpdate();
			try {
				for (int i = 0; i < count; i++) {
					NativeSparkline sparkline = sparklines[i] as NativeSparkline;
					modelSparklines.Add(sparkline.ModelSparkline);
					sparkline.Delete();
				}
				ModelCollection.Add(CreateModelGroup(modelSparklines, (Model.SparklineGroupType)type));
			}
			finally {
				ModelWorkbook.EndUpdate();
			}
			return InnerList[Count - 1];
		}
		Model.SparklineGroup CreateModelGroup(List<Model.Sparkline> sparklines, Model.SparklineGroupType type) {
			Model.SparklineGroup result = new Model.SparklineGroup(ModelWorksheet);
			result.BeginUpdate();
			try {
				result.Type = type;
				foreach (Model.Sparkline modelItem in sparklines)
					result.Sparklines.Add(modelItem);
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
		public void Clear() {
			CheckValid();
			ModelCollection.Clear();
		}
		public SparklineGroup GetSparklineGroup(Cell cell) {
			CheckValid();
			if (cell != null && object.ReferenceEquals(cell.Worksheet, NativeWorksheet)) {
				foreach (NativeSparklineGroup group in InnerList) {
					if (group.IsInCell(cell)) {
						return group;
					}
				}
			}
			return null;
		}
		public IList<SparklineGroup> GetSparklineGroups(Range range) {
			CheckValid();
			List<SparklineGroup> result = new List<SparklineGroup>();
			if (range != null && object.ReferenceEquals(range.Worksheet, NativeWorksheet)) {
				foreach (NativeSparklineGroup group in InnerList) {
					if (group.IsInRange(range)) {
						result.Add(group);
					}
				}
			}
			return result;
		}
		#endregion
		protected override NativeSparklineGroup CreateNativeObject(Model.SparklineGroup modelItem) {
			return new NativeSparklineGroup(modelItem, NativeWorksheet);
		}
	}
	#endregion
	#region NativeSparklineCollection
	partial class NativeSparklineCollection : NativeSparklinCollectionBase<Sparkline, NativeSparkline, Model.Sparkline>, SparklineCollection {
		readonly NativeSparklineGroup parent;
		public NativeSparklineCollection(NativeSparklineGroup parent)
			: base(parent.NativeWorksheet, parent.ModelSparklineGroup.Sparklines) {
			Guard.ArgumentNotNull(parent, "parent");
			this.parent = parent;
			Populate();
		}
		protected internal NativeSparklineGroup Parent { get { return parent; } }
		protected override NativeSparkline CreateNativeObject(Model.Sparkline modelItem) {
			return new NativeSparkline(modelItem, this);
		}
		#region SparklineCollection Members
		public Sparkline Add(int rowIndex, int columnIndex, Range dataRange) {
			CheckValid();
			NativeIndicesChecker.CheckRowIndex(rowIndex);
			NativeIndicesChecker.CheckColumnIndex(columnIndex);
			Model.CellRangeBase modelDataRange = dataRange == null ? null : ((NativeWorksheet)dataRange.Worksheet).GetModelRange(dataRange);
			Model.CellPosition position = new Model.CellPosition(columnIndex, rowIndex);
			Model.SparklineAddCommand command = new Model.SparklineAddCommand((Model.SparklineCollection)ModelCollection, ApiErrorHandler.Instance, modelDataRange, position);
			if (!command.Execute())
				return null;
			return InnerList[Count - 1];
		}
		public override void RemoveAt(int index) {
			CheckValid();
			ModelWorkbook.BeginUpdate();
			try {
				ModelCollection.RemoveAt(index);
				if (InnerList.Count == 0)
					parent.Delete();
			}
			finally {
				ModelWorkbook.EndUpdate();
			}
		}
		public Sparkline GetSparkline(Cell cell) {
			CheckValid();
			if (cell != null && object.ReferenceEquals(cell.Worksheet, NativeWorksheet)) {
				Sparkline sparkline = InnerList.Find(g => g.IsInCell(cell));				
				return sparkline;
			}
			return null;
		}
		public IList<Sparkline> GetSparklines(Range range) {
			CheckValid();
			List<Sparkline> result = new List<Sparkline>();
			if (range != null && object.ReferenceEquals(range.Worksheet, NativeWorksheet)) {
				foreach (NativeSparkline sparkline in InnerList) {
					if (sparkline.IsInRange(range)) {
						result.Add(sparkline);
					}
				}
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region NativeSparklineGroup
	partial class NativeSparklineGroup : NativeObjectBase, SparklineGroup, SparklineVerticalAxis, SparklineHorizontalAxis, SparklinePoints {
		#region DefaultColors
		internal static Color DefaultSeriesColor { get { return Color.FromArgb(55, 96, 146); } }
		internal static Color DefaultAxisColor { get { return Color.FromArgb(0, 0, 0); } }
		internal static Color DefaultPointColor { get { return Color.FromArgb(208, 0, 0); } }
		#endregion
		#region Fields
		readonly Model.SparklineGroup modelSparklineGroup;
		readonly NativeWorksheet nativeWorksheet;
		NativeSparklineCollection nativeSparklines;
		NativeSparklineColor nativeFirstColor;
		NativeSparklineColor nativeLastColor;
		NativeSparklineColor nativeHighestColor;
		NativeSparklineColor nativeLowestColor;
		NativeSparklineColor nativeNegativeColor;
		NativeSparklineColor nativeMarkersColor;
		#endregion
		public NativeSparklineGroup(Model.SparklineGroup modelSparklineGroup, NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Guard.ArgumentNotNull(modelSparklineGroup, "modelSparklineGroup");
			this.nativeWorksheet = nativeWorksheet;
			this.modelSparklineGroup = modelSparklineGroup;
			SetDefaultColors();
		}
		#region Properties
		protected internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		protected internal Model.SparklineGroup ModelSparklineGroup { get { return modelSparklineGroup; } }
		Model.DocumentModel DocumentModel { get { return modelSparklineGroup.DocumentModel; } }
		#region SparklineGroup Members
		public SparklineHorizontalAxis HorizontalAxis { get { return this; } }
		public SparklineVerticalAxis VerticalAxis { get { return this; } }
		public SparklinePoints Points { get { return this; } }
		#region Sparklines
		public SparklineCollection Sparklines {
			get {
				CheckValid();
				if (nativeSparklines == null)
					nativeSparklines = new NativeSparklineCollection(this);
				return nativeSparklines;
			}
		}
		#endregion
		#region Type
		public SparklineGroupType Type {
			get {
				CheckValid();
				return (SparklineGroupType)modelSparklineGroup.Type;
			}
			set {
				CheckValid();
				modelSparklineGroup.Type = (Model.SparklineGroupType)value;
			}
		}
		#endregion
		#region SeriesColor
		public Color SeriesColor {
			get {
				CheckValid();
				return modelSparklineGroup.ColorOf.Series;
			}
			set {
				CheckValid();
				modelSparklineGroup.ColorOf.Series = value;
			}
		}
		#endregion
		#region DisplayBlanksAs
		public DisplayBlanksAs DisplayBlanksAs {
			get {
				CheckValid();
				return (DisplayBlanksAs)modelSparklineGroup.DisplayBlanksAs;
			}
			set {
				CheckValid();
				modelSparklineGroup.DisplayBlanksAs = (Model.DisplayBlanksAs)value;
			}
		}
		#endregion
		#region ShowHidden
		public bool ShowHidden {
			get {
				CheckValid();
				return modelSparklineGroup.ShowHidden;
			}
			set {
				CheckValid();
				modelSparklineGroup.ShowHidden = value;
			}
		}
		#endregion
		#region LineWeight
		public double LineWeight {
			get {
				CheckValid();
				return modelSparklineGroup.LineWeightInPoints;
			}
			set {
				CheckValid();
				modelSparklineGroup.LineWeightInPoints = value;
			}
		}
		#endregion
		#region DateRange
		public Range DateRange {
			get {
				CheckValid();
				Model.CellRange dateRange = modelSparklineGroup.DateRange;
				if (dateRange == null)
					return null;
				return new NativeRange(dateRange, (NativeWorksheet)nativeWorksheet.Workbook.Worksheets[dateRange.Worksheet.Name]);
			}
			set {
				CheckValid();
				Model.CellRangeBase modelRange = value == null ? null : ((NativeWorksheet)value.Worksheet).GetModelRange(value);
				Model.SparklineGroupModifyDateRangeCommand command = new Model.SparklineGroupModifyDateRangeCommand(modelSparklineGroup, ApiErrorHandler.Instance, modelRange);
				command.Execute();
			}
		}
		#endregion
		#region Position
		public Range Position {
			get {
				CheckValid();
				Model.SparklineCollection sparklines = modelSparklineGroup.Sparklines;
				if (sparklines.Count == 0)
					return null;
				Model.Sparkline modelSparkline = sparklines[0];
				Model.CellRangeBase modelPosition = new Model.CellRange(modelSparkline.Sheet, modelSparkline.Position, modelSparkline.Position);
				for (int i = 1; i < sparklines.Count; i++) {
					modelSparkline = sparklines[i];
					modelPosition = modelPosition.MergeWithRange(new Model.CellRange(modelSparkline.Sheet, modelSparkline.Position, modelSparkline.Position));
				}
				return new NativeRange(modelPosition, nativeWorksheet);
			}
		}
		#endregion
		#endregion
		#region SparklineHorizontalAxis Members
		#region IsDateAxis
		bool SparklineHorizontalAxis.IsDateAxis {
			get {
				CheckValid();
				return modelSparklineGroup.UseDateAxis;
			}
		}
		#endregion
		#region RightToLeft
		bool SparklineHorizontalAxis.RightToLeft {
			get {
				CheckValid();
				return modelSparklineGroup.RightToLeft;
			}
			set {
				CheckValid();
				modelSparklineGroup.RightToLeft = value;
			}
		}
		#endregion
		#region IsVisible
		bool SparklineColor.IsVisible {
			get {
				CheckValid();
				return modelSparklineGroup.ShowXAxis;
			}
			set {
				CheckValid();
				modelSparklineGroup.ShowXAxis = value;
			}
		}
		#endregion
		#region Color
		Color SparklineColor.Color {
			get {
				CheckValid();
				return modelSparklineGroup.ColorOf.Axis;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					modelSparklineGroup.ColorOf.Axis = value;
					modelSparklineGroup.ShowXAxis = true;
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#endregion
		#region SparklineVerticalAxis Members
		#region MaxScaleType
		SparklineAxisScaling SparklineVerticalAxis.MaxScaleType {
			get {
				CheckValid();
				return (SparklineAxisScaling)modelSparklineGroup.MaxAxisScaleType;
			}
			set {
				CheckValid();
				modelSparklineGroup.MaxAxisScaleType = (Model.SparklineAxisScaling)value;
			}
		}
		#endregion
		#region MinScaleType
		SparklineAxisScaling SparklineVerticalAxis.MinScaleType {
			get {
				CheckValid();
				return (SparklineAxisScaling)modelSparklineGroup.MinAxisScaleType;
			}
			set {
				CheckValid();
				modelSparklineGroup.MinAxisScaleType = (Model.SparklineAxisScaling)value;
			}
		}
		#endregion
		#region MaxCustomValue
		double SparklineVerticalAxis.MaxCustomValue {
			get {
				CheckValid();
				return modelSparklineGroup.MaxAxisValue;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					modelSparklineGroup.MaxAxisValue = value;
					modelSparklineGroup.MaxAxisScaleType = Model.SparklineAxisScaling.Custom;
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#region MinCustomValue
		double SparklineVerticalAxis.MinCustomValue {
			get {
				CheckValid();
				return modelSparklineGroup.MinAxisValue;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					modelSparklineGroup.MinAxisValue = value;
					modelSparklineGroup.MinAxisScaleType = Model.SparklineAxisScaling.Custom;
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#endregion
		#region SparklinePoints Members
		SparklineColor SparklinePoints.First {
			get {
				CheckValid();
				if (nativeFirstColor == null)
					nativeFirstColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.First);
				return nativeFirstColor;
			}
		}
		SparklineColor SparklinePoints.Last {
			get {
				CheckValid();
				if (nativeLastColor == null)
					nativeLastColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.Last);
				return nativeLastColor;
			}
		}
		SparklineColor SparklinePoints.Highest {
			get {
				CheckValid();
				if (nativeHighestColor == null)
					nativeHighestColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.Highest);
				return nativeHighestColor;
			}
		}
		SparklineColor SparklinePoints.Lowest {
			get {
				CheckValid();
				if (nativeLowestColor == null)
					nativeLowestColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.Lowest);
				return nativeLowestColor;
			}
		}
		SparklineColor SparklinePoints.Negative {
			get {
				CheckValid();
				if (nativeNegativeColor == null)
					nativeNegativeColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.Negative);
				return nativeNegativeColor;
			}
		}
		SparklineColor SparklinePoints.Markers {
			get {
				CheckValid();
				if (nativeMarkersColor == null)
					nativeMarkersColor = new NativeSparklineColor(modelSparklineGroup, Model.SparklineColorType.Markers);
				return nativeMarkersColor;
			}
		}
		#endregion
		#endregion
		#region SetDefaultColors
		void SetDefaultColors() {
			if (!modelSparklineGroup.ColorsAreDefault)
				return;
			modelSparklineGroup.SetColorIndexCore(DefaultSeriesColor, Model.SparklineColorType.Series);
			modelSparklineGroup.SetColorIndexCore(DefaultAxisColor, Model.SparklineColorType.Axis);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.First);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.Last);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.Highest);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.Lowest);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.Markers);
			modelSparklineGroup.SetColorIndexCore(DefaultPointColor, Model.SparklineColorType.Negative);
		}
		#endregion
		#region SetIsValid
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeSparklines != null)
				nativeSparklines.IsValid = value;
			if (nativeFirstColor != null)
				nativeFirstColor.IsValid = value;
			if (nativeLastColor != null)
				nativeLastColor.IsValid = value;
			if (nativeHighestColor != null)
				nativeHighestColor.IsValid = value;
			if (nativeLowestColor != null)
				nativeLowestColor.IsValid = value;
			if (nativeNegativeColor != null)
				nativeNegativeColor.IsValid = value;
			if (nativeMarkersColor != null)
				nativeMarkersColor.IsValid = value;
			if (nativeSparklines != null)
				nativeSparklines.IsValid = value;
		}
		#endregion
		#region UnGroup
		public void UnGroup() {
			CheckValid();
			int count = Sparklines.Count;
			if (count < 2)
				return;
			DocumentModel.BeginUpdate();
			try {
				for (int i = 0; i < count; i++) {
					NativeSparkline sparkline = Sparklines[i] as NativeSparkline;
					Model.SparklineGroup newModelGroup = modelSparklineGroup.Clone(false);
					newModelGroup.Sparklines.Add(sparkline.ModelSparkline);
					nativeWorksheet.ModelWorksheet.SparklineGroups.Add(newModelGroup);
				}
				Delete();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		public void Delete() {
			CheckValid();
			nativeWorksheet.SparklineGroups.Remove(this);
		}
		internal bool IsInCell(Cell cell) {
			foreach (Model.Sparkline sparkline in ModelSparklineGroup.Sparklines) {
				if (sparkline.Position.Column == cell.ColumnIndex && sparkline.Position.Row == cell.RowIndex)
					return true;
			}
			return false;
		}
		internal bool IsInRange(Range range) {
			Model.CellRangeBase modelRange = nativeWorksheet.GetModelRange(range);
			foreach (Model.Sparkline sparkline in ModelSparklineGroup.Sparklines) {
				if (modelRange.ContainsCell(sparkline.Position.Column, sparkline.Position.Row)) {
					return true;
				}
			}
			return false;
		}
	}
	#endregion
	#region NativeSparklineColor
	partial class NativeSparklineColor : NativeObjectBase, SparklineColor {
		readonly Model.SparklineGroup modelSparklineGroup;
		readonly Model.SparklineColorType colorType;
		public NativeSparklineColor(Model.SparklineGroup modelSparklineGroup, Model.SparklineColorType colorType) {
			Guard.ArgumentNotNull(modelSparklineGroup, "modelSparklineGroup");
			System.Diagnostics.Debug.Assert(colorType != Model.SparklineColorType.Series);
			this.modelSparklineGroup = modelSparklineGroup;
			this.colorType = colorType;
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return modelSparklineGroup.GetColor(colorType);
			}
			set {
				CheckValid();
				modelSparklineGroup.DocumentModel.BeginUpdate();
				try {
					modelSparklineGroup.SetColor(value, colorType);
					SetIsVisible(true);
				}
				finally {
					modelSparklineGroup.DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#region IsVisible
		public bool IsVisible {
			get {
				CheckValid();
				return modelSparklineGroup.GetIsVisible(colorType);
			}
			set {
				CheckValid();
				SetIsVisible(value);
			}
		}
		void SetIsVisible(bool value) {
			switch (colorType) {
				case Model.SparklineColorType.First:
					modelSparklineGroup.ShowFirst = value;
					break;
				case Model.SparklineColorType.Last:
					modelSparklineGroup.ShowLast = value;
					break;
				case Model.SparklineColorType.Axis:
					modelSparklineGroup.ShowXAxis = value;
					break;
				case Model.SparklineColorType.Highest:
					modelSparklineGroup.ShowHighest = value;
					break;
				case Model.SparklineColorType.Lowest:
					modelSparklineGroup.ShowLowest = value;
					break;
				case Model.SparklineColorType.Negative:
					modelSparklineGroup.ShowNegative = value;
					break;
				case Model.SparklineColorType.Markers:
					modelSparklineGroup.ShowMarkers = value;
					break;
			}
		}
		#endregion
	}
	#endregion
	#region NativeSparkline
	partial class NativeSparkline : NativeObjectBase, Sparkline {
		readonly NativeSparklineCollection parent;
		readonly Model.Sparkline modelSparkline;
		public NativeSparkline(Model.Sparkline modelSparkline, NativeSparklineCollection parent) {
			Guard.ArgumentNotNull(modelSparkline, "modelSparkline");
			Guard.ArgumentNotNull(parent, "parent");
			this.parent = parent;
			this.modelSparkline = modelSparkline;
		}
		#region Properties
		protected internal Model.Sparkline ModelSparkline { get { return modelSparkline; } }
		protected internal NativeSparklineCollection Parent { get { return parent; } }
		IWorkbook NativeWorkbook { get { return parent.NativeWorksheet.Workbook; } }
		Model.DocumentModel DocumentModel { get { return NativeWorkbook.Model.DocumentModel; } }
		#endregion
		#region DataRange
		public Range DataRange {
			get {
				CheckValid();
				Model.CellRange dataRange = modelSparkline.SourceDataRange;
				if (dataRange == null)
					return null;
				return new NativeRange(dataRange, (NativeWorksheet)NativeWorkbook.Worksheets[dataRange.Worksheet.Name]);
			}
			set {
				CheckValid();
				Model.CellRangeBase modelRange = value == null ? null : ((NativeWorksheet)value.Worksheet).GetModelRange(value);
				Model.SparklineModifyDataRangeCommand command = new Model.SparklineModifyDataRangeCommand(modelSparkline, ApiErrorHandler.Instance, modelRange);
				command.Execute();
			}
		}
		#endregion
		#region Position
		public Range Position {
			get {
				CheckValid();
				Model.CellRange modelPosition = new Model.CellRange(modelSparkline.Sheet, modelSparkline.Position, modelSparkline.Position);
				return new NativeRange(modelPosition, parent.NativeWorksheet);
			}
		}
		public void SetPosition(int columnIndex, int rowIndex) {
			CheckValid();
			NativeIndicesChecker.CheckRowIndex(rowIndex);
			NativeIndicesChecker.CheckColumnIndex(columnIndex);
			modelSparkline.Position = new Model.CellPosition(columnIndex, rowIndex);
		}
		#endregion
		#region MoveTo
		public void MoveTo(SparklineGroup sparklineGroup) {
			CheckValid();
			NativeSparklineGroup nativeSparklineGroup = sparklineGroup as NativeSparklineGroup;
			Guard.ArgumentNotNull(nativeSparklineGroup, "sparklineGroup");
			if (!nativeSparklineGroup.IsValid)
				throw new InvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedObject));
			if (Object.ReferenceEquals(nativeSparklineGroup, parent.Parent))
				return;
			DocumentModel.BeginUpdate();
			try {
				nativeSparklineGroup.ModelSparklineGroup.Sparklines.Add(modelSparkline);
				Delete();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		public void Delete() {
			CheckValid();
			parent.Remove(this);
		}
		internal bool IsInCell(Cell cell) {
			return ModelSparkline.Position.Column == cell.ColumnIndex && ModelSparkline.Position.Row == cell.RowIndex;
		}
		internal bool IsInRange(Range range) {
			Model.CellRangeBase modelRange = parent.NativeWorksheet.GetModelRange(range);
			return modelRange.ContainsCell(ModelSparkline.Position.Column, ModelSparkline.Position.Row);
		}
	}
	#endregion
}
