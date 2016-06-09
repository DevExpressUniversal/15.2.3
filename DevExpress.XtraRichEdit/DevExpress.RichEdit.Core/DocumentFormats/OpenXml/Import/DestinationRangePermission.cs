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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region RangePermissionElementDestination (abstract class)
	public abstract class RangePermissionElementDestination : LeafElementDestination {
		internal static Dictionary<string, string> actualGroupNames = CreateActualGroupNames();
		private static Dictionary<string, string> CreateActualGroupNames() {
			Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var item in DevExpress.XtraRichEdit.Export.OpenXml.WordProcessingMLBaseExporter.predefinedGroupNames)
				result.Add(item.Value, item.Key);
			return result;
		}
		protected RangePermissionElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
			if (id != null)
				id = id.Trim();
			if (String.IsNullOrEmpty(id))
				return;
			ImportRangePermissionInfo rangePermission;
			if (!Importer.RangePermissions.TryGetValue(id, out rangePermission)) {
				rangePermission = new ImportRangePermissionInfo();
				Importer.RangePermissions.Add(id, rangePermission);
			}
			string value = reader.GetAttribute("ed", Importer.WordProcessingNamespaceConst);
			if (value != null) {
				value = value.Trim();
				rangePermission.PermissionInfo.UserName = value;
			}
			value = reader.GetAttribute("edGrp", Importer.WordProcessingNamespaceConst);
			if (value != null) {
				value = value.Trim();
				rangePermission.PermissionInfo.Group = GetActualGroupName(value);
			}
			AssignRangePermissionPosition(rangePermission);
		}
		string GetActualGroupName(string value) {
			string result;
			if (actualGroupNames.TryGetValue(value, out result))
				return result;
			return value;
		}
		protected internal abstract void AssignRangePermissionPosition(ImportRangePermissionInfo RangePermission);
	}
	#endregion
	#region RangePermissionStartElementDestination
	public class RangePermissionStartElementDestination : RangePermissionElementDestination {
		public RangePermissionStartElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignRangePermissionPosition(ImportRangePermissionInfo rangePermission) {
			rangePermission.Start = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region RangePermissionEndElementDestination
	public class RangePermissionEndElementDestination : RangePermissionElementDestination {
		public RangePermissionEndElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignRangePermissionPosition(ImportRangePermissionInfo rangePermission) {
			rangePermission.End = Importer.Position.LogPosition;
		}
	}
	#endregion
}
