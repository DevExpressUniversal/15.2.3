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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors.Design {
	public static class FilterPropertiesHelper {
		public static PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection properties, string[] filterPropertiesNames) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in properties) {
				bool found = false;
				foreach (string propertyName in filterPropertiesNames) {
					if (propertyName == desc.Name) {
						found = true;
						break;
					}
				}
				if (!found)
					result.Add(desc);
			}
			return new PropertyDescriptorCollection(result.ToArray());
		}
	}
	public class ChartRangeControlClientGridOptionsTypeConverter : ExpandableObjectConverter {
		static string[] manualModeProperties = new string[] { "GridSpacing", "SnapSpacing", "GridAlignment", "SnapAlignment", "ShowGridlinesErrorMessage" };
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			ChartRangeControlClientGridOptions options = value as ChartRangeControlClientGridOptions;
			if (options != null && options.Auto)
				return FilterPropertiesHelper.FilterProperties(collection, manualModeProperties);
			return collection;
		}
	}
	public class ChartRangeControlClientRangeTypeConverter : ExpandableObjectConverter {
		static string[] manualModeProperties = new string[] { "Min", "Max" };
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			ChartRangeControlClientRange range = value as ChartRangeControlClientRange;
			if (range != null && range.Auto)
				return FilterPropertiesHelper.FilterProperties(collection, manualModeProperties);
			return collection;
		}
	}
	public class ChartRangeControlClientPaletteTypeEditor : UITypeEditor {
		const int ColorCount = 6;
		static Size DropDownPreviewIconSize = new Size(64, 16);
		void DrawPalette(ChartRangeControlClientPalette palette, Graphics graphics, Rectangle bounds) {
			int count = palette.Count < ColorCount ? palette.Count : ColorCount;
			int x = bounds.X;
			for (int i = 0; i < count; i++) {
				int width = (int)Math.Round((double)(bounds.Right - x) / (count - i));
				using (Brush brush = new SolidBrush(palette[i].Color))
					graphics.FillRectangle(brush, new Rectangle(x, bounds.Y, width, bounds.Height));
				x += width;
			}
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			string paletteName = e.Value as string;
			if (!String.IsNullOrEmpty(paletteName)) {
				DrawPalette(ChartRangeControlClientPalette.GetPalette(paletteName), e.Graphics, e.Bounds);
				return;
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null)
				return val;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edSvc == null)
				return val;
			using (ImageListBoxControl listBox = new ImageListBoxControl()) {
				listBox.Click += (s, a) => {
					edSvc.CloseDropDown();
				};
				ImageCollection images = new ImageCollection();
				images.ImageSize = DropDownPreviewIconSize;
				ImageListBoxItem selection = null;
				for (int i = 0; i < ChartRangeControlClientPalette.PredefinedPalettes.Count; i++) {
					ChartRangeControlClientPalette palette = ChartRangeControlClientPalette.PredefinedPalettes[i];
					Bitmap bitmap = new Bitmap(DropDownPreviewIconSize.Width, DropDownPreviewIconSize.Height);
					using (Graphics graphics = Graphics.FromImage(bitmap)) {
						DrawPalette(palette, graphics, new Rectangle(new Point(0, 0), DropDownPreviewIconSize));
					}
					ImageListBoxItem item = new ImageListBoxItem(palette, i);
					listBox.Items.Add(item);
					images.AddImage(bitmap);
					if (palette == ChartRangeControlClientPalette.GetPalette(val.ToString()))
						selection = item;
				}
				listBox.ImageList = images;
				listBox.SelectedItem = selection;
				edSvc.DropDownControl(listBox);
				selection = listBox.SelectedItem as ImageListBoxItem;
				ChartRangeControlClientPalette selectedPalette = selection.Value as ChartRangeControlClientPalette;
				return (selectedPalette == null) ? val : selectedPalette.Name;
			}
		}
	}
	public class ChartRangeControlClientPaletteTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(ChartRangeControlClientPalette.PredefinedPalettes);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if ((value is string) && (destinationType == typeof(string)))
				return ChartRangeControlClientPalette.GetPalette(value.ToString()).DisplayName;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ChartRangeControlClientTemplatedColorTypeEditor : UITypeEditor {
		void DrawPaletteSimulation(Graphics gr, Rectangle bounds) {
			Color[] palette = new Color[] { 
				Color.FromArgb(241,97,86), 
				Color.FromArgb(252,232,86), 
				Color.FromArgb(215,74,255), 
				Color.FromArgb(119,224,121), 
				Color.FromArgb(80,142,224), 
				Color.FromArgb(53,185,193)
			};
			int stepX = (int)Math.Floor((double)bounds.Width / 3);
			int stepY = (int)Math.Floor((double)bounds.Height / 2);
			int paletteIndex = 0;
			for (int x = 0; x < 2; x++) 
				for (int y = 0; y < 2; y++) {
					Rectangle cell = new Rectangle(bounds.X + x * stepX, bounds.Y + y * stepY, stepX, stepY);
					using (SolidBrush brush = new SolidBrush(palette[paletteIndex++ % palette.Length]))
						gr.FillRectangle(brush, cell);
				}
			for (int y = 0; y < 2; y++) {
				Rectangle cell = new Rectangle(bounds.X + stepX * 2, bounds.Y + stepY * y, bounds.Width - (stepX * 2), stepY);
				using (SolidBrush brush = new SolidBrush(palette[paletteIndex++ % palette.Length]))
					gr.FillRectangle(brush, cell);
			}
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			string propertyName = (e.Context != null) ? ((e.Context.PropertyDescriptor != null) ? e.Context.PropertyDescriptor.Name : null) : null;
			if (e.Value is Color) {
				Color color = (Color)e.Value;
				if ((propertyName == "Color") && (color == ChartRangeControlClientView.DefaultColor))
					DrawPaletteSimulation(e.Graphics, e.Bounds);
				if ((propertyName == "MarkerColor") && (color == LineChartRangeControlClientView.DefaultMarkerColor))
					DrawPaletteSimulation(e.Graphics, e.Bounds);
				else
					using (Brush fill = new SolidBrush(color))
						e.Graphics.FillRectangle(fill, e.Bounds);
				return;
			}
			base.PaintValue(e);
		}
	}
	public class ChartRangeControlClientTemplatedColorTypeConverter : ColorConverter {
		const string UsingPalette = "(Using palette)";
		const string ViewColorProperty = "Color";
		const string LineViewMarkerColorProperty = "MarkerColor";
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string propertyName = (context != null) ? ((context.PropertyDescriptor != null) ? context.PropertyDescriptor.Name : null) : null;
			if (!string.IsNullOrEmpty(propertyName) && string.Equals(UsingPalette, (string)value)) {
				if (propertyName == ViewColorProperty)
					return ChartRangeControlClientView.DefaultColor;
				else if (propertyName == LineViewMarkerColorProperty)
					return LineChartRangeControlClientView.DefaultMarkerColor;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			string propertyName = (context != null) ? ((context.PropertyDescriptor != null) ? context.PropertyDescriptor.Name : null) : null;
			if ((value is Color) && (destinationType == typeof(string)) && !string.IsNullOrEmpty(propertyName)) {
				Color color = (Color)value;
				if ((propertyName == ViewColorProperty) && (color == ChartRangeControlClientView.DefaultColor))
					return UsingPalette;
				if ((propertyName == LineViewMarkerColorProperty) && (color == LineChartRangeControlClientView.DefaultMarkerColor))
					return UsingPalette;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
