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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.Core.Design.ConditionalFormatting {
	class FormatIconEditor : PropertyValueEditor {
		public FormatIconEditor() {
			InlineEditorTemplate = ConditionalFormattingDesignResourceHelper.Instance.EditorsTemplates["FormatIconEditorTemplate"] as DataTemplate;
		}
	}
	public class FormatIconComboBoxDesigTimeDecorator : ComboBoxDesigTimeDecorator {
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.DropDownOpened += new EventHandler(OnDropDownOpened);
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.DropDownOpened -= new EventHandler(OnDropDownOpened);
		}
		void OnDropDownOpened(object sender, EventArgs e) {
			IFormatsOwner formatsOwner = FindFormatsOwner(DataContext as PropertyValue);
			if(formatsOwner == null)
				return;
			var icons = formatsOwner.PredefinedIconSetFormats.Select(formatInfo => formatInfo.Format)
				.OfType<IconSetFormat>()
				.SelectMany(format => format.Elements)
				.Select(elem => elem.Icon)
				.ToList();
			Control.ItemsSource = icons;
		}
		IFormatsOwner FindFormatsOwner(PropertyValue propertyValue) {
			if(propertyValue == null)
				return null;
			ModelItem selectedItem = propertyValue.ParentProperty.Context.Items.GetValue<Microsoft.Windows.Design.Interaction.Selection>().PrimarySelection;
			ModelItem formatsOwnerItem = BarManagerDesignTimeHelper.FindParentByType<IFormatsOwner>(selectedItem);
			if(formatsOwnerItem == null)
				return null;
			return formatsOwnerItem.GetCurrentValue() as IFormatsOwner;
		}
	}
	static class RegistrationHelper {
		internal static void Register(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(IconSetElement), IconSetElement.IconProperty.Name, PropertyValueEditor.CreateEditorAttribute(typeof(FormatIconEditor)));
		}
	}
	class ConditionalFormattingDesignResourceHelper {
		static ConditionalFormattingDesignResourceHelper instance;
		private ConditionalFormattingDesignResourceHelper() { }
		public static ConditionalFormattingDesignResourceHelper Instance {
			get {
				if(instance == null)
					instance = new ConditionalFormattingDesignResourceHelper();
				return instance;
			}
		}
		ResourceDictionary editorsTemplates;
		public ResourceDictionary EditorsTemplates {
			get {
				if(editorsTemplates == null)
					editorsTemplates = new ResourceDictionary() { Source = new Uri(string.Format("/{0}{1}.Design;component/ConditionalFormatting/ConditionalFormattingEditorsTemplates.xaml", XmlNamespaceConstants.UtilsNamespace, AssemblyInfo.VSuffix), UriKind.Relative) };
				return editorsTemplates;
			}
		}
	}
}
