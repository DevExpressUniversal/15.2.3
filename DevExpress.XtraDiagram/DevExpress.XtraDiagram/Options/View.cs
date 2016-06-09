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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Options {
	public class DiagramOptionsView : DiagramOptionsBase {
		bool allowThemes;
		bool drawGrid;
		bool drawRulers;
		SizeF? gridSize;
		SizeF pageSize;
		Padding scrollMargin;
		double zoomFactor;
		DiagramTheme theme;
		MeasureUnit measureUnit;
		DefaultBoolean transparentRulerBackground;
		public DiagramOptionsView() {
			this.allowThemes = false;
			this.drawGrid = true;
			this.drawRulers = true;
			this.gridSize = null;
			this.pageSize = new SizeF(800, 600);
			this.scrollMargin = new Padding(20);
			this.zoomFactor = 1d;
			this.theme = DiagramThemes.Office;
			this.measureUnit = MeasureUnits.Pixels;
			this.transparentRulerBackground = DefaultBoolean.Default;
		}
		[DefaultValue(false)]
		public bool AllowThemes {
			get { return allowThemes; }
			set {
				if(AllowThemes == value) return;
				bool prevValue = AllowThemes;
				OnChanging("AllowThemes", AllowThemes);
				allowThemes = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("AllowThemes", prevValue, AllowThemes));
			}
		}
		[DefaultValue(true)]
		public bool DrawGrid {
			get { return drawGrid; }
			set {
				if(DrawGrid == value) return;
				bool prevValue = DrawGrid;
				OnChanging("DrawGrid", DrawGrid);
				drawGrid = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("DrawGrid", prevValue, DrawGrid, drawGridChanged: true));
			}
		}
		[DefaultValue(true)]
		public bool DrawRulers {
			get { return drawRulers; }
			set {
				if(DrawRulers == value) return;
				bool prevValue = DrawRulers;
				OnChanging("DrawRulers", DrawRulers);
				drawRulers = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("DrawRulers", prevValue, DrawRulers, drawRulersChanged: true));
			}
		}
		[DefaultValue(null)]
		public SizeF? GridSize {
			get { return gridSize; }
			set {
				if(GridSize == value)
					return;
				SizeF? prevValue = GridSize;
				OnChanging("GridSize", GridSize);
				gridSize = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("GridSize", prevValue, GridSize, gridSizeChanged: true));
			}
		}
		public SizeF PageSize {
			get { return pageSize; }
			set {
				if(PageSize == value)
					return;
				SizeF prevValue = PageSize;
				OnChanging("PageSize", PageSize);
				pageSize = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("PageSize", prevValue, PageSize, pageSizeChanged: true));
			}
		}
		public Padding ScrollMargin {
			get { return scrollMargin; }
			set {
				if(ScrollMargin == value) return;
				Padding prevValue = ScrollMargin;
				OnChanging("ScrollMargin", ScrollMargin);
				scrollMargin = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("ScrollMargin", prevValue, ScrollMargin, scrollMarginChanged: true));
			}
		}
		bool ShouldSerializeScrollMargin() { return !ScrollMargin.Equals(new Padding(20)); }
		void ResetScrollMargin() { ScrollMargin = new Padding(20); }
		const double MinZoomFactor = 0.1;
		const double MaxZoomFactor = 7.0;
		public double ZoomFactor {
			get { return zoomFactor; }
			set {
				value = MathUtils.Coerce(value, MinZoomFactor, MaxZoomFactor);
				if(MathUtils.IsEquals(ZoomFactor, value)) return;
				double prevValue = ZoomFactor;
				OnChanging("ZoomFactor", ZoomFactor, zoomFactorChanging: true);
				zoomFactor = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("ZoomFactor", prevValue, ZoomFactor, zoomFactorChanged: true));
			}
		}
		bool ShouldSerializeZoomFactor() { return MathUtils.IsNotEquals(ZoomFactor, 1d); }
		void ResetZoomFactor() { ZoomFactor = 1d; }
		public DiagramTheme Theme {
			get { return theme; }
			set {
				if(Theme == value) return;
				DiagramTheme prevValue = Theme;
				OnChanging("Theme", Theme);
				theme = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("Theme", prevValue, Theme, themeChanged: true));
			}
		}
		bool ShouldSerializeTheme() { return Theme != DiagramThemes.Office; }
		void ResetTheme() { Theme = DiagramThemes.Office;}
		public MeasureUnit MeasureUnit {
			get { return measureUnit; }
			set {
				if(MeasureUnit == value) return;
				MeasureUnit prevValue = MeasureUnit;
				OnChanging("MeasureUnit", MeasureUnit);
				measureUnit = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("MeasureUnit", prevValue, MeasureUnit));
			}
		}
		bool ShouldSerializeMeasureUnit() { return MeasureUnit != MeasureUnits.Pixels; }
		void ResetMeasureUnit() { MeasureUnit = MeasureUnits.Pixels; }
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean TransparentRulerBackground {
			get { return transparentRulerBackground; }
			set {
				if(TransparentRulerBackground == value) return;
				DefaultBoolean prevValue = TransparentRulerBackground;
				OnChanging("TransparentRulerBackground", TransparentRulerBackground);
				transparentRulerBackground = value;
				OnChanged(new DiagramViewOptionChangedEventArgs("TransparentRulerBackground", prevValue, TransparentRulerBackground, transparentRulerBackgroundChanged: true));
			}
		}
		internal bool AllowTransparentRulerBackground() {
			return TransparentRulerBackground != DefaultBoolean.False;
		}
		protected void OnChanging(string name, object value, bool zoomFactorChanging = false) {
			OnChanging(new DiagramViewOptionChangingEventArgs(name, value, zoomFactorChanging: zoomFactorChanging));
		}
		public override void Assign(BaseOptions other) {
			DiagramOptionsView options = (DiagramOptionsView)other;
			BeginUpdate();
			try {
				base.Assign(other);
				this.allowThemes = options.AllowThemes;
				this.drawGrid = options.DrawGrid;
				this.drawRulers = options.DrawRulers;
				this.gridSize = options.GridSize;
				this.pageSize = options.PageSize;
				this.scrollMargin = options.ScrollMargin;
				this.zoomFactor = options.ZoomFactor;
				this.theme = options.Theme;
				this.measureUnit = options.MeasureUnit;
				this.transparentRulerBackground = options.TransparentRulerBackground;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class DiagramViewOptionChangingEventArgs : DiagramOptionsChangingEventArgs {
		readonly bool zoomFactorChanging;
		public DiagramViewOptionChangingEventArgs(string name, object value, bool zoomFactorChanging = false) : base(name, value) {
			this.zoomFactorChanging = zoomFactorChanging;
		}
		public bool IsZoomFactorChanging { get { return zoomFactorChanging; } }
	}
	public class DiagramViewOptionChangedEventArgs : BaseOptionChangedEventArgs {
		readonly bool gridSizeChanged;
		readonly bool pageSizeChanged;
		readonly bool scrollMarginChanged;
		readonly bool zoomFactorChanged;
		readonly bool themeChanged;
		readonly bool drawGridChanged;
		readonly bool drawRulersChanged;
		readonly bool transparentRulerBackgroundChanged;
		public DiagramViewOptionChangedEventArgs(string name, object oldValue, object newValue, bool gridSizeChanged = false, bool pageSizeChanged = false, bool scrollMarginChanged = false, bool zoomFactorChanged = false, bool themeChanged = false, bool drawGridChanged = false, bool drawRulersChanged = false, bool transparentRulerBackgroundChanged = false) : base(name, oldValue, newValue) {
			this.gridSizeChanged = gridSizeChanged;
			this.pageSizeChanged = pageSizeChanged;
			this.scrollMarginChanged = scrollMarginChanged;
			this.zoomFactorChanged = zoomFactorChanged;
			this.themeChanged = themeChanged;
			this.drawGridChanged = drawGridChanged;
			this.drawRulersChanged = drawRulersChanged;
			this.transparentRulerBackgroundChanged = transparentRulerBackgroundChanged;
		}
		public bool IsGridSizeChanged { get { return gridSizeChanged; } }
		public bool IsPageSizeChanged { get { return pageSizeChanged; } }
		public bool IsScrollMarginChanged { get { return scrollMarginChanged; } }
		public bool IsZoomFactorChanged { get { return zoomFactorChanged; } }
		public bool IsThemeChanged { get { return themeChanged; } }
		public bool IsDrawGridChanged { get { return drawGridChanged; } }
		public bool IsDrawRulersChanged { get { return drawRulersChanged; } }
		public bool IsTransparentRulerBackgroundChanged { get { return transparentRulerBackgroundChanged; } }
	}
}
