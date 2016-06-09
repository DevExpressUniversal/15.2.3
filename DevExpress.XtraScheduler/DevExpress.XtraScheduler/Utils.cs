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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
using System.CodeDom.Compiler;
using DevExpress.Data.Helpers;
using System.Security;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Native {
	internal static class ExceptionMessages {
		internal const string SRWrongMaxValueArgument = "Value of '{0}' is not valid for '{1}'. '{1}' must be greater than or equal to {2}.";
		internal const string SRWrongMinValueArgument = "Value of '{0}' is not valid for '{1}'. '{1}' must be less than {2}.";
	}
	#region SchedulerWinUtils
	public static class SchedulerWinUtils {
		public static bool IsHorizontalImageAlign(HeaderImageAlign imageAlign) {
			return imageAlign == HeaderImageAlign.Left || imageAlign == HeaderImageAlign.Right;
		}
		public static bool IsVerticalImageAlign(HeaderImageAlign imageAlign) {
			return imageAlign == HeaderImageAlign.Top || imageAlign == HeaderImageAlign.Bottom;
		}
		public static bool IsWordWrap(AppearanceObject appearance) {
			return appearance != null ? appearance.TextOptions.WordWrap == WordWrap.Wrap : false;
		}
		public static Size FitSizeForVerticalText(Size size) {
			size.Width += 4;
			size.Height += 3;
			return size;
		}
		public static AppointmentViewInfo FindAppointmentViewInfoByAppointment(IEnumerable<AppointmentViewInfo> viewInfos, Appointment appointment) {
			foreach (AppointmentViewInfo vi in viewInfos) {
				if (vi.Appointment == appointment)
					return vi;
			}
			return null;
		}
		public static AppointmentViewInfo FindAppointmentViewInfoByAppointment(IEnumerable<AppointmentViewInfo> viewInfos, Appointment appointment, Rectangle bounds) {
			foreach (AppointmentViewInfo vi in viewInfos) {
				if (vi.Appointment != appointment)
					continue;
				Rectangle intersectionBounds = Rectangle.Intersect(vi.Bounds, bounds);
				if (intersectionBounds != Rectangle.Empty)
					return vi;
			}
			return null;
		}
		public static AppointmentViewInfoCollection FilterAppointmentViewInfoByContainer(AppointmentViewInfoCollection viewInfos, SchedulerViewCellContainer container) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			for (int i = 0; i < viewInfos.Count; i++) {
				if (viewInfos[i].ScrollContainer == container)
					result.Add(viewInfos[i]);
			}
			return result;
		}
	}
	#endregion
	[SecuritySafeCritical]
	public sealed class PaperSizeConverter {
		static readonly string[] unitsOfLengthNames = { "\"", "in", "pt", "mm", "cm", "m" };
		const string metricUnitName = "cm";
		const string USUnitName = "\"";
		const string formatString = "{0:F} {1}";
		static readonly Hashtable unitsOfLengthFactors = new Hashtable();
		const int LOCALE_IMEASURE = 0x0000000D;
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode), SecuritySafeCritical]
		static extern int GetLocaleInfo(int Locale, int LCType,
			[In, MarshalAs(UnmanagedType.LPWStr)] string lpLCData, int cchData);
		static PaperSizeConverter() {
			unitsOfLengthFactors[unitsOfLengthNames[0]] = (double)100;
			unitsOfLengthFactors[unitsOfLengthNames[1]] = (double)100;
			unitsOfLengthFactors[unitsOfLengthNames[2]] = (double)100 / 72;
			unitsOfLengthFactors[unitsOfLengthNames[3]] = (double)1000 / 254;
			unitsOfLengthFactors[unitsOfLengthNames[4]] = (double)10000 / 254;
			unitsOfLengthFactors[unitsOfLengthNames[5]] = (double)100000 / 254;
		}
		PaperSizeConverter() {
		}
		static bool IsMetric {
			get {
				string result = new string(' ', 2);
				GetLocaleInfo(CultureInfo.CurrentCulture.LCID, LOCALE_IMEASURE, result, 2);
				return result[0] == '0';
			}
		}
		public static string ToString(int hundredthsOfInch) {
			return ToString(hundredthsOfInch, IsMetric);
		}
		public static string ToString(int hundredthsOfInch, bool isMetric) {
			string measureName = isMetric ? metricUnitName : USUnitName;
			double factor = (double)unitsOfLengthFactors[measureName];
			return String.Format(formatString, hundredthsOfInch / factor, measureName);
		}
		public static int FromString(string src) {
			src = src.Trim().ToLower(CultureInfo.CurrentCulture);
			int count = unitsOfLengthNames.Length;
			int srcLen = src.Length;
			for (int i = 0; i < count; i++) {
				if (src.EndsWith(unitsOfLengthNames[i]) == false)
					continue;
				string val = src.Substring(0, srcLen - unitsOfLengthNames[i].Length);
				try {
					double result = Double.Parse(val);
					result *= (double)unitsOfLengthFactors[unitsOfLengthNames[i]];
					return (int)(result + 0.5);
				} catch (FormatException) {
					continue;
				} catch (OverflowException) {
					break;
				}
			}
			try {
				src = src.Trim();
				double result = Double.Parse(src);
				if (IsMetric)
					result = result * 100.0 / 2.54;
				else
					result = result * 100.0;
				return (int)(result + 0.5);
			} catch (FormatException) {
				return -1;
			} catch (OverflowException) {
				return -1;
			}
		}
	}
	public class TransformMatrix {
		int m11 = 1, m12, m13;
		int m21, m22 = 1, m23;
		int m31, m32, m33 = 1;
		TransformMatrix MulLeft(TransformMatrix m) {
			TransformMatrix result = new TransformMatrix();
			result.m11 = m.m11 * m11 + m.m12 * m21 + m.m13 * m31;
			result.m12 = m.m11 * m12 + m.m12 * m22 + m.m13 * m32;
			result.m13 = m.m11 * m13 + m.m12 * m23 + m.m13 * m33;
			result.m21 = m.m21 * m11 + m.m22 * m21 + m.m23 * m31;
			result.m22 = m.m21 * m12 + m.m22 * m22 + m.m23 * m32;
			result.m23 = m.m21 * m13 + m.m22 * m23 + m.m23 * m33;
			result.m31 = m.m31 * m11 + m.m32 * m21 + m.m33 * m31;
			result.m32 = m.m31 * m12 + m.m32 * m22 + m.m33 * m32;
			result.m33 = m.m31 * m13 + m.m32 * m23 + m.m33 * m33;
			return result;
		}
		public TransformMatrix Translate(int dx, int dy) {
			TransformMatrix m = new TransformMatrix();
			m.m13 = dx;
			m.m23 = dy;
			return MulLeft(m);
		}
		public TransformMatrix RotateCW90() {
			TransformMatrix m = new TransformMatrix();
			m.m11 = 0;
			m.m12 = -1;
			m.m21 = 1;
			m.m22 = 0;
			return MulLeft(m);
		}
		public TransformMatrix Scale(int scaleX, int scaleY) {
			TransformMatrix m = new TransformMatrix();
			m.m11 = scaleX;
			m.m22 = scaleY;
			return MulLeft(m);
		}
		Point Apply(Point pt) {
			int x = m11 * pt.X + m12 * pt.Y + m13;
			int y = m21 * pt.X + m22 * pt.Y + m23;
			return new Point(x, y);
		}
		public Rectangle Apply(Rectangle r) {
			Point leftTop = Apply(r.Location);
			Point rightTop = Apply(new Point(r.Right, r.Top));
			Point leftBottom = Apply(new Point(r.Left, r.Bottom));
			Point rightBottom = Apply(new Point(r.Right, r.Bottom));
			int minX = Math.Min(leftTop.X, Math.Min(rightTop.X, Math.Min(leftBottom.X, rightBottom.X)));
			int maxX = Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X)));
			int minY = Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y)));
			int maxY = Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y)));
			return new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	public static class SchedulerIconNames {
		public const string ResourceImagePath = "DevExpress.XtraScheduler.Images";
		public const string Appointment = ResourceImagePath + "." + SchedulerCommandImagesNames.Appointment + ".ico";
		public const string RecurringAppointment = ResourceImagePath + "." + SchedulerCommandImagesNames.RecurringAppointment + ".ico";
		public const string GoToDate = ResourceImagePath + "." + SchedulerCommandImagesNames.GoToDate + ".ico";
		public const string Reminder = ResourceImagePath + "." + SchedulerCommandImagesNames.Reminder + ".ico";
		public const string Delete = ResourceImagePath + "." + SchedulerCommandImagesNames.Delete + ".ico";
	}
#if DEBUGTEST
	public static class DebugConfig {
		static DebugConfig() {
			SchedulerLogger.UsageMessageType = WinLoggerTraceLevel.Warning | WinLoggerTraceLevel.GanttViewLayout;
		}
		internal static void Init() {
		}
	}
#endif
	[GeneratedCode("Suppress FxCop check", "")]
	public class WinLoggerTraceLevel : LoggerTraceLevel {
		public const int GanttViewLayout = 0x8;
	}
	public static class RemoteDesktopDetector {
		public static bool IsRemoteSession {
			get {
				if (SecurityHelper.IsPartialTrust)
					return false;
				return DetectRemoteSession();
				;
			}
		}
		static bool DetectRemoteSession() {
			bool result = false;
			try {
				result = System.Windows.Forms.SystemInformation.TerminalServerSession;
			} catch {
				result = false;
			}
			return result;
		}
	}
#if DEBUGTEST
	public static class DebuggerHelper {
		internal static void PrintBounds(Rectangle bounds, string name) {
			System.Diagnostics.Debug.WriteLine(String.Format("{0} =  new Rectangle({1}, {2}, {3}, {4})", name, bounds.X, bounds.Y, bounds.Width, bounds.Height));
		}
	}
	public static class DebuggerGantHelper {
		internal static void PrintVisibleAppointment(IGanttAppointmentViewInfo viewInfo) {
			Rectangle bounds = viewInfo.Bounds;
			System.Diagnostics.Debug.WriteLine(String.Format("id={0}", viewInfo.Appointment.Subject));
			System.Diagnostics.Debug.WriteLine(String.Format("    new Rectangle({0}, {1}, {2}, {3})", bounds.X, bounds.Y, bounds.Width, bounds.Height));
		}
		internal static void CheckPath(PathItemCollection path, IntersectionObjectsInfo IntersectionObjects) {
			if (path.Count > 1) {
				if (path.Count > 0) {
					System.Diagnostics.Debug.WriteLine("");
					for (int i = 1; i < path.Count; i++) {
						if (path[i].Start != path[i - 1].End) {
							IntersectionObjects.VisibleAppointments.ForEach(PrintVisibleAppointment);
							break;
						}
					}
				}
			}
		}
	}
#endif
}
namespace DevExpress.XtraScheduler.Design {
	public static class SchedulerRepositoryItemsRegistrator {
		public static void Register() {
			RepositoryItemAppointmentLabel.RegisterAppointmentLabelEdit();
			RepositoryItemAppointmentResource.RegisterAppointmentResourceEdit();
			RepositoryItemAppointmentStatus.RegisterAppointmentStatusEdit();
			RepositoryItemDayOfWeek.RegisterWeekDaysEdit();
			RepositoryItemDuration.RegisterDurationEdit();
			RepositoryItemMonth.RegisterMonthEdit();
			RepositoryItemResourcesComboBox.RegisterResourcesComboBoxControl();
			RepositoryItemWeekOfMonth.RegisterWeekOfMonthEdit();
		}
	}
}
