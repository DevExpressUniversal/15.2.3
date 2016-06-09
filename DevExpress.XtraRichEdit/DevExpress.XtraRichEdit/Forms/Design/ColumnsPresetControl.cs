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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraRichEdit.Forms.Design {
	[DXToolboxItem(false)]
	public partial class ColumnsPresetControl : XtraUserControl {
		private const int DEFAULT_IMAGE_MARGIN = 8;
		private const int IMAGE_PADDING = 5;
		public ColumnsPresetControl() {
			InitializeComponent();
		}
		#region Properties
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		[DefaultValue(""), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text { get { return label.Text; } set { label.Text = value; } }
		[DefaultValue(false)]
		public Image Image { 
			get { return checkBox.Properties.PictureUnchecked; }
			set {
				checkBox.Properties.PictureChecked = value;
				checkBox.Properties.PictureGrayed = value;
				checkBox.Properties.PictureUnchecked = value;
				if (!ReferenceEquals(value, null)) {
					int imageMargin = GetMinImageMargin() * 2;
					checkBox.Size = new Size(Image.Size.Width + imageMargin, Image.Size.Height + imageMargin);
					checkBox.Location = GetLocation(imageMargin);
				}
			} 
		}
		Point GetLocation(int indent) {
			int x = (this.Size.Width - checkBox.Size.Width) / 2;
			int y = (this.Size.Height - checkBox.Size.Height - label.Size.Height) / 2;
			return new Point(x, y);
		}
		int GetMinImageMargin() {
			int result = DEFAULT_IMAGE_MARGIN; 
			if (!IsHorizontalMarginAllowed(DEFAULT_IMAGE_MARGIN)) {
				int actualHorizontalMargin = (this.Size.Width - Image.Size.Width) / 2;
				result = Math.Min(result, actualHorizontalMargin);
			}
			if (!IsVerticalMarginAllowed(DEFAULT_IMAGE_MARGIN)) {
				int actualVerticalMargin = (this.Size.Height - label.Size.Height - Image.Size.Height) / 2;
				result = Math.Min(result, actualVerticalMargin);
			}
			return Math.Min(DEFAULT_IMAGE_MARGIN, result);
		}
		bool IsHorizontalMarginAllowed(int result) {
			return Image.Size.Width + result * 2 <= this.Size.Width;
		}
		bool IsVerticalMarginAllowed(int result) {
			return (Image.Size.Height + result * 2) + label.Size.Height <= this.Size.Height;
		}
		[DefaultValue(false)]
		public bool Checked { get { return checkBox.Checked; } set { checkBox.Checked = value; } }
		#endregion
		#region Events
		EventHandler onCheckedChanged;
		public event EventHandler CheckedChanged { add { onCheckedChanged += value; } remove { onCheckedChanged -= value; } }
		protected internal virtual void RaiseCheckedChanged() {
			if (onCheckedChanged != null)
				onCheckedChanged(this, EventArgs.Empty);
		}
		#endregion
		private void checkBox_Paint(object sender, PaintEventArgs e) {
			const int thickness = 2;
			if (checkBox.Checked) {
				using (Pen pen = new Pen(GetHighlightColor(checkBox), thickness)) {
					int x = IMAGE_PADDING;
					int y = IMAGE_PADDING;
					int width = checkBox.Size.Width - IMAGE_PADDING * 2;
					int height = checkBox.Size.Height - IMAGE_PADDING * 2;
					e.Graphics.DrawRectangle(pen, x, y, width, height);
				}
			}
		}
		void checkBox_CheckedChanged(object sender, EventArgs e) {
			RaiseCheckedChanged();
		}
		Color GetHighlightColor(CheckEdit checkEdit) {
				return SystemColors.Highlight;
		}
	}
}
