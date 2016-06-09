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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class SizeControl : XtraUserControl {
		public event EventHandler ValueChanged;
		int WidthValue {
			get { return (int)seWidth.Value; }
			set { seWidth.Value = value; }
		}
		int HeigthValue {
			get { return (int)seHeigth.Value; }
			set { seHeigth.Value = value; }
		}
		public Size Value {
			get { return new Size(WidthValue, HeigthValue); }
			set {
				if (value.Height != HeigthValue || value.Width != WidthValue) {
					WidthValue = value.Width;
					HeigthValue = value.Height;
					OnValueChanged();
				}
			}
		}	   
		public SizeControl() {
			InitializeComponent();
			this.seHeigth.ValueChanged += seHeigth_ValueChanged;
			this.seWidth.ValueChanged += seWidth_ValueChanged;
		}
		void OnValueChanged() {
			if (ValueChanged != null)
				ValueChanged(this, new EventArgs());
		}
		void seHeigth_ValueChanged(object sender, EventArgs e) {
			OnValueChanged();
		}
		void seWidth_ValueChanged(object sender, EventArgs e) {
			OnValueChanged();
		}
		private void seHeigth_EditValueChanged(object sender, EventArgs e) {
		}
	}
}
