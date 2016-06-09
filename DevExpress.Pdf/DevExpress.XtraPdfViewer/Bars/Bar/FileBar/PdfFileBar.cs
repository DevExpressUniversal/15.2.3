﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Bars {
	public class PdfFileRibbonPageGroup : PdfRibbonPageGroup {
		public override string DefaultText { get { return XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FileRibbonGroupCaption); } }
	}
	public class PdfFileOpenBarItem : PdfBarButtonItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.OpenFile; } }
		public PdfFileOpenBarItem() {
			ItemShortcut = new BarShortcut(Keys.Control | Keys.O);
		}			
	}
	public class PdfFileSaveAsBarItem : PdfBarButtonItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.SaveAsFile; } }
		public PdfFileSaveAsBarItem() {
			ItemShortcut = new BarShortcut(Keys.Control | Keys.S);
		}
	}
	public class PdfFilePrintBarItem : PdfBarButtonItem {
		protected override PdfViewerCommandId CommandId { get { return PdfViewerCommandId.PrintFile; } }
		public PdfFilePrintBarItem() {
			ItemShortcut = new BarShortcut(Keys.Control | Keys.P);
		}			
	}
}
