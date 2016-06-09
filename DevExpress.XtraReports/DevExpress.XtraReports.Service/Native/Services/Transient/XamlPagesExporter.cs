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

using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class XamlPagesExporter : PagesExporterBase, IXamlPagesExporter {
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly XamlExporter xamlExporter = new XamlExporter();
		public DevExpress.XtraPrinting.XamlExport.XamlCompatibility Compatibility { get; set; }
		public XamlExporter InnerExporter {
			get { return xamlExporter; }
		}
		public override bool ExclusivelyDocumentUsing {
			get { return false; }
		}
		public XamlPagesExporter(ISerializationService serializationService)
			: base(serializationService) {
		}
		#region IXamlPagesExporter
		public void Export(Document document, int pageIndex, Stream stream) {
			var ps = document.PrintingSystem;
			ps.ReplaceService<IBrickPublisher>(new DefaultBrickPublisher());
			try {
				xamlExporter.Export(stream, document.Pages[pageIndex], pageIndex + 1, document.Pages.Count, Compatibility, TextMeasurementSystem.GdiPlus);
			} finally {
				ps.RemoveService<IBrickPublisher>();
			}
		}
		#endregion
		protected override string ExportCore(Document document, int pageIndex) {
			using(var stream = new MemoryStream()) {
				Export(document, pageIndex, stream);
				return stream.GetUTF8String();
			}
		}
	}
}
