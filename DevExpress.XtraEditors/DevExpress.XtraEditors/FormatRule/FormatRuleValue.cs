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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum FormatCondition {
		None, Equal, NotEqual, Between, NotBetween, Less, Greater,
		GreaterOrEqual, LessOrEqual, Expression
	};
	public class FormatConditionRuleValue : FormatConditionRuleAppearanceBase, IFormatConditionRuleValue {
		FormatCondition condition = FormatCondition.None;
		object value1, value2;
		string expression = "";
		[DefaultValue(FormatCondition.None)]
		[XtraSerializableProperty()]
		public FormatCondition Condition { 
			get { return condition; }
			set {
				if(condition == value) return;
				condition = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleValue;
			if(source == null) return;
			Condition = source.Condition;
			Value1 = source.Value1;
			Value2 = source.Value2;
			Expression = source.Expression;
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleValue();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleValueValue1"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleValue.Value1"),
		XtraSerializableProperty(),
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Value1 { 
			get { return value1; }
			set {
				if(Value1 == value) return;
				value1 = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleValueValue2"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleValue.Value2"),
		XtraSerializableProperty(),
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Value2 {
			get { return value2; }
			set {
				if(Value2 == value) return;
				value2 = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleValueExpression"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleValue.Expression"),
		DefaultValue(""),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor)), XtraSerializableProperty()
		]
		public string Expression { 
			get { return expression; }
			set {
				if(value == null) value = "";
				if(Expression == value) return;
				expression = value;
				ResetEvaluator();
				if(Condition == FormatCondition.Expression) {
					OnModified(FormatConditionRuleChangeType.UI);
				}
			}
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			if(Condition == FormatCondition.Expression) return IsFitExpression(valueProvider);
			object val;
			if(!CheckQueryValue(valueProvider, out val)) return false;
			object value1 = Value1, value2 = Value2;
			if(val != null) {
				Type valType = val.GetType();
				if(value1 != null && !valType.Equals(value1.GetType())) {
					value1 = Convert.ChangeType(value1, valType);
				}
			}
			int res1 = CompareValues(val, value1);
			switch(Condition) {
				case FormatCondition.Equal: return res1 == 0;
				case FormatCondition.NotEqual: return res1 != 0;
				case FormatCondition.Less: return res1 < 0;
				case FormatCondition.Greater: return res1 > 0;
				case FormatCondition.GreaterOrEqual: return res1 >= 0;
				case FormatCondition.LessOrEqual: return res1 <= 0;
				case FormatCondition.Between:
				case FormatCondition.NotBetween:
					if(val != null) {
						value2 = Convert.ChangeType(value2, val.GetType());
					}
					int res2 = Comparer.Default.Compare(val, value2);
					if(Condition == FormatCondition.Between)
						return res1 > 0 && res2 < 0;
					return res1 <= 0 || res2 >= 0;
			}
			return true;
		}
		protected internal override void ResetDataSourceProperties() {
			base.ResetDataSourceProperties();
			ResetEvaluator();
		}
		void ResetEvaluator() {
			this.evaluator = null;
			this.evaluatorCreated = false;
		}
		ExpressionEvaluator evaluator;
		bool evaluatorCreated;
		protected virtual bool IsFitExpression(IFormatConditionRuleValueProvider valueProvider) {
			if(!evaluatorCreated) {
				evaluator = CreateEvaluator(out evaluatorCreated);
			}
			return FormatConditionRuleExpression.IsEvaluatorFit(this, evaluator, valueProvider);
		}
		protected virtual ExpressionEvaluator CreateEvaluator(out bool readyToCreate) {
			return Owner.CreateExpressionEvaluator(CriteriaOperator.TryParse(Expression), out readyToCreate);
		}
		#region IFormatConditionRuleValue
		XlDifferentialFormatting IFormatConditionRuleValue.Appearance {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		}  
		FormatConditions IFormatConditionRuleValue.Condition {
			get { return (FormatConditions)(int)Condition; }
		}   
		string IFormatConditionRuleValue.Expression {
			get {
				if(!string.IsNullOrEmpty(Expression)) return Expression;
				else return string.Empty;
			}
		}
		object IFormatConditionRuleValue.Value1 {
			get { return Value1; }
		}
		object IFormatConditionRuleValue.Value2 {
			get { return Value2; }
		}
		#endregion
	}
	public class FormatConditionRuleExpression : FormatConditionRuleAppearanceBase, IFormatConditionRuleExpression {
		string expression = "";
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleExpression();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleExpression;
			if(source == null) return;
			Expression = source.Expression;
		}
		protected internal override void ResetDataSourceProperties() {
			base.ResetDataSourceProperties();
			ResetEvaluator();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleExpressionExpression"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleExpression.Expression"),
		DefaultValue(""),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor)), XtraSerializableProperty()
		]
		public string Expression {
			get { return expression; }
			set {
				if(value == null) value = "";
				if(Expression == value) return;
				expression = value;
				ResetEvaluator();
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		void ResetEvaluator() {
			this.evaluator = null;
			this.evaluatorCreated = false;
		}
		ExpressionEvaluator evaluator;
		bool evaluatorCreated;
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			if(!evaluatorCreated) {
				evaluator = CreateEvaluator(out evaluatorCreated);
			}
			return IsEvaluatorFit(this, evaluator, valueProvider);
		}
		protected virtual ExpressionEvaluator CreateEvaluator(out bool readyToCreate) {
			return Owner.CreateExpressionEvaluator(CriteriaOperator.TryParse(Expression), out readyToCreate);
		}
		internal static bool IsEvaluatorFit(FormatConditionRuleBase rule, ExpressionEvaluator evaluator, IFormatConditionRuleValueProvider valueProvider) {
			if(evaluator == null) return false;
			try {
				object res = evaluator.Evaluate(valueProvider.GetValueExpression(rule));
				if(res is bool) return (bool)res;
				return Convert.ToBoolean(res);
			}
			catch {
			}
			return false;
		}
		#region IFormatConditionRuleExpression
		XlDifferentialFormatting IFormatConditionRuleExpression.Appearance {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		} 
		string IFormatConditionRuleExpression.Expression {
			get {
				if(!string.IsNullOrEmpty(Expression)) return Expression;
				else return string.Empty;
			}
		}
		#endregion
	}
	public class FormatConditionRuleUniqueDuplicate : FormatConditionRuleAppearanceBase, IFormatConditionRuleUniqueDuplicate {
		FormatConditionUniqueDuplicateType formatType = FormatConditionUniqueDuplicateType.Duplicate;
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleUniqueDuplicate();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleUniqueDuplicate;
			if(source == null) return;
			this.FormatType = source.FormatType;
		}
		[DefaultValue(FormatConditionUniqueDuplicateType.Duplicate), XtraSerializableProperty]
		public FormatConditionUniqueDuplicateType FormatType {
			get { return formatType; }
			set {
				if(FormatType == value) return;
				formatType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			FormatRuleValueQueryKind kind = FormatType == FormatConditionUniqueDuplicateType.Duplicate ? FormatRuleValueQueryKind.Duplicate : FormatRuleValueQueryKind.Unique;
			FormatConditionRuleState res = new FormatConditionRuleState(this, kind);
			return res;
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			var state = valueProvider.GetState(this);
			Hashtable items = state.GetValue<Hashtable>("Hash");
			bool hasNull = state.GetValue<bool>("HashHasNull");
			if(items == null) {
				items = PrepareItems(valueProvider.GetQueryValue(this, state.QueryKind) as IList, out hasNull);
				state.SetValue("Hash", items);
				state.SetValue("HashHasNull", hasNull);
			}
			object actualValue;
			if(!CheckQueryValue(valueProvider, out actualValue)) return hasNull;
			return items.ContainsKey(actualValue);
		}
		Hashtable PrepareItems(IList list, out bool hasNull) {
			hasNull = false;
			Hashtable res = new Hashtable();
			if(list == null) return res;
			try {
				foreach(var l in list) {
					if(CheckNullValue(l) == null) {
						hasNull = true;
						continue;
					}
					res[l] = true;
				}
			}
			catch { }
			return res;
		}
		#region IFormatConditionRuleUniqueDuplicate
		bool IFormatConditionRuleUniqueDuplicate.Duplicate {
			get { return FormatType == FormatConditionUniqueDuplicateType.Duplicate; }
		}					  
		XlDifferentialFormatting IFormatConditionRuleUniqueDuplicate.Formatting {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		} 
		bool IFormatConditionRuleUniqueDuplicate.Unique {
			get { return FormatType == FormatConditionUniqueDuplicateType.Unique; }
		}						 
		#endregion                                                                  
	}
	public class FormatConditionRuleContains : FormatConditionRuleAppearanceBase, IXtraSerializable, IFormatConditionRuleContains {
		IList values;
		Hashtable valuesHash;
		bool? hasNullValue = null;
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleContains();
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleContains;
			if(source == null) return;
			Values = source.Values;
		}
		[XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection)]
		[Editor("DevExpress.XtraEditors.Design.FormatRuleContainValuesCollectionEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public IList Values {
			get { return values; }
			set {
				if(Values == value) return;
				values = value;
				OnValuesChanged();
			}
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			EnsureHash();
			var value = valueProvider.GetValue(this);
			if(IsNull(value)) {
				return hasNullValue.HasValue && hasNullValue.Value;
			}
			return (valuesHash != null && valuesHash.ContainsKey(value));
		}
		bool IsNull(object value) {
			return (value == null || object.ReferenceEquals(value, DBNull.Value));
		}
		void EnsureHash() {
			if(valuesHash == null) {
				if(Values == null || Values.Count == 0) return;
				valuesHash = new Hashtable();
				try {
					foreach(object item in Values) {
						if(IsNull(item)) {
							hasNullValue = true;
							continue;
						}
						valuesHash[item] = true;
					}
				}
				catch { }
			}
		}
		void OnValuesChanged() {
			this.hasNullValue = null;
			this.valuesHash = null;
			OnModified(FormatConditionRuleChangeType.UI);
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			Values = new ArrayList();
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		#endregion
		#region IFormatConditionRuleContains
		XlDifferentialFormatting IFormatConditionRuleContains.Appearance {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		}	
		IList IFormatConditionRuleContains.Values {
			get { return Values; }
		}
		#endregion
	}
	public class FormatConditionRuleDateOccuring : FormatConditionRuleAppearanceBase, IFormatConditionRuleDateOccuring {
		FilterDateType dateType = FilterDateType.Yesterday;
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleDateOccuring();
		}
		[DefaultValue(FilterDateType.Yesterday), XtraSerializableProperty]
		[System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public FilterDateType DateType {
			get { return dateType;}
			set {
				value = (~(FilterDateType.SpecificDate | FilterDateType.User)) & value;
				if(DateType == value) return;
				dateType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		public void ResetCurrentDate() {
			OnModified(FormatConditionRuleChangeType.Data);
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleDateOccuring;
			if(source == null) return;
			DateType = source.DateType;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			var res = new FormatConditionRuleState(this, FormatRuleValueQueryKind.None);
			object criteriaOP = null;
			if(DateType != FilterDateType.None) {
				var criteria = FilterDateTypeHelper.ToCriteria(new DevExpress.Data.Filtering.OperandProperty("this"), DateType);
				ExpressionEvaluator ev = new ExpressionEvaluator(new PropertyDescriptorCollection(null), criteria);
				criteriaOP = ev;
			}
			res.SetValue(FormatConditionRuleState.DefaultValueName, criteriaOP);
			return res;
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			object val;
			if(!CheckQueryValue(valueProvider, out val)) return false;
			if(!(val is DateTime)) return false;
			DateTime d = (DateTime)val;
			var current = valueProvider.GetQueryValue(this) as ExpressionEvaluator;
			if(current == null) return false;
			if(current.Fit(d)) return true;
			return false;
		}
		#region IFormatConditionRuleDateOccuring
		XlDifferentialFormatting IFormatConditionRuleDateOccuring.Formatting {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		}
		XlCondFmtTimePeriod IFormatConditionRuleDateOccuring.DateType {
			get { return ExportHelper.GetDateOccuringType(DateType); }
		}
		#endregion
	}
}
