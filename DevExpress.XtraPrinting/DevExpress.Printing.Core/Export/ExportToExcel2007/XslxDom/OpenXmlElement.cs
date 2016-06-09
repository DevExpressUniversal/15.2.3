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
using System.Text;
using System.Xml;
namespace DevExpress.XtraExport {
	public abstract class OpenXmlElement : XmlElement {
		public OpenXmlElement(XmlDocument document)
			: base(document.Prefix, "default", document.NamespaceURI, document) {
		}
		protected virtual bool IsCollectChildsCount { get { return false; } }
		void CollectChildsCount() {
			if(IsCollectChildsCount)
				XlsxHelper.AppendAttribute(this, "count", ChildNodes.Count.ToString());
		}
		protected virtual void CollectDataBeforeWrite() {
			CollectChildsCount();
		}
		public override void WriteTo(XmlWriter writer) {
			CollectDataBeforeWrite();
			base.WriteTo(writer);
		}
	}
	public abstract class CachedXmlElement : OpenXmlElement {
		Dictionary<object, OpenXmlElement> keyToNode;
		public CachedXmlElement(XmlDocument document)
			: base(document) {
		}
		public OpenXmlElement GetNodeFromCache(object key) {
			if(keyToNode != null) {
				OpenXmlElement node;
				keyToNode.TryGetValue(key, out node);
				return node;
			}
			return null;
		}
		public void AddNodeToCache(object key, OpenXmlElement node) {
			if(keyToNode == null)
				keyToNode = new Dictionary<object, OpenXmlElement>();
			keyToNode.Add(key, node);
		}
	}
}
