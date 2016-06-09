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
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraCharts.Designer.Native {
	[AttributeUsage(AttributeTargets.Class)]
	public class ModelBinding : Attribute {
		Type modelType;
		public Type ModelType { get { return modelType; } }
		public ModelBinding(Type modelType) {
			this.modelType = modelType;
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class ModelOf : Attribute {
		Type chartElementType;
		public Type ChartElementType { get { return chartElementType; } }
		public ModelOf(Type type) {
			this.chartElementType = type;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyForOptions : Attribute {
		string groupName;
		int groupRank = 10;
		int rank = 10;
		public string GroupName { get { return groupName; } }
		public int GroupSortingRank { get { return groupRank; } }
		public int SortingRank { get { return rank; } }
		public PropertyForOptions(string groupName) {
			this.groupName = groupName;
		}
		public PropertyForOptions(int sortingRank, string groupName) {
			this.rank = sortingRank;
			this.groupName = groupName;
		}
		public PropertyForOptions(string groupName, int groupSortingRank) {
			this.groupName = groupName;
			this.groupRank = groupSortingRank;
		}
		public PropertyForOptions(int sortingRank, string groupName, int groupSortingRank) {
			this.rank = sortingRank;
			this.groupName = groupName;
			this.groupRank = groupSortingRank;
		}
		public PropertyForOptions() {
			this.groupName = "General";
			this.groupRank = -1;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class UseEditor : Attribute {
		Type editorType;
		Type adaptorType;
		public Type EditorType { get { return editorType; } }
		public Type AdaptorType { get { return adaptorType; } }
		public UseEditor(Type editorType, Type adaptorType) {
			this.adaptorType = adaptorType;
			this.editorType = editorType;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class ShowAsGallery : Attribute {
		Type providerType;
		public Type ProviderType { get { return providerType; } }
		public ShowAsGallery(Type providerType) {
			this.providerType = providerType;
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class HasOptionsControl : Attribute { }
	[AttributeUsage(AttributeTargets.Property)]
	public class UseAsSimpleProperty : Attribute { }
	[AttributeUsage(AttributeTargets.Property)]
	public class AllocateToGroup : Attribute {
		string groupName;
		int rank;
		public string GroupName { get { return groupName; } }
		public int SortingRank { get { return rank; } }
		public AllocateToGroup(string groupName, int sortingRank) {
			this.groupName = groupName;
			this.rank = sortingRank;
		}
		public AllocateToGroup(string groupName) {
			this.groupName = groupName;
			this.rank = 10;
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class GroupPrefix : Attribute {
		string prefix;
		public string Prefix {
			get {
				return prefix;				
			}
		}
		public GroupPrefix(string prefix) {
			this.prefix = prefix;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class DependentUpon : Attribute {
		string owningProperty;
		int relativeLevel;
		EditorActivity acivity;
		public string OwningProperty { get { return owningProperty; } }
		public int RelativeLevel { get { return relativeLevel; } }
		public EditorActivity Activity { get { return acivity; } }
		public DependentUpon(string owningProperty)
			: this(owningProperty, EditorActivity.Enable) {
		}
		public DependentUpon(string owningProperty, int relativeLevel)
			: this(owningProperty, relativeLevel, EditorActivity.Enable) {
		}
		public DependentUpon(string owningProperty, EditorActivity activity)
			: this(owningProperty, 0, activity) {
		}
		public DependentUpon(string owningProperty, int relativeLevel, EditorActivity activity) {
			this.owningProperty = owningProperty;
			this.relativeLevel = relativeLevel;
			this.acivity = activity;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class DesignerDisplayName : Attribute {
		string displayName;
		public string Name { get { return displayName; } }
		public DesignerDisplayName(string displayName) {
			this.displayName = displayName;
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class GenerateHeritableProperties : Attribute {
	}
}
