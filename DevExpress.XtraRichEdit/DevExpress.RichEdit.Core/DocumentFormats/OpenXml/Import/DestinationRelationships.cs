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
using System.Xml;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region RelationshipsDestination
	public class RelationshipsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("Relationship", OnRelation);
			return result;
		}
		readonly OpenXmlRelationCollection relations;
		static RelationshipsDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (RelationshipsDestination)importer.PeekDestination();
		}
		public RelationshipsDestination(WordProcessingMLBaseImporter importer, OpenXmlRelationCollection relations)
			: base(importer) {
			Guard.ArgumentNotNull(relations, "relations");
			this.relations = relations;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRelation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RelationDestination(importer, GetThis(importer).relations);
		}
	}
	#endregion
	#region RelationDestination
	public class RelationDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			return new ElementHandlerTable();
		}
		readonly OpenXmlRelationCollection relations;
		public RelationDestination(WordProcessingMLBaseImporter importer, OpenXmlRelationCollection relations)
			: base(importer) {
			Guard.ArgumentNotNull(relations, "relations");
			this.relations = relations;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			OpenXmlRelation relation = new OpenXmlRelation();
			relation.Id = reader.GetAttribute("Id");
			if (String.IsNullOrEmpty(relation.Id))
				return;
			relation.Type = reader.GetAttribute("Type");
			if (String.IsNullOrEmpty(relation.Type))
				return;
			relation.Target = reader.GetAttribute("Target");
			if (String.IsNullOrEmpty(relation.Target))
				return;
			relation.TargetMode = reader.GetAttribute("TargetMode");
			relations.Add(relation);
		}
	}
	#endregion
}
