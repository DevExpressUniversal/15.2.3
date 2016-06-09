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
using DevExpress.XtraTreeList;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
namespace DevExpress.XtraTreeList.Design {
	public class frmEditor : BaseDesignerForm {
		private System.ComponentModel.IContainer components = null;
		public frmEditor() {
			InitializeComponent();
			ProductInfo = new Utils.About.ProductInfo("XtraTreeList", typeof(TreeList), Utils.About.ProductKind.DXperienceWin, Utils.About.ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new TListDesigner();
		}
		public override void InitEditingObject(object editingObject) {
			base.InitEditingObject(editingObject);
			UpdatePainStyleItems();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(984, 662);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MinimumSize = new System.Drawing.Size(700, 500);
			this.Name = "frmEditor";
			this.Text = "TreeList Designer";
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
		protected override string RegistryStorePath { get { return "Software\\Developer Express\\Designer\\XtraTreeList\\"; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(frmEditor).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		void UpdatePainStyleItems() {
			foreach(NavBarItem item in NavBar.Items) {
				DesignerItem dItem = item.Tag as DesignerItem;
				if(dItem == null) continue;
				if(dItem.FrameTypeName == typeof(DevExpress.XtraTreeList.Frames.SchemeDesigner).FullName) {
					TreeList currentTreeList = EditingComponent as TreeList;
					item.Visible = currentTreeList != null && currentTreeList.LookAndFeel.ActiveStyle != DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
				}
			}
		}
	}
	public class TListDesigner : BaseDesigner {
		static ImageCollection largeImages = null;
		static ImageCollection smallImages = null;
		static ImageCollection groupImages = null;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		static TListDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Design.Images.icons32x32.png", typeof(TListDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Design.Images.icons16x16.png", typeof(TListDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Design.Images.navBarGroupIcons16x16.png", typeof(TListDesigner).Assembly, new Size(16, 16));
		}
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group;
			group = Groups.Add("Main", "Main TreeList settings(Columns, Editors).", GetDefaultGroupImage(0), true); 
			group.Add("Columns", "Adjust the Column collection of the current TreeList, assign in-place editors to columns and specify total summaries.", "DevExpress.XtraTreeList.Frames.ColumnDesigner", GetDefaultLargeImage(0), GetDefaultSmallImage(0));
			group.Add("Bands", "Adjust the Band collection of the current TreeList.", "DevExpress.XtraTreeList.Frames.BandDesigner", GetDefaultLargeImage(9), GetDefaultSmallImage(9));
			group.Add("In-place Editor Repository", "Adjust the editors used for in-place editing.", "DevExpress.XtraTreeList.Frames.PersistentRepositoryTreeListEditor", GetDefaultLargeImage(1), GetDefaultSmallImage(1));
			group.Add("Layout Designer", "Allows adjusting the TreeList look & feel(and saving/loading the XtraTreeListLayout to/from a file).", "DevExpress.XtraTreeList.Frames.Layouts", GetDefaultLargeImage(2), GetDefaultSmallImage(2));			
			group = Groups.Add("Appearances", "Adjust the appearance of the current TreeList.", GetDefaultGroupImage(1), false);
			group.Add("Appearances", "Manage the appearances for the current TreeList.", "DevExpress.XtraTreeList.Frames.AppearancesDesigner", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add("Style Schemes", "Apply a painting scheme to the current TreeList.", "DevExpress.XtraTreeList.Frames.SchemeDesigner", GetDefaultLargeImage(4), GetDefaultSmallImage(4));
			group.Add("Format Rules", "You can add, remove or modify items to apply styles to columns or entire rows based upon a custom format rule.", typeof(DevExpress.XtraTreeList.Frames.FormatConditionFrame), GetDefaultLargeImage(8), GetDefaultSmallImage(8), null); 
			group = Groups.Add("Printing", "Printing option management for the current TreeList.", GetDefaultGroupImage(2), false);
			group.Add("Print Appearances", "Adjust the print appearances of the current view.", "DevExpress.XtraTreeList.Frames.PrintAppearancesDesigner", GetDefaultLargeImage(7), GetDefaultSmallImage(7));
			group.Add("Print Settings", "Adjust the printing settings of the current TreeList.", "DevExpress.XtraTreeList.Frames.TreeListPrinting", GetDefaultLargeImage(5), GetDefaultSmallImage(5)); 
		}
	}
}
