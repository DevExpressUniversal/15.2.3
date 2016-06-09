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
using DevExpress.Xpf.Docking.ThemeKeys;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class DocumentSelectorContainerFactory {
		public static FloatingContainer CreateDocumentSelectorContainer(DockLayoutManager manager, object content) {
			FloatingContainer container = FloatingContainerFactory.Create(Core.FloatingMode.Window);
			container.BeginUpdate();
			container.CloseOnEscape = true;
			container.Owner = manager;
			container.ContainerStartupLocation = WindowStartupLocation.Manual;
			container.AllowMoving = false;
			container.AllowSizing = false;
			container.ContainerTemplate = null;
			container.ShowModal = true;
			container.Content = content;
			FloatingContainer.SetFloatingContainer((DependencyObject)content, container);
			manager.AddToLogicalTree(container, content);
			container.EndUpdate();
			return container;
		}
		public static DataTemplate GetTemplate(DockLayoutManager manager) {
			return ResourceHelper.FindResource(manager, new DocumentSelectorElementsThemeKeyExtension()
					{
						ResourceKey = DocumentSelectorElements.Template,
						ThemeName = DockLayoutManagerExtension.GetThemeName(manager)
					}
				) as DataTemplate;
		}
		public static Size GetSize(DockLayoutManager manager, Size minSize) {
			return new Size(
				Math.Max(minSize.Width, manager.ActualWidth * 0.6),
				Math.Max(minSize.Height, manager.ActualHeight * 0.6)
			);
		}
		public static Point GetLocation(DockLayoutManager manager, Size size) {
			Point location = new Point((manager.ActualWidth - size.Width) * 0.5, (manager.ActualHeight - size.Height) * 0.5);
			if(manager.FlowDirection == FlowDirection.RightToLeft)
				location.X += size.Width;
			return location;
		}
	}
	public class DocumentSelectorContainer : psvControl {
		static DocumentSelectorContainer() {
			var dProp = new DependencyPropertyRegistrator<DocumentSelectorContainer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
	}
}
