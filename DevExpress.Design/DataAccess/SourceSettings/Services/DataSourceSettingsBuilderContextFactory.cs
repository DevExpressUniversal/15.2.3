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

namespace DevExpress.Design.DataAccess {
	public abstract class BaseDataSourceSettingsBuilderContextFactory : IDataSourceSettingsBuilderContextFactory {
		public IDataSourceSettingsBuilderContext GetContext(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			IDataSourceSettingsModel model;
			if(processingMode.IsOLAP)
				model = CreateOLAPSettingsModel(technologyName, processingMode, info);
			else
				model = CreateSettingsModel(technologyName, processingMode, info);
			return new DataSourceSettingsBuilderContext(processingMode, model);
		}
		protected abstract IDataSourceSettingsModel CreateSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info);
		protected virtual IDataSourceSettingsModel CreateOLAPSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			return CreateOLAPDataSourceSettingsModel(processingMode, info);
		}
		protected IDataSourceSettingsModel CreateServerSideProcessingSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			return processingMode.IsParallel ?
				(IDataSourceSettingsModel)CreatePLinqServerModeSettingsModel(info) :
				(IDataSourceSettingsModel)CreateServerModeSettingsModel(info);
		}
		protected IDataSourceSettingsModel CreateAsynchronousServerSideProcessingSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			return processingMode.IsParallel ?
				(IDataSourceSettingsModel)CreatePLinqInstantFeedbackSettingsModel(info) :
				(IDataSourceSettingsModel)CreateInstantFeedbackSettingsModel(info);
		}
		protected virtual IPLinqServerModeSettingsModel CreatePLinqServerModeSettingsModel(IDataSourceInfo info) {
			return new PLinqServerModeSettingsModel(info);
		}
		protected virtual IServerModeSettingsModel CreateServerModeSettingsModel(IDataSourceInfo info) {
			return new ServerModeSettingsModel(info);
		}
		protected virtual IPLinqInstantFeedbackSettingsModel CreatePLinqInstantFeedbackSettingsModel(IDataSourceInfo info) {
			return new PLinqInstantFeedbackSettingsModel(info);
		}
		protected virtual IInstantFeedbackSettingsModel CreateInstantFeedbackSettingsModel(IDataSourceInfo info) {
			return new InstantFeedbackSettingsModel(info);
		}
		protected virtual ISimpleBindingSettingsModel CreateSimpleBindingSettingsModel(IDataSourceInfo info) {
			return new SimpleBindingSettingsModel(info);
		}
		protected virtual IOLAPDataSourceSettingsModel CreateOLAPDataSourceSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			return new OLAPDataSourceSettingsModel(processingMode, info);
		}
		protected virtual IDataSourceSettingsModel CreateXPOProcessingSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			switch(processingMode.GetCodeName()) {
				case DataProcessingModeCodeName.XPCollectionForXPO:
					return new XPCollectionSourceSettingsModel(info);
				case DataProcessingModeCodeName.XPViewForXPO:
					return new XPViewSourceSettingsModel(info);
				case DataProcessingModeCodeName.ServerMode:
					return new XPOServerModeSettingsModel(info);
				case DataProcessingModeCodeName.InstantFeedback:
					return new XPOInstantFeedbackSettingsModel(info);
				default:
					throw new System.NotSupportedException(processingMode.ToString());
			}
		}
		protected virtual IDataSourceSettingsModel CreateExcelProcessingSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			return processingMode.IsInMemoryProcessing ?
				(IDataSourceSettingsModel)new ExcelBindingListViewSourceSettingsModel(info) :
				(IDataSourceSettingsModel)new ExcelDirectBindingSettingsModel(info);
		}
	}
}
namespace DevExpress.Design.DataAccess {
	public abstract class XamlDataSourceSettingsBuilderContextFactory : BaseDataSourceSettingsBuilderContextFactory {
		protected IDataSourceSettingsModel CreateLocalProcessingSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.XPO)
				return CreateXPOProcessingSettingsModel(processingMode, info);
			return processingMode.IsInMemoryProcessing ?
				(IDataSourceSettingsModel)CreateCollectionViewSettingsModel(technologyName, info) :
				(IDataSourceSettingsModel)CreateSimpleBindingSettingsModel(info);
		}
		protected virtual ICollectionViewSettingsModel CreateCollectionViewSettingsModel(IDataAccessTechnologyName technologyName, IDataSourceInfo info) {
			return new CollectionViewSettingsModel(CreateCollectionViewTypesProvider(technologyName), info);
		}
		protected virtual IDomainDataSourceSettingsModel CreateDomainDataSourceSettingsModel(IDataSourceInfo info) {
			return new DomainDataSourceSettingsModel(info);
		}
		protected virtual ICollectionViewTypesProvider CreateCollectionViewTypesProvider(IDataAccessTechnologyName technologyName) {
			return new CollectionViewTypesProvider(technologyName.GetCodeName());
		}
		protected class CollectionViewTypesProvider : ICollectionViewTypesProvider {
			System.Collections.Generic.IEnumerable<string> types;
			public CollectionViewTypesProvider(DataAccessTechnologyCodeName technologyCodeName) {
				this.types = Resolve(technologyCodeName);
			}
			protected virtual System.Collections.Generic.IEnumerable<string> Resolve(DataAccessTechnologyCodeName technologyCodeName) {
				switch(technologyCodeName) {
					case DataAccessTechnologyCodeName.TypedDataSet:
						return new string[] { "ListCollectionView", "BindingListCollectionView", };
					case DataAccessTechnologyCodeName.EntityFramework:
					case DataAccessTechnologyCodeName.LinqToSql:
						return new string[] { "ListCollectionView", "CollectionView", };
					case DataAccessTechnologyCodeName.Wcf:
						return new string[] { "ListCollectionView", "CollectionView", };
					case DataAccessTechnologyCodeName.IEnumerable:
						return new string[] { "ListCollectionView", "BindingListCollectionView", "CollectionView", };
					default:
						return Throw(technologyCodeName);
				}
			}
			protected static System.Collections.Generic.IEnumerable<string> Throw(DataAccessTechnologyCodeName technologyCodeName) {
				throw new System.NotSupportedException("InitializeCollectionViewTypes() for " + technologyCodeName.ToString());
			}
			System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator() {
				return types.GetEnumerator();
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return types.GetEnumerator();
			}
		}
	}
}
namespace DevExpress.Design.DataAccess.Win {
	sealed class DataSourceSettingsBuilderContextFactory : BaseDataSourceSettingsBuilderContextFactory {
		protected override IDataSourceSettingsModel CreateSettingsModel(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info) {
			if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.XPO)
				return CreateXPOProcessingSettingsModel(processingMode, info);
			if(processingMode.IsServerSide) {
				return processingMode.IsAsynchronous ?
					CreateAsynchronousServerSideProcessingSettingsModel(processingMode, info) :
					CreateServerSideProcessingSettingsModel(processingMode, info);
			}
			else {
				if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.XmlDataSet)
					return new XmlDataSetSettingsModel(info);
				if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.ExcelDataSource)
					return CreateExcelProcessingSettingsModel(processingMode, info);
				if(technologyName.GetCodeName() == DataAccessTechnologyCodeName.IEnumerable && processingMode.IsInMemoryProcessing)
					return new TypedListSourceSettingsModel(info);
				return CreateLocalProcessingSettingsModel(processingMode, info);
			}
		}
		IDataSourceSettingsModel CreateLocalProcessingSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info) {
			return processingMode.IsInMemoryProcessing ?
				(IDataSourceSettingsModel)new BindingListViewSourceSettingsModel(info) :
				(IDataSourceSettingsModel)new DirectBindingSettingsModel(info);
		}
	}
}
