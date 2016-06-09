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

using DevExpress.Utils;
using DevExpress.XtraReports.Service.Native.Services;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
using DevExpress.XtraReports.Service.Native.Services.Domain;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.ServiceContainer {
	public class XRBootstrapper {
		static readonly XRBootstrapper Instance = new XRBootstrapper();
		static XRBootstrapper() {
			DefaultLogger.SetCurrent(new LoggingService());
		}
		public static void StaticInitialize() {
		}
		public static IntegrityContainerCreator CreateInitializedContainer() {
			return new IntegrityContainerCreator(0, () => CreateInitializedContainer(Instance));
		}
		public static IIntegrityContainer CreateContainer() {
			return new ServiceIntegrityContainer();
		}
		public static void InitializeContainer(IIntegrityContainer container) {
			Instance.InitializeContainerCore(container);
		}
		protected static IIntegrityContainer CreateInitializedContainer(XRBootstrapper bootstrapper) {
			var container = CreateContainer();
			bootstrapper.InitializeContainerCore(container);
			return container;
		}
		protected virtual void InitializeContainerCore(IIntegrityContainer container) {
			container.RegisterType<IDocumentStoreService, DocumentStoreService>();
			container.RegisterType<IDocumentExportService, DocumentExportService>();
			container.RegisterType<IPollingService, PollingService>();
			container.RegisterType<IConfigurationSettingsProvider, ConfigurationSettingsProvider>();
			container.RegisterType<IConfigurationService, ConfigurationService>();
			container.RegisterType<IThreadFactoryService, ThreadFactoryService>();
			container.RegisterType<ISerializationService, SerializationService>();
			container.RegisterType<IResourceProviderService, InMemoryResourceProviderService>();
			container.RegisterType<IDocumentMediatorFactory, DocumentMediatorFactory>();
			container.RegisterType<IExportFactory, ExportFactory>();
			container.RegisterType<IExtensionsResolver, ConfiguredMefExtensionsResolver>();
			container.RegisterType<IExtensionsFactory, ExtensionsFactory>();
			container.RegisterType<IBinaryDataStorageServiceProvider, BinaryDataStorageServiceProvider>();
			container.RegisterType<IBinaryDataStorageService, XpoBinaryDataStorageService>();
			container.RegisterType<ICleanService, CleanService>();
			container.RegisterType<IDALService, DALService>();
			container.RegisterType<IPrintingSystemFactory, PrintingSystemFactory>();
			container.RegisterType<IDocumentExporter, DocumentExporter>(ContainerRegistrationKind.Transient);
			container.RegisterType<IXamlPagesExporter, XamlPagesExporter>(ContainerRegistrationKind.Transient);
			container.RegisterType<IDocumentMediator, DocumentMediator>(ContainerRegistrationKind.Transient);
			container.RegisterType<IExportMediator, ExportMediator>(ContainerRegistrationKind.Transient);
			container.RegisterType<IIntermediateReportService, IntermediateReportService>();
			container.RegisterType<IDocumentBuildService, DocumentBuildService>();
			container.RegisterType<IDocumentStoreService, DocumentStoreService>();
			container.RegisterType<IPrintingSystemConfigurationService, PrintingSystemConfigurationService>();
			container.RegisterType<IReportFactory, ReportFactory>();
			container.RegisterType<IDocumentBuildFactory, DocumentBuildFactory>();
			container.RegisterType<IDocumentBuilder, DocumentBuilder>(ContainerRegistrationKind.Transient);
			container.RegisterType<IPagesRequestProcessor, PagesRequestProcessor>(ContainerRegistrationKind.Transient);
			container.RegisterType<UrlResolver, XRServiceUrlResolver>(ContainerRegistrationKind.Transient);
			container.RegisterType<IDocumentPrintService, DocumentPrintService>();
			container.RegisterType<IPrintFactory, PrintFactory>();
			container.RegisterType<IPrintMediator, PrintMediator>(ContainerRegistrationKind.Transient);
			container.RegisterType<IDocumentPrinter, DocumentPrinter>(ContainerRegistrationKind.Transient);
		}
	}
}
