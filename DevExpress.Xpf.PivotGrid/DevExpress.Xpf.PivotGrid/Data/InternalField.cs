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
using DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum FieldSyncProperty {
		Area,
		AreaIndex,
		Caption,
		DisplayFolder,
		EmptyCellText,
		EmptyValueText,
		ExpandedInFieldsGroup,
		FieldName,
		Filtered,
		KPIGraphic,
		Height,
		Width,
		MinHeight,
		MinWidth,
		SortBy,
		SortMode,
		SortOrder,
		Summary,
		UnboundName,
		UnboundType,		
		Visible,
		DisplayText
	};
	public class FieldSyncPropertyEventArgs : EventArgs {
		FieldSyncProperty? syncProperty;
		bool read, write;
		public FieldSyncPropertyEventArgs(FieldSyncProperty? syncProperty) {
			this.syncProperty = syncProperty;
			this.read = true;
			this.write = false;
		}
		public FieldSyncPropertyEventArgs(FieldSyncProperty? syncProperty, bool read, bool write) {
			this.syncProperty = syncProperty;
			this.read = read;
			this.write = write;
		}
		public bool Read { get { return read; } }
		public bool Write { get { return write; } }		
		public FieldSyncProperty? SyncProperty { get { return syncProperty; } }
	}
	public class PivotGridInternalField : PivotGridFieldBase, IPivotFieldSyncPropertyOwner {
		int height, minHeight;
		PivotGridField wrapper;
		public PivotGridInternalField()
			: this(string.Empty, PivotArea.FilterArea) { }
		public PivotGridInternalField(PivotGridWpfData data)
			: base(data) {
			Init();
		}
		public PivotGridInternalField(string fieldName, PivotArea area)
			: base(fieldName, area) {
			Init();
		}
		protected virtual void Init() {
			this.height = PivotGridField.DefaultHeight;
			this.minHeight = PivotGridField.DefaultMinHeight;
		}
		protected internal PivotGridWpfData WpfData { get { return (PivotGridWpfData)base.Data; } }
		internal PivotGridControl PivotGrid { get { return WpfData != null ? WpfData.PivotGrid : null; } }
		internal event EventHandler<FieldSyncPropertyEventArgs> FieldChanged;
		public new PivotGridInternalFieldOptions Options { get { return (PivotGridInternalFieldOptions)base.Options; } }
		public new FieldFilterValues FilterValues { get { return (FieldFilterValues)base.FilterValues; } }
		protected override bool CanFilterBySummary {
			get { return false; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeight"),
#endif
 Category("Layout"),
		DefaultValue(PivotGridField.DefaultHeight), 
		Localizable(true), NotifyParentProperty(true),
		]
		public virtual int Height {
			get {
				if(height < 0)
					return PivotGridField.DefaultHeight;
				else return height;
			}
			set {
				if(value < -1) value = -1;
				if(value == Height) return;
				if(value >= 0 && value < MinHeight) return;
				height = value;
				OnHeightChanged();
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldMinHeight"),
#endif
		Category("Layout"), DefaultValue(PivotGridField.DefaultMinHeight),
		Localizable(true), NotifyParentProperty(true),
		]
		public virtual int MinHeight {
			get {
				if(minHeight <= 0)
					return PivotGridField.DefaultMinWidth;
				else return minHeight;
			}
			set {
				if(value < -1) value = -1;
				if(value > Height) value = Height;
				minHeight = value;
				OnPropertyChanged();
			}
		}
		public new PivotGridCustomTotalCollectionBase CustomTotals { get { return base.CustomTotals; } }
		public PivotGridField Wrapper {
			get { return wrapper; }
			internal set { wrapper = value; }
		}
		protected virtual void OnHeightChanged() {
			OnPropertyChanged();
			if(Visible && Data != null || IsDataField) {
				Data.OnFieldSizeChanged(this, false, true);
			}
		}
		protected override PivotGridFieldFilterValues CreateFilterValues() {
			return new FieldFilterValues(this);
		}
		protected override PivotGridFieldOptions CreateOptions(PivotOptionsChangedEventHandler eventHandler, string name) {
			return new PivotGridInternalFieldOptions(eventHandler, this, "Options");
		}
		protected void RaisePropertyChanged(FieldSyncProperty? property) {
			if(FieldChanged != null)
				FieldChanged(this, new FieldSyncPropertyEventArgs(property));
		}
		protected override void OnAreaChanged(bool doRefresh) {
			base.OnAreaChanged(doRefresh);
			RaisePropertyChanged(FieldSyncProperty.Area);
		}
		protected override void OnAreaIndexCoreChagned() {
			base.OnAreaIndexCoreChagned();
			RaisePropertyChanged(FieldSyncProperty.AreaIndex);
		}
		protected override void OnVisibleChanged(int oldAreaIndex, bool doRefresh) {
			StartEventsRecording();
			base.OnVisibleChanged(oldAreaIndex, doRefresh);
			RaisePropertyChanged(FieldSyncProperty.Visible);
			FinishEventsRecording();
		}
		void StartEventsRecording() {
			if(Data != null)
				WpfData.StartRecordingInFieldSync();
		}
		void FinishEventsRecording() {
			if(Data != null)
				WpfData.FinishRecordingInFieldSync();
		}
		protected override void OnUnboundFieldNameChanged() {
			base.OnUnboundFieldNameChanged();
			RaisePropertyChanged(FieldSyncProperty.UnboundName);
		}
		protected override void OnUnboundChanged() {
			base.OnUnboundChanged();
			RaisePropertyChanged(FieldSyncProperty.UnboundType);
		}
		protected override void OnSortBySummaryInfoChanged() {
			base.OnSortBySummaryInfoChanged();
			RaisePropertyChanged(FieldSyncProperty.SortBy);
		}
		protected override void OnExpandedInFieldsGroupChanged() {
			base.OnExpandedInFieldsGroupChanged();
			RaisePropertyChanged(FieldSyncProperty.ExpandedInFieldsGroup);
		}
		protected override void OnSortModeChanged() {
			base.OnSortModeChanged();
			RaisePropertyChanged(FieldSyncProperty.SortMode);
		}
		protected override void OnSortOrderChanged() {
			base.OnSortOrderChanged();
			RaisePropertyChanged(FieldSyncProperty.SortOrder);
		}
		protected override void OnSummaryChanged(DevExpress.Data.PivotGrid.PivotSummaryType oldSummaryType) {
			base.OnSummaryChanged(oldSummaryType);
			RaisePropertyChanged(FieldSyncProperty.Summary);
		}
		protected override void OnEmptyCellTextChanged() {
			base.OnEmptyCellTextChanged();
			RaisePropertyChanged(FieldSyncProperty.EmptyCellText);
		}
		protected override void OnEmptyValueTextChanged() {
 			base.OnEmptyValueTextChanged();
			RaisePropertyChanged(FieldSyncProperty.EmptyValueText);
		}
		protected override void OnFieldNameChanged() {
			base.OnFieldNameChanged();
			RaisePropertyChanged(FieldSyncProperty.FieldName);
		}
		protected override void OnCaptionChanged() {
			base.OnCaptionChanged();
			RaisePropertyChanged(FieldSyncProperty.Caption);
		}
		protected override void OnDisplayFolderChanged() {
			base.OnDisplayFolderChanged();
			RaisePropertyChanged(FieldSyncProperty.DisplayFolder);
		}
		protected internal void AfterFilteredValueChanged() {
			RaisePropertyChanged(FieldSyncProperty.Filtered);
		}
		protected override void OnShowSummaryTypeNameChanged() {
			base.OnShowSummaryTypeNameChanged();
			RaisePropertyChanged(FieldSyncProperty.DisplayText);
		}
		#region IPivotFieldSyncPropertyOwner Members
		void ThrowNotImplementedException() {
			throw new NotImplementedException();
		}
		FieldAllowedAreas IPivotFieldSyncPropertyOwner.AllowedAreas {
			get { return AllowedAreas.ToFieldAllowedAreas(); }
			set { AllowedAreas = value.ToPivotAllowedAreas(); }
		}
		FieldArea IPivotFieldSyncPropertyOwner.Area {
			get { return Area.ToFieldArea(); }
			set { Area = value.ToPivotArea(); }
		}
		FieldGroupInterval IPivotFieldSyncPropertyOwner.GroupInterval {
			get { return GroupInterval.ToFieldGroupInterval(); }
			set { GroupInterval = value.ToPivotGroupInterval(); }
		}
		int IPivotFieldSyncPropertyOwner.AreaIndex {
			get { return AreaIndex; }
			set { AreaIndex = value; }
		}
		bool IPivotFieldSyncPropertyOwner.Visible {
			get { return Visible; }
			set { Visible = value; }
		}
		bool IPivotFieldSyncPropertyOwner.Filtered {
			get { 
				ThrowNotImplementedException();
				return false;
			}
			set { ThrowNotImplementedException(); }
		}
		bool? IPivotFieldSyncPropertyOwner.UseNativeFormat {
			get { return UseNativeFormat.ToNullableBoolean(); }
			set { UseNativeFormat = value.ToDefaultBoolean(); }
		}
		string IPivotFieldSyncPropertyOwner.Caption {
			get { return Caption; }
			set { Caption = value; }
		}
		string IPivotFieldSyncPropertyOwner.DisplayFolder {
			get { return DisplayFolder; }
			set { DisplayFolder = value; }
		}
		string IPivotFieldSyncPropertyOwner.HeaderDisplayText {
			get { return HeaderDisplayText; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.FieldName {
			get { return FieldName; }
			set { FieldName = value; }
		}
		string IPivotFieldSyncPropertyOwner.UnboundFieldName {
			get { return UnboundFieldName; }
			set { UnboundFieldName = value; }
		}
		string IPivotFieldSyncPropertyOwner.EmptyCellText {
			get { return EmptyCellText; }
			set { EmptyCellText = value; }
		}
		string IPivotFieldSyncPropertyOwner.EmptyValueText {
			get { return EmptyValueText; }
			set { EmptyValueText = value; }
		}
		int IPivotFieldSyncPropertyOwner.Height {
			get { return Height; }
			set { Height = value; }
		}
		int IPivotFieldSyncPropertyOwner.Width {
			get { return Width; }
			set { Width = value; }
		}
		SortByConditionCollection IPivotFieldSyncPropertyOwner.SortByConditions {
			get {
				SortByConditionCollection conditions = new SortByConditionCollection(null, this.GetWrapper());
				foreach(PivotGridFieldSortCondition internalCondition in SortBySummaryInfo.Conditions) {
					SortByCondition condition = new SortByCondition(internalCondition);
					conditions.Add(condition);
				}
				return conditions;
			}
			set {
				if(Data != null && !value.AreEqual(SortBySummaryInfo.Conditions)) {
					Data.BeginUpdate();
					SortBySummaryInfo.Conditions.Clear();
					foreach(SortByCondition condition in value) {
						PivotGridInternalField field = condition.Field.GetInternalField();
						PivotGridFieldSortCondition internalCondition = new PivotGridFieldSortCondition(field, condition.Value, condition.OlapUniqueMemberName);
						SortBySummaryInfo.Conditions.Add(internalCondition);
					}
					Data.EndUpdate();
				}
			}
		}
		PivotGridField IPivotFieldSyncPropertyOwner.SortByField {
			get { return SortBySummaryInfo.Field.GetWrapper(); }
			set { SortBySummaryInfo.Field = (value != null) ? value.InternalField : null; }
		}
		string IPivotFieldSyncPropertyOwner.SortByFieldName {
			get { return SortBySummaryInfo.FieldName; }
			set { SortBySummaryInfo.FieldName = value; }
		}
		FieldSummaryType IPivotFieldSyncPropertyOwner.SortBySummaryType {
			get { return SortBySummaryInfo.SummaryType.ToFieldSummaryType(); }
			set { SortBySummaryInfo.SummaryType = value.ToPivotSummaryType(); }
		}
		FieldSummaryType? IPivotFieldSyncPropertyOwner.SortByCustomTotalSummaryType {
			get { return SortBySummaryInfo.CustomTotalSummaryType.ToNullableFieldSummaryType(); }
			set { SortBySummaryInfo.CustomTotalSummaryType = value.ToNullablePivotSummaryType(); }
		}
		FieldSortOrder IPivotFieldSyncPropertyOwner.SortOrder {
			get { return SortOrder.ToFieldSortOrder(); }
			set { SortOrder = value.ToPivotSortOrder(); }
		}
		FieldUnboundColumnType IPivotFieldSyncPropertyOwner.UnboundType {
			get { return UnboundType.ToFieldUnboundColumnType(); }
			set { UnboundType = value.ToUnboundColumnType(); }
		}
		FieldSortMode IPivotFieldSyncPropertyOwner.SortMode {
			get { return SortMode.ToFieldSortMode(); }
			set { SortMode = value.ToPivotSortMode(); }
		}
		FieldSortMode IPivotFieldSyncPropertyOwner.ActualSortMode {
			get { return ActualSortMode.ToFieldSortMode(); }
			set { ActualSortMode = value.ToPivotSortMode(); }
		}
		string IPivotFieldSyncPropertyOwner.GrandTotalText {
			set { GrandTotalText = value; }
		}
		FieldSummaryType IPivotFieldSyncPropertyOwner.SummaryType {
			get { return SummaryType.ToFieldSummaryType(); }
			set { SummaryType = value.ToPivotSummaryType(); }
		}
		FieldSummaryDisplayType IPivotFieldSyncPropertyOwner.SummaryDisplayType {
			get { return SummaryDisplayType.ToFieldSummaryDisplayType(); }
			set { SummaryDisplayType = value.ToPivotSummaryDisplayType(); }
		}
		FieldTopValueType IPivotFieldSyncPropertyOwner.TopValueType {
			get { return TopValueType.ToFieldTopValueType(); }
			set { TopValueType = value.ToPivotTopValueType(); }
		}
		FieldTotalsVisibility IPivotFieldSyncPropertyOwner.TotalsVisibility {
			get { return TotalsVisibility.ToFieldTotalsVisibility(); }
			set { TotalsVisibility = value.ToPivotTotalsVisibility(); }
		}
		bool IPivotFieldSyncPropertyOwner.OLAPUseNonEmpty { get { return Options.OLAPUseNonEmpty; } set { Options.OLAPUseNonEmpty = value; } }
		bool? IPivotFieldSyncPropertyOwner.OLAPFilterByUniqueName { get { return Options.OLAPFilterByUniqueName.ToNullableBoolean(); } set { Options.OLAPFilterByUniqueName = value.ToDefaultBoolean(); } }
		#endregion
	}
	public class PivotGridInternalFieldCollection : PivotGridFieldCollectionBase {
		public PivotGridInternalFieldCollection(PivotGridWpfData data)
			: base(data) {
		}
		protected internal new PivotGridWpfData Data { get { return (PivotGridWpfData)base.Data; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCollectionItem"),
#endif
		EditorBrowsable(EditorBrowsableState.Always)
		]
		public new PivotGridInternalField this[int index] { get { return (PivotGridInternalField)base[index]; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCollectionItem"),
#endif
		EditorBrowsable(EditorBrowsableState.Always)
		]
		public new PivotGridInternalField this[string fieldName] { get { return (PivotGridInternalField)base[fieldName]; } }
		public PivotGridInternalField FieldByName(string name) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Name == name)
					return this[i];
			}
			return null;
		}
		public new PivotGridInternalField Add() {
			return (PivotGridInternalField)base.Add();
		}
		public new PivotGridInternalField Add(string fieldName, PivotArea area) {
			return (PivotGridInternalField)base.Add(fieldName, area);
		}
		public void Add(PivotGridInternalField field) {
			base.Add(field);
		}
		public virtual void AddRange(PivotGridInternalField[] fields) {
			foreach(PivotGridInternalField field in fields) {
				AddCore(field);
			}
		}
		public void Remove(PivotGridInternalField field) {
			List.Remove(field);
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			return new PivotGridInternalField(fieldName, area);
		}
		protected override void UpdateAreaIndexes(PivotArea area, PivotGridFieldBase ignoreField) {
			base.UpdateAreaIndexes(area, ignoreField);
			if(area == PivotArea.RowArea && Data != null && Data.IsRowTree)
				Data.RowTreeField.SyncFieldAreaIndex(true, false);
			if(Data != null && area == Data.DataField.InternalField.Area && Data.PivotGrid != null) {
					PivotGridControl.InvokeAction(() => {
						Data.DataField.AreaIndex = Data.OptionsDataField.AreaIndex;
						Data.PivotGrid.DataFieldAreaIndex = Data.OptionsDataField.AreaIndex;
						Data.PivotGrid.DataFieldArea = Data.OptionsDataField.Area.ToDataFieldAreaType();
					}, Data.PivotGrid);
			}
		}
		public override void GroupFieldsByHierarchies() {
			if(Data == null || !Data.IsOLAP || Data.IsDeserializing) return;
			base.GroupFieldsByHierarchies();
			Data.Groups.SyncGroupCollection(true, true);
			Data.Fields.SyncOLAPProperties();
		}
	}
	public class PivotGridInternalFieldOptions : PivotGridFieldOptions {
		bool allowUnboundExpressionEditor;
		public PivotGridInternalFieldOptions(PivotOptionsChangedEventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
				this.allowUnboundExpressionEditor = false;
		}
		public virtual bool AllowUnboundExpressionEditor {
			get {
				return allowUnboundExpressionEditor;
			}
			set {
				if(allowUnboundExpressionEditor == value) return;
				allowUnboundExpressionEditor = value;
				OnOptionsChanged(FieldOptions.AllowUnboundExpressionEditor);
			}
		}
	}
}
