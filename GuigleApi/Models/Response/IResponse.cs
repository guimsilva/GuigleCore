using System.Collections.Generic;

namespace GuigleApi.Models.Response
{
    public interface IResponse<T>
    {
        List<T> Candidates { get; set; }
        List<T> Results { get; set; }
        T Result { get; set; }
        List<string> HtmlAttributions { get; set; }
        string NextPageToken { get; set; }
        string Status { get; set; }
        string ErrorMessage { get; set; }
    }
}
