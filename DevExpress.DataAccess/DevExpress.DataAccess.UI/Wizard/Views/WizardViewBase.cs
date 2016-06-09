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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class WizardViewBase : XtraUserControl, IWizardPageView {
		public void EnableFinish(bool enable) {
			buttonFinish.Enabled = enable;
		}
		public void EnableNext(bool enable) {
			buttonNext.Enabled = enable;
		}
		public void EnablePrevious(bool enable) {
			buttonPrevious.Enabled = enable;
		}
		public event EventHandler Finish {
			add { buttonFinish.Click += value; }
			remove { buttonFinish.Click -= value; }
		}
		public event EventHandler Next {
			add { buttonNext.Click += value; }
			remove { buttonNext.Click -= value; }
		}
		public event EventHandler Previous {
			add { buttonPrevious.Click += value; }
			remove { buttonPrevious.Click -= value; }
		}
		public new event  PaintEventHandler Paint {
			add {
				layoutControlBase.Paint += value;
				panelBaseContent.Paint += value;
			}
			remove {
				layoutControlBase.Paint -= value;
				panelBaseContent.Paint -= value;
			}
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			switch(keyData) {
				case Keys.Back:
					buttonPrevious.PerformClick();
					return true;
				case Keys.Escape:
					Form form = FindForm();
					if(form != null) {
						form.Close();
						return true;
					}
					break;
			}
			return base.ProcessDialogKey(keyData);
		}
		public WizardViewBase() {
			InitializeComponent();
			LocalizeComponent();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			labelHeader.Text = HeaderDescription;
			barAndDockingController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected void MoveForward() {
			SimpleButton activeButton = ActiveButton;
			if(activeButton != null)
				activeButton.PerformClick();
		}
		public SimpleButton ActiveButton {
			get {
				if(buttonNext.Enabled)
					return buttonNext;
				if(buttonFinish.Enabled)
					return buttonFinish;
				return null;
			}
		}
		public virtual string HeaderDescription { get; private set; }
		void LocalizeComponent() {
			buttonNext.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Next);
			buttonFinish.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Finish);
		}
	}
}
