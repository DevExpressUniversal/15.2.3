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

using DevExpress.XtraPdfViewer.Commands;
namespace DevExpress.XtraPdfViewer.Bars {
	public class PdfZoom10CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom10; } }
	}
	public class PdfZoom25CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom25; } }
	}
	public class PdfZoom50CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom50; } }
	}
	public class PdfZoom75CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom75; } }
	}
	public class PdfZoom100CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom100; } }
	}
	public class PdfZoom125CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom125; } }
	}
	public class PdfZoom150CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom150; } }
	}
	public class PdfZoom200CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom200; } }
	}
	public class PdfZoom400CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom400; } }
	}
	public class PdfZoom500CheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.Zoom500; } }
	}
	public class PdfSetActualSizeZoomModeCheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SetActualSizeZoomMode; } }
	}
	public class PdfSetPageLevelZoomModeCheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SetPageLevelZoomMode; } }
	}
	public class PdfSetFitWidthZoomModeCheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SetFitWidthZoomMode; } }
	}
	public class PdfSetFitVisibleZoomModeCheckItem : PdfBarCheckItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SetFitVisibleZoomMode; } }
	}
}
