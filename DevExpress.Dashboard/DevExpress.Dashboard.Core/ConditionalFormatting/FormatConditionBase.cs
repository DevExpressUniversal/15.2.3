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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
using System;
namespace DevExpress.DashboardCommon {
	public abstract class FormatConditionBase : IAggregationInfo, IFormatCondition, IFormatStyleSettingsOwner, IMinMaxInfo {
		protected static IEnumerable<SummaryItemTypeEx> EmptyAggregationTypes = new SummaryItemTypeEx[] { };
		readonly Dictionary<SummaryItemTypeEx, object> aggregations;
		IFormatConditionOwner owner;
		int lockUpdate;
		internal IFormatConditionOwner Owner { 
			get { return owner; }
			set { this.owner = value; }
		}
		protected abstract IEnumerable<StyleSettingsBase> Styles { get; }
		decimal IMinMaxInfo.AbsoluteMinimum { get { return Convert.ToDecimal(ValueManager.ConvertToNumber(GetAggregationValue(SummaryItemTypeEx.Min))); } }
		decimal IMinMaxInfo.AbsoluteMaximum { get { return Convert.ToDecimal(ValueManager.ConvertToNumber(GetAggregationValue(SummaryItemTypeEx.Max))); } }
		decimal IMinMaxInfo.Minimum { get { return Min; } }
		decimal IMinMaxInfo.Maximum { get { return Max; } }
		DashboardFormatConditionValueType IMinMaxInfo.MinimumType { get { return MinType; } }
		DashboardFormatConditionValueType IMinMaxInfo.MaximumType { get { return MaxType; } }
		protected virtual decimal Min { get { return 0; } }
		protected virtual decimal Max { get { return 0; } }
		protected virtual DashboardFormatConditionValueType MinType { get { return DashboardFormatConditionValueType.Automatic; } }
		protected virtual DashboardFormatConditionValueType MaxType { get { return DashboardFormatConditionValueType.Automatic; } }
		protected FormatConditionBase()
			: this(null) {
		}
		protected FormatConditionBase(IFormatConditionOwner owner) {
			this.aggregations = new Dictionary<SummaryItemTypeEx, object>();
			this.owner = owner;
			this.lockUpdate = 0;
		}
		internal bool IsFit(IFormatConditionValueProvider valueProvider) {
			if(!IsValid) return false;
			try {
				return IsFitCore(valueProvider);
			} catch {
				return false;
			}
		}
		internal IStyleSettings CalcStyleSetting(IFormatConditionValueProvider valueProvider) {
			if(!IsValid || valueProvider == null)
				return null;
			try {
				return CalcStyleSettingCore(valueProvider);
			} catch {
				return null;
			}
		}
		internal decimal? CalcNormalizedValue(IFormatConditionValueProvider valueProvider) {
			if(!IsValid || valueProvider == null)
				return null;
			try {
				return CalcNormalizedValueCore(valueProvider);
			}
			catch {
				return null;
			}
		}
		internal decimal? CalcZeroPosition(IFormatConditionValueProvider valueProvider) {
			if(!IsValid || valueProvider == null)
				return null;
			try {
				return CalcZeroPositionCore(valueProvider);
			}
			catch {
				return null;
			}
		}
		[Browsable(false)]
		public virtual bool IsValid { get { return true; } }
		public void Assign(FormatConditionBase obj) {
			BeginUpdate();
			try {
				AssignCore(obj);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual bool IsBarAggregationsRequired { get { return false; } }
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				OnChanged();
			}
		}
		protected abstract FormatConditionBase CreateInstance();
		protected internal virtual void SaveToXml(XElement element) {
		}
		protected internal virtual void LoadFromXml(XElement element) {
		}
		protected abstract bool IsFitCore(IFormatConditionValueProvider valueProvider);
		protected abstract IStyleSettings CalcStyleSettingCore(IFormatConditionValueProvider valueProvider);
		protected abstract void AssignCore(FormatConditionBase obj);
		protected virtual decimal GetAggregationArgument() {
			return 0;
		}
		protected virtual decimal? CalcNormalizedValueCore(IFormatConditionValueProvider valueProvider) {
			return null;
		}
		protected virtual decimal? CalcZeroPositionCore(IFormatConditionValueProvider valueProvider) {
			return null;
		}
		protected virtual IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			return EmptyAggregationTypes;
		}
		protected object GetAggregationValue() {
			return GetAggregationValue(SummaryItemTypeEx.None);
		}
		protected object GetAggregationValue(SummaryItemTypeEx type) {
			if(type == SummaryItemTypeEx.None) {
				if(aggregations.Count == 1)
					return aggregations.Values.First();
				else
					throw new InvalidEnumArgumentException("AggregationInfo has incorrect value", (int)type, typeof(SummaryItemTypeEx));
			}
			object value;
			return aggregations.TryGetValue(type, out value) ? value : null;
		}
		protected internal virtual void OnChanged() {
			if(Owner != null)
				Owner.OnChanged();
		}
		protected bool CheckValue(object value) {
			return !ValueManager.IsNull(value);
		}
		protected internal FormatConditionBase Clone() {
			var res = CreateInstance();
			res.Assign(this);
			return res;
		}
		internal virtual ConditionModel CreateModel() {
			return null;
		}
		#region IAggregationInfo Members
		decimal IAggregationInfo.Argument {
			get { return GetAggregationArgument(); }
		}
		IEnumerable<SummaryItemTypeEx> IAggregationInfo.Types {
			get { return GetAggregationTypes(); }
		}
		object IAggregationInfo.this[SummaryItemTypeEx type] {
			get { return this.GetAggregationValue(type); }
			set { aggregations[type] = value; }
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
		#region IFormatCondition Members
		IEnumerable<IStyleSettings> IFormatCondition.Styles {
			get { return Styles; }
		}
		#endregion
		#region IFormatStyleSettingsOwner Members
		void IFormatStyleSettingsOwner.OnChanged() {
			OnChanged();
		}
		#endregion
	}
}
