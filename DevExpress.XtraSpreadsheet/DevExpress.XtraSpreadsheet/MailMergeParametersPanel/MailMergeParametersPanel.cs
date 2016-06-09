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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Utils.Menu;
using DevExpress.Utils.UI;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraReports.Native;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	[DXToolboxItem(false)]
	public class MailMergeParametersDockPanel : DockPanel {
		ISpreadsheetControl spreadsheetControl;
		MailMergeParametersGrid parametersGrid;
		[ DefaultValue(null)]
		public ISpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				spreadsheetControl = value;
				if(parametersGrid != null)
					parametersGrid.SpreadsheetControl = value;
			}
		}
		string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_Text); } }
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		public MailMergeParametersDockPanel() {
			InitializeControl();
			Text = DefaultText;
		}
		public MailMergeParametersDockPanel(DockManager dockManager, DockingStyle dock)
			: base(true, dock, dockManager) {
			Dock = dock;
			InitializeControl();
			Text = DefaultText;
		}
		protected void InitializeControl() {
			if(parametersGrid == null) {
				parametersGrid = new MailMergeParametersGrid {
					SpreadsheetControl = SpreadsheetControl
				};
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			Control control = parametersGrid;
			if(control != null && control.Parent == null && ControlContainer != null)
				ControlContainer.Controls.Add(control);
		}
		protected override void CreateControlContainer() {
			InitializeControl();
			base.CreateControlContainer();
		}
		public override void ResetText() {
			Text = DefaultText;
		}
		protected virtual bool ShouldSerializeText() {
			return Text != DefaultText;
		}
	}
	[DXToolboxItem(false)]
	internal class MailMergeParametersGrid : XtraGrid.GridControl, IMailMergeParametersGrid {
		#region Fields
		ISpreadsheetControl spreadsheetControl;
		XtraGrid.Views.Grid.GridView gridView;
		XtraGrid.Columns.GridColumn nameColumn;
		XtraGrid.Columns.GridColumn valueColumn;
		readonly MailMergeParametersGridController controller;
		GridHitInfo downHitInfo = null;
		#endregion
		#region Properties
		public ISpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				if(spreadsheetControl != value) {
					spreadsheetControl = value;
					Controller.SpreadsheetControl = value;
					UpdateDataSource();
					RefreshGrid();
				}
			}
		}
		public MailMergeParametersGridController Controller { get { return controller; } }
		#endregion
		public MailMergeParametersGrid() {
			InitializeComponent();
			controller = new MailMergeParametersGridController(this);
		}
		public void RefreshGrid() {
			if(SpreadsheetControl == null)
				return;
			this.BeginUpdate();
			BindingList<SpreadsheetParameter> parameters = new BindingList<SpreadsheetParameter>();
			MailMergeParametersCollection mailMergeParameters = SpreadsheetControl.InnerControl.DocumentModel.MailMergeParameters;
			mailMergeParameters.ForEach(parameters.Add);
			this.DataSource = parameters;
			RefreshDataSource();
			this.EndUpdate();
		}
		void UpdateDataSource() {
			DataSource = SpreadsheetControl == null ? null : SpreadsheetControl.InnerControl.DocumentModel.MailMergeParameters;
		}
		void InitializeComponent() {
			this.gridView = new XtraGrid.Views.Grid.GridView();
			this.nameColumn = new XtraGrid.Columns.GridColumn();
			this.valueColumn = new XtraGrid.Columns.GridColumn();
			((ISupportInitialize) (this)).BeginInit();
			((ISupportInitialize) (this.gridView)).BeginInit();
			this.SuspendLayout();
			this.Dock = DockStyle.Fill;
			this.MainView = this.gridView;
			this.ViewCollection.AddRange(new XtraGrid.Views.Base.BaseView[] {this.gridView});
			this.gridView.Columns.AddRange(new[] {
				this.nameColumn,
				this.valueColumn
			});
			this.gridView.GridControl = this;
			this.gridView.OptionsBehavior.AutoPopulateColumns = false;
			this.gridView.OptionsCustomization.AllowColumnMoving = false;
			this.gridView.OptionsCustomization.AllowColumnResizing = false;
			this.gridView.OptionsCustomization.AllowFilter = false;
			this.gridView.OptionsCustomization.AllowGroup = false;
			this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridView.OptionsCustomization.AllowSort = false;
			this.gridView.OptionsView.ShowGroupPanel = false;
			this.gridView.OptionsMenu.EnableColumnMenu = false;
			this.gridView.OptionsMenu.EnableFooterMenu = false;
			this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridView.CustomRowCellEdit += this.gridView_CustomRowCellEdit;
			this.gridView.CellValueChanged += this.gridView_CellValueChanged;
			this.nameColumn.Caption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_NameColumn);
			this.nameColumn.OptionsColumn.ReadOnly = true;
			this.nameColumn.OptionsColumn.AllowEdit = false;
			this.nameColumn.FieldName = "Name";
			this.nameColumn.Visible = true;
			this.valueColumn.Caption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_ValueColumn);
			this.valueColumn.FieldName = "Value";
			this.valueColumn.Visible = true;
			this.MouseDown += gridControl_MouseDown;
			this.MouseMove += gridControl_MouseMove;
			((ISupportInitialize) (this.gridView)).EndInit();
			((ISupportInitialize) (this)).EndInit();
			this.ResumeLayout(false);
		}
		void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			Controller.EditSpreadsheetValue(e.RowHandle, e.Value);
		}
		void gridView_CustomRowCellEdit(object sender, XtraGrid.Views.Grid.CustomRowCellEditEventArgs e) {
			if(e.Column.FieldName != "Value")
				return;
			if(SpreadsheetControl == null)
				return;
			int index = e.RowHandle;
			IParameter parameter = SpreadsheetControl.InnerControl.DocumentModel.MailMergeParameters[index];
			e.RepositoryItem = DataEditorService.GetRepositoryItem(parameter.Type);
		}
		void gridControl_MouseDown(object sender, MouseEventArgs e) {
			GridHitInfo calcHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
			switch(e.Button) {
				case MouseButtons.Right:
					DoShowMenu(calcHitInfo);
					break;
				case MouseButtons.Left:
					if(ModifierKeys != Keys.None)
						return;
					if(calcHitInfo.RowHandle >= 0)
						downHitInfo = calcHitInfo;
					break;
			}
		}
		void gridControl_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && downHitInfo != null) {
				Size dragSize = SystemInformation.DragSize;
				Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
					downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if(dragRect.Contains(new Point(e.X, e.Y)))
					return;
				SpreadsheetParameter parameter = (SpreadsheetParameter)gridView.GetRow(downHitInfo.RowHandle);
				this.DoDragDrop(parameter, DragDropEffects.Copy);
				downHitInfo = null;
				DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			}
		}
		void DoShowMenu(GridHitInfo hi) {
			GridViewMenu menu = null;
			menu = new GridViewColumnButtonMenu(gridView, SpreadsheetControl);
			menu.Init(hi);
			menu.Show(hi.HitPoint);
		}
	}
	internal class GridViewColumnButtonMenu : GridViewMenu {
		#region Fields
		readonly ISpreadsheetControl spreadsheetControl;
		#endregion
		#region Properties
		ISpreadsheetControl SpreadsheetControl { get { return spreadsheetControl; } }
		#endregion
		public GridViewColumnButtonMenu(XtraGrid.Views.Grid.GridView view, ISpreadsheetControl spreadsheetControl)
			: base(view) {
			this.spreadsheetControl = spreadsheetControl;
		}
		protected override void CreateItems() {
			Items.Clear();
			DXMenuItem columnsItem = new DXMenuItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MailMergeParametersDockPanel_EditParameters));
			columnsItem.Tag = "Edit";
			columnsItem.Click += OnMenuItemClick;
			Items.Add(columnsItem);
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, null))
				return;
			DXMenuItem item = sender as DXMenuItem;
			if(item == null)
				return;
			if((string) item.Tag == "Edit") {
				PerformEditParameters();
			}
		}
		[System.Security.SecuritySafeCritical]
		void PerformEditParameters() {
			SpreadsheetParametersEditor collectionEditor = new SpreadsheetParametersEditor(typeof(List<EditableParameter>), SpreadsheetControl);
			List<EditableParameter> parameters = new List<EditableParameter>();
			ParametersCollection mailMergeParameters = SpreadsheetControl.Document.MailMergeParameters;
			foreach(Parameter parameter in mailMergeParameters) {
				parameters.Add(new EditableParameter(parameter.Name, parameter.Type, parameter.Value));
			}
			parameters = collectionEditor.EditValue(SpreadsheetControl, parameters) as List<EditableParameter>;
			if(parameters == null)
				return;
			mailMergeParameters.Clear();
			foreach(EditableParameter parameter in parameters) {
				mailMergeParameters.AddParameter(parameter.Name, parameter.Type, parameter.Value);
			}
		}
	}
	internal class EditableParameter : Parameter {
		#region Fields
		readonly SpreadsheetParameter modelParameter;
		#endregion
		#region Implementation of Parameter
		public string Name { get { return modelParameter.Name; } set { modelParameter.Name = value; } }
		[TypeConverter("DevExpress.XtraReports.Design.ParameterTypeConverter," + AssemblyInfo.SRAssemblyUtilsUI), RefreshProperties(RefreshProperties.All)]
		public Type Type { get { return modelParameter.Type; } set { modelParameter.Type = value; } }
		[TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
		public object Value { get { return modelParameter.Value; } set { modelParameter.Value = value; } }
		#endregion
		public EditableParameter(string name, Type type, object value) {
			modelParameter = new SpreadsheetParameter();
			modelParameter.Name = name;
			modelParameter.Type = type;
			modelParameter.Value = value;
		}
		public EditableParameter() {
			modelParameter = new SpreadsheetParameter();
		}
	}
	internal class SpreadsheetParametersEditor : CollectionEditor {
		#region Fields
		const string ParameterName = "parameter";
		readonly ISpreadsheetControl spreadsheetControl;
		#endregion
		public SpreadsheetParametersEditor(Type type, ISpreadsheetControl spreadsheetControl) : base(type) {
			this.spreadsheetControl = spreadsheetControl;
		}
		protected override object CreateInstance(Type itemType) {
			if(itemType != typeof(EditableParameter))
				return base.CreateInstance(itemType);
			IItemsContainer itemsContainer = (IItemsContainer) this.GetService(typeof(IItemsContainer));
			return new EditableParameter {
				Name = GetParameterName(itemsContainer)
			};
		}
		public static string GetParameterName(IItemsContainer parameters) {
			List<string> parameterNames = new List<string>();
			if(parameters != null) {
				foreach(EditableParameter item in parameters.Items) {
					parameterNames.Add(item.Name);
				}
			}
			return GetName(parameterNames, ParameterName);
		}
		public static string GetName(List<string> names, string baseName) {
			int number = 1;
			while(names.Contains(baseName + number))
				number++;
			return baseName + number;
		}
		protected override CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			SpreadsheetParametersEditorForm form = new SpreadsheetParametersEditorForm(serviceProvider, this, spreadsheetControl);
			form.ShowDescription = CultureHelper.IsEnglishCulture(CultureInfo.CurrentCulture);
			return form;
		}
		#region Overrides of CollectionEditor
		protected override IList GetEditValue(object value) {
			List<EditableParameter> actualValue = value as List<EditableParameter>;
			if(actualValue == null)
				return base.GetEditValue(value);
			List<EditableParameter> result = new List<EditableParameter>();
			foreach(EditableParameter parameter in actualValue) {
				result.Add(new EditableParameter(parameter.Name, parameter.Type, parameter.Value));
			}
			return result;
		}
		#endregion
	}
	internal class SpreadsheetParametersEditorForm : CollectionEditorFormBase {
		#region Fields
		readonly List<String> incorrectParameters = new List<String>();
		readonly List<String> sameNameParameters = new List<String>();
		bool emptyParameters;
		string parametersErrorDialogTextInvalidCharacters;
		string parametersErrorDialogTextNoName;
		string parametersErrorDialogTextIdenticalNames;
		ISpreadsheetControl spreadsheetControl;
		#endregion
		public SpreadsheetParametersEditorForm(IServiceProvider provider, CollectionEditor collectionEditor, ISpreadsheetControl spreadsheetControl)
			: base(provider, collectionEditor) {
				parametersErrorDialogTextIdenticalNames = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ParametersIdenticalNames);
			parametersErrorDialogTextInvalidCharacters = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ParametersInvalidCharacters);
			parametersErrorDialogTextNoName = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ParametersNoName);
			this.spreadsheetControl = spreadsheetControl;
		}
		public override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) {
			return this.ShowDialog();
		}
		protected override bool IsValueValid() {
			incorrectParameters.Clear();
			sameNameParameters.Clear();
			emptyParameters = false;
			List<EditableParameter> parameters = EditValue as List<EditableParameter>;
			if(parameters == null)
				return true;
			for(int i = 0; i < parameters.Count; i++) {
				if(!IsThereIncorrectCharacter(parameters[i].Name))
					incorrectParameters.Add(parameters[i].Name);
				if(string.IsNullOrEmpty(parameters[i].Name))
					emptyParameters = true;
				for(int j = (i + 1); j < parameters.Count; j++) {
					if(string.Equals(parameters[i].Name, parameters[j].Name))
						sameNameParameters.Add(parameters[i].Name);
				}
			}
			return incorrectParameters.Count == 0 && !emptyParameters && sameNameParameters.Count == 0;
		}
		bool IsThereIncorrectCharacter(string parameterName) {
			if(parameterName != null) {
				foreach(char ch in parameterName) {
					if(!Data.Filtering.Helpers.CriteriaLexer.CanContinueColumn(ch))
						return false;
				}
			}
			return true;
		}
		protected override void ProcessInvalidValue() {
			string text = String.Empty;
			if(incorrectParameters.Count != 0)
				text = parametersErrorDialogTextInvalidCharacters + ConvertToString(incorrectParameters);
			else if(sameNameParameters.Count != 0)
				text = parametersErrorDialogTextIdenticalNames + ConvertToString(sameNameParameters);
			else if(emptyParameters)
				text = parametersErrorDialogTextNoName;
			spreadsheetControl.ShowWarningMessage(text);
		}
		string ConvertToString(List<String> parameters) {
			return String.Join(",", parameters);
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			if(incorrectParameters.Count != 0 || sameNameParameters.Count != 0 || emptyParameters) {
				e.Cancel = true;
				incorrectParameters.Clear();
				sameNameParameters.Clear();
				emptyParameters = false;
				return;
			}
			base.OnFormClosing(e);
		}
	}
}
