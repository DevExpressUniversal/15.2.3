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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class LayerOptions : GaugeDependencyObject {
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(LayerOptions), new PropertyMetadata(-100, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LayerOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LayerOptions();
		}
	}
	public abstract class LayerBase : GaugeDependencyObject, IOwnedElement, IModelSupported, IWeakEventListener, ILayoutCalculator {
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(LayerBase), new PropertyMetadata(true, VisiblePropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LayerBaseVisible"),
#endif
		Category(Categories.Behavior)
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LayerBase layerBase = d as LayerBase;
			if(layerBase != null)
				layerBase.Invalidate();
		}
		protected static void PresentationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LayerBase layer = d as LayerBase;
			if (layer != null && !Object.Equals(e.NewValue, e.OldValue))
				((IModelSupported)layer).UpdateModel();
		}
		protected static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LayerBase layer = d as LayerBase;
			if (layer != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as GaugeDependencyObject, e.NewValue as GaugeDependencyObject, layer);
				layer.OnOptionsChanged();
			}
		}
		readonly LayerInfo info;
		object owner;
		protected object Owner { get { return owner; } }
		protected abstract int ActualZIndex { get; }
		protected abstract LayerPresentation ActualPresentation { get; }
		internal virtual LayerInfo ElementInfo { get { return info; } }
		public LayerBase() {
			info = new LayerInfo(this, ActualZIndex, ActualPresentation.CreateLayerPresentationControl(), ActualPresentation);
		}
		void ChangeOwner() {
			((IModelSupported)this).UpdateModel();
		}
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdatePresentation();
			OnOptionsChanged();
		}
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(System.Type managerType, object sender, System.EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if((sender is GaugeDependencyObject))
					OnOptionsChanged();
				success = true;
			}
			return success;
		}
		#endregion
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				owner = value;
				ChangeOwner();
			}
		}
		#endregion
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			return Visible ? CreateLayout(constraint) : null;
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			CompleteLayout(elementInfo);
		}
		#endregion
		protected void Invalidate() {
			if (ElementInfo != null)
				ElementInfo.Invalidate();
		}
		protected void UpdatePresentation() {
			if(ElementInfo != null) {
				ElementInfo.Presentation = ActualPresentation;
				ElementInfo.PresentationControl = ActualPresentation.CreateLayerPresentationControl();
			}
		}
		protected virtual void OnOptionsChanged() {
			if (ElementInfo != null)
				ElementInfo.ZIndex = ActualZIndex;
			Invalidate();
		}
		protected abstract ElementLayout CreateLayout(Size constraint);
		protected abstract void CompleteLayout(ElementInfoBase elementInfo);
	}
	public abstract class LayerCollection<T> : GaugeDependencyObjectCollection<T> where T : LayerBase {
		Scale Scale { get { return Owner as Scale; } }
		public LayerCollection(Scale scale) {
			((IOwnedElement)this).Owner = scale;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Scale != null && Scale.Gauge != null)
				Scale.Gauge.UpdateElements();
		}
	}
	[NonCategorized]
	public class LayerInfo : ElementInfoBase {
		internal LayerInfo(ILayoutCalculator layoutCalculator, int zIndex, PresentationControl presentationControl, PresentationBase presentation)
			: base(layoutCalculator, zIndex, presentationControl, presentation) {
		}
	}
}
