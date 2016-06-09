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
using System.Configuration;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.ConfigSections;
using DevExpress.XtraReports.Service.ConfigSections.Native;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.Services.Factories;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class ConfigurationService : IConfigurationService {
		readonly object syncRoot = new object();
		readonly IExtensionsFactory extensionsFactory;
		readonly IConfigurationSettings configurationSettings;
		IDocumentDataStorageProvider documentStoreConfiguration;
		public ConfigurationService(IExtensionsFactory extensionsFactory, IConfigurationSettingsProvider provider) {
			Guard.ArgumentNotNull(extensionsFactory, "extensionsFactory");
			Guard.ArgumentNotNull(provider, "provider");
			IConfigurationSettings configurationSettings = provider.ConfigurationSettings;
			Guard.ArgumentNotNull(configurationSettings, "provider.ConfigurationSettings");
			this.extensionsFactory = extensionsFactory;
			this.configurationSettings = configurationSettings;
		}
		#region IConfigurationService
		public IDocumentDataStorageProvider DocumentStoreConfiguration {
			get {
				Helper.DoubleCheckInitialize(ref documentStoreConfiguration, syncRoot, CreateDocumentDataStorageProvider);
				return documentStoreConfiguration;
			}
		}
		#endregion
		IDocumentDataStorageProvider CreateDocumentDataStorageProvider() {
			return extensionsFactory.GetDocumentDataStorageProviders().FirstOrDefault()
				?? OptionalWrap(configurationSettings.GetPrintingServiceSection())
				?? CreateDefaultDocumentDataStorageProvider();
		}
		IDocumentDataStorageProvider OptionalWrap(PrintingServiceSection section) {
			if(section == null) {
				return null;
			}
			DocumentStoreSection documentStore = section.DocumentStore;
			if(documentStore == null) {
				return null;
			}
			return new DocumentDataStorageProvider(
				binaryStorageChunkSize: documentStore.BinaryStorageChunkSize,
				connectionString: ReadConnectionString(documentStore.ConnectionStringName),
				keepInterval: TimeSpan.FromMilliseconds(documentStore.KeepInterval));
		}
		IDocumentDataStorageProvider CreateDefaultDocumentDataStorageProvider() {
			return new DocumentDataStorageProvider(
				binaryStorageChunkSize: ConfigurationDefaultConstants.BinaryStorageChunkSizeValue,
				connectionString: ReadConnectionString(string.Empty),
				keepInterval: TimeSpan.FromMilliseconds(ConfigurationDefaultConstants.DocumentStoreKeepIntervalValue));
		}
		string ReadConnectionString(string connectionStringName) {
			bool useFallback = string.IsNullOrEmpty(connectionStringName);
			string usedConnectionStringName = useFallback ? ConfigurationDefaultConstants.ConnectionStringNameValue : connectionStringName;
			ConnectionStringSettings connectionString = configurationSettings.ConnectionStrings[usedConnectionStringName];
			if(connectionString == null) {
				if(useFallback) {
					return string.Empty;
				}
				throw new InvalidOperationException(string.Format("The connection name '{0}' was not found in the applications configuration", connectionStringName));
			}
			return connectionString.ConnectionString;
		}
	}
}
