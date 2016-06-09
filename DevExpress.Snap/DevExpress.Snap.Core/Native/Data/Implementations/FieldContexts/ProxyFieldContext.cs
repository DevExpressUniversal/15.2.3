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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Utils;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data;
using DevExpress.Data.Browsing;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class ProxyFieldContext : ISingleObjectFieldContext {
		IDataControllerListFieldContext listContext;
		public ProxyFieldContext(string fieldPath, IDataControllerListFieldContext listContext) {
			Guard.ArgumentNotNull(fieldPath, "fieldPath");
			Guard.ArgumentNotNull(listContext, "listContext");
			FieldPath = fieldPath;
			this.listContext = listContext;
		}
		public virtual IDataControllerFieldContext Parent { get { return listContext; } }
		public virtual string FieldPath { get; protected set; }
		public void BeginCalculation(IServiceProvider serviceProvider) {
		}
		public void EndCalculation() {
		}
		public void Accept(IFieldContextVisitor visitor) {
			visitor.Visit(this);
		}
		public T Accept<T>(IFieldContextVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			ProxyFieldContext other = obj as ProxyFieldContext;
			if(Object.ReferenceEquals(other, null))
				return false;
			return Object.Equals(Parent, other.Parent) && FieldPath == other.FieldPath;
		}
		public override int GetHashCode() {
			return Parent.GetHashCode() ^ FieldPath.GetHashCode();
		}
		public IDataControllerListFieldContext ListContext {
			get { return listContext; }
		}
		public int VisibleIndex {
			get { return -1; }
		}
		public int RowHandle {
			get { return -1; }
		}
		public int CurrentRecordIndex {
			get { return -1; } 
		}
		public int CurrentRecordIndexInGroup {
			get { return -1; }
		}
		public RootFieldContext Root {
			get { return listContext.Root; }
		}
	}
}
