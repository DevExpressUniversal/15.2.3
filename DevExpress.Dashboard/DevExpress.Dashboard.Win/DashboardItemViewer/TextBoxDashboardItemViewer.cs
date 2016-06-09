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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
namespace DevExpress.DashboardWin.Native {
	[
	DXToolboxItem(false),
	DashboardItemDesigner(typeof(TextBoxDashboardItemDesigner))
	]
	public class TextBoxDashboardItemViewer : DashboardItemViewer {
		readonly RichEditControl richEditControl = new RichEditControl();
		public RichEditControl RichEditControl { get { return richEditControl; } }
		public TextBoxDashboardItemViewer() : base() { 
			SubscribeControlEvents();
		}
		protected override Control GetViewControl() {
			return richEditControl;
		}
		protected override Control GetUnderlyingControl() {
			return richEditControl;
		}
		protected override void PrepareViewControl() {
			RichEditControlOptions options = richEditControl.Options;
			options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			options.Behavior.ShowPopupMenu = DocumentCapability.Disabled;
			richEditControl.ActiveViewType = RichEditViewType.Simple;
			richEditControl.ReadOnly = true;
			richEditControl.ShowCaretInReadOnly = false;
			richEditControl.RemoveShortcutKey(Keys.O, Keys.Control);
			richEditControl.RemoveShortcutKey(Keys.P, Keys.Control);
			richEditControl.RemoveShortcutKey(Keys.F, Keys.Control);
		}
		internal void UpdateViewer(TextBoxDashboardItemViewModel viewModel) {
			string base64Rtf = viewModel.Base64Rtf;
			byte[] result = Convert.FromBase64String(base64Rtf);
			ASCIIEncoding enc = new ASCIIEncoding();
			richEditControl.RtfText = enc.GetString(result);
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();
			UpdateViewer((TextBoxDashboardItemViewModel)ViewModel);
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(richEditControl);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				UnsubscribeControlEvents();
				richEditControl.Dispose();
			}
			base.Dispose(disposing);
		}
		void SubscribeControlEvents() {
			richEditControl.MouseClick += OnControlMouseClick;
			richEditControl.MouseDoubleClick += OnControlMouseDoubleClick;
			richEditControl.MouseMove += OnControlMouseMove;
			richEditControl.MouseEnter += OnControlMouseEnter;
			richEditControl.MouseLeave += OnControlMouseLeave;
			richEditControl.MouseDown += OnControlMouseDown;
			richEditControl.MouseUp += OnControlMouseUp;
			richEditControl.MouseHover += OnControlMouseHover;
			richEditControl.MouseWheel += OnControlMouseWheel;
			richEditControl.SizeChanged += OnControlSizeChanged;
		}
		void UnsubscribeControlEvents() {
			richEditControl.MouseClick -= OnControlMouseClick;
			richEditControl.MouseDoubleClick -= OnControlMouseDoubleClick;
			richEditControl.MouseMove -= OnControlMouseMove;
			richEditControl.MouseEnter -= OnControlMouseEnter;
			richEditControl.MouseLeave -= OnControlMouseLeave;
			richEditControl.MouseDown -= OnControlMouseDown;
			richEditControl.MouseUp -= OnControlMouseUp;
			richEditControl.MouseHover -= OnControlMouseHover;
			richEditControl.MouseWheel -= OnControlMouseWheel;
			richEditControl.SizeChanged -= OnControlSizeChanged;
		}
		void OnControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		void OnControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		void OnControlMouseLeave(object sender, EventArgs e) {
			RaiseMouseLeave();
		}
		void OnControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnControlMouseMove(object sender, MouseEventArgs e) {
			RaiseMouseMove(e.Location);
		}
		void OnControlMouseDoubleClick(object sender, MouseEventArgs e) {
			RaiseDoubleClick(e.Location);
		}
		void OnControlMouseClick(object sender, MouseEventArgs e) {
			RaiseClick(e.Location);
		}
		void OnControlSizeChanged(object sender, EventArgs e) {
			DocumentLayout documentLayout = richEditControl.DocumentLayout;
			int contentHeight = 0;
			for(int i = 0; i < documentLayout.GetFormattedPageCount(); i++)
				contentHeight += CalculatePageHeight(documentLayout.GetPage(i));
			if(contentHeight < richEditControl.Bounds.Height && richEditControl.VerticalScrollValue <= 0)
				richEditControl.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			else
				richEditControl.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Visible;
		}
		int CalculatePageHeight(LayoutPage page) {
			LayoutRowCollection rows = page.PageAreas.First.Columns.First.Rows;
			int height = 0;
			foreach(var row in rows)
				height += row.Bounds.Height;
			return height;
		}
	}
}
