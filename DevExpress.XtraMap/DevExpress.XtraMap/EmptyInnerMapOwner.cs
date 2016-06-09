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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap.Native {
	public sealed class EmptyInnerMapOwner : IMapControl {
		static readonly IMapControl instance = new EmptyInnerMapOwner();
		public static IMapControl Instance { get { return instance; } }
		bool IMapControl.IsDisposed { get { return false; } }
		bool IMapControl.IsDesignMode { get { return false; } }
		bool IMapControl.IsDesigntimeProcess { get { return false; } }
		Cursor IMapControl.Cursor { get { return Cursor.Current; } set { ; } }
		bool IMapControl.Capture { get { return false; } set { ; } }
		IntPtr IMapControl.Handle { get { return IntPtr.Zero; } }
		bool IMapControl.IsHandleCreated { get { return false; } }
		bool IMapControl.IsSkinActive { get { return false; } }
		ISkinProvider IMapControl.SkinProvider { get { return MapDefaultSkinProvider.Default; } }
		ToolTipController IMapControl.ToolTipController { get { return ToolTipController.DefaultController; } }
		Rectangle IMapControl.ClientRectangle { get { return new Rectangle(Point.Empty, InnerMap.DefaultMapControlSize); } }
		Graphics IMapControl.CreateGraphics() { return null; }
		void IMapControl.AddChildControl(Control child) { }
		void IMapControl.RemoveChildControl(Control child) { }
		void IMapControl.Refresh() { }
		void IMapControl.ShowToolTip(string s, Point point) { }
		void IMapControl.HideToolTip() { }
	}
}
