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
	public class RelationshipNode : OpenXmlElement {
		string id;
		string target;
		string targetType;
		bool external;
		public RelationshipNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "Relationship";
			}
		}
		public string Id {
			get { return id; }
			set { id = value; }
		}
		public string Target {
			get { return target; }
			set { target = value; }
		}
		public string TargetType {
			get { return targetType; }
			set { targetType = value; }
		}
		public bool External {
			get { return external; }
			set { external = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "Id", Id);
			XlsxHelper.AppendAttribute(this, "Type", targetType);
			XlsxHelper.AppendAttribute(this, "Target", target);
			if(external)
				XlsxHelper.AppendAttribute(this, "TargetMode", "External");
		}
	}
	public class RelationshipsNode : CachedXmlElement {
		public RelationshipsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "Relationships"; } }
		string GenerateId() {
			return "rId" + ChildNodes.Count.ToString();
		}
		public RelationshipNode GetRelationshipNodeWithCache(string target, string targetType, bool external) {
			RelationshipNode relationshipNode = (RelationshipNode)GetNodeFromCache(target);
			if (relationshipNode == null) {
				relationshipNode = new RelationshipNode(OwnerDocument);
				AppendChild(relationshipNode);
				AddNodeToCache(target, relationshipNode);
				relationshipNode.Id = GenerateId();
				relationshipNode.Target = target;
				relationshipNode.TargetType = targetType;
				relationshipNode.External = external;
			}			
			return relationshipNode;
		}
		public string AppendRelationshipNode(string target, string targetType, string id, bool external) {
			RelationshipNode relationshipNode = new RelationshipNode(OwnerDocument);
			AppendChild(relationshipNode);
			relationshipNode.Id = id;
			relationshipNode.Target = target;
			relationshipNode.TargetType = targetType;
			relationshipNode.External = external;
			return id;
		}
	}
}
