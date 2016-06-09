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
using DevExpress.XtraNavBar;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
namespace DevExpress.XtraNavBar.Design {
	public class NavBarEditorForm : BaseDesignerForm {
		public const string NavBarSettings = "Software\\Developer Express\\Designer\\XtraNavBar\\";
		public NavBarEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("XtraNavBar", typeof(NavBarControl), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
			this.ClientSize = new System.Drawing.Size(750, 400);		
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new NavBarDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).BeginInit();
			this.SuspendLayout();
			this.pnlFrame.Size = new System.Drawing.Size(650, 494);
			this.ClientSize = new System.Drawing.Size(792, 516);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "NavBarEditorForm";
			this.Text = "NavBar Designer";
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return NavBarSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(NavBarEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class NavBarDesigner : BaseDesigner {
		static DevExpress.Utils.ImageCollection largeImages = null;
		static DevExpress.Utils.ImageCollection smallImages = null;
		static DevExpress.Utils.ImageCollection groupImages = null;
		NavBarControl editingNavBar;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		static NavBarDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraNavBar.Design.Images.icons32x32.png", typeof(NavBarDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraNavBar.Design.Images.icons16x16.png", typeof(NavBarDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraNavBar.Design.Images.navBarGroupIcons16x16.png", typeof(NavBarDesigner).Assembly, new Size(16, 16));
		}
		public NavBarDesigner(object editingComponent) {
			this.editingNavBar = editingComponent as NavBarControl;
		}
		protected NavBarControl EditingNavBar { get { return editingNavBar; } }
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Main", "Main settings(Groups, Items, Appearances).", GetDefaultGroupImage(0), true);
			group.Add("Groups/Items/Links", "Manage groups, items and create items' links.", typeof(DevExpress.XtraNavBar.Frames.GroupItemDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
			group.Add("View Chooser", "Select the NavBar's view.", typeof(DevExpress.XtraNavBar.Frames.Views), GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group.Add("Appearances", "Adjust appearance settings for the NavBar.", typeof(DevExpress.XtraNavBar.Frames.AppearancesDesigner), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
			group = Groups.Add("Layout", "Save the layout of the NavBar to and restore it from an XML file.", GetDefaultGroupImage(1), false);
			group.Add("Layout", "Save and restore the NavBarControl layout", typeof(DevExpress.XtraNavBar.Frames.NavBarLayoutFrame), GetDefaultLargeImage(5), GetDefaultSmallImage(5), null);
		}
	}
	public class ImageHelper {
		public static void SaveImageList(ImageList list, string fileName) {
			if(list.Images.Count == 0) return;
			Size listSize = new Size(list.ImageSize.Width * list.Images.Count, list.ImageSize.Height);
			Size isize = list.ImageSize;
			Bitmap bmp = new Bitmap(listSize.Width, listSize.Height);
			Graphics g = Graphics.FromImage(bmp);
			g.FillRectangle(Brushes.Magenta, new Rectangle(Point.Empty, listSize));
			for(int n = 0; n < list.Images.Count; n++) {
				Rectangle r = new Rectangle(n * isize.Width, 0, isize.Width, isize.Height);
				list.Draw(g, r.X, r.Y, n);
			}
			g.Dispose();
			bmp.Save(fileName);
			bmp.Dispose();
		}
	}
}
