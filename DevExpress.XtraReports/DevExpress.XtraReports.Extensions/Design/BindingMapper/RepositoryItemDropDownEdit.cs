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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Data.Browsing.Design;
using System.Drawing;
namespace DevExpress.XtraReports.Design.BindingMapper {
	class RepositoryItemDropDownEdit : RepositoryItemPopupContainerEdit {
		readonly IServiceProvider serviceProvider;
		readonly XtraReport report;
		readonly Func<DesignBindingInfo, IServiceProvider, PopupContainerEdit, DesignTreeListBindingPicker> createPicker;
		readonly Action onValueChanged;
		DesignTreeListBindingPicker picker;
		bool popupShowed = false;
		public RepositoryItemDropDownEdit(XtraReport report, IServiceProvider serviceProvider, Func<DesignBindingInfo, IServiceProvider, PopupContainerEdit, DesignTreeListBindingPicker> createPicker, Action onValueChanged) {
			this.serviceProvider = serviceProvider;
			this.createPicker = createPicker;
			this.onValueChanged = onValueChanged;
			this.report = report;
			AllowFocused = false;
			PopupControl = new PopupContainerControl();
			QueryPopUp += OnQueryPopUp;
			Closed += RepositoryItemDropDownEdit_Closed;
		}
		void RepositoryItemDropDownEdit_Closed(object sender, ClosedEventArgs e) {
			PopupContainerEdit edit = (PopupContainerEdit)sender;
			DesignBindingInfo bindingInfo = (DesignBindingInfo)edit.EditValue;
			if(!object.Equals(bindingInfo.DesignBinding, picker.SelectedBinding)) {
				bindingInfo.AssignFrom(picker.SelectedBinding);
				edit.IsModified = true;
				onValueChanged();
			}
			picker.Dispose();
		}
		void OnQueryPopUp(object sender, CancelEventArgs e) {
			PopupContainerEdit edit = (PopupContainerEdit)sender;
			picker = createPicker((DesignBindingInfo)edit.EditValue, serviceProvider, edit);
			if(picker == null)
				return;
			if(!popupShowed) {
				PopupControl.Size = picker.Size;
				popupShowed = true;
			}
			picker.Dock = DockStyle.Fill;
			PopupControl.Controls.Clear();
			PopupControl.Controls.Add(picker);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				QueryPopUp -= OnQueryPopUp;
				Closed -= RepositoryItemDropDownEdit_Closed;
			}
			base.Dispose(disposing);
		}
	}
}
