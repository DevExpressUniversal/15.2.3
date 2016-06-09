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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public partial class SummaryFunctionBindingControl : XtraUserControl {
		readonly string noneString;
		Chart chart;
		SeriesBase series;
		object dataSource;
		string chartDataMember;
		SummaryFunctionParser parser;
		bool lockChanging = false;
		public SummaryFunctionBindingControl() {
			InitializeComponent();
			noneString = ChartLocalizer.GetString(ChartStringId.WizDataMemberNoneString);
		}
		public void Initialize(SeriesBase series, Chart chart, bool lockFunctionNameEditing, IServiceProvider provider) { 
			this.series = series;
			this.chart = chart;
			this.tvAvailable.SetServiceProvider(provider);
			dataSource = SeriesDataBindingUtils.GetDataSource(series);
			chartDataMember = SeriesDataBindingUtils.GetDataMember(series);
			parser = new SummaryFunctionParser(series.SummaryFunction);
			Text = ChartLocalizer.GetString(ChartStringId.TitleSummaryFunction);
			cbFunctionName.Properties.Items.Add(noneString);
			SummaryFunctionsStorage summaryFunctions = CommonUtils.GetSummaryFunctions(chart);
			foreach (SummaryFunctionDescription desc in summaryFunctions)
				if (desc.ResultScaleType == null || desc.ResultScaleType.Value == series.ValueScaleType)
					cbFunctionName.Properties.Items.Add(desc);
			if (String.IsNullOrEmpty(parser.FunctionName))
				cbFunctionName.SelectedIndex = 0;
			else {
				SummaryFunctionDescription description = summaryFunctions[parser.FunctionName];
				cbFunctionName.EditValue = description == null ? (object)parser.FunctionName : (object)description;
			}
			if (lockFunctionNameEditing) {
				cbFunctionName.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
				annotationPanel.Visible = false;
			}
		}
		public void ApplyChanges() {
			series.SummaryFunction = parser.GetSummaryFunction();
		}
		void UpdateButtonsState() {
			btnRemoveParameter.Enabled = parser.Arguments.Length > 0;
		}
		void FillDataSource(ScaleType? scaleType) {
			if (scaleType != null && !BindingHelper.AreAvailableFieldsPresent(dataSource, scaleType.Value)) {
				lblError.Text = String.Format(lblError.Text, ScaleTypeUtils.GetName(scaleType.Value));
				panelError.Visible = true;				
			}
			tvAvailable.Stop();
			ScaleType[] validScaleTypes = scaleType == null ? new ScaleType[0] : new ScaleType[] { scaleType.Value };
			tvAvailable.SetFilterCriteria(validScaleTypes, false);
			tvAvailable.FillDataSource(dataSource, chartDataMember);
			tvAvailable.Start();
		}
		string GetDisplayDataMember(string dataMember) {
			IChartDataProvider dataProvider = chart.Container.DataProvider;
			return String.IsNullOrEmpty(dataMember) ? noneString : BindingHelper.GetDataMemberName(dataProvider != null ? dataProvider.DataContext : null, dataSource, String.Empty, dataMember);
		}
		ListViewItem CreateListViewItem(string itemText, string dataMember) {
			ListViewItem lvItem = new ListViewItem(new string[] { itemText, GetDisplayDataMember(dataMember) });
			lvItem.SubItems[1].Tag = dataMember;
			return lvItem;
		}
		void UpdateControlsWithSummaryFunction() {
			SuspendLayout();
			try {
				lvSelected.Items.Clear();
				tvAvailable.Clear();
				panelError.Visible = false;
				if (String.IsNullOrEmpty(parser.FunctionName))
					panelButtons.Visible = false;
				else {
					SummaryFunctionsStorage summaryFunctions = CommonUtils.GetSummaryFunctions(chart);
					SummaryFunctionDescription description = summaryFunctions[parser.FunctionName];
					int parsedArgumentsCount = parser.Arguments.Length;
					string argumentPrefix = ChartLocalizer.GetString(ChartStringId.ArgumentMember);
					for (int i = 0; i < parsedArgumentsCount; i++) {
						string argumentString = description == null ? (argumentPrefix + (i + 1).ToString()) : description.ArgumentDescriptions[i].Name;
						lvSelected.Items.Add(CreateListViewItem(argumentString, parser.Arguments[i]));
					}
					ScaleType? parametersScaleType = null;
					if (description == null) {
						panelButtons.Visible = true;
						UpdateButtonsState();
					}
					else {
						if (description.ResultScaleType == null && description.IsStandard)
							parametersScaleType = series.ValueScaleType;
						else
							foreach (SummaryFunctionArgumentDescription desc in description.ArgumentDescriptions)
								if (desc.ScaleType != null)
									if (parametersScaleType == null)
										parametersScaleType = desc.ScaleType.Value;
									else if (parametersScaleType.Value != desc.ScaleType.Value) {
										parametersScaleType = null;
										break;
									}
						panelButtons.Visible = false;
					}
					if (parsedArgumentsCount > 0)
						FillDataSource(parametersScaleType);
				}
				if (lvSelected.Items.Count > 0)
					lvSelected.Items[0].Selected = true;
			}
			finally {
				ResumeLayout();
			}
		}
		string GetActualSummaryFunctionName(string name) {
			name = name.Trim();
			return name == noneString ? String.Empty : name;
		}
		void cbFunctionName_Properties_EditValueChanging(object sender, ChangingEventArgs e) {
			SummaryFunctionDescription description = e.NewValue as SummaryFunctionDescription;
			string newFunctionName = description != null ? description.Name : e.NewValue.ToString();
			SummaryFunctionsStorage summaryFunctions = CommonUtils.GetSummaryFunctions(chart);
			if (newFunctionName != noneString && !parser.UpdateFunctionName(GetActualSummaryFunctionName(newFunctionName), summaryFunctions)) {
				int selStart = cbFunctionName.SelectionStart;
				int selLen = cbFunctionName.SelectionLength;
				e.Cancel = true;
				BeginInvoke(new MethodInvoker(delegate() { cbFunctionName.Select(selStart, selLen); }));
			}
		}
		void cbFunctionName_Properties_EditValueChanged(object sender, EventArgs e) {
			SummaryFunctionDescription description = cbFunctionName.EditValue as SummaryFunctionDescription;
			string newFunctionName = description != null ? description.Name : cbFunctionName.EditValue.ToString();
			newFunctionName = newFunctionName == noneString ? String.Empty : GetActualSummaryFunctionName(newFunctionName);
			SummaryFunctionsStorage summaryFunctions = CommonUtils.GetSummaryFunctions(chart);
			parser.UpdateFunctionName(newFunctionName, summaryFunctions);
			UpdateControlsWithSummaryFunction();
		}
		void lvSelected_SizeChanged(object sender, EventArgs e) {
			lvSelected.Columns[1].Width = lvSelected.Size.Width - lvSelected.Columns[0].Width;
		}
		void lvSelected_SelectedIndexChanged(object sender, EventArgs e) {
			if (lvSelected.SelectedIndices.Count > 0) {
				lockChanging = true;
				string dataMember = (string)lvSelected.SelectedItems[0].SubItems[1].Tag;
				if (String.IsNullOrEmpty(dataMember))
					tvAvailable.SelectNoneDataMember();
				else
					tvAvailable.SelectDataMember(dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember));
				lockChanging = false;
			}
		}
		void btnAddParameter_Click(object sender, EventArgs e) {
			int selectedIndex = lvSelected.SelectedIndices.Count > 0 ? lvSelected.SelectedIndices[0] : -1;
			parser.AddNewArgument();
			UpdateControlsWithSummaryFunction();
			if (selectedIndex > 0)
				lvSelected.Items[selectedIndex].Selected = true; 
		}
		void btnRemoveParameter_Click(object sender, EventArgs e) {
			if (lvSelected.SelectedIndices.Count > 0) {
				int index = lvSelected.SelectedIndices[0];
				parser.RemoveArgument(index);
				UpdateControlsWithSummaryFunction();
				if (index >= lvSelected.Items.Count)
					index--;
				if (index >= 0)
					lvSelected.Items[index].Selected = true;
			}
		}
		void tvAvailable_SelectionChanged(object sender, EventArgs e) {
			if (!lockChanging && lvSelected.SelectedIndices.Count > 0) {
				string dataMember;
				if (tvAvailable.IsDataMemberNode) {
					dataMember = tvAvailable.DataMember;
					if (!String.IsNullOrEmpty(chartDataMember) && dataMember.StartsWith(chartDataMember + '.'))
						dataMember = dataMember.Substring(chartDataMember.Length + 1);
				}
				else
					dataMember = String.Empty;
				parser.Arguments[lvSelected.SelectedIndices[0]] = dataMember;
				ListViewItem.ListViewSubItem lvItem = lvSelected.SelectedItems[0].SubItems[1];
				lvItem.Text = GetDisplayDataMember(dataMember);
				lvItem.Tag = dataMember;
			}
		}
	}
}
