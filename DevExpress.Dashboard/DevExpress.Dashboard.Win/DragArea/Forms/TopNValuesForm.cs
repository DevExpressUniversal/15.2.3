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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public partial class TopNValuesForm : DashboardForm {
		class MeasureComboItem {
			public Measure Measure { get; private set; }
			public MeasureComboItem(Measure measure) {
				Measure = measure;
			}
			public override string ToString() {
				return Measure.DisplayName;
			}
		}
		readonly DashboardDesigner designer;
		readonly DataDashboardItem dashboardItem;
		readonly Dimension dimension;
		bool HasMeasures { get { return cbMeasure.Properties.Items.Count > 0; } }
		Measure Measure { get { return HasMeasures ? ((MeasureComboItem)cbMeasure.SelectedItem).Measure : null; } }
		bool TopNOptionsChanged {
			get {
				DimensionTopNOptions presentTopNOptions = dimension.TopNOptions;
				bool enabled = ceEnabledTopN.Checked;
				DimensionTopNMode mode = (DimensionTopNMode)cbMode.SelectedIndex;
				int count = (int)seTopValuesCount.Value;
				bool showOthers = ceShowOthersValue.Checked;
				return enabled != presentTopNOptions.Enabled ||
					mode != presentTopNOptions.Mode ||
					(presentTopNOptions.Measure != null && Measure != presentTopNOptions.Measure) ||
					count != presentTopNOptions.Count ||
					showOthers != presentTopNOptions.ShowOthers;
			}
		}
		DimensionTopNOptions TopNOptions {
			get {
				return new DimensionTopNOptions(null) { 
					Enabled = ceEnabledTopN.Checked, 
					Mode = (DimensionTopNMode)cbMode.SelectedIndex,
					Measure = Measure, 
					Count = (int)seTopValuesCount.Value, 
					ShowOthers = ceShowOthersValue.Checked 
				};
			}
		}
		public TopNValuesForm() {
			InitializeComponent();
		}
		public TopNValuesForm(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension)
			: this() {
			this.designer = designer;
			this.dashboardItem = dashboardItem;
			this.dimension = dimension;
			DashboardWinHelper.SetParentLookAndFeel(this, designer.LookAndFeel);
			InitializeCbMeasure();
			DimensionTopNOptions topNOptions = dimension.TopNOptions;
			ceEnabledTopN.Checked = topNOptions.Enabled;
			seTopValuesCount.Value = topNOptions.Count;
			ceShowOthersValue.Checked = topNOptions.ShowOthers;
			cbMode.SelectedIndex = (int)topNOptions.Mode;
			UpdateEnabledState(topNOptions.Enabled);
			if(!dashboardItem.TopNOthersValueEnabled) {
				Height -= panelShowOthersValue.Height;
				panelShowOthersValue.Visible = false;
			}
			UpdateApplyButtonEnabledState();
			SubscribeControlsEvents();
		}
		void SubscribeControlsEvents() {
			ceEnabledTopN.CheckedChanged += OnCeEnabledTopNCheckedChanged;
			cbMeasure.SelectedIndexChanged += OnPropertyChanged;
			seTopValuesCount.EditValueChanged += OnPropertyChanged;
			cbMode.SelectedIndexChanged += OnPropertyChanged;
			ceShowOthersValue.CheckedChanged += OnPropertyChanged;
			btnApply.Click += OnBtnApplyClick;
			btnOK.Click += OnBtnOKClick;
			btnCancel.Click += OnBtnCancelClick;
		}
		void UnsubscribeControlsEvents() {
			ceEnabledTopN.CheckedChanged -= OnCeEnabledTopNCheckedChanged;
			cbMeasure.SelectedIndexChanged -= OnPropertyChanged;
			seTopValuesCount.EditValueChanged -= OnPropertyChanged;
			cbMode.SelectedIndexChanged -= OnPropertyChanged;
			ceShowOthersValue.CheckedChanged -= OnPropertyChanged;
			btnApply.Click -= OnBtnApplyClick;
			btnOK.Click -= OnBtnOKClick;
			btnCancel.Click -= OnBtnCancelClick;
		}
		void InitializeCbMeasure() {
			Measure selectedMeasure = dimension.TopNOptions.Measure;
			int selectedIndex = 0;
			foreach (Measure measure in dashboardItem.UniqueMeasures) {
				MeasureComboItem item = new MeasureComboItem(measure);
				int index = cbMeasure.Properties.Items.Add(item);
				if (measure == selectedMeasure)
					selectedIndex = index;
			}
			cbMeasure.SelectedIndex = HasMeasures ? selectedIndex : -1;
		}
		void UpdateEnabledState(bool enabled) {
			seTopValuesCount.Enabled = enabled;
			ceShowOthersValue.Enabled = enabled;
			cbMode.Enabled = enabled;
			cbMeasure.Enabled = enabled;
			lblMeasure.Enabled = enabled;
			lblCount.Enabled = enabled;
			lblMode.Enabled = enabled;
			lblShowOthers.Enabled = enabled;
		}
		void UpdateApplyButtonEnabledState() {
			btnApply.Enabled = TopNOptionsChanged;
		}
		void OnPropertyChanged(object sender, EventArgs e) {
			UpdateApplyButtonEnabledState();
		}
		void OnCeEnabledTopNCheckedChanged(object sender, EventArgs e) {
			UpdateEnabledState(ceEnabledTopN.Checked);
			UpdateApplyButtonEnabledState();
		}
		void OnBtnOKClick(object sender, EventArgs e) {
			ApplyTopN();
			DialogResult = DialogResult.OK;
		}
		void OnBtnCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void OnBtnApplyClick(object sender, EventArgs e) {
			ApplyTopN();
			btnApply.Enabled = false;
		}
		void ApplyTopN() {
			DimensionTopNOptions topNOptions = TopNOptions;
			if(TopNOptionsChanged) {
				TopNHistoryItem historyItem = new TopNHistoryItem(dashboardItem, dimension, topNOptions);
				historyItem.Redo(designer);
				designer.History.Add(historyItem);
				UpdateApplyButtonEnabledState();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				UnsubscribeControlsEvents();
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
