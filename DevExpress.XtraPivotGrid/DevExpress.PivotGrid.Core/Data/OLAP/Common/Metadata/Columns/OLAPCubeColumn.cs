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
using System.Text;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.Linq;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPCubeColumn : QueryColumn {
		bool olapUseNonEmpty, olapFilterByUniqueName;
		OLAPMember totalMember;
		PivotOLAPFilterUsingWhereClause olapFilterUsingWhereClause;
		IOLAPHelpersOwner owner;
		string[] autoPopulatedProperties;
		string sortByAttribute;
		public override bool IsSortedByAttribute {
			get {
				return base.IsSortedByAttribute || !string.IsNullOrEmpty(ActualSortProperty);
			}
		}
		public override string ActualSortProperty {
			get {
				if(SortMode == PivotSortMode.DimensionAttribute && !string.IsNullOrEmpty(SortByAttribute) && Properties.ContainsKey(SortByAttribute))
					return SortByAttribute;
				if(SortMode == PivotSortMode.Default && !string.IsNullOrEmpty(DefaultSortProperty))
					return DefaultSortProperty;
				return null;
			}
		}
		public string SortByAttribute { 
			get { return sortByAttribute; } 
			set { sortByAttribute = value; } 
		}
		string[] AutoPopulatedProperties { get { return autoPopulatedProperties; } set { autoPopulatedProperties = value; } }
		public IEnumerable<OLAPPropertyDescriptor> AutoProperties {
			get {
				if(SortMode == PivotSortMode.ID && (autoPopulatedProperties == null || !autoPopulatedProperties.Contains("ID")))
					yield return new OLAPPropertyDescriptor("ID", UniqueName, typeof(int));
				else
					if(SortMode == PivotSortMode.Key && (autoPopulatedProperties == null || !autoPopulatedProperties.Contains("Key"))) 
						yield return new OLAPPropertyDescriptor("Key", UniqueName, OLAPDataTypeConverter.Convert(Properties["KEY0"]));
					else {
						string actualSortProperty = ActualSortProperty;
						if(!string.IsNullOrEmpty(actualSortProperty) && (autoPopulatedProperties == null || !autoPopulatedProperties.Contains(actualSortProperty)))
							yield return new OLAPPropertyDescriptor(actualSortProperty, UniqueName, OLAPDataTypeConverter.Convert(Properties[actualSortProperty]));
					}
				if(autoPopulatedProperties != null)
					foreach(string str in autoPopulatedProperties)
						yield return new OLAPPropertyDescriptor(str, UniqueName, OLAPDataTypeConverter.Convert(Properties[str]));
			}
		}
		public bool OLAPUseNonEmpty { get { return olapUseNonEmpty; } }
		public bool OLAPFilterByUniqueName { get { return olapFilterByUniqueName; } }
		public new OLAPMetadataColumn Metadata { get { return (OLAPMetadataColumn)base.Metadata; } }
		#region metadata properties
		public OLAPMember this[string name] { get { return Metadata[name]; } }
		public bool HasCalculatedMembers { get { return Metadata.HasCalculatedMembers; } }
		public OLAPCubeColumn ParentColumn { get { return Metadata.ParentColumn != null ? Owner.CubeColumns[Metadata.ParentColumn.UniqueName] : null; } }
		public OLAPCubeColumn ChildColumn { get { return Metadata.ChildColumn != null ? Owner.CubeColumns[Metadata.ChildColumn.UniqueName] : null; } }
		public OLAPHierarchy Hierarchy { get { return Metadata.Hierarchy; } }
		public bool HasParent { get { return Metadata.HasParent; } }
		public bool IsParent(OLAPCubeColumn parent) { return Metadata.IsParent(parent.Metadata); }
		public bool IsAggregatable { get { return Metadata.IsAggregatable; } }
		public bool HasCustomDefaultMember { get { return Metadata.HasCustomDefaultMember; } }
		public OLAPMember DefaultMember { get { return Metadata.GetDefaultMember(this); } }
		public OLAPMember AllMember { get { return Metadata.AllMember; } }
		public string DefaultSortProperty { get { return Metadata.DefaultSortProperty; } }
		public int KeyCount { get { return Metadata.KeyCount; } }
		public Dictionary<string, OLAPDataType> Properties { get { return Metadata.Properties; } }
		#endregion
		#region properties
		public IOLAPHelpersOwner Owner {
			get { return owner ?? Metadata.Owner as IOLAPHelpersOwner; }
			internal set { owner = value; }
		}
		public OLAPDataType BaseDataType { get { return Metadata.BaseDataType; } }
		public new OLAPCubeColumn SortBySummary {
			get { return (OLAPCubeColumn)base.SortBySummary; }
			set { base.SortBySummary = value; }
		}
		public new OLAPMember TotalMember { get { return !HasCustomTotal ? Metadata.AllMember : totalMember; } }
		public override bool HasCustomTotal {
			get { return !Metadata.IsAggregatable || !TopValueShowOthers && TopValueCount != 0; }
		}
		public PivotOLAPFilterUsingWhereClause OLAPFilterUsingWhereClause {
			get { return olapFilterUsingWhereClause; }
			set { olapFilterUsingWhereClause = value; }
		}
		public override bool TopValueHiddenOthersShowedInTotal {
			get { return true; }
		}
		#endregion
		protected virtual MetadataColumnBase CreateEmptyMetadata() {
			return new OLAPMetadataColumn();
		}
		public OLAPCubeColumn(OLAPMetadataColumn metadata) : base(metadata) {
			Guard.ArgumentNotNull(metadata, "metadata");
			this.olapFilterUsingWhereClause = PivotOLAPFilterUsingWhereClause.SingleValuesOnly;
			if(TotalMember == null)
				UpdateTotalMember();
		}
		public override string ToString() {
			return Metadata.UniqueName;
		}
		public override bool Equals(object obj) {
			OLAPCubeColumn b = obj as OLAPCubeColumn;
			if(b != null) {
				return base.Equals(obj) && UniqueName == b.UniqueName && Metadata.Caption == b.Metadata.Caption && Metadata.Level == b.Metadata.Level
					&& object.Equals(Metadata.Hierarchy, b.Metadata.Hierarchy) && Metadata.AllMemberUniqueName == b.Metadata.AllMemberUniqueName
					&& Metadata.DefaultMemberName == b.Metadata.DefaultMemberName;
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return !string.IsNullOrEmpty(UniqueName) ? UniqueName.GetHashCode() : 0;
		}
		public string GetSortBySummaryMDX() {
			List<QueryMember> sortBySummary = new List<QueryMember>(SortBySummaryMembers);
			for(int i = sortBySummary.Count - 2; i >= 0; i--) {
				if(sortBySummary[i].Column.IsParent(sortBySummary[i + 1].Column))
					sortBySummary.RemoveAt(i);
			}
			if(sortBySummary.Count == 0)
				return SortBySummary.UniqueName;
			else {
				StringBuilder res = new StringBuilder("(");
				int membersCount = sortBySummary.Count;
				for(int i = 0; i < membersCount; i++) {
					res.Append(((OLAPMember)sortBySummary[i]).UniqueName).Append(", ");
				}
				res.Append(SortBySummary.UniqueName).Append(")");
				return res.ToString();
			}
		}
		public void EnsureColumnMembersLoaded() {
			Metadata.EnsureColumnMembersLoaded(this);
		}
		public override void Assign(PivotGridFieldBase field, bool forceSort) {
			base.Assign(field, forceSort);
			olapUseNonEmpty = field.Options.OLAPUseNonEmpty;
			olapFilterByUniqueName = field.ActualOLAPFilterByUniqueName;
			SortByAttribute = field.SortByAttribute;
			AssignAutoPopulatedProperties(field);
		}
		protected internal bool AssignAutoPopulatedProperties(PivotGridFieldBase field) {
 			int count = 0;
			if(field.AutoPopulatedProperties != null) {
				AutoPopulatedProperties = new string[field.AutoPopulatedProperties.Length];			  
				for(int i = 0; i < field.AutoPopulatedProperties.Length; i++) {
					string p = field.AutoPopulatedProperties[i];
					if(Properties.ContainsKey(p)) {
						AutoPopulatedProperties[count] = p;
						count++;
					}
				}
				if(count != AutoPopulatedProperties.Length)
					Array.Resize(ref autoPopulatedProperties, count);
				return count != 0;
			}
			AutoPopulatedProperties = null;
			return false;
		}
		protected override bool Equals(PivotGridFieldBase field, bool forceSort) {
			if(forceSort && SortMode == PivotSortMode.DimensionAttribute && ActualSortProperty != field.SortByAttribute)
				return false;
			return base.Equals(field, forceSort) && olapUseNonEmpty == field.Options.OLAPUseNonEmpty && OLAPFilterByUniqueName == field.ActualOLAPFilterByUniqueName
				&& (field.AutoPopulatedProperties == null ||  field.AutoPopulatedProperties.All(s => !Properties.ContainsKey(s) || autoPopulatedProperties != null && autoPopulatedProperties.Contains(s)));
		}
		protected override void UpdateTotalMember() {
			if(HasCustomTotal) {
				totalMember = new OLAPVirtualMember(Metadata, Metadata.TotalMemberUniqueName);
			} else
				totalMember = null;
		}
		internal List<OLAPMember> GetMembers() {
			return Metadata.GetMembers(false);
		}
		internal List<QueryMember> GetQueryMembers() {
			return Metadata.GetQueryMembers();
		}
		internal override IComparer<TMember> GetByMemberComparer<TMember>(Func<object, string> customText) {
			IComparer<QueryMember> comparer = GetByMemberComparer(customText);
			return comparer == null ? null : new DevExpress.PivotGrid.QueryMode.Sorting.QueryMemberProviderComparer<TMember>(comparer);
		}
		public IComparer<QueryMember> GetByMemberComparer(Func<object, string> customText) {
			switch(SortMode) {
				case PivotSortMode.Value:
					return new OLAPByMemberValueComparer(this);
				case PivotSortMode.DimensionAttribute:
				case PivotSortMode.Custom:
				case PivotSortMode.Default: {
					string sortProp = ActualSortProperty;
					if(!string.IsNullOrEmpty(sortProp))
						return MakeByAttributeComparer(Metadata.Properties[ActualSortProperty], ActualSortProperty);
					else
						if(Metadata.BaseDataType != OLAPDataType.Binary)
							return new OLAPByMemberValueComparer(this);
						else
							return new OLAPByDisplayTextMemberComparer();
					}
				case PivotSortMode.DisplayText:
					if(owner.Options.SortByCustomFieldValueDisplayText)
						return new DevExpress.PivotGrid.QueryMode.Sorting.ByMemberCustomDisplayTextComparer(customText);
					else
						return new OLAPByDisplayTextMemberComparer();
				case PivotSortMode.ID:
					return MakeByAttributeComparer(OLAPDataType.Integer, "ID");
				case PivotSortMode.Key:
					return MakeByAttributeComparer(Metadata.Properties["KEY0"], "Key");
				case PivotSortMode.None:
					return null;
				default:
					throw new NotImplementedException("PivotSortMode: " + SortMode);
			}
		}
		IComparer<QueryMember> MakeByAttributeComparer(OLAPDataType type, string sortAttr) {
			return (IComparer<QueryMember>)Activator.CreateInstance(typeof(OLAPByMemberAttributeValueComparer<>).MakeGenericType(OLAPDataTypeConverter.Convert(type)), new object[] { this, sortAttr });
		}
	   internal List<OLAPCubeColumn> GetColumnHierarchy() {
		   OLAPMetadataColumn column = Metadata;
		   while(column.ParentColumn != null)
			   column = column.ParentColumn;
		   List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
		   OLAPCubeColumn oColumn = Owner.CubeColumns[column.UniqueName];
		   while(oColumn != null) {
			   result.Add(oColumn);
			   oColumn = oColumn.ChildColumn;
		   }
		   return result;
	   }
	}
}
