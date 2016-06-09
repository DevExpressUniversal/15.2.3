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
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConfigureExcelFileColumnsPage<TModel> : WizardPageBase<IConfigureExcelFileColumnsPageView, TModel> where TModel : IExcelDataSourceModel {
		public ConfigureExcelFileColumnsPage(IConfigureExcelFileColumnsPageView view) : base(view) { }
		#region Overrides of WizardPageBase<IConfigureExcelFileColumnsPageView,TModel>
		public override bool FinishEnabled {
			get { return CanMoveForward(); }
		}
		public override void Begin() {
			View.Changed += View_Changed;
			View.Initialize(Model.Schema, LoadPreviewData);
		}
		public override void Commit() {
			View.Changed -= View_Changed;
			Model.Schema = View.Schema;
			var selectedSchema = View.Schema.Where(fi => fi.Selected).ToArray();
			Model.DataSchema = new DataView(null, new SelectedDataEx(new IList[selectedSchema.Length], selectedSchema.Select(s => new ColumnInfoEx { Name = s.Name, Type = s.Type }).ToArray()));
		}
		#endregion
		protected bool CanMoveForward() {
			return View.Schema.Any(gr => gr.Selected);
		}
		SelectedDataEx LoadPreviewData(FieldInfo[] schema) {
			using(var source = ExcelDataLoaderHelper.CreateSource(null, ExcelDataLoaderHelper.DetectFormat(Model.FileName), Model.FileName, Model.SourceOptions))
				return ExcelDataLoaderHelper.LoadData(source, schema, Model.SourceOptions, true);
		}
		void View_Changed(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
}
