#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Win.EasyTest {
	public interface IControlFinder {
		object Find(Form activeForm, string contolType, string caption);
	}
	public class ControlFinder : IControlFinder {
		public const string ControlTypeAction = "Action";
		public const string ControlTypeField = "Field";
		public const string ControlTypeMessage = "Message";
		public const string ControlTypeTable = "Table";
		private static object locker = new object();
		private static ControlFinder instance;
		private List<IControlFinder> controlsFinders = new List<IControlFinder>();
		public object Find(Form activeForm, string contolType, string caption) {
			lock(locker) {
				foreach(IControlFinder controlsFinder in controlsFinders) {
					object control = controlsFinder.Find(activeForm, contolType, caption);
					if(control != null) {
						return control;
					}
				}
				return null;
			}
		}
		public void RegisterController(IControlFinder finder) {
			lock(locker) {
				controlsFinders.Add(finder);
			}
		}
		public void UnregisterController(IControlFinder finder) {
			lock(locker) {
				controlsFinders.Remove(finder);
			}
		}
		public static ControlFinder Instance {
			get {
				if(instance == null) {
					instance = new ControlFinder();
				}
				return instance;
			}
		}
	}
}
