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
using System.Data;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Text;
using System.Drawing.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[AutoFormatUrlPropertyClass]
	public class HeadlineSettings : PropertiesBase, IPropertiesOwner {
		protected internal const string DefaultDateFormatString = "{0:d}";
		private HeadlineTailImageProperties fTailImage = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsDateFormatString"),
#endif
		DefaultValue(DefaultDateFormatString), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string DateFormatString {
			get { return GetStringProperty("DateFormatString", DefaultDateFormatString); }
			set {
				SetStringProperty("DateFormatString", DefaultDateFormatString, value);
				Changed();
			}
		}
		static object dateHorizontalPositionLeft = DateHorizontalPosition.Left;
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsDateHorizontalPosition"),
#endif
		DefaultValue(DateHorizontalPosition.Left), NotifyParentProperty(true), AutoFormatEnable]
		public DateHorizontalPosition DateHorizontalPosition {
			get { return (DateHorizontalPosition)GetEnumProperty("DateHorizontalPosition", dateHorizontalPositionLeft); }
			set {
				SetEnumProperty("DateHorizontalPosition", dateHorizontalPositionLeft, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsDateVerticalPosition"),
#endif
		DefaultValue(DateVerticalPosition.BelowHeader), NotifyParentProperty(true), AutoFormatEnable]
		public virtual DateVerticalPosition DateVerticalPosition {
			get { return (DateVerticalPosition)GetEnumProperty("DateVerticalPosition", DateVerticalPosition.BelowHeader); }
			set {
				SetEnumProperty("DateVerticalPosition", DateVerticalPosition.BelowHeader, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsImagePosition"),
#endif
		DefaultValue(HeadlineImagePosition.Left), NotifyParentProperty(true), AutoFormatEnable]
		public HeadlineImagePosition ImagePosition {
			get { return (HeadlineImagePosition)GetEnumProperty("ImagePosition", HeadlineImagePosition.Left); }
			set {
				SetEnumProperty("ImagePosition", HeadlineImagePosition.Left, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsMaxLength"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int MaxLength {
			get { return GetIntProperty("MaxLength", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaxLength");
				SetIntProperty("MaxLength", 0, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsShowContentAsLink"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowContentAsLink {
			get { return GetBoolProperty("ShowContentAsLink", false); }
			set {
				SetBoolProperty("ShowContentAsLink", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsShowHeaderAsLink"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowHeaderAsLink {
			get { return GetBoolProperty("ShowHeaderAsLink", false); }
			set {
				SetBoolProperty("ShowHeaderAsLink", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsShowImageAsLink"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowImageAsLink {
			get { return GetBoolProperty("ShowImageAsLink", false); }
			set {
				SetBoolProperty("ShowImageAsLink", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsTailImagePosition"),
#endif
		DefaultValue(TailImagePosition.AfterTailText), NotifyParentProperty(true), AutoFormatEnable]
		public TailImagePosition TailImagePosition {
			get { return (TailImagePosition)GetEnumProperty("TailImagePosition", TailImagePosition.AfterTailText); }
			set {
				SetEnumProperty("TailImagePosition", TailImagePosition.AfterTailText, value);
				Changed();
			}
		}
		static object tailPositionInline = TailPosition.Inline;
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsTailPosition"),
#endif
		DefaultValue(TailPosition.Inline), NotifyParentProperty(true), AutoFormatEnable]
		public TailPosition TailPosition {
			get { return (TailPosition)GetEnumProperty("TailPosition", tailPositionInline); }
			set {
				SetEnumProperty("TailPosition", tailPositionInline, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsTailText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string TailText {
			get { return GetStringProperty("TailText", ""); }
			set {
				SetStringProperty("TailText", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsTarget"),
#endif
		DefaultValue(""), TypeConverter(typeof(TargetConverter)), Localizable(false),
		AutoFormatDisable, NotifyParentProperty(true)]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set {
				SetStringProperty("Target", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsTailImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true)]
		public HeadlineTailImageProperties TailImage {
			get {
				if(fTailImage == null)
					fTailImage = new HeadlineTailImageProperties(this);
				return fTailImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineSettingsShowContentInToolTip"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowContentInToolTip {
			get { return GetBoolProperty("ShowContentInToolTip", false); }
			set { SetBoolProperty("ShowContentInToolTip", false, value); }
		}
		public HeadlineSettings()
			: base() {
		}
		public HeadlineSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is HeadlineSettings) {
					HeadlineSettings src = source as HeadlineSettings;
					DateFormatString = src.DateFormatString;
					DateHorizontalPosition = src.DateHorizontalPosition;
					DateVerticalPosition = src.DateVerticalPosition;
					ImagePosition = src.ImagePosition;
					MaxLength = src.MaxLength;
					ShowContentAsLink = src.ShowContentAsLink;
					ShowHeaderAsLink = src.ShowHeaderAsLink;
					ShowImageAsLink = src.ShowImageAsLink;
					ShowContentInToolTip = src.ShowContentInToolTip;
					TailImage.Assign(src.TailImage);
					TailImagePosition = src.TailImagePosition;
					TailPosition = src.TailPosition;
					TailText = src.TailText;
					Target = src.Target;
					ToolTip = src.ToolTip;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { TailImage };
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
}
