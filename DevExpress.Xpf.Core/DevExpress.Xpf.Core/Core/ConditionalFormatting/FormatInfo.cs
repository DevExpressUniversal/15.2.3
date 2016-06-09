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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.GridData;
using DevExpress.Data;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using System.ComponentModel;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class FormatInfo : Freezable {
		public string FormatName {
			get { return (string)GetValue(FormatNameProperty); }
			set { SetValue(FormatNameProperty, value); }
		}
		public static readonly DependencyProperty FormatNameProperty = DependencyProperty.Register("FormatName", typeof(string), typeof(FormatInfo), new PropertyMetadata(null));
		public string DisplayName {
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(FormatInfo), new PropertyMetadata(null));
		public object Format {
			get { return GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(object), typeof(FormatInfo), new PropertyMetadata(null));
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(FormatInfo), new PropertyMetadata(null));
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FormatInfo), new PropertyMetadata(null));
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(FormatInfo), new PropertyMetadata(null));
		internal bool IsCustom { get { return string.IsNullOrEmpty(FormatName); } }
		public override string ToString() {
			return DisplayName;
		}
		protected override Freezable CreateInstanceCore() {
			return new FormatInfo();
		}
	}
	public class FormatInfoCollection : FreezableCollection<FormatInfo> {
		public FormatInfo this[string name] { get { return this.FirstOrDefault(x => x.FormatName == name); } }
		protected override Freezable CreateInstanceCore() {
			return new FormatInfoCollection();
		}
	}
}
