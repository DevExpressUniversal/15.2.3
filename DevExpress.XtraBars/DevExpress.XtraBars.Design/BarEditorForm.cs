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
namespace DevExpress.XtraBars.Design {
	public class BarEditorForm : BaseDesignerForm {
		public const string BarSettings = "Software\\Developer Express\\Designer\\XtraBars\\";
		public BarEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("BarManager", typeof(BarManager), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new BarAndDockingDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).BeginInit();
			this.SuspendLayout();
			this.pnlFrame.Size = new System.Drawing.Size(603, 434);
			this.ClientSize = new System.Drawing.Size(745, 456);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "BarEditorForm";
			this.Text = "BarManager Designer";
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return BarSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(BarEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class BarAndDockingDesigner : BaseDesigner {
		bool showBarItems = true;
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		static ImageCollection groupImages;
		object component;
		static BarAndDockingDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons32x32.png", typeof(BarAndDockingDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons16x16.png", typeof(BarAndDockingDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.navBarGroupIcons16x16.png", typeof(BarAndDockingDesigner).Assembly, new Size(16, 16));
		}
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		public BarAndDockingDesigner(object component) {
			this.component = component;
		}
		protected BarManager Manager { get { return component as BarManager; } }
		protected override void CreateGroups() {
			Groups.Clear();
			if(this.showBarItems) CreateBarGroups();
			if(Manager != null && Manager.DockManager != null) 
				CreateDockManagerGroup();
		}
		protected virtual void CreateDockManagerGroup() {
			DesignerGroup group = Groups.Add("DockManager", "Dock panels.", GetDefaultGroupImage(0), false);
			group.Add("DockManager", "Adjusts the dock panel collection (add, remove, customize).", typeof(DevExpress.XtraBars.Docking.Design.Frames.DockWindowsFrame), GetDefaultLargeImage(5), GetDefaultSmallImage(6), null);
		}
		protected virtual void CreateBarGroups() {
			DesignerGroup group = Groups.Add("Toolbars", "Toolbars and categories.", GetDefaultGroupImage(2), true);
			group.Add("Toolbars", "Manages toolbars (add, delete, modify bars, customize items).", typeof(BarsFrame), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
			group.Add("Categories", "Manages the collection of categories. Allows moving items between categories.", typeof(CategoriesFrame), GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group.Add("Paint Styles", "Customize the paint styles of bars.", typeof(PaintStyleFrame), GetDefaultLargeImage(2), GetDefaultSmallImage(2), null);
			group = Groups.Add("Repository", "Editors for in-place editing.", GetDefaultGroupImage(3), false);
			group.Add("Repository Editor", "Adjusts the editors collection for bar items (create and customize).", typeof(DevExpress.XtraBars.Design.PersistentRepositoryEditor), GetDefaultLargeImage(3), GetDefaultSmallImage(3), null);
			group = Groups.Add("Layouts", "Layout management (save and load).", GetDefaultGroupImage(1), false);
			group.Add("Layout", "Save and restore the BarManager layout", typeof(DevExpress.XtraBars.Design.BarManagerLayoutFrame), GetDefaultLargeImage(7), GetDefaultSmallImage(7), null);
		}
		protected virtual void CreateDockGroups() {
		}
		protected BarManagerDesigner GetDesigner(BaseDesignerForm form) {
			if(form == null) return null;
			IComponent comp = form.EditingComponent as IComponent;
			if(comp == null || comp.Site == null) return null;
			IDesignerHost host = comp.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			return host.GetDesigner(comp) as BarManagerDesigner;
		}
	}
}
