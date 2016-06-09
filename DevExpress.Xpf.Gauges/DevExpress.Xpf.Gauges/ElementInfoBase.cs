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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public abstract class ElementInfoBase : IElementInfo, INotifyPropertyChanged {
		readonly ILayoutCalculator layoutCalculator;
		int zIndex = 0;
		ElementInfoContainer container = null;
		ElementLayout layout = null;
		PresentationControl presentationControl = null;
		PresentationBase presentation = null;
		protected internal virtual Object HitTestableObject { get { return null; } }
		protected internal virtual Object HitTestableParent { get { return null; } }
		protected internal virtual bool IsHitTestVisible { get { return false; } }
		protected internal virtual bool InfluenceOnGaugeSize { get { return false; } }
		internal ElementInfoContainer Container {
			get { return container; }
			set { 
				container = value;
				UpdateZIndex(ZIndex);
			}
		}		
		public ElementLayout Layout {
			get { return layout; }
		}
		public PresentationBase Presentation {
			get { return presentation; }
			set {
				if (presentation != value) {
					PresentationChanging(presentation, value);
					presentation = value;
					PresentationChanged();
				}
			}
		}
		public PresentationControl PresentationControl {
			get { return presentationControl; }
			set {
				if (presentationControl != value) {
					presentationControl = value;
					NotifyPropertyChanged("PresentationControl");
				}
			}
		}
		public int ZIndex {
			get { return zIndex; }
			set {
				if (zIndex != value) {
					zIndex = value;
					NotifyPropertyChanged("ZIndex");
					UpdateZIndex(zIndex);
				}
			}
		}
		internal ElementInfoBase(ILayoutCalculator layoutCalculator, int zIndex, PresentationControl presentationControl, PresentationBase presentation) {
			this.layoutCalculator = layoutCalculator;
			this.ZIndex = zIndex;
			this.PresentationControl = presentationControl;
			this.Presentation = presentation;
		}
		#region IElementInfo implementation
		void IElementInfo.Invalidate() {
			Invalidate();
		}
		#endregion
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		void UpdateZIndex(int value) { 
			if(container != null)
				Canvas.SetZIndex(container, value);
		}
		protected virtual void PresentationChanging(PresentationBase oldValue, PresentationBase newValue) {
		}
		protected void PresentationChanged() {
			NotifyPropertyChanged("Presentation");
		}
		protected internal virtual void Invalidate() {
			if (Container != null && Container.PresentationContainer != null)
				Container.PresentationContainer.InvalidateMeasure();
		}
		internal void CreateLayout(Size constraint) {
			layout = layoutCalculator.CreateLayout(constraint);
		}
		internal void CompleteLayout() {
			if (layout != null)
				layoutCalculator.CompleteLayout(this);
		}		
	}
	[NonCategorized]
	public class ElementLayout {
		Transform renderTransform;
		Point anchorPoint;
		Geometry clipGeometry;
		double? height = null;
		double? width = null;
		public Point AnchorPoint { get { return anchorPoint; } }
		public Transform RenderTransform { get { return renderTransform; } }
		public Geometry ClipGeometry { get { return clipGeometry; } }
		public double? Height { get { return height; } }
		public double? Width { get { return width; } }
		internal ElementLayout() {
		}
		internal ElementLayout(double width) {
			this.width = width;
		}
		internal ElementLayout(double width, double height) : this(width) {
			this.height = height;
		}
		internal void CompleteLayout(Point location, Transform transform, Geometry clipGeometry) {
			this.anchorPoint = location;
			this.renderTransform = transform;
			this.clipGeometry = clipGeometry;
		}
	}
}
