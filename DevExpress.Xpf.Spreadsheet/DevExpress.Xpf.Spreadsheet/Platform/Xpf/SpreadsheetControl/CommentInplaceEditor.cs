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

using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl {
		ICommentInplaceEditor IInnerSpreadsheetControlOwner.CreateCommentInplaceEditor() {
			return GetCommentInplaceEditor();
		}
		XpfCommentInplaceEditor GetCommentInplaceEditor() {
			return ViewControl.WorksheetControl.CommentInplaceEditor;
		}
	}
}
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region XpfCommentInplaceEditor
	public class XpfCommentInplaceEditor : CommentItem, ICommentInplaceEditor {
		#region Fields
		TextBox textBox;
		#endregion
		#region Properties
		string ICommentInplaceEditor.Text { get { return textBox.Text; } set { textBox.Text = value; } }
		#endregion
		public XpfCommentInplaceEditor() {
			DefaultStyleKey = typeof(XpfCommentInplaceEditor);
			TextSettings = WorksheetCommentPanel.CreateTextSetting();
			SetVisible(false);
		}
		void SetVisible(bool visible) {
			Visibility = visible ? Visibility.Visible : Visibility.Hidden;
		}
		void ICommentInplaceEditor.Activate() {
			SetVisible(true);
		}
		void ICommentInplaceEditor.Deactivate() {
			SetVisible(false);
		}
		void ICommentInplaceEditor.SetBackColor(System.Drawing.Color color) {
			Background = new SolidColorBrush(color.ToWpfColor());
		}
		void ICommentInplaceEditor.SetBounds(System.Drawing.Rectangle bounds) {
			Rect rect = bounds.ToRect();
			rect.Offset(-1, -1);
			SetValue(Canvas.LeftProperty, rect.Left);
			SetValue(Canvas.TopProperty, rect.Top);
			Width = rect.Width;
			Height = rect.Height;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			textBox = LayoutHelper.FindElementByName(this, "PART_CommentBox") as TextBox;
		}
		void ICommentInplaceEditor.SetFocus() {
			textBox.Focus();
		}
		void ICommentInplaceEditor.SetSelection() {
			int index = textBox.Text.Length;
			textBox.Select(index, index);
		}
	}
	#endregion
}
