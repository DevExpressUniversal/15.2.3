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
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraTreeList.Nodes;
using System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class DataMemberListNode : DataMemberListNodeBase, IDataInfoContainer {
		object dataSource = null;
		string dataMember = "";
		PickManager pickManager;
		public override bool IsDataMemberNode { get { return true; } }
		public override string DataMember { get { return dataMember; } }
		public override object DataSource { get { return dataSource; } }
		public override bool IsList { get { return pickManager != null; } }
		public DataMemberListNode(object dataSource, string dataMember, string text, PickManager pickManager, TreeListNodes owner, PropertyDescriptor property, int imageIndex)
			: base(text, imageIndex, imageIndex, owner, property) {
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.pickManager = pickManager;
		}
		DataInfo[] IDataInfoContainer.GetData() {
			return pickManager != null ? pickManager.GetData(DataSource, DataMember) :
				new DataInfo[] { new DataInfo(DataSource, DataMember, this.GetDisplayText(0)) };
		}
	}
}
