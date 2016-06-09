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

using System.ComponentModel;
using DevExpress.XtraLayout.Utils;
using System;
namespace DevExpress.XtraLayout {
	public enum CustomizationModes { Default, Quick }
	public class LayoutControlDefaultsPropertyBag {
		Padding rootGroupPaddingCore, groupPaddingCore, groupwithoutBordersPaddingCore, tabbedGroupPaddingCore, layoutItemPaddingCore, rootGroupwithoutBordersPaddingCore, rootGroupwithoutBordersSpacingCore;
		Padding rootGroupSpacingCore, groupSpacingCore, groupwithoutBordersSpacingCore, tabbedGroupSpacingCore, layoutItemSpacingCore;
		Padding textToControlDistanceCore;
		int xafSmallVerticalSpacingCore, xafLargeVerticalSpacingCore, xafSmallHorizontalSpacingCore, xafLargeHorizontalSpacingCore;
		public LayoutControlDefaultsPropertyBag() { }
		public LayoutControlDefaultsPropertyBag(bool initWithDefaultValues) {
			if(initWithDefaultValues) InitWithDefaultValues();
		}
		static LayoutControlDefaultsPropertyBag() {
			if(Default != null) return;
		}
		[ThreadStatic]
		static LayoutControlDefaultsPropertyBag defaultLayoutControlDefaultsPropertyBag = null;
		public static LayoutControlDefaultsPropertyBag Default {
			get {
				if(defaultLayoutControlDefaultsPropertyBag == null) defaultLayoutControlDefaultsPropertyBag = new LayoutControlDefaultsPropertyBag(true);
				return defaultLayoutControlDefaultsPropertyBag;
			}
		}
		protected virtual void InitWithDefaultValues() {
			GroupPadding = new Padding(0);
			GroupSpacing = new Padding(2);
			GroupWithoutBordersPadding = new Padding(0);
			GroupWithoutBordersSpacing = new Padding(2);
			LayoutItemPadding = new Padding(5);
			LayoutItemSpacing = new Padding(0);
			RootGroupPadding = new Padding(0);
			RootGroupSpacing = new Padding(0);
			TabbedGroupPadding = new Padding(0);
			TabbedGroupSpacing = new Padding(1);
			TextToControlDistance = new Padding(5);
			RootGroupWithoutBordersPadding = new Padding(0);
			RootGroupWithoutBordersSpacing = new Padding(0);
			XafSmallVerticalSpacing = 2;
			XafLargeVerticalSpacing = 6;
			XafSmallHorizontalSpacing = 2;
			XafLargeHorizontalSpacing = 10;
		}
		public event EventHandler Changed;
		protected virtual void OnChanged() {
		   if(Changed != null) Changed(this, null);
		}
		public Padding RootGroupPadding {
			get { return rootGroupPaddingCore; }
			set {
				if(rootGroupPaddingCore != value) {
					rootGroupPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding RootGroupWithoutBordersPadding {
			get { return rootGroupwithoutBordersPaddingCore; }
			set {
				if(rootGroupwithoutBordersPaddingCore != value) {
					rootGroupwithoutBordersPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding GroupPadding {
			get { return groupPaddingCore; }
			set {
				if(groupPaddingCore != value) {
					groupPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding GroupWithoutBordersPadding {
			get { return groupwithoutBordersPaddingCore; }
			set {
				if(groupwithoutBordersPaddingCore != value) {
					groupwithoutBordersPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding TabbedGroupPadding {
			get { return tabbedGroupPaddingCore; }
			set {
				if(tabbedGroupPaddingCore != value) {
					tabbedGroupPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding LayoutItemPadding {
			get { return layoutItemPaddingCore; }
			set {
				if(layoutItemPaddingCore != value) {
					layoutItemPaddingCore = value;
					OnChanged();
				}
			}
		}
		public Padding RootGroupSpacing {
			get { return rootGroupSpacingCore; }
			set {
				if(rootGroupSpacingCore != value) {
					rootGroupSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding RootGroupWithoutBordersSpacing {
			get { return rootGroupwithoutBordersSpacingCore; }
			set {
				if(rootGroupwithoutBordersSpacingCore != value) {
					rootGroupwithoutBordersSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding GroupSpacing {
			get { return groupSpacingCore; }
			set {
				if(groupSpacingCore != value) {
					groupSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding GroupWithoutBordersSpacing {
			get { return groupwithoutBordersSpacingCore; }
			set {
				if(groupwithoutBordersSpacingCore != value) {
					groupwithoutBordersSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding TabbedGroupSpacing {
			get { return tabbedGroupSpacingCore; }
			set {
				if(tabbedGroupSpacingCore != value) {
					tabbedGroupSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding LayoutItemSpacing {
			get { return layoutItemSpacingCore; }
			set {
				if(layoutItemSpacingCore != value) {
					layoutItemSpacingCore = value;
					OnChanged();
				}
			}
		}
		public Padding TextToControlDistance {
			get { return textToControlDistanceCore; }
			set {
				if(textToControlDistanceCore != value) {
					textToControlDistanceCore = value;
					OnChanged();
				}
			}
		}
		public int XafSmallVerticalSpacing {
			get {
				return xafSmallVerticalSpacingCore;
			}
			set {
				if(xafSmallVerticalSpacingCore != value) {
					xafSmallVerticalSpacingCore = value;
					OnChanged();
				}
			}
		}
		public int XafLargeVerticalSpacing {
			get {
				return xafLargeVerticalSpacingCore;
			}
			set {
				if(xafLargeVerticalSpacingCore != value) {
					xafLargeVerticalSpacingCore = value;
					OnChanged();
				}
			}
		}
		public int XafSmallHorizontalSpacing {
			get {
				return xafSmallHorizontalSpacingCore;
			}
			set {
				if(xafSmallHorizontalSpacingCore != value) {
					xafSmallHorizontalSpacingCore = value;
					OnChanged();
				}
			}
		}
		public int XafLargeHorizontalSpacing {
			get {
				return xafLargeHorizontalSpacingCore;
			}
			set {
				if(xafLargeHorizontalSpacingCore != value) {
					xafLargeHorizontalSpacingCore = value;
					OnChanged();
				}
			}
		}
	}
}
