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

using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP.SchemaEntities {
		abstract class OlapEntity : OlapEntityBase, IOLAPNamedEntity {
		readonly string name;
		public OlapEntity(string name) {
			this.name = name;
		}
		public string Name {
			get { return this.name; }
		}
		public override string ToString() {
			return this.name;
		}
		protected internal override bool Compare(string uniqueName) {
			return this.name == uniqueName;
		}
	}
	abstract class OlapTitledEntity : OlapEntity, IOLAPTitledEntity {
		readonly string caption;
		readonly string description;
		public OlapTitledEntity(string name, string caption, string description)
			: base(name) {
			this.caption = caption;
			this.description = description;
		}
		public string Caption {
			get { return caption; }
		}
		public string Description {
			get { return description; }
		}
	}
	abstract class OlapEntityBase : IOLAPEntity {
		public OlapEntityBase() {
		}
		protected internal virtual bool Compare(string uniqueName) {
			return false;
		}
	}
	abstract class OlapPropertyEntity : OlapTitledEntity {
		public OlapPropertyEntity(string name, string caption, string description)
			: base(name, caption, description) {
		}
	}
	abstract class OlapUniqueEntity : OlapPropertyEntity, IOLAPUniqueEntity {
		readonly string uniqueName;
		public OlapUniqueEntity(string name, string uniqueName, string caption, string description)
			: base(name, caption, description) {
			this.uniqueName = uniqueName;
		}
		public string UniqueName {
			get { return uniqueName; }
		}
		public override string ToString() {
			return uniqueName;
		}
		protected internal override bool Compare(string uniqueName) {
			return this.uniqueName == uniqueName;
		}
	}
}
