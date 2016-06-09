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
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraReports.Design;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ConfigureExcelFileColumnsPageView : WizardViewBase, IConfigureExcelFileColumnsPageView {
		class ExcelColumn {
			#region TypeConverter
			public class ExcelColumnTypeConverter : ParameterTypeConverter {
				public const string guid = "ExcelColumnsEditor";
				const string context = "ExcelColumnsEditorExtension";
				protected override string GetContextName(ITypeDescriptorContext context) {
					string baseResult = base.GetContextName(context);
					if(string.IsNullOrEmpty(baseResult)) {
						return guid;
					}
					return baseResult;
				}
				protected override string GetExtensionKey() {
					return context;
				}
			}
			#endregion
			public string Name { get; set; }
			[TypeConverter(typeof(ExcelColumnTypeConverter))]
			public Type Type { get; set; }
			public bool Selected { get; set; }
		}
		Func<FieldInfo[], SelectedDataEx> loadPreviewData;
		List<ExcelColumn> dataSource;
		static ConfigureExcelFileColumnsPageView() {
			string context = ExcelColumn.ExcelColumnTypeConverter.guid;
			ParameterEditorService.AddParameterType(context, typeof(byte), "Byte");
			ParameterEditorService.AddParameterType(context, typeof(bool), "Boolean");
			ParameterEditorService.AddParameterType(context, typeof(sbyte), "Signed Byte");
			ParameterEditorService.AddParameterType(context, typeof(int), "Integer");
			ParameterEditorService.AddParameterType(context, typeof(uint), "Unsigned Integer");
			ParameterEditorService.AddParameterType(context, typeof(short), "Short Integer");
			ParameterEditorService.AddParameterType(context, typeof(ushort), "Unsigned Short Integer");
			ParameterEditorService.AddParameterType(context, typeof(long), "Long Integer");
			ParameterEditorService.AddParameterType(context, typeof(ulong), "Unsigned Long Integer");
			ParameterEditorService.AddParameterType(context, typeof(double), "Double");
			ParameterEditorService.AddParameterType(context, typeof(float), "Float");
			ParameterEditorService.AddParameterType(context, typeof(decimal), "Decimal");
			ParameterEditorService.AddParameterType(context, typeof(DateTime), "DateTime");
			ParameterEditorService.AddParameterType(context, typeof(char), "Char");
			ParameterEditorService.AddParameterType(context, typeof(string), "String");
			ParameterEditorService.AddParameterType(context, typeof(object), "Object");
		}
		public ConfigureExcelFileColumnsPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns); }
		}
		#endregion
		#region Implementation of IConfigureExcelFileColumnsPageView
		public FieldInfo[] Schema {
			get { return ((List<ExcelColumn>)gridControl.DataSource).Select(gr => new FieldInfo() { Name = gr.Name, Type = gr.Type, Selected = gr.Selected }).ToArray(); }
		}
		public event EventHandler Changed;
		public void Initialize(FieldInfo[] schema, Func<FieldInfo[], SelectedDataEx> loadPreviewData) {
			this.loadPreviewData = loadPreviewData;
			dataSource = schema.Select(c => new ExcelColumn() { Name = c.Name, Type = c.Type, Selected = c.Selected }).ToList();
			gridControl.DataSource = dataSource;
			PopulateTypeComboBox();
		}
		#endregion
		#region UI Event handlers
		void cbeType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			SetDisplayText(e.Value, s => e.DisplayText = s);
		}
		void ceSelected_CheckedChanged(object sender, EventArgs e) {
			gridView.PostEditor();
		}
		void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			if(e.Column == gridColumnSelected) {
				RaiseChanged();
			}
		}
		void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column == gridColumnType) {
				SetDisplayText(e.Value, s => e.DisplayText = s);
			}
		}
		void btnPreview_Click(object sender, EventArgs e) {
			if(loadPreviewData != null) {
				SelectedDataEx data = loadPreviewData(Schema);
				ShowDataPrviewForm(data);
			}
		}
		protected virtual void ShowDataPrviewForm(SelectedDataEx data) {
			using(DataPreviewForm dataPreviewForm = new DataPreviewForm(false) {
				DataSource = new DataView(null, data)
			}) {
				dataPreviewForm.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				dataPreviewForm.ShowDialog(this);
			}
		}
		void gridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e) {
			if(gridView.FocusedColumn == gridColumnName) {
				string newName = e.Value.ToString();
				if(string.IsNullOrEmpty(newName)) {
					e.ErrorText = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns_ColumnNameEmptyError);
					e.Valid = false;
					return;
				}
				e.Valid = dataSource.All(ci => ci.Name != newName);
				if(!e.Valid)
					e.ErrorText = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns_ColumnExist), newName);
			}
		}
		#endregion
		protected virtual void SetDisplayText(object value, Action<string> setText) {
			Type type = value as Type;
			if(type != null) {
				string displayText = ParameterEditorService.GetDisplayNameByParameterType(ExcelColumn.ExcelColumnTypeConverter.guid, type);
				setText(displayText);
			}
		}
		readonly IEnumerable<Type> allowedTypes = new List<Type> {
			typeof(byte),
			typeof(sbyte),
			typeof(int),
			typeof(uint),
			typeof(short),
			typeof(ushort),
			typeof(long),
			typeof(ulong),
			typeof(bool),
			typeof(double),
			typeof(float),
			typeof(decimal),
			typeof(DateTime),
			typeof(char),
			typeof(string)
		};
		void LocalizeComponent() {
			buttonPreview.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Preview);
			gridColumnSelected.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Selected);
			gridColumnName.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Name);
			gridColumnType.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Type);
		}
		void PopulateTypeComboBox() {
			comboBoxType.Items.Clear();
			Dictionary<Type, string> types = ParameterEditorService.GetParameterTypes(ExcelColumn.ExcelColumnTypeConverter.guid);
			comboBoxType.Items.AddRange(types.Where(t => allowedTypes.Any(at => at == t.Key)).Select(kv => kv.Value).ToList());
		}
		void RaiseChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
	}
}
