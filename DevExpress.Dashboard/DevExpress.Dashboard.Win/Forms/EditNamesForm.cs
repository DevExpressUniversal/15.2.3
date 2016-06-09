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

using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public partial class EditNamesForm : DashboardForm {
		const int LocationX = 33;
		const int LocationY = 3;
		const int Indent = 5;
		readonly List<TextEdit> textEdits = new List<TextEdit>();
		public EditNamesForm() {
			InitializeComponent();
		}
		public EditNamesForm(EditNamesHistoryItem historyItem)
			: this() {
			imageList.Images.Add(SystemIcons.Warning);
			scrollableControl.HorizontalScroll.Visible = false;
			SuspendLayout();
			try {
				Point location = new Point(LocationX, LocationY);
				foreach (EditNameCollection collection in historyItem.Collections) {
					if (collection.Items.Count > 0) {
						LabelControl label = new LabelControl();
						label.Location = location;
						label.Text = collection.Caption;
						scrollableControl.Controls.Add(label);
						location = new Point(LocationX, label.Location.Y + label.Height + Indent);
						foreach (EditNameItem item in collection.Items) {
							TextEdit edit = new TextEdit();
							edit.Location = location;
							edit.Width = scrollableControl.Width - LocationX * 2;
							edit.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
							edit.Tag = item;
							edit.Text = item.DisplayName;
							scrollableControl.Controls.Add(edit);
							location = new Point(LocationX, edit.Location.Y + edit.Height + Indent);
							textEdits.Add(edit);
						}
					}
				}
			}
			finally {
				ResumeLayout();
			}
		}
		void ButtonOKClick(object sender, EventArgs e) {
			foreach(TextEdit textEdit in textEdits) {
				EditNameItem item = (EditNameItem)textEdit.Tag;
				if(item.DisplayName != textEdit.Text)
					item.EditName = textEdit.Text;
			}
		}
	}
}
