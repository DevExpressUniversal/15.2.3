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
using System.Linq;
using System.Text;
using FieldPathService = DevExpress.Snap.Core.Native.Data.Implementations.FieldPathService;
namespace DevExpress.Snap.Core.Native.Data {
	public class DataMemberInfo {
		static readonly DataMemberInfo EmptyDataMemberInfo = CreateEmpty();
		static DataMemberInfo CreateEmpty() {
			DataMemberInfo result = new DataMemberInfo(String.Empty, null, String.Empty);
			result.ParentDataMemberInfo = result;
			return result;
		}
		public static DataMemberInfo Create(string[] escDataPaths) {
			if (escDataPaths == null || escDataPaths.Length == 0)
				return EmptyDataMemberInfo;
			if (escDataPaths.Length == 1)
				return new DataMemberInfo(escDataPaths[0], EmptyDataMemberInfo, escDataPaths[0]);
			string[] parentDataPaths = new string[escDataPaths.Length - 1];
			Array.Copy(escDataPaths, 0, parentDataPaths, 0, escDataPaths.Length - 1);
			return new DataMemberInfo(String.Join(".", escDataPaths), DataMemberInfo.Create(parentDataPaths), escDataPaths[escDataPaths.Length - 1]);
		}
		DataMemberInfo(string dataMember, DataMemberInfo parentDataMemberInfo, string columnName) {
			DataMember = dataMember;
			ParentDataMemberInfo = parentDataMemberInfo;
			ColumnName = columnName;
		}
		public string DataMember { get; private set; }
		public DataMemberInfo ParentDataMemberInfo { get; private set; }
		public string ColumnName { get; private set; }
	}
}
