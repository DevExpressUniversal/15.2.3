#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel.DesignService;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security;
using System.Windows.Forms.Design;
namespace DevExpress.DashboardWin.Native {
	public class DataItemDataMemberSelectorEditor : UITypeEditor {
		static DataFieldsBrowserDisplayMode GetDisplayMode(DataItem dataItem) {
			Measure measure = dataItem as Measure;
			if (measure != null)
				return DataFieldsBrowserDisplayMode.Measures;
			Dimension dimension = dataItem as Dimension;
			if (dimension != null)
				return DataFieldsBrowserDisplayMode.Dimensions;
			return DataFieldsBrowserDisplayMode.All;
		}
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
		public DataItemDataMemberSelectorEditor()
			: base() {
		}
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override bool IsDropDownResizable {
			[SecuritySafeCritical]
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
			get { return true; }
		}
		[SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editorService = provider.GetService<IWindowsFormsEditorService>();
			if (editorService != null) {			   
				using (DataFieldsBrowser dataMemberSelector = new DataFieldsBrowser()) {
					LookAndFeelProviderHelper.SetParentLookAndFeel(dataMemberSelector, provider);
					dataMemberSelector.SetToolbarVisibility(false);
					DataSourceInfo dataSource = GetDataSource(context);
					DataItem dataItem = context.Instance as DataItem;
					DataFieldsBrowserDisplayMode mode = GetDataFieldsBrowserDisplayMode(context.PropertyDescriptor.Attributes);
					if (mode == DataFieldsBrowserDisplayMode.All)
						mode = GetDisplayMode(dataItem);
					IServiceProvider dashboardServiceProvider = GetDashboardServiceProvider(context);
					IDataFieldsBrowserPresenterFactory factory = dashboardServiceProvider.RequestServiceStrictly<IDataFieldsBrowserPresenterFactory>();
					using (DataFieldsBrowserPresenter presenter = factory.CreatePresenter(dataMemberSelector, dataSource, provider)) {
						presenter.DisplayMode = mode;
						presenter.ExpandGroups = true;
						if (dataItem != null)
							presenter.SelectNode(dataItem.DataMember);
						presenter.DataFieldDoubleClick += (s, e) => {
							DataField field = e.DataField;
							if (field != null && field.IsDataMemberNode && !string.IsNullOrEmpty(field.DataMember)) {
								value = field.DataMember;
							}
							editorService.CloseDropDown();
						};
						editorService.DropDownControl(dataMemberSelector);
						return value;
					}
				}
			}
			else
				return base.EditValue(context, provider, value);
		}
		DataFieldsBrowserDisplayMode GetDataFieldsBrowserDisplayMode(AttributeCollection attributeCollection) {
			foreach(Attribute attr in attributeCollection) {
				DataFieldsBrowserAttribute att = attr as DataFieldsBrowserAttribute;
				if(att != null && att.DisplayMode != DataFieldsBrowserDisplayMode.All)
					return att.DisplayMode;
			}
			return DataFieldsBrowserDisplayMode.All;
		}
		IServiceProvider GetDashboardServiceProvider(ITypeDescriptorContext context) {
			SelectedContextService selectedContextService = context.GetService<SelectedContextService>();
			return selectedContextService.DashboardServiceProvider;
		}
		protected virtual DataSourceInfo GetDataSource(ITypeDescriptorContext context) {
			SelectedContextService selectedContextService = context.GetService<SelectedContextService>();
			return selectedContextService.DataSourceInfo;
		}
	}
}
