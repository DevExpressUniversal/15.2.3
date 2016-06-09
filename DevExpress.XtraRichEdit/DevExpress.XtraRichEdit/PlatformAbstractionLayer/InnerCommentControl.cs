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
using DevExpress.Office.History;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit {
	public partial class InnerCommentControl: RichEditControl {
		RichEditControl richEditControl;
		RichEditCommandFactoryService oldRichEditControlService;
		public InnerCommentControl()
			: base() {
				this.ActiveViewType = RichEditViewType.Simple;
				DocumentModel.RangePermissionOptions.Visibility = RichEditRangePermissionVisibility.Hidden;
				this.DocumentModel.ReplaceCommandCreationStrategy();
				UseDeferredDataBindingNotifications = false;
		}
		[DefaultValue(null)]
		public RichEditControl RichEditControl {
			get { return richEditControl; }
			set {
				if (richEditControl == value)
					return;
				if (richEditControl != null) {
					richEditControl.ReplaceService<IRichEditCommandFactoryService>(oldRichEditControlService);
					UnsubscribeRichEditCommentControlEvents(richEditControl);
				}
				richEditControl = value;
				if (richEditControl == null) {
					this.CreateNewDocument();
					return;
				}
				GenerateMainDocumentComments(false);
				SubscribeRichEditCommentControlEvents(richEditControl);
				ReplaceRichEditCommentControlService();
				ReplaceRichEditControlService();
				ObtainDockPanelVisible();				
			}
		}
		public RichEditView MainActiveView { get { return richEditControl.ActiveView; } }
		public DocumentModel MainDocumentModel { get { return richEditControl.ActiveView.DocumentModel; } }
		ReviewingPaneFormController CreateReviewingPaneFormController() {
			return new ReviewingPaneFormController(RichEditControl.DocumentModel);
		}
		int CalculateWidthCommentArea() {
			return this.ViewBounds.Width;
		}
		void ReplaceRichEditCommentControlService() {
			RichEditCommandFactoryService oldService = this.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			this.ReplaceService<IRichEditCommandFactoryService>(new RichEditCommentCommandFactoryService(this, richEditControl, oldService));
		}
		void ReplaceRichEditControlService() {
			RichEditCommandFactoryService oldService = richEditControl.GetService<IRichEditCommandFactoryService>() as RichEditCommandFactoryService;
			oldRichEditControlService = oldService;
			richEditControl.ReplaceService<IRichEditCommandFactoryService>(new RichEditWithCommentsCommandFactoryService(this, richEditControl, oldService));
		}
		void ObtainDockPanelVisible() {
			ControlContainer container = this.Parent as ControlContainer;
			if (container != null) {
				DockPanel dockPanel = container.Panel;
				if ((dockPanel != null) && (dockPanel.Visibility == DockVisibility.Visible)) {
					DockPanelVisible = true;
					dockPanel.ClosedPanel += dockPanel_ClosedPanel;
				}
				else {
					DockPanelVisible = false;
					dockPanel.ClosedPanel -= dockPanel_ClosedPanel;
				}
			}
			else {
				DockPanelVisible = false;
			}
		}
		protected virtual void RichEditControl_ShowReviewingPane(object sender, ShowReviewingPaneEventArg e) {
			if (!Focused) {
				RichEditCommentControl commentControl = this.Parent as RichEditCommentControl;
				ControlContainer container = commentControl.Parent as ControlContainer;
				DockPanel dockPanel = container.Panel;
				dockPanel.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_MainDocumentComments); 
				if (commentControl != null && container != null && dockPanel != null) {
					if (e.SetFocus)
						SetFocus();
					if (DockPanelVisible) {
						SetCursor(e.CommentViewInfo, e.SelectParagraph, e.Start, e.End);
						if (e.SynchronizeSelection) {
							SelectionChangeCore();
						}
					}
					else {
						dockPanel.Show();
						dockPanel.ClosedPanel += dockPanel_ClosedPanel;
						DockPanelVisible = true;
						SetCursor(e.CommentViewInfo, e.SelectParagraph, e.Start, e.End);
					}
				}
			}
		}
		void RichEditControl_CloseReviewingPane(object sender, EventArgs e) {
			RichEditCommentControl commentControl = this.Parent as RichEditCommentControl;
			ControlContainer container = commentControl.Parent as ControlContainer;
			DockPanel dockPanel = container.Panel;
			if (commentControl != null && container != null && dockPanel != null) {
				if (DockPanelVisible) {
					  dockPanel.Close();
					  dockPanel.ClosedPanel -= dockPanel_ClosedPanel;
					  DockPanelVisible = false;
				}
			}		   
		}
		void dockPanel_ClosedPanel(object sender, DockPanelEventArgs e) {
			DockPanelVisible = false;
			ActivateMainPieceTable();
			richEditControl.SetFocus();
		}
		protected internal override void DisposeCommon() {
			if (richEditControl != null && richEditControl.DocumentModel != null)
				UnsubscribeRichEditCommentControlEvents(richEditControl);
			base.DisposeCommon();
		}
	}
}
