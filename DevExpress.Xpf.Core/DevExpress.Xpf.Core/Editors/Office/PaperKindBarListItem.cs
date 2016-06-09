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
using System.ComponentModel;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Office;
using DevExpress.Xpf.Office.Internal;
using DevExpress.Xpf.Office.UI;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Utils;
using System.Drawing.Printing;
#endif
namespace DevExpress.Xpf.Office.UI {
	#region PaperKindBarListItemBase (abstract class)
	public abstract class PaperKindBarListItemBase : BarListItem {
#if!SL
		[ThreadStatic]
#endif
		static OfficeDefaultBarItemDataTemplates defaultBarItemTemplates;
		#region Properties
		#region DefaultBarItemTemplates
		OfficeDefaultBarItemDataTemplates DefaultBarItemTemplates {
			get {
				if (defaultBarItemTemplates == null) {
					defaultBarItemTemplates = new OfficeDefaultBarItemDataTemplates();
#if SILVERLIGHT
					AddLogicalChild(defaultBarItemTemplates);
					RemoveLogicalChild(defaultBarItemTemplates);
#endif
					defaultBarItemTemplates.ApplyTemplate();
				}
				return defaultBarItemTemplates;
			}
		}
		#endregion
		#region PaperKindList
		public static readonly DependencyProperty PaperKindListProperty = DependencyPropertyManager.Register("PaperKindList", typeof(IList<PaperKind>), typeof(PaperKindBarListItemBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPaperKindListChanged)));
		protected static void OnPaperKindListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PaperKindBarListItemBase instance = d as PaperKindBarListItemBase;
			if (instance != null)
				instance.OnPaperKindListChanged((IList<PaperKind>)e.OldValue, (IList<PaperKind>)e.NewValue);
		}
		public IList<PaperKind> PaperKindList {
			get { return (IList<PaperKind>)GetValue(PaperKindListProperty); }
			set { SetValue(PaperKindListProperty, value); }
		}
		protected internal virtual void OnPaperKindListChanged(IList<PaperKind> oldValue, IList<PaperKind> newValue) {
			UpdateItems();
		}
		#endregion
		protected abstract Control Control { get; }
		protected abstract IList<PaperKind> DefaultPaperKindList { get; }
		#endregion
		public override BarItemLink CreateLink(bool isPrivate) {
			return new PaperKindBarListItemLink();
		}
		protected override void UpdateItems() {
			InternalItems.BeginUpdate();
			try {
				InternalItems.Clear();
				if (Control != null)
					UpdateItemsCore();
			}
			finally {
				InternalItems.EndUpdate();
			}
		}
		protected internal virtual void UpdateItemsCore() {
			ResourceManager resourceManager = OfficeLocalizationHelper.CreateResourceManager(typeof(DevExpress.Data.ResFinder));
			IList<PaperKind> paperKindList = ObtainPaperKindList();
			int count = paperKindList.Count;
			for (int i = 0; i < count; i++) {
				PaperKind paperKind = paperKindList[i];
				string displayName = OfficeLocalizationHelper.GetPaperKindString(resourceManager, paperKind);
				AppendItem(paperKind, displayName);
			}
		}
		IList<PaperKind> ObtainPaperKindList() {
			if (PaperKindList == null || PaperKindList.Count <= 0)
				return DefaultPaperKindList;
			else
				return PaperKindList;
		}
		protected internal virtual void AppendItem(PaperKind paperKind, string displayName) {
			BarButtonItem item = new BarButtonItem();
			item.Content = ObtainPaperKindCaption(paperKind, displayName);
			item.Command = CreateChangeSectionPaperKindCommand(paperKind);
			item.CommandParameter = Control;
			item.ContentTemplate = DefaultBarItemTemplates.PaperKindBarItemContentTemplate;
			InternalItems.Add(item);
		}
		protected internal abstract string ObtainPaperKindCaption(PaperKind paperKind, string displayName);
		protected internal abstract ICommand CreateChangeSectionPaperKindCommand(PaperKind paperKind);
	}
	#endregion
	#region PaperKindBarListItemLink
	public class PaperKindBarListItemLink : BarListItemLink {
	}
	#endregion
}
