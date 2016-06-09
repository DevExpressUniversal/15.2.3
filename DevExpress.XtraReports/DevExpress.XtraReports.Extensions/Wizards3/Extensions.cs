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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraReports.Wizards3 {
	using DevExpress.DataAccess.UI.Wizard;
	using DevExpress.DataAccess.Wizard.Model;
	using DevExpress.XtraReports.UI;
	using System.ComponentModel.Design;
	using DevExpress.XtraReports.Wizards3.Builder;
	using DevExpress.XtraReports.Design;
	using DevExpress.DataAccess.Wizard;
	using DevExpress.DataAccess.UI.Native.Sql;
	using DevExpress.DataAccess.Wizard.Services;
	using DevExpress.XtraReports.UserDesigner;
	public static class IWizardCustomizationServiceExtentions {
		public static void CustomizeReportWizardSafely(this IWizardCustomizationService serv, IWizardCustomization<XtraReportModel> tool) {
			if(serv != null) serv.CustomizeReportWizard(tool);
		}
		public static void CustomizeDataSourceWizardSafely(this IWizardCustomizationService serv, IWizardCustomization<DataSourceModel> tool) {
			if(serv != null) serv.CustomizeDataSourceWizard(tool);
		}
		public static void CreateDataSourceSafely(this IWizardCustomizationService serv, IDataSourceModel model, out object dataSource, out string dataMember) {
			if(serv != null && serv.TryCreateDataSource(model, out dataSource, out dataMember))
				return;
			var c = new DataComponentCreator().CreateDataComponent(model);
			dataSource = c;
			dataMember = c != null ? c.DataMember : string.Empty;
		}
		public static void CreateReportSafely(this IWizardCustomizationService serv, IDesignerHost designerHost, XtraReportModel model, object dataSource, string dataMember) {
			if(serv != null && serv.TryCreateReport(designerHost, model, dataSource, dataMember))
				return;
			else {
				var builder = new XtraWizardReportBuilder(designerHost, dataSource, dataMember);
				builder.Build((XtraReport)designerHost.RootComponent, model);
			}
			new BandsValidator(designerHost).EnsureExistence(BandKind.TopMargin, BandKind.BottomMargin);
		}
		public static SqlWizardOptions GetSqlWizardOptions(this ISqlWizardOptionsProvider serv) {
			return serv != null ? serv.SqlWizardOptions : SqlWizardOptions.None;
		}
	}
	public interface IWizardCustomizationService {
		void CustomizeReportWizard(IWizardCustomization<XtraReportModel> tool);
		void CustomizeDataSourceWizard(IWizardCustomization<DataSourceModel> tool);
		bool TryCreateDataSource(IDataSourceModel model, out object dataSource, out string dataMember);
		bool TryCreateReport(IDesignerHost designerHost, XtraReportModel model, object dataSource, string dataMember);
	}
	public class SqlWizardOptionsProvider : ISqlWizardOptionsProvider {
		Func<SqlWizardOptions> getOptions;
		SqlWizardOptions ISqlWizardOptionsProvider.SqlWizardOptions {
			get { return getOptions(); }
		}
		public SqlWizardOptionsProvider(Func<SqlWizardOptions> getOptions) {
			this.getOptions = getOptions;
		}
	}
	class XRCustomQueryValidator : CustomQueryValidator {
		Action<ValidateSqlEventArgs> callback;
		public XRCustomQueryValidator(Action<ValidateSqlEventArgs> callback) {
			this.callback = callback;
		}
		public override bool Validate(DataAccess.ConnectionParameters.DataConnectionParametersBase connectionParameters, string sql, ref string message) {
			ValidateSqlEventArgs e = new ValidateSqlEventArgs(connectionParameters, sql);
			e.Valid = base.Validate(connectionParameters, sql, ref message);
			e.Message = message;
			callback(e);
			message = e.Message;
			return e.Valid;
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner {
	using DataAccess.ConnectionParameters;
	public class ValidateSqlEventArgs : EventArgs {
		public bool Valid { get; set; }
		public string Message { get; set; }
		public string Sql { get; private set; }
		public DataConnectionParametersBase ConnectionParameters { get; private set; }
		public ValidateSqlEventArgs(DataAccess.ConnectionParameters.DataConnectionParametersBase connectionParameters, string sql) {
			ConnectionParameters = connectionParameters;
			Sql = sql;
		}
	}
}
namespace DevExpress.XtraReports.Wizards3 {
	static class TreeListExtension {
		public static TreeListNode FindItem(this TreeList treeList, Predicate<TreeListNode> predicate){
			TreeListNode result = FindItem(treeList.Nodes, predicate);
			return result;
		}
		static TreeListNode FindItem(TreeListNodes items, Predicate<TreeListNode> predicate) {
			foreach(TreeListNode item in items) {
				if(predicate(item))
					return item;
				var result = FindItem(item.Nodes, predicate);
				if(result != null)
					return result;
			}
			return null;
		}
	}
	static class DisposableExtensions {
		public static void SafeDispose(this IDisposable disposableObject) {
			if(disposableObject != null)
				disposableObject.Dispose();
		}
		public static void SafeDispose(this IEnumerable<IDisposable> disposableObjectCollection) {
			if(disposableObjectCollection != null) {
				List<IDisposable> disposableObjectList = new List<IDisposable>(disposableObjectCollection);
				foreach(IDisposable disposableObject in disposableObjectList) {
					if(disposableObject != null)
						disposableObject.Dispose();
				}
				disposableObjectList.Clear();
			}
		}
		public static void SafeDispose(this IDisposable[] disposableObjectCollection) {
			if(disposableObjectCollection != null)
				SafeDispose(disposableObjectCollection.OfType<IDisposable>());
		}
		public static void TryDisposeIfPossible(this object instance) {
			IDisposable disposable = instance as IDisposable;
			disposable.SafeDispose();
		}
	}
}
