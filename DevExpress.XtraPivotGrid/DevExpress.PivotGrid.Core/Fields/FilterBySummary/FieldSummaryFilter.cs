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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[CLSCompliant(false)]
	public interface IPivotCustomSummaryValue : IConvertible, IComparable {
		object FromDouble(double value);
	}
	[Flags]
	public enum PivotSummaryFilterValidity { Valid = 0, InvalidRange = 1, InvalidLevel = 2, Invalid = InvalidRange | InvalidLevel };
	public enum PivotSummaryFilterMode { LastLevel, SpecificLevel };
	public class PivotSummaryFilter : ISummaryFilter {
		const PivotSummaryFilterMode DefaultSummaryFilterMode = PivotSummaryFilterMode.LastLevel;
		PivotGridFieldBase rowField;
		PivotGridFieldBase columnField;
		object startValue;
		object endValue;
		object startValueInternal;
		object endValueInternal;
		PivotSummaryFilterMode mode;
		string rowFieldComponentName;
		string columnFieldComponentName;
		int lockUpdateCount;
		[XtraSerializableProperty(), MergableProperty(false), DefaultValue(null), NotifyParentProperty(true), Localizable(false)]
		public object StartValue {
			get { return startValue; }
			set {
				if(startValue == value) return;
				startValue = value;
				StartValueInternal = startValue;
				OnChanged();
			}
		}
		[XtraSerializableProperty(), MergableProperty(false), DefaultValue(null), NotifyParentProperty(true), Localizable(false)]
		public object EndValue {
			get { return endValue; }
			set {
				if(endValue == value) return;
				endValue = value;
				EndValueInternal = endValue;
				OnChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true)]
		internal object StartValueInternal {
			get { return startValueInternal; }
			private set { startValueInternal = GetInternalValue(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true)]
		internal object EndValueInternal {
			get { return endValueInternal; }
			private set { endValueInternal = GetInternalValue(value); }
		}
		object GetInternalValue(object sourceValue) {
			if(sourceValue == null || !HasOwner) return sourceValue;
			Type valueType = PivotSummaryValue.GetValueType(SummaryType, sourceValue.GetType());
			return PivotValueTypeConverter.ConvertTo(sourceValue, valueType);
		}
		internal PivotGridFieldBase GetLevelField(bool isColumn) {
			return isColumn ? ColumnField : RowField;
		}
		internal void SetLevelField(bool isColumn, PivotGridFieldBase field) {
			if(isColumn) {
				ColumnField = field;
			} else {
				RowField = field;
			}
		}
		[XtraSerializableProperty(), MergableProperty(false), DefaultValue(PivotSummaryFilterMode.LastLevel), NotifyParentProperty(true), Localizable(false)]
		public PivotSummaryFilterMode Mode {
			get { return mode; }
			set {
				if(mode == value) return;
				mode = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotSummaryFilterRowField"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotSummaryFilter.RowField"),
		DefaultValue(null), NotifyParentProperty(true)
		]
		public PivotGridFieldBase RowField {
			get { return rowField; }
			set {
				if(GetIsOwner(value)) value = null;
				if(RowField == value) return;
				rowField = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotSummaryFilterColumnField"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotSummaryFilter.ColumnField"),
		DefaultValue(null), NotifyParentProperty(true)
		]
		public PivotGridFieldBase ColumnField {
			get { return columnField; }
			set {
				if(GetIsOwner(value)) value = null;
				if(ColumnField == value) return;
				columnField = value;
				OnChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string RowFieldComponentName {
			get { return RowField != null ? RowField.ComponentName : string.Empty; }
			set { rowFieldComponentName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string ColumnFieldComponentName {
			get { return ColumnField != null ? ColumnField.ComponentName : string.Empty; }
			set { columnFieldComponentName = value; }
		}
		protected internal void OnGridDeserialized() {
			if(!HasOwner || !HasOwnerCollection ||
				(string.IsNullOrEmpty(rowFieldComponentName) && string.IsNullOrEmpty(columnFieldComponentName))) return;
			rowField = GetField(rowFieldComponentName);
			columnField = GetField(columnFieldComponentName);
			ResetComponentNames();
		}
		public void Apply(object startValue, object endValue) {
			BeginUpdate();
			this.Mode = PivotSummaryFilterMode.LastLevel;
			this.StartValue = startValue;
			this.EndValue = endValue;
			EndUpdate();
		}
		public void Apply(object startValue, object endValue, PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			BeginUpdate();
			this.Mode = PivotSummaryFilterMode.SpecificLevel;
			this.StartValue = startValue;
			this.EndValue = endValue;
			this.RowField = rowField;
			this.ColumnField = columnField;
			EndUpdate();
		}
		public void Apply(PivotSummaryFilter filter) {
			BeginUpdate();
			Assign(filter);
			EndUpdate();
		}
		public void Clear() {
			bool isEmpty = IsEmpty;
			BeginUpdate();
			this.StartValue = this.EndValue = null;
			this.RowField = this.ColumnField = null;
			this.Mode = DefaultSummaryFilterMode;
			ResetComponentNames();
			if(isEmpty) {
				CancelUpdate();
			} else {
				EndUpdate();
			}
		}
		public bool IsEmpty {
			get { return StartValue == null && EndValue == null; }
		}
		public PivotSummaryFilterValidity Validity {
			get {
				PivotSummaryFilterValidity validity = PivotSummaryFilterValidity.Valid;
				if(Mode == PivotSummaryFilterMode.SpecificLevel && ((RowField == null && ColumnField == null)
					|| (RowField != null && RowField.Area != PivotArea.RowArea) || (ColumnField != null && ColumnField.Area != PivotArea.ColumnArea)))
					validity |= PivotSummaryFilterValidity.InvalidLevel;
				if(StartValue == null || EndValue == null)
					return validity;
				try {
					bool isValidRange = Comparer<object>.Default.Compare(EndValueInternal, StartValueInternal) >= 0;
					return validity | (isValidRange ? PivotSummaryFilterValidity.Valid : PivotSummaryFilterValidity.InvalidRange);
				} catch {
					return validity | PivotSummaryFilterValidity.InvalidRange;
				}
			}
		}
		protected internal void Assign(PivotSummaryFilter filter) {
			if(filter == null) return;
			this.startValue = filter.StartValue;
			this.endValue = filter.EndValue;
			this.startValueInternal = filter.startValueInternal;
			this.endValueInternal = filter.endValueInternal;
			this.mode = filter.Mode;
			this.rowField = filter.RowField;
			this.columnField = filter.ColumnField;
			this.rowFieldComponentName = filter.rowFieldComponentName;
			this.columnFieldComponentName = filter.columnFieldComponentName;
		}
		void ResetComponentNames() {
			this.rowFieldComponentName = this.columnFieldComponentName = string.Empty;
		}
		bool IsLockUpdate { get { return lockUpdateCount > 0; } }
		void BeginUpdate() {
			lockUpdateCount++;
		}
		void EndUpdate() {
			CancelUpdate();
			OnChanged();
		}
		void CancelUpdate() {
			lockUpdateCount--;
		}
		#region Owner access
		readonly PivotGridFieldBase owner;
		internal PivotSummaryFilter(PivotGridFieldBase owner) {
			this.owner = owner;
			this.mode = DefaultSummaryFilterMode;
			ResetComponentNames();
		}
		PivotGridFieldBase Owner { get { return owner; } }
		protected bool HasOwner { get { return Owner != null; } }
		protected bool HasOwnerCollection { get { return Owner.Collection != null; } }
		protected PivotGridFieldBase GetField(string componentName) {
			return string.IsNullOrEmpty(componentName) ? null : Owner.GetFieldFromComponentName(componentName);
		}
		protected bool GetIsOwner(PivotGridFieldBase field) {
			return Owner == field;
		}
		protected PivotSummaryType SummaryType { get { return Owner.SummaryType; } }
		protected virtual bool CanApplyChanges { get { return true; } }
		protected void OnChanged() {
			if(IsLockUpdate) return;
			if(HasOwner && CanApplyChanges && !Owner.IsDataDeserializing)
				Owner.OnSummaryFilterChanged();
		}
		#endregion
	}
	class PivotSummaryFilterInternal : PivotSummaryFilter {
		internal PivotSummaryFilterInternal(PivotGridFieldBase owner) : base(owner) {
			this.Assign(owner.SummaryFilter);
		}
		protected override bool CanApplyChanges {
			get { return false; }
		}
	}
	public static class PivotValueTypeConverter {
		public static object ConvertTo(object value, Type type) {
			if(value == null) return null;
			if(value.GetType() == type) return value;
			try {
				return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
			} catch {
				return null;
			}
		}
	}
}
