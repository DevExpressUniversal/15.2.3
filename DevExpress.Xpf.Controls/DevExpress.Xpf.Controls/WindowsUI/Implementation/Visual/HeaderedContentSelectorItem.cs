﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.Xpf.WindowsUI.Base {
	public class veHeaderedContentSelectorItem : veContentSelectorItem {
		#region static
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderStringFormatProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualHeaderProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasHeaderProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasHeaderTemplateProperty;
		static veHeaderedContentSelectorItem() {
			var dProp = new DependencyPropertyRegistrator<veHeaderedContentSelectorItem>();
			dProp.Register("Header", ref HeaderProperty, (object)null,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHeaderChanged(e.OldValue, e.NewValue));
			dProp.Register("HeaderStringFormat", ref HeaderStringFormatProperty, (string)null,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHeaderStringFormatChanged((string)e.NewValue));
			dProp.Register("HeaderTemplate", ref HeaderTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHeaderTemplateChanged((DataTemplate)e.NewValue));
			dProp.Register("HeaderTemplateSelector", ref HeaderTemplateSelectorProperty, (DataTemplateSelector)null,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHeaderTemplateSelectorChanged());
			dProp.Register("ActualHeader", ref ActualHeaderProperty, (object)null);
			dProp.Register("HasHeader", ref HasHeaderProperty, (bool)false,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHasHeaderChanged());
			dProp.Register("HasHeaderTemplate", ref HasHeaderTemplateProperty, (bool)false,
				(dObj, e) => ((veHeaderedContentSelectorItem)dObj).OnHasHeaderTemplateChanged());
		}
		#endregion
		protected internal veHeaderedContentSelectorItem() {
			UpdateActualHeader(Header, HeaderStringFormat);
		}
		protected virtual void OnHeaderChanged(object oldHeader, object newHeader) {
			UpdateActualHeader(newHeader, HeaderStringFormat);
		}
		protected virtual void OnHeaderStringFormatChanged(string headerFormat) {
			UpdateActualHeader(Header, headerFormat);
		}
		void UpdateActualHeader(object header, string headerFormat) {
			object actualHeader = HeaderedControlHelper.GetActualHeader(header, headerFormat, Language.IetfLanguageTag);
			SetValue(ActualHeaderProperty, actualHeader);
			SetValue(HasHeaderProperty, actualHeader != null);
		}
		protected virtual void OnHasHeaderChanged() { }
		protected virtual void OnHasHeaderTemplateChanged() { }
		protected virtual void OnHeaderTemplateChanged(DataTemplate headerTemplate) {
			SetValue(HasHeaderTemplateProperty, headerTemplate != null);
		}
		protected virtual void OnHeaderTemplateSelectorChanged() { }
		void PrepareHeaderedContentControl(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			if(item != this) {
				if(!(item is UIElement)) {
					Header = item;
				}
				if(itemTemplate != null) {
					base.SetValue(HeaderTemplateProperty, itemTemplate);
				}
				if(itemTemplateSelector != null) {
					base.SetValue(HeaderTemplateSelectorProperty, itemTemplateSelector);
				}
				ClearValue(ContentTemplateProperty);
				ClearValue(ContentTemplateSelectorProperty);
			}
		}
		protected override void ClearContainer() {
			ClearValue(HeaderProperty);
			ClearValue(HeaderTemplateProperty);
			ClearValue(HeaderTemplateSelectorProperty);
			ClearValue(HeaderStringFormatProperty);
			ClearValue(ActualHeaderProperty);
			base.ClearContainer();
		}
		protected override void PrepareContainer(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			PrepareHeaderedContentControl(item, itemTemplate, itemTemplateSelector);
		}
		#region Properties
		[TypeConverter(typeof(StringConverter))]
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		public string HeaderStringFormat {
			get { return (string)GetValue(HeaderStringFormatProperty); }
			set { SetValue(HeaderStringFormatProperty, value); }
		}
		public object ActualHeader {
			get { return GetValue(ActualHeaderProperty); }
		}
		public bool HasHeader {
			get { return (bool)GetValue(HasHeaderProperty); }
		}
		public bool HasHeaderTemplate {
			get { return (bool)GetValue(HasHeaderTemplateProperty); }
		}
		#endregion Properties
	}
}
