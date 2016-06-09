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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardWin.Native {
	public partial class GridOptionsForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly GridDashboardItem gridDashboardItem;
		readonly GridColumnDragSection section;
		readonly int groupIndex;
		readonly bool allowImageMode;
		readonly DataItem dataItem;
		readonly DataFieldType dataType;
		IHolderOptionsControl activeGridColumnControl;
		IHistoryItem historyItem;
		GridColumnDragGroup Group { get { return  groupIndex >= 0 ? (GridColumnDragGroup)section.Groups[groupIndex] : (GridColumnDragGroup)section.NewGroup; } }
		bool IsNewGroup { get { return groupIndex == -1; } }
		public GridOptionsForm() {
			InitializeComponent();
		}
		public GridOptionsForm(GridColumnDragGroup group, DashboardDesigner designer, GridDashboardItem gridDashboardItem)
			: this() {
			this.designer = designer;
			this.gridDashboardItem = gridDashboardItem;
			section = (GridColumnDragSection)group.Section;
			groupIndex = group.IsNewGroup ? -1 : group.GroupIndex;
			SuspendLayout();
			try {
				GridColumnBase initialColumn = group.Holder;
				dataItem = initialColumn.DataItems.FirstOrDefault();
				dataType = dataItem != null ? dataItem.DataFieldType : DataFieldType.Unknown;
				allowImageMode = dataType == DataFieldType.Custom;
				InitializeControls(initialColumn);
				InitializeCheckEdits(group, initialColumn);
				UpdateColumnControlVisibility();
				SubscribeControlsEvents();
				btnApply.Enabled = false;
			}
			finally {
				ResumeLayout();
			}
		}
		void UpdateColumnControlVisibility() {
			UpdateColumnControlVisibility(dimensionColumnControl, dimensionCheckEdit.Checked);
			UpdateColumnControlVisibility(measureColumnControl, measureCheckEdit.Checked);
			UpdateColumnControlVisibility(deltaColumnControl, deltaCheckEdit.Checked);
			UpdateColumnControlVisibility(sparklineColumnControl, sparklineCheckEdit.Checked);
		}
		void UpdateColumnControlVisibility(Control columnControl, bool visible) {
			columnControl.Visible = visible;
			if(visible)
				activeGridColumnControl = (IHolderOptionsControl)columnControl;
		}
		bool GetAllowBarMode() {
			Measure measure = DashboardWinHelper.ConvertToMeasure(dataItem);
			DashboardWinHelper.SetCorrectSummaryType(measure, gridDashboardItem.DataSourceSchema);
			IDashboardDataSource dataSource = gridDashboardItem.DataSource;						
			return GridMeasureColumn.IsBarModeAllowed(dataType, measure, measure != null ? measure.SummaryType : SummaryType.Count, dataSource != null && dataSource.GetIsOlap());
		}
		void InitializeCheckEdits(GridColumnDragGroup group, GridColumnBase initialColumn) {
			IDashboardDataSource dataSource = gridDashboardItem.DataSource;
			if(dataSource != null) {
				if(dataSource.GetIsOlap() && !group.IsNewGroup && initialColumn.GetColumnType() != GridColumnType.Dimension)
					dimensionCheckEdit.Enabled = false;
				else if(dataSource.CalculatedFields != null && initialColumn.DataItems != null) {
					DataItem dataItem = initialColumn.DataItems.FirstOrDefault();
					if(dataItem != null) {
						CalculatedField calcField = dataSource.CalculatedFields[dataItem.DataMember];
						if(calcField != null && calcField.CheckHasAggregate(dataSource.CalculatedFields))
							dimensionCheckEdit.Enabled = false;
					}
				}
			}
			dimensionCheckEdit.Checked = dimensionColumnControl.Active && !group.AutoMode;
			measureCheckEdit.Checked = measureColumnControl.Active;
			deltaCheckEdit.Checked = deltaColumnControl.Active;
			sparklineCheckEdit.Checked = sparklineColumnControl.Active;
			autoCheckEdit.Checked = group.AutoMode;
			autoCheckEdit.Visible = IsNewGroup;
		}
		void SubscribeControlsEvents() {
			btnCancel.Click += OnCancelButtonClick;
			btnOK.Click += OnOkButtonClick;
			btnApply.Click += OnApplyButtonClick;
			dimensionCheckEdit.CheckedChanged += OnDimensionCheckEditCheckedChanged;
			measureCheckEdit.CheckedChanged += OnMeasureCheckEditCheckedChanged;
			deltaCheckEdit.CheckedChanged += OnDeltaCheckEditCheckedChanged;
			sparklineCheckEdit.CheckedChanged += OnSparklineCheckEditCheckedChanged;
			autoCheckEdit.CheckedChanged += OnAutoCheckEditCheckedChanged;
			dimensionColumnControl.OptionsChanged += OnGridColumnControlOptionsChanged;
			measureColumnControl.OptionsChanged += OnGridColumnControlOptionsChanged;
			deltaColumnControl.OptionsChanged += OnGridColumnControlOptionsChanged;
			sparklineColumnControl.OptionsChanged += OnGridColumnControlOptionsChanged;
		}
		void InitializeControls(GridColumnBase initialColumn) {
			dimensionColumnControl.Initialize(section, groupIndex, gridDashboardItem, initialColumn, allowImageMode);
			measureColumnControl.Initialize(section, groupIndex, gridDashboardItem, initialColumn);
			deltaColumnControl.Initialize(section, groupIndex, gridDashboardItem, initialColumn);
			sparklineColumnControl.Initialize(section, groupIndex, gridDashboardItem, initialColumn);
		}
		void OnDimensionCheckEditCheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			bool isDimensionMode = dimensionCheckEdit.Checked;
			dimensionColumnControl.Visible = isDimensionMode;
			if (isDimensionMode)
				activeGridColumnControl = dimensionColumnControl;
			EnableApplyButton();
			ResumeLayout();
		}
		void OnMeasureCheckEditCheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			measureColumnControl.SetAllowBarMode(GetAllowBarMode());
			bool isMeasureMode = measureCheckEdit.Checked;
			measureColumnControl.Visible = isMeasureMode;
			if(isMeasureMode)
				activeGridColumnControl = measureColumnControl;
			EnableApplyButton();
			ResumeLayout();
		}
		void OnDeltaCheckEditCheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			deltaColumnControl.SetAllowBarMode(GetAllowBarMode());
			bool isDeltaMode = deltaCheckEdit.Checked;
			deltaColumnControl.Visible = isDeltaMode;
			if(isDeltaMode)
				activeGridColumnControl = deltaColumnControl;
			EnableApplyButton();
			ResumeLayout();
		}
		void OnSparklineCheckEditCheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			bool isSparklineMode = sparklineCheckEdit.Checked;
			sparklineColumnControl.Visible = isSparklineMode;
			if(isSparklineMode)
				activeGridColumnControl = sparklineColumnControl;
			EnableApplyButton();
			ResumeLayout();
		}
		void OnGridColumnControlOptionsChanged(object sender, EventArgs e) {
			EnableApplyButton();
		}
		void EnableApplyButton() {
			btnApply.Enabled = true;
		}
		void ApplyChanges() {
			DragAreaControl dragArea = section.Area.ParentControl as DragAreaControl;
			dragArea.BeginUpdate();
			try {
				dimensionColumnControl.AutoMode = autoCheckEdit.Checked;
				historyItem = activeGridColumnControl.CreateHistoryItem();
				historyItem.Redo(designer);
				if(!IsNewGroup)
					designer.History.Add(historyItem);
				InitializeControls(Group.Holder);
			}
			finally {
				dragArea.EndUpdate();
			}
		}
		void OnOkButtonClick(object sender, EventArgs e) {
			dimensionColumnControl.AutoMode = autoCheckEdit.Checked;
			if (btnApply.Enabled)
				ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		void OnCancelButtonClick(object sender, EventArgs e) {
			dimensionColumnControl.AutoMode = autoCheckEdit.Checked;
			DialogResult = DialogResult.Cancel;
		}
		void OnApplyButtonClick(object sender, EventArgs e) {
			ApplyChanges();
			btnApply.Enabled = false;
		}
		void OnAutoCheckEditCheckedChanged(object sender, EventArgs e) {
			SuspendLayout();
			bool isAutoMode = autoCheckEdit.Checked;
			dimensionColumnControl.Visible = isAutoMode;
			if (isAutoMode) 
				activeGridColumnControl = dimensionColumnControl;
			EnableApplyButton();
			ResumeLayout();
		}
	}
}
