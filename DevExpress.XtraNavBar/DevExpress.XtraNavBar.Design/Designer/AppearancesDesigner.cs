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
namespace DevExpress.XtraNavBar.Frames {
	[ToolboxItem(false)]
	public class AppearancesDesigner : DevExpress.XtraEditors.Frames.AppearancesDesignerSimple {
		private System.ComponentModel.Container components = null;
		public AppearancesDesigner() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
			this.pgMain.Size = new System.Drawing.Size(423, 296);
			this.pnlControl.Size = new System.Drawing.Size(588, 54);
			this.lbCaption.Size = new System.Drawing.Size(588, 38);
			this.horzSplitter.Size = new System.Drawing.Size(588, 4);
			this.Name = "AppearancesDesigner";
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
		public NavBarControl EditingNavBar { get { return EditingObject as NavBarControl; } }
		protected override BaseAppearanceCollection Appearances { get { return EditingNavBar.Appearance; } }
		protected override Image AppearanceImage { get { return EditingNavBar.BackgroundImage; } }
		protected override XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] {pgMain}, new string[] {"Properties"});
		}
		protected override void SetSelectedObject() {
			ArrayList arr = new ArrayList();
			foreach(AppearanceObject obj in this.SelectedObjects) {
				arr.Add(EditingNavBar.PaintAppearance.GetAppearance(obj.Name));
			}
			Preview.SetAppearance(arr.ToArray());
		}
		protected override void SetDefault() {
			EditingNavBar.BeginUpdate();
			base.SetDefault();
			EditingNavBar.EndUpdate();
		}
	}
}
