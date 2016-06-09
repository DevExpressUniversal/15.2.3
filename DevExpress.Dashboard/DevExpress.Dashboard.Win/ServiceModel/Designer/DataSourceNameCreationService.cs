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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public class DataSourceNameCreationService : IDataSourceNameCreationService {
		readonly IServiceProvider serviceProvider;
		CollectionPrefixCaptionGenerator<IDashboardDataSource> DataSourceCaptionGenerator {
			get { return serviceProvider.RequestServiceStrictly<IDashboardOwnerService>().Dashboard.DataSourceCaptionGenerator; }
		}
		public DataSourceNameCreationService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public string CreateName() {
			return DataSourceCaptionGenerator.GenerateName(new DataSourcePrefix());
		}
		public bool ValidateName(string name) {
			return DataSourceCaptionGenerator.ValidateName(name) == null;
		}
		class DataSourcePrefix : ISupportPrefix {
			public string Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultDataSourceName); } }
		}
	}
}
