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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.XtraSplashScreen;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Utils {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class DXSplashScreenForm : DevExpress.XtraWaitForm.ManualLayoutDemoWaitForm {
		public DXSplashScreenForm() {
			Name = GetType().Name;
			StartPosition = FormStartPosition.CenterScreen;
			Text = Application.ProductName;
			Width = 350;
		}
	}
	public class DXSplashScreen : ISplash, ISupportUpdateSplash {
		public static int DefaultUpdateDelay = 0;
		public static Type DefaultSplashScreenType = typeof(DevExpress.ExpressApp.Win.Utils.DXSplashScreenForm);
		private bool isWaitFormCore = false;
		private Type splashFormType;
		public DXSplashScreen() : this(DefaultSplashScreenType) { }
		public DXSplashScreen(Type xtraSplashFormType) {
			Guard.ArgumentNotNull(xtraSplashFormType, "xtraSplashFormType");
			this.SplashFormType = xtraSplashFormType;
			this.UpdateDelay = DefaultUpdateDelay;
		}
		public DXSplashScreen(Image splashImage) {
			Guard.ArgumentNotNull(splashImage, "splashImage");
			this.SplashImage = splashImage;
		}
		public DXSplashScreen(string embeddedResourceImageName) {
			Guard.ArgumentNotNullOrEmpty(embeddedResourceImageName, "embeddedResourceImageName");
			Assembly executingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
			InitializeDXSplashScreen(embeddedResourceImageName, executingAssembly);
		}
		private void InitializeDXSplashScreen(string embeddedResourceImageName, Assembly executingAssembly) {
			try {
				this.SplashImage = ImageLoader.Instance.LoadFromResource(executingAssembly, embeddedResourceImageName);
			}
			catch(Exception ex) {
				throw new ArgumentException(string.Format("Cannot load an image from the executable project assembly ({0}) by the specified name. Make sure that the image file's BuildAction is set to EmbeddedResource and its name includes the file extension.",
					executingAssembly.GetName().Name), ex
				);
			}
		}
		public Type SplashFormType {
			get { return splashFormType; }
			private set {
				if(!typeof(DevExpress.XtraSplashForm.SplashFormBase).IsAssignableFrom(value)) {
					ReflectionHelper.ThrowInvalidCastException(typeof(DevExpress.XtraSplashForm.SplashFormBase), value);
				}
				splashFormType = value;
				isWaitFormCore = typeof(DevExpress.XtraWaitForm.WaitForm).IsAssignableFrom(value);
			}
		}
		public Image SplashImage { get; private set; }
		public int UpdateDelay { get; set; }
		protected bool IsWaitForm { get { return isWaitFormCore; } }
		protected DateTime LastUpdatedOn { get; set; }
		public virtual void Start() {
			if(!IsStarted) {
				if(SplashFormType != null) {
					SplashScreenManager.ShowForm(null, SplashFormType, true, false, false);
				}
				else if(SplashImage != null) {
					SplashScreenManager.ShowImage(SplashImage, true, false);
				}
			}
		}
		public virtual void Stop() {
			if(IsStarted) {
				if(SplashFormType != null) {
					SplashScreenManager.CloseForm(false, 0, null, true);
				}
				else if(SplashImage != null) {
					SplashScreenManager.HideImage();
				}
			}
		}
		public bool IsStarted {
			get { return (SplashScreenManager.Default != null) && (SplashScreenManager.Default.IsSplashFormVisible); }
		}
		public void SetDisplayText(string displayText) {
			UpdateSplash(string.Empty, displayText);
		}
		protected bool ShouldUpdate {
			get {
				return IsStarted && (SplashFormType != null) && (SplashImage == null) && ((DateTime.Now - LastUpdatedOn).TotalMilliseconds >= UpdateDelay);
			}
		}
		public virtual void UpdateSplash(string caption, string description, params object[] additionalParams) {
			if(!ShouldUpdate) {
				return;
			}
			if(!string.IsNullOrEmpty(caption)) {
				if(IsWaitForm) {
					SplashScreenManager.Default.SetWaitFormCaption(caption);
				}
				else {
					SplashScreenManager.Default.SendCommand(UpdateSplashCommand.Caption, caption);
				}
			}
			if(!string.IsNullOrEmpty(description)) {
				if(IsWaitForm) {
					SplashScreenManager.Default.SetWaitFormDescription(description);
				}
				else {
					SplashScreenManager.Default.SendCommand(UpdateSplashCommand.Description, description);
				}
			}
			if(additionalParams != null) {
				SplashScreenManager.Default.SendCommand(UpdateSplashCommand.AdditionalParams, additionalParams);
			}
			LastUpdatedOn = DateTime.Now;
		}
	}
	public enum UpdateSplashCommand {
		Caption,
		Description,
		AdditionalParams
	}
}
