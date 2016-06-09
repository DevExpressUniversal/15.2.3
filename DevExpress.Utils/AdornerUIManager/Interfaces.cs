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
using System.Collections.Generic;
using DevExpress.Utils.Base;
using System.ComponentModel;
namespace DevExpress.Utils.VisualEffects {
	public interface IAdornerElement : IBaseObject, ICloneable {
		AppearanceObject Appearance { get; }
		AppearanceObject ParentAppearance { get; }
		object TargetElement { get; set; }
		bool Visible { get; set; }
		IAdornerElementViewInfo ViewInfo { get; }
		AdornerElementPainter Painter { get; }
		IBaseDefaultProperties Properties { get; }
		void EnsureParentProperties(IBaseProperties parentProperties);
		void EnsureParentAppearance(AppearanceObject parentAppearance);
		bool HitTest(Point p);
		int NCHitTest { get; }
		event EventHandler Updated;
		event EventHandler TargetChanged;
		bool IsVisible { get; }
		void Update();
		void Invalidate();
		void Assign(IAdornerElement element);
	}
	public interface IAdornerElementViewInfo : IDisposable {
		AppearanceObject PaintAppearance { get; }
		void Calc(Graphics g, Rectangle targetElementBounds);
		Rectangle Bounds { get; }
		IAdornerElement Element { get; }
		IEnumerable<Rectangle> CalcRegions();
		void SetDirty();
		bool IsReady { get; }
		Size CalcMinSize(Graphics g);
	}
	public interface ISupportAdornerElement {
		Rectangle Bounds { get; }
		ISupportAdornerUIManager Owner { get; }
		bool IsVisible { get; }
		event UpdateActionEventHandler Changed;
	}
	public interface ISupportAdornerElementBarItem {
		IEnumerable<ISupportAdornerElementBarItemLink> Elements { get; }
		event CollectionChangeEventHandler CollectionChanged;
		event UpdateActionEventHandler Changed;
	}
	public interface ISupportAdornerElementBarItemLink : ISupportAdornerElement {
	}
	public interface ISupportAdornerUIManager {
		event UpdateActionEventHandler Changed;
		void UpdateVisualEffects(UpdateAction action);
		IntPtr Handle { get; }
		bool IsHandleCreated { get; }
		Rectangle ClientRectangle { get; }
	}
	public interface IAdornerUIManager : ISupportBatchUpdate {
	}
	interface IAdornerUIManagerInternal : IAdornerUIManager {
		void RegisterElement(IAdornerElement element);
		void UnregisterElement(IAdornerElement element);
		void UpdateLayer(bool updateRegions);
	}
	public enum BadgeTypeColor { Custom, Critical, Information, Warning, Question }
	public delegate void UpdateActionEventHandler(object sender, UpdateActionEvendArgs e);
	public class UpdateActionEvendArgs {
		UpdateAction actionCore;
		public UpdateActionEvendArgs(UpdateAction action) {
			actionCore = action;
		}
		public UpdateAction Action { get { return actionCore; } }
	}
	public enum UpdateAction { BeginUpdate, Update, Invalidate, EndUpdate, Dispose, OwnerChanged }
	interface IAdornerElementNode : IDisposable {
		List<IAdornerElementNode> Nodes { get; }
		IAdornerElementNode ParentNode { get; }
		object Element { get; }
		IAdornerElementsTree Tree { get; }
		IEnumerable<IAdornerElement> GetAdornerElements();
	}
	interface IAdornerElementsTree : ISupportBatchUpdate {
		bool MoveNode(IAdornerElementNode node);
	}
}
