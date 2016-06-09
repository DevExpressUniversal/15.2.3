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

using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfSplitter : Splitter {
		PdfViewer viewer;
		NavigationPane navigationPane;
		INavigationPane navigationPaneInterface;
		bool splitterFirstMove = true;
		Size navigationPaneSavedSize = Size.Empty;
		public PdfSplitter(PdfViewer viewer, NavigationPane navigationPane) {
			this.viewer = viewer;
			this.navigationPane = navigationPane;
			Visible = false;
			MinExtra = 0;
			Dock = DockStyle.Left;
			TabStop = false;
			if (navigationPane != null) {
				navigationPaneInterface = navigationPane as INavigationPane;
				if (navigationPaneInterface != null)
					navigationPaneInterface.ExternalSplitterWidth = Size.Width;
			}
		}
		protected override void OnSplitterMoved(SplitterEventArgs e) {
			base.OnSplitterMoved(e);
			splitterFirstMove = true;
			if (navigationPaneInterface != null) {
				Size minSize = navigationPaneInterface.RegularMinSize;
				if (e.X <= minSize.Width) {
					navigationPane.State = NavigationPaneState.Collapsed;
					navigationPane.RegularSize = navigationPaneSavedSize;
				}
				else if (e.X >= viewer.ClientSize.Width - NavigationPane.StickyWidth) {
					if (navigationPane.State == NavigationPaneState.Expanded)
						navigationPane.Width = viewer.ClientRectangle.Width - navigationPaneInterface.ExternalSplitterWidth;
					else {
						navigationPane.State = NavigationPaneState.Expanded;
						navigationPane.RegularSize = navigationPaneSavedSize;
					}
				}
				else {
					navigationPane.RegularSize = new Size(e.X, navigationPane.RegularSize.Height);
					navigationPane.State = NavigationPaneState.Default;
					navigationPane.LayoutChanged();
				}
			}
		}
		protected override void OnSplitterMoving(SplitterEventArgs e) {
			base.OnSplitterMoving(e);
			if (navigationPaneInterface != null) {
				NavigationPaneViewInfo viewInfo = navigationPaneInterface.ViewInfo;
				MinSize = viewInfo.ButtonsBounds.Width;
				if (splitterFirstMove) {
					if (navigationPane.State == NavigationPaneState.Expanded)
						navigationPaneSavedSize = navigationPane.RegularSize;
					else
						navigationPaneSavedSize = navigationPane.Size;
					splitterFirstMove = false;
				}
			}
		}
	}
}
