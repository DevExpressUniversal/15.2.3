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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PdfViewer.Extensions;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.PdfViewer.Themes;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public class DefaultPdfBarManagerItemNames : DefaultBarManagerItemNames {
		public const string OpenSplit = "bOpenSplit";
		public const string OpenFromWeb = "bOpenFromWeb";
		public const string Find = "bFind";
		public const string FindContext = "bFindContext";
		public const string Print = "bPrint";
		public const string PrintContext = "bPrintContext";
		public const string HandTool = "bHandTool";
		public const string SelectTool = "bSelectTool";
		public const string MarqueeZoom = "bMarqueeZoom";
		public const string Properties = "bProperties";
		public const string SelectAll = "bSelectAll";
		public const string SaveAs = "bSaveAs";
		public const string Copy = "bCopy";
		public const string Import = "bImport";
		public const string Export = "bExport";
		public const string PageLayout = "bPageLayout";
		public const string FormDataRibbonGroup = "rgFormData";
		public const string FormDataRibbonPage = "rpFormData";
		public const string AttachmentsViewerOpen = "bAttachmentsViewerOpen";
		public const string AttachmentsViewerSave = "bAttachmentsViewerSave";
	}
	public class HandToolBarItem : PdfBarCheckItem {
	}
	public class SelectToolBarItem : PdfBarCheckItem {
	}
	public class MarqueeZoomBarItem : PdfBarCheckItem {
	}
	public class FindTextBarItem : DocumentViewerBarButtonItem {
		static FindTextBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(FindTextBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
		}		
	}
	public class OpenDocumentSplitBarItem : PdfMRUSplitItem {
	}
	public class OpenDocumentFromWebBarItem : PdfBarSplitItem {
	}
	public class PrintDocumentBarItem : DocumentViewerBarButtonItem {
		static PrintDocumentBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(PrintDocumentBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.P, ModifierKeys.Control)));
		}
	}
	public class PropertiesBarItem : DocumentViewerBarButtonItem {
	}
	public class SelectAllBarItem : DocumentViewerBarButtonItem {
		static SelectAllBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(SelectAllBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.A, ModifierKeys.Control)));
		}
	}
	public class SaveAsBarItem : DocumentViewerBarButtonItem {
		static SaveAsBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(SaveAsBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.S, ModifierKeys.Control)));
		}
	}
	public class CopyBarItem : DocumentViewerBarButtonItem {
		static CopyBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(CopyBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.C, ModifierKeys.Control)));
		}
	}
	public class ImportBarItem : DocumentViewerBarButtonItem { 
	}
	public class ExportBarItem : DocumentViewerBarButtonItem {
	}
	public class PageLayoutBarItem : DocumentViewerBarSubItem {
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as CommandBase;
			if (command == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = command;
			Style = (Style)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.BarSubItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
}
