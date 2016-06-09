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
using DevExpress.XtraBars.Navigation;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Navigation.Frames;
namespace DevExpress.XtraBars.Navigation.Design {
	public class AccordionControlEditorForm : BaseDesignerForm {
		public const string AccordionControlSettings = "Software\\Developer Express\\Designer\\XtraBars\\AccordionControlDesigner\\";
		public AccordionControlEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("XtraBars", typeof(AccordionControl), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
			this.ClientSize = new System.Drawing.Size(750, 400);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new AccordionDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).BeginInit();
			this.SuspendLayout();
			this.pnlFrame.Size = new System.Drawing.Size(650, 494);
			this.ClientSize = new System.Drawing.Size(792, 516);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "AccordionControlEditorForm";
			this.Text = "AccordionControl Designer";
			((System.ComponentModel.ISupportInitialize)(this.pnlFrame)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return AccordionControlSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(AccordionControlEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		protected override void Dispose(bool disposing) {
			AccordionControl control = EditingComponent as AccordionControl;
			if(control != null)
				control.Refresh();
			base.Dispose(disposing);
		}
	}
	public class AccordionDesigner : BaseDesigner {
		static DevExpress.Utils.ImageCollection largeImages = null;
		static DevExpress.Utils.ImageCollection smallImages = null;
		static DevExpress.Utils.ImageCollection groupImages = null;
		AccordionControl editingAccordionControl;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		static AccordionDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons32x32.png", typeof(AccordionDesigner).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.icons16x16.png", typeof(AccordionDesigner).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Images.navBarGroupIcons16x16.png", typeof(AccordionDesigner).Assembly, new Size(16, 16));
		}
		public AccordionDesigner(object editingComponent) {
			this.editingAccordionControl = editingComponent as AccordionControl;
		}
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Main", "Main settings(Elements, Appearances).", GetDefaultGroupImage(0), true);
			group.Add("Elements", "Manage and create elements.", typeof(ElementsDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null);
			group.Add("Appearances", "Adjust appearance settings for the AccordionControl.", typeof(AccordionAppearancesDesigner), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
			group = Groups.Add("Layout", "Save the layout of the AccordionControl to and restore it from an XML file.", GetDefaultGroupImage(1), false);
			group.Add("Layout", "Save and restore the AccordionControl layout", typeof(AccordionControlLayoutFrame), GetDefaultLargeImage(5), GetDefaultSmallImage(5), null);
		}
	}
}
