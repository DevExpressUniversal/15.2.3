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
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
#if !SL
using System.ComponentModel;
#endif
namespace DevExpress.Xpf.Printing {
	public class PrintingSystem : PrintingSystemBase {
		#region Fields & Properties
		IDocumentFactory documentFactory = new PrintingDocumentFactory();
		internal IDocumentFactory DocumentFactory {
			get {
				return documentFactory;
			}
			set {
				Guard.ArgumentNotNull(value, "value");
				documentFactory = value;
			}
		}
#if !SL
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("PrintingSystemExportOptions")]
#endif
		public new ExportOptionsContainer ExportOptions { get { return (ExportOptionsContainer)base.ExportOptions; } }
#endif
		#endregion
		public PrintingSystem()
			: base(null, new ExportOptionsContainer()) {
			AddService(typeof(BackgroundPageBuildEngineStrategy), new DispatcherPageBuildStrategy());
		}
		public override CommandVisibility GetCommandVisibility(PrintingSystemCommand command) {
			return CommandVisibility.All;
		}
		protected override PrintingDocument CreateDocument() {
			return DocumentFactory.Create(this);
		}
#if !SL
		#region Export to XPS
		public void ExportToXps(string filePath) {
			ExportToXps(filePath, ExportOptions.Xps);
		}
		#endregion
#endif
	}
}
