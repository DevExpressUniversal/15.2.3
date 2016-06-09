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

using System;
using DevExpress.DashboardWin.Localization;
using DashboardNative = DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public partial class DashboardCurrencyCultureForm : DashboardForm {
		const decimal PreviewValue = 123M;
		readonly string currentCurrencyCultureName;
		public string CurrencyCulture { get { return currencyChooser.CurrencyCulture; } }
		string SelectedCurrencyCultureName { 
			get {
				string currencyCulture = currencyChooser.CurrencyCulture;
				if(currencyCulture != null)
					return currencyCulture;
				return this.currentCurrencyCultureName ?? DashboardNative.Helper.DefaultCurrencyCultureName;
			} 
		}
		public DashboardCurrencyCultureForm() {
			InitializeComponent();
		}
		public DashboardCurrencyCultureForm(string currentCurrencyCultureName)
			: this() {
			this.currentCurrencyCultureName = currentCurrencyCultureName;
			currencyChooser.Initialize(currentCurrencyCultureName, DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardCurrencyUseCurrentCurrency));
			currencyChooser.CurrencyCultureChanged += OnCurrencyCultureChanged;
			currencyPreview.Initialize(PreviewValue);
			currencyPreview.UpdatePreview(SelectedCurrencyCultureName);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				currencyChooser.CurrencyCultureChanged -= OnCurrencyCultureChanged;
		}
		void OnCurrencyCultureChanged(object sender, EventArgs e) {
			currencyPreview.UpdatePreview(SelectedCurrencyCultureName);
		}
	}
}
