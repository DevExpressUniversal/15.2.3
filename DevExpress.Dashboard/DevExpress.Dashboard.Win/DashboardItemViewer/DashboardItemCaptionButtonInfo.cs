#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using System;
namespace DevExpress.DashboardWin.Native {
	public class DashboardItemCaptionButtonInfo : IDisposable {
		readonly DashboardViewerCommandBarItemsContainer barItemsContainer = new DashboardViewerCommandBarItemsContainer();
		DashboardPopupMenu popupMenu;
		DashboardPopupMenu PopupMenu {
			get {
				if(popupMenu == null)
					popupMenu = new DashboardPopupMenu();
				return popupMenu;
			}
		}
		public DashboardButtonType ButtonType { get; private set; }
		public Image ButtonImage { get; private set; }
		public ObjectState ButtonState { get; private set; }
		public string Tooltip { get; private set; }
		public bool SuppressPopupShowing { get; set; }
		public DashboardItemCaptionButtonInfo(DashboardButtonType buttonType, Image buttonImage, ObjectState buttonState, string tooltip) {
			ButtonType = buttonType;
			ButtonImage = buttonImage;
			ButtonState = buttonState;
			Tooltip = tooltip;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				barItemsContainer.Clear();
				if(popupMenu != null) {
					popupMenu.ClearLinks();
					popupMenu.Dispose();
					popupMenu = null;
				}
			}
		}
		public void AddBarItems(IList<BarItem> items) {
			barItemsContainer.AddBarItems(items);
		}
		public List<BarItem> GetBarItems() {
			return barItemsContainer.Items;
		}
		public void Execute(DashboardViewer viewer, DashboardItemViewer itemViewer, DashboardArea area) {
			barItemsContainer.Viewer = viewer;
			barItemsContainer.ItemViewer = itemViewer;
			if(barItemsContainer.Items.Count > 1) {
				DashboardPopupMenu menu = PopupMenu;
				menu.ClearLinks();
				menu.Manager = viewer.ViewerBarManager;
				foreach(BarItem item in barItemsContainer.Items)
					menu.AddItem(item);
				DashboardPopupMenuShowingEventArgs args = new DashboardPopupMenuShowingEventArgs(itemViewer, Cursor.Position) {
					Menu = menu,
					ButtonType = ButtonType,
					DashboardArea = area,
					DashboardItemArea = itemViewer != null ? DashboardItemArea.DashboardItem : DashboardItemArea.None
				};
				viewer.RaisePopupMenuShowing(args);
				PopupMenu argsMenu = args.Menu;
				if(args.Allow && argsMenu != null && argsMenu.ItemLinks.Count > 0 && !SuppressPopupShowing)
					argsMenu.ShowPopup(Cursor.Position);
			}
			else if(barItemsContainer.Items.Count == 1)
				barItemsContainer.ExecuteBarItemCommand((IDashboardViewerCommandBarItem)barItemsContainer.Items[0]);
		}
	}
}
