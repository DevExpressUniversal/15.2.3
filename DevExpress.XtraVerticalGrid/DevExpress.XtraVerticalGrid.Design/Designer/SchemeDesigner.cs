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
using System.ComponentModel.Design;
using DevExpress.XtraVerticalGrid.Design;
namespace DevExpress.XtraVerticalGrid.Frames {
	[ToolboxItem(false)]
	public class SchemeDesigner : DevExpress.XtraEditors.Frames.SchemeDesignerBase {
		private System.ComponentModel.Container components = null;
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
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pnlMain,
																		  this.horzSplitter,
																		  this.lbCaption});
			this.Name = "SchemeDesigner";
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init
		public VGridControlBase EditingVGrid { get { return EditingObject as VGridControlBase; } }
		public SchemeDesigner() {
			InitializeComponent();
		}
		private XAppearances xs = null;
		XAppearances XS {
			get {
				if(xs == null) xs = new XAppearances("System");
				return xs;
			}
		}
		protected internal VGridControlBase vGridPattern;
		public virtual void CreateGridControl() {
			vGridPattern = new VGridControl();
			XViews.ConfigureDemoView(vGridPattern);
		}
		public override void InitComponent() {
			CreateGridControl();
			vGridPattern.Dock = DockStyle.Fill;
			AddPreviewControl(vGridPattern);
			StyleList.Items.Clear();
			StyleList.Items.AddRange(XS.FormatNames);
			vGridPattern.LookAndFeel.Assign(EditingVGrid.LookAndFeel);
			vGridPattern.Appearance.Assign(EditingVGrid.Appearance);
		}
		#endregion
		#region Editing
		protected override void LoadSchemePreview() {
			XS.LoadScheme(StyleList.SelectedItem.ToString(), vGridPattern);
		}
		protected override void LoadScheme() {
			XS.LoadScheme(StyleList.SelectedItem.ToString(), EditingVGrid);
			if(StyleList.SelectedIndex == 0) FireChanged();
		}
		protected override void SetFormatNames(bool isEnabled) {
			XS.ShowNewStylesOnly = isEnabled;
			StyleList.Items.AddRange(XS.FormatNames);
		}
		private void FireChanged() {
			IComponentChangeService srv = EditingVGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingVGrid, null, null, null);
		}
		#endregion
	}
}
