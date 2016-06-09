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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ResourceNavigatorStyles : StylesBase {
		public const string ButtonStyleName = "Button";
		public const string ComboBoxStyleName = "ComboBox";
		public const string ComboBoxListStyleName = "ComboBoxList";
		public const string ComboBoxItemStyleName = "ComboBoxItem";
		Paddings paddings = new Paddings();
		public ResourceNavigatorStyles(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ButtonControlStyle Button {
			get { return (ButtonControlStyle)GetStyle(ButtonStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public EditStyleBase ComboBox {
			get { return (EditStyleBase)GetStyle(ComboBoxStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase ComboBoxList {
			get { return GetStyle(ComboBoxListStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ListBoxItemStyle ComboBoxItem {
			get { return (ListBoxItemStyle)GetStyle(ComboBoxItemStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public Paddings Paddings {
			get { return paddings; }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ComboBoxSpacing {
			get { return GetUnitProperty("ComboBoxSpacing", Unit.Empty); }
			set { SetUnitProperty("ComboBoxSpacing", Unit.Empty, value); }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ButtonSpacing {
			get { return GetUnitProperty("ButtonSpacing", Unit.Empty); }
			set { SetUnitProperty("ButtonSpacing", Unit.Empty, value); }
		}
		#endregion
		protected override string GetCssClassNamePrefix() {
			return "dxsc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(ComboBoxStyleName, delegate() { return new EditStyleBase(); } ));
			list.Add(new StyleInfo(ComboBoxListStyleName, delegate() { return new AppearanceStyleBase(); } ));
			list.Add(new StyleInfo(ComboBoxItemStyleName, delegate() { return new ListBoxItemStyle(); } ));
		}
		protected internal new AppearanceStyleBase GetDefaultControlStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ResourceNavigator));
			return style;
		}
		protected internal ButtonControlStyle GetButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(Button);
			return style;
		}
		protected internal EditStyleBase GetComboStyle() {
			EditStyleBase style = new EditStyleBase();
			style.CopyFrom(ComboBox);
			return style;
		}
		protected internal AppearanceStyleBase GetComboListStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(ComboBoxList);
			return style;
		}
		protected internal ListBoxItemStyle GetComboItemStyle() {
			ListBoxItemStyle style = new ListBoxItemStyle();
			style.CopyFrom(ComboBoxItem);
			return style;
		}
		protected internal Unit GetComboSpacing() {
			return ComboBoxSpacing.IsEmpty ? 60 : ComboBoxSpacing;
		}
		protected internal Unit GetButtonSpacing() {
			return ButtonSpacing.IsEmpty ? 1 : ButtonSpacing;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Paddings });
		}
		public override void Reset() {
			base.Reset();
			ButtonSpacing = Unit.Empty;
			ComboBoxSpacing = Unit.Empty;
			Paddings.Reset();
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			ResourceNavigatorStyles src = source as ResourceNavigatorStyles;
			if (src != null) {
				ButtonSpacing = src.ButtonSpacing;
				ComboBoxSpacing = src.ComboBoxSpacing;
				Paddings.CopyFrom(src.Paddings);
			}
		}
	}
}
