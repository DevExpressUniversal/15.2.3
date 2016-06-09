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

#if OPENDOCUMENT
using DevExpress.Office;
using System;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region DocumentStylesDestination
	public class DocumentStylesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("automatic-styles", OnAutoStyles);
			result.Add("font-face-decls", OnFontFaceDecls);
			result.Add("styles", OnStyles);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public DocumentStylesDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnAutoStyles(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new AutoStylesDestination(importer);
		}
		static Destination OnFontFaceDecls(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new FontFaceDeclsDestination(importer);
		}
		static Destination OnStyles(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new StylesDestination(importer);
		}
		#endregion
	}
	#endregion
	#region StylesDestinationBase (abstract class)
	public abstract class StylesDestinationBase : ElementDestination {
		#region Static
		protected static void AddCommonHandlers(ElementHandlerTable table) {
			AddDataStylesHandlers(table);
		}
		static void AddDataStylesHandlers(ElementHandlerTable table) {
			table.Add("boolean-style", OnBooleanDataStyle);
			table.Add("currency-style", OnCurrencyDataStyle);
			table.Add("date-style", OnDateDataStyle);
			table.Add("number-style", OnNumberDataStyle);
			table.Add("percentage-style", OnPercentageDataStyle);
			table.Add("text-style", OnTextDataStyle);
			table.Add("time-style", OnTimeDataStyle);
		}
		#endregion
		protected StylesDestinationBase(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnBooleanDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new BooleanDataStyleDestination(importer);
		}
		static Destination OnCurrencyDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new CurrencyDataStyleDestination(importer);
		}
		static Destination OnDateDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new DateDataStyleDestination(importer);
		}
		static Destination OnNumberDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new NumberDataStyleDestination(importer);
		}
		static Destination OnPercentageDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new PercentageDataStyleDestination(importer);
		}
		static Destination OnTextDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TextDataStyleDestination(importer);
		}
		static Destination OnTimeDataStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TimeDataStyleDestination(importer);
		}
		#endregion
	}
	#endregion
	#region AutoStylesDestination
	public class AutoStylesDestination : StylesDestinationBase {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("style", OnStyle);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public AutoStylesDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new AutoStyleDestination(importer);
		}
		#endregion
	}
	#endregion
	#region StylesDestination
	public class StylesDestination : StylesDestinationBase {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			AddCommonHandlers(result);
			result.Add("default-style", OnDefaultStyle);
			result.Add("style", OnStyle);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public StylesDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new StyleDestination(importer);
		}
		static Destination OnDefaultStyle(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new DefaultStyleDestination(importer);
		}
		#endregion
	}
	#endregion
}
#endif
