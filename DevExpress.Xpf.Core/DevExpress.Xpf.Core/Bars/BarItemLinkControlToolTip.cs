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
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Linq;
namespace DevExpress.Xpf.Bars {	
	public enum BarItemLinkControlToolTipHorizontalPlacement { LeftAtTargetLeft, RightAtTargetLeft, LeftAtTargetRight, RightAtTargetRight, CenterAtTargetCenter, LeftAtMousePoint, RightAtMousePoint, LeftAtMouse, RightAtMouse, System }
	public enum BarItemLinkControlToolTipVerticalPlacement { TopAtTargetTop, BottomAtTargetTop, TopAtTargetBottom, BottomAtTargetBottom, CenterAtTargetCenter, TopAtMousePoint, BottomAtMousePoint, TopAtMouse, BottomAtMouse, System }
	public enum BarItemLinkControlToolTipPlacementTargetType { Internal, External }
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BarItemLinkControlToolTip : ToolTip {
		#region static
		public static readonly DependencyProperty UseToolTipPlacementTargetProperty =
			DependencyProperty.Register("UseToolTipPlacementTarget", typeof(bool), typeof(BarItemLinkControlToolTip), new PropertyMetadata(false));		
		public static readonly DependencyProperty HorizontalPlacementProperty =
			DependencyProperty.RegisterAttached("HorizontalPlacement", typeof(BarItemLinkControlToolTipHorizontalPlacement), typeof(BarItemLinkControlToolTip), new PropertyMetadata(BarItemLinkControlToolTipHorizontalPlacement.System));
		public static readonly DependencyProperty VerticalPlacementProperty =
			DependencyProperty.RegisterAttached("VerticalPlacement", typeof(BarItemLinkControlToolTipVerticalPlacement), typeof(BarItemLinkControlToolTip), new PropertyMetadata(BarItemLinkControlToolTipVerticalPlacement.System));
		public static BarItemLinkControlToolTipHorizontalPlacement GetHorizontalPlacement(DependencyObject obj) {
			return (BarItemLinkControlToolTipHorizontalPlacement)obj.GetValue(HorizontalPlacementProperty);
		}
		public static void SetHorizontalPlacement(DependencyObject obj, BarItemLinkControlToolTipHorizontalPlacement value) {
			obj.SetValue(HorizontalPlacementProperty, value);
		}
		public static BarItemLinkControlToolTipVerticalPlacement GetVerticalPlacement(DependencyObject obj) {
			return (BarItemLinkControlToolTipVerticalPlacement)obj.GetValue(VerticalPlacementProperty);
		}
		public static void SetVerticalPlacement(DependencyObject obj, BarItemLinkControlToolTipVerticalPlacement value) {
			obj.SetValue(VerticalPlacementProperty, value);
		}
		static BarItemLinkControlToolTip() {
			PlacementProperty.OverrideMetadata(typeof(BarItemLinkControlToolTip), new FrameworkPropertyMetadata(PlacementMode.Custom));
		}
		#endregion
		#region dep props
		public bool UseToolTipPlacementTarget {
			get { return (bool)GetValue(UseToolTipPlacementTargetProperty); }
			set { SetValue(UseToolTipPlacementTargetProperty, value); }
		}
		#endregion
		public BarItemLinkControlToolTip() {
			CustomPopupPlacementCallback = CalculatePlacement;
		}
		new public PlacementMode Placement { get { return base.Placement; } protected set { base.Placement = value; } }
		protected internal Size GetMouseCursorSize() {
			Popup parentPopup = typeof(ToolTip).GetField("_parentPopup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Popup;
			object[] args = new object[] { 0, 0, 0, 0 };
			typeof(Popup).GetMethod("GetMouseCursorSize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Invoke(parentPopup, args);
			int width = (int)args[0];
			int height = (int)args[1];
			int hotX = (int)args[2];
			int hotY = (int)args[3];
			return new Size(Math.Max(0, width - hotX), Math.Max(0, height - hotY + 1));
		}
		private Thickness GetMouseCursorPos(object targetElement, FlowDirection flowDirection) {
			Thickness pos = new Thickness();
			if(targetElement is IInputElement) {
				Point topLeftPos = Mouse.GetPosition(PlacementTarget as IInputElement);
				pos.Top = topLeftPos.Y;
				pos.Left = topLeftPos.X;
				Size cursorSize = GetMouseCursorSize();
				if(flowDirection == FlowDirection.RightToLeft) {
					pos.Right = pos.Left - cursorSize.Width;
				} else {
					pos.Right = pos.Left + cursorSize.Width;
				}
				pos.Bottom = pos.Top + cursorSize.Height;
				return pos;
			}
			return pos;
		}
		protected virtual CustomPopupPlacement[] CalculatePlacement(Size toolTipSize, Size targetSize, Point offset) {
			FlowDirection flowDirection = PlacementTarget != null ? (FlowDirection)PlacementTarget.GetValue(FrameworkElement.FlowDirectionProperty) : FlowDirection.LeftToRight;
			if(UseToolTipPlacementTarget && PlacementTarget is DependencyObject) {
				IToolTipPlacementTarget propertiesSource = (PlacementTarget as Visual).VisualParents(true).OfType<IToolTipPlacementTarget>().FirstOrDefault();
				if(propertiesSource != null && propertiesSource.ExternalPlacementTarget is UIElement) {
					return CalculatePlacement(propertiesSource, targetSize, toolTipSize, offset, flowDirection);
				}
			}
			Rect targetBounds = new Rect(new Point(), targetSize);
			Thickness mouseCursorPos = GetMouseCursorPos(PlacementTarget, flowDirection);
			BarItemLinkControlToolTipHorizontalPlacement horizontalPlacement = PlacementTarget is DependencyObject ? BarItemLinkControlToolTip.GetHorizontalPlacement(PlacementTarget) : BarItemLinkControlToolTipHorizontalPlacement.System;
			BarItemLinkControlToolTipVerticalPlacement verticalPlacement = PlacementTarget is DependencyObject ? BarItemLinkControlToolTip.GetVerticalPlacement(PlacementTarget) : BarItemLinkControlToolTipVerticalPlacement.System;
			double x = Math.Round(CalcHorizontalOffset(horizontalPlacement, toolTipSize, targetBounds, mouseCursorPos));
			double y = Math.Round(CalcVerticalOffset(verticalPlacement, toolTipSize, targetBounds, mouseCursorPos));
			if(flowDirection == FlowDirection.RightToLeft) {
				x = -x - toolTipSize.Width;
			}
			return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None) };
		}
		protected virtual CustomPopupPlacement[] CalculatePlacement(IToolTipPlacementTarget propertiesSource, Size internalTargetSize, Size toolTipSize, Point offset, FlowDirection flowDirection) {
			Rect externalTargetBounds = new Rect(new Point(), internalTargetSize);
			Rect internalTargetBounds = externalTargetBounds;			
			UIElement externalTarget = propertiesSource.ExternalPlacementTarget as UIElement;
			Thickness mouseCursorPos = GetMouseCursorPos(PlacementTarget, flowDirection);
			if(externalTarget != null && PlacementTarget is Visual) {
				Point placementTargetLeftTopScreenPoint = PlacementTarget.PointToScreen(new Point(0, 0));
				Point externalTargetLeftTopScreenPoint = externalTarget.PointToScreen(new Point(0, 0));
				Point externalTargetRightBottomScreenPoint = externalTarget.PointToScreen(new Point(externalTarget.RenderSize.Width, externalTarget.RenderSize.Height));
				externalTargetBounds = new Rect(externalTargetLeftTopScreenPoint.X - placementTargetLeftTopScreenPoint.X,
						externalTargetLeftTopScreenPoint.Y - placementTargetLeftTopScreenPoint.Y,
						Math.Abs(externalTargetRightBottomScreenPoint.X - externalTargetLeftTopScreenPoint.X),
						externalTargetRightBottomScreenPoint.Y - externalTargetLeftTopScreenPoint.Y);
			}
			if(flowDirection == FlowDirection.RightToLeft) {
				externalTargetBounds.X = -externalTargetBounds.X;
			}
			double x = Math.Round(CalcHorizontalOffset(propertiesSource.HorizontalPlacement, toolTipSize, propertiesSource.HorizontalPlacementTargetType == BarItemLinkControlToolTipPlacementTargetType.External ? externalTargetBounds : internalTargetBounds, mouseCursorPos));
			if(flowDirection == FlowDirection.RightToLeft) {
				x = -x - toolTipSize.Width;
			}
			double y = Math.Round(CalcVerticalOffset(propertiesSource.VerticalPlacement, toolTipSize, propertiesSource.VerticalPlacementTargetType == BarItemLinkControlToolTipPlacementTargetType.External ? externalTargetBounds : internalTargetBounds, mouseCursorPos));
			if(propertiesSource.HorizontalOffset.IsNumber()) {
				if(flowDirection == System.Windows.FlowDirection.LeftToRight)
					x = x - offset.X + propertiesSource.HorizontalOffset;
				else {
					if(propertiesSource.HorizontalPlacement == BarItemLinkControlToolTipHorizontalPlacement.System ||
						propertiesSource.HorizontalPlacement == BarItemLinkControlToolTipHorizontalPlacement.LeftAtMousePoint ||
						propertiesSource.HorizontalPlacement == BarItemLinkControlToolTipHorizontalPlacement.RightAtMousePoint) {
						x = x + offset.X + propertiesSource.HorizontalOffset;
					} else {
						x = x + offset.X - propertiesSource.HorizontalOffset;
					}
				}
			}
			if(propertiesSource.VerticalOffset.IsNumber())
				y = y - offset.Y + propertiesSource.VerticalOffset;
			return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None) };
		}
		internal static double CalcVerticalOffset(BarItemLinkControlToolTipVerticalPlacement verticalPlacement, Size toolTipSize, Rect targetBounds, Thickness mouseCursorPos) {
			double offsetY = 0;
			switch(verticalPlacement) {
				case BarItemLinkControlToolTipVerticalPlacement.BottomAtTargetBottom:
					offsetY = targetBounds.Bottom;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.BottomAtTargetTop:
					offsetY = targetBounds.Top;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.CenterAtTargetCenter:
					offsetY = targetBounds.Top + targetBounds.Height / 2 - toolTipSize.Height / 2;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.TopAtTargetBottom:
					offsetY = targetBounds.Bottom - toolTipSize.Height;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.TopAtTargetTop:
					offsetY = targetBounds.Top - toolTipSize.Height;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.System:
				case BarItemLinkControlToolTipVerticalPlacement.BottomAtMouse:
					offsetY = mouseCursorPos.Bottom;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.TopAtMouse:
					offsetY = mouseCursorPos.Bottom - toolTipSize.Height;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.BottomAtMousePoint:
					offsetY = mouseCursorPos.Top;
					break;
				case BarItemLinkControlToolTipVerticalPlacement.TopAtMousePoint:
					offsetY = mouseCursorPos.Top - toolTipSize.Height;
					break;
				default:
					throw new InvalidOperationException("Unsupported verticalPlacement - " + verticalPlacement.ToString());
			}
			return offsetY;
		}
		internal static double CalcHorizontalOffset(BarItemLinkControlToolTipHorizontalPlacement horizontalPlacement, Size toolTipSize, Rect targetBounds, Thickness mouseCursorPos, FlowDirection flowDirection = FlowDirection.LeftToRight) {
			double offsetX = 0;
			switch(horizontalPlacement) {
				case BarItemLinkControlToolTipHorizontalPlacement.CenterAtTargetCenter:
					offsetX = targetBounds.Left + targetBounds.Width / 2 - toolTipSize.Width / 2;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.LeftAtTargetLeft:
					offsetX = targetBounds.Left - toolTipSize.Width;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.LeftAtTargetRight:
					offsetX = targetBounds.Right - toolTipSize.Width;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.RightAtTargetLeft:
					offsetX = targetBounds.Left;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.RightAtTargetRight:
					offsetX = targetBounds.Right;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.LeftAtMouse:
					offsetX = mouseCursorPos.Right - toolTipSize.Width;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.RightAtMouse:
					offsetX = mouseCursorPos.Right;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.LeftAtMousePoint:
					offsetX = mouseCursorPos.Left - toolTipSize.Width;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.RightAtMousePoint:
					offsetX = mouseCursorPos.Left;
					break;
				case BarItemLinkControlToolTipHorizontalPlacement.System:
					if(SystemParameters.MenuDropAlignment)
						offsetX = mouseCursorPos.Right - toolTipSize.Width;
					else
						offsetX = mouseCursorPos.Left;
					break;
				default:
					throw new InvalidOperationException("Unsupported horizontalPlacement - " + horizontalPlacement.ToString());
			}
			return offsetX;
		}
	}
	public interface IToolTipPlacementTarget {
		BarItemLinkControlToolTipHorizontalPlacement HorizontalPlacement { get; }
		BarItemLinkControlToolTipVerticalPlacement VerticalPlacement { get; }
		BarItemLinkControlToolTipPlacementTargetType HorizontalPlacementTargetType { get; }
		BarItemLinkControlToolTipPlacementTargetType VerticalPlacementTargetType { get; }
		double HorizontalOffset { get; }
		double VerticalOffset { get; }
		DependencyObject ExternalPlacementTarget { get; }
	}
}
