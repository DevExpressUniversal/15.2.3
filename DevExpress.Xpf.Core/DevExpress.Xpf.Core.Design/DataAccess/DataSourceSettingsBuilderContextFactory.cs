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

namespace DevExpress.Design.DataAccess.Wpf {
	sealed class DataSourceSettingsBuilderContextFactory : XamlDataSourceSettingsBuilderContextFactory {
		protected override IDataSourceSettingsModel CreateSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			if(processingMode.IsServerSide) {
				return processingMode.IsAsynchronous ?
					CreateAsynchronousServerSideProcessingSettingsModel(processingMode, info) :
					CreateServerSideProcessingSettingsModel(processingMode, info);
			}
			return CreateLocalProcessingSettingsModel(technologyName, processingMode, info);
		}
	}
}
namespace DevExpress.Design.DataAccess.Silverlight {
	sealed class DataSourceSettingsBuilderContextFactory : XamlDataSourceSettingsBuilderContextFactory {
		protected override IDataSourceSettingsModel CreateSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.Ria)
				return CreateDomainDataSourceSettingsModel(info);
			else {
				if(processingMode.IsServerSide) {
					return processingMode.IsAsynchronous ?
						CreateAsynchronousServerSideProcessingSettingsModel(processingMode, info) :
						CreateServerSideProcessingSettingsModel(processingMode, info);
				}
				return CreateLocalProcessingSettingsModel(technologyName, processingMode, info);
			}
		}
		protected override ICollectionViewTypesProvider CreateCollectionViewTypesProvider(IDataAccessTechnologyName technologyName) {
			return new CollectionViewTypesProviderSL(technologyName.GetCodeName());
		}
		class CollectionViewTypesProviderSL : CollectionViewTypesProvider {
			public CollectionViewTypesProviderSL(DataAccessTechnologyCodeName technologyCodeName)
				: base(technologyCodeName) {
			}
			protected override System.Collections.Generic.IEnumerable<string> Resolve(DataAccessTechnologyCodeName technologyCodeName) {
				switch(technologyCodeName) {
					case DataAccessTechnologyCodeName.Ria:
					case DataAccessTechnologyCodeName.Wcf:
						return new string[] { "CollectionViewSource", "PagedCollectionView", };
					case DataAccessTechnologyCodeName.IEnumerable:
						return new string[] { "CollectionViewSource" };
					default:
						return Throw(technologyCodeName);
				}
			}
		}
	}
}
