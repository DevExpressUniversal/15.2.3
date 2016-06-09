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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraPdfViewer.Forms;
using DevExpress.XtraPdfViewer.Native;
namespace DevExpress.XtraPdfViewer.Controls {
	[DXToolboxItem(false)]
	public partial class PdfAttachmentsViewerControl : XtraUserControl {
		readonly PdfViewer viewer;
		PdfAttachmentsViewerNode hintItem;
		bool cancelSearchOperation;
		long previousProgressChangedTimeTick = -1;
		public PdfAttachmentsViewerControl() {
			InitializeComponent();
		}
		public PdfAttachmentsViewerControl(PdfViewer viewer) : this() {
			this.viewer = viewer;
			UserLookAndFeel lookAndFell = viewer.LookAndFeel;
			LookAndFeel.ParentLookAndFeel = lookAndFell;
			attachmentsViewerBarAndDockingController.LookAndFeel.ParentLookAndFeel = lookAndFell;
			attachmentsListBoxControl.LookAndFeel.ParentLookAndFeel = lookAndFell;
		}
		public void CancelSearchAttachment() {
			cancelSearchOperation = true;
		}
		public void EnsureAttachments() {
			ImageList imageList = attachmentsListBoxControl.ImageList as ImageList;
			PdfDocument document = viewer.Document;
			if (imageList == null && document != null) {
				PdfFileAttachmentList attacments = document.FileAttachments as PdfFileAttachmentList;
				if (document != null) {
					attacments.SearchAttachmentProgressChanged += new PdfProgressChangedEventHandler(OnSearchAttachmentProgressChanged);
					try {
						cancelSearchOperation = false;
						imageList = new ImageList();
						ImageList.ImageCollection images = imageList.Images;
						SplashScreenManager.ShowForm(viewer.ParentForm, typeof(PdfSearchProgressForm), true, true, true, SplashFormStartPosition.Default, Point.Empty, 100, ParentFormState.Locked);
						SplashScreenManager.Default.SendCommand(SearchProgressFormCommand.SetPagesCmd, viewer.PageCount);
						PdfDocumentState documentState = viewer.DocumentState;
						IList<PdfAttachmentsViewerNode> nodes = documentState.AttachmentsViewerNodes;
						if (nodes.Count > 0) {
							foreach (byte[] data in documentState.AttachmentsViewerNodesIcons)
								images.Add(new Bitmap(new MemoryStream(data)));
							attachmentsViewerSaveBarButtonItem.Enabled = true;
							attachmentsViewerPreviewBarButtonItem.Enabled = true;
						}
						attachmentsListBoxControl.DataSource = nodes;
						attachmentsListBoxControl.ImageList = imageList;
					}
					catch { 
					}
					finally {
						SplashScreenManager.CloseForm();
						attacments.SearchAttachmentProgressChanged -= new PdfProgressChangedEventHandler(OnSearchAttachmentProgressChanged);
					}
				}
			}
		}
		public void InvalidateAttachments() {
			attachmentsViewerSaveBarButtonItem.Enabled = false;
			attachmentsViewerPreviewBarButtonItem.Enabled = false;
			ImageList imageList = attachmentsListBoxControl.ImageList as ImageList;
			if (imageList != null)
				imageList.Dispose();
			attachmentsListBoxControl.ImageList = null;
		}
		void OnSearchAttachmentProgressChanged(object sender, PdfProgressChangedEventArgs e) {
			long ticks = DateTime.Now.Ticks;
			if ((ticks - previousProgressChangedTimeTick) > TimeSpan.TicksPerMillisecond * 200) {
				previousProgressChangedTimeTick = ticks;
				SplashScreenManager splashScreenManager = SplashScreenManager.Default;
				splashScreenManager.SendCommand(SearchProgressFormCommand.CmdId, e.ProgressValue);
				splashScreenManager.SendCommand(SearchProgressFormCommand.CancelSearchAttachmentId, this);
			}
			if (cancelSearchOperation)
				throw new PdfCancelSearchAttachmentsOperationException();
		}
		void AttachmentsViewerPreviewBarButtonItemItemClick(object sender, ItemClickEventArgs e) {
			OpenAttachment();
		}
		void OpenAttachment() {
			PdfAttachmentsViewerNode node = attachmentsListBoxControl.SelectedItem as PdfAttachmentsViewerNode;
			if (node != null)
				viewer.DocumentStateController.OpenFileAttachment(node.FileAttachment);
		}
		void AttachmentsViewerSaveBarButtonItemItemClick(object sender, ItemClickEventArgs e) {
			PdfAttachmentsViewerNode node = attachmentsListBoxControl.SelectedItem as PdfAttachmentsViewerNode;
			if (node != null)
			   viewer.DocumentStateController.SaveFileAttachment(node.FileAttachment);
		}
		void AttachmentsListBoxControlMouseMove(object sender, MouseEventArgs e) {
			ToolTipController toolTipController = ToolTipController.DefaultController;
			Point point = new Point(e.X, e.Y);
			int index = attachmentsListBoxControl.IndexFromPoint(point);
			if (index != -1) {
				PdfAttachmentsViewerNode item = attachmentsListBoxControl.GetItem(index) as PdfAttachmentsViewerNode;
				if (item != null) {
					if (item != hintItem) {
						toolTipController.ShowHint(item.Hint, attachmentsListBoxControl.PointToScreen(point));
						hintItem = item;
					}
					return;
				}
			}
			toolTipController.HideHint();
			hintItem = null;
		}
		void AttachmentsListBoxControlMouseLeave(object sender, EventArgs e) {
			ToolTipController.DefaultController.HideHint();
			hintItem = null;
		}
		void AttachmentsListBoxControlMouseDoubleClick(object sender, MouseEventArgs e) {
			if (attachmentsListBoxControl.IndexFromPoint(e.Location) != -1)
				OpenAttachment();
		}
		void AttachmentsListBoxControlKeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Enter:
				case Keys.Space:
					Keys modifiers = e.Modifiers;
					if (modifiers == Keys.Shift || modifiers == Keys.None)
						OpenAttachment();
					break;
			}
		}
	}
}
