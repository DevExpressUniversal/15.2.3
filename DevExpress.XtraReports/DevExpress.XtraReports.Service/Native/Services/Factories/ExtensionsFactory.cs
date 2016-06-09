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
using System.Linq;
using System.Reflection;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.Extensions;
namespace DevExpress.XtraReports.Service.Native.Services.Factories {
	public class ExtensionsFactory : IExtensionsFactory {
		public class ReflectionTypeLoadExceptionWrapper : Exception {
			static readonly string Separator = new string('-', 10) + Environment.NewLine;
			static string CombineMessages(ReflectionTypeLoadException exception) {
				var details = string.Join(Separator, exception.LoaderExceptions.Select(e => e.Message));
				return string.Concat(exception.Message, Separator, details);
			}
			public ReflectionTypeLoadExceptionWrapper() {
			}
			public ReflectionTypeLoadExceptionWrapper(ReflectionTypeLoadException exception)
				: base(CombineMessages(exception), exception) {
			}
		}
		readonly IServiceProvider serviceProvider;
		public ExtensionsFactory(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
		}
		#region IExtensionService Members
		public IEnumerable<IReportResolver> GetReportResolvers() {
			return Get<IReportResolver>();
		}
		public IEnumerable<IDataSourceService> GetDataSourceServices() {
			return Get<IDataSourceService>();
		}
		public IEnumerable<IInstanceIdentityResolver> GetInstanceIdentityResolvers() {
			return Get<IInstanceIdentityResolver>();
		}
		public IEnumerable<IReportBuildInterceptor> GetReportBuildInterceptors() {
			return Get<IReportBuildInterceptor>();
		}
		public IEnumerable<IDocumentPrintInterceptor> GetDocumentPrintInterceptors() {
			return Get<IDocumentPrintInterceptor>();
		}
		public IEnumerable<IDocumentDataStorageProvider> GetDocumentDataStorageProviders() {
			return Get<IDocumentDataStorageProvider>();
		}
		public IEnumerable<IDocumentExportInterceptor> GetDocumentExportInterceptors() {
			return Get<IDocumentExportInterceptor>();
		}
		public IEnumerable<IBinaryDataStorageExtension> GetBinaryDataStorageExtensions() {
			return Get<IBinaryDataStorageExtension>();
		}
		#endregion
		IEnumerable<T> Get<T>() {
			var extensionsResolver = serviceProvider.GetService<IExtensionsResolver>();
			try {
				return extensionsResolver.GetExtensions<T>();
			} catch(ReflectionTypeLoadException e) {
				throw new ReflectionTypeLoadExceptionWrapper(e);
			}
		}
	}
}
