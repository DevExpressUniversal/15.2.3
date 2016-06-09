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
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DefaultProperty("Text")]
	public class CloudControlItem : CollectionItem {		
		private int fRank = 0;
		private object fDataItem = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("CloudControlItemName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CloudControlItemNavigateUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", ""); }
			set { SetStringProperty("NavigateUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CloudControlItemText"),
#endif
		DefaultValue("Item"), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", "Item"); }
			set { SetStringProperty("Text", "Item", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CloudControlItemValue"),
#endif
		DefaultValue(0), NotifyParentProperty(true)]
		public double Value {
			get { return (double)GetDecimalProperty("Value", 0); }
			set { SetDecimalProperty("Value", 0, (decimal)value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CloudControlItemToolTip"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", String.Empty); }
			set { SetStringProperty("ToolTip", String.Empty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return fDataItem; }
			set { SetDataItem(value); }
		}
		protected internal int Rank {
			get { return fRank; }
			set { fRank = value; }
		}
		public CloudControlItem() 
			: base() {		 
		}
		public CloudControlItem(string text, double value)
			: this(text, value, "", "") {
		}
		public CloudControlItem(string text, double value, string url)
			: this(text, value, url, "") {
		}
		public CloudControlItem(string text, double value, string url, string name)
			: base() {
			NavigateUrl = url;
			Text = text;
			Value = value;
			Name = name;
		}
		public override void Assign(CollectionItem source) {
			if (source is CloudControlItem) {
				CloudControlItem src = source as CloudControlItem;
				Name = src.Name;
				NavigateUrl = src.NavigateUrl;
				Text = src.Text;
				Value = src.Value;
				ToolTip = src.ToolTip;
			}
			base.Assign(source);
		}
		public override string ToString() {
			return !String.IsNullOrEmpty(Text) ? Text : GetType().Name;
		}
		protected internal void SetDataItem(object obj) {
			fDataItem = obj;
		}
	}
	public class CloudControlItemCollection : Collection<CloudControlItem> {
		public CloudControlItemCollection() 
			: base() {
		}
		public CloudControlItemCollection(ASPxCloudControl control)
			: base(control) {
		}
		protected internal ASPxCloudControl CloudControl {
			get { return Owner as ASPxCloudControl; }
		}
		public CloudControlItem this[string name] { get { return FindByIndex(IndexOfName(name)); } }
		public CloudControlItem Add() {
			return AddInternal(new CloudControlItem());
		}
		public void Add(string text, double value) {
			AddInternal(new CloudControlItem(text, value));
		}
		public void Add(string text, double value, string url) {
			AddInternal(new CloudControlItem(text, value, url, ""));
		}
		public void Add(string text, double value, string url, string name) {
			AddInternal(new CloudControlItem(text, value, url, name));
		}
		[Obsolete("This method is now obsolete. Use the Item[string name] property instead.")]
		public CloudControlItem FindByName(string name) {
			return this[name];
		}
		public CloudControlItem FindByNameOrIndex(string nameOrIndex) {
			CloudControlItem item = this[nameOrIndex];
			if(item == null) {
				int index;
				if(Int32.TryParse(nameOrIndex, out index) && index >= 0 && index < Count)
					return this[index];
			}
			return item;
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(CloudControlItem item) {
				return item.Name == name;
			});
		}
		protected override void OnChanged() {
			if (CloudControl != null)
				CloudControl.ItemsChanged();
		}
	}
}
