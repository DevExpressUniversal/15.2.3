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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.Web.Design {
	public class ControlCreator {
		public static Button CreateButton(string text) {
			Button button = new Button();		  
			button.Text = text;
			return button;
		}
		public static Label CreateLabel(string text) {
			Label label = new Label();
			label.Text = text;
			label.Dock = DockStyle.Top;
			label.Margin = new Padding(0);
			label.TextAlign  = ContentAlignment.MiddleLeft;
			return label;
		}
		public static Label CreateLabelWithTopLeftAnchor(string text, Point location) {
			Label label = new Label();
			label.Text = text;
			label.AutoSize = true;
			label.Location = location;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			return label;
		}
		public static ListBox CreateListBox() {
			ListBox listBox = new ListBox();
			listBox.IntegralHeight = false;
			return listBox;
		}
		public static TextBox CreateMultilineTextBox() {
			return new MultilineTextBox();
		}
		public static TextBox CreateTextBox() {
			TextBox ret = new TextBox();
			ret.Margin = new Padding(0);
			return ret;
		}
		public static TextBox CreateTextBoxWithTopLeftAnchor(Size size, Point location) {
			TextBox ret = CreateTextBox();
			ret.Size = size;
			ret.Location = location;
			return ret;
		}
	}
}
