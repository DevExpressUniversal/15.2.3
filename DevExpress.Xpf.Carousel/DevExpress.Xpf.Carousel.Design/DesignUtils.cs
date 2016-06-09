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
using System.Reflection;
using System.Windows;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Policies;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.Design {
	public interface IAdornedElementProvider : INotifyPropertyChanged {
		ModelItem AdornedElement { get; }
	}
	public abstract class DXAdornerProviderBase : PrimarySelectionAdornerProvider, IAdornedElementProvider {
		protected AdornerPanel myPanel;
		protected readonly Control hookPanel;
		protected DependencyObject platformObject;
		protected DXAdornerProviderBase() {
			hookPanel = CreateHookPanel();
		}
		protected abstract Control CreateHookPanel();
		protected virtual void ConfigurePlacements(AdornerPlacementCollection placements) {
			placements.SizeRelativeToContentWidth(1.0, 0.0);
			placements.SizeRelativeToContentHeight(1.0, 0.0);
			placements.SizeRelativeToAdornerDesiredHeight(1.0, 0);
			placements.SizeRelativeToAdornerDesiredWidth(1.0, 0);
			placements.PositionRelativeToAdornerHeight(0.0, 0);
			placements.PositionRelativeToAdornerWidth(0.0, 0);
		}
		AdornerPanel CreateHookMousePanel() {
			myPanel = new AdornerPanel();
			AdornerPanel.SetHorizontalStretch(hookPanel, AdornerStretch.Stretch);
			AdornerPanel.SetVerticalStretch(hookPanel, AdornerStretch.Stretch);
			AdornerPlacementCollection placements = new AdornerPlacementCollection();
			ConfigurePlacements(placements);
			AdornerPanel.SetPlacements(hookPanel, placements);
			myPanel.Children.Add(hookPanel);
			return myPanel;
		}
		protected override void Activate(ModelItem item) {
			AdornedElement = item;
			RefreshPlatformObject();
			AdornerPanel panel = CreateHookMousePanel();
			Adorners.Add(panel);
			base.Activate(item);
		}
		protected void RefreshPlatformObject() {
			platformObject = AdornedElement.View != null ? AdornedElement.View.PlatformObject as DependencyObject : null;
		}
		protected override void Deactivate() {
			if(myPanel != null)
				myPanel.Children.Remove(hookPanel);
			base.Deactivate();
		}
		private void NotifyPropertyChanged(String info) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#region IAdornedElementProvider
		private ModelItem adornedElement;
		public ModelItem AdornedElement {
			get { return this.adornedElement; }
			private set {
				if(value != this.adornedElement) {
					this.adornedElement = value;
					NotifyPropertyChanged("AdornedElement");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
	[RequiresService(typeof(ModelService))]
	internal class EverythingPolicy<ItemType> : ItemPolicy {
		public EverythingPolicy() {
		}
		protected override void OnActivated() {
			Context.Services.GetRequiredService<ModelService>().ModelChanged += new EventHandler<ModelChangedEventArgs>(OnModelChanged);
		}
		protected override void OnDeactivated() {
			ModelService requiredService = Context.Services.GetRequiredService<ModelService>();
			if(requiredService != null) {
				requiredService.ModelChanged -= new EventHandler<ModelChangedEventArgs>(OnModelChanged);
			}
		}
		private void OnModelChanged(object sender, ModelChangedEventArgs e) {
			List<ModelItem> itemsAdded = new List<ModelItem>();
			List<ModelItem> itemsRemoved = new List<ModelItem>();
			foreach(ModelItem item in e.ItemsRemoved) {
				if(item.IsItemOfType(typeof(ItemType))) {
					itemsRemoved.Add(item);
				}
			}
			foreach(ModelItem item in e.ItemsAdded) {
				if(item.IsItemOfType(typeof(ItemType))) {
					itemsAdded.Add(item);
				}
			}
			this.OnPolicyItemsChanged(new PolicyItemsChangedEventArgs(this, itemsAdded, itemsRemoved));
		}
		public override IEnumerable<ModelItem> PolicyItems {
			get {
				return Context.Services.GetRequiredService<ModelService>().Find(null, typeof(ItemType));
			}
		}
	}
	public class DesignModeFixedValuesProvider<T> : DesignModeValueProvider {
		readonly IEnumerable<KeyValuePair<DependencyProperty, object>> propertyValuePairs;
		public DesignModeFixedValuesProvider(IEnumerable<KeyValuePair<DependencyProperty, object>> propertyValuePairs) {
			this.propertyValuePairs = propertyValuePairs;
			foreach(KeyValuePair<DependencyProperty, object> pair in propertyValuePairs) {
				Properties.Add(typeof(T), DesignHelper.GetPropertyName(pair.Key));
			}
		}
		public DesignModeFixedValuesProvider(DependencyProperty property, object designTimeValue)
			: this(new KeyValuePair<DependencyProperty, object>[] { new KeyValuePair<DependencyProperty, object>(property, designTimeValue) }) {
		}
		public DesignModeFixedValuesProvider()
			: this(new KeyValuePair<DependencyProperty, object>[] { }) {
		}
		public sealed override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if(identifier.DeclaringType != typeof(T)) {
				return value;
			}
			return TranslatePropertyValueCore(item, identifier, value);
		}
		protected virtual object TranslatePropertyValueCore(ModelItem item, PropertyIdentifier identifier, object value) {
			foreach(KeyValuePair<DependencyProperty, object> pair in propertyValuePairs) {
				if(identifier.Name == DesignHelper.GetPropertyName(pair.Key)) {
					return pair.Value;
				}
			}
			return value;
		}
	}
}
