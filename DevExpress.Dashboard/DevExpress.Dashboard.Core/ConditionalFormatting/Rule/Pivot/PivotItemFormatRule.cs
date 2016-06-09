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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class PivotItemFormatRuleCollection : FormatRuleCollection<PivotItemFormatRule> {
		public PivotItemFormatRuleCollection() : base() {
		}
		protected override IFormatRuleCollection CreateInstance() {
			return new PivotItemFormatRuleCollection();
		}
	}
	public class PivotItemFormatRule : CellsItemFormatRule, IFormatRuleIntersectionLevel {
		const string XmlApplyToColumn = "ApplyToColumn";
		const string XmlIntersectionLevelMode = "IntersectionLevelMode";
		const bool DefaultApplyToColumn = false;
		const FormatConditionIntersectionLevelMode DefaultIntersectionLevelMode = FormatConditionIntersectionLevelMode.Auto;
		bool applyToColumn = DefaultApplyToColumn;
		PivotItemFormatRuleLevel level;
		FormatConditionIntersectionLevelMode intersectionLevelMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotItemFormatRuleApplyToColumn"),
#endif
		DefaultValue(DefaultApplyToColumn)
		]
		public bool ApplyToColumn {
			get { return applyToColumn; }
			set {
				if(ApplyToColumn == value) return;
				applyToColumn = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotItemFormatRuleIntersectionLevelMode"),
#endif
		DefaultValue(DefaultIntersectionLevelMode)
		]
		public FormatConditionIntersectionLevelMode IntersectionLevelMode {
			get { return intersectionLevelMode; }
			set {
				if(intersectionLevelMode != value) {
					this.intersectionLevelMode = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotItemFormatRuleLevel"),
#endif
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PivotItemFormatRuleLevel Level { get { return level; } }
		protected internal override bool EnableLevel { 
			get { return ActualIntersectionLevelMode != FormatConditionIntersectionLevelMode.AllLevels; } 
		}
		internal FormatConditionIntersectionLevelMode ActualIntersectionLevelMode {
			get { return (IntersectionLevelMode == FormatConditionIntersectionLevelMode.Auto) ? FormatConditionIntersectionLevelMode.FirstLevel : IntersectionLevelMode; }
		}
		[
		Browsable(false)
		]
		public override bool IsValid {
			get {
				bool applyToDimension = ActualDataItemApplyTo is Dimension;
				if(!base.IsValid || (ApplyToColumn && applyToDimension) || (ApplyToRow && applyToDimension) || (IsAggregationsRequired && IntersectionLevelMode == FormatConditionIntersectionLevelMode.AllLevels))
					return false;
				return true;
			}
		}
		public PivotItemFormatRule() : this(null) {
		}
		public PivotItemFormatRule(DataItem item)
			: this(item, null) {
		}
		public PivotItemFormatRule(DataItem item, DataItem itemApplyTo) : base(item, itemApplyTo) {
			this.level = new PivotItemFormatRuleLevel(Context);
		}
		protected override void AssignCore(DashboardItemFormatRule source) {
			base.AssignCore(source);
			PivotItemFormatRule obj = source as PivotItemFormatRule;
			if(obj != null) {
				ApplyToColumn = obj.ApplyToColumn;
				IntersectionLevelMode = obj.IntersectionLevelMode;
				Level.Assign(obj.Level);
			}
		}
		protected internal override DashboardItemFormatRule CreateInstance() {
			return new PivotItemFormatRule();
		}
		protected internal override FormatRuleModelBase CreateModelInternal() {
			return new PivotFormatRuleModel() {
				ApplyToDataId = LevelCore.ItemApplyTo.ActualId,
				ApplyToRow = ApplyToRow,
				ApplyToColumn = ApplyToColumn
			};
		}
		protected override void OnContextChanged(IFormatRulesContext oldContext, IFormatRulesContext newContext) {
			level.Context = newContext;
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			level.OnEndLoading();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlApplyToColumn, ApplyToColumn, DefaultApplyToColumn);
			XmlHelper.Save(element, XmlIntersectionLevelMode, IntersectionLevelMode, DefaultIntersectionLevelMode);
			ConditionalFormattingSerializer.Save(level, element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.Load<bool>(element, XmlApplyToColumn, x => applyToColumn = x);
			XmlHelper.LoadEnum<FormatConditionIntersectionLevelMode>(element, XmlIntersectionLevelMode, x => intersectionLevelMode = x);
			level = ConditionalFormattingSerializer.LoadFirst<PivotItemFormatRuleLevel>(element);
			level.Context = Context;
		}
		protected override Dimension GetAxisItem(bool isSecond) {
			IList<Dimension> axisItems = Context.GetAxisItems(isSecond);
			Dimension filterAxisPoint = FilterByEvaluatorRequiredDataMembers(axisItems);
			if(filterAxisPoint != null)
				return filterAxisPoint;
			bool hasItems = axisItems.Count != 0;
			switch(ActualIntersectionLevelMode) {
				case FormatConditionIntersectionLevelMode.FirstLevel:
					return hasItems ? axisItems[0] : null;
				case FormatConditionIntersectionLevelMode.LastLevel:
					return hasItems ? axisItems[axisItems.Count - 1] : null;
				case FormatConditionIntersectionLevelMode.AllLevels:
					return null;
				case FormatConditionIntersectionLevelMode.SpecificLevel:
					return isSecond ? level.Row : level.Column;
				case FormatConditionIntersectionLevelMode.Auto:
					throw new ArgumentOutOfRangeException("Pivot: ActualIntersectionLevelMode cannot be 'Auto'");
				default:
					throw new ArgumentOutOfRangeException("Pivot: ActualIntersectionLevelMode is unsupported");
			}
		}
		protected Dimension FilterByEvaluatorRequiredDataMembers(IList<Dimension> axisItems) {
			IEvaluatorRequired evaluatorRequired = Condition as IEvaluatorRequired;
			if(evaluatorRequired != null) {
				IEnumerable<string> filterDataMembers = evaluatorRequired.GetDataMembers();
				return FilterByEvaluatorRequiredDataMembers(axisItems, filterDataMembers);
			}
			return  null;
		}
		#region IFormatRuleIntersectionLevel Members
		bool IFormatRuleIntersectionLevel.ApplyToColumn { get { return ApplyToColumn; } }
		#endregion
	}
}
