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
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter)), AutoFormatUrlPropertyClass]
	public class RankProperties : CollectionItem {
		private AppearanceStyleBase fStyle = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("RankPropertiesCssClass"),
#endif
		Category("Appearance"), DefaultValue(""), NotifyParentProperty(true)]
		public string CssClass {
			get { return Style.CssClass; }
			set { Style.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RankPropertiesFont"),
#endif
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public FontInfo Font {
			get { return Style.Font; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RankPropertiesForeColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""),
		 NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter))]
		public Color ForeColor {
			get { return Style.ForeColor; }
			set { Style.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RankPropertiesItemBeginEndTextColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter))]
		public Color ItemBeginEndTextColor {
			get { return GetColorProperty("ItemBeginEndTextColor", Color.Empty); }
			set { SetColorProperty("ItemBeginEndTextColor", Color.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RankPropertiesValueColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""), NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter))]
		public Color ValueColor {
			get { return GetColorProperty("ValueColor", Color.Empty); }
			set { SetColorProperty("ValueColor", Color.Empty, value); }
		}
		protected internal AppearanceStyleBase Style {
			get {
				if (fStyle == null)
					fStyle = new AppearanceStyleBase();
				return fStyle;
			}
		}
		public RankProperties()
			: base() {
		}
		public override void Assign(CollectionItem source) {
			if (source is RankProperties) {
				RankProperties src = source as RankProperties;
				Style.Assign(src.Style);
				ItemBeginEndTextColor = src.ItemBeginEndTextColor;
				ValueColor = src.ValueColor;
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Style };
		}
	}
	[AutoFormatUrlPropertyClass]
	public class RankPropertiesCollection : Collection<RankProperties> {
		protected internal bool IsRankCountLoaded = false;
		public RankPropertiesCollection()
			: base() {
		}
		public RankPropertiesCollection(ASPxCloudControl cloudControl)
			: base(cloudControl) {
		}
		public RankProperties Add() {
			return AddInternal(new RankProperties());
		}
		protected override void OnInsert(int index, object value) {
			if (IsLoading && IsRankCountLoaded) {
				Clear();
				IsRankCountLoaded = false;
			}
			base.OnInsert(index, value);
		}
	}
}
