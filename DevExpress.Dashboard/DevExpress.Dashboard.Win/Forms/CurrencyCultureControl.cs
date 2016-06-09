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
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.DashboardWin.Native {
	public partial class CurrencyCultureControl : DashboardUserControl {
		string currentCurrencyCulture;
		string currentCultureDisplayText;
		Dictionary<RegionInfoPresenter, List<string>> culturesCache;
		public string CurrencyCulture { get { return ((CultureInfoPresenter)cbeCurrencyCulture.SelectedItem).Name; } }
		public event EventHandler CurrencyCultureChanged;
		public CurrencyCultureControl() {
			InitializeComponent();
			culturesCache = new Dictionary<RegionInfoPresenter, List<string>>();
			foreach(CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
				AddCulture(ci);
			}
			cbeCurrency.Properties.Items.Clear();
			cbeCurrencyCulture.Properties.Items.Clear();
		}
		public void Initialize(string currentCurrencyCulture, string currentCultureDisplayText) {
			this.currentCurrencyCulture = currentCurrencyCulture;
			this.currentCultureDisplayText = currentCultureDisplayText;
			AddCurrentCulture();
			if(currentCurrencyCulture != null && currentCurrencyCulture.Length == 0) {
				AddInvariantCulture();
			}
			cbeCurrency.Properties.Items.AddRange(culturesCache.Keys);
			cbeCurrency.SelectedItem = CreateRegionInfoPresenter(currentCurrencyCulture);
			cbeCurrencyCulture.SelectedItem = CreateCulturePresenter(currentCurrencyCulture);
		}
		void AddCurrentCulture() {
			RegionInfoPresenter currentRegion = RegionInfoPresenter.CreateCurrentPresenter(currentCultureDisplayText);
			culturesCache.Add(currentRegion, new List<string> { null });
		}
		void AddInvariantCulture() {
			RegionInfoPresenter invariantRegion = RegionInfoPresenter.CreateInvariantPresenter();
			culturesCache.Add(invariantRegion, new List<string> { String.Empty });
		}
		void AddCulture(CultureInfo culture) {
			RegionInfoPresenter presenter = RegionInfoPresenter.CreateRegionInfoPresenter(culture.Name);
			if(culturesCache.ContainsKey(presenter)) {
				culturesCache[presenter].Add(culture.Name);
			}
			else {
				culturesCache.Add(presenter, new List<string> { culture.Name });
			}
		}
		List<string> GetCulturesByCurrency(RegionInfoPresenter presenter) {
			List<string> cultureNames;
			return culturesCache.TryGetValue(presenter, out cultureNames) ? cultureNames : null;
		}
		RegionInfoPresenter CreateRegionInfoPresenter(string cultureName) {
			if(cultureName == null) {
				return RegionInfoPresenter.CreateCurrentPresenter(currentCultureDisplayText);
			}
			if(cultureName.Length == 0) {
				return RegionInfoPresenter.CreateInvariantPresenter();
			}
			return RegionInfoPresenter.CreateRegionInfoPresenter(cultureName);
		}
		CultureInfoPresenter CreateCulturePresenter(string cultureName) {
			if(cultureName != null) {
				return new CultureInfoPresenter(CultureInfo.CreateSpecificCulture((cultureName)));
			}
			return new CultureInfoPresenter(currentCultureDisplayText);
		}
		void OnCurrencySelectedIndexChanged(object sender, EventArgs e) {
			cbeCurrencyCulture.Properties.Items.Clear();
			RegionInfoPresenter presenter = ((RegionInfoPresenter)cbeCurrency.SelectedItem);
			List<string> cultureNames = GetCulturesByCurrency(presenter);
			foreach(string cultureName in cultureNames) {
				cbeCurrencyCulture.Properties.Items.Add(CreateCulturePresenter(cultureName));
			}
			if(((RegionInfoPresenter)cbeCurrency.SelectedItem).Equals(CreateRegionInfoPresenter(currentCurrencyCulture))) {
				cbeCurrencyCulture.SelectedItem = CreateCulturePresenter(currentCurrencyCulture);
			}
			else {
				cbeCurrencyCulture.SelectedIndex = 0;
			}
			cbeCurrencyCulture.Enabled = (cbeCurrencyCulture.Properties.Items.Count > 1);
		}
		void OnCurrencyCultureSelectedIndexChanged(object sender, EventArgs e) {
			RaiseCurrencyCultureChanged();
		}
		void RaiseCurrencyCultureChanged() {
			if (CurrencyCultureChanged != null)
				CurrencyCultureChanged(this, EventArgs.Empty);
		}
		public void StateEnabled(bool enabled) {
			cbeCurrency.Enabled = enabled;
			cbeCurrencyCulture.Enabled = enabled && (cbeCurrencyCulture.Properties.Items.Count > 1);
		}
	}
}
