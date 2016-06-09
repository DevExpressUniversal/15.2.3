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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars {
#if DXWhidbey
	[DesignerSerializer("DevExpress.Utils.Design.DXCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
#endif
	public class LinksInfo : CollectionBase, ICloneable {
		public LinksInfo() { }
		public LinksInfo(object[] info) { this.AddRange(info); 	}
		object ICloneable.Clone() {
			LinksInfo li = new LinksInfo();
			li.AddRange(this);
			return li;
		}
		public override int GetHashCode() { return base.GetHashCode(); }
		public override bool Equals(object obj) {
			LinksInfo li = obj as LinksInfo;
			if(li == null) return false;
			if(li.Count == this.Count) return true;
			return false;
		}
		public LinkPersistInfo this[int index] {
			get { return List[index] as LinkPersistInfo; }
		}
		internal void RemoveLink(BarItemLink link) {
			foreach(LinkPersistInfo info in this) {
				if(info.Link == link) {
					Remove(info);
					break;
				}
			}
		}
		public void Insert(int index, LinkPersistInfo pInfo) { InnerList.Insert(index, pInfo); }
		public void Remove(LinkPersistInfo pInfo) { List.Remove(pInfo); }
		public int Add(LinkPersistInfo pInfo) {
			int index = List.IndexOf(pInfo);
			if(index != -1) return index;
			return List.Add(pInfo);
		}
		protected void AddRange(ICollection collection) { InnerList.AddRange(collection); }
		public void AddRange(LinkPersistInfo[] links) {
			foreach(LinkPersistInfo pi in links) {
				this.Add(pi);
			}
		}
	 }
	[TypeConverter(typeof(DevExpress.XtraBars.Design.LinkPersistInfoTypeConverter))]
	public class LinkPersistInfo {
		BarLinkUserDefines userDefine;
		BarItem item;
		BarItemPaintStyle userPaintStyle;
		Image userGlyph;
		string userCaption;
		RibbonItemStyles userRibbonStyle;
		BarItemLinkAlignment userAlignment;
		string userKeyTip;
		string userDropDownKeyTip;
		int userWidth, userEditWidth;
		bool beginGroup, visible;
		bool mostRecentlyUsed;
		BarItemLink link = null;
		public BarItem Item { get { return item; } set { item = value; } }
		bool IsUserDefine(BarLinkUserDefines userDefine) { return ((UserDefine & userDefine) != 0); }
		bool ShouldSerializeUserPaintStyle() { return IsUserDefine(BarLinkUserDefines.PaintStyle); }
		public BarItemPaintStyle UserPaintStyle { get { return userPaintStyle; } set { userPaintStyle = value; } }
		bool ShouldSerializeUserGlyph() { return IsUserDefine(BarLinkUserDefines.Glyph); }
		public Image UserGlyph { get { return userGlyph; } set { userGlyph = value; } }
		bool ShouldSerializeUserCaption() { return IsUserDefine(BarLinkUserDefines.Caption); }
		public string UserCaption { get { return userCaption; } set { userCaption = value; } }
		bool ShouldSerializeUserWidth() { return IsUserDefine(BarLinkUserDefines.Width); }
		public int UserWidth  { get { return userWidth; } set { userWidth = value; } }
		bool ShouldSerializeUserKeyTip() { return IsUserDefine(BarLinkUserDefines.KeyTip); }
		public string UserKeyTip { get { return userKeyTip; } set { userKeyTip = value; } }
		bool ShouldSerializeUserDropDownKeyTip() { return IsUserDefine(BarLinkUserDefines.KeyTip); }
		public string UserDropDownKeyTip { get { return userDropDownKeyTip; } set { userDropDownKeyTip = value; } }
		bool ShouldSerializeUserEditWidth() { return IsUserDefine(BarLinkUserDefines.EditWidth); }
		public int UserEditWidth { get { return userEditWidth; } set { userEditWidth = value; } }
		bool ShouldSerializeUserRibbonStyle() { return IsUserDefine(BarLinkUserDefines.RibbonStyle); }
		public RibbonItemStyles UserRibbonStyle { get { return userRibbonStyle; } set { userRibbonStyle = value; } }
		bool ShouldSerializeUserAlignment() { return IsUserDefine(BarLinkUserDefines.Alignment); }
		public BarItemLinkAlignment UserAlignment { get { return userAlignment; } set { userAlignment = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(BarLinkUserDefines.None)]
		public BarLinkUserDefines UserDefine { get { return userDefine; } }
		[DefaultValue(BarItemLink.DefaultBeginGroup)]
		public bool BeginGroup  { get { return beginGroup; } set { beginGroup = value; } }
		[DefaultValue(BarItemLink.DefaultVisible)]
		public bool Visible  { get { return visible; } set { visible = value; } }
		[DefaultValue(BarItemLink.DefaultMostRecentlyUsed)]
		public bool MostRecentlyUsed { get { return mostRecentlyUsed; } set { mostRecentlyUsed = value; } }
		internal LinkPersistInfo(BarItemLink link) {
			this.Item = link.Item;
			this.link = link;
			UpdateFromLink();
		}
		protected internal void UpdateFromLink() {
			if(Link == null) return;
			this.mostRecentlyUsed = link.MostRecentlyUsed;
			this.userPaintStyle = link.UserPaintStyle;
			this.userGlyph = link.UserGlyph;
			this.userWidth = link.UserWidth;
			this.userEditWidth = link.UserEditWidth;
			this.userCaption = link.UserCaption;
			this.userDefine = link.UserDefine;
			this.beginGroup = link.BeginGroup;
			this.userKeyTip = link.KeyTip;
			this.userRibbonStyle = link.UserRibbonStyle;
			this.userAlignment = link.UserAlignment;
			BarButtonItemLink blink = link as BarButtonItemLink;
			this.userDropDownKeyTip = blink != null ? blink.DropDownKeyTip : "";
			this.mostRecentlyUsed = link.MostRecentlyUsed;
			this.visible = link.Visible;
		}
		public LinkPersistInfo() {
			this.userDefine = BarLinkUserDefines.None;
			this.item = null;
			this.userCaption = "";
			this.userPaintStyle = BarItemPaintStyle.Standard;
			this.userWidth = 0;
			this.userEditWidth = 0;
			this.userKeyTip = "";
			this.userDropDownKeyTip = "";
			this.userGlyph = null;
			this.userRibbonStyle = RibbonItemStyles.Default;
			this.userAlignment = BarItemLinkAlignment.Default;
			this.link = null;
			this.beginGroup = BarItemLink.DefaultBeginGroup;
			this.visible = BarItemLink.DefaultVisible;
			this.mostRecentlyUsed = BarItemLink.DefaultMostRecentlyUsed;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item) : this() {
			this.item = item;
			this.userDefine = userDefine;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, BarItemPaintStyle userPaintStyle) : this(userDefine, item) {
			this.userPaintStyle = userPaintStyle;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption) : this(userDefine, item) {
			this.userCaption = userCaption;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, bool beginGroup) : this(userDefine, item) {
			this.beginGroup = beginGroup;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, bool visible, BarItem item, bool beginGroup) : this(userDefine, item) {
			this.beginGroup = beginGroup;
			this.visible = visible;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, bool beginGroup, bool mostRecenltyUsed) : this(userDefine, item, beginGroup) {
			this.mostRecentlyUsed = beginGroup;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption, bool beginGroup) : this(userDefine, item, userCaption) {
			this.beginGroup = beginGroup;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption, bool beginGroup, string userKeyTip)
			: this(userDefine, item, userCaption, beginGroup) {
			this.userKeyTip = userKeyTip;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption, 
			bool beginGroup, bool mostRecentlyUsed, bool visible, int userWidth) : this(userDefine, item, userCaption, beginGroup) {
			this.mostRecentlyUsed = mostRecentlyUsed;
			this.visible = visible;
			this.userWidth = userWidth;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption, 
			bool beginGroup, bool mostRecentlyUsed, bool visible, int userWidth, Image userGlyph) : this(userDefine, item, userCaption, beginGroup, mostRecentlyUsed, visible, userWidth) {
			this.userGlyph = userGlyph;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption, 
			bool beginGroup, bool mostRecentlyUsed, bool visible, int userWidth, Image userGlyph, BarItemPaintStyle userPaintStyle) : this(userDefine, item, userCaption, beginGroup, mostRecentlyUsed, visible, userWidth, userGlyph) {
			this.userPaintStyle = userPaintStyle;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption,
			bool beginGroup, bool mostRecentlyUsed, bool visible, int userWidth, Image userGlyph, BarItemPaintStyle userPaintStyle, string userKeyTip)
			: this(userDefine, item, userCaption, beginGroup, mostRecentlyUsed, visible, userWidth, userGlyph, userPaintStyle) {
			this.userKeyTip = userKeyTip;
		}
		public LinkPersistInfo(BarLinkUserDefines userDefine, BarItem item, string userCaption,
			bool beginGroup, bool mostRecentlyUsed, bool visible, int userWidth, Image userGlyph, BarItemPaintStyle userPaintStyle, string userKeyTip, string userDropDownKeyTip)
			: this(userDefine, item, userCaption, beginGroup, mostRecentlyUsed, visible, userWidth, userGlyph, userPaintStyle, userKeyTip) {
				this.userDropDownKeyTip = userDropDownKeyTip;
		}
		public LinkPersistInfo(BarItem item) : this() {
			this.item = item;
		}
		public LinkPersistInfo(BarItem item, bool beginGroup) : this(item, beginGroup, null, 0) {
		}
		public LinkPersistInfo(BarItem item, string caption) : this(item, BarItemLink.DefaultBeginGroup, caption, 0) {
		}
		public LinkPersistInfo(BarItem item, int imageIndex) : this(item, BarItemLink.DefaultBeginGroup, "", imageIndex) {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex) : this(item, beginGroup, caption, imageIndex, 0) {
		}
		public LinkPersistInfo(int width, BarItem item) : this(item, BarItemLink.DefaultBeginGroup, "", 0, width) {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width) : this(item, beginGroup, caption, imageIndex, width, 0) {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, int editWidth) : this(item, beginGroup, caption, imageIndex, width, editWidth, BarItemLink.DefaultMostRecentlyUsed) { }
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, bool mostRecentlyUsed) : this(item, beginGroup, caption, imageIndex, width, 0, mostRecentlyUsed) {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, int editWidth, bool mostRecentlyUsed) :
			this(item, beginGroup, caption, imageIndex, width, editWidth, mostRecentlyUsed, BarItemLink.DefaultVisible) { }
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, bool mostRecentlyUsed, bool visible) : this(item, beginGroup, caption, imageIndex, width, 0, mostRecentlyUsed, visible) { }
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, int editWidth, bool mostRecentlyUsed, bool visible) :
			this(item, beginGroup, caption, imageIndex, width, editWidth, mostRecentlyUsed, visible, "") { 
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, bool mostRecentlyUsed, bool visible, string userKeyTip) : 
			this(item, beginGroup, caption, imageIndex, width, 0, mostRecentlyUsed, visible, userKeyTip) {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, int editWidth, bool mostRecentlyUsed, bool visible, string userKeyTip) :
			this(item, beginGroup, caption, imageIndex, width, editWidth, mostRecentlyUsed, visible, userKeyTip, "") {
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, bool mostRecentlyUsed, bool visible, string userKeyTip, string userDropDownKeyTip) : 
		this(item, beginGroup, caption, imageIndex, width, 0, mostRecentlyUsed, visible, userKeyTip, userDropDownKeyTip) { 
		}
		public LinkPersistInfo(BarItem item, bool beginGroup, string caption, int imageIndex, int width, int editWidth, bool mostRecentlyUsed, bool visible, string userKeyTip, string userDropDownKeyTip)
			: this() {
			this.mostRecentlyUsed = mostRecentlyUsed;
			this.item = item;
			this.beginGroup = beginGroup;
			this.userWidth = width;
			this.userEditWidth = editWidth;
			this.userKeyTip = userKeyTip;
			if(userWidth != 0) this.userDefine = BarLinkUserDefines.Width;
			if(userKeyTip != "" && userKeyTip != null) 
				this.userDefine |= BarLinkUserDefines.KeyTip;
			if(userDropDownKeyTip != "" && userDropDownKeyTip != null)
				this.userDefine |= BarLinkUserDefines.DropDownKeyTip;
			this.visible = visible;
		}
		public BarItemLink Link { get { return link; } }
		protected internal void SetLink(BarItemLink link) { this.link = link; }
		public virtual bool IsEquals(LinkPersistInfo pi) {
			if(pi == null) return false;
			if(Item != pi.Item) return false;
			return true;
		}
	}
}
namespace DevExpress.XtraBars.Design {
	using DevExpress.Utils.Design;
	public class LinkPersistInfoTypeConverter : UniversalTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,	Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) && value != null) {
				LinkPersistInfo pInfo = value as LinkPersistInfo;
				if(pInfo != null) pInfo.UpdateFromLink();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
