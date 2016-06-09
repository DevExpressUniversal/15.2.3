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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.Skins;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public partial class NewColorRecordDialog : DashboardForm {
		readonly DataFieldsBrowserPresenter browserPresenter;
		ColorTableServerKey colorTableKey;
		List<TextEdit> dimensionValueEditors = new List<TextEdit>();
		DataSourceInfo dataSourceInfo;
		IEnumerable<ColorTableServerKey> existingValues;
		List<MeasureDefinitionDataSourceRow> measureDefinitions = new List<MeasureDefinitionDataSourceRow>();
		public ColorTableServerKey ColorTableKey { get { return colorTableKey; } }
		public NewColorRecordDialog() {
			InitializeComponent();
		}
		public NewColorRecordDialog(UserLookAndFeel lookAndFeel, IDataSourceInfoProvider dataInfoProvider, IServiceProvider serviceProvider, ColorRepositoryKey repositoryKey, IEnumerable<ColorTableServerKey> existingValues)
			: base(lookAndFeel) {
			InitializeComponent();
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.dataSourceInfo = dataInfoProvider.GetDataSourceInfo(repositoryKey.DataSourceName, repositoryKey.DataMember);
			this.existingValues = existingValues;
			IDataFieldsBrowserPresenterFactory factory = serviceProvider.RequestServiceStrictly<IDataFieldsBrowserPresenterFactory>();
			browserPresenter = factory.CreatePresenter(dataFieldBrowser, dataSourceInfo, serviceProvider);
			browserPresenter.DisplayMode = DataFieldsBrowserDisplayMode.Measures;
			browserPresenter.FocusedDataFieldChanged += OnFocusedDataFieldChanged;
			browserPresenter.DataFieldDoubleClick += OnDataFieldDoubleClick;
			InitializeEditors(repositoryKey);
			lbMeasureDefinitions.ValueMember = "Definition";
			lbMeasureDefinitions.DisplayMember = "DisplayText";
			lbMeasureDefinitions.DataSource = measureDefinitions;
			ClientSize = new Size(1, 1);
			ClientSize = layoutControl1.GetPreferredSize(Size.Empty);
			dataFieldBrowser.SetToolbarVisibility(false);
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
		void OnFocusedDataFieldChanged(object sender, DataFieldEventArgs e) {
			DataField field = e.DataField;
			btnAdd.Enabled = field != null && field.IsDataMemberNode;
		}
		void OnDataFieldDoubleClick(object sender, DataFieldEventArgs e) {
			AddMeasureDefinitions();
		}
		void UpdateRemoveButtonState() {
			btnRemove.Enabled = measureDefinitions.Count > 0 && lbMeasureDefinitions.SelectedIndex >= 0;
		}
		void InitializeEditors(ColorRepositoryKey repositoryKey) {
			SuspendLayout();
			try {
				foreach(DimensionDefinition dimensionDefinition in repositoryKey.DimensionDefinitions) {
					if(!dimensionDefinition.Equals(DimensionDefinition.MeasureNamesDefinition)) {
						CreateDimensionValueEditors(DataItemDefinitionDisplayTextProvider.GetDimensionDefinitionDisplayText(dimensionDefinition,
							dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetDataSourceSchema(dataSourceInfo.DataMember) : null));
					}
				}
				if(!repositoryKey.DimensionDefinitions.Contains(DimensionDefinition.MeasureNamesDefinition)) {
					layoutControlGroup2.Visibility = LayoutVisibility.Never;
				}
				else {
					if(dimensionValueEditors.Count > 0)
						AddIndent();
					LoadImages();
				}
			}
			finally {
				ResumeLayout();
			}
			cbSummaryType.Properties.Items.AddEnum<SummaryType>(value => Measure.GetSummaryTypeCaption(value));
			cbSummaryType.SelectedIndex = 0;
		}
		void AddIndent() {
			EmptySpaceItem indent = new EmptySpaceItem();
			indent.SizeConstraintsType = SizeConstraintsType.Custom;
			indent.MinSize = new Size(1, 19);
			indent.Size = indent.MinSize;
			layoutControl1.AddItem(indent, layoutControlGroup2, InsertType.Top);
		}
		void LoadImages() {
			SkinElement skinElement = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinButton];
			Color imageColor = skinElement.Color.GetForeColor();
			Image addImage = ImageHelper.GetImage("AddToList");
			btnAdd.Image = ImageHelper.ColorBitmap(addImage, imageColor);
			Image removeImage = ImageHelper.GetImage("RemoveFromList");
			btnRemove.Image = ImageHelper.ColorBitmap(removeImage, imageColor);
		}
		void CreateDimensionValueEditors(string name)  {
			TextEdit editor = new TextEdit();
			LayoutControlItem layoutItem = layoutControl1.AddItem(layoutControlGroup2, InsertType.Top);
			layoutItem.Control = editor;
			layoutItem.Text = name + ":";
			dimensionValueEditors.Add(editor);
		}
		void GenerateColorTableKey() {
			List<object> dimensionValues = new List<object>();
			foreach(TextEdit textEdit in dimensionValueEditors) {
				dimensionValues.Add(DashboardSpecialValuesInternal.ToSpecialUniqueValue(textEdit.EditValue));
			}
			colorTableKey = new ColorTableServerKey(dimensionValues.Count > 0 ? dimensionValues.ToArray() : null,
				measureDefinitions.Count > 0 ? measureDefinitions.Select(row => row.Definition).ToArray() : null);
		}
		string GetDataMember() {
			DataField field = browserPresenter.SelectedDataField;
			if(field != null && field.IsDataMemberNode) {
				string dataMember = field.DataMember;
				if(!string.IsNullOrEmpty(dataMember)) {
					return dataMember;
				}
			}
			return null;
		}
		void OnBtnAddClick(object sender, EventArgs e) {
			AddMeasureDefinitions();
		}
		void AddMeasureDefinitions() {
			string dataMember = GetDataMember();
			if(dataMember != null) {
				MeasureDefinition definition = new MeasureDefinition(dataMember);
				measureDefinitions.Add(new MeasureDefinitionDataSourceRow() {
					Definition = definition,
					DisplayText = DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionDisplayText(definition, dataSourceInfo.GetPickManager())
				});
				UpdateDefinitionListBox();
				lbMeasureDefinitions.SelectedIndex = lbMeasureDefinitions.ItemCount - 1;
			}
		}
		void btnRemove_Click(object sender, EventArgs e) {
			int selectedIndex = lbMeasureDefinitions.SelectedIndex;
			if(selectedIndex >= 0) {
				measureDefinitions.RemoveAt(selectedIndex);
				UpdateDefinitionListBox();
				UpdateSummaryTypeEditor();
			}
		}
		void OnFormClosing(object sender, FormClosingEventArgs e) {
			if(DialogResult == DialogResult.OK) {
				GenerateColorTableKey();
				if(existingValues.Contains(colorTableKey, new ColorTableServerKeyComparer())) {
					XtraMessageBox.Show(LookAndFeel,
						this,
						DashboardWinLocalizer.GetString(DashboardWinStringId.MessageColorSchemeValueAlreadyExists),
						DashboardWinLocalizer.GetString(DashboardWinStringId.MessageColorSchemeValueAlreadyExistsCaption),
						MessageBoxButtons.OK);
					e.Cancel = true;
				}
			}
		}
		void UpdateDefinitionListBox() {
			lbMeasureDefinitions.Refresh();
			UpdateDefinitionListBoxSelection();
			UpdateRemoveButtonState();
		}
		void UpdateDefinitionListBoxSelection() {
			int selectedIndex = lbMeasureDefinitions.SelectedIndex;
			if(selectedIndex > measureDefinitions.Count - 1)
				selectedIndex -= 1;
			if(selectedIndex < 0 && measureDefinitions.Count > 0)
				selectedIndex = 0;
			lbMeasureDefinitions.SelectedIndex = selectedIndex;
		}
		void OnMeasureDefinitionsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateRemoveButtonState();
			UpdateSummaryTypeEditor();
		}
		void UpdateSummaryTypeEditor() {
			int selectedIndex = lbMeasureDefinitions.SelectedIndex;
			cbSummaryType.Enabled = selectedIndex >= 0;
			if(selectedIndex >= 0) {
				MeasureDefinition definition = measureDefinitions[selectedIndex].Definition;
				cbSummaryType.EditValue = definition.SummaryType;
			}
		}
		void cbSummaryType_EditValueChanged(object sender, EventArgs e) {
			int selectedIndex = lbMeasureDefinitions.SelectedIndex;
			if(selectedIndex >= 0) {
				string dataMember = measureDefinitions[selectedIndex].Definition.DataMember;
				MeasureDefinition definition = new MeasureDefinition(dataMember, (SummaryType)cbSummaryType.EditValue);
				measureDefinitions[selectedIndex].Definition = definition;
				measureDefinitions[selectedIndex].DisplayText = DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionDisplayText(definition, dataSourceInfo.GetPickManager());
				UpdateDefinitionListBox();
			}
		}
	}
	public class MeasureDefinitionDataSourceRow {
		public MeasureDefinition Definition { get; set; }
		public string DisplayText { get; set; }
	}
}
