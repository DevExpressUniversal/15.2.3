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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraBars {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TabFormControlAppearances : BaseAppearanceCollection {
		public TabFormControlAppearances() : this(null) { }
		public TabFormControlAppearances(TabFormControlBase owner)
			: base() {
			this.owner = owner;
			this.page = CreateElementAppearance();
		}
		TabFormControlBase owner;
		AppearanceObject tabFormControl;
		TabFormElementAppearances page;
		protected override void CreateAppearances() {
			this.tabFormControl = CreateAppearance("TabFormControl");
			TabFormControl.Changed += OnChanged;
		}
		protected TabFormElementAppearances CreateElementAppearance() {
			return new TabFormElementAppearances(this.owner);
		}
		void ResetTabFormControl() { TabFormControl.Reset(); }
		bool ShouldSerializeTabFormControl() { return TabFormControl.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject TabFormControl { get { return tabFormControl; } }
		void ResetPage() { Page.Reset(); }
		bool ShouldSerializePage() { return Page.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabFormElementAppearances Page { get { return page; } }
		public override bool ShouldSerialize() {
			return base.ShouldSerialize() || Page.ShouldSerialize();
		}
		public override void Reset() {
			base.Reset();
			Page.Reset();
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			if(Page != null) Page.Dispose();
			DisposeAppearanceObject(TabFormControl);
			base.Dispose();
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null) return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(this.owner != null) this.owner.OnAppearanceChanged();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TabFormElementAppearances : BaseAppearanceCollection {
		public TabFormElementAppearances() : base() { }
		public TabFormElementAppearances(TabFormControlBase owner)
			: base() {
			this.owner = owner;
		}
		TabFormControlBase owner;
		AppearanceObject normal, hovered, pressed, disabled;
		protected override void CreateAppearances() {
			this.normal = CreateAppearance("Normal");
			this.hovered = CreateAppearance("Hovered");
			this.pressed = CreateAppearance("Pressed");
			this.disabled = CreateAppearance("Disabled");
			Normal.Changed += OnChanged;
			Hovered.Changed += OnChanged;
			Pressed.Changed += OnChanged;
			Disabled.Changed += OnChanged;
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(this.owner != null) this.owner.OnAppearanceChanged();
		}
		void ResetNormal() { Normal.Reset(); }
		bool ShouldSerializeNormal() { return Normal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Normal { get { return normal; } }
		void ResetHovered() { Hovered.Reset(); }
		bool ShouldSerializeHovered() { return Hovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hovered { get { return hovered; } }
		void ResetPressed() { Pressed.Reset(); }
		bool ShouldSerializePressed() { return Pressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Pressed { get { return pressed; } }
		void ResetDisabled() { Disabled.Reset(); }
		bool ShouldSerializeDisabled() { return Disabled.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Disabled { get { return disabled; } }
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			DisposeAppearanceObject(Normal);
			DisposeAppearanceObject(Pressed);
			DisposeAppearanceObject(Hovered);
			DisposeAppearanceObject(Disabled);
			base.Dispose();
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null) return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
	}
	public class TabFormControlDefaultAppearances {
		TabFormControlViewInfoBase viewInfo;
		AppearanceDefault pageNormal, pageHovered, pagePressed, pageDisabled;
		public TabFormControlDefaultAppearances(TabFormControlViewInfoBase viewInfo) {
			this.viewInfo = viewInfo;
			Update();
		}
		public TabFormControlViewInfoBase ViewInfo { get { return viewInfo; } }
		public virtual void Update() {
			this.pageNormal = CreateAppearance(ObjectState.Normal);
			this.pagePressed = CreateAppearance(ObjectState.Pressed);
			this.pageHovered = CreateAppearance(ObjectState.Hot);
			this.pageDisabled = CreateAppearance(ObjectState.Disabled);
		}
		public AppearanceDefault PageNormal { get { return pageNormal; } }
		public AppearanceDefault PageHovered { get { return pageHovered; } }
		public AppearanceDefault PagePressed { get { return pagePressed; } }
		public AppearanceDefault PageDisabled { get { return pageDisabled; } }
		public AppearanceDefault GetAppearance(ObjectState state) {
			if(state == ObjectState.Hot) return PageHovered;
			if(state == ObjectState.Pressed) return PagePressed;
			if(state == ObjectState.Disabled) return PageDisabled;
			return PageNormal;
		}
		AppearanceDefault CreateAppearance(ObjectState state) {
			DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel = ViewInfo.Owner.LookAndFeel.ActiveLookAndFeel;
			SkinElement elem = FormSkins.GetSkin(lookAndFeel)[FormSkins.SkinTabFormPage];
			if(elem == null) elem = FormSkins.GetSkin(lookAndFeel)[FormSkins.SkinFormCaption];
			AppearanceDefault def = elem.GetAppearanceDefault(lookAndFeel);
			def.ForeColor = ViewInfo.GetForeColorByState(state);
			return def;
		}
	}
}
