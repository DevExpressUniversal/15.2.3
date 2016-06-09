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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ListBoxColumn : WebColumnBase {
		public ListBoxColumn()
			: this(string.Empty) {
		}
		public ListBoxColumn(string fieldName)
			: this(fieldName, string.Empty) {
		}
		public ListBoxColumn(string fieldName, string caption)
			: this(fieldName, caption, Unit.Empty) {
		}
		public ListBoxColumn(string fieldName, string caption, Unit width) {
			SetFieldName(fieldName);
			Caption = caption;
			Width = width;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxColumnFieldName"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), 
		TypeConverter("DevExpress.Web.Design.ListBoxColumnDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		NotifyParentProperty(true)]
		public string FieldName {
			get { return GetStringProperty("FieldName", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				if(value != FieldName) {
					SetFieldName(value);
					OnColumnChanged();
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int VisibleIndex {
			get { return base.VisibleIndex; }
			set { base.VisibleIndex = value; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ListBoxColumn column = source as ListBoxColumn;
			if(column != null)
				FieldName = column.FieldName;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			return FieldNameToCaption();
		}
		protected internal string GetCaption() {
			return string.IsNullOrEmpty(Caption) ? FieldNameToCaption() : Caption;
		}
		string FieldNameToCaption() {
			return CommonUtils.SplitPascalCaseString(FieldName);
		}
		private void SetFieldName(string fieldName) {
			SetStringProperty("FieldName", string.Empty, fieldName);
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ListBoxColumnCollection : WebColumnCollectionBase {
		public ListBoxColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public ListBoxColumn this[int index] {
			get { return (ListBoxColumn)GetItem(index); }
		}
		protected IWebColumnsOwner OwnerControl {
			get {
				if(Owner != null)
					return Owner as IWebColumnsOwner;
				return null;
			}
		}
		public void Add(ListBoxColumn column) {
			base.Add(column);
		}
		public ListBoxColumn Add() {
			ListBoxColumn column = new ListBoxColumn();
			Add(column);
			return column;
		}
		public ListBoxColumn Add(string fieldName) {
			return Add(fieldName, string.Empty);
		}
		public ListBoxColumn Add(string fieldName, string caption) {
			return Add(fieldName, caption, Unit.Empty);
		}
		public ListBoxColumn Add(string fieldName, string caption, Unit width) {
			ListBoxColumn column = new ListBoxColumn(fieldName, caption, width);
			Add(column);
			return column;
		}
		public void Remove(ListBoxColumn column) {
			base.Remove(column);
		}
		public void Insert(int index, ListBoxColumn column) {
			base.Insert(index, column);
		}
		public int IndexOf(ListBoxColumn column) {
			return base.IndexOf(column);
		}
		protected override Type GetKnownType() {
			return typeof(ListBoxColumn);
		}
		protected override void OnChanged() {
			if(OwnerControl != null)
				OwnerControl.OnColumnCollectionChanged();
			else if(Owner != null)
				Owner.LayoutChanged();
		}
	}
}
