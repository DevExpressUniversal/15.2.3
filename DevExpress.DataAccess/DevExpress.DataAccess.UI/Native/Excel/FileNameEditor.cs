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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.UI.Native.Excel {
	public class FileNameEditor : UITypeEditor {
		static void UpdateOptions(ITypeDescriptorContext context, ExcelDataSource excelDataSource, ExcelSourceOptionsBase sourceOptions) {
			var componentChangeService = (IComponentChangeService)context.GetService(typeof(IComponentChangeService));
			componentChangeService.OnComponentChanging(excelDataSource, XRExcelDataSourceDesigner.sourceOptionsPropertyDescriptor);
			excelDataSource.SourceOptions = sourceOptions;
			componentChangeService.OnComponentChanged(excelDataSource, XRExcelDataSourceDesigner.sourceOptionsPropertyDescriptor, null, null);
			IDesignerHost host = context.GetService<IDesignerHost>();
			ISelectionService selectionService = context.GetService<ISelectionService>();
			selectionService.SetSelectedComponents(new object[] { host.RootComponent });
			selectionService.SetSelectedComponents(new object[] { context.Instance });
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = FileNameFilterStrings.ExcelCsv }) {
				DialogResult dialogResult = openFileDialog.ShowDialog();
				if(dialogResult == DialogResult.OK) {
					var excelDataSource = (ExcelDataSource) context.Instance;
					ExcelDocumentFormat docFormat;
					try {
						docFormat = ExcelDataLoaderHelper.DetectFormat(openFileDialog.FileName);
					}
					catch(ArgumentException) {
						return value;
					}
					catch(IOException) {
						return value;
					}
					if(docFormat == ExcelDocumentFormat.Csv && excelDataSource.SourceOptions is ExcelSourceOptions) {
						UpdateOptions(context, excelDataSource, new CsvSourceOptions());
					}
					else if(docFormat != ExcelDocumentFormat.Csv && excelDataSource.SourceOptions is CsvSourceOptions) {
						UpdateOptions(context, excelDataSource, new ExcelSourceOptions());
					}
					return openFileDialog.FileName;
				}
				return value;
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext typeDescriptorContext) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
