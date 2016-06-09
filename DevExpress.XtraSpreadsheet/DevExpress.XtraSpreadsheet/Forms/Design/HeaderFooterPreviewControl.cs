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

using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class HeaderFooterPreviewControl : XtraUserControl, INotifyPropertyChanged {
		#region Fileds
		HeaderFooterBuilder headerFooterValueBuilder;
		HeaderFooterFormatTagProvider provider;
		string headerFooterValue;
		#endregion
		public HeaderFooterPreviewControl() {
			InitializeComponent();
		}
		#region Properties
		public string HeaderFooterValue {
			get { return headerFooterValue; }
			set {
				if (HeaderFooterValue == value)
					return;
				headerFooterValue = value;
				OnPropertyChanged("Provider");
				SetPreview();
			}
		}
		public HeaderFooterFormatTagProvider Provider {
			get { return provider; }
			set {
				if (Provider == value)
					return;
				provider = value;
				OnPropertyChanged("Provider");
				SetPreview();
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
		void SetPreview() {
			if (headerFooterValue == null || provider == null)
				return;
			headerFooterValueBuilder = new HeaderFooterBuilder(headerFooterValue, true, provider);
			edtLeftHeaderFooterPreview.Text = headerFooterValueBuilder.Left;
			edtCenterHeaderFooterPreview.Text = headerFooterValueBuilder.Center;
			edtRightHeaderFooterPreview.Text = headerFooterValueBuilder.Right;
		}
	}
}
