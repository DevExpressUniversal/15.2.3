#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum DashboardTitleAlignment {
		Left,
		Center
	}
	public class DashboardTitle {
		const string xmlVisible = "Visible";
		const string xmlText = "Text";
		const string xmlAlignment = "Alignment";
		const string xmlIncludeMasterFilter = "IncludeMasterFilterState";
		const DashboardTitleAlignment DefaultAlignment = DashboardTitleAlignment.Center;
		const bool DefaultIncludeMasterFilterState = true;
		const bool DefaultVisible = true;
		readonly Dashboard dashboard;
		readonly DashboardImage image = new DashboardImage();
		string text;
		bool visible = DefaultVisible;
		DashboardTitleAlignment alignment = DefaultAlignment;
		bool includeMasterFilterState = DefaultIncludeMasterFilterState;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitleVisible"),
#endif
		DefaultValue(DefaultVisible)
		]
		public bool Visible {
			get { return visible; }
			set {
				if(value != visible) {
					visible = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitleText"),
#endif
		DefaultValue(null),
		Localizable(true)
		]
		public string Text {
			get { return text; }
			set {
				if(value != text) {
					text = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitleAlignment"),
#endif
		DefaultValue(DefaultAlignment)
		]
		public DashboardTitleAlignment Alignment {
			get { return alignment; }
			set {
				if(value != alignment) {
					alignment = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultIncludeMasterFilterState)
		]
		public bool ShowMasterFilterState {
			get { return includeMasterFilterState; }
			set {
				if(value != includeMasterFilterState) {
					includeMasterFilterState = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitleImageUrl"),
#endif
		Category(CategoryNames.General),
		Editor(TypeNames.ImageFileNameEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string ImageUrl { get { return image.Url; } set { image.Url = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitleImageData"),
#endif
		Category(CategoryNames.General),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(TypeNames.DisplayNameNoneObjectConverter),
		Editor(TypeNames.ImageDataEditor, typeof(UITypeEditor))
		]
		public byte[] ImageData { get { return image.Data; } set { image.Data = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string ImageDataSerializable { get { return image.Base64Data; } set { image.Base64Data = value; } }
		internal DashboardTitle(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			this.dashboard = dashboard;
			image.Changed += (sender, e) => OnChanged();
			text = DashboardLocalizer.GetString(DashboardStringId.DefaultDashboardTitleText);
		}
		internal DashboardTitleViewModel CreateViewModel() {
			DashboardTitleViewModel viewModel = new DashboardTitleViewModel { Text = text };
			viewModel.LayoutModel = CreateTitleLayoutViewModel();
			viewModel.IncludeMasterFilterValues = dashboard.Title.ShowMasterFilterState;
			viewModel.ShowParametersButton = dashboard.Parameters.Any<DashboardParameter>((p) => p.Visible);
			viewModel.Visible = Visible;
			return viewModel;
		}
		internal TitleLayoutViewModel CreateTitleLayoutViewModel() {
			TitleLayoutViewModel viewModel = new TitleLayoutViewModel { Alignment = alignment };
			if(image.HasImage)
				viewModel.ImageViewModel = new DashboardImageViewModel(image);
			return viewModel;
		}
		internal string GetExportTitle() {
			return Text;
		}
		internal void SaveToXml(XElement element) {
			if(visible != DefaultVisible)
				element.Add(new XAttribute(xmlVisible, visible));
			element.Add(new XAttribute(xmlText, text));
			if(alignment != DefaultAlignment)
				element.Add(new XAttribute(xmlAlignment, alignment));
			if(includeMasterFilterState != DefaultIncludeMasterFilterState)
				element.Add(new XAttribute(xmlIncludeMasterFilter, includeMasterFilterState));
			image.SaveToXml(element);
		}
		internal void LoadFromXml(XElement element) {
			string visibleAttr = element.GetAttributeValue(xmlVisible);
			if(!string.IsNullOrEmpty(visibleAttr))
				visible = XmlHelper.FromString<bool>(visibleAttr);
			string textAttr = element.GetAttributeValue(xmlText);
			if(textAttr != null)
				this.text = textAttr;
			string alignmentAttr = element.GetAttributeValue(xmlAlignment);
			if(!string.IsNullOrEmpty(alignmentAttr))
				alignment = XmlHelper.EnumFromString<DashboardTitleAlignment>(alignmentAttr);
			string includeMasterFilterStateAttr = element.GetAttributeValue(xmlIncludeMasterFilter);
			if(!String.IsNullOrEmpty(includeMasterFilterStateAttr))
				includeMasterFilterState = XmlHelper.FromString<bool>(includeMasterFilterStateAttr);
			image.LoadFromXml(element);
		}
		void OnChanged() {
			if(dashboard != null)
				dashboard.OnTitleChanged();
		}
	}
}
