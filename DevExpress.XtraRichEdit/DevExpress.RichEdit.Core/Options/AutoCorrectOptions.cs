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
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region AutoCorrectOptions
	[ComVisible(true)]
	public class AutoCorrectOptions : RichEditNotificationOptions {
		#region Fields
		bool replaceTextAsYouType;
		bool detectUrls;
		bool correctTwoInitialCapitals;
		bool useSpellCheckerSuggestions;
		#endregion
		#region Properties
		#region ReplaceTextAsYouType
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AutoCorrectOptionsReplaceTextAsYouType"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ReplaceTextAsYouType {
			get { return replaceTextAsYouType; }
			set {
				if (replaceTextAsYouType == value)
					return;
				replaceTextAsYouType = value;
				OnChanged("ReplaceTextAsYouType", !value, value);
			}
		}
		#endregion
		#region DetectUrls
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AutoCorrectOptionsDetectUrls"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool DetectUrls {
			get { return detectUrls; }
			set {
				if (detectUrls == value)
					return;
				detectUrls = value;
				OnChanged("DetectUrls", !value, value);
			}
		}
		#endregion
		#region CorrectTwoInitialCapitals
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AutoCorrectOptionsCorrectTwoInitialCapitals"),
#endif
 DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool CorrectTwoInitialCapitals {
			get { return correctTwoInitialCapitals; }
			set {
				if (correctTwoInitialCapitals == value)
					return;
				correctTwoInitialCapitals = value;
				OnChanged("CorrectTwoInitialCapitals", !value, value);
			}
		}
		#endregion
		#region UseSpellCheckerSuggestions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AutoCorrectOptionsUseSpellCheckerSuggestions"),
#endif
 DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool UseSpellCheckerSuggestions {
			get { return useSpellCheckerSuggestions; }
			set {
				if (useSpellCheckerSuggestions == value)
					return;
				useSpellCheckerSuggestions = value;
				OnChanged("UseSpellCheckerSuggestions", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			ReplaceTextAsYouType = true;
			DetectUrls = true;
			CorrectTwoInitialCapitals = false;
			UseSpellCheckerSuggestions = false;
		}
	}
	#endregion
}
