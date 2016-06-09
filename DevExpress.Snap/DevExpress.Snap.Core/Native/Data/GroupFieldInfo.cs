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
using DevExpress.Data;
using DevExpress.Snap.Core.API;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Design;
namespace DevExpress.Snap.Core.Native.Data {
	public class GroupFieldInfo {
		string fieldName;
		public GroupFieldInfo() { }
		public GroupFieldInfo(string fieldName) {
			this.fieldName = fieldName;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.Snap.Extensions.Native.FieldNameEditor, " + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
		public string FieldName { get { return fieldName; } set { fieldName = value; } }
		[DefaultValue(ColumnSortOrder.Ascending)]
		public ColumnSortOrder SortOrder { get; set; }
		[DefaultValue(GroupInterval.Default)]
		public GroupInterval GroupInterval { get; set; }
		public static bool operator ==(GroupFieldInfo a, GroupFieldInfo b) {
			if (Object.ReferenceEquals(a, b))
				return true;
			if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
				return false;
			if (a.SortOrder != b.SortOrder)
				return false;
			if (a.GroupInterval != b.GroupInterval)
				return false;
			return String.Compare(a.FieldName, b.FieldName, true, CultureInfo.InvariantCulture) == 0;
		}
		public static bool operator !=(GroupFieldInfo a, GroupFieldInfo b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			GroupFieldInfo other = obj as GroupFieldInfo;
			if (Object.ReferenceEquals(other, null))
				return false;
			return FieldName == other.FieldName && SortOrder == other.SortOrder;
		}
		public override int GetHashCode() {
			return !string.IsNullOrEmpty(FieldName) ? FieldName.GetHashCode() : 0;
		}
	}
}
