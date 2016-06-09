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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl]
	public abstract class XYDiagramPaneBaseModel : ChartElementNamedModel, ISupportModelVisibility {
		RectangleFillStyleModel fillStyleModel;
		ShadowModel shadowModel;
		BackgroundImageModel backImageModel;
		ScrollBarOptionsModel scrollBarOptionsModel;
		ZoomRectangleModel zoomRectangleModel;
		internal bool ActualEnableAxisXScrolling { get { return Pane.ActualEnableAxisXScrolling; } }
		internal bool ActualEnableAxisYScrolling { get { return Pane.ActualEnableAxisYScrolling; } }
		protected internal XYDiagramPaneBase Pane { get { return (XYDiagramPaneBase)base.ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.PaneKey; } }
		[Category("Appearance")]
		public double Weight {
			get { return Pane.Weight; }
			set { SetProperty("Weight", value); }
		}
		[Category("Appearance")]
		public PaneSizeMode SizeMode {
			get { return Pane.SizeMode; }
			set { SetProperty("SizeMode", value); }
		}
		[Category("Appearance")]
		public int SizeInPixels {
			get { return Pane.SizeInPixels; }
			set { SetProperty("SizeInPixels", value); }
		}
		[PropertyForOptions("Appearance", 1),
		Category("Appearance")]
		public Color BackColor {
			get { return Pane.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[PropertyForOptions("Appearance", 1),
		Category("Appearance"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool BorderVisible {
			get { return Pane.BorderVisible; }
			set { SetProperty("BorderVisible", value); }
		}
		[
		PropertyForOptions("Appearance", 1),
		DependentUpon("BorderVisible"),
		Category("Appearance")
		]
		public Color BorderColor {
			get { return Pane.BorderColor; }
			set { SetProperty("BorderColor", value); }
		}
		[Category("Misc")]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public ShadowModel Shadow { get { return shadowModel; } }
		[Category("Appearance"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Pane.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions("Navigation", 0),
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAxisXScrolling {
			get { return Pane.EnableAxisXScrolling; }
			set { SetProperty("EnableAxisXScrolling", value); }
		}
		[PropertyForOptions("Navigation", 0),
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAxisYScrolling {
			get { return Pane.EnableAxisYScrolling; }
			set { SetProperty("EnableAxisYScrolling", value); }
		}
		[PropertyForOptions("Navigation", 0),
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAxisXZooming {
			get { return Pane.EnableAxisXZooming; }
			set { SetProperty("EnableAxisXZooming", value); }
		}
		[PropertyForOptions("Navigation", 0),
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAxisYZooming {
			get { return Pane.EnableAxisYZooming; }
			set { SetProperty("EnableAxisYZooming", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public BackgroundImageModel BackImage { get { return backImageModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("ScrollBar Options"),
		Category("Appearance")]
		public ScrollBarOptionsModel ScrollBarOptions { get { return scrollBarOptionsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public ZoomRectangleModel ZoomRectangle { get { return zoomRectangleModel; } }
		public XYDiagramPaneBaseModel(XYDiagramPaneBase pane, CommandManager commandManager)
			: base(pane, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			if(shadowModel != null)
				Children.Add(shadowModel);
			if(backImageModel != null)
				Children.Add(backImageModel);
			if(scrollBarOptionsModel != null)
				Children.Add(scrollBarOptionsModel);
			if(zoomRectangleModel != null)
				Children.Add(zoomRectangleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new RectangleFillStyleModel(Pane.FillStyle, CommandManager);
			this.shadowModel = new ShadowModel(Pane.Shadow, CommandManager);
			this.backImageModel = new BackgroundImageModel(Pane.BackImage, CommandManager);
			this.scrollBarOptionsModel = new ScrollBarOptionsModel(Pane.ScrollBarOptions, CommandManager);
			this.zoomRectangleModel = new ZoomRectangleModel(Pane.ZoomRectangle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(XYDiagramDefaultPane)), TypeConverter(typeof(XYDiagramDefaultPaneTypeConverter))]
	public class XYDiagramDefaultPaneModel : XYDiagramPaneBaseModel {
		protected new XYDiagramDefaultPane Pane { get { return (XYDiagramDefaultPane)base.Pane; } }
		[Browsable(false)]
		public new string Name { get; set; }
		public XYDiagramDefaultPaneModel(XYDiagramDefaultPane pane, CommandManager commandManager)
			: base(pane, commandManager) {
				Name = "Default Pane";
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeDefaultPaneElement(this);
		}
	}
	[ModelOf(typeof(XYDiagramPane)), TypeConverter(typeof(XYDiagramPaneTypeConverter))]
	public class XYDiagramPaneModel : XYDiagramPaneBaseModel {
		protected new XYDiagramPane Pane { get { return (XYDiagramPane)base.Pane; } }
		public XYDiagramPaneModel(XYDiagramPane pane, CommandManager commandManager)
			: base(pane, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
}
