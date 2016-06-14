using GW2PAO.API.Data.Enums;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace GW2PAO.Converters
{
	/// <summary>
	/// Converts a CycleSeverity to various different types, including Visibility, Color, Brush, and double (for opacity)
	/// (1-way conversion)
	/// </summary>
	public class CycleSeverityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			CycleSeverity state = (CycleSeverity)value;

			if (targetType == typeof(ImageSource))
			{
				switch (state)
				{
					case CycleSeverity.Boss: return @"/Images/Misc/red_boss.png";
					case CycleSeverity.BossWarmup: return @"/Images/Misc/boss.png";
					case CycleSeverity.Buildup: return @"/Images/Misc/buildup.png";
					case CycleSeverity.None:
					case CycleSeverity.Unknown:
					default: return string.Empty;
				}
				//return @"/Images/Misc/treasure_15px.png";
			} //else return @"/Images/Misc/treasure_gray_15px.png";

			//         if (targetType == typeof(Visibility))
			//         {
			//             switch (state)
			//             {
			//                 case EventState.Active:
			//                 case EventState.Preparation:
			//                 case EventState.Warmup:
			//                     return Visibility.Visible;
			//                 case EventState.Fail:
			//                 case EventState.Inactive:
			//                 case EventState.Success:
			//                 case EventState.Unknown:
			//                 default:
			//                     return Visibility.Collapsed;
			//             }
			//         }
			//         else
			//if (targetType == typeof(System.Windows.Media.Color))
			//         {
			//             switch (state)
			//             {
			//                 case EventState.Active:
			//                     return System.Windows.Media.Colors.LawnGreen;
			//                 case EventState.Preparation:
			//                 case EventState.Warmup:
			//                     return System.Windows.Media.Colors.Goldenrod;
			//                 case EventState.Fail:
			//                 case EventState.Inactive:
			//                 case EventState.Success:
			//                 case EventState.Unknown:
			//                 default:
			//                     return System.Windows.Media.Colors.White;
			//             }
			//         }
			//         else
			//if (targetType == typeof(System.Windows.Media.Brush)
			//                 || targetType == typeof(System.Windows.Media.SolidColorBrush))
			//         {
			//             switch (state)
			//             {
			//                 case EventState.Active:
			//                     return System.Windows.Media.Brushes.LawnGreen;
			//                 case EventState.Preparation:
			//                 case EventState.Warmup:
			//                     return System.Windows.Media.Brushes.Yellow;
			//                 case EventState.Fail:
			//                 case EventState.Inactive:
			//                 case EventState.Success:
			//                 case EventState.Unknown:
			//                 default:
			//                     return System.Windows.Media.Brushes.White;
			//             }
			//         }
			//         else
			//if (targetType == typeof(double))
			//         {
			//             switch (state)
			//             {
			//                 case EventState.Active:
			//                     return 1.0;
			//                 case EventState.Preparation:
			//                 case EventState.Warmup:
			//                     return 0.75;
			//                 case EventState.Fail:
			//                 case EventState.Inactive:
			//                 case EventState.Success:
			//                 case EventState.Unknown:
			//                 default:
			//                     return 0.35;
			//             }
			//         }
			else
			{
				return state;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}