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
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	public interface IOleMemberWrapper {
		IQueryMetadataColumn Column { get; }
		string LevelName { get; }
		string UniqueName { get; }
		OLAPMember GetMember();
	}
	public class OleMeasureMemberWrapper : IOleMemberWrapper {
		OLAPCubeColumn measure;
		OLAPMember member;
		public OleMeasureMemberWrapper(OLAPCubeColumn measure) {
			this.measure = measure;
			member = new OLAPMember(measure.Metadata, measure.UniqueName, null);
		}
		string IOleMemberWrapper.LevelName {
			get { return measure.UniqueName; }
		}
		string IOleMemberWrapper.UniqueName {
			get { return measure.UniqueName; }
		}
		public OLAPMember GetMember() {
			return member;
		}
		IQueryMetadataColumn IOleMemberWrapper.Column {
			get { return measure.Metadata; }
		}
	}
	public class OleMemberWrapper : IOleMemberWrapper {
		string uniqueName;
		IQueryMetadataColumn column;
		OLAPMember member;
		public IQueryMetadataColumn Column {
			get { return column; }
			set { column = value; }
		}
		public string UniqueName { get { return uniqueName; } }
		public OleMemberWrapper(string uniqueName) {
			this.uniqueName = uniqueName;
		}
		public OleMemberWrapper(string uniqueName, IQueryMetadataColumn hierarchyColumn) {
			this.uniqueName = uniqueName;
			column = hierarchyColumn;
			Bind(null, true);
		}
		public OleMemberWrapper(string uniqueName, List<IQueryMetadataColumn> hierarchyColumns) {
			this.uniqueName = uniqueName;
			column = hierarchyColumns[0];
			Bind(hierarchyColumns, true);
		}
		public void Bind(List<IQueryMetadataColumn> desired, bool firstPass) {
			if(member != null)
				return;
			if(desired != null)
				foreach(OLAPMetadataColumn meta in desired) {
					member = meta[uniqueName];
					if(member != null)
						break;
				} else {
				member = ((OLAPMetadataColumn)column)[uniqueName];
			}
		}
		public bool NeedMember {
			get { return member == null || !member.IsTotal; }
		}   
		public bool HasMember {
			get { return member != null; }
		}
		public string LevelName {
			get {
				if(member == null)
					return column.UniqueName;
				return member.Column.UniqueName;
			}
		}
		public OLAPMember GetMember() {
			if(member == null)
				throw new NotImplementedException();
			return member;
		}
	}
	class OleDbTupleCollection : List<IOLAPTuple>, ITupleCollection {
		int ITupleCollection.ReadedCount {
			get { return Count; }
		}
		public OleDbTupleCollection(int count)
			: base(count) {
		}
	}
	class OleDbAxis : IOLAPAxis {
		OleDbTupleCollection tuples;
		public OleDbAxis(OleDbTupleCollection tuples) {
			this.tuples = tuples;
		}
		public ITupleCollection Tuples {
			get { return tuples; }
		}
		public string Name {
			get { return null; }
		}
	}
	class OleDbTuple : List<IOleMemberWrapper>, IOLAPTuple {
		OLAPMember IOLAPTuple.this[int index] {
			get {
				return this[index].GetMember();
			}
		}
		public OleDbTuple() { }
		public OLAPMember Single() {
			if(Count != 1)
				throw new ArgumentException();
			return this[0].GetMember();
		}
	}
	class OleAxisCollection : List<OleDbAxis>, IOLAPCollection<OleDbAxis> {
	}
	class OleDbCell : IOLAPCell {
		public OleDbCell(object value) {
			Value = value;
		}
		public object Value { get; private set; }
		string IOLAPCell.FormatString { get { return null; } }
		int IOLAPCell.Locale { get { return -1; } }
	}
}
