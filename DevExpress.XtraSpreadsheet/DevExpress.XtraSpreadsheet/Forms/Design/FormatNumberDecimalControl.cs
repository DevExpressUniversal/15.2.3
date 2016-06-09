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
using System.Windows.Forms;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class FormatNumberDecimalControl : XtraUserControl, INotifyPropertyChanged {
		#region Fields
		int decimalPlaces;
		#endregion
		public FormatNumberDecimalControl() {
			InitializeComponent();
			List<string> dataSource = new List<string>();
			dataSource.Add("$");
			this.edtSymbol.Properties.DataSource = dataSource;
			this.edtSymbol.EditValue = "$";
			SetBindings();
		}
		#region Properties
		public int DecimalPlaces {
			get { return decimalPlaces; }
			set {
				if (decimalPlaces == value)
					return;
				this.decimalPlaces = value;
				OnPropertyChanged("DecimalPlaces");
			}
		}
		public bool UseThousandSeparatorVisible {
			get { return chkUseThousandSeparator.Visible; }
			set { chkUseThousandSeparator.Visible = value; }
		}
		public bool SymbolListVisible {
			get { return lblSymbol.Visible; }
			set {
				lblSymbol.Visible = value;
				edtSymbol.Visible = value;
			}
		}
		public bool NegativeNumbersVisible {
			get { return lblNegativeNumbers.Visible; }
			set {
				lblNegativeNumbers.Visible = value;
				lBoxNegativeNumbers.Visible = value;
			}
		}
		#endregion
		#region Events
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		void SetBindings() {
			this.edtDecimalPlaces.DataBindings.Add("EditValue", this, "DecimalPlaces", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
