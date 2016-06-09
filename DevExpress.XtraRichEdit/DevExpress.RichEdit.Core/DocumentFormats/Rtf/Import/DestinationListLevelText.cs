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
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region ListLevelTextDestination
	public class ListLevelTextDestination : StringValueDestination {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateListLevelTextKeywordTable();
		static KeywordTranslatorTable CreateListLevelTextKeywordTable() {
			KeywordTranslatorTable table = StringValueDestination.CreateKeywordTable();
			table.Add("leveltemplateid", OnListLevelTemplateIdKeyword);
			return table;
		}
		#endregion
		int levelTemplateId;
		public ListLevelTextDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		public int LevelTemplateId { get { return levelTemplateId; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new ListLevelTextDestination(Importer);
		}
		protected override DestinationBase CreateClone() {
			ListLevelTextDestination clone = (ListLevelTextDestination)base.CreateClone();
			clone.levelTemplateId = levelTemplateId;
			return clone;
		}
		static void OnListLevelTemplateIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			((ListLevelTextDestination)importer.Destination).levelTemplateId = hasParameter ? parameterValue : 0;
		}
	}
	#endregion
}
