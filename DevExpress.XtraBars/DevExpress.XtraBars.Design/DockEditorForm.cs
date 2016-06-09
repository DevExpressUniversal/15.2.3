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
namespace DevExpress.XtraBars.Docking.Design {
	public class DockEditorForm : BaseDesignerForm {
		public const string DockSettings = "Software\\Developer Express\\Designer\\XtraBars\\Docking\\";
		public DockEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("DockManager", typeof(DockManager), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new DockingDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).BeginInit();
			this.SuspendLayout();
			this.pnlFrame.Size = new System.Drawing.Size(415, 353);
			this.ClientSize = new System.Drawing.Size(557, 375);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "DockEditorForm";
			this.Text = "DockManager Designer";
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return DockSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(DockEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class DockingDesigner : BaseDesigner {
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		static ImageCollection groupImages;
		static DockingDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons32x32.png", typeof(DockingDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons16x16.png", typeof(DockingDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.navBarGroupIcons16x16.png", typeof(DockingDesigner).Assembly, new Size(16, 16));
		}
		public DockingDesigner() {}
		public DockingDesigner(object component) {}
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Dock Panels", "Dock panels.", GetDefaultGroupImage(0), false);
			group.Add("Dock Panels", "Adjusts the new dock panel collection (add, remove, customize).", typeof(DevExpress.XtraBars.Docking.Design.Frames.DockWindowsFrame), GetDefaultLargeImage(6), GetDefaultSmallImage(6), null);
			group = Groups.Add("Layouts", "Layout management (save and load).", GetDefaultGroupImage(1), false);
			group.Add("Layout", "Save and restore the DockManager layout", typeof(DevExpress.XtraBars.Docking.Design.Frames.DockPanelLayoutFrame), GetDefaultLargeImage(7), GetDefaultSmallImage(7), null);
		}
		protected DockManagerDesigner GetDesigner(BaseDesignerForm form) {
			if(form == null) return null;
			IComponent comp = form.EditingComponent as IComponent;
			if(comp == null || comp.Site == null) return null;
			IDesignerHost host = comp.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			return host.GetDesigner(comp) as DockManagerDesigner;
		}
	}
}
