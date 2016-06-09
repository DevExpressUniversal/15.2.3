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
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public class ScaleElementInfoContainer : Control , IScaleLayoutElement {
		public static readonly DependencyProperty ElementInfoProperty = DependencyPropertyManager.Register("ElementInfo",
			typeof(ScaleElementInfoBase), typeof(ScaleElementInfoContainer));
		public ScaleElementInfoBase ElementInfo {
			get { return (ScaleElementInfoBase)GetValue(ElementInfoProperty); }
			set { SetValue(ElementInfoProperty, value); }
		}
		IScaleLayoutElement LayoutElement { get { return ElementInfo as IScaleLayoutElement; } }
		public ScaleElementInfoContainer() {
			DefaultStyleKey = typeof(ScaleElementInfoContainer);
		}
		ScaleElementLayout IScaleLayoutElement.Layout { get { return LayoutElement != null ? LayoutElement.Layout : null; } }
		Point IScaleLayoutElement.RenderTransformOrigin { get { return LayoutElement != null ? LayoutElement.RenderTransformOrigin : new Point(0, 0); } }
		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
	}
	[NonCategorized]
	public abstract class ScaleElementInfoBase : IScaleLayoutElement, INotifyPropertyChanged {
		PresentationControl presentationControl;
		PresentationBase presentation;
		ScaleElementLayout layout = null;
		public PresentationControl PresentationControl {
			get { return presentationControl; }
			set {
				if (presentationControl != value) {
					presentationControl = value;
					RaisePropertyChanged("PresentationControl");
				}
			}
		}
		public PresentationBase Presentation {
			get { return presentation; }
			set {
				if (presentation != value) {
					presentation = value;
					RaisePropertyChanged("Presentation");
				}
			}
		}
		public ScaleElementLayout Layout {
			get { return layout; }
			internal set { layout = value; }
		}
		internal ScaleElementInfoBase(PresentationControl presentationControl, PresentationBase presentation) {
			this.PresentationControl = presentationControl;
			this.Presentation = presentation;
		}
		#region IScaleLayoutElement implementation
		ScaleElementLayout IScaleLayoutElement.Layout { get { return Layout; } }
		Point IScaleLayoutElement.RenderTransformOrigin { get { return PresentationControl != null ? PresentationControl.GetRenderTransformOrigin() : new Point(0, 0); } }
		#endregion
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	public class ScaleElementLayout {
		readonly Point scaleFactor;
		readonly double angle;
		readonly Point anchorPoint;
		readonly Geometry clipGeometry;
		readonly Size? size;
		public double Angle { get { return angle; } }
		public Point AnchorPoint { get { return anchorPoint; } }
		public Point ScaleFactor { get { return scaleFactor; } }
		public Geometry ClipGeometry { get { return clipGeometry; } }
		public Size? Size { get { return size; } }
		public ScaleElementLayout(double angle, Point anchorPoint, Point scaleFactor, Geometry clipGeometry, Size? size) {
			this.angle = angle;
			this.anchorPoint = anchorPoint;
			this.scaleFactor = scaleFactor;
			this.clipGeometry = clipGeometry;
			this.size = size;
		}
		public ScaleElementLayout(double angle, Point anchorPoint, Point scaleFactor) : this(angle, anchorPoint, scaleFactor, null, null) {			
		}
		public ScaleElementLayout(double angle, Point anchorPoint) : this(angle, anchorPoint, new Point(1.0, 1.0)) {
		}
		public ScaleElementLayout(double angle, Point anchorPoint, Size size) : this(angle, anchorPoint, new Point(1.0, 1.0), null, size) {
		}
		public ScaleElementLayout(Point anchorPoint, Size size, Geometry clipGeometry) : this(0, anchorPoint, new Point(1.0, 1.0), clipGeometry, size) {
		}
	}	
}
