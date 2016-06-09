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
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class BordersUI : XtraUserControl {
		#region static
		static BorderSide SetEnumFlag(BorderSide borderSide, BorderSide flag, bool add) {
			return add ? borderSide | flag : borderSide & ~flag;
		}
		static bool EnumContainsFlag(BorderSide borderSide, BorderSide flag) {
			return (borderSide & flag) > 0;
		}
		#endregion
		private System.ComponentModel.Container components = null;
		private ContrastCheckButton left;
		private ContrastCheckButton right;
		private ContrastCheckButton top;
		private ContrastCheckButton bottom;
		private DevExpress.XtraEditors.SimpleButton buttonAll;
		private DevExpress.XtraEditors.SimpleButton buttonNone;
		private object editValue;
		private TableLayoutPanel tableLayoutPanel1;
		private object oldValue;
		public object Value { get { return editValue; }
		}
		public BordersUI() {
			InitializeComponent();
			left.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			top.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			right.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			bottom.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			buttonAll.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			buttonNone.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			this.LookAndFeel.StyleChanged +=new EventHandler(LookAndFeel_StyleChanged);
			ApplyBackColor();
			left.Tag = DevExpress.XtraPrinting.BorderSide.Left;
			top.Tag = DevExpress.XtraPrinting.BorderSide.Top;
			right.Tag = DevExpress.XtraPrinting.BorderSide.Right;
			bottom.Tag = DevExpress.XtraPrinting.BorderSide.Bottom;
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
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BordersUI));
			this.buttonAll = new DevExpress.XtraEditors.SimpleButton();
			this.buttonNone = new DevExpress.XtraEditors.SimpleButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.left = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.right = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.top = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.bottom = new DevExpress.XtraReports.Design.ContrastCheckButton();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.buttonAll, "buttonAll");
			this.buttonAll.Name = "buttonAll";
			this.buttonAll.Click += new System.EventHandler(this.button_Click);
			resources.ApplyResources(this.buttonNone, "buttonNone");
			this.buttonNone.Name = "buttonNone";
			this.buttonNone.Click += new System.EventHandler(this.button_Click);
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.buttonAll, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonNone, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this.left, "left");
			this.left.Name = "left";
			this.left.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.right, "right");
			this.right.Name = "right";
			this.right.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.top, "top");
			this.top.Name = "top";
			this.top.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			resources.ApplyResources(this.bottom, "bottom");
			this.bottom.Name = "bottom";
			this.bottom.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.right);
			this.Controls.Add(this.left);
			this.Controls.Add(this.bottom);
			this.Controls.Add(this.top);
			this.Name = "BordersUI";
			resources.ApplyResources(this, "$this");
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			buttonAll.Focus();
		}
		public void Start(IWindowsFormsEditorService edSvc, object val) {
			this.editValue = val ?? BorderSide.None;
			if(val is BorderSide) {
				BorderSide borderSide = (BorderSide)val;
				left.Checked = EnumContainsFlag(borderSide, BorderSide.Left);
				top.Checked = EnumContainsFlag(borderSide, BorderSide.Top);
				right.Checked = EnumContainsFlag(borderSide, BorderSide.Right);
				bottom.Checked = EnumContainsFlag(borderSide, BorderSide.Bottom);
				oldValue = val;
			}
		}
		public void End() {
			this.editValue = null;
		}
		private void chbox_CheckedChanged(object sender, System.EventArgs e) {
			if(editValue is BorderSide) {
				CheckButton chBox = (CheckButton)sender;
				editValue  = SetEnumFlag((BorderSide)editValue, (BorderSide)chBox.Tag, chBox.Checked);
			}
		}
		private void button_Click(object sender, System.EventArgs e) {
			bool val = sender.Equals(buttonAll) ? true : false;
			left.Checked = val;		
			top.Checked = val;		
			right.Checked = val;
			bottom.Checked = val;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) editValue = oldValue;
			return base.ProcessDialogKey(keyData);
		}
#if DEBUGTEST
		internal CheckButton Test_GetLeftButton() {
			return this.left;
		}
		internal void Test_RunCheckedChanged(object sender) {
			chbox_CheckedChanged(sender, EventArgs.Empty);
		}
#endif
	}
	[ToolboxItem(false)]
	public class ContrastCheckButton : CheckButton {
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ContrastCheckButtonViewInfo(this);
		}
	}
	public class ContrastCheckButtonViewInfo : SimpleButtonViewInfo {
		public ContrastCheckButtonViewInfo(CheckButton owner) : base(owner) { }
		protected override EditorButtonPainter GetButtonPainter() {
			if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && OwnerControl.ButtonStyle == DevExpress.XtraEditors.Controls.BorderStyles.Default)
				return new SkinCustomCheckButtonPainter(OwnerControl.LookAndFeel);
			return base.GetButtonPainter();
		}
	}
	public class SkinCustomCheckButtonPainter : SkinEditorButtonPainter {
		public SkinCustomCheckButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateSkinElementInfo() {
			return base.CreateSkinElementInfo();
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, DevExpress.XtraEditors.Controls.ButtonPredefines kind) {
			return ReportsSkins.GetSkin(Provider)[ReportsSkins.SkinContrastCheckButton];
		}
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			base.UpdateButtonInfo(e);
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			bool isPressed = (ee.State & ObjectState.Pressed) != ObjectState.Normal;
			info.ImageIndex = isPressed ? 1 : 0;
		}
	}
	public class BordersEditor : UITypeEditor
	{
		public BordersEditor() : base() {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (context != null 
				&& context.Instance != null
				&& provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					BordersUI bordersUI = new BordersUI();
					ILookAndFeelService lookAndFeelService = provider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
					if(lookAndFeelService != null)
						bordersUI.LookAndFeel.ParentLookAndFeel = lookAndFeelService.LookAndFeel;
					bordersUI.Start(edSvc, val);
					edSvc.DropDownControl(bordersUI);
					val = bordersUI.Value;
					bordersUI.End();
				}
			}
			return val;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
	}
}
