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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.LookAndFeel;
using DevExpress.DashboardCommon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Skins;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public partial class NewColorTableDialog : DashboardForm {
		ColorRepositoryKey colorRepositoryKey;
		IList<DataSourceInfo> dataSources;
		IServiceProvider serviceProvider;
		DataFieldsBrowserPresenter browserPresenter;
		IEnumerable<ColorRepositoryKey> existingKeys;
		List<DimensionDefinitionDataSourceRow> dimensionDefinitions = new List<DimensionDefinitionDataSourceRow>();
		public ColorRepositoryKey ColorRepositoryKey { get { return colorRepositoryKey; } }
		public NewColorTableDialog() {
			InitializeComponent();
		}
		public NewColorTableDialog(UserLookAndFeel lookAndFeel, IList<DataSourceInfo> dataSources, IServiceProvider serviceProvider, IEnumerable<ColorRepositoryKey> existingKeys)
			: base(lookAndFeel) {
			InitializeComponent();
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.dataSources = dataSources;
			this.serviceProvider = serviceProvider;
			this.existingKeys = existingKeys;
			IDataFieldsBrowserPresenterFactory factory = serviceProvider.RequestServiceStrictly<IDataFieldsBrowserPresenterFactory>();
			browserPresenter = factory.CreatePresenter(dataFieldsBrowser1, null, serviceProvider);
			browserPresenter.DisplayMode = DataFieldsBrowserDisplayMode.Dimensions;
			browserPresenter.DataFieldDoubleClick += OnDataFieldDoubleClick;
			browserPresenter.FocusedDataFieldChanged += OnFocusedDataFieldChanged;
			InitializeEditors();
			lbDimensionDefinitions.DisplayMember = "DisplayText";
			lbDimensionDefinitions.ValueMember = "Definition";
			lbDimensionDefinitions.DataSource = dimensionDefinitions;
			LoadImages();
			dataFieldsBrowser1.SetToolbarVisibility(false);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
				if (browserPresenter != null)
					browserPresenter.Dispose();
			}
			base.Dispose(disposing);
		}
		void LoadImages() {
			SkinElement skinElement = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinButton];
			Color imageColor = skinElement.Color.GetForeColor();
			Image addImage = ImageHelper.GetImage("AddToList");
			btnAdd.Image = ImageHelper.ColorBitmap(addImage, imageColor);
			Image removeImage = ImageHelper.GetImage("RemoveFromList");
			btnRemove.Image = ImageHelper.ColorBitmap(removeImage, imageColor);
		}
		void InitializeEditors() {
			if(dataSources != null && dataSources.Count > 0) {
				ceDataSource.Properties.Items.AddRange(dataSources.ToArray());
				IDataSourceSelectionService service = serviceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
				ceDataSource.SelectedItem = service.SelectedDataSourceInfo;
				ceIncludeMeasureNames.Enabled = true;
			}
			cbDateTimeGroupInterval.Properties.Items.AddEnum<DateTimeGroupInterval>(value => GroupIntervalCaptionProvider.GetDateTimeGroupIntervalCaption(value));
			cbDateTimeGroupInterval.SelectedIndex = 0;
			cbTextGroupInterval.Properties.Items.AddEnum<TextGroupInterval>(value => GroupIntervalCaptionProvider.GetTextGroupIntervalCaption(value));
			cbTextGroupInterval.SelectedIndex = 0;
		}
		List<string> GetDataMembers() {
			DataField field = browserPresenter.SelectedDataField;
			if(field != null && field.IsDataMemberNode) {
				if(field.NodeType == DataNodeType.OlapHierarchy) {
					OlapHierarchyDataField olapHierarchyDataField = field as OlapHierarchyDataField;
					if(olapHierarchyDataField != null) {
						return olapHierarchyDataField.GroupDataMembers;
					}
				}
				else {
					string dataMember = field.DataMember;
					if(!string.IsNullOrEmpty(dataMember)) {
						return new List<string>() { dataMember };
					}
				}
			}
			return null;
		}
		void GenerateRepositoryKey() {
			colorRepositoryKey = new ColorRepositoryKey();
			foreach(DimensionDefinition definition in GetDefinitions()) {
				colorRepositoryKey.DimensionDefinitions.Add(definition);
			}
			if(ceIncludeMeasureNames.Checked)
				colorRepositoryKey.DimensionDefinitions.Add(DimensionDefinition.MeasureNamesDefinition);
			DataSourceInfo dataSourceInfo = (DataSourceInfo)ceDataSource.SelectedItem;
			colorRepositoryKey.DataSourceName = dataSourceInfo.DataSource.ComponentName;
			colorRepositoryKey.DataMember = dataSourceInfo.DataMember;
		}
		void OnBtnAddClick(object sender, EventArgs e) {
			AddDimensionDefinitions();
		}
		void AddDimensionDefinitions() {
			List<string> dataMembers = GetDataMembers();
			if(dataMembers != null) {
				DataSourceInfo dataSourceInfo = (DataSourceInfo)ceDataSource.SelectedItem;
				IEnumerable<DimensionDefinition> definitions = dataMembers.Select(
					dataMember => new DimensionDefinition(dataMember));
				dimensionDefinitions.AddRange(definitions.Select(definition => new DimensionDefinitionDataSourceRow() {
					Definition = definition,
					DisplayText = DataItemDefinitionDisplayTextProvider.GetDimensionDefinitionDisplayText(definition, dataSourceInfo.GetPickManager())
				}));
				UpdateDefinitionListBox();
				lbDimensionDefinitions.SelectedIndex = lbDimensionDefinitions.ItemCount - 1;
			}
		}
		void OnDefinitionRemoved() {
			UpdateDefinitionListBox();
			UpdateGroupIntervalEditors();
		}
		void OnBtnRemoveClick(object sender, EventArgs e) {
			int selectedIndex = lbDimensionDefinitions.SelectedIndex;
			if(selectedIndex >= 0) {
				dimensionDefinitions.RemoveAt(selectedIndex);
				OnDefinitionRemoved();
			}
		}
		List<DimensionDefinition> GetDefinitions() {
			return dimensionDefinitions.Select(row => row.Definition).ToList();
		}
		void ceDataSource_EditValueChanging(object sender, ChangingEventArgs e) {
			if(dimensionDefinitions.Count > 0) { 
				DialogResult result = XtraMessageBox.Show(LookAndFeel,
					DashboardWinLocalizer.GetString(DashboardWinStringId.MessageNewColorTableDialogSwitchingDataSource),
					DashboardWinLocalizer.GetString(DashboardWinStringId.MessageNewColorTableDialogSwitchingDataSourceCaption),
					MessageBoxButtons.YesNo);
				if(result == System.Windows.Forms.DialogResult.Yes) {
					dimensionDefinitions.Clear();
					OnDefinitionRemoved();
				}
				else
					e.Cancel = true;
			}
		}
		void ceDataSource_EditValueChanged(object sender, EventArgs e) {
			browserPresenter.DataSourceInfo = (DataSourceInfo)ceDataSource.SelectedItem;
			UpdateGroupIntervalEditors();
		}
		void OnFormClosing(object sender, FormClosingEventArgs e) {
			if(DialogResult == DialogResult.OK) {
				GenerateRepositoryKey();
				if(existingKeys.Contains(colorRepositoryKey, new ColorRepositoryKeyComparer())) {
					XtraMessageBox.Show(LookAndFeel,
						this,
						DashboardWinLocalizer.GetString(DashboardWinStringId.MessageColorTableAlreadyExists),
						DashboardWinLocalizer.GetString(DashboardWinStringId.MessageColorTableAlreadyExistsCaption),
						MessageBoxButtons.OK);
					e.Cancel = true;					
				}
			}
		}
		void UpdateGroupIntervalEditors() {
			DataSourceInfo selectedDataSource = ceDataSource.SelectedItem as DataSourceInfo;
			if(selectedDataSource != null) {
				IDashboardDataSource dataSource = selectedDataSource.DataSource;				
				if(!dataSource.GetIsOlap()) {
					DataFieldType dataFieldType = DataFieldType.Unknown;
					int selectedIndex = lbDimensionDefinitions.SelectedIndex;
					if(selectedIndex >= 0) {
						DimensionDefinition definition = dimensionDefinitions[selectedIndex].Definition;
						dataFieldType = selectedDataSource.GetFieldType(definition.DataMember);
						cbDateTimeGroupInterval.EditValue = definition.DateTimeGroupInterval;
						cbTextGroupInterval.EditValue = definition.TextGroupInterval;
						switch(dataFieldType) {
							case DataFieldType.DateTime:
								cbDateTimeGroupInterval.Enabled = true;
								cbTextGroupInterval.Enabled = false;
								break;
							case DataFieldType.Text:
								cbDateTimeGroupInterval.Enabled = false;
								cbTextGroupInterval.Enabled = true;
								break;
							case DataFieldType.Unknown:
								cbDateTimeGroupInterval.Enabled = true;
								cbTextGroupInterval.Enabled = true;
								break;
							default:
								cbDateTimeGroupInterval.Enabled = false;
								cbTextGroupInterval.Enabled = false;
								break;
						}
						return;
					}
				}
				cbDateTimeGroupInterval.Enabled = false;
				cbTextGroupInterval.Enabled = false;
			}
		}
		void UpdateDefinitionListBox() {
			lbDimensionDefinitions.Refresh();
			UpdateDefinitionListBoxSelection();
			UpdateRemoveButtonState();
		}
		void UpdateDefinitionListBoxSelection() {
			int selectedIndex = lbDimensionDefinitions.SelectedIndex;
			if(selectedIndex > dimensionDefinitions.Count - 1) 
				selectedIndex -= 1;
			if(selectedIndex < 0 && dimensionDefinitions.Count > 0)
				selectedIndex = 0;
			lbDimensionDefinitions.SelectedIndex = selectedIndex;
		}
		void UpdateRemoveButtonState() {
			btnRemove.Enabled = dimensionDefinitions.Count > 0 && lbDimensionDefinitions.SelectedIndex >= 0;
		}
		void cbTextGroupInterval_EditValueChanged(object sender, EventArgs e) {
			int selectedIndex = lbDimensionDefinitions.SelectedIndex;
			if(selectedIndex >= 0) {
				DimensionDefinition oldDefinition = dimensionDefinitions[selectedIndex].Definition;
				string dataMember = oldDefinition.DataMember;
				DimensionDefinition definition = new DimensionDefinition(dataMember, oldDefinition.DateTimeGroupInterval, (TextGroupInterval)cbTextGroupInterval.EditValue);
				dimensionDefinitions[selectedIndex].Definition = definition;
				dimensionDefinitions[selectedIndex].DisplayText = DataItemDefinitionDisplayTextProvider.GetDimensionDefinitionDisplayText(definition, ((DataSourceInfo)ceDataSource.SelectedItem).GetPickManager());
				UpdateDefinitionListBox();
			}
		}
		void cbDateTimeGroupInterval_EditValueChanged(object sender, EventArgs e) {
			int selectedIndex = lbDimensionDefinitions.SelectedIndex;
			if(selectedIndex >= 0) {
				DimensionDefinition oldDefinition = dimensionDefinitions[selectedIndex].Definition;
				string dataMember = oldDefinition.DataMember;
				DimensionDefinition definition = new DimensionDefinition(dataMember, (DateTimeGroupInterval)cbDateTimeGroupInterval.EditValue, oldDefinition.TextGroupInterval);
				dimensionDefinitions[selectedIndex].Definition = definition;
				DataSourceInfo dataSourceInfo = (DataSourceInfo)ceDataSource.SelectedItem;
				dimensionDefinitions[selectedIndex].DisplayText = DataItemDefinitionDisplayTextProvider.GetDimensionDefinitionDisplayText(definition, (dataSourceInfo.DataSource.GetDataSourceSchema(dataSourceInfo.DataMember)));
				UpdateDefinitionListBox();
			}
		}
		void OnFocusedDataFieldChanged(object sender, DataFieldEventArgs e) {
			DataField field = e.DataField;
			btnAdd.Enabled = field != null && field.IsDataMemberNode;
		}
		void OnDataFieldDoubleClick(object sender, DataFieldEventArgs e) {
			AddDimensionDefinitions();
		}
		void OnDimensionDefinitionsSelectedValueChanged(object sender, EventArgs e) {
			UpdateGroupIntervalEditors();
			UpdateRemoveButtonState();
		}
		void ceIncludeMeasureNames_CheckedChanged(object sender, EventArgs e) {
		}
	}
	public class DimensionDefinitionDataSourceRow {
		public DimensionDefinition Definition { get; set; }
		public string DisplayText { get; set; }
	}
}
