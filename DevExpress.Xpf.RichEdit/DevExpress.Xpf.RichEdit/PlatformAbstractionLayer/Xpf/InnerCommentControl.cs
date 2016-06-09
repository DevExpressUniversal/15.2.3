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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
#if !SL
using System.Windows.Forms;
#endif
using System.Windows.Media;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Docking.Base;
using System.Windows.Threading;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Services;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.RichEdit {
	public partial class InnerCommentControl : RichEditControl {
		RichEditCommandFactoryService oldRichEditControlService;
		public InnerCommentControl()
			: base() {
				this.Loaded += new RoutedEventHandler(RichEditCommentControl_Loaded);
		}
		public RichEditView MainActiveView { get { return RichEditControl.ActiveView; } }
		public DocumentModel MainDocumentModel { get { return RichEditControl.ActiveView.DocumentModel; } }
		void RichEditCommentControl_Loaded(object sender, RoutedEventArgs e) {
			ActiveViewType = RichEditViewType.Simple;
			DocumentModel.RangePermissionOptions.Visibility = RichEditRangePermissionVisibility.Hidden;
			this.DocumentModel.ReplaceCommandCreationStrategy();
			SetDockPanelVisible();
		}
		void SetDockPanelVisible() {
			DockPanelVisible = true;
			if (RichEditControl != null)
				RichEditControl.InnerControl.OnUpdateUI();
			if (this.DockLayoutManager == null)
				return;
			else
				this.DockLayoutManager.DockItemClosed += dockLayoutManager_DockItemClosed;
		}
#if SL
#else
#endif
		public RichEditControl RichEditControl { get; set; }
		public LayoutPanel LayoutPanel { get; set; }
		public DockLayoutManager DockLayoutManager { get; set; }
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			if (oldValue != null) {
				oldValue.ReplaceService<IRichEditCommandFactoryService>(oldRichEditControlService);
				UnsubscribeRichEditCommentControlEvents(oldValue);
			}
			if (newValue != null) {
				GenerateMainDocumentComments(false);
				SubscribeRichEditCommentControlEvents(newValue);
				ReplaceRichEditCommentControlService(newValue);
				ReplaceRichEditControlService(oldValue, newValue);
			}
			else {
				this.CreateNewDocument();
				return;
			}
			ObtainDockPanelVisible();
		}
		void ReplaceRichEditCommentControlService(RichEditControl newValue) {			
			RichEditCommandFactoryService oldService = this.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			if (oldService == null) {
				this.ApplyTemplate();
				oldService = this.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			}
			if (oldService!=null)
				this.ReplaceService<IRichEditCommandFactoryService>(new RichEditCommentCommandFactoryService(this, newValue, oldService));
		}
		void ReplaceRichEditControlService(RichEditControl oldValue, RichEditControl newValue) {
			RichEditCommandFactoryService oldService = newValue.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			if (oldService == null) {
				newValue.ApplyTemplate();
				oldService = newValue.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			}
			if (oldService != null) {
				oldRichEditControlService = oldService;
				newValue.ReplaceService<IRichEditCommandFactoryService>(new RichEditWithCommentsCommandFactoryService(this, newValue, oldService));
			}
		}
		ReviewingPaneFormController CreateReviewingPaneFormController() {
			return new ReviewingPaneFormController(RichEditControl.DocumentModel);
		}
		int CalculateWidthCommentArea() {
			if (this.ActiveView == null)
				return 0;
			return this.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(this.ActiveView.Bounds.Width, DpiX);
		}
		void ObtainDockPanelVisible() {
			DockLayoutManager manager = this.DockLayoutManager;
			LayoutPanel layoutPanel = this.LayoutPanel;
			if ((manager != null) && DockPanelVisible) {
				DockPanelVisible = true;
				manager.DockItemClosed += dockLayoutManager_DockItemClosed;
			}
			else {
				DockPanelVisible = false;
				if (manager != null)
					manager.DockItemClosed -= dockLayoutManager_DockItemClosed;
			}
		}
		void RichEditControl_ShowReviewingPane(object sender, ShowReviewingPaneEventArg e) {
			DockLayoutManager manager = this.DockLayoutManager;
			LayoutPanel layoutPanel = this.LayoutPanel;
			if (manager != null) {
				if (DockPanelVisible) {
					SetCursor(e.CommentViewInfo, e.SelectParagraph, e.Start, e.End);
				}
				else {
					manager.DockController.Restore(layoutPanel);
					SetDockPanelVisible();
					SetCursor(e.CommentViewInfo, e.SelectParagraph, e.Start, e.End);
				}
				RichEditControl.InnerControl.OnUpdateUI();
			}
		}
		void RichEditControl_CloseReviewingPane(object sender, EventArgs e) {
			DockLayoutManager manager = this.DockLayoutManager;
			LayoutPanel layoutPanel = this.LayoutPanel;
			if (manager != null) {
				if (DockPanelVisible) {
					manager.DockController.Close(layoutPanel);
					manager.DockItemClosed -= dockLayoutManager_DockItemClosed;
					DockPanelVisible = false;
				}
			}
		}
		void dockLayoutManager_DockItemClosed(object sender, DockItemClosedEventArgs e) {
			if (e.Item == this.LayoutPanel)
				DockPanelVisible = false;
			RichEditControl.InnerControl.OnUpdateUI();
		}
		protected internal override void OnResizeCore() {
			base.OnResizeCore();
			int widthCommentArea = CalculateWidthCommentArea();
			ResizeCore(widthCommentArea);
		}
	}
}
