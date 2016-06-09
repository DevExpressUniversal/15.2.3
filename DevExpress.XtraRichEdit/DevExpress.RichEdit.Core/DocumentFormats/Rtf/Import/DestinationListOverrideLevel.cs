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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RtfListOverrideLevel
	public class RtfListOverrideLevel {
		#region Fields
		RtfListLevel level;
		int startAt = Int32.MinValue;
		bool overrideStartAt;
		bool overrideFormat;
		#endregion
		public RtfListOverrideLevel(DocumentModel documentModel) {
			this.level = new RtfListLevel(documentModel);
		}
		#region Properties
		public RtfListLevel Level { get { return level; } set { level = value; } }
		public int StartAt { get { return startAt; } set { startAt = value; } }
		public bool OverrideStartAt { get { return overrideStartAt; } set { overrideStartAt = value; } }
		public bool OverrideFormat { get { return overrideFormat; } set { overrideFormat = value; } }
		#endregion
	}
	#endregion
	#region ListOverrideLevelDestination
	public class ListOverrideLevelDestination : DestinationBase {
		RtfListOverrideLevel overrideLevel;
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("listoverrideformat", OnListOverrideFormatKeyword);
			table.Add("listoverridestartat", OnListOverrideStartAtKeyword);
			table.Add("levelstartat", OnListOverrideStartAtValueKeyword);
			table.Add("listlevel", OnListOverrideListLevelKeyword);
			return table;
		}
		#endregion
		public ListOverrideLevelDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
			this.overrideLevel = new RtfListOverrideLevel(rtfImporter.DocumentModel);
		}
		#region Properties
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public RtfListOverrideLevel OverrideLevel { get { return overrideLevel; } }
		#endregion
		#region Keyword handlers
		static void OnListOverrideFormatKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideLevelDestination destination = (ListOverrideLevelDestination)importer.Destination;
			destination.OverrideLevel.OverrideFormat = true;
		}
		static void OnListOverrideStartAtKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideLevelDestination destination = (ListOverrideLevelDestination)importer.Destination;
			destination.OverrideLevel.OverrideStartAt = true;
		}
		static void OnListOverrideStartAtValueKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideLevelDestination destination = (ListOverrideLevelDestination)importer.Destination;
			if (hasParameter)
				destination.OverrideLevel.StartAt = parameterValue;
		}
		static void OnListOverrideListLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideLevelDestination destination = (ListOverrideLevelDestination)importer.Destination;
			ListLevelDestination newDestination = new ListLevelDestination(importer);
			importer.Destination = newDestination;
			destination.OverrideLevel.Level = newDestination.Level;
		}
		#endregion
		protected override DestinationBase CreateClone() {
			ListOverrideLevelDestination clone = new ListOverrideLevelDestination(Importer);
			clone.overrideLevel = this.overrideLevel;
			return clone;
		}
	}
	#endregion
}
