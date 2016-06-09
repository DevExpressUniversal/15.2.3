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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public class SideBarPanel : NavBarPositionPanel {
		static readonly DependencyPropertyKey ResidualSizePropertyKey;
		public static readonly DependencyProperty ResidualSizeProperty;
		public static readonly DependencyProperty ActiveGroupMinHeightProperty;
		static SideBarPanel() {
			ResidualSizePropertyKey = DependencyPropertyManager.RegisterReadOnly("ResidualSize", typeof(Size), typeof(SideBarPanel), new FrameworkPropertyMetadata(new Size()));
			ResidualSizeProperty = ResidualSizePropertyKey.DependencyProperty;
			ActiveGroupMinHeightProperty = DependencyPropertyManager.Register("ActiveGroupMinHeight", typeof(double), typeof(SideBarPanel), new FrameworkPropertyMetadata(150d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		public SideBarPanel() {
			LayoutUpdated += OnLayoutUpdated;
		}
		public Size ResidualSize {
			get { return (Size)GetValue(ResidualSizeProperty); }
			internal set { this.SetValue(ResidualSizePropertyKey, value); }
		}
		public double ActiveGroupMinHeight {
			get { return (double)GetValue(ActiveGroupMinHeightProperty); }
			set { SetValue(ActiveGroupMinHeightProperty, value); }
		}
		SizeHelperBase sizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		bool IsActiveGroup(NavBarGroupControl child) {
			NavBarGroup group = child.DataContext as NavBarGroup;
			return group != null ? group.IsActive : false;
		}
		NavBarGroupControl GetActiveGroup() {
			foreach(NavBarGroupControl child in Children)
				if(IsActiveGroup(child)) return child;
			return null;
		}
		double GetDesiredWidthWithoutActiveGroup() {
			double res = 0d;
			foreach(NavBarGroupControl child in Children) {
				if(IsActiveGroup(child)) continue;
				res += sizeHelper.GetDefineSize(child.DesiredSize);
			}
			return res;
		}
		protected double GetActiveGroupRestSize(Size size) {
			NavBarGroupControl activeGroup = GetActiveGroup();
			double width = 0;
			double height = 0;
			foreach(NavBarGroupControl child in Children) {
				height = Math.Max(height, sizeHelper.GetSecondarySize(child.DesiredSize));
				width += child.Visibility == System.Windows.Visibility.Visible ? child.MinDesiredSize.Height : 0;
			}
			double rest = 0;
			if(activeGroup != null) {
				rest = sizeHelper.GetDefineSize(size) - width;
				if(rest < ActiveGroupMinHeight) rest = ActiveGroupMinHeight;
			}
			return rest;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			double rest = GetActiveGroupRestSize(arrangeSize);
			Rect rect = new Rect(0, 0, 0, 0);
			foreach(NavBarGroupControl group in Children) {
				if(group.Visibility == Visibility.Visible) {
					rect = new Rect(new Point(rect.X, rect.Y), group.MinDesiredSize);
					if(Orientation == Orientation.Vertical) {
						double height = group.MinDesiredSize.Height;
						if(group.Expander != null)
							height += rest * group.Expander.AnimationProgress;
						rect.Height = height;
						rect.Width = arrangeSize.Width;
					}
					else {
						double width = group.MinDesiredSize.Height;
						if(group.Expander != null)
							width += rest * group.Expander.AnimationProgress;
						rect.Width = width;
						rect.Height = arrangeSize.Height;
					}
					group.Arrange(rect);
					if(Orientation == Orientation.Vertical)
						rect.Y += rect.Height;
					else
						rect.X += rect.Width;
				}
			}
			if(Orientation == Orientation.Vertical)
				rect.Height = 0;
			else
				rect.Width = 0;
			return new Size(rect.Right, rect.Bottom);
		}
		protected override Size MeasureChildrenOverride(Size constraint) {
			NavBarGroupControl activeGroup = GetActiveGroup();
			double height = 0;
			double width = 0;
			foreach(NavBarGroupControl child in Children) {
				child.Measure(sizeHelper.CreateSize(double.PositiveInfinity, sizeHelper.GetSecondarySize(constraint)));
				width = Math.Max(width, sizeHelper.GetSecondarySize(child.DesiredSize));
				height += child.MinDesiredSize.Height;
			}
			double rest = GetActiveGroupRestSize(constraint);
			if(double.IsInfinity(rest))
				rest = ActiveGroupMinHeight;
			if(!double.IsInfinity(rest))
				height += rest;
			if(!double.IsPositiveInfinity(sizeHelper.GetDefineSize(constraint)))
				height = sizeHelper.GetDefineSize(constraint);
			if(!double.IsPositiveInfinity(sizeHelper.GetSecondarySize(constraint)))
				width = sizeHelper.GetSecondarySize(constraint);
			if(double.IsPositiveInfinity(height))
				height = 0;
			if(double.IsPositiveInfinity(width))
				width = 0;
			Size sz = sizeHelper.CreateSize(rest, width);
			ResidualSize = new Size(Orientation == Orientation.Horizontal ? sz.Width : double.PositiveInfinity, Orientation == Orientation.Vertical ? sz.Height : double.PositiveInfinity);			
			return sizeHelper.CreateSize(height, width);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			if(DataContext is NavBarControl && ((NavBarControl)DataContext).View is SideBarView) {
				LayoutUpdated -= OnLayoutUpdated;
				((SideBarView)((NavBarControl)DataContext).View).SideBarPanel = this;
			} else {
				if(this.IsInDesignTool()) {
					SideBarView view = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<SideBarView>(this);
					if(view != null) {
						LayoutUpdated -= OnLayoutUpdated;
						view.SideBarPanel = this;
					}
				}
			}
		}
	}
	[TemplateVisualState(Name = "SideBarFirst", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarFirst0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarMiddle", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarMiddle0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarLast", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarLast0", GroupName = "ViewAnimationProgressGroupPositionStates")]
	[TemplateVisualState(Name = "SideBarSingle", GroupName = "ViewAnimationProgressGroupPositionStates")]	
	public class SideBarHeader : NavBarGroupHeader {
	}
	public class SideBarLayoutTransformPanel : LayoutTransformPanel {
		public SideBarLayoutTransformPanel() {
			SizeChanged += OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateMeasure();
		}		
	}
}
