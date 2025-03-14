using Serilog;

namespace Plutus.Infrastructure.Services;

public static class ObligorIndexer
{
    private static readonly Dictionary<string, string> Patterns = new()
        {
            {"omv", "OMV"},
            {"glovo", "Glovo"},
            {"netflix", "Netflix"},
            {"apple", "Apple"},
            {"payu*tazz.ro", "Tazz"},
            {"rompetrol", "Rompetrol"},
            {"megaimage", "Mega Image"},
            {"profi", "Profi"},
            {"sbx", "Starbucks"},
            {"bolt", "Bolt"},
            {"pago*vodafone", "Vodafone"},
            {"pago*e.on", "e-on"},
            {"spotify", "Spotify"},
            {"CAMELIA VIERIU", "Asociatie Tudor Neculai"},
            {"MARIN MOLDOVANU", "Parcare Tudor Neculai"},
            {"selgros", "Selgros"},
            {"cinema city", "Cinema City"},
            {"cinemacity", "Cinema City"},
            {"Mol ", "MOL"},
            {"Kaufland ", "Kaufland"},
            {"Ia Bilet ", "Ia Bilet"},
            {"Digi_romania_sa", "Digi"},
            {"Lukoil", "Lukoil"},
            {"Altex", "Altex"},
            {"Lidl", "Lidl"},
            {"Digi Romania Sa", "Digi"},
            {"Payu*vodafone.ro", "Vodafone"},
            {"Payu*emag.ro", "emag"},
            {"zooplus", "Zooplus"},
            {"steamgames", "Steam"},
            {"Rcs And Rds", "Digi"},
            {"Penny", "Penny"},
            {"Payu*altex.ro", "Altex"},
            {"Payu*emag.ro/marketplace", "emag"},
            {"Payu*iabilet.ro", "Ia Bilet"},
        };

    public static string GetDisplayName(string name)
    {
        var displayName = name;
        foreach (var pattern in Patterns)
        {
            if (displayName.ToLower().StartsWith(pattern.Key.ToLower()))
            {
                Log.Information("Matched pattern {Pattern} for {Name}", pattern.Key, name);
                displayName = pattern.Value;
            }
        }

        if (displayName.StartsWith("Payu*"))
        {
            displayName = displayName.Replace("Payu*", "");
        }
        ;

        return displayName;
    }
}
