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

using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraBars {
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class TabFormLinkCollection : BarItemLinkCollection {
		TabFormLinkProvider provider;
		public TabFormLinkCollection(TabFormLinkProvider provider) : base() {
			this.provider = provider;
		}
		protected override BarManager Manager { get { return this.provider.Manager; } }
		protected override bool OnInsert(int index, object item) {
			bool res = base.OnInsert(index, item);
			if(!res) return false;
			BarItemLink link = item as BarItemLink;
			if(link != null && link.Item != null) {
				link.ownerControl = this.provider.Owner;
				if(link.Item.Manager == null) link.Item.Manager = Manager;
			}
			return true;
		}
		public void Remove(BarItem item) {
			foreach(BarItemLink link in InnerList) {
				if(link.Item == item) {
					Remove(link);
					break;
				}
			}
		}
		public override void Clear() {
			base.ClearItems();
		}
		protected internal override object Owner { get { return this; } }
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			this.provider.OnCollectionChanged();
		}
	}
	public class TabFormLinkProvider {
		TabFormControlBase owner;
		TabFormLinkCollection titleItemLinks, tabLeftItemLinks, tabRightItemLinks;
		public TabFormLinkProvider(TabFormControlBase owner) {
			this.owner = owner;
			this.titleItemLinks = CreateLinkCollection();
			this.tabLeftItemLinks = CreateLinkCollection();
			this.tabRightItemLinks = CreateLinkCollection();
		}
		TabFormLinkCollection CreateLinkCollection() {
			return new TabFormLinkCollection(this);
		}
		void ResetTopLinks() { this.titleItemLinks = CreateLinkCollection(); }
		bool ShouldSerializeTopLinks() { return this.titleItemLinks.Count != 0; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormLinkCollection TitleItemLinks { get { return titleItemLinks; } }
		void ResetLeftLinks() { this.tabLeftItemLinks = CreateLinkCollection(); }
		bool ShouldSerializeLeftLinks() { return this.tabLeftItemLinks.Count != 0; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormLinkCollection TabLeftItemLinks { get { return tabLeftItemLinks; } }
		void ResetRigthLinks() { this.tabRightItemLinks = CreateLinkCollection(); }
		bool ShouldSerializeRigthLinks() { return this.tabRightItemLinks.Count != 0; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormLinkCollection TabRightItemLinks { get { return tabRightItemLinks; } }
		[Browsable(false)]
		public TabFormControlBase Owner { get { return owner; } }
		protected internal BarManager Manager { get { return Owner == null ? null : Owner.Manager; } }
		protected internal void OnCollectionChanged() {
			if(Owner.IsInInit) return;
			Owner.LayoutChanged();
		}
		public void ForEachLink(Action<BarItemLink> handler) {
			ForEachLinkCore(TitleItemLinks, handler);
			ForEachLinkCore(TabLeftItemLinks, handler);
			ForEachLinkCore(TabRightItemLinks, handler);
		}
		void ForEachLinkCore(TabFormLinkCollection col, Action<BarItemLink> handler) {
			for(int i = 0; i < col.Count; i++) {
				handler(col[i]);
			}
		}
		internal bool ShouldSerialize() {
			return ShouldSerializeTopLinks() || ShouldSerializeLeftLinks() || ShouldSerializeRigthLinks();
		}
		internal void Reset() {
			TitleItemLinks.Clear();
			TabLeftItemLinks.Clear();
			TabRightItemLinks.Clear();
		}
		public bool Contains(BarItemLink barItemLink) {
			if(TitleItemLinks.Contains(barItemLink)) return true;
			if(TabLeftItemLinks.Contains(barItemLink)) return true;
			return TabRightItemLinks.Contains(barItemLink);
		}
	}
	public enum TabFormLinksLayoutType { Left, Top, Right }
	public class TabFormLinkInfoCollection : List<BarLinkViewInfo> {
		Rectangle bounds;
		TabFormLinksLayoutType layoutType;
		public TabFormLinkInfoCollection(TabFormLinksLayoutType layoutType)
			: base() {
			this.bounds = Rectangle.Empty;
			this.layoutType = layoutType;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		internal TabFormLinksLayoutType LayoutType { get { return layoutType; } }
	}
	public class TabFormLinkInfoProvider {
		TabFormLinkInfoCollection titleItemLinks, tabLeftItemLinks, tabRightItemLinks;
		public TabFormLinkInfoProvider() {
			Reset();
		}
		internal void Reset() {
			this.titleItemLinks = CreateLinkInfoCollection(TabFormLinksLayoutType.Top);
			this.tabLeftItemLinks = CreateLinkInfoCollection(TabFormLinksLayoutType.Left);
			this.tabRightItemLinks = CreateLinkInfoCollection(TabFormLinksLayoutType.Right);
		}
		TabFormLinkInfoCollection CreateLinkInfoCollection(TabFormLinksLayoutType layoutType) {
			return new TabFormLinkInfoCollection(layoutType);
		}
		public TabFormLinkInfoCollection TitleItemLinks { get { return titleItemLinks; } }
		public TabFormLinkInfoCollection TabLeftItemLinks { get { return tabLeftItemLinks; } }
		public TabFormLinkInfoCollection TabRightItemLinks { get { return tabRightItemLinks; } }
		public BarItemLink GetItemLinkByPoint(Point p) {
			BarItemLink res = GetItemLinkByPoint(TitleItemLinks, p);
			if(res != null) return res;
			res = GetItemLinkByPoint(TabLeftItemLinks, p);
			if(res != null) return res;
			return GetItemLinkByPoint(TabRightItemLinks, p);
		}
		public BarItemLink GetItemLinkByPoint(List<BarLinkViewInfo> col, Point p) {
			for(int i = 0; i < col.Count; i++) {
				if(col[i].Bounds.Contains(p))
					return col[i].Link;
			}
			return null;
		}
		public void ForEachLinkInfo(Action<BarLinkViewInfo> handler) {
			ForEachLinkInfoCore(TitleItemLinks, handler);
			ForEachLinkInfoCore(TabLeftItemLinks, handler);
			ForEachLinkInfoCore(TabRightItemLinks, handler);
		}
		void ForEachLinkInfoCore(TabFormLinkInfoCollection col, Action<BarLinkViewInfo> handler) {
			for(int i = 0; i < col.Count; i++) {
				handler(col[i]);
			}
		}
	}
}
