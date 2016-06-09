#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class DashboardItemFormatRule : IFormatConditionOwner, IDataItemRepositoryProvider, IXmlSerializableElement, ISupportPrefix, INameContainer {
		const string XmlFormatRule = "FormatRule";
		const string XmlName = "Name";
		const string XmlEnabled = "Enabled";
		const string XmlStopIfTrue = "StopIfTrue";
		const string DefaultName = "";
		const bool DefaultEnabled = true;
		const bool DefaultStopIfTrue = false;
		const string FormatRulePrefix = "FormatRule";
		string name = DefaultName;
		bool stopIfTrue = false;
		bool enabled = true;
		int lockUpdate;
		IFormatRulesContext context;
		FormatConditionBase condition;
		DashboardFormatConditionValueProvider valueProvider;
		event EventHandler<NameChangingEventArgs> nameChanging;
		[
		DefaultValue(DefaultName),
		Localizable(false)
		]
		public string Name {
			get { return name; }
			set {
				if(Name != value) {
					if(value == null) value = DefaultName;
					if(nameChanging != null)
						nameChanging(this, new NameChangingEventArgs(value));
					string old = name;
					this.name = value;
					OnNameChanged(old, name);
				}
			}
		}		
		[
		DefaultValue(DefaultEnabled)
		]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				OnChanged();
			}
		}
		[
		DefaultValue(DefaultStopIfTrue)
		]
		internal bool StopIfTrue {
			get { return stopIfTrue; }
			set {
				if(StopIfTrue == value) return;
				stopIfTrue = value;
				OnChanged();
			}
		}		
		[
		DefaultValue(null),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public FormatConditionBase Condition {
			get { return condition; }
			set {
				if(Condition == value) return;
				if(value != null && value.Owner != null && value.Owner != this) {
					throw new InvalidOperationException("Condition already has an Owner.");
				}
				if(HasCondition) Condition.Owner = null;
				condition = value;
				if(HasCondition) Condition.Owner = this;
				OnChanged();
			}
		}
		internal abstract IFormatRuleLevel LevelCore { get; }
		protected internal IFormatRulesContext Context { 
			get { return context; }
			internal set {
				if(context != value) {
					IFormatRulesContext oldContext = context;
					context = value; 
					OnContextChanged(oldContext, context);
				}
			}
		}
		internal DashboardFormatConditionValueProvider ValueProvider {
			get {
				if(valueProvider == null) {
					valueProvider = new DashboardFormatConditionValueProvider();
				}
				return valueProvider;
			}
		}
		internal bool IsAggregationsRequired {
			get {
				return HasCondition && ((IAggregationInfo)Condition).Types.Count<DevExpress.Data.SummaryItemTypeEx>() > 0;
			}
		}
		internal bool IsBarAggregationsRequired {
			get { return IsValid && Condition.IsBarAggregationsRequired; }
		}
		[Browsable(false)]
		public virtual bool IsValid {
			get { return HasCondition && Condition.IsValid; }
		}
		internal bool Checked {
			get { return IsValid && Enabled; }
		}
		bool HasCondition { get { return Condition != null; } }
		bool IsLocked { get { return lockUpdate > 0; } }
		public void Assign(DashboardItemFormatRule source) {
			BeginUpdate();
			try {
				AssignCore(source);
			} finally {
				EndUpdate();
			}
		}
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				OnChanged();
			}
		}
		internal void CancelUpdate() {
			--lockUpdate;
		}
		public override string ToString() {
			return string.Format("{0}: {1}", Name, Condition == null ? "" : Condition.ToString());
		}		
		protected virtual void AssignCore(DashboardItemFormatRule source) {
			this.Name = source.Name;
			this.StopIfTrue = source.StopIfTrue;
			this.Enabled = source.Enabled;
			this.Condition = source.Condition == null ? null : source.Condition.Clone();
		}
		internal bool IsFit() {
			return ValueProvider != null && IsValid && Condition.IsFit(ValueProvider); 
		}
		internal IStyleSettings CalcStyleSetting() {
			return IsValid ? Condition.CalcStyleSetting(ValueProvider) : null;
		}
		internal decimal? CalcNormalizedValue() {
			return IsValid ? Condition.CalcNormalizedValue(ValueProvider) : null;
		}
		internal decimal? CalcZeroPosition() {
			return IsValid ? Condition.CalcZeroPosition(ValueProvider) : null;
		}
		protected internal virtual DashboardItemFormatRule Clone() {
			var res = CreateInstance();
			res.Assign(this);
			return res;
		}
		protected internal abstract DashboardItemFormatRule CreateInstance();
		protected internal abstract FormatRuleModelBase CreateModelInternal();
		protected virtual void OnContextChanged(IFormatRulesContext oldContext, IFormatRulesContext newContext) {
		}
		protected internal virtual void OnEndLoading() {
		}
		internal FormatRuleModelBase CreateModel() {
			FormatRuleModelBase model = CreateModelInternal();
			model.FormatConditionMeasureId = FormatConditionMeasureDescriptorIdManager.GetFormatConditionMeasureDescriptorId(Name);
			if(Condition.IsBarAggregationsRequired) {
				model.NormalizedValueMeasureId = FormatConditionMeasureDescriptorIdManager.GetNormalizedValueMeasureDescriptorId(LevelCore);
				model.ZeroPositionMeasureId = FormatConditionMeasureDescriptorIdManager.GetZeroPositionMeasureDescriptorId(LevelCore);
			}
			model.ConditionModel = Condition.CreateModel();
			return model;
		}
		protected internal virtual void SaveToXml(XElement element) {
			XmlHelper.Save(element, XmlName, Name, DefaultName);
			XmlHelper.Save(element, XmlEnabled, Enabled, DefaultEnabled);
			if(HasCondition) {
				ConditionalFormattingSerializer.Save(Condition, element);
			}
		}
		protected internal virtual void LoadFromXml(XElement element) {
			XmlHelper.Load<string>(element, XmlName, x => name = x);
			XmlHelper.Load<bool>(element, XmlEnabled, x => enabled = x);
			Condition = ConditionalFormattingSerializer.LoadFirst<FormatConditionBase>(element);
		}
		protected internal void OnChanged() {
			if(!IsLocked && Context != null)
				Context.OnChanged(null);
		}
		void OnNameChanged(string oldName, string newName) {
		}
		#region IFormatConditionRuleOwner Members
		void IFormatConditionOwner.OnChanged() {
			OnChanged();
		}
		#endregion
		#region IDataItemRepositoryProvider Members
		DataItemRepository IDataItemRepositoryProvider.DataItemRepository {
			get {
				if(Context != null)
					return Context.DataItemRepositoryProvider != null ? Context.DataItemRepositoryProvider.DataItemRepository : null;
				return null;
			}
		}
		#endregion
		#region IXmlSerializableElement Members
		void IXmlSerializableElement.SaveToXml(XElement element) {
			SaveToXml(element);
		}
		void IXmlSerializableElement.LoadFromXml(XElement element) {
			LoadFromXml(element);
		}
		#endregion
		#region INameContainer Members
		event EventHandler<NameChangingEventArgs> INameContainer.NameChanging {
			add { nameChanging += value; } 
			remove { nameChanging -= value; }
		}
		#endregion
		#region INamedItem Members
		string INamedItem.Name {
			get { return Name; }
			set { Name = value; }
		}
		#endregion
		#region ISupportPrefix Members
		string ISupportPrefix.Prefix {
			get { return FormatRulePrefix ; }
		}
		#endregion
	}
}
