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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class FilterControlStyleBase : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class FilterControlTableStyle : FilterControlStyleBase { }
	public class FilterControlLinkStyle : FilterControlStyleBase { }
	public class FilterControlImageButtonStyle : FilterControlStyleBase { }
	public class FilterControlStyles : StylesBase {
		public const string TableStyleName = "Table";
		public const string PropertyNameStyleName = "PropertyName";
		public const string GroupTypeStyleName = "GroupType";
		public const string OperationStyleName = "Operation";
		public const string ValueStyleName = "Value";
		public const string ImageButtonStyleName = "ImageButton";
		protected internal const string FilterControlPrefix = "dxfc";
		public FilterControlStyles(ISkinOwner skinOwner) : base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(TableStyleName, delegate() { return new FilterControlTableStyle(); }));
			list.Add(new StyleInfo(PropertyNameStyleName, delegate() { return new FilterControlLinkStyle(); }));
			list.Add(new StyleInfo(GroupTypeStyleName, delegate() { return new FilterControlLinkStyle(); }));
			list.Add(new StyleInfo(OperationStyleName, delegate() { return new FilterControlLinkStyle(); }));
			list.Add(new StyleInfo(ValueStyleName, delegate() { return new FilterControlLinkStyle(); }));
			list.Add(new StyleInfo(ImageButtonStyleName, delegate() { return new FilterControlImageButtonStyle(); }));
		}
		protected internal override string GetCssClassNamePrefix() { return FilterControlPrefix; }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesTable"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlTableStyle Table { get { return (FilterControlTableStyle)GetStyle(TableStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesPropertyName"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlLinkStyle PropertyName { get { return (FilterControlLinkStyle)GetStyle(PropertyNameStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesGroupType"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlLinkStyle GroupType { get { return (FilterControlLinkStyle)GetStyle(GroupTypeStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesOperation"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlLinkStyle Operation { get { return (FilterControlLinkStyle)GetStyle(OperationStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesValue"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlLinkStyle Value { get { return (FilterControlLinkStyle)GetStyle(ValueStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlStylesImageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterControlImageButtonStyle ImageButton { get { return (FilterControlImageButtonStyle)GetStyle(ImageButtonStyleName); } }
	}
}
