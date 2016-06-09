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
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.Windows.Forms;
#if !DXWINDOW
using DevExpress.Utils.Design;
using DevExpress.WebUtils;
#endif
#if !SL && !DXWINDOW && !DXPORTABLE
namespace DevExpress.Utils {
	public class ControlConstants {
#if DXWhidbey
		public const SelectionTypes SelectionNormal = SelectionTypes.Auto;
		public const SelectionTypes SelectionClick = SelectionTypes.Primary;
		public const string DataMemberEditor = "System.Windows.Forms.Design.DataMemberListEditor, System.Design";
		public const ControlStyles DoubleBuffer = ControlStyles.OptimizedDoubleBuffer;
		public const ViewTechnology ViewTechnologyDefault = ViewTechnology.Default;
		public const SelectionTypes SelectionTypeAuto = SelectionTypes.Auto;
		public const string BitmapPath = "Bitmaps256.";
		public const bool NonObjectBindable = false;
		public const string MultilineStringEditor = "System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
#else
		public const string DataMemberEditor = "System.Windows.Forms.Design.DataMemberListEditor, System.Design";
		public const SelectionTypes SelectionClick = SelectionTypes.Click;
		public const SelectionTypes SelectionNormal = SelectionTypes.Normal;
		public const ControlStyles DoubleBuffer = ControlStyles.DoubleBuffer;
		public const ViewTechnology ViewTechnologyDefault = ViewTechnology.WindowsForms;
		public const SelectionTypes SelectionTypeAuto = SelectionTypes.Normal;
		public const string BitmapPath = "Bitmaps.";
		public const bool NonObjectBindable = false;
#endif
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class HiddenToolboxItemAttribute : Attribute {
	}
}
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Data {
#else
namespace DevExpress.Utils.Controls {
#endif
	public abstract class DisposableObject : IDisposable {
		bool isDisposed;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsDisposed { get { return isDisposed; } }
		[Browsable(false)]
		public event EventHandler Disposed;
		~DisposableObject() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			isDisposed = true;
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
	}
	public interface IFilterItem {
		bool? IsChecked { get; set; }
		bool IsVisible { get; set; }
	}
	public interface IFilterItems : IEnumerable {
		bool? CheckState { get; }
		bool CanAccept { get; }
		int Count { get; }
		void ApplyFilter();
		void CheckAllItems(bool isChecked);
		IFilterItem this[int index] { get; }
	}
}
namespace DevExpress.Utils {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	public class ToolboxTabNameAttribute : Attribute {
		string tabName;
		public ToolboxTabNameAttribute(string tabName) { this.tabName = tabName; }
		public string TabName { get { return tabName; } }
	}
}
namespace System.ComponentModel {
	public enum DXToolboxItemKind { Free, Regular, Hidden };
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.None)]
	public class DXToolboxItemAttribute : ToolboxItemAttribute {
		protected DXToolboxItemAttribute(DXToolboxItemKind kind, string toolboxTypeName) : base(toolboxTypeName) {
#if !DXWINDOW && !DXPORTABLE
#endif
		}
		public DXToolboxItemAttribute(DXToolboxItemKind kind)
			: base(kind != DXToolboxItemKind.Hidden) {
#if !DXWINDOW && !DXPORTABLE
#endif
		}
		public DXToolboxItemAttribute(bool defaultType)
			: this(defaultType  ? DXToolboxItemKind.Regular : DXToolboxItemKind.Hidden) {
		}
#if !DXWINDOW && !DXPORTABLE
#endif
		public override object TypeId { get { return ToolboxItemAttribute.Default.TypeId; } }
	}
	public class DXWebToolboxItemAttribute : DXToolboxItemAttribute {
		public DXWebToolboxItemAttribute(DXToolboxItemKind kind) : base(kind, "System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") { }
		public DXWebToolboxItemAttribute(bool defaultType)
			: this(defaultType ? DXToolboxItemKind.Regular : DXToolboxItemKind.Hidden) { }
#if !DXWINDOW && !DXPORTABLE
#endif
		public override object TypeId { get { return ToolboxItemAttribute.Default.TypeId; } }
	}
}
