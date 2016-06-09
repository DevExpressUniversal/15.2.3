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

using System.Drawing;
namespace DevExpress.XtraCharts.Design
{
	partial class PaletteEditControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (Image image in paletteImages.Images)
					image.Dispose();
				if  (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteEditControl));
			this.lbPalettes = new DevExpress.XtraEditors.ImageListBoxControl();
			this.paletteImages = new DevExpress.Utils.ImageCollection(this.components);
			this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.lbPalettes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.paletteImages)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbPalettes, "lbPalettes");
			this.lbPalettes.ImageList = this.paletteImages;
			this.lbPalettes.Name = "lbPalettes";
			this.lbPalettes.SelectedIndexChanged += new System.EventHandler(this.lbPalettes_SelectedIndexChanged);
			this.lbPalettes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbPalettes_KeyDown);
			this.lbPalettes.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbPalettes_MouseClick);
			this.paletteImages.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("paletteImages.ImageStream")));
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.lbPalettes);
			this.Name = "PaletteEditControl";
			((System.ComponentModel.ISupportInitialize)(this.lbPalettes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.paletteImages)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.ImageListBoxControl lbPalettes;
		private DevExpress.XtraEditors.SimpleButton btnEdit;
		private DevExpress.Utils.ImageCollection paletteImages;
	}
}
