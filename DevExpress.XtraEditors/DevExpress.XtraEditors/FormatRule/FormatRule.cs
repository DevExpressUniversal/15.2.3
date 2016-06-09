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
using System.Linq;
using DevExpress.XtraEditors.Helpers;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraEditors {
	public abstract class FormatRuleBase : IFormatConditionRuleOwner {
		string name = string.Empty;
		object tag;
		IFormatRuleCollection collection;
		FormatConditionRuleBase rule;
		bool stopIfTrue = false, enabled = true;
		public FormatRuleBase() { }
		protected internal IFormatRuleCollection Collection { get { return collection; } }
		[XtraSerializableProperty, DefaultValue("")]
		public string Name {
			get { return name; }
			set {
				if(value == null) value = "";
				if(Name == value) return;
				string old = name;
				name = value;
				OnNameChanged(old, name);
			}
		}
		[DefaultValue(true), XtraSerializableProperty]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				if(Collection != null) Collection.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.All));
			}
		}
		[DefaultValue(null), XtraSerializableProperty]
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		public virtual void Assign(FormatRuleBase source) {
			this.Name = source.Name;
			this.Tag = source.Tag;
			this.StopIfTrue = source.StopIfTrue;
			this.Enabled = source.Enabled;
			this.Rule = source.Rule == null ? null : source.Rule.Clone();
		}
		public override string ToString() {
			return string.Format("{0}: {1}", Name, Rule == null ? "" : Rule.ToString());
		}
		protected internal virtual FormatRuleBase Clone() {
			var res = CreateInstance();
			res.Assign(this);
			return res;
		}
		protected internal abstract IDataColumnInfo GetColumnInfo();
		protected internal abstract FormatRuleBase CreateInstance();
		protected internal virtual string FieldNameCore { get { return Name; } }
		protected void InvalidateRuleState() {
			if(Rule != null) Rule.InvalidateState();
		}
		void OnNameChanged(string oldName, string newName) {
			if(Collection != null) Collection.OnNameChanged(this, oldName, newName);
		}
		internal void SetNameInternal(string name) { this.name = name; }
		[DefaultValue(false), XtraSerializableProperty]
		public bool StopIfTrue {
			get { return stopIfTrue; }
			set {
				if(StopIfTrue == value) return;
				stopIfTrue = value;
				if(Collection != null) Collection.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.UI));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ColumnFieldName {
			get { return FieldNameCore; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(999)]
		public string RuleType {
			get { return Rule == null ? "" : Rule.GetSerializableTypeName(); }
			set {
				if(string.IsNullOrEmpty(value))
					Rule = null;
				else
					Rule = FormatConditionRuleBase.CreateInstance(value);
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue, 1000)]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatRuleUITypeEditor), typeof(System.Drawing.Design.UITypeEditor)), RefreshProperties(RefreshProperties.All)]
		public FormatConditionRuleBase Rule {
			get { return rule; }
			set {
				if(Rule == value) return;
				if(value != null && value.Owner != this && value.Owner != null) {
					throw new InvalidOperationException("The specified FormatConditionRule object is already assigned to another FormatRule object.");
				}
				if(Rule != null) Rule.SetOwner(null);
				rule = value;
				if(Rule != null) Rule.SetOwner(this);
				if(Collection != null) Collection.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this));
			}
		}
		public T RuleCast<T>() where T : FormatConditionRuleBase { return (T)Rule; }
		[Browsable(false)]
		public virtual bool IsValid {
			get { return Rule != null && Enabled && Rule.IsValid; }
		}
		public bool IsFit(IFormatConditionRuleValueProvider valueProvider) { return IsValid && Rule != null && Rule.IsFit(valueProvider); }
		#region IFormatConditionRuleOwner Members
		void IFormatConditionRuleOwner.OnModified(FormatConditionRuleChangeType changeType) {
			if(Collection != null) Collection.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, changeType));
		}
		bool IFormatConditionRuleOwner.IsLoading { get { return Collection == null || Collection.IsLoading; } }
		ExpressionEvaluator IFormatConditionRuleOwner.CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out bool readyToCreate) {
			return CreateExpressionEvaluator(criteriaOperator, out readyToCreate);
		}
		bool IFormatConditionRuleOwner.GetIsRightToLeft() { return GetIsRightToLeft(); }
		protected virtual bool GetIsRightToLeft() { return false; }
		protected abstract ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out bool readyToCreate);
		#endregion
		internal void SetOwner<T, TColumnType>(FormatRuleCollection<T, TColumnType> owner) where T : FormatRuleBase, new() {
			this.collection = owner;
		}
		UserLookAndFeel IFormatConditionRuleOwner.LookAndFeel { get { return Collection == null ? null : Collection.LookAndFeel; } }
		protected internal virtual void ResetVisualCache() {
			if(Rule != null) Rule.ResetVisualCache();
		}
		protected internal void ResetDataSourceProperties() {
			if(Rule != null) Rule.ResetDataSourceProperties();
		}
	}
	public interface IFormatRuleCollection {
		void OnCollectionChanged(FormatConditionCollectionChangedEventArgs e);
		bool IsLoading { get; }
		void OnNameChanged(FormatRuleBase formatRuleBase, string oldName, string newName);
		UserLookAndFeel LookAndFeel { get; }
		FormatRuleBase GetRule(int i);
		int IndexOfRule(FormatRuleBase rule);
		void AddRule(FormatRuleBase rule);
		int Count { get; }
		void RemoveAt(int index);
		void Clear();
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	public abstract class FormatRuleCollection<T, TColumnType> : DXCollectionBase<T>, IFormatRuleCollection where T : FormatRuleBase, new() {
		bool changed = false;
		Dictionary<string, T> namesHash = null;
		public event EventHandler<FormatConditionCollectionChangedEventArgs> CollectionChanged;
		public T this[string name] {
			get {
				EnsureNamesHash();
				T res;
				if(!namesHash.TryGetValue(name, out res)) return null;
				return res;
			}
		}
		public FormatRuleBase GetRule(int i) { return this[i]; }
		public int IndexOfRule(FormatRuleBase rule) { return this.IndexOf(rule as T); }
		public void AddRule(FormatRuleBase rule) { this.Add(rule as T); }
		void EnsureNamesHash() {
			if(namesHash == null) namesHash = new Dictionary<string, T>();
			if(namesHash.Count != Count) {
				namesHash.Clear();
				foreach(var t in this) namesHash.Add(t.Name, t);
			}
		}
		public override string ToString() {
			if(Count == 0) return "";
			return string.Format("(count={0})", Count);
		}
		protected internal virtual T CreateItemInstance() { return new T(); }
		protected internal T AddInstance() {
			var format = CreateItemInstance();
			Add(format);
			return format;
		}
		protected void ResetDataSourceProperties() {
			foreach(var rule in this) rule.ResetDataSourceProperties();
		}
		protected internal virtual void Assign(FormatRuleCollection<T, TColumnType> source) {
			BeginUpdate();
			try {
				Clear();
				foreach(var format in source) {
					var local = format.CreateInstance();
					Add(local as T);
					local.Assign(format);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public T Add(TColumnType column, FormatConditionRuleBase rule) {
			T format = CreateItemInstance();
			format.Rule = rule;
			AssignColumn(format, column);
			Add(format);
			return format;
		}
		public T AddIconSetRule(TColumnType column, FormatConditionIconSet iconSet) {
			T format = CreateAndAssign(column);
			format.Rule = new FormatConditionRuleIconSet() { IconSet = iconSet };
			Add(format);
			return format;
		}
		public T AddUniqueRule(TColumnType column, AppearanceDefault appearanceDefault) {
			return AddUniqueDuplicateRule(column, appearanceDefault, FormatConditionUniqueDuplicateType.Unique);
		}
		public T AddDuplicateRule(TColumnType column, AppearanceDefault appearanceDefault) {
			return AddUniqueDuplicateRule(column, appearanceDefault, FormatConditionUniqueDuplicateType.Duplicate);
		}
		public T AddTopRule(TColumnType column, AppearanceDefault appearanceDefault, decimal rank) {
			return AddTopBottomRule(column, appearanceDefault, FormatConditionTopBottomType.Top, rank);
		}
		public T AddBottomRule(TColumnType column, AppearanceDefault appearanceDefault, decimal rank) {
			return AddTopBottomRule(column, appearanceDefault, FormatConditionTopBottomType.Bottom, rank);
		}
		public T AddDataBar(TColumnType column) {
			T format = CreateAndAssign(column);
			var rule = new FormatConditionRuleDataBar();
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T Add2ColorScale(TColumnType column, Color minColor, Color maxColor, FormatConditionValueType minType =  FormatConditionValueType.Automatic,
			FormatConditionValueType maxType = FormatConditionValueType.Automatic, decimal minValue = 0, decimal maxValue = 0) {
			T format = CreateAndAssign(column);
			FormatConditionRule2ColorScale rule = new FormatConditionRule2ColorScale() { Minimum = minValue, Maximum = maxValue };
			if(minColor != Color.Empty) rule.MinimumColor = minColor;
			if(maxColor != Color.Empty) rule.MaximumColor = maxColor;
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T Add3ColorScale(TColumnType column, Color minColor, Color midColor, Color maxColor, 
			FormatConditionValueType minType = FormatConditionValueType.Automatic,
			FormatConditionValueType midType = FormatConditionValueType.Automatic,
			FormatConditionValueType maxType = FormatConditionValueType.Automatic, decimal minValue = 0, decimal midValue = 0, decimal maxValue = 0) {
			T format = CreateAndAssign(column);
			var rule = new FormatConditionRule3ColorScale() { Minimum = minValue, Maximum = maxValue, Middle = midValue };
			if(minColor != Color.Empty) rule.MinimumColor = minColor;
			if(maxColor != Color.Empty) rule.MaximumColor = maxColor;
			if(midColor != Color.Empty) rule.MiddleColor = midColor;
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T AddUniqueDuplicateRule(TColumnType column, AppearanceDefault appearanceDefault, FormatConditionUniqueDuplicateType formatType) {
			T format = CreateAndAssign(column);
			var rule = new FormatConditionRuleUniqueDuplicate() { FormatType = formatType};
			AssignAppearance(rule, appearanceDefault);
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T AddTopBottomRule(TColumnType column, AppearanceDefault appearanceDefault, FormatConditionTopBottomType topBottom, decimal rank) {
			T format = CreateAndAssign(column);
			FormatConditionValueType rankType = FormatConditionValueType.Number;
			if(rank > 0 && rank < 1) {
				rankType = FormatConditionValueType.Percent;
				rank *= 100;
			}
			FormatConditionRuleTopBottom rule = new FormatConditionRuleTopBottom() { TopBottom = topBottom, Rank = rank, RankType = rankType };
			AssignAppearance(rule, appearanceDefault);
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T AddAboveAverageRule(TColumnType column, AppearanceDefault appearanceDefault) {
			return AddAboveBelowAverageRule(column, appearanceDefault, FormatConditionAboveBelowType.Above);
		}
		public T AddBelowAverageRule(TColumnType column, AppearanceDefault appearanceDefault) {
			return AddAboveBelowAverageRule(column, appearanceDefault, FormatConditionAboveBelowType.Below);
		}
		public T AddAboveBelowAverageRule(TColumnType column, AppearanceDefault appearanceDefault, FormatConditionAboveBelowType averageType) {
			T format = CreateItemInstance();
			AssignColumn(format, column);
			var rule = new FormatConditionRuleAboveBelowAverage() {  AverageType = averageType };
			AssignAppearance(rule, appearanceDefault);
			format.Rule = rule;
			Add(format);
			return format;
		}
		public T AddValueRule(TColumnType column, AppearanceDefault appearanceDefault, FormatCondition condition, object value1, object value2 = null) {
			FormatConditionRuleValue rule = new FormatConditionRuleValue() { Condition = condition, Value1 = value1, Value2 = value2 };
			return AddAppearanceRule(column, rule, appearanceDefault);
		}
		public T AddExpressionRule(TColumnType column, AppearanceDefault appearanceDefault, string expression) {
			var rule = new FormatConditionRuleExpression() { Expression = expression };
			return AddAppearanceRule(column, rule, appearanceDefault);
		}
		public T AddAppearanceRule(TColumnType column, FormatConditionRuleAppearanceBase rule, AppearanceDefault appearanceDefault) {
			T format = CreateItemInstance();
			format.Rule = rule;
			AssignColumn(format, column);
			AssignAppearance(rule, appearanceDefault);
			Add(format);
			return format;
		}
		[Obsolete("Use AddDateOccurringRule"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public T AddDateOccuringRule(TColumnType column, FilterDateType dateType, AppearanceDefault appearanceDefault) {
			return AddDateOccurringRule(column, dateType, appearanceDefault);
		}
		public T AddDateOccurringRule(TColumnType column, FilterDateType dateType, AppearanceDefault appearanceDefault) {
			return AddAppearanceRule(column, new FormatConditionRuleDateOccuring() { DateType = dateType }, appearanceDefault);
		}
		void AssignAppearance(FormatConditionRuleAppearanceBase rule, AppearanceDefault appearanceDefault) {
			if(appearanceDefault != null) rule.Appearance.Assign(appearanceDefault);
		}
		protected abstract void AssignColumn(T format, TColumnType column);
		protected T CreateAndAssign(TColumnType column) {
			var format = CreateItemInstance();
			AssignColumn(format, column);
			return format;
		}
		public T this[int index] { get { return GetItem(index); } }
		protected override bool OnInsert(int index, T value) {
			if(value.Collection != null) {
				if(value.Collection == this) {
					if(IndexOf(value) > -1) return false;
				}
				else
					throw new InvalidOperationException("FormatCondition already belongs to other collection");
			}
			if(string.IsNullOrEmpty(value.Name)) value.SetNameInternal(GenerateName());
			if(this[value.Name] != null) throw new InvalidOperationException(string.Format("FormatCondition.Name ('{0}') already exists", value.Name));
			if(namesHash != null) namesHash[value.Name] = value;
			return base.OnInsert(index, value);
		}
		protected virtual string GenerateName() {
			for(int c = 0; ; c++) {
				string name = "Format" + c.ToString();
				if(this[name] == null) return name;
			}
		}
		protected override void OnInsertComplete(int index, T value) {
			value.SetOwner<T, TColumnType>(this);
			base.OnInsertComplete(index, value);
			OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Add, value));
		}
		protected override void OnRemoveComplete(int index, T value) {
			value.SetOwner<T, TColumnType>(null);
			base.OnRemoveComplete(index, value);
			if(this[value.Name] != null) namesHash.Remove(value.Name);
			OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Remove, value));
		}
		protected override bool OnClear() {
			foreach(var i in this) i.SetOwner<T, TColumnType>(null);
			return base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Clear, null));
			this.namesHash = null;
		}
		int lockUpdate = 0;
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.EndBatchUpdate, null));
		}
		protected internal virtual void OnCollectionChanged(FormatConditionCollectionChangedEventArgs e) {
			changed = true;
			if(IsLoading) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected bool Changed { get { return changed; } }
		void IFormatRuleCollection.OnCollectionChanged(FormatConditionCollectionChangedEventArgs e) { OnCollectionChanged(e); }
		void IFormatRuleCollection.OnNameChanged(FormatRuleBase format, string oldName, string newName) {
			if(namesHash != null && namesHash.ContainsKey(oldName)) {
				namesHash.Remove(oldName);
				namesHash[newName] = (T)format;
			}
		}
		UserLookAndFeel IFormatRuleCollection.LookAndFeel { get { return ElementsLookAndFeel; } }
		protected abstract UserLookAndFeel ElementsLookAndFeel { get; }
		[Browsable(false)]
		public virtual bool IsLoading { get { return lockUpdate != 0; } }
		public bool HasValidRules {
			get {
				if(Count == 0) return false;
				for(int n = 0; n < Count; n++) {
					if(GetItem(n).IsValid) return true;
				}
				return false;
			}
		}
		protected internal virtual bool TryUpdateStateValues(DataController controller, FormatRuleSummaryInfoCollection summaryInfo) {
			if(CheckAllValuesReady()) return true;
			if(!controller.IsReady || controller.IsUpdateLocked) return false;
			var summaryItems = FormatRuleSummaryInfoCollection.GetRuleSummaryItems(controller);
			foreach(var f in this) {
				summaryInfo.UpdateValues(summaryItems, f.FieldNameCore, GetRuleState(f));
			}
			return CheckAllValuesReady();
		}
		protected internal virtual void ResetValuesReady() {
			foreach(var f in this) {
				var state = GetRuleState(f);
				if(state != null)  state.ValuesReady = false;
			}
		}
		protected internal virtual bool CheckAllValuesReady() {
			foreach(var f in this) {
				var state = GetRuleState(f);
				if(state != null && !state.ValuesReady) return false;
			}
			return true;
		}
		protected abstract FormatConditionRuleState GetRuleState(FormatRuleBase format);
		protected internal virtual FormatRuleSummaryInfoCollection CreateSummaryInfo() {
			Dictionary<FormatRuleSummaryInfo, bool> list = new Dictionary<FormatRuleSummaryInfo, bool>();
			foreach(var format in this) {
				if(!format.IsValid) continue;
				var ruleState = GetRuleState(format);
				if(ruleState.QueryKind == FormatRuleValueQueryKind.None) continue;
				FormatRuleSummaryInfo[] summaryInfos = Create(format, ruleState);
				foreach(var summaryInfo in summaryInfos) {
					AddInfo(list, summaryInfo);
				}
			}
			return new FormatRuleSummaryInfoCollection(list.Keys.ToArray());
		}
		FormatRuleSummaryInfo[] Create(FormatRuleBase format, FormatConditionRuleState ruleState) {
			FormatRuleSummaryInfo[] res;
			var summaryTypes = FormatRuleSummaryInfoCollection.SummaryTypeFromQueryKind(ruleState.QueryKind);
			res = new FormatRuleSummaryInfo[summaryTypes.Length];
			int len = 0;
			foreach(var summaryType in summaryTypes) {
				res[len ++] = new FormatRuleSummaryInfo() { Column = format.FieldNameCore, SummaryArgument = ruleState.CountPercent, SummaryType = summaryType };
			}
			return res;
		}
		protected internal virtual void UpdateRuleStateValues(DataController controller) {
			var summaryItems = FormatRuleSummaryInfoCollection.GetRuleSummaryItems(controller);
			if(summaryItems.Count == 0) return;
			foreach(var format in this) {
				UpdateRuleStateValues(summaryItems, format);
			}
		}
		protected virtual void UpdateRuleStateValues(List<SummaryItem> summaryItems, FormatRuleBase format) {
			var ruleState = GetRuleState(format);
			ruleState.ValuesReady = true;
			var infos = Create(format, ruleState);
			if(infos.Length == 0) return;
			int foundValues = 0;
			foreach(var info in infos) {
				SummaryItem item = summaryItems.FirstOrDefault(q => (q.Tag as FormatRuleSummaryInfo).Equals(info));
				if(item != null) {
					ruleState.SetValue(FormatRuleSummaryInfoCollection.QueryKindFromSummaryType(item.SummaryTypeEx).ToString(), item.SummaryValue);
					foundValues++;
				}
			}
			ruleState.ValuesReady = foundValues == infos.Length;
		}
		void AddInfo(Dictionary<FormatRuleSummaryInfo, bool> list, FormatRuleSummaryInfo info) {
			if(list.ContainsKey(info)) return;
			list[info] = true;
		}
		bool CheckResetIf(ref FormatRuleValueQueryKind kind, FormatRuleValueQueryKind checkKind) {
			if((kind & checkKind) == 0) return false;
			kind &= ~checkKind;
			return true;
		}
		protected virtual void ResetVisualCache() {
			foreach(FormatRuleBase format in this) format.ResetVisualCache();
		}
	}
	public class FormatRuleSummaryInfoCollection : DXCollectionBase<FormatRuleSummaryInfo> {
		public FormatRuleSummaryInfoCollection() { }
		public FormatRuleSummaryInfoCollection(FormatRuleSummaryInfo[] collection) {
			AddRange(collection);
		}
		public FormatRuleSummaryInfo[] GetInfo(string columnName, FormatConditionRuleState ruleState) {
			if(ruleState.QueryKind == FormatRuleValueQueryKind.None) return new FormatRuleSummaryInfo[0];
			List<FormatRuleSummaryInfo> list = new List<FormatRuleSummaryInfo>();
			foreach(var summaryType in FormatRuleSummaryInfoCollection.SummaryTypeFromQueryKind(ruleState.QueryKind)) {
				var formatInfo = Find(columnName, summaryType, ruleState.CountPercent);
				if(formatInfo != null) list.Add(formatInfo);
			}
			return list.ToArray();
		}
		public FormatRuleSummaryInfo Find(string columnName, SummaryItemTypeEx summaryType, decimal summaryArgument = 0) {
			if(Count == 0) return null;
			for(int n = 0; n < Count; n++) {
				var info = this[n];
				if(info.SummaryType == summaryType && info.Column == columnName && info.SummaryArgument == summaryArgument) return info;
			}
			return null;
		}
		public FormatRuleSummaryInfo this[int index] { get { return GetItem(index); } }
		public static FormatRuleValueQueryKind QueryKindFromSummaryType(SummaryItemTypeEx summaryType) {
			switch(summaryType) {
				case SummaryItemTypeEx.Min: return FormatRuleValueQueryKind.Minimum;
				case SummaryItemTypeEx.Average: return FormatRuleValueQueryKind.Average;
				case SummaryItemTypeEx.Bottom: return FormatRuleValueQueryKind.Bottom;
				case SummaryItemTypeEx.BottomPercent: return FormatRuleValueQueryKind.BottomPercent;
				case SummaryItemTypeEx.Max: return FormatRuleValueQueryKind.Maximum;
				case SummaryItemTypeEx.Top: return FormatRuleValueQueryKind.Top;
				case SummaryItemTypeEx.TopPercent: return FormatRuleValueQueryKind.TopPercent;
				case SummaryItemTypeEx.Unique: return FormatRuleValueQueryKind.Unique;
				case SummaryItemTypeEx.Duplicate: return FormatRuleValueQueryKind.Duplicate;
				case SummaryItemTypeEx.None: return FormatRuleValueQueryKind.None;
				default: throw new ArgumentException("summaryType");
			}
		}
		public static SummaryItemTypeEx[] SummaryTypeFromQueryKind(FormatRuleValueQueryKind kind) {
			if(kind == FormatRuleValueQueryKind.None) return new SummaryItemTypeEx[0];
			SummaryItemTypeEx? res = null;
			List<SummaryItemTypeEx> resArray = null;
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Maximum)) Update(ref res, SummaryItemTypeEx.Max, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Minimum)) Update(ref res, SummaryItemTypeEx.Min, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Count)) Update(ref res, SummaryItemTypeEx.Min, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Average)) Update(ref res, SummaryItemTypeEx.Average, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Top)) Update(ref res, SummaryItemTypeEx.Top, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.TopPercent)) Update(ref res, SummaryItemTypeEx.TopPercent, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Bottom)) Update(ref res, SummaryItemTypeEx.Bottom, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.BottomPercent)) Update(ref res, SummaryItemTypeEx.BottomPercent, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Unique)) Update(ref res, SummaryItemTypeEx.Unique, ref resArray);
			if(CheckResetIf(ref kind, FormatRuleValueQueryKind.Duplicate)) Update(ref res, SummaryItemTypeEx.Duplicate, ref resArray);
			if(resArray == null) {
				return res == null ? new SummaryItemTypeEx[0] : new SummaryItemTypeEx[1] { res.Value };
			}
			return resArray.ToArray();
		}
		static void Update(ref SummaryItemTypeEx? res, SummaryItemTypeEx itemType, ref List<SummaryItemTypeEx> resArray) {
			if(res == null) {
				res = itemType;
				return;
			}
			if(resArray == null) {
				resArray = new List<SummaryItemTypeEx>();
				resArray.Add(res.Value);
			}
			resArray.Add(itemType);
		}
		static bool CheckResetIf(ref FormatRuleValueQueryKind kind, FormatRuleValueQueryKind checkKind) {
			if((kind & checkKind) == 0) return false;
			kind &= ~checkKind;
			return true;
		}
		public bool UpdateValues(List<SummaryItem> ruleSummaryItems, string columnName, FormatConditionRuleState ruleState) {
			if(ruleState.QueryKind == FormatRuleValueQueryKind.None) {
				ruleState.ValuesReady = true;
				return true;
			}
			SummaryItemTypeEx[] stypes = SummaryTypeFromQueryKind(ruleState.QueryKind);
			var infos = GetInfo(columnName, ruleState);
			int vc = 0;
			if(infos.Length > 0) {
				for(int n = 0; n < ruleSummaryItems.Count; n++) {
					var formatRuleSummaryInfo = ruleSummaryItems[n].Tag as FormatRuleSummaryInfo;
					foreach(var info in infos) {
						if(formatRuleSummaryInfo.Equals(info)) {
							ruleState.SetValue(FormatRuleSummaryInfoCollection.QueryKindFromSummaryType(info.SummaryType).ToString(), ruleSummaryItems[n].SummaryValue);
							vc++;
							break;
						}
					}
					if(vc == infos.Length) break;
				}
			}
			ruleState.ValuesReady = (vc == infos.Length);
			return ruleState.ValuesReady;
		}
		public virtual bool Apply(DataController controller, out SummaryItem[] changedItems) {
			bool changed = false;
			controller.TotalSummary.BeginUpdate();
			try {
				changed = ApplyCore(controller, true, out changedItems);
			}
			finally {
				if(changed)
					controller.TotalSummary.EndUpdate();
				else
					controller.TotalSummary.CancelUpdate();
			}
			return changed;
		}
		protected internal static List<SummaryItem> GetRuleSummaryItems(DataController controller) {
			return controller.TotalSummary.Cast<SummaryItem>().Where(q => q.Tag is FormatRuleSummaryInfo).ToList();
		}
		protected virtual bool ApplyCore(DataController controller, bool removeOld, out SummaryItem[] changedItems) {
			changedItems = new SummaryItem[0];
			var items = GetRuleSummaryItems(controller);
			if(Count == 0 && items.Count == 0) return false;
			if(Count == 0) {
				if(removeOld) controller.TotalSummary.RemoveItems(items);
				return true;
			}
			List<SummaryItem> newItems = new List<SummaryItem>();
			foreach(var info in this) {
				var columnInfo = controller.Columns[info.Column];
				if(columnInfo == null) continue;
				var current = items.FirstOrDefault(q => (q.Tag as FormatRuleSummaryInfo).Equals(info));
				if(current != null) {
					items.Remove(current);
					continue;
				}
				newItems.Add(new SummaryItem(columnInfo, info.SummaryType, info.SummaryArgument, true) { Tag = info });
			}
			if(newItems.Count == 0 && (items.Count == 0 || !removeOld)) return false;
			controller.TotalSummary.BeginUpdate();
			try {
				if(newItems.Count > 0) controller.TotalSummary.AddRange(newItems.ToArray());
				if(removeOld && items.Count > 0) controller.TotalSummary.RemoveItems(items);
			}
			finally {
				controller.TotalSummary.EndUpdate();
			}
			changedItems = newItems.ToArray();
			return true;
		}
	}
	public class FormatRuleSummaryInfo {
		public string Column { get; set; }
		public SummaryItemTypeEx SummaryType { get; set; }
		public decimal SummaryArgument { get; set; }
		public override bool Equals(object obj) {
			FormatRuleSummaryInfo info = obj as FormatRuleSummaryInfo;
			if(info == null) return false;
			return info.SummaryType == this.SummaryType && info.Column == this.Column && info.SummaryArgument == this.SummaryArgument;
		}
		public override int GetHashCode() {
			return (int)SummaryType * 1000 + (Column == null ? 0 : Column.GetHashCode()) + (int)(SummaryArgument * 10);
		}
	}
	public class FormatConditionCollectionChangedEventArgs : CollectionChangedEventArgs<FormatRuleBase> {
		FormatConditionRuleChangeType elementChangeType = FormatConditionRuleChangeType.None;
		public FormatConditionCollectionChangedEventArgs(CollectionChangedAction action, FormatRuleBase element, FormatConditionRuleChangeType elementChangeType) : 
			base(action, element) {
				this.elementChangeType = elementChangeType;
		}
		public FormatConditionCollectionChangedEventArgs(CollectionChangedAction action, FormatRuleBase element) : base(action, element) { }
	}
}
