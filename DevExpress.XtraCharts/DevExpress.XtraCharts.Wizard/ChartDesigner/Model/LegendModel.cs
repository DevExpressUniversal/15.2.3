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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelOf(typeof(Legend)),
	HasOptionsControl,
	TypeConverter(typeof(LegendTypeConverter))
	]
	public class LegendModel : DesignerChartElementModelBase, ISupportModelVisibility {
		readonly DevExpress.XtraCharts.Legend legend;
		RectangleIndentsModel paddingModel;
		RectangleIndentsModel marginsModel;
		BackgroundImageModel backImageModel;
		RectangleFillStyleModel fillStyleModel;
		RectangularBorderModel borderModel;
		ShadowModel shadowModel;
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override ChartElement ChartElement { get { return legend; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.LegendKey; } }
		[PropertyForOptions("Layout", 1),
		ShowAsGallery(typeof(LegendAlignmentGalleryProvider)),
		Browsable(false)]
		public LegendAlignment Alignment {
			get {
				LegendAlignment v = new LegendAlignment();
				v.HorizontalAlignment = legend.AlignmentHorizontal;
				v.VerticalAlignment = legend.AlignmentVertical;
				return v;
			}
			set {
				BatchSetProperties(new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("AlignmentHorizontal", value.HorizontalAlignment),
					new KeyValuePair<string, object>("AlignmentVertical", value.VerticalAlignment)
				}); 
			}
		}
		[Category("Behavior")]
		public DevExpress.XtraCharts.LegendAlignmentHorizontal AlignmentHorizontal {
			get { return legend.AlignmentHorizontal; }
			set { SetProperty("AlignmentHorizontal", value); }
		}
		[Category("Behavior")]
		public DevExpress.XtraCharts.LegendAlignmentVertical AlignmentVertical {
			get { return legend.AlignmentVertical; }
			set { SetProperty("AlignmentVertical", value); }
		}
		[PropertyForOptions,
		Category("Appearance"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DevExpress.Utils.DefaultBoolean Visibility {
			get { return legend.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public System.Boolean MarkerVisible {
			get { return legend.MarkerVisible; }
			set { SetProperty("MarkerVisible", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public System.Boolean TextVisible {
			get { return legend.TextVisible; }
			set { SetProperty("TextVisible", value); }
		}
		[Category("Behavior")]
		public System.Int32 HorizontalIndent {
			get { return legend.HorizontalIndent; }
			set { SetProperty("HorizontalIndent", value); }
		}
		[Category("Behavior")]
		public System.Int32 VerticalIndent {
			get { return legend.VerticalIndent; }
			set { SetProperty("VerticalIndent", value); }
		}
		[Category("Behavior")]
		public System.Double MaxHorizontalPercentage {
			get { return legend.MaxHorizontalPercentage; }
			set { SetProperty("MaxHorizontalPercentage", value); }
		}
		[Category("Behavior")]
		public System.Double MaxVerticalPercentage {
			get { return legend.MaxVerticalPercentage; }
			set { SetProperty("MaxVerticalPercentage", value); }
		}
		[PropertyForOptions("Layout"),
		Category("Behavior")]
		public DevExpress.XtraCharts.LegendDirection Direction {
			get { return legend.Direction; }
			set { SetProperty("Direction", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public System.Boolean EquallySpacedItems {
			get { return legend.EquallySpacedItems; }
			set { SetProperty("EquallySpacedItems", value); }
		}
		[Category("Behavior")]
		public System.Int32 TextOffset {
			get { return legend.TextOffset; }
			set { SetProperty("TextOffset", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(SizeTypeConverter))]
		public System.Drawing.Size MarkerSize {
			get { return legend.MarkerSize; }
			set { SetProperty("MarkerSize", value); }
		}
		[PropertyForOptions("Appearance", 2),
		Browsable(false)]
		public System.Int32 MarkerHeight {
			get { return legend.MarkerSize.Height; }
			set {
				Size size = new Size(legend.MarkerSize.Width, value);
				SetProperty("MarkerSize", size);
			}
		}
		[PropertyForOptions("Appearance", 2),
		Browsable(false)]
		public System.Int32 MarkerWidth {
			get { return legend.MarkerSize.Width; }
			set {
				Size size = new Size(value, legend.MarkerSize.Height);
				SetProperty("MarkerSize", size);
			}
		}
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public System.Drawing.Color BackColor {
			get { return legend.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public System.Drawing.Color TextColor {
			get { return legend.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(FontTypeConverter))]
		public System.Drawing.Font Font {
			get { return legend.Font; }
			set { SetProperty("Font", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAntialiasing {
			get { return legend.EnableAntialiasing; }
			set { SetProperty("EnableAntialiasing", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public System.Boolean UseCheckBoxes {
			get { return legend.UseCheckBoxes; }
			set { SetProperty("UseCheckBoxes", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangleIndentsModel Padding { get { return paddingModel; } }
		[Category("Behavior"), TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangleIndentsModel Margins { get { return marginsModel; } }
		[Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundImageModel BackImage { get { return backImageModel; } }
		[Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter))]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance"),
		Category("Appearance")
		]
		public RectangularBorderModel Border { get { return borderModel; } }
		[Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter))]
		public ShadowModel Shadow { get { return shadowModel; } }
		public LegendModel(Legend legend, CommandManager commandManager)
			: base(commandManager) {
			this.legend = legend;
			Update();
		}
		#region ISupportModelVisibility implementation
		bool ISupportModelVisibility.Visible {
			get { return Visibility != DefaultBoolean.False; }
			set { Visibility = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		#endregion
		protected override void ProcessMessage(ViewMessage message) {
			if (message.Name == "Alignment")
				this.Alignment = (LegendAlignment)message.Value;
			else if (message.Name == "MarkerHeight")
				this.MarkerHeight = (int)message.Value;
			else if (message.Name == "MarkerWidth")
				this.MarkerWidth = (int)message.Value;
			else
				base.ProcessMessage(message);
		}
		protected override void AddChildren() {
			if (paddingModel != null)
				Children.Add(paddingModel);
			if (marginsModel != null)
				Children.Add(marginsModel);
			if (backImageModel != null)
				Children.Add(backImageModel);
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (borderModel != null)
				Children.Add(borderModel);
			if (shadowModel != null)
				Children.Add(shadowModel);
			base.AddChildren();
		}
		public override void Update() {
			this.paddingModel = new RectangleIndentsModel(legend.Padding, CommandManager);
			this.marginsModel = new RectangleIndentsModel(legend.Margins, CommandManager);
			this.backImageModel = new BackgroundImageModel(legend.BackImage, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(legend.FillStyle, CommandManager);
			this.borderModel = (RectangularBorderModel)ModelHelper.CreateBorderModelInstance(legend.Border, CommandManager);
			this.shadowModel = new ShadowModel(legend.Shadow, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this);
		}
	}
	public struct LegendAlignment {
		public LegendAlignmentVertical VerticalAlignment;
		public LegendAlignmentHorizontal HorizontalAlignment;
		public override string ToString() {
			return VerticalAlignment.ToString() + HorizontalAlignment.ToString() + "_32";
		}
	}
	public class LegendAlignmentGalleryProvider : GalleryElementsProvider {
		public static string ImagePath { get { return "DevExpress.XtraCharts.Wizard.ChartDesigner.Images."; } }
		public override List<GalleryElement> GetElements() {
			List<GalleryElement> elements = new List<GalleryElement>();
			Assembly asm = Assembly.GetExecutingAssembly();
			foreach (var vertical in Enum.GetValues(typeof(LegendAlignmentVertical))) {
				foreach (var horizontal in Enum.GetValues(typeof(LegendAlignmentHorizontal))) {
					LegendAlignment value = new LegendAlignment();
					value.HorizontalAlignment = (LegendAlignmentHorizontal)horizontal;
					value.VerticalAlignment = (LegendAlignmentVertical)vertical;
					elements.Add(new GalleryElement(value, "", ResourceImageHelper.CreateBitmapFromResources(ImagePath + value.ToString() + ".png", asm)));
				}
			}
			return elements;
		}
	}
}
