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
using System.Collections.Generic;
using System;
using System.Windows.Markup;
using System.Linq;
using DevExpress.Xpf.Utils.Themes;
using System.Diagnostics;
namespace DevExpress.Xpf.Core {
	public class ThemePartResourceDictionary : ResourceDictionary {
		bool isEnabled;
		static List<ThemePartResourceDictionary> resourceDictionaries = new List<ThemePartResourceDictionary>();
		Uri mutableSource;
		object key;
		public ThemePartResourceDictionary() {
			AllowExnernalSetIsEnabled = true;
		}
		internal static List<ThemePartResourceDictionary> ResourceDictionaries { get { return resourceDictionaries; } }
		protected bool AllowExnernalSetIsEnabled { get; set; }
		protected bool IsEnabled {
			get { return isEnabled; }
			set {
				if(isEnabled == value)
					return;
				isEnabled = value;
				OnIsEnabledChanged();
			}
		}
		public Uri DisabledSource {
			get { return mutableSource; }
			set {
				if(mutableSource == value)
					return;
				mutableSource = value;
				OnDisabledSourceChanged();
			}
		}
		public object Key {
			get { return key; }
			set {
				if(key == value)
					return;
				if(key != null)
					throw new ArgumentException("Key");
				key = value;
				OnKeyChanged();
			}
		}
		internal static void EnableSource(ThemePartResourceDictionary dictionary) {
			dictionary.IsEnabled = true;
		}
		internal static bool EnableSource(object key) {
			IEnumerable<ThemePartResourceDictionary> enumerable = GetResourceDictionaries(key);
			if(!enumerable.Any())
				return false;
			foreach(ThemePartResourceDictionary resourceDictionary in enumerable) {
				if(resourceDictionary.AllowExnernalSetIsEnabled) resourceDictionary.IsEnabled = true;
			}
			return true;
		}
		static List<ThemePartResourceDictionary> GetResourceDictionaries(object key) {
			return ResourceDictionaries.Where(resourceDictionary => key.Equals(resourceDictionary.Key)).ToList();
		}
		protected virtual void OnDisabledSourceChanged() { }
		void OnIsEnabledChanged() {
			if(!IsEnabled)
				return;
			this.MergedDictionaries.Add(new ResourceDictionary() { Source = DisabledSource });
			ResourceDictionaries.Remove(this);
		}
		void OnKeyChanged() {
			ThemePartKeyExtension themePartKey = Key as ThemePartKeyExtension;
			if(DisabledSource == null && themePartKey != null) {
				DisabledSource = themePartKey.Uri;
			}
			ResourceDictionaries.Add(this);
		}
	}
	public class ThemePartKeyExtension : ThemePartLoaderExtension {
		internal Uri Uri { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			Uri = base.ProvideValue(serviceProvider) as Uri;
			return this;
		}
		public override int GetHashCode() {
			if(AssemblyName == null)
				return 0;
			return AssemblyName.GetHashCode();
		}
		public override bool Equals(object obj) {
			ThemePartKeyExtension key = obj as ThemePartKeyExtension;
			if(key == null)
				return false;
			return string.Equals(AssemblyName, key.AssemblyName, StringComparison.InvariantCultureIgnoreCase);
		}
	}
	public class GenericThemePartResourceDictionary : ThemePartResourceDictionary {
		static List<GenericThemePartResourceDictionary> disabledResourceDictionaries = new List<GenericThemePartResourceDictionary>();
		public GenericThemePartResourceDictionary() {
			AllowExnernalSetIsEnabled = false;
			UpdateIsEnabled();
		}
		private void UpdateIsEnabled() {
			if((ThemeManager.EnableDefaultThemeLoading || string.IsNullOrEmpty(ThemeManager.ApplicationThemeName)
				|| ThemeManager.ApplicationThemeName == Theme.DefaultThemeName) && DisabledSource != null) IsEnabled = true;
			else {
				IsEnabled = false;
				AddDisabledResourceDictionaries();
			}
		}
		private void AddDisabledResourceDictionaries() {
			if(!disabledResourceDictionaries.Contains(this))
				disabledResourceDictionaries.Add(this);
		}
		protected override void OnDisabledSourceChanged() {
			base.OnDisabledSourceChanged();
			UpdateIsEnabled();
		}
		public static void EnableResourceDictionaries() {
			if(disabledResourceDictionaries.Count == 0)
				return;
			foreach(GenericThemePartResourceDictionary dictionary in disabledResourceDictionaries) {
				dictionary.IsEnabled = true;
			}
		}
	}
}
namespace DevExpress.Xpf.Utils.Themes {
	public class ResourceDictionaryEx : ResourceDictionary {
		public bool DisableCache { get; set; }
		private static Dictionary<Uri, WeakReference> sharedCache;
		static ResourceDictionaryEx() {
			sharedCache = new Dictionary<Uri, WeakReference>();
		}
		private Uri sourceCore;
		public new Uri Source {
			get { return sourceCore; }
			set {
				sourceCore = value;
				if(!sharedCache.ContainsKey(sourceCore) || DisableCache) {
					base.Source = sourceCore;
					if(!DisableCache)
						CacheSource();
				}
				else {
					ResourceDictionary temp = (ResourceDictionary)sharedCache[sourceCore].Target;
					if(temp != null) {
						MergedDictionaries.Add(temp);
				sourceCore = temp.Source;
					}
					else {
						base.Source = sourceCore;
						CacheSource();
					}
				}
			}
		}
		private void CacheSource() {
			if(sharedCache.ContainsKey(sourceCore)) sharedCache.Remove(sourceCore);
			sharedCache.Add(sourceCore, new WeakReference(this, false));
		}
	}
}
