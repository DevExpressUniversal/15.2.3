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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Updating {
	public class ConvertXmlParameters : IDisposable {
		private ModelNode node;
		private Type nodeType;
		private string xmlNodeName;
		private IDictionary<string, string> values;
		private IModelNode subNode;
		public ModelNode Node {
			get { return node; }
		}
		public Type NodeType {
			get { return nodeType; }
			set { nodeType = value; }
		}
		public string XmlNodeName {
			get { return xmlNodeName; }
		}
		public IDictionary<string, string> Values {
			get { return values; }
		}
		public IModelNode SubNode {
			get { return subNode; }
			set { subNode = value; }
		}
		public bool ContainsKey(string key) {
			return GetRealKey(key) != null;
		}
		public string GetRealKey(string key) {
			foreach(string dictionaryKey in values.Keys) {
				if(dictionaryKey.ToLowerInvariant() == key.ToLowerInvariant()) {
					return dictionaryKey;
				}
			}
			return null;
		}
		public void Assign(ModelNode node, Type nodeType, string xmlNodeName, IDictionary<string, string> values) {
			this.node = node;
			this.nodeType = nodeType;
			this.xmlNodeName = xmlNodeName;
			this.values = values;
			this.subNode = null;
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			node = null;
			nodeType = null;
			if(values != null) {
				values.Clear();
				values = null;
			}
			subNode = null;
		}
		#endregion
	}
	public interface IModelXmlConverter {
		void ConvertXml(ConvertXmlParameters parameters);
	}
}
