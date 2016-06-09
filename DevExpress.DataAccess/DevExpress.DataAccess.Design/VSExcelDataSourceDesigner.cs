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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Excel;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
namespace DevExpress.DataAccess.Design {
	public class VSExcelDataSourceDesigner : XRExcelDataSourceDesigner {
		#region Overrides of XRExcelDataSourceDesigner
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ProjectHelper.AddReference(component.Site, AssemblyInfo.SRAssemblyXpo + AssemblyInfo.FullAssemblyVersionExtension);
			ProjectHelper.AddReference(component.Site, AssemblyInfo.SRAssemblyDataAccess + AssemblyInfo.FullAssemblyVersionExtension);
		}
		protected override UserLookAndFeel GetLookAndFeel(IServiceProvider serviceProvider) {
			return VSLookAndFeelHelper.GetLookAndFeel(designerHost);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			var uiService = this.designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IExcelSchemaProvider excelSchemaProvider = this.designerHost.GetService<IExcelSchemaProvider>();
			using (var lookAndFeel = GetLookAndFeel(designerHost)) {
				var transaction = designerHost.CreateTransaction();
				componentChangeService.OnComponentChanging(Component, fileNamePropertyDescriptor);
				componentChangeService.OnComponentChanging(Component, sourceOptionsPropertyDescriptor);
				componentChangeService.OnComponentChanging(Component, schemaPropertyDescriptor);
				if(!((ExcelDataSource) Component).EditDataSource(lookAndFeel, owner, excelSchemaProvider)) {
					transaction.Cancel();
					return;
				}
				componentChangeService.OnComponentChanged(Component, fileNamePropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(Component, sourceOptionsPropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(Component, schemaPropertyDescriptor, null, null);
				transaction.Commit();
			}
		}
		#endregion
	}
}
