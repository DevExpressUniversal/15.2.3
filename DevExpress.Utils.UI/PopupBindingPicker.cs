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

using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
using DevExpress.Data.Browsing.Design;
using System;
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Native.Data;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Browsing;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using DevExpress.Data;
namespace DevExpress.XtraReports.Design {
	public class PopupEditorService : IWindowsFormsEditorService, IServiceProvider {
		Control control;
		IServiceProvider serviceProvider;
		public PopupEditorService(Control control, IServiceProvider serviceProvider) {
			this.control = control;
			this.serviceProvider = serviceProvider;
		}
		public void CloseDropDown() {
			PopupContainerEdit popupEdit = control as PopupContainerEdit;
			if (popupEdit != null) {
				popupEdit.ClosePopup();
				return;
			}
			DevExpress.XtraBars.PopupControlContainer popupControlContainer = control as DevExpress.XtraBars.PopupControlContainer;
			if (popupControlContainer != null)
				popupControlContainer.HidePopup();
		}
		public void DropDownControl(Control control) {
		}
		public DialogResult ShowDialog(Form dialog) {
			return DialogResult.None;
		}
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			if (serviceType == typeof(IWindowsFormsEditorService))
				return this;
			return serviceProvider.GetService(serviceType);
		}
		#endregion
	}
	public class PopupBindingPickerBase : DesignTreeListBindingPicker {
		public PopupBindingPickerBase(TreeListPickManager manager)
			: base(manager) {
			BorderStyle = BorderStyles.NoBorder;
			HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Auto;
			VertScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Auto;
		}
		public void Start(Collection<Pair<object, string>> dataSourceDataMemberPairs, IServiceProvider serviceProvider, DesignBinding selectedBinding, Control containerControl) {
			if(containerControl == null) {
				Start(dataSourceDataMemberPairs, serviceProvider, selectedBinding);
				return;
			}
			Start(dataSourceDataMemberPairs, new PopupEditorService(containerControl, serviceProvider), selectedBinding);
			Control parent = GetParent(containerControl);
			parent.Size = this.Size;
			this.Dock = DockStyle.Fill;
			parent.Controls.Add(this);
		}
		public void Start(IList dataSources, IServiceProvider serviceProvider, DesignBinding selectedBinding, Control containerControl) {
			Collection<Pair<object, string>> dataSourcesDictionary = new Collection<Pair<object, string>>();
			foreach(object dataSource in dataSources)
				dataSourcesDictionary.Add(new Pair<object, string>(dataSource, string.Empty));
			Start(dataSourcesDictionary, serviceProvider, selectedBinding, containerControl);
		}
		static Control GetParent(Control control) {
			return control is PopupContainerEdit ? ((PopupContainerEdit)control).Properties.PopupControl : control;
		}
		protected DesignBinding GetResultBinding() {
			DesignBinding val = SelectedBinding;
			End();
			return val;
		}
	}
	public class PopupBindingPicker : PopupBindingPickerBase {
		public PopupBindingPicker()
			: base(new TreeListPickManager(new DataContextOptions(true, true))) {
		}
		public DesignBinding EndBindingPicker() {
			return GetResultBinding();
		}
	}
}
