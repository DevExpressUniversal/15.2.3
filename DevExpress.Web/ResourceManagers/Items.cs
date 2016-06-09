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
using DevExpress.Web;
namespace DevExpress.Web {
	public class ResourceItem : CollectionItem {
		[
#if !SL
	DevExpressWebLocalizedDescription("ResourceItemSuite"),
#endif
 DefaultValue(Suite.None), Localizable(false), AutoFormatDisable]
		public Suite Suite { get; set; }
	}
	public class ResourceStyleSheet : ResourceItem {
		[
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string SkinID { get; set; }
		[
		DefaultValue(""), Localizable(false), AutoFormatDisable, 
		TypeConverter("DevExpress.Web.Design.ThemeTypeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string Theme { get; set; }
		public ResourceStyleSheet()
			: base() {
			Suite = Suite.All;
		}
	}
	public class ResourceScript : ResourceItem {
		[ DefaultValue(ControlType.None), Localizable(false), AutoFormatDisable]
		public ControlType Control { get; set; }
	}
	public class ResourceItemCollection : Collection<ResourceItem> {
		public ResourceItemCollection()
			: base() {
		}
		public ResourceItemCollection(ASPxResourceManagerBase manager)
			: base(manager) {
		}
		public ResourceItem Add() {
			return AddInternal(new ResourceItem());
		}
	}
	public class StyleSheetCollection : ResourceItemCollection {
#if !SL
	[DevExpressWebLocalizedDescription("StyleSheetCollectionItem")]
#endif
		public new ResourceStyleSheet this[int index] {
			get { return (GetItem(index) as ResourceStyleSheet); }
		}
		public StyleSheetCollection()
			: base() {
		}
		public StyleSheetCollection(ASPxStyleSheetManager styleSheetManager)
			: base(styleSheetManager) {
		}
		public void Add(ResourceStyleSheet item) {
			base.Add(item);
		}
		public new ResourceStyleSheet Add() {
			ResourceStyleSheet item = new ResourceStyleSheet();
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(ResourceStyleSheet);
		}
	}
	public class ScriptCollection : ResourceItemCollection {
#if !SL
	[DevExpressWebLocalizedDescription("ScriptCollectionItem")]
#endif
		public new ResourceScript this[int index] {
			get { return (GetItem(index) as ResourceScript); }
		}
		public ScriptCollection()
			: base() {
		}
		public ScriptCollection(ASPxScriptManager scriptManager)
			: base(scriptManager) {
		}
		public void Add(ResourceScript item) {
			base.Add(item);
		}
		public new ResourceScript Add() {
			ResourceScript item = new ResourceScript();
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(ResourceScript);
		}
	}
}
