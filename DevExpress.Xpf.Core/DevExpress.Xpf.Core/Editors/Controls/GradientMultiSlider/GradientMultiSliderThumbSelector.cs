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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public class GradientMultiSliderThumbSelector : Control {
		#region static
		public static readonly DependencyProperty SelectedThumbProperty;
		static readonly DependencyPropertyKey SelectedThumbColorPropertyKey;
		public static readonly DependencyProperty SelectedThumbColorProperty;
		static GradientMultiSliderThumbSelector() {
			Type ownerType = typeof(GradientMultiSliderThumbSelector);
			SelectedThumbProperty = DependencyPropertyManager.Register("SelectedThumb", typeof(GradientMultiSliderThumb), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (obj, args) => ((GradientMultiSliderThumbSelector)obj).OnSelectedThumbChanged((GradientMultiSliderThumb)args.NewValue)));
			SelectedThumbColorPropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectedThumbColor", typeof(Color), ownerType, new FrameworkPropertyMetadata(Colors.Black));
			SelectedThumbColorProperty = SelectedThumbColorPropertyKey.DependencyProperty;
		}
		void OnSelectedThumbChanged(GradientMultiSliderThumb newValue) {
			SelectedThumbColor = newValue.Color;
		}
		#endregion
		public GradientMultiSliderThumb SelectedThumb {
			get { return (GradientMultiSliderThumb)GetValue(SelectedThumbProperty); }
			set { SetValue(SelectedThumbProperty, value); }
		}
		public Color SelectedThumbColor {
			get { return (Color)GetValue(SelectedThumbColorProperty); }
			private set { SetValue(SelectedThumbColorPropertyKey, value); }
		}
		public ICommand NextThumbCommand { get; private set; }
		public ICommand PreviousThumbCommand { get; private set; }
		public GradientMultiSliderThumbSelector() {
			this.SetDefaultStyleKey(typeof(GradientMultiSliderThumbSelector));
			NextThumbCommand = DelegateCommandFactory.Create(NextThumb);
			PreviousThumbCommand = DelegateCommandFactory.Create(PreviousThumb);
		}
		void NextThumb() {
			SelectedThumb = GetNextThumb();
		}
		void PreviousThumb() {
			SelectedThumb = GetPreviousThumb();
		}
		GradientMultiSliderThumb GetNextThumb() {
			if (SelectedThumb == null)
				return null;
			var thumbs = SelectedThumb.OwnerSlider.Thumbs;
			var orderedGroups = thumbs.GroupBy(x => x.Offset).OrderBy(x => x.Key).ToList();
			for (int i = 0; i < orderedGroups.Count; ++i) {
				if (!orderedGroups[i].Key.AreClose(SelectedThumb.Offset))
					continue;
				GradientMultiSliderThumb nextThumb = GetNextThumb(orderedGroups[i]);
				return nextThumb ?? GetNextThumb(i + 1 < orderedGroups.Count ? orderedGroups[i + 1] : orderedGroups[0]);
			}
			return null;
		}
		GradientMultiSliderThumb GetNextThumb(IGrouping<double, GradientMultiSliderThumb> group) {
			var groupList = group.ToList();
			if (!group.Key.AreClose(SelectedThumb.Offset))
				return groupList.FirstOrDefault();
			if (groupList.Count <= 1)
				return null;
			for (int i = 0; i < groupList.Count; ++i) {
				if (!groupList[i].Color.Equals(SelectedThumb.Color))
					continue;
				return i + 1 < groupList.Count ? groupList[i + 1] : null;
			}
			return null;
		}
		GradientMultiSliderThumb GetPreviousThumb() {
			if (SelectedThumb == null)
				return null;
			var thumbs = SelectedThumb.OwnerSlider.Thumbs;
			var orderedGroups = thumbs.GroupBy(x => x.Offset).OrderBy(x => x.Key).ToList();
			for (int i = orderedGroups.Count - 1; i >= 0; --i) {
				if (!orderedGroups[i].Key.AreClose(SelectedThumb.Offset))
					continue;
				GradientMultiSliderThumb previousThumb = GetPreviousThumb(orderedGroups[i]);
				return previousThumb ?? GetPreviousThumb(i - 1 >= 0 ? orderedGroups[i - 1] : orderedGroups[orderedGroups.Count - 1]);
			}
			return null;
		}
		GradientMultiSliderThumb GetPreviousThumb(IGrouping<double, GradientMultiSliderThumb> group) {
			var groupList = group.ToList();
			if (!group.Key.AreClose(SelectedThumb.Offset)) 
				return groupList.LastOrDefault();
			if (groupList.Count <= 1) 
				return null;
			for (int i = groupList.Count - 1; i >= 0; --i) {
				if (!groupList[i].Color.Equals(SelectedThumb.Color))
					continue;
				return i - 1 >= 0 ? groupList[i - 1] : null;
			}
			return null;
		}
	}
}
