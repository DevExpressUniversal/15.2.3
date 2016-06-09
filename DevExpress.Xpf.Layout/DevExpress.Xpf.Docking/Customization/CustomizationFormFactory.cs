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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Images;
using DevExpress.Xpf.Docking.Platform;
using ScreenHelper = DevExpress.Xpf.Core.Native.ScreenHelper;
namespace DevExpress.Xpf.Docking.Customization {
	public class CustomizationFormFactory {
		static Size DesiredSize = new Size(350, 400);
		public static FloatingContainer CreateCustomizationForm(DockLayoutManager manager, object content) {
			FloatingContainer container = FloatingContainerFactory.Create(Core.FloatingMode.Window);
			container.BeginUpdate();
			container.Owner = manager;
			container.MinWidth = 300;
			container.MinHeight = 300;
			container.FloatSize = DesiredSize;
			container.FloatLocation = CalcFormLocation(manager, container);
			container.Icon = ImageHelper.GetImage(DefaultImages.BeginCustomizationIcon);
			container.Caption = DockingLocalizer.GetString(DockingStringId.TitleCustomizationForm);
			container.Content = content;
			manager.AddToLogicalTree(container, content);
			container.EndUpdate();
			return container;
		}
		static Point CalcFormLocation(DockLayoutManager owner, FloatingContainer container) {
			Size desiredSize = container.FloatSize;
			Point location = new Point(owner.ActualWidth - desiredSize.Width, 0);
			if(WindowHelper.IsXBAP) {
				location.Y = owner.ActualHeight - desiredSize.Height;
			}
			if(owner.FlowDirection == FlowDirection.RightToLeft) {
				if(container is FloatingAdornerContainer) {
					if(!((FloatingAdornerContainer)container).InvertLeftAndRightOffsets)
						return location;
				}
				location.X += desiredSize.Width;
			}
			bool inPopup = DockLayoutManagerHelper.IsPopupRoot(Core.Native.LayoutHelper.FindRoot(owner));
			if(!inPopup) {
				Window ownerWindow = Window.GetWindow(owner);
				if(ownerWindow != null) {
					var relativePosition = ownerWindow.TransformToDescendant(owner).Transform(location);
					if(relativePosition.Y > 0) location.Y = relativePosition.Y;
				}
				var screenLocation = ScreenHelper.GetScreenLocation(location, owner);
				location = ScreenHelper.GetClientLocation(ScreenHelper.UpdateContainerLocation(new Rect(screenLocation, desiredSize)), owner);
			}
			return location;
		}
	}
}
