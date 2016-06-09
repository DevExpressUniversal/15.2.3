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
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public class DescendantBuilderFactoryBase {
		List<DescendantBuilderProvider> providers;
		public DescendantBuilderFactoryBase() {
			this.providers = new List<DescendantBuilderProvider>();
		}
		public void Initialize() {
			InitializeProviders();
		}
		protected virtual void InitializeProviders() {
			Add(new DefaultDescendantBuilderProvider());
			Add(new SqlCeDescendantBuilderProvider());
			Add(new SqlClientDescendantBuilderProvider());
		}
		protected void Add(DescendantBuilderProvider provider) { 
			providers.Add(provider);
		}
		public DbDescendantBuilder GetDbDescendantBuilder(IDXTypeInfo dXTypeInfo, ISolutionTypesProvider typesProvider) {
			DXTypeInfo typeInfo = dXTypeInfo as DXTypeInfo;
			if (typeInfo == null)
				return null;
			TypesCollector typesCollector = new TypesCollector(typeInfo);
			if (typesCollector.DbContextType == null || typesCollector.DbDescendantType == null)
				return null;
			foreach (DescendantBuilderProvider provider in providers)
				if (provider.Available(typesCollector.DbContextType, typesCollector.DbDescendantInfo, typesProvider))
					return provider.GetBuilder(typesCollector, typesProvider);
			return null;
		}
	}
}
