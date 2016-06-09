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

using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Wizard {
	public interface IDataSourceWizardClientBase {
		IParameterService ParameterService { get; }
		IConnectionStorageService ConnectionStorageService { get; }
		IConnectionStringsProvider ConnectionStringsProvider { get; }
		IDataConnectionParametersService DataConnectionParametersProvider { get; }
	}
	public abstract class DataSourceWizardClientBase : IDataSourceWizardClientBase {
		readonly IParameterService parameterService;
		protected DataSourceWizardClientBase(IParameterService parameterService, IConnectionStorageService connectionStorage) : this(parameterService, connectionStorage, null) { }
		protected DataSourceWizardClientBase(IParameterService parameterService, IConnectionStorageService connectionStorage, IConnectionStringsProvider connectionStringsProvider) {
			this.parameterService = parameterService;
			ConnectionStorageService = connectionStorage;
			ConnectionStringsProvider = connectionStringsProvider;
		}
		#region IDataSourceWizardClientBase Members
		public IParameterService ParameterService { get { return parameterService; } }
		public IConnectionStorageService ConnectionStorageService { get; private set; }
		public IConnectionStringsProvider ConnectionStringsProvider { get; private set; }
		public IDataConnectionParametersService DataConnectionParametersProvider { get; set; }
		#endregion
	}
}
