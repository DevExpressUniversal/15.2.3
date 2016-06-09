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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms.Design {
	[DXToolboxItem(false)]
	public partial class BorderShadingTypeLineUserControl : UserControl {
		#region Fields
		BorderInfo border;
		DocumentModel documentModel;
		IRichEditControl richEditControl;
		#endregion
		public BorderShadingTypeLineUserControl() {
			InitializeComponent();
			border = new BorderInfo();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } set { documentModel = value; borderLineStyleListBox1.DocumentModel = value; } }
		public IRichEditControl RichEditControl{ get { return richEditControl; } set { richEditControl = value; borderLineWeightEdit1.Control = (RichEditControl)value; } }
		public BorderInfo Border { get { return border;} }
		#endregion
		public event  EventHandler BorderChanged;
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		protected void RaiseBorderChanged() {
			if (BorderChanged != null)
				BorderChanged(this, EventArgs.Empty);
		}
		private void borderLineStyleListBox1_SelectedValueChanged(object sender, EventArgs e) {
			border.Style = borderLineStyleListBox1.BorderLineStyle;
			RaiseBorderChanged();
		}
		private void colorEdit1_EditValueChanged(object sender, EventArgs e) {
			border.Color = colorEdit1.Color;
			RaiseBorderChanged();
		}
		private void borderLineWeightEdit1_EditValueChanged(object sender, EventArgs e) {
			border.Width = (int) (borderLineWeightEdit1.EditValue);
			RaiseBorderChanged();
		}
		public void SetInitialValue (BorderInfo border) {
			this.border.CopyFrom(border);
			borderLineStyleListBox1.BorderLineStyle = border.Style;
			colorEdit1.Color = border.Color;
			borderLineWeightEdit1.EditValue = border.Width;
		}
	}
}
