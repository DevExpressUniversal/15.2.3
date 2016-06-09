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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartTextDestination
	public class ChartTextDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("strRef", OnStringReference);
			result.Add("rich", OnRichText);
			return result;
		}
		static ChartTextDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartTextDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Action<IChartText> setterMethod;
		IChartText chartText;
		OpenXmlStringReference stringReference;
		IChart parent;
		#endregion
		public ChartTextDestination(SpreadsheetMLBaseImporter importer, IChart parent, Action<IChartText> setterMethod)
			: base(importer) {
			this.parent = parent;
			this.setterMethod = setterMethod;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (stringReference != null)
				chartText = stringReference.ToChartTextRef(parent);
			if (chartText == null)
				Importer.ThrowInvalidFile();
			setterMethod(chartText);
		}
		#region Handlers
		static Destination OnStringReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartTextDestination thisDestination = GetThis(importer);
			if (thisDestination.chartText != null || thisDestination.stringReference != null)
				importer.ThrowInvalidFile();
			thisDestination.stringReference = new OpenXmlStringReference();
			return new ChartStringReferenceDestination(importer, thisDestination.stringReference);
		}
		static Destination OnRichText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartTextDestination thisDestination = GetThis(importer);
			if (thisDestination.chartText != null || thisDestination.stringReference != null)
				importer.ThrowInvalidFile();
			ChartRichText chartRichText = new ChartRichText(thisDestination.parent);
			thisDestination.chartText = chartRichText;
			return new TextPropertiesDestination(importer, chartRichText.TextProperties);
		}
		#endregion
	}
	#endregion
	#region RichText
	#endregion
}
