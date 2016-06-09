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
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Frames;
using DevExpress.Utils.Design;
using DevExpress.Utils;
namespace DevExpress.XtraVerticalGrid.Design {
	public class frmMain : BaseDesignerForm {
		private System.ComponentModel.IContainer components = null;
		public frmMain() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo(DevExpress.Utils.About.ProductKind.DXperienceWin.ToString(), typeof(DevExpress.XtraVerticalGrid.VGridControl),
				DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new VertGridDesigner();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(984, 662);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MinimumSize = new System.Drawing.Size(700, 450);
			this.Name = "frmMain";
			this.Text = "VerticalGrid Designer";
			this.ResumeLayout(false);
		}
		#endregion
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		protected override void InitFrame(string caption, Bitmap bitmap) {
			XF.InitFrame(EditingComponent, caption, bitmap);
		}
		protected override string RegistryStorePath { get { return "Software\\Developer Express\\Designer\\XtraVertGrid\\"; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(frmMain).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class VertGridDesigner : BaseDesigner {
		static ImageCollection largeImages = null;
		static ImageCollection smallImages = null;
		static ImageCollection groupImages = null;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		static VertGridDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Design.Images.icons32x32.png", typeof(VertGridDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Design.Images.icons16x16.png", typeof(VertGridDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Design.Images.navBarGroupIcons16x16.png", typeof(VertGridDesigner).Assembly, new Size(16, 16));
		}
		internal static string groupMainDescription = "Main Vertical Grid settings (Rows, Editors).";
		internal static string rowsDescription = "Adjust Rows collection of the current Vertical Grid (add, delete or modify properties).";
		internal static string editorsDescription = "Adjust the editors used for in-place editing.";
		internal static string layoutDescription = "Adjust the current look & feel of the Vertical Grid (and save/load a XtraVerticalGrid Layout to/from a file).";
		internal static string groupAppearanceDescription = "Adjust the appearance of the current Vertical Grid.";
		internal static string appearanceDescription = "Manage the appearances for the current Vertical Grid.";
		internal static string styleSchemesDescription = "Apply a painting scheme to the current Vertical Grid.";
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group;
			group = Groups.Add("Main", groupMainDescription, GetDefaultGroupImage(0), true); 
			group.Add("Rows", rowsDescription, "DevExpress.XtraVerticalGrid.Frames.RowDesigner", GetDefaultLargeImage(0), GetDefaultSmallImage(0));
			group.Add("In-place Editor Repository", editorsDescription, "DevExpress.XtraVerticalGrid.Frames.PersistentRepositoryVGridEditor", GetDefaultLargeImage(1), GetDefaultSmallImage(1));
			group.Add("Layout Designer", layoutDescription, "DevExpress.XtraVerticalGrid.Frames.Layouts", GetDefaultLargeImage(2), GetDefaultSmallImage(2));
			group = Groups.Add("Appearances", groupAppearanceDescription, GetDefaultGroupImage(1), true);
			group.Add("Appearances", appearanceDescription, "DevExpress.XtraVerticalGrid.Frames.AppearancesDesigner", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add("Style Schemes", styleSchemesDescription, "DevExpress.XtraVerticalGrid.Frames.SchemeDesigner", GetDefaultLargeImage(4), GetDefaultSmallImage(4));
		}
	}
	public class frmMainProperties : frmMain {
		protected override void CreateDesigner() {
			ActiveDesigner = new VertGridDesignerProperties();
		}
	}
	public class VertGridDesignerProperties : VertGridDesigner {
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group;
			group = Groups.Add("Main", groupMainDescription, GetDefaultGroupImage(0), true);
			group.Add("Rows", rowsDescription, "DevExpress.XtraVerticalGrid.Frames.RowPropertiesDesigner", GetDefaultLargeImage(0), GetDefaultSmallImage(0));
			group.Add("In-place Editor Repository", editorsDescription, "DevExpress.XtraVerticalGrid.Frames.PersistentRepositoryVGridEditor", GetDefaultLargeImage(1), GetDefaultSmallImage(1));
			group.Add("Default Editors", "Customize the editors that are used for in-place editing by default.", "DevExpress.XtraVerticalGrid.Frames.DefaultEditorsDesigner", GetDefaultLargeImage(6), GetDefaultSmallImage(6));
			group = Groups.Add("Appearances", groupAppearanceDescription, GetDefaultGroupImage(1), true);
			group.Add("Appearances", appearanceDescription, "DevExpress.XtraVerticalGrid.Frames.AppearancesDesigner", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add("Style Schemes", styleSchemesDescription, "DevExpress.XtraVerticalGrid.Frames.SchemePropertiesDesigner", GetDefaultLargeImage(4), GetDefaultSmallImage(4));
		}
	}
}
