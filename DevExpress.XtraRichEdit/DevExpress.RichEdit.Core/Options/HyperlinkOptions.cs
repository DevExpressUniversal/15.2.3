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
using System.Drawing.Design;
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraRichEdit {
	#region HyperlinkOptions
	[ComVisible(true)]
	public class HyperlinkOptions : RichEditNotificationOptions {
		#region Fields
		bool showToolTip;
		Keys modifierKeys;
		#endregion
		#region Properties
		#region ShowToolTip
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HyperlinkOptionsShowToolTip"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowToolTip {
			get { return showToolTip; }
			set {
				if (value == showToolTip)
					return;
				showToolTip = value;
				OnChanged("ShowToolTip", !value, value);
			}
		}
		#endregion
		#region ModifierKeys
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("HyperlinkOptionsModifierKeys"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true)]
		[TypeConverter("DevExpress.XtraRichEdit.Design.ModifiersConverter, " + AssemblyInfo.SRAssemblyRichEditDesign)]
		[Editor("DevExpress.XtraRichEdit.Design.ModifiersEditor, " + AssemblyInfo.SRAssemblyRichEditDesign, typeof(UITypeEditor))]
		public Keys ModifierKeys {
			get { return modifierKeys; }
			set {
				if (Object.Equals(value, modifierKeys))
					return;
				Keys oldValue = modifierKeys;
				modifierKeys = value;
				OnChanged("ModifierKeys", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeModifierKeys() {
			return ModifierKeys != Keys.Control;
		}
		protected internal virtual void ResetModifierKeys() {
			ModifierKeys = Keys.Control;
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.ShowToolTip = true;
			this.ModifierKeys = Keys.Control;
		}
	}
	#endregion
}
