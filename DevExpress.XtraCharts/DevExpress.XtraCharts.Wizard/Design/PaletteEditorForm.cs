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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public class PaletteEditorForm : XtraForm {
		SimpleButton btnClose;
		SimpleButton btnRemovePalette;
		SimpleButton btnNewPalette;
		SimpleButton btnCopyPalette;
		SimpleButton btnAddColor;
		SimpleButton btnRemoveColor;
		SimpleButton btnClearColor;
		TextEdit tbPaletteName;
		ListBoxControl lbPaletteColors;
		ListBoxControl lbPalettes;
		LabelControl labelControl1;
		readonly IChartContainer chartContainer;
		readonly PaletteRepository repository;
		string oldPaletteName;
		int[] customColors = new int[0];
		SimpleButton sbLoadPalette;
		SimpleButton sbSavePalette;
		Font palettesFont;
		public Palette CurrentPalette {
			get { return lbPalettes.SelectedItem as Palette; }
			set { lbPalettes.SelectedItem = value; }
		}
		PaletteEditorForm() {
			InitializeComponent();
		}
		public PaletteEditorForm(IChartContainer chartContainer,  PaletteRepository repository) : this() {
			this.repository = repository;
			this.chartContainer = chartContainer;
			UpdatePaletteListView();
			SetPaletteListBoxSelectedIndex(0);
			UpdatePaletteButtons();
		}
		#region Windows Form Designer generated code
		void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteEditorForm));
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.btnClearColor = new DevExpress.XtraEditors.SimpleButton();
			this.tbPaletteName = new DevExpress.XtraEditors.TextEdit();
			this.lbPaletteColors = new DevExpress.XtraEditors.ListBoxControl();
			this.lbPalettes = new DevExpress.XtraEditors.ListBoxControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnRemoveColor = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddColor = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemovePalette = new DevExpress.XtraEditors.SimpleButton();
			this.btnCopyPalette = new DevExpress.XtraEditors.SimpleButton();
			this.sbSavePalette = new DevExpress.XtraEditors.SimpleButton();
			this.btnNewPalette = new DevExpress.XtraEditors.SimpleButton();
			this.sbLoadPalette = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tbPaletteName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPaletteColors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPalettes)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Name = "btnClose";
			resources.ApplyResources(this.btnClearColor, "btnClearColor");
			this.btnClearColor.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.cancel_32x32;
			this.btnClearColor.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnClearColor.Name = "btnClearColor";
			this.btnClearColor.Click += new System.EventHandler(this.btnClearColor_Click);
			resources.ApplyResources(this.tbPaletteName, "tbPaletteName");
			this.tbPaletteName.Name = "tbPaletteName";
			this.tbPaletteName.Enter += new System.EventHandler(this.tbPaletteName_Enter);
			this.tbPaletteName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPaletteName_KeyDown);
			this.tbPaletteName.Leave += new System.EventHandler(this.tbPaletteName_Leave);
			resources.ApplyResources(this.lbPaletteColors, "lbPaletteColors");
			this.lbPaletteColors.Name = "lbPaletteColors";
			this.lbPaletteColors.SelectedIndexChanged += new System.EventHandler(this.lbPaletteColors_SelectedIndexChanged);
			this.lbPaletteColors.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbPaletteColors_DrawItem);
			this.lbPaletteColors.DoubleClick += new System.EventHandler(this.lbPaletteColors_DoubleClick);
			resources.ApplyResources(this.lbPalettes, "lbPalettes");
			this.lbPalettes.Name = "lbPalettes";
			this.lbPalettes.SelectedIndexChanged += new System.EventHandler(this.lbPalettes_SelectedIndexChanged);
			this.lbPalettes.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbPalettes_DrawItem);
			this.lbPalettes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbPalettes_KeyDown);
			this.lbPalettes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbPalettes_MouseDoubleClick);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.btnRemoveColor, "btnRemoveColor");
			this.btnRemoveColor.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.remove_32x32;
			this.btnRemoveColor.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemoveColor.Name = "btnRemoveColor";
			this.btnRemoveColor.Click += new System.EventHandler(this.btnRemoveColor_Click);
			resources.ApplyResources(this.btnAddColor, "btnAddColor");
			this.btnAddColor.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.add_32x32;
			this.btnAddColor.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAddColor.Name = "btnAddColor";
			this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
			resources.ApplyResources(this.btnRemovePalette, "btnRemovePalette");
			this.btnRemovePalette.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.deletelist_32x32;
			this.btnRemovePalette.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemovePalette.Name = "btnRemovePalette";
			this.btnRemovePalette.Click += new System.EventHandler(this.btnRemovePalette_Click);
			resources.ApplyResources(this.btnCopyPalette, "btnCopyPalette");
			this.btnCopyPalette.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.copy_32x32;
			this.btnCopyPalette.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnCopyPalette.Name = "btnCopyPalette";
			this.btnCopyPalette.Click += new System.EventHandler(this.btnCopyPalette_Click);
			resources.ApplyResources(this.sbSavePalette, "sbSavePalette");
			this.sbSavePalette.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.save_32x32;
			this.sbSavePalette.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.sbSavePalette.Name = "sbSavePalette";
			this.sbSavePalette.Click += new System.EventHandler(this.sbSave_Click);
			resources.ApplyResources(this.btnNewPalette, "btnNewPalette");
			this.btnNewPalette.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.new_32x32;
			this.btnNewPalette.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnNewPalette.Name = "btnNewPalette";
			this.btnNewPalette.Click += new System.EventHandler(this.btnAddPalette_Click);
			resources.ApplyResources(this.sbLoadPalette, "sbLoadPalette");
			this.sbLoadPalette.Image = global::DevExpress.XtraCharts.Wizard.Properties.Resources.open_32x32;
			this.sbLoadPalette.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.sbLoadPalette.Name = "sbLoadPalette";
			this.sbLoadPalette.Click += new System.EventHandler(this.sbLoad_Click);
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnClose;
			this.ControlBox = false;
			this.Controls.Add(this.sbSavePalette);
			this.Controls.Add(this.sbLoadPalette);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.lbPalettes);
			this.Controls.Add(this.lbPaletteColors);
			this.Controls.Add(this.tbPaletteName);
			this.Controls.Add(this.btnClearColor);
			this.Controls.Add(this.btnRemoveColor);
			this.Controls.Add(this.btnAddColor);
			this.Controls.Add(this.btnRemovePalette);
			this.Controls.Add(this.btnNewPalette);
			this.Controls.Add(this.btnCopyPalette);
			this.Controls.Add(this.btnClose);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PaletteEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.tbPaletteName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPaletteColors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPalettes)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		string GeneratePaletteName() {
			return NameGenerator.UniqueName(ChartLocalizer.GetString(ChartStringId.PalettePrefix), repository.PaletteNames);
		}
		OpenFileDialog CreateOpenFileDialog() {
			var ofdPalette = new OpenFileDialog();
			ofdPalette.FileName = "CustomPalette";
			var resources = new ComponentResourceManager(typeof(PaletteEditorForm));
			resources.ApplyResources(ofdPalette, "ofdPalette");
			return ofdPalette;
		}
		SaveFileDialog CreateSaveFileDialog() {
			var sfdPalette = new SaveFileDialog();
			var resources = new ComponentResourceManager(typeof(PaletteEditorForm));
			resources.ApplyResources(sfdPalette, "sfdPalette");
			return sfdPalette;
		}
		void UpdatePaletteListView() {
			lbPalettes.Items.BeginUpdate();
			lbPalettes.Items.Clear();
			foreach (string paletteName in repository.PaletteNames)
				lbPalettes.Items.Add(repository[paletteName]);
			lbPalettes.Items.EndUpdate();
		}
		void SetPaletteListBoxSelectedIndex(int index) {
			if (lbPalettes.Items.Count > 0) {
				if (index < 0)
					index = 0;
				else if (index >= lbPalettes.Items.Count)
					index = lbPalettes.Items.Count - 1;
				lbPalettes.SelectedIndex = index;
			}
		}
		void CurrentPaletteChanged() {
			Palette palette = CurrentPalette;
			if (palette != null) {
				tbPaletteName.Text = palette.Name;
				bool predefined = IsCurrentPalettePredefined();
				tbPaletteName.Enabled = !predefined;
				tbPaletteName.Font = new Font(tbPaletteName.Font, predefined ? FontStyle.Regular : FontStyle.Bold);
			} 
			else {
				tbPaletteName.Text = String.Empty;
				tbPaletteName.Enabled = false;
			}
			lbPalettes.Invalidate();
			UpdatePaletteButtons();
			UpdateColorListBox();
			UpdateColorButtons();
		}
		void UpdatePaletteButtons() {	
			Palette palette = CurrentPalette;
			btnCopyPalette.Enabled = palette != null;
			btnRemovePalette.Enabled = palette != null && lbPalettes.Items.IndexOf(palette) >= ((IPaletteRepository)repository).PredefinedCount;
		}
		void AddPalette(Palette palette) {
			((IPalette)palette).AddDefaultColorIfEmpty();
			repository.RegisterPalette(palette);
			UpdatePaletteListView();
			lbPalettes.SelectedItem = palette;
			CurrentPaletteChanged();
		}
		void UpdateColorListBox() {
			lbPaletteColors.Items.Clear();
			Palette palette = CurrentPalette;
			if (palette != null)
				foreach (PaletteEntry entry in palette)
					lbPaletteColors.Items.Add(entry.Color);
		}
		void UpdateColorButtons() {
			Palette palette = CurrentPalette;
			btnAddColor.Enabled = palette != null && !IsCurrentPalettePredefined();
			btnClearColor.Enabled = palette != null && !palette.IsEmpty && !IsCurrentPalettePredefined();
			btnRemoveColor.Enabled = palette != null && !palette.IsEmpty && !IsCurrentPalettePredefined();
		}
		void SetColorListBoxSelectedIndex(int index) {
			if (lbPaletteColors.Items.Count > 0) {
				if (index < 0)
					index = 0;
				if (index >= lbPaletteColors.Items.Count)
					index = lbPaletteColors.Items.Count - 1;
				lbPaletteColors.SelectedIndex = index;
			}
		}
		void ColorCountChanged(int currentColorIndex) {
			UpdateColorListBox();
			SetColorListBoxSelectedIndex(currentColorIndex);
			UpdateColorButtons();
		}
		bool IsCurrentPalettePredefined() {
			return CurrentPalette.Predefined;
		}
		void tbPaletteName_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				tbPaletteName.Text = oldPaletteName;
				lbPalettes.Focus();
			} 
			else if (e.KeyCode == Keys.Enter)
				lbPalettes.Focus();
		}
		void tbPaletteName_Enter(object sender, EventArgs e) {
			oldPaletteName = tbPaletteName.Text;
		}
		void tbPaletteName_Leave(object sender, EventArgs e) {
			string name = tbPaletteName.Text;
			if (String.IsNullOrEmpty(name))
				tbPaletteName.Text = oldPaletteName;
			else {
				Palette palette = CurrentPalette;
				if (palette != null) {
					try {
						if (repository[name] != palette) {
							tbPaletteName.Text = oldPaletteName;
							return;
						}
					}
					catch {
					}
					try {
						((IPalette)palette).SetName(name);
						((IPaletteRepository)repository).ReRegisterPalette(palette);
					} 
					catch (PaletteException) {
						tbPaletteName.Text = oldPaletteName;
						return;
					}
					UpdatePaletteListView();
					SetPaletteListBoxSelectedIndex(lbPalettes.Items.IndexOf(palette));
				}
			}
		}
		void lbPalettes_SelectedIndexChanged(object sender, EventArgs e) {
			CurrentPaletteChanged();
		}
		void lbPalettes_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			Palette palette = e.Item as Palette;
			if (palette != null && !palette.Predefined) {
				if (palettesFont == null)
					palettesFont = new Font(lbPalettes.Font, FontStyle.Bold);
				e.Appearance.Font = palettesFont;
			}
		}
		void lbPalettes_MouseDoubleClick(object sender, MouseEventArgs e) {
			DialogResult = DialogResult.OK;
		}
		void lbPalettes_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				DialogResult = DialogResult.OK;
		}
		void btnAddPalette_Click(object sender, EventArgs e) {
			AddPalette(new Palette(GeneratePaletteName()));
		}
		void btnCopyPalette_Click(object sender, EventArgs e) {			
			Palette palette = CurrentPalette;
			if (palette != null) {
				Palette newPalette = (Palette)palette.Clone();
				string name = GeneratePaletteName();
				((IPalette)newPalette).SetName(name);
				((IPalette)newPalette).SetDisplayName(name);
				AddPalette(newPalette);
			}
		}
		void btnRemovePalette_Click(object sender, EventArgs e) {
			Palette palette = CurrentPalette;
			if (palette != null) {
				repository.Remove(palette.Name);
				int itemIndex = lbPalettes.SelectedIndex;
				UpdatePaletteListView();
				SetPaletteListBoxSelectedIndex(itemIndex);
				CurrentPaletteChanged();
			}
		}
		void lbPaletteColors_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateColorButtons();
		}
		void lbPaletteColors_DoubleClick(object sender, EventArgs e) {
			if (!IsCurrentPalettePredefined()) {
				int index = lbPaletteColors.SelectedIndex;
				Palette palette = CurrentPalette;
				if (index != -1 && palette != null) {
					Color color = (Color)lbPaletteColors.Items[index];
					using (ColorDialog colorDialog = new ColorDialog()) { 
						colorDialog.Color = color;
						colorDialog.FullOpen = true;
						colorDialog.CustomColors = (int[])customColors.Clone();
						bool result = colorDialog.ShowDialog() == DialogResult.OK;
						customColors = (int[])colorDialog.CustomColors.Clone();
						if (result) {
							color = colorDialog.Color;
							palette[index].Color = color;
							palette[index].Color2 = color;
							UpdateColorListBox();
							lbPaletteColors.SelectedIndex = index;
						}
					}
				}
			}
		}
		void lbPaletteColors_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			if (e.Index != -1) {
				e.Handled = true;
				e.Graphics.FillRectangle(Brushes.White, e.Bounds);
				Color color = (Color)lbPaletteColors.Items[e.Index];
				using (Brush brush = new SolidBrush(color))
					e.Graphics.FillRectangle(brush, e.Bounds);
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
					using (StringFormat sf = (StringFormat)StringFormat.GenericDefault.Clone()) {
						sf.Alignment = StringAlignment.Center;
						sf.LineAlignment = StringAlignment.Center;				
						using (Font font = new Font(lbPaletteColors.Font, FontStyle.Bold))
							using (Brush brush = new SolidBrush(GraphicUtils.XorColor(color))) {
								string text = IsCurrentPalettePredefined() ? ChartLocalizer.GetString(ChartStringId.MsgPaletteEditingIsNotAllowed) : 
																			 ChartLocalizer.GetString(ChartStringId.MsgPaletteDoubleClickToEdit);
								e.Graphics.DrawString(text, font, brush, e.Bounds, sf); 
							}
					}
					ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
				}
			}
		}
		void btnAddColor_Click(object sender, EventArgs e) {
			Palette palette = CurrentPalette;
			if (palette != null) {
				palette.Add(Palette.EmptyPaletteColor);
				ColorCountChanged(palette.Count - 1);
			}
		}
		void btnRemoveColor_Click(object sender, EventArgs e) {
			Palette palette = CurrentPalette;
			if (palette != null) {
				int index = lbPaletteColors.SelectedIndex;
				if (index != -1) {
					palette.RemoveAt(index);
					((IPalette)palette).AddDefaultColorIfEmpty();
					ColorCountChanged(index);
				}
			}
		}
		void btnClearColor_Click(object sender, EventArgs e) {
			Palette palette = CurrentPalette;
			if (palette != null) {
				palette.Clear();
				((IPalette)palette).AddDefaultColorIfEmpty();
				ColorCountChanged(0);
			}
		}
		void sbLoad_Click(object sender, EventArgs e) {
			using (OpenFileDialog ofdPalette = CreateOpenFileDialog()) { 
				DialogResult result = ofdPalette.ShowDialog();
				if (result == DialogResult.OK) {
					using (Stream stream = ofdPalette.OpenFile()) {
						Palette palette = PaletteSerializer.LoadFromStream(chartContainer, stream);
						if (palette != null) {
							foreach (string paletteName in repository.PaletteNames) {
								if (paletteName == palette.Name) {
									PaletteSerializer.SetPaletteName(palette, NameGenerator.UniqueName(palette.Name, repository.PaletteNames));
									break;
								}
							}
							AddPalette(palette);
						}
					}
				}
			}
		}
		void sbSave_Click(object sender, EventArgs e) {
			if (CurrentPalette == null)
				return;
			using (SaveFileDialog sfdPalette = CreateSaveFileDialog()) {
				sfdPalette.FileName = CurrentPalette.Name;
				DialogResult result = sfdPalette.ShowDialog(this);
				if (result == DialogResult.OK && !string.IsNullOrEmpty(sfdPalette.FileName)) {
					using (Stream stream = sfdPalette.OpenFile()) {
						PaletteSerializer.SaveToStream(CurrentPalette, stream);
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing && palettesFont != null) {
				palettesFont.Dispose();
				palettesFont = null;
			}
			base.Dispose(disposing);
		}
	}
}
