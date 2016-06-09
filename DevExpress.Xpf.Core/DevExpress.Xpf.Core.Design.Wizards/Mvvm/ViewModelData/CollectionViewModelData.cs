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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections;
using DevExpress.Design.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Utils.Design;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class CollectionViewModelData : ViewModelDataBase {
		public CollectionViewModelData(bool useProxyFactory,
			string assemblyName,
			bool isSolutionType,
			string name,
			string nameSpace,
			bool isLocalType,
			bool supportServices,
			string defaultViewFolderName,
			Type entityType,
			string entityPropertyName,
			string collectionPropertyName,
			Func<ImageType, CommandInfo[]> createCommands,
			IEnumerable<IEdmPropertyInfo> scaffoldingProperties,
			IEnumerable<TypeNamePropertyPair> collectionProperties,
			bool activated,
			XamlNamespaceDeclaration nameSpaceDeclaration,
			bool useIsLoadingBinding,
			Func<IEdmPropertyInfo, ForeignKeyInfo> getForeignKeyProperty,
			IEnumerable<TypeNamePropertyPair> collectionPropertiesForWin)
			: base(useProxyFactory, assemblyName, isSolutionType, name, nameSpace, isLocalType, supportServices, defaultViewFolderName, entityType, entityPropertyName, createCommands, scaffoldingProperties, collectionProperties, activated, nameSpaceDeclaration) {
			this.CollectionPropertyName = collectionPropertyName;
			this.UseIsLoadingBinding = useIsLoadingBinding;
			this.GetForeignKeyProperty = getForeignKeyProperty;
			this.collectionPropertiesForWin = collectionPropertiesForWin;
		}
		readonly IEnumerable<TypeNamePropertyPair> collectionPropertiesForWin;
		public IEnumerable<TypeNamePropertyPair> GetCollectionPropertiesForWin() {
			return collectionPropertiesForWin;
		}
		public Func<IEdmPropertyInfo, ForeignKeyInfo> GetForeignKeyProperty { get; private set; }
		public bool HasEntityPropertyName() {
			return !string.IsNullOrEmpty(EntityPropertyName);
		}
		public bool HasEntityEditProperty() {
			return Commands != null && Commands.Any(x => x.CommandPropertyName == "EditCommand" && !string.IsNullOrEmpty(x.ParameterPropertyName));
		}
		public string CollectionPropertyName { get; private set; }
		public bool UseIsLoadingBinding { get; private set; }
		public override ViewModelType ViewModelType {
			get {
				return ViewModelType.EntityRepository;
			}
		}
	}
	public class LookUpCollectionViewModelData : CollectionViewModelData {
		public LookUpCollectionViewModelData(bool useProxyFactory,
			string assemblyName,
			bool isSolutionType,
			string name,
			string nameSpace,
			bool isLocalType,
			bool supportServices,
			Type entityType,
			string entityPropertyName,
			string collectionPropertyName,
			Func<ImageType, CommandInfo[]> createCommands,
			IEnumerable<IEdmPropertyInfo> scaffoldingProperties,
			IEnumerable<TypeNamePropertyPair> collectionProperties,
			bool activated,
			string lookUpCollectionPropertyAssociationName,
			string lookUpCollectionPropertyName,
			XamlNamespaceDeclaration nameSpaceDeclaration)
			: base(useProxyFactory, assemblyName, isSolutionType, name, nameSpace, isLocalType, supportServices, entityType.Name, entityType, entityPropertyName, collectionPropertyName, createCommands, scaffoldingProperties, collectionProperties, activated, nameSpaceDeclaration, true, null,null) {
			this.LookUpCollectionPropertyAssociationName = lookUpCollectionPropertyAssociationName;
			this.LookUpCollectionPropertyName = lookUpCollectionPropertyName;
		}
		public string LookUpCollectionPropertyAssociationName { get; private set; }
		public string LookUpCollectionPropertyName { get; private set; }
	}
}
