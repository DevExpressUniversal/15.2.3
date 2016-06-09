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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public abstract class SummaryEditorForm : DevExpress.XtraReports.Design.SummaryEditorFormBase {
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label lblFormatString;
		private DevExpress.XtraEditors.ButtonEdit tbFormatString;
		private GroupControl grpSummaryRunning;
		protected DevExpress.XtraEditors.Internal.RadioGroupLocalizable radioGroup1;
		protected DesignBinding fDesignBinding = new DesignBinding();
		IDataSourceCollectionProvider dataSourceCollectionProvider;
		protected SummaryEditorForm() {
			InitializeComponent();
		}
		protected SummaryEditorForm(DesignBinding designBinding, object summaryFuncValue, Type summaryFuncType, string formatString, bool ignoreNullValues, object summaryRunnning, IServiceProvider serviceProvider)
			: base(ignoreNullValues, serviceProvider) {
			this.fDesignBinding = designBinding;
			InitializeComponent();
			tbFormatString.Text = formatString;
			dataSourceCollectionProvider = (IDataSourceCollectionProvider)serviceProvider.GetService(typeof(IDataSourceCollectionProvider));
			cbSummaryFunction.Properties.Items.AddRange(GetEnumDisplayNames(summaryFuncType));
			TypeConverter typeConverter = TypeDescriptor.GetConverter(summaryFuncType);
			cbSummaryFunction.EditValue = typeConverter.ConvertToString(new StubTypeDescriptorContext(), summaryFuncValue);
			cbBoundField.Text = designBinding.GetDisplayName(serviceProvider);
			radioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			radioGroup1.Properties.Appearance.Options.UseBackColor = true;
			radioGroup1.EditValue = summaryRunnning;
			radioGroup1.Properties.Columns = 2;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryEditorForm));
			InitializeRunningSummaryRadioGroup(resources);
			this.tbFormatString.EditValueChanged += new System.EventHandler(this.tbFormatString_EditValueChanged);
			this.radioGroup1.EditValueChanged += new System.EventHandler(this.radioGroup1_EditValueChanged);
			UpdatePreview();
		}
		protected virtual void InitializeRunningSummaryRadioGroup(System.ComponentModel.ComponentResourceManager resources) {
		}
		public override string FormatString {
			get { return tbFormatString.Text; }
		}
		public DesignBinding DesignBinding { get { return fDesignBinding; } }
		protected new PopupBindingPicker BindingPicker { get { return (PopupBindingPicker)base.BindingPicker; } }
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryEditorForm));
			this.lblFormatString = new System.Windows.Forms.Label();
			this.tbFormatString = new DevExpress.XtraEditors.ButtonEdit();
			this.grpSummaryRunning = new DevExpress.XtraEditors.GroupControl();
			this.radioGroup1 = new DevExpress.XtraEditors.Internal.RadioGroupLocalizable();
			((System.ComponentModel.ISupportInitialize)(this.grpSummaryRunning)).BeginInit();
			this.grpSummaryRunning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFormatString.Properties)).BeginInit();
			this.SuspendLayout();
			this.lblFormatString.AccessibleDescription = ((string)(resources.GetObject("lblFormatString.AccessibleDescription")));
			this.lblFormatString.AccessibleName = ((string)(resources.GetObject("lblFormatString.AccessibleName")));
			this.lblFormatString.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblFormatString.Anchor")));
			this.lblFormatString.AutoSize = ((bool)(resources.GetObject("lblFormatString.AutoSize")));
			this.lblFormatString.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblFormatString.Dock")));
			this.lblFormatString.Enabled = ((bool)(resources.GetObject("lblFormatString.Enabled")));
			this.lblFormatString.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblFormatString.Font = ((System.Drawing.Font)(resources.GetObject("lblFormatString.Font")));
			this.lblFormatString.Image = ((System.Drawing.Image)(resources.GetObject("lblFormatString.Image")));
			this.lblFormatString.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFormatString.ImageAlign")));
			this.lblFormatString.ImageIndex = ((int)(resources.GetObject("lblFormatString.ImageIndex")));
			this.lblFormatString.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblFormatString.ImeMode")));
			this.lblFormatString.Location = ((System.Drawing.Point)(resources.GetObject("lblFormatString.Location")));
			this.lblFormatString.Name = "lblFormatString";
			this.lblFormatString.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblFormatString.RightToLeft")));
			this.lblFormatString.Size = ((System.Drawing.Size)(resources.GetObject("lblFormatString.Size")));
			this.lblFormatString.TabIndex = ((int)(resources.GetObject("lblFormatString.TabIndex")));
			this.lblFormatString.Text = resources.GetString("lblFormatString.Text");
			this.lblFormatString.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFormatString.TextAlign")));
			this.lblFormatString.Visible = ((bool)(resources.GetObject("lblFormatString.Visible")));
			this.tbFormatString.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tbFormatString.Anchor")));
			this.tbFormatString.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tbFormatString.BackgroundImage")));
			this.tbFormatString.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tbFormatString.Dock")));
			this.tbFormatString.EditValue = resources.GetString("tbFormatString.EditValue");
			this.tbFormatString.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tbFormatString.ImeMode")));
			this.tbFormatString.Location = ((System.Drawing.Point)(resources.GetObject("tbFormatString.Location")));
			this.tbFormatString.Name = "tbFormatString";
			this.tbFormatString.Properties.AccessibleDescription = ((string)(resources.GetObject("tbFormatString.Properties.AccessibleDescription")));
			this.tbFormatString.Properties.AccessibleName = ((string)(resources.GetObject("tbFormatString.Properties.AccessibleName")));
			this.tbFormatString.Properties.AutoHeight = ((bool)(resources.GetObject("tbFormatString.Properties.AutoHeight")));
			this.tbFormatString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																												   new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, DevExpress.Utils.HorzAlignment.Center, null, new DevExpress.Utils.KeyShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Enter)))});
			this.tbFormatString.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("tbFormatString.Properties.Mask.AutoComplete")));
			this.tbFormatString.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("tbFormatString.Properties.Mask.BeepOnError")));
			this.tbFormatString.Properties.Mask.EditMask = resources.GetString("tbFormatString.Properties.Mask.EditMask");
			this.tbFormatString.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("tbFormatString.Properties.Mask.IgnoreMaskBlank")));
			this.tbFormatString.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("tbFormatString.Properties.Mask.MaskType")));
			this.tbFormatString.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("tbFormatString.Properties.Mask.PlaceHolder")));
			this.tbFormatString.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("tbFormatString.Properties.Mask.SaveLiteral")));
			this.tbFormatString.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("tbFormatString.Properties.Mask.ShowPlaceHolders")));
			this.tbFormatString.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("tbFormatString.Properties.Mask.UseMaskAsDisplayFormat")));
			this.tbFormatString.Properties.NullText = resources.GetString("tbFormatString.Properties.NullText");
			this.tbFormatString.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tbFormatString.RightToLeft")));
			this.tbFormatString.Size = ((System.Drawing.Size)(resources.GetObject("tbFormatString.Size")));
			this.tbFormatString.TabIndex = ((int)(resources.GetObject("tbFormatString.TabIndex")));
			this.tbFormatString.ToolTip = resources.GetString("tbFormatString.ToolTip");
			this.tbFormatString.ToolTipIconType = ((DevExpress.Utils.ToolTipIconType)(resources.GetObject("tbFormatString.ToolTipIconType")));
			this.tbFormatString.Visible = ((bool)(resources.GetObject("tbFormatString.Visible")));
			this.tbFormatString.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.tbFormatString_ButtonClick);
			this.grpSummaryRunning.AccessibleDescription = ((string)(resources.GetObject("grpSummaryRunning.AccessibleDescription")));
			this.grpSummaryRunning.AccessibleName = ((string)(resources.GetObject("grpSummaryRunning.AccessibleName")));
			this.grpSummaryRunning.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("grpSummaryRunning.Anchor")));
			this.grpSummaryRunning.AutoScroll = ((bool)(resources.GetObject("grpSummaryRunning.AutoScroll")));
			this.grpSummaryRunning.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("grpSummaryRunning.AutoScrollMargin")));
			this.grpSummaryRunning.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("grpSummaryRunning.AutoScrollMinSize")));
			this.grpSummaryRunning.Controls.AddRange(new System.Windows.Forms.Control[] {
																							this.radioGroup1});
			this.grpSummaryRunning.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("grpSummaryRunning.Dock")));
			this.grpSummaryRunning.Enabled = ((bool)(resources.GetObject("grpSummaryRunning.Enabled")));
			this.grpSummaryRunning.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("grpSummaryRunning.ImeMode")));
			this.grpSummaryRunning.Location = ((System.Drawing.Point)(resources.GetObject("grpSummaryRunning.Location")));
			this.grpSummaryRunning.Name = "grpSummaryRunning";
			this.grpSummaryRunning.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("grpSummaryRunning.RightToLeft")));
			this.grpSummaryRunning.Size = ((System.Drawing.Size)(resources.GetObject("grpSummaryRunning.Size")));
			this.grpSummaryRunning.TabIndex = ((int)(resources.GetObject("grpSummaryRunning.TabIndex")));
			this.grpSummaryRunning.Text = resources.GetString("grpSummaryRunning.Text");
			this.grpSummaryRunning.Visible = ((bool)(resources.GetObject("grpSummaryRunning.Visible")));
			this.radioGroup1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("radioGroup1.Anchor")));
			this.radioGroup1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("radioGroup1.BackgroundImage")));
			this.radioGroup1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("radioGroup1.Dock")));
			this.radioGroup1.EditValue = ((object)(resources.GetObject("radioGroup1.EditValue")));
			this.radioGroup1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("radioGroup1.ImeMode")));
			this.radioGroup1.Location = ((System.Drawing.Point)(resources.GetObject("radioGroup1.Location")));
			this.radioGroup1.Name = "radioGroup1";
			this.radioGroup1.Properties.AccessibleDescription = ((string)(resources.GetObject("radioGroup1.Properties.AccessibleDescription")));
			this.radioGroup1.Properties.AccessibleName = ((string)(resources.GetObject("radioGroup1.Properties.AccessibleName")));
			this.radioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroup1.Properties.NullText = resources.GetString("radioGroup1.Properties.NullText");
			this.radioGroup1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("radioGroup1.RightToLeft")));
			this.radioGroup1.Size = ((System.Drawing.Size)(resources.GetObject("radioGroup1.Size")));
			this.radioGroup1.TabIndex = ((int)(resources.GetObject("radioGroup1.TabIndex")));
			this.radioGroup1.ToolTip = resources.GetString("radioGroup1.ToolTip");
			this.radioGroup1.ToolTipIconType = ((DevExpress.Utils.ToolTipIconType)(resources.GetObject("radioGroup1.ToolTipIconType")));
			this.radioGroup1.Visible = ((bool)(resources.GetObject("radioGroup1.Visible")));
			resources.ApplyResources(this, "$this");
			this.Name = "SummaryEditorForm";
			this.Controls.AddRange(new System.Windows.Forms.Control[] { 
				this.lblFormatString,
				this.tbFormatString,
				this.grpSummaryRunning
			});
			((System.ComponentModel.ISupportInitialize)(this.tbFormatString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpSummaryRunning)).EndInit();
			this.grpSummaryRunning.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void tbFormatString_EditValueChanged(object sender, System.EventArgs e) {
			UpdatePreview();
			UpdateAcceptButtonDialogResult();
		}
		protected abstract FormatStringEditorForm GetFormatStringEditorForm(string formatString, IServiceProvider serviceProvider);
		private void tbFormatString_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			FormatStringEditorForm dlg = GetFormatStringEditorForm(tbFormatString.Text, serviceProvider);
			if (dlg.ShowDialog(this) == DialogResult.OK) {
				tbFormatString.Text = dlg.EditValue;
			}
		}
		private void radioGroup1_EditValueChanged(object sender, System.EventArgs e) {
			UpdatePreview();
		}
		#region field name edit
		protected override PopupBindingPickerBase CreatePopupBindingPicker() {
			return new PopupBindingPicker();
		}
		protected override void StartFieldNameEdit() {
			BindingPicker.Start(dataSourceCollectionProvider.GetDataSourceCollection(this.serviceProvider), serviceProvider, fDesignBinding, cbBoundField);
		}
		protected override void EndFieldNameEdit() {
			DesignBinding designBinding = BindingPicker.EndBindingPicker();
			if (!this.fDesignBinding.Equals(designBinding)) {
				SetDesignBinding(designBinding);
				cbBoundField.Text = designBinding.GetDisplayName(serviceProvider);
				UpdatePreview();
			}
		}
		protected virtual void SetDesignBinding(DesignBinding newDesignBinding) {
			this.fDesignBinding = newDesignBinding;
		}
		protected override Type GetPropertyType(DataContext dataContext) {
			if (fDesignBinding.DataSource == null)
				return typeof(System.Int32);
			return dataContext.GetPropertyType(fDesignBinding.DataSource, fDesignBinding.DataMember);
		}
		#endregion
	}
}
