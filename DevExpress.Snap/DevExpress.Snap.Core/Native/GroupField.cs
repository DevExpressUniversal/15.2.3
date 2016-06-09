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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.Snap.Core.Native {
	public class GroupField {
		DesignBinding designBinding;
		public GroupField() { }
		public GroupField(DesignBinding designBinding) {
			Guard.ArgumentNotNull(designBinding, "designBinding");
			this.designBinding = designBinding;
			this.SortOrder = SNSortOrder.Ascending;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.GroupField_FieldName", "Field Name")]
		public DesignBinding FieldName {
			get { return designBinding; }
			set { designBinding = new DesignBinding(designBinding.DataSource, string.IsNullOrEmpty(value.DataMember) ? string.Empty : value.DataMember); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.GroupField_SortOrder", "Sort Order"),
		DefaultValue(SNSortOrder.Ascending)]
		public SNSortOrder SortOrder { get; set; }
		public static bool operator ==(GroupField a, GroupField b) {
			if (Object.ReferenceEquals(a, b))
				return true;
			if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
				return false;
			return EqualsCore(a, b);
		}
		public static bool operator !=(GroupField a, GroupField b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			GroupField value = obj as GroupField;
			if (Object.ReferenceEquals(value, null))
				return false;
			return EqualsCore(this, value);
		}
		static bool EqualsCore(GroupField a, GroupField b) {
			if (a.SortOrder != b.SortOrder)
				return false;
			return a.designBinding.Equals(b.designBinding); 
		}
		public override int GetHashCode() {
			if (Object.ReferenceEquals(this.designBinding, null))
				return (int)SortOrder;
			return (int)SortOrder ^ this.designBinding.GetHashCode();
		}
	}
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(DevExpress.Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum SNSortOrder {
		Ascending = ColumnSortOrder.Ascending,
		Descending = ColumnSortOrder.Descending
	};
}
