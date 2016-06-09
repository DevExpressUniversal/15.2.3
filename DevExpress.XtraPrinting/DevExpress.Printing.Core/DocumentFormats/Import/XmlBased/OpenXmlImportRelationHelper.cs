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
namespace DevExpress.Office.Utils {
	#region OpenXmlImportRelationHelper
	public static class OpenXmlImportRelationHelper {
		public static string LookupRelationTargetById(OpenXmlRelationCollection relations, string id, string rootFolder, string defaultFileName) {
			OpenXmlRelation relation = relations.LookupRelationById(id);
			return CalculateRelationTarget(relation, rootFolder, defaultFileName);
		}
		public static string LookupRelationTargetByType(OpenXmlRelationCollection relations, string type, string rootFolder, string defaultFileName) {
			OpenXmlRelation relation = relations.LookupRelationByType(type);
			return CalculateRelationTarget(relation, rootFolder, defaultFileName);
		}
		public static string CalculateRelationTarget(OpenXmlRelation relation, string rootFolder, string defaultFileName) {
			if (relation == null) {
				if (string.IsNullOrEmpty(defaultFileName))
					return string.Empty;
				if (String.IsNullOrEmpty(rootFolder))
					return defaultFileName;
				else
					return rootFolder + "/" + defaultFileName;
			}
			if (relation.Target.StartsWith("..", StringComparison.Ordinal)) {
				return rootFolder + relation.Target.Substring(2);
			}
			if (relation.Target.StartsWith("/", StringComparison.Ordinal))
				return relation.Target;
			else {
				if (String.IsNullOrEmpty(rootFolder))
					return relation.Target;
				else
					return rootFolder + "/" + relation.Target;
			}
		}
	}
	#endregion
}
