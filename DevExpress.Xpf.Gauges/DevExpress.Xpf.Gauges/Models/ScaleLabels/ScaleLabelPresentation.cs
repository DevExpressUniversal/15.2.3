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
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class ScaleLabelPresentation : PresentationBase {
		protected internal abstract PresentationControl CreateLabelPresentationControl();
	}
	public abstract class PredefinedScaleLabelPresentation : ScaleLabelPresentation {
	}
	public class DefaultScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Default Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new DefaultScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DefaultScaleLabelPresentation();
		}
	}
	public class CleanWhiteScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Clean White Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new CleanWhiteScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CleanWhiteScaleLabelPresentation();
		}
	}
	public class CosmicScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Cosmic Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new CosmicScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CosmicScaleLabelPresentation();
		}
	}
	public class SmartScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Smart Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new SmartScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new SmartScaleLabelPresentation();
		}
	}
	public class ProgressiveScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Progressive Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new ProgressiveScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ProgressiveScaleLabelPresentation();
		}
	}
	public class EcoScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Eco Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new EcoScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new EcoScaleLabelPresentation();
		}
	}
	public class FutureScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Future Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new FutureScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FutureScaleLabelPresentation();
		}
	}
	public class ClassicScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Classic Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new ClassicScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ClassicScaleLabelPresentation();
		}
	}
	public class IStyleScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "IStyle Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new IStyleScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IStyleScaleLabelPresentation();
		}
	}
	public class YellowSubmarineScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Yellow Submarine Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new YellowSubmarineScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new YellowSubmarineScaleLabelPresentation();
		}
	}
	public class MagicLightScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Magic Light Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new MagicLightScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new MagicLightScaleLabelPresentation();
		}
	}
	public class FlatLightScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Flat Light Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new FlatLightScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatLightScaleLabelPresentation();
		}
	}
	public class FlatDarkScaleLabelPresentation : PredefinedScaleLabelPresentation {
		public override string PresentationName { get { return "Flat Dark Scale Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			return new FlatDarkScaleLabelControl();
		}
		protected override GaugeDependencyObject CreateObject() {
			return new FlatDarkScaleLabelPresentation();
		}
	}
	public class CustomScaleLabelPresentation : ScaleLabelPresentation {
		public static readonly DependencyProperty LabelTemplateProperty = DependencyPropertyManager.Register("LabelTemplate",
			typeof(ControlTemplate), typeof(CustomScaleLabelPresentation));
		[Category(Categories.Common)]
		public ControlTemplate LabelTemplate {
			get { return (ControlTemplate)GetValue(LabelTemplateProperty); }
			set { SetValue(LabelTemplateProperty, value); }
		}
		public override string PresentationName { get { return "Custom Label"; } }
		protected internal override PresentationControl CreateLabelPresentationControl() {
			CustomPresentationControl modelControl = new CustomPresentationControl();
			modelControl.SetBinding(CustomPresentationControl.TemplateProperty, new Binding("LabelTemplate") { Source = this });
			return modelControl;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CustomScaleLabelPresentation();
		}
	}
}
