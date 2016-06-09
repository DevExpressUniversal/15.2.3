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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
namespace System.ComponentModel {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public sealed class DXDisplayNameIgnoreAttribute : Attribute {
		public bool IgnoreRecursionOnly { get; set; }
	}
	public sealed class DXDisplayNameAttribute : ResDisplayNameAttribute {
		public const string DefaultResourceFile = "PropertyNamesRes";
		static bool useResourceManager;
		public static bool UseResourceManager {
			get { return useResourceManager; }
			set {
				if(useResourceManager != value) {
					useResourceManager = value;
#if !DXPORTABLE
					DevExpress.Utils.Design.EnumTypeConverter.Refresh();
#endif
				}
			}
		}
		Type resourceFinder;
		string resourceName;
		string resourceFile;
		string defaultDisplayName;
		string displayName;
		string cultureName;
		public string ResourceFile { get { return resourceFile; } }
		public string ResourceName { get { return resourceName; } }
		public Type ResourceFinder { get { return resourceFinder; } }
		public DXDisplayNameAttribute(Type resourceFinder, string resourceName)
			: this(resourceFinder, DefaultResourceFile, resourceName) {
		}
		public DXDisplayNameAttribute(Type resourceFinder, string resourceFile, string resourceName)
			: this(resourceFinder, resourceFile, resourceName, GetDisplayName(resourceName)) {
		}
		public DXDisplayNameAttribute(Type resourceFinder, string resourceFile, string resourceName, string defaultDisplayName) {
			this.resourceFile = resourceFile;
			this.resourceFinder = resourceFinder;
			this.resourceName = resourceName;
			this.defaultDisplayName = defaultDisplayName;
		}
		public override string DisplayName {
			get {
				if(UseResourceManager) {
					EnsureLocalizedDisplayName();
					return displayName;
				}
				return defaultDisplayName;
			}
		}
		public string GetLocalizedDisplayName() {
			EnsureLocalizedDisplayName();
			return displayName;
		}
		void EnsureLocalizedDisplayName() {
			string currentCultureName =
#if !DXPORTABLE
			Thread.CurrentThread.CurrentUICulture.Name;
#else
			CultureInfo.CurrentUICulture.Name;
#endif
			lock (this) {
				if(displayName == null || cultureName != currentCultureName) {
					cultureName = currentCultureName;
					displayName = GetResourceString(resourceFinder, resourceFile, resourceName, defaultDisplayName);
				}
			}
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
	public class ResDisplayNameAttribute : DisplayNameAttribute {
		protected static string GetDisplayName(string resourceName) {
			int lastIndexOfPoint = resourceName.LastIndexOf(".");
			return lastIndexOfPoint > 0 ? resourceName.Substring(lastIndexOfPoint + 1) : resourceName;
		}
		static System.Resources.ResourceManager CreateResourceManager(Type resourceFinder, string resourceFileName)
		{
			Assembly assembly =
#if DXRESTRICTED
				resourceFinder.GetTypeInfo().Assembly;
#else
				resourceFinder.Assembly;
#endif
			return new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), assembly);
		}
#if DEBUGTEST
		public
#else
		internal
#endif
		static string GetResourceString(Type resourceFinder, string resourceFileName, string resourceName, string defaultString) {
			try {
				string s = CreateResourceManager(resourceFinder, resourceFileName).GetString(resourceName);
				System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(s));
				return s;
			} catch {
				return defaultString;
			}
		}
		public ResDisplayNameAttribute() {
		}
		public ResDisplayNameAttribute(Type resourceFinder, string resourceFile, string resourceName)
			: this(resourceFinder, resourceFile, resourceName, DXDisplayNameAttribute.GetDisplayName(resourceName)) {
		}
		public ResDisplayNameAttribute(Type resourceFinder, string resourceFile, string resourceName, string defaultDisplayName) {
			base.DisplayNameValue = GetResourceString(resourceFinder, resourceFile, resourceName, defaultDisplayName);
		}
	}
}
#if DXRESTRICTED
namespace DevExpress.Compatibility.System.ComponentModel {
	[AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
	public class DisplayNameAttribute : Attribute {
		private string displayName;
		public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();
		public DisplayNameAttribute() : this(string.Empty) { }
		public DisplayNameAttribute(string displayName) {
			this.displayName = displayName;
		}
		public override bool Equals(object obj) {
			if(obj == this) {
				return true;
			}
			DisplayNameAttribute attribute = obj as DisplayNameAttribute;
			return ((attribute != null) && (attribute.DisplayName == this.DisplayName));
		}
		public override int GetHashCode() {
			return this.DisplayName.GetHashCode();
		}
		public virtual bool IsDefaultAttribute() {
			return this.Equals(Default);
		}
		public virtual string DisplayName {
			get {
				return this.DisplayNameValue;
			}
		}
		protected string DisplayNameValue {
			get { return this.displayName; }
			set { this.displayName = value; }
		}
	}
}
#endif
