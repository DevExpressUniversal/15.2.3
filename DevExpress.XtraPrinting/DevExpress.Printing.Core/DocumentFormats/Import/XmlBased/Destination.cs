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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.Office {
	#region Destination (abstract class)
	public abstract class Destination {
		readonly IDestinationImporter importer;
		protected Destination(IDestinationImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		public IDestinationImporter Importer { get { return importer; } }
		public abstract void ProcessElementOpen(XmlReader reader);
		public abstract bool ProcessText(XmlReader reader);
		public abstract void ProcessElementClose(XmlReader reader);
		public bool Process(XmlReader reader) {
			for (; ; ) {
				if (ProcessCore(reader))
					return true;
				if (reader.ReadState == ReadState.EndOfFile)
					return false;
			}
		}
		protected abstract Destination ProcessCurrentElement(XmlReader reader);
		internal Destination ProcessCurrentElementInternal(XmlReader reader) {
			return ProcessCurrentElement(reader);
		}
		protected internal virtual bool ProcessCore(XmlReader reader) {
			if (reader.NodeType == XmlNodeType.EndElement) {
				Destination destination = Importer.PopDestination();
				destination.ProcessElementClose(reader);
				return true;
			}
			if (ShouldProcessWhitespaces(reader)) {
				return Importer.PeekDestination().ProcessText(reader);
			}
			else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.SignificantWhitespace || reader.NodeType == XmlNodeType.CDATA) {
				return Importer.PeekDestination().ProcessText(reader);
			}
			Destination nextDestination = ProcessCurrentElement(reader);
			if (nextDestination == null) {
				reader.Skip();
				return false;
			}
			if (reader.NodeType == XmlNodeType.Element) {
				if (reader.IsEmptyElement) {
					nextDestination.ProcessElementOpen(reader);
					nextDestination.ProcessElementClose(reader);
				}
				else {
					Importer.PushDestination(nextDestination);
					nextDestination.ProcessElementOpen(reader);
				}
			}
			return true;
		}
		public virtual Destination Peek() {
			return this;
		}
		public virtual bool ShouldProcessWhitespaces(XmlReader reader) {
			return false;
		}
	}
	#endregion
	#region ElementDestination<T> (abstract class)
	public abstract class ElementDestination<T> : Destination where T : IDestinationImporter {
		protected ElementDestination(T importer)
			: base(importer) {
		}
		protected internal virtual new T Importer { get { return (T)base.Importer; } }
		protected internal abstract ElementHandlerTable<T> ElementHandlerTable { get; }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		public override bool ProcessText(XmlReader reader) {
			return true;
		}
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			string localName = reader.LocalName;
			ElementHandler<T> handler;
			if (ElementHandlerTable.TryGetValue(localName, out handler))
				return handler(Importer, reader);
			else
				return null;
		}
	}
	#endregion
	#region LeafElementDestination<T> (abstract class)
	public abstract class LeafElementDestination<T> : ElementDestination<T> where T : IDestinationImporter {
		static readonly ElementHandlerTable<T> handlerTable = new ElementHandlerTable<T>();
		protected LeafElementDestination(T importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<T> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region EmptyDestination<T>
	public class EmptyDestination<T> : LeafElementDestination<T> where T : IDestinationImporter {
		public EmptyDestination(T importer)
			: base(importer) {
		}
	}
	#endregion
	#region ElementHandlerTable<T>
	public class ElementHandlerTable<T> : Dictionary<string, ElementHandler<T>> where T : IDestinationImporter {
	}
	#endregion
	public delegate Destination ElementHandler<T>(T importer, XmlReader reader) where T : IDestinationImporter;
}
