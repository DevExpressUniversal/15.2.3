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

using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Helpers;
using DevExpress.XtraPivotGrid.TypeConverters;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridFormatRule : FormatRuleBase, IPivotGridViewInfoDataOwner {
		PivotGridField measure;
		FormatRuleSettings settings = new FormatRuleTotalTypeSettings();
		PivotGridFormatRuleValueProvider valueProvider;
		[XtraSerializableProperty(XtraSerializationVisibility.Content, 100),
		TypeConverter(typeof(ExpandableObjectConverter)), RefreshProperties(RefreshProperties.All),
		Editor(typeof(FormatRuleSettingsTypeEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public FormatRuleSettings Settings {
			get { return settings; }
			set {
				if(value == settings)
					return;
				if(value == null)
					value = new FormatRuleTotalTypeSettings();
				settings = value;
				value.Rule = this;
			}
		}
		protected override DevExpress.Data.IDataColumnInfo GetColumnInfo() {
			return new RuleDataColumnInfoWrapper(null, Collection.GetColumns());
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(0)]
		public string SettingsName {
			get { return FormatRuleSettings.GetSerializationName(settings); }
			set {
				Settings = FormatRuleSettings.Create(value);
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFormatRuleMeasure"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.Measure"),
		DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraPivotGrid.TypeConverters.FieldReferenceConverter, " + AssemblyInfo.SRAssemblyPivotGrid),
		PivotAreaProperty(PivotArea.DataArea)
		]
		public PivotGridField Measure {
			get { return measure; }
			set { measure = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string MeasureName {
			get {
				if(Measure != null)
					return Measure.Name;
				return string.Empty;
			}
			set {
				if(value != null && value != string.Empty && Data != null) {
					Measure = Data.Fields.GetFieldByName(value);
				}
			}
		}
		public override bool IsValid {
			get {
				if(!base.IsValid || Measure == null || Data == null)
					return false;
				if(this.Collection.GetRuleQueryKind(this) != FormatRuleValueQueryKind.None && !(settings is FormatRuleFieldIntersectionSettings))
					return false;
				return settings.IsValid();
			}
		}
		protected new PivotGridFormatRuleCollection Collection { get { return (PivotGridFormatRuleCollection)base.Collection; } }
		internal PivotGridViewInfoData Data {
			get {
				PivotGridFormatRuleCollection collection = Collection;
				return collection != null ? collection.Data : null;
			}
		}
		protected internal PivotGridFormatRuleValueProvider ValueProvider {
			get {
				if(valueProvider == null)
					valueProvider = CreateValueProvider();
				return valueProvider;
			}
		}
		protected virtual PivotGridFormatRuleValueProvider CreateValueProvider() {
			return new PivotGridFormatRuleValueProvider(this);
		}
		internal void ItemChanged() {
			IFormatRuleCollection coll = Collection;
			if(coll != null)
				coll.OnCollectionChanged(new FormatConditionCollectionChangedEventArgs(CollectionChangedAction.Changed, this, FormatConditionRuleChangeType.All));
		}
		protected override ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out bool readyToCreate) {
			readyToCreate = Data != null && Data.IsDataBound && Data.IsControlReady && Data.Fields.Count > 0;
			if(!readyToCreate)
				return null;
			try {
				return new ExpressionEvaluator(Collection.GetProperties(), criteriaOperator) { DataAccess = ValueProvider };
			} catch {
				return null;
			}
		}
		protected override FormatRuleBase CreateInstance() {
			if(Collection == null)
				return new PivotGridFormatRule();
			else
				return Collection.CreateItemInstance();
		}
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return Data; }
		}
		public bool CheckValue(System.Drawing.Point cell) {
			if(Data == null)
				return false;
			return CheckValue(Data.Cells.GetCellInfo(cell.X, cell.Y).Item);
		}
		public bool CheckValue(int rowIndex, int columnIndex) {
			if(Data == null)
				return false;
			return CheckValue(Data.Cells.GetCellInfo(columnIndex, rowIndex).Item);
		}
		public bool CheckValue(PivotGridCellItem item) {
			if(!IsValid || !settings.CanApplyToCell(item) || Data == null || Data.GetFieldItem(Measure) != item.DataField)
				return false;
			ValueProvider.Cell = item;
			return IsFit(ValueProvider);
		}
	}
}
namespace DevExpress.XtraPivotGrid.Helpers {
	public class PivotGridFormatRuleValueProvider : FormatConditionRuleValueProviderBase, IEvaluatorDataAccess {
		PivotGridCellItem cell;
		IEvaluatorDataAccess access;
		public PivotGridCellItem Cell {
			get { return cell; }
			set { 
				cell = value; 
				access = value;
			}
		}
		public PivotGridFormatRuleValueProvider(PivotGridFormatRule format)
			: base(format) {
		}
		protected override object GetValueCore(FormatConditionRuleBase rule) {
			return cell != null ? cell.Value : null;
		}
		protected override object GetValueExpressionCore(FormatConditionRuleBase rule) {
			return cell;
		}
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			return access.GetValue(descriptor, theObject);
		}
	}
}
