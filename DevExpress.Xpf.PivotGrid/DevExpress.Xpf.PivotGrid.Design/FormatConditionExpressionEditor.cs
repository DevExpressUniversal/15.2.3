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
using System.Windows;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.ExpressionEditor;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using System.Linq;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Editors.Filtering;
using System.Windows.Markup;
namespace DevExpress.Xpf.PivotGrid.Design {
	public abstract class PivotGridDialogPropertyValueEditorBase : DialogPropertyValueEditor {
		public PivotGridDialogPropertyValueEditorBase() {
		   this.InlineEditorTemplate = PivotGridDesignTimeHelper.EditorsTemplates["DialogEditorTemplate"] as DataTemplate;
		}
		public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource) {
			ModelItem selectedItem = PivotGridDesignTimeHelper.GetPrimarySelection(propertyValue);
			PivotGridControl dataControl = PivotGridDesignTimeHelper.GetPivotGrid(selectedItem);
			if(dataControl == null) {
				MessageBox.Show(CantShowEditorMessage);
				return;
			}
			string newStringValue = ShowEditorCore(dataControl, propertyValue, selectedItem);
			if(newStringValue != propertyValue.StringValue)
				propertyValue.StringValue = newStringValue;
		}
		protected abstract string CantShowEditorMessage { get; }
		protected abstract string ShowEditorCore(PivotGridControl pivotGrid, PropertyValue propertyValue, ModelItem selectedItem);
	}
	public class FormatConditionExpressionEditor : PivotGridDialogPropertyValueEditorBase {
		public FormatConditionExpressionEditor() { }
		class IDataColumnInfo2 : IDataColumnInfo {
			IDataColumnInfo info;
			List<IDataColumnInfo> columns;
			public IDataColumnInfo2(IDataColumnInfo info, List<IDataColumnInfo> columns) {
				this.info = info;
				this.columns = columns;
			}
			string IDataColumnInfo.Caption {
				get { return info.Caption ; }
			}
			List<IDataColumnInfo> IDataColumnInfo.Columns {
				get { return columns; }
			}
			DataControllerBase IDataColumnInfo.Controller {
				get { return info.Controller; }
			}
			string IDataColumnInfo.FieldName {
				get { return info.Name; }
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
		class EmptyColumnInfo : IDataColumnInfo {
			public EmptyColumnInfo(PivotGridControl pivotGrid) {
				List<IDataColumnInfo> result = new List<IDataColumnInfo>();
				result.AddRange(pivotGrid.Fields.Where((f) => !string.IsNullOrEmpty(f.Name)).Select((f) => new IDataColumnInfo2((IDataColumnInfo)f.GetInternalField(), result)));
				Columns = result; 
			}
			#region IDataColumnInfo Members
			public string Caption { get { return string.Empty; } }
			public List<IDataColumnInfo> Columns { get; set; }
			public DataControllerBase Controller { get { return null; } }
			public string FieldName { get { return string.Empty; } }
			public Type FieldType { get { return typeof(object); } }
			public string Name { get { return string.Empty; } }
			public string UnboundExpression { get { return string.Empty; } }
			#endregion
		}
		protected override string ShowEditorCore(PivotGridControl pivotGrid, PropertyValue propertyValue, ModelItem selectedItem) {
			string expression = propertyValue.StringValue;
			ExpressionEditorControl expressionEditorControl = new ExpressionEditorControl(new EmptyColumnInfo(pivotGrid));
			DialogClosedDelegate closedHandler = delegate(bool? dialogResult) {
				if(dialogResult == true)
					expression = expressionEditorControl.Expression;
			};
			DevExpress.Xpf.Editors.ExpressionEditor.Native.ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, pivotGrid, closedHandler);
			return expression;
		}
		protected override string CantShowEditorMessage { get { return SR.CantShowFormatConditionExpressionEditorMessage; } }
	}
	public class FormatConditionExpressionFilterEditor : FormatConditionExpressionEditor {
		protected override string ShowEditorCore(PivotGridControl pivotGrid, PropertyValue propertyValue, ModelItem selectedItem) {
			FilterControl filterControl = new FilterControl();
			filterControl.FilterCriteria = DevExpress.Data.Filtering.CriteriaOperator.TryParse(propertyValue.StringValue);
			filterControl.ShowBorder = false;
			filterControl.SourceControl = PivotGridControl.GetData(pivotGrid);
			FloatingContainer.ShowDialogContent(filterControl, pivotGrid, new Size(500, 350), new FloatingContainerParameters() {
				Title = "Custom Condition Editor",
				AllowSizing = true,
				ShowApplyButton = false,
				CloseOnEscape = false,
			});
			if(!Object.ReferenceEquals(filterControl.FilterCriteria, null))
				return filterControl.FilterCriteria.ToString();
			return propertyValue.StringValue;
		}
	}
}
