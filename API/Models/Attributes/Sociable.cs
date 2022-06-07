namespace API.Models.Attributes;

public class Sociable : ISociable
{
    public Dictionary<string, Social> Socials { get; private set; } = new();

    public void SetSocials(Dictionary<string, Social> socials)
    {
        foreach (KeyValuePair<string, Social> kvp in socials)
            if (!kvp.Key.Equals(kvp.Value.Id))
                throw new ArgumentException($"Invalid {nameof(socials)} provided");

        Socials = socials;
    }

    public bool AddSocial(Social social)
    {
        if (Socials.ContainsKey(social.Id))
            return false;

        Socials.Add(social.Id, social);
        return true;
    }

    public bool RemoveSocial(string id)
    {
        if (!Socials.ContainsKey(id))
            return false;

        Socials.Remove(id);
        return true;
    }
}
