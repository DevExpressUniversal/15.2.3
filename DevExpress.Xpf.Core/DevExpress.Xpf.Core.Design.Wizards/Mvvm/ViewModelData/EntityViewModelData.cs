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
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class EntityViewModelData : ViewModelDataBase {
		public EntityViewModelData(bool useProxyFactory,
			string assemblyName,
			bool isSolutionType,
			string name,
			string nameSpace,
			bool isLocalType,
			bool supportServices,
			string entityPropertyName,
			Func<ImageType, CommandInfo[]> createCommands,
			IEnumerable<TypeNamePropertyPair> collectionProperties,
			bool activated,
			IEnumerable<LookUpCollectionViewModelData> lookUpTables,
			XamlNamespaceDeclaration xamlNamespace,
			IEntityTypeInfo typeInfo,
			Func<IEdmPropertyInfo, ForeignKeyInfo> getForeignKeyProperty)
			: base(useProxyFactory, assemblyName, isSolutionType, name, nameSpace, isLocalType, supportServices, typeInfo.Type.Name, typeInfo.Type, entityPropertyName, createCommands, typeInfo.AllProperties, collectionProperties, activated, xamlNamespace) {
			this.LookUpTables = lookUpTables ?? new LookUpCollectionViewModelData[0];
			this.typeInfo = typeInfo;
			this.GetForeignKeyProperty = getForeignKeyProperty;
		}
		public IEnumerable<LookUpCollectionViewModelData> LookUpTables {
			get; private set;
		}
		readonly IEntityTypeInfo typeInfo;
		public IEntityTypeInfo TypeInfo { get { return typeInfo; } }
		public Func<IEdmPropertyInfo, ForeignKeyInfo> GetForeignKeyProperty { get; private set; }
	}
}
