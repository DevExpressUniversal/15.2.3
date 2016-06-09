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

using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class LocationInformation : MapDependencyObject {
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
			typeof(GeoPoint), typeof(LocationInformation), new PropertyMetadata(new GeoPoint(), NotifyPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty EntityTypeProperty = DependencyPropertyManager.Register("EntityType",
			typeof(string), typeof(LocationInformation), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty DisplayNameProperty = DependencyPropertyManager.Register("DisplayName",
			typeof(string), typeof(LocationInformation), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty AddressProperty = DependencyPropertyManager.Register("Address",
		   typeof(AddressBase), typeof(LocationInformation), new PropertyMetadata(null, NotifyPropertyChanged));
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		public GeoPoint Location {
			get { return (GeoPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public string EntityType {
			get { return (string)GetValue(EntityTypeProperty); }
			set { SetValue(EntityTypeProperty, value); }
		}
		public string DisplayName {
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		public AddressBase Address {
			get { return (AddressBase)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}
		public LocationInformation() {
		}
		public LocationInformation(GeoPoint location, string entityType, string displayName, AddressBase address) {
			SetValue(LocationProperty, location);
			SetValue(EntityTypeProperty, entityType);
			SetValue(DisplayNameProperty, displayName);
			SetValue(AddressProperty, address);
		}
		protected override MapDependencyObject CreateObject() {
			return new LocationInformation();
		}
		public override string ToString() {
			return DisplayName;
		}
	}
}
