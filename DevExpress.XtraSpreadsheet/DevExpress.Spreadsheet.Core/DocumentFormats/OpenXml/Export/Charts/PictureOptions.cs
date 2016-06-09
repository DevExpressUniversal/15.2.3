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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		internal static Dictionary<PictureFormat, string> PictureFormatTable = CreatePictureFormatTable();
		static Dictionary<PictureFormat, string> CreatePictureFormatTable() {
			Dictionary<PictureFormat, string> result = new Dictionary<PictureFormat, string>();
			result.Add(PictureFormat.Stack, "stack");
			result.Add(PictureFormat.StackScale, "stackScale");
			result.Add(PictureFormat.Stretch, "stretch");
			return result;
		}
		protected internal void GenerateChartPictureOptions(PictureOptions options) {
			WriteStartElement("pictureOptions", DrawingMLChartNamespace);
			try {
				if(!options.ApplyToFront)
					GenerateChartSimpleBoolAttributeTag("applyToFront", options.ApplyToFront);
				if(!options.ApplyToSides)
					GenerateChartSimpleBoolAttributeTag("applyToSides", options.ApplyToSides);
				if(!options.ApplyToEnd)
					GenerateChartSimpleBoolAttributeTag("applyToEnd", options.ApplyToEnd);
				GenerateChartSimpleStringAttributeTag("pictureFormat", PictureFormatTable[options.PictureFormat]);
				if(options.PictureStackUnit != 1.0)
					GenerateChartSimpleDoubleAttributeTag("pictureStackUnit", options.PictureStackUnit);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
