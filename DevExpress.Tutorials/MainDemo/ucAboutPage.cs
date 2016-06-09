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
using DevExpress.DXperience.Demos;
using DevExpress.Utils.About;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Frames;
namespace DevExpress.Tutorials {
	public partial class ucAboutPage : TutorialControlBase {
		public ucAboutPage() {
			InitializeComponent();
			versionControl1.SetProduct(ProductKind);
			InitData();
			UpdateImages();
			UserLookAndFeel.Default.StyleChanged += new EventHandler(Default_StyleChanged);
			if(!ShowStartButton) {
				esiStart1.Visibility = 
				esiStart2.Visibility = 
				esiStart3.Visibility = 
				lciStart.Visibility = 
				sliStart.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			}
			simpleButton1.Image = MainFormHelper.GetImage("ActiveDemo", ImageSize.Large32);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			if(disposing)
				UserLookAndFeel.Default.StyleChanged -= new EventHandler(Default_StyleChanged);
			base.Dispose(disposing);
		}
		protected virtual ProductKind ProductKind { get { return ProductKind.Default; } }
		protected virtual bool ShowStartButton { get { return true; } }
		protected internal override bool ShowCaption { get { return false; } }
		protected virtual string ProductText { get { return string.Empty; } }
		protected virtual Image ProductImage { get { return null; } }
		protected virtual Image ProductImageLight { get { return null; } }
		void Default_StyleChanged(object sender, EventArgs e) {
			UpdateImages();
		}
		public static string GetLogoImageName() {
			if(FrameHelper.IsDarkSkin(UserLookAndFeel.Default))
				return "DevExpress.Utils.XtraFrames.dx-logo-light.png";
			return "DevExpress.Utils.XtraFrames.dx-logo.png";
		}
		void UpdateImages() {
			peLogo.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(GetLogoImageName(), typeof(ApplicationCaption).Assembly);
			Image img = FrameHelper.IsDarkSkin(UserLookAndFeel.Default) ? ProductImageLight : ProductImage;
			if(FrameHelper.IsDarkSkin(UserLookAndFeel.Default) && img == null)
				img = ProductImage;
			peProduct.Image = img;
		}
		void InitData() {
			peAwards.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.awards.png", typeof(ucAboutPage).Assembly);
			lcCopyright.Text = AboutHelper.CopyRight;
			lcAbout.Text = ProductText;
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			RibbonMainForm form = this.FindForm() as RibbonMainForm;
			if(form != null)
				form.StartDemo();
		}
	}
}
