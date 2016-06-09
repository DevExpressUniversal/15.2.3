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

using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Bars;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfBarsGenerator : RunTimeBarsGenerator<PdfViewer, PdfViewerCommandId> {
		readonly PdfViewer pdfViewer;
		readonly BarManager barManager;
		public PdfBarsGenerator(PdfViewer pdfViewer, BarManager barManager) : base(pdfViewer) {
			this.pdfViewer = pdfViewer;
			this.barManager = barManager;
		}
		public override Component GetBarContainer() {
			List<Component> supportedBarContainerCollection = new List<Component>();
			Control control = pdfViewer.Parent;
			while (control != null) {
				foreach (Control ctrl in control.Controls)
					if (ctrl is RibbonControl)
						supportedBarContainerCollection.Add(ctrl);
				control = control.Parent;
			}
			if (barManager != null)
				supportedBarContainerCollection.Add(barManager);
			return ChooseBarContainer(supportedBarContainerCollection);
		}
		protected override BarGenerationManagerFactory<PdfViewer, PdfViewerCommandId> CreateBarGenerationManagerFactory() {
			return new PdfBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<PdfViewer, PdfViewerCommandId> CreateBarController() {
			return new PdfBarController();
		}
	}
}
