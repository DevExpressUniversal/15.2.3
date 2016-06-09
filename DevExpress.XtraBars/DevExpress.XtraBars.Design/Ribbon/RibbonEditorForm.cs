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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Design.Frames;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Ribbon.Gallery;
namespace DevExpress.XtraBars.Ribbon.Design {
	public class RibbonEditorForm : BaseDesignerForm {
		public const string RibbonSettings = "Software\\Developer Express\\Designer\\XtraBars\\Ribbon\\";
		object component = null;
		public RibbonEditorForm(object component) : this() {
			this.component = component;
		}
		public RibbonEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("Ribbon", typeof(RibbonControl), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new RibbonDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		protected virtual void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(1000, 600);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "RibbonEditorForm";
			this.Text = "Ribbon Control Designer";
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return RibbonSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(RibbonEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		public object ComponentObj { get { return component; } set { component = value; } }
		public virtual bool IsGallery() {
			if(ComponentObj is RibbonGalleryBarItem || ComponentObj is RibbonGalleryBarItemLink || ComponentObj is GalleryDropDown || ComponentObj is GalleryControl) return true;
			return false;
		}
		public virtual bool IsPopupMenu() {
			if(ComponentObj is PopupMenuBase) return true;
			return false;
		}
		public override void InitEditingObject(object editingObject) {
			base.InitEditingObject(editingObject);
			if(IsGallery()) UpdateActiveDesigner("Gallery");
			else if(IsPopupMenu()) UpdateActiveDesigner("Sub Menus & Popup Menus");
			else if(IsMiniToolbar()) UpdateActiveDesigner("Ribbon MiniToolbar Items");
		}
		bool IsMiniToolbar() {
			return ComponentObj is RibbonMiniToolbar;
		}
	}
	public class RibbonDesigner : BaseDesigner {
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		static ImageCollection groupImages;
		object component;
		static RibbonDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Ribbon.Images.icons.png", typeof(RibbonDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Ribbon.Images.icons-small.png", typeof(RibbonDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Ribbon.Images.navBarGroupIcons16x16.png", typeof(RibbonDesigner).Assembly, new Size(16, 16));
		}
		public RibbonDesigner() {}
		public RibbonDesigner(object component) { 
			this.component = component; 
		}
		public RibbonControl Ribbon { get { return component as RibbonControl; } }
		public StandaloneGallery Gallery { get { return component as StandaloneGallery; } }
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		protected override void CreateGroups() {
			Groups.Clear();
			if(Ribbon != null) {
				DesignerGroup group = Groups.Add("Toolbars", "Toolbars.", GetDefaultGroupImage(0), true);
				group.Add("Ribbon Items", "Manage ribbon items (add, delete, modify ribbon pages, groups and bar items).", typeof(RibbonItemsManagerBase), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
				group.Add("Quick Access Toolbar", "Manage Quick Access Toolbar items", typeof(QuickAccessToolbarManager), GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
				group.Add("Ribbon PageHeader Items", "Manage Ribbon PageHeader items", typeof(PageHeaderItemsManager), GetDefaultLargeImage(8), GetDefaultSmallImage(2), null);
				if(StatusBarHelper.GetStatusBar(Ribbon) != null)
					group.Add("Status Bar", "Manage StatusBar items", typeof(StatusBarManager), GetDefaultLargeImage(4), GetDefaultSmallImage(10), null);
				group.Add("Ribbon MiniToolbar Items", "Manage Ribbon MiniToolbar items", typeof(RibbonMiniToolbarItemsManager), GetDefaultLargeImage(9), GetDefaultSmallImage(3), null);
				group = Groups.Add("Reduce Operations", "Reduce Operations", GetDefaultGroupImage(1), true);
				group.Add("Reduce Operations", "Customize reduce operations priority", typeof(ReduceOperationsFrame), GetDefaultLargeImage(2), GetDefaultSmallImage(4), null);
				group = Groups.Add("Categories", "Categories.", GetDefaultGroupImage(2), true);
				group.Add("Categories", "Manages the collection of categories. Allows moving items between categories.", typeof(CategoriesFrame), GetDefaultLargeImage(2), GetDefaultSmallImage(5), null);
				group = Groups.Add("Menus", "PopupMenus and SubItems.", GetDefaultGroupImage(3), true);
				group.Add("Sub Menus & Popup Menus", "Customize sub menus and popup menus", typeof(RibbonPopupItemsEditor), GetDefaultLargeImage(3), GetDefaultSmallImage(6), null);
			}
			if(Ribbon != null || Gallery != null) {
				DesignerGroup galleryGroup = Groups.Add("Gallery Controls", "Image gallery controls.", GetDefaultGroupImage(4), true);
				galleryGroup.Add("Gallery", "Customize galleries, gallery containers and items.", typeof(GalleryManager), GetDefaultLargeImage(6), GetDefaultSmallImage(7), null);
			}
			if(Ribbon != null) {
				DesignerGroup keyTipGroup = Groups.Add("Ribbon KeyTips", "Ribbon key tip items", GetDefaultGroupImage(5), true);
				keyTipGroup.Add("KeyTips", "Customize ribbon key tips", typeof(KeyTipManager), GetDefaultLargeImage(7), GetDefaultSmallImage(8), null);
				DesignerGroup editorGroup = Groups.Add("Repository", "Editors for in-place editing.", GetDefaultGroupImage(6), true);
				editorGroup.Add("Repository Editor", "Adjusts the editors collection for bar items (create and customize).", typeof(DevExpress.XtraBars.Design.PersistentRepositoryEditor), GetDefaultLargeImage(5), GetDefaultSmallImage(9), null);
			}
		}
	}
}
