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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartAxisLabelPropertyGridModel : PropertyGridModelBase {
		readonly AxisLabel label;
		readonly SetAxisLabelPropertyCommand setPropertyCommand;
		readonly SetAxisLabelAttachedPropertyCommand setAttachedPropertyCommand;
		protected internal AxisLabel Label { get { return label; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected override ICommand SetObjectAttachedPropertyCommand { get { return setAttachedPropertyCommand; } }
		[Category(Categories.Presentation)]
		public int Angle {
			get { return Label.Angle; }
			set { SetProperty("Angle", value); } 
		} 
		[Category(Categories.Behavior)]
		public bool Staggered {
			get { return Label.Staggered; }
			set { SetProperty("Staggered", value); } 
		}
		[Category(Categories.Appearance)]
		public FontFamily FontFamily {
			get { return Label.FontFamily; }
			set { SetProperty("FontFamily", value); }
		}
		[Category(Categories.Appearance)]
		public double FontSize {
			get { return Label.FontSize; }
			set { SetProperty("FontSize", value); }
		}
		[Category(Categories.Appearance)]
		public FontStretch FontStretch {
			get { return Label.FontStretch; }
			set { SetProperty("FontStretch", value); }
		}
		[Category(Categories.Appearance)]
		public FontStyle FontStyle {
			get { return Label.FontStyle; }
			set { SetProperty("FontStyle", value); }
		}
		[Category(Categories.Appearance)]
		public FontWeight FontWeight {
			get { return Label.FontWeight; }
			set { SetProperty("FontWeight", value); }
		}
		[Category(Categories.Brushes)]
		public Brush Background {
			get { return Label.Background; }
			set { SetProperty("Background", value); }
		}
		[Category(Categories.Appearance)]
		public Brush Foreground {
			get { return Label.Foreground; }
			set { SetProperty("Foreground", value); }
		}
		[Category(Categories.Common)]
		public string TextPattern {
			get { return Label.TextPattern; }
			set { SetProperty("TextPattern", value); }
		}
		public WpfChartAxisLabelPropertyGridModel() : this(null, null) { }
		public WpfChartAxisLabelPropertyGridModel(ChartModelElement modelElement, AxisLabel label)
			: base(modelElement) {
			this.label = label;
			this.setPropertyCommand = new SetAxisLabelPropertyCommand(ChartModel);
			this.setAttachedPropertyCommand = new SetAxisLabelAttachedPropertyCommand(ChartModel);
		}
	}
	public class WpfChartAxis2DLabelPropertyGridModel : WpfChartAxisLabelPropertyGridModel {
		WpfChartResolveOverlappingOptionsPropertyGridModel resolveOverlappingOptions;
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartResolveOverlappingOptionsPropertyGridModel ResolveOverlappingOptions {
			get { return resolveOverlappingOptions; }
			set {
				SetAttachedProperty("ResolveOverlappingOptions", new AxisLabelResolveOverlappingOptions(), ModelElement, typeof(Axis2D),
					delegate(object targetObject, object newValue) {
						Axis2D.SetResolveOverlappingOptions((AxisLabel)targetObject, (AxisLabelResolveOverlappingOptions)newValue);
					},
					delegate(object targetObject) {
						return Axis2D.GetResolveOverlappingOptions((AxisLabel)targetObject);
					});
			}
		}
		public WpfChartAxis2DLabelPropertyGridModel() : base() { 
		}
		public WpfChartAxis2DLabelPropertyGridModel(ChartModelElement modelElement, AxisLabel label)
			: base(modelElement, label) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Axis2D.GetResolveOverlappingOptions(Label) != null) {
				if (resolveOverlappingOptions != null && Axis2D.GetResolveOverlappingOptions(Label) != resolveOverlappingOptions.ResolveOverlappingOptions || resolveOverlappingOptions == null)
					resolveOverlappingOptions = new WpfChartResolveOverlappingOptionsPropertyGridModel(ModelElement, Axis2D.GetResolveOverlappingOptions(Label));
			}
			else
				resolveOverlappingOptions = null;
		}
	}
}
