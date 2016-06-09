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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid {
	public interface IPrefilterOwnerBase {
		void CriteriaChanged();
	}
	public enum PrefilterState { Normal, Invalid }
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class PrefilterBase : IDisposable {
		CriteriaOperator criteria;
		PrefilterState state;
		bool enabled;
		List<string> prefilterColumnNames;
		[Browsable(false)]
		public static string InvalidCriteriaDisplayText {
			get { return PivotGridLocalizer.GetString(PivotGridStringId.PrefilterInvalidCriteria); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrefilterBaseEnabled"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.Enabled"),
		XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true)
		]
		public bool Enabled {
			get { return EnabledCore; }
			set {
				if(EnabledCore == value) return;
				EnabledCore = value;
				RaiseCriteriaChanged();
			}
		}
		protected bool EnabledCore { get { return enabled; } set { enabled = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrefilterBaseCriteria"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.Criteria"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(),
		DefaultValue(null),
		NotifyParentProperty(true)
		]
		public virtual CriteriaOperator Criteria {
			get { return criteria; }
			set {
				if(object.Equals(criteria, value)) return;
				criteria = value;
				prefilterColumnNames = null;
				RaiseCriteriaChanged();
			}
		}
		protected internal void PatchPrefilterColumnNames(PivotGridFieldCollectionBase fields, bool patchDataFields) {
			if(IsEmpty) return;
			prefilterColumnNames = null;
			new OperatorNameToPrefilterColumnNamePatcher(fields, patchDataFields).ApplyPatch(Criteria);
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrefilterBaseCriteriaString"),
#endif
		DefaultValue(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.CriteriaString"),
		NotifyParentProperty(true)
		]
		public virtual string CriteriaString {
			get { return Convert.ToString(Criteria); }
			set { 
				if(CriteriaString != value)
					Criteria = CriteriaOperator.Parse(value);
			}
		}
		readonly IPrefilterOwnerBase owner;
		protected IPrefilterOwnerBase Owner { get { return owner; } }
		protected void RaiseCriteriaChanged() {
			if (Owner == null) return;
			Owner.CriteriaChanged();
		}
		public void Clear() {
			Criteria = null;
		}
		[
		Browsable(false),
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrefilterBaseIsEmpty")
#else
	Description("")
#endif
		]
		public bool IsEmpty {
			get {
				return ReferenceEquals(Criteria, null);
			}
		}
		[
		Browsable(false),
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrefilterBaseState")
#else
	Description("")
#endif
		]
		public PrefilterState State {
			get { 
				return state; 
			}
			private set {
				if(value == state) return;
				state = value;
				EnabledCore = (State != PrefilterState.Invalid);
				RaiseCriteriaChanged();
			}
		}
		[Browsable(false)]
		public List<string> PrefilterColumnNames {
			get {
				if(prefilterColumnNames == null) {
					if(!IsEmpty) {
						ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
						Criteria.Accept(visitor);
						prefilterColumnNames = visitor.ColumnNames;
					} else
						prefilterColumnNames = new List<string>();
				}
				return prefilterColumnNames;
			}
		}
		public PrefilterBase(IPrefilterOwnerBase owner) {
			this.owner = owner;
			this.enabled = true;
			this.state = PrefilterState.Normal;
		}
		public virtual void Dispose() {			
		}
		public void Assign(PrefilterBase prefilter) {
			this.enabled = prefilter.Enabled;
			this.criteria = CriteriaOperator.Clone(prefilter.Criteria);
			this.state = prefilter.State;
			RaiseCriteriaChanged();
		}
		public bool Contains(PivotGridFieldBase field) {
			if(!Enabled || IsEmpty) return false;
			return PrefilterColumnNames.Contains(field.Name) || PrefilterColumnNames.Contains(field.DataControllerColumnName);
		}
		protected internal void UpdateState(PivotGridFieldCollectionBase fields) {
			State = PrefilterState.Normal;
			if(IsEmpty) return;
			foreach(string columnName in PrefilterColumnNames) {
				int counter = fields.Count;
				foreach(PivotGridFieldBase field in fields) {
					if(!string.Equals(columnName, field.Name) && !string.Equals(columnName, field.DataControllerColumnName)) {
						counter--;
					} else {
						if(!field.IsValidForPrefilter) counter = 0;
						break;
					}
				}
				if(counter == 0) {
					State = PrefilterState.Invalid;
					break;
				}
			}
		}		
	}
	public abstract class OperatorNamePatcher : DevExpress.Data.Filtering.Helpers.EvaluatorCriteriaValidator {
		readonly PivotGridFieldCollectionBase fields;
		readonly bool patchDataFields;
		public OperatorNamePatcher(PivotGridFieldCollectionBase fields, bool patchDataFields)
			: base(null) {
			this.fields = fields;
			this.patchDataFields = patchDataFields;
		}
		protected PivotGridFieldCollectionBase Fields { get { return fields; } } 
		protected abstract string GetPatchName(PivotGridFieldBase field);
		protected abstract PivotGridFieldBase GetFieldByPropertyName(string propertyName);
		public override void Visit(OperandProperty theOperand) {
			PivotGridFieldBase field = GetFieldByPropertyName(theOperand.PropertyName);
			if(field != null && (field.Area != PivotArea.DataArea || patchDataFields))
				theOperand.PropertyName = GetPatchName(field);
		}
		public CriteriaOperator Patch(CriteriaOperator criteria) {
			return PatchCore(criteria, true);
		}
		protected internal void ApplyPatch(CriteriaOperator criteria) {
			PatchCore(criteria, false);
		}
		protected CriteriaOperator PatchCore(CriteriaOperator criteria, bool makeClone) {
			if(ReferenceEquals(criteria, null))
				return null;
			CriteriaOperator criteriaOperator = criteria;
			if(makeClone)
				criteriaOperator = (CriteriaOperator)((ICloneable)criteria).Clone();
			Validate(criteriaOperator);
			return criteriaOperator;
		}
	}
	public class OperatorNameToFieldNamePatcher : OperatorNamePatcher {
		public OperatorNameToFieldNamePatcher(PivotGridFieldCollectionBase fields, bool patchDataFields)
			: base(fields, patchDataFields) {
		}
		protected override string GetPatchName(PivotGridFieldBase field) { return field.DataControllerColumnName; }
		protected override PivotGridFieldBase GetFieldByPropertyName(string propertyName) {
			return Fields.GetFieldByName(propertyName);
		}
	}
	public class OperatorNameToPrefilterColumnNamePatcher : OperatorNamePatcher {
		public OperatorNameToPrefilterColumnNamePatcher(PivotGridFieldCollectionBase fields, bool patchDataFields)
			: base(fields, patchDataFields) {
		}
		protected override string GetPatchName(PivotGridFieldBase field) { return field.PrefilterColumnName; }
		protected override PivotGridFieldBase GetFieldByPropertyName(string propertyName) {
			return Fields.GetFieldByNameOrDataControllerColumnName(propertyName);
		}
	}
	[Serializable]
	public class FilterItem : IConvertible {
		object value;
		string displayText;
		public object Value { get { return value; } set { this.value = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public FilterItem(object value, string displayText) {
			this.value = value;
			this.displayText = displayText;
		}
		public override string ToString() {
			return displayText;
		}
		public override bool Equals(object obj) {
			FilterItem b = obj as FilterItem;
			if(b != null)
				return object.Equals(Value, b.Value) && object.Equals(DisplayText, b.DisplayText);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return Value != null ? Value.GetHashCode() : 0;
		}
		#region IConvertible Members
		TypeCode IConvertible.GetTypeCode() {
			if(Value == null) return TypeCode.Object;
			return DXTypeExtensions.GetTypeCode(Value.GetType());
		}
		bool IConvertible.ToBoolean(IFormatProvider provider) {
			return Convert.ToBoolean(Value);
		}
		byte IConvertible.ToByte(IFormatProvider provider) {
			return Convert.ToByte(Value);
		}
		char IConvertible.ToChar(IFormatProvider provider) {
			return Convert.ToChar(Value);
		}
		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			return Convert.ToDateTime(Value);
		}
		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			return Convert.ToDecimal(Value);
		}
		double IConvertible.ToDouble(IFormatProvider provider) {
			return Convert.ToDouble(Value);
		}
		short IConvertible.ToInt16(IFormatProvider provider) {
			return Convert.ToInt16(Value);
		}
		int IConvertible.ToInt32(IFormatProvider provider) {
			return Convert.ToInt32(Value);
		}
		long IConvertible.ToInt64(IFormatProvider provider) {
			return Convert.ToInt64(Value);
		}
		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			return Convert.ToSByte(Value);
		}
		float IConvertible.ToSingle(IFormatProvider provider) {
			return Convert.ToSingle(Value);
		}
		string IConvertible.ToString(IFormatProvider provider) {
			return Convert.ToString(Value);
		}
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			IConvertible convertibleValue = Value as IConvertible;
			if(convertibleValue != null)
				return convertibleValue.ToType(conversionType, provider);
			return Value;
		}
		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			return Convert.ToUInt16(Value);
		}
		uint IConvertible.ToUInt32(IFormatProvider provider) {
			return Convert.ToUInt32(Value);
		}
		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			return Convert.ToUInt64(Value);
		}
		#endregion
	}
}
