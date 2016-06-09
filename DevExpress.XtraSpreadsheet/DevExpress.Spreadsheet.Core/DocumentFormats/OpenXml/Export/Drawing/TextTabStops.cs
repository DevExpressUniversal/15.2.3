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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static Members
		internal static Dictionary<DrawingTextTabAlignmentType, string> DrawingTextTabAlignmentTypeTable = CreateDrawingTextTabAlignmentTypeTable();
		static Dictionary<DrawingTextTabAlignmentType, string> CreateDrawingTextTabAlignmentTypeTable() {
			Dictionary<DrawingTextTabAlignmentType, string> result = new Dictionary<DrawingTextTabAlignmentType, string>();
			result.Add(DrawingTextTabAlignmentType.Center, "ctr");
			result.Add(DrawingTextTabAlignmentType.Decimal, "dec");
			result.Add(DrawingTextTabAlignmentType.Left, "l");
			result.Add(DrawingTextTabAlignmentType.Right, "r");
			return result;
		}
		#endregion
		#region GenerateDrawingTextTabStopListContent
		protected internal void GenerateDrawingTextTabStopListContent(DrawingTextTabStopCollection collection) {
			if (collection.Count == 0)
				return;
			WriteStartElement("tabLst", DrawingMLNamespace);
			try {
				collection.ForEach(GenerateDrawingTextTabStopContent);
			} finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingTextTabStopContent(DrawingTextTabStop tabStop) {
			WriteStartElement("tab", DrawingMLNamespace);
			try {
				WriteIntValue("pos", tabStop.Position, tabStop.HasPosition);
				WriteEnumValue("algn", tabStop.Alignment, DrawingTextTabAlignmentTypeTable, DrawingTextTabAlignmentType.Automatic);
			} finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
