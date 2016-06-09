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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Native;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraCharts.Designer {
	public partial class ChartDataControl : XtraUserControl {
		DesignerChartModel chartModel;
		IServiceProvider serviceProvider;
		DesignerChartElementModelBase selectedmodel;
		string currentDataMember = String.Empty;
		bool lockDataSourceChange = false;
		bool isDesignTime = false;
		internal DesignerChartModel ChartModel {
			get { return chartModel; }
			set {
				chartModel = value;
				OnChartModelChanged();
			}
		}
		internal IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set { serviceProvider = value; }
		}
		internal DesignerChartElementModelBase SelectedModel {
			get { return selectedmodel; }
			set {
				if (selectedmodel != value) {
					selectedmodel = value;
					UpdateControls();
				}
			}
		}
		internal bool IsDesignTime {
			get { return isDesignTime; }
			set { 
				isDesignTime = value;
				cbChooseDataSource.Enabled = isDesignTime;
			}
		}
		public ChartDataControl() {
			InitializeComponent();
			UpdateArrowColor();
			this.Disposed += ChartDataControl_Disposed;
		}
		void ChartDataControl_Disposed(object sender, EventArgs e) {
			this.Disposed -= ChartDataControl_Disposed;
			ClearDataMembersControls();
		}
		void UpdateArrowColor() {
			Bitmap arrowBitmap = (Bitmap)lblArrow.Appearance.Image;
			Color color = CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.GrayText);
			for (int x = 0; x < arrowBitmap.Width; x++)
				for (int y = 0; y < arrowBitmap.Height; y++)
					if (arrowBitmap.GetPixel(x, y).A != 0)
						arrowBitmap.SetPixel(x, y, color);
		}
		void OnChartModelChanged() {
			if(chartModel != null)
				WizardDataBindingHelper.UpdateDataSourceComboBox(cbChooseDataSource, ((Chart)chartModel.ChartElement).Container, serviceProvider, chartModel.Chart.DataContainer.ActualDataSource);
			UpdateControls();
		}
		void OnChooseDataSourceSelectedIndexChanged(object sender, EventArgs e) {
			if (!lockDataSourceChange) {
				DataSourceComboBoxItem selectedItem = ((ComboBoxEdit)sender).SelectedItem as DataSourceComboBoxItem;
				if (selectedItem != null) {
					StartDataMemberPicker(selectedItem.DataSource, currentDataMember);
					SetObjectDataSource(selectedItem.DataSource);
				}
			}
		}
		void UpdateDataSourceComboBoxState(object dataSource) {
			lockDataSourceChange = true;
			foreach(DataSourceComboBoxItem item in cbChooseDataSource.Properties.Items)
				if (item.DataSource == dataSource) {
					cbChooseDataSource.SelectedItem = item;
					break;
				}
			lockDataSourceChange = false;
		}
		void SetObjectDataSource(object dataSource) {
			if (selectedmodel is DesignerSeriesModel)
				((DesignerSeriesModel)selectedmodel).DataSource = dataSource;
			else if (chartModel.DataContainer.DataSource != dataSource && (selectedmodel is DesignerChartModel || selectedmodel is DesignerSeriesModelBase))
				chartModel.DataContainer.DataSource = dataSource;
		}
		void StartDataMemberPicker(object dataSource, string dataMember) {
			dataMemberPicker.SetServiceProvider(serviceProvider);
			dataMemberPicker.FillDataSource(dataSource, dataMember);
			dataMemberPicker.Start();
			dataMemberPicker.SelectNoneDataMember();
		}
		void ClearDataMembersControls() {
			foreach (Control control in pnlDataMembers.Controls) {
				DropTargetControl dataMemberControl = control as DropTargetControl;
				if (dataMemberControl != null) {
					dataMemberControl.DataDropped -= OnDataMemberControlDataDropped;
					dataMemberControl.AllowDrop = false;
				}
			}
			pnlDataMembers.Controls.Clear();
		}
		void OnDataMemberControlDataDropped(object sender, DataDroppedEventArgs e) {
			DropTargetControl control = sender as DropTargetControl;
			string dataMember = BindingHelper.ExtractDataMember(e.DataMemberName, currentDataMember);
			string selectedDataMemberCaption = " (none)";
			if (!string.IsNullOrEmpty(dataMember))
				selectedDataMemberCaption = " (" + dataMember + ")";
			control.Text = control.DataMemberInfo.Caption + selectedDataMemberCaption;
			selectedmodel.SetDataMember(((DropTargetControl)sender).DataMemberInfo, dataMember);
		}
		DropTargetControl GetDataMemberControl(DataMemberInfo dataMemberInfo, object dataSource) {
			DropTargetControl dataMemberControl = new DropTargetControl();
			dataMemberControl.DataMemberInfo = dataMemberInfo;
			dataMemberControl.DataSource = dataSource;
			string selectedDataMemberCaption = " (none)";
			if (!string.IsNullOrEmpty(dataMemberInfo.SelectedDataMember))
				selectedDataMemberCaption = " (" + dataMemberInfo.SelectedDataMember + ")";
			dataMemberControl.Text = dataMemberInfo.Caption + selectedDataMemberCaption;
			dataMemberControl.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			dataMemberControl.AppearanceHovered.TextOptions.HAlignment = HorzAlignment.Center;
			dataMemberControl.AppearancePressed.TextOptions.HAlignment = HorzAlignment.Center;
			dataMemberControl.Dock = DockStyle.Bottom;
			dataMemberControl.AutoSizeMode = LabelAutoSizeMode.None;
			dataMemberControl.AllowDrop = true;
			dataMemberControl.Size = new Size(100, 35);
			dataMemberControl.BackColor = Color.Transparent;
			dataMemberControl.DataDropped += OnDataMemberControlDataDropped;
			return dataMemberControl;
		}
		void UpdateControls() {
			cbChooseDataSource.Enabled = IsDesignTime;
			pnlDataMembers.SuspendLayout();
			ClearDataMembersControls();
			if (selectedmodel != null && chartModel != null) {
				List<DataMemberInfo> dataMembersInfo = selectedmodel.GetDataMembersInfo();
				if (dataMembersInfo != null) {
					object dataSource = null;
					if (selectedmodel is DesignerSeriesModel) {
						dataSource = SeriesDataBindingUtils.GetDataSource(((DesignerSeriesModel)selectedmodel).Series);
						currentDataMember = SeriesDataBindingUtils.GetDataMember(((DesignerSeriesModel)selectedmodel).Series);
					}
					else if (selectedmodel is DesignerChartModel || selectedmodel is DesignerSeriesModelBase) {
						dataSource = chartModel.DataContainer.DataSource;
						if (selectedmodel is DesignerChartModel)
							currentDataMember = chartModel.DataContainer.DataMember;
						else
							currentDataMember = SeriesDataBindingUtils.GetDataMember(((DesignerSeriesModelBase)selectedmodel).SeriesBase);
					}
					UpdateDataSourceComboBoxState(dataSource);
					StartDataMemberPicker(dataSource, currentDataMember);
					foreach (DataMemberInfo dataMemberInfo in dataMembersInfo) {
						DropTargetControl control = GetDataMemberControl(dataMemberInfo, dataSource);
						pnlDataMembers.Controls.Add(control);
					}
				}
			}
			pnlDataMembers.ResumeLayout();
		}
		public void UpdateState() {
			if (selectedmodel != null && chartModel != null) {
				List<DataMemberInfo> dataMembersInfo = selectedmodel.GetDataMembersInfo();
				if (dataMembersInfo != null) {
					if (dataMembersInfo.Count != pnlDataMembers.Controls.Count)
						UpdateControls();
					else {
						int i = 0;
						foreach (DropTargetControl control in pnlDataMembers.Controls) {
							DataMemberInfo dataMemberInfo = dataMembersInfo[i];
							control.DataMemberInfo = dataMemberInfo;
							string selectedDataMemberCaption = " (none)";
							if (!string.IsNullOrEmpty(dataMemberInfo.SelectedDataMember))
								selectedDataMemberCaption = " (" + dataMemberInfo.SelectedDataMember + ")";
							control.Text = dataMemberInfo.Caption + selectedDataMemberCaption;
							i++;
						}
					}
				}
			}
		}
	}
	public class DraggableDataMemberPicker : DataMemberPicker {
		public DraggableDataMemberPicker() : base() {
			TreeView.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Single;
			TreeView.DragOver += treeView_DragOver;
			TreeView.CalcNodeDragImageIndex += treeView_CalcNodeDragImageIndex;
			TreeView.BeforeDragNode += treeView_BeforeDragNode;
			TreeView.BorderStyle = XtraEditors.Controls.BorderStyles.Default;
			TreeView.GiveFeedback += TreeView_GiveFeedback;
		}
		void TreeView_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			if (e.Effect == DragDropEffects.Link) {
				e.UseDefaultCursors = false;
				Cursor.Current = ResLoaderBase.LoadColoredCursor("ChartDesigner.Images.DataControl.Apply.cur", typeof(ChartWizard));
			}
			else if (e.Effect == DragDropEffects.None) {
				e.UseDefaultCursors = false;
				Cursor.Current = Cursors.No;
			}
		}
		void treeView_DragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
		}
		void treeView_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e) {
			e.ImageIndex = -1;
		}
		void treeView_BeforeDragNode(object sender, BeforeDragNodeEventArgs e) {
			e.CanDrag = e.Node.Nodes.Count == 0;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			TreeView.OptionsDragAndDrop.DragNodesMode = DragNodesMode.None;
		}
	}
	public class DataMemberInfo {
		readonly string propertyName;
		readonly string caption;
		readonly string selectedDataMember;
		readonly ScaleType[] allowedScaleTypes;
		public string PropertyName { get { return propertyName; } }
		public string Caption { get { return caption; } }
		public string SelectedDataMember { get { return selectedDataMember; } }
		public ScaleType[] AllowedScaleTypes { get { return allowedScaleTypes; } }
		public DataMemberInfo(string propertyName, string caption, string selectedDataMember, ScaleType[] allowedScaleTypes) {
			this.propertyName = propertyName;
			this.caption = caption;
			this.selectedDataMember = selectedDataMember;
			this.allowedScaleTypes = allowedScaleTypes;
		}
	}
}
