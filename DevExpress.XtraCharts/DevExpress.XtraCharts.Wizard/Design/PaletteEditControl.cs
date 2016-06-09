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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class PaletteEditControl : XtraUserControl {
		const int MaxPalleteEntiesInListBoxItem = 12;
		IWindowsFormsEditorService edSvc;
		Chart chart;
		PaletteRepository paletteRepository;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Chart Chart { 
			get { return chart; } 
			set { 
				chart = value; 
				PaletteRepository = chart.PaletteRepository;
				SelectedPalette = chart.Palette;
			} 
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PaletteRepository PaletteRepository {
			get { return paletteRepository; }
			set { 
				paletteRepository = value;
				FillPalettes();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Palette SelectedPalette { 
			get { return lbPalettes.SelectedValue as Palette; }
			set { 
				if (value == null) {
					if (lbPalettes.ItemCount > 0)
						lbPalettes.SelectedIndex = 0;
				}
				else
					lbPalettes.SelectedValue = value; 
			}
		}
		public event EventHandler OnPaletteChanged;
		public event EventHandler OnNeedClose;
		public PaletteEditControl(IWindowsFormsEditorService edSvc) {
			this.edSvc = edSvc;
			InitializeComponent();
		}
		public PaletteEditControl() : this(null) {
		}
		public void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			lbPalettes.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			btnEdit.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		void FillPalettes() {
			string[] names = paletteRepository.PaletteNames;
			Image[] images = new Image[names.Length];
			Size imageSize = Size.Empty;
			lbPalettes.Items.BeginUpdate();
			lbPalettes.Items.Clear();
			for (int i = 0; i < names.Length; i++) {
				string name = names[i];
				Palette palette = paletteRepository[name];
				Image image = PaletteUtils.CreateEditorImage(palette, MaxPalleteEntiesInListBoxItem);
				images[i] = image;
				if (image.Width > imageSize.Width)
					imageSize.Width = image.Width;
				if (image.Height > imageSize.Height)
					imageSize.Height = image.Height;
				lbPalettes.Items.Add(palette, i);
			}
			lbPalettes.Items.EndUpdate();
			paletteImages.BeginInit();
			paletteImages.Clear();
			paletteImages.ImageSize = imageSize;
			for (int i = 0; i < images.Length; i++) {
				Image image = images[i];
				Bitmap newImage = null;
				if (image.Size != imageSize) {
					try {
						newImage = new Bitmap(imageSize.Width, imageSize.Height);
						using (Graphics gr = Graphics.FromImage(newImage))
							gr.DrawImage(image, Point.Empty);
						image.Dispose();
					}
					catch {
						if (newImage != null) {
							newImage.Dispose();
							newImage = null;
						}
					}
				}
				paletteImages.AddImage(newImage == null ? image : newImage);
			}
			paletteImages.EndInit();
		}
		internal void RaisePaletteChanged() {
			if (OnPaletteChanged != null)
				OnPaletteChanged(this, EventArgs.Empty);
		}
		void RaiseNeedClose() {
			if (edSvc != null)
				edSvc.CloseDropDown();
			if (OnNeedClose != null) 
				OnNeedClose(this, EventArgs.Empty);
		}
		void lbPalettes_SelectedIndexChanged(object sender, EventArgs e) {
			OnSelectedPaletteChanged();
		}
		internal virtual void OnSelectedPaletteChanged() {
			if (chart != null)
				chart.Palette = SelectedPalette;
			RaisePaletteChanged();
		}
		void lbPalettes_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left)
				RaiseNeedClose();
		}
		void lbPalettes_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				RaiseNeedClose();
		}
		void btnEdit_Click(object sender, EventArgs e) {
			Palette palette = SelectedPalette;
			IChartContainer container = chart != null ? chart.Container : null;
			using (PaletteEditorForm form = new PaletteEditorForm(container, paletteRepository)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel.ParentLookAndFeel;
				form.Location = ControlUtils.CalcLocation(Cursor.Position, Cursor.Position, form.Size);
				form.TopMost = true;
				form.CurrentPalette = palette;
				DialogResult result = form.ShowDialog();
				FillPalettes();
				SelectedPalette = result == DialogResult.OK ? form.CurrentPalette : palette;
			}
		}
	}
}
