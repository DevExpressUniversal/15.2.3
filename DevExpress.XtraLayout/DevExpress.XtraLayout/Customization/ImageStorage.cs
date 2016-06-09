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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.Controls;
using System.Reflection;
using System.Drawing;
namespace DevExpress.XtraLayout.Customization {
	public class LayoutControlImageStorage {
		[ThreadStatic]
		static LayoutControlImageStorage defaultInstance;
		public static LayoutControlImageStorage Default {
			get {
				if(defaultInstance == null) defaultInstance = new LayoutControlImageStorage();
				return defaultInstance;
			}
			set {defaultInstance = value; }
		}
		protected virtual ImageCollection CreateLayoutTreeViewItemImages() {
			return ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraLayout.Images.itemTypes.png",
				Assembly.GetExecutingAssembly(), new Size(16, 16));
		}
		protected virtual ImageCollection CreateDragHeaderPainterImages() {
			return ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraLayout.Images.itemTypes.png",
				Assembly.GetExecutingAssembly(), new Size(16, 16));
		}
		protected virtual ImageCollection CreateRightButtonManagerImages() {
			return ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraLayout.Images.RightButtonMenuIcons.png",
				Assembly.GetExecutingAssembly(), new Size(16, 16));
		}
		protected virtual ImageCollection CreateCustomizationFormImages() {
			return ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraLayout.Images.layout-icons.png",
				Assembly.GetExecutingAssembly(), new Size(16, 16));
		}
		ImageCollection customizationTreeViewImages;
		ImageCollection dragHeaderPainterImages;
		ImageCollection customizationFormButtonImages;
		ImageCollection customizationFormMenuImages;
		Image downArrowCore, upArrowCore, leftArrowCore, rightArrowCore, lockedSignCore, bigSignCore,
			crossCore, crossInvertCore, horzCore, horzInvertCore, vertCore, vertInvertCore, crossLockCore, crossHorizontalCore, crossVerticalCore,
			bestFitCore,convertRegularCore,convertFlowCore,createTemplateCore,removeCore,tableCore;
		protected Image LoadImageFromResourcesConstraint(string imageName) {
			return ResourceImageHelper.CreateBitmapFromResources(
				"DevExpress.XtraLayout.Images.Constraints." + imageName, Assembly.GetExecutingAssembly());
		}
		protected Image LoadImageFromResources(string imageName) {
			return ResourceImageHelper.CreateBitmapFromResources(
				"DevExpress.XtraLayout.Images." + imageName, Assembly.GetExecutingAssembly());
		}
		public Image Table {
			get {
				if(tableCore == null) {
					tableCore = LoadImageFromResources("Table.png");
				}
				return tableCore;
			}
		}
		public Image BestFit {
			get {
				if(bestFitCore == null) {
					bestFitCore = LoadImageFromResources("Best-Fit.png");
				}
				return bestFitCore;
			}
		}
		public Image Remove {
			get {
				if(removeCore == null) {
					removeCore = LoadImageFromResources("Remove.png");
				}
				return removeCore;
			}
		}
		public Image ConvertRegular {
			get {
				if(convertRegularCore == null) {
					convertRegularCore = LoadImageFromResources("Convert-to-Regular-Layout.png");
				}
				return convertRegularCore;
			}
		}
		public Image ConvertFlow {
			get {
				if(convertFlowCore == null) {
					convertFlowCore = LoadImageFromResources("Convet-to-Flow-Layout.png");
				}
				return convertFlowCore;
			}
		}
		public Image CreateTemplate {
			get {
				if(createTemplateCore == null) {
					createTemplateCore = LoadImageFromResources("Create-Template.png");
				}
				return createTemplateCore;
			}
		}
		public Image Cross {
			get {
				if(crossCore == null) {
					crossCore = LoadImageFromResourcesConstraint("icon-cross.png");
				}
				return crossCore;
			}
		}
		public Image CrossInverted {
			get {
				if(crossInvertCore == null) {
					crossInvertCore = LoadImageFromResourcesConstraint("icon-cross-invert.png");
				}
				return crossInvertCore;
			}
		}
		public Image CrossLock {
			get {
				if(crossLockCore == null) {
					crossLockCore = LoadImageFromResourcesConstraint("Lock.png");
				}
				return crossLockCore;
			}
		}
		public Image CrossHorizontal {
			get {
				if(crossHorizontalCore == null) {
					crossHorizontalCore = LoadImageFromResourcesConstraint("Horz.png");
				}
				return crossHorizontalCore;
			}
		}
		public Image CrossVertical {
			get {
				if(crossVerticalCore == null) {
					crossVerticalCore = LoadImageFromResourcesConstraint("Vert.png");
				}
				return crossVerticalCore;
			}
		}
		public Image Horizontal {
			get {
				if(horzCore == null) {
					horzCore = LoadImageFromResourcesConstraint("icon-horizont.png");
				}
				return horzCore;
			}
		}
		public Image HorizontalInverted {
			get {
				if(horzInvertCore == null) {
					horzInvertCore = LoadImageFromResourcesConstraint("icon-horizont-invert.png");
				}
				return horzInvertCore;
			}
		}
		public Image Vertical {
			get {
				if(vertCore == null) {
					vertCore = LoadImageFromResourcesConstraint("icon-vert.png");
				}
				return vertCore;
			}
		}
		public Image VerticalInverted {
			get {
				if(vertInvertCore == null) {
					vertInvertCore = LoadImageFromResourcesConstraint("icon-vert-invert.png");
				}
				return vertInvertCore;
			}
		}
		public Image DownArrow {
			get {
				if (downArrowCore == null) {
					downArrowCore = LoadImageFromResourcesConstraint("down-arrow.png");
				}
				return downArrowCore;
			}
		}
		public Image UpArrow {
			get {
				if (upArrowCore == null) {
					upArrowCore = LoadImageFromResourcesConstraint("up-arrow.png");
				}
				return upArrowCore;
			}
		}
		public Image LeftArrow {
			get {
				if (leftArrowCore == null) {
					leftArrowCore = LoadImageFromResourcesConstraint("left-arrow.png");
				}
				return leftArrowCore;
			}
		}
		public Image RightArrow {
			get {
				if (rightArrowCore == null) {
					rightArrowCore = LoadImageFromResourcesConstraint("right-arrow.png");
				}
				return rightArrowCore;
			}
		}
		public Image LockedSign {
			get {
				if (lockedSignCore == null) {
					lockedSignCore = LoadImageFromResourcesConstraint("locked.png");
				}
				return lockedSignCore;
			}
		}
		public Image BigSign {
			get {
				if (bigSignCore == null) {
					bigSignCore = LoadImageFromResourcesConstraint("sign.png");				}
				return bigSignCore;
			}
		}
		public ImageCollection CustomizationTreeView {
			get {
				if(customizationTreeViewImages == null) {
					customizationTreeViewImages = CreateLayoutTreeViewItemImages();
				}
				return customizationTreeViewImages;
			}
		}
		public ImageCollection DragHeaderPainter {
			get {
				if(dragHeaderPainterImages == null) {
					dragHeaderPainterImages = CreateDragHeaderPainterImages();
				}
				return dragHeaderPainterImages;
			}
		}
		public ImageCollection CustomizationFormButton {
			get {
				if(customizationFormButtonImages == null) {
					customizationFormButtonImages = CreateCustomizationFormImages();
				}
				return customizationFormButtonImages;
			}
		}
		public ImageCollection CustomizationFormMenu {
			get {
				if(customizationFormMenuImages == null) {
					customizationFormMenuImages = CreateRightButtonManagerImages();
				}
				return customizationFormMenuImages;
			}
		}
	}
}
