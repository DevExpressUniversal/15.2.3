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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Customization {
	class UCIconsHelper {
		[ThreadStatic]
		static Image addIcon;
		[ThreadStatic]
		static Image bestFitIcon;
		[ThreadStatic]
		static Image hideIcon;
		[ThreadStatic]
		static Image openFileIcon;
		[ThreadStatic]
		static Image parentIcon;
		[ThreadStatic]
		static Image redoIcon;
		[ThreadStatic]
		static Image resetDefaultIcon;
		[ThreadStatic]	  
		static Image saveIcon;		
		[ThreadStatic]
		static Image showCustomizationFormIcon;		
		[ThreadStatic]
		static Image undoIcon;
		[ThreadStatic]
		static Image hideTextIcon;
		[ThreadStatic]
		static Image controlBotCIcon;
		[ThreadStatic]
		static Image controlBotLIcon;
		[ThreadStatic]
		static Image controlBotRIcon;
		[ThreadStatic]
		static Image controlMidCIcon;
		[ThreadStatic]
		static Image controlMidLIcon;
		[ThreadStatic]
		static Image controlMidRIcon;
		[ThreadStatic]
		static Image controlTopCIcon;
		[ThreadStatic]
		static Image controlTopLIcon;
		[ThreadStatic]
		static Image controlTopRIcon;
		[ThreadStatic]
		static Image groupIcon;
		[ThreadStatic]
		static Image ungroupIcon;
		[ThreadStatic]
		static Image textBotCIcon;
		[ThreadStatic]
		static Image textBotLIcon;
		[ThreadStatic]
		static Image textBotRIcon;
		[ThreadStatic]
		static Image textBottomIcon;
		[ThreadStatic]
		static Image textLeftIcon;
		[ThreadStatic]
		static Image textMidCIcon;
		[ThreadStatic]
		static Image textMidLIcon;
		[ThreadStatic]
		static Image textMidRIcon;
		[ThreadStatic]
		static Image textRightIcon;
		[ThreadStatic]
		static Image textTopIcon;
		[ThreadStatic]
		static Image textTopCIcon;
		[ThreadStatic]
		static Image textTopLIcon;
		[ThreadStatic]
		static Image textTopRIcon;
		[ThreadStatic]
		static Image hLockIcon;
		[ThreadStatic]
		static Image wLockIcon;
		[ThreadStatic]
		static Image unlockIcon;
		[ThreadStatic]
		static Image lockIcon;
		[ThreadStatic]
		static Image addTabIcon;
		[ThreadStatic]
		static Image delTabIcon;
		public static Image AddIcon {
			get {
				if(addIcon == null) {
					addIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Add_16x16.png", Assembly.GetExecutingAssembly());
				}
				return addIcon;
			}
		}
		public static Image BestFitIcon {
			get {
				if(bestFitIcon == null) {
					bestFitIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.BestFit_16x16.png", Assembly.GetExecutingAssembly());
				}
				return bestFitIcon;
			}
		}
		public static Image HideIcon {
			get {
				if(hideIcon == null) {
					hideIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Hide_16x16.png", Assembly.GetExecutingAssembly());
				}
				return hideIcon;
			}
		}	  
		public static Image OpenFileIcon {
			get {
				if(openFileIcon == null) {
					openFileIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.OpenFile_16x16.png", Assembly.GetExecutingAssembly());
				}
				return openFileIcon;
			}
		}
		public static Image ParentIcon {
			get {
				if(parentIcon == null) {
					parentIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Parent_16x16.png", Assembly.GetExecutingAssembly());
				}
				return parentIcon;
			}
		}
		public static Image RedoIcon {
			get {
				if(redoIcon == null) {
					redoIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Redo_16x16.png", Assembly.GetExecutingAssembly());
				}
				return redoIcon;
			}
		}
		public static Image ResetDefaultIcon {
			get {
				if(resetDefaultIcon == null) {
					resetDefaultIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ResetDefault_16x16.png", Assembly.GetExecutingAssembly());
				}
				return resetDefaultIcon;
			}
		}
		public static Image SaveIcon {
			get {
				if(saveIcon == null) {
					saveIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Save_16x16.png", Assembly.GetExecutingAssembly());
				}
				return saveIcon;
			}
		}
		public static Image ShowCustomizationFormIcon {
			get {
				if(showCustomizationFormIcon == null) {
					showCustomizationFormIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ShowCustomForm_16x16.png", Assembly.GetExecutingAssembly());
				}
				return showCustomizationFormIcon;
			}
		}
		public static Image UndoIcon {
			get {
				if(undoIcon == null) {
					undoIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Undo_16x16.png", Assembly.GetExecutingAssembly());
				}
				return undoIcon;
			}
		}
		public static Image HideTextIcon {
			get {
				if(hideTextIcon == null) {
					hideTextIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Label-hide_16x16.png", Assembly.GetExecutingAssembly());
				}
				return hideTextIcon;
			}
		}
		public static Image ControlBotCIcon {
			get {
				if(controlBotCIcon == null) {
					controlBotCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlBotC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlBotCIcon;
			}
		}
		public static Image ControlBotLIcon {
			get {
				if(controlBotLIcon == null) {
					controlBotLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlBotL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlBotLIcon;
			}
		}
		public static Image ControlBotRIcon {
			get {
				if(controlBotRIcon == null) {
					controlBotRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlBotR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlBotRIcon;
			}
		}
		public static Image ControlMidCIcon {
			get {
				if(controlMidCIcon == null) {
					controlMidCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlMidC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlMidCIcon;
			}
		}
		public static Image ControlMidLIcon {
			get {
				if(controlMidLIcon == null) {
					controlMidLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlMidL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlMidLIcon;
			}
		}
		public static Image ControlMidRIcon {
			get {
				if(controlMidRIcon == null) {
					controlMidRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlMidR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlMidRIcon;
			}
		}
		public static Image ControlTopCIcon {
			get {
				if(controlTopCIcon == null) {
					controlTopCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlTopC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlTopCIcon;
			}
		}
		public static Image ControlTopLIcon {
			get {
				if(controlTopLIcon == null) {
					controlTopLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlTopL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlTopLIcon;
			}
		}
		public static Image ControlTopRIcon {
			get {
				if(controlTopRIcon == null) {
					controlTopRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.ControlOptions.ControlTopR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return controlTopRIcon;
			}
		}
		public static Image GroupIcon {
			get {
				if(groupIcon == null) {
					groupIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Group.Group_16x16.png", Assembly.GetExecutingAssembly());
				}
				return groupIcon;
			}
		}
		public static Image UngroupIcon {
			get {
				if(ungroupIcon == null) {
					ungroupIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Group.Ungroup_16x16.png", Assembly.GetExecutingAssembly());
				}
				return ungroupIcon;
			}
		}
		public static Image TextBotCIcon {
			get {
				if(textBotCIcon == null) {
					textBotCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextBotC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textBotCIcon;
			}
		}
		public static Image TextBotLIcon {
			get {
				if(textBotLIcon == null) {
					textBotLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextBotL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textBotLIcon;
			}
		}
		public static Image TextBotRIcon {
			get {
				if(textBotRIcon == null) {
					textBotRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextBotR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textBotRIcon;
			}
		}
		public static Image TextBottomIcon {
			get {
				if(textBottomIcon == null) {
					textBottomIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextBottom_16x16.png", Assembly.GetExecutingAssembly());
				}
				return textBottomIcon;
			}
		}
		public static Image TextLeftIcon {
			get {
				if(textLeftIcon == null) {
					textLeftIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextLeft_16x16.png", Assembly.GetExecutingAssembly());
				}
				return textLeftIcon;
			}
		}
		public static Image TextMidCIcon {
			get {
				if(textMidCIcon == null) {
					textMidCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextMidC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textMidCIcon;
			}
		}
		public static Image TextMidLIcon {
			get {
				if(textMidLIcon == null) {
					textMidLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextMidL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textMidLIcon;
			}
		}
		public static Image TextMidRIcon {
			get {
				if(textMidRIcon == null) {
					textMidRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextMidR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textMidRIcon;
			}
		}
		public static Image TextRightIcon {
			get {
				if(textRightIcon == null) {
					textRightIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextRight_16x16.png", Assembly.GetExecutingAssembly());
				}
				return textRightIcon;
			}
		}
		public static Image TextTopIcon {
			get {
				if(textTopIcon == null) {
					textTopIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextTop_16x16.png", Assembly.GetExecutingAssembly());
				}
				return textTopIcon;
			}
		}
		public static Image TextTopCIcon {
			get {
				if(textTopCIcon == null) {
					textTopCIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextTopC-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textTopCIcon;
			}
		}
		public static Image TextTopLIcon {
			get {
				if(textTopLIcon == null) {
					textTopLIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextTopL-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textTopLIcon;
			}
		}
		public static Image TextTopRIcon {
			get {
				if(textTopRIcon == null) {
					textTopRIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LabelOptions.TextTopR-16x16.png", Assembly.GetExecutingAssembly());
				}
				return textTopRIcon;
			}
		}
		public static Image HLockIcon {
			get {
				if(hLockIcon == null) {
					hLockIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LockUnLock.HLock_16x16.png", Assembly.GetExecutingAssembly());
				}
				return hLockIcon;
			}
		}
		public static Image LockIcon {
			get {
				if(lockIcon == null) {
					lockIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LockUnLock.Lock_16x16.png", Assembly.GetExecutingAssembly());
				}
				return lockIcon;
			}
		}
		public static Image UnlockIcon {
			get {
				if(unlockIcon == null) {
					unlockIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LockUnLock.Unlock_16x16.png", Assembly.GetExecutingAssembly());
				}
				return unlockIcon;
			}
		}
		public static Image WLockIcon {
			get {
				if(wLockIcon == null) {
					wLockIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.LockUnLock.WLock_16x16.png", Assembly.GetExecutingAssembly());
				}
				return wLockIcon;
			}
		}
		public static Image AddTabIcon {
			get {
				if(addTabIcon == null) {
					addTabIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Tab.AddTab_16x16.png", Assembly.GetExecutingAssembly());
				}
				return addTabIcon;
			}
		}
		public static Image DelTabIcon {
			get {
				if(delTabIcon == null) {
					delTabIcon = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Customization.UserCustomization.Images.Tab.DelTab_16x16.png", Assembly.GetExecutingAssembly());
				}
				return delTabIcon;
			}
		}
	}
}
