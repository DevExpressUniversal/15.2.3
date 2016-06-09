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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
namespace DevExpress.XtraNavBar {
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class NavBarSeparatorItem : NavBarItem {
		public NavBarSeparatorItem() : base() { }
		public override bool IsSeparator() {
			return true;
		}
		[Browsable(false)]
		public override string Caption {
			get { return String.Empty; }
			set { base.Caption = value; }
		}
		[Browsable(false)]
		public override bool CanDrag {
			get { return false; }
			set { base.CanDrag = value; }
		}
		[Browsable(false)]
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public override bool Enabled { get; set; }
		[Browsable(false)]
		public override SuperToolTip SuperTip { get; set; }
		[Browsable(false)]
		public override Image LargeImage { get; set; }
		[Browsable(false)]
		public override int LargeImageIndex { get; set; }
		[Browsable(false)]
		public override Size LargeImageSize { get; set; }
		[Browsable(false)]
		public override Image SmallImage { get; set; }
		[Browsable(false)]
		public override int SmallImageIndex { get; set; }
		[Browsable(false)]
		public override Size SmallImageSize { get; set; }
		[Browsable(false)]
		public override AppearanceObject Appearance { get { return base.Appearance; } }
		[Browsable(false)]
		public override AppearanceObject AppearanceHotTracked { get { return base.AppearanceHotTracked; } }
		[Browsable(false)]
		public override AppearanceObject AppearancePressed { get { return base.AppearancePressed; } }
		[Browsable(false)]
		public override AppearanceObject AppearanceDisabled { get { return base.AppearanceDisabled; } }
		[Browsable(false)]
		public override string Hint { get; set; }
		[Browsable(false)]
		public override object Tag { get; set; }
	}
	[DesignTimeVisible(false), ToolboxItem(false), SmartTagSupport(typeof(NavBarItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner),
	Designer("DevExpress.XtraNavBar.Design.NavBarItemDesigner, " + AssemblyInfo.SRAssemblyNavBarDesign)]
	public class NavBarItem : NavElement, DevExpress.Utils.MVVM.ISupportCommandBinding {
		NavReadOnlyLinkCollection links;
		AppearanceObject appearanceDisabled;
		bool allowAutoSelectCore;
		bool enabled, canDrag;
		string styleDisabledName;
		public NavBarItem(string caption) : this() {
			Caption = caption;
		}
		public NavBarItem() {
			this.canDrag = true;
			this.enabled = true;
			this.styleDisabledName = string.Empty;
			this.links = new NavReadOnlyLinkCollection();
			this.appearanceDisabled = CreateAppearance("Disabled");
			this.allowAutoSelectCore = true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				NavReadOnlyLinkCollection oldLinks = links;
				links = new NavReadOnlyLinkCollection();
				NavBarControl oldNavBar = NavBar;
				if(oldNavBar != null) oldNavBar.BeginUpdate();
				try {
					foreach(NavBarItemLink link in oldLinks) {
						link.Dispose();
					}
				}
				finally {
					if(oldNavBar != null) oldNavBar.EndUpdate();
				}
				SetNavBarCore(null);
				base.Dispose(disposing);
				if(oldNavBar != null && oldNavBar.Items.IndexOf(this) != -1) oldNavBar.Items.Remove(this);
			}
			base.Dispose(disposing);
		}
		protected override void DestroyAppearances() {
			ResetCache();
			DestroyAppearance(this.appearanceDisabled);
			base.DestroyAppearances();
		}
		protected override void RaiseItemChanged() {
			RemoveLinksFromCache();
			base.RaiseItemChanged();
		}
		protected void RemoveLinksFromCache() {
			if(NavBar == null || NavBar.ViewInfo == null) return;
			foreach(NavBarItemLink link in Links) {
				NavBar.ViewInfo.LinkSizesCache.Remove(link);
			}
		}
		[Browsable(false)]
		public NavReadOnlyLinkCollection Links { get { return links; } }
		[Browsable(false)]
		public NavItemCollection Collection { get { return ((ICollectionItem)this).Collection as NavItemCollection; } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty()]
		public virtual string StyleDisabledName {
			get { return styleDisabledName; }
			set {
				if(value == null) value = string.Empty;
				if(StyleDisabledName == value) return;
				styleDisabledName = value;
				RaiseItemChanged();
			}
		}
		bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled.ShouldSerialize(); }
		void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemAppearanceDisabled"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceDisabled { get { return appearanceDisabled; } }
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemEnabled"),
#endif
 Category("Behavior"), DefaultValue(true), XtraSerializableProperty(), SmartTagProperty("Enabled", "Appearance", 10)]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				RaiseItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemCanDrag"),
#endif
 Category("Behavior"), DefaultValue(true), XtraSerializableProperty()]
		public virtual bool CanDrag {
			get { return canDrag; }
			set {
				if(CanDrag == value) return;
				canDrag = value;
			}
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemAllowAutoSelect"),
#endif
 Category("Behavior"), DefaultValue(true), XtraSerializableProperty()]
		public bool AllowAutoSelect {
			get { return allowAutoSelectCore; }
			set { allowAutoSelectCore = value; }
		}
		public virtual bool IsSeparator() {
			return false;
		}
		protected override void OnAppearanceChanged(object sender, EventArgs e) {
			ResetCache();
			base.OnAppearanceChanged(sender, e);
		}
		class ObjectStateComparer : IEqualityComparer<ObjectState> {
			public bool Equals(ObjectState x, ObjectState y) {
				return x == y;
			}
			public int GetHashCode(ObjectState obj) {
				return (int)obj;
			}
			public static ObjectStateComparer Instance = new ObjectStateComparer();
		}
		Dictionary<ObjectState, AppearanceObject> appearanceCache = new Dictionary<ObjectState, AppearanceObject>(ObjectStateComparer.Instance);
		NavBarAppearances paintAppearance;
		void OnPaintAppearanceChanged(object sender, EventArgs e) {
			ResetCache();
		}
		void ResetCache() {
			if(paintAppearance != null) {
				appearanceCache.Clear();
				paintAppearance.Changed -= OnPaintAppearanceChanged;
				paintAppearance = null;
			}
		}
		protected internal AppearanceObject GetItemAppearance(ObjectState state) {
			if(NavBar == null) return Appearance;
			if(paintAppearance != NavBar.PaintAppearance) {
				ResetCache();
			}
			if(paintAppearance == null) {
				paintAppearance = NavBar.PaintAppearance;
				paintAppearance.Changed += OnPaintAppearanceChanged;
			}
			AppearanceObject res;
			if(appearanceCache.TryGetValue(state, out res))
				return res;
			res = new AppearanceObject();
			AppearanceObject selected = (state & ObjectState.Selected) != 0 ? paintAppearance.ItemActive : null;
			AppearanceObject[] combine;
			switch(state & (~ObjectState.Selected)) {
			case ObjectState.Disabled: combine = new AppearanceObject[] { AppearanceDisabled, paintAppearance.ItemDisabled, Appearance, paintAppearance.Item }; break;
			case ObjectState.Hot: combine = new AppearanceObject[] { AppearanceHotTracked, selected, paintAppearance.ItemHotTracked, Appearance, paintAppearance.Item }; break;
			case ObjectState.Pressed: combine = new AppearanceObject[] { AppearancePressed, selected, paintAppearance.ItemPressed, Appearance, paintAppearance.Item }; break;
				default:
					combine = new AppearanceObject[] { selected, Appearance,  paintAppearance.Item }; break;
			}
			AppearanceHelper.Combine(res, combine);
			appearanceCache.Add(state, res);
			return res;
		}
		private static readonly object linkPressed = new object();
		private static readonly object linkClicked = new object();
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkPressed"),
#endif
 Category("NavBar")]
		public event NavBarLinkEventHandler LinkPressed {
			add { Events.AddHandler(linkPressed, value); }
			remove { Events.RemoveHandler(linkPressed, value); }
		}
		[
#if !SL
	DevExpressXtraNavBarLocalizedDescription("NavBarItemLinkClicked"),
#endif
 Category("NavBar")]
		public event NavBarLinkEventHandler LinkClicked {
			add { Events.AddHandler(linkClicked, value); }
			remove { Events.RemoveHandler(linkClicked, value); }
		}
		internal void RaiseLinkClickedCore(NavBarItemLink link) { RaiseLinkClicked(link); }
		internal void RaiseLinkPressedCore(NavBarItemLink link) { RaiseLinkPressed(link); }
		protected virtual void RaiseLinkPressed(NavBarItemLink link) {
			RaiseLinkEvent(linkPressed, link);
		}
		protected virtual void RaiseLinkClicked(NavBarItemLink link) {
			RaiseLinkEvent(linkClicked, link);
		}
		protected virtual void RaiseLinkEvent(object linkEvent, NavBarItemLink link) {
			NavBarLinkEventHandler handler = (NavBarLinkEventHandler)this.Events[linkEvent];
			if(handler != null) handler(this, new NavBarLinkEventArgs(link));
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.LinkClicked += (s, e) => execute(),
				(item, canExecute) => item.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.LinkClicked += (s, e) => execute(),
				(item, canExecute) => item.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(item, execute) => item.LinkClicked += (s, e) => execute(),
				(item, canExecute) => item.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
}
