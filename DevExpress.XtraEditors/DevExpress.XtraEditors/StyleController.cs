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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	public interface ISupportStyleController {
		IStyleController StyleController { get; set; }
	}
	public interface IStyleController {
		AppearanceObject Appearance { get ; }
		AppearanceObject AppearanceReadOnly { get; }
		AppearanceObject AppearanceDisabled { get; }
		AppearanceObject AppearanceFocused { get ; } 
		AppearanceObject AppearanceDropDown { get ; } 
		AppearanceObject AppearanceDropDownHeader { get ; }
		UserLookAndFeel LookAndFeel { get; }
		BorderStyles BorderStyle { get ; }
		BorderStyles ButtonsStyle { get; }
		PopupBorderStyles PopupBorderStyle { get; }
		event EventHandler PropertiesChanged, Disposed;
	}
	[Designer("DevExpress.XtraEditors.Design.StyleControllerDesigner, " + AssemblyInfo.SRAssemblyDesign),
	 Description("Provides centralized way to manipulate appearance settings for multiple editors."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "StyleController")
	]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	public class StyleController : Component, ISupportInitialize, IStyleController {
		UserLookAndFeel lookAndFeel;
		int lockUpdate;
		PopupBorderStyles _popupBorderStyle;
		BorderStyles fBorderStyle, fButtonsStyle;
		AppearanceObject appearance, appearanceDisabled, appearanceFocused, appearanceReadOnly, appearanceDropDown, appearanceDropDownHeader;
		private static readonly object propertiesChanged = new object();
		public StyleController(IContainer container) : this() {
			container.Add(this);
		}
		public StyleController() {
			this.appearance = CreateAppearance("Control");
			this.appearanceDisabled = CreateAppearance("ControlDisabled");
			this.appearanceFocused = CreateAppearance("ControlFocused");
			this.appearanceReadOnly = CreateAppearance("ControlReadOnly");
			this.appearanceDropDown = CreateAppearance("DropDown");
			this.appearanceDropDownHeader = CreateAppearance("DropDownHeader");
			this.fBorderStyle = this.fButtonsStyle = BorderStyles.Default;
			this._popupBorderStyle = PopupBorderStyles.Default;
			this.lockUpdate = 0;
			CreateLookAndFeel();
		}
		protected AppearanceObject CreateAppearance(string name) {
			AppearanceObject res = new AppearanceObject(name);
			res.Changed += new EventHandler(OnStyleChanged);
			return res;
		}
		protected void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnStyleChanged);
			appearance.Dispose();
		}
		public virtual void BeginInit() {
			this.lockUpdate ++;
		}
		public virtual void EndInit() {
			this.lockUpdate --;
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if(-- this.lockUpdate == 0)
				OnPropertiesChanged();
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerBorderStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStyle { 
			get { return fBorderStyle; }
			set {
				if(BorderStyle == value) return;
				fBorderStyle = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerPopupBorderStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(PopupBorderStyles.Default)]
		public virtual PopupBorderStyles PopupBorderStyle { 
			get { return _popupBorderStyle; }
			set {
				if(PopupBorderStyle == value) return;
				_popupBorderStyle = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerButtonsStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles ButtonsStyle { 
			get { return fButtonsStyle; }
			set {
				if(ButtonsStyle == value) return;
				fButtonsStyle = value;
				OnPropertiesChanged();
			}
		}
		void ResetAppearanceDropDownHeader() { AppearanceDropDownHeader.Reset(); }
		bool ShouldSerializeAppearanceDropDownHeader() { return AppearanceDropDownHeader.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearanceDropDownHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownHeader { get { return appearanceDropDownHeader; }	}
		void ResetAppearanceDropDown() { AppearanceDropDown.Reset(); }
		bool ShouldSerializeAppearanceDropDown() { return AppearanceDropDown.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearanceDropDown"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDown { get { return appearanceDropDown; }	}
		void ResetAppearanceFocused() { AppearanceFocused.Reset(); }
		bool ShouldSerializeAppearanceFocused() { return AppearanceFocused.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearanceFocused"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceFocused { get { return appearanceFocused; }	}
		void ResetAppearanceReadOnly() { AppearanceReadOnly.Reset(); }
		bool ShouldSerializeAppearanceReadOnly() { return AppearanceReadOnly.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearanceReadOnly"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceReadOnly { get { return appearanceReadOnly; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearance"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerAppearanceDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDisabled {
			get { return appearanceDisabled; }
		}
		protected virtual void CreateLookAndFeel() {
			lookAndFeel = new UserLookAndFeel(this);
			lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyAppearance(Appearance);
				DestroyAppearance(AppearanceDisabled);
				DestroyAppearance(AppearanceFocused);
				DestroyAppearance(AppearanceReadOnly);
				DestroyAppearance(AppearanceDropDown);
				DestroyAppearance(AppearanceDropDownHeader);
				if(this.lookAndFeel != null) {
					lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					lookAndFeel.Dispose();
					lookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void OnStyleChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnPropertiesChanged() {
			RaisePropertiesChanged(EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleControllerPropertiesChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler PropertiesChanged {
			add { this.Events.AddHandler(propertiesChanged, value); }
			remove { this.Events.RemoveHandler(propertiesChanged, value); }
		}
		protected virtual void RaisePropertiesChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[propertiesChanged];
			if(handler != null) handler(this, e);
		}
	}
}
