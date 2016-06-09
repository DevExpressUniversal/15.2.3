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

using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("FullAddress")]
	[CalculatedPersistentAliasAttribute("FullAddress", "FullAddressPersistentAlias")]
	public class Address : BaseObject, IAddress {
		private const string defaultFullAddressFormat = "{Country.Name}; {StateProvince}; {City}; {Street}; {ZipPostal}";
		private const string defaultfullAddressPersistentAlias = "concat(Country.Name, StateProvince, City, Street, ZipPostal)";
		static Address() {
			AddressImpl.FullAddressFormat = defaultFullAddressFormat;
		}
		public Address(Session session) : base(session) { }
		private static string fullAddressPersistentAlias = defaultfullAddressPersistentAlias;
		private AddressImpl address = new AddressImpl();
		public static string FullAddressPersistentAlias {
			get { return fullAddressPersistentAlias; }
		}
		public static void SetFullAddressFormat(string format, string persistentAlias) {
			AddressImpl.FullAddressFormat = format;
			fullAddressPersistentAlias = persistentAlias;
		}
		public string Street {
			get { return address.Street; }
			set {
				string oldValue = address.Street;
				address.Street = value;
				OnChanged("Street", oldValue, address.Street);
			}
		}
		public string City {
			get { return address.City; }
			set {
				string oldValue = address.City;
				address.City = value;
				OnChanged("City", oldValue, address.City);
			}
		}
		public string StateProvince {
			get { return address.StateProvince; }
			set {
				string oldValue = address.StateProvince;
				address.StateProvince = value;
				OnChanged("StateProvince", oldValue, address.StateProvince);
			}
		}
		public string ZipPostal {
			get { return address.ZipPostal; }
			set {
				string oldValue = address.ZipPostal;
				address.ZipPostal = value;
				OnChanged("ZipPostal", oldValue, address.ZipPostal);
			}
		}
		ICountry IAddress.Country {
			get { return address.Country; }
			set {
				ICountry oldValue = address.Country;
				address.Country = value;
				OnChanged("Country", oldValue, address.Country);
			}
		}
		public Country Country {
			get { return address.Country as Country; }
			set {
				ICountry oldValue = address.Country;
				address.Country = value as ICountry;
				OnChanged("Country", oldValue, address.Country);
			}
		}
		public virtual string FullAddress {
			get { return ObjectFormatter.Format(AddressImpl.FullAddressFormat, this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty); }
		}
	}
}
