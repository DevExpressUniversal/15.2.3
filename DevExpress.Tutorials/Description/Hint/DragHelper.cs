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
using System.Windows.Input;
namespace DevExpress.NoteHint
{
  class DragHelper
  {
	Window window;
	FrameworkElement container;
	FrameworkElement dragElement;
	Point dragOffset;
	bool isInDragging;
	void StopDrag()
	{
	  if (isInDragging)
	  {
		isInDragging = false;
		dragElement.ReleaseMouseCapture();
		if (StopDragging != null)
		  StopDragging(this, EventArgs.Empty);
	  }
	}
	void SubsribeEvents(FrameworkElement dragElement)
	{
	  if (dragElement == null)
		return;
	  dragElement.MouseDown += ehMouseDown;
	  dragElement.MouseMove += ehMouseMove;
	  dragElement.MouseLeftButtonUp += ehMouseLeftButtonUp;
	}
	void UnsubscribeEvents()
	{
	  if (dragElement == null)
		return;
	  dragElement.MouseDown -= ehMouseDown;
	  dragElement.MouseMove -= ehMouseMove;
	  dragElement.MouseLeftButtonUp -= ehMouseLeftButtonUp;
	}
	void ehMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
	  if (e.LeftButton != MouseButtonState.Pressed)
		return;
	  isInDragging = true;
	  dragOffset = e.GetPosition(container);
	  dragElement.CaptureMouse();
	  if (StartDragging != null)
		StartDragging(this, EventArgs.Empty);
	}
	void ehMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
	{
	  if (!isInDragging)
		return;
	  Point point = e.GetPosition(container);
	  double deltaX = point.X - dragOffset.X;
	  double deltaY = point.Y - dragOffset.Y;
	  Point newLocation = new Point(this.window.Left + deltaX, this.window.Top + deltaY);
	  window.Left = newLocation.X;
	  window.Top = newLocation.Y;
	}
	void ehMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
	  StopDrag();
	}
	public void Start(Window window, FrameworkElement container, FrameworkElement dragElement)
	{
	  this.window = window;
	  this.container = container;
	  this.dragElement = dragElement;
	  SubsribeEvents(dragElement);
	}
	public void Stop()
	{
	  StopDrag();
	  UnsubscribeEvents();
	}
	public event EventHandler StartDragging;
	public event EventHandler StopDragging;
  }
}
