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

using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Utils;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region RichEditUnderlineComboBoxEdit
	[DXToolboxBrowsableAttribute(false)]
	public class RichEditUnderlineComboBoxEdit : ComboBoxEdit {
		public RichEditUnderlineComboBoxEdit()
			: base() {
			IsTextEditable = false;
			ApplyItemTemplateToSelectedItem = true;
			Populate();
		}
		#region Properties
		public UnderlineType? Value {
			get {
				RichEditUnderlineComboBoxEditItem item = EditValue as RichEditUnderlineComboBoxEditItem;
				if (item != null)
					return item.UnderlineType;
				else
					return null;
			}
			set {
				if (value.HasValue) {
					foreach (object obj in Items) {
						RichEditUnderlineComboBoxEditItem item = obj as RichEditUnderlineComboBoxEditItem;
						if (item != null && item.UnderlineType == value) {
							EditValue = item;
							break;
						}
					}
				}
				else
					EditValue = null;
			}
		}
		#endregion
		protected virtual void Populate() {
			using (DocumentModel documentModel = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				Items.Clear(); 
				foreach (var prop in typeof(UnderlineType).GetFields()) {
					if (prop.MemberType == System.Reflection.MemberTypes.Field && prop.FieldType == typeof(UnderlineType)) {
						UnderlineType val = (UnderlineType)(typeof(UnderlineType).GetField(prop.Name).GetValue(new UnderlineType()));
						if (val != UnderlineType.DashSmallGap) {
							Items.Add(new RichEditUnderlineComboBoxEditItem() { UnderlineType = val, Content = GenerateItemContent(documentModel, val, prop.Name, DXColor.Black) });
						}
					}
				}
			}
		}
		protected internal static FrameworkElement GenerateItemContent(DocumentModel documentModel, UnderlineType val, string name, PlatformIndependentColor color) {
			Grid grid = new Grid() { Margin = new Thickness(3, 0, 3, 0) };
			TextBlock textBlock = new TextBlock();
			textBlock.Text = " ";
			grid.Children.Add(textBlock);
			Painter painter = new XpfSlowPainter(new DocumentLayoutUnitDocumentConverter(), grid.Children);
			GraphicsDocumentLayoutExporterAdapter adapter = new XpfGraphicsDocumentLayoutExporterAdapter();
			ICharacterLinePainter ex = new GraphicsDocumentLayoutExporter(documentModel, painter, adapter, new System.Drawing.Rectangle(), TextColors.Defaults);
			RectangleF thinRect = new RectangleF(0, 0, 500, 5);
			RectangleF thickRect = new RectangleF(0, 0, 500, 10);
			if (val == UnderlineType.Single) ex.DrawUnderline(new UnderlineSingle(), thinRect, color);
			else if (val == UnderlineType.DashDotDotted) ex.DrawUnderline(new UnderlineDashDotDotted(), thinRect, color);
			else if (val == UnderlineType.DashDotted) ex.DrawUnderline(new UnderlineDashDotted(), thinRect, color);
			else if (val == UnderlineType.Dashed) ex.DrawUnderline(new UnderlineDashed(), thinRect, color);
			else if (val == UnderlineType.Dotted) ex.DrawUnderline(new UnderlineDotted(), thinRect, color);
			else if (val == UnderlineType.Double) ex.DrawUnderline(new UnderlineDouble(), thinRect, color);
			else if (val == UnderlineType.DoubleWave) ex.DrawUnderline(new UnderlineDoubleWave(), thinRect, color);
			else if (val == UnderlineType.HeavyWave) ex.DrawUnderline(new UnderlineHeavyWave(), thinRect, color);
			else if (val == UnderlineType.LongDashed) ex.DrawUnderline(new UnderlineLongDashed(), thinRect, color);
			else if (val == UnderlineType.ThickDashDotDotted) ex.DrawUnderline(new UnderlineThickDashDotDotted(), thickRect, color);
			else if (val == UnderlineType.ThickDashDotted) ex.DrawUnderline(new UnderlineThickDashDotted(), thickRect, color);
			else if (val == UnderlineType.ThickDashed) ex.DrawUnderline(new UnderlineThickDashed(), thickRect, color);
			else if (val == UnderlineType.ThickDotted) ex.DrawUnderline(new UnderlineThickDotted(), thickRect, color);
			else if (val == UnderlineType.ThickLongDashed) ex.DrawUnderline(new UnderlineThickLongDashed(), thickRect, color);
			else if (val == UnderlineType.ThickSingle) ex.DrawUnderline(new UnderlineThickSingle(), thickRect, color);
			else if (val == UnderlineType.Wave) ex.DrawUnderline(new UnderlineWave(), thinRect, color);
			else {
				if (val == UnderlineType.None)
					name = Underline.UnderlineNone.ToString();				
				grid.Margin = new Thickness(0, 0, 3, 0);
				textBlock.Text = name;
				textBlock.Margin = new Thickness(3, 0, 0, 0);
			}
			foreach (UIElement child in grid.Children) {
				FrameworkElement element = child as FrameworkElement;
				if (element != null)
					element.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			}
			return grid;
		}
	}
	#endregion
	#region RichEditUnderlineComboBoxEditItem
	public class RichEditUnderlineComboBoxEditItem : ContentControl {
		UnderlineType? underlineType;
		public UnderlineType? UnderlineType { get { return underlineType; } set { underlineType = value; } }
	}
	#endregion
}
