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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Drawing;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class ParametersEditor : UserControl {		
		List<ParameterEditorLine> parameterLines;
		public List<ParameterEditorLine> ParameterLines { get { return parameterLines; } }
		public bool DiffersFromDefault {
			get {
				foreach(ParameterEditorLine line in parameterLines) {
					if(line.Value != null && line.ViewModel.DefaultValue != null) {
						if(line.Value.ToString() != line.ViewModel.DefaultValue.ToString())
							return true;
					}
					else if(!object.ReferenceEquals(line.Value, line.ViewModel.DefaultValue))
							return true;
				}
				return false;
			}
		}
		public int ExpectedHeight { get {
			BaseViewInfo viewInfo = parametersEditorGridView.GetViewInfo();
			return viewInfo.CalcRealViewHeight(new Rectangle(parametersEditorGridControl.Location.X, parametersEditorGridControl.Location.Y, parametersEditorGridControl.Width, 10000))
				+ panel1.Height;						
		} }
		public int ParametersGridWidth { get { return gridColumnParameterName.Width + gridColumnValue.Width + (gridColumnPassNull != null ? gridColumnPassNull.Width : 0); } }			
		public IList<IParameter> ActualParameters { get { return GetActualParameters(); } }
		public event EventHandler ValueChanged;
		public ParametersEditor() {
			InitializeComponent();
		}
		public void Init(DataSourceCollection dataSources, IList<DashboardParameterViewModel> parameters, IEnumerable<IParameter> actualParameters) {
			Init(dataSources, parameters, actualParameters, null);
		}
		public void Init(DataSourceCollection dataSources, IList<DashboardParameterViewModel> parameters, IEnumerable<IParameter> actualParameters, UserLookAndFeel parentLookAndFeel ) {
			CreateParameterEditorLines(parameters, actualParameters);
			parametersEditorGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.BackColor =  CommonSkins.GetSkin(parentLookAndFeel).Colors.GetColor("Control");
			parametersEditorGridControl.DataSource = parameterLines;
			parametersEditorGridView.CustomRowCellEdit += ParametersEditorGridViewOnCustomRowCellEdit;		  
			if(parameters.Any(p => p.AllowNull))
				CreateAllowNullColumn(parentLookAndFeel);
		}
		public void Reset() {
			foreach(ParameterEditorLine line in parameterLines)
				line.Value = line.ViewModel.DefaultValue;
			parametersEditorGridView.RefreshData();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				foreach(ParameterEditorLine editorLine in parameterLines) {
					editorLine.ValueChanged -= RaiseValueChanged;
					editorLine.Dispose();
				}
				parameterLines.Clear();
			}
			base.Dispose(disposing);
		}
		void CreateAllowNullColumn(UserLookAndFeel parentLookAndFeel) {
			gridColumnPassNull = new DevExpress.XtraGrid.Columns.GridColumn();
			parametersEditorGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumnPassNull });
			gridColumnPassNull.AppearanceCell.Options.UseTextOptions = true;
			gridColumnPassNull.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			gridColumnPassNull.Caption = "Pass Null";
			gridColumnPassNull.FieldName = "IsNull";
			gridColumnPassNull.Name = "gridColumnPassAllowNull";
			gridColumnPassNull.AppearanceCell.ForeColor = CommonSkins.GetSkin(parentLookAndFeel).Colors.GetColor("DisabledText");
			gridColumnPassNull.AppearanceCell.Options.UseForeColor = true;
			gridColumnPassNull.AppearanceCell.Options.UseTextOptions = true;
			gridColumnPassNull.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			gridColumnPassNull.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			gridColumnPassNull.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
			gridColumnPassNull.OptionsColumn.AllowMove = false;
			gridColumnPassNull.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
			gridColumnPassNull.OptionsFilter.AllowAutoFilter = false;
			gridColumnPassNull.OptionsFilter.AllowFilter = false;
			gridColumnPassNull.Visible = true;
			gridColumnPassNull.VisibleIndex = 2;
			gridColumnPassNull.Width = 125;
			gridColumnValue.Width = 125;
			gridColumnParameterName.Width = 250;			
		}
		void CreateParameterEditorLines(IList<DashboardParameterViewModel> parameterViewModels, IEnumerable<IParameter> actualParameters) {
			parameterLines = new List<ParameterEditorLine>();
			foreach(DashboardParameterViewModel viewModel in parameterViewModels){
				if(!viewModel.Visible)
					continue;
				IParameter actualParameter = actualParameters.FirstOrDefault(p => p.Name == viewModel.Name);
				if(actualParameter == null)
					continue;
				ParameterEditorLine line = new ParameterEditorLine(viewModel, actualParameter.Value, parametersEditorGridView, parametersEditorGridControl);
				line.ValueChanged += RaiseValueChanged;
				parameterLines.Add(line);
			}
		}
		void ParametersEditorGridViewOnCustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e) {
			if (e.Column == gridColumnValue) 
				e.RepositoryItem = parameterLines[e.RowHandle].IsNull.ToString()=="True" ? repositoryItemTextEdit : parameterLines[e.RowHandle].RepositoryItemValue;				
			else if (e.Column == gridColumnPassNull)
				e.RepositoryItem = parameterLines[e.RowHandle].ViewModel.AllowNull ? (RepositoryItem)repositoryItemCheckEdit : repositoryItemTextEdit; 
		}
		IList<IParameter> GetActualParameters() {
			IList<IParameter> list = new List<IParameter>();
			foreach(ParameterEditorLine line in parameterLines) 
				list.Add(line.GetActualParameter());							
			return list;
		}
		void RepositoryItemCheckEditOnCheckedChanged(object sender, EventArgs e) {
			 parametersEditorGridView.CloseEditor();
		}
		void RaiseValueChanged(object sender, EventArgs e) {
			 if(ValueChanged != null)
				 ValueChanged(this, EventArgs.Empty);
		 }
		private void parametersEditorGridControl_Click(object sender, EventArgs e) {
		}
		private void panel1_Paint(object sender, PaintEventArgs e) {
		}
	}
}
