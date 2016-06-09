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

using DevExpress.PivotGrid.OLAP;
namespace DevExpress.PivotGrid.OLAP.SchemaEntities {
	sealed class OlapHierarchy : OlapUniqueEntity, IOLAPHierarchy {
		OlapLevelCollection levels;
		readonly OlapDimension dimension;
		readonly string allMember;
		readonly string defaultMember;
		readonly string displayFolder;
		readonly short structure;
		readonly short origin;
		public OlapHierarchy(OlapDimension dimension, string name, string uniqueName, string caption, string description,
				string displayFolder, string defaultMember, string allMember, short structure, short origin)
			: base(name, uniqueName, caption, description) {
			this.dimension = dimension;
			this.displayFolder = displayFolder;
			this.allMember = allMember;
			this.defaultMember = defaultMember;
			this.structure = structure;
			this.origin = origin;
		}
		public OlapLevelCollection Levels {
			get {
				if(levels == null)
					levels = new OlapLevelCollection(this);
				return levels;
			}
		}
		public OlapDimension ParentDimension { get { return dimension; } }
		#region IOLAPHierarchy Members
		string IOLAPHierarchy.DisplayFolder {
			get { return this.displayFolder; }
		}
		string IOLAPHierarchy.AllMember {
			get { return this.allMember; }
		}
		string IOLAPHierarchy.DefaultMember {
			get { return this.defaultMember; }
		}
		IOLAPCollection IOLAPHierarchy.Levels {
			get { return this.Levels; }
		}
		short IOLAPHierarchy.Structure {
			get { return this.structure; }
		}
		short IOLAPHierarchy.Origin {
			get { return this.origin; }
		}
		#endregion
	}
}
