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
using System.Globalization;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class RegionInfoPresenter : IComparable {
		enum PresenterKind {
			Current = 0,
			Invariant = 1,
			RegionInfo = 2,
		}
		public static RegionInfoPresenter CreateRegionInfoPresenter(string cultureName) {
			CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureName);
			return new RegionInfoPresenter(new RegionInfo(culture.LCID));
		}
		public static RegionInfoPresenter CreateInvariantPresenter() {
			return new RegionInfoPresenter();
		}
		public static RegionInfoPresenter CreateCurrentPresenter(string displayName) {
			return new RegionInfoPresenter(displayName);
		}
		readonly PresenterKind kind;
		readonly RegionInfo regionInfo;
		readonly string currentDisplayName;
		string RegionInfoDisplayName { get { return String.Format("{0} ({1})", regionInfo.ISOCurrencySymbol, regionInfo.CurrencyEnglishName); } }
		RegionInfoPresenter(RegionInfo regionInfo) {
			kind = PresenterKind.RegionInfo;
			this.regionInfo = regionInfo;
		}
		RegionInfoPresenter() {
			kind = PresenterKind.Invariant;
		}
		RegionInfoPresenter(string currentDisplayName) {
			kind = PresenterKind.Current;
			this.currentDisplayName = currentDisplayName;
		}
		public int CompareTo(object obj) {
			RegionInfoPresenter presenter = (RegionInfoPresenter)obj;
			int result = kind.CompareTo(presenter.kind);
			if(result == 0 && kind == PresenterKind.RegionInfo)
				return RegionInfoDisplayName.CompareTo(presenter.RegionInfoDisplayName);
			return result;
		}
		public override string ToString() {
			if(kind == PresenterKind.Current)
				return currentDisplayName;
			if(kind == PresenterKind.Invariant)
				return DashboardWinLocalizer.GetString(DashboardWinStringId.CurrencyInvariantRegion);
			return RegionInfoDisplayName;
		}
		public override bool Equals(object obj) {
			RegionInfoPresenter presenter = (RegionInfoPresenter)obj;
			if(presenter.kind == kind) {
				if(kind == PresenterKind.RegionInfo)
					return presenter.RegionInfoDisplayName.Equals(RegionInfoDisplayName);
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			int hashCode = kind.GetHashCode();
			if(kind == PresenterKind.RegionInfo)
				hashCode ^= RegionInfoDisplayName.GetHashCode();
			return hashCode;
		}
	}
}
