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
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
namespace DevExpress.XtraRichEdit.Native {
	[DXToolboxItem(false)]
	public class PreviewRichEditControl : RichEditControl {
		public PreviewRichEditControl() {
		}
		public TableStyleFormControllerBase Controller { get; set; }
		public ITableBorders CurrentBorders { get { return Controller.CurrentBorders; } }
		protected override void OnKeyPress(KeyPressEventArgs e) {
		}
		protected override void OnClick(EventArgs e) {
		}
		protected override void OnDoubleClick(EventArgs e) {
		}
		protected override void OnEnabledChanged(EventArgs e) {
		}
		protected override void OnDragDrop(DragEventArgs e) {
		}
		protected override void OnDragEnter(DragEventArgs e) {
		}
		protected override void OnDragLeave(EventArgs e) {
		}
		protected override void OnDragOver(DragEventArgs e) {
		}
		protected override void OnEnter(EventArgs e) {
		}
		protected override void OnMouseCaptureChanged(EventArgs e) {
		}
		protected override void OnMouseClick(MouseEventArgs e) {
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
		}
		protected override void OnMouseDown(MouseEventArgs e) {
		}
		protected override void OnMouseEnter(EventArgs e) {
		}
		protected override void OnMouseHover(EventArgs e) {
		}
		protected override void OnMouseLeave(EventArgs e) {
		}
		protected override void OnMouseMove(MouseEventArgs e) {
		}
		protected override void OnMouseUp(MouseEventArgs e) {
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
		}
		protected override void OnMove(EventArgs e) {
		}
		protected override void OnKeyDown(KeyEventArgs e) {
		}
		protected override void OnKeyUp(KeyEventArgs e) {
		}
		protected internal override void ShowCaret() {
		}
		protected internal override void ShowCaretCore() {
		}
		protected internal override bool ShouldShowCaret { get { return false; } }
	}
}
