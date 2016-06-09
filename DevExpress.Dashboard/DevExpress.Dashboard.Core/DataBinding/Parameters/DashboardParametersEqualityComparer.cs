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

using System.Collections.Generic;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardParametersEqualityComparer : IEqualityComparer<DashboardParameter> {
		public bool Equals(DashboardParameter x, DashboardParameter y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(!new ParametersEqualityComparer().Equals(x, y))
				return false;
			if(!Equals(x.AllowNull, y.AllowNull))
				return false;
			if(!Equals(x.AllowMultiselect, y.AllowMultiselect))
				return false;
			if(!string.Equals(x.Description, y.Description))
				return false;
			if(!x.Visible.Equals(y.Visible))
				return false;
			if(x.LookUpSettings == null && y.LookUpSettings == null)
				return true;
			if(x.LookUpSettings == null || y.LookUpSettings == null)
				return false;
			return x.LookUpSettings.SettingsEquals(y.LookUpSettings);
		}
		public int GetHashCode(DashboardParameter obj) {
			return obj.Name.GetHashCode();
		}
	}
}
