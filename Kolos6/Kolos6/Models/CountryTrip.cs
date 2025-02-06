using System;
using System.Collections.Generic;

namespace Kolos6.Models;

public partial class CountryTrip
{
    public int IdCountry { get; set; }

    public int IdTrip { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Country IdCountryNavigation { get; set; } = null!;

    public virtual Trip IdTripNavigation { get; set; } = null!;
}
