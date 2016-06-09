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
using System.Linq;
using System.Reflection;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Data;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Services;
namespace DevExpress.Xpf.PivotGrid.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		protected override Assembly RuntimeAssembly {
			get { return typeof(PivotGridControl).Assembly; }
		}
		protected override string ToolboxCategoryPath {
			get { return AssemblyInfo.DXTabNameData; }
		}
		protected override void PrepareAttributeTable(Microsoft.Windows.Design.Metadata.AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			builder.AddCustomAttributes(typeof(PivotGridField), "FieldName", new TypeConverterAttribute(typeof(PivotGridFieldFieldNameTypeConverter)));
			builder.AddCustomAttributes(typeof(PivotGridControl), new FeatureAttribute(typeof(PivotGridControlAdornerProvider)));
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PivotGridControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new FieldPropertyLinesProvider());
			TypeDescriptor.AddAttributes(typeof(PivotGridField), new DevExpress.Xpf.Core.Design.SmartTags.DesignTimeParentAttribute(typeof(PivotGridControl), typeof(PivotGridFieldProvider)));
			builder.AddCustomAttributes(typeof(PivotGridControl), "FormatConditions", new NewItemTypesAttribute(typeof(ColorScaleFormatCondition)));
			builder.AddCustomAttributes(typeof(PivotGridControl), "FormatConditions", new NewItemTypesAttribute(typeof(DataBarFormatCondition)));
			builder.AddCustomAttributes(typeof(PivotGridControl), "FormatConditions", new NewItemTypesAttribute(typeof(FormatCondition)));
			builder.AddCustomAttributes(typeof(PivotGridControl), "FormatConditions", new NewItemTypesAttribute(typeof(IconSetFormatCondition)));
			builder.AddCustomAttributes(typeof(PivotGridControl), "FormatConditions", new NewItemTypesAttribute(typeof(TopBottomRuleFormatCondition)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.MeasureNameProperty), new TypeConverterAttribute(typeof(PivotGridFieldNameTypeConverter)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.RowNameProperty), new TypeConverterAttribute(typeof(PivotGridFieldNameTypeConverter)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.ColumnNameProperty), new TypeConverterAttribute(typeof(PivotGridFieldNameTypeConverter)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.PredefinedFormatNameProperty), PropertyValueEditor.CreateEditorAttribute(typeof(ConditionFormatNameEditor)));
			builder.AddCustomAttributes(typeof(ColorScaleFormatCondition), DesignHelper.GetPropertyName(ColorScaleFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(DataBarFormatCondition), DesignHelper.GetPropertyName(DataBarFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(FormatCondition), DesignHelper.GetPropertyName(FormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionFilterEditor)));
			builder.AddCustomAttributes(typeof(IconSetFormatCondition), DesignHelper.GetPropertyName(IconSetFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(TopBottomRuleFormatCondition), DesignHelper.GetPropertyName(TopBottomRuleFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(IndicatorFormatConditionBase), DesignHelper.GetPropertyName(IndicatorFormatConditionBase.SelectiveExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionFilterEditor)));
		}
	}
	public class PivotGridFieldNameTypeConverter : PivotGridFieldFieldNameTypeConverter {
		public PivotGridFieldNameTypeConverter() { }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			PivotGridControl pivot = GetPivotFromContext(context);
			return pivot != null && pivot.Fields.Where((f) => !string.IsNullOrEmpty(f.Name)).Count() > 0;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			PivotGridControl pivot = GetPivotFromContext(context);
			return new StandardValuesCollection(pivot.Fields.Where((f) => !string.IsNullOrEmpty(f.Name)).Select((f) => f.Name).ToList());
		}
	}
	public class PivotGridFieldFieldNameTypeConverter : TypeConverter {
		public PivotGridFieldFieldNameTypeConverter() { }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			PivotGridControl pivot = GetPivotFromContext(context);
			if(pivot == null)
				return false;
			if(pivot.DataSource as IPivotOLAPDataSource != null)
				return false;
			PivotGridWpfData data = PivotGridControl.GetData(pivot);
			if(data == null || data.ListDataSource == null || data.ListSource == null || pivot.DataSource == null)
				return false;
			string[] fields = ((PivotGridWpfData)PivotGridControl.GetData(pivot)).ListDataSource.GetFieldList();
			return fields != null && fields.Length > 0;
		}
		public static PivotGridControl GetPivotFromContext(ITypeDescriptorContext context) {
			ModelService service = context.GetService(typeof(ModelService)) as ModelService;
			if(service == null || service.Root == null || service.Root.Context == null || service.Root.Context.Items == null)
				return null;
			Selection selection = service.Root.Context.Items.GetValue<Selection>();
			if(selection == null || selection.PrimarySelection == null)
				return null;
			PivotGridControl pivot = selection.PrimarySelection.GetCurrentValue() as PivotGridControl;
			if(pivot != null)
				return pivot;
			FormatConditionBase cond = selection.PrimarySelection.GetCurrentValue() as FormatConditionBase;
			if(cond != null)
				return cond.Owner.Owner as PivotGridControl;
			return null;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> list = new List<string>();
			PivotGridControl pivot = GetPivotFromContext(context);
			if(!(pivot.DataSource != null && pivot.DataSource as IPivotOLAPDataSource == null))
				return new StandardValuesCollection(list);
			PivotGridWpfData data = PivotGridControl.GetData(pivot);
			if(data == null || data.ListDataSource == null || data.ListSource == null)
				return new StandardValuesCollection(list);
			list.AddRange(((PivotGridWpfData)PivotGridControl.GetData(pivot)).ListDataSource.GetFieldList());
			return new StandardValuesCollection(list);
		}
	}
}
