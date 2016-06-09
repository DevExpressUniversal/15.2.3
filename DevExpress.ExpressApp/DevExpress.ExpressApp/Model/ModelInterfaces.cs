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
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Model {
	public interface IModelNodesNotifierGenerator {
		bool IsCompatible(ModelNode node);
		void OnPropertyChanged(ModelNode node, string propertyName);
		void OnNodeRemoved(ModelNode node);
		void OnNodeAdded(ModelNode node);
	}
	public interface IModelNode  {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNodeParent")]
#endif
		IModelNode Parent { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNodeRoot")]
#endif
		IModelNode Root { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNodeIndex")]
#endif
		int? Index { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNodeNodeCount")]
#endif
		int NodeCount { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNodeApplication")]
#endif
		IModelApplication Application { get; }
		IModelNode GetNode(int index);
		IModelNode GetNode(string id);
		NodeType AddNode<NodeType>();
		NodeType AddNode<NodeType>(string id);
		void Remove();
		ValueType GetValue<ValueType>(string name);
		void SetValue<ValueType>(string name, ValueType value);
		void ClearValue(string name);
		bool HasValue(string name);
	}
	public interface IModelList<NodeType> : IList<NodeType> {
		NodeType this[string id] { get; }
		IEnumerable<SpecificNodeType> GetNodes<SpecificNodeType>() where SpecificNodeType : NodeType;
		void ClearNodes();
		#region Obsolete 11.2
		[Obsolete(ModelNodeListObsoleteMessages.InsertMethodObsolete, true)]	
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		new void Insert(int index, NodeType item);
		[Obsolete(ModelNodeListObsoleteMessages.AddMethodObsolete, true)]	   
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		new void Add(NodeType item);
		[Obsolete(ModelNodeListObsoleteMessages.RemoveAtMethodObsolete, true)]  
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		new void RemoveAt(int index);
		[Obsolete(ModelNodeListObsoleteMessages.RemoveMethodObsolete, true)]	
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		new bool Remove(NodeType item);
		[Obsolete(ModelNodeListObsoleteMessages.ClearMethodObsolete, true)]	 
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		new void Clear();
		#endregion
	}
	public interface IModelList {
		object this[string id] { get; }
	}
	public interface IModelIndexedNode {
		int? Index { get; set; }
	}
	public interface IModelApplicationServices {
		string CurrentAspect { get; }
		void AddAspect(string aspect);
		int AspectCount { get; }
		string GetAspect(int index);
		void RemoveAspect(string aspect);
		ICurrentAspectProvider CurrentAspectProvider { get; set; }
		void SetSchemaModuleInfos(Dictionary<string, Version> schemaModuleInfos);
		Version GetModuleVersion(string moduleName);
	}
	public interface IModelSources {
		IEnumerable<IXafResourceLocalizer> Localizers { get; set; }
		IEnumerable<Type> BOModelTypes { get; set; }
		IEnumerable<Controller> Controllers { get; set; }
		EditorDescriptors EditorDescriptors { get; set; }
		IEnumerable<ModuleBase> Modules { get; set; }
	}
	public interface IModelIsVisible {
		bool IsVisible(IModelNode node, String propertyName);
	}
	public interface IModelIsReadOnly {
		bool IsReadOnly(IModelNode node, String propertyName);
		bool IsReadOnly(IModelNode node, IModelNode childNode);
	}
	public interface IModelIsRequired {
		Boolean IsRequired(IModelNode node, String propertyName);
	}
}
