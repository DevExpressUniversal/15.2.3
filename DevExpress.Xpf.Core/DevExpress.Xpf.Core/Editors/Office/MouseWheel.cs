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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Office.Internal {
	public interface IMouseWheelScrollClient {
		void OnMouseWheel(MouseWheelEventArgs e);
	}
	public class MouseWheelScrollHelper {
		int distance = 0;
		DateTime lastEvent = DateTime.MinValue;
		const int SkipInterval = 400;
		const int PixelLineHeight = 120;
		IMouseWheelScrollClient client;
		public MouseWheelScrollHelper(IMouseWheelScrollClient client, bool usePixeScrolling) {
			this.client = client;
			UsePixelScrolling = usePixeScrolling;
		}
		public bool UsePixelScrolling { get; private set; }
		public void OnMouseWheel(MouseWheelEventArgs e) {
#if !SL
			MouseWheelEventArgsEx dxmea = e as MouseWheelEventArgsEx;
			if (DateTime.Now.Subtract(lastEvent).TotalMilliseconds > SkipInterval) 
				ResetDistance();
			this.lastEvent = DateTime.Now;
			int delta = (dxmea == null) ? e.Delta : dxmea.DeltaX;
#else
			int delta = e.Delta;
#endif
			if (UsePixelScrolling) {
				OnScrollLine(e, delta, true);
				return;
			}
			if (delta % 120 == 0 && distance == 0) {
				OnScrollLine(e, delta / PixelLineHeight, true);
				return;
			}
			if (Math.Sign(distance) != Math.Sign(delta))
				distance = 0;
			distance += delta;
			int lineCount = distance / PixelLineHeight;
			distance = distance % PixelLineHeight;
			if (lineCount == 0)
				return;
			OnScrollLine(e, lineCount, false);
		}
		void ResetDistance() {
			this.distance = 0;
		}
		void OnScrollLine(MouseWheelEventArgs e, int linesCount, bool allowSystemLinesCount) {
			if (client == null)
				return;
#if !SL
			if (e is MouseWheelEventArgsEx)
				client.OnMouseWheel(new MouseWheelEventArgsEx(e.MouseDevice, e.Timestamp, linesCount * PixelLineHeight, 0));
			else
				client.OnMouseWheel(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, linesCount * PixelLineHeight));
#else
			client.OnMouseWheel(e);
#endif
		}
		int GetDirection(MouseWheelEventArgs e) {
			return e.Delta > 0 ? -1 : 1;
		}
	}
	public class OfficeMouseWheelScrollBehavior<T> : Behavior<T> where T : Control, IMouseWheelScrollClient {
		MouseWheelScrollHelper scrollHelper;
		bool usePixelScroll;
		public OfficeMouseWheelScrollBehavior(bool usePixelScroll) {
			this.usePixelScroll = usePixelScroll;
		}
		protected override void OnAttached() {
			base.OnAttached();
			this.scrollHelper = new MouseWheelScrollHelper(AssociatedObject, this.usePixelScroll);
#if !SL
			AssociatedObject.Loaded += AssociatedObjectLoaded;
			AssociatedObject.PreviewMouseWheel += AssociatedObjectPreviewMouseWheel;
#else
			AssociatedObject.MouseWheel += AssociatedObjectPreviewMouseWheel;
#endif
		}
		protected override void OnDetaching() {
#if !SL
			AssociatedObject.Loaded -= AssociatedObjectLoaded;
			AssociatedObject.PreviewMouseWheel -= AssociatedObjectPreviewMouseWheel;
#else
			AssociatedObject.MouseWheel -= AssociatedObjectPreviewMouseWheel;
#endif
			base.OnDetaching();
		}
#if !SL
		void AssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e) {
			VerifyMouseHWheelListening();
		}
		void VerifyMouseHWheelListening() {
			if (!AssociatedObject.IsLoaded)
				return;
			var root = LayoutHelper.FindRoot(AssociatedObject);
			if (object.Equals(root, AssociatedObject))
				return;
			var behaviors = Interaction.GetBehaviors(root);
			var behavior = behaviors.FirstOrDefault(x => x is HWndHostWMMouseHWheelBehavior);
			if (behavior == null)
				behaviors.Add(new HWndHostWMMouseHWheelBehavior());
		}
#endif
		void AssociatedObjectPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			this.scrollHelper.OnMouseWheel(e);
			e.Handled = true;
		}
	}
}
