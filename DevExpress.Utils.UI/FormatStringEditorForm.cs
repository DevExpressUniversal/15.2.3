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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Reflection;
using Microsoft.Win32;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.UI.Localization;
using DevExpress.Utils.FormatStrings;
using DevExpress.XtraLayout;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	public class FormatStringEditorForm : ReportsEditorFormBase {
		public const string general = "General";
		public const string DateTime = "DateTime";
		public const string Number = "Int32";
		#region static
		static void SetControlEnablement(IList controls, bool enabled) {
			foreach(Control ctl in controls)
				ctl.Enabled = enabled;
		}
		static string GetControlText(Control ctl) {
			return ctl.Visible ? ctl.Text : "";
		}
		static void SetControlVisibility(IList controls, bool visible) {
			foreach(Control ctl in controls)
				ctl.Visible = visible;
		}
		static void FillData(DevExpress.XtraEditors.ListBoxControl lbx, string[] formatStrings) {
			lbx.Items.AddRange(formatStrings);
		}
		static CategoryItem[] FilterCategoryItems(CategoryItem[] items) {
			System.Collections.Generic.List<CategoryItem> result = new System.Collections.Generic.List<CategoryItem>();
			foreach(CategoryItem categoryItem in items) {
				if(categoryItem.ToString() != CategoryItem.GetDisplayName(general))
					result.Add(categoryItem);
			}
			return result.ToArray();
		}
		#endregion
		private string lastSelectedFormat = "";
		private System.ComponentModel.IContainer components = null;
		private LayoutControl layoutControl;
		private LayoutControlGroup Root;
		private DevExpress.XtraEditors.ListBoxControl lbxCategory;
		private LayoutControlItem layoutControlItem1;
		private DevExpress.XtraEditors.LabelControl lbCustomResult;
		private DevExpress.XtraEditors.SimpleButton btnAddCustomFormat;
		protected DevExpress.XtraEditors.TextEdit tbCustomFormat;
		private DevExpress.XtraEditors.LabelControl labelSuffix;
		private DevExpress.XtraEditors.ListBoxControl lbxCustomFormat;
		protected DevExpress.XtraEditors.ListBoxControl lbxFormat;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private LayoutControlGroup groupCategory;
		private TabbedControlGroup tbcTypes;
		private LayoutControlGroup tabPageCustom;
		private LayoutControlItem layoutControlItem2;
		private LayoutControlItem layoutControlItem5;
		private LayoutControlItem layoutControlItem6;
		private LayoutControlItem layoutControlItem7;
		private LayoutControlGroup tabPageStandard;
		private LayoutControlItem layoutControlItem3;
		private LayoutControlItem layoutControlItem4;
		private LayoutControlGroup grpButtons;
		private LayoutControlItem layoutControlItem16;
		private LayoutControlItem layoutControlItem17;
		private LayoutControlGroup nonGeneralCustomGroup;
		private LayoutControlGroup nonGeneralStandardGroup;
		private LabelControl labelPrefix;
		private LabelControl labelControl1;
		private LayoutControlGroup generalCustomGroup;
		private LayoutControlItem layoutControlItem8;
		private LayoutControlGroup generalStandardGroup;
		private LayoutControlItem layoutControlItem9;
		private LabelControl lbStandardResult;
		protected TextEdit tbSuffix;
		protected TextEdit tbPrefix;
		private LayoutControlItem layoutControlItem10;
		private LayoutControlItem layoutControlItem11;
		private LayoutControlItem layoutControlItem12;
		private LabelControl lbCategory;
		private LayoutControlItem layoutControlItem13;
		string[] currentRegistryValues;
		protected CategoryItem SelectedItem {
			get {
				return lbxCategory.SelectedItem as CategoryItem;
			}
		}
		public string CategoryName {
			get { return lbxCategory.Text; }
		}
		private string editValue = string.Empty;
		public string EditValue {
			get { return editValue; }
			set { editValue = value == null ? string.Empty : value; }
		}
		protected bool IsCustomFormat {
			get { return tbcTypes.SelectedTabPageIndex == 1; }
		}
		protected IServiceProvider provider;
		XmlDataLoader xmlDataLoader;
		public FormatStringEditorForm(string initialFormat, IServiceProvider provider)
			: base(provider) {
			this.provider = provider;
			this.xmlDataLoader = new XmlDataLoader();
			InitializeComponent();
			InitializeLocalStrings();
			lbxCustomFormat.ContextButtons[0].Glyph = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Utils.UI.Images.Delete.png"));
			EditValue = Parse(initialFormat);
			InitializeCategoryItems();
		}
		private void InitializeLocalStrings() { 
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatStringEditorForm));
			DevExpress.Utils.SimpleContextButton simpleContextButton1 = new DevExpress.Utils.SimpleContextButton();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition16 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition17 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition18 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition17 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition18 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition19 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition14 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition15 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition16 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.lbCategory = new DevExpress.XtraEditors.LabelControl();
			this.lbStandardResult = new DevExpress.XtraEditors.LabelControl();
			this.tbSuffix = new DevExpress.XtraEditors.TextEdit();
			this.tbPrefix = new DevExpress.XtraEditors.TextEdit();
			this.labelPrefix = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lbCustomResult = new DevExpress.XtraEditors.LabelControl();
			this.btnAddCustomFormat = new DevExpress.XtraEditors.SimpleButton();
			this.tbCustomFormat = new DevExpress.XtraEditors.TextEdit();
			this.labelSuffix = new DevExpress.XtraEditors.LabelControl();
			this.lbxCustomFormat = new DevExpress.XtraEditors.ListBoxControl();
			this.lbxFormat = new DevExpress.XtraEditors.ListBoxControl();
			this.lbxCategory = new DevExpress.XtraEditors.ListBoxControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.groupCategory = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
			this.tbcTypes = new DevExpress.XtraLayout.TabbedControlGroup();
			this.tabPageStandard = new DevExpress.XtraLayout.LayoutControlGroup();
			this.nonGeneralStandardGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.generalStandardGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.tabPageCustom = new DevExpress.XtraLayout.LayoutControlGroup();
			this.nonGeneralCustomGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.generalCustomGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbSuffix.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbPrefix.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbCustomFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxCustomFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxCategory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupCategory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbcTypes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPageStandard)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nonGeneralStandardGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.generalStandardGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPageCustom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nonGeneralCustomGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.generalCustomGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
			this.SuspendLayout();
			this.layoutControl.Controls.Add(this.lbCategory);
			this.layoutControl.Controls.Add(this.lbStandardResult);
			this.layoutControl.Controls.Add(this.tbSuffix);
			this.layoutControl.Controls.Add(this.tbPrefix);
			this.layoutControl.Controls.Add(this.labelPrefix);
			this.layoutControl.Controls.Add(this.labelControl1);
			this.layoutControl.Controls.Add(this.lbCustomResult);
			this.layoutControl.Controls.Add(this.btnAddCustomFormat);
			this.layoutControl.Controls.Add(this.tbCustomFormat);
			this.layoutControl.Controls.Add(this.labelSuffix);
			this.layoutControl.Controls.Add(this.lbxCustomFormat);
			this.layoutControl.Controls.Add(this.lbxFormat);
			this.layoutControl.Controls.Add(this.lbxCategory);
			this.layoutControl.Controls.Add(this.btnCancel);
			this.layoutControl.Controls.Add(this.btnOK);
			resources.ApplyResources(this.layoutControl, "layoutControl");
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(544, 117, 935, 641);
			this.layoutControl.Root = this.Root;
			this.lbCategory.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Show;
			resources.ApplyResources(this.lbCategory, "lbCategory");
			this.lbCategory.Name = "lbCategory";
			this.lbCategory.StyleController = this.layoutControl;
			resources.ApplyResources(this.lbStandardResult, "lbStandardResult");
			this.lbStandardResult.Name = "lbStandardResult";
			this.lbStandardResult.StyleController = this.layoutControl;
			resources.ApplyResources(this.tbSuffix, "tbSuffix");
			this.tbSuffix.Name = "tbSuffix";
			this.tbSuffix.StyleController = this.layoutControl;
			this.tbSuffix.TextChanged += new System.EventHandler(this.tbSuffPref_TextChanged);
			resources.ApplyResources(this.tbPrefix, "tbPrefix");
			this.tbPrefix.Name = "tbPrefix";
			this.tbPrefix.StyleController = this.layoutControl;
			this.tbPrefix.TextChanged += new System.EventHandler(this.tbSuffPref_TextChanged);
			resources.ApplyResources(this.labelPrefix, "labelPrefix");
			this.labelPrefix.Name = "labelPrefix";
			this.labelPrefix.StyleController = this.layoutControl;
			this.labelControl1.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl1.Appearance.Font")));
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.StyleController = this.layoutControl;
			resources.ApplyResources(this.lbCustomResult, "lbCustomResult");
			this.lbCustomResult.Name = "lbCustomResult";
			this.lbCustomResult.StyleController = this.layoutControl;
			resources.ApplyResources(this.btnAddCustomFormat, "btnAddCustomFormat");
			this.btnAddCustomFormat.Name = "btnAddCustomFormat";
			this.btnAddCustomFormat.StyleController = this.layoutControl;
			this.btnAddCustomFormat.Click += new System.EventHandler(this.btnAddCustomFormat_Click);
			resources.ApplyResources(this.tbCustomFormat, "tbCustomFormat");
			this.tbCustomFormat.Name = "tbCustomFormat";
			this.tbCustomFormat.StyleController = this.layoutControl;
			this.tbCustomFormat.EditValueChanged += new System.EventHandler(this.tbCustomFormat_EditValueChanged);
			this.tbCustomFormat.TextChanged += new System.EventHandler(this.textBoxEdit_TextChanged);
			resources.ApplyResources(this.labelSuffix, "labelSuffix");
			this.labelSuffix.Name = "labelSuffix";
			this.labelSuffix.StyleController = this.layoutControl;
			simpleContextButton1.Id = new System.Guid("c2ff7ea3-2c5f-4507-b1b5-286c290e6980");
			simpleContextButton1.Name = "btnRemoveCustomFormat";
			simpleContextButton1.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
			this.lbxCustomFormat.ContextButtons.Add(simpleContextButton1);
			resources.ApplyResources(this.lbxCustomFormat, "lbxCustomFormat");
			this.lbxCustomFormat.Name = "lbxCustomFormat";
			this.lbxCustomFormat.StyleController = this.layoutControl;
			this.lbxCustomFormat.SelectedIndexChanged += new System.EventHandler(this.lbxCustomFormat_SelectedIndexChanged);
			this.lbxCustomFormat.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.lbxCustomFormat_ContextButtonClick);
			this.lbxCustomFormat.CustomizeContextItem += new DevExpress.XtraEditors.ViewInfo.ListBoxControlContextButtonCustomizeEventHandler(this.lbxCustomFormat_CustomizeContextItem);
			this.lbxCustomFormat.DoubleClick += new System.EventHandler(this.lbxFormat_DoubleClick);
			resources.ApplyResources(this.lbxFormat, "lbxFormat");
			this.lbxFormat.Name = "lbxFormat";
			this.lbxFormat.StyleController = this.layoutControl;
			this.lbxFormat.SelectedIndexChanged += new System.EventHandler(this.lbxFormat_SelectedIndexChanged);
			this.lbxFormat.DoubleClick += new System.EventHandler(this.lbxFormat_DoubleClick);
			resources.ApplyResources(this.lbxCategory, "lbxCategory");
			this.lbxCategory.Name = "lbxCategory";
			this.lbxCategory.StyleController = this.layoutControl;
			this.lbxCategory.SelectedIndexChanged += new System.EventHandler(this.CategorylistBox_SelectedIndexChanged);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.groupCategory,
			this.tbcTypes,
			this.grpButtons});
			this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			columnDefinition16.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition16.Width = 127D;
			columnDefinition17.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition17.Width = 10D;
			columnDefinition18.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition18.Width = 100D;
			this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition16,
			columnDefinition17,
			columnDefinition18});
			rowDefinition17.Height = 50D;
			rowDefinition17.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition18.Height = 50D;
			rowDefinition18.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition19.Height = 34D;
			rowDefinition19.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition17,
			rowDefinition18,
			rowDefinition19});
			this.Root.Size = new System.Drawing.Size(481, 302);
			this.groupCategory.AppearanceGroup.Options.UseTextOptions = true;
			this.groupCategory.AppearanceGroup.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Show;
			this.groupCategory.GroupBordersVisible = false;
			this.groupCategory.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem13});
			this.groupCategory.Location = new System.Drawing.Point(0, 0);
			this.groupCategory.Name = "groupCategory";
			this.groupCategory.OptionsTableLayoutItem.RowSpan = 2;
			this.groupCategory.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.groupCategory.Size = new System.Drawing.Size(127, 248);
			resources.ApplyResources(this.groupCategory, "groupCategory");
			this.layoutControlItem1.Control = this.lbxCategory;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 2);
			this.layoutControlItem1.Size = new System.Drawing.Size(127, 224);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem13.Control = this.lbCategory;
			this.layoutControlItem13.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem13.Name = "layoutControlItem13";
			this.layoutControlItem13.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem13.Size = new System.Drawing.Size(127, 24);
			this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem13.TextVisible = false;
			this.tbcTypes.Location = new System.Drawing.Point(137, 0);
			this.tbcTypes.Name = "tabbedControlGroup1";
			this.tbcTypes.OptionsTableLayoutItem.ColumnIndex = 2;
			this.tbcTypes.OptionsTableLayoutItem.RowSpan = 2;
			this.tbcTypes.SelectedTabPage = this.tabPageStandard;
			this.tbcTypes.SelectedTabPageIndex = 0;
			this.tbcTypes.Size = new System.Drawing.Size(324, 248);
			this.tbcTypes.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.tabPageStandard,
			this.tabPageCustom});
			this.tbcTypes.SelectedPageChanged += new DevExpress.XtraLayout.LayoutTabPageChangedEventHandler(this.tbcTypes_SelectedPageChanged);
			this.tabPageStandard.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.nonGeneralStandardGroup,
			this.generalStandardGroup,
			this.layoutControlItem12});
			this.tabPageStandard.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.tabPageStandard.Location = new System.Drawing.Point(0, 0);
			this.tabPageStandard.Name = "tabPageStandard";
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition6.Width = 100D;
			this.tabPageStandard.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6});
			rowDefinition6.Height = 100D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition7.Height = 53D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.tabPageStandard.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition6,
			rowDefinition7});
			this.tabPageStandard.Padding = new DevExpress.XtraLayout.Utils.Padding(9, 9, 9, 7);
			this.tabPageStandard.Size = new System.Drawing.Size(300, 202);
			this.tabPageStandard.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, -2);
			resources.ApplyResources(this.tabPageStandard, "tabPageStandard");
			this.nonGeneralStandardGroup.GroupBordersVisible = false;
			this.nonGeneralStandardGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3});
			this.nonGeneralStandardGroup.Location = new System.Drawing.Point(0, 0);
			this.nonGeneralStandardGroup.Name = "nonGeneralStandardGroup";
			this.nonGeneralStandardGroup.Size = new System.Drawing.Size(300, 149);
			this.nonGeneralStandardGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.layoutControlItem3.Control = this.lbxFormat;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(300, 149);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.generalStandardGroup.GroupBordersVisible = false;
			this.generalStandardGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem9,
			this.layoutControlItem10,
			this.layoutControlItem11,
			this.layoutControlItem4});
			this.generalStandardGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.generalStandardGroup.Location = new System.Drawing.Point(0, 0);
			this.generalStandardGroup.Name = "generalStandardGroup";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition1.Width = 22D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition2.Width = 50D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 6D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition4.Width = 50D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 22D;
			this.generalStandardGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 20D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition2.Height = 50D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition3.Height = 17D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition4.Height = 24D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition5.Height = 50D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			this.generalStandardGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3,
			rowDefinition4,
			rowDefinition5});
			this.generalStandardGroup.Size = new System.Drawing.Size(300, 149);
			this.layoutControlItem9.Control = this.labelPrefix;
			this.layoutControlItem9.Location = new System.Drawing.Point(22, 64);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem9.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem9.Size = new System.Drawing.Size(125, 17);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem10.Control = this.tbPrefix;
			this.layoutControlItem10.Location = new System.Drawing.Point(22, 81);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem10.Size = new System.Drawing.Size(125, 24);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem11.Control = this.tbSuffix;
			this.layoutControlItem11.Location = new System.Drawing.Point(153, 81);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem11.Size = new System.Drawing.Size(125, 24);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem4.Control = this.labelSuffix;
			this.layoutControlItem4.Location = new System.Drawing.Point(153, 64);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem4.Size = new System.Drawing.Size(125, 17);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem12.Control = this.lbStandardResult;
			this.layoutControlItem12.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem12.Location = new System.Drawing.Point(0, 149);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 20, 20);
			this.layoutControlItem12.Size = new System.Drawing.Size(300, 53);
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextVisible = false;
			this.tabPageCustom.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.nonGeneralCustomGroup,
			this.generalCustomGroup});
			this.tabPageCustom.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.tabPageCustom.Location = new System.Drawing.Point(0, 0);
			this.tabPageCustom.Name = "tabPageCustom";
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition10.Width = 100D;
			this.tabPageCustom.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition10});
			rowDefinition14.Height = 100D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.Percent;
			this.tabPageCustom.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition14});
			this.tabPageCustom.OptionsTableLayoutItem.ColumnIndex = 1;
			this.tabPageCustom.Size = new System.Drawing.Size(300, 202);
			resources.ApplyResources(this.tabPageCustom, "tabPageCustom");
			this.nonGeneralCustomGroup.GroupBordersVisible = false;
			this.nonGeneralCustomGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.layoutControlItem7});
			this.nonGeneralCustomGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.nonGeneralCustomGroup.Location = new System.Drawing.Point(0, 0);
			this.nonGeneralCustomGroup.Name = "nonGeneralCustomGroup";
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition7.Width = 100D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 80D;
			this.nonGeneralCustomGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition7,
			columnDefinition8});
			rowDefinition8.Height = 100D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition9.Height = 20D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition10.Height = 20D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.nonGeneralCustomGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition8,
			rowDefinition9,
			rowDefinition10});
			this.nonGeneralCustomGroup.Size = new System.Drawing.Size(300, 202);
			this.nonGeneralCustomGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.layoutControlItem2.Control = this.lbxCustomFormat;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 4);
			this.layoutControlItem2.Size = new System.Drawing.Size(300, 162);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem5.Control = this.tbCustomFormat;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 162);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem5.Size = new System.Drawing.Size(220, 20);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.layoutControlItem6.Control = this.btnAddCustomFormat;
			this.layoutControlItem6.Location = new System.Drawing.Point(220, 162);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem6.Size = new System.Drawing.Size(80, 20);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.lbCustomResult;
			this.layoutControlItem7.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 182);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem7.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 20, 20);
			this.layoutControlItem7.Size = new System.Drawing.Size(300, 20);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.generalCustomGroup.GroupBordersVisible = false;
			this.generalCustomGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem8});
			this.generalCustomGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.generalCustomGroup.Location = new System.Drawing.Point(0, 0);
			this.generalCustomGroup.Name = "generalCustomGroup";
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition9.Width = 100D;
			this.generalCustomGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition9});
			rowDefinition11.Height = 50D;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition12.Height = 20D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition13.Height = 50D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
			this.generalCustomGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition11,
			rowDefinition12,
			rowDefinition13});
			this.generalCustomGroup.Size = new System.Drawing.Size(300, 202);
			this.layoutControlItem8.Control = this.labelControl1;
			this.layoutControlItem8.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 91);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem8.Size = new System.Drawing.Size(300, 20);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			resources.ApplyResources(this.grpButtons, "grpButtons");
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem16,
			this.layoutControlItem17});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 248);
			this.grpButtons.Name = "grpButtons";
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 100D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition12.Width = 80D;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition13.Width = 1D;
			columnDefinition14.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition14.Width = 80D;
			columnDefinition15.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition15.Width = 1D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition11,
			columnDefinition12,
			columnDefinition13,
			columnDefinition14,
			columnDefinition15});
			rowDefinition15.Height = 8D;
			rowDefinition15.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition16.Height = 26D;
			rowDefinition16.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition15,
			rowDefinition16});
			this.grpButtons.OptionsTableLayoutItem.ColumnSpan = 3;
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(461, 34);
			this.layoutControlItem16.Control = this.btnCancel;
			resources.ApplyResources(this.layoutControlItem16, "layoutControlItem16");
			this.layoutControlItem16.Location = new System.Drawing.Point(380, 8);
			this.layoutControlItem16.Name = "layoutControlItem16";
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem16.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem16.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem16.TextVisible = false;
			this.layoutControlItem17.Control = this.btnOK;
			resources.ApplyResources(this.layoutControlItem17, "layoutControlItem17");
			this.layoutControlItem17.Location = new System.Drawing.Point(299, 8);
			this.layoutControlItem17.Name = "layoutControlItem17";
			this.layoutControlItem17.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem17.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem17.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.TextVisible = false;
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.layoutControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormatStringEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tbSuffix.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbPrefix.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbCustomFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxCustomFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbxCategory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupCategory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbcTypes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPageStandard)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nonGeneralStandardGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.generalStandardGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabPageCustom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nonGeneralCustomGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.generalCustomGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual string RegistryPath {
			get { return string.Empty; }
		}
		private void InitializeCategoryItems() {
			lbxCategory.Items.Clear();
			CategoryItem[] items = xmlDataLoader.CreateCategoryItems();
			if(ShouldHideGeneralItem)
				items = FilterCategoryItems(items);
			lbxCategory.Items.AddRange(items);
			int index = lbxCategory.FindString(CategoryItem.GetDisplayName(DateTime));
			string[] formatStrings = CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns();
			if(index >= 0) ((CategoryItem)lbxCategory.Items[0]).SetFormatStrings(formatStrings);
			CategoryItem defaultItem = FindCategoryItemByFormat(items, editValue);
			if(defaultItem != null) {
				lbxCategory.SelectedIndex = lbxCategory.Items.IndexOf(defaultItem);
				SelectEditValue();
			} else
				lbxCategory.SelectedIndex = 0;
		}
		protected virtual bool ShouldHideGeneralItem {
			get {
				return true;
			}
		}
		private void SelectEditValue() {
			if(CategoryName != CategoryItem.GetDisplayName(general)) {
				int index = lbxFormat.Items.IndexOf(editValue);
				if(index >= 0)
					lbxFormat.SelectedIndex = index;
				else {
					FillCustomData();
					index = lbxCustomFormat.Items.IndexOf(editValue);
					if(index >= 0) {
						tbcTypes.SelectedTabPageIndex = 1;
						lbxCustomFormat.SelectedIndex = index;
					}
				}
			} else {
				string prefix, suffix = String.Empty;
				int index = editValue.IndexOf("{0}");
				if(index < 0)
					prefix = editValue;
				else {
					prefix = editValue.Substring(0, index);
					suffix = editValue.Substring(index + 3);
				}
				tbPrefix.Text = prefix;
				tbSuffix.Text = suffix;
			}
		}
		static int FindCorrespondingFigure(string format, int openFigureIndex) {
			int count = format.Length;
			int weight = 1;
			for(int i = openFigureIndex + 1; i < count; i++) {
				if(format[i] == '{')
					weight++;
				else if(format[i] == '}')
					weight--;
				if(weight == 0)
					return i;
			}
			return -1;
		}
		static string Parse(string format) {
			int index = !string.IsNullOrEmpty(format) ? format.IndexOf("{0:") : -1;
			if(index >= 0) {
				int closeBraceIndex = FindCorrespondingFigure(format, index);
				if(closeBraceIndex >= 0)
					return format.Substring(0, index) + format.Substring(index + 3, closeBraceIndex - index - 3) + format.Substring(closeBraceIndex + 1);
			}
			return format;
		}
		ArrayList GetAllFormats(CategoryItem item) {
			ArrayList formats = new ArrayList();
			formats.AddRange(item.FormatStrings);
			formats.AddRange(item.GetCustomFormats());
			currentRegistryValues = RegistryHelper.LoadRegistryValue(RegistryPath, GetRegistryKey(item.Type));
			formats.AddRange(currentRegistryValues);
			return formats;
		}
		private CategoryItem FindCategoryItemByFormat(CategoryItem[] items, string format) {
			CategoryItem generalCategory = null;
			int count = items.Length;
			for(int i = 0; i < count; i++) {
				CategoryItem item = items[i];
				if(GetAllFormats(item).IndexOf(format) >= 0)
					return item;
				if(item.ToString() == CategoryItem.GetDisplayName(general))
					generalCategory = item;
			}
			return generalCategory;
		}
		private void CategorylistBox_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateControlVisibility(CategoryName);
			if(IsCustomFormat) {
				FillCustomData();
				if(lbxCustomFormat.Items.Count > 0 && lbxCustomFormat.SelectedIndex < 0)
					lbxCustomFormat.SelectedIndex = 0;
			} else {
				FillStandardData();
			}
		}
		static private string GetRegistryKey(Type type) {
			return type == typeof(Int32) ? CategoryItem.GetDisplayName(Number) :
				type == typeof(DateTime) ? CategoryItem.GetDisplayName(DateTime) : "";
		}
		private void lbxFormat_SelectedIndexChanged(object sender, System.EventArgs e) {
			lastSelectedFormat = "";
			if(CategoryName != CategoryItem.GetDisplayName(general)) {
				lastSelectedFormat = lbxFormat.Text;
				SelectedItem.UpdateFormatedValue(lbxFormat.Text);
			} else
				SelectedItem.FormatedValue = "###";
			tbCustomFormat.Text = lastSelectedFormat;
			UpdateStandardResult();
		}
		void UpdateCustomResult() {
			lbCustomResult.Text = GetControlText(tbPrefix) + SelectedItem.FormatedValue + GetControlText(tbSuffix);
		}
		void UpdateStandardResult() {
			lbStandardResult.Text = GetControlText(tbPrefix) + SelectedItem.FormatedValue + GetControlText(tbSuffix);
		}
		private void tbSuffPref_TextChanged(object sender, System.EventArgs e) {
			UpdateStandardResult();
		}
		private void UpdateControlVisibility(string categoryName) {
			if(categoryName == CategoryItem.GetDisplayName(general)) {
				nonGeneralStandardGroup.Visibility = nonGeneralCustomGroup.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
				generalStandardGroup.Visibility = generalCustomGroup.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
			} else {
				generalStandardGroup.Visibility = generalCustomGroup.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
				nonGeneralStandardGroup.Visibility = nonGeneralCustomGroup.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
			}
		}
		private void textBoxEdit_TextChanged(object sender, System.EventArgs e) {
			SelectedItem.UpdateFormatedValue(tbCustomFormat.Text);
			UpdateCustomResult();
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			string badSymbolString = UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Msg_BadSymbol);
			if(lbStandardResult.Text == badSymbolString || lbCustomResult.Text == badSymbolString) {
				XtraMessageBox.Show(LookAndFeel, this, UtilsUILocalizer.GetString(UtilsUIStringId.Msg_ContainsIllegalSymbols), UtilsUILocalizer.GetString(UtilsUIStringId.Msg_ErrorTitle), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			ExitForm(true);
		}
		protected virtual string GetEditValue() {
			string s0 = tbPrefix.Text;
			string s1 = lbxFormat.Text;
			string s2 = tbSuffix.Text;
			if(IsCustomFormat) {
				s0 = s2 = "";
				s1 = tbCustomFormat.Text;
			} else if(CategoryName == CategoryItem.GetDisplayName(general)) {
				return s0 + "{0}" + s2;
			}
			return s1;
		}
		private void btnCancel_Click(object sender, System.EventArgs e) {
			ExitForm(false);
		}
		private void CloseDialog(DialogResult ds) {
			DialogResult = ds;
			Close();
		}
		private void lbxCustomFormat_SelectedIndexChanged(object sender, System.EventArgs e) {
			tbCustomFormat.Text = lbxCustomFormat.Text;
			SelectedItem.UpdateFormatedValue(tbCustomFormat.Text);
			UpdateCustomResult();
		}
		private void FillStandardData() {
			lbxFormat.Items.Clear();
			FillData(lbxFormat, SelectedItem.FormatStrings);
			SelectedItem.UpdateFormatedValue(lbxFormat.Text);
			if(lbxFormat.Items.Count > 0 && lbxFormat.SelectedIndex < 0)
				lbxFormat.SelectedIndex = 0;
			UpdateStandardResult();
		}
		private void FillCustomData() {
			string savedCustomFormatString = tbCustomFormat.Text;
			lbxCustomFormat.Items.Clear();
			FillData(lbxCustomFormat, SelectedItem.GetCustomFormats());
			SelectedItem.UpdateFormatedValue(savedCustomFormatString);
			currentRegistryValues = RegistryHelper.LoadRegistryValue(RegistryPath, GetRegistryKey(SelectedItem.Type));
			lbxCustomFormat.Items.AddRange(currentRegistryValues);
			UpdateCustomResult();
		}
		private void lbxFormat_DoubleClick(object sender, System.EventArgs e) {
			ExitForm(true);
		}
		private void ExitForm(bool CloseMode) {
			if(CloseMode) {
				string format = tbCustomFormat.Text;
				if(IsCustomFormat && SelectedItem.CustomFormatsContain(format) == false)
					RegistryHelper.AddRegistryValue(format, RegistryPath, GetRegistryKey(SelectedItem.Type));
				CloseDialog(DialogResult.OK);
				editValue = GetEditValue();
			} else
				CloseDialog(DialogResult.Cancel);
		}
		private void tbcTypes_SelectedPageChanged(object sender, LayoutTabPageChangedEventArgs e) {
			if(IsCustomFormat)
				FillCustomData();
			else
				FillStandardData();
		}
		private void btnAddCustomFormat_Click(object sender, EventArgs e) {
			string format = tbCustomFormat.Text;
			RegistryHelper.AddRegistryValue(format, RegistryPath, GetRegistryKey(SelectedItem.Type));
			FillCustomData();
			tbCustomFormat_EditValueChanged(null, null);
		}
		private void tbCustomFormat_EditValueChanged(object sender, EventArgs e) {
			if(string.IsNullOrEmpty(tbCustomFormat.Text))
				return;
			int index = lbxCustomFormat.Items.IndexOf(tbCustomFormat.Text);
			if(index >= 0) {
				lbxCustomFormat.SelectedIndex = index;
				btnAddCustomFormat.Enabled = false;
			} else {
				lbxCustomFormat.SelectedIndex = -1;
				btnAddCustomFormat.Enabled = true;
			}
		}
		private void lbxCustomFormat_ContextButtonClick(object sender, Utils.ContextItemClickEventArgs e) {
			string currentValue = (string)e.DataItem;
			if(currentRegistryValues.ToList().Contains(currentValue)) {
				RegistryHelper.DeleteRegistryValue(currentValue, RegistryPath, GetRegistryKey(SelectedItem.Type));
				lbxCustomFormat.Items.RemoveAt(lbxCustomFormat.Items.IndexOf(currentValue));
			}
		}
		private void lbxCustomFormat_CustomizeContextItem(object sender, XtraEditors.ViewInfo.ListBoxControlContextButtonCustomizeEventArgs e) {
			if(!currentRegistryValues.ToList().Contains(e.Item))
				e.ContextItem.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
		}
	}
}
namespace DevExpress.Utils.FormatStrings {
	public class CategoryItem {
		public static string GetDisplayName(string categoryName) {
			switch(categoryName) {
				case "DateTime": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_DateTime);
				case "Int32": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_Int32);
				case "Number": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_Number);
				case "Percent": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_Percent);
				case "Currency": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_Currency);
				case "Special": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_Special);
				case "General": return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Cat_General);
			}
			return categoryName;
		}
		private string[] formatStrings = { };
		private Type type = null;
		private object val = null;
		private string categoryName = "";
		private string formatedValue = "";
		XmlDataLoader xmlDataLoader;
		public string FormatedValue {
			get { return formatedValue; }
			set { formatedValue = value; }
		}
		public Type Type { get { return type; } }
		public object Value {
			get { return val; }
		}
		public string[] FormatStrings {
			get { return formatStrings; }
		}
		public CategoryItem(XmlDataLoader xmlDataLoader, string categoryName, Type type, object val, string[] formatStrings) {
			System.Diagnostics.Debug.Assert(type != null);
			this.xmlDataLoader = xmlDataLoader;
			this.categoryName = categoryName;
			this.type = type;
			this.val = val;
			this.formatStrings = formatStrings;
		}
		public void SetFormatStrings(string[] formatStrings) {
			this.formatStrings = formatStrings;
		}
		public override string ToString() {
			return GetDisplayName(categoryName);
		}
		public string[] GetCustomFormats() {
			return xmlDataLoader.GetCustomFormats(type);
		}
		public void Update(Type type, object val) {
			this.type = type;
			this.val = val;
		}
		public void UpdateFormatedValue(string formatStr) {
			formatedValue = Format(val, formatStr);
		}
		string Format(object val, string formatStr) {
			try {
				return string.Format(MailMergeFieldInfo.MakeFormatString(formatStr), val);
			} catch {
				return UtilsUILocalizer.GetString(UtilsUIStringId.FSForm_Msg_BadSymbol);
			}
		}
		public bool CustomFormatsContain(string name) {
			ArrayList items = new ArrayList(GetCustomFormats());
			return items.Contains(name);
		}
	}
	public class XmlDataLoader {
		#region static
		XmlDocument doc = new XmlDocument();
		public XmlDataLoader() {
			System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Utils.UI.formatstring.xml");
			doc.Load(stream);
			stream.Close();
		}
		public CategoryItem[] CreateCategoryItems() {
			ArrayList items = new ArrayList();
			try {
				XmlNodeList nodes = GetElementsByTagName("Category");
				foreach(XmlNode node in nodes) {
					string name = node.Attributes["Name"].Value;
					Type type = Type.GetType(node.Attributes["type"].Value);
					object val = GetObject(type, node.Attributes["value"].Value);
					if(type == typeof(DateTime)) val = System.DateTime.Now;
					string[] formatStrings = GetNodeContent(node.ChildNodes);
					CategoryItem item = new CategoryItem(this, name, type, val, formatStrings);
					items.Add(item);
				}
			} catch { }
			return items.ToArray(typeof(CategoryItem)) as CategoryItem[];
		}
		public string[] GetCustomFormats(Type type) {
			ArrayList items = new ArrayList();
			try {
				XmlNodeList nodes = GetElementsByTagName("CustomFormats");
				foreach(XmlNode node in nodes) {
					Type nodeType = Type.GetType(node.Attributes["type"].Value);
					if(nodeType.Equals(type)) {
						string[] formatStrings = GetNodeContent(node.ChildNodes);
						items.AddRange(formatStrings);
					}
				}
			} catch { }
			return items.ToArray(typeof(string)) as string[];
		}
		XmlNodeList GetElementsByTagName(string tagName) {
			return doc.GetElementsByTagName(tagName);
		}
		static string[] GetNodeContent(XmlNodeList nodes) {
			ArrayList items = new ArrayList();
			foreach(XmlNode node in nodes)
				items.Add(node.InnerText);
			return items.ToArray(typeof(string)) as string[];
		}
		static object GetObject(Type type, string s) {
			TypeConverter conv = TypeDescriptor.GetConverter(type);
			return conv.CanConvertFrom(typeof(string)) ? conv.ConvertFromString(s) : "";
		}
		#endregion
	}
}
