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

using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Services {
	#region IPivotTableNameCreationService
	public interface IPivotTableNameCreationService {
		string Prefix { get; set; }
		string GetNewName(IList<string> existingNames);
	}
	#endregion
	#region IPivotCacheFieldNameCreationService
	public interface IPivotCacheFieldNameCreationService {
		string GetNewName(string name, IList<string> existingNames);
	}
	#endregion
	#region IPivotDataFieldNameCreationService
	public interface IPivotDataFieldNameCreationService {
		string GetUniqueName(string name, IList<string> existingNames);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region PivotTableNameCreationService
	public class PivotTableNameCreationService : IPivotTableNameCreationService {
		string currentPrefix;
		public PivotTableNameCreationService() {
			this.currentPrefix = "PivotTable";
		}
		public string Prefix { get { return currentPrefix; } set { currentPrefix = value; } }
		public string GetNewName(IList<string> existingNames) {
			return NextObjectNameGenerator.GetNameForNextObject(Prefix, existingNames);
		}
	}
	#endregion
	#region PivotCacheFieldNameCreationService
	public class PivotCacheFieldNameCreationService : IPivotCacheFieldNameCreationService {
		public string GetNewName(string name, IList<string> existingNames) {
			return NextObjectNameGenerator.GetUniqueName(name, existingNames, false);
		}
	}
	#endregion
	#region PivotDataFieldNameCreationService
	public class PivotDataFieldNameCreationService : IPivotDataFieldNameCreationService {
		public string GetUniqueName(string name, IList<string> existingNames) {
			return NextObjectNameGenerator.GetUniqueName(name, existingNames, true);
		}
	}
	#endregion
}
