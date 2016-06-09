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
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class DefinedNameToCopyAnalizer {
		Worksheet source;
		Worksheet target;
		Dictionary<string, DefinedNameBase> sourceDefinedNamesToCopy;
		DefinedNameBase conflictedDefinedName;
		public DefinedNameToCopyAnalizer(Worksheet source, Worksheet target) {
			Guard.ArgumentNotNull(source, "source");
			Guard.ArgumentNotNull(target, "target");
			this.source = source;
			this.target = target;
			sourceDefinedNamesToCopy = new Dictionary<string, DefinedNameBase>();
			conflictedDefinedName = null;
		}
		Worksheet SourceWorksheet { get { return source; } }
		Worksheet TargetWorksheet { get { return target; } }
		public Dictionary<string, DefinedNameBase> SourceDefinedNamesToCopy { get { return sourceDefinedNamesToCopy; } }
		public DefinedNameBase ConflictedDefinedName { get { return conflictedDefinedName; } }
		public void Process(IEnumerable<string> sourceDefinedNamesUnique) {
			foreach (string definedName in sourceDefinedNamesUnique) {
				ProcessCore(definedName);
			}
		}
		DefinedNameBase GetDefinedName(string definedName, Worksheet sheet) {
			try {
				sheet.DataContext.PushCurrentWorksheet(sheet);
				return sheet.DataContext.GetDefinedName(definedName);
			}
			finally {
				sheet.DataContext.PopCurrentWorksheet();
			}
		}
		bool RegisterConflict(DefinedNameBase sourceDefinedName) {
			this.conflictedDefinedName = sourceDefinedName;
			return false;
		}
		bool ProcessCore(string definedName) {
			DefinedNameBase sourceDefinedName = GetDefinedName(definedName, SourceWorksheet);
			bool hasLocalSourceDefinedName = false;
			bool hasWBSourceDefinedName = false;
			if (sourceDefinedName != null) {
				if (sourceDefinedName.ScopedSheetId >= 0)
					hasLocalSourceDefinedName = true;
				else
					hasWBSourceDefinedName = true;
			}
			DefinedNameBase targetExistingDefinedName = GetDefinedName(definedName, TargetWorksheet);
			bool hasLocalTargetDefinedName = false;
			bool hasWBTargetDefinedName = false;
			if (targetExistingDefinedName != null) {
				if (targetExistingDefinedName.ScopedSheetId >= 0)
					hasLocalTargetDefinedName = true;
				else
					hasWBTargetDefinedName = true;
			}
			if (!hasLocalSourceDefinedName && !hasWBSourceDefinedName && !hasLocalTargetDefinedName && !hasWBTargetDefinedName) 
				return true; 
			if (!hasLocalSourceDefinedName && !hasWBSourceDefinedName)
				return RegisterConflict(sourceDefinedName);
			if (hasLocalSourceDefinedName) {
				if (hasWBTargetDefinedName) {
					if (hasLocalTargetDefinedName) {
						return RegisterConflict(sourceDefinedName);
					}
					else {
						RegisterConflict(sourceDefinedName);
					}
				}
				else {
					if (hasLocalTargetDefinedName) {
						if (Object.ReferenceEquals(SourceWorksheet, TargetWorksheet))
							return true; 
						return RegisterConflict(sourceDefinedName);
					}
					else {
					}
				}
			}
			else {
				if (hasLocalTargetDefinedName) {
					return RegisterConflict(sourceDefinedName);
				}
				else {
					if (!Object.ReferenceEquals(SourceWorksheet.Workbook, TargetWorksheet.Workbook)) {
					}
					else
						return true;
				}
			}
			if (!sourceDefinedNamesToCopy.ContainsKey(definedName))
				sourceDefinedNamesToCopy.Add(definedName, sourceDefinedName);
			return true;
		}
	}
}
