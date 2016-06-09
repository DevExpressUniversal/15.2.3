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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region DocumentWebSettingsDestination
	public class DocumentWebSettingsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("divs", OnDivsCollection);
			return result;
		}
		static Destination OnDivsCollection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DivsDestination(importer);
		}
		public DocumentWebSettingsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DivsDestination
	public class DivsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("div", OnDiv);
			return result;
		}
		static Destination OnDiv(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DivDestination(importer);
		}
		public DivsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DivDestination
	public class DivDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("bodyDiv", OnBodyDiv);
			result.Add("marLeft", OnLeftMargin);
			result.Add("marRight", OnRightMargin);
			result.Add("marTop", OnTopMargin);
			result.Add("marBottom", OnBottomMargin);
			return result;
		}
		static DivDestination GetThis(WordProcessingMLBaseImporter importer) {
			if (importer.DestinationStack.Count == 0)
				return null;
			return (DivDestination)importer.DestinationStack.Peek();
		}
		static Destination OnBodyDiv(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).IsBodyDiv = value != 0;
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnLeftMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).LeftMargin = value;
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnRightMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).RightMargin = value;
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnTopMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).TopMargin = value;
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnBottomMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).BottomMargin = value;
			return new PropertyDestination<int>(importer, setter);
		}
		bool isBodyDiv;
		int id;
		int leftMargin;
		int rightMargin;
		int topMargin;
		int bottomMargin;
		public DivDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal bool IsBodyDiv { get { return isBodyDiv; } set { isBodyDiv = value; } }
		protected internal int Id { get { return id; } set { id = value; } }
		protected internal int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		protected internal int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		protected internal int TopMargin { get { return topMargin; } set { topMargin = value; } }
		protected internal int BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			this.id = Importer.GetWpSTIntegerValue(reader, "id", -1);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!IsBodyDiv)
				return;
			WebSettings webSettings = DocumentModel.WebSettings;
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			webSettings.LeftMargin = unitConverter.TwipsToModelUnits(LeftMargin);
			webSettings.RightMargin = unitConverter.TwipsToModelUnits(RightMargin);
			webSettings.TopMargin = unitConverter.TwipsToModelUnits(TopMargin);
			webSettings.BottomMargin = unitConverter.TwipsToModelUnits(BottomMargin);
		}
	}
	#endregion
	#region PropertyDestination<T>
	public class PropertyDestination<T> : LeafElementDestination {
		readonly Action<T> setter;
		public PropertyDestination(WordProcessingMLBaseImporter importer, Action<T> setter)
			: base(importer) {
			this.setter = setter;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = value = reader.GetAttribute("val", reader.NamespaceURI); 
			if (value == null)
				value = reader.GetAttribute("val");
			if (String.IsNullOrEmpty(value))
				return;
			try {
				setter((T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture));
			}
			catch {
			}
		}
	}
	#endregion
}
