namespace API.Models.Attributes;

public interface ISociable
{
    Dictionary<string, Social> Socials { get; }

    /// <summary>
    /// Set the social links on the model
    /// </summary>
    /// <param name="socials">The social links to set on the model</param>
    void SetSocials(Dictionary<string, Social> socials);

    /// <summary>
    /// Add a social link to the model
    /// </summary>
    /// <param name="social">The social link to add to the model</param>
    /// <returns>Whether or not the account was added to the model</returns>
    bool AddSocial(Social social);

    /// <summary>
    /// Remove a social link from the model
    /// </summary>
    /// <param name="id">The ID of the link to be removed</param>
    /// <returns>Whether or not the account was removed from the model</returns>
    bool RemoveSocial(string id);
}
