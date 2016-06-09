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

using DevExpress.Web;
using DevExpress.Web.Mvc.Internal;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	public class FormLayoutExtension<ModelType>: ExtensionBase {
		public FormLayoutExtension(FormLayoutSettings<ModelType> settings) :
			base(settings) {
		}
		public FormLayoutExtension(FormLayoutSettings<ModelType> settings, ViewContext viewContext) :
			base(settings, viewContext) {
		}
		protected internal new MVCxFormLayout Control {
			get { return (MVCxFormLayout)base.Control; }
		}
		protected internal new FormLayoutSettings<ModelType> Settings {
			get { return (FormLayoutSettings<ModelType>)base.Settings; }
		}
		protected override void ApplySettings(SettingsBase settings) {
			ConfigureLayoutItems(((FormLayoutSettings<ModelType>)settings).Items);
			base.ApplySettings(settings);
		}
		void ConfigureLayoutItems(LayoutItemCollection items) {
			foreach(LayoutItemBase item in items) {
				FormLayoutItemHelper.ConfigureByMetadata(item);
				var groupItem = item as LayoutGroupBase;
				if(groupItem != null)
					ConfigureLayoutItems(groupItem.Items);
			}
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AlignItemCaptions = Settings.AlignItemCaptions;
			Control.AlignItemCaptionsInAllGroups = Settings.AlignItemCaptionsInAllGroups;
			Control.LeftAndRightCaptionsWidth = Settings.LeftAndRightCaptionsWidth;
			Control.ColCount = Settings.ColCount;
			Control.Items.Assign(Settings.Items);
			Control.SettingsItems.Assign(Settings.SettingsItems);
			Control.SettingsItemCaptions.Assign(Settings.SettingsItemCaptions);
			Control.SettingsItemHelpTexts.Assign(Settings.SettingsItemHelpTexts);
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ShowItemCaptionColon  = Settings.ShowItemCaptionColon;
			Control.RequiredMarkDisplayMode = Settings.RequiredMarkDisplayMode;
			Control.RequiredMark = Settings.RequiredMark;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.OptionalMark = Settings.OptionalMark;
			Control.RightToLeft = Settings.RightToLeft;
			Control.NestedControlWidth = Settings.NestedExtensionWidth;
			Control.UseDefaultPaddings = Settings.UseDefaultPaddings;
			Control.SettingsAdaptivity.Assign(Settings.SettingsAdaptivity);
			Control.LayoutItemDataBinding += Settings.LayoutItemDataBinding;
			Control.LayoutItemDataBound += Settings.LayoutItemDataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			AssignNestedContent(Control.Items);
		}
		void AssignNestedContent(LayoutItemCollection items) {
			if(items == null)
				return;
			for(var i = 0; i < items.Count; i++) {
				if(items[i] is LayoutGroupBase)
					AssignNestedContent(((LayoutGroupBase)items[i]).Items);
				if(items[i] is MVCxFormLayoutItem)
					TryToCreateNestedContent((MVCxFormLayoutItem)items[i]);
			}
		}
		void TryToCreateNestedContent(MVCxFormLayoutItem item) {
			if(item.HasNestedContentTemplate) {
				item.Controls.Clear();
				item.Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(item.NestedContent, item.NestedContentMethod));
				return;
			}
			Control.BindLayoutItem(item, null);
			if(item.NestedExtensionInst != null) {
				ApplyThemeToNestedExtension(item.NestedExtensionInst);
				item.Controls.Add(DevExpress.Web.Mvc.Internal.ContentControl.Create(() => item.NestedExtensionInst.Render()));
			}
		}
		void ApplyThemeToNestedExtension(ExtensionBase extension) {
			if(!extension.Settings.EnableTheming)
				return;
			string theme = !string.IsNullOrEmpty(extension.Settings.Theme) ? extension.Settings.Theme : Settings.Theme;
			ApplyThemeToControl(extension.Control, theme);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareNestedControlsRecursive(Control.Items);
		}
		void PrepareNestedControlsRecursive(LayoutItemCollection items) {
			foreach(LayoutItemBase item in items) {
				FormLayoutItemHelper.ConfigureByMetadata(item);
				var groupItem = item as LayoutGroupBase;
				if(groupItem != null)
					PrepareNestedControlsRecursive(groupItem.Items);
				var layoutItem = item as MVCxFormLayoutItem;
				if(layoutItem != null) {
					var comboBox = layoutItem.GetNestedControl() as ASPxComboBox;
					if(comboBox != null && comboBox.Items.Count == 0)
						NestedControlHelper.PrepareControl(comboBox, layoutItem.DataType);
				}
			}
		}
		public FormLayoutExtension<ModelType> Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxFormLayout();
		}
	}
}
