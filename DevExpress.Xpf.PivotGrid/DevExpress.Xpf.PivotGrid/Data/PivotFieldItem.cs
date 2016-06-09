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

using DevExpress.Xpf.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Utils;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public interface IPivotFieldSyncPropertyOwner {
		FieldAllowedAreas AllowedAreas { get; set; }
		FieldArea Area { get; set; }
		FieldGroupInterval GroupInterval { get; set; }
		int AreaIndex { get; set; }
		bool OLAPUseNonEmpty { get; set; }
		bool? OLAPFilterByUniqueName { get; set; }
		bool ExpandedInFieldsGroup { get; set; }
		bool Visible { get; set; }
		bool Filtered { get; set; }
		bool RunningTotal { get; set; }
		bool ShowNewValues { get; set; }
		bool TopValueShowOthers { get; set; }
		bool? UseNativeFormat { get; set; }
		string Caption { get; set; }
		string DisplayFolder { get; set; }
		string HeaderDisplayText { get; set; }
		string FieldName { get; set; }
		string UnboundFieldName { get; set; }
		string EmptyCellText { get; set; }
		string EmptyValueText { get; set; }
		string GrandTotalText { set; }
		string UnboundExpression { get; set; }
		int GroupIntervalNumericRange { get; set; }
		int Height { get; set; }
		int Width { get; set; }
		int MinHeight { get; set; }
		int MinWidth { get; set; }
		int TopValueCount { get; set; }
		SortByConditionCollection SortByConditions { get; set; }
		PivotGridField SortByField { get; set; }
		string SortByFieldName { get; set; }
		FieldSummaryType SortBySummaryType { get; set; }
		FieldSummaryType? SortByCustomTotalSummaryType { get; set; }
		FieldSortOrder SortOrder { get; set; }
		FieldUnboundColumnType UnboundType { get; set; }
		FieldSortMode ActualSortMode { get; set; }
		FieldSortMode SortMode { get; set; }
		FieldSummaryType SummaryType { get; set; }
		string[] AutoPopulatedProperties { get; set; }
		string SortByAttribute { get; set; }
		FieldSummaryDisplayType SummaryDisplayType { get; set; }
		FieldTopValueType TopValueType { get; set; }
		FieldTotalsVisibility TotalsVisibility { get; set; }
	}
	public class PivotFieldItem : PivotFieldItemBase, IPivotFieldSyncPropertyOwner {
		int height;
		int minHeight;
		PivotGridField wrapper;
		public PivotFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridInternalField field)
			: base(data, groupItems, field) {
			this.height = field.Height;
			this.minHeight = field.MinHeight;
			this.wrapper = field.Wrapper;
		}
		public int Height {
			get { return height; }
		}
		public int MinHeight {
			get { return minHeight; }
		}
		public PivotGridField Wrapper {
			get { return wrapper; }
		}
		#region IPivotFieldSyncPropertyOwner Members
		void ThrowNotImplementedException() {
			throw new System.NotImplementedException();
		}
		bool IPivotFieldSyncPropertyOwner.OLAPUseNonEmpty {
			get { return true; }
			set { ThrowNotImplementedException(); }
		}
		bool? IPivotFieldSyncPropertyOwner.OLAPFilterByUniqueName {
			get { return false; }
			set { ThrowNotImplementedException(); }
		}
		FieldAllowedAreas IPivotFieldSyncPropertyOwner.AllowedAreas {
			get { return AllowedAreas.ToFieldAllowedAreas(); }
			set { ThrowNotImplementedException(); }
		}
		FieldArea IPivotFieldSyncPropertyOwner.Area {
			get { return Area.ToFieldArea(); }
			set { ThrowNotImplementedException(); }
		}
		FieldGroupInterval IPivotFieldSyncPropertyOwner.GroupInterval {
			get { return GroupInterval.ToFieldGroupInterval(); }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.AreaIndex {
			get { return AreaIndex; }
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.Visible {
			get { return Visible; }
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.RunningTotal {
			get { return RunningTotal; }
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.ShowNewValues {
			get { return ShowNewValues; }
			set { ThrowNotImplementedException(); }
		}
		bool? IPivotFieldSyncPropertyOwner.UseNativeFormat {
			get { return UseNativeFormat.ToNullableBoolean(); }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.Caption {
			get { return Caption; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.DisplayFolder {
			get { return DisplayFolder; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.HeaderDisplayText {
			get { return HeaderDisplayText; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.FieldName {
			get { return FieldName; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.UnboundFieldName {
			get { return UnboundFieldName; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.UnboundExpression {
			get { return UnboundExpression; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.GroupIntervalNumericRange {
			get { return GroupIntervalNumericRange; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.Height {
			get { return Height; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.Width {
			get { return Width; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.MinHeight {
			get { return MinHeight; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.MinWidth {
			get { return MinWidth; }
			set { ThrowNotImplementedException(); }
		}
		int IPivotFieldSyncPropertyOwner.TopValueCount {
			get { return TopValueCount; }
			set { ThrowNotImplementedException(); }
		}
		FieldSortOrder IPivotFieldSyncPropertyOwner.SortOrder {
			get { return SortOrder.ToFieldSortOrder(); }
			set { ThrowNotImplementedException(); }
		}
		FieldUnboundColumnType IPivotFieldSyncPropertyOwner.UnboundType {
			get { return UnboundType.ToFieldUnboundColumnType(); }
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.ExpandedInFieldsGroup {
			get { return ExpandedInFieldsGroup; }
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.Filtered {
			get {
				ThrowNotImplementedException();
				return false;
			}
			set { ThrowNotImplementedException(); }
		}
		bool IPivotFieldSyncPropertyOwner.TopValueShowOthers {
			get { return TopValueShowOthers; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.EmptyCellText {
			get { return EmptyCellText; }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.EmptyValueText {
			get { return EmptyValueText; }
			set { ThrowNotImplementedException(); }
		}
		SortByConditionCollection IPivotFieldSyncPropertyOwner.SortByConditions {
			get {
				ThrowNotImplementedException();
				return null;
			}
			set { ThrowNotImplementedException(); }
		}
		PivotGridField IPivotFieldSyncPropertyOwner.SortByField {
			get { return SortBySummaryInfo.Field.GetWrapper(); }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.SortByFieldName {
			get { return SortBySummaryInfo.FieldName; }
			set { ThrowNotImplementedException(); }
		}
		FieldSummaryType IPivotFieldSyncPropertyOwner.SortBySummaryType {
			get { return SortBySummaryInfo.SummaryType.ToFieldSummaryType(); }
			set { ThrowNotImplementedException(); }
		}
		FieldSummaryType? IPivotFieldSyncPropertyOwner.SortByCustomTotalSummaryType {
			get { return SortBySummaryInfo.CustomTotalSummaryType.ToNullableFieldSummaryType(); }
			set { ThrowNotImplementedException(); }
		}
		FieldSortMode IPivotFieldSyncPropertyOwner.SortMode {
			get { return SortMode.ToFieldSortMode(); }
			set { ThrowNotImplementedException(); }
		}
		FieldSortMode IPivotFieldSyncPropertyOwner.ActualSortMode {
			get { return ActualSortMode.ToFieldSortMode(); }
			set { ThrowNotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.GrandTotalText {
			set { ThrowNotImplementedException(); }
		}
		FieldSummaryType IPivotFieldSyncPropertyOwner.SummaryType {
			get { return SummaryType.ToFieldSummaryType(); }
			set { ThrowNotImplementedException(); }
		}
		FieldSummaryDisplayType IPivotFieldSyncPropertyOwner.SummaryDisplayType {
			get { return SummaryDisplayType.ToFieldSummaryDisplayType(); }
			set { ThrowNotImplementedException(); }
		}
		FieldTopValueType IPivotFieldSyncPropertyOwner.TopValueType {
			get { return TopValueType.ToFieldTopValueType(); }
			set { ThrowNotImplementedException(); }
		}
		FieldTotalsVisibility IPivotFieldSyncPropertyOwner.TotalsVisibility {
			get { return TotalsVisibility.ToFieldTotalsVisibility(); }
			set { ThrowNotImplementedException(); }
		}
		#endregion
		string[] IPivotFieldSyncPropertyOwner.AutoPopulatedProperties {
			get { return AutoPopulatedProperties; }
			set { throw new System.NotImplementedException(); }
		}
		string IPivotFieldSyncPropertyOwner.SortByAttribute {
			get { return SortByAttribute; }
			set { throw new System.NotImplementedException(); }
		}
	}
}
