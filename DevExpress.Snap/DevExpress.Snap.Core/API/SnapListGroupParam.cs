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
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Data;
using System.Drawing.Design;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.Snap.Core.API {
	public struct SnapListGroupParam {
		public string FieldName { get; set; }
		public ColumnSortOrder SortOrder { get; set; }
		public GroupInterval Interval { get; set; }
		public SnapListGroupParam(string fieldName, ColumnSortOrder sortOrder, GroupInterval interval) : this() {
			this.FieldName = fieldName;
			this.SortOrder = sortOrder;
			this.Interval = interval;
		}
		public SnapListGroupParam(string fieldName, ColumnSortOrder sortOrder) : this() {
			this.FieldName = fieldName;
			this.SortOrder = sortOrder;
			this.Interval = GroupInterval.Default;
		}
		public static bool operator ==(SnapListGroupParam a, SnapListGroupParam b) {
			if(a.SortOrder != b.SortOrder || a.Interval != b.Interval)
				return false;
			if(String.IsNullOrEmpty(a.FieldName))
				return String.IsNullOrEmpty(b.FieldName);
			return a.FieldName.Equals(b.FieldName);
		}
		public static bool operator !=(SnapListGroupParam a, SnapListGroupParam b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			if(obj is SnapListGroupParam)
				return (SnapListGroupParam)obj == this;
			return false;
		}
		public override int GetHashCode() {
			int fieldNameCode = !string.IsNullOrEmpty(FieldName) ? FieldName.GetHashCode() : 0;
			return fieldNameCode ^ (((int)Interval << 16) | ((int)Interval >> 16) | ((int)SortOrder));
		}
		public override string ToString() {
			return String.Format("({0}, {1}, {2})", this.FieldName, this.SortOrder, this.Interval);
		}
	}
}
