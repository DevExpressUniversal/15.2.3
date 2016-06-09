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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Native.Lines {
	public class BooleanLine : PropertyLineBase {
		#region static
		static Size MeasureCheckEdit(CheckEdit checkBox) {
			Size size = MeasureString(checkBox.Text, checkBox.Font);
			size.Width += 3 * glyphSize.Width;
			int preferredHight = checkBox.GetPreferredSize(Size.Empty).Height;
			size.Height = Math.Max(size.Height, preferredHight);
			return size;
		}
		#endregion
		CheckEdit checkBox;
		string checkBoxText;
		public BooleanLine(PropertyDescriptor property, object obj)
			: base(property, obj) {
		}
		public override void SetText(string checkBoxText) {
			this.checkBoxText = checkBoxText;
		}
		protected override void Initialize() {
			checkBox = new CheckEdit();
			SetParentLookAndFeel(checkBox);
			checkBox.TabStop = true;
			checkBox.Dock = DockStyle.Left;
			checkBox.Properties.AutoHeight = true;
			checkBox.Text = checkBoxText;
			RefreshProperty();
			checkBox.CheckStateChanged += new EventHandler(this.HandleCheckBoxChanged);
			checkBox.Size = MeasureCheckEdit(checkBox);
		}
		public override Size GetLineSize() {
			return checkBox.Size;
		}
		protected override System.Windows.Forms.Control[] GetControls() {
			return new System.Windows.Forms.Control[] { checkBox };
		}
		void HandleCheckBoxChanged(object sender, EventArgs e) {
			Value = ((CheckEdit)sender).Checked;
		}
		public override void RefreshProperty() {
			base.RefreshProperty();
			checkBox.Checked = (bool)Value;
		}
	}
}
