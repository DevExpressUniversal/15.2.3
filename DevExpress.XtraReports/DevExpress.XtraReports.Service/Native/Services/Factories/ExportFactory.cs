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
using System.Drawing.Imaging;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.Services.Factories {
	public class ExportFactory : IExportFactory {
		readonly IServiceProvider serviceProvider;
		public ExportFactory(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
		}
		#region IExportFactory
		public IExportMediator CreateExportMediator(ExportId exportId, MediatorInitialization init) {
			var mediator = serviceProvider.GetService<IExportMediator>();
			mediator.Initialize(exportId, init);
			return mediator;
		}
		public IDocumentExporter CreateDocumentExporter() {
			return serviceProvider.GetService<IDocumentExporter>();
		}
		public IXamlPagesExporter CreateXamlPagesExporter() {
			return serviceProvider.GetService<IXamlPagesExporter>();
		}
		public IPagesExporter CreatePrnxPagesExporter() {
			return serviceProvider.GetService<PrnxPagesExporter>();
		}
		public IPagesExporter CreateHtmlPagesExporter() {
			return serviceProvider.GetService<HtmlPagesExporter>();
		}
		public IPagesExporter CreateImagePagesExporter(ImageFormat imageFormat, int? resolution) {
			Guard.ArgumentNotNull(imageFormat, "imageFormat");
			var exporter = serviceProvider.GetService<ImagePagesExporter>();
			exporter.ImageFormat = imageFormat;
			if(resolution.HasValue) {
				exporter.Resolution = resolution.Value;
			}
			return exporter;
		}
		#endregion
	}
}
