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
using DevExpress.Services.Internal;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraReports.Native {
	public class DataMemberListNodeBase : ComponentNodeBase, INode {
		PropertyDescriptor property;
		public virtual string DataMember { get { return null; } }
		public virtual object DataSource { get { return null; } }
		public virtual PropertyDescriptor Property { get { return property; } }
		public override IComponent Component { get { return GetComponent(); } }
		public object Object {
			get {
				return property is IContainerComponent ? ((IContainerComponent)property).Component : null;
			}
		}
		public bool IsEmpty { get { return DataSource == null && DataMember == null; } }
		public DataMemberListNodeBase(TreeListNodes owner)
			: base(owner) {
		}
		public DataMemberListNodeBase(string text, int imageIndex, int selectedImageIndex, TreeListNodes owner, PropertyDescriptor property)
			: this(owner) {
			this.Text = text;
			this.StateImageIndex = imageIndex;
			this.SelectImageIndex = selectedImageIndex;
			this.property = property;
		}
		IComponent GetComponent() {
			if (property is IContainerComponent)
				return ((IContainerComponent)property).Component as IComponent;
			return null;
		}
		#region INode Members
		public virtual bool IsDummyNode { get { return false; } }
		public virtual bool IsDataMemberNode { get { return false; } }
		public virtual bool IsDataSourceNode { get { return false; } }
		public virtual bool IsList { get { return false; } }
		public virtual bool IsComplex { get { return false; } }
		public IList ChildNodes { get { return (IList)this.Nodes; } }
		public object Parent { get { return this.ParentNode; } }
		public bool HasDataSource(object dataSource) {
			return this.DataSource != null && this.DataSource.Equals(dataSource);
		}
		public void Expand(EventHandler callback) {
			base.Expand();
			if (callback != null)
				callback(this, EventArgs.Empty);
		}
		#endregion
	}
}
