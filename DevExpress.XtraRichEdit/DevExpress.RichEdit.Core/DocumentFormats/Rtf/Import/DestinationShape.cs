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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region ShapeDestination
	public class ShapeDestination : ShapeInstanceDestination {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = ShapeDestinationCreateKeywordTable();
		static KeywordTranslatorTable ShapeDestinationCreateKeywordTable() {
			KeywordTranslatorTable table = ShapeInstanceDestination.CreateKeywordTable();
			table.Add("shpinst", OnShapeInstanceKeyword);
			return table;
		}
		#endregion
		static void OnShapeInstanceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ShapeInstanceDestination(importer, ShapeInstanceDestination.GetThis(importer).FloatingObjectImportInfo, ShapeInstanceDestination.GetThis(importer).ShapeProperties);
		}
		public ShapeDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		public ShapeDestination(RtfImporter rtfImporter, RtfFloatingObjectImportInfo floatingObjectImportInfo, RtfShapeProperties properties)
			: base(rtfImporter, floatingObjectImportInfo, properties) {
		}
		protected override void ProcessControlCharCore(char ch) {
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator;
			if (KeywordHT.TryGetValue(keyword, out translator)) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected override void ProcessCharCore(char ch) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			return new ShapeDestination(Importer, FloatingObjectImportInfo, ShapeProperties);
		}
	}
	#endregion
}
