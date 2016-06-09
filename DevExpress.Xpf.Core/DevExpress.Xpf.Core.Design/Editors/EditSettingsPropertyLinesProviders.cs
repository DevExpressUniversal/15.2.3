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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Editors.Helpers;
using Platform::System.Windows.Controls;
#else
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Controls;
#endif
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.SmartTags;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Design {
	public class ShowGridMaskWizardCommand : ShowMaskWizardCommandCore {
		protected override IMaskProperties GetMaskProperties(IModelItem source) {
			TextEditSettings settings = source.GetCurrentValue() as TextEditSettings;
			Type editorType = EditorSettingsProvider.Default.CreateEditor(settings.GetType(), EditorOptimizationMode.Disabled).GetType();
			return ShowMaskWizardCommandCore.ApplyMaskProperties(settings, editorType);
		}
	}
	public static class TypeLists {
		static TypeLists() {
#if !SL
			SetWindowTypes();
#endif
			SetEditSettingsTypes();
			SetBarItemTypes();
		}
#if !SL
		public static ReadOnlyCollection<Type> WindowTypes { get; private set; }
#endif
		public static ReadOnlyCollection<Tuple<Type, PropertyTarget>> EditSettingsTypes { get; private set; }
		public static ReadOnlyCollection<Type> BarItemTypes { get; private set; }
#if !SL
		static void SetWindowTypes() {
			List<Type> windowTypes = new List<Type>() { typeof(Window), typeof(DXTabbedWindow), typeof(DXWindow) };
			TryResolveType(RuntimeTypes.DXRibbonWindow).Do(t => windowTypes.Add(t));
			WindowTypes = windowTypes.AsReadOnly();
		}
#endif
		static void SetEditSettingsTypes() {
			List<Tuple<Type, PropertyTarget>> editSettingsTypes = new List<Tuple<Type, PropertyTarget>>() {
#if !SILVERLIGHT
				new Tuple<Type, PropertyTarget>(typeof(BarCodeEditSettings), PropertyTarget.Editor | PropertyTarget.Grid | PropertyTarget.PropertyGrid),
#endif
				new Tuple<Type, PropertyTarget>(typeof(ButtonEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(CalcEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(CheckEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(ColorEditSettings), PropertyTarget.Editor),
				new Tuple<Type, PropertyTarget>(typeof(ComboBoxEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(DateEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(ImageEditSettings), PropertyTarget.Grid | PropertyTarget.Editor),
				new Tuple<Type, PropertyTarget>(typeof(ListBoxEditSettings), PropertyTarget.All)
			};
			TryResolveType(RuntimeTypes.LookUpEditSettings).Do(t => editSettingsTypes.Add(new Tuple<Type, PropertyTarget>(t, PropertyTarget.All ^ PropertyTarget.Bar)));
			editSettingsTypes.AddRange(new List<Tuple<Type, PropertyTarget>>() {
				new Tuple<Type, PropertyTarget>(typeof(MemoEditSettings), PropertyTarget.Editor | PropertyTarget.Grid),
				new Tuple<Type, PropertyTarget>(typeof(PasswordBoxEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(PopupColorEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(PopupImageEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(FontEditSettings), PropertyTarget.ExceptGrid),
#if !SILVERLIGHT
				new Tuple<Type, PropertyTarget>(typeof(ProgressBarEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(SparklineEditSettings), PropertyTarget.Editor | PropertyTarget.Grid),
#endif
				new Tuple<Type, PropertyTarget>(typeof(SpinEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(TextEditSettings), PropertyTarget.All),
				new Tuple<Type, PropertyTarget>(typeof(TrackBarEditSettings), PropertyTarget.All),
			});
			EditSettingsTypes = editSettingsTypes.AsReadOnly();
		}
		static void SetBarItemTypes() {
			List<Type> barItemTypes = new List<Type>() { 
				typeof(BarButtonItem), 
				typeof(BarCheckItem), 
				typeof(BarSplitButtonItem), 
				typeof(BarSplitCheckItem), 
				typeof(BarSubItem), 
				typeof(BarStaticItem),
				typeof(BarLinkContainerItem),
			};
#if !SILVERLIGHT
			TryResolveType(RuntimeTypes.BarButtonGroup).Do(t => barItemTypes.Add(t));
			TryResolveType(RuntimeTypes.RibbonGalleryBarItem).Do(t => barItemTypes.Add(t));
#endif
			BarItemTypes = barItemTypes.AsReadOnly();
		}
		static Type TryResolveType(DXTypeInfo typeInfo) {
			try {
				return typeInfo.ResolveType();
			} catch {
				return null;
			}
		}
	}
}
