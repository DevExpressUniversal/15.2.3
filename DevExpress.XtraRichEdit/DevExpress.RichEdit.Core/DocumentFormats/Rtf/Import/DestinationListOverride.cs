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

using DevExpress.Compatibility.System;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RtfListOverrideId
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct RtfListOverrideId  {
		int m_value;
		internal RtfListOverrideId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is RtfListOverrideId) && (this.m_value == ((RtfListOverrideId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(RtfListOverrideId id1, RtfListOverrideId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(RtfListOverrideId id1, RtfListOverrideId id2) {
			return id1.m_value != id2.m_value;
		}
		public static explicit operator int(RtfListOverrideId id) {
			return id.m_value;
		}
	}
	#endregion
	#region RtfNumberingListOverride
	public class RtfNumberingListOverride {
		#region Fields
		RtfListOverrideId id;
		RtfListId listId;
		List<RtfListOverrideLevel> levels = new List<RtfListOverrideLevel>();
		#endregion
		#region Properties
		public RtfListOverrideId Id { get { return id; } set { id = value; } }
		public RtfListId ListId { get { return listId; } set { listId = value; } }
		public List<RtfListOverrideLevel> Levels { get { return levels; } }
		#endregion
	}
	#endregion
	#region ListOverrideTable
	public class ListOverrideTable : List<RtfNumberingListOverride> {
	}
	#endregion
	#region ListOverrideTableDestination
	public class ListOverrideTableDestination : DestinationBase {
		#region Fields
		RtfNumberingListOverride currentOverride;
		#endregion
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("listoverride", OnListOverrideKeyword);
			table.Add("listid", OnListOverrideListIdKeyword);
			table.Add("listoverridecount", OnListOverrideCountKeyword);
			table.Add("ls", OnListOverrideIdKeyword);
			table.Add("lfolevel", OnListOverrideLevelKeyword);
			return table;
		}
		#endregion
		#region Properties
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public RtfNumberingListOverride CurrentOverride { get { return currentOverride; } set { currentOverride = value; } }
		#endregion
		public ListOverrideTableDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		#region Keyword handlers
		static void OnListOverrideKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideTableDestination destination = (ListOverrideTableDestination)importer.Destination;
			destination.CurrentOverride = new RtfNumberingListOverride();
			importer.DocumentProperties.ListOverrideTable.Add(destination.CurrentOverride);
		}
		static void OnListOverrideListIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideTableDestination destination = (ListOverrideTableDestination)importer.Destination;
			if (destination.CurrentOverride != null && hasParameter)
				destination.CurrentOverride.ListId = new RtfListId(parameterValue);
		}
		static void OnListOverrideCountKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListOverrideIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideTableDestination destination = (ListOverrideTableDestination)importer.Destination;
			if (destination.CurrentOverride != null && hasParameter)
				destination.CurrentOverride.Id = new RtfListOverrideId(parameterValue);
		}
		static void OnListOverrideLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListOverrideTableDestination destination = (ListOverrideTableDestination)importer.Destination;
			ListOverrideLevelDestination newDestination = new ListOverrideLevelDestination(importer);
			importer.Destination = newDestination;
			if (destination.CurrentOverride != null)
				destination.CurrentOverride.Levels.Add(newDestination.OverrideLevel);
		}
		#endregion
		protected override DestinationBase CreateClone() {
			ListOverrideTableDestination clone = new ListOverrideTableDestination(Importer);
			clone.CurrentOverride = this.CurrentOverride;
			return clone;
		}
	}
	#endregion
}
