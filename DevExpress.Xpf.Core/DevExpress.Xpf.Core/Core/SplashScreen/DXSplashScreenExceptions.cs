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

#if !FREE
namespace DevExpress.Xpf.Core {
#else
namespace DevExpress.Mvvm.UI {
#endif
	static class DXSplashScreenExceptions {
		public const string Exception1 = "SplashScreen has been displayed. Only one splash screen can be displayed simultaneously.";
		public const string Exception2 = "Incorrect SplashScreen Type.";
		public const string Exception3 = "Show SplashScreen before calling this method.";
		public const string Exception4 = "This method is not supported if UserControl is used as SplashScreen.";
		public const string Exception5 = "Splash Screen should be inherited from the defined type.";
		public const string Exception6 = "{0} should implement the ISplashScreen interface";
		public const string ServiceException1 = "Cannot use ViewLocator, ViewTemplate and DocumentType if SplashScreenType is set. If you set the SplashScreenType property, do not set the other properties.";
		public const string ServiceException2 = "Show splash screen before calling this method.";
		public const string ServiceException3 = "This method is not supported when SplashScreenType is used.";
	}
}
