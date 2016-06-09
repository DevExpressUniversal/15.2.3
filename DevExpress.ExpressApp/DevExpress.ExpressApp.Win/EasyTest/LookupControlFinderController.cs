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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp.EasyTest;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates;
namespace DevExpress.ExpressApp.Win.EasyTest {
	public class LookupControlFinderController : ViewController, IControlFinder, IControlsEnumeration {
		private static FrameControlFinder frameControlFinder = new FrameControlFinder();
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame.Template is LookupControlTemplate) {
				ControlFinder.Instance.RegisterController(this);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(Frame.Template is LookupControlTemplate) {
				ControlFinder.Instance.UnregisterController(this);
			}
		}
		#region IControlFinder Members
		public object Find(Form activeForm, string contolType, string caption) {
			LookupEditPopupForm lookupForm = activeForm as LookupEditPopupForm;
			if(lookupForm != null && ((Control)(Frame.Template)).FindForm() == activeForm) {
				return frameControlFinder.Find(Frame, contolType, caption);
			}
			return null;
		}
		#endregion
		#region IControlsEnumeration Members
		public IDictionary<int, string> CollectAllControls() {
			Dictionary<int, string> allControls = new Dictionary<int, string>();
			Add(allControls, frameControlFinder.GetControlCaptions(Frame, ControlFinder.ControlTypeField));
			Add(allControls, frameControlFinder.GetControlCaptions(Frame, ControlFinder.ControlTypeAction));
			return allControls;
		}
		private void Add(IDictionary<int, string> target, IDictionary<int, string> source) {
			foreach(KeyValuePair<int, string> pair in source) {
				target.Add(pair.Key, pair.Value);
			}
		}
		#endregion
	}
}
