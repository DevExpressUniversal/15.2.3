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
using System.Windows.Forms;
using DevExpress.Pdf;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraPdfViewer.Forms;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.FindControl {
	[DXToolboxItem(false)]
	public partial class PdfFindControl : FlyoutPanel {
		readonly BarManager barManager = new BarManager();
		readonly PopupMenu popupMenu;
		readonly BarCheckItem caseSensitive;
		readonly BarCheckItem wholeWords;
		PdfViewer viewer;
		bool cancelFindOperation;
		public TextEdit TextEdit { get { return textEdit; } }
		public PdfFindDialogOptions FindDialogOptions {
			get { return new PdfFindDialogOptions(textEdit.Text, caseSensitive.Checked, wholeWords.Checked); }
			set {
				textEdit.Text = value.Text;
				caseSensitive.Checked = value.CaseSensitive;
				wholeWords.Checked = value.WholeWords;
			}
		}
		public event EventHandler Close;
		public PdfFindControl() {
			InitializeComponent();
			UpdateButtonsState();
			popupMenu = new PopupMenu(barManager);
			popupMenu.BeginUpdate();
			try {
				caseSensitive = new BarCheckItem(barManager);
				caseSensitive.Caption = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FindControlCaseSensitive);
				popupMenu.AddItem(caseSensitive);
				wholeWords = new BarCheckItem(barManager);
				wholeWords.Caption = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FindControlWholeWordsOnly);
				popupMenu.AddItem(wholeWords);
			}
			finally {
				popupMenu.EndUpdate();
			}
			ddBtnParameters.DropDownControl = popupMenu;
			ddBtnParameters.LostFocus += (sender, e) => ddBtnParameters.HideDropDown();
			textEdit.TextChanged += (sender, e) => UpdateButtonsState();
			textEdit.KeyDown += (sender, e) => {
				if (e.KeyCode == Keys.Enter && btnFindNext.Enabled)
					FindText(PdfTextSearchDirection.Forward);
			};
		}
		public void SetViewer(PdfViewer viewer) {
			this.viewer = viewer;
			if (viewer != null && viewer.MenuManager != null) {
				IDXMenuManager menuManager = viewer.MenuManager;
				textEdit.MenuManager = menuManager;
				RibbonControl ribbon = menuManager as RibbonControl;
				if (ribbon == null)
					popupMenu.Manager = menuManager as BarManager;
				else
					popupMenu.Manager = ribbon.Manager;
			}
		}
		public void CancelFindOperation() {
			cancelFindOperation = true;
		}
		void FindText(PdfTextSearchDirection direction) {
			PdfTextSearchParameters parameters = new PdfTextSearchParameters();
			parameters.CaseSensitive = caseSensitive.Checked;
			parameters.WholeWords = wholeWords.Checked;
			parameters.Direction = direction;
			PdfTextSearchStatus status;
			try {
				cancelFindOperation = false;
				try {
					SplashScreenManager.ShowForm(viewer.ParentForm, typeof(PdfSearchProgressForm), true, true, true, SplashFormStartPosition.Default, Point.Empty, 1000, ParentFormState.Locked);
					Enabled = false;
					SplashScreenManager splashScreenManager = SplashScreenManager.Default;
					splashScreenManager.SendCommand(SearchProgressFormCommand.SetPagesCmd, viewer.PageCount);
					status = viewer.FindText(textEdit.Text, parameters, progress => {
						splashScreenManager.SendCommand(SearchProgressFormCommand.CmdId, progress);
						splashScreenManager.SendCommand(SearchProgressFormCommand.CancelSearchId, this);
						return cancelFindOperation;
					}).Status;
				}
				finally {
					SplashScreenManager.CloseForm();
				}
				if (status != PdfTextSearchStatus.Found && !cancelFindOperation) {
					string message = status == PdfTextSearchStatus.Finished ? XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSearchFinished) :
																			  XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSearchFinishedNoMatchesFound);
					XtraMessageBox.Show(LookAndFeel, ParentForm, message, XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSearchCaption), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch { }
			finally {
				Enabled = true;
				textEdit.Focus();
			}
		}
		void UpdateButtonsState() {
			bool enabled = !String.IsNullOrEmpty(textEdit.Text);
			btnFindNext.Enabled = enabled;
			btnFindPrev.Enabled = enabled;
		}
		void OnBtnFindNextClick(object sender, EventArgs e) {
			FindText(PdfTextSearchDirection.Forward);
		}
		void OnBtnFindPrevClick(object sender, EventArgs e) {
			FindText(PdfTextSearchDirection.Backward);
		}
		void OnBtnCloseClick(object sender, EventArgs e) {
			Close(this, e);
		}
	}
}
