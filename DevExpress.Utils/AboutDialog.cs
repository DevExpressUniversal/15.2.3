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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Utils.About {
	public class ProductInfo {
		bool isBetaVersion = false;
		string title;
		ProductKind[] kind;
		Type versionType;
		ProductInfoStage stage = ProductInfoStage.Trial;
		ProductStringInfo stringInfo = null;
		public ProductInfo(string title, Type versionType) :
			this(title, versionType, ProductKind.Default) { }
		public ProductInfo(string title, Type versionType, ProductKind[] kind, ProductInfoStage stage) { 
			this.stage = stage;
			this.title = title;
			this.kind = kind ;
			this.versionType = versionType;
			this.stringInfo = null;
		}
		public ProductInfo(string title, Type versionType, ProductKind kind, ProductInfoStage stage) : this(title, versionType, new ProductKind[] { kind }, stage) { }
		public ProductInfo(ProductKind kind, ProductStringInfo info) : this(new ProductKind[] { kind }, info) { }
		public ProductInfo(ProductKind[] kind, ProductStringInfo info) {
			this.stage = ProductInfoStage.Registered;
			this.kind = kind;
			this.stringInfo = info;
		}
		public ProductInfo(string title, Type versionType, params ProductKind[] kind) : this(title, versionType, kind, ProductInfoStage.Trial) {  }
		public bool IsBetaVersion { get { return isBetaVersion; } }
		public string Title { get { return title; } }
		public ProductKind Kind { get { return kind == null ? ProductKind.Default : kind[0]; } }
		public ProductKind[] Kinds { 
			get { return kind; } 
			set { 
				kind = value;
				this.stringInfo = null;
			} 
		}
		public Type VersionType { get { return versionType; } }
		public ProductInfoStage Stage { get { return stage; } }
		public ProductStringInfo StringInfo { 
			get {
				if(stringInfo == null) {
					return ProductInfoHelper.GetProductInfo(Kind);
				}
				return stringInfo; 
			} 
			set { stringInfo = value; } }
	}
	public class AboutHelper {
		const bool forceExit = true;
		public const string FreeVersion = "-- FREE VERSION --";
		public const string TrialVersion = "-- TRIAL VERSION --";
		public const string CustomVersion = "-- RECOMPILED VERSION --";
		public static string CopyRight { get { return string.Format(CopyRightTemplate, DateTime.Now.Year.ToString()); } }
		public static string CopyRightOverview { get { return string.Format(CopyRightOverviewTemplate, DateTime.Now.Year.ToString()); } }
		public static Bitmap EmptyImage = new Bitmap(1, 1);
		const string CopyRightTemplate = "Copyright © 2000-{0} Developer Express Inc.\r\nALL  RIGHTS  RESERVED.";
		const string CopyRightOverviewTemplate = "Copyright © 2000-{0} Developer Express Inc.<br><size=-1>All trademarks or registered trademarks are property of their respective owners.";
		public static string GetSerial(ProductInfo pInfo) {
			string serial = CustomVersion;
			return serial;
		}
		public static void ShowStatic(ProductKind kind, ProductStringInfo info) { ShowStatic(new ProductInfo(kind, info)); }
		public static void ShowStatic(ProductInfo info) {
			if(!AllowStaticAbout) return;
		}
		protected static bool AllowStaticAbout {
			get {
				return false;
			}
		}
		public static void Show(ProductKind kind, ProductStringInfo info) {
			Show(new ProductInfo(kind, info));
		}
		public static void Show(ProductInfo info, bool useStaticAbout) {
			if(useStaticAbout)
				ShowStatic(info);
			else
				Show(info);
		}
		static int aboutOpen = 0;
		public static bool IsAboutOpen { 
			get { return aboutOpen > 0; } 
		}
		public static void Show(ProductInfo info) {
			using(AboutForm12 form = new AboutForm12(info)) {
				aboutOpen++;
				form.ShowDialog();
#if !DEBUG
				if(form.IsExpired) {
					DoExit();
				}
#endif
				aboutOpen--;
			}
		}
		public static void DoExit() {
		}
		static void timer_Tick(object sender, EventArgs e) {
			Application.Exit();
		}
	}
}
