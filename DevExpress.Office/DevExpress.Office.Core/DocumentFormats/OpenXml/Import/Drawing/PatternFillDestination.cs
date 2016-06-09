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

using System.Collections.Generic;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Import.OpenXml {
	public class DrawingPatternFillDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		static readonly Dictionary<string, DrawingPatternType> patternTypeTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporterBase.DrawingPatternTypeTable);
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("bgClr", OnBackgroundColor);
			result.Add("fgClr", OnForegroundColor);
			return result;
		}
		static DrawingPatternFillDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingPatternFillDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingPatternFill patternFill;
		public DrawingPatternFillDestination(DestinationAndXmlBasedImporter importer, DrawingPatternFill patternFill)
			: base(importer) {
			this.patternFill = patternFill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnBackgroundColor(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetThis(importer).patternFill.BackgroundColor);
		}
		static Destination OnForegroundColor(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetThis(importer).patternFill.ForegroundColor);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			patternFill.PatternType = Importer.GetWpEnumValue(reader, "prst", patternTypeTable, DrawingPatternType.Percent5);
		}
	}
}
