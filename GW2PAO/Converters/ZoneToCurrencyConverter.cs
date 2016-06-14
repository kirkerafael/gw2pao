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
	public class ZoneToCurrencyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			int mapId = (int)value;

			if (targetType == typeof(ImageSource))
			{
				switch (mapId)
				{
					case MapId.VerdantBrink: return @"/Images/Wallet/Airship_Part.png";
					case MapId.AuricBasin: return @"/Images/Wallet/Lump_of_Aurillium.png";
					case MapId.TangledDepths: return @"/Images/Wallet/Ley_Line_Crystal.png";
					case MapId.DragonsStand: return string.Empty;
					case MapId.DryTop: return @"/Images/Wallet/Geode.png";
					default: return string.Empty;
				}
				//return @"/Images/Misc/treasure_15px.png";
			} //else return @"/Images/Misc/treasure_gray_15px.png";
			else
			{
				return mapId;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}