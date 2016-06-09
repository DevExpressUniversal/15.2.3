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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System.Windows;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Utils;
using System.Windows.Media;
namespace DevExpress.Xpf.Scheduler.UI {
	public abstract class FixedSourceComboBoxEditSettings : ComboBoxEditSettings {
		protected FixedSourceComboBoxEditSettings() {
			PopulateItems();
		}
		protected abstract void PopulateItems();
	}
	public abstract class FixedSourceComboBoxEdit : ComboBoxEdit {
		protected FixedSourceComboBoxEdit() {
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
			IsTextEditable = false;
		}
	}
	public abstract class FixedSourceListBoxEdit : ListBoxEdit {
		protected FixedSourceListBoxEdit() {
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
		}
		protected abstract void FillFixedSourceItems(NamedElementList sourceItems);
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			NamedElementList items = new NamedElementList();
			FillFixedSourceItems(items);
			ItemsSource = items;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class NamedElement : DependencyObject {
		public const string DisplayMember = "Caption";
		public const string ValueMember = "Id";
		public NamedElement() { 
		}
		public NamedElement(object id, string caption) {
			Id = id;
			Caption = caption;
		}
		public NamedElement(object id, string caption, object dataContext) {
			Id = id;
			Caption = caption;
			DataContext = dataContext;
		}
		#region Caption
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NamedElement, string>("Caption", String.Empty);
		#endregion
		#region Id
		public object Id {
			get { return (object)GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}
		public static readonly DependencyProperty IdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NamedElement, object>("Id", null);
		#endregion
		#region DataContext
		public object DataContext {
			get { return (object)GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}
		public static readonly DependencyProperty DataContextProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NamedElement, object>("DataContext", null);
		#endregion
		public override string ToString() { 
			return Caption;
		}
	}
	 public class ColoredNamedElement : NamedElement {
		public ColoredNamedElement(object id, string caption, Color color)
			: base(id, caption) {
			Color = color;
		}
		#region Color
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ColoredNamedElement, Color>("Color", ColorExtension.Empty);
		#endregion
	}
	public class NamedElementList : List<NamedElement> { 
	}
}
