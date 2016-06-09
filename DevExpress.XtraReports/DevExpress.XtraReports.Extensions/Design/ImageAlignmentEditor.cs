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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Skins;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class ImageAlignmentUI : XtraUserControl {
		private System.ComponentModel.Container components = null;
		private ContrastCheckButton topLeft;
		private ContrastCheckButton topCenter;
		private ContrastCheckButton topRight;
		private ContrastCheckButton middleLeft;
		private ContrastCheckButton middleCenter;
		private ContrastCheckButton middleRight;
		private ContrastCheckButton bottomLeft;
		private ContrastCheckButton bottomCenter;
		private ContrastCheckButton bottomRight;
		private IWindowsFormsEditorService edSvc;
		private TableLayoutPanel tableLayoutPanel1;
		private object editValue = ImageAlignment.Default;
		private object oldValue;
		public object Value {
			get { return editValue; }
		}
		public ImageAlignmentUI() {
			InitializeComponent();
			topLeft.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			topCenter.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			topRight.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			middleLeft.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			middleCenter.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			middleRight.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			bottomLeft.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			bottomCenter.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			bottomRight.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
			ApplyBackColor();
			topLeft.Tag = ImageAlignment.TopLeft;
			topCenter.Tag = ImageAlignment.TopCenter;
			topRight.Tag = ImageAlignment.TopRight;
			middleLeft.Tag = ImageAlignment.MiddleLeft;
			middleCenter.Tag = ImageAlignment.MiddleCenter;
			middleRight.Tag = ImageAlignment.MiddleRight;
			bottomLeft.Tag = ImageAlignment.BottomLeft;
			bottomCenter.Tag = ImageAlignment.BottomCenter;
			bottomRight.Tag = ImageAlignment.BottomRight;
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			ApplyBackColor();
		}
		void ApplyBackColor() {
			this.BackColor = LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ?
				CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinForm].Color.GetBackColor() :
				System.Drawing.SystemColors.Control;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageAlignmentUI));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.topLeft = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.topCenter = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.topRight = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.middleLeft = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.middleCenter = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.middleRight = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.bottomLeft = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.bottomCenter = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.bottomRight = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.topLeft, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.topCenter, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.topRight, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.middleLeft, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.middleCenter, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.middleRight, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.bottomLeft, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.bottomCenter, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.bottomRight, 2, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this.topLeft, "topLeft");
			this.topLeft.Name = "topLeft";
			this.topLeft.TabStop = false;
			this.topLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.topCenter, "topCenter");
			this.topCenter.Name = "topCenter";
			this.topCenter.TabStop = false;
			this.topCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.topRight, "topRight");
			this.topRight.Name = "topRight";
			this.topRight.TabStop = false;
			this.topRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.middleLeft, "middleLeft");
			this.middleLeft.Name = "middleLeft";
			this.middleLeft.TabStop = false;
			this.middleLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.middleCenter, "middleCenter");
			this.middleCenter.Name = "middleCenter";
			this.middleCenter.TabStop = false;
			this.middleCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.middleRight, "middleRight");
			this.middleRight.Name = "middleRight";
			this.middleRight.TabStop = false;
			this.middleRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.bottomLeft, "bottomLeft");
			this.bottomLeft.Name = "bottomLeft";
			this.bottomLeft.TabStop = false;
			this.bottomLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.bottomCenter, "bottomCenter");
			this.bottomCenter.Name = "bottomCenter";
			this.bottomCenter.TabStop = false;
			this.bottomCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.bottomRight, "bottomRight");
			this.bottomRight.Name = "bottomRight";
			this.bottomRight.TabStop = false;
			this.bottomRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ImageAlignmentUI";
			resources.ApplyResources(this, "$this");
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		public void Start(IWindowsFormsEditorService edSvc, object val, object instance) {
			this.edSvc = edSvc;
			if(val is ImageAlignment) {
				ImageAlignment align = (ImageAlignment)val;
				oldValue = align;
				switch(align) {
					case ImageAlignment.TopLeft:
						topLeft.Checked = true;
						break;
					case ImageAlignment.TopCenter:
						topCenter.Checked = true;
						break;
					case ImageAlignment.TopRight:
						topRight.Checked = true;
						break;
					case ImageAlignment.MiddleLeft:
						middleLeft.Checked = true;
						break;
					case ImageAlignment.MiddleCenter:
						middleCenter.Checked = true;
						break;
					case ImageAlignment.MiddleRight:
						middleRight.Checked = true;
						break;
					case ImageAlignment.BottomLeft:
						bottomLeft.Checked = true;
						break;
					case ImageAlignment.BottomCenter:
						bottomCenter.Checked = true;
						break;
					case ImageAlignment.BottomRight:
						bottomRight.Checked = true;
						break;
					default:
						break;
				}
			}
		}
		public void End() {
			this.edSvc = null;
			this.editValue = null;
		}
		bool isUnchecking = false;
		private void chbox_CheckedChanged(object sender, System.EventArgs e) {
			if(isUnchecking)
				return;
			CheckButton chBox = (CheckButton)sender;
			if(chBox.Checked) {
				UncheckAllExcept(chBox);
				editValue = (ImageAlignment)chBox.Tag;
			} else
				editValue = ImageAlignment.Default;
		}
		void UncheckAllExcept(CheckButton checkedChBox) {
			isUnchecking = true;
			foreach(CheckButton chBox in tableLayoutPanel1.Controls) {
				if(chBox != checkedChBox)
					chBox.Checked = false;
			}
			isUnchecking = false;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) editValue = oldValue;
			return base.ProcessDialogKey(keyData);
		}
	}
	public class ImageAlignmentEditor : UITypeEditor {
		public ImageAlignmentEditor()
			: base() {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if(context != null
				&& context.Instance != null
				&& provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(edSvc != null) {
					ImageAlignmentUI imageAlignmentUI = new ImageAlignmentUI();
					ILookAndFeelService lookAndFeelService = provider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
					if(lookAndFeelService != null)
						imageAlignmentUI.LookAndFeel.ParentLookAndFeel = lookAndFeelService.LookAndFeel;
					imageAlignmentUI.Start(edSvc, val, context.Instance);
					edSvc.DropDownControl(imageAlignmentUI);
					if(NeedUpdateEditValue(val, imageAlignmentUI.Value))
						val = imageAlignmentUI.Value;
					imageAlignmentUI.End();
				}
			}
			return val;
		}
		bool NeedUpdateEditValue(object oldObject, object newObject) {
			ImageAlignment oldValue = (ImageAlignment)oldObject;
			ImageAlignment newValue = (ImageAlignment)newObject;
			return oldValue != newValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Converter != null && context.PropertyDescriptor.Converter.GetStandardValuesSupported(context))
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
	}
}
