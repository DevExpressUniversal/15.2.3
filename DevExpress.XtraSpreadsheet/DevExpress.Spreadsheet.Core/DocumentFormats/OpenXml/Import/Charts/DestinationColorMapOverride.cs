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
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class ColorMapOverrideDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		static readonly Dictionary<string, ColorSchemeIndex> colorSchemeIndexTable = CreateTranslationTable();
		static Dictionary<string, ColorSchemeIndex> CreateTranslationTable() {
			Dictionary<string, ColorSchemeIndex> result = new Dictionary<string, ColorSchemeIndex>();
			result.Add("accent1", ColorSchemeIndex.Accent1);
			result.Add("accent2", ColorSchemeIndex.Accent2);
			result.Add("accent3", ColorSchemeIndex.Accent3);
			result.Add("accent4", ColorSchemeIndex.Accent4);
			result.Add("accent5", ColorSchemeIndex.Accent5);
			result.Add("accent6", ColorSchemeIndex.Accent6);
			result.Add("dk1", ColorSchemeIndex.Dark1);
			result.Add("dk2", ColorSchemeIndex.Dark2);
			result.Add("folHlink", ColorSchemeIndex.FollowedHyperlink);
			result.Add("hlink", ColorSchemeIndex.Hyperlink);
			result.Add("lt1", ColorSchemeIndex.Light1);
			result.Add("lt2", ColorSchemeIndex.Light2);
			return result;
		}
		#endregion
		ColorMapOverride colorMapOverride;
		public ColorMapOverrideDestination(SpreadsheetMLBaseImporter importer, ColorMapOverride colorMapOverride)
			: base(importer) {
			Guard.ArgumentNotNull(colorMapOverride, "colorMapOverride");
			this.colorMapOverride = colorMapOverride;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			colorMapOverride.Accent1 = Importer.GetWpEnumValue(reader, "accent1", colorSchemeIndexTable, ColorSchemeIndex.Accent1);
			colorMapOverride.Accent2 = Importer.GetWpEnumValue(reader, "accent2", colorSchemeIndexTable, ColorSchemeIndex.Accent2);
			colorMapOverride.Accent3 = Importer.GetWpEnumValue(reader, "accent3", colorSchemeIndexTable, ColorSchemeIndex.Accent3);
			colorMapOverride.Accent4 = Importer.GetWpEnumValue(reader, "accent4", colorSchemeIndexTable, ColorSchemeIndex.Accent4);
			colorMapOverride.Accent5 = Importer.GetWpEnumValue(reader, "accent5", colorSchemeIndexTable, ColorSchemeIndex.Accent5);
			colorMapOverride.Accent6 = Importer.GetWpEnumValue(reader, "accent6", colorSchemeIndexTable, ColorSchemeIndex.Accent6);
			colorMapOverride.Background1 = Importer.GetWpEnumValue(reader, "bg1", colorSchemeIndexTable, ColorSchemeIndex.Light1);
			colorMapOverride.Background2 = Importer.GetWpEnumValue(reader, "bg2", colorSchemeIndexTable, ColorSchemeIndex.Light2);
			colorMapOverride.Text1 = Importer.GetWpEnumValue(reader, "tx1", colorSchemeIndexTable, ColorSchemeIndex.Dark1);
			colorMapOverride.Text2 = Importer.GetWpEnumValue(reader, "tx2", colorSchemeIndexTable, ColorSchemeIndex.Dark2);
			colorMapOverride.Hyperlink = Importer.GetWpEnumValue(reader, "hlink", colorSchemeIndexTable, ColorSchemeIndex.Hyperlink);
			colorMapOverride.FollowedHyperlink = Importer.GetWpEnumValue(reader, "folHlink", colorSchemeIndexTable, ColorSchemeIndex.FollowedHyperlink);
			colorMapOverride.IsDefined = true;
		}
	}
}
