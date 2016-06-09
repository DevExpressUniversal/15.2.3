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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IUIElementInfo {
		Type GetUIElementKey();
	}
	public interface IBaseElementInfo : IUIElementInfo, IDisposable {
		BaseView Owner { get; }
		bool IsVisible { get; }
		bool IsDisposing { get; }
		Rectangle Bounds { get; }
		void Calc(Graphics g, Rectangle bounds);
		void Draw(GraphicsCache cache);
		void UpdateStyle();
		void ResetStyle();
	}
	public interface IBaseSplitterInfo : IBaseElementInfo {
		bool IsHorizontal { get; }
		int SplitLength1 { get; set; }
		int SplitLength2 { get; set; }
		int SplitConstraint1 { get; }
		int SplitConstraint2 { get; }
		BaseDocument[] GetDocuments();
	}
	public interface IBaseDocumentInfo : IBaseElementInfo {
		BaseDocument BaseDocument { get; }
	}
	public interface IDockingAdornerInfo : IBaseElementInfo {
		void UpdateDocking(Adorner adorner, Point point, BaseDocument dragItem);
		void ResetDocking(Adorner adorner);
		bool CanDock(Point point);
		void Dock(Point point, BaseDocument dragItem);
	}
	public interface IInteractiveElementInfo {
		void ProcessMouseDown(MouseEventArgs e);
		void ProcessMouseMove(MouseEventArgs e);
		void ProcessMouseUp(MouseEventArgs e);
		void ProcessMouseLeave();
		void ProcessMouseWheel(MouseEventArgs e);
		bool ProcessFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args);
		bool ProcessGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters);
	}
	public interface IScrollableElementInfo {
		bool IsHorizontal { get; }
		void OnStartScroll();
		void OnScroll(int delta);
		void OnEndScroll();
	}
	public interface IAppearanceCollectionAccessor {
		DevExpress.Utils.BaseAppearanceCollection Appearances { get; }
	}
	public interface IContainerRemove {
		void ContainerRemoved();
	}
	public interface IDesignTimeSupport {
		bool IsLoaded { get; }
		bool IsSerializing { get; set; }
		void Load();
		void Unload();
	}
	public interface IDocumentsHostWindowRoot : DragEngine.IUIElement, IBaseDocumentInfo {
		IDocumentsHostWindow Window { get; }
	}
	public interface IDocumentCaptionAppearanceProvider {
		AppearanceObject ActiveCaptionAppearance { get; }
		AppearanceObject CaptionAppearance { get; }
		bool AllowCaptionColorBlending { get; }
	}
}
