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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
namespace DevExpress.Design.SmartTags {
	public interface IMarkupAccessService2012 : IMarkupAccessService {
		ISceneNodeModelItem GetModelItem(IModelItem modelItem);
	}
	public interface IDocumentLocator {
		string Path { get; }
		string RelativePath { get; }
	}
	public interface IDocumentContext {
		string DocumentUrl { get; }
		IDocumentLocator DocumentLocator { get; }
	}
	public interface ISceneDocumentNode {
		ISceneDocumentNode Parent { get; }
		ISceneNode SceneNode { get; }
		IVS2012TypeMetadata Type { get; }
		IVS2012PropertyMetadata SitePropertyKey { get; }
		IEnumerable<ISceneDocumentNode> Properties { get; }
	}
	public interface ISceneNode {
		ISceneNodeModelItem ModelItem { get; }
		ISceneDocumentNode DocumentNode { get; }
		IProjectContext ProjectContext { get; }
		IDocumentContext DocumentContext { get; }
	}
	public interface IXamlSceneNode : ISceneNode {
	}
	public interface ISceneNodeModelItem {
		IModelItem ModelItem { get; }
		ISceneNode SceneNode { get; }
		IXamlExpressionSerializer XamlExpressionSerializer { get; }
	}
	public interface IDesignerContext {
		IServiceContainer Services { get; }
		IResourceManager ResourceManager { get; }
	}
	public interface IResourceManager {
		IEnumerable<ISceneDocumentNode> GetResourcesInElementsScope(ISceneNode element, IVS2012TypeMetadata type, bool includeApplicationResources, bool uniqueKeysOnly);
	}
	public interface IXamlExpressionSerializer {
		string GetStringFromExpression(ISceneDocumentNode expression, ISceneDocumentNode parentNode);
	}
	public interface IProjectRoot {
	}
	public interface IProjectContext : IProjectRoot, IDisposable {
		IVS2012TypeResolver Metadata { get; }
		string ProjectRoot { get; }
		IProjectDocument Application { get; }
		IDesignerContext DesignerContext { get; }
	}
	public interface IProjectDocument {
		IDocumentRoot DocumentRoot { get; }
	}
	public interface IDocumentRoot : IDisposable {
		ISceneDocumentNode RootNode { get; }
	}
}
