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
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public abstract class ViewModelDataBase : ViewModelInfoBase, IViewModelInfo {
		readonly IEnumerable<IEdmPropertyInfo> scaffoldingProperties;
		readonly IEnumerable<TypeNamePropertyPair> collectionProperties;
		protected ViewModelDataBase(bool useProxyFactory,
			string assemblyName,
			bool isSolutionType,
			string name,
			string nameSpace,
			bool isLocalType,
			bool supportServices,
			string defaultViewFolderName,
			Type entityType,
			string entityPropertyName,
			Func<ImageType, CommandInfo[]> createCommands,
			IEnumerable<IEdmPropertyInfo> scaffoldingProperties,
			IEnumerable<TypeNamePropertyPair> collectionProperties,
			bool activated,
			XamlNamespaceDeclaration xamlNamespace)
			: base(name, nameSpace, assemblyName, isSolutionType, isLocalType) 
		{
			this.scaffoldingProperties = scaffoldingProperties;
			this.collectionProperties = collectionProperties;
			SupportServices = supportServices;
			this.entityType = entityType;
			this.createCommands = createCommands;
			DefaultViewFolderName = defaultViewFolderName;
			EntityPropertyName = entityPropertyName;
			this.UseProxyFactory = useProxyFactory;
			this.Activated = activated;
			this.XamlNamespace = xamlNamespace;
		}
		readonly Type entityType;
		Func<ImageType, CommandInfo[]> createCommands;
		public string EntityPropertyName { get; private set; }
		public CommandInfo[] Commands { get; private set; }
		public CommandInfo[] LayoutCommands {
			get {
				return Commands.Where(ci => new[] { "SaveLayoutCommand", "ResetLayoutCommand" }.Contains(ci.CommandPropertyName)).ToArray();
			}
		}
		public CommandInfo[] NonLayoutCommands { get { return Commands.Except(LayoutCommands).ToArray(); } }
		public string DefaultViewFolderName { get; private set; }
		public string EntityTypeName { get { return entityType.With(x => x.Name); } }
		public string EntityTypeFullName { get { return entityType.With(x => x.FullName); } }
		public bool SupportServices { get; private set; }
		public bool UseProxyFactory { get; private set; }
		public void CreateCommands(ImageType imageType) {
			Commands = createCommands(imageType);
		}
		public IEnumerable<IEdmPropertyInfo> GetScaffoldingProperties() {
			return scaffoldingProperties;
		}
		public IEnumerable<TypeNamePropertyPair> GetCollectionProperties() {
			return collectionProperties;
		}
		public virtual ViewModelType ViewModelType { get { return ViewModelType.Entity; } }
		public XamlNamespaceDeclaration XamlNamespace {
			get; private set;
		}
		public bool Activated { get; private set; }
		public Type EntityType {
			get { return entityType; }
		}
	}
}
