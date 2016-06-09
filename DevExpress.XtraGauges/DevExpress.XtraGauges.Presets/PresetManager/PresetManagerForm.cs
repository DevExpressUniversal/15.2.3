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
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Presets;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public class PresetManagerForm : XtraForm, IPlatformTypeProvider {
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private PresetEditControl presetEditForm1;
		private PresetsTreeView presetsTreeView1;
		private System.ComponentModel.IContainer components;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.SplitterItem splitterItem1;
		private DevExpress.XtraLayout.LayoutControlItem presetEditForm;
		private CategoryPreview presetsCategoryPreview1;
		private DevExpress.XtraLayout.LayoutControlItem presetsPreview;
		private DevExpress.XtraLayout.SplitterItem splitterItem2;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private CheckEdit checkEdit1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
		string presetsDir = @"C:\presets\";
		IGaugeContainer gaugeContrainerCore;
		bool isWinCore;
		public PresetManagerForm(IGaugeContainer gaugeContainer) {
			this.gaugeContrainerCore = gaugeContainer;
			InitializeComponent();
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Preset Manager";
			this.Size = new Size(800, 600);
			this.MinimumSize = new Size(640, 480);
			IGaugeContainer testContainer = ControlLoader.CreateGaugeContainer();
			if(testContainer != null)
				isWinCore = testContainer.GetType().IsInstanceOfType(this.gaugeContrainerCore);
			else
				isWinCore = true;
			presetsTreeView1.AfterSelect += new TreeViewEventHandler(presetsTreeView1_AfterSelect);
			presetsTreeView1.FillCategories();
			if((Control.ModifierKeys & Keys.Control) != Keys.Control) {
				presetEditForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				splitterItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			}
			layoutControl1.AllowCustomization = false;
		}
		public bool IsWin { get { return isWinCore; } }
		protected override bool GetAllowSkin() {
			return (LookAndFeel != null) && LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
		}
		protected List<GaugePreset> LoadPresets(string filter) {
			List<GaugePreset> source = new List<GaugePreset>();
			FindPresetsInResources(filter, source);
			FindPresetsOnDisk(presetsDir, filter, source);
			if(!isWinCore)
				source.RemoveAll(p => p.Name.Equals("Haze") || p.Name.Equals("Ignis"));
			source.Sort(new GaugePresetComparer());
			return source;
		}
		protected void FindPresetsOnDisk(string path, string filter, List<GaugePreset> source) {
			DirectoryInfo presetsDir = new DirectoryInfo(path);
			if(presetsDir.Exists) {
				FileInfo[] fi = presetsDir.GetFiles(filter + "*");
				foreach(FileInfo fileInfo in fi) {
					source.Add(PresetLoader.LoadFromFile(fileInfo.FullName));
				}
			}
		}
		protected void FindPresetsInResources(string filter, List<GaugePreset> source) {
			foreach(string path in PresetLoader.Resources) {
				if(path.StartsWith("DevExpress.XtraGauges.Presets.Resources." + filter)) {
					source.Add(PresetLoader.LoadFromResources(path));
				}
			}
		}
		void presetsTreeView1_AfterSelect(object sender, TreeViewEventArgs e) {
			presetsCategoryPreview1.SetDataSource(LoadPresets(e.Node.Text), (data) =>
			{
				IGaugeContainer container = ControlLoader.CreateGaugeContainer();
				using(container as System.IDisposable) {
					using(MemoryStream ms = new MemoryStream(data)) {
						new BinaryXtraSerializer().DeserializeObject(container, ms, "IGaugeContainer");
					}
					return container.GetImage(container.Bounds.Width, container.Bounds.Height);
				}
			});
		}
		public void InitCurrentPreset(IGaugeContainer container) {
			BaseGaugePreset cPreset = BaseGaugePreset.FromGaugeContainer(container);
			using(MemoryStream ms = new MemoryStream()) {
				new BinaryXtraSerializer().SerializeObject(container, ms, "IGaugeContainer");
				cPreset.LayoutInfo = ms.ToArray();
				ms.Close();
			}
			presetEditForm1.SetTargetPreset(cPreset, container.GetImage(container.Bounds.Width, container.Bounds.Height));
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresetManagerForm));
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
			this.presetsCategoryPreview1 = new DevExpress.XtraGauges.Presets.PresetManager.CategoryPreview();
			this.presetEditForm1 = new DevExpress.XtraGauges.Presets.PresetManager.PresetEditControl();
			this.presetsTreeView1 = new DevExpress.XtraGauges.Presets.PresetManager.PresetsTreeView();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.presetEditForm = new DevExpress.XtraLayout.LayoutControlItem();
			this.presetsPreview = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem2 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.presetEditForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.presetsPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.presetsCategoryPreview1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.checkEdit1);
			this.layoutControl1.Controls.Add(this.presetsCategoryPreview1);
			this.layoutControl1.Controls.Add(this.presetEditForm1);
			this.layoutControl1.Controls.Add(this.presetsTreeView1);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(542, 473);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.checkEdit1.Location = new System.Drawing.Point(7, 448);
			this.checkEdit1.Name = "checkEdit1";
			this.checkEdit1.Properties.Caption = "Show the preset manager every time a new Gauge control is dropped onto the form";
			this.checkEdit1.Size = new System.Drawing.Size(529, 19);
			this.checkEdit1.StyleController = this.layoutControl1;
			this.checkEdit1.TabIndex = 7;
			this.checkEdit1.CheckedChanged += new System.EventHandler(this.checkEdit1_CheckedChanged);
			this.presetsCategoryPreview1.Location = new System.Drawing.Point(174, 7);
			this.presetsCategoryPreview1.Name = "presetsCategoryPreview1";
			this.presetsCategoryPreview1.Size = new System.Drawing.Size(362, 195);
			this.presetsCategoryPreview1.TabIndex = 6;
			this.presetsCategoryPreview1.TargetPreset = null;
			this.presetEditForm1.Location = new System.Drawing.Point(174, 219);
			this.presetEditForm1.Name = "presetEditForm1";
			this.presetEditForm1.Size = new System.Drawing.Size(362, 216);
			this.presetEditForm1.TabIndex = 5;
			this.presetsTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.presetsTreeView1.ImageIndex = 0;
			this.presetsTreeView1.ItemHeight = 20;
			this.presetsTreeView1.Location = new System.Drawing.Point(2, 2);
			this.presetsTreeView1.MaximumSize = new System.Drawing.Size(160, 0);
			this.presetsTreeView1.Name = "presetsTreeView1";
			this.presetsTreeView1.SelectedImageIndex = 0;
			this.presetsTreeView1.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.presetsTreeView1.ShowLines = false;
			this.presetsTreeView1.ShowRootLines = false;
			this.presetsTreeView1.Size = new System.Drawing.Size(160, 438);
			this.presetsTreeView1.TabIndex = 4;
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.splitterItem1,
			this.presetEditForm,
			this.presetsPreview,
			this.splitterItem2,
			this.layoutControlItem2,
			this.simpleSeparator1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(542, 473);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Control = this.presetsTreeView1;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(161, 439);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.splitterItem1.CustomizationFormText = "splitterItem1";
			this.splitterItem1.Location = new System.Drawing.Point(161, 0);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(6, 439);
			this.presetEditForm.Control = this.presetEditForm1;
			this.presetEditForm.CustomizationFormText = "layoutControlItem2";
			this.presetEditForm.Location = new System.Drawing.Point(167, 212);
			this.presetEditForm.Name = "presetEditForm";
			this.presetEditForm.Size = new System.Drawing.Size(373, 227);
			this.presetEditForm.Text = "presetEditForm";
			this.presetEditForm.TextLocation = DevExpress.Utils.Locations.Left;
			this.presetEditForm.TextSize = new System.Drawing.Size(0, 0);
			this.presetEditForm.TextToControlDistance = 0;
			this.presetEditForm.TextVisible = false;
			this.presetsPreview.Control = this.presetsCategoryPreview1;
			this.presetsPreview.CustomizationFormText = "presetsPreview";
			this.presetsPreview.Location = new System.Drawing.Point(167, 0);
			this.presetsPreview.Name = "presetsPreview";
			this.presetsPreview.Size = new System.Drawing.Size(373, 206);
			this.presetsPreview.Text = "presetsPreview";
			this.presetsPreview.TextLocation = DevExpress.Utils.Locations.Left;
			this.presetsPreview.TextSize = new System.Drawing.Size(0, 0);
			this.presetsPreview.TextToControlDistance = 0;
			this.presetsPreview.TextVisible = false;
			this.splitterItem2.CustomizationFormText = "splitterItem2";
			this.splitterItem2.Location = new System.Drawing.Point(167, 206);
			this.splitterItem2.Name = "splitterItem2";
			this.splitterItem2.Size = new System.Drawing.Size(373, 6);
			this.layoutControlItem2.Control = this.checkEdit1;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 441);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(540, 30);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.simpleSeparator1.CustomizationFormText = "simpleSeparator1";
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 439);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Size = new System.Drawing.Size(540, 2);
			this.simpleSeparator1.Text = "simpleSeparator1";
			this.ClientSize = new System.Drawing.Size(542, 473);
			this.Controls.Add(this.layoutControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PresetManagerForm";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.presetEditForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.presetsPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.presetsCategoryPreview1)).EndInit();
			this.ResumeLayout(false);
		}
		public IGaugeContainer GaugeContainer {
			get { return gaugeContrainerCore; }
		}
		public BaseGaugePreset TargetPreset {
			get { return presetsCategoryPreview1.TargetPreset; }
		}
		bool shouldShowPresetManager;
		public bool ShowManagerOnComponentAdding {
			get { return shouldShowPresetManager; }
			set { shouldShowPresetManager = value; checkEdit1.Checked = value; }
		}
		private void checkEdit1_CheckedChanged(object sender, EventArgs e) {
			ShowManagerOnComponentAdding = checkEdit1.Checked;
		}
	}
	public sealed class GaugePresetComparer : IComparer<GaugePreset> {
		public int Compare(GaugePreset x, GaugePreset y) {
			return x.Name.CompareTo(y.Name);
		}
	}
}
