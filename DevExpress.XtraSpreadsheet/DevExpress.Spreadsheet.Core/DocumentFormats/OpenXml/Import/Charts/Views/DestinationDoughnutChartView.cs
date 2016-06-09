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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DoughnutChartViewDestination
	public class DoughnutChartViewDestination : PieChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			result.Add("firstSliceAng", OnFirstSliceAngle);
			result.Add("holeSize", OnHoleSize);
			return result;
		}
		static DoughnutChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DoughnutChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DoughnutChartView view;
		#endregion
		public DoughnutChartViewDestination(SpreadsheetMLBaseImporter importer, DoughnutChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DoughnutChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnFirstSliceAngle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DoughnutChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 360)
						importer.ThrowInvalidFile();
					view.SetFirstSliceAngleCore(value);
				},
				"val",
				0);
		}
		static Destination OnHoleSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DoughnutChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 10 || value > 90)
						importer.ThrowInvalidFile();
					view.SetHoleSizeCore(value);
				},
				"val",
				10);
		}
		#endregion
	}
	#endregion
}
