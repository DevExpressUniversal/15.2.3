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
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit {
	#region RichEditControlOptions
	public class RichEditControlOptions : RichEditControlOptionsBase {
		#region Fields
		VerticalScrollbarOptions verticalScrollbar = new VerticalScrollbarOptions();
		HorizontalScrollbarOptions horizontalScrollbar = new HorizontalScrollbarOptions();
		#endregion
		public RichEditControlOptions(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlOptionsVerticalScrollbar"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public VerticalScrollbarOptions VerticalScrollbar { get { return verticalScrollbar; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlOptionsHorizontalScrollbar"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HorizontalScrollbarOptions HorizontalScrollbar { get { return horizontalScrollbar; } }
		#endregion
		protected internal override void SubscribeInnerOptionsEvents() {
			base.SubscribeInnerOptionsEvents();
			VerticalScrollbar.Changed += OnInnerOptionsChanged;
			HorizontalScrollbar.Changed += OnInnerOptionsChanged;
		}
		protected internal override void UnsubscribeInnerOptionsEvents() {
			VerticalScrollbar.Changed -= OnInnerOptionsChanged;
			HorizontalScrollbar.Changed -= OnInnerOptionsChanged;
			base.UnsubscribeInnerOptionsEvents();
		}
		protected internal override void ResetCore() {
			VerticalScrollbar.Reset();
			HorizontalScrollbar.Reset();
			base.ResetCore();
		}
	}
	#endregion
}
