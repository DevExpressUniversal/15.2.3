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
		protected internal virtual void GenerateHeaderFooterContent(HeaderFooterOptions options) {
			HeaderFooterInfo defaultInfo = Workbook.Cache.HeaderFooterInfoCache.DefaultItem;
			if(options.Info.Equals(defaultInfo))
				return;
			WriteShStartElement("headerFooter");
			try {
				if(defaultInfo.AlignWithMargins != options.AlignWithMargins)
					WriteBoolValue("alignWithMargins", options.AlignWithMargins);
				if(defaultInfo.DifferentFirst != options.DifferentFirst)
					WriteBoolValue("differentFirst", options.DifferentFirst);
				if(defaultInfo.DifferentOddEven != options.DifferentOddEven)
					WriteBoolValue("differentOddEven", options.DifferentOddEven);
				if(defaultInfo.ScaleWithDoc != options.ScaleWithDoc)
					WriteBoolValue("scaleWithDoc", options.ScaleWithDoc);
				WriteHeaderFooterItem("oddHeader", options.OddHeader);
				WriteHeaderFooterItem("oddFooter", options.OddFooter);
				WriteHeaderFooterItem("evenHeader", options.EvenHeader);
				WriteHeaderFooterItem("evenFooter", options.EvenFooter);
				WriteHeaderFooterItem("firstHeader", options.FirstHeader);
				WriteHeaderFooterItem("firstFooter", options.FirstFooter);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteHeaderFooterItem(string tag, string value) {
			if(string.IsNullOrEmpty(value))
				return;
			WriteShString(tag, EncodeXmlCharsXML1_0(value), true);
		}
	}
}
