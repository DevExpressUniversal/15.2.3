#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.Persistent.Base.General {
	public class AddressImpl {
		private static string fullAddressFormat;
		public static string FullAddressFormat {
			get { return fullAddressFormat; }
			set {
				fullAddressFormat = value;
			}
		}
		private string street;
		private string city;
		private string stateProvince;
		private string zipPostal;
		private ICountry country;
		public string Street {
			get { return street; }
			set { street = value; }
		}
		public string City {
			get { return city; }
			set { city = value; }
		}
		public string StateProvince {
			get { return stateProvince; }
			set { stateProvince = value; }
		}
		public string ZipPostal {
			get { return zipPostal; }
			set { zipPostal = value; }
		}
		public ICountry Country {
			get { return country; }
			set { country = value; }
		}
		public string FullAddress {
			get {
				return ObjectFormatter.Format(fullAddressFormat, this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);
			}
		}
	}
}
