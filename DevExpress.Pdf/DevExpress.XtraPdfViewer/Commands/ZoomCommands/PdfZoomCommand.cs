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
	public abstract class PdfZoomCommand : PdfCheckableCommand {		
		protected override bool IsChecked { 
			get { 
				PdfDocumentViewer viewer = Viewer;
				return viewer != null && viewer.Zoom == ZoomValue; 
			} 
		}
		protected virtual float ZoomValue { get { return 1f; } }
		protected PdfZoomCommand(PdfViewer control) : base(control) { 
		}
		protected override void ExecuteInternal() {
			PdfViewer pdfViewer = Control;
			if (pdfViewer != null) {
				float zoomValue = ZoomValue;
				pdfViewer.ZoomFactor = zoomValue * 100;
				pdfViewer.ZoomMode = zoomValue == 1f ? PdfZoomMode.ActualSize : PdfZoomMode.Custom;
			}
		}
	}
	public class PdfZoom500Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 5f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom500Caption; } }
		public PdfZoom500Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom400Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 4f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom400Caption; } }
		public PdfZoom400Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom200Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 2f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom200Caption; } }
		public PdfZoom200Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom150Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 1.5f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom150Caption; } }
		public PdfZoom150Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom125Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 1.25f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom125Caption; } }
		public PdfZoom125Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom100Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 1f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom100Caption; } }
		public PdfZoom100Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom75Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 0.75f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom75Caption; } }
		public PdfZoom75Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom50Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 0.5f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom50Caption; } }
		public PdfZoom50Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom25Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 0.25f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom25Caption; } }
		public PdfZoom25Command(PdfViewer control) : base(control) { 
		}
	}
	public class PdfZoom10Command : PdfZoomCommand {
		protected override float ZoomValue { get { return 0.1f; } }
		public override XtraPdfViewerStringId MenuCaptionStringId { get { return XtraPdfViewerStringId.CommandZoom10Caption; } }
		public PdfZoom10Command(PdfViewer control) : base(control) { 
		}
	}
}
