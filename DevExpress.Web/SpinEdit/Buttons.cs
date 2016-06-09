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
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum SpinButtonKind { Increment, Decrement, LargeIncrement, LargeDecrement }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SpinButtons : EditButton {
		private IWebControlObject owner = null;
		private ButtonImageProperties incrementImage = null;
		private ButtonImageProperties decrementImage = null;
		private ButtonImageProperties largeIncrementImage = null;
		private ButtonImageProperties largeDecrementImage = null;
		public SpinButtons()
			: base() {
		}
		public SpinButtons(IWebControlObject owner)
			: base() {
			this.owner = owner;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ButtonImageProperties Image {
			get { return base.Image; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePosition ImagePosition {
			get { return base.ImagePosition; }
			set { base.ImagePosition = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsIncrementImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties IncrementImage {
			get {
				if(incrementImage == null)
					incrementImage = new ButtonImageProperties(this);
				return incrementImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsDecrementImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties DecrementImage {
			get {
				if(decrementImage == null)
					decrementImage = new ButtonImageProperties(this);
				return decrementImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsLargeIncrementImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonImageProperties LargeIncrementImage {
			get {
				if(largeIncrementImage == null)
					largeIncrementImage = new ButtonImageProperties(this);
				return largeIncrementImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsLargeDecrementImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonImageProperties LargeDecrementImage {
			get {
				if(largeDecrementImage == null)
					largeDecrementImage = new ButtonImageProperties(this);
				return largeDecrementImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsShowLargeIncrementButtons"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowLargeIncrementButtons {
			get { return GetBoolProperty("ShowLargeIncrementButtons", false); }
			set {
				SetBoolProperty("ShowLargeIncrementButtons", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsShowIncrementButtons"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowIncrementButtons {
			get { return GetBoolProperty("ShowIncrementButtons", true); }
			set {
				SetBoolProperty("ShowIncrementButtons", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinButtonsHorizontalSpacing"),
#endif
		Obsolete("This property is now obsolete. Use the editor's Spacing property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(EditorStyles.DefaultSpinButtonsHorizontalSpacing), NotifyParentProperty(true), AutoFormatEnable]
		public virtual int HorizontalSpacing {
			get { return GetIntProperty("HorizontalSpacing", EditorStyles.DefaultSpinButtonsHorizontalSpacing); }
			set { SetIntProperty("HorizontalSpacing", EditorStyles.DefaultSpinButtonsHorizontalSpacing, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			SpinButtons src = source as SpinButtons;
			if (src != null) {
				IncrementImage.Assign(src.IncrementImage);
				DecrementImage.Assign(src.DecrementImage);
				LargeIncrementImage.Assign(src.LargeIncrementImage);
				LargeDecrementImage.Assign(src.LargeDecrementImage);
				ShowLargeIncrementButtons = src.ShowLargeIncrementButtons;
				ShowIncrementButtons = src.ShowIncrementButtons;
			}
		}
		protected internal override ImageProperties GetDefaultImage(Page page, EditorImages images, bool rtl) {
			return null;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { IncrementImage, DecrementImage, 
					LargeIncrementImage, LargeDecrementImage });
		}
		protected override bool IsDesignMode() {
			if (this.owner != null)
				return this.owner.IsDesignMode();
			return false;
		}
		protected override bool IsLoading() {
			if (this.owner != null)
				return this.owner.IsLoading();
			return false;
		}
		protected override void LayoutChanged() {
			if (this.owner != null)
				this.owner.LayoutChanged();
		}
		protected override void TemplatesChanged() {
			if (this.owner != null)
				this.owner.TemplatesChanged();
		}
		public override string ToString() {
			return "";
		}
	}
	public class SpinButtonExtended : EditButton {
		SpinButtonKind buttonKind;
		public SpinButtonExtended()
			: base() {
		}
		public SpinButtonExtended(SpinButtonKind buttonKind)
			: base() {
			this.buttonKind = buttonKind;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePosition ImagePosition {
			get { return base.ImagePosition; }
			set { base.ImagePosition = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("SpinButtonExtendedButtonKind")]
#endif
		public SpinButtonKind ButtonKind {
			get { return buttonKind; }
		}
		protected internal override ImageProperties GetDefaultImage(Page page, EditorImages images, bool rtl) {
			if(ButtonKind == SpinButtonKind.LargeDecrement && !rtl || ButtonKind == SpinButtonKind.LargeIncrement && rtl)
				return images.GetImageProperties(page, EditorImages.SpinEditLargeDecrementImageName);
			if(ButtonKind == SpinButtonKind.LargeIncrement && !rtl || ButtonKind == SpinButtonKind.LargeDecrement && rtl)
				return images.GetImageProperties(page, EditorImages.SpinEditLargeIncrementImageName);
			if(ButtonKind == SpinButtonKind.Increment)
				return images.GetImageProperties(page, EditorImages.SpinEditIncrementImageName);
			if(ButtonKind == SpinButtonKind.Decrement)
				return images.GetImageProperties(page, EditorImages.SpinEditDecrementImageName);
			return null;
		}
	}
	public class SimpleSpinButtons : SpinButtons {
		public SimpleSpinButtons()
			: this(null) {
		}
		public SimpleSpinButtons(IWebControlObject owner)
			: base(owner) {
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ButtonImageProperties LargeDecrementImage {
			get { return base.LargeDecrementImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ButtonImageProperties LargeIncrementImage {
			get { return base.LargeIncrementImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowLargeIncrementButtons {
			get { return base.ShowLargeIncrementButtons; }
			set { base.ShowLargeIncrementButtons = value; }
		}
		#endregion
	}
}
