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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	public delegate Destination ElementHandler(WordProcessingMLBaseImporter importer, XmlReader reader);
	#region ElementHandlerTable
	public class ElementHandlerTable : Dictionary<string, ElementHandler> {
		static readonly ElementHandlerTable empty = new ElementHandlerTable();
		public static ElementHandlerTable Empty { get { return empty; } }
	}
	#endregion
	#region ElementDestination (abstract class)
	public abstract class ElementDestination : Destination {
		protected ElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		internal virtual new WordProcessingMLBaseImporter Importer { get { return (WordProcessingMLBaseImporter)base.Importer; } }
		protected internal DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		protected internal PieceTable PieceTable { get { return Importer.PieceTable; } }
		protected internal abstract ElementHandlerTable ElementHandlerTable { get; }
		protected internal DocumentModelUnitConverter UnitConverter { get { return Importer.UnitConverter; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		public override bool ProcessText(XmlReader reader) {
			return true;
		}
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			string localName = reader.LocalName;
			if (localName == "AlternateContent")
				return OnAlternateContent();
			ElementHandler handler;
			if (ElementHandlerTable.TryGetValue(localName, out handler))
				return handler(Importer, reader);
			else
				return null;
		}
		Destination OnAlternateContent() {
			return new AlternateContentDestination(Importer, this);
		}
		public virtual bool IsChoiceNamespaceSupported(string requeriesNamespaceUri) {
			return false;
		}
	}
	#endregion
	#region AlternateContentDestination
	public class AlternateContentDestination : ElementDestination {
		ElementDestination parentDestination;
		bool hasProcessedChoice;
		protected internal override ElementHandlerTable ElementHandlerTable { get { Exceptions.ThrowInternalException(); return null; } }
		public AlternateContentDestination(WordProcessingMLBaseImporter importer, ElementDestination parentDestination)
			: base(importer) {
				this.parentDestination = parentDestination;
		}
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			string localName = reader.LocalName;
			if (localName == "Choice" && !hasProcessedChoice) {
				string requeries = reader.GetAttribute("Requires");
				string namespaceUri = reader.LookupNamespace(requeries);
				if (parentDestination.IsChoiceNamespaceSupported(namespaceUri)) {
					hasProcessedChoice = true;
					return parentDestination;
				}
			}
			else if (localName == "Fallback" && !hasProcessedChoice)
				return parentDestination;
			return new EmptyDestination(Importer);
		}
	}
	#endregion
	#region LeafElementDestination (abstract class)
	public abstract class LeafElementDestination : ElementDestination {
		protected LeafElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
	#region EmptyDestination
	public class EmptyDestination : LeafElementDestination {
		public EmptyDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
}
