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
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public interface ISupportBatchUpdate {
		bool IsUpdateLocked { get; }
		void BeginUpdate();
		void EndUpdate();
		void CancelUpdate();
	}
	public abstract class BaseItemsPanel : Panel, ISupportBatchUpdate {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemSpacingProperty;
		static BaseItemsPanel() {
			var dProp = new DependencyPropertyRegistrator<BaseItemsPanel>();
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((BaseItemsPanel)dObj).OnOrientationChanged());
			dProp.Register("ItemSpacing", ref ItemSpacingProperty, double.NaN,
				(dObj, e) => ((BaseItemsPanel)dObj).OnItemSpacingChanged());
		}
		protected static void InvalidateItemsPanelMeasure(DependencyObject dObj) {
			BaseItemsPanel baseItemsPanel = VisualTreeHelper.GetParent(dObj) as BaseItemsPanel;
			if(baseItemsPanel != null) {
				baseItemsPanel.isMeasureValidCore = false;
				baseItemsPanel.OnObjectChanged();
			}
		}
		#endregion static
		protected BaseItemsPanel() { }
		#region Properties
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		public bool IsHorizontal {
			get { return Orientation == Orientation.Horizontal; }
		}
		#endregion Properties
		#region Properties Changed
		protected virtual void OnOrientationChanged() {
			OnObjectChanged();
		}
		protected virtual void OnItemSpacingChanged() {
			OnObjectChanged();
		}
		#endregion Properties Changed
		#region ISupportBatchUpdate
		int lockUpdateCounter;
		public void BeginUpdate() {
			lockUpdateCounter++;
		}
		public void EndUpdate() {
			if(--lockUpdateCounter == 0) OnUnlockUpdate();
		}
		public void CancelUpdate() {
			lockUpdateCounter--;
		}
		public bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		protected void OnUnlockUpdate() {
			OnObjectChanged();
		}
		protected void OnObjectChanged() {
			if(IsUpdateLocked) return;
			if(isMeasureValidCore)
				InvalidateMeasure();
		}
		#endregion ISupportBatchUpdate
		protected sealed override Size ArrangeOverride(Size finalSize) {
			Size result = OnArrange(finalSize);
			return result;
		}
		bool isMeasureValidCore;
		protected sealed override Size MeasureOverride(Size availableSize) {
			Size result = OnMeasure(availableSize);
			isMeasureValidCore = true;
			return result;
		}
		protected virtual Size OnArrange(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}
		protected virtual Size OnMeasure(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
	}
}
