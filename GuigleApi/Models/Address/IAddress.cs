using System.Collections.Generic;

namespace GuigleApi.Models.Address
{
    public interface IAddress
    {
        string PlaceId { get; set; }
        List<AddressComponent> AddressComponents { get; set; }
        List<AddressComponentT> AddressComponentsT { get; set; }
        List<AddressComponentS> AddressComponentsS { get; set; }
        string FormattedAddress { get; set; }
        Geometry Geometry { get; set; }
        List<AddressType> Types { get; set; }
        List<string> StringTypes { get; set; }
    }
}
