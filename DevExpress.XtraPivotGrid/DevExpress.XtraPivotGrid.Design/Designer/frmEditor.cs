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
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPivotGrid.Design {
	public class PivotGridEditorForm : BaseDesignerForm {
		public const string PivotGridSettings = "Software\\Developer Express\\Designer\\XtraPivotGrid\\";
		public PivotGridEditorForm() {
			InitializeComponent();
			ProductInfo = new DevExpress.Utils.About.ProductInfo("XtraPivotGrid", typeof(PivotGridControl), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
			MinimumSize = new Size(780, 480);
			ClientSize = new Size(1175, 715);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new PivotGridDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(792, 516);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "PivotGridEditorForm";
			this.Text = "PivotGrid Designer";
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return PivotGridSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(PivotGridEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class PivotGridDesigner : BaseDesigner {
		static DevExpress.Utils.ImageCollection largeImages = null;
		static DevExpress.Utils.ImageCollection smallImages = null;
		static DevExpress.Utils.ImageCollection groupImages = null;
		PivotGridControl editingPivotGrid;
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		static PivotGridDesigner() {
			largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons32x32.png", typeof(PivotGridControl).Assembly, new Size(32, 32));
			smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.icons16x16.png", typeof(PivotGridControl).Assembly, new Size(16, 16));
			groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Design.Images.navBarGroupIcon16x16.png", typeof(PivotGridControl).Assembly, new Size(16, 16));
		}
		public PivotGridDesigner(object editingComponent) {
			this.editingPivotGrid = editingComponent as PivotGridControl;
		}
		protected PivotGridControl EditingPivotGrid { get { return editingPivotGrid; } }
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group = Groups.Add("Main", "Main settings(Fields, Appearances).", GetDefaultGroupImage(0), true);
			group.Add("Fields", "Manage fields.", typeof(DevExpress.XtraPivotGrid.Design.Frames.FieldDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(0), null); 
			group.Add("Groups", "Manage groups.", typeof(DevExpress.XtraPivotGrid.Frames.GroupsDesigner), GetDefaultLargeImage(0), GetDefaultSmallImage(1), null); 
			group.Add("Layout", "Customize the current PivotGrid's layout and preview its data.", "DevExpress.XtraPivotGrid.Frames.Layouts", GetDefaultLargeImage(1), GetDefaultSmallImage(2), null);
			group = Groups.Add("Appearances", "Adjust the appearance of the current PivotGrid.", GetDefaultGroupImage(1), true);
			group.Add("Appearances", "Manage the appearances for the current PivotGrid.", typeof(DevExpress.XtraPivotGrid.Frames.AppearancesDesigner), GetDefaultLargeImage(2), GetDefaultSmallImage(3), null); 
			group.Add("Format Rules", "Manage the format rules for the current PivotGrid.", typeof(DevExpress.XtraPivotGrid.Frames.FormatRuleFrame), GetDefaultLargeImage(3), GetDefaultSmallImage(4), null);
			group = Groups.Add("Printing", "Printing option management for the current PivotGrid.", GetDefaultGroupImage(2), true);
			group.Add("Print Appearances", "Adjust the print appearances of the current PivotGrid.", typeof(DevExpress.XtraPivotGrid.Design.Frames.PrintAppearancesDesigner), GetDefaultLargeImage(4), GetDefaultSmallImage(5), null); 
			group.Add("Print Settings", "Set up various printing options for the current PivotGrid.", typeof(DevExpress.XtraPivotGrid.Frames.PivotGridPrinting), GetDefaultLargeImage(5), GetDefaultSmallImage(6), null); 
		}
	}
}
