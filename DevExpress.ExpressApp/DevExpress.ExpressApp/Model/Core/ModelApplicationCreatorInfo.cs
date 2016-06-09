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
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Model.Core {
	public interface IModelApplicationCreatorInfo {
	}
	public abstract class ModelApplicationCreatorInfoBase : IModelApplicationCreatorInfo {
		readonly ModelApplicationCreator applicationCreator;
		readonly Dictionary<string, ModelNodeInfo> nodesInfo;
		readonly Dictionary<Type, ModelNodeInfo> nodesInfoByClassType;
		readonly Dictionary<Type, ModelNodeInfo> nodesInfoByInterfaceType;
		readonly List<IModelNodesNotifierGenerator> notifierGenerators;
		readonly List<ModelNodesGeneratorBase> generators;
		protected ModelApplicationCreatorInfoBase(ModelApplicationCreator applicationCreator) {
			Guard.ArgumentNotNull(applicationCreator, "applicationCreator");
			this.applicationCreator = applicationCreator;
			nodesInfo = new Dictionary<string, ModelNodeInfo>();
			nodesInfoByClassType = new Dictionary<Type, ModelNodeInfo>();
			nodesInfoByInterfaceType = new Dictionary<Type, ModelNodeInfo>();
			notifierGenerators = new List<IModelNodesNotifierGenerator>();
			generators = new List<ModelNodesGeneratorBase>();
		}
		public List<ModelNodesGeneratorBase> Generators { get { return generators; } }
		public List<IModelNodesNotifierGenerator> NotifierGenerators { get { return notifierGenerators; } }
		protected Dictionary<string, ModelNodeInfo> NodesInfo { get { return nodesInfo; } }
		protected Dictionary<Type, ModelNodeInfo> NodesInfoByClassType { get { return nodesInfoByClassType; } }
		protected Dictionary<Type, ModelNodeInfo> NodesInfoByInterfaceType { get { return nodesInfoByInterfaceType; } }
		protected ModelApplicationCreator ApplicationCreator { get { return applicationCreator; } }
		public ModelNodeInfo GetNodeInfo(Type type) {
			Dictionary<Type, ModelNodeInfo> nodesInfo = type.IsInterface ? nodesInfoByInterfaceType : nodesInfoByClassType;
			return GetNodeInfoCore(type, nodesInfo);
		}
		public IEnumerable<ModelNodeInfo> GetNodeInfos(Type nodeType) {
			Dictionary<Type, ModelNodeInfo> nodesInfo = nodeType.IsInterface ? nodesInfoByInterfaceType : nodesInfoByClassType;
			foreach(KeyValuePair<Type, ModelNodeInfo> item in nodesInfo) {
				if(nodeType.IsAssignableFrom(item.Key)) {
					yield return item.Value;
				}
			}
		}
		public ModelNodeInfo GetNodeInfoCore(Type type, Dictionary<Type, ModelNodeInfo> nodesInfo) {
			ModelNodeInfo result;
			nodesInfo.TryGetValue(type, out result);
			return result;
		}
		public Type GetModelType(string className) {
			ModelNodeInfo nodeInfo = GetNodeInfo(className);
			return nodeInfo != null ? nodeInfo.GeneratedClass : null;
		}
		public void ClearValidators() {
			foreach(ModelNodeInfo nodeInfo in NodesInfo.Values) {
				nodeInfo.ClearValidators();
			}
		}
		public void ClearUpdaters() {
			foreach(ModelNodeInfo nodeInfo in NodesInfo.Values) {
				nodeInfo.ClearUpdaters();
			}
		}
		protected ModelNodeInfo GetNodeInfo(string className) {
			ModelNodeInfo result;
			NodesInfo.TryGetValue(className, out result);
			return result;
		}
		protected ModelNodeInfo AddNodeInfo(ModelNodeCreatorMethod nodeCreator, Type generatedClass, Type baseInterface, ModelNodesGeneratorBase nodesGenerator, string keyProperty) {
			ModelNodeInfo nodeInfo = new ModelNodeInfo(ApplicationCreator, nodeCreator, generatedClass, baseInterface, nodesGenerator, keyProperty);
			NodesInfoByClassType[generatedClass] = nodeInfo;
			NodesInfoByInterfaceType[baseInterface] = nodeInfo;
			NodesInfo[generatedClass.Name] = nodeInfo;
			return nodeInfo;
		}
		protected void AddNodeGenerator(ModelNodesGeneratorBase generator) {
			Generators.Add(generator);
			IModelNodesNotifierGenerator notifierGenerator = generator as IModelNodesNotifierGenerator;
			if(notifierGenerator != null && !NotifierGenerators.Contains(notifierGenerator)) {
				NotifierGenerators.Add(notifierGenerator);
			}
		}
	}
}
