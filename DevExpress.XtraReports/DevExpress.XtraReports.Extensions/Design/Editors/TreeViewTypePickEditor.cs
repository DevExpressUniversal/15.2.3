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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Design.TypePickEditor;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public class DataSourcePickerPanel : DataSourcePickerPanelBase {	
		public DataSourcePickerPanel(TypePickerTreeView treeView, string hyperLinkText) : base(treeView, hyperLinkText) {
		}
		protected override bool CanAddNewDataSource(object instance) {
			ReportCommandService reportCommandService = Provider.GetService<ReportCommandService>();
			return !(instance is CalculatedField) && !(instance is FormattingRule) &&			   
				reportCommandService != null && (reportCommandService.GetCommandVisibility(ReportCommand.AddNewDataSource) & CommandVisibility.Verb) > 0;
		}
		protected override void OnHyperLinkClick(object sender, LinkLabelLinkClickedEventArgs e) {
			try {
				IDesignerHost designerHost = Provider.GetService<IDesignerHost>();
				if(designerHost != null) {
					IPopupOwner popupOwner = Provider.GetService<IWindowsFormsEditorService>() as IPopupOwner;
					IWin32Window owner = GetOwnerWindow();
					IMenuCommandServiceEx menuCommandService = designerHost.GetService<IMenuCommandService, IMenuCommandServiceEx>();
					if(menuCommandService != null) {
						if(popupOwner != null)
							popupOwner.DisableClosing();
						try {
							IDataContainer dataContainer = (context != null) ? (context.Instance as IDataContainer) : null;
							menuCommandService.GlobalInvoke(ReportCommands.AddNewDataSource, new object[] { owner, dataContainer });
						} finally {
							if(popupOwner != null)
								popupOwner.EnableClosing();
						}
					}
				}
				CloseDropDown();
			} catch { }
		}
		protected IWin32Window GetOwnerWindow() {
			XRSmartTagService smartTagService = Provider.GetService<XRSmartTagService>();
			if(smartTagService != null && smartTagService.PopupIsVisible)
				return smartTagService.PopupWindow;
			Form form = FindForm();
			return (form != null)
				? ((form.Owner != null) ? form.Owner : form)
				: (IWin32Window)this;
		}
	}
	public class DataSourceEditor : TreeViewTypePickEditor {
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new TypePickerTreeView(typeof(System.Data.DataSet), ReportStringId.UD_Title_FieldList_NonePickerNodeText.GetString());
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new DataSourcePickerPanel(picker, ReportStringId.UD_Title_FieldList_AddNewDataSourceText.GetString());
		}
	}
	public class DataAdapterEditor : TreeViewTypePickEditor {
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new TypePickerTreeView(typeof(System.Data.IDataAdapter), ReportStringId.UD_Title_FieldList_NonePickerNodeText.GetString());
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new TypePickerPanel(picker);
		}
	}
}
