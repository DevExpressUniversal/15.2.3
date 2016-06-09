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

namespace DevExpress.XtraBars.Ribbon.Design {
	partial class KeyTipManagerToolbar {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyTipManagerToolbar));
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.generateUserKeyTipsInGroup = new DevExpress.XtraBars.BarButtonItem();
			this.clearUserKeyTipsInGroup = new DevExpress.XtraBars.BarButtonItem();
			this.clearAllUserKeyTips = new DevExpress.XtraBars.BarButtonItem();
			this.generateAllUserKeyTips = new DevExpress.XtraBars.BarButtonItem();
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			this.SuspendLayout();
			this.ribbonControl1.ApplicationButtonKeyTip = "";
			this.ribbonControl1.ApplicationIcon = null;
			this.ribbonControl1.Controller = this.barAndDockingController1;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.generateUserKeyTipsInGroup,
			this.clearUserKeyTipsInGroup,
			this.clearAllUserKeyTips,
			this.generateAllUserKeyTips});
			this.ribbonControl1.LargeImages = this.imageCollection1;
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.Margin = new System.Windows.Forms.Padding(0);
			this.ribbonControl1.MaxItemId = 4;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.ribbonPage1});
			this.ribbonControl1.SelectedPage = this.ribbonPage1;
			this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
			this.ribbonControl1.Size = new System.Drawing.Size(744, 95);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.generateUserKeyTipsInGroup.Caption = "Generate User Key Tips in Group";
			this.generateUserKeyTipsInGroup.Id = 0;
			this.generateUserKeyTipsInGroup.LargeImageIndex = 0;
			this.generateUserKeyTipsInGroup.Name = "generateUserKeyTipsInGroup";
			this.generateUserKeyTipsInGroup.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.clearUserKeyTipsInGroup.Caption = "Clear User Key Tips in Group";
			this.clearUserKeyTipsInGroup.Id = 1;
			this.clearUserKeyTipsInGroup.LargeImageIndex = 2;
			this.clearUserKeyTipsInGroup.Name = "clearUserKeyTipsInGroup";
			this.clearUserKeyTipsInGroup.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.clearAllUserKeyTips.Caption = "Clear All User Key Tips";
			this.clearAllUserKeyTips.Id = 2;
			this.clearAllUserKeyTips.LargeImageIndex = 3;
			this.clearAllUserKeyTips.Name = "clearAllUserKeyTips";
			this.clearAllUserKeyTips.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.generateAllUserKeyTips.Caption = "Generate All User Key Tips";
			this.generateAllUserKeyTips.Id = 3;
			this.generateAllUserKeyTips.LargeImageIndex = 1;
			this.generateAllUserKeyTips.Name = "generateAllUserKeyTips";
			this.generateAllUserKeyTips.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.imageCollection1.ImageSize = new System.Drawing.Size(32, 32);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.ribbonPageGroup1});
			this.ribbonPage1.KeyTip = "";
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ribbonPage1";
			this.ribbonPageGroup1.ItemLinks.Add(this.generateUserKeyTipsInGroup);
			this.ribbonPageGroup1.ItemLinks.Add(this.generateAllUserKeyTips);
			this.ribbonPageGroup1.ItemLinks.Add(this.clearUserKeyTipsInGroup);
			this.ribbonPageGroup1.ItemLinks.Add(this.clearAllUserKeyTips);
			this.ribbonPageGroup1.KeyTip = "";
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			this.ribbonPageGroup1.ShowCaptionButton = false;
			this.ribbonPageGroup1.Text = "User KeyTips";
			this.ribbonPageGroup1.AllowTextClipping = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ribbonControl1);
			this.Name = "KeyTipManagerToolbar";
			this.Size = new System.Drawing.Size(744, 95);
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private BarButtonItem generateUserKeyTipsInGroup;
		private BarButtonItem clearUserKeyTipsInGroup;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private BarButtonItem clearAllUserKeyTips;
		private BarButtonItem generateAllUserKeyTips;
		private BarAndDockingController barAndDockingController1;
		private DevExpress.Utils.ImageCollection imageCollection1;
	}
}
