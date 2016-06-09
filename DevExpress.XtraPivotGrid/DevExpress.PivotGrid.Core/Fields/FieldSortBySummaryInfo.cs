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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridFieldSortBySummaryInfo : ViewStatePersisterCore, IXtraSupportDeserializeCollection, IXtraSupportDeserializeCollectionItem {
		PivotGridFieldBase owner;
		PivotGridFieldBase field;
		string fieldName;
		string fieldComponentName;
		PivotSummaryType summaryType;
		PivotSummaryType? customTotalSummaryType;
		PivotGridFieldSortConditionCollection conditions;
		public PivotGridFieldSortBySummaryInfo(PivotGridFieldBase owner, string objectPath)
			: base(owner, objectPath) {
			this.owner = owner;
			this.summaryType = PivotSummaryType.Sum;
			this.field = null;
			this.fieldName = string.Empty;
			this.fieldComponentName = string.Empty;
			this.conditions = new PivotGridFieldSortConditionCollection(Owner);
			Conditions.Changed += ConditionsChanged;
		}
		public void Assign(PivotGridFieldSortBySummaryInfo sortInfo) {
			this.summaryType = sortInfo.summaryType;
			this.customTotalSummaryType = sortInfo.customTotalSummaryType;
			this.field = sortInfo.field;
			this.fieldName = sortInfo.fieldName;
			this.fieldComponentName = sortInfo.fieldComponentName;
			Conditions.AddRange(sortInfo.Conditions.Clone());
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoField"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.Field"),
		DefaultValue(null), NotifyParentProperty(true)
		]
		public PivotGridFieldBase Field {
			get { return field; }
			set {
				if(value == Owner) value = null;
				if(Field == value) return;
				field = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoConditions"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.Conditions"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public PivotGridFieldSortConditionCollection Conditions {
			get { return conditions; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConditions() {
			return Conditions.Count > 0;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetConditions() {
			Conditions.Clear();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string FieldComponentName {
			get { return Field != null ? Field.ComponentName : string.Empty; }
			set { fieldComponentName = value; }
		}
		protected internal void OnGridDeserialized() {
			if(Owner == null || Owner.Collection == null || (string.IsNullOrEmpty(fieldComponentName) && string.IsNullOrEmpty(FieldName))) return;
			field = Owner.GetFieldFromComponentName(fieldComponentName);
			fieldComponentName = string.Empty;
			Conditions.Changed -= ConditionsChanged;
			for(int i = 0; i < Conditions.Count; i++) {
				if(string.IsNullOrEmpty(Conditions[i].FieldComponentName) || Conditions[i].Field != null)
					continue;
				Conditions[i].Field = Owner.GetFieldFromComponentName(Conditions[i].FieldComponentName);
				Conditions[i].FieldComponentName = null;
			}
			Conditions.Changed += ConditionsChanged;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoFieldName"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.FieldName"),
		Localizable(true),
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true)
#if !SL && !DXPORTABLE
		,Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotColumnNameEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))
#endif
		]
		public string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value) return;
				fieldName = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoSummaryType"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.SummaryType"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(SummaryType == value) return;
				summaryType = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoCustomTotalSummaryType"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo.CustomTotalSummaryType"),
		DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotSummaryType? CustomTotalSummaryType {
			get { return customTotalSummaryType; }
			set {
				if(CustomTotalSummaryType == value) return;
				customTotalSummaryType = value;
				OnChanged();
			}
		}
		public bool ShouldSerialize() {
			return !string.IsNullOrEmpty(FieldName) || Field != null || SummaryType != PivotSummaryType.Sum
				|| CustomTotalSummaryType != null || Conditions.Count > 0;
		}
		public void Reset() {
			SummaryType = PivotSummaryType.Sum;
			Field = null;
			FieldName = string.Empty;
			Conditions.Clear();
		}
		[Browsable(false)]
		public PivotGridFieldBase Owner { get { return owner; } }
		protected virtual void OnChanged() {
			if(Owner != null) Owner.OnSortBySummaryInfoChanged();
		}
		protected void ConditionsChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if(!string.IsNullOrEmpty(FieldName))
				sb.Append(FieldName).Append(" ");
			if(Field != null)
				sb.Append(Field).Append(" ");
			if(sb.Length == 0)
				sb.Append("(None)");
			else
				sb.Append(SummaryType);
			return sb.ToString();
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortBySummaryInfoIsEmpty"),
#endif
		Browsable(false)
		]
		public bool IsEmpty {
			get { return Field == null && String.IsNullOrEmpty(FieldName); }
		}
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
		}
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "Conditions") {
				Conditions.Clear();
				return true;
			} else
				throw new ArgumentException("propertyName");
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "Conditions") {
				PivotGridFieldSortCondition condition = new PivotGridFieldSortCondition();
				Conditions.Add(condition);
				return condition;
			}
			else
				throw new ArgumentException("propertyName");
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
		}
	}
	public class PivotGridFieldSortConditionCollection : Collection<PivotGridFieldSortCondition> {
		PivotGridFieldBase owner;
		protected PivotGridFieldBase Owner { get { return owner; } }
		protected bool IsDeserializing { get { return Owner.IsDataDeserializing; } }
		public PivotGridFieldSortConditionCollection(PivotGridFieldBase owner) {
			this.owner = owner;
		}
		public int IndexOf(PivotGridFieldBase field) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Field == field)
					return i;
			}
			return -1;
		}
		public bool Contains(PivotGridFieldBase field) {
			return IndexOf(field) >= 0;
		}
		public PivotGridFieldSortCondition this[PivotGridFieldBase field] {
			get {
				int index = IndexOf(field);
				if(index < 0) return null;
				return this[index];
			}
		}
		public void AddRange(IList<PivotGridFieldSortCondition> conditions) {
			for(int i = 0; i < conditions.Count; i++)
				Add(conditions[i]);
		}
		#region Collection overrides
		protected override void InsertItem(int index, PivotGridFieldSortCondition item) {
			CheckNewItem(item);
			base.InsertItem(index, item);
			SubscribeEvents(item);
			RaiseChanged();
		}
		protected override void RemoveItem(int index) {
			UnsubscribeEvents(this[index]);
			base.RemoveItem(index);
			RaiseChanged();
		}
		protected override void SetItem(int index, PivotGridFieldSortCondition item) {
			base.SetItem(index, item);
			SubscribeEvents(item);
			RaiseChanged();
		}
		protected override void ClearItems() {
			for(int i = 0; i < Count; i++)
				UnsubscribeEvents(this[i]);
			base.ClearItems();
			RaiseChanged();
		}
		protected virtual void ItemChanged(PivotGridFieldSortCondition item) {
			RaiseChanged();
		}
		protected void CheckNewItem(PivotGridFieldSortCondition item) {
			if(!IsDeserializing && Contains(item.Field))
				throw new ArgumentException("Duplicate entry: collection already contains this field's condition.");
		}
		#endregion
		protected void SubscribeEvents(PivotGridFieldSortCondition item) {
			if(item == null) return;
			item.Changed += item_Changed;
		}
		protected void UnsubscribeEvents(PivotGridFieldSortCondition item) {
			if(item == null) return;
			item.Changed -= item_Changed;
		}
		void item_Changed(object sender, EventArgs e) {
			ItemChanged((PivotGridFieldSortCondition)sender);
		}
		public event EventHandler Changed;
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public List<PivotGridFieldSortCondition> GetSortedConditions() {
			List<PivotGridFieldSortCondition> sortedConditions = new List<PivotGridFieldSortCondition>(this);
			IComparer<PivotGridFieldBase> comparer = new PivotGridFieldAreaIndexComparer();
			sortedConditions.Sort(delegate(PivotGridFieldSortCondition x, PivotGridFieldSortCondition y) {
				if(x.Field == null) return -1;
				if(y.Field == null) return 1;
				return comparer.Compare(x.Field, y.Field);
			});
			return sortedConditions;
		}
		internal IList<PivotGridFieldSortCondition> Clone() {
			List<PivotGridFieldSortCondition> conditions = new List<PivotGridFieldSortCondition>();
			foreach(PivotGridFieldSortCondition condition in this)
				conditions.Add(condition.Clone());
			return conditions;
		}
	}
	public class PivotGridFieldSortCondition {
		PivotGridFieldBase field;
		object value;
		string olapUniqueName;
		string fieldComponentName;
		public PivotGridFieldSortCondition() : this(null, null) { }
		public PivotGridFieldSortCondition(PivotGridFieldBase field, object value)
			: this(field, value, null) { }
		public PivotGridFieldSortCondition(PivotFieldValueItem item)
			: this(item.Data.GetField(item.Field), item.Value, null) {
#if !SL
			IOLAPMember member = item.Data.GetOLAPMember(item.IsColumn, item.VisibleIndex);
			if(member != null)
				this.olapUniqueName = member.UniqueName;
#endif
		}
		public PivotGridFieldSortCondition(PivotGridFieldBase field, object value, string olapUniqueName) {
			this.field = field;
			this.value = value;
			this.olapUniqueName = olapUniqueName;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortConditionField"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.Field"),
		DefaultValue(null),
		]
		public PivotGridFieldBase Field {
			get { return field; }
			set {
				if(Field == value) return;
				field = value;
				RaiseChanged();
			}
		}
		[
		DefaultValue(null), XtraSerializableProperty(),
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortConditionValue"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.Value"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#if !SL && !DXPORTABLE
		[
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
#endif
		public object Value {
			get { return value; }
			set {
				if(Value == value) return;
				this.value = value;
				RaiseChanged();
			}
		}
		[
		DefaultValue(null), XtraSerializableProperty(),
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldSortConditionOLAPUniqueMemberName"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldSortCondition.OLAPUniqueMemberName"),
		]
		public string OLAPUniqueMemberName {
			get { return olapUniqueName; }
			set {
				if(OLAPUniqueMemberName == value) return;
				olapUniqueName = value;
				RaiseChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true)]
		public string FieldComponentName {
			get { return Field != null ? Field.ComponentName : fieldComponentName; }
			set { fieldComponentName = value; }
		}
		public event EventHandler Changed;
		protected void RaiseChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public bool IsEqual(PivotGridFieldSortCondition condition, Comparison<object> compareValues, bool isOLAP) {
			return (!isOLAP && compareValues(Value, condition.Value) == 0) ||
				(isOLAP && OLAPUniqueMemberName == condition.OLAPUniqueMemberName);
		}
		internal PivotGridFieldSortCondition Clone() {
			return new PivotGridFieldSortCondition(field, value, olapUniqueName);
		}
	}
}
