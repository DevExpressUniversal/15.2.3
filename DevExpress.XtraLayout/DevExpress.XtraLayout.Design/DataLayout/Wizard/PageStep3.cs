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

using DevExpress.Utils;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Controls;
using System.Reflection;
namespace DevExpress.XtraDataLayout.DesignTime {
	[ToolboxItem(false)]
	public class WizardPageStep3 : LayoutBasedWizardPage {
		public WizardPageStep3() {
			InitTexts();
			pictureBox1.Image = AnimateImage;
			columnCountItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			useGroupNameAttributeItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			selectAllLCI.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
		}
		private void InitTexts() {
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPageStep3));
			this.subtitleLabel.Text = resources.GetString("subtitleLabel.Text");
			this.titleLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.titleLabel.Appearance.Options.UseFont = true;
			this.titleLabel.Text = "How to further customize the layout?";
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(2, 2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(442, 275);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.Parent = panelControl1;
			this.headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Design.DataLayout.Wizard.layoutcustomization.gif", Assembly.GetExecutingAssembly());
		}
		protected override bool OnSetActive() {
			Wizard.WizardButtons = WizardButton.Finish | WizardButton.Back;
			Wizard.Text = "Step 3. Customize Layout";
			return true;
		}
		private System.Windows.Forms.PictureBox pictureBox1;
		static Image _image;
		protected static Image AnimateImage {
			get {
				if(_image == null) {
					_image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Design.DataLayout.Wizard.collage.gif", Assembly.GetExecutingAssembly());
				}
				return _image;
			}
		}
	}
}
