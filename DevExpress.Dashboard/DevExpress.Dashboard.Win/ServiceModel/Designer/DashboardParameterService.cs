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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.Utils.UI;
using System;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardParameterService : IActualParametersProvider {
		DashboardParameterCollection ParameterCollection { get; }
	}
	public class DashboardParameterService : IDashboardParameterService, IParameterService, IParameterCreator {
		readonly IServiceProvider serviceProvider;
		public DashboardParameterCollection ParameterCollection {
			get {
				IDashboardOwnerService ownerService = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				return ownerService.Dashboard.Parameters;
			}
		}
		public DashboardParameterService(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
		}
		public string AddParameterString { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.AddDashboardParameter); } }
		public string CreateParameterString { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.ParameterFormCaption); } }
		public IEnumerable<IParameter> Parameters { get { return ParameterCollection; } }
		public bool CanCreateParameters { get { return true; } }
		public void AddParameter(IParameter parameter) {
			DashboardParameter dasboardParameter = (DashboardParameter)parameter;
			ParameterCollection.Add(dasboardParameter);
		}
		public IParameter CreateParameter(Type type) {
			return CreateParameter(ParameterDescriptionProvider.GenerateName(Parameters), type);
		}
		public IParameter CreateParameter(string name, Type type) {
			return new DashboardParameter { 
				Name = name, 
				Type = type 
			};
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			throw new InvalidOperationException();
		}
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			return ParameterCollection;
		}
	}
}
