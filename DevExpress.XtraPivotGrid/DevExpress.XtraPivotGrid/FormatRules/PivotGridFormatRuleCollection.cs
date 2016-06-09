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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Menu;
using System;
using System.Linq;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridFormatRuleCollection : FormatRuleCollection<PivotGridFormatRule, PivotGridField> {
		PivotGridViewInfoData data;
		internal PivotGridViewInfoData Data { get { return data; } }
		internal PivotGridFormatRule Add(PivotGridField field, FormatConditionRuleBase rule, PivotGridCellItem item, bool applyToLevel) {
			BeginUpdate();
			PivotGridFormatRule formatRule = Add(field, rule);
			if(applyToLevel)
				formatRule.Settings = new FormatRuleFieldIntersectionSettings(item);
			EndUpdate();
			return formatRule;
		}
		public PivotGridFormatRuleCollection(PivotGridViewInfoData data) {
			this.data = data;
		}
		public override void BeginUpdate() {
			data.BeginUpdate();
			base.BeginUpdate();
		}
		public override void EndUpdate() {
			base.EndUpdate();
			data.EndUpdate();
		}
		protected override void AssignColumn(PivotGridFormatRule format, PivotGridField field) {
			format.Measure = field;
		}
		protected override UserLookAndFeel ElementsLookAndFeel {
			get { return data.ControlLookAndFeel; }
		}
		protected override FormatConditionRuleState GetRuleState(FormatRuleBase format) {
			PivotGridFormatRule formatRule = format as PivotGridFormatRule;
			if(formatRule.ValueProvider == null)
				return FormatConditionRuleState.NullState;
			return formatRule.ValueProvider.RuleState;
		}
		protected new internal PivotGridFormatRule CreateItemInstance() {
			return base.CreateItemInstance();
		}
		internal FormatRuleSummaryInfoCollection GetSummaries() {
			return CreateSummaryInfo();
		}
		internal FormatRuleValueQueryKind GetRuleQueryKind(FormatRuleBase rule) {
			return GetRuleState(rule).QueryKind;
		}
		protected internal FilterColumnCollection GetFilterColumnCollection() {
			FilterColumnCollection collection = new FilterColumnCollection();
			foreach(PivotGridField field in GetFields())
				collection.Add(new FormatRuleFieldFilterColumn(field, data.MenuManager));
			return collection;
		}
		protected internal PropertyDescriptorCollection GetProperties() {
			List<PivotGridFieldBase> fields = GetFields();
			PropertyDescriptor[] res = new PropertyDescriptor[fields.Count];
			for(int i = 0; i < fields.Count; i++) {
				res[i] = fields[i].PropertyDescriptor;
			}
			return new PropertyDescriptorCollection(res);
		}
		protected internal List<IDataColumnInfo> GetColumns() {
			List<PivotGridFieldBase> fields = GetFields();
			List<IDataColumnInfo> res = new List<IDataColumnInfo>(fields.Count);
			for(int i = 0; i < fields.Count; i++) {
				res.Add(new FieldSummaryLevelDataColumnInfo(fields[i]));
			}
			return res;
		}
		class FieldSummaryLevelDataColumnInfo : IDataColumnInfo {
			IDataColumnInfo info;
			public FieldSummaryLevelDataColumnInfo(PivotGridFieldBase field) {
				info = field;
			}
			string IDataColumnInfo.Caption {
				get { return info.Caption; }
			}
			List<IDataColumnInfo> IDataColumnInfo.Columns {
				get { return info.Columns; }
			}
			DataControllerBase IDataColumnInfo.Controller {
				get { return info.Controller; }
			}
			string IDataColumnInfo.FieldName {
				get { return info.FieldName.Contains("].[") ? info.Name : info.FieldName; }
			}
			Type IDataColumnInfo.FieldType {
				get { return info.FieldType; }
			}
			string IDataColumnInfo.Name {
				get { return info.Name; }
			}
			string IDataColumnInfo.UnboundExpression {
				get { return info.UnboundExpression; }
			}
		}
		protected List<PivotGridFieldBase> GetFields() {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			if(Data == null)
				return res;
			foreach(PivotGridField field in Data.Fields) {
				if(field.Area == PivotArea.FilterArea || !field.Visible)
					continue;
				res.Add(field);
			}
			return res;
		}
	}
	class FormatRuleFieldFilterColumn : FieldFilterColumnBase {
		public FormatRuleFieldFilterColumn(PivotGridFieldBase field, IDXMenuManager menuManager)
			: base(field, menuManager) {
		}
		public override string FieldName {
			get { return PivotGridFieldPropertyDescriptor.GetNameByField(Field); }
		}
	}
}
