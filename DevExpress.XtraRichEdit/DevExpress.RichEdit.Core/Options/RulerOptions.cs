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
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region RichEditRulerVisibility
	[ComVisible(true)]
	public enum RichEditRulerVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region RulerOptions (abstract class)
	[ComVisible(true)]
	public abstract class RulerOptions : RichEditNotificationOptions {
		#region Fields
		const RichEditRulerVisibility defaultVisibility = RichEditRulerVisibility.Auto;
		RichEditRulerVisibility visibility;
		#endregion
		#region Properties
		#region Visibility
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RulerOptionsVisibility"),
#endif
 DefaultValue(defaultVisibility), XtraSerializableProperty(), NotifyParentProperty(true)]
		public virtual RichEditRulerVisibility Visibility {
			get { return visibility; }
			set {
				if (visibility == value)
					return;
				RichEditRulerVisibility oldValue = this.visibility;
				visibility = value;
				OnChanged("Visibility", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			Visibility = defaultVisibility;
		}
	}
	#endregion
	#region HorizontalRulerOptions
	[ComVisible(true)]
	public class HorizontalRulerOptions : RulerOptions {
		#region Fields
		bool showTabs;
		bool showLeftIndent;
		bool showRightIndent;
		#endregion
		#region Properties
		#region ShowTabs
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HorizontalRulerOptionsShowTabs"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public virtual bool ShowTabs {
			get { return showTabs; }
			set {
				if (showTabs == value)
					return;
				showTabs = value;
				OnChanged("ShowTabs", !value, value);
			}
		}
		#endregion
		#region ShowLeftIndent
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HorizontalRulerOptionsShowLeftIndent"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public virtual bool ShowLeftIndent {
			get { return showLeftIndent; }
			set {
				if (showLeftIndent == value)
					return;
				showLeftIndent = value;
				OnChanged("ShowLeftIndent", !value, value);
			}
		}
		#endregion
		#region ShowRightIndent
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HorizontalRulerOptionsShowRightIndent"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public virtual bool ShowRightIndent {
			get { return showRightIndent; }
			set {
				if (showRightIndent == value)
					return;
				showRightIndent = value;
				OnChanged("ShowRightIndent", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			ShowTabs = true;
			ShowLeftIndent = true;
			ShowRightIndent = true;
		}
	}
	#endregion
	#region VerticalRulerOptions
	[ComVisible(true)]
	public class VerticalRulerOptions : RulerOptions {
	}
	#endregion
}
