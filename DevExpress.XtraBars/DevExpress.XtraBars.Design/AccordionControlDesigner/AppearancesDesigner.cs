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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraBars.Navigation.Frames {
	[ToolboxItem(false)]
	public class AccordionAppearancesDesigner : DevExpress.XtraEditors.Frames.AppearancesDesignerSimple {
		private System.ComponentModel.Container components = null;
		public AccordionAppearancesDesigner() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).BeginInit();
			this.pnlAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.scAppearance.Size = new System.Drawing.Size(5, 300);
			this.gcAppearances.Size = new System.Drawing.Size(160, 270);
			this.lbcAppearances.Size = new System.Drawing.Size(152, 243);
			this.bpAppearances.Location = new System.Drawing.Point(0, 270);
			this.bpAppearances.Size = new System.Drawing.Size(160, 30);
			this.pnlAppearances.Size = new System.Drawing.Size(160, 300);
			this.splMain.Location = new System.Drawing.Point(160, 96);
			this.splMain.Size = new System.Drawing.Size(5, 300);
			this.pgMain.Location = new System.Drawing.Point(165, 96);
			this.pgMain.Size = new System.Drawing.Size(423, 300);
			this.pnlControl.Location = new System.Drawing.Point(0, 42);
			this.pnlControl.Size = new System.Drawing.Size(588, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(588, 38);
			this.pnlMain.Location = new System.Drawing.Point(0, 96);
			this.pnlMain.Size = new System.Drawing.Size(160, 300);
			this.horzSplitter.Location = new System.Drawing.Point(0, 38);
			this.horzSplitter.Size = new System.Drawing.Size(588, 4);
			this.Name = "AccordionAppearancesDesigner";
			this.Size = new System.Drawing.Size(588, 396);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).EndInit();
			this.pnlAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string DescriptionText { get { return "Select one or more Appearance objects which correspond to the control's particular elements to access and modify their settings via the property grid."; } }
		public AccordionControl EditingAccordionControl { get { return EditingObject as AccordionControl; } }
		protected override BaseAppearanceCollection Appearances { get { return EditingAccordionControl.Appearance; } }
		protected BaseAppearanceCollection ItemAppearances { get { return EditingAccordionControl.Appearance.Item; } }
		protected BaseAppearanceCollection GroupAppearances { get { return EditingAccordionControl.Appearance.Group; } }
		protected BaseAppearanceCollection ItemWithContainerAppearances { get { return EditingAccordionControl.Appearance.ItemWithContainer; } }
		protected override Image AppearanceImage { get { return EditingAccordionControl.BackgroundImage; } }
		protected override XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] { pgMain }, new string[] { "Properties" });
		}
		protected override void SetSelectedObject() {
			if(SelectedObjects == null) return;
			ArrayList arr = new ArrayList();
			foreach(AppearanceObject obj in this.SelectedObjects) {
				arr.Add(EditingAccordionControl.Appearance.GetAppearance(obj.Name));
			}
			Preview.SetAppearance(arr.ToArray());
		}
		protected override void SetDefault() {
			EditingAccordionControl.BeginUpdate();
			base.SetDefault();
			EditingAccordionControl.EndUpdate();
		}
		public override void InitComponent() {
			base.InitComponent();
			UpdateAppearancesListBox();
		}
		const string ItemAppearancesPrefix = "Item";
		const string ItemWithContainerAppearancesPrefix = "ItemWithContainer";
		const string GroupAppearancesPrefix = "Group";
		protected void UpdateAppearancesListBox() {
			lbcAppearances.BeginUpdate();
			UpdateAppearancesListBoxCore(ItemAppearances, ItemAppearancesPrefix);
			UpdateAppearancesListBoxCore(ItemWithContainerAppearances, ItemWithContainerAppearancesPrefix);
			UpdateAppearancesListBoxCore(GroupAppearances, GroupAppearancesPrefix);
			lbcAppearances.EndUpdate();
		}
		protected void UpdateAppearancesListBoxCore(BaseAppearanceCollection appCollection, string prefix) {
			if(appCollection == null) return;
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(appCollection);
			for(int i = 0; i < collection.Count; i++)
				if(typeof(AppearanceObject).IsAssignableFrom(collection[i].PropertyType)) {
					if(collection[i].Name == "Hover") continue; 
					string appearanceName = prefix + collection[i].Name;
					lbcAppearances.Items.Add(appearanceName);
				}
		}
		protected override void AddObject(ArrayList ret, string item) {
			if(item.Contains(ItemWithContainerAppearancesPrefix)) {
				ret.Add(GetAppearanceObjectByName(ItemWithContainerAppearances, item, ItemWithContainerAppearancesPrefix));
				return;
			}
			if(item.Contains(ItemAppearancesPrefix)) {
				ret.Add(GetAppearanceObjectByName(ItemAppearances, item, ItemAppearancesPrefix));
				return;
			}
			if(item.Contains(GroupAppearancesPrefix)) {
				ret.Add(GetAppearanceObjectByName(GroupAppearances, item, GroupAppearancesPrefix));
				return;
			}
			base.AddObject(ret, item);
		}
		protected AppearanceObject GetAppearanceObjectByName(BaseAppearanceCollection appearances, string item, string prefix) {
			string name = item.Replace(prefix, string.Empty);
			return GetAppearanceObjectByName(appearances, name);
		}
	}
}
