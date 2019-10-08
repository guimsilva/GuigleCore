using System.Collections.Generic;

namespace GuigleApi.Models.Address
{
    public interface IAddressComponent
    {
        string LongName { get; set; }

        string ShortName { get; set; }

        List<AddressType> Types { get; set; }

        List<string> StringTypes { get; set; }
    }
}
