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
using System.IO;
using System.Windows.Documents;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.Xpf.Printing.Exports {
	class XpsExportService : XpsExportServiceBase {
		readonly DocumentPaginator paginator;
		ProgressReflector progressReflector;
		public DocumentPaginator Paginator { 
			get { return paginator;  } 
		}
		public XpsExportService(DocumentPaginator paginator) {
			Guard.ArgumentNotNull(paginator, "paginator");
			this.paginator = paginator;
		}
		public override void Export(Stream stream, XpsExportOptions options, ProgressReflector progressReflector) {
			try {
				this.progressReflector = progressReflector;
				progressReflector.InitializeRange(paginator.PageCount + 2); 
				var exporter = new XpsExporter();
				exporter.ProgressChanged += Exporter_ProgressChanged;
				exporter.CreateDocument(paginator, stream, options);
				exporter.ProgressChanged -= Exporter_ProgressChanged;
			} finally {
				progressReflector.MaximizeRange();
			}
		}
		void Exporter_ProgressChanged(object sender, EventArgs e) {
			Action increaseProgress = () => progressReflector.RangeValue++;
			Dispatcher.CurrentDispatcher.Invoke(increaseProgress, DispatcherPriority.Render);
		}
	}
}
