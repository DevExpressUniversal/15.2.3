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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.DashboardCommon.Service;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class ImageDashboardItemViewer : DashboardItemViewer {		
		readonly DashboardPictureEdit pictureEdit = new DashboardPictureEdit();
		protected override Control GetViewControl() {
			return pictureEdit;
		}
		protected override Control GetUnderlyingControl() {
			return pictureEdit;
		}
		protected override void PrepareViewControl() {
			pictureEdit.BorderStyle = BorderStyles.NoBorder;
		}	  
		protected override void UpdateViewer() {
			base.UpdateViewer();
			UpdateViewer((ImageDashboardItemViewModel)ViewModel);
		}
		public ImageDashboardItemViewer() : base() {
			SubscribeControlEvents();
		}
		void UpdateViewer(ImageDashboardItemViewModel viewModel) {
			byte[] data = null;
			string base64SourceString = viewModel.ImageViewModel.SourceBase64String;
			if(!String.IsNullOrEmpty(base64SourceString))
				data = Convert.FromBase64String(base64SourceString);
			pictureEdit.UpdateImage(data, viewModel.ImageViewModel.IsInternalImage, viewModel.ImageViewModel.Url);
			pictureEdit.SizeMode = viewModel.SizeMode;
			pictureEdit.SetAlignment(viewModel.HorizontalAlignment, viewModel.VerticalAlignment);
		}
		public override void OnLookAndFeelChanged() {			
			pictureEdit.BackColor = BackColor;
			UpdateViewer();
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(pictureEdit);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				UnsubscribeControlEvents();
				pictureEdit.Dispose();
			}
			base.Dispose(disposing);
		}
		void SubscribeControlEvents() {
			pictureEdit.MouseClick += OnControlMouseClick;
			pictureEdit.MouseDoubleClick += OnControlMouseDoubleClick;
			pictureEdit.MouseMove += OnControlMouseMove;
			pictureEdit.MouseEnter += OnControlMouseEnter;
			pictureEdit.MouseLeave += OnControlMouseLeave;
			pictureEdit.MouseDown += OnControlMouseDown;
			pictureEdit.MouseUp += OnControlMouseUp;
			pictureEdit.MouseHover += OnControlMouseHover;
			pictureEdit.MouseWheel += OnControlMouseWheel;
		}
		void UnsubscribeControlEvents() {
			pictureEdit.MouseClick -= OnControlMouseClick;
			pictureEdit.MouseDoubleClick -= OnControlMouseDoubleClick;
			pictureEdit.MouseMove -= OnControlMouseMove;
			pictureEdit.MouseEnter -= OnControlMouseEnter;
			pictureEdit.MouseLeave -= OnControlMouseLeave;
			pictureEdit.MouseDown -= OnControlMouseDown;
			pictureEdit.MouseUp -= OnControlMouseUp;
			pictureEdit.MouseHover -= OnControlMouseHover;
			pictureEdit.MouseWheel -= OnControlMouseWheel;
		}
		private void OnControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		private void OnControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		private void OnControlMouseUp(object sender, MouseEventArgs e) {
			 RaiseMouseUp(e.Location); 
		}
		private void OnControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		private void OnControlMouseLeave(object sender, EventArgs e) {
			RaiseMouseLeave();
		}
		private void OnControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		private void OnControlMouseMove(object sender, MouseEventArgs e) {
			RaiseMouseMove(e.Location);
		}
		private void OnControlMouseDoubleClick(object sender, MouseEventArgs e) {
			RaiseDoubleClick(e.Location);
		}
		private void OnControlMouseClick(object sender, MouseEventArgs e) {
			 RaiseClick(e.Location);
		}
	}
}
