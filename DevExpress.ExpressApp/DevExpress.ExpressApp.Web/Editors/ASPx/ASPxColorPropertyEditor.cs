#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Reflection;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.Web;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxColorPropertyEditor : ASPxEnumIntPropertyEditor<System.Drawing.KnownColor> {
		private WebImageCache imageCache = new WebImageCache();
		private static Dictionary<int, List<KnownColor>> knownColorsList;
		private static List<string> systemColorsNames;
		private static void InitKnownColorsTable() {
			knownColorsList = new Dictionary<int, List<KnownColor>>();
			foreach(KnownColor knownColor in Enum.GetValues(typeof(KnownColor))) {
				int argb = Color.FromKnownColor(knownColor).ToArgb();
				List<KnownColor> colorList;
				if(!knownColorsList.TryGetValue(argb, out colorList)) {
					colorList = new List<KnownColor>();
					knownColorsList.Add(argb, colorList);
				}
				colorList.Add(knownColor);
			}
		}
		private static void InitSystemColorsTable() {
			PropertyInfo[] propertyInfo = typeof(SystemColors).GetProperties();
			systemColorsNames = new List<string>();
			foreach(PropertyInfo property in propertyInfo) {
				if(property.PropertyType == typeof(Color)) {
					systemColorsNames.Add(property.Name);
				}
			}
		}
		private System.Drawing.Image GetImageByColor(Color color) {
			System.Drawing.Image image = new Bitmap(12, 12);
			Graphics gr = Graphics.FromImage(image);
			gr.FillRectangle(new SolidBrush(color), 1, 1, 10, 10);
			MemoryStream stream = new MemoryStream();
			image.Save(stream, ImageFormat.Bmp);
			return new Bitmap(stream);
		}
		protected override Type GetComboBoxValueType() {
			return typeof(String);
		}
		protected override object ConvertEnumValueForComboBox(object enumValue) {
			return Color.FromKnownColor((KnownColor)enumValue).ToArgb();
		}
		protected override object GetControlValueCore() {
			object controlValue = Editor.SelectedItem.Value;
			if(controlValue is Color) {
				return controlValue;
			}
			int argb = Int32.Parse((string)controlValue);
			List<KnownColor> list;
			if(!knownColorsList.TryGetValue(argb, out list)) {
				return Color.FromArgb(argb);
			} 
			foreach(KnownColor knownColor in list) {
				if(!systemColorsNames.Contains(knownColor.ToString())) {
					return Color.FromKnownColor(knownColor);
				}
				return Color.FromKnownColor(list[0]);
			}
			return null;
		}
		protected override ImageInfo GetImageInfo(object enumValue) {
			Color color = Color.FromArgb((int)ConvertEnumValueForComboBox(enumValue));
			return imageCache.GetImageInfo(GetImageByColor(color));
		}
		private static string GetCustomColorString(Color color) {
			return color.R + ";" + color.G + ";" + color.B;
		}
		protected override void ReadEditModeValueCore() {
			if(PropertyValue != null) {
				Color propertyValue = (Color)PropertyValue;
				int argb = propertyValue.ToArgb();
				if(knownColorsList.ContainsKey(argb)) {
					Editor.SelectedIndex = Editor.Items.IndexOfValue(argb.ToString());
				}
				else {
					string name = GetCustomColorString(propertyValue);
					if(Editor.Items.IndexOfText(name) == -1) {
						Editor.Items.Add(name, propertyValue.ToArgb());
					}
					Editor.SelectedIndex = Editor.Items.IndexOfValue(propertyValue.ToArgb().ToString());
				}
			}
		}
		protected override void ReadViewModeValueCore() {
			ASPxImageLabelControl control = (ASPxImageLabelControl)InplaceViewModeEditor;
			if(PropertyValue != null) {
				control.Image.ImageUrl = imageCache.GetImageInfo(GetImageByColor((Color)PropertyValue)).ImageUrl;
			}
			else {
				control.Image.Visible = false;
			}
			control.Text = GetPropertyDisplayValue();
		}
		protected override string GetPropertyDisplayValue() {
			return PropertyValue != null ? GetPropertyTextValue((Color)PropertyValue) : CaptionHelper.NullValueText;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && imageCache != null) {
				imageCache.Dispose();
				imageCache = null;
			}
			base.Dispose(disposing);
		}
		public static string GetPropertyTextValue(Color propertyValue) {
			if(knownColorsList.ContainsKey(propertyValue.ToArgb())) {
				return knownColorsList[propertyValue.ToArgb()][0].ToString();
			}
			return GetCustomColorString(propertyValue); 
		}
		static ASPxColorPropertyEditor() {
			InitKnownColorsTable();
			InitSystemColorsTable();
		}
		public ASPxColorPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) { }
	}
}
