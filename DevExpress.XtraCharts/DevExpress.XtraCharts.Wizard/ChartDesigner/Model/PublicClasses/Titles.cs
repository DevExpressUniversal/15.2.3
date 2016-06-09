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
using System.Drawing.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class TitleBaseModel : DesignerChartElementModelBase {
		readonly TitleBase title;
		protected TitleBase Title { get { return title; } }
		protected internal override ChartElement ChartElement { get { return title; } }
		[Category("Appearance"), TypeConverter(typeof(FontTypeConverter))]
		public Font Font {
			get { return Title.Font; }
			set { SetProperty("Font", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAntialiasing {
			get { return Title.EnableAntialiasing; }
			set { SetProperty("EnableAntialiasing", value); }
		}
		[PropertyForOptions(0, "Behavior", -1), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return Title.Visible; }
			set { SetProperty("Visible", value); }
		}
		[
		PropertyForOptions(0, "Behavior", 0),
		DependentUpon("Visible")]
		public Color TextColor {
			get { return Title.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		public TitleBaseModel(TitleBase title, CommandManager commandManager)
			: base(commandManager) {
			this.title = title;
		}
	}
	public abstract class TitleModel : TitleBaseModel {
		protected new Title Title { get { return (Title)base.Title; } }
		[PropertyForOptions(100, "General"),
		DependentUpon("Visible")]
		public string Text {
			get { return Title.Text; }
			set { SetProperty("Text", value); }
		}
		public TitleModel(Title title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	public abstract class MultilineTitleModel : TitleModel {
		protected new MultilineTitle Title { get { return (MultilineTitle)base.Title; } }
		[Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)),
		PropertyForOptions(100, "General"),
		DesignerDisplayName("Text")]
		public string[] Lines {
			get { return Title.Lines; }
			set { SetProperty("Lines", value); }
		}
		public MultilineTitleModel(MultilineTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	public abstract class DockableTitleModel : MultilineTitleModel, ISupportModelVisibility {
		protected new DockableTitle Title { get { return (DockableTitle)base.Title; } }
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Visibility {
			get { return Title.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[Category("Behavior")]
		public new string Text {
			get { return Title.Text; }
			set { SetProperty("Text", value); }
		}
		[Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)),
		PropertyForOptions(100, "General"),
		DesignerDisplayName("Text"),
		DependentUpon("Visibility"),
		Category("Behavior")]
		public new string[] Lines {
			get { return base.Lines; }
			set { base.Lines = value; }
		}
		[Category("Appearance")]
		public new Color TextColor {
			get { return base.TextColor; }
			set { base.TextColor = value; }
		}
		[
		PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool WordWrap {
			get { return Title.WordWrap; }
			set { SetProperty("WordWrap", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior")]
		public int MaxLineCount {
			get { return Title.MaxLineCount; }
			set { SetProperty("MaxLineCount", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior"),
		TypeConverter(typeof(EnumTypeConverter))]
		public ChartTitleDockStyle Dock {
			get { return Title.Dock; }
			set { SetProperty("Dock", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior"),
		TypeConverter(typeof(StringAlignmentTypeConvertor))]
		public StringAlignment Alignment {
			get { return Title.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visibility"),
		Category("Behavior")]
		public int Indent {
			get { return Title.Indent; }
			set { SetProperty("Indent", value); }
		}
		[Browsable(false)]
		public new bool Visible { get; set; }
		public DockableTitleModel(DockableTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
		#region ISupportModelVisibility implementation
		bool ISupportModelVisibility.Visible {
			get { return Visibility != DefaultBoolean.False; }
			set { Visibility = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		#endregion
	}
	[ModelOf(typeof(SeriesTitle)),
	HasOptionsControl,
	TypeConverter(typeof(SeriesTitleTypeConverter))]
	public class SeriesTitleModel : DockableTitleModel {
		protected new SeriesTitle Title { get { return (SeriesTitle)base.Title; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.SeriesTitleKey; } }
		internal override string ChartTreeText { get { return "Title"; } }
		public SeriesTitleModel(SeriesTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[ModelOf(typeof(ChartTitle)),
	HasOptionsControl,
	TypeConverter(typeof(ChartTitleTypeConverter))]
	public class ChartTitleModel : DockableTitleModel {
		protected new ChartTitle Title { get { return (ChartTitle)base.Title; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.TitleKey; } }
		internal override string ChartTreeText { get { return "Title"; } }
		public ChartTitleModel(ChartTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[ModelOf(typeof(AxisTitle))]
	public class AxisTitleModel : TitleModel {
		protected new AxisTitle Title { get { return (AxisTitle)base.Title; } }
		[Browsable(false)]
		public new bool Visible { get; set; }
		[TypeConverter(typeof(StringAlignmentTypeConvertor))]
		public StringAlignment Alignment {
			get { return Title.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		public new Color TextColor {
			get { return Title.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[PropertyForOptions(100, "General"),
		DependentUpon("Visibility")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[PropertyForOptions("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Visibility {
			get { return Title.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		public AxisTitleModel(AxisTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	[ModelOf(typeof(AxisTitleX))]
	public class AxisTitleXModel : AxisTitleModel {
		protected new AxisTitleX Title { get { return (AxisTitleX)base.Title; } }
		public AxisTitleXModel(AxisTitleX title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	[ModelOf(typeof(AxisTitleY))]
	public class AxisTitleYModel : AxisTitleModel {
		protected new AxisTitleY Title { get { return (AxisTitleY)base.Title; } }
		public AxisTitleYModel(AxisTitleY title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	[ModelOf(typeof(ConstantLineTitle))]
	public class ConstantLineTitleModel : TitleModel {
		protected new ConstantLineTitle Title { get { return (ConstantLineTitle)base.Title; } }
		[
		PropertyForOptions,
		DependentUpon("Visible")]
		public ConstantLineTitleAlignment Alignment {
			get { return Title.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visible"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowBelowLine {
			get { return Title.ShowBelowLine; }
			set { SetProperty("ShowBelowLine", value); }
		}
		public new Color TextColor {
			get { return base.TextColor; }
			set { base.TextColor = value; }
		}
		public ConstantLineTitleModel(ConstantLineTitle title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	public abstract class NotificationBaseModel : MultilineTitleModel {
		protected new NotificationBase Title { get { return (NotificationBase)base.Title; } }
		[Browsable(false)]
		public new bool Visible { get; set; }
		public NotificationBaseModel(NotificationBase title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	[ModelOf(typeof(EmptyChartText))]
	public class EmptyChartTextModel : NotificationBaseModel {
		protected new EmptyChartText Title { get { return (EmptyChartText)base.Title; } }
		public EmptyChartTextModel(EmptyChartText title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
	[ModelOf(typeof(SmallChartText))]
	public class SmallChartTextModel : NotificationBaseModel {
		protected new SmallChartText Title { get { return (SmallChartText)base.Title; } }
		public SmallChartTextModel(SmallChartText title, CommandManager commandManager)
			: base(title, commandManager) {
		}
	}
}
