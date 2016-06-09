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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	class PickManagerNode : INode {
		readonly string dataMember;
		readonly Type dataType;
		readonly string displayName;
		readonly bool isList;
		List<INode> childNodes;
		bool isComplex;
		public string DataMember {
			get { return dataMember; }
		}
		public Type DataType {
			get { return dataType; }
		}
		public string DisplayName {
			get { return displayName; }
		}
		public bool IsList {
			get { return isList; }
		}
		public PickManagerNode(string dataMember, string displayName, bool isList, bool isComplex) {
			this.dataMember = dataMember;
			this.dataType = typeof(object);
			this.displayName = displayName;
			this.isList = isList;
			this.isComplex = isComplex;
		}
		public PickManagerNode(string dataMember, string displayName, Type dataType, bool isList) {
			this.dataMember = dataMember;
			this.dataType = dataType;
			this.displayName = displayName;
			this.isList = isList;
			this.isComplex = false;
		}
		public PickManagerNode() {
			this.dataMember = string.Empty;
			this.dataType = typeof(object);
			this.displayName = string.Empty;
			this.isList = false;
		}
		IList INode.ChildNodes {
			get {
				if(childNodes == null)
					childNodes = new List<INode>();
				return childNodes;
			}
		}
		void INode.Expand(EventHandler callback) {
			throw new NotImplementedException();
		}
		bool INode.HasDataSource(object dataSource) {
			throw new NotImplementedException();
		}
		bool INode.IsComplex {
			get { return isComplex; }
		}
		bool INode.IsDataMemberNode {
			get { return !string.IsNullOrEmpty(dataMember); }
		}
		bool INode.IsDataSourceNode {
			get { throw new NotImplementedException(); }
		}
		bool INode.IsDummyNode {
			get { return string.IsNullOrEmpty(dataMember); }
		}
		bool INode.IsEmpty {
			get { throw new NotImplementedException(); }
		}
		object INode.Parent {
			get { throw new NotImplementedException(); }
		}
	}
}
