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
namespace DevExpress.DashboardWin.Native {
	public class CultureInfoPresenter : IComparable {
		readonly CultureInfo cultureInfo;
		readonly string displayName;
		string DisplayName { get { return cultureInfo != null ? cultureInfo.DisplayName : displayName; } }
		public string Name { get { return cultureInfo != null ? cultureInfo.Name : null; } }
		public CultureInfoPresenter(CultureInfo cultureInfo) {
			this.cultureInfo = cultureInfo;
		}
		public CultureInfoPresenter(string displayName) {
			this.displayName = displayName;
		}
		public int CompareTo(object obj) {
			CultureInfoPresenter presenter = (CultureInfoPresenter)obj;
			return DisplayName.CompareTo(presenter.DisplayName);
		}
		public override string ToString() {
			return DisplayName;
		}
		public override bool Equals(object obj) {
			CultureInfoPresenter presenter = (CultureInfoPresenter)obj;
			return object.Equals(cultureInfo, presenter.cultureInfo);
		}
		public override int GetHashCode() {
			return cultureInfo != null ? cultureInfo.GetHashCode() : 0;
		}
	}
}
