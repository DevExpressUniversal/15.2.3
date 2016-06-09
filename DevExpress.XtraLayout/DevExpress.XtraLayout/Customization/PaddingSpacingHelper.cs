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
using DevExpress.XtraLayout;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Utils;
using System.Collections;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Text;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Customization {
	public enum RecomendedPaddingTypes { 
		DialogBoxMargins, 
		BetweenTextLabelsAndTheirAssociatedControls, 
		BetweenRelatedControls, 
		BetweenUnrelatedControls, 
		FirstControlInAGroupBox, 
		BetweenControlsInAGroupBox,
		BetweenHorizontallyOrVerticallyArrangedButtons,
		LastControlInAGroupBox,
		FromTheLeftEdgeOfAGroupBox,
		TextLabelBesideAControl,
		SingleItemInTab,
		GroupBox
	}
	public class LayoutStyleManager {
		protected ILayoutControl control;
		protected Dictionary<RecomendedPaddingTypes, Padding> paddingConstants;
		public LayoutStyleManager(ILayoutControl control) {
			this.control = control;
			FillPaddingConstants();
		}
		protected virtual void FillPaddingConstants() {
		}
		public ILayoutControl Owner { get { return control; } }
		protected virtual Padding CalcMSGuidelinesPaddingSpacing(BaseLayoutItem item, bool calcSpacing) {
			if(item == null) return Padding.Empty;
			LayoutClassificationArgs itemClassification = LayoutClassifier.Default.Classify(item);
			LayoutClassificationArgs itemParentClassification = LayoutClassifier.Default.Classify(item.Parent);
			if(itemParentClassification.IsGroup && itemParentClassification.Group.Items.Count == 1 && itemParentClassification.Group.ParentTabbedGroup != null) return paddingConstants[RecomendedPaddingTypes.SingleItemInTab];
			if(itemClassification.IsGroup && item.Parent == null && !calcSpacing) return paddingConstants[RecomendedPaddingTypes.DialogBoxMargins];
			if(itemClassification.IsLayoutControlItem && !calcSpacing) return paddingConstants[RecomendedPaddingTypes.BetweenRelatedControls];
			if(itemClassification.IsGroup && itemClassification.Group.ParentTabbedGroup == null && itemClassification.Group.Items.Count > 1 && !calcSpacing) return paddingConstants[RecomendedPaddingTypes.GroupBox];
			return Padding.Empty;
		}
		public virtual Padding GetItemPadding(BaseLayoutItem item) {
			if(Owner.OptionsView.PaddingSpacingMode == PaddingMode.MSGuidelines)
				return CalcMSGuidelinesPaddingSpacing(item, false);
			return ((LayoutStyleManagerClient)item).GetDefaultPaddings();
		}
		public virtual TextAlignModeGroup CorrectGroupTextAlignMode(LayoutGroup item, TextAlignModeGroup proposedValue) {
			return proposedValue;
		}
		public virtual Padding GetItemSpacing(BaseLayoutItem item) {
			if(Owner.OptionsView.PaddingSpacingMode == PaddingMode.MSGuidelines)
				return CalcMSGuidelinesPaddingSpacing(item, true);
			return ((LayoutStyleManagerClient)item).GetDefaultSpaces();
		}
	}
	public interface LayoutStyleManagerClient {
		void Update();
		Padding GetDefaultPaddings();
		Padding GetDefaultSpaces();
	}
}
