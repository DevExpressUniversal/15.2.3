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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Map{
	[NonCategorized]
	public abstract class AddressBase : MapDependencyObject {
		public static readonly DependencyProperty FormattedAddressProperty = DependencyPropertyManager.Register("FormattedAddress",
		   typeof(string), typeof(AddressBase), new PropertyMetadata(null, NotifyPropertyChanged));
		public string FormattedAddress {
			get { return (string)GetValue(FormattedAddressProperty); }
			set { SetValue(FormattedAddressProperty, value); }
		}
	}
	[NonCategorized]
	public class BingAddress : AddressBase {
		public static readonly DependencyProperty AddressLineProperty = DependencyPropertyManager.Register("AddressLine",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LocalityProperty = DependencyPropertyManager.Register("Locality",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty AdminDistrictProperty = DependencyPropertyManager.Register("AdminDistrict",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty CountryRegionProperty = DependencyPropertyManager.Register("CountryRegion",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty PostalTownProperty = DependencyPropertyManager.Register("PostalTown",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty DistrictProperty = DependencyPropertyManager.Register("District",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty PostalCodeProperty = DependencyPropertyManager.Register("PostalCode",
		   typeof(string), typeof(BingAddress), new PropertyMetadata(null, NotifyPropertyChanged));
		public string AddressLine {
			get { return (string)GetValue(AddressLineProperty); }
			set { SetValue(AddressLineProperty, value); }
		}
		public string Locality {
			get { return (string)GetValue(LocalityProperty); }
			set { SetValue(LocalityProperty, value); }
		}
		public string AdminDistrict {
			get { return (string)GetValue(AdminDistrictProperty); }
			set { SetValue(AdminDistrictProperty, value); }
		}
		public string CountryRegion {
			get { return (string)GetValue(CountryRegionProperty); }
			set { SetValue(CountryRegionProperty, value); }
		}
		public string PostalTown {
			get { return (string)GetValue(PostalTownProperty); }
			set { SetValue(PostalTownProperty, value); }
		}
		public string District {
			get { return (string)GetValue(DistrictProperty); }
			set { SetValue(DistrictProperty, value); }
		}
		public string PostalCode {
			get { return (string)GetValue(PostalCodeProperty); }
			set { SetValue(PostalCodeProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new BingAddress();
		}
		public override string ToString() {
			return FormattedAddress;
		}
	}
}
