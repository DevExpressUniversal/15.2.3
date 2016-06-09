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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class FillStyleBaseModel : DesignerChartElementModelBase {
		readonly FillStyleBase fillStyle;
		FillOptionsBaseModel fillOptionsBaseModel;
		protected FillStyleBase FillStyle { get { return fillStyle; } }
		protected internal override ChartElement ChartElement { get { return fillStyle; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		]
		public FillOptionsBaseModel Options {
			get { return fillOptionsBaseModel; }
			set { SetProperty("Options", value); }
		}
		public FillStyleBaseModel(FillStyleBase fillStyle, CommandManager commandManager)
			: base(commandManager) {
			this.fillStyle = fillStyle;
			Update();
		}
		protected override PropertyInfo GetChildProperty(string name) {
			if(name == "HatchFillOptionsModel" || name == "RectangleGradientFillOptionsModel" || name == "PolygonGradientFillOptionsModel" || name == "SolidFillOptionsModel") {
				if(NeedUpdate())
					Update();
				return typeof(FillStyleBaseModel).GetProperty("Options");
			}
			return base.GetChildProperty(name);
		}
		protected override void AddChildren() {
			if(fillOptionsBaseModel != null)
				Children.Add(fillOptionsBaseModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillOptionsBaseModel = ModelHelper.CreateFillOptionsBaseModelInstance(FillStyle.Options, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class FillStyle2DModel : FillStyleBaseModel {
		protected new FillStyle2D FillStyle { get { return (FillStyle2D)base.FillStyle; } }
		[PropertyForOptions]
		public FillMode FillMode {
			get { return FillStyle.FillMode; }
			set { SetProperty("FillMode", value); }
		}
		[
		PropertyForOptions,
		AllocateToGroup("FillOptions"),
		DependentUpon("FillMode"),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public new FillOptionsBaseModel Options {
			get { return base.Options; }
			set { base.Options = value; }
		}
		public FillStyle2DModel(FillStyle2D fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
		protected override bool NeedUpdate() {
			return (Options == null && FillMode != FillMode.Empty) || (Options != null && Options.FillMode != FillMode);
		}
	}
	[ModelOf(typeof(RectangleFillStyle))]
	public class RectangleFillStyleModel : FillStyle2DModel {
		protected new RectangleFillStyle FillStyle { get { return (RectangleFillStyle)base.FillStyle; } }
		public RectangleFillStyleModel(RectangleFillStyle fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
	}
	[ModelOf(typeof(PolygonFillStyle))]
	public class PolygonFillStyleModel : FillStyle2DModel {
		protected new PolygonFillStyle FillStyle { get { return (PolygonFillStyle)base.FillStyle; } }
		public PolygonFillStyleModel(PolygonFillStyle fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
	}
	[ModelOf(typeof(LineStyle)), TypeConverter(typeof(ExpandableObjectConverter))]
	public class LineStyleModel : DesignerChartElementModelBase {
		readonly LineStyle lineStyle;
		protected LineStyle FillStyle { get { return lineStyle; } }
		protected internal override ChartElement ChartElement { get { return lineStyle; } }
		[PropertyForOptions]
		public int Thickness {
			get { return FillStyle.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[TypeConverter(typeof(DashStyleTypeConterter))]
		public DashStyle DashStyle {
			get { return FillStyle.DashStyle; }
			set { SetProperty("DashStyle", value); }
		}
		[TypeConverter(typeof(LineJoinTypeConverter))]
		public LineJoin LineJoin {
			get { return FillStyle.LineJoin; }
			set { SetProperty("LineJoin", value); }
		}
		public LineStyleModel(LineStyle lineStyle, CommandManager commandManager)
			: base(commandManager) {
			this.lineStyle = lineStyle;
		}
	}
	public abstract class FillStyle3DModel : FillStyleBaseModel {
		protected new FillStyle3D FillStyle { get { return (FillStyle3D)base.FillStyle; } }
		[PropertyForOptions]
		public FillMode3D FillMode {
			get { return FillStyle.FillMode; }
			set { SetProperty("FillMode", value); }
		}
		[
		PropertyForOptions,
		AllocateToGroup("FillOptions"),
		DependentUpon("FillMode"),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public new FillOptionsBaseModel Options {
			get { return base.Options; }
			set { base.Options = value; }
		}
		public FillStyle3DModel(FillStyle3D fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
		protected override bool NeedUpdate() {
			return (Options == null && FillMode != FillMode3D.Empty) || (Options != null && Options.FillMode3D != FillMode);
		}
	}
	[ModelOf(typeof(RectangleFillStyle3D))]
	public class RectangleFillStyle3DModel : FillStyle3DModel {
		protected new RectangleFillStyle3D FillStyle { get { return (RectangleFillStyle3D)base.FillStyle; } }
		public RectangleFillStyle3DModel(RectangleFillStyle3D fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
	}
	[ModelOf(typeof(PolygonFillStyle3D))]
	public class PolygonFillStyle3DModel : FillStyle3DModel {
		protected new PolygonFillStyle3D FillStyle { get { return (PolygonFillStyle3D)base.FillStyle; } }
		public PolygonFillStyle3DModel(PolygonFillStyle3D fillStyle, CommandManager commandManager)
			: base(fillStyle, commandManager) {
		}
	}
}
