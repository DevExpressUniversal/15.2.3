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

using DevExpress.Map.BingServices;
using DevExpress.XtraMap.Native;
using System;
using System.ComponentModel;
using System.Reflection;
namespace DevExpress.XtraMap {
	public abstract class BingMapDataProviderBase : InformationDataProviderBase {
		string bingKey = String.Empty;
		bool isBusy;
		bool isKeyRestricted = false;
		protected string ActualBingKey { get { return isKeyRestricted ? string.Empty : BingKey; } }
		protected internal override int MaxVisibleResultCountInternal { get { return 1; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("BingMapDataProviderBaseIsBusy")]
#endif
		public override bool IsBusy { get { return isBusy; } protected set { isBusy = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("BingMapDataProviderBaseBingKey"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue("")]
		public string BingKey {
			get { return bingKey; }
			set {
				if (bingKey != value) {
					bingKey = value;
					UpdateBingKey();
				}
			}
		}
		static BingMapDataProviderBase() {
			DevExpress.Map.Native.DXBingKeyVerifier.RegisterPlatform("Win");
		}
		protected BingMapDataProviderBase() {
		}
		void UpdateBingKey() {
			Assembly asm = Assembly.GetEntryAssembly();
			this.isKeyRestricted = DevExpress.Map.Native.DXBingKeyVerifier.IsKeyRestricted(BingKey, asm != null ? asm.FullName : string.Empty);
		}
		protected AddressBase ConvertToBingAddress(Address address) {
			return address != null ? new BingAddress() {
				AddressLine = address.AddressLine,
				AdminDistrict = address.AdminDistrict,
				CountryRegion = address.CountryRegion,
				FormattedAddress = address.FormattedAddress,
				PostalCode = address.PostalCode,
				Locality = address.Locality,
				District = address.District,
				PostalTown = address.PostalTown
			} : null;
		}
	}
}
