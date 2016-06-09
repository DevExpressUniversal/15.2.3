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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraWizard {
	public delegate void WizardPageChangedEventHandler(object sender, WizardPageChangedEventArgs e);
	public delegate void WizardPageChangingEventHandler(object sender, WizardPageChangingEventArgs e);
	public delegate void WizardCommandButtonClickEventHandler(object sender, WizardCommandButtonClickEventArgs e);
	public delegate void WizardButtonClickEventHandler(object sender, WizardButtonClickEventArgs e);
	public delegate void WizardPageValidatingEventHandler(object sender, WizardPageValidatingEventArgs e);
	public delegate void WizardPageCommitEventHandler(object sender, EventArgs e);
	public delegate void WizardPageRollbackEventHandler(object sender, EventArgs e);
	public delegate void WizardCustomizeCommandButtonsEventHandler(object sender, CustomizeCommandButtonsEventArgs e); 
	public class WizardPageEventArgs : EventArgs {
		BaseWizardPage page;
		public WizardPageEventArgs(BaseWizardPage page) {
			this.page = page;
		}
		public BaseWizardPage Page { get { return page; } protected set { page = value; } }
	}
	public class WizardCommandButtonClickEventArgs : WizardButtonClickEventArgs {
		bool handled;
		public WizardCommandButtonClickEventArgs(BaseWizardPage page)
			: base(page) {
			this.handled = false;
		}
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public class WizardButtonClickEventArgs : WizardPageEventArgs {
		public WizardButtonClickEventArgs(BaseWizardPage page) : base(page) { }
	}
	public class WizardPageChangedEventArgs : WizardPageEventArgs {
		BaseWizardPage prevPage;
		Direction direction;
		public WizardPageChangedEventArgs(BaseWizardPage prevPage, BaseWizardPage page, Direction direction)
			: base(page) {
			this.prevPage = prevPage;
			this.direction = direction;
		}
		public BaseWizardPage PrevPage { get { return prevPage; } }
		public Direction Direction { get { return direction; } }
	}
	public class WizardPageChangingEventArgs : WizardPageChangedEventArgs {
		bool cancel = false;
		public WizardPageChangingEventArgs(BaseWizardPage prevPage, BaseWizardPage page, Direction direction) : base(prevPage, page, direction) { }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public new BaseWizardPage Page { get { return base.Page; } set { base.Page = value; } }
	}
	public class WizardPageValidatingEventArgs : EventArgs {
		bool valid;
		string errorText;
		MessageBoxIcon errorIconType;
		Direction direction;
		public WizardPageValidatingEventArgs(Direction direction) {
			this.valid = true;
			this.errorText = string.Empty;
			this.errorIconType = MessageBoxIcon.Error;
			this.direction = direction;
		}
		public Direction Direction {
			get { return direction; }
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		public MessageBoxIcon ErrorIconType {
			get { return errorIconType; }
			set { errorIconType = value; }
		}
	}
	public class CustomizeCommandButtonsEventArgs : WizardPageEventArgs {
		WizardViewInfo viewInfo;
		public CustomizeCommandButtonsEventArgs(WizardViewInfo viewInfo, BaseWizardPage page) : base(page) {
			this.viewInfo = viewInfo;
		}
		protected WizardViewInfo ViewInfo { get { return viewInfo; } }
		public ButtonInfo HelpButton { get { return ViewInfo.HelpButton; } }
		public ButtonInfo CancelButton { get { return ViewInfo.CancelButton; } }
		public ButtonInfo PrevButton { get { return ViewInfo.PrevButton; } }
		public ButtonInfo NextButton { get { return ViewInfo.NextButton; } }
		public ButtonInfo FinishButton { get { return ViewInfo.FinishButton; } }
	}
}
