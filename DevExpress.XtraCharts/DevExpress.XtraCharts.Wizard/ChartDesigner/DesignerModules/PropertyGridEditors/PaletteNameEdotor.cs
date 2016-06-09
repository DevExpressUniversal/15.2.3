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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class PaletteNameUITypeEditor : UITypeEditor {
		const int ColorCount = 6;
		static Chart GetChart(ITypeDescriptorContext context) {
			if (context == null)
				return null;
			var model = context.Instance as DesignerChartModel;
			return model.Chart;
		}
		static PaletteRepository GetPaletteRepository(ITypeDescriptorContext context) {
			Chart chart = GetChart(context);
			if (chart == null)
				return null;
			return context.PropertyDescriptor.Name.StartsWith("Indicators") ? chart.IndicatorsPaletteRepository : chart.PaletteRepository;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			PaletteRepository paletteRepository = GetPaletteRepository(e.Context);
			if (paletteRepository != null) {
				string paletteName = e.Value as string;
				if (!String.IsNullOrEmpty(paletteName)) {
					Graphics gr = e.Graphics;
					Rectangle bounds = e.Bounds;
					Palette palette = paletteRepository[paletteName];
					int count = palette.Count < ColorCount ? palette.Count : ColorCount;
					int x = bounds.X;
					for (int i = 0; i < count; i++) {
						int width = (int)Math.Round((double)(bounds.Right - x) / (count - i));
						using (Brush brush = new SolidBrush(palette[i].Color))
							gr.FillRectangle(brush, new Rectangle(x, bounds.Y, width, bounds.Height));
						x += width;
					}
					return;
				}
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null)
				return val;
			PaletteRepository paletteRepository = GetPaletteRepository(context);
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (paletteRepository == null || edSvc == null)
				return val;
			using (PaletteEditControl ctrl = new PaletteEditControl(edSvc)) {
				ctrl.PaletteRepository = paletteRepository;
				Chart chart = GetChart(context);
				ctrl.SelectedPalette = context.PropertyDescriptor.Name.StartsWith("Indicators") ? chart.IndicatorsPalette : chart.Palette;
				edSvc.DropDownControl(ctrl);
				Palette currentPalette = ctrl.SelectedPalette;
				return currentPalette == null ? val : currentPalette.Name;
			}
		}
	}
}
