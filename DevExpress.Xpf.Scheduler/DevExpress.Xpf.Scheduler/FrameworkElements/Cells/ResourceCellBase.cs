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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Internal;
#if SL
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformIndependentImage = System.Windows.Controls.Image;
#else
using PlatformIndependentColor = System.Drawing.Color;
using PlatformIndependentImage = System.Drawing.Image;
using System.Collections.ObjectModel;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ICellBrushSelector {
		Brush SelectBrush(VisualResourceCellBaseContent content);
	}
	[TemplatePart(Name = "PART_CONTENT", Type = typeof(UIElement))]
	public abstract class VisualResourceCellBase : SchedulerContentControl, ISupportCopyFrom<ResourceCellBase> {
		UIElement contentElement;
		protected VisualResourceCellBase() {
			DefaultStyleKey = typeof(VisualResourceCellBase);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceCellBaseContentElement")]
#endif
		public UIElement ContentElement { get { return contentElement; } }
		#region BackgroundBrushSelector
		public static readonly DependencyProperty BackgroundBrushSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBase, ICellBrushSelector>("BackgroundBrushSelector", null, (d, e) => d.OnBackgroundBrushSelectorChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceCellBaseBackgroundBrushSelector")]
#endif
		public ICellBrushSelector BackgroundBrushSelector { get { return (ICellBrushSelector)GetValue(BackgroundBrushSelectorProperty); } set { SetValue(BackgroundBrushSelectorProperty, value); } }
		#endregion
		#region BorderBrushSelector
		public static readonly DependencyProperty BorderBrushSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBase, ICellBrushSelector>("BorderBrushSelector", null, (d, e) => d.OnBorderBrushSelectorChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceCellBaseBorderBrushSelector")]
#endif
		public ICellBrushSelector BorderBrushSelector { get { return (ICellBrushSelector)GetValue(BorderBrushSelectorProperty); } set { SetValue(BorderBrushSelectorProperty, value); } }
		#endregion
		protected internal void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeContentEvents(Content);
		}
		protected internal void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeContentEvents(Content);
			SubscribeContentEvents(Content);
			if (Content != null) {
				RecalculateBrush(true);
			}
		}
		protected virtual void OnBackgroundBrushSelectorChanged(ICellBrushSelector iCellBrushSelector, ICellBrushSelector iCellBrushSelector_2) {
			RecalculateBrush(true);
		}
		protected virtual void OnBorderBrushSelectorChanged(ICellBrushSelector iCellBrushSelector, ICellBrushSelector iCellBrushSelector_2) {
			RecalculateBrush(true);
		}
		protected virtual void RecalculateBrush(bool visualViewInfoChanged) {
			VisualResourceCellBaseContent content = Content as VisualResourceCellBaseContent;
			if (CanRecalculateBrush(BackgroundBrushSelector, visualViewInfoChanged))
				Background = BackgroundBrushSelector.SelectBrush(content);
			if (CanRecalculateBrush(BorderBrushSelector, visualViewInfoChanged))
				BorderBrush = BorderBrushSelector.SelectBrush(content);
		}
		bool CanRecalculateBrush(ICellBrushSelector selector, bool visualViewInfoWasChanged) {
			if (selector == null)
				return false;
			if (visualViewInfoWasChanged)
				return true;
			return !(selector is ISchedulerDefaultCellBrushSelector);
		}
		protected virtual void CopyFromCore(ResourceCellBase source) {
			VisualResourceCellBaseContent content = Content as VisualResourceCellBaseContent;
			if (content != null)
				content.CopyFrom(source);
		}
		void OnContentCopyFromCompleted(object sender, CopyFromCompletedEventArgs e) {
			RecalculateBrush(e.IsStateChanged);
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			if (oldContent != null)
				UnsubscribeContentEvents(oldContent);
			if (newContent != null) {
				SubscribeContentEvents(newContent);
			}
			RecalculateBrush(true);
		}
		protected virtual void SubscribeContentEvents(object content) {
			VisualResourceCellBaseContent cellContent = content as VisualResourceCellBaseContent;
			if (cellContent == null)
				return;
			cellContent.CopyFromCompleted += OnContentCopyFromCompleted;
		}
		protected virtual void UnsubscribeContentEvents(object content) {
			VisualResourceCellBaseContent cellContent = content as VisualResourceCellBaseContent;
			if (cellContent == null)
				return;
			cellContent.CopyFromCompleted -= OnContentCopyFromCompleted;
		}
		void ContentBrushesChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RecalculateBrush(true);
		}
		void BrushContentChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RecalculateBrush(true);
		}
		void ContentResourceIdChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RecalculateBrush(true);
		}
		protected virtual UIElement GetContentElement() {
			return GetTemplateChild("PART_CONTENT") as UIElement;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.contentElement = GetContentElement();
		}
		#region ISupportCopyFrom<ResourceCellBase> Members
		public void CopyFrom(ResourceCellBase source) {
			CopyFromCore(source);
		}
		#endregion
	}
	public class VisualResourceCellBaseContent : DependencyObject {
		object lastSettedResourceId;
		string lastSettedResourceCaption;
		Color lastSettedResourceColor;
		ImageSource lastSettedResourceImage;
		bool lastSettedResourceVisible;
		#region CopyFromCompleted
		WeakEventHandler<CopyFromCompletedEventArgs, EventHandler> onCopyFromCompleted;
		internal event EventHandler<CopyFromCompletedEventArgs> CopyFromCompleted { add { onCopyFromCompleted += value; } remove { onCopyFromCompleted -= value; } }
		protected virtual void RaiseCopyFromCompleted(bool wasChanged) {
			if (onCopyFromCompleted != null)
				onCopyFromCompleted.Raise(this, new CopyFromCompletedEventArgs(wasChanged));
		}
		#endregion
		#region ResourceId
		public static readonly DependencyProperty ResourceIdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, object>("ResourceId", null, (d, e) => d.OnResourceIdChanged(e.OldValue, e.NewValue));
		public object ResourceId { get { return GetValue(ResourceIdProperty); } set { SetValue(ResourceIdProperty, value); } }
		DependencyPropertyChangedEventHandler onResourceIdChanged;
		public event DependencyPropertyChangedEventHandler ResourceIdChanged { add { onResourceIdChanged += value; } remove { onResourceIdChanged -= value; } }
		protected virtual void OnResourceIdChanged(object oldResourceId, object newResourceId) {
			RaiseOnResourceIdChanged(oldResourceId, newResourceId);
			lastSettedResourceId = newResourceId;
		}
		protected virtual void RaiseOnResourceIdChanged(object oldResourceId, object newResourceId) {
			if (onResourceIdChanged != null) {
				DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs(ResourceIdProperty, oldResourceId, newResourceId);
				onResourceIdChanged(this, e);
			}
		}
		#endregion
		#region Brushes
		public static readonly DependencyProperty BrushesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, VisualResourceBrushes>("Brushes", null, (d, e) => d.OnResourceBrushesChanged(e.OldValue, e.NewValue));
		public VisualResourceBrushes Brushes { get { return (VisualResourceBrushes)GetValue(BrushesProperty); } set { SetValue(BrushesProperty, value); } }
		DependencyPropertyChangedEventHandler onBrushesChanged;
		public event DependencyPropertyChangedEventHandler BrushesChanged { add { onBrushesChanged += value; } remove { onBrushesChanged -= value; } }
		protected virtual void RaiseOnBrushesChanged(VisualResourceBrushes oldBrushes, VisualResourceBrushes newBrushes) {
			if (onBrushesChanged != null) {
				DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs(BrushesProperty, oldBrushes, newBrushes);
				onBrushesChanged(this, e);
			}
		}
		protected virtual void OnResourceBrushesChanged(VisualResourceBrushes oldBrushes, VisualResourceBrushes newBrushes) {
			RaiseOnBrushesChanged(oldBrushes, newBrushes);
		}
		#endregion
		#region ResourceCaption
		public static readonly DependencyProperty ResourceCaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, string>("ResourceCaption", String.Empty, (d, e) => d.OnResourceCaptionChanged(e.OldValue, e.NewValue));
		protected virtual void OnResourceCaptionChanged(string oldCaption, string newCaption) {
			lastSettedResourceCaption = newCaption;
		}
		public string ResourceCaption { get { return (string)GetValue(ResourceCaptionProperty); } set { SetValue(ResourceCaptionProperty, value); } }
		#endregion
		#region ResourceColor
		public static readonly DependencyProperty ResourceColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, Color>("ResourceColor", ColorExtension.Empty, (d, e) => d.OnResourceColorChanged(e.OldValue, e.NewValue));
		public Color ResourceColor {
			get { return (Color)GetValue(ResourceColorProperty); }
			set { SetValue(ResourceColorProperty, value); }
		}
		protected virtual void OnResourceColorChanged(Color oldColor, Color newColor) {
			lastSettedResourceColor = newColor;
		}
		#endregion
		#region ResourceImage
		public static readonly DependencyProperty ResourceImageProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, ImageSource>("ResourceImage", null, (d, e) => d.OnResourceImageChanged(e.OldValue, e.NewValue));
		public ImageSource ResourceImage {
			get { return (ImageSource)GetValue(ResourceImageProperty); }
			set { SetValue(ResourceImageProperty, value); }
		}
		protected virtual void OnResourceImageChanged(ImageSource oldImage, ImageSource newImage) {
			lastSettedResourceImage = newImage;
		}
		#endregion
		#region ResourceVisible
		public static readonly DependencyProperty ResourceVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, bool>("ResourceVisible", false, (d, e) => d.OnResourceVisibleChanged(e.OldValue, e.NewValue));
		public bool ResourceVisible { get { return (bool)GetValue(ResourceVisibleProperty); } set { SetValue(ResourceVisibleProperty, value); } }
		protected virtual void OnResourceVisibleChanged(bool oldVisible, bool newVisible) {
			lastSettedResourceVisible = newVisible;
		}
		#endregion
		#region CustomFields
		public VisualCustomFieldCollection CustomFields {
			get { return (VisualCustomFieldCollection)GetValue(CustomFieldsProperty); }
			set { SetValue(CustomFieldsProperty, value); }
		}
		public static readonly DependencyProperty CustomFieldsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceCellBaseContent, VisualCustomFieldCollection>("CustomFields", null);
		#endregion
		protected internal bool CopyFrom(ResourceCellBase source) {
			bool wasChanged = CopyFromCore(source);
			RaiseCopyFromCompleted(wasChanged);
			return wasChanged;
		}
		protected internal virtual bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = false;
			if (lastSettedResourceId != source.Resource.Id) {
				ResourceId = source.Resource.Id;
				wasChanged = true;
			}
			if (Brushes == null) {
				Brushes = new VisualResourceBrushes();
				wasChanged = true;
			}
			wasChanged |= this.Brushes.CopyFrom(source.Brushes, (object)source.Resource.Id);
			if (lastSettedResourceCaption != source.Resource.Caption) {
				ResourceCaption = source.Resource.Caption;
				wasChanged = true;
			}
			if (lastSettedResourceColor != source.Resource.GetColor()) {
				ResourceColor = source.Resource.GetColor();
				wasChanged = true;
			}
			if (lastSettedResourceImage != source.Resource.GetImage()) {
				ResourceImage = source.Resource.GetImage();
				wasChanged = true;
			}
			if (lastSettedResourceVisible != source.Resource.Visible) {
				ResourceVisible = source.Resource.Visible;
				wasChanged = true;
			}
			if (CustomFields == null) {
				CustomFields = new VisualCustomFieldCollection();
				wasChanged = true;
			}
			wasChanged |= this.CustomFields.CopyFrom((XtraScheduler.CustomFieldCollection)source.Resource.CustomFields);
			return wasChanged;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Internal {
	public interface ISchedulerDefaultCellBrushSelector {
	}
}
