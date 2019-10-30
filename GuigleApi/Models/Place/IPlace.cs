using System.Collections.Generic;
using GuigleApi.Models.Address;

namespace GuigleApi.Models.Place
{
    public interface IPlace
    {
        string PlaceId { get; set; }
        string Icon { get; set; }
        Geometry Geometry { get; set; }
        string Name { get; set; }
        OpeningHours OpeningHours { get; set; }
        List<Photo> Photos { get; set; }
        string Scope { get; set; }
        Dictionary<string, string> AltIds { get; set; }
        int PriceLevel { get; set; }
        double Rating { get; set; }
        double UserRatingsTotal { get; set; }
        string Reference { get; set; }
        List<PlaceType> Types { get; set; }
        List<string> StringTypes { get; set; }
        string Vicinity { get; set; }
        string FormattedAddress { get; set; }
        bool PermanentlyClosed { get; set; }
        List<Address.Address> Addresses { get; set; }
    }
}
