#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardWin.Native {
	public struct IntersectionLevelCacheKey {
		readonly string columnDataItemId;
		readonly string rowDataItemId;
		readonly string valueFieldName;
		public IntersectionLevelCacheKey(AxisPoint columnPoint, AxisPoint rowPoint, string valueFieldName) {
			this.columnDataItemId = columnPoint != null ? columnPoint.Level.LevelColumn.Name : String.Empty;
			this.rowDataItemId = rowPoint != null ? rowPoint.Level.LevelColumn.Name : string.Empty;
			this.valueFieldName = valueFieldName;
		}
		public override int GetHashCode() {
			return HashcodeHelper.GetCompositeHashCode(columnDataItemId, rowDataItemId, valueFieldName);
		}
		public override bool Equals(object obj) {
			IntersectionLevelCacheKey key = (IntersectionLevelCacheKey)obj;
			if(columnDataItemId != key.columnDataItemId || rowDataItemId != key.rowDataItemId || valueFieldName != key.valueFieldName)
				return false;
			return true;
		}
	}
}
