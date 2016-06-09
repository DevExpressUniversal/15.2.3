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
using DevExpress.XtraBars;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfOutlineViewerSubItemLink : BarSubItemLink {
		readonly PdfViewer viewer;
		readonly BarManager barManager;
		public PdfOutlineViewerSubItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject, PdfViewer viewer, BarManager barManager) : base(ALinks, AItem, ALinkedObject) { 
			this.viewer = viewer;
			this.barManager = barManager;
		}
		protected override void OpenCore(bool setOpened) {
			if (setOpened) { 
				PdfOutlineViewerSubItem outlineViewerMenu = Item as PdfOutlineViewerSubItem;
				if (outlineViewerMenu != null) { 
					outlineViewerMenu.BeginUpdate();
					try {
						outlineViewerMenu.ClearLinks();
						outlineViewerMenu.AddItem(new PdfOutlinesExpandCurrentCommand(viewer).CreateContextMenuBarItem(barManager));
						outlineViewerMenu.AddItem(new PdfOutlinesExpandCollapseTopLevelCommand(viewer).CreateContextMenuBarItem(barManager)).BeginGroup = true;
						outlineViewerMenu.AddItem(new PdfOutlinesHideAfterUseCommand(viewer).CreateContextMenuBarItem(barManager)).BeginGroup = true;
						BarSubItem textSizeSubMenu = new BarSubItem();
						textSizeSubMenu.Caption = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.CommandOutlinesTextSizeCaption);
						textSizeSubMenu.AddItem(new PdfOutlinesTextSizeToSmallCommand(viewer).CreateContextMenuBarItem(barManager));
						textSizeSubMenu.AddItem(new PdfOutlinesTextSizeToMediumCommand(viewer).CreateContextMenuBarItem(barManager));
						textSizeSubMenu.AddItem(new PdfOutlinesTextSizeToLargeCommand(viewer).CreateContextMenuBarItem(barManager));
						outlineViewerMenu.AddItem(textSizeSubMenu).BeginGroup = true;
						outlineViewerMenu.AddItem(new PdfOutlinesGotoCommand(viewer).CreateContextMenuBarItem(barManager)).BeginGroup = true;
						outlineViewerMenu.AddItem(new PdfOutlinesPrintPagesCommand(viewer).CreateContextMenuBarItem(barManager));
						outlineViewerMenu.AddItem(new PdfOutlinesPrintSectionsCommand(viewer).CreateContextMenuBarItem(barManager));
						outlineViewerMenu.AddItem(new PdfOutlinesWrapLongLinesCommand(viewer).CreateContextMenuBarItem(barManager)).BeginGroup = true;
					}
					finally {
						outlineViewerMenu.EndUpdate();
					}
					try { 
						viewer.RaisePopupMenuShowing(new PdfPopupMenuShowingEventArgs(Item.ItemLinks, PdfPopupMenuKind.BookmarkOptions));
					}
					catch (NullReferenceException) { 
					}
				}
			}
			base.OpenCore(setOpened);
		}
	}
}
