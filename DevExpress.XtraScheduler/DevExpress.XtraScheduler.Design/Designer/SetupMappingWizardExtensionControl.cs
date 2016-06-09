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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Design.Wizards;
namespace DevExpress.XtraScheduler.Design {
	public partial class SetupMappingWizardExtensionControl : UserControl, IMappingWizardExtensionView {
		bool isRestrictionActive;
		public SetupMappingWizardExtensionControl() {
			InitializeComponent();
			this.chkGanttSupport.CheckedChanged += new EventHandler(OnChkGanttSupportCheckedChanged);
			IsRestrictionActive = chkGanttSupport.Checked;
		}
		#region ISetupMappingsWizardExtension implementation
		public string Caption {
			get {
				return this.chkGanttSupport.Text;
			}
			set {
				this.chkGanttSupport.Text = value;
			}
		}
		public string Description {
			get {
				return this.lblDescription.Text;
			}
			set {
				this.lblDescription.Text = value;
			}
		}
		public string Link {
			get {
				return this.hyperLinkEdit1.Text;
			}
			set {
				this.hyperLinkEdit1.Text = value;
			}
		}
		public string LinkCaption {
			get {
				return this.hyperLinkEdit1.Properties.Caption;
			}
			set {
				this.hyperLinkEdit1.Properties.Caption = value;
			}
		}
		public bool IsRestrictionActive {
			get {
				return isRestrictionActive;
			}
			set {
				if (IsRestrictionActive == value)
					return;
				isRestrictionActive = value;
				chkGanttSupport.Checked = value;
				RaiseRestrictionActiveChanged();
			}
		}
		#region RestrictionActiveChanged event
		public event EventHandler RestrictionActiveChanged;
		protected void RaiseRestrictionActiveChanged() {
			if (RestrictionActiveChanged != null)
				RestrictionActiveChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void OnChkGanttSupportCheckedChanged(object sender, EventArgs e) {
			IsRestrictionActive = this.chkGanttSupport.Checked;
		}
	}
}
