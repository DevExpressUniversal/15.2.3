﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardItemColorSchemeProvider : ColorShemeProvider, IColorSchemeProvider {
		readonly DataDashboardItem item;
		public DashboardItemColorSchemeProvider(DataDashboardItem item, IDataSessionProvider dataSessionProvider, IDataSourceInfoProvider dataInfoProvider)
			: base(dataSessionProvider, dataInfoProvider) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		#region IColorSchemeProvider
		ColoringSchemeDefinition IColorSchemeProvider.GetColoringScheme() {
			return item.GetColoringScheme();
		}
		IList<object[]> IColorSchemeProvider.GetColoringValues(ColorRepositoryKey repositoryKey, IActualParametersProvider parameters) {
			return GetDashboardItemColoringValues(item, repositoryKey.DimensionDefinitions);
		}
		#endregion
	}
}
