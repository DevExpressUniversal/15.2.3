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
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraReports.Design;
using PopupMenuShowingEventArgs = DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs;
namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	[ToolboxItem(false)]
	public partial class ParametersGrid : XtraUserControl, IParametersGridView {
		#region static
		static ParametersGrid() {
			ParameterEditorService.AddParameterType(guid, typeof(Expression), "Expression");
		}
		#endregion
		const string guid = "ParametersGrid";
		SimpleButton buttonAdd;
		SimpleButton buttonRemove;
		SimpleButton buttonPreview;
		IServiceProvider serviceProvider;
		IParameterService parameterService;
		IRepositoryItemsProvider repositoryItemsProvider;
		IParametersGridViewModel viewModel;
		public ParametersGrid() {
			InitializeComponent();
			LocalizeComponent();
		}
		GridRecord FocusedRecord {
			get { return (GridRecord)gridView.GetFocusedRow(); }
		}
		#region auto popup
		void isExprCheckEdit_CheckedChanged(object sender, EventArgs e) {
			gridView.CloseEditor();
			if(FocusedRecord.IsExpression) {
				gridView.FocusedColumn = valueColumn;
				gridView.ShowEditor();
			}
		}
		void GridViewShownEditor(object sender, EventArgs e) {
			if(gridView.FocusedColumn != valueColumn)
				return;
			GridView view = (GridView)sender;
			PopupBaseAutoSearchEdit editor = view.ActiveEditor as PopupBaseAutoSearchEdit;
			if(editor != null) {
				editor.ShowPopup();
			}
		}
		#endregion
		#region custom type display text
		void typeComboBox_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			e.DisplayText = viewModel.GetDisplayName(FocusedRecord);
		}
		void GridViewCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column != typeColumn)
				return;
			GridRecord record = GetRecordByIndex(e.ListSourceRowIndex);
			e.DisplayText = viewModel.GetDisplayName(record);
		}
		#endregion
		#region buttons event handlers
		void ButtonPreviewClick(object sender, EventArgs e) {
			if(!viewModel.IsPreviewAvailable)
				return;
			object dataPreview = viewModel.LoadPreviewData();
			if(dataPreview == null)
				return;
			using(DataPreviewForm form = new DataPreviewForm(!this.viewModel.PreviewDataRowLimit)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				form.DataSource = dataPreview;
				form.ShowDialog(this);
			}
		}
		void ButtonAddClick(object sender, EventArgs e) {
			if(viewModel.IsFixedParameters)
				return;
			viewModel.OnCreateParameter();
			gridView.FocusedColumn = gridView.Columns["Value"];
			gridView.ShowEditor();
		}
		void ButtonRemoveClick(object sender, EventArgs e) {
			if(gridView.SelectedRowsCount >= 1)
				viewModel.OnRemoveParameter(FocusedRecord);
		}
		#endregion
		#region viewmodel events
		void viewModel_ExternalParameterAdded(object sender, EventArgs e) {
			PopulateValueCombobox(viewModel.ExternalParameterExpressions);
		}
		void viewModel_CanRemoveParametersChanged(object sender, EventArgs e) {
			if(buttonRemove != null) {
				buttonRemove.Enabled = viewModel.CanRemoveParameters;
			}
		}
		#endregion
		void GridViewCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
			if(e.Column == valueColumn) {
				object isExpression = gridView.GetRowCellValue(e.RowHandle, "IsExpression");
				if(isExpression != null && (bool)isExpression) {
					if(e.RepositoryItem.GetType() != valueComboBox.GetType())
						e.RepositoryItem = valueComboBox;
				} else {
					Type type = (Type)gridView.GetRowCellValue(e.RowHandle, "Type");
					e.RepositoryItem = repositoryItemsProvider.GetRepositoryItem(type);
				}
			}
		}
		void GridViewPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			switch(e.MenuType) {
				case GridMenuType.Column:
				case GridMenuType.Group:
					foreach(DXMenuItem item in e.Menu.Items.Cast<DXMenuItem>().Where(item => item.Tag is GridStringId)) {
						switch((GridStringId)item.Tag) {
							case GridStringId.MenuColumnBestFit:
							case GridStringId.MenuColumnGroupBox:
							case GridStringId.MenuColumnRemoveColumn:
							case GridStringId.MenuColumnBestFitAllColumns:
							case GridStringId.MenuColumnGroup:
								item.Visible = false;
								break;
						}
					}
					break;
			}
		}
		void GridViewValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e) {
			switch(gridView.FocusedColumn.FieldName) {
				case "Name":
					e.Valid = e.Value != null && !string.IsNullOrEmpty(e.Value.ToString());
					break;
				case "Type":
					e.Valid = true;
					break;
				case "Value": {
						object oldValue = e.Value;
						Type type = (Type)gridView.GetFocusedRowCellValue("Type");
						if(type == typeof(Expression)) {
							CriteriaOperator criteriaOperator = CriteriaOperator.TryParse((oldValue as Expression ?? new Expression(oldValue.ToString())).ExpressionString);
							e.Valid = !ReferenceEquals(criteriaOperator, null);
							return;
						}
						object newValue;
						e.Valid = ValueHelper.ConvertValue(oldValue, type, out newValue);
					}
					break;
			}
		}
		void valueComboBox_EditValueChanging(object sender, ChangingEventArgs e) {
			if(e.NewValue == valueComboBox.Items[0]) {
				viewModel.OnExpressionEdit(CreateExpressionView, FocusedRecord);
				e.Cancel = true;
				gridView.CloseEditor();
			} else if(parameterService != null && e.NewValue == valueComboBox.Items[1]) {
				viewModel.OnCreateExternalParameter(CreatePropertyGridView, FocusedRecord);
				e.Cancel = true;
				gridView.CloseEditor();
			}
		}
		void typeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			gridView.CloseEditor(); 
		}
		#region IParametersGridView
		public IEnumerable<IParameter> Parameters { get { gridView.CloseEditor(); return viewModel.Records.Select(r => new QueryParameter(r.Name, r.Type, r.Value)); } }
		public void Initialize(ParametersGridViewModel viewModel, IServiceProvider serviceProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			Guard.ArgumentNotNull(repositoryItemsProvider, "repositoryItemsProvider");
			this.viewModel = viewModel;
			this.parameterService = parameterService;
			this.repositoryItemsProvider = repositoryItemsProvider;
			this.serviceProvider = serviceProvider;
			this.PopulateTypeCombobox(viewModel.Types);
			this.PopulateValueCombobox(viewModel.ExternalParameterExpressions);
			nameColumn.OptionsColumn.ReadOnly = viewModel.IsFixedParameters;
			nameColumn.OptionsColumn.AllowFocus = !viewModel.IsFixedParameters;
			typeColumn.OptionsColumn.ReadOnly = viewModel.IsFixedParameters;
			typeColumn.OptionsColumn.AllowFocus = !viewModel.IsFixedParameters;
			viewModel.CanRemoveParametersChanged += viewModel_CanRemoveParametersChanged;
			viewModel.ExternalParameterAdded += viewModel_ExternalParameterAdded;
			gridView.OptionsCustomization.AllowSort = !viewModel.IsFixedParameters;
			gridControl.DataSource = this.viewModel.Records;
		}
		public void SetButtons(SimpleButton buttonAdd, SimpleButton buttonRemove, SimpleButton buttonPreview) {
			if(this.buttonAdd != null) {
				this.buttonAdd.Click -= ButtonAddClick;
			}
			if(buttonAdd != null) {
				this.buttonAdd = buttonAdd;
				this.buttonAdd.Click += ButtonAddClick;
			}
			if(this.buttonRemove != null) {
				this.buttonRemove.Click -= ButtonRemoveClick;
			}
			if(buttonRemove != null) {
				this.buttonRemove = buttonRemove;
				this.buttonRemove.Click += ButtonRemoveClick;
				this.buttonRemove.Enabled = viewModel.CanRemoveParameters;
			}
			if(this.buttonPreview != null) {
				this.buttonPreview.Click -= ButtonPreviewClick;
			}
			if(buttonPreview != null) {
				this.buttonPreview = buttonPreview;
				this.buttonPreview.Click += ButtonPreviewClick;
				this.buttonPreview.Enabled = viewModel.IsPreviewAvailable;
			}
		}
		public void PopulateValueCombobox(object[] values) {
			valueComboBox.Items.Clear();
			valueComboBox.Items.Add("Expression editor..."); 
			if(parameterService != null && parameterService.CanCreateParameters) {
				valueComboBox.Items.Add(parameterService.AddParameterString);
			}
			valueComboBox.Items.AddRange(values);
		}
		public void PopulateTypeCombobox(object[] types) {
			typeComboBox.Items.Clear();
			typeComboBox.Items.AddRange(types);
		}
		#endregion
		#region internal use
		internal bool BorderVisible {
			set { gridView.BorderStyle = value ? BorderStyles.Default : BorderStyles.NoBorder; }
		}
		internal bool ShowPreviewButton {
			get {
				LayoutControl layoutControl = buttonPreview.Parent as LayoutControl;
				if(layoutControl == null)
					return buttonPreview.Visible;
				LayoutControlItem layoutControlItem = layoutControl.GetItemByControl(buttonPreview);
				return layoutControlItem.ContentVisible;
			}
			set {
				LayoutControl layoutControl = buttonPreview.Parent as LayoutControl;
				if(layoutControl == null) {
					buttonPreview.Visible = value;
					return;
				}
				LayoutControlItem layoutControlItem = layoutControl.GetItemByControl(buttonPreview);
				layoutControlItem.ContentVisible = value;
			}
		}
		#endregion
		#region Views
		PropertyGridView CreatePropertyGridView() {
			IServiceContainer serviceContainer = new ServiceContainer(serviceProvider);
			if(parameterService != null)
				serviceContainer.AddService(typeof(IParameterService), parameterService);
			return new PropertyGridView(FindForm(), serviceContainer);
		}
		ExpressionView CreateExpressionView(DataSourceParameterBase parameter) {
			return new ExpressionView(parameter, FindForm(), serviceProvider, LookAndFeel);
		}
		#endregion
		GridRecord GetRecordByIndex(int index) {
			return (GridRecord)gridView.GetRow(index);
		}
		void LocalizeComponent() {
			nameColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Name);
			typeColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Type);
			isExprColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Expression);
			valueColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Value);
		}
	}
}
