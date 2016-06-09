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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Drawing.Design;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.Design {
	public class SuperToolTipEditForm : XtraForm, ITypeDescriptorContext {
		public SuperToolTipEditForm() {
			InitializeComponent();
			toolPreview.ToolContainer = new SuperToolTip();
			toolPreview.ToolContainer.LookAndFeel = this.LookAndFeel;
			propertyGrid1.SelectedObject = toolPreview.ToolContainer;
			propertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid1_SelectedGridItemChanged);
		}
		internal ITypeDescriptorContext context;
		internal IServiceProvider provider;
		private SimpleButton btCancel;
		private SimpleButton btOk;
		private SplitContainerControl splitContainerControl1;
		private Panel panel1;
		private PropertyGrid propertyGrid1;
		private LabelControl labelControl4;
		private ComboBoxEdit allowHtmlTextCombo;
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuperToolTipEditForm));
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.allowHtmlTextCombo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOk = new DevExpress.XtraEditors.SimpleButton();
			this.imFooter = new SuperTipImageEdit();
			this.imContent = new SuperTipImageEdit();
			this.imTitle = new SuperTipImageEdit();
			this.meFooter = new DevExpress.XtraEditors.MemoEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.meContent = new DevExpress.XtraEditors.MemoEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.meTitle = new DevExpress.XtraEditors.MemoEdit();
			this.cbFooterSeparator = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
			this.toolPreview = new DevExpress.Utils.SuperToolTipPreviewControl();
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.allowHtmlTextCombo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imContent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imTitle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meContent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meTitle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFooterSeparator.Properties)).BeginInit();
			this.xtraScrollableControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			this.SuspendLayout();
			this.groupControl1.Controls.Add(this.labelControl4);
			this.groupControl1.Controls.Add(this.allowHtmlTextCombo);
			this.groupControl1.Controls.Add(this.btCancel);
			this.groupControl1.Controls.Add(this.btOk);
			this.groupControl1.Controls.Add(this.imFooter);
			this.groupControl1.Controls.Add(this.imContent);
			this.groupControl1.Controls.Add(this.imTitle);
			this.groupControl1.Controls.Add(this.meFooter);
			this.groupControl1.Controls.Add(this.labelControl3);
			this.groupControl1.Controls.Add(this.meContent);
			this.groupControl1.Controls.Add(this.labelControl2);
			this.groupControl1.Controls.Add(this.meTitle);
			this.groupControl1.Controls.Add(this.cbFooterSeparator);
			this.groupControl1.Controls.Add(this.labelControl1);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.allowHtmlTextCombo, "allowHtmlTextCombo");
			this.allowHtmlTextCombo.Name = "allowHtmlTextCombo";
			this.allowHtmlTextCombo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("allowHtmlTextCombo.Properties.Buttons"))))});
			this.allowHtmlTextCombo.Properties.Items.AddRange(new object[] {
			resources.GetString("allowHtmlTextCombo.Properties.Items"),
			resources.GetString("allowHtmlTextCombo.Properties.Items1"),
			resources.GetString("allowHtmlTextCombo.Properties.Items2")});
			this.allowHtmlTextCombo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.allowHtmlTextCombo.SelectedIndexChanged += new System.EventHandler(this.allowHtmlTextCombo_SelectedIndexChanged);
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btCancel, "btCancel");
			this.btCancel.Name = "btCancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			resources.ApplyResources(this.btOk, "btOk");
			this.btOk.Name = "btOk";
			this.btOk.Click += new System.EventHandler(this.btOk_Click);
			resources.ApplyResources(this.imFooter, "imFooter");
			this.imFooter.Name = "imFooter";
			this.imFooter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imFooter.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imFooter.Properties.Buttons1"))), resources.GetString("imFooter.Properties.Buttons2"), ((int)(resources.GetObject("imFooter.Properties.Buttons3"))), ((bool)(resources.GetObject("imFooter.Properties.Buttons4"))), ((bool)(resources.GetObject("imFooter.Properties.Buttons5"))), ((bool)(resources.GetObject("imFooter.Properties.Buttons6"))), ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, resources.GetString("imFooter.Properties.Buttons8"), resources.GetString("imFooter.Properties.Buttons9"), null, ((bool)(resources.GetObject("imFooter.Properties.Buttons10"))))});
			this.imFooter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.imEditor_ButtonClick);
			this.imFooter.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.imContent, "imContent");
			this.imContent.Name = "imContent";
			this.imContent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imContent.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imContent.Properties.Buttons1"))), resources.GetString("imContent.Properties.Buttons2"), ((int)(resources.GetObject("imContent.Properties.Buttons3"))), ((bool)(resources.GetObject("imContent.Properties.Buttons4"))), ((bool)(resources.GetObject("imContent.Properties.Buttons5"))), ((bool)(resources.GetObject("imContent.Properties.Buttons6"))), ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, resources.GetString("imContent.Properties.Buttons8"), resources.GetString("imContent.Properties.Buttons9"), null, ((bool)(resources.GetObject("imContent.Properties.Buttons10"))))});
			this.imContent.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.imEditor_ButtonClick);
			this.imContent.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.imTitle, "imTitle");
			this.imTitle.Name = "imTitle";
			this.imTitle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imTitle.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imTitle.Properties.Buttons1"))), resources.GetString("imTitle.Properties.Buttons2"), ((int)(resources.GetObject("imTitle.Properties.Buttons3"))), ((bool)(resources.GetObject("imTitle.Properties.Buttons4"))), ((bool)(resources.GetObject("imTitle.Properties.Buttons5"))), ((bool)(resources.GetObject("imTitle.Properties.Buttons6"))), ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, resources.GetString("imTitle.Properties.Buttons8"), resources.GetString("imTitle.Properties.Buttons9"), null, ((bool)(resources.GetObject("imTitle.Properties.Buttons10"))))});
			this.imTitle.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.imEditor_ButtonClick);
			this.imTitle.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.meFooter, "meFooter");
			this.meFooter.Name = "meFooter";
			this.meFooter.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.meContent, "meContent");
			this.meContent.Name = "meContent";
			this.meContent.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.meTitle, "meTitle");
			this.meTitle.Name = "meTitle";
			this.meTitle.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.cbFooterSeparator, "cbFooterSeparator");
			this.cbFooterSeparator.Name = "cbFooterSeparator";
			this.cbFooterSeparator.Properties.Caption = resources.GetString("cbFooterSeparator.Properties.Caption");
			this.cbFooterSeparator.EditValueChanged += new System.EventHandler(this.editor_EditValueChanged);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.xtraScrollableControl1.Controls.Add(this.toolPreview);
			resources.ApplyResources(this.xtraScrollableControl1, "xtraScrollableControl1");
			this.xtraScrollableControl1.Name = "xtraScrollableControl1";
			resources.ApplyResources(this.toolPreview, "toolPreview");
			this.toolPreview.Name = "toolPreview";
			resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.propertyGrid1);
			this.splitContainerControl1.Panel1.ShowCaption = true;
			resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
			this.splitContainerControl1.Panel2.Controls.Add(this.xtraScrollableControl1);
			this.splitContainerControl1.Panel2.ShowCaption = true;
			resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
			this.splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
			this.splitContainerControl1.SplitterPosition = 124;
			resources.ApplyResources(this.propertyGrid1, "propertyGrid1");
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.ToolbarVisible = false;
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			this.AcceptButton = this.btOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btCancel;
			this.Controls.Add(this.splitContainerControl1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupControl1);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "SuperToolTipEditForm";
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.allowHtmlTextCombo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imContent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imTitle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meContent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meTitle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFooterSeparator.Properties)).EndInit();
			this.xtraScrollableControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.MemoEdit meFooter;
		private DevExpress.XtraEditors.LabelControl labelControl3;
		private DevExpress.XtraEditors.MemoEdit meContent;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraEditors.MemoEdit meTitle;
		private DevExpress.XtraEditors.CheckEdit cbFooterSeparator;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
		private DevExpress.Utils.SuperToolTipPreviewControl toolPreview;
		private DevExpress.XtraEditors.ImageEdit imFooter;
		private DevExpress.XtraEditors.ImageEdit imContent;
		private DevExpress.XtraEditors.ImageEdit imTitle;
		void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e) {
			ParseSuperTip();
		}
		public void ParseSuperTip() {
			this.lockApply = true;
			try {
				SuperToolTipSetupArgs info = new SuperToolTipSetupArgs(EditingContainer);
				meTitle.Text = info.Title.Text;
				imTitle.Image = info.Title.Image;
				meContent.Text = info.Contents.Text;
				imContent.Image = info.Contents.Image;
				meFooter.Text = info.Footer.Text;
				imFooter.Image = info.Footer.Image;
				cbFooterSeparator.Checked = info.ShowFooterSeparator;
				allowHtmlTextCombo.SelectedItem = info.AllowHtmlText.ToString();
			}
			finally {
				this.lockApply = false;
			}
		}
		private void editor_EditValueChanged(object sender, EventArgs e) {
			Apply();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SuperToolTip EditingContainer { get { return toolPreview.ToolContainer; } }
		public void Apply() {
			if(lockApply) return;
			SuperToolTipSetupArgs info = new SuperToolTipSetupArgs();
			info.Title.Text = meTitle.Text;
			info.Title.Image = imTitle.Image;
			info.Contents.Text = meContent.Text;
			info.Contents.Image = imContent.Image;
			info.Footer.Text = meFooter.Text;
			info.Footer.Image = imFooter.Image;
			info.ShowFooterSeparator = cbFooterSeparator.Checked;
			info.AllowHtmlText = GetAllowHtmlText();
			this.toolPreview.ToolContainer.Setup(info);
			this.toolPreview.Invalidate();
			this.propertyGrid1.Refresh();
		}
		bool lockApply = false;
		protected virtual DefaultBoolean GetAllowHtmlText() {
			return (DefaultBoolean)Enum.Parse(typeof(DefaultBoolean), allowHtmlTextCombo.SelectedItem.ToString());
		}
		private void btOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}
		private void btCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		Image ImageProperty { get { return null; } set { } }
		private void imEditor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			if(!object.Equals(e.Button.Tag, "image")) return;
			ButtonEdit be = (ButtonEdit)sender;
			System.Drawing.Design.UITypeEditor editor = editor = GetEditor();
			if(editor != null) {
				be.EditValue = editor.EditValue(this, provider, be.EditValue);
				Apply();
			}
		}
		protected UITypeEditor GetEditor() {
			return DevExpress.Utils.Design.DesignTimeOptions.DefaultImageEditor;
		}
		IContainer ITypeDescriptorContext.Container { get { return context != null ? context.Container : null; } }
		object ITypeDescriptorContext.Instance { get { return context != null ? context.Instance : null; } }
		void ITypeDescriptorContext.OnComponentChanged() {
			if(context == null) return;
			context.OnComponentChanged();
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			if(context == null) return true;
			return context.OnComponentChanging();
		}
		PropertyDescriptor image;
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor {
			get {
				if(image == null) image = TypeDescriptor.CreateProperty(this.GetType(), "ImageProperty", typeof(Image));
				return image;
			}
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(provider != null) return provider.GetService(serviceType);
			return null;
		}
		private void allowHtmlTextCombo_SelectedIndexChanged(object sender, EventArgs e) {
			Apply();
		}
		#region SuperTipImageEdit
		class SuperTipImageEdit : ImageEdit {
			protected override PopupBaseForm CreatePopupForm() {
				return new SuperTipImagePopupForm(this);
			}
		}
		class SuperTipImagePopupForm : ImagePopupForm {
			public SuperTipImagePopupForm(ImageEdit ownerEdit)
				: base(ownerEdit) {
			}
			protected override PictureEdit CreatePictureEdit() {
				return new SuperTipPictureEdit(this);
			}
			class SuperTipPictureEdit : PictureEditEx {
				public SuperTipPictureEdit(ImagePopupForm form)
					: base(form) {
				}
				protected override PictureMenu CreatePictureMenu() {
					return new SuperTipPictureMenu(this);
				}
			}
			class SuperTipPictureMenu : PictureMenu {
				public SuperTipPictureMenu(IPictureMenu menuControl)
					: base(menuControl) {
				}
				protected override void PasteImage(Image image) {
					if(MenuControl != null) MenuControl.EditValue = image;
				}
			}
		}
		#endregion
	}
}
