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

using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
namespace DevExpress.Design.Mvvm {
	public interface IValidated {
		bool IsValid { get; }
	}
	public interface IDataModel : IValidated, IHasName {
		ExpressionHelperInfo ExpressionHelper { get; }
		IContainerInfo ContainerInfo { get; }
		IDbContainerInfo DbContainer { get; }
		IEnumerable<IEntitySetInfo> Entities { get; }
		UnitOfWorkSourceInfo UnitOfWorkSource { get; }
		UnitOfWorkFactoryInfo UnitOfWorkFactory { get; }
		EntitiesUnitOfWorkInfo EntitiesUnitOfWork { get; }
		IEnumerable<EntityRepositoryInfo> EntityRepositories { get; }
		bool IsInSolution { get; }
	}
	public interface IDataAccessLayerService : IHasTypesCache {
		IEnumerable<IDataModel> GetAvailableDataModels();
		void Register(IDataModel dataModel);
		IEntitySetInfo FindEntitySetInAvailableDataModels(IDXTypeInfo entityTypeInfo, out IDataModel model);
	}
}
