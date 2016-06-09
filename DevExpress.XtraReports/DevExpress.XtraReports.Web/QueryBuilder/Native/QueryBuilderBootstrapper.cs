#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.Utils;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.QueryBuilder.Native.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
namespace DevExpress.XtraReports.Web.QueryBuilder.Native {
	public static class QueryBuilderBootstrapper {
		readonly static List<Action<IServiceContainerRegistrator>> appendServices = new List<Action<IServiceContainerRegistrator>>(0);
		internal static ServiceIntegrityContainer CreateInitializedContainer() {
			var container = new ServiceIntegrityContainer();
			var registrator = new IntegrityContainerRegistrator(container);
			RegisterStandardServices(registrator);
			foreach(var action in appendServices) {
				action(registrator);
			}
			return container;
		}
		public static void AppendRegistration(Action<IServiceContainerRegistrator> action) {
			Guard.ArgumentNotNull(action, "action");
			appendServices.Add(action);
		}
		public static void RegisterStandardServices(IServiceContainerRegistrator registrator) {
			registrator.RegisterSingleton<IQueryBuilderRequestManager, QueryBuilderRequestManager>();
			registrator.RegisterTransient<IQueryBuilderRequestController, QueryBuilderRequestController>();
			registrator.RegisterSingleton<IJSContentGenerator<QueryBuilderModel>, QueryBuilderJSContentGenerator>();
			registrator.RegisterSingleton<IQueryBuilderHtmlContentGenerator, QueryBuilderHtmlContentGenerator>();
			registrator.RegisterTransient<IQueryBuilderModelGenerator, QueryBuilderModelGenerator>();
			registrator.RegisterSingleton<ISqlDataSourceWizardService, SqlDataSourceWizardService>();
			registrator.RegisterSingleton<IDataSourceWizardDBSchemaProviderFactory, DataSourceWizardDBSchemaProviderFactory>();
			registrator.RegisterSingleton<IDataSourceWizardConnectionStringsProvider, WizardConnectionStringsProvider>();
			registrator.RegisterSingleton<ISqlDataSourceWizardCustomizationService, SqlDataSourceWizardCustomizationService>();
		}
	}
}
