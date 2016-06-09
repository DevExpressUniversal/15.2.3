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

#if SILVERLIGHT
extern alias Platform;
#endif
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using Platform::DevExpress.Xpf.Grid;
#else
using DevExpress.Xpf.Grid;
#endif
namespace DevExpress.Xpf.Grid.Design {
	public class GridDataAccessTechnologyCollectionProvider : IDataAccessTechnologyCollectionProvider {
		DataControlAdornerProvider provider;
		DataViewBase view;
		public GridDataAccessTechnologyCollectionProvider(DataControlAdornerProvider provider) {
			this.provider = provider;
		}
		public GridDataAccessTechnologyCollectionProvider(DataViewBase view) {
			this.view = view;
		}
		bool IsTreeListViewSelected { get { return (provider == null ? view : provider.View) is TreeListView; } }
		IList<IDataAccessTechnologyInfo> GetDataAccessTechnologyCollection(bool isSupportServerMode) {
			List<IDataAccessTechnologyInfo> collection = new List<IDataAccessTechnologyInfo>() { 
#if !SILVERLIGHT
				new EntityFrameworkInfo<GridDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode }, 
				new TypedDataSourceInfo<TypedDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode },
				new LinqToSQLDataSourceInfo<GridDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode },
#else
				new RIADataSourceInfo<GridDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode },
#endif
				new WCFDataSourceInfo<GridDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode },
				new IEnumerableDataSourceInfo<GridDataSourceGenerator>() { IsSupportServerMode = isSupportServerMode }
			};
			return collection;
		}
		public IList<IDataAccessTechnologyInfo> DataAccessTechnologyCollection { get { return GetDataAccessTechnologyCollection(!IsTreeListViewSelected);  } }
	}
}
