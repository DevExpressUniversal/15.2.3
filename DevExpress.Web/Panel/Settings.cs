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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum PanelFixedPosition { None, WindowTop, WindowBottom, WindowLeft, WindowRight }
	public enum PanelExpandEffect { Auto, PopupToLeft, PopupToRight, PopupToTop, PopupToBottom, Slide }
	public class PanelAdaptivitySettings : PropertiesBase {
		public PanelAdaptivitySettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelAdaptivitySettingsCollapseAtWindowInnerWidth"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int CollapseAtWindowInnerWidth {
			get { return GetIntProperty("CollapseAtWindowInnerWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "CollapseAtWindowInnerWidth");
				SetIntProperty("CollapseAtWindowInnerWidth", 0, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelAdaptivitySettingsCollapseAtWindowInnerHeight"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int CollapseAtWindowInnerHeight {
			get { return GetIntProperty("CollapseAtWindowInnerHeight", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "CollapseAtWindowInnerHeight");
				SetIntProperty("CollapseAtWindowInnerHeight", 0, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelAdaptivitySettingsHideAtWindowInnerWidth"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int HideAtWindowInnerWidth {
			get { return GetIntProperty("HideAtWindowInnerWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "HideAtWindowInnerWidth");
				SetIntProperty("HideAtWindowInnerWidth", 0, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelAdaptivitySettingsHideAtWindowInnerHeight"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(0)]
		public int HideAtWindowInnerHeight {
			get { return GetIntProperty("HideAtWindowInnerHeight", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "HideAtWindowInnerHeight");
				SetIntProperty("HideAtWindowInnerHeight", 0, value); 
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			PanelAdaptivitySettings src = source as PanelAdaptivitySettings;
			if(src != null) {
				CollapseAtWindowInnerWidth = src.CollapseAtWindowInnerWidth;
				CollapseAtWindowInnerHeight = src.CollapseAtWindowInnerHeight;
				HideAtWindowInnerWidth = src.HideAtWindowInnerWidth;
				HideAtWindowInnerHeight = src.HideAtWindowInnerHeight;
			}
		}
	}
	public enum PanelExpandButtonPosition { Auto, Center, Near, Far }
	public enum PanelExpandButtonGlyphType { Auto, Strips, ArrowLeft, ArrowRight, ArrowTop, ArrowBottom }
	public class PanelExpandButtonSettings : PropertiesBase {
		public PanelExpandButtonSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelExpandButtonSettingsGlyphType"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(PanelExpandButtonGlyphType.Auto)]
		public PanelExpandButtonGlyphType GlyphType {
			get { return (PanelExpandButtonGlyphType)GetEnumProperty("GlyphType", PanelExpandButtonGlyphType.Auto); }
			set {
				SetEnumProperty("GlyphType", PanelExpandButtonGlyphType.Auto, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelExpandButtonSettingsPosition"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(PanelExpandButtonPosition.Auto)]
		public PanelExpandButtonPosition Position {
			get { return (PanelExpandButtonPosition)GetEnumProperty("Position", PanelExpandButtonPosition.Auto); }
			set {
				SetEnumProperty("Position", PanelExpandButtonPosition.Auto, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelExpandButtonSettingsVisible"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			PanelExpandButtonSettings src = source as PanelExpandButtonSettings;
			if(src != null) {
				GlyphType = src.GlyphType;
				Position = src.Position;
				Visible = src.Visible;
			}
		}
	}
	public class PanelCollapsingSettings : PropertiesBase {
		PanelExpandButtonSettings expandButton;
		public PanelCollapsingSettings(IPropertiesOwner owner)
			: base(owner) {
			this.expandButton = new PanelExpandButtonSettings(owner);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelCollapsingSettingsAnimationType"),
#endif
		DefaultValue(AnimationType.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public AnimationType AnimationType {
			get { return (AnimationType)GetEnumProperty("AnimationType", AnimationType.Auto); }
			set { SetEnumProperty("AnimationType", AnimationType.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelCollapsingSettingsExpandEffect"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(PanelExpandEffect.Auto)]
		public PanelExpandEffect ExpandEffect {
			get { return (PanelExpandEffect)GetEnumProperty("ExpandEffect", PanelExpandEffect.Auto); }
			set { SetEnumProperty("ExpandEffect", PanelExpandEffect.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelCollapsingSettingsExpandButton"),
#endif
		AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PanelExpandButtonSettings ExpandButton {
			get { return expandButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelCollapsingSettingsExpandOnPageLoad"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public bool ExpandOnPageLoad {
			get { return GetBoolProperty("ExpandOnPageLoad", false); }
			set { SetBoolProperty("ExpandOnPageLoad", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelCollapsingSettingsGroupName"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string GroupName {
			get { return GetStringProperty("GroupName", ""); }
			set { SetStringProperty("GroupName", "", value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			PanelCollapsingSettings src = source as PanelCollapsingSettings;
			if(src != null) {
				AnimationType = src.AnimationType;
				ExpandEffect = src.ExpandEffect;
				ExpandButton.Assign(src.ExpandButton);
				ExpandOnPageLoad = src.ExpandOnPageLoad;
				GroupName = src.GroupName;
			}
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ExpandButton });
		}
	}
}
