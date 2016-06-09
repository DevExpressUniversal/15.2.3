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
using System.Windows.Forms;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon {
	public partial class RibbonCustomizationForm : XtraForm, ICustomizationTopForm {
		public RibbonCustomizationForm() : this(null) {
		}
		public RibbonCustomizationForm(RibbonControl ribbonControl) {
			InitializeComponent();
			this.RibbonControl = ribbonControl;
			if(ribbonControl != null) RibbonSourceStateInfo = RibbonSourceStateInfo.Create(ribbonControl);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitDialog();
			ScaleForm();
		}
		protected virtual void ScaleForm() {
			if(RibbonControl != null && RibbonControl.OptionsTouch.GetTouchUI()) {
				Scale(new SizeF(1.4f + RibbonControl.GetController().LookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + RibbonControl.GetController().LookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
		}
		protected virtual void InitDialog() {
			InitDialogBase();
		}
		protected virtual void InitDialogBase() {
			ApplyFormOptions();
			RibbonCustomizationControl = new RibbonCustomizationUserControl(this);
			substratePanel.Controls.Add(RibbonCustomizationControl);
			RibbonCustomizationControl.Dock = DockStyle.Fill;
		}
		protected virtual void ApplyFormOptions() {
			if(RibbonControl == null)
				return;
			RibbonCustomizationFormOptions options = RibbonControl.OptionsCustomizationForm;
			if(options.FormIcon != null) Icon = options.FormIcon;
		}
		protected internal RibbonCustomizationUserControl RibbonCustomizationControl { get; set; }
		protected internal static RibbonSourceStateInfo RibbonSourceStateInfo { get; private set; }
		#region Public
		public RibbonCustomizationModel GetResult() {
			return ResultModelCreator.Instance.Create(RibbonCustomizationControl.RightTreeView, RibbonControl);
		}
		#endregion
		#region Handlers
		void btnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		#endregion
		#region ICustomizationTopForm
		public OpenFileDialog OpenDialog {
			get { return this.openFileDialog; }
		}
		public SaveFileDialog SaveDialog {
			get { return this.saveFileDialog; }
		}
		public RibbonControl RibbonControl {
			get;
			private set;
		}
		public BarManager BarManager {
			get { return RibbonControl != null ? RibbonControl.Manager : null; }
		}
		public DropDownButton ResetRibbonDropDownButton {
			get { return this.resetOptionsDropDownButton; }
		}
		public void ResetRibbon() {
			RibbonCustomizationModel model = GetResult();
			RibbonControl.ApplyCustomizationSettings(model);
		}
		#endregion
	}
	public interface ICustomizationTopForm {
		OpenFileDialog OpenDialog { get; }
		SaveFileDialog SaveDialog { get; }
		RibbonControl RibbonControl { get; }
		BarManager BarManager { get; }
		DropDownButton ResetRibbonDropDownButton { get; }
		void ResetRibbon();
		RibbonCustomizationModel GetResult();
	}
}
