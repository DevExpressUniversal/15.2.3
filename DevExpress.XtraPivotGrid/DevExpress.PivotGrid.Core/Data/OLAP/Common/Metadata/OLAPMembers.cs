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

using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using System;
using System.Collections;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPChildMember : CaptionableOLAPMember {
		OLAPChildMember parentMember;
		OLAPMemberCollection childMembers;
		public OLAPChildMember ParentMember {
			get { return parentMember; }
			private set { parentMember = value; }
		}
		public override IOLAPEditableMemberCollection ChildMembers {
			get {
				if(childMembers == null)
					childMembers = CreateMemberCollection();
				return childMembers;
			}
		}
		OLAPMemberCollection CreateMemberCollection() {
			return new OLAPMemberCollection(Column);
		}
		public OLAPChildMember(OLAPMetadataColumn column, string uniqueName, object value, string caption)
			: base(column, uniqueName, value, caption) {
		}
		public void SetChildMembers(List<OLAPMember> childMembers) {
			if(childMembers == null)
				return;
			if(ChildMembers.Count > 0)
				throw new QueryException("ChildMembers are already loaded");
			ChildMembers.AddRange(childMembers);
			foreach(OLAPChildMember childMember in childMembers)
				childMember.ParentMember = this;
		}
	}
	public class CaptionableOLAPMember : OLAPMember {
		readonly string caption;
		public override string Caption {
			get {
				if(caption != null)
					return caption;
				return base.Caption;
			}
		}
		internal CaptionableOLAPMember(OLAPMetadataColumn column, string uniqueName, object value, string caption)
			: base(column, uniqueName, value) {
			string strVal = value as string;
			if(caption != null && caption.Equals(strVal))
				caption = strVal;
			else
				this.caption = caption;
		}
	}
	public class OLAPMember : QueryMember, IOLAPMember, IList<OLAPMember> {
		readonly string uniqueName;
		OLAPMemberProperties properties;
		internal OLAPMemberProperties autoProperties;
		public OLAPMember(OLAPMetadataColumn column, string uniqueName, object value)
			: base(column, value) {
			this.uniqueName = uniqueName;
		}
		public virtual IOLAPEditableMemberCollection ChildMembers {
			get {
				return Column.ChildColumn == null ? null : Column.ChildColumn;
			}
		}
		public new OLAPMetadataColumn Column { get { return ((OLAPMetadataColumn)base.Column); } }
		public string UniqueName { get { return uniqueName; } }
		IOLAPLevel IOLAPMember.Level { get { return Column; } }
		public override bool IsTotal { get { return Column.IsTotalMember(UniqueName); } }
		public override bool IsOthers { get { return OLAPDataSourceQueryBase.IsOthersMember(UniqueName); } }
		public OLAPMemberProperties AutoPopulatedProperties { get { return autoProperties; } }
		public OLAPMemberProperties Properties {
			get {
				if(properties == null)
					FetchAllProperties();
				return properties;
			}
		}
		protected void FetchAllProperties() {
			Column.Owner.FetchMemberProperties(this);
		}
		protected internal void InitProperties(IList<OLAPMemberProperty> list) {
			this.properties = (list != null) ? new OLAPMemberProperties(list) : null;
		}
		public void InitProperties(IOLAPRowSet rowSet) {
			List<OLAPMemberProperty> list = new List<OLAPMemberProperty>();
			rowSet.NextRow();
			for(int i = 0; i < rowSet.ColumnCount; i++) {
				string name = rowSet.GetColumnName(i);
				if(OLAPMetadataHelper.IsIntrinsicMemberProperty(name))
					continue;
				list.Add(new OLAPTypedProperty<object>(new OLAPPropertyDescriptor(name, Column.UniqueName, rowSet.GetColumnType(i)), rowSet.GetCellValue(i)));
			}
			InitProperties(list);
		}
		public override string ToString() {
			return UniqueName;
		}
		public override object UniqueLevelValue {
			get { return uniqueName; }
		}
		#region IOLAPMember Members
		string IOLAPNamedEntity.Name {
			get { return this.UniqueName; }
		}
		#endregion
		#region IList<IOLAPMember>
		int IList<OLAPMember>.IndexOf(OLAPMember item) {
			return item == this ? 0 : -1;
		}
		void IList<OLAPMember>.Insert(int index, OLAPMember item) {
			throw new NotImplementedException();
		}
		void IList<OLAPMember>.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		OLAPMember IList<OLAPMember>.this[int index] {
			get {
				if(index == 0)
					return this;
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		void ICollection<OLAPMember>.Add(OLAPMember item) {
			throw new NotImplementedException();
		}
		void ICollection<OLAPMember>.Clear() {
			throw new NotImplementedException();
		}
		bool ICollection<OLAPMember>.Contains(OLAPMember item) {
			return Equals(item);
		}
		void ICollection<OLAPMember>.CopyTo(OLAPMember[] array, int arrayIndex) {
			array[arrayIndex] = this;
		}
		int ICollection<OLAPMember>.Count {
			get { return 1; }
		}
		bool ICollection<OLAPMember>.IsReadOnly {
			get { return true; }
		}
		bool ICollection<OLAPMember>.Remove(OLAPMember item) {
			throw new NotImplementedException();
		}
		IEnumerator<OLAPMember> IEnumerable<OLAPMember>.GetEnumerator() {
			yield return this;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			yield return this;
		}
		#endregion
	}
	public class OLAPVirtualMember : OLAPMember {
		public OLAPVirtualMember(OLAPMetadataColumn column, string uniqueName)
			: base(column, uniqueName, null) {
		}
	}
}
#if DEBUGTEST
namespace DevExpress {
	using System;
	public class NonCoverAttribute : Attribute {
	}
}
#endif
