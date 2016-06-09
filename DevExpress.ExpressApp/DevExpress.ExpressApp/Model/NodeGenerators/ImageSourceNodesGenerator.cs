#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public class ImageSourceNodesGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			IModelImageSources modelImagesSources = ((IModelImageSources)node);
			int counter = 0;
			IModelImageSource newNode = modelImagesSources.AddNode<IModelFileImageSource>("Images");
			newNode.Index = counter++;			
			List<string> assemblyNames = new List<string>();
			if(((IModelSources)node.Application).Modules != null) {
				foreach(ModuleBase currentModule in ((IModelSources)node.Application).Modules) {
					if(assemblyNames.IndexOf(currentModule.AssemblyName) == -1) {
						newNode = modelImagesSources.AddNode<IModelAssemblyResourceImageSource>(currentModule.AssemblyName);
						newNode.Index = counter++;
						assemblyNames.Add(currentModule.AssemblyName);
					}
				}
				newNode = modelImagesSources.AddNode<IModelAssemblyResourceImageSource>("DevExpress.ExpressApp.Images" + XafApplication.CurrentVersion);
				newNode.Index = counter++; 
			}
		}
	}
}
