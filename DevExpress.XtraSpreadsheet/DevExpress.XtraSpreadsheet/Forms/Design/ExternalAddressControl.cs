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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class ExternalAddressControl : XtraUserControl {
		public ExternalAddressControl() {
			InitializeComponent();
		}
		#region Properties
		public override string Text { get { return btnEditAddress.Text; } set { btnEditAddress.Text = value; } }
		public int TextEditSelectionStart { get { return btnEditAddress.SelectionStart; } set { btnEditAddress.SelectionStart = value; } }
		public XtraEditors.TextEdit TextEdit { get { return btnEditAddress; } }
		#endregion
		#region Events
		public event EventHandler EditValueChanged { add { btnEditAddress.EditValueChanged += value; } remove { btnEditAddress.EditValueChanged -= value; } }
		public event ChangingEventHandler EditValueChanging { add { btnEditAddress.EditValueChanging += value; } remove { btnEditAddress.EditValueChanging -= value; } }
		public event ButtonPressedEventHandler ButtonClick { add { btnEditAddress.ButtonClick += value; } remove { btnEditAddress.ButtonClick -= value; } }
		#endregion
	}
}
