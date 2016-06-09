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
using DevExpress.DashboardCommon.Service;
using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	class ParameterInfoWrapper : IParameter {
		public static IEnumerable<IParameter> Wrap(IEnumerable<DashboardParameterInfo> parameterInfos) {
			List<IParameter> list = new List<IParameter>();
			if(parameterInfos != null) {
				foreach(DashboardParameterInfo info in parameterInfos) {
					list.Add(new ParameterInfoWrapper(info));
				}
			}
			return list;
		}
		readonly DashboardParameterInfo info;
		ParameterInfoWrapper(DashboardParameterInfo info) {
			this.info = info;
		}
		#region IParameter Members
		object IParameter.Value {
			get { return info.Value; }
			set { info.Value = value; }
		}
		#endregion
		#region IFilterParameter Members
		string XtraEditors.Filtering.IFilterParameter.Name {
			get { return info.Name; }
		}
		Type XtraEditors.Filtering.IFilterParameter.Type {
			get { throw new NotImplementedException("ParameterInfoWrapper"); }
		}
		#endregion
	}
	class ParameterComparer : IComparer<IParameter> {
		static ParameterComparer defaultComparer = new ParameterComparer();
		public static ParameterComparer Default { get { return defaultComparer; } }
		#region IComparer<IParameter> Members
		int IComparer<IParameter>.Compare(IParameter p1, IParameter p2) {
			return StringExtensions.ComparerInvariantCulture.Compare(p1.Name, p2.Name);
		}
		#endregion
	}
}
