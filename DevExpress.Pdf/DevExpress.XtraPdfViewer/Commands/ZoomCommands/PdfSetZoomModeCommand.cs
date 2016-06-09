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

using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public abstract class PdfSetZoomModeCommand : PdfCheckableCommand {		
		protected override bool IsChecked { 
			get { 
				PdfViewer pdfViewer = Control;
				if (pdfViewer == null)
					return false;
				PdfZoomMode zoomMode = ZoomMode;
				PdfDocumentViewer pdfDocumentViewer = pdfViewer.Viewer;
				return pdfDocumentViewer != null && pdfViewer.ZoomMode == zoomMode && (zoomMode != PdfZoomMode.Custom || pdfDocumentViewer.ZoomFactor == pdfDocumentViewer.Zoom);
			} 
		}
		protected virtual PdfZoomMode ZoomMode { get { return PdfZoomMode.Custom; } }
		protected PdfSetZoomModeCommand(PdfViewer control) : base(control) { 
		}
		protected override void ExecuteInternal() {
			Control.ZoomMode = ZoomMode;
		}
	}
	public class PdfSetActualSizeZoomModeCommand : PdfSetZoomModeCommand {		
		protected override PdfZoomMode ZoomMode { get { return PdfZoomMode.ActualSize; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandSetActualSizeZoomModeCaption; } }
		public PdfSetActualSizeZoomModeCommand(PdfViewer control) : base(control) { 
		}
	}
	public class PdfSetPageLevelZoomModeCommand : PdfSetZoomModeCommand {		
		protected override PdfZoomMode ZoomMode { get { return PdfZoomMode.PageLevel; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandSetPageLevelZoomModeCaption; } }
		public PdfSetPageLevelZoomModeCommand(PdfViewer control) : base(control) { 
		}
	}
	public class PdfSetFitVisibleZoomModeCommand : PdfSetZoomModeCommand {		
		protected override PdfZoomMode ZoomMode { get { return PdfZoomMode.FitToVisible; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandSetFitVisibleZoomModeCaption; } }
		public PdfSetFitVisibleZoomModeCommand(PdfViewer control) : base(control) { 
		}
	}
	public class PdfSetFitWidthZoomModeCommand : PdfSetZoomModeCommand {		
		protected override PdfZoomMode ZoomMode { get { return PdfZoomMode.FitToWidth; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandSetFitWidthZoomModeCaption; } }
		public PdfSetFitWidthZoomModeCommand(PdfViewer control) : base(control) { 
		}
	}
}
